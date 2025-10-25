<?php
require_once __DIR__ . '/CommonFunctions.php';

// Prefer the configured dir injected via CommonFunctions.php
if (!defined('HOME_LEADERBOARD_CACHE_DIR')) {
    if (!empty($homeLeaderboardCacheDir)) {
        define('HOME_LEADERBOARD_CACHE_DIR', rtrim($homeLeaderboardCacheDir, '/\\'));
    } else {
        // Fallback for safety (legacy behaviour)
        define('HOME_LEADERBOARD_CACHE_DIR', dirname(__DIR__) . '/otherdata/homeleaderboardcache');
    }
}

if (!function_exists('getHomeLeaderboardCacheDir')) {
    function getHomeLeaderboardCacheDir(): string
    {
        $dir = HOME_LEADERBOARD_CACHE_DIR;
        if (!is_dir($dir)) {
            if (!mkdir($dir, 0775, true) && !is_dir($dir)) {
                throw new RuntimeException('Failed to create home leaderboard cache directory.');
            }
        }
        return $dir;
    }
}

if (!function_exists('getHomeLeaderboardFiltersPath')) {
    function getHomeLeaderboardFiltersPath(): string
    {
        return getHomeLeaderboardCacheDir() . '/filters.json';
    }
}

if (!function_exists('buildHomeLeaderboardCacheKey')) {
    /**
     * Build a stable, filesystem-safe cache key for home leaderboard entries.
     *
     * Examples:
     *   SIM=All__CLASS=All__GLIDER=All__MODE=all__H=0123abcd
     *   SIM=MSFS 2020__CLASS=All__GLIDER=All__MODE=top__H=89ef4567
     *   SIM=MSFS 2024__CLASS=18m flapped__GLIDER=AS33-18__MODE=top__H=7654fedc
     */
    function buildHomeLeaderboardCacheKey(
        string $sim,
        string $competitionClass,
        string $gliderType,
        string $mode
    ): string {
        $normalizedMode = (strtolower($mode) === 'all') ? 'all' : 'top';

        $canonical = sprintf(
            'SIM=%s__CLASS=%s__GLIDER=%s__MODE=%s',
            $sim,
            $competitionClass,
            $gliderType,
            $normalizedMode
        );

        $hash = substr(sha1($canonical), 0, 8);

        // Replace disallowed filename characters while keeping readable separators intact.
        $disallowed = ['/', '\\', ':', '?', '*', '"', '<', '>', '|', "\0"];
        $safeBase = str_replace($disallowed, '_', $canonical);

        return $safeBase . '__H=' . $hash;
    }
}

if (!function_exists('normalizeHomeLeaderboardFilterValue')) {
    function normalizeHomeLeaderboardFilterValue($value): string
    {
        if ($value === null) {
            return 'All';
        }

        $stringValue = is_string($value) ? $value : (string) $value;
        $trimmed = trim($stringValue);

        return $trimmed === '' ? 'All' : $trimmed;
    }
}

if (!function_exists('normalizeHomeLeaderboardMode')) {
    function normalizeHomeLeaderboardMode($mode): string
    {
        if (is_string($mode) && strtolower($mode) === 'all') {
            return 'all';
        }

        return 'top';
    }
}

if (!function_exists('getHomeLeaderboardCachePath')) {
    function getHomeLeaderboardCachePath(
        $sim = 'All',
        $competitionClass = 'All',
        $gliderType = 'All',
        $mode = 'top'
    ): string {
        $normalizedSim = normalizeHomeLeaderboardFilterValue($sim);
        $normalizedClass = normalizeHomeLeaderboardFilterValue($competitionClass);
        $normalizedGlider = normalizeHomeLeaderboardFilterValue($gliderType);
        $normalizedMode = normalizeHomeLeaderboardMode($mode);

        $key = buildHomeLeaderboardCacheKey(
            $normalizedSim,
            $normalizedClass,
            $normalizedGlider,
            $normalizedMode
        );

        return getHomeLeaderboardCacheDir() . '/' . $key . '.json';
    }
}

if (!function_exists('writeHomeLeaderboardCacheAtomically')) {
    function writeHomeLeaderboardCacheAtomically(string $path, string $contents): void
    {
        $directory = dirname($path);
        if (!is_dir($directory)) {
            if (!mkdir($directory, 0775, true) && !is_dir($directory)) {
                throw new RuntimeException('Failed to create home leaderboard cache directory.');
            }
        }

        try {
            $suffix = bin2hex(random_bytes(8));
        } catch (Throwable $exception) {
            $suffix = str_replace('.', '', uniqid('', true));
        }

        $temporaryPath = $path . '.tmp.' . $suffix;

        $bytesWritten = file_put_contents($temporaryPath, $contents, LOCK_EX);
        if ($bytesWritten === false) {
            throw new RuntimeException(sprintf('Failed to write temporary cache file: %s', $temporaryPath));
        }

        if (!rename($temporaryPath, $path)) {
            @unlink($temporaryPath);
            throw new RuntimeException(sprintf('Failed to finalize cache file: %s', $path));
        }
    }
}

