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

    if (!isset($jsonData['IGCKeys']) || !is_array($jsonData['IGCKeys'])) {
        throw new Exception('Missing required field: IGCKeys');
    }
    if (!isset($jsonData['isPrivate'])) {
        throw new Exception('Missing required field: isPrivate');
    }

    $isPrivate = ((int) $jsonData['isPrivate'] === 1) ? 1 : 0;

    $keys = [];
    foreach ($jsonData['IGCKeys'] as $key) {
        $trimmed = trim((string) $key);
        if ($trimmed !== '') {
            $keys[$trimmed] = true;
        }
    }
    $keys = array_keys($keys);

    if (empty($keys)) {
        echo json_encode([
            'status' => 'success',
            'updatedCount' => 0,
            'cacheRefreshCount' => 0,
            'queuedGroups' => 0,
            'asyncRefreshQueued' => 0,
        ]);
        exit;
    }

    // Keep below SQLite host-parameter limits (SQLite 3.7.x compatibility).
    $maxKeysPerChunk = 400;

    $pdo = new PDO("sqlite:$databasePath");
    $pdo->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);

    $groupsByKey = [];
    $updatedCount = 0;

    $pdo->beginTransaction();

    foreach (array_chunk($keys, $maxKeysPerChunk) as $chunkKeys) {
        $placeholders = implode(',', array_fill(0, count($chunkKeys), '?'));

        $selectSql = "
            SELECT DISTINCT EntrySeqID, Sim, CompetitionClass, GliderType
            FROM IGCRecords
            WHERE WSGUserID = ?
              AND COALESCE(IsPrivate, 0) != ?
              AND IGCKey IN ($placeholders)
        ";
        $selectStmt = $pdo->prepare($selectSql);
        $selectParams = array_merge([$wsgUserID, $isPrivate], $chunkKeys);
        $selectStmt->execute($selectParams);

        while ($row = $selectStmt->fetch(PDO::FETCH_ASSOC)) {
            $groupKey = buildGroupKey(
                (int) ($row['EntrySeqID'] ?? 0),
                (string) ($row['Sim'] ?? ''),
                $row['CompetitionClass'] ?? null,
                $row['GliderType'] ?? null
            );
            $groupsByKey[$groupKey] = $row;
        }

        $updateSql = "
            UPDATE IGCRecords
            SET IsPrivate = ?
            WHERE WSGUserID = ?
              AND COALESCE(IsPrivate, 0) != ?
              AND IGCKey IN ($placeholders)
        ";
        $updateStmt = $pdo->prepare($updateSql);
        $updateParams = array_merge([$isPrivate, $wsgUserID, $isPrivate], $chunkKeys);
        $updateStmt->execute($updateParams);
        $updatedCount += (int) $updateStmt->rowCount();
    }

    $pdo->commit();

    $queuedGroups = enqueueIgcPrivacyRefreshWork($groupsByKey, true);
    triggerIgcPrivacyRefreshWorker();

    echo json_encode([
        'status' => 'success',
        'updatedCount' => $updatedCount,
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
