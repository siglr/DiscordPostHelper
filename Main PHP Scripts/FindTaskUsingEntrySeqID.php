<?php
require __DIR__ . '/CommonFunctions.php';

try {
    // Open the database connection
    $pdo = new PDO("sqlite:$databasePath");
    $pdo->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);

    // Check if EntrySeqID parameter is set
    if (!isset($_GET['EntrySeqID'])) {
        throw new Exception('EntrySeqID is missing');
    }

    $entrySeqID = $_GET['EntrySeqID'];

    // Check if OnlyAvailable is present, default to 0 if not provided
    $onlyAvailable = isset($_GET['OnlyAvailable']) ? (int)$_GET['OnlyAvailable'] : 0;

    // Prepare the SQL statement
    $stmt = $pdo->prepare("SELECT TaskID, Title, EntrySeqID, LastUpdate, DBEntryUpdate, Status, OwnerName, SharedWith, Availability, DiscordPostID, EXISTS(SELECT 1 FROM IGCRecords r WHERE r.EntrySeqID = Tasks.EntrySeqID) AS HasLeaderBoard FROM Tasks WHERE EntrySeqID = :EntrySeqID");
    $stmt->execute([':EntrySeqID' => $entrySeqID]);
    $taskDetails = $stmt->fetch(PDO::FETCH_ASSOC);
    if ($taskDetails) { $taskDetails['HasLeaderBoard'] = (bool)$taskDetails['HasLeaderBoard']; }

    if ($taskDetails) {
        if ($onlyAvailable === 1) { // Ensure OnlyAvailable is treated as a boolean flag
            // Get the current UTC timestamp
            $nowUTC = (new DateTime('now', new DateTimeZone('UTC')))->getTimestamp();
            // Convert Availability to UTC timestamp
            $availabilityTimestamp = !empty($taskDetails['Availability']) 
                ? DateTime::createFromFormat('Y-m-d H:i:s', $taskDetails['Availability'], new DateTimeZone('UTC'))->getTimestamp()
                : null;
            // Check if the task is unavailable due to the Availability date
            if ($availabilityTimestamp !== null && $availabilityTimestamp > $nowUTC) {
                header('Content-Type: application/json');
                echo json_encode([
                    'status' => 'unavailable',
                    'message' => 'Task is not available yet.',
                    'availability' => $taskDetails['Availability'] 
                ]);
                exit;
            }
        }
        echo json_encode(['status' => 'success', 'taskDetails' => $taskDetails]);
    } else {
        echo json_encode(['status' => 'error', 'message' => 'Task not found.']);
    }

} catch (PDOException $e) {
    logMessage("Connection failed: " . $e->getMessage());
    echo json_encode(['status' => 'error', 'message' => 'Connection failed: ' . $e->getMessage()]);
    logMessage("--- End of script FetchTaskDetails ---");
} catch (Exception $e) {
    logMessage("Error: " . $e->getMessage());
    echo json_encode(['status' => 'error', 'message' => $e->getMessage()]);
    logMessage("--- End of script FetchTaskDetails ---");
}
?>
