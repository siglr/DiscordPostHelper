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

    // Check if the required fields are set
    if (!isset($_FILES['file']) || !isset($_FILES['image']) || !isset($_POST['task_data']) || !isset($_POST['UserID'])) {
        throw new Exception('File, image, task data, or UserID missing.');
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
    $dphxPath = $fileRootPath . 'TaskBrowser/Tasks/';
    $target_file = $dphxPath . basename($taskID . '.dphx');

    if (!move_uploaded_file($file['tmp_name'], $target_file)) {
        throw new Exception('Failed to upload the DPHX file.');
    }

    // Check if the task exists
    $checkStmt = $pdo->prepare("SELECT COUNT(*) FROM Tasks WHERE TaskID = :TaskID");
    $checkStmt->execute([':TaskID' => $taskID]);
    $taskExists = $checkStmt->fetchColumn() > 0;

    // Prepare BaroPressureExtraInfo
    $baroPressureExtraInfo = isset($taskData['BaroPressureExtraInfo']) ? trim($taskData['BaroPressureExtraInfo']) : null;
    if (trim($baroPressureExtraInfo) === 'Non standard: Set your altimeter! (Press "B" once in your glider)' || 
        trim($baroPressureExtraInfo) === 'Non standard: Set your altimeter!') {
        $baroPressureExtraInfo = null; // Set to NULL for default values
    }

    // Prepare RecommendedAddOnsList
    $recommendedAddOnsList = isset($taskData['RecommendedAddOnsList']) ? $taskData['RecommendedAddOnsList'] : '[]';
    if (json_decode($recommendedAddOnsList) === null && $recommendedAddOnsList !== '[]') {
        logMessage("Invalid JSON format for RecommendedAddOnsList. Resetting to an empty JSON array.");
        $recommendedAddOnsList = '[]'; // Default to an empty JSON array
    }

    // Prepare SuppressBaroPressureWarningSymbol
    $suppressBaroPressureWarningSymbol = isset($taskData['SuppressBaroPressureWarningSymbol']) ? (int)$taskData['SuppressBaroPressureWarningSymbol'] : 0;

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
                RecommendedAddOns, RecommendedAddOnsList, MapImage, CoverImage, DBEntryUpdate, 
                PLNFilename, PLNXML, WPRFilename, WPRXML, LatMin, LatMax, LongMin, LongMax, RepostText,
                SuppressBaroPressureWarningSymbol, BaroPressureExtraInfo
            ) VALUES (
                :TaskID, :Title, :LastUpdate, :SimDateTime, :IncludeYear, :SimDateTimeExtraInfo,
                :MainAreaPOI, :DepartureName, :DepartureICAO, :DepartureExtra, :ArrivalName,
                :ArrivalICAO, :ArrivalExtra, :SoaringRidge, :SoaringThermals, :SoaringWaves,
                :SoaringDynamic, :SoaringExtraInfo, :DurationMin, :DurationMax, :DurationExtraInfo,
                :TaskDistance, :TotalDistance, :RecommendedGliders, :DifficultyRating, :DifficultyExtraInfo,
                :ShortDescription, :LongDescription, :WeatherSummary, :Credits, :Countries,
                :RecommendedAddOns, :RecommendedAddOnsList, :MapImage, :CoverImage, :DBEntryUpdate, 
                :PLNFilename, :PLNXML, :WPRFilename, :WPRXML, :LatMin, :LatMax, :LongMin, :LongMax, :RepostText,
                :SuppressBaroPressureWarningSymbol, :BaroPressureExtraInfo
            )
        ");
        logMessage("Insert statement prepared.");
 
        // Ensure datetime fields are properly formatted
        $taskData['LastUpdate'] = formatDatetime($taskData['LastUpdate']);
        $taskData['SimDateTime'] = formatDatetime($taskData['SimDateTime']);
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
            ':RecommendedAddOnsList' => $recommendedAddOnsList,
            ':MapImage' => $mapImage,
            ':CoverImage' => $coverImage,
            ':DBEntryUpdate' => $taskData['DBEntryUpdate'],
            ':PLNFilename' => $taskData['PLNFilename'],
            ':PLNXML' => $taskData['PLNXML'],
            ':WPRFilename' => $taskData['WPRFilename'],
            ':WPRXML' => $taskData['WPRXML'],
            ':LatMin' => $taskData['LatMin'],
            ':LatMax' => $taskData['LatMax'],
            ':LongMin' => $taskData['LongMin'],
            ':LongMax' => $taskData['LongMax'],
            ':RepostText' => $taskData['RepostText'],
            ':SuppressBaroPressureWarningSymbol' => $suppressBaroPressureWarningSymbol,
            ':BaroPressureExtraInfo' => $baroPressureExtraInfo
        ]);
        logMessage("Inserted task with TaskID: " . $taskData['TaskID']);
        
        // Retrieve the EntrySeqID of the newly inserted task
        $taskData['EntrySeqID'] = $pdo->lastInsertId();
        if (!$taskData['EntrySeqID']) {
            throw new Exception("Failed to retrieve EntrySeqID for TaskID: " . $taskData['TaskID']);
        }

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
                RecommendedAddOnsList = :RecommendedAddOnsList, MapImage = :MapImage, CoverImage = :CoverImage, DBEntryUpdate = :DBEntryUpdate,
                PLNFilename = :PLNFilename, PLNXML = :PLNXML, WPRFilename = :WPRFilename, WPRXML = :WPRXML, 
                LatMin = :LatMin, LatMax = :LatMax, LongMin = :LongMin, LongMax = :LongMax, RepostText = :RepostText, 
                SuppressBaroPressureWarningSymbol = :SuppressBaroPressureWarningSymbol, BaroPressureExtraInfo = :BaroPressureExtraInfo
            WHERE TaskID = :TaskID
        ");
        logMessage("Update statement prepared.");

        // Ensure datetime fields are properly formatted
        $taskData['LastUpdate'] = formatDatetime($taskData['LastUpdate']);
        $taskData['SimDateTime'] = formatDatetime($taskData['SimDateTime']);
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
            ':RecommendedAddOnsList' => $recommendedAddOnsList,
            ':MapImage' => $mapImage,
            ':CoverImage' => $coverImage,
            ':DBEntryUpdate' => $taskData['DBEntryUpdate'],
            ':PLNFilename' => $taskData['PLNFilename'],
            ':PLNXML' => $taskData['PLNXML'],
            ':WPRFilename' => $taskData['WPRFilename'],
            ':WPRXML' => $taskData['WPRXML'],
            ':LatMin' => $taskData['LatMin'],
            ':LatMax' => $taskData['LatMax'],
            ':LongMin' => $taskData['LongMin'],
            ':LongMax' => $taskData['LongMax'],
            ':RepostText' => $taskData['RepostText'],
            ':SuppressBaroPressureWarningSymbol' => $suppressBaroPressureWarningSymbol,
            ':BaroPressureExtraInfo' => $baroPressureExtraInfo
        ]);
        logMessage("Updated task with TaskID: " . $taskData['TaskID']);

        // Retrieve the EntrySeqID of the updated task
        $stmt = $pdo->prepare("SELECT EntrySeqID FROM Tasks WHERE TaskID = :TaskID");
        $stmt->execute([':TaskID' => $taskData['TaskID']]);
        $taskData['EntrySeqID'] = $stmt->fetchColumn();
        if (!$taskData['EntrySeqID']) {
            throw new Exception("Failed to retrieve EntrySeqID for TaskID: " . $taskData['TaskID']);
        }

        // Update corresponding news entry
        createOrUpdateTaskNewsEntry($taskData, true);
    }

    // Ensure the image file is uploaded
    $image = $_FILES['image'];
    $target_image_dir = '/home2/siglr3/public_html/DiscordPostHelper/TaskBrowser/WeatherCharts/';
    $target_image_file = $target_image_dir . basename($taskData['EntrySeqID'] . '.jpg'); // Assuming the image is a JPG

    if (!move_uploaded_file($image['tmp_name'], $target_image_file)) {
        throw new Exception('Failed to upload the image file.');
    }

    echo json_encode(['status' => 'success', 'message' => 'Task uploaded and database updated successfully.']);
    logMessage("--- End of script CreateUpdateTaskFromDPHTool ---");

} catch (Exception $e) {
    logMessage("Connection failed: " . $e->getMessage());
    echo json_encode(['status' => 'error', 'message' => 'Connection failed: ' . $e->getMessage()]);
    logMessage("--- End of script CreateUpdateTaskFromDPHTool ---");
}
?>
