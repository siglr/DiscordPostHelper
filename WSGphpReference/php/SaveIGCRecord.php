<?php
require __DIR__ . '/CommonFunctions.php';
require_once __DIR__ . '/HomeLeaderboardCache.php';

const EVENT_MATCH_WINDOW_BEFORE_SECONDS = 15 * 60; // 15 minutes
const EVENT_MATCH_WINDOW_AFTER_SECONDS = 30 * 60;  // 30 minutes

function bindNullableString($stmt, string $parameter, $value): void
{
    if ($value === null) {
        $stmt->bindValue($parameter, null, PDO::PARAM_NULL);
    } else {
        $stmt->bindValue($parameter, $value, PDO::PARAM_STR);
    }
}

function normalizeUtcDateTime(string $value): ?string
{
    $value = trim($value);
    if ($value === '') {
        return null;
    }

    // Handle compact YYMMDDhhmmss format (e.g. 231103205320 → 2023-11-03 20:53:20)
    if (preg_match('/^\d{12}$/', $value) === 1) {
        $yy = substr($value, 0, 2);
        $mm = substr($value, 2, 2);
        $dd = substr($value, 4, 2);
        $hh = substr($value, 6, 2);
        $mi = substr($value, 8, 2);
        $ss = substr($value, 10, 2);

        $year = 2000 + (int) $yy;

        try {
            $dt = new DateTimeImmutable(
                sprintf('%04d-%02d-%02d %02d:%02d:%02d', $year, $mm, $dd, $hh, $mi, $ss),
                new DateTimeZone('UTC')
            );

            return $dt->format('Y-m-d H:i:s');
        } catch (Exception $e) {
            return null;
        }
    }

    $timestamp = strtotime($value);
    if ($timestamp === false) {
        return null;
    }

    return gmdate('Y-m-d H:i:s', $timestamp);
}

function findMatchingClubEvent(PDO $pdo, int $entrySeqId, string $igcRecordDateTimeUtc): ?string
{
    $normalizedIgcTime = normalizeUtcDateTime($igcRecordDateTimeUtc);
    if ($normalizedIgcTime === null) {
        return null;
    }

    $dt = DateTimeImmutable::createFromFormat('Y-m-d H:i:s', $normalizedIgcTime, new DateTimeZone('UTC'));
    if ($dt === false) {
        return null;
    }

    $igcEpoch = $dt->getTimestamp();

    $sql = <<<SQL
        SELECT
            ClubEventNewsID,
            EventDateTime,
            abs(:IgcEpoch - strftime('%s', EventDateTime)) AS TimeDifference
        FROM TaskEvents
        WHERE EntrySeqID = :EntrySeqID
          AND ClubEventNewsID IS NOT NULL
          AND trim(ClubEventNewsID) != ''
          AND :IgcEpoch BETWEEN (strftime('%s', EventDateTime) - :BeforeWindow)
                           AND (strftime('%s', EventDateTime) + :AfterWindow)
        ORDER BY TimeDifference ASC,
                 EventDateTime ASC,
                 ClubEventNewsID ASC
        LIMIT 1
SQL;

    $stmt = $pdo->prepare($sql);
    $stmt->bindValue(':EntrySeqID', $entrySeqId, PDO::PARAM_INT);
    $stmt->bindValue(':IgcEpoch', $igcEpoch, PDO::PARAM_INT);
    $stmt->bindValue(':BeforeWindow', EVENT_MATCH_WINDOW_BEFORE_SECONDS, PDO::PARAM_INT);
    $stmt->bindValue(':AfterWindow', EVENT_MATCH_WINDOW_AFTER_SECONDS, PDO::PARAM_INT);
    $stmt->execute();

    $row = $stmt->fetch(PDO::FETCH_ASSOC);
    if (!$row) {
        return null;
    }

    $clubEventNewsId = $row['ClubEventNewsID'] ?? null;

    if ($clubEventNewsId === null || trim($clubEventNewsId) === '') {
        return null;
    }

    return $clubEventNewsId;
}

