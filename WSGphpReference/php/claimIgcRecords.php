<?php
// php/claimIgcRecords.php
require_once __DIR__ . '/session_restore.php';
require_once __DIR__ . '/CommonFunctions.php';

header('Content-Type: application/json');

// 1) Auth check
if (empty($_SESSION['user']['id'])) {
    http_response_code(401);
    echo json_encode(array('success'=>false,'message'=>'User not authenticated'));
    exit;
}
$wsgUserID = (int) $_SESSION['user']['id'];
if ($wsgUserID <= 0) {
    http_response_code(400);
    echo json_encode(array('success'=>false,'message'=>'Invalid user ID'));
    exit;
}

// 2) Decode JSON body
$raw  = file_get_contents('php://input');
$data = json_decode($raw, true);
if (!isset($data['igcKeys']) || !is_array($data['igcKeys'])) {
    http_response_code(400);
    echo json_encode(array('success'=>false,'message'=>'Invalid request format'));
    exit;
}
$igcKeys = array_filter(array_map('trim', $data['igcKeys']), 'strlen');
if (count($igcKeys) === 0) {
    http_response_code(400);
    echo json_encode(array('success'=>false,'message'=>'No IGC keys provided'));
    exit;
}

try {
    $pdo = new PDO('sqlite:' . $databasePath);
    $pdo->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);

    // Build placeholders
    $placeholders = implode(',', array_fill(0, count($igcKeys), '?'));

    // 3) Prepare update statement
    $sql = "
        UPDATE IGCRecords
           SET WSGUserID = ?
         WHERE IGCKey    IN ($placeholders)
           AND WSGUserID = 0
    ";
    $stmt = $pdo->prepare($sql);

    // 4) Bind parameters: first the user ID, then each key
    $params = array_merge(array($wsgUserID), $igcKeys);

    $stmt->execute($params);
    $updatedCount = $stmt->rowCount();

    echo json_encode(array(
        'success'     => true,
        'updatedCount'=> $updatedCount
    ));
}
catch (Exception $e) {
    logMessage('claimIgcRecords error: ' . $e->getMessage());
    http_response_code(500);
    echo json_encode(array('success'=>false,'message'=>'Database error'));
}
