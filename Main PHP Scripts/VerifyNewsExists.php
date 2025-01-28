<?php
require __DIR__ . '/CommonFunctions.php';

try {
    // Open the database connection
    $pdo = new PDO("sqlite:$newsDBPath");
    $pdo->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);

    // Ensure the request method is GET
    if ($_SERVER['REQUEST_METHOD'] !== 'GET') {
        throw new Exception('Invalid request method.');
    }

    // Check if the key parameter is set
    if (!isset($_GET['key']) || empty($_GET['key'])) {
        throw new Exception('Key parameter is missing.');
    }

    $key = $_GET['key'];

    // Query to check if the key exists in the News table
    $stmt = $pdo->prepare("SELECT COUNT(*) FROM News WHERE Key = :key");
    $stmt->execute([':key' => $key]);
    $count = $stmt->fetchColumn();

    if ($count > 0) {
        echo json_encode(['status' => 'success']);
    } else {
        echo json_encode(['status' => 'notfound']);
    }
} catch (Exception $e) {
    // Log error and return as JSON
    logMessage("Error: " . $e->getMessage());
    echo json_encode(['status' => 'error', 'message' => $e->getMessage()]);
}
?>
