<?php
require_once __DIR__ . '/session_restore.php';
require_once __DIR__ . '/CommonFunctions.php';
require_once __DIR__ . '/HomeLeaderboardCache.php';

header('Content-Type: application/json');

function respond(array $payload, int $status = 200): void
{
    http_response_code($status);
    echo json_encode($payload);
    exit;
}

function mergedInput(): array
{
    $json = json_decode(file_get_contents('php://input'), true);
    $json = is_array($json) ? $json : [];
    return array_merge($_GET, $_POST, $json);
}

function fetchUserOrUnauthorized(PDO $pdo): array
{
    if (empty($_SESSION['user']['id'])) {
        respond(['success' => false, 'error' => 'Unauthorized'], 403);
    }

    $stmt = $pdo->prepare(
        'SELECT WSGUserID, WSGDisplayName, PoolSuperAdmin
         FROM Users
         WHERE WSGUserID = :id
         LIMIT 1'
    );
    $stmt->execute([':id' => (int) $_SESSION['user']['id']]);
    $user = $stmt->fetch(PDO::FETCH_ASSOC);

    if (!$user || (int) $user['PoolSuperAdmin'] !== 1) {
        respond(['success' => false, 'error' => 'Unauthorized'], 403);
    }

    return $user;
}

function fetchIgcRecord(PDO $pdo, string $igcKey, int $entrySeqID): ?array
{
    $stmt = $pdo->prepare(
        'SELECT IGCKey, EntrySeqID, LocalTime, LocalDate, IGCRecordDateTimeUTC, TaskCompleted,
                Penalties, Duration, Distance, Speed, IGCValid, ClubEventNewsID,
                Sim, CompetitionClass, GliderType, TPVersion
         FROM IGCRecords
         WHERE IGCKey = :igcKey AND EntrySeqID = :entrySeqID
         LIMIT 1'
    );
    $stmt->execute([
        ':igcKey' => $igcKey,
        ':entrySeqID' => $entrySeqID,
    ]);
    $row = $stmt->fetch(PDO::FETCH_ASSOC);
    return $row ?: null;
}

function fetchOverride(PDO $pdo, string $igcKey, int $entrySeqID): ?array
{
    $stmt = $pdo->prepare(
        'SELECT *
         FROM IGCOverrides
         WHERE IGCKey = :igcKey AND EntrySeqID = :entrySeqID
         LIMIT 1'
    );
    $stmt->execute([
        ':igcKey' => $igcKey,
        ':entrySeqID' => $entrySeqID,
    ]);
    $row = $stmt->fetch(PDO::FETCH_ASSOC);
    return $row ?: null;
}

function formatDuration(?string $duration): ?string
{
    if ($duration === null || $duration === '') {
        return null;
    }

    if (strpos($duration, ':') !== false) {
        return $duration;
    }

    if (!is_numeric($duration)) {
        return null;
    }

    $seconds = (int) $duration;
    $hours = intdiv($seconds, 3600);
    $minutes = intdiv($seconds % 3600, 60);
    $secs = $seconds % 60;
    return sprintf('%02d:%02d:%02d', $hours, $minutes, $secs);
}

function formatLaunchUtc(?string $value): ?string
{
    if ($value === null) {
        return null;
    }

    $trimmed = trim($value);
    if (preg_match('/^\d{12}$/', $trimmed)) {
        $yy = substr($trimmed, 0, 2);
        $mm = substr($trimmed, 2, 2);
        $dd = substr($trimmed, 4, 2);
        $hh = substr($trimmed, 6, 2);
        $mi = substr($trimmed, 8, 2);
        $ss = substr($trimmed, 10, 2);
        $year = 2000 + (int) $yy;
        return sprintf('%04d-%02d-%02d %02d:%02d:%02d', $year, $mm, $dd, $hh, $mi, $ss);
    }

    return $trimmed !== '' ? $trimmed : null;
}

