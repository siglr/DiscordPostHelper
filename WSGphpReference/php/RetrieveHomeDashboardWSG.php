<?php
// measure start
$__start = microtime(true);

// —————————————————————————————
// 0) JSON CACHING (lightweight, no require yet)
// —————————————————————————————
$cacheDir  = __DIR__ . '/../otherdata';
$cacheFile = $cacheDir . '/dashboardCache.json';
$cacheTTL  = 60; // seconds
$skipCache = isset($_GET['nocache']);

// ensure cache directory exists
if (! is_dir($cacheDir)) {
    @mkdir($cacheDir, 0755, true);
}

if (! $skipCache
    && file_exists($cacheFile)
    && (time() - filemtime($cacheFile) < $cacheTTL)
) {
    header('X-Debug: served-from-cache');
    // timing header
    header('X-Timing: ' . round((microtime(true) - $__start)*1000) . 'ms');
    header('Content-Type: application/json; charset=UTF-8');
    readfile($cacheFile);
    exit;
}

// cache miss → we’ll regenerate
header('X-Debug: regenerating-cache');

// —————————————————————————————
// 1) Now pull in your config & CommonFunctions
// —————————————————————————————
require __DIR__ . '/CommonFunctions.php';

try {
    // 2) Open News DB and attach Tasks DB
    $pdo = new PDO("sqlite:$newsDBPath");
    $pdo->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);
    $pdo->exec("ATTACH DATABASE '$databasePath' AS taskdb");

    // speed-up pragmas
    $pdo->exec("PRAGMA journal_mode = WAL");
    $pdo->exec("PRAGMA synchronous = NORMAL");

    $sevenAgo = (new DateTime('now', new DateTimeZone('UTC')))
                  ->modify('-7 days')
                  ->format('Y-m-d H:i:s');

    // — popularTasks query …
    $popularResults = [];            // key: days → array of entries
    $popularWindows = [7,14,30,90,180];

    foreach ($popularWindows as $days) {
        // compute cutoff
        $cutoff = (new DateTime('now', new DateTimeZone('UTC')))
                      ->modify("-{$days} days")
                      ->format('Y-m-d H:i:s');

        // fetch top 10 downloads since cutoff
        $stmt = $pdo->prepare(<<<'SQL'
            SELECT
              t.EntrySeqID,
              t.TaskID,
              t.Title    AS Subtitle,
              COUNT(*)   AS DownloadCount
            FROM taskdb.TaskDownloads td
            JOIN taskdb.Tasks t
              ON td.EntrySeqID = t.EntrySeqID
            WHERE
              td.Date >= :cutoff
              AND (t.Availability IS NULL OR datetime(t.Availability) <= datetime('now'))
            GROUP BY
              t.EntrySeqID, t.TaskID, t.Title
            ORDER BY
              DownloadCount DESC
            LIMIT 10
        SQL);
        $stmt->execute([':cutoff' => $cutoff]);
        $raw = $stmt->fetchAll(PDO::FETCH_ASSOC);

        // map to your JS schema
        $mapped = array_map(function($r){
            return [
                'EntrySeqID'    => (int)$r['EntrySeqID'],
                'TaskID'        => $r['TaskID'],
                'Subtitle'      => $r['Subtitle'],
                'DownloadCount' => (int)$r['DownloadCount'],
                'available'     => true
            ];
        }, $raw);

        // store in array
        $popularResults[$days] = $mapped;
    }

    // all-time popular tasks (no cutoff)
    $stmt = $pdo->prepare(<<<'SQL'
        SELECT
          t.EntrySeqID,
          t.TaskID,
          t.Title    AS Subtitle,
          COUNT(*)   AS DownloadCount
        FROM taskdb.TaskDownloads td
        JOIN taskdb.Tasks t
          ON td.EntrySeqID = t.EntrySeqID
        WHERE
          (t.Availability IS NULL OR datetime(t.Availability) <= datetime('now'))
        GROUP BY
          t.EntrySeqID, t.TaskID, t.Title
        ORDER BY
          DownloadCount DESC
        LIMIT 10
    SQL);
    $stmt->execute();
    $raw = $stmt->fetchAll(PDO::FETCH_ASSOC);

    $mapped = array_map(function($r){
        return [
            'EntrySeqID'    => (int)$r['EntrySeqID'],
            'TaskID'        => $r['TaskID'],
            'Subtitle'      => $r['Subtitle'],
            'DownloadCount' => (int)$r['DownloadCount'],
            'available'     => true
        ];
    }, $raw);

    $popularResults['All'] = $mapped;

    // — newTasks query …
    $stmt = $pdo->prepare(<<<'SQL'
        SELECT n.EntrySeqID, t.TaskID, n.Subtitle AS Subtitle, n.Published AS PublishedUTC
        FROM News n
        JOIN taskdb.Tasks t ON n.EntrySeqID = t.EntrySeqID
        WHERE n.NewsType = 0
          AND n.Published >= :sevenAgo
          AND (t.Availability IS NULL OR datetime(t.Availability) <= datetime('now'))
        ORDER BY n.Published DESC
        LIMIT 10
    SQL);
    $stmt->execute([':sevenAgo' => $sevenAgo]);
    $newRaw = $stmt->fetchAll(PDO::FETCH_ASSOC);
    $newTasks = array_map(function($r){
        return [
            'EntrySeqID'   => (int)$r['EntrySeqID'],
            'TaskID'       => $r['TaskID'],
            'Subtitle'     => $r['Subtitle'],
            'PublishedUTC' => $r['PublishedUTC'],
            'available'    => true
        ];
    }, $newRaw);

    // — upcomingEvents query — include any event from now–90min onward,
    // and compute availability exactly as in your other queries (NULL or ≤ now)
    $stmt = $pdo->prepare(<<<'SQL'
        SELECT
          n.Key                     AS NewsKey,
          n.EntrySeqID,
          t.TaskID,
          t.Availability,
          n.Title                   AS ClubName,
          n.Subtitle                AS Subtitle,
          e.EventMeetDateTime,
          -- same availability rule as popular/new tasks:
          (t.Availability IS NULL
           OR datetime(t.Availability) <= datetime('now')
          )                         AS available
        FROM News n
        JOIN Events e
          ON n.Key = e.EventKey
        LEFT JOIN taskdb.Tasks t
          ON n.EntrySeqID = t.EntrySeqID
        WHERE
          n.NewsType = 1
          AND e.EventMeetDateTime >= datetime('now', '-90 minutes')
        ORDER BY
          e.EventMeetDateTime ASC
        LIMIT 10
    SQL
    );
    $stmt->execute();
    $evtRaw = $stmt->fetchAll(PDO::FETCH_ASSOC);
    
    $upcomingEvents = array_map(function($r){
        return [
            'NewsKey'           => $r['NewsKey'],
            'EntrySeqID'        => (int)$r['EntrySeqID'],
            'TaskID'            => $r['TaskID'],
            'ClubName'          => $r['ClubName'],
            'Subtitle'          => $r['Subtitle'],
            'EventMeetDateTime' => $r['EventMeetDateTime'],
            // cast the SQLite Boolean (0/1) to PHP true/false
            'available'         => (bool)$r['available']
        ];
    }, $evtRaw);

    // 4) Build JSON & write cache
$payload = ['status' => 'success'];

    // loop our popularResults map and inject each into payload
    foreach ($popularResults as $days => $list) {
        $payload["popularTasks{$days}"] = $list;
    }

    // add the rest under their existing keys
    $payload['newTasks']       = $newTasks;
    $payload['upcomingEvents'] = $upcomingEvents;

    $json = json_encode($payload, JSON_UNESCAPED_SLASHES);
    if (file_put_contents($cacheFile, $json) === false) {
        error_log("Failed to write cache: $cacheFile");
    }
     
    // 5) Send JSON
    header('Content-Type: application/json; charset=UTF-8');
    // timing header
    header('X-Timing: ' . round((microtime(true) - $__start)*1000) . 'ms');
    echo $json;

} catch (Exception $e) {
    error_log("RetrieveHomeDashboardWSG error: " . $e->getMessage());
    header('Content-Type: application/json', true, 500);
    echo json_encode(['status'=>'error','message'=>$e->getMessage()]);
}
