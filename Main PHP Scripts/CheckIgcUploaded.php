<?php
require __DIR__ . '/CommonFunctions.php';

try {
    // 1) Grab POST data
    $key = $_POST['IGCKey']      ?? null;
    $entrySeqID = $_POST['EntrySeqID'] ?? null;
    if (!$key || !$entrySeqID) {
        throw new Exception("Missing required parameters: IGCKey and EntrySeqID.");
    }

    // 2) Open the database
    $pdo = new PDO("sqlite:$databasePath");
    $pdo->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);

    // 3) Check IGCRecords for this key+task
    $sql = "SELECT 1 FROM IGCRecords WHERE IGCKey = :igcKey AND EntrySeqID = :entrySeq";
    $stmt = $pdo->prepare($sql);
    $stmt->execute([
        ':igcKey'    => $key,
        ':entrySeq'  => $entrySeqID
    ]);
    $exists = (bool) $stmt->fetchColumn();

    // 4) Return JSON
    if ($exists) {
        echo json_encode(['status' => 'exists']);
    } else {
        echo json_encode(['status' => 'not_found']);
    }
}
catch (Exception $e) {
    echo json_encode([
        'status'  => 'error',
        'message' => $e->getMessage()
    ]);
    exit;
}