function formatRecordForResponse(array $row): array
{
    return [
        'IGCKey' => $row['IGCKey'],
        'EntrySeqID' => (int) $row['EntrySeqID'],
        'LocalTime' => $row['LocalTime'],
        'LocalDate' => $row['LocalDate'],
        'IGCRecordDateTimeUTC' => formatLaunchUtc($row['IGCRecordDateTimeUTC']),
        'IGCRecordDateTimeUTCRaw' => $row['IGCRecordDateTimeUTC'],
        'TaskCompleted' => (int) $row['TaskCompleted'] === 1,
        'Penalties' => (int) $row['Penalties'] === 1,
        'Duration' => formatDuration($row['Duration']),
        'DurationSeconds' => is_numeric($row['Duration']) ? (int) $row['Duration'] : null,
        'Distance' => isset($row['Distance']) ? (float) $row['Distance'] : null,
        'Speed' => isset($row['Speed']) ? (float) $row['Speed'] : null,
        'IGCValid' => (int) $row['IGCValid'] === 1,
        'ClubEventNewsID' => $row['ClubEventNewsID'],
        'TPVersion' => $row['TPVersion'],
    ];
}

function formatOverrideForResponse(?array $row): array
{
    if (!$row) {
        return ['exists' => false];
    }

    return [
        'exists' => true,
        'OriginalLocalTime' => $row['OriginalLocalTime'],
        'OriginalLocalDate' => $row['OriginalLocalDate'],
        'OriginalIGCRecordDateTimeUTC' => formatLaunchUtc($row['OriginalIGCRecordDateTimeUTC']),
        'OriginalIGCRecordDateTimeUTCRaw' => $row['OriginalIGCRecordDateTimeUTC'],
        'OriginalTaskCompleted' => (int) $row['OriginalTaskCompleted'] === 1,
        'OriginalPenalties' => (int) $row['OriginalPenalties'] === 1,
        'OriginalDuration' => formatDuration($row['OriginalDuration']),
        'OriginalDurationSeconds' => is_numeric($row['OriginalDuration']) ? (int) $row['OriginalDuration'] : null,
        'OriginalDistance' => isset($row['OriginalDistance']) ? (float) $row['OriginalDistance'] : null,
        'OriginalSpeed' => isset($row['OriginalSpeed']) ? (float) $row['OriginalSpeed'] : null,
        'OriginalIGCValid' => (int) $row['OriginalIGCValid'] === 1,
        'OriginalClubEventNewsID' => $row['OriginalClubEventNewsID'],
        'Reason' => $row['Reason'],
        'UpdatedBy' => $row['UpdatedBy'],
        'OverriddenOn' => $row['OverriddenOn'],
    ];
}

function buildProposalFromParsedResults(array $parsedResults): array
{
    $taskCompletedInt = !empty($parsedResults["TaskCompleted"]) ? 1 : 0;
    $penaltiesInt = !empty($parsedResults["Penalties"]) ? 1 : 0;
    $igcValidInt = !empty($parsedResults["IGCValid"]) ? 1 : 0;

    $durationRaw = $parsedResults["Duration"] ?? null;
    $durationSeconds = durationToSeconds($durationRaw);
    $durationDisplay = $durationSeconds !== null ? formatDuration((string) $durationSeconds) : null;

    $distanceFloat = (!empty($parsedResults["Distance"])) ? (float) $parsedResults["Distance"] : null;
    $speedFloat = (!empty($parsedResults["Speed"])) ? (float) $parsedResults["Speed"] : null;
    $tpVersion = $parsedResults["TPVersion"] ?? null;

    return [
        'proposed' => [
            'TaskCompleted' => $taskCompletedInt === 1,
            'Penalties' => $penaltiesInt === 1,
            'Duration' => $durationDisplay,
            'DurationSeconds' => $durationSeconds,
            'Distance' => $distanceFloat,
            'Speed' => $speedFloat,
            'IGCValid' => $igcValidInt === 1,
            'TPVersion' => $tpVersion,
        ],
        'proposalRaw' => [
            'TaskCompleted' => $taskCompletedInt,
            'Penalties' => $penaltiesInt,
            'DurationSeconds' => $durationSeconds,
            'Distance' => $distanceFloat,
            'Speed' => $speedFloat,
            'IGCValid' => $igcValidInt,
            'TPVersion' => $tpVersion,
        ],
    ];
}

