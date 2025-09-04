<?php
require __DIR__ . '/CommonFunctions.php';

try {
    // Require parameters
    if (!isset($_GET['WSGUserID'])) {
        throw new Exception('WSGUserID is missing.');
    }
    if (!isset($_GET['EntrySeqID'])) {
        throw new Exception('EntrySeqID is missing.');
    }

    $wsgUserId = (int)$_GET['WSGUserID'];
    $entrySeqId = (int)$_GET['EntrySeqID'];

    // Open DB
    $pdo = new PDO("sqlite:$databasePath");
    $pdo->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);

    // Query UsersTasks (PRIMARY KEY: WSGUserID, EntrySeqID)
    $sql = "
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
        WHERE WSGUserID = :uid AND EntrySeqID = :eid
        LIMIT 1
    ";
    $stmt = $pdo->prepare($sql);
    $stmt->execute([':uid' => $wsgUserId, ':eid' => $entrySeqId]);
    $row = $stmt->fetch(PDO::FETCH_ASSOC);

    if ($row) {
        echo json_encode(['status' => 'success', 'usersTask' => $row]);
    } else {
        echo json_encode(['status' => 'error', 'message' => 'Entry not found.']);
    }

} catch (PDOException $e) {
    logMessage("Connection failed: " . $e->getMessage());
    echo json_encode(['status' => 'error', 'message' => 'Connection failed: ' . $e->getMessage()]);
    logMessage("--- End of script FetchUsersTask ---");
} catch (Exception $e) {
    logMessage("Error: " . $e->getMessage());
    echo json_encode(['status' => 'error', 'message' => $e->getMessage()]);
    logMessage("--- End of script FetchUsersTask ---");
}
