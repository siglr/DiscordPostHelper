<?php
require __DIR__ . '/CommonFunctions.php';

try {
    logMessage("--- Script running UpdateImages ---");

    // Open the database connection
    $pdo = new PDO("sqlite:$databasePath");
    $pdo->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);
    logMessage("Database connection established.");

    // Update CoverImage
    $updateCoverImageQuery = "
        UPDATE Tasks
        SET CoverImage = NULL
        WHERE CoverImage = '';
    ";
    $stmtCoverImage = $pdo->prepare($updateCoverImageQuery);
    $stmtCoverImage->execute();
    $coverImageUpdatedRows = $stmtCoverImage->rowCount();
    logMessage("CoverImage updated rows: " . $coverImageUpdatedRows);

    // Update MapImage
    $updateMapImageQuery = "
        UPDATE Tasks
        SET MapImage = NULL
        WHERE MapImage = '';
    ";
    $stmtMapImage = $pdo->prepare($updateMapImageQuery);
    $stmtMapImage->execute();
    $mapImageUpdatedRows = $stmtMapImage->rowCount();
    logMessage("MapImage updated rows: " . $mapImageUpdatedRows);

    // Return the number of rows updated for each field
    echo json_encode([
        'status' => 'success',
        'CoverImageUpdatedRows' => $coverImageUpdatedRows,
        'MapImageUpdatedRows' => $mapImageUpdatedRows
    ]);
    logMessage("--- End of script UpdateImages ---");

} catch (PDOException $e) {
    logMessage("Connection failed: " . $e->getMessage());
    echo json_encode([
        'status' => 'error',
        'message' => 'Connection failed: ' . $e->getMessage()
    ]);
    logMessage("--- End of script UpdateImages ---");
}
?>