if (!function_exists('loadHomeLeaderboardCache')) {
    function loadHomeLeaderboardCache(
        $sim,
        $competitionClass,
        $gliderType,
        $mode
    ): ?array
    {
        $path = getHomeLeaderboardCachePath($sim, $competitionClass, $gliderType, $mode);
        if (!is_file($path)) {
            return null;
        }

        $json = file_get_contents($path);
        if ($json === false) {
            return null;
        }

        $payload = json_decode($json, true);
        if (!is_array($payload) || ($payload['status'] ?? null) !== 'success') {
            @unlink($path);
            return null;
        }

        return [
            'path' => $path,
            'key' => basename($path, '.json'),
            'json' => $json,
            'payload' => $payload,
        ];
    }
}

if (!function_exists('saveHomeLeaderboardCache')) {
    function saveHomeLeaderboardCache(
        array $payload,
        $sim,
        $competitionClass,
        $gliderType,
        $mode
    ): string
    {
        $json = json_encode($payload, JSON_UNESCAPED_SLASHES | JSON_UNESCAPED_UNICODE);
        if ($json === false) {
            throw new RuntimeException('Failed to encode home leaderboard payload as JSON.');
        }

        $path = getHomeLeaderboardCachePath($sim, $competitionClass, $gliderType, $mode);
        writeHomeLeaderboardCacheAtomically($path, $json);

        return $json;
    }
}

if (!function_exists('loadHomeLeaderboardFilters')) {
    function loadHomeLeaderboardFilters(): ?array
    {
        $path = getHomeLeaderboardFiltersPath();
        if (!is_file($path)) {
            return null;
        }

        $json = file_get_contents($path);
        if ($json === false) {
            return null;
        }

        $data = json_decode($json, true);
        if (!is_array($data) || !isset($data['filters']) || !is_array($data['filters'])) {
            @unlink($path);
            return null;
        }

        if (!isset($data['signature']) || !is_string($data['signature'])) {
            $data['signature'] = null;
        }

        return $data;
    }
}

if (!function_exists('saveHomeLeaderboardFilters')) {
    function saveHomeLeaderboardFilters(array $filters): array
    {
        $encodedFilters = json_encode($filters, JSON_UNESCAPED_SLASHES | JSON_UNESCAPED_UNICODE);
        if ($encodedFilters === false) {
            throw new RuntimeException('Failed to encode home leaderboard filters as JSON.');
        }

        $signature = sha1($encodedFilters);
        $payload = [
            'signature' => $signature,
            'filters' => $filters,
            'generatedUTC' => gmdate('Y-m-d H:i:s'),
        ];

        $json = json_encode($payload, JSON_UNESCAPED_SLASHES | JSON_UNESCAPED_UNICODE);
        if ($json === false) {
            throw new RuntimeException('Failed to encode home leaderboard filters payload.');
        }

        $path = getHomeLeaderboardFiltersPath();
        if (file_put_contents($path, $json, LOCK_EX) === false) {
            throw new RuntimeException(sprintf('Failed to write home leaderboard filters cache: %s', $path));
        }

        return $payload;
    }
}

