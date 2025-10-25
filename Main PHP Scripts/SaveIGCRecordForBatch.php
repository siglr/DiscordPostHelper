<?php
require __DIR__ . '/CommonFunctions.php';
require_once __DIR__ . '/HomeLeaderboardCache.php';
header('Content-Type: application/json');

// toggle this to turn debug logging on or off
$loggingEnabled = false;

// ─────────────────────────────────────────────
// Helper functions brought in from SaveIGCRecord.php
// ─────────────────────────────────────────────

function bindNullableString($stmt, string $parameter, $value): void
{
    if ($value === null || $value === '') {
        $stmt->bindValue($parameter, null, PDO::PARAM_NULL);
    } else {
        $stmt->bindValue($parameter, $value, PDO::PARAM_STR);
    }
}

function isIgcRecordEligible(PDO $pdo, string $igcKey): bool
{
    $eligibilityQuery = "
        SELECT 1
        FROM IGCRecords IGC
        JOIN Tasks T ON IGC.EntrySeqID = T.EntrySeqID
        WHERE IGC.IGCKey = :IGCKey
          AND IGC.IGCValid = 1
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
          AND ((IGC.CompetitionClass IS NULL AND :CompetitionClass IS NULL) OR IGC.CompetitionClass = :CompetitionClass)
          AND ((IGC.GliderType IS NULL AND :GliderType IS NULL) OR IGC.GliderType = :GliderType)
          AND IGC.TaskCompleted = 1
          AND IGC.IGCValid = 1
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
            $bestUpload    = $bestRow['IGCUploadDateTimeUTC'] ?? null;

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

// ─────────────────────────────────────────────
// Main
// ─────────────────────────────────────────────

try {
    if ($loggingEnabled && function_exists('logMessage')) {
        logMessage("SaveIGCRecordForBatch called with POST: " . print_r($_POST, true));
    }

    // 1) Required POST fields (parsed-results fields are now optional)
    $required = [
        'IGCKey','EntrySeqID','IGCRecordDateTimeUTC','IGCUploadDateTimeUTC',
        'LocalDate','LocalTime','BeginTimeUTC','Pilot','GliderType','GliderID',
        'CompetitionID','CompetitionClass','NB21Version','Sim'
    ];
    foreach ($required as $f) {
        if (!isset($_POST[$f]) || trim($_POST[$f]) === '') {
            throw new Exception("Missing required field: $f");
        }
    }
    if (!isset($_FILES['igcFile']) || $_FILES['igcFile']['error'] !== UPLOAD_ERR_OK) {
        throw new Exception("IGC file upload failed");
    }

    // 2) Pull the base parameters
    $IGCKey               = trim($_POST['IGCKey']);
    $EntrySeqID           = (int) $_POST['EntrySeqID'];
    $IGCRecordDateTimeUTC = trim($_POST['IGCRecordDateTimeUTC']);
    $IGCUploadDateTimeUTC = trim($_POST['IGCUploadDateTimeUTC']);
    $LocalDate            = trim($_POST['LocalDate']);
    $LocalTime            = trim($_POST['LocalTime']);
    $BeginTimeUTC         = trim($_POST['BeginTimeUTC']);
    $Pilot                = trim($_POST['Pilot']);
    $GliderType           = trim($_POST['GliderType']);
    $GliderID             = trim($_POST['GliderID']);
    $CompetitionID        = trim($_POST['CompetitionID']);
    $CompetitionClass     = trim($_POST['CompetitionClass']);
    $NB21Version          = trim($_POST['NB21Version']);
    $Sim                  = trim($_POST['Sim']);

    // Optional user ID; skip UsersTasks if zero or absent
    $WSGUserID            = isset($_POST['WSGUserID']) ? (int) $_POST['WSGUserID'] : 0;

    // UsersTasks payload + IGC comment
    $UT_InfoFetched        = isset($_POST['UT_InfoFetched']) ? (int) $_POST['UT_InfoFetched'] : 0;
    $UT_MarkedFlyNextUTC   = trim($_POST['UT_MarkedFlyNextUTC']   ?? '');
    $UT_MarkedFavoritesUTC = trim($_POST['UT_MarkedFavoritesUTC'] ?? '');
    $UT_DifficultyRating   = isset($_POST['UT_DifficultyRating']) ? (int) $_POST['UT_DifficultyRating'] : 0;
    $UT_QualityRating      = isset($_POST['UT_QualityRating'])    ? (int) $_POST['UT_QualityRating']    : 0;
    $UT_PrivateNote        = trim($_POST['UT_PrivateNote'] ?? '');
    $UT_PublicNote         = trim($_POST['UT_PublicNote']  ?? '');
    $IGCUserComment        = trim($_POST['IGCUserComment'] ?? '');

    // 3) Optional parsed-results fields
    $TaskCompletedInt = isset($_POST['TaskCompleted']) ? (int) $_POST['TaskCompleted'] : 0;
    $PenaltiesInt     = isset($_POST['Penalties'])     ? (int) $_POST['Penalties']     : 0;
    $DurationSeconds  = isset($_POST['Duration'])      ? (int) $_POST['Duration']      : 0; // seconds
    $DistanceFloat    = (isset($_POST['Distance']) && trim($_POST['Distance']) !== '') ? (float) $_POST['Distance'] : null;
    $SpeedFloat       = (isset($_POST['Speed'])    && trim($_POST['Speed'])    !== '') ? (float) $_POST['Speed']    : null;
    $IGCValidInt      = isset($_POST['IGCValid'])  ? (int) $_POST['IGCValid']  : 0;
    $TPVersion        = isset($_POST['TPVersion']) ? trim($_POST['TPVersion']) : null;

    // 4) Move uploaded IGC file into the per-task folder
    $destFolder = rtrim($taskBrowserPath, '/\\') . '/IGCFiles/' . $EntrySeqID;
    if (!is_dir($destFolder) && !mkdir($destFolder, 0755, true)) {
        throw new Exception("Failed to create folder: $destFolder");
    }
    $destPath = "$destFolder/$IGCKey.igc";
    if (!move_uploaded_file($_FILES['igcFile']['tmp_name'], $destPath)) {
        throw new Exception("Failed to move IGC file to destination");
    }

    if ($loggingEnabled && function_exists('logMessage')) {
        logMessage("IGC file saved to $destPath");
    }

    // 5) Insert into IGCRecords
    $pdo = new PDO("sqlite:$databasePath");
    $pdo->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);

    // Duplicate check
    $dupQ = "SELECT 1 FROM IGCRecords WHERE IGCKey = :IGCKey";
    $stmt = $pdo->prepare($dupQ);
    $stmt->execute([':IGCKey' => $IGCKey]);
    if ($stmt->fetch()) {
        echo json_encode(['status'=>'duplicate','message'=>'IGCKey already exists']);
        exit;
    }

    $insertQ = "
      INSERT INTO IGCRecords (
        IGCKey, EntrySeqID, IGCRecordDateTimeUTC, IGCUploadDateTimeUTC,
        LocalTime, BeginTimeUTC, Pilot, GliderType, GliderID, CompetitionID,
        CompetitionClass, NB21Version, Sim, WSGUserID, TaskCompleted, Penalties,
        Duration, Distance, Speed, IGCValid, TPVersion, LocalDate, Comment
      ) VALUES (
        :IGCKey, :EntrySeqID, :IGCRecordDateTimeUTC, :IGCUploadDateTimeUTC,
        :LocalTime, :BeginTimeUTC, :Pilot, :GliderType, :GliderID, :CompetitionID,
        :CompetitionClass, :NB21Version, :Sim, :WSGUserID, :TaskCompleted, :Penalties,
        :Duration, :Distance, :Speed, :IGCValid, :TPVersion, :LocalDate, :Comment
      )
    ";
    $stmt = $pdo->prepare($insertQ);
    $stmt->execute([
      ':IGCKey'               => $IGCKey,
      ':EntrySeqID'           => $EntrySeqID,
      ':IGCRecordDateTimeUTC' => $IGCRecordDateTimeUTC,
      ':IGCUploadDateTimeUTC' => $IGCUploadDateTimeUTC,
      ':LocalTime'            => $LocalTime,
      ':BeginTimeUTC'         => $BeginTimeUTC,
      ':Pilot'                => $Pilot,
      ':GliderType'           => $GliderType,
      ':GliderID'             => $GliderID,
      ':CompetitionID'        => $CompetitionID,
      ':CompetitionClass'     => $CompetitionClass,
      ':NB21Version'          => $NB21Version,
      ':Sim'                  => $Sim,
      ':WSGUserID'            => $WSGUserID,
      ':TaskCompleted'        => $TaskCompletedInt,
      ':Penalties'            => $PenaltiesInt,
      ':Duration'             => $DurationSeconds,
      ':Distance'             => $DistanceFloat,
      ':Speed'                => $SpeedFloat,
      ':IGCValid'             => $IGCValidInt,
      ':TPVersion'            => $TPVersion,
      ':LocalDate'            => $LocalDate,
      ':Comment'              => ($IGCUserComment !== '' ? $IGCUserComment : null)
    ]);

    // ─────────────────────────────────────────
    // 5b) Maintain TaskBestPerformances + cache
    // ─────────────────────────────────────────
    $bestChanged = false;

    if (isIgcRecordEligible($pdo, $IGCKey)) {

        $bestEligible = findBestEligiblePerformance(
            $pdo,
            $EntrySeqID,
            $Sim,
            $CompetitionClass,
            $GliderType
        );

        if ($bestEligible !== null) {
            // Check if we already have a row for this combo
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
                // insert new best row
                $insertBestQuery = "
                    INSERT INTO TaskBestPerformances
                        (EntrySeqID, Sim, CompetitionClass, GliderType, IGCKey)
                    VALUES
                        (:EntrySeqID, :Sim, :CompetitionClass, :GliderType, :IGCKey)
                ";
                $stmt = $pdo->prepare($insertBestQuery);
                $stmt->bindValue(':EntrySeqID', $EntrySeqID, PDO::PARAM_INT);
                $stmt->bindValue(':Sim', $Sim, PDO::PARAM_STR);
                bindNullableString($stmt, ':CompetitionClass', $CompetitionClass);
                bindNullableString($stmt, ':GliderType', $GliderType);
                $stmt->bindValue(':IGCKey', $bestEligible['IGCKey'], PDO::PARAM_STR);
                $stmt->execute();

                $bestChanged = true;

                if ($loggingEnabled && function_exists('logMessage')) {
                    logMessage("Inserted TaskBestPerformances for EntrySeqID {$EntrySeqID}, Sim {$Sim}.");
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

                    if ($loggingEnabled && function_exists('logMessage')) {
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
            if ($loggingEnabled && function_exists('logMessage')) {
                logMessage('Failed to refresh home leaderboard cache after batch save: ' . $cacheException->getMessage());
            }
        }
    }

    // 6) UsersTasks upsert whenever WSGUserID > 0 (IGC upload implies "flown")
    if ($WSGUserID > 0) {

        // Build MarkedFlownDateUTC from IGCRecordDateTimeUTC (YYMMDDHHMMSS); fallback to now-UTC
        if (strlen($IGCRecordDateTimeUTC) === 12) {
            $y  = 2000 + (int)substr($IGCRecordDateTimeUTC, 0, 2);
            $m  = (int)substr($IGCRecordDateTimeUTC, 2, 2);
            $d  = (int)substr($IGCRecordDateTimeUTC, 4, 2);
            $H  = (int)substr($IGCRecordDateTimeUTC, 6, 2);
            $i  = (int)substr($IGCRecordDateTimeUTC, 8, 2);
            $markedDate = sprintf("%04d-%02d-%02d %02d:%02d:00", $y,$m,$d,$H,$i);
        } else {
            $markedDate = gmdate('Y-m-d H:i:s');
        }

        // Normalize optional UT fields (null keeps existing on update)
        $valFlyNext = ($UT_MarkedFlyNextUTC   !== '') ? $UT_MarkedFlyNextUTC   : null;
        $valFav     = ($UT_MarkedFavoritesUTC !== '') ? $UT_MarkedFavoritesUTC : null;
        $valDiff    = ($UT_DifficultyRating   > 0)    ? $UT_DifficultyRating   : null;
        $valQual    = ($UT_QualityRating      > 0)    ? $UT_QualityRating      : null;
        $valPriv    = ($UT_PrivateNote        !== '') ? $UT_PrivateNote        : null;
        $valPub     = ($UT_PublicNote         !== '') ? $UT_PublicNote         : null;

        // Check if a row already exists
        $checkQ = "SELECT MarkedFlownDateUTC FROM UsersTasks WHERE WSGUserID=:u AND EntrySeqID=:e";
        $stmt   = $pdo->prepare($checkQ);
        $stmt->execute([':u'=>$WSGUserID, ':e'=>$EntrySeqID]);
        $row    = $stmt->fetch(PDO::FETCH_ASSOC);

        if ($row) {
            // Update MarkedFlownDateUTC only if empty or older, and apply any provided UT fields
            $updateParts = [];
            $params = [':u'=>$WSGUserID, ':e'=>$EntrySeqID];

            // MarkedFlownDateUTC: only advance it
            if (empty($row['MarkedFlownDateUTC']) || strtotime($markedDate) > strtotime($row['MarkedFlownDateUTC'])) {
                $updateParts[] = "MarkedFlownDateUTC = :md";
                $params[':md'] = $markedDate;
            }

            // Optional UT fields: only set when provided (non-null)
            if ($valPriv !== null) { $updateParts[] = "PrivateNotes = :priv"; $params[':priv'] = $valPriv; }
            if ($valPub  !== null) { $updateParts[] = "PublicFeedback = :pub"; $params[':pub'] = $valPub; }
            if ($valDiff !== null) { $updateParts[] = "DifficultyRating = :diff"; $params[':diff'] = $valDiff; }
            if ($valQual !== null) { $updateParts[] = "QualityRating = :qual"; $params[':qual'] = $valQual; }
            if ($valFlyNext !== null) { $updateParts[] = "MarkedFlyNextUTC = :flyn"; $params[':flyn'] = $valFlyNext; }
            if ($valFav    !== null) { $updateParts[] = "MarkedFavoritesUTC = :fav"; $params[':fav'] = $valFav; }

            if (!empty($updateParts)) {
                $updQ = "UPDATE UsersTasks SET " . implode(", ", $updateParts) .
                        " WHERE WSGUserID=:u AND EntrySeqID=:e";
                $uStmt = $pdo->prepare($updQ);
                $uStmt->execute($params);
            }
        } else {
            // Insert new row, always setting MarkedFlownDateUTC; include any provided UT fields
            $insQ = "INSERT INTO UsersTasks
                     (WSGUserID, EntrySeqID, MarkedFlownDateUTC, PrivateNotes, PublicFeedback,
                      DifficultyRating, QualityRating, MarkedFlyNextUTC, MarkedFavoritesUTC)
                     VALUES (:u, :e, :md, :priv, :pub, :diff, :qual, :flyn, :fav)";
            $iStmt = $pdo->prepare($insQ);
            $iStmt->execute([
                ':u'=>$WSGUserID, ':e'=>$EntrySeqID, ':md'=>$markedDate,
                ':priv'=>$valPriv, ':pub'=>$valPub, ':diff'=>$valDiff, ':qual'=>$valQual,
                ':flyn'=>$valFlyNext, ':fav'=>$valFav
            ]);
        }
    }

    // ─────────────────────────────────────────
    // 7) Trigger update of latest IGC leaders file, only if valid & completed
    // ─────────────────────────────────────────
    if ($TaskCompletedInt === 1 && $IGCValidInt === 1) {
        $updateScript = __DIR__ . '/UpdateLatestIGCLeaders.php';
        if (file_exists($updateScript)) {
            shell_exec("php " . escapeshellarg($updateScript) . " > /dev/null 2>&1 &");
            if ($loggingEnabled && function_exists('logMessage')) {
                logMessage("Triggered UpdateLatestIGCLeaders.php (valid + completed).");
            }
        }
    }

    // 8) Success
    echo json_encode([
      'status'  => 'success',
      'message' => 'IGC record saved.',
      'IGCKey'  => $IGCKey
    ]);

} catch (Exception $e) {
    echo json_encode([
      'status'  => 'error',
      'message' => $e->getMessage()
    ]);
    exit;
}
