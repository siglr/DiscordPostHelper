<?php
require __DIR__ . '/CommonFunctions.php';

try {
    logMessage("--- Script running FetchTaskDetails ---");

    // Open the database connection
    $pdo = new PDO("sqlite:$databasePath");
    $pdo->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);
    logMessage("Database connection established.");

    // Check if TaskID parameter is set
    if (!isset($_GET['TaskID'])) {
        throw new Exception('TaskID is missing.');
    }

    $taskID = $_GET['TaskID'];

    // Prepare the SQL statement
    $stmt = $pdo->prepare("SELECT EntrySeqID, LastUpdate, DBEntryUpdate FROM Tasks WHERE TaskID = :TaskID");
    $stmt->execute([':TaskID' => $taskID]);
    $taskDetails = $stmt->fetch(PDO::FETCH_ASSOC);

    if ($taskDetails) {
        echo json_encode(['status' => 'success', 'taskDetails' => $taskDetails]);
        logMessage("Task details fetched for TaskID: " . $taskID);
    } else {
        echo json_encode(['status' => 'error', 'message' => 'Task not found.']);
        logMessage("Task not found for TaskID: " . $taskID);
    }

    logMessage("--- End of script FetchTaskDetails ---");

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