if (!function_exists('computeHomeLeaderboardFilters')) {
    function computeHomeLeaderboardFilters(PDO $pdo): array
    {
        $sims = $pdo
            ->query("SELECT DISTINCT Sim FROM TaskBestPerformances WHERE Sim IS NOT NULL AND Sim != '' ORDER BY Sim COLLATE NOCASE")
            ->fetchAll(PDO::FETCH_COLUMN);

        $classes = $pdo
            ->query("SELECT DISTINCT CompetitionClass FROM TaskBestPerformances WHERE CompetitionClass IS NOT NULL AND CompetitionClass != '' ORDER BY CompetitionClass COLLATE NOCASE")
            ->fetchAll(PDO::FETCH_COLUMN);

        $gliderStmt = $pdo->query(<<<'SQL'
            SELECT DISTINCT
                GliderType,
                CompetitionClass
            FROM TaskBestPerformances
            WHERE
                GliderType IS NOT NULL
                AND GliderType != ''
            ORDER BY GliderType COLLATE NOCASE
        SQL);
        $gliderRows = $gliderStmt->fetchAll(PDO::FETCH_ASSOC);

        $gliders = [];
        $glidersByClass = [];

        foreach ($gliderRows as $row) {
            $gliderType = $row['GliderType'];
            $competitionClass = ($row['CompetitionClass'] !== null && $row['CompetitionClass'] !== '')
                ? $row['CompetitionClass']
                : null;

            $gliders[] = [
                'gliderType' => $gliderType,
                'competitionClass' => $competitionClass,
            ];

            if ($competitionClass !== null) {
                if (!isset($glidersByClass[$competitionClass])) {
                    $glidersByClass[$competitionClass] = [];
                }
                if (!in_array($gliderType, $glidersByClass[$competitionClass], true)) {
                    $glidersByClass[$competitionClass][] = $gliderType;
                }
            }
        }

        sort($sims, SORT_NATURAL | SORT_FLAG_CASE);
        sort($classes, SORT_NATURAL | SORT_FLAG_CASE);
        usort($gliders, function ($a, $b) {
            return strcasecmp($a['gliderType'], $b['gliderType']);
        });
        foreach ($glidersByClass as &$list) {
            sort($list, SORT_NATURAL | SORT_FLAG_CASE);
        }
        unset($list);

        return [
            'sims' => $sims,
            'competitionClasses' => $classes,
            'gliders' => $gliders,
            'glidersByClass' => $glidersByClass,
        ];
    }
}

if (!function_exists('fetchHomeLeaderboardRows')) {
    function fetchHomeLeaderboardRows(PDO $pdo, string $mode, string $sim, string $competitionClass, string $gliderType): array
    {
        $modeNormalized = normalizeHomeLeaderboardMode($mode);
        $simNormalized = normalizeHomeLeaderboardFilterValue($sim);
        $classNormalized = normalizeHomeLeaderboardFilterValue($competitionClass);
        $gliderNormalized = normalizeHomeLeaderboardFilterValue($gliderType);

        $params = [];

        $conditions = [
            'ir.Speed IS NOT NULL',
            'ir.IGCUploadDateTimeUTC IS NOT NULL',
        ];

        if ($simNormalized !== 'All') {
            $conditions[] = 'tbp.Sim = :sim';
            $params[':sim'] = $simNormalized;
        }

        if ($classNormalized !== 'All') {
            $conditions[] = 'COALESCE(tbp.CompetitionClass, ir.CompetitionClass) = :class';
            $params[':class'] = $classNormalized;
        }

        if ($gliderNormalized !== 'All') {
            $conditions[] = 'COALESCE(tbp.GliderType, ir.GliderType) = :glider';
            $params[':glider'] = $gliderNormalized;
        }

        if ($modeNormalized === 'all') {
            $whereClause = $conditions ? 'WHERE ' . implode(' AND ', $conditions) : '';
            $sql = <<<SQL
                SELECT
                    tbp.EntrySeqID,
                    tbp.Sim,
                    COALESCE(tbp.CompetitionClass, ir.CompetitionClass) AS CompetitionClass,
                    COALESCE(tbp.GliderType, ir.GliderType) AS GliderType,
                    tbp.IGCKey,
                    ir.Pilot,
                    ir.GliderID,
                    ir.Speed,
                    ir.IGCUploadDateTimeUTC,
                    t.Title
                FROM TaskBestPerformances tbp
                JOIN IGCRecords ir ON tbp.IGCKey = ir.IGCKey
                JOIN Tasks t ON tbp.EntrySeqID = t.EntrySeqID
                $whereClause
                ORDER BY ir.IGCUploadDateTimeUTC DESC
                LIMIT 10
            SQL;
        } else {
            $innerConditions = [
                'tbp2.EntrySeqID = tbp.EntrySeqID',
                'ir2.Speed IS NOT NULL',
                'ir2.IGCUploadDateTimeUTC IS NOT NULL',
            ];

            if ($simNormalized !== 'All') {
                $innerConditions[] = 'tbp2.Sim = :sim';
            }

            if ($classNormalized !== 'All') {
                $innerConditions[] = 'COALESCE(tbp2.CompetitionClass, ir2.CompetitionClass) = :class';
            }

            if ($gliderNormalized !== 'All') {
                $innerConditions[] = 'COALESCE(tbp2.GliderType, ir2.GliderType) = :glider';
            }

            $conditions[] = 'tbp.IGCKey = (
                SELECT tbp2.IGCKey
                FROM TaskBestPerformances tbp2
                JOIN IGCRecords ir2 ON tbp2.IGCKey = ir2.IGCKey
                WHERE ' . implode(' AND ', $innerConditions) . '
                ORDER BY ir2.Speed DESC, ir2.IGCUploadDateTimeUTC DESC, tbp2.IGCKey DESC
                LIMIT 1
            )';

            $whereClause = 'WHERE ' . implode(' AND ', $conditions);

            $sql = <<<SQL
                SELECT
                    tbp.EntrySeqID,
                    tbp.Sim,
                    COALESCE(tbp.CompetitionClass, ir.CompetitionClass) AS CompetitionClass,
                    COALESCE(tbp.GliderType, ir.GliderType) AS GliderType,
                    tbp.IGCKey,
                    ir.Pilot,
                    ir.GliderID,
                    ir.Speed,
                    ir.IGCUploadDateTimeUTC,
                    t.Title
                FROM TaskBestPerformances tbp
                JOIN IGCRecords ir ON tbp.IGCKey = ir.IGCKey
                JOIN Tasks t ON tbp.EntrySeqID = t.EntrySeqID
                $whereClause
                ORDER BY ir.IGCUploadDateTimeUTC DESC
                LIMIT 10
            SQL;
        }

        $stmt = $pdo->prepare($sql);
        $stmt->execute($params);
        $records = $stmt->fetchAll(PDO::FETCH_ASSOC);

        return array_map(function (array $row): array {
            return [
                'igcKey' => $row['IGCKey'],
                'entrySeqID' => isset($row['EntrySeqID']) ? (int) $row['EntrySeqID'] : null,
                'taskTitle' => $row['Title'],
                'pilot' => $row['Pilot'],
                'gliderType' => $row['GliderType'],
                'gliderId' => $row['GliderID'],
                'competitionClass' => $row['CompetitionClass'],
                'speed' => $row['Speed'] !== null ? round((float) $row['Speed'], 1) : null,
                'sim' => $row['Sim'],
                'igcUploadDateTimeUTC' => $row['IGCUploadDateTimeUTC'],
            ];
        }, $records);
    }
}

