<?php
header('Content-Type: text/plain');
require __DIR__ . '/CommonFunctions.php';

try {
    $discordId = null;
    if (isset($_GET['DiscordID'])) {
        $discordId = trim((string)$_GET['DiscordID']);
    } elseif (isset($_POST['DiscordID'])) {
        $discordId = trim((string)$_POST['DiscordID']);
    } elseif (isset($_GET['discord_id'])) {
        $discordId = trim((string)$_GET['discord_id']);
    } elseif (isset($_POST['discord_id'])) {
        $discordId = trim((string)$_POST['discord_id']);
    }

    if ($discordId === null || $discordId === '') {
        throw new Exception('Discord ID missing!');
    }

    $pdo = new PDO("sqlite:$databasePath");
    $pdo->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);

    $stmt = $pdo->prepare("
        SELECT 1
        FROM UsersDiscord
        WHERE DiscordID = :discordId
        LIMIT 1
    ");
    $stmt->execute([':discordId' => $discordId]);
    $row = $stmt->fetch(PDO::FETCH_ASSOC);

    echo $row ? 'Found' : 'NotFound';
} catch (Exception $e) {
    echo 'Error';
}