function normalizeProposalInput($proposal): array
{
    if (!is_array($proposal)) {
        respond(['success' => false, 'error' => 'Missing proposed values for update.'], 400);
    }

    $taskCompleted = !empty($proposal['TaskCompleted']) ? 1 : 0;
    $penalties = !empty($proposal['Penalties']) ? 1 : 0;
    $igcValid = !empty($proposal['IGCValid']) ? 1 : 0;

    $durationSeconds = $proposal['DurationSeconds'] ?? null;
    if ($durationSeconds === '' || $durationSeconds === null) {
        $durationSeconds = 0;
    } elseif (is_numeric($durationSeconds)) {
        $durationSeconds = (int) $durationSeconds;
    } else {
        respond(['success' => false, 'error' => 'Invalid duration value.'], 400);
    }

    $distance = $proposal['Distance'] ?? null;
    if ($distance === '' || $distance === null) {
        $distance = null;
    } elseif (is_numeric($distance)) {
        $distance = (float) $distance;
    } else {
        respond(['success' => false, 'error' => 'Invalid distance value.'], 400);
    }

    $speed = $proposal['Speed'] ?? null;
    if ($speed === '' || $speed === null) {
        $speed = null;
    } elseif (is_numeric($speed)) {
        $speed = (float) $speed;
    } else {
        respond(['success' => false, 'error' => 'Invalid speed value.'], 400);
    }

    $tpVersion = isset($proposal['TPVersion']) ? trim((string) $proposal['TPVersion']) : null;
    if ($tpVersion === '') {
        $tpVersion = null;
    }

    return [
        'TaskCompleted' => $taskCompleted,
        'Penalties' => $penalties,
        'DurationSeconds' => $durationSeconds,
        'Distance' => $distance,
        'Speed' => $speed,
        'IGCValid' => $igcValid,
        'TPVersion' => $tpVersion,
    ];
}

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
    $gliderType
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

    $bestQuery .= "\n        ORDER BY IGC.Speed DESC, IGC.IGCUploadDateTimeUTC ASC";

    $stmt = $pdo->prepare($bestQuery);
    $stmt->bindValue(':EntrySeqID', $entrySeqId, PDO::PARAM_INT);
    $stmt->bindValue(':Sim', $sim, PDO::PARAM_STR);
    bindNullableString($stmt, ':CompetitionClass', $competitionClass);
    bindNullableString($stmt, ':GliderType', $gliderType);
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

function recomputeBestPerformance(PDO $pdo, array $record): bool
{
    if (empty($record['EntrySeqID']) || empty($record['Sim'])) {
        return false;
    }

    $entrySeqID = (int) $record['EntrySeqID'];
    $sim = $record['Sim'];
    $competitionClass = $record['CompetitionClass'] === '' ? null : $record['CompetitionClass'];
    $gliderType = $record['GliderType'] === '' ? null : $record['GliderType'];

    $bestEligible = findBestEligiblePerformance($pdo, $entrySeqID, $sim, $competitionClass, $gliderType);

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
    $stmt->bindValue(':EntrySeqID', $entrySeqID, PDO::PARAM_INT);
    $stmt->bindValue(':Sim', $sim, PDO::PARAM_STR);
    bindNullableString($stmt, ':CompetitionClass', $competitionClass);
    bindNullableString($stmt, ':GliderType', $gliderType);
    $stmt->execute();
    $existingBest = $stmt->fetch(PDO::FETCH_ASSOC);

    if ($bestEligible === null) {
        if ($existingBest) {
            $deleteBestQuery = "
                DELETE FROM TaskBestPerformances
                WHERE EntrySeqID = :EntrySeqID
                  AND Sim = :Sim
                  AND ((CompetitionClass IS NULL AND :CompetitionClass IS NULL) OR CompetitionClass = :CompetitionClass)
                  AND ((GliderType IS NULL AND :GliderType IS NULL) OR GliderType = :GliderType)
            ";
            $stmt = $pdo->prepare($deleteBestQuery);
            $stmt->bindValue(':EntrySeqID', $entrySeqID, PDO::PARAM_INT);
            $stmt->bindValue(':Sim', $sim, PDO::PARAM_STR);
            bindNullableString($stmt, ':CompetitionClass', $competitionClass);
            bindNullableString($stmt, ':GliderType', $gliderType);
            $stmt->execute();

            return true;
        }

        return false;
    }

    if (!$existingBest) {
        $insertBestQuery = "
            INSERT INTO TaskBestPerformances (EntrySeqID, Sim, CompetitionClass, GliderType, IGCKey)
            VALUES (:EntrySeqID, :Sim, :CompetitionClass, :GliderType, :IGCKey)
        ";

        $stmt = $pdo->prepare($insertBestQuery);
        $stmt->bindValue(':EntrySeqID', $entrySeqID, PDO::PARAM_INT);
        $stmt->bindValue(':Sim', $sim, PDO::PARAM_STR);
        bindNullableString($stmt, ':CompetitionClass', $competitionClass);
        bindNullableString($stmt, ':GliderType', $gliderType);
        $stmt->bindValue(':IGCKey', $bestEligible['IGCKey'], PDO::PARAM_STR);
        $stmt->execute();

        return true;
    }

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
        $stmt->bindValue(':EntrySeqID', $entrySeqID, PDO::PARAM_INT);
        $stmt->bindValue(':Sim', $sim, PDO::PARAM_STR);
        bindNullableString($stmt, ':CompetitionClass', $competitionClass);
        bindNullableString($stmt, ':GliderType', $gliderType);
        $stmt->execute();

        return true;
    }

    return false;
}

