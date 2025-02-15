<?php
// Include the configuration file
$config = include 'config.php';

// Assign paths from the configuration array
$databasePath = $config['databasePath'];
$newsDBPath = $config['newsDBPath'];

// Dynamically insert the date into the log file name
$logFile = preg_replace(
    '/(error_log)(\.txt)$/i',
    '${1}_' . date('Ymd') . '${2}',
    $config['logFile']
);

$userPermissionsPath = $config['userPermissionsPath'];
$fileRootPath = $config['fileRootPath'];

// Function to log messages
function logMessage($message) {
    $clientIp = $_SERVER['REMOTE_ADDR'];
    global $logFile;
    $timestamp = date('Y-m-d H:i:s');
    file_put_contents($logFile, "[$timestamp] $clientIp $message\n", FILE_APPEND);
}

// Function to ensure datetime fields are properly formatted
function formatDatetime($datetime) {
    $dt = new DateTime($datetime);
    return $dt->format('Y-m-d H:i:s');
}

// Function to get user permissions
function getUserPermissions($userID) {
    global $userPermissionsPath;
    if (!file_exists($userPermissionsPath)) {
        logMessage("User permissions file not found: $userPermissionsPath");
        return null;
    }
    $xml = simplexml_load_file($userPermissionsPath);
    if ($xml === false) {
        logMessage("Failed to load user permissions file: $userPermissionsPath");
        return null;
    }
    foreach ($xml->User as $user) {
        if ((string)$user->ID === $userID) {
            return $user;
        }
    }
    return null;
}

// Function to retrieve user rights
function getUserRights($userPermissions) {
    if ($userPermissions === null) {
        return null;
    }
    $rights = [];
    foreach ($userPermissions->children() as $key => $value) {
        if ($key !== 'ID' && $key !== 'Name') {
            $rights[$key] = (string)$value;
        }
    }
    return $rights;
}

// Function to check user right
function checkUserRight($userPermissions, $right) {
    if ($userPermissions === null) {
        return false;
    }
    return (string)$userPermissions->$right === 'True';
}

