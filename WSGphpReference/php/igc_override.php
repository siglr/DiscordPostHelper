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
                Sim, CompetitionClass, GliderType
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

function bindNullableString($stmt, string $parameter, $value): void
{
    if ($value === null) {
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

function normalizeBoolFlag($value, int $fallback): int
{
    if ($value === null) {
        return $fallback;
    }
    if (is_string($value)) {
        $value = trim($value);
    }
    $bool = filter_var($value, FILTER_VALIDATE_BOOLEAN, FILTER_NULL_ON_FAILURE);
    if ($bool === null) {
        return $fallback;
    }
    return $bool ? 1 : 0;
}

function parseDurationSeconds($value, $fallback)
{
    if ($value === null || $value === '') {
        return 0;
    }

    if (is_string($value) && strpos($value, ':') !== false) {
        $parts = explode(':', $value);
        if (count($parts) === 3) {
            [$h, $m, $s] = $parts;
            if (is_numeric($h) && is_numeric($m) && is_numeric($s)) {
                return ((int) $h * 3600) + ((int) $m * 60) + (int) $s;
            }
        }
    }

    if (is_numeric($value)) {
        return (int) $value;
    }

    return $fallback;
}

function normalizeNullableFloat($value, $fallback)
{
    if ($value === null) {
        return $fallback;
    }

    if (is_string($value)) {
        $trimmed = trim($value);
        if ($trimmed === '') {
            return null;
        }
        $value = $trimmed;
    }

    if (!is_numeric($value)) {
        return $fallback;
    }

    return (float) $value;
}

function normalizeLaunchUtc($value, $fallback)
{
    if ($value === null || $value === '') {
        return $fallback;
    }

    $trimmed = trim((string) $value);
    if (preg_match('/^\d{12}$/', $trimmed)) {
        return $trimmed;
    }

    $dt = DateTime::createFromFormat('Y-m-d H:i:s', $trimmed, new DateTimeZone('UTC'))
        ?: DateTime::createFromFormat('Y-m-d H:i', $trimmed, new DateTimeZone('UTC'));

    if ($dt) {
        return $dt->format('ymdHis');
    }

    return $fallback;
}

function normalizeString($value, $fallback)
{
    if ($value === null) {
        return $fallback;
    }
    $trimmed = trim((string) $value);
    return $trimmed === '' ? $fallback : $trimmed;
}

function normalizeClubEventNewsId($value, $fallback)
{
    if ($value === null) {
        return $fallback;
    }
    $trimmed = trim((string) $value);
    return $trimmed === '' ? null : $trimmed;
}

try {
    $pdo = new PDO('sqlite:' . $databasePath);
    $pdo->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);
} catch (Exception $e) {
    respond(['success' => false, 'error' => 'Database unavailable'], 500);
}

$user = fetchUserOrUnauthorized($pdo);
$input = mergedInput();

$action = isset($input['action']) ? strtolower(trim((string) $input['action'])) : '';
$igcKey = isset($input['IGCKey']) ? trim((string) $input['IGCKey']) : '';
$entrySeqID = isset($input['EntrySeqID']) ? (int) $input['EntrySeqID'] : 0;

if ($action === '' || $igcKey === '' || $entrySeqID <= 0) {
    respond(['success' => false, 'error' => 'Missing required parameters'], 400);
}

if ($action === 'get') {
    $record = fetchIgcRecord($pdo, $igcKey, $entrySeqID);
    if (!$record) {
        respond(['success' => false, 'error' => 'Record not found'], 404);
    }

    $override = fetchOverride($pdo, $igcKey, $entrySeqID);
    respond([
        'success' => true,
        'record' => formatRecordForResponse($record),
        'override' => formatOverrideForResponse($override),
    ]);
}

if ($action === 'save') {
    $record = fetchIgcRecord($pdo, $igcKey, $entrySeqID);
    if (!$record) {
        respond(['success' => false, 'error' => 'Record not found'], 404);
    }

    $reason = isset($input['Reason']) ? trim((string) $input['Reason']) : '';
    if ($reason === '') {
        respond(['success' => false, 'error' => 'Reason is required'], 400);
    }

    $override = fetchOverride($pdo, $igcKey, $entrySeqID);

    $newLocalTime = normalizeString($input['NewLocalTime'] ?? null, $record['LocalTime']);
    $newLocalDate = normalizeString($input['NewLocalDate'] ?? null, $record['LocalDate']);
    $newUtc = normalizeLaunchUtc($input['NewIGCRecordDateTimeUTC'] ?? null, $record['IGCRecordDateTimeUTC']);
    $newTaskCompleted = normalizeBoolFlag($input['NewTaskCompleted'] ?? null, (int) $record['TaskCompleted']);
    $newPenalties = normalizeBoolFlag($input['NewPenalties'] ?? null, (int) $record['Penalties']);
    $newDuration = parseDurationSeconds($input['NewDuration'] ?? null, $record['Duration']);
    $newDistance = normalizeNullableFloat($input['NewDistance'] ?? null, $record['Distance']);
    $newSpeed = normalizeNullableFloat($input['NewSpeed'] ?? null, $record['Speed']);
    $newValid = normalizeBoolFlag($input['NewIGCValid'] ?? null, (int) $record['IGCValid']);
    $newClubEvent = normalizeClubEventNewsId($input['NewClubEventNewsID'] ?? null, $record['ClubEventNewsID']);

    $updatedBy = trim((string) ($user['WSGDisplayName'] ?? ($_SESSION['user']['displayName'] ?? '')));
    if (isset($input['UpdatedBy']) && trim((string) $input['UpdatedBy']) !== '') {
        $updatedBy = trim((string) $input['UpdatedBy']);
    }
    if ($updatedBy === '') {
        $updatedBy = 'PoolSuperAdmin';
    }

    try {
        $pdo->beginTransaction();

        if (!$override) {
            $stmtInsert = $pdo->prepare(
                'INSERT INTO IGCOverrides (
                    IGCKey, EntrySeqID,
                    OriginalLocalTime, OriginalLocalDate, OriginalIGCRecordDateTimeUTC,
                    OriginalTaskCompleted, OriginalPenalties, OriginalDuration,
                    OriginalDistance, OriginalSpeed, OriginalIGCValid, OriginalClubEventNewsID,
                    Reason, UpdatedBy
                ) VALUES (
                    :igcKey, :entrySeqID,
                    :origLocalTime, :origLocalDate, :origUtc,
                    :origTaskCompleted, :origPenalties, :origDuration,
                    :origDistance, :origSpeed, :origValid, :origClubEvent,
                    :reason, :updatedBy
                )'
            );
            $stmtInsert->execute([
                ':igcKey' => $igcKey,
                ':entrySeqID' => $entrySeqID,
                ':origLocalTime' => $record['LocalTime'],
                ':origLocalDate' => $record['LocalDate'],
                ':origUtc' => $record['IGCRecordDateTimeUTC'],
                ':origTaskCompleted' => $record['TaskCompleted'],
                ':origPenalties' => $record['Penalties'],
                ':origDuration' => $record['Duration'],
                ':origDistance' => $record['Distance'],
                ':origSpeed' => $record['Speed'],
                ':origValid' => $record['IGCValid'],
                ':origClubEvent' => $record['ClubEventNewsID'],
                ':reason' => $reason,
                ':updatedBy' => $updatedBy,
            ]);
        } else {
            $stmtUpdateOverride = $pdo->prepare(
                'UPDATE IGCOverrides
                 SET Reason = :reason,
                     UpdatedBy = :updatedBy,
                     OverriddenOn = datetime("now")
                 WHERE IGCKey = :igcKey AND EntrySeqID = :entrySeqID'
            );
            $stmtUpdateOverride->execute([
                ':reason' => $reason,
                ':updatedBy' => $updatedBy,
                ':igcKey' => $igcKey,
                ':entrySeqID' => $entrySeqID,
            ]);
        }

        $stmtRecordUpdate = $pdo->prepare(
            'UPDATE IGCRecords
             SET LocalTime = :localTime,
                 LocalDate = :localDate,
                 IGCRecordDateTimeUTC = :utc,
                 TaskCompleted = :taskCompleted,
                 Penalties = :penalties,
                 Duration = :duration,
                 Distance = :distance,
                 Speed = :speed,
                 IGCValid = :valid,
                 ClubEventNewsID = :clubEvent
             WHERE IGCKey = :igcKey AND EntrySeqID = :entrySeqID'
        );

        $stmtRecordUpdate->bindValue(':localTime', $newLocalTime, PDO::PARAM_STR);
        $stmtRecordUpdate->bindValue(':localDate', $newLocalDate, PDO::PARAM_STR);
        $stmtRecordUpdate->bindValue(':utc', $newUtc, PDO::PARAM_STR);
        $stmtRecordUpdate->bindValue(':taskCompleted', $newTaskCompleted, PDO::PARAM_INT);
        $stmtRecordUpdate->bindValue(':penalties', $newPenalties, PDO::PARAM_INT);

        if ($newDuration === null || $newDuration === '') {
            $stmtRecordUpdate->bindValue(':duration', 0, PDO::PARAM_INT);
        } else {
            $stmtRecordUpdate->bindValue(':duration', $newDuration, PDO::PARAM_INT);
        }

        if ($newDistance === null || $newDistance === '') {
            $stmtRecordUpdate->bindValue(':distance', null, PDO::PARAM_NULL);
        } else {
            $stmtRecordUpdate->bindValue(':distance', $newDistance);
        }

        if ($newSpeed === null || $newSpeed === '') {
            $stmtRecordUpdate->bindValue(':speed', null, PDO::PARAM_NULL);
        } else {
            $stmtRecordUpdate->bindValue(':speed', $newSpeed);
        }

        $stmtRecordUpdate->bindValue(':valid', $newValid, PDO::PARAM_INT);
        if ($newClubEvent === null || $newClubEvent === '') {
            $stmtRecordUpdate->bindValue(':clubEvent', null, PDO::PARAM_NULL);
        } else {
            $stmtRecordUpdate->bindValue(':clubEvent', $newClubEvent, PDO::PARAM_STR);
        }
        $stmtRecordUpdate->bindValue(':igcKey', $igcKey, PDO::PARAM_STR);
        $stmtRecordUpdate->bindValue(':entrySeqID', $entrySeqID, PDO::PARAM_INT);
        $stmtRecordUpdate->execute();

        $pdo->commit();

        $updatedRecord = fetchIgcRecord($pdo, $igcKey, $entrySeqID);
        $bestChanged = $updatedRecord ? recomputeBestPerformance($pdo, $updatedRecord) : false;
        $isCurrentBest = $updatedRecord ? isCurrentBestPerformance($pdo, $updatedRecord) : false;
        if ($bestChanged || $isCurrentBest) {
            try {
                refreshHomeLeaderboardCaches(
                    $pdo,
                    $updatedRecord['Sim'],
                    $updatedRecord['CompetitionClass'],
                    $updatedRecord['GliderType']
                );
            } catch (Throwable $cacheException) {
                if (function_exists('logMessage')) {
                    logMessage('Failed to refresh home leaderboard cache after override save: ' . $cacheException->getMessage());
                }
            }
        }

        respond([
            'success' => true,
            'record' => formatRecordForResponse($updatedRecord),
            'override' => formatOverrideForResponse(fetchOverride($pdo, $igcKey, $entrySeqID)),
        ]);
    } catch (Exception $e) {
        if ($pdo->inTransaction()) {
            $pdo->rollBack();
        }
        logMessage('igc_override save error: ' . $e->getMessage());
        respond(['success' => false, 'error' => 'Unable to save override'], 500);
    }
}

