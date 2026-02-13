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

function bindNullableString(PDOStatement $stmt, string $parameter, $value): void
{
    if ($value === null) {
        $stmt->bindValue($parameter, null, PDO::PARAM_NULL);
        return;
    }

    $stmt->bindValue($parameter, $value, PDO::PARAM_STR);
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
        'SELECT IGCKey, EntrySeqID, Sim, CompetitionClass, GliderType, MarkedAsDesigner
         FROM IGCRecords
         WHERE IGCKey = :igcKey AND EntrySeqID = :entrySeqID
         LIMIT 1'
    );
    $stmt->execute([
        ':igcKey' => $igcKey,
        ':entrySeqID' => $entrySeqID,
    ]);

    $record = $stmt->fetch(PDO::FETCH_ASSOC);
    return $record ?: null;
}

function findBestEligiblePerformance(PDO $pdo, int $entrySeqID, string $sim, $competitionClass, $gliderType): ?array
{
    $stmt = $pdo->prepare(
        "SELECT
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
        ORDER BY IGC.Speed DESC, IGC.IGCUploadDateTimeUTC ASC"
    );

    $stmt->bindValue(':EntrySeqID', $entrySeqID, PDO::PARAM_INT);
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
    $competitionClass = ($record['CompetitionClass'] ?? '') === '' ? null : $record['CompetitionClass'];
    $gliderType = ($record['GliderType'] ?? '') === '' ? null : $record['GliderType'];

    $bestEligible = findBestEligiblePerformance($pdo, $entrySeqID, $sim, $competitionClass, $gliderType);

    $stmt = $pdo->prepare(
        'SELECT IGCKey
         FROM TaskBestPerformances
         WHERE EntrySeqID = :EntrySeqID
           AND Sim = :Sim
           AND ((CompetitionClass IS NULL AND :CompetitionClass IS NULL) OR CompetitionClass = :CompetitionClass)
           AND ((GliderType IS NULL AND :GliderType IS NULL) OR GliderType = :GliderType)
         LIMIT 1'
    );
    $stmt->bindValue(':EntrySeqID', $entrySeqID, PDO::PARAM_INT);
    $stmt->bindValue(':Sim', $sim, PDO::PARAM_STR);
    bindNullableString($stmt, ':CompetitionClass', $competitionClass);
    bindNullableString($stmt, ':GliderType', $gliderType);
    $stmt->execute();
    $existing = $stmt->fetch(PDO::FETCH_ASSOC);

    if ($bestEligible === null) {
        if (!$existing) {
            return false;
        }

        $stmt = $pdo->prepare(
            'DELETE FROM TaskBestPerformances
             WHERE EntrySeqID = :EntrySeqID
               AND Sim = :Sim
               AND ((CompetitionClass IS NULL AND :CompetitionClass IS NULL) OR CompetitionClass = :CompetitionClass)
               AND ((GliderType IS NULL AND :GliderType IS NULL) OR GliderType = :GliderType)'
        );
        $stmt->bindValue(':EntrySeqID', $entrySeqID, PDO::PARAM_INT);
        $stmt->bindValue(':Sim', $sim, PDO::PARAM_STR);
        bindNullableString($stmt, ':CompetitionClass', $competitionClass);
        bindNullableString($stmt, ':GliderType', $gliderType);
        $stmt->execute();
        return true;
    }

    if (!$existing) {
        $stmt = $pdo->prepare(
            'INSERT INTO TaskBestPerformances (EntrySeqID, Sim, CompetitionClass, GliderType, IGCKey)
             VALUES (:EntrySeqID, :Sim, :CompetitionClass, :GliderType, :IGCKey)'
        );
        $stmt->bindValue(':EntrySeqID', $entrySeqID, PDO::PARAM_INT);
        $stmt->bindValue(':Sim', $sim, PDO::PARAM_STR);
        bindNullableString($stmt, ':CompetitionClass', $competitionClass);
        bindNullableString($stmt, ':GliderType', $gliderType);
        $stmt->bindValue(':IGCKey', $bestEligible['IGCKey'], PDO::PARAM_STR);
        $stmt->execute();
        return true;
    }

    if (($existing['IGCKey'] ?? '') === $bestEligible['IGCKey']) {
        return false;
    }

    $stmt = $pdo->prepare(
        'UPDATE TaskBestPerformances
         SET IGCKey = :IGCKey
         WHERE EntrySeqID = :EntrySeqID
           AND Sim = :Sim
           AND ((CompetitionClass IS NULL AND :CompetitionClass IS NULL) OR CompetitionClass = :CompetitionClass)
           AND ((GliderType IS NULL AND :GliderType IS NULL) OR GliderType = :GliderType)'
    );
    $stmt->bindValue(':IGCKey', $bestEligible['IGCKey'], PDO::PARAM_STR);
    $stmt->bindValue(':EntrySeqID', $entrySeqID, PDO::PARAM_INT);
    $stmt->bindValue(':Sim', $sim, PDO::PARAM_STR);
    bindNullableString($stmt, ':CompetitionClass', $competitionClass);
    bindNullableString($stmt, ':GliderType', $gliderType);
    $stmt->execute();

    return true;
}

