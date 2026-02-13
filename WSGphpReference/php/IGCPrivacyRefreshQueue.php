<?php

require_once __DIR__ . '/CommonFunctions.php';

function getIgcPrivacyRefreshQueuePath(): string
{
    global $homeLeaderboardCacheDir;

    $baseDir = '';
    if (!empty($homeLeaderboardCacheDir)) {
        $baseDir = dirname(rtrim((string) $homeLeaderboardCacheDir, '/\\'));
    }

    if ($baseDir === '' || $baseDir === '.' || $baseDir === DIRECTORY_SEPARATOR) {
        $baseDir = dirname(__DIR__) . '/otherdata';
    }

    if (!is_dir($baseDir)) {
        mkdir($baseDir, 0775, true);
    }

    return rtrim($baseDir, '/\\') . '/igcPrivacyRefreshQueue.json';
}

function withIgcPrivacyQueueLock(callable $callback)
{
    $queuePath = getIgcPrivacyRefreshQueuePath();
    $lockPath = $queuePath . '.lock';

    $lockHandle = fopen($lockPath, 'c+');
    if ($lockHandle === false) {
        throw new RuntimeException('Unable to open IGC privacy queue lock file.');
    }

    try {
        if (!flock($lockHandle, LOCK_EX)) {
            throw new RuntimeException('Unable to lock IGC privacy queue lock file.');
        }

        return $callback($queuePath);
    } finally {
        flock($lockHandle, LOCK_UN);
        fclose($lockHandle);
    }
}

function loadIgcPrivacyRefreshQueueState(string $queuePath): array
{
    if (!file_exists($queuePath)) {
        return [
            'groups' => [],
            'refreshFastest' => 0,
            'updatedAtUtc' => gmdate('Y-m-d H:i:s'),
        ];
    }

    $raw = file_get_contents($queuePath);
    if ($raw === false || trim($raw) === '') {
        return [
            'groups' => [],
            'refreshFastest' => 0,
            'updatedAtUtc' => gmdate('Y-m-d H:i:s'),
        ];
    }

    $decoded = json_decode($raw, true);
    if (!is_array($decoded)) {
        return [
            'groups' => [],
            'refreshFastest' => 0,
            'updatedAtUtc' => gmdate('Y-m-d H:i:s'),
        ];
    }

    if (!isset($decoded['groups']) || !is_array($decoded['groups'])) {
        $decoded['groups'] = [];
    }
    $decoded['refreshFastest'] = !empty($decoded['refreshFastest']) ? 1 : 0;

    return $decoded;
}

function saveIgcPrivacyRefreshQueueState(string $queuePath, array $state): void
{
    $state['updatedAtUtc'] = gmdate('Y-m-d H:i:s');
    $json = json_encode($state, JSON_PRETTY_PRINT | JSON_UNESCAPED_UNICODE);
    if ($json === false) {
        throw new RuntimeException('Unable to encode IGC privacy refresh queue.');
    }

    $tmpPath = $queuePath . '.tmp.' . str_replace('.', '', uniqid('', true));
    if (file_put_contents($tmpPath, $json, LOCK_EX) === false) {
        throw new RuntimeException('Unable to write temporary IGC privacy queue file.');
    }
    if (!rename($tmpPath, $queuePath)) {
        @unlink($tmpPath);
        throw new RuntimeException('Unable to finalize IGC privacy queue file.');
    }
}

function enqueueIgcPrivacyRefreshWork(array $groups, bool $refreshFastest): int
{
    return withIgcPrivacyQueueLock(function (string $queuePath) use ($groups, $refreshFastest): int {
        $state = loadIgcPrivacyRefreshQueueState($queuePath);

        foreach ($groups as $groupKey => $group) {
            if (!is_array($group)) {
                continue;
            }

            $entrySeqID = isset($group['EntrySeqID']) ? (int) $group['EntrySeqID'] : 0;
            $sim = isset($group['Sim']) ? trim((string) $group['Sim']) : '';
            if ($entrySeqID <= 0 || $sim === '') {
                continue;
            }

            $state['groups'][(string) $groupKey] = [
                'EntrySeqID' => $entrySeqID,
                'Sim' => $sim,
                'CompetitionClass' => array_key_exists('CompetitionClass', $group) ? $group['CompetitionClass'] : null,
                'GliderType' => array_key_exists('GliderType', $group) ? $group['GliderType'] : null,
            ];
        }

        if ($refreshFastest) {
            $state['refreshFastest'] = 1;
        }

        saveIgcPrivacyRefreshQueueState($queuePath, $state);
        return count($state['groups']);
    });
}


