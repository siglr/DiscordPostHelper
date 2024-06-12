<?php
require __DIR__ . '/CommonFunctions.php';

try {
    // Open the database connection
    $pdo = new PDO("sqlite:$databasePath");
    $pdo->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);

    // Retrieve deleted tasks
    $stmt = $pdo->query("SELECT EntrySeqID FROM DeletedTasks");
    $deletedTasks = $stmt->fetchAll(PDO::FETCH_ASSOC);

    echo json_encode(['status' => 'success', 'deletedTasks' => $deletedTasks]);

} catch (PDOException $e) {
    // Log the script start
    logMessage("--- Script running RetrieveDeletedTasks ---");
    echo json_encode(['status' => 'error', 'message' => 'Connection failed: ' . $e->getMessage()]);
    logMessage("--- End of script RetrieveDeletedTasks ---");

}
?>
