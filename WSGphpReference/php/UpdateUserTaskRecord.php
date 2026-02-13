<?php
require_once __DIR__ . '/session_restore.php';
require_once __DIR__ . '/CommonFunctions.php';

header('Content-Type: application/json');

// Ensure the user is logged in.
if (!isset($_SESSION['user']) || !isset($_SESSION['user']['id'])) {
    http_response_code(401);
    echo json_encode(["error" => "User not authenticated"]);
    exit;
}

$wsgUserID = $_SESSION['user']['id'];

// Validate that the user ID is a valid positive integer.
if ($wsgUserID <= 0) {
    http_response_code(400);
    error_log("Invalid WSGUserID: $wsgUserID");
    echo json_encode(["error" => "Invalid user ID"]);
    exit;
}

// Ensure the required parameter (entrySeqID) is provided via POST.
if (!isset($_POST['entrySeqID'])) {
    http_response_code(400);
    echo json_encode(["error" => "Missing required parameter: entrySeqID"]);
    exit;
}

$entrySeqID = (int) $_POST['entrySeqID'];

/**
 * Helper function to read a POST field. 
 * Returns NULL if the field is not set, is an empty string, or literally 'null' (case-insensitive).
 */
function getPostValueOrNull($key) {
    if (!isset($_POST[$key])) {
        return null;
    }
    $val = trim($_POST[$key]);
    if ($val === "" || strtolower($val) === "null") {
        return null;
    }
    return $val;
}

/*
 * Define a mapping between the POST field keys and the actual database column names.
 * For example, the client will send "MarkedFlown", which maps to the column "MarkedFlownDateUTC".
 */
$fieldsMap = [
    'PrivateNotes'    => 'PrivateNotes',
    'Tags'            => 'Tags',
    'PublicFeedback'  => 'PublicFeedback',
    'DifficultyRating'=> 'DifficultyRating',
    'QualityRating'   => 'QualityRating',
    'MarkedFlown'     => 'MarkedFlownDateUTC',
    'MarkedFlyNext'   => 'MarkedFlyNextUTC',
    'MarkedFavorites' => 'MarkedFavoritesUTC'
];

// For an existing record, we only update fields that are present in $_POST.
$updates = [];

// For an insert, we build complete lists.
$insertColumns = ['WSGUserID', 'EntrySeqID'];
$insertPlaceholders = [':wsgUserID', ':entrySeqID'];

// Always bind these parameters.
$params = [
    ':wsgUserID'  => $wsgUserID,
    ':entrySeqID' => $entrySeqID
];

try {
    // Open the database connection.
    $pdo = new PDO("sqlite:$databasePath");
    $pdo->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);

    // Check if a record exists for this user/task.
    $sqlCheck = "SELECT COUNT(*) FROM UsersTasks WHERE WSGUserID = :wsgUserID AND EntrySeqID = :entrySeqID";
    $stmtCheck = $pdo->prepare($sqlCheck);
    $stmtCheck->execute([
        ':wsgUserID'  => $wsgUserID,
        ':entrySeqID' => $entrySeqID
    ]);
    $recordExists = ($stmtCheck->fetchColumn() > 0);

    // Loop over the field mappings.
    foreach ($fieldsMap as $postKey => $dbColumn) {
        $postValue = getPostValueOrNull($postKey);
        
        if ($recordExists) {
            // For an existing record, update only if the field was explicitly provided.
            if (array_key_exists($postKey, $_POST)) {
                $updates[] = "$dbColumn = :$postKey";
                $params[":$postKey"] = $postValue;
            }
        } else {
            // For a new record, include all fields—using NULL when no value is provided.
            $insertColumns[] = $dbColumn;
            $insertPlaceholders[] = ":$postKey";
            $params[":$postKey"] = $postValue;
        }
    }
    
    if ($recordExists) {
        if (empty($updates)) {
            echo json_encode(["success" => true, "message" => "No fields were updated"]);
            exit;
        }
        $sqlUpdate = "UPDATE UsersTasks SET " . implode(", ", $updates) . " WHERE WSGUserID = :wsgUserID AND EntrySeqID = :entrySeqID";
        $stmtUpdate = $pdo->prepare($sqlUpdate);
        foreach ($params as $key => $value) {
            $stmtUpdate->bindValue($key, $value, is_null($value) ? PDO::PARAM_NULL : PDO::PARAM_STR);
        }
        $stmtUpdate->execute();
        echo json_encode(["success" => true, "message" => "Record updated"]);
    } else {
        $sqlInsert = "INSERT INTO UsersTasks (" . implode(", ", $insertColumns) . ") VALUES (" . implode(", ", $insertPlaceholders) . ")";
        $stmtInsert = $pdo->prepare($sqlInsert);
        foreach ($params as $key => $value) {
            $stmtInsert->bindValue($key, $value, is_null($value) ? PDO::PARAM_NULL : PDO::PARAM_STR);
        }
        $stmtInsert->execute();
        echo json_encode(["success" => true, "message" => "Record created"]);
    }
} catch (Exception $e) {
    http_response_code(500);
    echo json_encode(["error" => $e->getMessage()]);
}
?>
