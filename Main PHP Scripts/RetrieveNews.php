<?php
require __DIR__ . '/CommonFunctions.php';

try {

    //logMessage("--- Script running RetrieveNews ---");

    // Open the database connection
    $pdo = new PDO("sqlite:$newsDBPath");
    $pdo->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);

    // Call the cleanup function
    cleanUpNewsEntries($pdo);

    // Query to fetch all news entries
    $stmt = $pdo->query("SELECT * FROM News ORDER BY NewsType DESC, EventDate ASC, Published DESC;");
    $newsEntries = $stmt->fetchAll(PDO::FETCH_ASSOC);

    // Return the news entries as JSON
    echo json_encode(['status' => 'success', 'data' => $newsEntries]);

} catch (Exception $e) {
    // Log the script start
    logMessage("--- Script running RetrieveNews ---");
    logMessage("Error: " . $e->getMessage());
    echo json_encode(['status' => 'error', 'message' => $e->getMessage()]);
    logMessage("--- End of script RetrieveNews ---");
}
?>