function isIgcRecordEligible(PDO $pdo, string $igcKey): bool
{
    $eligibilityQuery = "
        SELECT 1
        FROM IGCRecords IGC
        JOIN Tasks T ON IGC.EntrySeqID = T.EntrySeqID
        WHERE IGC.IGCKey = :IGCKey
          AND COALESCE(IGC.IsPrivate, 0) = 0
          AND IGC.IGCValid = 1
          AND COALESCE(IGC.MarkedAsDesigner, 0) <> 1
          AND IGC.TaskCompleted = 1
          AND abs(strftime('%s', T.SimDateTime) - strftime(
                '%s',
                substr(T.SimDateTime, 1, 4) || '-' ||
                substr(IGC.LocalDate, 6, 5) || ' ' ||
                substr(IGC.LocalTime, 1, 2) || ':' ||
                substr(IGC.LocalTime, 3, 2) || ':' ||
                substr(IGC.LocalTime, 5, 2)
          )) <= 1800
        LIMIT 1
    ";

    $stmt = $pdo->prepare($eligibilityQuery);
    $stmt->bindValue(':IGCKey', $igcKey, PDO::PARAM_STR);
    $stmt->execute();

    return $stmt->fetchColumn() !== false;
}

function findBestEligiblePerformance(
    PDO $pdo,
    int $entrySeqId,
    string $sim,
    $competitionClass,
    $gliderType,
    ?string $excludeIgcKey = null
) {
    $bestQuery = "
        SELECT
            IGC.IGCKey,
            IGC.Speed,
            IGC.IGCUploadDateTimeUTC
        FROM IGCRecords IGC
        JOIN Tasks T ON IGC.EntrySeqID = T.EntrySeqID
        WHERE IGC.EntrySeqID = :EntrySeqID
          AND IGC.Sim = :Sim
          AND COALESCE(IGC.IsPrivate, 0) = 0
          AND ((IGC.CompetitionClass IS NULL AND :CompetitionClass IS NULL) OR IGC.CompetitionClass = :CompetitionClass)
          AND ((IGC.GliderType IS NULL AND :GliderType IS NULL) OR IGC.GliderType = :GliderType)
          AND IGC.TaskCompleted = 1
          AND IGC.IGCValid = 1
          AND COALESCE(IGC.MarkedAsDesigner, 0) <> 1
          AND abs(strftime('%s', T.SimDateTime) - strftime(
                '%s',
                substr(T.SimDateTime, 1, 4) || '-' ||
                substr(IGC.LocalDate, 6, 5) || ' ' ||
                substr(IGC.LocalTime, 1, 2) || ':' ||
                substr(IGC.LocalTime, 3, 2) || ':' ||
                substr(IGC.LocalTime, 5, 2)
          )) <= 1800
    ";

    if ($excludeIgcKey !== null) {
        $bestQuery .= "\n          AND IGC.IGCKey != :ExcludeIGCKey";
    }

    $bestQuery .= "\n        ORDER BY IGC.Speed DESC, IGC.IGCUploadDateTimeUTC ASC";

    $stmt = $pdo->prepare($bestQuery);
    $stmt->bindValue(':EntrySeqID', $entrySeqId, PDO::PARAM_INT);
    $stmt->bindValue(':Sim', $sim, PDO::PARAM_STR);
    bindNullableString($stmt, ':CompetitionClass', $competitionClass);
    bindNullableString($stmt, ':GliderType', $gliderType);
    if ($excludeIgcKey !== null) {
        $stmt->bindValue(':ExcludeIGCKey', $excludeIgcKey, PDO::PARAM_STR);
    }
    $stmt->execute();

    $bestRow = null;
    $bestRoundedSpeed = null;

    while ($row = $stmt->fetch(PDO::FETCH_ASSOC)) {
        if ($row['Speed'] === null) {
            continue;
        }

        $roundedSpeed = (float) number_format((float) $row['Speed'], 1, '.', '');

        if ($bestRow === null || $roundedSpeed > $bestRoundedSpeed) {
            $bestRow = $row;
            $bestRoundedSpeed = $roundedSpeed;
            continue;
        }

        if ($roundedSpeed === $bestRoundedSpeed) {
            $currentUpload = $row['IGCUploadDateTimeUTC'] ?? null;
            $bestUpload = $bestRow['IGCUploadDateTimeUTC'] ?? null;

            if ($currentUpload !== null && ($bestUpload === null || strcmp($currentUpload, $bestUpload) < 0)) {
                $bestRow = $row;
                $bestRoundedSpeed = $roundedSpeed;
            }
        }
    }

    if ($bestRow === null) {
        return null;
    }

    $bestRow['RoundedSpeed'] = $bestRoundedSpeed;

    return $bestRow;
}

