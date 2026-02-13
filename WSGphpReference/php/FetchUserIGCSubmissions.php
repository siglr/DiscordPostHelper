<?php
// Use the dedicated session restoration file instead of calling session_start() directly.
require_once __DIR__ . '/session_restore.php';
require_once __DIR__ . '/CommonFunctions.php';

function formatLaunchUtc(?string $value): ?string
{
    if (!empty($value) && strlen($value) === 12) {
        $raw = $value;  // e.g. "241113003550"

        $yy = (int) substr($raw, 0, 2);
        $mm = (int) substr($raw, 2, 2);
        $dd = (int) substr($raw, 4, 2);
        $HH = (int) substr($raw, 6, 2);
        $mi = (int) substr($raw, 8, 2);

        $fullYear = $yy + 2000;

        return sprintf("%04d-%02d-%02d %02d:%02d", $fullYear, $mm, $dd, $HH, $mi);
    }

    return $value !== null ? (string) $value : null;
}

function formatDurationToHms($seconds): ?string
{
    if ($seconds === null) {
        return null;
    }

    if (!is_numeric($seconds)) {
        return null;
    }

    $secs = (int) $seconds;
    $h = floor($secs / 3600);
    $m = floor(($secs % 3600) / 60);
    $s = $secs % 60;

    return sprintf("%02d:%02d:%02d", $h, $m, $s);
}

// Ensure the user is logged in; if not, return an error response.
if (!isset($_SESSION['user']) || !isset($_SESSION['user']['id'])) {
    http_response_code(401);
    echo json_encode(["error" => "User not authenticated"]);
    exit;
}

// Use the user id stored in the session.
$wsgUserID = $_SESSION['user']['id'];

// Validate that the user ID is a valid positive integer.
if ($wsgUserID <= 0) {
    http_response_code(400);
    error_log("Invalid WSGUserID: $wsgUserID");
    echo json_encode(["error" => "Invalid user ID"]);
    exit;
}

