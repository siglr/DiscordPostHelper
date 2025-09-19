<?php
require __DIR__ . '/CommonFunctions.php';

try {
    if ($_SERVER['REQUEST_METHOD'] !== 'POST') {
        throw new Exception('Invalid request method.');
    }
    if (!isset($_POST['user_id'])) {
        throw new Exception('User ID missing.');
    }
    $userID = trim((string)$_POST['user_id']);

    // Open DB
    $pdo = new PDO("sqlite:$databasePath");
    $pdo->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);

    // 1) Fetch the single user by UserRightsID
    $sql = "
        SELECT
            UserRightsName,
            CreateTask, UpdateTask, DeleteTask,
            CreateEvent, UpdateEvent, DeleteEvent,
            CreateNews,  UpdateNews,  DeleteNews
        FROM Users
        WHERE UserRightsID = :rid
        LIMIT 1
    ";
    $stmt = $pdo->prepare($sql);
    $stmt->execute([':rid' => $userID]);
    $row = $stmt->fetch(PDO::FETCH_ASSOC);

    if (!$row) {
        throw new Exception('User not found.');
    }

    // 2) Build rights object (true/false like the XML version)
    $rights = [
        'CreateTask'  => (bool)$row['CreateTask'],
        'UpdateTask'  => (bool)$row['UpdateTask'],
        'DeleteTask'  => (bool)$row['DeleteTask'],
        'CreateEvent' => (bool)$row['CreateEvent'],
        'UpdateEvent' => (bool)$row['UpdateEvent'],
        'DeleteEvent' => (bool)$row['DeleteEvent'],
        'CreateNews'  => (bool)$row['CreateNews'],
        'UpdateNews'  => (bool)$row['UpdateNews'],
        'DeleteNews'  => (bool)$row['DeleteNews'],
    ];

    // 3) Collect all user names (equivalent to iterating the XML <User><Name>)
    $namesStmt = $pdo->query("
        SELECT UserRightsName
        FROM Users
        WHERE UserRightsName IS NOT NULL AND UserRightsName <> ''
        ORDER BY UserRightsName COLLATE NOCASE
    ");
    $allUserNames = array_map(
        fn($r) => $r['UserRightsName'],
        $namesStmt->fetchAll(PDO::FETCH_ASSOC)
    );

    echo json_encode([
        'status'       => 'success',
        'name'         => (string)$row['UserRightsName'],
        'rights'       => $rights,
        'allUserNames' => $allUserNames
    ]);

} catch (Exception $e) {
    // Optional: logMessage("RetrieveUserRights error: " . $e->getMessage());
    echo json_encode(['status' => 'error', 'message' => $e->getMessage()]);
}
