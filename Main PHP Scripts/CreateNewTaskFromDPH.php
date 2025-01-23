<?php
require __DIR__ . '/CommonFunctions.php';

try {
    logMessage("--- Script running CreateNewTaskFromDPH ---");

    // Open the database connection
    $pdo = new PDO("sqlite:$databasePath");
    $pdo->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);
    logMessage("Database connection established.");

    // Ensure the request method is POST
    if ($_SERVER['REQUEST_METHOD'] !== 'POST') {
        throw new Exception('Invalid request method.');
    }

    // Parse task data from POST
    $taskData = isset($_POST['task_data']) ? json_decode($_POST['task_data'], true) : null;
    $userID = isset($_POST['UserID']) ? $_POST['UserID'] : null;

    if (!$taskData || !$userID) {
        throw new Exception('Missing task data or UserID.');
    }

    // Check if the user has CreateTask rights
    if (!checkUserPermission($userID, 'CreateTask')) {
        throw new Exception('User ' . $userID . ' does not have permission to create tasks.');
    }

    // Check if the task already exists
    $checkStmt = $pdo->prepare("SELECT COUNT(*) FROM Tasks WHERE TaskID = :TemporaryTaskID");
    $checkStmt->execute([':TemporaryTaskID' => $taskData['TemporaryTaskID']]);
    $taskExists = $checkStmt->fetchColumn() > 0;

    if (!$taskExists) {
        // Step 1: Create new task (minimal fields only)
        logMessage("Creating new temporary task...");

        // Validate required fields
        $requiredFields = ['TemporaryTaskID', 'Title', 'LastUpdate'];
        foreach ($requiredFields as $field) {
            if (empty($taskData[$field])) {
                throw new Exception("Missing required field: $field");
            }
        }

        // Insert minimal data into the database
        $stmt = $pdo->prepare("
            INSERT INTO Tasks (
                TaskID, Title, LastUpdate, SimDateTime, IncludeYear, DBEntryUpdate,
                TaskDistance, TotalDistance, LatMin, LatMax, LongMin, LongMax, Status
            ) VALUES (
                :TemporaryTaskID, :Title, :LastUpdate, :SimDateTime, :IncludeYear, :DBEntryUpdate,
                :TaskDistance, :TotalDistance, :LatMin, :LatMax, :LongMin, :LongMax, :Status
            )
        ");

        // Ensure datetime fields are properly formatted
        $taskData['LastUpdate'] = formatDatetime($taskData['LastUpdate']);
        $taskData['SimDateTime'] = formatDatetime($taskData['SimDateTime']);
        $taskData['DBEntryUpdate'] = formatDatetime($taskData['DBEntryUpdate']);

        $stmt->execute([
            ':TemporaryTaskID' => $taskData['TemporaryTaskID'],
            ':Title' => $taskData['Title'],
            ':LastUpdate' => $taskData['LastUpdate'],
            ':SimDateTime' => $taskData['SimDateTime'],
            ':IncludeYear' => $taskData['IncludeYear'],
            ':DBEntryUpdate' => $taskData['DBEntryUpdate'],
            ':TaskDistance' => $taskData['TaskDistance'],
            ':TotalDistance' => $taskData['TotalDistance'],
            ':LatMin' => $taskData['LatMin'],
            ':LatMax' => $taskData['LatMax'],
            ':LongMin' => $taskData['LongMin'],
            ':LongMax' => $taskData['LongMax'],
            ':Status' => 10 // PendingCreation
        ]);

        // Retrieve EntrySeqID
        $entrySeqID = $pdo->lastInsertId();
        if (!$entrySeqID) {
            throw new Exception("Failed to retrieve EntrySeqID for the newly created task.");
        }

        echo json_encode(['status' => 'success', 'EntrySeqID' => $entrySeqID]);
        logMessage("New temporary task created with EntrySeqID: $entrySeqID");

    } else {
        // Step 2: Update existing task with full details
        logMessage("Completing task creation...");

        // Validate that required files are present
        if (!isset($_FILES['file']) || !isset($_FILES['image'])) {
            throw new Exception("DPHX file and Weather image are required for task creation.");
        }

        // Ensure the DPHX file is uploaded
        $file = $_FILES['file'];
        $taskID = $taskData['RealTaskID'];
        $dphxPath = $fileRootPath . 'TaskBrowser/Tasks/';
        $target_file = $dphxPath . basename($taskID . '.dphx');
        if (!move_uploaded_file($file['tmp_name'], $target_file)) {
            throw new Exception('Failed to upload the DPHX file.');
        }

        // Ensure the weather image file is uploaded
        $image = $_FILES['image'];
        $target_image_dir = $fileRootPath . 'TaskBrowser/WeatherCharts/';
        $target_image_file = $target_image_dir . basename($taskData['EntrySeqID'] . '.jpg');

        if (!move_uploaded_file($image['tmp_name'], $target_image_file)) {
            throw new Exception('Failed to upload the weather image file.');
        }

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

        // Prepare ExtraFilesList
        $extraFilesList = isset($taskData['ExtraFilesList']) ? $taskData['ExtraFilesList'] : '[]';
        if (json_decode($extraFilesList) === null && $extraFilesList !== '[]') {
            logMessage("Invalid JSON format for ExtraFilesList. Resetting to an empty JSON array.");
            $extraFilesList = '[]'; // Default to an empty JSON array
        }

        // Prepare SuppressBaroPressureWarningSymbol
        $suppressBaroPressureWarningSymbol = isset($taskData['SuppressBaroPressureWarningSymbol']) ? (int)$taskData['SuppressBaroPressureWarningSymbol'] : 0;

        // Update the database with full task details
        $stmt = $pdo->prepare("
            UPDATE Tasks SET
                TaskID = :RealTaskID, Title = :Title, LastUpdate = :LastUpdate, SimDateTime = :SimDateTime,
                IncludeYear = :IncludeYear, SimDateTimeExtraInfo = :SimDateTimeExtraInfo, MainAreaPOI = :MainAreaPOI,
                DepartureName = :DepartureName, DepartureICAO = :DepartureICAO, DepartureExtra = :DepartureExtra,
                ArrivalName = :ArrivalName, ArrivalICAO = :ArrivalICAO, ArrivalExtra = :ArrivalExtra, SoaringRidge = :SoaringRidge,
                SoaringThermals = :SoaringThermals, SoaringWaves = :SoaringWaves, SoaringDynamic = :SoaringDynamic,
                SoaringExtraInfo = :SoaringExtraInfo, DurationMin = :DurationMin, DurationMax = :DurationMax,
                DurationExtraInfo = :DurationExtraInfo, TaskDistance = :TaskDistance, TotalDistance = :TotalDistance,
                RecommendedGliders = :RecommendedGliders, DifficultyRating = :DifficultyRating,
                DifficultyExtraInfo = :DifficultyExtraInfo, ShortDescription = :ShortDescription,
                LongDescription = :LongDescription, WeatherSummary = :WeatherSummary, Credits = :Credits, Countries = :Countries,
                RecommendedAddOns = :RecommendedAddOns, RecommendedAddOnsList = :RecommendedAddOnsList, MapImage = :MapImage,
                CoverImage = :CoverImage, DBEntryUpdate = :DBEntryUpdate, PLNFilename = :PLNFilename, PLNXML = :PLNXML,
                WPRFilename = :WPRFilename, WPRXML = :WPRXML, LatMin = :LatMin, LatMax = :LatMax, LongMin = :LongMin,
                LongMax = :LongMax, RepostText = :RepostText, SuppressBaroPressureWarningSymbol = :SuppressBaroPressureWarningSymbol,
                BaroPressureExtraInfo = :BaroPressureExtraInfo, ExtraFilesList = :ExtraFilesList, Status = :Status
            WHERE TaskID = :TemporaryTaskID
        ");

        // Ensure datetime fields are properly formatted
        $taskData['LastUpdate'] = formatDatetime($taskData['LastUpdate']);
        $taskData['SimDateTime'] = formatDatetime($taskData['SimDateTime']);
        $taskData['DBEntryUpdate'] = formatDatetime($taskData['DBEntryUpdate']);

        // Handle the task data
        $mapImage = isset($taskData['MapImage']) && !empty($taskData['MapImage']) ? base64_decode($taskData['MapImage']) : null;
        $coverImage = isset($taskData['CoverImage']) && !empty($taskData['CoverImage']) ? base64_decode($taskData['CoverImage']) : null;

        // Perform the update
        $stmt->execute([
            ':RealTaskID' => $taskData['RealTaskID'],
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
            ':BaroPressureExtraInfo' => $baroPressureExtraInfo,
            ':ExtraFilesList' => $extraFilesList,
            ':Status' => $status,
            ':TemporaryTaskID' => $taskData['TemporaryTaskID']
        ]);

        // Call createOrUpdateTaskNewsEntry if Status = 99
        if ($taskData['Status'] === 99) {
            // Add TaskID to $taskData and set it to RealTaskID
            $taskData['TaskID'] = $taskData['RealTaskID'];
            // Call the function with updated $taskData
            createOrUpdateTaskNewsEntry($taskData, false);
        }

        echo json_encode(['status' => 'success', 'message' => 'Task updated successfully.']);
        logMessage("Task creation successfully completed with new TaskID: " . $taskData['RealTaskID']);
    }
} catch (Exception $e) {
    logMessage("Error: " . $e->getMessage());
    echo json_encode(['status' => 'error', 'message' => $e->getMessage()]);
}
?>
