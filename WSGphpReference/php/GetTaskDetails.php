<?php
require __DIR__ . '/CommonFunctions.php';

function formatLaunchUtc(?string $value): ?string
{
    if (!empty($value) && strlen($value) === 12) {
        $yy = (int) substr($value, 0, 2);
        $mm = (int) substr($value, 2, 2);
        $dd = (int) substr($value, 4, 2);
        $HH = (int) substr($value, 6, 2);
        $mi = (int) substr($value, 8, 2);
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

    $durationSec = (int) $seconds;
    $hours = floor($durationSec / 3600);
    $minutes = floor(($durationSec % 3600) / 60);
    $seconds = $durationSec % 60;

    return sprintf("%02d:%02d:%02d", $hours, $minutes, $seconds);
}

try {
    // Check if EntrySeqID is provided
    if (!isset($_GET['entrySeqID'])) {
        throw new Exception('Missing required parameter: entrySeqID');
    }

    $entrySeqID = (int)$_GET['entrySeqID'];

    // Open the database connection
    $pdo = new PDO("sqlite:$databasePath");
    $pdo->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);

    // Define the query to retrieve the task details
    $query = "
        SELECT
            TaskID,
            EntrySeqID, 
            Title,
            ShortDescription,
            MainAreaPOI,
            DepartureName,
            DepartureICAO,
            DepartureExtra,
            ArrivalName,
            ArrivalICAO,
            ArrivalExtra,
            SimDateTime,
            SimDateTimeExtraInfo,
            IncludeYear,
            SoaringRidge,
            SoaringThermals,
            SoaringWaves,
            SoaringDynamic,
            SoaringExtraInfo,
            DurationMin,
            DurationMax,
            DurationExtraInfo,
            TaskDistance,
            TotalDistance,
            RecommendedGliders,
            RecommendedAddOnsList,
            DifficultyRating,
            DifficultyExtraInfo,
            LongDescription,
            WeatherSummary,
            Credits,
            Countries,
            PLNFilename,
            PLNXML,
            WPRFilename,
            WPRXML,
            WPRSecondaryFilename,
            WPRSecondaryName,
            RepostText,
            LastUpdate,
            LastUpdateDescription,
            TotDownloads,
            SuppressBaroPressureWarningSymbol,
            BaroPressureExtraInfo,
            ExtraFilesList,
            Status,
            DiscordPostID,
            Availability
        FROM Tasks
        WHERE EntrySeqID = :entrySeqID AND Status = 99
    ";

    // Prepare and execute the query
    $stmt = $pdo->prepare($query);
    $stmt->bindParam(':entrySeqID', $entrySeqID, PDO::PARAM_INT);
    $stmt->execute();

    // Fetch the task details
    $task = $stmt->fetch(PDO::FETCH_ASSOC);

    if ($task) {
        // Get the current UTC timestamp
        $nowUTC = (new DateTime('now', new DateTimeZone('UTC')))->getTimestamp();

        // Convert Availability to UTC timestamp (if set)
        $availabilityTimestamp = !empty($task['Availability'])
            ? DateTime::createFromFormat('Y-m-d H:i:s', $task['Availability'], new DateTimeZone('UTC'))->getTimestamp()
            : null;

        // Check if the task is unavailable due to the Availability date
        if ($availabilityTimestamp !== null && $availabilityTimestamp > $nowUTC) {
            header('Content-Type: application/json');
            echo json_encode([
                'status' => 'unavailable',
                'message' => 'Task is not available yet.',
                'availability' => $task['Availability']
            ]);
            exit;
        }

        // Format XML fields for nicer display.
        $task['PLNXML'] = prettyPrintXml($task['PLNXML']);
        $task['WPRXML'] = prettyPrintXml($task['WPRXML']);

        // Retrieve IGCRecords for this task (via EntrySeqID)
        $igcQuery = "SELECT
                        R.IGCKey,
                        R.EntrySeqID,
                        R.IGCRecordDateTimeUTC,
                        R.IGCUploadDateTimeUTC,
                        R.LocalDate,
                        R.LocalTime,
                        R.BeginTimeUTC,
                        R.Pilot,
                        R.GliderType,
                        R.GliderID,
                        R.CompetitionID,
                        R.CompetitionClass,
                        R.NB21Version,
                        R.Sim,
                        R.TaskCompleted,
                        R.Penalties,
                        R.Duration,
                        R.Distance,
                        R.Speed,
                        R.IGCValid,
                        R.MarkedAsDesigner,
                        R.ClubEventNewsID,
                        R.TPVersion,
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
                     LEFT JOIN IGCOverrides O
                        ON O.IGCKey = R.IGCKey AND O.EntrySeqID = R.EntrySeqID
                     WHERE R.EntrySeqID = :entrySeqID
                       AND COALESCE(R.IsPrivate, 0) = 0";
        $stmtIgc = $pdo->prepare($igcQuery);
        $stmtIgc->bindParam(':entrySeqID', $entrySeqID, PDO::PARAM_INT);
        $stmtIgc->execute();
        $igcRecords = $stmtIgc->fetchAll(PDO::FETCH_ASSOC);

        foreach ($igcRecords as &$record) {
            $record['IGCRecordDateTimeUTC'] = formatLaunchUtc($record['IGCRecordDateTimeUTC']);

            if (!empty($record['Sim'])) {
                $record['Sim'] = 'MS' . substr($record['Sim'], -4);
            }
    
            // TaskCompleted and Penalties: convert stored INTEGER (0/1) back to a boolean.
            $record['TaskCompleted'] = (bool)$record['TaskCompleted'];
            $record['Penalties'] = (bool)$record['Penalties'];
            $record['IGCValid'] = (bool)$record['IGCValid'];
            $record['MarkedAsDesigner'] = (int)($record['MarkedAsDesigner'] ?? 0) === 1;

            // Parse the task’s SimDateTime once
            if (!isset($taskDT)) {
                $taskDT = DateTime::createFromFormat(
                    'Y-m-d H:i:s',
                    $task['SimDateTime'],
                    new DateTimeZone('UTC')
                );
                $taskYear = $taskDT->format('Y');
            }

            // Build a DateTime for the IGC record, using the task’s year
            // LocalDate is "YYYY-MM-DD", LocalTime is "HHMMSS"
            if (!empty($record['LocalDate']) && !empty($record['LocalTime'])) {
                $md = substr($record['LocalDate'], 5);            // "MM-DD"
                $lh = substr($record['LocalTime'],  0, 2);        // "HH"
                $lm = substr($record['LocalTime'],  2, 2);        // "MM"
                $ls = substr($record['LocalTime'],  4, 2);        // "SS"

                $recDT = DateTime::createFromFormat(
                    'Y-m-d H:i:s',
                    sprintf('%s-%s %s:%s:%s', $taskYear, $md, $lh, $lm, $ls),
                    new DateTimeZone('UTC')
                );

                // Compare timestamps with a ±30 minute tolerance
                $diffSec = abs($taskDT->getTimestamp() - $recDT->getTimestamp());
                $record['LocalDateTimeMatch'] = ($diffSec <= 30 * 60);
            } else {
                $record['LocalDateTimeMatch'] = false;
            }
    
            // Duration: Convert from seconds (stored as an INTEGER) back to "HH:MM:SS" text.
            $record['Duration'] = formatDurationToHms($record['Duration']);
    
            // Distance and Speed: We leave them as float.
            // (Ensure they are numbers; no extra formatting is applied here.)
            $record['Distance'] = isset($record['Distance']) ? (float)$record['Distance'] : null;
            $record['Speed']    = isset($record['Speed']) ? (float)$record['Speed'] : null;

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
        unset($record); // Clean up the reference.

        // Attach the IGCRecords to the task details.
        $task['IGCRecords'] = $igcRecords;

        // Retrieve TaskEvents for this task (via EntrySeqID) ---
        $eventsQuery = "
            SELECT *
            FROM TaskEvents
            WHERE EntrySeqID = :entrySeqID
            ORDER BY EventDateTime DESC
        ";
        $stmtEvents = $pdo->prepare($eventsQuery);
        $stmtEvents->bindParam(':entrySeqID', $entrySeqID, PDO::PARAM_INT);
        $stmtEvents->execute();
        $taskEvents = $stmtEvents->fetchAll(PDO::FETCH_ASSOC);

        foreach ($taskEvents as &$event) {
            $full = isset($event['EventDateTime']) ? $event['EventDateTime'] : null;
            $dateOnly = '';
            if ($full !== null && $full !== '') {
                $dateOnly = substr($full, 0, 10);
            }
            $event['EventDateTimeWithTime'] = $full;
            $event['EventDateTime'] = $dateOnly;
        }
        unset($event);

        $task['TaskEvents'] = $taskEvents;
        // End retrieve TaskEvents

        // Output the task details as JSON.
        header('Content-Type: application/json');
        echo json_encode($task);
    } else {
        // If no task was found, return a clean message.
        header('Content-Type: application/json');
        echo json_encode(['status' => 'not_found', 'message' => 'Task not found']);
    }
} catch (Exception $e) {
    header('Content-Type: application/json');
    echo json_encode(['error' => $e->getMessage()]);
}
?>
