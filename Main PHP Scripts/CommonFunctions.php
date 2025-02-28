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
$soaringClubsPath = $config['soaringClubsPath'];
$fileRootPath = $config['fileRootPath'];
$disWHPrefix = 'https://discord.com/api/webhooks/';
$disWHFlights = $disWHPrefix . $config['disWHFlights'];
$disWHAnnouncements = $disWHPrefix . $config['disWHAnnouncements'];

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
    global $disWHAnnouncements;

    // Get the current UTC datetime
    $currentDatetime = $pdo->query("SELECT datetime('now')")->fetchColumn();

    // Get the datetime for 7 days ago
    $datetimeMinus7Days = $pdo->query("SELECT datetime('now', '-7 days')")->fetchColumn();

    // Cleanup expired News and Event entries
    // Step 1: Fetch Keys for expired News entries of NewsType 1 and 2
    $expiredKeys = $pdo->query("SELECT DISTINCT Key FROM News WHERE NewsType IN (1, 2) AND Expiration < datetime('now')")->fetchAll(PDO::FETCH_COLUMN);

    if (!empty($expiredKeys)) {
        // Before deleting from the Events table, try to delete the corresponding Discord posts.
        foreach ($expiredKeys as $key) {
            // Query the Events table for a Discord post ID (assuming your field is WSGAnnouncementID)
            $stmt = $pdo->prepare("SELECT WSGAnnouncementID FROM Events WHERE EventKey = ?");
            $stmt->execute([$key]);
            if ($row = $stmt->fetch(PDO::FETCH_ASSOC)) {
                $discordPostID = $row['WSGAnnouncementID'];
                if (!empty($discordPostID)) {
                    // Attempt to delete the Discord post
                    $result = manageDiscordPost($disWHAnnouncements, "", $discordPostID, true);
                    $resultObj = json_decode($result, true);
                    if ($resultObj['result'] !== "success") {
                        logMessage("CleanUp: Failed to delete Discord post with ID $discordPostID for event key $key. Error: " . $resultObj['error']);
                    }
                }
            }
        }

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

function manageDiscordPost($webhookUrl, $messageContent = '', $postID = null, $deletePost = false) {
    // Prepare the base response structure.
    $response = [
        "result" => "error",
        "error" => "",
        "postID" => null
    ];
    
    // Validate parameters.
    if ($deletePost) {
        if (empty($postID)) {
            $response['error'] = "Post ID is required when deletePost is true.";
            return json_encode($response);
        }
    } else {
        // For creation or update, message content must be provided.
        if (empty($messageContent)) {
            $response['error'] = "Message content is required for creating or updating a post.";
            return json_encode($response);
        }
    }
    
    // Function to execute cURL and handle errors.
    function executeCurl($ch) {
        $result = curl_exec($ch);
        if (curl_errno($ch)) {
            $error = curl_error($ch);
            curl_close($ch);
            return ["error" => $error, "result" => false, "resultData" => null];
        }
        $httpCode = curl_getinfo($ch, CURLINFO_HTTP_CODE);
        curl_close($ch);
        return ["error" => "", "result" => $httpCode, "resultData" => $result];
    }
    
    // DELETE operation.
    if ($deletePost) {
        $url = rtrim($webhookUrl, '/') . "/messages/" . $postID;
        $ch = curl_init($url);
        curl_setopt($ch, CURLOPT_CUSTOMREQUEST, "DELETE");
        curl_setopt($ch, CURLOPT_RETURNTRANSFER, true);
        // Optional: set a timeout, e.g., 10 seconds.
        curl_setopt($ch, CURLOPT_TIMEOUT, 10);
        
        $resultData = executeCurl($ch);
        if ($resultData["result"] === false) {
            $response['error'] = "cURL Error: " . $resultData["error"];
        } elseif ($resultData["result"] == 204) { // 204 No Content indicates success.
            $response['result'] = "success";
        } else {
            $response['error'] = "Error deleting message. HTTP Code: " . $resultData["result"];
        }
        return json_encode($response);
    }
    
    // UPDATE operation (postID is provided and deletePost is false).
    if (!empty($postID)) {
        $url = rtrim($webhookUrl, '/') . "/messages/" . $postID;
        $data = ["content" => $messageContent];
        
        $ch = curl_init($url);
        curl_setopt($ch, CURLOPT_CUSTOMREQUEST, "PATCH");
        curl_setopt($ch, CURLOPT_HTTPHEADER, ['Content-Type: application/json']);
        curl_setopt($ch, CURLOPT_POSTFIELDS, json_encode($data));
        curl_setopt($ch, CURLOPT_RETURNTRANSFER, true);
        curl_setopt($ch, CURLOPT_TIMEOUT, 10);
        
        $resultData = executeCurl($ch);
        if ($resultData["result"] === false) {
            $response['error'] = "cURL Error: " . $resultData["error"];
        } elseif ($resultData["result"] == 200) {
            $response['result'] = "success";
            $response['postID'] = $postID;
        } else {
            $response['error'] = "Error updating message. HTTP Code: " . $resultData["result"];
        }
        return json_encode($response);
    }
    
    // CREATE operation (no postID provided).
    // Use ?wait=true to have Discord return the full message object including the post ID.
    $url = rtrim($webhookUrl, '/') . "?wait=true";
    $data = ["content" => $messageContent];
    
    $ch = curl_init($url);
    curl_setopt($ch, CURLOPT_HTTPHEADER, ['Content-Type: application/json']);
    curl_setopt($ch, CURLOPT_POST, true);
    curl_setopt($ch, CURLOPT_POSTFIELDS, json_encode($data));
    curl_setopt($ch, CURLOPT_RETURNTRANSFER, true);
    curl_setopt($ch, CURLOPT_TIMEOUT, 10);
    
    $resultData = executeCurl($ch);
    if ($resultData["result"] === false) {
        $response['error'] = "cURL Error: " . $resultData["error"];
    } else {
        $httpCode = $resultData["result"];
        $resultJson = json_decode($resultData["resultData"], true);
        if ($httpCode == 200 && isset($resultJson['id'])) {
            $response['result'] = "success";
            $response['postID'] = $resultJson['id'];
        } else {
            $response['error'] = "Error creating message. HTTP Code: " . $httpCode;
            if (isset($resultJson['message'])) {
                $response['error'] .= " - " . $resultJson['message'];
            }
        }
    }
    
    return json_encode($response);
}

?>
