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

    // Get user permissions for the specified user
    $userPermissions = getUserPermissions($userID);

    if ($userPermissions === null) {
        throw new Exception('User not found.');
    }

    // Extract user rights
    $userRights = getUserRights($userPermissions);

    // Extract the specified user's name
    $userName = (string)$userPermissions->Name;

    // Extract all user names from the XML
    $allUserNames = [];
    $xml = simplexml_load_file($userPermissionsPath);
    if ($xml !== false) {
        foreach ($xml->User as $user) {
            $allUserNames[] = (string)$user->Name;
        }
    } else {
        throw new Exception('Failed to load user permissions file.');
    }

    // Return the user rights, user's name, and all user names
    echo json_encode([
        'status' => 'success',
        'name' => $userName,
        'rights' => $userRights,
        'allUserNames' => $allUserNames
    ]);
} catch (Exception $e) {
    echo json_encode(['status' => 'error', 'message' => $e->getMessage()]);
}
