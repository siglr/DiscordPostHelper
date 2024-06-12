<?php
require __DIR__ . '/CommonFunctions.php';

try {
    // Open the database connection
    $pdo = new PDO("sqlite:$databasePath");
    $pdo->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);

    // Update the Tasks table to trim Title and set DBEntryUpdate to now
    $updateQuery = "
        UPDATE Tasks
        SET DBEntryUpdate = datetime('now'), Title = TRIM(Title)
        WHERE Title != TRIM(Title)
    ";
    $pdo->exec($updateQuery);

    echo "Spaces trimmed from Title successfully.";
} catch (PDOException $e) {
    echo "Update failed: " . $e->getMessage();
}
