<?php
// Include the configuration file
$config = include 'config.php';

// Assign paths from the configuration array
$databasePath = $config['databasePath'];
$newsDBPath = $config['newsDBPath'];

// Dynamically insert the date into the log file name (VB.net convention)
$logFile = preg_replace(
    '/(error_log)(\.txt)$/i',
    '${1}_' . date('Ymd') . '${2}',
    $config['logFile']
);

$fileRootPath = $config['fileRootPath'];
$wsgRoot = $config['wsgRoot'];
$blesstok = $config['blesstok'];

// Repository paths (used by WeSimGlide.org)
$taskBrowserPath = isset($config['taskBrowserPath']) ? $config['taskBrowserPath'] : '';
$taskBrowserPathHTTPS = isset($config['taskBrowserPathHTTPS']) ? $config['taskBrowserPathHTTPS'] : '';
$homeLeaderboardCacheDir = isset($config['homeLeaderboardCacheDir']) ? $config['homeLeaderboardCacheDir'] : '';

$disWHPrefix = 'https://discord.com/api/webhooks/';
$disWHFlights = $disWHPrefix . $config['disWHFlights'];
$disWHAnnouncements = $disWHPrefix . $config['disWHAnnouncements'];
$disWHTestFlights = $disWHPrefix . $config['disWHTestFlights'];

// Function to log messages
function logMessage($message) {
    $clientIp = $_SERVER['REMOTE_ADDR'];
    global $logFile;
    $timestamp = date('Y-m-d H:i:s');
    file_put_contents($logFile, "[$timestamp] $clientIp $message\n", FILE_APPEND);
}

function logDiscordSyncMessage($message) {
    global $ENABLE_LOGGING;

    if (!empty($ENABLE_LOGGING)) {
        logMessage($message);
    }
}

function createSqliteConnection($databasePath) {
    $pdo = new PDO("sqlite:$databasePath");
    $pdo->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);
    $pdo->exec('PRAGMA busy_timeout = 5000');

    return $pdo;
}

function isSqliteDatabaseLockedException($exception) {
    if (!($exception instanceof Exception)) {
        return false;
    }

    $message = strtolower($exception->getMessage());
    return strpos($message, 'database is locked') !== false
        || strpos($message, 'database table is locked') !== false
        || strpos($message, 'sqlstate[hy000]: general error: 5') !== false;
}

function withSqliteBusyRetry($callback, $contextLabel = 'sqlite operation', $contextData = [], $maxAttempts = 3) {
    $attempt = 0;

    while ($attempt < $maxAttempts) {
        $attempt++;

        try {
            return $callback($attempt);
        } catch (Exception $e) {
            if (!isSqliteDatabaseLockedException($e) || $attempt >= $maxAttempts) {
                throw $e;
            }

            $contextParts = [];
            foreach ($contextData as $key => $value) {
                $contextParts[] = "$key=$value";
            }
            $contextSuffix = empty($contextParts) ? '' : ' [' . implode(', ', $contextParts) . ']';

            logDiscordSyncMessage(
                "Retrying $contextLabel due to SQLite lock (attempt $attempt/$maxAttempts)$contextSuffix"
            );

            usleep(100000 * $attempt);
        }
    }

    return null;
}

