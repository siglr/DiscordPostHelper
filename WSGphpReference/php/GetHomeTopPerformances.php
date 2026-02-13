<?php
require __DIR__ . '/CommonFunctions.php';
require_once __DIR__ . '/HomeLeaderboardCache.php';

header('Content-Type: application/json; charset=UTF-8');

try {
    $simParam = filter_input(INPUT_GET, 'sim', FILTER_SANITIZE_FULL_SPECIAL_CHARS);
    $classParam = filter_input(INPUT_GET, 'competitionClass', FILTER_SANITIZE_FULL_SPECIAL_CHARS);
    $gliderParam = filter_input(INPUT_GET, 'gliderType', FILTER_SANITIZE_FULL_SPECIAL_CHARS);
    $modeParam = filter_input(INPUT_GET, 'mode', FILTER_SANITIZE_FULL_SPECIAL_CHARS);

    $sim = normalizeHomeLeaderboardFilterValue($simParam);
    $class = normalizeHomeLeaderboardFilterValue($classParam);
    $glider = normalizeHomeLeaderboardFilterValue($gliderParam);
    $mode = normalizeHomeLeaderboardMode($modeParam);

    $filtersCache = loadHomeLeaderboardFilters();
    $filters = $filtersCache['filters'] ?? null;
    $filtersSignature = $filtersCache['signature'] ?? null;

    $cacheEntry = loadHomeLeaderboardCache($sim, $class, $glider, $mode);

    if ($cacheEntry !== null && $filters !== null) {
        $payload = $cacheEntry['payload'];
        if (!isset($payload['filtersSignature']) || $payload['filtersSignature'] !== $filtersSignature || !isset($payload['filters'])) {
            $payload['filters'] = $filters;
            $payload['filtersSignature'] = $filtersSignature;
            $json = saveHomeLeaderboardCache($payload, $sim, $class, $glider, $mode);
            echo $json;
        } else {
            echo $cacheEntry['json'];
        }
        return;
    }

    $pdo = new PDO("sqlite:$databasePath");
    $pdo->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);

    if ($filters === null) {
        $filtersPayload = saveHomeLeaderboardFilters(computeHomeLeaderboardFilters($pdo));
        $filters = $filtersPayload['filters'];
        $filtersSignature = $filtersPayload['signature'];
    }

    if ($cacheEntry !== null) {
        $payload = $cacheEntry['payload'];
        $payload['filters'] = $filters;
        $payload['filtersSignature'] = $filtersSignature;
        $json = saveHomeLeaderboardCache($payload, $sim, $class, $glider, $mode);
        echo $json;
        return;
    }

    $payload = generateHomeLeaderboardPayload($pdo, $sim, $class, $glider, $mode, $filters, $filtersSignature);
    $json = saveHomeLeaderboardCache($payload, $sim, $class, $glider, $mode);
    echo $json;
} catch (Throwable $e) {
    http_response_code(500);
    echo json_encode([
        'status' => 'error',
        'message' => $e->getMessage(),
    ], JSON_UNESCAPED_SLASHES | JSON_UNESCAPED_UNICODE);
}
