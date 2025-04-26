<?php
require __DIR__ . '/CommonFunctions.php';

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

} catch (Exception $e) {
    logMessage("Error in EveryMinuteCronJob: " . $e->getMessage());
}
?>
