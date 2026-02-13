<?php
require __DIR__ . '/CommonFunctions.php';

$taskID = $_GET['taskID'] ?? null;
$filename = $_GET['filename'] ?? null;

if (!$taskID || !$filename) {
    http_response_code(400);
    die("Missing parameters.");
}

try {
    // Retrieve and unpack the DPHX file using the common function
    $taskFolder = retrieveAndUnpackDPHX($taskID);

    // Serve the requested file
    $requestedFile = "$taskFolder/$filename";
    if (file_exists($requestedFile)) {
        $fileExtension = strtolower(pathinfo($requestedFile, PATHINFO_EXTENSION));
        $mimeTypes = [
            'jpg' => 'image/jpeg',
            'jpeg' => 'image/jpeg',
            'png' => 'image/png',
            'gif' => 'image/gif',
            'bmp' => 'image/bmp',
            'webp' => 'image/webp',
            'xml' => 'application/xml',
            'zip' => 'application/zip',
            'pdf' => 'application/pdf',
            'txt' => 'text/plain'
        ];

        $mimeType = $mimeTypes[$fileExtension] ?? 'application/octet-stream';

        header('Content-Description: File Transfer');
        header('Content-Type: ' . $mimeType);
        header('Content-Disposition: inline; filename="' . basename($requestedFile) . '"');
        header('Expires: 0');
        header('Cache-Control: must-revalidate');
        header('Pragma: public');
        header('Content-Length: ' . filesize($requestedFile));
        readfile($requestedFile);
        exit;
    } else {
        http_response_code(404);
        logMessage("Error: Requested file not found - $requestedFile.");
        die("Requested file not found.");
    }
} catch (Exception $e) {
    http_response_code(500);
    logMessage("Error: " . $e->getMessage());
    die($e->getMessage());
}
?>