function refreshDiscordProfileIfStale($pdo, $wsgUserID) {
    global $config;

    $clientId = isset($config['discordClientId']) ? $config['discordClientId'] : '';
    $clientSecret = isset($config['discordClientSecret']) ? $config['discordClientSecret'] : '';

    if ($clientId === '' || $clientSecret === '') {
        return null;
    }

    $tableCheckStmt = $pdo->prepare(
        "SELECT name FROM sqlite_master WHERE type = 'table' AND name = 'UsersDiscordTokens'"
    );
    $tableCheckStmt->execute();
    if (!$tableCheckStmt->fetch(PDO::FETCH_ASSOC)) {
        return null;
    }

    $stmt = $pdo->prepare(
        "SELECT ud.DiscordID,
                t.AccessToken,
                t.RefreshToken,
                t.TokenExpiresUTC,
                t.LastDiscordSyncUTC
         FROM UsersDiscord ud
         INNER JOIN UsersDiscordTokens t ON t.WSGUserID = ud.WSGUserID
         WHERE ud.WSGUserID = ?
         LIMIT 1"
    );
    $stmt->execute([$wsgUserID]);
    $row = $stmt->fetch(PDO::FETCH_ASSOC);

    if (!$row) {
        return null;
    }

    $nowTimestamp = time();
    if (!empty($row['LastDiscordSyncUTC'])) {
        $lastSync = strtotime($row['LastDiscordSyncUTC'] . ' UTC');
        if ($lastSync !== false && ($nowTimestamp - $lastSync) < 86400) {
            return null;
        }
    }

    $accessToken = $row['AccessToken'];
    $refreshToken = $row['RefreshToken'];
    $tokenExpiryToPersist = $row['TokenExpiresUTC'];
    $tokenChanged = false;
    $tokenPersistedEarly = false;

    if (!empty($row['TokenExpiresUTC'])) {
        $tokenExpires = strtotime($row['TokenExpiresUTC'] . ' UTC');
        if ($tokenExpires !== false && $tokenExpires <= $nowTimestamp) {
            if (empty($refreshToken)) {
                logDiscordSyncMessage("Discord token expired and no refresh token for WSGUserID: $wsgUserID");
                return null;
            }

            $tokenUrl = 'https://discord.com/api/oauth2/token';
            $data = [
                'client_id' => $clientId,
                'client_secret' => $clientSecret,
                'grant_type' => 'refresh_token',
                'refresh_token' => $refreshToken
            ];

            $options = [
                'http' => [
                    'header'  => "Content-type: application/x-www-form-urlencoded\r\n",
                    'method'  => 'POST',
                    'content' => http_build_query($data)
                ]
            ];

            $context = stream_context_create($options);
            $result = file_get_contents($tokenUrl, false, $context);
            if ($result === false) {
                logDiscordSyncMessage("Failed to refresh Discord token for WSGUserID: $wsgUserID");
                return null;
            }

            $tokenData = json_decode($result, true);
            if (!isset($tokenData['access_token'])) {
                logDiscordSyncMessage("Invalid refresh token response for WSGUserID: $wsgUserID");
                return null;
            }

            $accessToken = $tokenData['access_token'];
            if (!empty($tokenData['refresh_token'])) {
                $refreshToken = $tokenData['refresh_token'];
            }

            $tokenExpiresUTC = null;
            if (!empty($tokenData['expires_in'])) {
                $tokenExpiresUTC = gmdate('Y-m-d H:i:s', $nowTimestamp + (int) $tokenData['expires_in']);
            }
            $tokenExpiryToPersist = $tokenExpiresUTC;
            $tokenChanged = true;

            // Persist rotated tokens immediately so a later profile fetch failure
            // does not leave stale refresh credentials in the database.
            withSqliteBusyRetry(function () use (
                $pdo,
                $accessToken,
                $refreshToken,
                $tokenExpiryToPersist,
                $wsgUserID
            ) {
                $updateTokenStmt = $pdo->prepare(
                    "UPDATE UsersDiscordTokens
                     SET AccessToken = ?,
                         RefreshToken = ?,
                         TokenExpiresUTC = ?
                     WHERE WSGUserID = ?"
                );
                $updateTokenStmt->execute([
                    $accessToken,
                    $refreshToken,
                    $tokenExpiryToPersist,
                    $wsgUserID
                ]);
            }, 'discord token refresh persist', ['WSGUserID' => $wsgUserID]);

            $tokenPersistedEarly = true;
        }
    }

    $userUrl = 'https://discord.com/api/users/@me';
    $options = [
        'http' => [
            'header' => "Authorization: Bearer $accessToken\r\n",
            'method' => 'GET'
        ]
    ];

    $context = stream_context_create($options);
    $userResult = file_get_contents($userUrl, false, $context);
    if ($userResult === false) {
        logDiscordSyncMessage("Failed to fetch Discord user profile for WSGUserID: $wsgUserID");
        return null;
    }

    $discordUser = json_decode($userResult, true);
    if (!isset($discordUser['id'])) {
        logDiscordSyncMessage("Invalid Discord user profile for WSGUserID: $wsgUserID");
        return null;
    }

    $displayName = !empty($discordUser['global_name'])
        ? $discordUser['global_name']
        : $discordUser['username'];

    $avatarURL = '';
    if (!empty($discordUser['avatar'])) {
        $avatarURL = "https://cdn.discordapp.com/avatars/" . $discordUser['id'] . "/" . $discordUser['avatar'] . ".png";
    }

    $nowUTC = gmdate('Y-m-d H:i:s');

    $currentUserStmt = $pdo->prepare(
        "SELECT WSGDisplayName, AvatarURL
         FROM Users
         WHERE WSGUserID = ?"
    );
    $currentUserStmt->execute([$wsgUserID]);
    $currentUser = $currentUserStmt->fetch(PDO::FETCH_ASSOC);

    $shouldUpdateUser = !$currentUser
        || (string) $currentUser['WSGDisplayName'] !== (string) $displayName
        || (string) $currentUser['AvatarURL'] !== (string) $avatarURL;

    withSqliteBusyRetry(function () use (
        $pdo,
        $shouldUpdateUser,
        $displayName,
        $avatarURL,
        $wsgUserID,
        $nowUTC,
        $tokenChanged,
        $tokenPersistedEarly,
        $accessToken,
        $refreshToken,
        $tokenExpiryToPersist
    ) {
        $pdo->exec('BEGIN IMMEDIATE');

        try {
            if ($tokenChanged && !$tokenPersistedEarly) {
                $updateTokenStmt = $pdo->prepare(
                    "UPDATE UsersDiscordTokens
                     SET AccessToken = ?,
                         RefreshToken = ?,
                         TokenExpiresUTC = ?
                     WHERE WSGUserID = ?"
                );
                $updateTokenStmt->execute([
                    $accessToken,
                    $refreshToken,
                    $tokenExpiryToPersist,
                    $wsgUserID
                ]);
            }

            if ($shouldUpdateUser) {
                $updateUserStmt = $pdo->prepare(
                    "UPDATE Users
                     SET WSGDisplayName = ?,
                         AvatarURL = ?
                     WHERE WSGUserID = ?"
                );
                $updateUserStmt->execute([$displayName, $avatarURL, $wsgUserID]);
            }

            $updateSyncStmt = $pdo->prepare(
                "UPDATE UsersDiscordTokens
                 SET LastDiscordSyncUTC = ?
                 WHERE WSGUserID = ?"
            );
            $updateSyncStmt->execute([$nowUTC, $wsgUserID]);

            $pdo->exec('COMMIT');
        } catch (Exception $e) {
            if ($pdo->inTransaction()) {
                $pdo->rollBack();
            }
            throw $e;
        }
    }, 'discord profile sync persist', ['WSGUserID' => $wsgUserID]);

    return [
        'displayName' => $displayName,
        'avatar' => $avatarURL
    ];
}

