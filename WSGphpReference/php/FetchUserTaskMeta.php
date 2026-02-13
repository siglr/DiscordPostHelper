<?php
require_once __DIR__ . '/session_restore.php';
require_once __DIR__ . '/CommonFunctions.php';

if (!isset($_SESSION['user']['id'])) {
    http_response_code(401);
    echo json_encode(['error' => 'User not authenticated']); exit;
}
$wsgUserID = (int) $_SESSION['user']['id'];
if ($wsgUserID <= 0) { http_response_code(400); echo json_encode(['error'=>'Invalid user ID']); exit; }

try {
    $pdo = new PDO("sqlite:$databasePath");
    $pdo->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);

    // UsersTasks = per-user task meta
    $sql = "
      SELECT
        U.EntrySeqID,
        T.Title,
        U.PrivateNotes,
        U.PublicFeedback,
        U.DifficultyRating,
        U.QualityRating,
        U.MarkedFlownDateUTC,
        U.MarkedFlyNextUTC,
        U.MarkedFavoritesUTC
      FROM UsersTasks U
      JOIN Tasks T ON T.EntrySeqID = U.EntrySeqID
      WHERE U.WSGUserID = :uid
      ORDER BY U.EntrySeqID DESC
    ";
    $st = $pdo->prepare($sql);
    $st->bindValue(':uid', $wsgUserID, PDO::PARAM_INT);
    $st->execute();
    $rows = $st->fetchAll(PDO::FETCH_ASSOC);

    header('Content-Type: application/json');
    echo json_encode($rows);
} catch (Exception $e) {
    http_response_code(500);
    echo json_encode(['error' => $e->getMessage()]);
}
