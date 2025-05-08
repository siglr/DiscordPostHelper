<?php
require __DIR__ . '/CommonFunctions.php';

try {
    // 1) Grab POST data
    $key         = $_POST['IGCKey']      ?? null;
    $entrySeqID  = $_POST['EntrySeqID']  ?? null;
    if (!$key || !$entrySeqID) {
        throw new Exception("Missing required parameters: IGCKey and EntrySeqID.");
    }

    // 2) Open the database
    $pdo = new PDO("sqlite:$databasePath");
    $pdo->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);

    // 3) Case-insensitive check for this key+task
    $sql = "
      SELECT 1
        FROM IGCRecords
       WHERE IGCKey   COLLATE NOCASE = :igcKey
         AND EntrySeqID           = :entrySeq
      LIMIT 1
    ";
    $stmt = $pdo->prepare($sql);
    $stmt->execute([
        ':igcKey'   => $key,
        ':entrySeq' => (int)$entrySeqID
    ]);
    $exists = (bool)$stmt->fetchColumn();

    // 4) Return JSON
    echo json_encode([
        'status' => $exists ? 'exists' : 'not_found'
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
