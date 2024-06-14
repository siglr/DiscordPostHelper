<?php
require __DIR__ . '/CommonFunctions.php';

try {
    // Open the database connection
    $pdo = new PDO("sqlite:$databasePath");
    $pdo->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);

    // Create the WorldMapInfo table if it does not exist
    $createQuery = "
        CREATE TABLE IF NOT EXISTS WorldMapInfo (
            EntrySeqID INTEGER NOT NULL UNIQUE,
            TaskID TEXT NOT NULL UNIQUE,
            PLNFilename TEXT,
            PLNXML TEXT,
            WPRFilename TEXT,
            WPRXML TEXT,
            LatMin REAL NOT NULL,
            LatMax REAL NOT NULL,
            LongMin REAL NOT NULL,
            LongMax REAL NOT NULL,
            PRIMARY KEY(EntrySeqID)
        )
    ";
    $pdo->exec($createQuery);

    echo "Table WorldMapInfo created successfully.\n";
} catch (PDOException $e) {
    echo "Update failed: " . $e->getMessage();
}
