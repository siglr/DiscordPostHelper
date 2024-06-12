<?php
require __DIR__ . '/CommonFunctions.php';

try {
    // Log the script start
    logMessage("--- Script running permission check ---");

    // Ensure the request method is POST
    if ($_SERVER['REQUEST_METHOD'] !== 'POST') {
        throw new Exception('Invalid request method.');
    }

    // Check if user_id and right are set
    if (!isset($_POST['user_id']) || !isset($_POST['right'])) {
        throw new Exception('User ID or right missing.');
    }

    // Get the user ID and right
    $userID = $_POST['user_id'];
    $right = $_POST['right'];

    // Check user permission
    $hasRight = checkUserPermission($userID, $right);

    // Return the result
    echo json_encode(['status' => 'success', 'hasRight' => $hasRight]);

    // Log the script end
    logMessage("--- End of script permission check ---");

} catch (Exception $e) {
    logMessage("Error: " . $e->getMessage());
    echo json_encode(['status' => 'error', 'message' => $e->getMessage()]);

    logMessage("--- End of script permission check ---");
}
?>
