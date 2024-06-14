<?php
require __DIR__ . '/CommonFunctions.php';

try {
    logMessage("--- Script running CreateUpdateTaskFromDPHTool ---");

    // Open the database connection
    $pdo = new PDO("sqlite:$databasePath");
    $pdo->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);
    logMessage("Database connection established.");

    // Ensure the request method is POST
    if ($_SERVER['REQUEST_METHOD'] !== 'POST') {
        throw new Exception('Invalid request method.');
    }

    // Check if the file and task_data are set
    if (!isset($_FILES['file']) || !isset($_POST['task_data']) || !isset($_POST['UserID'])) {
        throw new Exception('File, task data, or UserID missing.');
    }

    // Get the task data from POST
    $taskData = json_decode($_POST['task_data'], true);
    if (json_last_error() !== JSON_ERROR_NONE) {
        throw new Exception('Invalid task data.');
    }

    // Get the user ID
    $userID = $_POST['UserID'];

    // Ensure the DPHX file is uploaded
    $file = $_FILES['file'];
    $taskID = $taskData['TaskID'];
    $target_dir = '/home2/siglr3/public_html/DiscordPostHelper/TaskBrowser/Tasks/';
    $target_file = $target_dir . basename($taskID . '.dphx');

    if (!move_uploaded_file($file['tmp_name'], $target_file)) {
        throw new Exception('Failed to upload the file.');
    }

    // Check if the task exists
    $checkStmt = $pdo->prepare("SELECT COUNT(*) FROM Tasks WHERE TaskID = :TaskID");
    $checkStmt->execute([':TaskID' => $taskID]);
    $taskExists = $checkStmt->fetchColumn() > 0;

    if (!$taskExists) {
        // Check if the user has CreateTask rights
        if (!checkUserPermission($userID, 'CreateTask')) {
            throw new Exception('User ' . $userID . ' does not have permission to create tasks.');
        }

        // Prepare the insert statement
        $stmt = $pdo->prepare("
            INSERT INTO Tasks (
                TaskID, Title, LastUpdate, SimDateTime, IncludeYear, SimDateTimeExtraInfo,
                MainAreaPOI, DepartureName, DepartureICAO, DepartureExtra, ArrivalName,
                ArrivalICAO, ArrivalExtra, SoaringRidge, SoaringThermals, SoaringWaves,
                SoaringDynamic, SoaringExtraInfo, DurationMin, DurationMax, DurationExtraInfo,
                TaskDistance, TotalDistance, RecommendedGliders, DifficultyRating, DifficultyExtraInfo,
                ShortDescription, LongDescription, WeatherSummary, Credits, Countries,
                RecommendedAddOns, MapImage, CoverImage, DBEntryUpdate
            ) VALUES (
                :TaskID, :Title, :LastUpdate, :SimDateTime, :IncludeYear, :SimDateTimeExtraInfo,
                :MainAreaPOI, :DepartureName, :DepartureICAO, :DepartureExtra, :ArrivalName,
                :ArrivalICAO, :ArrivalExtra, :SoaringRidge, :SoaringThermals, :SoaringWaves,
                :SoaringDynamic, :SoaringExtraInfo, :DurationMin, :DurationMax, :DurationExtraInfo,
                :TaskDistance, :TotalDistance, :RecommendedGliders, :DifficultyRating, :DifficultyExtraInfo,
                :ShortDescription, :LongDescription, :WeatherSummary, :Credits, :Countries,
                :RecommendedAddOns, :MapImage, :CoverImage, :DBEntryUpdate
            )
        ");
        logMessage("Insert statement prepared.");

        // Ensure datetime fields are properly formatted
        $taskData['LastUpdate'] = formatDatetime($taskData['LastUpdate']);
        $taskData['SimDateTime'] = formatDatetime($taskData['SimDateTime']);
        $taskData['LastDownloadUpdate'] = formatDatetime($taskData['LastDownloadUpdate']);
        $taskData['DBEntryUpdate'] = formatDatetime($taskData['DBEntryUpdate']);

        // Handle the task data
        $mapImage = isset($taskData['MapImage']) && !empty($taskData['MapImage']) ? base64_decode($taskData['MapImage']) : null;
        $coverImage = isset($taskData['CoverImage']) && !empty($taskData['CoverImage']) ? base64_decode($taskData['CoverImage']) : null;

        // Perform the insert
        $stmt->execute([
            ':TaskID' => $taskData['TaskID'],
            ':Title' => $taskData['Title'],
            ':LastUpdate' => $taskData['LastUpdate'],
            ':SimDateTime' => $taskData['SimDateTime'],
            ':IncludeYear' => $taskData['IncludeYear'],
            ':SimDateTimeExtraInfo' => $taskData['SimDateTimeExtraInfo'],
            ':MainAreaPOI' => $taskData['MainAreaPOI'],
            ':DepartureName' => $taskData['DepartureName'],
            ':DepartureICAO' => $taskData['DepartureICAO'],
            ':DepartureExtra' => $taskData['DepartureExtra'],
            ':ArrivalName' => $taskData['ArrivalName'],
            ':ArrivalICAO' => $taskData['ArrivalICAO'],
            ':ArrivalExtra' => $taskData['ArrivalExtra'],
            ':SoaringRidge' => $taskData['SoaringRidge'],
            ':SoaringThermals' => $taskData['SoaringThermals'],
            ':SoaringWaves' => $taskData['SoaringWaves'],
            ':SoaringDynamic' => $taskData['SoaringDynamic'],
            ':SoaringExtraInfo' => $taskData['SoaringExtraInfo'],
            ':DurationMin' => $taskData['DurationMin'],
            ':DurationMax' => $taskData['DurationMax'],
            ':DurationExtraInfo' => $taskData['DurationExtraInfo'],
            ':TaskDistance' => $taskData['TaskDistance'],
            ':TotalDistance' => $taskData['TotalDistance'],
            ':RecommendedGliders' => $taskData['RecommendedGliders'],
            ':DifficultyRating' => $taskData['DifficultyRating'],
            ':DifficultyExtraInfo' => $taskData['DifficultyExtraInfo'],
            ':ShortDescription' => $taskData['ShortDescription'],
            ':LongDescription' => $taskData['LongDescription'],
            ':WeatherSummary' => $taskData['WeatherSummary'],
            ':Credits' => $taskData['Credits'],
            ':Countries' => $taskData['Countries'],
            ':RecommendedAddOns' => $taskData['RecommendedAddOns'],
            ':MapImage' => $mapImage,
            ':CoverImage' => $coverImage,
            ':DBEntryUpdate' => $taskData['DBEntryUpdate']
        ]);
        logMessage("Inserted task with TaskID: " . $taskData['TaskID']);
        
        // Retrieve the EntrySeqID of the newly inserted task
        $taskData['EntrySeqID'] = $pdo->lastInsertId();

        // Create or update WorldMapInfo
        createOrUpdateWorldMapInfo($pdo, $taskData);

        // Create corresponding news entry
        createOrUpdateTaskNewsEntry($taskData, false);

    } else {
        // Check if the user has UpdateTask rights
        if (!checkUserPermission($userID, 'UpdateTask')) {
            throw new Exception('User ' . $userID . ' does not have permission to update tasks.');
        }

        // Prepare the update statement
        $stmt = $pdo->prepare("
            UPDATE Tasks SET
                Title = :Title, LastUpdate = :LastUpdate, SimDateTime = :SimDateTime,
                IncludeYear = :IncludeYear, SimDateTimeExtraInfo = :SimDateTimeExtraInfo, MainAreaPOI = :MainAreaPOI,
                DepartureName = :DepartureName, DepartureICAO = :DepartureICAO, DepartureExtra = :DepartureExtra,
                ArrivalName = :ArrivalName, ArrivalICAO = :ArrivalICAO, ArrivalExtra = :ArrivalExtra, SoaringRidge = :SoaringRidge,
                SoaringThermals = :SoaringThermals, SoaringWaves = :SoaringWaves, SoaringDynamic = :SoaringDynamic,
                SoaringExtraInfo = :SoaringExtraInfo, DurationMin = :DurationMin, DurationMax = :DurationMax, DurationExtraInfo = :DurationExtraInfo,
                TaskDistance = :TaskDistance, TotalDistance = :TotalDistance, RecommendedGliders = :RecommendedGliders, DifficultyRating = :DifficultyRating,
                DifficultyExtraInfo = :DifficultyExtraInfo, ShortDescription = :ShortDescription, LongDescription = :LongDescription,
                WeatherSummary = :WeatherSummary, Credits = :Credits, Countries = :Countries, RecommendedAddOns = :RecommendedAddOns,
                MapImage = :MapImage, CoverImage = :CoverImage, DBEntryUpdate = :DBEntryUpdate
            WHERE TaskID = :TaskID
        ");
        logMessage("Update statement prepared.");

        // Ensure datetime fields are properly formatted
        $taskData['LastUpdate'] = formatDatetime($taskData['LastUpdate']);
        $taskData['SimDateTime'] = formatDatetime($taskData['SimDateTime']);
        $taskData['LastDownloadUpdate'] = formatDatetime($taskData['LastDownloadUpdate']);
        $taskData['DBEntryUpdate'] = formatDatetime($taskData['DBEntryUpdate']);

        // Handle the task data
        $mapImage = isset($taskData['MapImage']) && !empty($taskData['MapImage']) ? base64_decode($taskData['MapImage']) : null;
        $coverImage = isset($taskData['CoverImage']) && !empty($taskData['CoverImage']) ? base64_decode($taskData['CoverImage']) : null;

        // Perform the update
        $stmt->execute([
            ':TaskID' => $taskData['TaskID'],
            ':Title' => $taskData['Title'],
            ':LastUpdate' => $taskData['LastUpdate'],
            ':SimDateTime' => $taskData['SimDateTime'],
            ':IncludeYear' => $taskData['IncludeYear'],
            ':SimDateTimeExtraInfo' => $taskData['SimDateTimeExtraInfo'],
            ':MainAreaPOI' => $taskData['MainAreaPOI'],
            ':DepartureName' => $taskData['DepartureName'],
            ':DepartureICAO' => $taskData['DepartureICAO'],
            ':DepartureExtra' => $taskData['DepartureExtra'],
            ':ArrivalName' => $taskData['ArrivalName'],
            ':ArrivalICAO' => $taskData['ArrivalICAO'],
            ':ArrivalExtra' => $taskData['ArrivalExtra'],
            ':SoaringRidge' => $taskData['SoaringRidge'],
            ':SoaringThermals' => $taskData['SoaringThermals'],
            ':SoaringWaves' => $taskData['SoaringWaves'],
            ':SoaringDynamic' => $taskData['SoaringDynamic'],
            ':SoaringExtraInfo' => $taskData['SoaringExtraInfo'],
            ':DurationMin' => $taskData['DurationMin'],
            ':DurationMax' => $taskData['DurationMax'],
            ':DurationExtraInfo' => $taskData['DurationExtraInfo'],
            ':TaskDistance' => $taskData['TaskDistance'],
            ':TotalDistance' => $taskData['TotalDistance'],
            ':RecommendedGliders' => $taskData['RecommendedGliders'],
            ':DifficultyRating' => $taskData['DifficultyRating'],
            ':DifficultyExtraInfo' => $taskData['DifficultyExtraInfo'],
            ':ShortDescription' => $taskData['ShortDescription'],
            ':LongDescription' => $taskData['LongDescription'],
            ':WeatherSummary' => $taskData['WeatherSummary'],
            ':Credits' => $taskData['Credits'],
            ':Countries' => $taskData['Countries'],
            ':RecommendedAddOns' => $taskData['RecommendedAddOns'],
            ':MapImage' => $mapImage,
            ':CoverImage' => $coverImage,
            ':DBEntryUpdate' => $taskData['DBEntryUpdate']
        ]);
        logMessage("Updated task with TaskID: " . $taskData['TaskID']);

        // Retrieve the EntrySeqID of the updated task
        $stmt = $pdo->prepare("SELECT EntrySeqID FROM Tasks WHERE TaskID = :TaskID");
        $stmt->execute([':TaskID' => $taskData['TaskID']]);
        $taskData['EntrySeqID'] = $stmt->fetchColumn();

        // Update WorldMapInfo
        createOrUpdateWorldMapInfo($pdo, $taskData);

        // Update corresponding news entry
        createOrUpdateTaskNewsEntry($taskData, true);
    }

    echo json_encode(['status' => 'success', 'message' => 'Task uploaded and database updated successfully.']);
    logMessage("--- End of script CreateUpdateTaskFromDPHTool ---");

} catch (Exception $e) {
    logMessage("Connection failed: " . $e->getMessage());
    echo json_encode(['status' => 'error', 'message' => 'Connection failed: ' . $e->getMessage()]);
    logMessage("--- End of script CreateUpdateTaskFromDPHTool ---");
}

// Function to create or update WorldMapInfo entry
function createOrUpdateWorldMapInfo($pdo, $taskData) {
    $stmt = $pdo->prepare("
        INSERT OR REPLACE INTO WorldMapInfo (
            EntrySeqID, TaskID, PLNFilename, PLNXML, WPRFilename, WPRXML, LatMin, LatMax, LongMin, LongMax
        ) VALUES (
            :EntrySeqID, :TaskID, :PLNFilename, :PLNXML, :WPRFilename, :WPRXML, :LatMin, :LatMax, :LongMin, :LongMax
        )
    ");

    $stmt->execute([
        ':EntrySeqID' => $taskData['EntrySeqID'],
        ':TaskID' => $taskData['TaskID'],
        ':PLNFilename' => $taskData['PLNFilename'],
        ':PLNXML' => $taskData['PLNXML'],
        ':WPRFilename' => $taskData['WPRFilename'],
        ':WPRXML' => $taskData['WPRXML'],
        ':LatMin' => $taskData['LatMin'],
        ':LatMax' => $taskData['LatMax'],
        ':LongMin' => $taskData['LongMin'],
        ':LongMax' => $taskData['LongMax']
    ]);

    logMessage("WorldMapInfo entry updated for TaskID: " . $taskData['TaskID']);
}
?>
