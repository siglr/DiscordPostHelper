<?php
require_once __DIR__ . '/session_restore.php';
require_once __DIR__ . '/CommonFunctions.php';
header('Content-Type: application/json');

// Auth
if (!isset($_SESSION['user']['id'])) { http_response_code(401); echo json_encode(['error'=>'User not authenticated']); exit; }
$wsgUserID = (int) $_SESSION['user']['id'];
if ($wsgUserID <= 0) { http_response_code(400); echo json_encode(['error'=>'Invalid user ID']); exit; }

// Input
$in = json_decode(file_get_contents('php://input'), true);
if (!$in || empty($in['EntrySeqID'])) { http_response_code(400); echo json_encode(['error'=>'Missing EntrySeqID']); exit; }
$eid = (string)$in['EntrySeqID'];

try {
    $pdo = new PDO("sqlite:$databasePath");
    $pdo->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);

    $st = $pdo->prepare("DELETE FROM UsersTasks WHERE WSGUserID = :uid AND EntrySeqID = :eid");
    $st->execute([':uid' => $wsgUserID, ':eid' => $eid]);

    echo json_encode(['status' => 'success']);
} catch (Exception $e) {
    http_response_code(500);
    echo json_encode(['error' => $e->getMessage()]);
}
