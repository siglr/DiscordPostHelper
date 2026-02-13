<?php
require __DIR__ . '/CommonFunctions.php';

// Check required GET parameters.
if (!isset($_GET['IGCKey']) || !isset($_GET['EntrySeqID'])) {
    header('HTTP/1.1 400 Bad Request');
    echo "Error: Missing IGCKey or EntrySeqID.";
    exit;
}

$IGCKey = trim($_GET['IGCKey']);
$EntrySeqID = intval($_GET['EntrySeqID']);

// Determine the destination folder and filename.
$destFolder = rtrim($taskBrowserPath, '/\\') . '/IGCFiles/' . $EntrySeqID;
$filePath = $destFolder . '/' . $IGCKey . '.igc';

if (!file_exists($filePath)) {
    header('HTTP/1.1 404 Not Found');
    echo "Error: IGC file not found.";
    exit;
}

// Set header to return plain text.
header('Content-Type: text/plain');

// Output the contents of the IGC file.
readfile($filePath);
?>
