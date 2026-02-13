<?php
require __DIR__ . '/CommonFunctions.php';

try {
    $entrySeqID = filter_input(INPUT_GET, 'entrySeqID', FILTER_VALIDATE_INT);
    if ($entrySeqID === null || $entrySeqID === false) {
        throw new Exception('Missing or invalid parameter: entrySeqID');
    }

    $pdo = new PDO("sqlite:$databasePath");
    $pdo->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);

    $query = "
        SELECT
            EntrySeqID,
            TaskID,
            Title,
            Availability,
            WPRSecondaryFilename
        FROM Tasks
        WHERE EntrySeqID = :entrySeqID
          AND Status = 99
    ";

    $stmt = $pdo->prepare($query);
    $stmt->bindParam(':entrySeqID', $entrySeqID, PDO::PARAM_INT);
    $stmt->execute();

    $task = $stmt->fetch(PDO::FETCH_ASSOC);

    if (!$task) {
        header('Content-Type: application/json');
        echo json_encode(['status' => 'not_found', 'message' => 'Task not found']);
        exit;
    }

    $nowUTC = (new DateTime('now', new DateTimeZone('UTC')))->getTimestamp();
    $availabilityTimestamp = null;

    if (!empty($task['Availability'])) {
        $availabilityDate = DateTime::createFromFormat('Y-m-d H:i:s', $task['Availability'], new DateTimeZone('UTC'));
        if ($availabilityDate !== false) {
            $availabilityTimestamp = $availabilityDate->getTimestamp();
        }
    }

    if ($availabilityTimestamp !== null && $availabilityTimestamp > $nowUTC) {
        header('Content-Type: application/json');
        echo json_encode([
            'status' => 'unavailable',
            'message' => 'Task is not available yet.',
            'availability' => $task['Availability']
        ]);
        exit;
    }

    header('Content-Type: application/json');
    echo json_encode($task);
} catch (Exception $e) {
    header('Content-Type: application/json');
    echo json_encode(['error' => $e->getMessage()]);
}
?>
