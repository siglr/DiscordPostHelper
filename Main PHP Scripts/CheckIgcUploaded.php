<?php
require __DIR__ . '/CommonFunctions.php';

try {
    // 1) Grab POST data
    $key         = $_POST['IGCKey']     ?? null;
    $entrySeqID  = $_POST['EntrySeqID'] ?? null;
    $pilotName   = $_POST['PilotName']  ?? null;
    $compID      = $_POST['CompID']     ?? null;

    if (!$key || !$entrySeqID) {
        throw new Exception("Missing required parameters: IGCKey and EntrySeqID.");
    }

    // 2) Open the database
    $pdo = new PDO("sqlite:$databasePath");
    $pdo->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);

    // 3) Case-insensitive check for this key+task
    $stmt = $pdo->prepare("
      SELECT 1
        FROM IGCRecords
       WHERE IGCKey   COLLATE NOCASE = :igcKey
         AND EntrySeqID           = :entrySeq
      LIMIT 1
    ");
    $stmt->execute([
        ':igcKey'   => $key,
        ':entrySeq'=> (int)$entrySeqID
    ]);

    if ($stmt->fetchColumn()) {
        // Already uploaded
        echo json_encode(['status' => 'exists']);
        exit;
    }

    // 4) Not found → try to infer WSGUserID
    $wsgUserID = 0;

    // 4.1) Option #1: exact PilotName + CompID
    if ($pilotName && $compID) {
        $stmt = $pdo->prepare("
          SELECT WSGUserID
            FROM Users
           WHERE PilotName    COLLATE NOCASE = :pilotName
             AND CompID             = :compID
        ");
        $stmt->execute([
            ':pilotName'=> $pilotName,
            ':compID'   => $compID
        ]);
        $rows = $stmt->fetchAll(PDO::FETCH_COLUMN);
        if (count($rows) === 1) {
            $wsgUserID = (int)$rows[0];
        }
    }

    // 4.2) Option #2: PilotName only (if still unknown)
    if ($wsgUserID === 0 && $pilotName) {
        // All distinct users matching PilotName
        $stmt = $pdo->prepare("
          SELECT DISTINCT WSGUserID
            FROM Users
           WHERE PilotName COLLATE NOCASE = :pilotName
        ");
        $stmt->execute([':pilotName'=> $pilotName]);
        $pilotRows = $stmt->fetchAll(PDO::FETCH_COLUMN);

        If (count($pilotRows) === 1) {
            $pilotMatch = (int)$pilotRows[0];
            // If we also have a CompID, check for a conflicting user
            if ($compID) {
                $stmt = $pdo->prepare("
                  SELECT DISTINCT WSGUserID
                    FROM Users
                   WHERE CompID = :compID
                ");
                $stmt->execute([':compID'=> $compID]);
                $compRows = $stmt->fetchAll(PDO::FETCH_COLUMN);

                if (count($compRows) === 1 && (int)$compRows[0] !== $pilotMatch) {
                    // conflict → leave as 0
                } else {
                    $wsgUserID = $pilotMatch;
                }
            } else {
                $wsgUserID = $pilotMatch;
            }
        }
    }

    // 4.3) Option #3: CompID only (if still unknown)
    if ($wsgUserID === 0 && $compID) {
        $stmt = $pdo->prepare("
          SELECT DISTINCT WSGUserID
            FROM Users
           WHERE CompID = :compID
        ");
        $stmt->execute([':compID'=> $compID]);
        $compRows = $stmt->fetchAll(PDO::FETCH_COLUMN);

        If (count($compRows) === 1) {
            $compMatch = (int)$compRows[0];
            // If we also have a PilotName, check for conflict
            if ($pilotName) {
                $stmt = $pdo->prepare("
                  SELECT DISTINCT WSGUserID
                    FROM Users
                   WHERE PilotName COLLATE NOCASE = :pilotName
                ");
                $stmt->execute([':pilotName'=> $pilotName]);
                $pilotRows2 = $stmt->fetchAll(PDO::FETCH_COLUMN);

                if (count($pilotRows2) === 1 && (int)$pilotRows2[0] !== $compMatch) {
                    // conflict → leave as 0
                } else {
                    $wsgUserID = $compMatch;
                }
            } else {
                $wsgUserID = $compMatch;
            }
        }
    }

    // 5) Return JSON
    echo json_encode([
        'status'    => 'not_found',
        'wsgUserID' => $wsgUserID
    ]);
}
catch (Exception $e) {
    echo json_encode([
        'status'  => 'error',
        'message' => $e->getMessage()
    ]);
    exit;
}
?>
