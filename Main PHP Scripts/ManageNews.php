<?php
require __DIR__ . '/CommonFunctions.php';

try {
    logMessage("--- Script running ManageNews ---");

    // Open the database connection
    $pdo = new PDO("sqlite:$newsDBPath");
    $pdo->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);
    logMessage("Database connection established.");

    // Call the cleanup function
    cleanUpNewsEntries($pdo);

    // Ensure the request method is POST
    if ($_SERVER['REQUEST_METHOD'] !== 'POST') {
        throw new Exception('Invalid request method.');
    }

    // Get the action from POST
    if (!isset($_POST['action'])) {
        throw new Exception('Action not specified.');
    }

    $action = $_POST['action'];

    // Handle the different actions
    switch ($action) {
        case 'CreateTask':
        case 'UpdateTask':
            if (!isset($_POST['TaskID'])) {
                throw new Exception('TaskID missing.');
            }
            $taskID = $_POST['TaskID'];
            $key = "T-$taskID";

            // Delete existing Task entry
            $pdo->prepare("DELETE FROM News WHERE NewsType = 0 AND Key = ?")->execute([$key]);
            logMessage("Existing Task entry deleted for TaskID: $taskID.");

            // Insert new Task entry
            $stmt = $pdo->prepare("
                INSERT INTO News (Key, Published, Title, Subtitle, Comments, Credits, EventDate, News, NewsType, TaskID, EntrySeqID, URLToGo, Expiration)
                VALUES (:Key, :Published, :Title, :Subtitle, :Comments, :Credits, NULL, :News, 0, :TaskID, :EntrySeqID, :URLToGo, NULL)
            ");
            $stmt->execute([
                ':Key' => $key,
                ':Published' => formatDatetime($_POST['Published']),
                ':Title' => $_POST['Title'],
                ':Subtitle' => $_POST['Subtitle'],
                ':Comments' => $_POST['Comments'],
                ':Credits' => $_POST['Credits'],
                ':News' => $_POST['News'],
                ':TaskID' => $taskID,
                ':EntrySeqID' => $_POST['EntrySeqID'],
                ':URLToGo' => $_POST['URLToGo']
            ]);
            logMessage("Task entry created for TaskID: $taskID.");
            break;

        case 'DeleteTask':
            if (!isset($_POST['TaskID'])) {
                throw new Exception('TaskID missing.');
            }
            $taskID = $_POST['TaskID'];
            $key = "T-$taskID";

            // Delete existing Task entry
            $pdo->prepare("DELETE FROM News WHERE NewsType = 0 AND Key = ?")->execute([$key]);
            logMessage("Task entry deleted for TaskID: $taskID.");
            break;

        case 'CreateEvent':
            if (!isset($_POST['Key'])) {
                throw new Exception('Key missing.');
            }
            $key = $_POST['Key'];

            // Delete existing Event entry
            $pdo->prepare("DELETE FROM News WHERE NewsType = 1 AND Key = ?")->execute([$key]);
            logMessage("Existing Event entry deleted for Key: $key.");

            // Insert new Event entry
            $stmt = $pdo->prepare("
                INSERT INTO News (Key, Published, Title, Subtitle, Comments, Credits, EventDate, News, NewsType, TaskID, EntrySeqID, URLToGo, Expiration)
                VALUES (:Key, :Published, :Title, :Subtitle, :Comments, :Credits, :EventDate, :News, 1, :TaskID, :EntrySeqID, :URLToGo, :Expiration)
            ");
            $stmt->execute([
                ':Key' => $key,
                ':Published' => formatDatetime($_POST['Published']),
                ':Title' => $_POST['Title'],
                ':Subtitle' => $_POST['Subtitle'],
                ':Comments' => $_POST['Comments'],
                ':Credits' => $_POST['Credits'],
                ':EventDate' => formatDatetime($_POST['EventDate']),
                ':News' => $_POST['News'],
                ':TaskID' => $_POST['TaskID'],
                ':EntrySeqID' => $_POST['EntrySeqID'],
                ':URLToGo' => $_POST['URLToGo'],
                ':Expiration' => formatDatetime($_POST['Expiration'])
            ]);
            logMessage("Event entry created for Key: $key.");
            break;

        case 'DeleteEvent':
            if (!isset($_POST['Key'])) {
                throw new Exception('Key missing.');
            }
            $key = $_POST['Key'];

            // Delete existing Event entry
            $pdo->prepare("DELETE FROM News WHERE NewsType = 1 AND Key = ?")->execute([$key]);
            logMessage("Event entry deleted for Key: $key.");
            break;

        case 'CreateNews':
            if (!isset($_POST['Key'])) {
                throw new Exception('Key missing.');
            }
            $key = $_POST['Key'];

            // Delete existing News entry
            $pdo->prepare("DELETE FROM News WHERE NewsType = 2 AND Key = ?")->execute([$key]);
            logMessage("Existing News entry deleted for Key: $key.");

            // Insert new News entry
            $stmt = $pdo->prepare("
                INSERT INTO News (Key, Published, Title, Subtitle, Comments, Credits, EventDate, News, NewsType, TaskID, EntrySeqID, URLToGo, Expiration)
                VALUES (:Key, :Published, :Title, :Subtitle, :Comments, :Credits, :EventDate, :News, 2, :TaskID, :EntrySeqID, :URLToGo, :Expiration)
            ");
            $stmt->execute([
                ':Key' => $key,
                ':Published' => formatDatetime($_POST['Published']),
                ':Title' => $_POST['Title'],
                ':Subtitle' => $_POST['Subtitle'],
                ':Comments' => $_POST['Comments'],
                ':Credits' => $_POST['Credits'],
                ':EventDate' => formatDatetime($_POST['EventDate']),
                ':News' => $_POST['News'],
                ':TaskID' => $_POST['TaskID'],
                ':EntrySeqID' => $_POST['EntrySeqID'],
                ':URLToGo' => $_POST['URLToGo'],
                ':Expiration' => formatDatetime($_POST['Expiration'])
            ]);
            logMessage("News entry created for Key: $key.");
            break;

        case 'DeleteNews':
            if (!isset($_POST['Key'])) {
                throw new Exception('Key missing.');
            }
            $key = $_POST['Key'];

            // Delete existing News entry
            $pdo->prepare("DELETE FROM News WHERE NewsType = 2 AND Key = ?")->execute([$key]);
            logMessage("News entry deleted for Key: $key.");
            break;

        default:
            throw new Exception('Invalid action specified.');
    }

    echo json_encode(['status' => 'success', 'message' => 'Action processed successfully.']);
    logMessage("--- End of script ManageNews ---");

} catch (Exception $e) {
    logMessage("Error: " . $e->getMessage());
    echo json_encode(['status' => 'error', 'message' => $e->getMessage()]);
    logMessage("--- End of script ManageNews ---");
}
?>
