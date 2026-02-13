<?php
require_once __DIR__ . '/session_restore.php';
require_once __DIR__ . '/CommonFunctions.php';
require_once __DIR__ . '/HomeLeaderboardCache.php';
require_once __DIR__ . '/IGCPrivacyUtils.php';
require_once __DIR__ . '/LatestIGCLeadersCache.php';
require_once __DIR__ . '/IGCPrivacyRefreshQueue.php';

header('Content-Type: application/json');

try {
    if (!isset($_SESSION['user']) || !isset($_SESSION['user']['id'])) {
        http_response_code(401);
        echo json_encode(['error' => 'User not authenticated']);
        exit;
    }

    $wsgUserID = (int) $_SESSION['user']['id'];
    if ($wsgUserID <= 0) {
        http_response_code(400);
        echo json_encode(['error' => 'Invalid user ID']);
        exit;
    }

    $jsonData = json_decode(file_get_contents('php://input'), true);
    if (!is_array($jsonData)) {
        throw new Exception('Invalid payload');
    }

    if (!isset($jsonData['privateDefault'])) {
        throw new Exception('Missing required field: privateDefault');
    }

    $privateDefault = ((int) $jsonData['privateDefault'] === 1) ? 1 : 0;
    $applyToExisting = isset($jsonData['applyToExisting']) && ((int) $jsonData['applyToExisting'] === 1);

    $pdo = new PDO("sqlite:$databasePath");
    $pdo->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);

    $groupsToRecalc = [];
    if ($applyToExisting) {
        $groupStmt = $pdo->prepare(
            "SELECT DISTINCT EntrySeqID, Sim, CompetitionClass, GliderType
             FROM IGCRecords
             WHERE WSGUserID = :WSGUserID
               AND COALESCE(IsPrivate, 0) != :IsPrivate"
        );
        $groupStmt->bindValue(':WSGUserID', $wsgUserID, PDO::PARAM_INT);
        $groupStmt->bindValue(':IsPrivate', $privateDefault, PDO::PARAM_INT);
        $groupStmt->execute();
        $groupsToRecalc = $groupStmt->fetchAll(PDO::FETCH_ASSOC);
    }

    $pdo->beginTransaction();

    $userStmt = $pdo->prepare('UPDATE Users SET IGCPrivateDefault = :IGCPrivateDefault WHERE WSGUserID = :WSGUserID');
    $userStmt->bindValue(':IGCPrivateDefault', $privateDefault, PDO::PARAM_INT);
    $userStmt->bindValue(':WSGUserID', $wsgUserID, PDO::PARAM_INT);
    $userStmt->execute();

    $affectedRows = 0;
    if ($applyToExisting) {
        $updateStmt = $pdo->prepare(
            'UPDATE IGCRecords
             SET IsPrivate = :IsPrivate
             WHERE WSGUserID = :WSGUserID
               AND COALESCE(IsPrivate, 0) != :IsPrivate'
        );
        $updateStmt->bindValue(':IsPrivate', $privateDefault, PDO::PARAM_INT);
        $updateStmt->bindValue(':WSGUserID', $wsgUserID, PDO::PARAM_INT);
        $updateStmt->execute();
        $affectedRows = $updateStmt->rowCount();
    }

    $pdo->commit();

    $groupsByKey = [];
    if ($applyToExisting && !empty($groupsToRecalc)) {
        foreach ($groupsToRecalc as $group) {
            $entrySeqID = isset($group['EntrySeqID']) ? (int) $group['EntrySeqID'] : 0;
            $sim = isset($group['Sim']) ? (string) $group['Sim'] : '';
            $competitionClass = $group['CompetitionClass'] ?? null;
            $gliderType = $group['GliderType'] ?? null;

            if ($entrySeqID <= 0 || $sim === '') {
                continue;
            }

            $groupKey = buildGroupKey($entrySeqID, $sim, $competitionClass, $gliderType);
            $groupsByKey[$groupKey] = [
                'EntrySeqID' => $entrySeqID,
                'Sim' => $sim,
                'CompetitionClass' => $competitionClass,
                'GliderType' => $gliderType,
            ];
        }
    }

    $queuedGroups = enqueueIgcPrivacyRefreshWork($groupsByKey, $applyToExisting);
    triggerIgcPrivacyRefreshWorker();

    $_SESSION['user']['igcPrivateDefault'] = $privateDefault === 1;

    echo json_encode([
        'status' => 'success',
        'privateDefault' => $privateDefault,
        'applyToExisting' => $applyToExisting ? 1 : 0,
        'affectedRows' => $affectedRows,
        'cacheRefreshCount' => 0,
        'fastestCacheRefreshed' => 0,
        'queuedGroups' => $queuedGroups,
        'asyncRefreshQueued' => 1,
    ]);
} catch (Throwable $e) {
    if (isset($pdo) && $pdo instanceof PDO && $pdo->inTransaction()) {
        $pdo->rollBack();
    }

    http_response_code(400);
    echo json_encode(['error' => $e->getMessage()]);
}