function isCurrentBestPerformance(PDO $pdo, array $record): bool
{
    if (empty($record['EntrySeqID']) || empty($record['Sim']) || empty($record['IGCKey'])) {
        return false;
    }

    $entrySeqID = (int) $record['EntrySeqID'];
    $sim = $record['Sim'];
    $competitionClass = $record['CompetitionClass'] === '' ? null : $record['CompetitionClass'];
    $gliderType = $record['GliderType'] === '' ? null : $record['GliderType'];
    $igcKey = $record['IGCKey'];

    $query = "
        SELECT 1
        FROM TaskBestPerformances
        WHERE EntrySeqID = :EntrySeqID
          AND Sim = :Sim
          AND IGCKey = :IGCKey
          AND ((CompetitionClass IS NULL AND :CompetitionClass IS NULL) OR CompetitionClass = :CompetitionClass)
          AND ((GliderType IS NULL AND :GliderType IS NULL) OR GliderType = :GliderType)
        LIMIT 1
    ";

    $stmt = $pdo->prepare($query);
    $stmt->bindValue(':EntrySeqID', $entrySeqID, PDO::PARAM_INT);
    $stmt->bindValue(':Sim', $sim, PDO::PARAM_STR);
    $stmt->bindValue(':IGCKey', $igcKey, PDO::PARAM_STR);
    bindNullableString($stmt, ':CompetitionClass', $competitionClass);
    bindNullableString($stmt, ':GliderType', $gliderType);
    $stmt->execute();

    return (bool) $stmt->fetchColumn();
}

function durationToSeconds(?string $duration): ?int
{
    if ($duration === null || $duration === '') {
        return 0;
    }

    if (strpos($duration, ':') !== false) {
        $parts = explode(':', $duration);
        if (count($parts) === 3) {
            [$h, $m, $s] = $parts;
            if (is_numeric($h) && is_numeric($m) && is_numeric($s)) {
                return ((int) $h * 3600) + ((int) $m * 60) + (int) $s;
            }
        }
    }

    if (is_numeric($duration)) {
        return (int) $duration;
    }

    return null;
}

$input = mergedInput();
$igcKey = isset($input['IGCKey']) ? trim((string) $input['IGCKey']) : '';
$entrySeqID = isset($input['EntrySeqID']) ? (int) $input['EntrySeqID'] : 0;
$action = isset($input['action']) ? strtolower(trim((string) $input['action'])) : 'preview';

if ($igcKey === '' || $entrySeqID <= 0) {
    respond(['success' => false, 'error' => 'Missing required parameters'], 400);
}

$pdo = new PDO("sqlite:$databasePath");
$pdo->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);

fetchUserOrUnauthorized($pdo);

$record = fetchIgcRecord($pdo, $igcKey, $entrySeqID);
if (!$record) {
    respond(['success' => false, 'error' => 'Record not found'], 404);
}

$existingOverride = fetchOverride($pdo, $igcKey, $entrySeqID);
if ($existingOverride) {
    respond([
        'success' => false,
        'error' => 'Re-analysis is blocked because an override exists. Clear the override before re-analyzing.',
    ], 409);
}

