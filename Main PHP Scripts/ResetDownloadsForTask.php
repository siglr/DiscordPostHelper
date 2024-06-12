<?php
require __DIR__ . '/CommonFunctions.php';

try {
    // Open the database connection
    $pdo = new PDO("sqlite:$databasePath");
    $pdo->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);

    // Retrieve the EntrySeqID parameter from the query string
    if (isset($_GET['EntrySeqID']) && !empty($_GET['EntrySeqID'])) {
        $entrySeqID = $_GET['EntrySeqID'];
        $now = date('Y-m-d H:i:s');

        // Define the query to reset the TotDownloads to 0 and update LastDownloadUpdate
        $query = "
            UPDATE Tasks 
            SET 
                TotDownloads = 0, 
                LastDownloadUpdate = :lastDownloadUpdate 
            WHERE 
                EntrySeqID = :entrySeqID
        ";

        // Prepare and execute the query
        $stmt = $pdo->prepare($query);
        $stmt->bindParam(':lastDownloadUpdate', $now, PDO::PARAM_STR);
        $stmt->bindParam(':entrySeqID', $entrySeqID, PDO::PARAM_INT);
        $stmt->execute();

        // Check if any row was updated
        if ($stmt->rowCount() > 0) {
            echo json_encode(['status' => 'success', 'message' => 'TotDownloads reset to 0 and LastDownloadUpdate updated successfully']);
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
