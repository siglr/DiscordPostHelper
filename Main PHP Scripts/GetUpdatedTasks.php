<?php
require __DIR__ . '/CommonFunctions.php';

try {
    // Open the database connection
    $pdo = new PDO("sqlite:$databasePath");
    $pdo->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);

    // Retrieve the DBEntryUpdate parameter from the query string
    if (isset($_GET['DBEntryUpdate']) && !empty($_GET['DBEntryUpdate'])) {
        $dbEntryUpdate = $_GET['DBEntryUpdate'];

        // Compute current UTC time in PHP
        $nowUTC = (new DateTime('now', new DateTimeZone('UTC')))->format('Y-m-d H:i:s');

        // Define the query to retrieve the records with DBEntryUpdate greater than the provided parameter
        $query = "
            SELECT 
                EntrySeqID, TaskID, Title, LastUpdate, SimDateTime, IncludeYear, SimDateTimeExtraInfo,
                MainAreaPOI, DepartureName, DepartureICAO, DepartureExtra, ArrivalName,
                ArrivalICAO, ArrivalExtra, SoaringRidge, SoaringThermals, SoaringWaves,
                SoaringDynamic, SoaringExtraInfo, COALESCE(NULLIF(DurationMin, ''), 0) as DurationMin, COALESCE(NULLIF(DurationMax, ''), 0) as DurationMax, DurationExtraInfo,
                TaskDistance, TotalDistance, RecommendedGliders, DifficultyRating, DifficultyExtraInfo,
                ShortDescription, LongDescription, WeatherSummary, Credits, Countries,
                RecommendedAddOns, MapImage, CoverImage, DBEntryUpdate, RepostText, LastUpdateDescription, DiscordPostID
            FROM Tasks
            WHERE 
                (DBEntryUpdate > :dbEntryUpdate 
                 AND (Availability IS NULL OR Availability <= :nowUTC))
                 OR 
                (Availability IS NOT NULL 
                 AND Availability > :dbEntryUpdate 
                 AND Availability <= :nowUTC)
        ";

        // Prepare and execute the query
        $stmt = $pdo->prepare($query);
        $stmt->bindParam(':dbEntryUpdate', $dbEntryUpdate, PDO::PARAM_STR);
        $stmt->bindParam(':nowUTC', $nowUTC, PDO::PARAM_STR);
        $stmt->execute();
        $tasks = $stmt->fetchAll(PDO::FETCH_ASSOC);

        // Encode blob images in base64
        foreach ($tasks as &$task) {
            if (!empty($task['MapImage'])) {
                $task['MapImage'] = base64_encode($task['MapImage']);
            }
            if (!empty($task['CoverImage'])) {
                $task['CoverImage'] = base64_encode($task['CoverImage']);
            }
        }

        // Output the results as JSON
        header('Content-Type: application/json');
        echo json_encode($tasks);
    } else {
        // Handle missing or empty DBEntryUpdate parameter
        logMessage("--- Script running GetUpdatedTasks ---");
        echo json_encode([
            'error' => 'Missing or empty DBEntryUpdate parameter'
        ]);
        logMessage("--- End of script GetUpdatedTasks ---");
    }

} catch (PDOException $e) {
    logMessage("--- Script running GetUpdatedTasks ---");
    echo json_encode([
        'error' => 'Connection failed: ' . $e->getMessage()
    ]);
    logMessage("--- End of script GetUpdatedTasks ---");
}
?>
