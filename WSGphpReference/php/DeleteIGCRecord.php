<?php
require_once __DIR__ . '/session_restore.php';
require_once __DIR__ . '/CommonFunctions.php';
require_once __DIR__ . '/HomeLeaderboardCache.php';

function bindNullableString($stmt, string $parameter, $value): void
{
    if ($value === null) {
        $stmt->bindValue($parameter, null, PDO::PARAM_NULL);
    } else {
        $stmt->bindValue($parameter, $value, PDO::PARAM_STR);
    }
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
    $cacheNeedsRefresh = false;
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

try {
    if (!isset($_SESSION['user']) || !isset($_SESSION['user']['id'])) {
        http_response_code(401);
        echo json_encode(['error' => 'User not authenticated']);
        exit;
    }

    $sessionUserId = (int) $_SESSION['user']['id'];
    if ($sessionUserId <= 0) {
        http_response_code(400);
        echo json_encode(['error' => 'Invalid user ID']);
        exit;
    }

    // Check required POST parameter.
    if (!isset($_POST['IGCKey']) || trim($_POST['IGCKey']) === "") {
        throw new Exception("Missing required field: IGCKey");
    }
    
    $IGCKey = trim($_POST['IGCKey']);
    
    // Open the database connection.
    $pdo = new PDO("sqlite:$databasePath");
    $pdo->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);
    
    // Check if a record with the given IGCKey exists.
    $checkQuery = "SELECT * FROM IGCRecords WHERE IGCKey = :igcKey";
    $stmt = $pdo->prepare($checkQuery);
    $stmt->bindParam(':igcKey', $IGCKey, PDO::PARAM_STR);
    $stmt->execute();
    $record = $stmt->fetch(PDO::FETCH_ASSOC);

    if (!$record) {
        echo json_encode([
            'status' => 'not_found',
            'message' => 'No IGC record found with the given key.'
        ]);
        exit;
    }

    $userStmt = $pdo->prepare(
        'SELECT PoolSuperAdmin
         FROM Users
         WHERE WSGUserID = :id
         LIMIT 1'
    );
    $userStmt->execute([':id' => $sessionUserId]);
    $user = $userStmt->fetch(PDO::FETCH_ASSOC);
    if (!$user) {
        http_response_code(403);
        echo json_encode(['error' => 'Unauthorized']);
        exit;
    }

    $isAdmin = (int) ($user['PoolSuperAdmin'] ?? 0) === 1;
    $recordOwnerId = $record['WSGUserID'] ?? null;
    if (!$isAdmin && (int) $recordOwnerId !== $sessionUserId) {
        http_response_code(403);
        echo json_encode(['error' => 'Unauthorized']);
        exit;
    }

    $bestEntrySeqID = isset($record['EntrySeqID']) ? (int) $record['EntrySeqID'] : null;
    $bestSim = isset($record['Sim']) ? $record['Sim'] : null;
    $bestCompetitionClass = array_key_exists('CompetitionClass', $record) ? $record['CompetitionClass'] : null;
    $bestGliderType = array_key_exists('GliderType', $record) ? $record['GliderType'] : null;

    $bestRow = null;
    $cacheNeedsRefresh = false;
    if ($bestEntrySeqID !== null && $bestSim !== null) {
        $bestCheckQuery = "
            SELECT IGCKey
            FROM TaskBestPerformances
            WHERE EntrySeqID = :EntrySeqID
              AND Sim = :Sim
              AND ((CompetitionClass IS NULL AND :CompetitionClass IS NULL) OR CompetitionClass = :CompetitionClass)
              AND ((GliderType IS NULL AND :GliderType IS NULL) OR GliderType = :GliderType)
            LIMIT 1
        ";
        $stmt = $pdo->prepare($bestCheckQuery);
        $stmt->bindValue(':EntrySeqID', $bestEntrySeqID, PDO::PARAM_INT);
        $stmt->bindValue(':Sim', $bestSim, PDO::PARAM_STR);
        if ($bestCompetitionClass === null) {
            $stmt->bindValue(':CompetitionClass', null, PDO::PARAM_NULL);
        } else {
            $stmt->bindValue(':CompetitionClass', $bestCompetitionClass, PDO::PARAM_STR);
        }
        if ($bestGliderType === null) {
            $stmt->bindValue(':GliderType', null, PDO::PARAM_NULL);
        } else {
            $stmt->bindValue(':GliderType', $bestGliderType, PDO::PARAM_STR);
        }
        $stmt->execute();
        $bestRow = $stmt->fetch(PDO::FETCH_ASSOC);
    }

    // Extract EntrySeqID from the IGCKey.
    // Expected format: EntrySeqID_CompetitionID_GliderType_IGCRecordDateTimeUTC
    $parts = explode('_', $IGCKey);
    if (count($parts) < 4) {
        throw new Exception("Invalid IGCKey format.");
    }
    $EntrySeqID = $parts[0];
    
    // Determine the destination folder.
    // Note: $taskBrowserPath should be a filesystem path to the TaskBrowser folder.
    $destFolder = rtrim($taskBrowserPath, '/\\') . '/IGCFiles/' . $EntrySeqID;
    $destFilename = $destFolder . '/' . $IGCKey . '.igc';
    
    // Delete the file if it exists.
    if (file_exists($destFilename)) {
        if (!unlink($destFilename)) {
            throw new Exception("Failed to delete the file: $destFilename");
        }
    }

    if ($bestRow && isset($bestRow['IGCKey']) && $bestRow['IGCKey'] === $IGCKey && $bestEntrySeqID !== null && $bestSim !== null) {
        $newBest = findBestEligiblePerformance(
            $pdo,
            $bestEntrySeqID,
            $bestSim,
            $bestCompetitionClass,
            $bestGliderType,
            $IGCKey
        );

        if ($newBest && isset($newBest['IGCKey'])) {
            $updateBestQuery = "
                UPDATE TaskBestPerformances
                SET IGCKey = :NewIGCKey
                WHERE EntrySeqID = :EntrySeqID
                  AND Sim = :Sim
                  AND ((CompetitionClass IS NULL AND :CompetitionClass IS NULL) OR CompetitionClass = :CompetitionClass)
                  AND ((GliderType IS NULL AND :GliderType IS NULL) OR GliderType = :GliderType)
            ";
            $stmt = $pdo->prepare($updateBestQuery);
            $stmt->bindValue(':NewIGCKey', $newBest['IGCKey'], PDO::PARAM_STR);
            $stmt->bindValue(':EntrySeqID', $bestEntrySeqID, PDO::PARAM_INT);
            $stmt->bindValue(':Sim', $bestSim, PDO::PARAM_STR);
            bindNullableString($stmt, ':CompetitionClass', $bestCompetitionClass);
            bindNullableString($stmt, ':GliderType', $bestGliderType);
            $stmt->execute();

            $cacheNeedsRefresh = true;
        } else {
            $deleteBestQuery = "
                DELETE FROM TaskBestPerformances
                WHERE EntrySeqID = :EntrySeqID
                  AND Sim = :Sim
                  AND ((CompetitionClass IS NULL AND :CompetitionClass IS NULL) OR CompetitionClass = :CompetitionClass)
                  AND ((GliderType IS NULL AND :GliderType IS NULL) OR GliderType = :GliderType)
            ";
            $stmt = $pdo->prepare($deleteBestQuery);
            $stmt->bindValue(':EntrySeqID', $bestEntrySeqID, PDO::PARAM_INT);
            $stmt->bindValue(':Sim', $bestSim, PDO::PARAM_STR);
            bindNullableString($stmt, ':CompetitionClass', $bestCompetitionClass);
            bindNullableString($stmt, ':GliderType', $bestGliderType);
            $stmt->execute();

            $cacheNeedsRefresh = true;
        }
    }

    // Delete the record from IGCRecords table.
    $deleteQuery = "DELETE FROM IGCRecords WHERE IGCKey = :igcKey";
    $stmt = $pdo->prepare($deleteQuery);
    $stmt->bindParam(':igcKey', $IGCKey, PDO::PARAM_STR);
    $stmt->execute();

    logMessage("***** DELETION ***** Deleted IGC Record {$IGCKey}");

    if ($cacheNeedsRefresh && $bestSim !== null) {
        try {
            refreshHomeLeaderboardCaches($pdo, $bestSim, $bestCompetitionClass, $bestGliderType);
        } catch (Throwable $cacheException) {
            if (function_exists('logMessage')) {
                logMessage('Failed to refresh home leaderboard cache after delete: ' . $cacheException->getMessage());
            }
        }
    }

    echo json_encode([
        'status' => 'success',
        'message' => 'IGC record and file deleted successfully.',
        'IGCKey' => $IGCKey
    ]);
    
} catch (Exception $e) {
    echo json_encode(['error' => $e->getMessage()]);
    exit;
}
?>
