<?php
require __DIR__ . '/CommonFunctions.php';

try {
    // Open database connections
    $pdoNews = new PDO("sqlite:$newsDBPath");
    $pdoNews->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);

    $pdoTasks = new PDO("sqlite:$databasePath");
    $pdoTasks->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);

    // Call cleanup function
    cleanUpNewsEntries($pdoNews);

    // Get the optional fullText parameter
    $fullText = isset($_GET['fullText']) ? filter_var($_GET['fullText'], FILTER_VALIDATE_BOOLEAN) : false;

    // Query to fetch news entries with events info (if applicable)
    $stmtNews = $pdoNews->query("
        SELECT 
            News.Key, Published, Title, Subtitle, Comments, Credits, EventDate, News, NewsType, EntrySeqID, URLToGo, Expiration,
            Events.Availability AS EventAvailability, Events.Refly
        FROM News
        LEFT JOIN Events ON News.Key = Events.EventKey
        ORDER BY NewsType DESC, EventDate ASC, Published DESC
    ");
    $newsEntries = $stmtNews->fetchAll(PDO::FETCH_ASSOC);

    // Function to truncate text fields
    function truncate($text, $length = 75) {
        return strlen($text) > $length ? substr($text, 0, $length) . '...' : $text;
    }

    $filteredNews = [];

    foreach ($newsEntries as &$entry) {
        // Get current UTC time
        $nowUTC = (new DateTime('now', new DateTimeZone('UTC')))->format('Y-m-d H:i:s');

        // Handle Event News (NewsType = 1)
        if ($entry['NewsType'] == 1) {
            if ($entry['Refly'] == 1) {
                // If Refly = 1, remove EntrySeqID
                $entry['EntrySeqID'] = null;
            }
        }

        // Handle Task News (NewsType = 0)
        if ($entry['NewsType'] == 0 && !empty($entry['EntrySeqID'])) {
            // Check the task availability
            $stmtTask = $pdoTasks->prepare("
                SELECT Availability FROM Tasks WHERE EntrySeqID = :entrySeqID
            ");
            $stmtTask->execute([':entrySeqID' => $entry['EntrySeqID']]);
            $taskAvailability = $stmtTask->fetchColumn();

            if ($taskAvailability && $taskAvailability > $nowUTC) {
                // Skip this news entry if the task is not available
                continue;
            }
        }

        // Truncate text fields if fullText is false
        if (!$fullText) {
            foreach ($entry as $key => $value) {
                if (is_string($value) && $key !== 'URLToGo') {  // Skip truncation for 'URLToGo' field
                    $entry[$key] = truncate($value);
                }
            }
        }

        $filteredNews[] = $entry;
    }

    // Return the filtered news entries as JSON
    echo json_encode(['status' => 'success', 'data' => $filteredNews]);

} catch (Exception $e) {
    logMessage("--- Script running RetrieveNews ---");
    logMessage("Error: " . $e->getMessage());
    echo json_encode(['status' => 'error', 'message' => $e->getMessage()]);
    logMessage("--- End of script RetrieveNews ---");
}
?>
