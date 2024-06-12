<?php
require __DIR__ . '/CommonFunctions.php';

try {
    // Open the database connection
    $pdo = new PDO("sqlite:$databasePath");
    $pdo->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);

    // Retrieve the EntrySeqID parameter from the query string
    if (isset($_GET['EntrySeqID']) && !empty($_GET['EntrySeqID'])) {
        $entrySeqID = $_GET['EntrySeqID'];

        // Define the query to reset the ThreadAccess to 0
        $query = "
            UPDATE Tasks 
            SET 
                ThreadAccess = 0
            WHERE 
                EntrySeqID = :entrySeqID
        ";

        // Prepare and execute the query
        $stmt = $pdo->prepare($query);
        $stmt->bindParam(':entrySeqID', $entrySeqID, PDO::PARAM_INT);
        $stmt->execute();

        // Check if any row was updated
        if ($stmt->rowCount() > 0) {
            echo json_encode(['status' => 'success', 'message' => 'ThreadAccess reset to 0 successfully']);
        } else {
            echo json_encode(['status' => 'error', 'message' => 'No task found with the provided EntrySeqID']);
        }
    } else {
        // Handle missing or empty EntrySeqID parameter
        echo json_encode([
            'status' => 'error',
            'message' => 'Missing or empty EntrySeqID parameter'
        ]);
    }

} catch (PDOException $e) {
    echo json_encode([
        'status' => 'error',
        'message' => 'Connection failed: ' . $e->getMessage()
    ]);
}
?>