// Function to ensure datetime fields are properly formatted
function formatDatetime($datetime) {
    $dt = new DateTime($datetime);
    return $dt->format('Y-m-d H:i:s');
}

// Function to pretty print XML string
function prettyPrintXml($xmlString) {
    if (empty($xmlString)) {
        return '';
    }

    try {
        $dom = new DOMDocument('1.0', 'UTF-8');
        $dom->preserveWhiteSpace = false;
        $dom->formatOutput = true;

        if ($dom->loadXML($xmlString)) {
            return $dom->saveXML();
        } else {
            return $xmlString; // Return the original if parsing fails
        }
    } catch (Exception $e) {
        return $xmlString; // Fallback in case of errors
    }
}

// --- DB-backed: fetch one user's permissions by UserRightsID (or fallback by UserRightsName)
function getUserPermissions($userID) {
    global $databasePath;

    try {
        $pdo = createSqliteConnection($databasePath);

        // 1) Try by UserRightsID (this is what callers pass)
        $sql = "SELECT UserRightsID, UserRightsName,
                       CreateTask, UpdateTask, DeleteTask,
                       CreateEvent, UpdateEvent, DeleteEvent,
                       CreateNews,  UpdateNews,  DeleteNews
                FROM Users
                WHERE UserRightsID = :rid
                LIMIT 1";
        $stmt = $pdo->prepare($sql);
        $stmt->execute([':rid' => (string)$userID]);
        $row = $stmt->fetch(PDO::FETCH_ASSOC);

        // 2) Fallback: allow callers to pass the name (legacy callers)
        if (!$row) {
            $sql2 = str_replace('UserRightsID = :rid', 'UserRightsName = :rname COLLATE NOCASE', $sql);
            $stmt = $pdo->prepare($sql2);
            $stmt->execute([':rname' => (string)$userID]);
            $row = $stmt->fetch(PDO::FETCH_ASSOC);
        }

        if (!$row) {
            logMessage("No permissions found (DB) for identifier: $userID");
            return null;
        }

        // Build an object that mimics the old SimpleXMLElement shape:
        // ->ID, ->Name, then each right as 'True'/'False' strings
        $o = new stdClass();
        $o->ID   = (string)$row['UserRightsID'];
        $o->Name = (string)$row['UserRightsName'];

        $rights = [
            'CreateTask','UpdateTask','DeleteTask',
            'CreateEvent','UpdateEvent','DeleteEvent',
            'CreateNews','UpdateNews','DeleteNews'
        ];
        foreach ($rights as $r) {
            $o->{$r} = ((int)$row[$r] === 1) ? 'True' : 'False';
        }
        return $o;

    } catch (Exception $e) {
        logMessage("getUserPermissions DB error: " . $e->getMessage());
        return null;
    }
}

