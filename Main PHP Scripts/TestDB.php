<?php
require __DIR__ . '/CommonFunctions.php';

try {
    // Open the database connection
    $pdo = new PDO("sqlite:$databasePath");
    $pdo->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);

    // Check if the Tasks table exists
    $checkQuery = "
        SELECT name
        FROM sqlite_master
        WHERE type='table' AND name='Tasks';
    ";
    $result = $pdo->query($checkQuery)->fetch(PDO::FETCH_ASSOC);

    if ($result) {
        // Check if the RepostText and LastUpdateDescription columns exist
        $columnCheckQuery = "
            PRAGMA table_info(Tasks);
        ";
        $columns = $pdo->query($columnCheckQuery)->fetchAll(PDO::FETCH_ASSOC);

        $existingColumns = array_column($columns, 'name');

        if (!in_array('RepostText', $existingColumns)) {
            $pdo->exec("ALTER TABLE Tasks ADD COLUMN RepostText TEXT");
            echo "Field RepostText added successfully.\n";
        } else {
            echo "Field RepostText already exists.\n";
        }

        if (!in_array('LastUpdateDescription', $existingColumns)) {
            $pdo->exec("ALTER TABLE Tasks ADD COLUMN LastUpdateDescription TEXT");
            echo "Field LastUpdateDescription added successfully.\n";
        } else {
            echo "Field LastUpdateDescription already exists.\n";
        }
    } else {
        echo "Table Tasks does not exist. No changes made.\n";
    }
} catch (PDOException $e) {
    echo "Update failed: " . $e->getMessage();
}
?>
