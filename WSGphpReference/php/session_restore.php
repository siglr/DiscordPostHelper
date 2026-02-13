<?php
require_once __DIR__ . '/CommonFunctions.php';

// Set session and cookie parameters to persist for 30 days
ini_set('session.gc_maxlifetime', 86400 * 30);

$domain = preg_replace('#^https?://#', '', $wsgRoot);
$domain = rtrim($domain, '/');

session_set_cookie_params([
    'lifetime' => 86400 * 30,
    'path'     => '/',
    'domain'   => $domain,
    'secure'   => isset($_SERVER['HTTPS']) && $_SERVER['HTTPS'] === 'on',
    'httponly' => true,
    'samesite' => 'Lax'
]);

if (session_status() !== PHP_SESSION_ACTIVE) {
    session_start();
}

if (!isset($_SESSION['user']) && isset($_COOKIE['WSGUserID'])) {
    $config       = include __DIR__ . '/config.php';
    $databasePath = $config['databasePath'];

    try {
        withSqliteBusyRetry(function () use ($databasePath) {
            $pdo = createSqliteConnection($databasePath);

            // Prepare and execute query to fetch user data.
            $stmt = $pdo->prepare(
                "SELECT
                    WSGUserID,
                    WSGDisplayName,
                    AvatarURL,
                    PilotName,
                    CompID
                 FROM Users
                 WHERE WSGUserID = ?"
            );
            $stmt->execute([$_COOKIE['WSGUserID']]);
            $userRow = $stmt->fetch(PDO::FETCH_ASSOC);

            if ($userRow) {
                $_SESSION['user'] = [
                    'id'          => $userRow['WSGUserID'],
                    'displayName' => $userRow['WSGDisplayName'],
                    'avatar'      => $userRow['AvatarURL'],
                    'pilotName'   => $userRow['PilotName'] ?? '',
                    'compId'      => $userRow['CompID'] ?? ''
                ];

                $discordUpdate = refreshDiscordProfileIfStale($pdo, $userRow['WSGUserID']);
                if ($discordUpdate) {
                    $_SESSION['user']['displayName'] = $discordUpdate['displayName'];
                    $_SESSION['user']['avatar'] = $discordUpdate['avatar'];
                }
            }
        }, 'session restore', ['WSGUserID' => $_COOKIE['WSGUserID']]);
    } catch (Exception $e) {
        logMessage(
            "Error restoring session for WSGUserID "
            . $_COOKIE['WSGUserID']
            . ': '
            . $e->getMessage()
        );
    }
}