// Works with BOTH SimpleXMLElement (old) and stdClass (new)
function getUserRights($userPermissions) {
    if ($userPermissions === null) return null;

    $rights = [];

    // New stdClass path
    if (!($userPermissions instanceof SimpleXMLElement)) {
        foreach (get_object_vars($userPermissions) as $key => $value) {
            if ($key !== 'ID' && $key !== 'Name') {
                $rights[$key] = (string)$value;   // 'True' or 'False'
            }
        }
        return $rights;
    }

    // Legacy XML path (kept for compatibility)
    foreach ($userPermissions->children() as $key => $value) {
        if ($key !== 'ID' && $key !== 'Name') {
            $rights[$key] = (string)$value;
        }
    }
    return $rights;
}

// Works with BOTH shapes
function checkUserRight($userPermissions, $right) {
    if ($userPermissions === null) return false;

    if (!($userPermissions instanceof SimpleXMLElement)) {
        // stdClass
        $val = isset($userPermissions->{$right}) ? (string)$userPermissions->{$right} : 'False';
        return $val === 'True';
    }
    // XML
    return (string)$userPermissions->$right === 'True';
}

// Unchanged signature; now uses the DB-backed getUserPermissions()
function checkUserPermission($userID, $permission) {
    $userPermissions = getUserPermissions($userID);
    if ($userPermissions === null) {
        logMessage("No permissions found for userID: $userID");
        return false;
    }
    $hasRight = checkUserRight($userPermissions, $permission);
    logMessage("Permission check result for userID: $userID, Permission: $permission, HasRight: " . ($hasRight ? 'True' : 'False'));
    return $hasRight;
}

// Function to clean up expired News and Event entries (including Discord posts)
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

