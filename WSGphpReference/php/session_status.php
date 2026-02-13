<?php
require_once __DIR__ . '/CommonFunctions.php';
require_once __DIR__ . '/session_restore.php';

// 2) Bring in $databasePath
$config       = include __DIR__ . '/config.php';
$databasePath = $config['databasePath'];

$response = [];

if (isset($_SESSION['user']['id'])) {
    $wsgUserID = $_SESSION['user']['id'];

    try {
        $pdo = createSqliteConnection($databasePath);

        // 3) Update LastLoginUTC
        $nowUTC = gmdate('Y-m-d H:i:s');
        $stmt = $pdo->prepare(
            "UPDATE Users
             SET LastLoginUTC = ?
             WHERE WSGUserID = ?"
        );
        $stmt->execute([$nowUTC, $wsgUserID]);

        // 4) Re-fetch PilotName & CompID (in case they changed elsewhere)
        $stmt = $pdo->prepare(
            "SELECT PilotName, CompID, PoolSuperAdmin, COALESCE(IGCPrivateDefault, 0) AS IGCPrivateDefault
             FROM Users
             WHERE WSGUserID = ?"
        );
        $stmt->execute([$wsgUserID]);
        $row = $stmt->fetch(PDO::FETCH_ASSOC);

        if ($row) {
            // merge into session user
            $_SESSION['user']['pilotName'] = $row['PilotName'] ?? '';
            $_SESSION['user']['compId'] = $row['CompID'] ?? '';
            $_SESSION['user']['poolSuperAdmin'] = isset($row['PoolSuperAdmin'])
                ? (int) $row['PoolSuperAdmin'] === 1
                : false;
            $_SESSION['user']['igcPrivateDefault'] = (int) ($row['IGCPrivateDefault'] ?? 0) === 1;
        }

        $response = [
            'loggedIn' => true,
            'user'     => $_SESSION['user']
        ];
    } catch (Exception $e) {
        logMessage("session_status error: " . $e->getMessage());
        $response = [
            'loggedIn' => false,
            'error'    => 'Database error'
        ];
    }
} else {
    $response = ['loggedIn' => false];
}

header('Content-Type: application/json');
echo json_encode($response);
