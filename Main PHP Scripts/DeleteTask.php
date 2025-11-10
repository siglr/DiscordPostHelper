<?php
require __DIR__ . '/CommonFunctions.php';

try {
    logMessage("--- Script running DeleteTask ---");

    // Open the database connection
    $pdo = new PDO("sqlite:$databasePath");
    $pdo->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);
    logMessage("Database connection established.");

    // Ensure the request method is POST
    if ($_SERVER['REQUEST_METHOD'] !== 'POST') {
        throw new Exception('Invalid request method.');
    }

    // Check if EntrySeqID and UserID are set
    if (!isset($_POST['EntrySeqID']) || !isset($_POST['UserID'])) {
        throw new Exception('EntrySeqID or UserID is missing.');
    }

    $entrySeqID = $_POST['EntrySeqID'];
    $userID = $_POST['UserID'];

    // Check if the user has DeleteTask rights
    if (!checkUserPermission($userID, 'DeleteTask')) {
        throw new Exception('User does not have permission to delete tasks.');
    }

    // Ensure the task exists so we can queue its deletion.
    $stmt = $pdo->prepare("SELECT TaskID FROM Tasks WHERE EntrySeqID = :EntrySeqID");
    $stmt->execute([':EntrySeqID' => $entrySeqID]);
    $result = $stmt->fetch(PDO::FETCH_ASSOC);

    if (!$result) {
        throw new Exception('Task not found.');
    }

    // Get the current UTC time
    $now = new DateTime("now", new DateTimeZone("UTC"));
    $nowFormatted = $now->format('Y-m-d H:i:s');

    // Either update an existing pending deletion entry or create a new one.
    $checkStmt = $pdo->prepare("SELECT EntrySeqID FROM DeletedTasks WHERE EntrySeqID = :EntrySeqID AND Completed = 0 LIMIT 1");
    $checkStmt->execute([':EntrySeqID' => $entrySeqID]);

    if ($checkStmt->fetch(PDO::FETCH_ASSOC)) {
        $stmt = $pdo->prepare("UPDATE DeletedTasks SET DeletionDate = :DeletionDate, UserID = :UserID WHERE EntrySeqID = :EntrySeqID AND Completed = 0");
        $stmt->execute([
            ':DeletionDate' => $nowFormatted,
            ':UserID' => $userID,
            ':EntrySeqID' => $entrySeqID
        ]);
        logMessage("Updated pending deletion request for EntrySeqID: " . $entrySeqID . " by UserID: " . $userID);
    } else {
        $stmt = $pdo->prepare("INSERT INTO DeletedTasks (EntrySeqID, DeletionDate, UserID, Completed) VALUES (:EntrySeqID, :DeletionDate, :UserID, 0)");
        if (!$stmt->execute([
            ':EntrySeqID' => $entrySeqID,
            ':DeletionDate' => $nowFormatted,
            ':UserID' => $userID
        ])) {
            throw new Exception('Failed to insert deletion request.');
        }
        logMessage("Queued deletion request for EntrySeqID: " . $entrySeqID . " by UserID: " . $userID);
    }

    $output = [
        'status'  => 'success',
        'message' => 'Deletion request queued successfully.'
    ];

    echo json_encode($output);
    logMessage("--- End of script DeleteTask ---");

} catch (Exception $e) {
    logMessage("Error: " . $e->getMessage());
    echo json_encode(['status' => 'error', 'message' => $e->getMessage()]);
    logMessage("--- End of script DeleteTask ---");
}
?>
