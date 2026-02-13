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
if (!$in || !isset($in['EntrySeqID'])) { http_response_code(400); echo json_encode(['error'=>'Missing EntrySeqID']); exit; }
$eid = (string)$in['EntrySeqID'];

// Accept any subset of these fields
$allowed = [
  'PrivateNotes',
  'PublicFeedback',
  'DifficultyRating',
  'QualityRating',
  'MarkedFlownDateUTC',
  'MarkedFlyNextUTC',
  'MarkedFavoritesUTC'
];

// Build SET clause dynamically
$sets   = [];
$params = [':uid' => $wsgUserID, ':eid' => $eid];

foreach ($allowed as $k) {
    if (array_key_exists($k, $in)) {
        // Normalize empty strings to NULL for date/text fields
        $val = $in[$k];
        if ($val === '') $val = null;

        // Optional clamping for rating fields
        if ($k === 'DifficultyRating' || $k === 'QualityRating') {
            if ($val === null || $val === '') $val = null;
            else {
                $val = (int)$val;
                if ($val < 0) $val = 0;
                if ($val > 5) $val = 5;
            }
        }

        $sets[] = "$k = :$k";
        $params[":$k"] = $val;
    }
}

if (empty($sets)) { echo json_encode(['status' => 'noop']); exit; }

try {
    $pdo = new PDO("sqlite:$databasePath");
    $pdo->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);

    // Ensure row exists (UPSERT); SQLite-safe
    $ins = $pdo->prepare("INSERT OR IGNORE INTO UsersTasks (WSGUserID, EntrySeqID) VALUES (:uid, :eid)");
    $ins->execute([':uid' => $wsgUserID, ':eid' => $eid]);

    $sql = "UPDATE UsersTasks SET " . implode(', ', $sets) . " WHERE WSGUserID = :uid AND EntrySeqID = :eid";
    $st  = $pdo->prepare($sql);
    foreach ($params as $k => $v) $st->bindValue($k, $v);
    $st->execute();

    echo json_encode(['status' => 'success']);
} catch (Exception $e) {
    http_response_code(500);
    echo json_encode(['error' => $e->getMessage()]);
}
