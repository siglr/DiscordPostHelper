<?php
require_once __DIR__ . '/CommonFunctions.php';
require_once __DIR__ . '/TrackerTask.php';
require_once __DIR__ . '/SetTrackerGroup.php';

try {
    // Calculate the current UTC time once
    $nowUTC = (new DateTime('now', new DateTimeZone('UTC')))->format('Y-m-d H:i:s');

    /*
     * Update Tasks Routine:
     * Update Discord posts for tasks whose AvailabilityPostContent is set and whose availability has passed.
     */
    $pdoTasks = new PDO("sqlite:$databasePath");
    $pdoTasks->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);

    $queryTasks = "SELECT EntrySeqID, DiscordPostID, NormalPostContent, Availability 
                   FROM Tasks 
                   WHERE TRIM(AvailabilityPostContent) <> '' 
                     AND Availability <= :nowUTC";
    $stmtTasks = $pdoTasks->prepare($queryTasks);
    $stmtTasks->execute([':nowUTC' => $nowUTC]);
    $tasks = $stmtTasks->fetchAll(PDO::FETCH_ASSOC);

    if ($tasks) {
        foreach ($tasks as $task) {
            logMessage("Availability: " . $task['Availability'] . " RightNow: " . $nowUTC);

            $entrySeqID    = $task['EntrySeqID'];
            $discordPostID = $task['DiscordPostID'];
            $normalContent = $task['NormalPostContent'];

            // Update Discord post if both DiscordPostID and NormalPostContent are provided.
            if (!empty($discordPostID) && !empty($normalContent)) {
                if (strpos($normalContent, "Don't forget to upload your IGC log file") === false) {
                    $reminder = "*Don't forget to upload your IGC log file to WeSimGlide.org after flying this task!*";

                    if (preg_match('/\[Task Cover\]/', $normalContent)) {
                        // insert reminder with a blank line before and after
                        $normalContent = preg_replace(
                            '/(\[Task Cover\].*)/s',
                            "\n{$reminder}\n\n\$1",
                            $normalContent,
                            1
                        );
                    } else {
                        // no cover: append with a blank line before
                        $normalContent .= "\n\n{$reminder}";
                    }
                }
                $discordResult = manageDiscordPost($disWHFlights, $normalContent, $discordPostID, false);
                $discordResponse = json_decode($discordResult, true);
                if ($discordResponse['result'] !== "success") {
                    logMessage("Error updating Discord post for task EntrySeqID $entrySeqID: " . $discordResponse['error']);
                } else {
                    logMessage("Discord post updated successfully for task EntrySeqID $entrySeqID.");
                }
            } else {
                logMessage("Error: Task EntrySeqID $entrySeqID missing DiscordPostID or NormalPostContent.");
            }

            // Clear the AvailabilityPostContent field regardless of Discord update outcome.
            $updateStmt = $pdoTasks->prepare("UPDATE Tasks SET AvailabilityPostContent = '' WHERE EntrySeqID = :EntrySeqID");
            if (!$updateStmt->execute([':EntrySeqID' => $entrySeqID])) {
                logMessage("Error: Failed to clear AvailabilityPostContent for task EntrySeqID: " . $entrySeqID);
            }
        }
    }

    /*
     * Process queued task deletions
     */
    $pendingDeletesStmt = $pdoTasks->prepare("SELECT EntrySeqID, UserID FROM DeletedTasks WHERE Completed = 0 ORDER BY DeletionDate ASC, EntrySeqID ASC");
    $pendingDeletesStmt->execute();
    $pendingDeletes = $pendingDeletesStmt->fetchAll(PDO::FETCH_ASSOC);

    if ($pendingDeletes) {
        $fetchTaskStmt = $pdoTasks->prepare("SELECT TaskID, DiscordPostID FROM Tasks WHERE EntrySeqID = :EntrySeqID LIMIT 1");
        $deleteTaskStmt = $pdoTasks->prepare("DELETE FROM Tasks WHERE EntrySeqID = :EntrySeqID");
        $markCompletedStmt = $pdoTasks->prepare("UPDATE DeletedTasks SET Completed = 1 WHERE EntrySeqID = :EntrySeqID");
        $fetchGuardStmt = $pdoTasks->prepare("SELECT Enabled FROM GuardFlags WHERE Name = :name LIMIT 1");
        $updateGuardStmt = $pdoTasks->prepare("UPDATE GuardFlags SET Enabled = :enabled WHERE Name = :name");

        foreach ($pendingDeletes as $deleteRequest) {
            $entrySeqID = (int)$deleteRequest['EntrySeqID'];
            $requestUser = isset($deleteRequest['UserID']) ? $deleteRequest['UserID'] : '';
            logMessage("Cron: Processing queued deletion for EntrySeqID {$entrySeqID} (requested by {$requestUser}).");

            $fetchTaskStmt->execute([':EntrySeqID' => $entrySeqID]);
            $taskRow = $fetchTaskStmt->fetch(PDO::FETCH_ASSOC);

            if (!$taskRow) {
                logMessage("Cron: Task EntrySeqID {$entrySeqID} not found. Marking deletion request as completed.");
                $markCompletedStmt->execute([':EntrySeqID' => $entrySeqID]);
                continue;
            }

            $taskID = $taskRow['TaskID'];
            $discordPostID = $taskRow['DiscordPostID'];
            $fatalError = false;
            $restoreGuard = false;
            $originalGuardState = 0;

            try {
                $fetchGuardStmt->execute([':name' => 'AdminDelete']);
                $guardRow = $fetchGuardStmt->fetch(PDO::FETCH_ASSOC);

                if (!$guardRow) {
                    throw new Exception('AdminDelete guard flag not found.');
                }

                $originalGuardState = (int)$guardRow['Enabled'];

                if ($originalGuardState !== 1) {
                    if (!$updateGuardStmt->execute([':enabled' => 1, ':name' => 'AdminDelete'])) {
                        throw new Exception('Failed to enable AdminDelete guard flag.');
                    }
                    $restoreGuard = true;
                } else {
                    $restoreGuard = true; // ensure we restore to the original state later even if already enabled
                }

                if (!$deleteTaskStmt->execute([':EntrySeqID' => $entrySeqID]) || $deleteTaskStmt->rowCount() === 0) {
                    throw new Exception('Deleting task from Tasks table failed.');
                }
            } catch (Exception $ex) {
                $fatalError = true;
                logMessage("Cron: Failed to delete task EntrySeqID {$entrySeqID}: " . $ex->getMessage());
            } finally {
                if ($restoreGuard) {
                    $updateGuardStmt->execute([':enabled' => $originalGuardState, ':name' => 'AdminDelete']);
                }
            }

            if ($fatalError) {
                logMessage("Cron: Leaving deletion request pending for EntrySeqID {$entrySeqID} due to fatal error.");
                continue;
            }

            // Delete associated news entries
            try {
                deleteTaskNewsEntries($taskID);
            } catch (Exception $ex) {
                logMessage("Cron: Failed to delete news entries for TaskID {$taskID}: " . $ex->getMessage());
            }

            // Delete DPHX file
            $dphxDir = $fileRootPath . 'TaskBrowser/Tasks/';
            $dphxFile = $dphxDir . basename($taskID . '.dphx');
            if (file_exists($dphxFile)) {
                if (!unlink($dphxFile)) {
                    logMessage("Cron: Failed to delete DPHX file {$dphxFile}.");
                } else {
                    logMessage("Cron: Deleted DPHX file {$dphxFile}.");
                }
            } else {
                logMessage("Cron: DPHX file not found {$dphxFile}.");
            }

            // Delete weather chart
            $weatherDir = $fileRootPath . 'TaskBrowser/WeatherCharts/';
            $weatherFile = $weatherDir . basename($entrySeqID . '.jpg');
            if (file_exists($weatherFile)) {
                if (!unlink($weatherFile)) {
                    logMessage("Cron: Failed to delete weather chart {$weatherFile}.");
                } else {
                    logMessage("Cron: Deleted weather chart {$weatherFile}.");
                }
            } else {
                logMessage("Cron: Weather chart not found {$weatherFile}.");
            }

            // Delete cover image
            $coverDir = $fileRootPath . 'TaskBrowser/Covers/';
            $coverFile = $coverDir . basename($entrySeqID . '.jpg');
            if (file_exists($coverFile)) {
                if (!unlink($coverFile)) {
                    logMessage("Cron: Failed to delete cover image {$coverFile}.");
                } else {
                    logMessage("Cron: Deleted cover image {$coverFile}.");
                }
            } else {
                logMessage("Cron: Cover image not found {$coverFile}.");
            }

            // Delete Discord post, if any
            if (!empty($discordPostID)) {
                $discordResult = manageDiscordPost($disWHFlights, "", $discordPostID, true);
                $discordResponse = json_decode($discordResult, true);

                if (!isset($discordResponse['result']) || $discordResponse['result'] !== 'success') {
                    $discordError = isset($discordResponse['error']) ? $discordResponse['error'] : 'Unknown error';
                    logMessage("Cron: Failed to delete Discord post {$discordPostID}. Error: {$discordError}");
                } else {
                    logMessage("Cron: Deleted Discord post {$discordPostID}.");
                }
            }

            $markCompletedStmt->execute([':EntrySeqID' => $entrySeqID]);
            logMessage("Cron: Completed queued deletion for EntrySeqID {$entrySeqID}.");
        }
    }

    /*
     * Update Announcements Routine:
     * Modify the WSGAnnouncement in the News database for events that need to be updated on Discord.
     */
    $pdoNews = new PDO("sqlite:$newsDBPath");
    $pdoNews->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);

    $queryNews = "
        SELECT 
            Events.EventKey, Events.Availability, Events.WSGAnnouncementID, Events.WSGAnnouncement, News.EntrySeqID
        FROM Events
        JOIN News ON Events.EventKey = News.Key
        WHERE WSGAnnouncement LIKE '%Please monitor the event as task has been set for later availability%'
          AND Availability <= :nowUTC
    ";
    $stmtNews = $pdoNews->prepare($queryNews);
    $stmtNews->execute([':nowUTC' => $nowUTC]);
    $entries = $stmtNews->fetchAll(PDO::FETCH_ASSOC);

    if ($entries) {
        foreach ($entries as $entry) {
            $eventKey         = $entry['EventKey'];
            $wsgAnnouncementID = $entry['WSGAnnouncementID'];
            $oldAnnouncement  = $entry['WSGAnnouncement'];
            $entrySeqID       = $entry['EntrySeqID'];

            // Build the new announcement line dynamically.
            $newAnnouncementLine = "### <:wsg:1296813102893105203> [Task #{$entrySeqID}](<{$wsgRoot}/index.html?task={$entrySeqID}>)";

            // Replace the last line of the announcement.
            $lines = explode("\n", $oldAnnouncement);
            $lastLine = array_pop($lines);
            if (strpos($lastLine, "Please monitor the event as task has been set for later availability") !== false) {
                $lines[] = $newAnnouncementLine;
            } else {
                $lines[] = $lastLine; // keep original
                $lines[] = $newAnnouncementLine; // append new line
            }
            $newAnnouncement = implode("\n", $lines);

            // Update the Discord announcement using the announcements webhook.
            $discordResult = manageDiscordPost($disWHAnnouncements, $newAnnouncement, $wsgAnnouncementID, false);
            $discordResponse = json_decode($discordResult, true);
            if ($discordResponse['result'] !== "success") {
                logMessage("Error updating Discord announcement for EventKey {$eventKey}: " . $discordResponse['error']);
            } else {
                logMessage("Discord announcement updated successfully for EventKey {$eventKey}.");

                // Update the Events table with the new announcement.
                $updateStmt = $pdoNews->prepare("UPDATE Events SET WSGAnnouncement = :wsgAnnouncement WHERE EventKey = :eventKey");
                if (!$updateStmt->execute([':wsgAnnouncement' => $newAnnouncement, ':eventKey' => $eventKey])) {
                    logMessage("Error: Failed to update WSGAnnouncement for EventKey {$eventKey}.");
                }
            }
        }
    }

    // ─────────────────────────────────────────────
    // Push available, next-up events to Tracker
    // ─────────────────────────────────────────────
    $nowPlus24 = (new DateTime('now', new DateTimeZone('UTC')))
        ->add(new DateInterval('PT24H'))
        ->format('Y-m-d H:i:s');

    // 1) Pull candidate events (unsent & available)
    $candStmt = $pdoNews->prepare("
        SELECT EventKey, Availability, EventMeetDateTime
        FROM Events
        WHERE (SentToTrackerDateTime IS NULL OR TRIM(SentToTrackerDateTime) = '')
          AND Availability <= :nowUTC
          AND (
                EventMeetDateTime IS NULL
                OR EventMeetDateTime <= :nowPlus24
              )
    ");
    $candStmt->execute([
        ':nowUTC'    => $nowUTC,
        ':nowPlus24' => $nowPlus24,
    ]);
    $candidates = $candStmt->fetchAll(PDO::FETCH_ASSOC);

    if ($candidates) {
        // 2) Group by TrackerGroup (via EventNewsID → Clubs)
        $byGroup = [];              // [TrackerGroup] => [ [eventKey, availability], ... ]
        $clubCache = [];            // [EventNewsID] => TrackerGroup (cache to reduce queries)

        $stmtClubLookup = $pdoTasks->prepare("
            SELECT TrackerGroup, TrackerSecret
            FROM Clubs
            WHERE EventNewsID = :eid
            LIMIT 1
        ");

        foreach ($candidates as $row) {
            $eventKey = (string)$row['EventKey'];
            $eid = extractEventNewsId($eventKey);
            if (!$eid) { logMessage("Cron: skip '$eventKey' (invalid EventNewsID)"); continue; }

            if (!array_key_exists($eid, $clubCache)) {
                $stmtClubLookup->execute([':eid' => $eid]);
                $clubRow = $stmtClubLookup->fetch(PDO::FETCH_ASSOC);
                $clubCache[$eid] = [
                    'group'  => $clubRow ? trim((string)$clubRow['TrackerGroup'])  : '',
                    'secret' => $clubRow ? trim((string)$clubRow['TrackerSecret']) : '',
                ];
            }
            $group  = $clubCache[$eid]['group'];
            $secret = $clubCache[$eid]['secret'];

            if ($group === '') {
                continue;
            }
            if ($secret === '') {
                continue; 
            }

            $byGroup[$group][] = [
                'eventKey'     => $eventKey,
                'availability' => (string)$row['Availability'],
            ];
        }
        // 3) Keep earliest (min Availability) per group
        $toPush = [];  // list of eventKey to push
        foreach ($byGroup as $group => $items) {
            usort($items, function($a, $b) {
                $cmp = strcmp($a['availability'], $b['availability']);
                return $cmp !== 0 ? $cmp : strcmp($a['eventKey'], $b['eventKey']);
            });
            // earliest is first
            if (!empty($items)) {
                $toPush[] = $items[0]['eventKey'];
            }
        }

        // 4) Push each selected event and stamp SentToTrackerDateTime on success
        if ($toPush) {
            $updStmt = $pdoNews->prepare("
                UPDATE Events
                SET SentToTrackerDateTime = :nowUTC
                WHERE EventKey = :eventKey
                  AND (SentToTrackerDateTime IS NULL OR TRIM(SentToTrackerDateTime) = '')
            ");

            foreach ($toPush as $eventKey) {
                try {
                    dbg("Cron: pushing EventKey $eventKey to Tracker...");
                    $res = runTrackerPushForKey($eventKey);   // uses trackerSetSharedTask internally
                    if (!empty($res['ok'])) {
                        $updStmt->execute([':nowUTC' => $nowUTC, ':eventKey' => $eventKey]);
                        logMessage("Cron: Tracker push OK and stamped SentToTrackerDateTime for $eventKey.");
                    } else {
                        logMessage("Cron: Tracker push FAILED for $eventKey [HTTP {$res['status']}] {$res['body']} {$res['error']}");
                    }
                } catch (Throwable $e) {
                    logMessage("Cron: Exception while pushing $eventKey → " . $e->getMessage());
                }
            }
        }
    }

} catch (Exception $e) {
    logMessage("Error in EveryMinuteCronJob: " . $e->getMessage());
}
?>
