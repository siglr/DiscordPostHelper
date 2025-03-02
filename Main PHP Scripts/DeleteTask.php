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

    // Check if EntrySeqID and UserID are set
    if (!isset($_POST['EntrySeqID']) || !isset($_POST['UserID'])) {
        throw new Exception('EntrySeqID or UserID is missing.');
    }

    $entrySeqID = $_POST['EntrySeqID'];
    $userID = $_POST['UserID'];

    // Check if the user has DeleteTask rights
    if (!checkUserPermission($userID, 'DeleteTask')) {
        throw new Exception('User does not have permission to delete tasks.');
    }

    // Retrieve TaskID and DiscordPostID using EntrySeqID
    $stmt = $pdo->prepare("SELECT TaskID, DiscordPostID FROM Tasks WHERE EntrySeqID = :EntrySeqID");
    $stmt->execute([':EntrySeqID' => $entrySeqID]);
    $result = $stmt->fetch(PDO::FETCH_ASSOC);

    if (!$result) {
        throw new Exception('Task not found.');
    }

    $taskID = $result['TaskID'];
    $discordPostID = $result['DiscordPostID'];

    // Delete the task from the main Tasks table
    $stmt = $pdo->prepare("DELETE FROM Tasks WHERE EntrySeqID = :EntrySeqID");
    if (!$stmt->execute([':EntrySeqID' => $entrySeqID])) {
        throw new Exception('Delete task from Tasks table failed.');
    }

    // Delete the associated news entries
    try {
        deleteTaskNewsEntries($taskID);
    } catch (Exception $ex) {
        $errors[] = "Failed to delete news entries: " . $ex->getMessage();
    }

    // Get the current UTC time
    $now = new DateTime("now", new DateTimeZone("UTC"));
    $nowFormatted = $now->format('Y-m-d H:i:s');

    // Insert the EntrySeqID and DeletionDate into the DeletedTasks table
    $stmt = $pdo->prepare("INSERT INTO DeletedTasks (EntrySeqID, DeletionDate) VALUES (:EntrySeqID, :DeletionDate)");
    if (!$stmt->execute([
        ':EntrySeqID' => $entrySeqID,
        ':DeletionDate' => $nowFormatted
    ])) {
        $errors[] = "Failed to insert deletion record into DeletedTasks.";
    }

    // Delete the associated DPHX file
    $target_dir = $fileRootPath . 'TaskBrowser/Tasks/';
    $target_file = $target_dir . basename($taskID . '.dphx');
    if (file_exists($target_file)) {
        if (!unlink($target_file)) {
            $errors[] = 'Failed to delete the DPHX file.';
        } else {
            logMessage("DPHX file deleted: " . $target_file);
        }
    } else {
        logMessage("DPHX file not found: " . $target_file);
    }

    // Delete the associated Weather chart
    $target_dir = $fileRootPath . 'TaskBrowser/WeatherCharts/';
    $target_file = $target_dir . basename($entrySeqID . '.jpg');
    if (file_exists($target_file)) {
        if (!unlink($target_file)) {
            $errors[] = 'Failed to delete the weather chart file.';
        } else {
            logMessage("Weather chart file deleted: " . $target_file);
        }
    } else {
        logMessage("Weather chart file not found: " . $target_file);
    }

    // Delete the associated cover image - if any
    $target_dir = $fileRootPath . 'TaskBrowser/Covers/';
    $target_file = $target_dir . basename($entrySeqID . '.jpg');
    if (file_exists($target_file)) {
        if (!unlink($target_file)) {
            $errors[] = 'Failed to delete the cover image.';
        } else {
            logMessage("Cover image deleted: " . $target_file);
        }
    } else {
        logMessage("No cover image found: " . $target_file);
    }

    // Delete the associated Discord post, if one exists.
    if (!empty($discordPostID)) {
        $discordResult = manageDiscordPost($disWHFlights, "", $discordPostID, true);
        $discordResponse = json_decode($discordResult, true);
        if ($discordResponse['result'] === "success") {
            logMessage("Deleted Discord post with ID: " . $discordPostID);
        } else {
            $discordError = $discordResponse['error'];
            $errors[] = "Failed to delete Discord post with ID: " . $discordPostID . ". Error: " . $discordError;
            logMessage("Failed to delete Discord post with ID: " . $discordPostID . ". Error: " . $discordError);
        }
    }

    // Prepare output based on accumulated errors.
    if (!empty($errors)) {
        $output = [
            'status'  => 'success',
            'message' => implode(' ', $errors),
            'discordError' => isset($discordError) ? $discordError : ''
        ];
    } else {
        $output = [
            'status'       => 'success',
            'message'      => 'Task, associated news, files, and Discord post deleted successfully.'
        ];
    }
    
    echo json_encode($output);
    logMessage("Deleted task with EntrySeqID: " . $entrySeqID . " by UserID: " . $userID);
    logMessage("--- End of script DeleteTask ---");

} catch (Exception $e) {
    logMessage("Error: " . $e->getMessage());
    echo json_encode(['status' => 'error', 'message' => $e->getMessage()]);
    logMessage("--- End of script DeleteTask ---");
}
?>
