<?php
require_once __DIR__ . '/CommonFunctions.php';

header('Content-Type: application/json');

$taskID = $_GET['taskID'] ?? null;

if (!$taskID) {
    echo json_encode(['status' => 'error', 'message' => 'Missing task ID.']);
    exit;
}

try {
    // Retrieve and unpack the DPHX file
    $taskFolder = retrieveAndUnpackDPHX($taskID);

    // Path transformation based on root folder
    if (strpos($taskFolder, '/home3/siglr3/soaring.siglr.com/') === 0) {
        $taskFolder = str_replace('/home3/siglr3/soaring.siglr.com/', 'soaring.siglr.com/', $taskFolder);
    } elseif (strpos($taskFolder, '/home3/siglr3/wesimglide/') === 0) {
        $taskFolder = str_replace('/home3/siglr3/wesimglide/', 'wesimglide.org/', $taskFolder);
    }

    echo json_encode([
        'status' => 'success',
        'taskFolder' => $taskFolder
    ]);
} catch (Exception $e) {
    echo json_encode(['status' => 'error', 'message' => $e->getMessage()]);
}
?>