$baseTempDir = __DIR__ . '/DPHXTemp';
if (!file_exists($baseTempDir)) {
    mkdir($baseTempDir, 0755, true);
}
register_shutdown_function('cleanupOldTempFolders', $baseTempDir);

$tempDir = $baseTempDir . '/Reanalyze/' . $igcKey;

if ($action === 'commit') {
    $resultsFile = $tempDir . '/reanalyze_results.json';
    if (!is_file($resultsFile)) {
        respond(['success' => false, 'error' => 'Re-analysis results were not found. Please run the preview again.'], 409);
    }

    $storedContents = file_get_contents($resultsFile);
    if ($storedContents === false) {
        respond(['success' => false, 'error' => 'Unable to read stored re-analysis results. Please run the preview again.'], 409);
    }

    $storedPayload = json_decode($storedContents, true);
    if (!is_array($storedPayload)) {
        respond(['success' => false, 'error' => 'Stored re-analysis results are invalid. Please run the preview again.'], 409);
    }

    $storedIgcKey = isset($storedPayload['IGCKey']) ? trim((string) $storedPayload['IGCKey']) : '';
    $storedEntrySeqID = isset($storedPayload['EntrySeqID']) ? (int) $storedPayload['EntrySeqID'] : 0;
    $storedParsedResults = $storedPayload['ParsedResults'] ?? null;

    if ($storedIgcKey !== $igcKey || $storedEntrySeqID !== $entrySeqID || !is_array($storedParsedResults)) {
        respond(['success' => false, 'error' => 'Stored re-analysis results do not match this request. Please run the preview again.'], 409);
    }

    $proposal = buildProposalFromParsedResults($storedParsedResults);
    $proposal = normalizeProposalInput($proposal['proposalRaw']);

    try {
        $pdo->beginTransaction();

        $updateQuery = '
            UPDATE IGCRecords
            SET TaskCompleted = :taskCompleted,
                Penalties = :penalties,
                Duration = :duration,
                Distance = :distance,
                Speed = :speed,
                IGCValid = :igcValid,
                TPVersion = :tpVersion
            WHERE IGCKey = :igcKey AND EntrySeqID = :entrySeqID
        ';
        $stmtUpdate = $pdo->prepare($updateQuery);
        $stmtUpdate->bindValue(':taskCompleted', $proposal['TaskCompleted'], PDO::PARAM_INT);
        $stmtUpdate->bindValue(':penalties', $proposal['Penalties'], PDO::PARAM_INT);
        if ($proposal['DurationSeconds'] === null || $proposal['DurationSeconds'] === '') {
            $stmtUpdate->bindValue(':duration', 0, PDO::PARAM_INT);
        } else {
            $stmtUpdate->bindValue(':duration', $proposal['DurationSeconds'], PDO::PARAM_INT);
        }
        if ($proposal['Distance'] === null) {
            $stmtUpdate->bindValue(':distance', null, PDO::PARAM_NULL);
        } else {
            $stmtUpdate->bindValue(':distance', $proposal['Distance']);
        }
        if ($proposal['Speed'] === null) {
            $stmtUpdate->bindValue(':speed', null, PDO::PARAM_NULL);
        } else {
            $stmtUpdate->bindValue(':speed', $proposal['Speed']);
        }
        $stmtUpdate->bindValue(':igcValid', $proposal['IGCValid'], PDO::PARAM_INT);
        bindNullableString($stmtUpdate, ':tpVersion', $proposal['TPVersion']);
        $stmtUpdate->bindValue(':igcKey', $igcKey, PDO::PARAM_STR);
        $stmtUpdate->bindValue(':entrySeqID', $entrySeqID, PDO::PARAM_INT);
        $stmtUpdate->execute();

        $pdo->commit();
    } catch (Exception $e) {
        if ($pdo->inTransaction()) {
            $pdo->rollBack();
        }
        respond(['success' => false, 'error' => 'Unable to update IGC record'], 500);
    }

    $updatedRecord = fetchIgcRecord($pdo, $igcKey, $entrySeqID);
    if (!$updatedRecord) {
        respond(['success' => false, 'error' => 'Failed to load updated record'], 500);
    }

    $bestChanged = recomputeBestPerformance($pdo, $updatedRecord);
    $isCurrentBest = isCurrentBestPerformance($pdo, $updatedRecord);
    if ($bestChanged || $isCurrentBest) {
        try {
            refreshHomeLeaderboardCaches($pdo, $updatedRecord['Sim'], $updatedRecord['CompetitionClass'], $updatedRecord['GliderType']);
        } catch (Throwable $cacheException) {
            if (function_exists('logMessage')) {
                logMessage('Failed to refresh home leaderboard cache after re-analysis: ' . $cacheException->getMessage());
            }
        }
    }

    $override = fetchOverride($pdo, $igcKey, $entrySeqID);

    respond([
        'success' => true,
        'record' => formatRecordForResponse($updatedRecord),
        'override' => formatOverrideForResponse($override),
    ]);
}

