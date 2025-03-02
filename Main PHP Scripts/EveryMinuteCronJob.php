<?php
require __DIR__ . '/CommonFunctions.php';

try {
    // Open the database connection
    $pdo = new PDO("sqlite:$databasePath");
    $pdo->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);

    // Query tasks that need to be updated:
    // Those with non-empty AvailabilityPostContent and with Availability <= current UTC time.
    $nowUTC = (new DateTime('now', new DateTimeZone('UTC')))->format('Y-m-d H:i:s');    

    // Use the computed time as a parameter in your query
    $query = "SELECT EntrySeqID, DiscordPostID, NormalPostContent, Availability 
              FROM Tasks 
              WHERE TRIM(AvailabilityPostContent) <> '' 
                AND Availability <= :nowUTC";
    $stmt = $pdo->prepare($query);
    $stmt->execute([':nowUTC' => $nowUTC]);
    $tasks = $stmt->fetchAll(PDO::FETCH_ASSOC);

    if ($tasks) {
        foreach ($tasks as $task) {
            logMessage("Availability: " . $task['Availability'] . " RightNow: " . $nowUTC);

            $entrySeqID = $task['EntrySeqID'];
            $discordPostID = $task['DiscordPostID'];
            $normalContent = $task['NormalPostContent'];

            // Update the Discord post if both DiscordPostID and NormalPostContent are provided.
            if (!empty($discordPostID) && !empty($normalContent)) {
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
            $updateStmt = $pdo->prepare("UPDATE Tasks SET AvailabilityPostContent = '' WHERE EntrySeqID = :EntrySeqID");
            if (!$updateStmt->execute([':EntrySeqID' => $entrySeqID])) {
                logMessage("Error: Failed to clear AvailabilityPostContent for task EntrySeqID: " . $entrySeqID);
            }
        }
    }
} catch (Exception $e) {
    logMessage("Error in EveryMinuteCronJob: " . $e->getMessage());
}
?>
