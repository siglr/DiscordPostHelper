<?php
require __DIR__ . '/CommonFunctions.php';

try {
    // Open the database connection
    $pdo = new PDO("sqlite:$databasePath");
    $pdo->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);
    logMessage("Database connection established.");

    // Get the JSON input
    $json = file_get_contents('php://input');
    $tasks = json_decode($json, true);
    logMessage("JSON input received and decoded.");

    // Prepare the insert and update statements
    $insertStmt = $pdo->prepare("
        INSERT INTO Tasks (
            TaskID, Title, LastUpdate, SimDateTime, IncludeYear, SimDateTimeExtraInfo,
            MainAreaPOI, DepartureName, DepartureICAO, DepartureExtra, ArrivalName,
            ArrivalICAO, ArrivalExtra, SoaringRidge, SoaringThermals, SoaringWaves,
            SoaringDynamic, SoaringExtraInfo, DurationMin, DurationMax, DurationExtraInfo,
            TaskDistance, TotalDistance, RecommendedGliders, DifficultyRating, DifficultyExtraInfo,
            ShortDescription, LongDescription, WeatherSummary, Credits, Countries,
            RecommendedAddOns, MapImage, CoverImage, TotDownloads, LastDownloadUpdate, DBEntryUpdate
        ) VALUES (
            :TaskID, :Title, :LastUpdate, :SimDateTime, :IncludeYear, :SimDateTimeExtraInfo,
            :MainAreaPOI, :DepartureName, :DepartureICAO, :DepartureExtra, :ArrivalName,
            :ArrivalICAO, :ArrivalExtra, :SoaringRidge, :SoaringThermals, :SoaringWaves,
            :SoaringDynamic, :SoaringExtraInfo, :DurationMin, :DurationMax, :DurationExtraInfo,
            :TaskDistance, :TotalDistance, :RecommendedGliders, :DifficultyRating, :DifficultyExtraInfo,
            :ShortDescription, :LongDescription, :WeatherSummary, :Credits, :Countries,
            :RecommendedAddOns, :MapImage, :CoverImage, :TotDownloads, :LastDownloadUpdate, :DBEntryUpdate
        )
    ");
    logMessage("Insert statement prepared.");

    $updateStmt = $pdo->prepare("
        UPDATE Tasks SET
            TaskID = :TaskID, Title = :Title, LastUpdate = :LastUpdate, SimDateTime = :SimDateTime,
            IncludeYear = :IncludeYear, SimDateTimeExtraInfo = :SimDateTimeExtraInfo, MainAreaPOI = :MainAreaPOI,
            DepartureName = :DepartureName, DepartureICAO = :DepartureICAO, DepartureExtra = :DepartureExtra,
            ArrivalName = :ArrivalName, ArrivalICAO = :ArrivalICAO, ArrivalExtra = :ArrivalExtra, SoaringRidge = :SoaringRidge,
            SoaringThermals = :SoaringThermals, SoaringWaves = :SoaringWaves, SoaringDynamic = :SoaringDynamic,
            SoaringExtraInfo = :SoaringExtraInfo, DurationMin = :DurationMin, DurationMax = :DurationMax, DurationExtraInfo = :DurationExtraInfo,
            TaskDistance = :TaskDistance, TotalDistance = :TotalDistance, RecommendedGliders = :RecommendedGliders, DifficultyRating = :DifficultyRating,
            DifficultyExtraInfo = :DifficultyExtraInfo, ShortDescription = :ShortDescription, LongDescription = :LongDescription,
            WeatherSummary = :WeatherSummary, Credits = :Credits, Countries = :Countries, RecommendedAddOns = :RecommendedAddOns,
            MapImage = :MapImage, CoverImage = :CoverImage, DBEntryUpdate = :DBEntryUpdate
        WHERE EntrySeqID = :EntrySeqID
    ");
    logMessage("Update statement prepared.");

    foreach ($tasks as $task) {
        try {
            $mapImage = isset($task['MapImage']) && !empty($task['MapImage']) ? base64_decode($task['MapImage']) : null;
            $coverImage = isset($task['CoverImage']) && !empty($task['CoverImage']) ? base64_decode($task['CoverImage']) : null;

            // Ensure datetime fields are properly formatted
            $task['LastUpdate'] = formatDatetime($task['LastUpdate']);
            $task['SimDateTime'] = formatDatetime($task['SimDateTime']);
            $task['LastDownloadUpdate'] = formatDatetime($task['LastDownloadUpdate']);
            $task['DBEntryUpdate'] = formatDatetime($task['DBEntryUpdate']);

            if ($task['EntrySeqID'] == 0) {
                // Perform the insert
                $insertStmt->execute([
                    ':TaskID' => $task['TaskID'],
                    ':Title' => $task['Title'],
                    ':LastUpdate' => $task['LastUpdate'],
                    ':SimDateTime' => $task['SimDateTime'],
                    ':IncludeYear' => $task['IncludeYear'],
                    ':SimDateTimeExtraInfo' => $task['SimDateTimeExtraInfo'],
                    ':MainAreaPOI' => $task['MainAreaPOI'],
                    ':DepartureName' => $task['DepartureName'],
                    ':DepartureICAO' => $task['DepartureICAO'],
                    ':DepartureExtra' => $task['DepartureExtra'],
                    ':ArrivalName' => $task['ArrivalName'],
                    ':ArrivalICAO' => $task['ArrivalICAO'],
                    ':ArrivalExtra' => $task['ArrivalExtra'],
                    ':SoaringRidge' => $task['SoaringRidge'],
                    ':SoaringThermals' => $task['SoaringThermals'],
                    ':SoaringWaves' => $task['SoaringWaves'],
                    ':SoaringDynamic' => $task['SoaringDynamic'],
                    ':SoaringExtraInfo' => $task['SoaringExtraInfo'],
                    ':DurationMin' => $task['DurationMin'],
                    ':DurationMax' => $task['DurationMax'],
                    ':DurationExtraInfo' => $task['DurationExtraInfo'],
                    ':TaskDistance' => $task['TaskDistance'],
                    ':TotalDistance' => $task['TotalDistance'],
                    ':RecommendedGliders' => $task['RecommendedGliders'],
                    ':DifficultyRating' => $task['DifficultyRating'],
                    ':DifficultyExtraInfo' => $task['DifficultyExtraInfo'],
                    ':ShortDescription' => $task['ShortDescription'],
                    ':LongDescription' => $task['LongDescription'],
                    ':WeatherSummary' => $task['WeatherSummary'],
                    ':Credits' => $task['Credits'],
                    ':Countries' => $task['Countries'],
                    ':RecommendedAddOns' => $task['RecommendedAddOns'],
                    ':MapImage' => $mapImage,
                    ':CoverImage' => $coverImage,
                    ':TotDownloads' => $task['TotDownloads'],
                    ':LastDownloadUpdate' => $task['LastDownloadUpdate'],
                    ':DBEntryUpdate' => $task['DBEntryUpdate']
                ]);
                logMessage("Inserted task with TaskID: " . $task['TaskID']);
            } else {
                // Perform the update
                $updateStmt->execute([
                    ':EntrySeqID' => $task['EntrySeqID'],
                    ':TaskID' => $task['TaskID'],
                    ':Title' => $task['Title'],
                    ':LastUpdate' => $task['LastUpdate'],
                    ':SimDateTime' => $task['SimDateTime'],
                    ':IncludeYear' => $task['IncludeYear'],
                    ':SimDateTimeExtraInfo' => $task['SimDateTimeExtraInfo'],
                    ':MainAreaPOI' => $task['MainAreaPOI'],
                    ':DepartureName' => $task['DepartureName'],
                    ':DepartureICAO' => $task['DepartureICAO'],
                    ':DepartureExtra' => $task['DepartureExtra'],
                    ':ArrivalName' => $task['ArrivalName'],
                    ':ArrivalICAO' => $task['ArrivalICAO'],
                    ':ArrivalExtra' => $task['ArrivalExtra'],
                    ':SoaringRidge' => $task['SoaringRidge'],
                    ':SoaringThermals' => $task['SoaringThermals'],
                    ':SoaringWaves' => $task['SoaringWaves'],
                    ':SoaringDynamic' => $task['SoaringDynamic'],
                    ':SoaringExtraInfo' => $task['SoaringExtraInfo'],
                    ':DurationMin' => $task['DurationMin'],
                    ':DurationMax' => $task['DurationMax'],
                    ':DurationExtraInfo' => $task['DurationExtraInfo'],
                    ':TaskDistance' => $task['TaskDistance'],
                    ':TotalDistance' => $task['TotalDistance'],
                    ':RecommendedGliders' => $task['RecommendedGliders'],
                    ':DifficultyRating' => $task['DifficultyRating'],
                    ':DifficultyExtraInfo' => $task['DifficultyExtraInfo'],
                    ':ShortDescription' => $task['ShortDescription'],
                    ':LongDescription' => $task['LongDescription'],
                    ':WeatherSummary' => $task['WeatherSummary'],
                    ':Credits' => $task['Credits'],
                    ':Countries' => $task['Countries'],
                    ':RecommendedAddOns' => $task['RecommendedAddOns'],
                    ':MapImage' => $mapImage,
                    ':CoverImage' => $coverImage,
                    ':DBEntryUpdate' => $task['DBEntryUpdate']
                ]);
                logMessage("Updated task with EntrySeqID: " . $task['EntrySeqID']);
            }
        } catch (PDOException $e) {
            logMessage("Error processing task with TaskID: " . $task['TaskID'] . " - " . $e->getMessage());
            echo json_encode(['status' => 'error', 'message' => $e->getMessage()]);
        }
    }

    echo json_encode(['status' => 'success', 'message' => 'Tasks updated successfully']);
    logMessage("Tasks updated successfully.");

} catch (PDOException $e) {
    logMessage("Connection failed: " . $e->getMessage());
    echo json_encode(['status' => 'error', 'message' => 'Connection failed: ' . $e->getMessage()]);
}
?>