function triggerIgcPrivacyRefreshWorker(): bool
{
    $scriptPath = __DIR__ . '/ProcessIGCPrivacyRefreshQueue.php';
    if (!is_file($scriptPath)) {
        return false;
    }

    $phpBinary = defined('PHP_BINARY') && PHP_BINARY ? PHP_BINARY : 'php';
    $command = escapeshellarg($phpBinary) . ' ' . escapeshellarg($scriptPath) . ' > /dev/null 2>&1 &';

    if (function_exists('exec')) {
        @exec($command, $output, $resultCode);
        if ((int) $resultCode === 0) {
            return true;
        }
    }

    // Fallback for hosts where exec is disabled: process after response flush.
    register_shutdown_function(static function (): void {
        try {
            if (function_exists('fastcgi_finish_request')) {
                @fastcgi_finish_request();
            }

            global $databasePath;
            $pdo = createSqliteConnection($databasePath);
            processIgcPrivacyRefreshQueue($pdo);
        } catch (Throwable $fallbackError) {
            // Keep silent to avoid breaking user-facing responses.
        }
    });

    return false;
}

function processIgcPrivacyRefreshQueue(PDO $pdo): array
{
    $work = withIgcPrivacyQueueLock(function (string $queuePath): array {
        $state = loadIgcPrivacyRefreshQueueState($queuePath);

        return [
            'groups' => $state['groups'] ?? [],
            'refreshFastest' => !empty($state['refreshFastest']),
            'updatedAtUtc' => $state['updatedAtUtc'] ?? '',
        ];
    });

    $cacheRefreshCount = 0;
    $uniqueScopes = [];

    foreach ($work['groups'] as $groupKey => $group) {
        if (!is_array($group)) {
            continue;
        }

        $entrySeqID = isset($group['EntrySeqID']) ? (int) $group['EntrySeqID'] : 0;
        $sim = isset($group['Sim']) ? (string) $group['Sim'] : '';
        $competitionClass = $group['CompetitionClass'] ?? null;
        $gliderType = $group['GliderType'] ?? null;

        if ($entrySeqID <= 0 || $sim === '') {
            continue;
        }

        $groupChanged = refreshTaskBestPerformanceGroup($pdo, $entrySeqID, $sim, $competitionClass, $gliderType);
        if ($groupChanged) {
            $scopeKey = buildGroupKey(0, $sim, $competitionClass, $gliderType);
            $uniqueScopes[$scopeKey] = [
                'Sim' => $sim,
                'CompetitionClass' => $competitionClass,
                'GliderType' => $gliderType,
            ];
        }
    }

    foreach ($uniqueScopes as $scope) {
        refreshHomeLeaderboardCaches(
            $pdo,
            $scope['Sim'],
            $scope['CompetitionClass'],
            $scope['GliderType']
        );
        $cacheRefreshCount++;
    }

    $fastestCacheRefreshed = false;
    if (!empty($work['refreshFastest'])) {
        $fastestCacheRefreshed = refreshFastestGliderSpeedsCache($pdo);
    }

    // Remove only completed work after successful processing.
    withIgcPrivacyQueueLock(function (string $queuePath) use ($work): void {
        $state = loadIgcPrivacyRefreshQueueState($queuePath);

        if (!empty($work['groups']) && isset($state['groups']) && is_array($state['groups'])) {
            foreach ($work['groups'] as $groupKey => $group) {
                unset($state['groups'][(string) $groupKey]);
            }
        }

        if (!empty($work['refreshFastest'])) {
            // Clear only when no newer enqueue happened while processing.
            if (($state['updatedAtUtc'] ?? '') === ($work['updatedAtUtc'] ?? '')) {
                $state['refreshFastest'] = 0;
            }
        }

        saveIgcPrivacyRefreshQueueState($queuePath, $state);
    });

    return [
        'queuedGroupCount' => count($work['groups']),
        'cacheRefreshCount' => $cacheRefreshCount,
        'fastestCacheRefreshed' => $fastestCacheRefreshed ? 1 : 0,
    ];
}