try {
    // Open the database connection using the path from CommonFunctions.php.
    $pdo = new PDO("sqlite:$databasePath");
    $pdo->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);

    // Retrieve IGC records for the logged in user, ordered by upload date descending,
    // including the new result fields.
    $query = "
          SELECT
            R.IGCKey,
            R.EntrySeqID,
            R.IGCUploadDateTimeUTC,
            R.IGCRecordDateTimeUTC,
            R.Pilot,
            R.GliderType,
            R.GliderID,
            R.CompetitionID,
            R.CompetitionClass,
            R.NB21Version,
            R.Sim,
            R.WSGUserID,
            R.Comment,
            R.TaskCompleted,
            R.Penalties,
            R.Duration,
            R.Distance,
            R.Speed,
            R.IGCValid,
            R.TPVersion,
            R.IsPrivate,
            R.LocalDate,
            R.LocalTime,
            T.SimDateTime,
            O.OriginalLocalTime          AS OV_OriginalLocalTime,
            O.OriginalLocalDate          AS OV_OriginalLocalDate,
            O.OriginalIGCRecordDateTimeUTC AS OV_OriginalIGCRecordDateTimeUTC,
            O.OriginalTaskCompleted      AS OV_OriginalTaskCompleted,
            O.OriginalPenalties          AS OV_OriginalPenalties,
            O.OriginalDuration           AS OV_OriginalDuration,
            O.OriginalDistance           AS OV_OriginalDistance,
            O.OriginalSpeed              AS OV_OriginalSpeed,
            O.OriginalIGCValid           AS OV_OriginalIGCValid,
            O.OriginalClubEventNewsID    AS OV_OriginalClubEventNewsID,
            O.Reason                     AS OV_Reason,
            O.UpdatedBy                  AS OV_UpdatedBy,
            O.OverriddenOn               AS OV_OverriddenOn
          FROM IGCRecords R
          LEFT JOIN Tasks T
            ON R.EntrySeqID = T.EntrySeqID
          LEFT JOIN IGCOverrides O
            ON O.IGCKey = R.IGCKey AND O.EntrySeqID = R.EntrySeqID
          WHERE R.WSGUserID = :wsgUserID
          ORDER BY R.IGCUploadDateTimeUTC DESC
        ";
    $stmt = $pdo->prepare($query);
    $stmt->bindParam(':wsgUserID', $wsgUserID, PDO::PARAM_STR);
    $stmt->execute();
    $records = $stmt->fetchAll(PDO::FETCH_ASSOC);

    // Process each record.
    foreach ($records as &$record) {
        $record['IGCRecordDateTimeUTC'] = formatLaunchUtc($record['IGCRecordDateTimeUTC']);

        // Transform the "Sim" field so that it only returns the year, prefixed by "MS".
        if (!empty($record['Sim'])) {
            $record['Sim'] = 'MS' . substr($record['Sim'], -4);
        }

        // Flags → booleans
        $record['TaskCompleted'] = (bool)$record['TaskCompleted'];
        $record['Penalties']     = (bool)$record['Penalties'];
        $record['IGCValid']      = (bool)$record['IGCValid'];
        $record['IsPrivate']     = (int)($record['IsPrivate'] ?? 0) === 1;

        // Duration (seconds) → "HH:MM:SS" or null
        $record['Duration'] = formatDurationToHms($record['Duration']);

        // Distance & Speed → floats or null
        $record['Distance'] = $record['Distance'] !== null ? (float)$record['Distance'] : null;
        $record['Speed']    = $record['Speed']    !== null ? (float)$record['Speed']    : null;

        // === Compute LocalDateTimeMatch flag ===
        if (
            !empty($record['LocalDate']) &&
            !empty($record['LocalTime']) &&
            !empty($record['SimDateTime'])
        ) {
            // 1) Task’s simulation DateTime
            $taskDT = DateTime::createFromFormat(
                'Y-m-d H:i:s',
                $record['SimDateTime'],
                new DateTimeZone('UTC')
            );
            $taskYear = $taskDT->format('Y');

            // 2) Build a DateTime from the IGC’s LocalDate + LocalTime, but force the task’s year
            //    LocalDate: YYYY-MM-DD, LocalTime: HHMMSS
            $md = substr($record['LocalDate'], 5);      // MM-DD
            $lh = substr($record['LocalTime'],  0, 2);  // HH
            $lm = substr($record['LocalTime'],  2, 2);  // MM
            $ls = substr($record['LocalTime'],  4, 2);  // SS
            $recDT = DateTime::createFromFormat(
                'Y-m-d H:i:s',
                sprintf('%s-%s %s:%s:%s', $taskYear, $md, $lh, $lm, $ls),
                new DateTimeZone('UTC')
            );

            // 3) Compare, allow ± 30 minutes (1,800 seconds)
            if ($taskDT && $recDT) {
                $diffSec = abs($taskDT->getTimestamp() - $recDT->getTimestamp());
                $record['LocalDateTimeMatch'] = ($diffSec <= 30 * 60);
            } else {
                $record['LocalDateTimeMatch'] = false;
            }
        } else {
            $record['LocalDateTimeMatch'] = false;
        }
        // === end LocalDateTimeMatch ===

        // Override metadata (if present)
        $hasOverride = (
            $record['OV_Reason'] !== null
            || $record['OV_OverriddenOn'] !== null
            || $record['OV_OriginalLocalTime'] !== null
        );

        $record['OverrideExists'] = $hasOverride;
        if ($hasOverride) {
            $record['Override'] = [
                'exists' => true,
                'OriginalLocalTime' => $record['OV_OriginalLocalTime'],
                'OriginalLocalDate' => $record['OV_OriginalLocalDate'],
                'OriginalIGCRecordDateTimeUTC' => formatLaunchUtc($record['OV_OriginalIGCRecordDateTimeUTC']),
                'OriginalIGCRecordDateTimeUTCRaw' => $record['OV_OriginalIGCRecordDateTimeUTC'],
                'OriginalTaskCompleted' => (bool)$record['OV_OriginalTaskCompleted'],
                'OriginalPenalties' => (bool)$record['OV_OriginalPenalties'],
                'OriginalDuration' => formatDurationToHms($record['OV_OriginalDuration']),
                'OriginalDurationSeconds' => $record['OV_OriginalDuration'] !== null
                    ? (int)$record['OV_OriginalDuration']
                    : null,
                'OriginalDistance' => $record['OV_OriginalDistance'] !== null
                    ? (float)$record['OV_OriginalDistance']
                    : null,
                'OriginalSpeed' => $record['OV_OriginalSpeed'] !== null
                    ? (float)$record['OV_OriginalSpeed']
                    : null,
                'OriginalIGCValid' => (bool)$record['OV_OriginalIGCValid'],
                'OriginalClubEventNewsID' => $record['OV_OriginalClubEventNewsID'],
                'Reason' => $record['OV_Reason'],
                'UpdatedBy' => $record['OV_UpdatedBy'],
                'OverriddenOn' => $record['OV_OverriddenOn'],
            ];
        } else {
            $record['Override'] = ['exists' => false];
        }

        unset(
            $record['OV_OriginalLocalTime'],
            $record['OV_OriginalLocalDate'],
            $record['OV_OriginalIGCRecordDateTimeUTC'],
            $record['OV_OriginalTaskCompleted'],
            $record['OV_OriginalPenalties'],
            $record['OV_OriginalDuration'],
            $record['OV_OriginalDistance'],
            $record['OV_OriginalSpeed'],
            $record['OV_OriginalIGCValid'],
            $record['OV_OriginalClubEventNewsID'],
            $record['OV_Reason'],
            $record['OV_UpdatedBy'],
            $record['OV_OverriddenOn']
        );
    }
    unset($record); // Good practice when iterating by reference.

    // Output the records as JSON.
    header('Content-Type: application/json');
    echo json_encode($records);
} catch (Exception $e) {
    http_response_code(500);
    echo json_encode(["error" => $e->getMessage()]);
}
?>