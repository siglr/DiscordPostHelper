<?php
require __DIR__ . '/CommonFunctions.php';

try {
    logMessage("--- Script running DeleteTask ---");

    // Open the database connection
    $pdo = new PDO("sqlite:$databasePath");
    $pdo->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);
    logMessage("Database connection established.");

    // Ensure the request method is POST
    if ($_SERVER['REQUEST_METHOD'] !== 'POST') {
        throw new Exception('Invalid request method.');
    }

    // Check if TaskID and UserID are set
    if (!isset($_POST['TaskID']) || !isset($_POST['UserID'])) {
        throw new Exception('TaskID or UserID is missing.');
    }

    $taskID = $_POST['TaskID'];
    $userID = $_POST['UserID'];

    // Check if the user has DeleteTask rights
    if (!checkUserPermission($userID, 'DeleteTask')) {
        throw new Exception('User does not have permission to delete tasks.');
    }

    // Retrieve EntrySeqID using TaskID
    $stmt = $pdo->prepare("SELECT EntrySeqID FROM Tasks WHERE TaskID = :TaskID");
    $stmt->execute([':TaskID' => $taskID]);
    $result = $stmt->fetch(PDO::FETCH_ASSOC);

    if (!$result) {
        throw new Exception('Task not found.');
    }

    $entrySeqID = $result['EntrySeqID'];

    // Delete the task from the main Tasks table
    $stmt = $pdo->prepare("DELETE FROM Tasks WHERE TaskID = :TaskID");
    $stmt->execute([':TaskID' => $taskID]);

    // Delete the associated news entries
    deleteTaskNewsEntries($taskID);

    // Get the current UTC time
    $now = new DateTime("now", new DateTimeZone("UTC"));
    $nowFormatted = $now->format('Y-m-d H:i:s');

    // Insert the EntrySeqID and DeletionDate into the DeletedTasks table
    $stmt = $pdo->prepare("INSERT INTO DeletedTasks (EntrySeqID, DeletionDate) VALUES (:EntrySeqID, :DeletionDate)");
    $stmt->execute([
        ':EntrySeqID' => $entrySeqID,
        ':DeletionDate' => $nowFormatted
    ]);

    // Delete the associated file
    $target_dir = '/home2/siglr3/public_html/DiscordPostHelper/TaskBrowser/Tasks/';
    $target_file = $target_dir . basename($taskID . '.dphx');

    if (file_exists($target_file)) {
        if (!unlink($target_file)) {
            throw new Exception('Failed to delete the associated file.');
        } else {
            logMessage("Associated file deleted: " . $target_file);
        }
    } else {
        logMessage("Associated file not found: " . $target_file);
    }

    logMessage("Deleted task with TaskID: " . $taskID . " by UserID: " . $userID);
    echo json_encode(['status' => 'success', 'message' => 'Task, associated news, and file deleted successfully.']);
    logMessage("--- End of script DeleteTask ---");

} catch (Exception $e) {
    logMessage("Error: " . $e->getMessage());
    echo json_encode(['status' => 'error', 'message' => $e->getMessage()]);
    logMessage("--- End of script DeleteTask ---");
}
?>
