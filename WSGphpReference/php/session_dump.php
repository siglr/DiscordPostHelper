<?php
require __DIR__ . '/CommonFunctions.php';
require_once __DIR__ . '/session_restore.php';

header('Content-Type: application/json');

// Ensure the user is logged in; if not, return an error response.
if (!isset($_SESSION['user']) || !isset($_SESSION['user']['id'])) {
    http_response_code(401);
    echo json_encode(["error" => "User not authenticated"]);
    exit;
}

// Set the content-type header to JSON.
header('Content-Type: application/json');

// Output the current session data as JSON.
echo json_encode($_SESSION, JSON_PRETTY_PRINT);
?>
