<?php
require __DIR__ . '/CommonFunctions.php';

try {
    // Ensure the request method is POST
    if ($_SERVER['REQUEST_METHOD'] !== 'POST') {
        throw new Exception('Invalid request method.');
    }

    // Check if user_id is set
    if (!isset($_POST['user_id'])) {
        throw new Exception('User ID missing.');
    }

    // Get the user ID
    $userID = $_POST['user_id'];

    // Get user permissions
    $userPermissions = getUserPermissions($userID);
    $userRights = getUserRights($userPermissions);

    if ($userRights === null) {
        throw new Exception('User not found.');
    }

    // Return the user rights
    echo json_encode(['status' => 'success', 'rights' => $userRights]);
} catch (Exception $e) {
    echo json_encode(['status' => 'error', 'message' => $e->getMessage()]);
}
?>