if (!function_exists('generateHomeLeaderboardPayload')) {
    function generateHomeLeaderboardPayload(
        PDO $pdo,
        string $sim,
        string $competitionClass,
        string $gliderType,
        string $mode,
        array $filters,
        string $filtersSignature
    ): array {
        $normalizedSim = normalizeHomeLeaderboardFilterValue($sim);
        $normalizedClass = normalizeHomeLeaderboardFilterValue($competitionClass);
        $normalizedGlider = normalizeHomeLeaderboardFilterValue($gliderType);
        $modeNormalized = normalizeHomeLeaderboardMode($mode);

        $rows = fetchHomeLeaderboardRows(
            $pdo,
            $modeNormalized,
            $normalizedSim,
            $normalizedClass,
            $normalizedGlider
        );

        return [
            'status' => 'success',
            'mode' => $modeNormalized,
            'activeFilters' => [
                'sim' => $normalizedSim,
                'competitionClass' => $normalizedClass,
                'gliderType' => $normalizedGlider,
            ],
            'filters' => $filters,
            'rows' => $rows,
            'resultCount' => count($rows),
            'filtersSignature' => $filtersSignature,
        ];
    }
}

if (!function_exists('buildHomeLeaderboardFilterVariants')) {
    function buildHomeLeaderboardFilterVariants($value): array
    {
        $variants = ['All'];
        if ($value === null) {
            return $variants;
        }

        $stringValue = is_string($value) ? trim($value) : trim((string) $value);
        if ($stringValue !== '' && !in_array($stringValue, $variants, true)) {
            $variants[] = $stringValue;
        }

        return $variants;
    }
}

if (!function_exists('refreshHomeLeaderboardCaches')) {
    function refreshHomeLeaderboardCaches(PDO $pdo, $sim, $competitionClass, $gliderType): void
    {
        $filtersData = computeHomeLeaderboardFilters($pdo);
        $filtersPayload = saveHomeLeaderboardFilters($filtersData);
        $filtersSignature = $filtersPayload['signature'];
        $filtersData = $filtersPayload['filters'];

        $simVariants = buildHomeLeaderboardFilterVariants($sim);
        $classVariants = buildHomeLeaderboardFilterVariants($competitionClass);
        $gliderVariants = buildHomeLeaderboardFilterVariants($gliderType);

        foreach ($simVariants as $simValue) {
            foreach ($classVariants as $classValue) {
                foreach ($gliderVariants as $gliderValue) {
                    foreach (['top', 'all'] as $mode) {
                        $payload = generateHomeLeaderboardPayload(
                            $pdo,
                            $simValue,
                            $classValue,
                            $gliderValue,
                            $mode,
                            $filtersData,
                            $filtersSignature
                        );
                        saveHomeLeaderboardCache(
                            $payload,
                            $simValue,
                            $classValue,
                            $gliderValue,
                            $mode
                        );
                    }
                }
            }
        }
    }
}
