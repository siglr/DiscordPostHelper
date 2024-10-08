<?php
require __DIR__ . '/CommonFunctions.php';

try {

    //logMessage("--- Script running RetrieveNews ---");

    // Open the database connection
    $pdo = new PDO("sqlite:$newsDBPath");
    $pdo->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);

    // Call the cleanup function
    cleanUpNewsEntries($pdo);

    // Get the optional fullText parameter
    $fullText = isset($_GET['fullText']) ? filter_var($_GET['fullText'], FILTER_VALIDATE_BOOLEAN) : false;

    // Query to fetch all news entries
    $stmt = $pdo->query("SELECT * FROM News ORDER BY NewsType DESC, EventDate ASC, Published DESC;");
    $newsEntries = $stmt->fetchAll(PDO::FETCH_ASSOC);

    // Function to truncate text fields
    function truncate($text, $length = 75) {
        return strlen($text) > $length ? substr($text, 0, $length) . '...' : $text;
    }

    // Truncate text fields if fullText is false
    if (!$fullText) {
        foreach ($newsEntries as &$entry) {
            foreach ($entry as $key => $value) {
                if (is_string($value) && $key !== 'URLToGo') {  // Skip truncation for 'URLToGo' field
                    $entry[$key] = truncate($value);
                }
            }
        }
    }

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
