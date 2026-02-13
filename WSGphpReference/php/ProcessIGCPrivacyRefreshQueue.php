<?php

require_once __DIR__ . '/CommonFunctions.php';
require_once __DIR__ . '/HomeLeaderboardCache.php';
require_once __DIR__ . '/IGCPrivacyUtils.php';
require_once __DIR__ . '/LatestIGCLeadersCache.php';
require_once __DIR__ . '/IGCPrivacyRefreshQueue.php';

header('Content-Type: application/json');

try {
    $pdo = createSqliteConnection($databasePath);
    $result = processIgcPrivacyRefreshQueue($pdo);

    echo json_encode([
        'status' => 'success',
        'queuedGroupCount' => $result['queuedGroupCount'],
        'cacheRefreshCount' => $result['cacheRefreshCount'],
        'fastestCacheRefreshed' => $result['fastestCacheRefreshed'],
    ]);
} catch (Throwable $e) {
    http_response_code(500);
    echo json_encode([
        'status' => 'error',
        'error' => $e->getMessage(),
    ]);
}

