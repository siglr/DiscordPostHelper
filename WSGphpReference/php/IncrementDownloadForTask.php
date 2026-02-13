<?php
require __DIR__ . '/CommonFunctions.php';

try {
    // Open the database connection
    $pdo = new PDO("sqlite:$databasePath");
    $pdo->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);

    // Retrieve and validate the EntrySeqID parameter as an integer
    $entrySeqID = filter_input(INPUT_GET, 'EntrySeqID', FILTER_VALIDATE_INT);
    if ($entrySeqID === null || $entrySeqID === false) {
        logMessage("--- Script running IncrementDownloadForTask ---");
        echo json_encode([
            'status'  => 'error',
            'message' => 'Missing or invalid EntrySeqID parameter'
        ]);
        logMessage("--- End of script IncrementDownloadForTask ---");
        exit;
    }

    recordUniqueTaskDownload($pdo, $entrySeqID);

    // 3) Retrieve and return the updated TotDownloads
    $selectQuery = "
        SELECT TotDownloads, LastDownloadUpdate
          FROM Tasks
         WHERE EntrySeqID = :id
    ";
    $stmt = $pdo->prepare($selectQuery);
    $stmt->bindParam(':id', $entrySeqID, PDO::PARAM_INT);
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
    // Roll back on error
    logMessage("--- Script running IncrementDownloadForTask ---");
    echo json_encode([
        'status'  => 'error',
        'message' => 'Connection failed: ' . $e->getMessage()
    ]);
    logMessage("--- End of script IncrementDownloadForTask ---");
}
?>
