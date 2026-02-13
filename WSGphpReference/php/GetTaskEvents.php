<?php
require __DIR__ . '/CommonFunctions.php';

header('Content-Type: application/json');

try {
    // Open DB
    $pdo = new PDO("sqlite:$databasePath");
    $pdo->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);

    // GET only
    if ($_SERVER['REQUEST_METHOD'] !== 'GET') {
        throw new Exception('Invalid request method.');
    }

    // Required param
    if (!isset($_GET['eventNewsId']) || trim($_GET['eventNewsId']) === '') {
        throw new Exception('eventNewsId is required.');
    }
    $eventNewsId = trim($_GET['eventNewsId']);

    // Build query:
    // - Join TaskEvents (te) to Tasks (t) via EntrySeqID
    // - Match ClubEventNewsID (case-insensitive)
    // - Only published tasks (Status = 99)
    // - Exclude tasks with Availability in the future (UTC)
    // - Order by most recent events
    $sql = "
        SELECT
            te.EventDateTime,          -- UTC 'YYYY-MM-DD HH:MM:SS'
            te.ClubEventNewsID,
            te.EventURL,
            t.EntrySeqID,
            t.Title,
            t.Availability            -- for debugging/inspection if needed
        FROM TaskEvents AS te
        INNER JOIN Tasks AS t
            ON t.EntrySeqID = te.EntrySeqID
        WHERE
            UPPER(te.ClubEventNewsID) = UPPER(:eventNewsId)
            AND t.Status = 99
            AND (
                t.Availability IS NULL
                OR t.Availability = ''
                OR datetime(t.Availability) <= datetime('now','utc')
            )
        ORDER BY te.EventDateTime DESC
    ";

    $stmt = $pdo->prepare($sql);
    $stmt->execute([':eventNewsId' => $eventNewsId]);

    $rows = $stmt->fetchAll(PDO::FETCH_ASSOC) ?: [];

    foreach ($rows as &$row) {
        $full = isset($row['EventDateTime']) ? $row['EventDateTime'] : null;
        $dateOnly = '';
        if ($full !== null && $full !== '') {
            $dateOnly = substr($full, 0, 10);
        }
        $row['EventDateTimeWithTime'] = $full;
        $row['EventDateTime'] = $dateOnly;
    }
    unset($row);

    echo json_encode([
        'status' => 'success',
        'events' => $rows
    ]);

} catch (Exception $e) {
    logMessage("Error in GetTaskEvents.php: " . $e->getMessage());
    echo json_encode([
        'status'  => 'error',
        'message' => $e->getMessage()
    ]);
}
