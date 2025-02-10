<?php
require __DIR__ . '/CommonFunctions.php';

try {
    // Open the database connection
    $pdo = new PDO("sqlite:$databasePath");
    $pdo->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);

    // Retrieve the DBEntryUpdate parameter from the query string
    if (isset($_GET['DBEntryUpdate']) && !empty($_GET['DBEntryUpdate'])) {
        $dbEntryUpdate = $_GET['DBEntryUpdate'];

        // Define the query to retrieve the records with DBEntryUpdate greater than the provided parameter
        $query = "
                    SELECT 
                        EntrySeqID, TaskID, Title, LastUpdate, SimDateTime, IncludeYear, SimDateTimeExtraInfo,
                        MainAreaPOI, DepartureName, DepartureICAO, DepartureExtra, ArrivalName,
                        ArrivalICAO, ArrivalExtra, SoaringRidge, SoaringThermals, SoaringWaves,
                        SoaringDynamic, SoaringExtraInfo, DurationMin, DurationMax, DurationExtraInfo,
                        TaskDistance, TotalDistance, RecommendedGliders, DifficultyRating, DifficultyExtraInfo,
                        ShortDescription, LongDescription, WeatherSummary, Credits, Countries,
                        RecommendedAddOns, MapImage, CoverImage, DBEntryUpdate, RepostText, LastUpdateDescription
                    FROM Tasks
                    WHERE 
                        -- 1. Tasks that have been updated and are available
                        (DBEntryUpdate > :dbEntryUpdate 
                        AND (Availability IS NULL OR Availability <= datetime('now', 'utc')))
                        OR 
                        -- 2. Tasks that have become available since last update
                        (Availability IS NOT NULL 
                        AND Availability > :dbEntryUpdate 
                        AND Availability <= datetime('now', 'utc'))
        ";

        // Prepare and execute the query
        $stmt = $pdo->prepare($query);
        $stmt->bindParam(':dbEntryUpdate', $dbEntryUpdate, PDO::PARAM_STR);
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
