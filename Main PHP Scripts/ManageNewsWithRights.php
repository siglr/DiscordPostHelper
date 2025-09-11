<?php
require __DIR__ . '/CommonFunctions.php';

try {
    logMessage("--- Script running ManageNewsWithRights ---");

    // Open the database connection
    $pdo = new PDO("sqlite:$newsDBPath");
    $pdo->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);

    // attach the tasks DB to this connection
    $pdo->exec("ATTACH DATABASE " . $pdo->quote($databasePath) . " AS TasksDB");

    // Call the cleanup function
    cleanUpNewsEntries($pdo);

    // Ensure the request method is POST
    if ($_SERVER['REQUEST_METHOD'] !== 'POST') {
        throw new Exception('Invalid request method.');
    }

    // Check if action and userID are set
    if (!isset($_POST['action']) || !isset($_POST['UserID'])) {
        throw new Exception('Action or UserID not specified.');
    }

    $action = $_POST['action'];
    $userID = $_POST['UserID'];
    $pastEventOnly = isset($_POST['PastEventOnly'])
        ? filter_var($_POST['PastEventOnly'], FILTER_VALIDATE_BOOLEAN)
        : false;

    // Permission map for actions
    $permissions = [
        'CreateTask' => 'CreateTask',
        'UpdateTask' => 'UpdateTask',
        'DeleteTask' => 'DeleteTask',
        'CreateEvent' => 'CreateEvent',
        'UpdateEvent' => 'UpdateEvent',
        'DeleteEvent' => 'DeleteEvent',
        'CreateNews' => 'CreateNews',
        'UpdateNews' => 'UpdateNews',
        'DeleteNews' => 'DeleteNews'
    ];

    // Check if the action is valid and permission exists
    if (!isset($permissions[$action])) {
        throw new Exception('Invalid action specified.');
    }

    // Check if the user has the required permission
    if (!checkUserPermission($userID, $permissions[$action])) {
        throw new Exception('User does not have permission to perform this action.');
    }

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
            logMessage("Created Task News entry: $key");
            break;

        case 'DeleteTask':
            if (!isset($_POST['TaskID'])) {
                throw new Exception('TaskID missing.');
            }
            $taskID = $_POST['TaskID'];
            $key = "T-$taskID";

            // Delete existing Task entry
            $stmt = $pdo->prepare("DELETE FROM News WHERE NewsType = 0 AND Key = ?");
            $stmt->execute([$key]);
            $affectedRows = $stmt->rowCount();

            if ($affectedRows === 0) {
                throw new Exception('No task found with the specified TaskID.');
            }
            logMessage("Deleted Task News entry: $key");
            break;

        case 'CreateEvent':
            if (!isset($_POST['Key'])) {
                throw new Exception('Key missing.');
            }
            $key = $_POST['Key'];

            // ---- PastEventOnly mode: only create TaskEvents link and exit ----
            if ($pastEventOnly === true) {
                // REQUIREMENTS for TaskEvents row
                $entrySeqID = $_POST['EntrySeqID'] ?? '';
                $meetDT     = $_POST['EventMeetDateTime'] ?? '';

                if (empty($entrySeqID)) {
                    throw new Exception('EntrySeqID is required when PastEventOnly is true.');
                }
                if (empty($meetDT)) {
                    throw new Exception('EventMeetDateTime is required when PastEventOnly is true.');
                }

                // Extract ClubEventNewsID from EventKey: "E-<ClubEventNewsID><yyyyMMdd>"
                $clubEventNewsID = '';
                if (preg_match('/^E-(.+)(\d{8})$/', $key, $m)) {
                    $clubEventNewsID = $m[1];
                } else {
                    // fallback: strip "E-" prefix and trailing 8-digit date if present
                    $clubEventNewsID = substr($key, 2, -8);
                }

                // Keep only the date part (YYYY-MM-DD) for TaskEvents.EventDateTime
                $eventDTForLink = formatDatetime($meetDT);
                if (!empty($eventDTForLink)) {
                    $eventDTForLink = substr($eventDTForLink, 0, 10);
                }

                if (empty($eventDTForLink)) {
                    throw new Exception('Invalid EventMeetDateTime (date part could not be determined).');
                }

                // Maintain TaskEvents like normal flow: clear previous links for this EventKey, then insert
                $pdo->beginTransaction();
                try {
                    $pdo->prepare("DELETE FROM TasksDB.TaskEvents WHERE EventKey = ?")->execute([$key]);

                    $stmtTE = $pdo->prepare('
                        INSERT INTO TasksDB.TaskEvents (EntrySeqID, ClubEventNewsID, EventDateTime, EventKey)
                        VALUES (:EntrySeqID, :ClubEventNewsID, :EventDateTime, :EventKey)
                    ');
                    $stmtTE->execute([
                        ':EntrySeqID'      => $entrySeqID,
                        ':ClubEventNewsID' => $clubEventNewsID,
                        ':EventDateTime'   => $eventDTForLink,
                        ':EventKey'        => $key
                    ]);

                    $pdo->commit();
                    logMessage("PastEventOnly: Created TaskEvents entry for event key $key");
                } catch (Throwable $e) {
                    $pdo->rollBack();
                    throw $e;
                }

                // Short-circuit the normal CreateEvent flow
                break;
            }

            // Retrieve existing Discord announcement info (if any) before deletion
            $existingAnnouncement = null;
            $existingAnnouncementID = null;
            $stmt = $pdo->prepare("SELECT WSGAnnouncement, WSGAnnouncementID FROM Events WHERE EventKey = ?");
            $stmt->execute([$key]);
            if ($row = $stmt->fetch(PDO::FETCH_ASSOC)) {
                $existingAnnouncement = $row['WSGAnnouncement'];
                $existingAnnouncementID = $row['WSGAnnouncementID'];
            }

            $pdo->beginTransaction();
            try {
                // Delete existing entries in both News and Events tables
                $pdo->prepare("DELETE FROM News WHERE NewsType = 1 AND Key = ?")->execute([$key]);
                $pdo->prepare("DELETE FROM Events WHERE EventKey = ?")->execute([$key]);

                // Insert new Event entry into News table
                $stmt = $pdo->prepare("
                    INSERT INTO News (Key, Published, Title, Subtitle, Comments, Credits, EventDate, NewsType, EntrySeqID, URLToGo, Expiration)
                    VALUES (:Key, :Published, :Title, :Subtitle, :Comments, :Credits, :EventDate, 1, :EntrySeqID, :URLToGo, :Expiration)
                ");
                $stmt->execute([
                    ':Key'        => $key,
                    ':Published'  => formatDatetime($_POST['Published']),
                    ':Title'      => $_POST['Title'],
                    ':Subtitle'   => $_POST['Subtitle'],
                    ':Comments'   => $_POST['Comments'],
                    ':Credits'    => $_POST['Credits'],
                    ':EventDate'  => formatDatetime($_POST['EventDate']),
                    ':EntrySeqID' => $_POST['EntrySeqID'],
                    ':URLToGo'    => $_POST['URLToGo'],
                    ':Expiration' => formatDatetime($_POST['Expiration'])
                ]);

                // Process Discord announcement if EventMeetDateTime is provided
                $wsgAnnouncementID = "";
                if (isset($_POST['EventMeetDateTime']) && !empty($_POST['EventMeetDateTime'])) {
                    if (isset($_POST['WSGAnnouncement'])) {
                        $newAnnouncement = trim($_POST['WSGAnnouncement']);
                        // Case 1: New announcement is blank
                        if ($newAnnouncement === "") {
                            // If there is an existing Discord post, delete it.
                            if (!empty($existingAnnouncementID)) {
                                $result = manageDiscordPost($disWHAnnouncements, "", $existingAnnouncementID, true);
                                $resultObj = json_decode($result, true);
                                if ($resultObj['result'] !== "success") {
                                    logMessage("Failed to delete Discord post with ID $existingAnnouncementID for event key $key due to blank announcement. Error: " . $resultObj['error']);
                                }
                            }
                            $wsgAnnouncementID = "";
                        }
                        // Case 2: New announcement is non-blank
                        else {
                            $discordPostID = "";
                            if (!empty($existingAnnouncementID)) {
                                // There is an existing Discord post.
                                if ($existingAnnouncement !== null && $existingAnnouncement === $newAnnouncement) {
                                    // Content unchanged; reuse the existing Discord post ID.
                                    $discordPostID = $existingAnnouncementID;
                                } else {
                                    // Content changed; update the Discord post.
                                    $result = manageDiscordPost($disWHAnnouncements, $newAnnouncement, $existingAnnouncementID, false);
                                    $resultObj = json_decode($result, true);
                                    if ($resultObj['result'] === "success") {
                                        $discordPostID = $resultObj['postID'];
                                    }
                                }
                            } else {
                                // No existing Discord post; create a new one.
                                $result = manageDiscordPost($disWHAnnouncements, $newAnnouncement, null, false);
                                $resultObj = json_decode($result, true);
                                if ($resultObj['result'] === "success") {
                                    $discordPostID = $resultObj['postID'];
                                }
                            }
                            $wsgAnnouncementID = $discordPostID;
                        }
                    }
                }

                // Insert new entry into Events table, including the Discord announcement info
                $stmt = $pdo->prepare("
                    INSERT INTO Events (
                        EventKey, EventMeetDateTime, UseEventSyncFly, SyncFlyDateTime, UseEventLaunch,
                        EventLaunchDateTime, UseEventStartTask, EventStartTaskDateTime, EventDescription, 
                        GroupEventTeaserEnabled, GroupEventTeaserMessage, GroupEventTeaserImage, VoiceChannel,
                        MSFSServer, TrackerGroup, EligibleAward, BeginnersGuide, Notam, Availability, Refly, WSGAnnouncement, WSGAnnouncementID
                    )
                    VALUES (
                        :EventKey, :EventMeetDateTime, :UseEventSyncFly, :SyncFlyDateTime, :UseEventLaunch,
                        :EventLaunchDateTime, :UseEventStartTask, :EventStartTaskDateTime, :EventDescription, 
                        :GroupEventTeaserEnabled, :GroupEventTeaserMessage, :GroupEventTeaserImage, :VoiceChannel,
                        :MSFSServer, :TrackerGroup, :EligibleAward, :BeginnersGuide, :Notam, :Availability, :Refly, :WSGAnnouncement, :WSGAnnouncementID
                    )
                ");

                $stmt->execute([
                    ':EventKey'                => $key,
                    ':EventMeetDateTime'       => formatDatetime($_POST['EventMeetDateTime']),
                    ':UseEventSyncFly'         => isset($_POST['UseEventSyncFly']) ? (int)$_POST['UseEventSyncFly'] : 0,
                    ':SyncFlyDateTime'         => formatDatetime($_POST['SyncFlyDateTime']),
                    ':UseEventLaunch'          => isset($_POST['UseEventLaunch']) ? (int)$_POST['UseEventLaunch'] : 0,
                    ':EventLaunchDateTime'     => formatDatetime($_POST['EventLaunchDateTime']),
                    ':UseEventStartTask'       => isset($_POST['UseEventStartTask']) ? (int)$_POST['UseEventStartTask'] : 0,
                    ':EventStartTaskDateTime'  => formatDatetime($_POST['EventStartTaskDateTime']),
                    ':EventDescription'        => $_POST['EventDescription'] ?? '',
                    ':GroupEventTeaserEnabled' => isset($_POST['GroupEventTeaserEnabled']) ? (int)$_POST['GroupEventTeaserEnabled'] : 0,
                    ':GroupEventTeaserMessage' => $_POST['GroupEventTeaserMessage'] ?? '',
                    ':GroupEventTeaserImage'   => isset($teaserImage) ? $teaserImage : null,
                    ':VoiceChannel'            => $_POST['VoiceChannel'] ?? '',
                    ':MSFSServer'              => $_POST['MSFSServer'] ?? '',
                    ':TrackerGroup'            => $_POST['TrackerGroup'] ?? '',
                    ':EligibleAward'           => $_POST['EligibleAward'] ?? '',
                    ':BeginnersGuide'          => $_POST['BeginnersGuide'] ?? '',
                    ':Notam'                   => $_POST['Notam'] ?? '',
                    ':Availability'            => formatDatetime($_POST['Availability']),
                    ':Refly'                   => $_POST['Refly'] ?? '',
                    ':WSGAnnouncement'         => $_POST['WSGAnnouncement'] ?? '',
                    ':WSGAnnouncementID'       => $wsgAnnouncementID
                ]);

                // ---- TaskEvents maintenance ----

                // Always clear any previous links for this EventKey (event can be recreated or re-assigned)
                $pdo->prepare("DELETE FROM TasksDB.TaskEvents WHERE EventKey = ?")->execute([$key]);

                // Only add a TaskEvents row if the event has a task assigned
                $entrySeqID = $_POST['EntrySeqID'] ?? null;
                if (!empty($entrySeqID)) {
                    // Extract ClubEventNewsID from EventKey: "E-<ClubEventNewsID><yyyyMMdd>"
                    $clubEventNewsID = '';
                    if (preg_match('/^E-(.+)(\d{8})$/', $key, $m)) {
                        $clubEventNewsID = $m[1];
                    } else {
                        // fallback: strip "E-" prefix and trailing 8-digit date if present
                        $clubEventNewsID = substr($key, 2, -8);
                    }

                    // Always use MeetDateTime for TaskEvents (date only)
                    $eventDTForLink = formatDatetime($_POST['EventMeetDateTime'] ?? '');
                    if (!empty($eventDTForLink)) {
                        // keep only the date (YYYY-MM-DD)
                        $eventDTForLink = substr($eventDTForLink, 0, 10);
                    }

                    if (!empty($eventDTForLink)) {
                        $stmtTE = $pdo->prepare('
                            INSERT INTO TasksDB.TaskEvents (EntrySeqID, ClubEventNewsID, EventDateTime, EventKey)
                            VALUES (:EntrySeqID, :ClubEventNewsID, :EventDateTime, :EventKey)
                        ');
                        $stmtTE->execute([
                            ':EntrySeqID'       => $entrySeqID,
                            ':ClubEventNewsID'  => $clubEventNewsID,
                            ':EventDateTime'    => $eventDTForLink,
                            ':EventKey'         => $key
                        ]);
                    }
                }
                // ---- end TaskEvents maintenance ----

                $pdo->commit();
            } catch (Throwable $e) {
                $pdo->rollBack();
                throw $e;
            }

            logMessage("Created Event News entry: $key");
            break;

        case 'DeleteEvent':
            if (!isset($_POST['Key'])) {
                throw new Exception('Key missing.');
            }
            $key = $_POST['Key'];

            // Retrieve existing Discord post ID from Events table, if any
            $discordPostID = "";
            $stmt = $pdo->prepare("SELECT WSGAnnouncementID FROM Events WHERE EventKey = ?");
            $stmt->execute([$key]);
            if ($row = $stmt->fetch(PDO::FETCH_ASSOC)) {
                $discordPostID = $row['WSGAnnouncementID'];
            }

            // If a Discord post ID exists, attempt to delete the Discord post
            if (!empty($discordPostID)) {
                $result = manageDiscordPost($disWHAnnouncements, "", $discordPostID, true);
                $resultObj = json_decode($result, true);
                if ($resultObj['result'] !== "success") {
                    logMessage("Failed to delete Discord post with ID $discordPostID: " . $resultObj['error']);
                } else {
                    logMessage("Deleted Discord post with ID $discordPostID successfully.");
                }
            }

            // Delete existing Event entry from News
            $stmtNews = $pdo->prepare("DELETE FROM News WHERE NewsType = 1 AND Key = ?");
            $stmtNews->execute([$key]);
            $affectedNewsRows = $stmtNews->rowCount();

            // Delete corresponding Event entry from Events
            $stmtEvent = $pdo->prepare("DELETE FROM Events WHERE EventKey = ?");
            $stmtEvent->execute([$key]);
            $affectedEventRows = $stmtEvent->rowCount();

            // Delete TaskEvents link(s) for this event
            $pdo->prepare("DELETE FROM TasksDB.TaskEvents WHERE EventKey = ?")->execute([$key]);

            // after both deletes
            if ($affectedNewsRows === 0 && $affectedEventRows === 0) {
                throw new Exception('No event found with the specified Key.');
            }
            logMessage("Deleted Event News entry: $key");
            break;

        case 'CreateNews':
            if (!isset($_POST['Key'])) {
                throw new Exception('Key missing.');
            }
            $key = $_POST['Key'];

            // Delete existing News entry
            $pdo->prepare("DELETE FROM News WHERE NewsType = 2 AND Key = ?")->execute([$key]);

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
            logMessage("Created General News entry: $key");
            break;

        case 'DeleteNews':
            if (!isset($_POST['Key'])) {
                throw new Exception('Key missing.');
            }
            $key = $_POST['Key'];

            // Delete existing News entry
            $stmt = $pdo->prepare("DELETE FROM News WHERE NewsType = 2 AND Key = ?");
            $stmt->execute([$key]);
            $affectedRows = $stmt->rowCount();

            if ($affectedRows === 0) {
                throw new Exception('No news entry found with the specified Key.');
            }
            logMessage("Deleted General News entry: $key");
            break;

        default:
            throw new Exception('Invalid action specified.');
    }

    echo json_encode(['status' => 'success', 'message' => 'Action processed successfully.']);

} catch (Exception $e) {
    logMessage("Error: " . $e->getMessage());
    echo json_encode(['status' => 'error', 'message' => $e->getMessage()]);
    logMessage("--- End of script ManageNews ---");
}
?>
