<?php
require_once __DIR__ . '/session_restore.php';
require_once __DIR__ . '/CommonFunctions.php';

try {
    $pdo = new PDO("sqlite:$databasePath");
    $pdo->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);

    // Check user session; if active, get the user's ID, otherwise 0.
    $wsgUserID = (isset($_SESSION['user']) && isset($_SESSION['user']['id']))
        ? (int)$_SESSION['user']['id']
        : 0;

    // Get the filter values from query parameters
    $taskCount = isset($_GET['taskCount']) ? (int)$_GET['taskCount'] : PHP_INT_MAX;
    $startDate = $_GET['startDate'] ?? '2000-01-01';
    $endDate = $_GET['endDate'] ?? date('Y-m-d');
    $endDate = date('Y-m-d', strtotime($endDate . ' +1 day'));
    $durationMin = isset($_GET['durationMin']) ? (int)$_GET['durationMin'] : 0;
    $durationMax = isset($_GET['durationMax']) ? (int)$_GET['durationMax'] : PHP_INT_MAX;
    $includeNoDuration = isset($_GET['includeNoDuration']) ? (bool)$_GET['includeNoDuration'] : true;

    // Get soaring type filters from query parameters
    $soaringTypes = [
        'soaringRidge'    => isset($_GET['soaringRidge']) ? (int)$_GET['soaringRidge'] : 1,
        'soaringThermals' => isset($_GET['soaringThermals']) ? (int)$_GET['soaringThermals'] : 1,
        'soaringWaves'    => isset($_GET['soaringWaves']) ? (int)$_GET['soaringWaves'] : 1,
        'soaringDynamic'  => isset($_GET['soaringDynamic']) ? (int)$_GET['soaringDynamic'] : 1
    ];
    $soaringTypeFilter = $_GET['soaringTypeFilter'] ?? 'any';

    // Get group events filter values from query parameters
    $groupEvents = isset($_GET['groupEvents']) ? strtolower(trim($_GET['groupEvents'])) : null; // any | none | specific | (null=ignore)
    $eventNewsIDs = [];
    if ($groupEvents === 'specific' && !empty($_GET['eventNewsIDs'])) {
        $eventNewsIDs = array_values(array_filter(array_map('trim', explode(',', $_GET['eventNewsIDs']))));
    }

    $difficultyFiltersRaw = $_GET['difficultyFilters'] ?? null;
    $difficultyFilters = [];
    if ($difficultyFiltersRaw) {
        $decoded = json_decode($difficultyFiltersRaw, true);
        if (json_last_error() === JSON_ERROR_NONE && is_array($decoded)) {
            foreach ($decoded as $value) {
                if (!is_string($value)) {
                    continue;
                }
                $trimmed = trim($value);
                if ($trimmed === '') {
                    continue;
                }
                $difficultyFilters[] = function_exists('mb_substr') ? mb_substr($trimmed, 0, 255) : substr($trimmed, 0, 255);
            }
        }
        $difficultyFilters = array_values(array_unique($difficultyFilters));
    }

    // Compute current UTC time in PHP
    $nowUTC = (new DateTime('now', new DateTimeZone('UTC')))->format('Y-m-d H:i:s');

    // Base WHERE clause for date range, status, and availability
    $whereClauses = [
        "LastUpdate BETWEEN :startDate AND :endDate",
        "Status = 99",
        "(Availability IS NULL OR Availability <= :nowUTC)"
    ];

    $params = [
        ':startDate'   => $startDate,
        ':endDate'     => $endDate,
        ':taskCount'   => $taskCount,
        ':durationMin' => $durationMin,
        ':durationMax' => $durationMax,
        ':nowUTC'      => $nowUTC
    ];

    // Add soaring type conditions if required
    $soaringConditions = [];
    $allTypesSelected = array_reduce($soaringTypes, fn($carry, $value) => $carry && $value, true);
    if (!($soaringTypeFilter === 'any' && $allTypesSelected)) {
        foreach ($soaringTypes as $column => $value) {
            if ($value) {
                switch ($soaringTypeFilter) {
                    case 'any':
                        $soaringConditions[] = "$column = 1";
                        break;
                    case 'all':
                        $soaringConditions[] = "$column = 1";
                        break;
                    case 'only':
                        $soaringConditions[] = "$column = 1";
                        break;
                    case 'exclude':
                        $soaringConditions[] = "$column = 0";
                        break;
                }
            } elseif ($soaringTypeFilter === 'only') {
                $soaringConditions[] = "$column = 0";
            }
        }
    }

    // Add soaring type conditions to WHERE clause
    if (!empty($soaringConditions)) {
        $whereClauses[] = $soaringTypeFilter === 'any'
            ? '(' . implode(' OR ', $soaringConditions) . ')'
            : '(' . implode(' AND ', $soaringConditions) . ')';
    }

    if (!empty($difficultyFilters)) {
        $difficultyClauses = [];
        foreach ($difficultyFilters as $index => $difficultyValue) {
            if (strcasecmp($difficultyValue, '0. None / Custom') === 0) {
                $difficultyClauses[] = "(T.DifficultyRating = '0. None / Custom' AND (T.DifficultyExtraInfo IS NULL OR TRIM(T.DifficultyExtraInfo) = ''))";
            } else {
                $paramName = ":difficulty_$index";
                $difficultyClauses[] = "((T.DifficultyRating = $paramName) OR (T.DifficultyRating = '0. None / Custom' AND TRIM(COALESCE(T.DifficultyExtraInfo, '')) = $paramName))";
                $params[$paramName] = $difficultyValue;
            }
        }

        if (!empty($difficultyClauses)) {
            $whereClauses[] = '(' . implode(' OR ', $difficultyClauses) . ')';
        }
    }

    // Add duration conditions
    $durationConditions = [
        "((NOT (DurationMin IS NULL OR DurationMin = '' OR DurationMin = 0)) AND (NOT (DurationMax IS NULL OR DurationMax = '' OR DurationMax = 0)) AND DurationMin >= :durationMin AND DurationMax <= :durationMax)",
        "((NOT (DurationMin IS NULL OR DurationMin = '' OR DurationMin = 0)) AND (DurationMax IS NULL OR DurationMax = '' OR DurationMax = 0) AND DurationMin >= :durationMin AND DurationMin <= :durationMax)",
        "((DurationMin IS NULL OR DurationMin = '' OR DurationMin = 0) AND (NOT (DurationMax IS NULL OR DurationMax = '' OR DurationMax = 0)) AND DurationMax >= :durationMin AND DurationMax <= :durationMax)"
    ];

    // Include tasks with no duration if specified
    if ($includeNoDuration) {
        $durationConditions[] = "((DurationMin = 0 OR DurationMin = '' OR DurationMin IS NULL) AND (DurationMax = 0 OR DurationMax = '' OR DurationMax IS NULL))";
    }

    // Add duration conditions to WHERE clause
    $whereClauses[] = '(' . implode(' OR ', $durationConditions) . ')';

    // --- TaskEvents club-based filtering ---
    if ($groupEvents === 'any') {
        $whereClauses[] =
            "EXISTS (SELECT 1 FROM TaskEvents TE WHERE TE.EntrySeqID = T.EntrySeqID)";
    } elseif ($groupEvents === 'none') {
        $whereClauses[] =
            "NOT EXISTS (SELECT 1 FROM TaskEvents TE WHERE TE.EntrySeqID = T.EntrySeqID)";
    } elseif ($groupEvents === 'specific') {
        if (!empty($eventNewsIDs)) {
            // Build IN (...) with named placeholders
            $inPlaceholders = [];
            foreach ($eventNewsIDs as $i => $val) {
                $ph = ":eventNews_$i";
                $inPlaceholders[] = $ph;
                $params[$ph] = $val;
            }
            $whereClauses[] =
                "EXISTS (SELECT 1 FROM TaskEvents TE
                         WHERE TE.EntrySeqID = T.EntrySeqID
                           AND TE.ClubEventNewsID IN (" . implode(',', $inPlaceholders) . "))";
        } else {
            // If 'specific' but no IDs provided, treat as 'none' or ignore — choose one:
            // Here we choose to IGNORE (no-op). If you prefer 'none', swap for NOT EXISTS.
            // $whereClauses[] = "1=1"; // no-op
        }
    }

    // Final query
    $query = "
        SELECT 
            T.EntrySeqID, 
            T.TaskID, 
            T.Title, 
            T.LatMin, 
            T.LatMax, 
            T.LongMin, 
            T.LongMax, 
            T.PLNXML,
            T.MainAreaPOI, 
            T.DepartureName, 
            T.DepartureICAO, 
            T.ArrivalName, 
            T.ArrivalICAO,
            T.SoaringRidge, 
            T.SoaringThermals, 
            T.SoaringWaves, 
            T.SoaringDynamic, 
            T.SoaringExtraInfo,
            T.DurationMin, 
            T.DurationMax, 
            T.TaskDistance, 
            T.TotalDistance, 
            T.RecommendedGliders,
            T.DifficultyRating, 
            T.DifficultyExtraInfo, 
            T.Credits, 
            T.Countries, 
            T.LastUpdate,
            CASE WHEN UT.MarkedFlownDateUTC IS NOT NULL THEN 1 ELSE 0 END AS MarkedFlown,
            CASE WHEN UT.MarkedFlyNextUTC IS NOT NULL THEN 1 ELSE 0 END AS MarkedFlyNext,
            CASE WHEN UT.MarkedFavoritesUTC IS NOT NULL THEN 1 ELSE 0 END AS MarkedFavorites,
            COALESCE(IRCounts.IGCRecordCount, 0) AS IGCRecordCount
        FROM Tasks T
        LEFT JOIN (
            SELECT EntrySeqID, COUNT(*) AS IGCRecordCount
              FROM IGCRecords
             WHERE COALESCE(IsPrivate, 0) = 0
             GROUP BY EntrySeqID
        ) IRCounts
          ON IRCounts.EntrySeqID = T.EntrySeqID
        LEFT JOIN UsersTasks UT
            ON UT.EntrySeqID = T.EntrySeqID 
            AND UT.WSGUserID = :wsgUserID
        WHERE " . implode(' AND ', $whereClauses) . "
        ORDER BY T.LastUpdate DESC
        LIMIT :taskCount
    ";
    // Bind the user ID for the join
    $params[':wsgUserID'] = $wsgUserID;

    // Execute query with parameters
    $stmt = $pdo->prepare($query);
    foreach ($params as $key => $value) {
        $stmt->bindValue($key, $value, is_int($value) ? PDO::PARAM_INT : PDO::PARAM_STR);
    }
    $stmt->execute();
    $tasks = $stmt->fetchAll(PDO::FETCH_ASSOC);

    // Additional query for total task count
    $countQuery = "SELECT COUNT(*) as totalTasks FROM Tasks WHERE (Availability IS NULL OR Availability <= :nowUTC)";
    $countStmt = $pdo->prepare($countQuery);
    $countStmt->execute([':nowUTC' => $nowUTC]);
    $totalTasks = $countStmt->fetch(PDO::FETCH_ASSOC)['totalTasks'];

    // Additional query for oldest and newest dates
    $dateQuery = "SELECT MIN(LastUpdate) as oldestDate, MAX(LastUpdate) as newestDate FROM Tasks";
    $dateStmt = $pdo->prepare($dateQuery);
    $dateStmt->execute();
    $dates = $dateStmt->fetch(PDO::FETCH_ASSOC);

    // Response
    $response = [
        'tasks'      => $tasks,
        'totalTasks' => $totalTasks,
        'oldestDate' => $dates['oldestDate'],
        'newestDate' => $dates['newestDate'],
        'difficultyOptions' => []
    ];

    $difficultySet = [];
    $difficultyQuery = "SELECT DifficultyRating, DifficultyExtraInfo FROM Tasks";
    $difficultyStmt = $pdo->prepare($difficultyQuery);
    $difficultyStmt->execute();
    while ($row = $difficultyStmt->fetch(PDO::FETCH_ASSOC)) {
        $rating = trim((string)($row['DifficultyRating'] ?? ''));
        $extra = trim((string)($row['DifficultyExtraInfo'] ?? ''));

        if ($rating === '0. None / Custom' || $rating === '') {
            if ($extra !== '') {
                $difficultySet[$extra] = true;
            } else {
                $difficultySet['0. None / Custom'] = true;
            }
        } elseif ($rating !== '') {
            $difficultySet[$rating] = true;
        }
    }

    if (!empty($difficultySet)) {
        $difficultyOptions = array_keys($difficultySet);
        natcasesort($difficultyOptions);
        $response['difficultyOptions'] = array_values($difficultyOptions);
    }

    header('Content-Type: application/json');
    echo json_encode($response);

} catch (PDOException $e) {
    logMessage("Connection failed: " . $e->getMessage());
    header('Content-Type: application/json');
    echo json_encode(['error' => 'Connection failed']);
}
?>
