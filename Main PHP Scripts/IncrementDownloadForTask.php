<?php
// IncrementDownloadForTaskDPHX.php
// DPHX Unpack & Load tool: increment Tasks.TotDownloads and log to TaskDownloads.
// Ensures one entry per IP per day in TaskDownloads.

require __DIR__ . '/CommonFunctions.php';

try {
    // Open the database connection
    $pdo = new PDO("sqlite:$databasePath");
    $pdo->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);

    // Validate EntrySeqID parameter
    $entrySeqID = filter_input(INPUT_GET, 'EntrySeqID', FILTER_VALIDATE_INT);
    if ($entrySeqID === null || $entrySeqID === false) {
        echo json_encode([
            'status'  => 'error',
            'message' => 'Missing or invalid EntrySeqID parameter'
        ]);
        exit;
    }

    // Get current UTC times
    $now = new DateTime("now", new DateTimeZone("UTC"));
    $nowFormatted = $now->format('Y-m-d H:i:s');    // full timestamp
    $dateOnly     = $now->format('Y-m-d');         // date only

    // Get client IP
    $ipSource = getClientIP();

    // Begin transaction for atomic update + log
    $pdo->beginTransaction();

    // 1) Increment the Tasks table
    $updateSql = "
        UPDATE Tasks
           SET TotDownloads       = TotDownloads + 1,
               LastDownloadUpdate = :lastDownloadUpdate
         WHERE EntrySeqID        = :entrySeqID
    ";
    $stmt = $pdo->prepare($updateSql);
    $stmt->bindParam(':lastDownloadUpdate', $nowFormatted, PDO::PARAM_STR);
    $stmt->bindParam(':entrySeqID',         $entrySeqID,   PDO::PARAM_INT);
    $stmt->execute();

    // 2) Log to TaskDownloads if not already logged today for this IP & task
    $checkSql = "
        SELECT 1
          FROM TaskDownloads
         WHERE IPSource   = :ipSource
           AND Date       = :dateOnly
           AND EntrySeqID = :entrySeqID
         LIMIT 1
    ";
    $stmt = $pdo->prepare($checkSql);
    $stmt->execute([
        ':ipSource'   => $ipSource,
        ':dateOnly'   => $dateOnly,
        ':entrySeqID' => $entrySeqID
    ]);

    if (!$stmt->fetch()) {
        $insertSql = "
            INSERT INTO TaskDownloads (IPSource, Date, EntrySeqID)
            VALUES (:ipSource, :dateOnly, :entrySeqID)
        ";
        $ins = $pdo->prepare($insertSql);
        $ins->execute([
            ':ipSource'   => $ipSource,
            ':dateOnly'   => $dateOnly,
            ':entrySeqID' => $entrySeqID
        ]);
    } else {
    }

    // Commit transaction
    $pdo->commit();

    // 3) Fetch updated TotDownloads
    $selectSql = "
        SELECT TotDownloads, LastDownloadUpdate
          FROM Tasks
         WHERE EntrySeqID = :entrySeqID
    ";
    $stmt = $pdo->prepare($selectSql);
    $stmt->bindParam(':entrySeqID', $entrySeqID, PDO::PARAM_INT);
    $stmt->execute();
    $task = $stmt->fetch(PDO::FETCH_ASSOC);

    if ($task) {
        echo json_encode([
            'status'             => 'success',
            'TotDownloads'       => $task['TotDownloads'],
            'LastDownloadUpdate' => $task['LastDownloadUpdate']
        ]);
    } else {
        echo json_encode([
            'status'  => 'error',
            'message' => "No task found with EntrySeqID = $entrySeqID"
        ]);
    }

} catch (PDOException $e) {
    // Rollback if in transaction
    if ($pdo->inTransaction()) {
        $pdo->rollBack();
    }
    echo json_encode([
        'status'  => 'error',
        'message' => 'Database error: ' . $e->getMessage()
    ]);
} catch (Exception $e) {
    if ($pdo->inTransaction()) {
        $pdo->rollBack();
    }
    echo json_encode([
        'status'  => 'error',
        'message' => 'General error: ' . $e->getMessage()
    ]);
}
?>
