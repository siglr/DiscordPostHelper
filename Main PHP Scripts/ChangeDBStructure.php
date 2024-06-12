<?php
require __DIR__ . '/CommonFunctions.php';

try {
    // Log the script
    logMessage("--- Script running ChangeDBStructure ---");

    // Open the database connection
    $pdo = new PDO("sqlite:$databasePath");
    $pdo->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);
    logMessage("Database connection established.");

    // Drop the News table
    $pdo->exec("DROP TABLE IF EXISTS News");
    logMessage("News table dropped successfully.");
    echo json_encode(['status' => 'success', 'message' => 'News table dropped successfully.']);

    logMessage("--- End of script ChangeDBStructure ---");

} catch (Exception $e) {
    logMessage("Error: " . $e->getMessage());
    echo json_encode(['status' => 'error', 'message' => $e->getMessage()]);

    logMessage("--- End of script ChangeDBStructure ---");
}
?>
