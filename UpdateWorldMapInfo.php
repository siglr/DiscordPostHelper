<?php
require __DIR__ . '/CommonFunctions.php';

try {
    // Open the database connection
    $pdo = new PDO("sqlite:$databasePath");
    $pdo->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);

    // Prepare the SQL statement for inserting or updating the entry
    $insertOrUpdateQuery = "
        INSERT OR REPLACE INTO WorldMapInfo (
            EntrySeqID, TaskID, PLNFilename, PLNXML, WPRFilename, WPRXML, LatMin, LatMax, LongMin, LongMax
        ) VALUES (
            :EntrySeqID, :TaskID, :PLNFilename, :PLNXML, :WPRFilename, :WPRXML, :LatMin, :LatMax, :LongMin, :LongMax
        )
    ";

    // Prepare the statement
    $stmt = $pdo->prepare($insertOrUpdateQuery);

    // Bind parameters from the request
    $stmt->bindParam(':EntrySeqID', $_POST['EntrySeqID']);
    $stmt->bindParam(':TaskID', $_POST['TaskID']);
    $stmt->bindParam(':PLNFilename', $_POST['PLNFilename']);
    $stmt->bindParam(':PLNXML', $_POST['PLNXML']);
    $stmt->bindParam(':WPRFilename', $_POST['WPRFilename']);
    $stmt->bindParam(':WPRXML', $_POST['WPRXML']);
    $stmt->bindParam(':LatMin', $_POST['LatMin']);
    $stmt->bindParam(':LatMax', $_POST['LatMax']);
    $stmt->bindParam(':LongMin', $_POST['LongMin']);
    $stmt->bindParam(':LongMax', $_POST['LongMax']);

    // Execute the statement
    $stmt->execute();

    echo "Entry for EntrySeqID {$_POST['EntrySeqID']} has been successfully created or updated.";
} catch (PDOException $e) {
    echo "Operation failed: " . $e->getMessage();
}
?>
