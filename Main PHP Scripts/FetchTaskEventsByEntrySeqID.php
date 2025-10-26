<?php
require __DIR__ . '/CommonFunctions.php';

try {

    // Open the database connection (Tasks DB is the main one here)
    $pdo = new PDO("sqlite:$databasePath");
    $pdo->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);

    // Ensure the request method is POST
    if ($_SERVER['REQUEST_METHOD'] !== 'POST') {
        throw new Exception('Invalid request method.');
    }

    // Ensure EntrySeqID is passed
    if (!isset($_POST['EntrySeqID'])) {
        throw new Exception('EntrySeqID is required.');
    }

    $entrySeqID = $_POST['EntrySeqID'];

    // Prepare and execute query
    $stmt = $pdo->prepare("SELECT * FROM TaskEvents WHERE EntrySeqID = :EntrySeqID ORDER BY EventDateTime DESC");
    $stmt->execute([':EntrySeqID' => $entrySeqID]);

    $rows = $stmt->fetchAll(PDO::FETCH_ASSOC);

    if (!$rows) {
        echo json_encode([
            'status' => 'success',
            'data'   => [],
            'message'=> 'No TaskEvents found for this EntrySeqID.'
        ]);
    } else {

        // For the UI/consumer: expose a date-only field based on EventDateTime
        $cleanRows = [];
        foreach ($rows as $r) {
            $dateOnly = $r['EventDateTime'];
            if (!empty($dateOnly)) {
                // Keep only YYYY-MM-DD
                $dateOnly = substr($dateOnly, 0, 10);
            }

            $r['EventDateTimeDateOnly'] = $dateOnly;
            $cleanRows[] = $r;
        }

        echo json_encode([
            'status' => 'success',
            'data'   => $cleanRows
        ]);
    }

} catch (Exception $e) {
    logMessage("Error: " . $e->getMessage());
    echo json_encode([
        'status'  => 'error',
        'message' => $e->getMessage()
    ]);
}
?>
