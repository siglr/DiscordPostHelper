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

    // 1) Pull candidate events (unsent & available)
    $candStmt = $pdoNews->prepare("
        SELECT EventKey, Availability
        FROM Events
        WHERE (SentToTrackerDateTime IS NULL OR TRIM(SentToTrackerDateTime) = '')
          AND Availability <= :nowUTC
    ");
    $candStmt->execute([':nowUTC' => $nowUTC]);
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
                logMessage("Cron: skip '$eventKey' (no Clubs.TrackerGroup for '$eid').");
                continue;
            }
            if ($secret === '') {
                logMessage("Cron: skip '$eventKey' (no Clubs.TrackerSecret for '$eid').");
                continue; // do not consider events with missing secret
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
