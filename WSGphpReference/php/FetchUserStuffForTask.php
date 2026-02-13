<?php
require_once __DIR__ . '/session_restore.php';
require_once __DIR__ . '/CommonFunctions.php';

// Ensure the user is logged in; if not, return an error response.
if (!isset($_SESSION['user']) || !isset($_SESSION['user']['id'])) {
    http_response_code(401);
    echo json_encode(["error" => "User not authenticated"]);
    exit;
}

$wsgUserID = $_SESSION['user']['id'];

// Validate that the user ID is a valid positive integer.
if ($wsgUserID <= 0) {
    http_response_code(400);
    error_log("Invalid WSGUserID: $wsgUserID");
    echo json_encode(["error" => "Invalid user ID"]);
    exit;
}

// Check that the required parameter (EntrySeqID) is provided.
if (!isset($_GET['entrySeqID'])) {
    http_response_code(400);
    echo json_encode(["error" => "Missing required parameter: entrySeqID"]);
    exit;
}

$entrySeqID = (int) $_GET['entrySeqID'];

try {
    // Open the database connection.
    $pdo = new PDO("sqlite:$databasePath");
    $pdo->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);

    // Retrieve the UsersTasks record (if any) for the current user and task.
    $usersTasksQuery = "
        SELECT 
            WSGUserID,
            EntrySeqID,
            PrivateNotes,
            Tags,
            PublicFeedback,
            DifficultyRating,
            QualityRating,
            MarkedFlownDateUTC,
            MarkedFlyNextUTC,
            MarkedFavoritesUTC
        FROM UsersTasks
        WHERE WSGUserID = :wsgUserID AND EntrySeqID = :entrySeqID
    ";
    $stmtUserTask = $pdo->prepare($usersTasksQuery);
    $stmtUserTask->bindParam(':wsgUserID', $wsgUserID, PDO::PARAM_INT);
    $stmtUserTask->bindParam(':entrySeqID', $entrySeqID, PDO::PARAM_INT);
    $stmtUserTask->execute();
    $userTask = $stmtUserTask->fetch(PDO::FETCH_ASSOC);

    // Retrieve IGCRecords for the current user and task.
    $igcQuery = "
        SELECT 
            IGCKey,
            EntrySeqID,
            IGCRecordDateTimeUTC,
            IGCUploadDateTimeUTC,
            LocalTime,
            BeginTimeUTC,
            Pilot,
            GliderType,
            GliderID,
            CompetitionID,
            CompetitionClass,
            NB21Version,
            Sim,
            WSGUserID,
            Comment
        FROM IGCRecords
        WHERE EntrySeqID = :entrySeqID AND WSGUserID = :wsgUserID
    ";
    $stmtIgc = $pdo->prepare($igcQuery);
    $stmtIgc->bindParam(':entrySeqID', $entrySeqID, PDO::PARAM_INT);
    $stmtIgc->bindParam(':wsgUserID', $wsgUserID, PDO::PARAM_INT);
    $stmtIgc->execute();
    $igcRecords = $stmtIgc->fetchAll(PDO::FETCH_ASSOC);

    // Process each IGC record for display purposes.
    foreach ($igcRecords as &$record) {
        // If IGCRecordDateTimeUTC is exactly 12 characters (YYMMDDHHMMSS), format it.
        if (!empty($record['IGCRecordDateTimeUTC']) && strlen($record['IGCRecordDateTimeUTC']) === 12) {
            $raw = $record['IGCRecordDateTimeUTC'];  // Example: "241113003550"
            
            // Extract components from the string.
            $yy = (int) substr($raw, 0, 2);
            $mm = (int) substr($raw, 2, 2);
            $dd = (int) substr($raw, 4, 2);
            $HH = (int) substr($raw, 6, 2);
            $mi = (int) substr($raw, 8, 2);
            // Optionally, extract seconds if needed.
            // $ss = (int) substr($raw, 10, 2);

            // Convert the two-digit year to a full year.
            $fullYear = $yy + 2000;

            // Format the date/time string (ignoring seconds for display).
            $record['IGCRecordDateTimeUTC'] = sprintf(
                "%04d-%02d-%02d %02d:%02d",
                $fullYear, $mm, $dd, $HH, $mi
            );
        }
        
        // Transform the "Sim" field to only return the year, prefixed with "MS".
        if (!empty($record['Sim'])) {
            $record['Sim'] = 'MS' . substr($record['Sim'], -4);
        }
    }
    unset($record);

    // If there was no UsersTasks row, fetch() returned false — force it to an empty object
    if ($userTask === false) {
        $userTask = (object)[];
    }

    // Build the JSON response containing both UsersTasks and IGCRecords data.
    $response = [
        'userTask'   => $userTask,   // May be null if no record is found.
        'igcRecords' => $igcRecords
    ];

    header('Content-Type: application/json');
    echo json_encode($response);
} catch (Exception $e) {
    http_response_code(500);
    echo json_encode(["error" => $e->getMessage()]);
}
?>
