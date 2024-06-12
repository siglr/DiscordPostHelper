<?php
require __DIR__ . '/CommonFunctions.php';

try {
    // Open the database connection
    $pdo = new PDO("sqlite:$databasePath");
    $pdo->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);

    // Retrieve the lastDownloadUpdate parameter from the query string
    if (isset($_GET['lastDownloadUpdate']) && !empty($_GET['lastDownloadUpdate'])) {
        $lastDownloadUpdate = $_GET['lastDownloadUpdate'];

        // Define the query to retrieve the records with LastDownloadUpdate greater than the provided parameter
        $query = "SELECT EntrySeqID, TotDownloads, LastDownloadUpdate
                  FROM Tasks
                  WHERE LastDownloadUpdate > :lastDownloadUpdate";

        // Prepare and execute the query
        $stmt = $pdo->prepare($query);
        $stmt->bindParam(':lastDownloadUpdate', $lastDownloadUpdate, PDO::PARAM_STR);
        $stmt->execute();
        $tasks = $stmt->fetchAll(PDO::FETCH_ASSOC);

        // Output the results as JSON
        header('Content-Type: application/json');
        echo json_encode($tasks);
    } else {
        // Handle missing or empty lastDownloadUpdate parameter
        echo json_encode([
            'error' => 'Missing or empty lastDownloadUpdate parameter'
        ]);
    }

} catch (PDOException $e) {
    logMessage("--- Script running GetUpdatedDownloads ---");
    echo json_encode([
        'error' => 'Connection failed: ' . $e->getMessage()
    ]);
    logMessage("--- End of script GetUpdatedDownloads ---");
}
?>
