<?php
require_once __DIR__ . '/CommonFunctions.php';
require_once __DIR__ . '/TrackerTask.php';

/**
 * ─────────────────────────────────────────────
 * DEBUG TOGGLE
 * Set to true for verbose logging via logMessage()
 * ─────────────────────────────────────────────
 */
$DEBUG_LOG = false;   // ← flip to false when done testing

function dbg($msg) {
    global $DEBUG_LOG;
    if ($DEBUG_LOG) logMessage('[TrackerPushEvent] ' . $msg);
}

function taskToJson(Task $task): string {
    // Formatting.None → default; keep slashes unescaped like C#
    return json_encode($task, JSON_UNESCAPED_SLASHES);
}

/** Parse EventNewsID from EventKey like "E-DIAMTU20251014" → "DIAMTU" */
function extractEventNewsId(string $eventKey): ?string {
    if (preg_match('/^E-([A-Za-z0-9]+)\d{8}$/', trim($eventKey), $m)) {
        return $m[1];
    }
    return null;
}

/** Post task to Tracker server */
function trackerSetSharedTask(array $opts): array {
    global $DEBUG_LOG;
    $endpoint = $opts['endpoint'] ?? 'https://ssc-tracker.org/settask.php';

    $payload = [
        'CMD'          => 'UPLOAD',
        'GN'           => $opts['groupName'],
        'ID'           => $opts['pilotId'] ?? 'WSG',
        'SECRET'       => md5($opts['plaintextSecret']),
        'TASK'         => $opts['taskTitle'],
        'TASKDATA'     => $opts['taskData'],
        'TASKDATAJSON' => $opts['taskDataJson'] ?? '',
        'TASKINFO'     => $opts['taskInfo']   ?? null,
        'WEATHER'      => $opts['weather']    ?? null,
        'WEATHERDATA'  => $opts['weatherData']?? null,
    ];

    dbg("Posting to Tracker endpoint: $endpoint");
    if ($DEBUG_LOG) {
      $logPayload = $payload;
      $logPayload['SECRET'] = '***';
      foreach (['TASKDATA','TASKDATAJSON'] as $k) {
        if (!empty($logPayload[$k]) && strlen($logPayload[$k]) > 800) {
          $logPayload[$k] = substr($logPayload[$k], 0, 800) . ' …';
        }
      }
      dbg('Payload: ' . json_encode($logPayload, JSON_UNESCAPED_SLASHES));
    }

    $ch = curl_init($endpoint);
    curl_setopt_array($ch, [
        CURLOPT_POST           => true,
        CURLOPT_HTTPHEADER     => ['Content-Type: application/json'],
        CURLOPT_POSTFIELDS     => json_encode($payload, JSON_UNESCAPED_SLASHES),
        CURLOPT_RETURNTRANSFER => true,
        CURLOPT_CONNECTTIMEOUT => 5,
        CURLOPT_TIMEOUT        => 20,
    ]);
    $body   = curl_exec($ch);
    $err    = curl_error($ch);
    $status = curl_getinfo($ch, CURLINFO_HTTP_CODE);
    curl_close($ch);

    return [
        'ok'     => (trim((string)$body) === 'TRUE'),
        'status' => $status,
        'body'   => $body,
        'error'  => $err ?: null,
        'sent'   => $payload,
    ];
}