// Function to check user permissions directly
function checkUserPermission($userID, $permission) {

    // Get user permissions
    $userPermissions = getUserPermissions($userID);
    if ($userPermissions === null) {
        logMessage("No permissions found for userID: $userID");
        return false;
    }

    // Check the specified permission
    $hasRight = checkUserRight($userPermissions, $permission);
    logMessage("Permission check result for userID: $userID, Permission: $permission, HasRight: " . ($hasRight ? 'True' : 'False'));

    return $hasRight;
}
function cleanUpNewsEntries($pdo) {
    // Get the current UTC datetime
    $currentDatetime = $pdo->query("SELECT datetime('now')")->fetchColumn();

    // Get the datetime for 7 days ago
    $datetimeMinus7Days = $pdo->query("SELECT datetime('now', '-7 days')")->fetchColumn();

    // Cleanup expired News and Event entries
    // Step 1: Fetch Keys for expired News entries of NewsType 1 and 2
    $expiredKeys = $pdo->query("SELECT DISTINCT Key FROM News WHERE NewsType IN (1, 2) AND Expiration < datetime('now')")->fetchAll(PDO::FETCH_COLUMN);

    if (!empty($expiredKeys)) {
        // Step 2: Delete corresponding entries in the Events table
        $placeholders = implode(',', array_fill(0, count($expiredKeys), '?'));
        $deleteEventsStmt = $pdo->prepare("DELETE FROM Events WHERE EventKey IN ($placeholders)");
        $deleteEventsStmt->execute($expiredKeys);
    }

    // Step 3: Delete the expired News entries from the News table
    $pdo->exec("DELETE FROM News WHERE NewsType IN (1, 2) AND Expiration < datetime('now')");

    // Cleanup old Task entries (NewsType 0) older than 7 days
    $pdo->exec("DELETE FROM News WHERE NewsType = 0 AND Published < datetime('now', '-7 days')");
}
function cleanUpPendingTasks($pdo) {
    try {
        logMessage("Cleaning up old pending tasks of more than 5 days");
        // Get the datetime for 5 days ago
        $datetimeMinus5Days = $pdo->query("SELECT datetime('now', '-5 days')")->fetchColumn();

        // Delete tasks with Status = 10 and LastUpdate older than 5 days
        $deleteStmt = $pdo->prepare("
            DELETE FROM Tasks
            WHERE Status = 10
            AND LastUpdate < :datetimeMinus5Days
        ");
        $deleteStmt->execute([':datetimeMinus5Days' => $datetimeMinus5Days]);

        // Get the count of rows affected
        $rowsDeleted = $deleteStmt->rowCount();
        if ($rowsDeleted > 0) {
            logMessage("Deleted $rowsDeleted pending tasks with Status = 10 and LastUpdate < 5 days ago.");
        }

    } catch (Exception $e) {
        logMessage("Error during clean-up of pending tasks: " . $e->getMessage());
    }
}
// Function to create or update a task news entry
function createOrUpdateTaskNewsEntry($taskData, $isUpdate) {
    global $newsDBPath;

    try {
        // Open the news database connection
        $newsPdo = new PDO("sqlite:$newsDBPath");
        $newsPdo->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);

        cleanUpNewsEntries($newsPdo);

        $action = $isUpdate ? 'UpdateTask' : 'CreateTask';
        $title = $isUpdate ? "Updated task #" . $taskData['EntrySeqID'] : "New task #" . $taskData['EntrySeqID'];

        // Handle comments
        if ($isUpdate && !empty($taskData['LastUpdateDescription'])) {
            $comments = $taskData['LastUpdateDescription'];
        } else {
            $comments = !empty($taskData['MainAreaPOI']) ? $taskData['MainAreaPOI'] : $taskData['ShortDescription'];
        }
        
        // Process credits
        $credits = str_replace("All credits to ", "By ", preg_replace("/ for this task.*/", "", $taskData['Credits']));

        // Prepare the delete statement
        $deleteStmt = $newsPdo->prepare("DELETE FROM News WHERE NewsType = 0 AND Key = ?");
        $deleteStmt->execute(["T-{$taskData['TaskID']}"]);

        // Prepare the insert statement
        $stmt = $newsPdo->prepare("
            INSERT INTO News (Key, Published, Title, Subtitle, Comments, Credits, EventDate, News, NewsType, TaskID, EntrySeqID, URLToGo, Expiration)
            VALUES (:Key, :Published, :Title, :Subtitle, :Comments, :Credits, :EventDate, :News, 0, :TaskID, :EntrySeqID, :URLToGo, :Expiration)
        ");

        // Execute the insert statement
        $stmt->execute([
            ':Key' => "T-{$taskData['TaskID']}",
            ':Published' => gmdate('Y-m-d H:i:s'), // Current UTC time
            ':Title' => $title,
            ':Subtitle' => $taskData['Title'],
            ':Comments' => $comments,
            ':Credits' => $credits,
            ':EventDate' => null,
            ':News' => null,
            ':TaskID' => $taskData['TaskID'],
            ':EntrySeqID' => $taskData['EntrySeqID'],
            ':URLToGo' => null,
            ':Expiration' => null
        ]);
        logMessage("News entry created for TaskID: {$taskData['TaskID']}.");

    } catch (Exception $e) {
        logMessage("Error in createOrUpdateTaskNewsEntry: " . $e->getMessage());
        throw new Exception('Failed to create or update news entry.');
    }
}

// Function to delete task news entries
function deleteTaskNewsEntries($taskID) {
    global $newsDBPath;

    try {
        // Open the news database connection
        $newsPdo = new PDO("sqlite:$newsDBPath");
        $newsPdo->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);

        // Prepare the delete statement
        $stmt = $newsPdo->prepare("DELETE FROM News WHERE TaskID = :TaskID");
        $stmt->execute([':TaskID' => $taskID]);

        logMessage("News entries deleted for TaskID: $taskID.");

    } catch (Exception $e) {
        logMessage("Error in deleteTaskNewsEntries: " . $e->getMessage());
        throw new Exception('Failed to delete task news entries.');
    }
}
?>