if ($action === 'delete') {
    $override = fetchOverride($pdo, $igcKey, $entrySeqID);
    if (!$override) {
        respond(['success' => false, 'error' => 'No override to delete'], 404);
    }

    try {
        $pdo->beginTransaction();

        $stmtRestore = $pdo->prepare(
            'UPDATE IGCRecords
             SET LocalTime = :localTime,
                 LocalDate = :localDate,
                 IGCRecordDateTimeUTC = :utc,
                 TaskCompleted = :taskCompleted,
                 Penalties = :penalties,
                 Duration = :duration,
                 Distance = :distance,
                 Speed = :speed,
                 IGCValid = :valid,
                 ClubEventNewsID = :clubEvent
             WHERE IGCKey = :igcKey AND EntrySeqID = :entrySeqID'
        );

        $stmtRestore->bindValue(':localTime', $override['OriginalLocalTime'], PDO::PARAM_STR);
        $stmtRestore->bindValue(':localDate', $override['OriginalLocalDate'], PDO::PARAM_STR);
        $stmtRestore->bindValue(':utc', $override['OriginalIGCRecordDateTimeUTC'], PDO::PARAM_STR);
        $stmtRestore->bindValue(':taskCompleted', $override['OriginalTaskCompleted'], PDO::PARAM_INT);
        $stmtRestore->bindValue(':penalties', $override['OriginalPenalties'], PDO::PARAM_INT);

        if ($override['OriginalDuration'] === null || $override['OriginalDuration'] === '') {
            $stmtRestore->bindValue(':duration', 0, PDO::PARAM_INT);
        } else {
            $stmtRestore->bindValue(':duration', $override['OriginalDuration'], PDO::PARAM_INT);
        }

        if ($override['OriginalDistance'] === null || $override['OriginalDistance'] === '') {
            $stmtRestore->bindValue(':distance', null, PDO::PARAM_NULL);
        } else {
            $stmtRestore->bindValue(':distance', $override['OriginalDistance']);
        }

        if ($override['OriginalSpeed'] === null || $override['OriginalSpeed'] === '') {
            $stmtRestore->bindValue(':speed', null, PDO::PARAM_NULL);
        } else {
            $stmtRestore->bindValue(':speed', $override['OriginalSpeed']);
        }

        $stmtRestore->bindValue(':valid', $override['OriginalIGCValid'], PDO::PARAM_INT);
        if ($override['OriginalClubEventNewsID'] === null || $override['OriginalClubEventNewsID'] === '') {
            $stmtRestore->bindValue(':clubEvent', null, PDO::PARAM_NULL);
        } else {
            $stmtRestore->bindValue(':clubEvent', $override['OriginalClubEventNewsID'], PDO::PARAM_STR);
        }
        $stmtRestore->bindValue(':igcKey', $igcKey, PDO::PARAM_STR);
        $stmtRestore->bindValue(':entrySeqID', $entrySeqID, PDO::PARAM_INT);
        $stmtRestore->execute();

        $stmtDelete = $pdo->prepare(
            'DELETE FROM IGCOverrides WHERE IGCKey = :igcKey AND EntrySeqID = :entrySeqID'
        );
        $stmtDelete->execute([
            ':igcKey' => $igcKey,
            ':entrySeqID' => $entrySeqID,
        ]);

        $pdo->commit();

        $restored = fetchIgcRecord($pdo, $igcKey, $entrySeqID);
        $bestChanged = $restored ? recomputeBestPerformance($pdo, $restored) : false;
        $isCurrentBest = $restored ? isCurrentBestPerformance($pdo, $restored) : false;
        if ($bestChanged || $isCurrentBest) {
            try {
                refreshHomeLeaderboardCaches(
                    $pdo,
                    $restored['Sim'],
                    $restored['CompetitionClass'],
                    $restored['GliderType']
                );
            } catch (Throwable $cacheException) {
                if (function_exists('logMessage')) {
                    logMessage('Failed to refresh home leaderboard cache after override delete: ' . $cacheException->getMessage());
                }
            }
        }

        respond([
            'success' => true,
            'record' => formatRecordForResponse($restored),
            'override' => formatOverrideForResponse(fetchOverride($pdo, $igcKey, $entrySeqID)),
        ]);
    } catch (Exception $e) {
        if ($pdo->inTransaction()) {
            $pdo->rollBack();
        }
        logMessage('igc_override delete error: ' . $e->getMessage());
        respond(['success' => false, 'error' => 'Unable to delete override'], 500);
    }
}

respond(['success' => false, 'error' => 'Unsupported action'], 400);
