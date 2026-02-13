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

    if (!isset($jsonData['IGCKey']) || trim((string) $jsonData['IGCKey']) === '') {
        throw new Exception('Missing required field: IGCKey');
    }
    if (!isset($jsonData['isPrivate'])) {
        throw new Exception('Missing required field: isPrivate');
    }

    $igcKey = trim((string) $jsonData['IGCKey']);
    $isPrivate = ((int) $jsonData['isPrivate'] === 1) ? 1 : 0;

    $pdo = new PDO("sqlite:$databasePath");
    $pdo->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);

    $recordStmt = $pdo->prepare(
        'SELECT EntrySeqID, Sim, CompetitionClass, GliderType, COALESCE(IsPrivate, 0) AS IsPrivate
         FROM IGCRecords
         WHERE IGCKey = :IGCKey
           AND WSGUserID = :WSGUserID
         LIMIT 1'
    );
    $recordStmt->bindValue(':IGCKey', $igcKey, PDO::PARAM_STR);
    $recordStmt->bindValue(':WSGUserID', $wsgUserID, PDO::PARAM_INT);
    $recordStmt->execute();
    $record = $recordStmt->fetch(PDO::FETCH_ASSOC);

    if (!$record) {
        http_response_code(404);
        echo json_encode(['error' => 'IGC record not found']);
        exit;
    }

    $entrySeqID = (int) ($record['EntrySeqID'] ?? 0);
    $sim = (string) ($record['Sim'] ?? '');
    $competitionClass = $record['CompetitionClass'] ?? null;
    $gliderType = $record['GliderType'] ?? null;
    $previousIsPrivate = (int) ($record['IsPrivate'] ?? 0);

    if ($previousIsPrivate === $isPrivate) {
        echo json_encode([
            'status' => 'success',
            'IGCKey' => $igcKey,
            'isPrivate' => $isPrivate,
            'affectedRows' => 0,
            'cacheRefreshCount' => 0,
        ]);
        exit;
    }

    $updateStmt = $pdo->prepare(
        'UPDATE IGCRecords
         SET IsPrivate = :IsPrivate
         WHERE IGCKey = :IGCKey
           AND WSGUserID = :WSGUserID'
    );
    $updateStmt->bindValue(':IsPrivate', $isPrivate, PDO::PARAM_INT);
    $updateStmt->bindValue(':IGCKey', $igcKey, PDO::PARAM_STR);
    $updateStmt->bindValue(':WSGUserID', $wsgUserID, PDO::PARAM_INT);
    $updateStmt->execute();

    $queuedGroups = 0;
    if ($entrySeqID > 0 && $sim !== '') {
        $groupKey = buildGroupKey($entrySeqID, $sim, $competitionClass, $gliderType);
        $queuedGroups = enqueueIgcPrivacyRefreshWork([
            $groupKey => [
                'EntrySeqID' => $entrySeqID,
                'Sim' => $sim,
                'CompetitionClass' => $competitionClass,
                'GliderType' => $gliderType,
            ],
        ], true);
        triggerIgcPrivacyRefreshWorker();
    }

    echo json_encode([
        'status' => 'success',
        'IGCKey' => $igcKey,
        'isPrivate' => $isPrivate,
        'affectedRows' => $updateStmt->rowCount(),
        'cacheRefreshCount' => 0,
        'fastestCacheRefreshed' => 0,
        'queuedGroups' => $queuedGroups,
        'asyncRefreshQueued' => 1,
    ]);
} catch (Throwable $e) {
    http_response_code(400);
    echo json_encode(['error' => $e->getMessage()]);
}