$input = mergedInput();
$igcKey = isset($input['IGCKey']) ? trim((string) $input['IGCKey']) : '';
$entrySeqID = isset($input['EntrySeqID']) ? (int) $input['EntrySeqID'] : 0;

if ($igcKey === '' || $entrySeqID <= 0) {
    respond(['success' => false, 'error' => 'Missing required parameters'], 400);
}

try {
    $pdo = new PDO("sqlite:$databasePath");
    $pdo->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);

    $user = fetchUserOrUnauthorized($pdo);
    $record = fetchIgcRecord($pdo, $igcKey, $entrySeqID);
    if (!$record) {
        respond(['success' => false, 'error' => 'IGC record not found'], 404);
    }

    $currentMarked = (int) ($record['MarkedAsDesigner'] ?? 0) === 1;
    $newMarked = !$currentMarked;

    $pdo->beginTransaction();

    $stmt = $pdo->prepare(
        'UPDATE IGCRecords
         SET MarkedAsDesigner = :MarkedAsDesigner
         WHERE IGCKey = :IGCKey AND EntrySeqID = :EntrySeqID'
    );
    if ($newMarked) {
        $stmt->bindValue(':MarkedAsDesigner', 1, PDO::PARAM_INT);
    } else {
        $stmt->bindValue(':MarkedAsDesigner', null, PDO::PARAM_NULL);
    }
    $stmt->bindValue(':IGCKey', $igcKey, PDO::PARAM_STR);
    $stmt->bindValue(':EntrySeqID', $entrySeqID, PDO::PARAM_INT);
    $stmt->execute();

    $record['MarkedAsDesigner'] = $newMarked ? 1 : null;
    $changedBest = recomputeBestPerformance($pdo, $record);

    $pdo->commit();

    if ($changedBest) {
        try {
            refreshHomeLeaderboardCaches(
                $pdo,
                $record['Sim'] ?? null,
                ($record['CompetitionClass'] ?? '') === '' ? null : $record['CompetitionClass'],
                ($record['GliderType'] ?? '') === '' ? null : $record['GliderType']
            );
        } catch (Throwable $cacheException) {
            logMessage('igc_toggle_designer cache refresh error: ' . $cacheException->getMessage());
        }
    }

    if ((int) ($record['TaskCompleted'] ?? 0) === 1 && (int) ($record['IGCValid'] ?? 0) === 1) {
        $updateScript = __DIR__ . '/UpdateLatestIGCLeaders.php';
        if (file_exists($updateScript)) {
            shell_exec('php ' . escapeshellarg($updateScript) . ' > /dev/null 2>&1 &');
        }
    }

    logMessage(sprintf(
        'igc_toggle_designer: %s %s by %s (%d)',
        $newMarked ? 'marked' : 'unmarked',
        $igcKey,
        $user['WSGDisplayName'] ?? 'unknown',
        (int) ($user['WSGUserID'] ?? 0)
    ));

    respond([
        'success' => true,
        'record' => [
            'IGCKey' => $igcKey,
            'EntrySeqID' => $entrySeqID,
            'MarkedAsDesigner' => $newMarked,
        ],
    ]);
} catch (Exception $e) {
    if (isset($pdo) && $pdo instanceof PDO && $pdo->inTransaction()) {
        $pdo->rollBack();
    }

    logMessage('igc_toggle_designer error: ' . $e->getMessage());
    respond(['success' => false, 'error' => 'Unable to toggle designer status'], 500);
}
