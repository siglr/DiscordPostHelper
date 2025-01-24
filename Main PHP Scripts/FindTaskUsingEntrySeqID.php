<?php
require __DIR__ . '/CommonFunctions.php';

try {

    // Open the database connection
    $pdo = new PDO("sqlite:$databasePath");
    $pdo->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);

    // Check if EntrySeqID parameter is set
    if (!isset($_GET['EntrySeqID'])) {
        throw new Exception('EntrySeqID is missing');
    }

    $entrySeqID = $_GET['EntrySeqID'];

    // Prepare the SQL statement
    $stmt = $pdo->prepare("SELECT TaskID, EntrySeqID, LastUpdate, DBEntryUpdate FROM Tasks WHERE EntrySeqID = :EntrySeqID");
    $stmt->execute([':EntrySeqID' => $entrySeqID]);
    $taskDetails = $stmt->fetch(PDO::FETCH_ASSOC);

    if ($taskDetails) {
        echo json_encode(['status' => 'success', 'taskDetails' => $taskDetails]);
    } else {
        echo json_encode(['status' => 'error', 'message' => 'Task not found.']);
    }

} catch (PDOException $e) {
    logMessage("Connection failed: " . $e->getMessage());
    echo json_encode(['status' => 'error', 'message' => 'Connection failed: ' . $e->getMessage()]);
    logMessage("--- End of script FetchTaskDetails ---");
} catch (Exception $e) {
    logMessage("Error: " . $e->getMessage());
    echo json_encode(['status' => 'error', 'message' => $e->getMessage()]);
    logMessage("--- End of script FetchTaskDetails ---");
}
?>
