<?php
require __DIR__ . '/CommonFunctions.php';

try {
    logMessage("--- Script running CreateNewTaskFromDPH ---");

    // Open the database connection
    $pdo = new PDO("sqlite:$databasePath");
    $pdo->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);

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

    // Check if the user has the proper rights
    if (!checkUserPermission($userID, $taskData['Mode'])) {
        throw new Exception('User ' . $userID . ' does not have permission for ' . $taskData['Mode']);
    }

    // Set default values for OwnerName and SharedWith
    $ownerName = isset($taskData['OwnerName']) ? $taskData['OwnerName'] : 'MajorDad';
    $sharedWith = isset($taskData['SharedWith']) ? $taskData['SharedWith'] : json_encode([]);

    // Determine if it's Step 1 (new task creation) or Step 2 (update)
    $isCreatingNewTask = isset($taskData['TemporaryTaskID']) && !isset($taskData['EntrySeqID']);

    if ($isCreatingNewTask) {
        // Step 1: Create new task (minimal fields only)
        logMessage("Creating new temporary task...");

        // Validate required fields
        $requiredFields = ['TemporaryTaskID', 'Title', 'LastUpdate'];
        foreach ($requiredFields as $field) {
            if (empty($taskData[$field])) {
                throw new Exception("Missing required field: $field");
            }
        }

        // Check if the task already exists
        $checkStmt = $pdo->prepare("SELECT COUNT(*) FROM Tasks WHERE TaskID = :TemporaryTaskID");
        $checkStmt->execute([':TemporaryTaskID' => $taskData['TemporaryTaskID']]);
        $taskExists = $checkStmt->fetchColumn() > 0;

        if ($taskExists) {
            throw new Exception("A task with the same TemporaryTaskID already exists: " . $taskData['TemporaryTaskID']);
        }

        // Insert minimal data into the database
        $stmt = $pdo->prepare("
            INSERT INTO Tasks (
                TaskID, 
                Title, 
                LastUpdate, 
                SimDateTime, 
                IncludeYear, 
                DBEntryUpdate, 
                SoaringRidge, 
                SoaringThermals, 
                SoaringWaves, 
                SoaringDynamic,
                TaskDistance, 
                TotalDistance, 
                RecommendedAddOns,
                LatMin, 
                LatMax, 
                LongMin, 
                LongMax, 
                Status,
                OwnerName
            ) VALUES (
                :TemporaryTaskID, 
                :Title, 
                :LastUpdate, 
                :SimDateTime, 
                :IncludeYear, 
                :DBEntryUpdate,
                :SoaringRidge, 
                :SoaringThermals, 
                :SoaringWaves, 
                :SoaringDynamic,
                :TaskDistance, 
                :TotalDistance, 
                :RecommendedAddOns,
                :LatMin, 
                :LatMax, 
                :LongMin, 
                :LongMax, 
                :Status,
                :OwnerName
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
            ':SoaringRidge' => $taskData['SoaringRidge'],
            ':SoaringThermals' => $taskData['SoaringThermals'],
            ':SoaringWaves' => $taskData['SoaringWaves'],
            ':SoaringDynamic' => $taskData['SoaringDynamic'],
            ':TaskDistance' => $taskData['TaskDistance'],
            ':TotalDistance' => $taskData['TotalDistance'],
            ':RecommendedAddOns' => $taskData['RecommendedAddOns'],
            ':LatMin' => $taskData['LatMin'],
            ':LatMax' => $taskData['LatMax'],
            ':LongMin' => $taskData['LongMin'],
            ':LongMax' => $taskData['LongMax'],
            ':OwnerName' => $ownerName,
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
        if ($taskData['Mode'] == 'CreateTask') {
            logMessage("Completing task creation...");
        }
        else {
            logMessage("Updating task...");
        }

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

        // Check if a cover image was uploaded and process it
        if (isset($_FILES['cover'])) {

            // Check for upload errors
            if ($_FILES['cover']['error'] !== UPLOAD_ERR_OK) {
                logMessage("Cover image upload error code: " . $_FILES['cover']['error']);
                throw new Exception("Cover image upload failed with error code: " . $_FILES['cover']['error']);
            }
        
            $cover = $_FILES['cover'];
            $target_cover_dir = $fileRootPath . 'TaskBrowser/Covers/';
            $target_cover_file = $target_cover_dir . basename($taskData['EntrySeqID'] . '.jpg');
        
            // Ensure the destination directory exists
            if (!is_dir($target_cover_dir)) {
                if (!mkdir($target_cover_dir, 0775, true) && !is_dir($target_cover_dir)) {
                    logMessage("Failed to create cover image directory");
                    throw new Exception("Failed to create cover image directory: " . $target_cover_dir);
                }
            }
        
            // Verify the uploaded file actually exists before moving
            if (!file_exists($cover['tmp_name'])) {
                logMessage("Cover image temporary file does not exist");
                throw new Exception("Cover image temporary file does not exist: " . $cover['tmp_name']);
            }
        
            // Move the uploaded file
            if (!move_uploaded_file($cover['tmp_name'], $target_cover_file)) {
                logMessage("Failed to upload the cover image file");
                throw new Exception('Failed to upload the cover image file.');
            }
        
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
                TaskID = :RealTaskID,
                DiscordPostID = :DiscordPostID,
                Title = :Title, 
                LastUpdate = :LastUpdate,
                SimDateTime = :SimDateTime,
                IncludeYear = :IncludeYear,
                SimDateTimeExtraInfo = :SimDateTimeExtraInfo, 
                MainAreaPOI = :MainAreaPOI,
                DepartureName = :DepartureName, 
                DepartureICAO = :DepartureICAO, 
                DepartureExtra = :DepartureExtra,
                ArrivalName = :ArrivalName, 
                ArrivalICAO = :ArrivalICAO, 
                ArrivalExtra = :ArrivalExtra, 
                SoaringRidge = :SoaringRidge, 
                SoaringThermals = :SoaringThermals, 
                SoaringWaves = :SoaringWaves, 
                SoaringDynamic = :SoaringDynamic,
                SoaringExtraInfo = :SoaringExtraInfo, 
                DurationMin = :DurationMin, 
                DurationMax = :DurationMax,
                DurationExtraInfo = :DurationExtraInfo, 
                TaskDistance = :TaskDistance,
                TotalDistance = :TotalDistance,
                RecommendedGliders = :RecommendedGliders, 
                DifficultyRating = :DifficultyRating,
                DifficultyExtraInfo = :DifficultyExtraInfo, 
                ShortDescription = :ShortDescription,
                LongDescription = :LongDescription, 
                WeatherSummary = :WeatherSummary, 
                Credits = :Credits, 
                Countries = :Countries,
                RecommendedAddOns = :RecommendedAddOns,
                RecommendedAddOnsList = :RecommendedAddOnsList, 
                MapImage = :MapImage,
                CoverImage = :CoverImage, 
                DBEntryUpdate = :DBEntryUpdate, 
                PLNFilename = :PLNFilename, 
                PLNXML = :PLNXML,
                WPRFilename = :WPRFilename, 
                WPRXML = :WPRXML, 
                LatMin = :LatMin, 
                LatMax = :LatMax, 
                LongMin = :LongMin, 
                LongMax = :LongMax,
                RepostText = :RepostText, 
                SuppressBaroPressureWarningSymbol = :SuppressBaroPressureWarningSymbol,
                BaroPressureExtraInfo = :BaroPressureExtraInfo, 
                ExtraFilesList = :ExtraFilesList, 
                Status = :Status,
                LastUpdateDescription = :LastUpdateDescription,
                OwnerName = :OwnerName, 
                SharedWith = :SharedWith,
                Availability = :Availability,
                NormalPostContent = :NormalPostContent,
                AvailabilityPostContent = :AvailabilityPostContent
            WHERE EntrySeqID = :EntrySeqID
        ");

        // Ensure datetime fields are properly formatted
        $taskData['LastUpdate'] = formatDatetime($taskData['LastUpdate']);
        $taskData['SimDateTime'] = formatDatetime($taskData['SimDateTime']);
        $taskData['DBEntryUpdate'] = formatDatetime($taskData['DBEntryUpdate']);
        $taskData['Availability'] = formatDatetime($taskData['Availability']);

        // Handle the task data
        $mapImage = isset($taskData['MapImage']) && !empty($taskData['MapImage']) ? base64_decode($taskData['MapImage']) : null;
        $coverImage = isset($taskData['CoverImage']) && !empty($taskData['CoverImage']) ? base64_decode($taskData['CoverImage']) : null;

        // Handle task posting to Discord
        $nowUTC = (new DateTime('now', new DateTimeZone('UTC')))->getTimestamp();
        $availabilityTimestamp = !empty($taskData['Availability']) 
            ? DateTime::createFromFormat('Y-m-d H:i:s', $taskData['Availability'], new DateTimeZone('UTC'))->getTimestamp()
            : null;
        if ($availabilityTimestamp !== null && $availabilityTimestamp > $nowUTC) {
            // Task should be unavailable - we'll use AvailabilityPostContent
            $discordMsg = $taskData['AvailabilityPostContent'];
        } else {
            // Task is available - we'll use NormalPostContent
            $discordMsg = $taskData['NormalPostContent'];
        }

        // Handle task posting to Discord using the $disWHFlights webhook
        $discordLogMessage = '';
        if (trim($discordMsg) === "") {
            // No content provided; skip Discord operations.
            $discordResponse = [
                "result" => "success",
                "postID" => (isset($taskData['DiscordPostID']) ? $taskData['DiscordPostID'] : null)
            ];
            $discordLogMessage = 'No Discord content provided - skipping Discord operations.';
        } else {
            if (isset($taskData['DiscordPostID']) && !empty($taskData['DiscordPostID'])) {
                // Retrieve the existing Discord message content
                $retrieveResult = getDiscordMessageContent($disWHFlights, $taskData['DiscordPostID']);
                $retrieveResponse = json_decode($retrieveResult, true);

                if ($retrieveResponse['result'] !== "success") {
                    // Retrieval failed; likely the webhook is not the author
                    $discordResponse = [
                        "result" => "error",
                        "error" => "Could not retrieve existing Discord post. (" . $taskData['DiscordPostID'] . ")",
                        "postID" => (isset($taskData['DiscordPostID']) ? $taskData['DiscordPostID'] : null)
                    ];
                } else {
                    $existingContent = $retrieveResponse['content'];
                    // Compare the retrieved content with the new content
                    if ($existingContent === $discordMsg) {
                        // Content unchanged; reuse the existing Discord post ID without updating
                        $discordResponse = [
                            "result" => "success",
                            "postID" => $taskData['DiscordPostID']
                        ];
                        $discordLogMessage = "Discord post content is the same - no update required. (" . $taskData['DiscordPostID'] . ")";
                    } else {
                        // Content changed; update the Discord post.
                        $discordResult = manageDiscordPost($disWHFlights, $discordMsg, $taskData['DiscordPostID'], false);
                        $discordResponse = json_decode($discordResult, true);
                        $discordLogMessage = "Discord post content updated. (" . $taskData['DiscordPostID'] . ")";
                    }
                }
            } else {
                // No existing Discord post: create a new one.
                $discordResult = manageDiscordPost($disWHFlights, $discordMsg, null, false);
                $discordResponse = json_decode($discordResult, true);
                $discordLogMessage = "Discord post content created. (" . $taskData['DiscordPostID'] . ")";
            }
        }

        if ($discordResponse['result'] === "success") {
            // Update the task data with the returned Discord post ID
            $taskData['DiscordPostID'] = $discordResponse['postID'];
        } else {
            // Return the error message to the caller
            $discordError = $discordResponse['error'];
            $discordLogMessage = $discordError;
        }
        logMessage($discordLogMessage);

        // Perform the update
        $stmt->execute([
            ':RealTaskID' => $taskData['RealTaskID'],
            ':Title' => $taskData['Title'],
            ':DiscordPostID' => $taskData['DiscordPostID'],
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
            ':Status' => $taskData['Status'],
            ':EntrySeqID' => $taskData['EntrySeqID'],
            ':LastUpdateDescription' => $taskData['LastUpdateDescription'],
            ':OwnerName' => $ownerName,
            ':SharedWith' => $sharedWith,
            ':NormalPostContent' => $taskData['NormalPostContent'],
            ':AvailabilityPostContent' => $taskData['AvailabilityPostContent'],
            ':Availability' => $taskData['Availability']
        ]);

        // Call createOrUpdateTaskNewsEntry if Status = 99
        if ($taskData['Status'] === 99) {
            // Add TaskID to $taskData and set it to RealTaskID
            $taskData['TaskID'] = $taskData['RealTaskID'];
            // Call the function with updated $taskData
            createOrUpdateTaskNewsEntry($taskData, false);
            cleanUpPendingTasks($pdo);
        }

        $output = [
            'status'           => 'success',
            'message'          => 'Task updated successfully.',
            'discordError'     => isset($discordError) ? $discordError : ''
        ];
        echo json_encode($output);
        logMessage("Task update successfully completed for TaskID: " . $taskData['RealTaskID']);
        if ($output['discordError'] !== '') {
            logMessage("Unable to handle Discord post: " . $output['discordError']);
        }
    }
} catch (Exception $e) {
    logMessage("Error: " . $e->getMessage());
    echo json_encode(['status' => 'error', 'message' => $e->getMessage()]);
}
?>