$stmtTask = $pdo->prepare('SELECT EntrySeqID, PLNXML FROM Tasks WHERE EntrySeqID = :entrySeqID LIMIT 1');
$stmtTask->execute([':entrySeqID' => $entrySeqID]);
$taskRow = $stmtTask->fetch(PDO::FETCH_ASSOC);
if (!$taskRow || empty($taskRow['PLNXML'])) {
    respond(['success' => false, 'error' => 'Task plan not found for re-analysis'], 404);
}

$igcPath = rtrim($taskBrowserPath, '/\\') . '/IGCFiles/' . $entrySeqID . '/' . $igcKey . '.igc';
if (!file_exists($igcPath)) {
    respond(['success' => false, 'error' => 'IGC file not found for re-analysis'], 404);
}

$baseHttps = $taskBrowserPathHTTPS ?: $wsgRoot;
if (!$baseHttps) {
    respond(['success' => false, 'error' => 'Task browser HTTPS base URL is not configured'], 500);
}

$rootWithoutProtocol = preg_replace('#^https?://#', '', $baseHttps);
if (!$rootWithoutProtocol) {
    respond(['success' => false, 'error' => 'Task browser base URL is not configured'], 500);
}

if (!is_dir($tempDir) && !mkdir($tempDir, 0755, true)) {
    respond(['success' => false, 'error' => 'Unable to prepare temporary folder for PLN'], 500);
}

$plnFile = $tempDir . '/' . $entrySeqID . '.pln';
if (file_put_contents($plnFile, $taskRow['PLNXML']) === false) {
    respond(['success' => false, 'error' => 'Unable to write PLN file for re-analysis'], 500);
}

$igcFileUrl = rtrim($baseHttps, '/') . '/IGCFiles/' . $entrySeqID . '/' . $igcKey . '.igc';
$plnFileUrl = $rootWithoutProtocol . '/php/DPHXTemp/Reanalyze/' . $igcKey . '/' . $entrySeqID . '.pln';

$browserlessResponse = browserlessExtractTracklogsOnly($igcFileUrl, $plnFileUrl, 'forced');
if (!empty($browserlessResponse['data'])) {
    $browserlessResult = $browserlessResponse['data'];
    if (!$browserlessResponse['ok']) {
        $browserlessResult['error'] = $browserlessResponse['error'] ?? 'Browserless request failed.';
    }
} else {
    $browserlessResult = [
        'error' => $browserlessResponse['error'] ?? 'Browserless did not return data.'
    ];
}

$browserlessResult = parseBrowserlessTracklogs($browserlessResult, null, false);

if (empty($browserlessResult['parsedResults'])) {
    respond([
        'success' => false,
        'error' => $browserlessResult['error'] ?? 'Failed to parse Browserless results.'
    ], 500);
}

$parsedResults = $browserlessResult['parsedResults'];
$proposal = buildProposalFromParsedResults($parsedResults);
$resultsFile = $tempDir . '/reanalyze_results.json';
$storedPayload = [
    'IGCKey' => $igcKey,
    'EntrySeqID' => $entrySeqID,
    'ParsedResults' => $parsedResults,
];
if (file_put_contents($resultsFile, json_encode($storedPayload)) === false) {
    respond(['success' => false, 'error' => 'Unable to store re-analysis results.'], 500);
}

respond([
    'success' => true,
    'record' => formatRecordForResponse($record),
    'proposed' => $proposal['proposed'],
    'proposalRaw' => $proposal['proposalRaw'],
]);
