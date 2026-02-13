<?php
require __DIR__ . '/CommonFunctions.php';

header('Content-Type: application/json');

try {
    // Read raw JSON input
    $jsonData = json_decode(file_get_contents('php://input'), true);

    // Validate required fields
    if (!isset($jsonData['IGCKey']) || trim($jsonData['IGCKey']) === "") {
        throw new Exception("Missing required field: IGCKey");
    }
    if (!isset($jsonData['Comment'])) {
        throw new Exception("Missing required field: Comment");
    }
    
    $IGCKey = trim($jsonData['IGCKey']);
    $newComment = trim($jsonData['Comment']);
    
    // Open the database connection.
    $pdo = new PDO("sqlite:$databasePath");
    $pdo->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);
    
    // Update the comment for the given IGCKey.
    $updateQuery = "UPDATE IGCRecords SET Comment = :comment WHERE IGCKey = :igcKey";
    $stmt = $pdo->prepare($updateQuery);
    $stmt->bindParam(':comment', $newComment, PDO::PARAM_STR);
    $stmt->bindParam(':igcKey', $IGCKey, PDO::PARAM_STR);
    $stmt->execute();
    
    echo json_encode([
        'status' => 'success',
        'message' => 'IGC comment updated successfully.',
        'IGCKey' => $IGCKey,
        'Comment' => $newComment
    ]);
    
} catch (Exception $e) {
    echo json_encode(['error' => $e->getMessage()]);
    exit;
}
?>