/** Main: Push a single event (News.Key) to Tracker */
function runTrackerPushForKey(string $eventNewsKey): array {
    global $newsDBPath, $databasePath;
    dbg("=== Starting push for EventNewsKey: $eventNewsKey ===");

    $pdoNews  = new PDO("sqlite:$newsDBPath");
    $pdoNews->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);

    $pdoTasks = new PDO("sqlite:$databasePath");
    $pdoTasks->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);

    // Step 1: Retrieve event
    dbg("Fetching event from News DB...");
    $stmt = $pdoNews->prepare("
        SELECT 
            N.Key               AS NewsKey,
            N.EntrySeqID        AS NewsEntrySeqID,
            N.Title             AS NewsTitle,
            N.URLToGo           AS NewsURLToGo,
            E.EventKey          AS EventKey,
            E.EventDescription  AS EventDescription
        FROM News N
        LEFT JOIN Events E ON N.Key = E.EventKey
        WHERE N.Key = :key
        LIMIT 1
    ");
    $stmt->execute([':key' => $eventNewsKey]);
    $event = $stmt->fetch(PDO::FETCH_ASSOC);
    dbg("Event record: " . json_encode($event));

    if (!$event) {
        logMessage("Event not found for key '$eventNewsKey'.");
        throw new RuntimeException("Event not found for key '$eventNewsKey'.");
    }

    // Step 2: Parse EventNewsID
    $eventKey = (string)$event['EventKey'];
    $eventNewsId = extractEventNewsId($eventKey);
    dbg("Parsed EventNewsID: $eventNewsId from EventKey: $eventKey");

    if ($eventNewsId === null) {
        logMessage("Could not parse EventNewsID from EventKey '$eventKey'.");
        throw new RuntimeException("Could not parse EventNewsID from EventKey '$eventKey'.");
    }

    // Step 3: Fetch task
    dbg("Fetching task from Tasks DB for EntrySeqID: {$event['NewsEntrySeqID']}...");
    $stmtTask = $pdoTasks->prepare("
        SELECT Title, PLNXML, WPRXML
        FROM Tasks
        WHERE EntrySeqID = :tid
        LIMIT 1
    ");
    $stmtTask->execute([':tid' => $event['NewsEntrySeqID']]);
    $task = $stmtTask->fetch(PDO::FETCH_ASSOC);
    dbg("Task record: " . json_encode($task));

    if (!$task) {
        logMessage("Task not found for EntrySeqID '{$event['NewsEntrySeqID']}'.");
        throw new RuntimeException("Task not found for EntrySeqID '{$event['NewsEntrySeqID']}'.");
    }

    $taskTitle = trim((string)$task['Title']);
    $pln       = (string)$task['PLNXML'];
    if ($taskTitle === '' || $pln === '') {
        logMessage("Missing Title or PLNXML for EntrySeqID '{$event['NewsEntrySeqID']}'.");
        throw new RuntimeException("Missing Title or PLNXML for EntrySeqID '{$event['NewsEntrySeqID']}'.");
    }

    // Step 4: Fetch club info
    dbg("Fetching club info for EventNewsID: $eventNewsId...");
    $stmtClub = $pdoTasks->prepare("
        SELECT TrackerGroup, TrackerSecret
        FROM Clubs
        WHERE EventNewsID = :eid
        LIMIT 1
    ");
    $stmtClub->execute([':eid' => $eventNewsId]);
    $club = $stmtClub->fetch(PDO::FETCH_ASSOC);
    dbg("Club record: " . json_encode($club));

    if (!$club) {
        logMessage("No Clubs entry found for EventNewsID '$eventNewsId'.");
        throw new RuntimeException("No Clubs entry found for EventNewsID '$eventNewsId'.");
    }

    $group  = trim((string)$club['TrackerGroup']);
    $secret = trim((string)$club['TrackerSecret']);
    if ($group === '' || $secret === '') {
        logMessage("Invalid TrackerGroup or TrackerSecret for '$eventNewsId'.");
        throw new RuntimeException("Invalid TrackerGroup or TrackerSecret for '$eventNewsId'.");
    }

    // Build parsed task + JSON (C# parity)
    $parsedTask   = new Task($pln, $group);  // Task parsedTask = new Task(newTask.TASKDATA, this.GroupName);
    $taskDataJson = taskToJson($parsedTask); // newTask.TASKDATAJSON = this.TaskToJson(parsedTask);

    // Step 5: Weather
    $weatherName = null;
    $wpr         = (string)$task['WPRXML'];
    if (!empty($task['WPRXML'])) {
        try {
            $xml = new SimpleXMLElement($task['WPRXML']);
            $name = (string)($xml->{"WeatherPreset.Preset"}->Name ?? '');
            $weatherName = $name !== '' ? $name : null;
        } catch (Throwable $e) {
            logMessage("WPR parse failed for NewsKey {$event['NewsKey']}: " . $e->getMessage());
        }
    }

    // Step 6: Task info
    $taskInfo = trim((string)$event['NewsURLToGo']);
    if ($taskInfo === '') $taskInfo = $taskTitle;
    dbg("Built taskInfo: $taskInfo");

    // Step 7: Send to Tracker
    dbg("Sending task '$taskTitle' for group '$group'...");
    $push = trackerSetSharedTask([
        'groupName'       => $group,
        'plaintextSecret' => $secret,
        'taskTitle'       => $taskTitle,
        'taskData'        => $pln,
        'taskDataJson'    => $taskDataJson,
        'taskInfo'        => $taskInfo,
        'weather'         => $weatherName,
        'weatherData'     => $wpr,
    ]);

    $msg = $push['ok']
        ? "Tracker push OK for NewsKey {$event['NewsKey']} (group=$group, taskTitle=\"$taskTitle\")"
        : "Tracker push FAIL for NewsKey {$event['NewsKey']} (group=$group, taskTitle=\"$taskTitle\") "
          . "[HTTP {$push['status']}] {$push['body']} {$push['error']}";
    logMessage($msg);

    dbg("=== End push for $eventNewsKey ===");

    return array_merge([
        'newsKey'    => $event['NewsKey'],
        'eventKey'   => $eventKey,
        'eventNewsId'=> $eventNewsId,
        'group'      => $group,
        'taskTitle'  => $taskTitle,
        'weather'    => $weatherName,
    ], $push);
}

// ────────────────────────────────
// HTTP entrypoint (only when executed directly)
// ────────────────────────────────
if (basename(__FILE__) === basename($_SERVER['SCRIPT_FILENAME'])) {
    try {
        $key = isset($_GET['key']) ? trim((string)$_GET['key']) : '';
        if ($key === '') {
            http_response_code(400);
            echo json_encode(['status' => 'error', 'message' => 'Missing required EventNewsKey (?key=...)']);
            exit;
        }

        $res = runTrackerPushForKey($key);
        header('Content-Type: application/json');
        echo json_encode(['status' => 'success', 'result' => $res], JSON_UNESCAPED_SLASHES);

    } catch (Throwable $e) {
        logMessage('[Tracker Push ERROR] ' . $e->getMessage());
        header('Content-Type: application/json', true, 500);
        echo json_encode(['status' => 'error', 'message' => $e->getMessage()]);
    }
}