// Function to clean up old pending tasks
function cleanUpPendingTasks($pdo) {
    try {
        logMessage("Cleaning up old pending tasks of more than 5 days");
        $datetimeMinus5Days = $pdo->query("SELECT datetime('now', '-5 days')")->fetchColumn();
        $deleteStmt = $pdo->prepare("
            DELETE FROM Tasks
            WHERE Status = 10
            AND LastUpdate < :datetimeMinus5Days
        ");
        $deleteStmt->execute([':datetimeMinus5Days' => $datetimeMinus5Days]);
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
        logMessage("News entry created or updated for TaskID: {$taskData['TaskID']}.");

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

// Function to retrieve and unpack a DPHX file into a task-specific folder
function retrieveAndUnpackDPHX($taskID) {
    global $taskBrowserPath, $taskBrowserPathHTTPS;

    $tempDir = __DIR__ . '/DPHXTemp';
    $taskFolder = "$tempDir/$taskID";
    $dphxFile = "$taskFolder/$taskID.dphx";
    $repositoryUrl = "$taskBrowserPath/Tasks/$taskID.dphx";
    $repositoryUrlHTTPS = "$taskBrowserPathHTTPS/Tasks/$taskID.dphx";

    // Ensure the temp directory exists
    if (!file_exists($tempDir)) {
        mkdir($tempDir, 0755, true);
    }

    // **Register cleanup function to run at the end**
    register_shutdown_function('cleanupOldTempFolders', $tempDir);

    // Get last modified time of the remote file
    $remoteLastModified = getRemoteFileLastModified($repositoryUrlHTTPS);
    $localLastModified = file_exists($taskFolder) ? filemtime($taskFolder) : 0;

    // If the folder doesn't exist OR the DPHX file was updated, delete the folder and refresh
    if (!file_exists($taskFolder) || ($remoteLastModified > $localLastModified && $remoteLastModified > 0)) {
        if (file_exists($taskFolder)) {
            deleteFolder($taskFolder);
        }

        mkdir($taskFolder, 0755, true);

        // Download the DPHX file
        $dphxContent = @file_get_contents($repositoryUrl);
        if ($dphxContent === false) {
            throw new Exception("DPHX file not found in repository for TaskID $taskID.");
        }

        file_put_contents($dphxFile, $dphxContent);

        // Extract the DPHX file
        $zip = new ZipArchive();
        if ($zip->open($dphxFile) === TRUE) {
            $zip->extractTo($taskFolder);
            $zip->close();
        } else {
            throw new Exception("Failed to extract DPHX file for TaskID $taskID.");
        }
    }

    return $taskFolder;
}

// Function to clean up old temporary folders
function cleanupOldTempFolders($tempDir) {
    foreach (glob("$tempDir/*") as $folder) {
        if (is_dir($folder) && time() - filemtime($folder) > 48 * 3600) {
            deleteFolder($folder);
        }
    }
}

// Function to delete a folder and its contents
function deleteFolder($folder) {
    if (!is_dir($folder)) return;
    foreach (glob("$folder/*") as $file) {
        is_dir($file) ? deleteFolder($file) : unlink($file);
    }
    rmdir($folder);
}

// Function to fetch the last modified timestamp of a remote file
function getRemoteFileLastModified($url) {
    $ch = curl_init($url);
    curl_setopt($ch, CURLOPT_NOBODY, true); // Fetch headers only
    curl_setopt($ch, CURLOPT_RETURNTRANSFER, true);
    curl_setopt($ch, CURLOPT_HEADER, true);
    curl_setopt($ch, CURLOPT_FILETIME, true);
    curl_setopt($ch, CURLOPT_SSL_VERIFYPEER, false); // Disable SSL verification if needed
    curl_setopt($ch, CURLOPT_FOLLOWLOCATION, true); // Follow redirects
    curl_setopt($ch, CURLOPT_TIMEOUT, 10); // Prevent infinite waiting

    $headers = curl_exec($ch);
    $filetime = curl_getinfo($ch, CURLINFO_FILETIME);
    $http_code = curl_getinfo($ch, CURLINFO_HTTP_CODE);
    $curl_error = curl_error($ch);
    curl_close($ch);

    if ($http_code !== 200 || $filetime === -1) {
        logMessage("Error: Unable to retrieve Last-Modified for $url. HTTP Code: $http_code. cURL Error: $curl_error.");
        return 0;
    }

    return $filetime;
}

// Function to manage Discord posts (create, update, delete)
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
    if (!function_exists('executeCurl')) {
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
function getDiscordMessageContent($webhookUrl, $postID) {
    // Construct the URL to retrieve the message.
    $url = rtrim($webhookUrl, '/') . "/messages/" . $postID;
    
    $ch = curl_init($url);
    curl_setopt($ch, CURLOPT_RETURNTRANSFER, true);
    curl_setopt($ch, CURLOPT_TIMEOUT, 10);
    
    $responseData = curl_exec($ch);
    if (curl_errno($ch)) {
        $error = curl_error($ch);
        curl_close($ch);
        return json_encode([
            "result"  => "error",
            "error"   => "cURL Error: " . $error,
            "content" => ""
        ]);
    }
    $httpCode = curl_getinfo($ch, CURLINFO_HTTP_CODE);
    curl_close($ch);
    
    if ($httpCode == 200) {
        $messageObj = json_decode($responseData, true);
        $content = isset($messageObj['content']) ? $messageObj['content'] : "";
        return json_encode([
            "result"  => "success",
            "content" => $content
        ]);
    } else {
        return json_encode([
            "result"  => "error",
            "error"   => "Error retrieving message. HTTP Code: " . $httpCode,
            "content" => ""
        ]);
    }
}

/**
 * Returns the IP address of the current user.
 *
 * Checks various headers in case of proxies (e.g. Cloudflare),
 * then falls back to REMOTE_ADDR.
 *
 * @return string Client IP address.
 */
function getClientIP(): string
{
    // If behind Cloudflare, this header will have the real IP
    if (!empty($_SERVER['HTTP_CF_CONNECTING_IP'])) {
        return $_SERVER['HTTP_CF_CONNECTING_IP'];
    }

    // Check for shared internet/ISP IP
    if (!empty($_SERVER['HTTP_CLIENT_IP'])) {
        return $_SERVER['HTTP_CLIENT_IP'];
    }

    // Check for IPs passed from proxies
    if (!empty($_SERVER['HTTP_X_FORWARDED_FOR'])) {
        // Can be a comma-separated list; take the first one (original client)
        $ips = explode(',', $_SERVER['HTTP_X_FORWARDED_FOR']);
        return trim($ips[0]);
    }

    // Default fallback
    return $_SERVER['REMOTE_ADDR'] ?? '0.0.0.0';
}

/**
 * Builds a download requester signature to reduce duplicate counts.
 *
 * Uses IP, user-agent, and session cookie (if present) to better
 * differentiate users behind CGNAT while keeping a stable fingerprint
 * for the same browser session.
 */
function getDownloadRequesterSignature(): string
{
    $ipSource = getClientIP();
    $userAgent = $_SERVER['HTTP_USER_AGENT'] ?? '';
    $sessionName = session_name();
    $sessionId = '';

    if (!empty($sessionName) && isset($_COOKIE[$sessionName])) {
        $sessionId = (string)$_COOKIE[$sessionName];
    }

    $signatureSource = $ipSource . '|' . $userAgent . '|' . $sessionId;
    return 'sig:' . hash('sha256', $signatureSource);
}

/**
 * Records a task download if this requester has not already downloaded today.
 *
 * Returns true if the download was counted, false if it was a duplicate.
 */
function recordUniqueTaskDownload(PDO $pdo, int $entrySeqID): bool
{
    try {
        $now = new DateTime("now", new DateTimeZone("UTC"));
        $nowFormatted = $now->format('Y-m-d H:i:s');
        $dateOnly = $now->format('Y-m-d');
        $ipSource = getDownloadRequesterSignature();

        $pdo->beginTransaction();

        $checkQuery = "
            SELECT 1
              FROM TaskDownloads
             WHERE IPSource   = :ipSource
               AND Date       = :date
               AND EntrySeqID = :entrySeqID
             LIMIT 1
        ";
        $stmt = $pdo->prepare($checkQuery);
        $stmt->execute([
            ':ipSource' => $ipSource,
            ':date' => $dateOnly,
            ':entrySeqID' => $entrySeqID
        ]);

        if ($stmt->fetch()) {
            $pdo->commit();
            return false;
        }

        $insertQuery = "
            INSERT INTO TaskDownloads (IPSource, Date, EntrySeqID)
            VALUES (:ipSource, :date, :entrySeqID)
        ";
        $ins = $pdo->prepare($insertQuery);
        $ins->execute([
            ':ipSource' => $ipSource,
            ':date' => $dateOnly,
            ':entrySeqID' => $entrySeqID
        ]);

        $updateQuery = "
            UPDATE Tasks
               SET TotDownloads       = TotDownloads + 1,
                   LastDownloadUpdate = :lastDownloadUpdate
             WHERE EntrySeqID        = :id
        ";
        $update = $pdo->prepare($updateQuery);
        $update->bindParam(':lastDownloadUpdate', $nowFormatted, PDO::PARAM_STR);
        $update->bindParam(':id', $entrySeqID, PDO::PARAM_INT);
        $update->execute();

        $pdo->commit();
        return true;
    } catch (Exception $error) {
        if ($pdo->inTransaction()) {
            $pdo->rollBack();
        }
        throw $error;
    }
}

function browserlessExtractTracklogsOnly(
    string $igcUrl,
    string $plnUrl,
    string $flow = 'forced',
    int $timeoutSeconds = 60
): array {
    global $blesstok;

    $flow = strtolower(trim($flow));
    if ($flow !== 'forced' && $flow !== 'normal') {
        $flow = 'forced';
    }

    if (!isset($blesstok) || trim($blesstok) === '') {
        return [
            'ok' => false,
            'http_code' => 0,
            'plannerUrl' => '',
            'data' => null,
            'raw' => null,
            'error' => 'Browserless token ($blesstok) is not configured in CommonFunctions.php',
        ];
    }

    $igcUrl = trim($igcUrl);
    $plnUrl = trim($plnUrl);

    if ($igcUrl === '' || $plnUrl === '') {
        return [
            'ok' => false,
            'http_code' => 0,
            'plannerUrl' => '',
            'data' => null,
            'raw' => null,
            'error' => 'Missing igcUrl or plnUrl',
        ];
    }

    // Keep EXACT behavior of your working curl:
    // - planner expects host/path (no https:// prefix)
    // - do NOT urlencode the igc/pln values
    $igcUrl = preg_replace('#^https?://#i', '', $igcUrl);
    $plnUrl = preg_replace('#^https?://#i', '', $plnUrl);

    // Build planner URL exactly like curl (spaces allowed)
    $plannerUrl = 'https://xp-soaring.github.io/tasks/b21_task_planner/index.html'
        . '?igc=' . $igcUrl
        . '&pln=' . $plnUrl;

    // Same endpoint/options as your curl (token + blockConsentModals=true)
    $endpoint = 'https://production-sfo.browserless.io/chrome/bql?token='
              . rawurlencode($blesstok)
              . '&blockConsentModals=true';

    // Build the BQL mutation (forced flow matches your working curl)
    $query = <<<GQL
mutation ExtractTracklogsOnly {
  viewport(width: 1366, height: 768) {
    width
    height
    time
  }

  goto(
    url: "{$plannerUrl}",
    waitUntil: networkIdle
  ) {
    status
  }

  waitTabReady: waitForSelector(selector: "#tab_tracklogs a", visible: true) {
    time
  }
  clickTabTracklogs: click(selector: "#tab_tracklogs a", wait: true, visible: true) {
    time
  }

  # Wait for at least one tracklog entry
  waitTracklogRow: waitForSelector(
    selector: "#tracklogs_table .tracklogs_entry_current .tracklogs_entry_info, #tracklogs_table .tracklogs_entry_info",
    visible: true
  ) {
    time
  }

  # Click the first tracklog info cell
  clickTracklogInfo: click(
    selector: "#tracklogs_table .tracklogs_entry_current .tracklogs_entry_info, #tracklogs_table .tracklogs_entry_info",
    wait: true,
    visible: true
  ) {
    time
  }

GQL;

    // Optional “normal” flow includes clicking “Load task”
    if ($flow === 'normal') {
        $query .= <<<GQL
  # Wait for the Load Task button to show
  waitLoadTaskButton: waitForSelector(selector: "#tracklog_info_load_task", visible: true) {
    time
  }
  # Click the Load Task button
  clickLoadTask: click(selector: "#tracklog_info_load_task", wait: true, visible: true) {
    time
  }

  # Planner may switch tabs after loading; force Tracklogs tab back on.
  waitTabTracklogsAfterLoad: waitForSelector(selector: "#tab_tracklogs a", visible: true) {
    time
  }
  # Click the Tracklogs tab again
  clickTabTracklogsAfterLoad: click(selector: "#tab_tracklogs a", wait: true, visible: true) {
    time
  }

  # Wait for at least one tracklog entry
  waitTracklogRowAfterLoad: waitForSelector(
    selector: "#tracklogs_table .tracklogs_entry_current .tracklogs_entry_info, #tracklogs_table .tracklogs_entry_info",
    visible: true
  ) {
    time
  }

  # Click the first tracklog info cell
  clickTracklogInfoAfterLoad: click(
    selector: "#tracklogs_table .tracklogs_entry_current .tracklogs_entry_info, #tracklogs_table .tracklogs_entry_info",
    wait: true,
    visible: true
  ) {
    time
  }

GQL;
    }

    $query .= <<<GQL
  # Read the IGC line from the right-hand info panel
  waitIgcInfo: waitForSelector(selector: "#tracklog_info_details .igc", visible: true) {
    time
  }
  igcInfo: html(selector: "#tracklog_info_details .igc", visible: true) {
    html
  }

  # Tracklogs table HTML
  waitTracklogsTable: waitForSelector(selector: "#tracklogs_table", visible: true) {
    time
  }
  tracklogsHTML: html(selector: "#tracklogs_table", visible: true) {
    html
  }

  plannerVersion: html(selector: "#b21_task_planner_version", visible: true) {
    html
  }
}
GQL;

    $payload = json_encode([
        'query' => $query,
        'variables' => (object)[],
        'operationName' => 'ExtractTracklogsOnly'
    ], JSON_UNESCAPED_SLASHES);

    if ($payload === false) {
        return [
            'ok' => false,
            'http_code' => 0,
            'plannerUrl' => $plannerUrl,
            'data' => null,
            'raw' => null,
            'error' => 'Failed to JSON-encode request payload: ' . json_last_error_msg(),
        ];
    }

    $ch = curl_init();
    curl_setopt_array($ch, [
        CURLOPT_URL => $endpoint,
        CURLOPT_RETURNTRANSFER => true,
        CURLOPT_POST => true,
        CURLOPT_POSTFIELDS => $payload,
        CURLOPT_HTTPHEADER => [
            'Content-Type: application/json',
            'Accept: application/json',
            'Expect:', // avoids 100-continue edge cases
        ],
        CURLOPT_TIMEOUT => $timeoutSeconds,
        CURLOPT_CONNECTTIMEOUT => 20,
    ]);

    $respBody = curl_exec($ch);
    $curlErrNo = curl_errno($ch);
    $curlErr   = $curlErrNo ? curl_error($ch) : null;
    $httpCode  = (int) curl_getinfo($ch, CURLINFO_HTTP_CODE);
    curl_close($ch);

    if ($curlErrNo) {
        return [
            'ok' => false,
            'http_code' => $httpCode,
            'plannerUrl' => $plannerUrl,
            'data' => null,
            'raw' => null,
            'error' => 'cURL error calling Browserless: ' . $curlErr,
        ];
    }

    if ($respBody === false || $respBody === '') {
        return [
            'ok' => false,
            'http_code' => $httpCode,
            'plannerUrl' => $plannerUrl,
            'data' => null,
            'raw' => null,
            'error' => 'Empty response from Browserless',
        ];
    }

    $decoded = json_decode($respBody, true);
    if (json_last_error() !== JSON_ERROR_NONE) {
        return [
            'ok' => false,
            'http_code' => ($httpCode ?: 0),
            'plannerUrl' => $plannerUrl,
            'data' => null,
            'raw' => $respBody,
            'error' => 'Browserless returned non-JSON response: ' . json_last_error_msg(),
        ];
    }

    // Browserless can return HTTP 200 with GraphQL "errors"
    $ok = ($httpCode >= 200 && $httpCode < 300) && !isset($decoded['errors']);

    return [
        'ok' => $ok,
        'http_code' => ($httpCode ?: 200),
        'plannerUrl' => $plannerUrl,
        'data' => $decoded,
        'raw' => null,
        'error' => $ok ? null : (isset($decoded['errors']) ? 'Browserless GraphQL errors present.' : 'HTTP error from Browserless.'),
    ];
}

function parseBrowserlessTracklogs(array $browserlessResult, ?string $igcKeyDir = null, bool $logEnabled = false): array
{
    if ($logEnabled && $igcKeyDir) {
        file_put_contents(
            $igcKeyDir . '/browserless_full_dump.json',
            json_encode($browserlessResult, JSON_PRETTY_PRINT)
        );
    }

    if (!isset($browserlessResult['data']['tracklogsHTML']['html'])) {
        $browserlessResult['error'] = "Browserless response did not include tracklogs HTML.";
        return $browserlessResult;
    }

    $htmlContent = $browserlessResult['data']['tracklogsHTML']['html'];
    if (stripos(ltrim($htmlContent), '<tr') === 0) {
        $htmlContent = '<table id="tracklogs_table">' . $htmlContent . '</table>';
    }

    $plannerVersion = '';
    if (isset($browserlessResult['data']['plannerVersion']['html'])) {
        $plannerVersion = trim($browserlessResult['data']['plannerVersion']['html']);
    }

    $igcValid = false;
    if (isset($browserlessResult['data']['igcInfo']['html'])) {
        $igcInfoHtml = $browserlessResult['data']['igcInfo']['html'];
        $igcInfoText = trim(strip_tags($igcInfoHtml));
        if (mb_strpos($igcInfoText, '🔒') !== false) {
            $igcValid = true;
        }
    }

    $htmlContent = '<?xml encoding="UTF-8">' . $htmlContent;
    $dom = new DOMDocument('1.0', 'UTF-8');
    libxml_use_internal_errors(true);
    $dom->loadHTML($htmlContent);
    libxml_clear_errors();
    $xpath = new DOMXPath($dom);

    $targetRow = null;
    $nodes = $xpath->query('//*[contains(@class,"tracklogs_entry_current")]');
    if ($nodes->length > 0) {
        $targetRow = $nodes->item(0);
    } else {
        $nodes = $xpath->query('//*[contains(@class,"tracklogs_entry")]');
        if ($nodes->length > 0) {
            $targetRow = $nodes->item(0);
        }
    }

    if (!$targetRow) {
        $browserlessResult['error'] = "No tracklogs entry node found in the HTML.";
        return $browserlessResult;
    }

    $infoDiv = $xpath->query('.//*[contains(@class,"tracklogs_entry_info")]', $targetRow)->item(0);
    if (!$infoDiv) {
        $browserlessResult['error'] = "Information column not found in tracklogs entry.";
        return $browserlessResult;
    }

    $nameDiv = $xpath->query('.//div[contains(@class,"tracklogs_entry_name")]', $infoDiv)->item(0);
    $searchContext = $nameDiv ?: $infoDiv;

    $resultDivCandidates = $xpath->query('.//div[contains(@class,"tracklogs_entry_finished")]', $searchContext);
    if ($resultDivCandidates->length === 0) {
        $browserlessResult['error'] = "Result element not found in tracklogs entry.";
        return $browserlessResult;
    }

    $resultDiv = $resultDivCandidates->item(0);
    $class     = $resultDiv->getAttribute('class');
    $taskCompleted = (strpos($class, "tracklogs_entry_finished_ok") !== false);
    $penalties     = (strpos($class, "penalties") !== false);

    $span       = $xpath->query('.//span', $resultDiv)->item(0);
    $resultText = $span ? trim($span->textContent) : '';

    $duration = $distance = $speed = null;

    if ($taskCompleted) {
        $parts = preg_split('/\s+/', $resultText);
        if (count($parts) >= 3 && strpos($parts[1], 'km') !== false) {
            $duration = $parts[0];
            $distance = sprintf('%.1f', floatval(str_replace('km', '', $parts[1])));
            $speed    = sprintf('%.1f', floatval(str_replace('kph', '', $parts[2])));
        } elseif (count($parts) >= 2) {
            $duration = $parts[0];
            $speed    = sprintf('%.1f', floatval(str_replace('kph', '', $parts[1])));
        }
    } else {
        if ($resultText !== '') {
            $distance = sprintf('%.1f', floatval(str_replace('km', '', $resultText)));
        }
    }

    $parsedResults = [
        "TaskCompleted" => $taskCompleted,
        "Penalties"     => $penalties,
        "Duration"      => $duration,
        "Distance"      => $distance,
        "Speed"         => $speed,
        "IGCValid"      => $igcValid,
        "TPVersion"     => $plannerVersion
    ];

    if ($igcKeyDir) {
        $resultsFile = $igcKeyDir . '/results.json';
        $jsonData    = json_encode($parsedResults, JSON_PRETTY_PRINT);
        if ($jsonData === false) {
            $browserlessResult['error'] = "Failed to encode parsed results as JSON: " . json_last_error_msg();
            return $browserlessResult;
        }
        if (file_put_contents($resultsFile, $jsonData) === false) {
            $browserlessResult['error'] = "Failed to write results file to $resultsFile";
            return $browserlessResult;
        }
    }

    $browserlessResult['parsedResults'] = $parsedResults;
    return $browserlessResult;
}

?>