header('Content-Type: application/json');

// toggle this to turn debug logging on or off
$loggingEnabled = false;

try {
    if ($loggingEnabled) {
        logMessage("saveIGCRecord called with POST: " . print_r($_POST, true));
    }

    // Check required POST parameters.
    $required = [
        'IGCKey',
        'EntrySeqID',
        'IGCRecordDateTimeUTC',
        'IGCUploadDateTimeUTC',
        'LocalDate',
        'LocalTime',
        'BeginTimeUTC',
        'Pilot',
        'GliderType',
        'GliderID',
        'CompetitionID',
        'CompetitionClass',
        'NB21Version',
        'Sim',
        'WSGUserID'
    ];
    
    foreach ($required as $field) {
        if (!isset($_POST[$field]) || trim($_POST[$field]) === "") {
            throw new Exception("Missing required field: $field");
        }
    }
    
    // Retrieve parameters.
    $IGCKey = trim($_POST['IGCKey']);
    $EntrySeqID = (int) $_POST['EntrySeqID'];
    $IGCRecordDateTimeUTC = trim($_POST['IGCRecordDateTimeUTC']);
    $IGCUploadDateTimeUTC = trim($_POST['IGCUploadDateTimeUTC']);
    $LocalDate = trim($_POST['LocalDate']);
    $LocalTime = trim($_POST['LocalTime']);
    $BeginTimeUTC = trim($_POST['BeginTimeUTC']);
    $Pilot = trim($_POST['Pilot']);
    $GliderType = trim($_POST['GliderType']);
    $GliderID = trim($_POST['GliderID']);
    $CompetitionID = trim($_POST['CompetitionID']);
    $CompetitionClass = trim($_POST['CompetitionClass']);
    $NB21Version = trim($_POST['NB21Version']);
    $Sim = trim($_POST['Sim']);
    $IGCComment = isset($_POST['IGCComment']) ? trim($_POST['IGCComment']) : "";
    $WSGUserID = (int) trim($_POST['WSGUserID']);

    
    // Instead of ensuring an uploaded file exists in $_FILES, 
    // locate the temp folder (case-insensitive)
    $tempDir      = __DIR__ . '/DPHXTemp';
    $sourceFolder = null;
    foreach (scandir($tempDir) as $entry) {
        if (strcasecmp($entry, $IGCKey) === 0 && is_dir("$tempDir/$entry")) {
            $sourceFolder = "$tempDir/$entry";
            break;
        }
    }
    if (!$sourceFolder) {
        throw new Exception("Temp folder not found for IGCKey: $IGCKey");
    }

    // locate the .igc file (case-insensitive)
    $expected = $IGCKey . '.igc';
    $found    = null;
    foreach (scandir($sourceFolder) as $file) {
        if (strcasecmp($file, $expected) === 0) {
            $found = $file;
            break;
        }
    }
    if (!$found) {
        throw new Exception("IGC file not found in $sourceFolder");
    }
    $sourceFilePath = "$sourceFolder/$found";

    if ($loggingEnabled) {
        logMessage("saveIGCRecord tempDir      = $tempDir");
        logMessage("saveIGCRecord sourceFolder = $sourceFolder");
        logMessage("saveIGCRecord sourceFile   = $sourceFilePath");
    }

    if (!file_exists($sourceFilePath)) {
        throw new Exception("IGC file not found in temporary folder.");
    }
    
    // Open the database connection.
    $pdo = new PDO("sqlite:$databasePath");
    $pdo->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);

    $isPrivate = 0;
    $privacyDefaultStmt = $pdo->prepare(
        'SELECT COALESCE(IGCPrivateDefault, 0) AS IGCPrivateDefault
         FROM Users
         WHERE WSGUserID = :WSGUserID
         LIMIT 1'
    );
    $privacyDefaultStmt->bindValue(':WSGUserID', $WSGUserID, PDO::PARAM_INT);
    $privacyDefaultStmt->execute();
    $privacyRow = $privacyDefaultStmt->fetch(PDO::FETCH_ASSOC);
    if ($privacyRow) {
        $isPrivate = (int) ($privacyRow['IGCPrivateDefault'] ?? 0) === 1 ? 1 : 0;
    }
    
    // Check if a record with the same IGCKey already exists.
    $checkQuery = "SELECT * FROM IGCRecords WHERE IGCKey = :igcKey";
    $stmt = $pdo->prepare($checkQuery);
    $stmt->bindParam(':igcKey', $IGCKey, PDO::PARAM_STR);
    $stmt->execute();
    $existingRecord = $stmt->fetch(PDO::FETCH_ASSOC);
    
    if ($existingRecord) {
        echo json_encode([
            'status' => 'duplicate',
            'message' => 'An IGC record with this key already exists.'
        ]);
        exit;
    }
    
    // Determine the destination folder.
    $destFolder = rtrim($taskBrowserPath, '/\\') . '/IGCFiles/' . $EntrySeqID;
    if (!is_dir($destFolder)) {
        if (!mkdir($destFolder, 0755, true)) {
            throw new Exception("Failed to create destination folder: $destFolder");
        }
    }
    
    // Destination filename: [IGCKey].igc
    $destFilename = $destFolder . '/' . $IGCKey . '.igc';
    
    // Move the saved IGC file from its temporary folder to the official destination.
    if (!rename($sourceFilePath, $destFilename)) {
        throw new Exception("Failed to move saved IGC file to destination folder.");
    }

    // Read the results from the results.json file ===
    $resultsFile = $sourceFolder . '/results.json';
    if (!file_exists($resultsFile)) {
        throw new Exception("Results file not found in temporary folder.");
    }
    $resultsContent = file_get_contents($resultsFile);
    $parsedResults = json_decode($resultsContent, true);
    if (json_last_error() !== JSON_ERROR_NONE) {
        throw new Exception("Failed to decode results file: " . json_last_error_msg());
    }

    // Extract the new TPVersion (planner version)
    $tpVersion = isset($parsedResults['TPVersion']) ? $parsedResults['TPVersion'] : null;

    // Convert boolean values to integers.
    $taskCompletedInt = !empty($parsedResults["TaskCompleted"]) ? 1 : 0;
    $penaltiesInt     = !empty($parsedResults["Penalties"])    ? 1 : 0;
    $igcValidInt      = !empty($parsedResults["IGCValid"])     ? 1 : 0;

    // Convert Duration from "HH:MM:SS" to seconds.
    $durationSeconds = 0;
    if (!empty($parsedResults["Duration"])) {
        list($hours, $minutes, $seconds) = explode(":", $parsedResults["Duration"]);
        $durationSeconds = ((int)$hours * 3600) + ((int)$minutes * 60) + ((int)$seconds);
    }

    // Convert Distance and Speed to float.
    $distanceFloat = (!empty($parsedResults["Distance"])) ? (float)$parsedResults["Distance"] : null;
    $speedFloat    = (!empty($parsedResults["Speed"]))    ? (float)$parsedResults["Speed"]    : null;

    // Insert the new record into IGCRecords table.
    // Note the added TPVersion column and placeholder.
    $matchedClubEventNewsID = findMatchingClubEvent($pdo, $EntrySeqID, $IGCRecordDateTimeUTC);

    $insertQuery = "
      INSERT INTO IGCRecords (
        IGCKey, EntrySeqID, IGCRecordDateTimeUTC, IGCUploadDateTimeUTC, LocalTime,
        BeginTimeUTC, Pilot, GliderType, GliderID, CompetitionID,
        CompetitionClass, NB21Version, Sim, WSGUserID, Comment,
        TaskCompleted, Penalties, Duration, Distance, Speed, IGCValid,
        TPVersion, LocalDate, ClubEventNewsID, IsPrivate
      ) VALUES (
        :IGCKey, :EntrySeqID, :IGCRecordDateTimeUTC, :IGCUploadDateTimeUTC, :LocalTime,
        :BeginTimeUTC, :Pilot, :GliderType, :GliderID, :CompetitionID,
        :CompetitionClass, :NB21Version, :Sim, :WSGUserID, :Comment,
        :TaskCompleted, :Penalties, :Duration, :Distance, :Speed, :IGCValid,
        :TPVersion, :LocalDate, :ClubEventNewsID, :IsPrivate
      )
    ";

    $stmt = $pdo->prepare($insertQuery);
    $stmt->bindParam(':IGCKey',                 $IGCKey,               PDO::PARAM_STR);
    $stmt->bindParam(':EntrySeqID',             $EntrySeqID,           PDO::PARAM_INT);
    $stmt->bindParam(':IGCRecordDateTimeUTC',   $IGCRecordDateTimeUTC, PDO::PARAM_STR);
    $stmt->bindParam(':IGCUploadDateTimeUTC',   $IGCUploadDateTimeUTC, PDO::PARAM_STR);
    $stmt->bindParam(':LocalDate',              $LocalDate,            PDO::PARAM_STR);
    $stmt->bindParam(':LocalTime',              $LocalTime,            PDO::PARAM_STR);
    $stmt->bindParam(':BeginTimeUTC',           $BeginTimeUTC,         PDO::PARAM_STR);
    $stmt->bindParam(':Pilot',                  $Pilot,                PDO::PARAM_STR);
    $stmt->bindParam(':GliderType',             $GliderType,           PDO::PARAM_STR);
    $stmt->bindParam(':GliderID',               $GliderID,             PDO::PARAM_STR);
    $stmt->bindParam(':CompetitionID',          $CompetitionID,        PDO::PARAM_STR);
    $stmt->bindParam(':CompetitionClass',       $CompetitionClass,     PDO::PARAM_STR);
    $stmt->bindParam(':NB21Version',            $NB21Version,          PDO::PARAM_STR);
    $stmt->bindParam(':Sim',                    $Sim,                  PDO::PARAM_STR);
    $stmt->bindParam(':WSGUserID',              $WSGUserID,            PDO::PARAM_INT);
    $stmt->bindParam(':Comment',                $IGCComment,           PDO::PARAM_STR);
    $stmt->bindParam(':TaskCompleted',          $taskCompletedInt,     PDO::PARAM_INT);
    $stmt->bindParam(':Penalties',              $penaltiesInt,         PDO::PARAM_INT);
    $stmt->bindParam(':Duration',               $durationSeconds,      PDO::PARAM_INT);
    $stmt->bindParam(':Distance',               $distanceFloat);
    $stmt->bindParam(':Speed',                  $speedFloat);
    $stmt->bindParam(':IGCValid',               $igcValidInt,          PDO::PARAM_INT);
    $stmt->bindParam(':TPVersion',              $tpVersion,            PDO::PARAM_STR);
    $stmt->bindParam(':IsPrivate',              $isPrivate,            PDO::PARAM_INT);
    bindNullableString($stmt, ':ClubEventNewsID', $matchedClubEventNewsID);

    $stmt->execute();

    $isEligibleForBest = isIgcRecordEligible($pdo, $IGCKey);
    $bestChanged = false;

    if ($isEligibleForBest) {
        $bestEligible = findBestEligiblePerformance(
            $pdo,
            $EntrySeqID,
            $Sim,
            $CompetitionClass,
            $GliderType
        );

        if ($bestEligible !== null) {
            $selectBestQuery = "
                SELECT IGCKey
                FROM TaskBestPerformances
                WHERE EntrySeqID = :EntrySeqID
                  AND Sim = :Sim
                  AND ((CompetitionClass IS NULL AND :CompetitionClass IS NULL) OR CompetitionClass = :CompetitionClass)
                  AND ((GliderType IS NULL AND :GliderType IS NULL) OR GliderType = :GliderType)
                LIMIT 1
            ";

            $stmt = $pdo->prepare($selectBestQuery);
            $stmt->bindValue(':EntrySeqID', $EntrySeqID, PDO::PARAM_INT);
            $stmt->bindValue(':Sim', $Sim, PDO::PARAM_STR);
            bindNullableString($stmt, ':CompetitionClass', $CompetitionClass);
            bindNullableString($stmt, ':GliderType', $GliderType);
            $stmt->execute();
            $existingBest = $stmt->fetch(PDO::FETCH_ASSOC);

            if (!$existingBest) {
                $insertBestQuery = "
                    INSERT INTO TaskBestPerformances (EntrySeqID, Sim, CompetitionClass, GliderType, IGCKey)
                    VALUES (:EntrySeqID, :Sim, :CompetitionClass, :GliderType, :IGCKey)
                ";

                $stmt = $pdo->prepare($insertBestQuery);
                $stmt->bindValue(':EntrySeqID', $EntrySeqID, PDO::PARAM_INT);
                $stmt->bindValue(':Sim', $Sim, PDO::PARAM_STR);
                bindNullableString($stmt, ':CompetitionClass', $CompetitionClass);
                bindNullableString($stmt, ':GliderType', $GliderType);
                $stmt->bindValue(':IGCKey', $bestEligible['IGCKey'], PDO::PARAM_STR);
                $stmt->execute();

                $bestChanged = true;

                if ($loggingEnabled) {
                    logMessage("Inserted new TaskBestPerformances row for EntrySeqID {$EntrySeqID}, Sim {$Sim}.");
                }
            } else {
                $currentBestKey = $existingBest['IGCKey'];

                if ($currentBestKey !== $bestEligible['IGCKey']) {
                    $updateBestQuery = "
                        UPDATE TaskBestPerformances
                        SET IGCKey = :IGCKey
                        WHERE EntrySeqID = :EntrySeqID
                          AND Sim = :Sim
                          AND ((CompetitionClass IS NULL AND :CompetitionClass IS NULL) OR CompetitionClass = :CompetitionClass)
                          AND ((GliderType IS NULL AND :GliderType IS NULL) OR GliderType = :GliderType)
                    ";

                    $stmt = $pdo->prepare($updateBestQuery);
                    $stmt->bindValue(':IGCKey', $bestEligible['IGCKey'], PDO::PARAM_STR);
                    $stmt->bindValue(':EntrySeqID', $EntrySeqID, PDO::PARAM_INT);
                    $stmt->bindValue(':Sim', $Sim, PDO::PARAM_STR);
                    bindNullableString($stmt, ':CompetitionClass', $CompetitionClass);
                    bindNullableString($stmt, ':GliderType', $GliderType);
                    $stmt->execute();

                    $bestChanged = true;

                    if ($loggingEnabled) {
                        logMessage("Updated TaskBestPerformances for EntrySeqID {$EntrySeqID}, Sim {$Sim} to IGCKey {$bestEligible['IGCKey']}.");
                    }
                }
            }
        }
    }

    if ($bestChanged) {
        try {
            refreshHomeLeaderboardCaches($pdo, $Sim, $CompetitionClass, $GliderType);
        } catch (Throwable $cacheException) {
            if (function_exists('logMessage')) {
                logMessage('Failed to refresh home leaderboard cache after save: ' . $cacheException->getMessage());
            }
        }
    }

    // Transform IGCRecordDateTimeUTC to the desired format for saving in MarkedFlownDateUTC.
    if (!empty($IGCRecordDateTimeUTC) && strlen($IGCRecordDateTimeUTC) === 12) {
        $yy = (int) substr($IGCRecordDateTimeUTC, 0, 2);
        $mm = (int) substr($IGCRecordDateTimeUTC, 2, 2);
        $dd = (int) substr($IGCRecordDateTimeUTC, 4, 2);
        $HH = (int) substr($IGCRecordDateTimeUTC, 6, 2);
        $mi = (int) substr($IGCRecordDateTimeUTC, 8, 2);
        $fullYear = $yy + 2000;
        $formattedDate = sprintf("%04d-%02d-%02d %02d:%02d", $fullYear, $mm, $dd, $HH, $mi);
    } else {
        $formattedDate = $IGCRecordDateTimeUTC;
    }

    // Now, create or update the corresponding record in the UsersTasks table.
    // Check if a record already exists for this WSGUserID and EntrySeqID.
    $checkTaskQuery = "SELECT * FROM UsersTasks WHERE WSGUserID = :WSGUserID AND EntrySeqID = :EntrySeqID";
    $stmt = $pdo->prepare($checkTaskQuery);
    $stmt->bindParam(':WSGUserID', $WSGUserID, PDO::PARAM_INT);
    $stmt->bindParam(':EntrySeqID', $EntrySeqID, PDO::PARAM_INT);
    $stmt->execute();
    $existingTask = $stmt->fetch(PDO::FETCH_ASSOC);

    if ($existingTask) {
        // Determine if we should update: update if MarkedFlownDateUTC is empty or
        // if the new formatted date is more recent.
        $currentMarked = $existingTask['MarkedFlownDateUTC'];
        if (empty($currentMarked) || strtotime($formattedDate) > strtotime($currentMarked)) {
            $updateTaskQuery = "UPDATE UsersTasks SET MarkedFlownDateUTC = :markedDate 
                WHERE WSGUserID = :WSGUserID AND EntrySeqID = :EntrySeqID";
            $stmt = $pdo->prepare($updateTaskQuery);
            $stmt->bindParam(':markedDate', $formattedDate, PDO::PARAM_STR);
            $stmt->bindParam(':WSGUserID', $WSGUserID, PDO::PARAM_INT);
            $stmt->bindParam(':EntrySeqID', $EntrySeqID, PDO::PARAM_INT);
            $stmt->execute();
        }
    } else {
        // Insert a new record with the formatted MarkedFlownDateUTC value.
        $insertTaskQuery = "INSERT INTO UsersTasks (WSGUserID, EntrySeqID, MarkedFlownDateUTC)
            VALUES (:WSGUserID, :EntrySeqID, :markedDate)";
        $stmt = $pdo->prepare($insertTaskQuery);
        $stmt->bindParam(':WSGUserID', $WSGUserID, PDO::PARAM_INT);
        $stmt->bindParam(':EntrySeqID', $EntrySeqID, PDO::PARAM_INT);
        $stmt->bindParam(':markedDate', $formattedDate, PDO::PARAM_STR);
        $stmt->execute();
    }

    // === Trigger update of latest IGC leaders file, only if valid & completed ===
    if ($taskCompletedInt === 1 && $igcValidInt === 1) {
        $updateScript = __DIR__ . '/UpdateLatestIGCLeaders.php';
        if (file_exists($updateScript)) {
            shell_exec("php " . escapeshellarg($updateScript) . " > /dev/null 2>&1 &");
            if ($loggingEnabled) {
                logMessage("Triggered UpdateLatestIGCLeaders.php (valid + completed).");
            }
        }
    }

    echo json_encode([
        'status' => 'success',
        'message' => 'IGC record saved successfully.',
        'IGCKey' => $IGCKey
    ]);
    
} catch (Exception $e) {
    echo json_encode(['error' => $e->getMessage()]);
    exit;
}
?>
