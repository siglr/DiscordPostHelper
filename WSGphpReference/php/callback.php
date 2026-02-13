<?php
// Set session and cookie parameters to persist for 30 days BEFORE starting the session
ini_set('session.gc_maxlifetime', 86400 * 30);
session_set_cookie_params(86400 * 30);
session_start();

// Include common functions and configuration
include_once 'CommonFunctions.php';

// Retrieve Discord credentials from configuration
$clientId     = $config['discordClientId'];
$clientSecret = $config['discordClientSecret'];
$redirectUri  = $config['discordRedirectUri'];

// Check for error (user cancelled, etc.)
if (isset($_GET['error'])) {
    // Optionally log the error:
    // logMessage("Discord OAuth error: " . $_GET['error'] . " - " . $_GET['error_description']);
    
    // Redirect back to the account tab without logging in
    header('Location: ../index.html?tab=account');
    exit();
}

if (isset($_GET['code'])) {
    $code = $_GET['code'];

    // Exchange the authorization code for an access token
    $tokenUrl = 'https://discord.com/api/oauth2/token';
    $data = [
        'client_id'     => $clientId,
        'client_secret' => $clientSecret,
        'grant_type'    => 'authorization_code',
        'code'          => $code,
        'redirect_uri'  => $redirectUri
    ];

    $options = [
        'http' => [
            'header'  => "Content-type: application/x-www-form-urlencoded\r\n",
            'method'  => 'POST',
            'content' => http_build_query($data)
        ]
    ];

    $context = stream_context_create($options);
    $result = file_get_contents($tokenUrl, false, $context);
    if ($result === FALSE) {
        die('Error fetching access token.');
    }
    $tokenData = json_decode($result, true);
    if (!isset($tokenData['access_token'])) {
        die('Invalid access token response: ' . json_encode($tokenData));
    }
    $accessToken = $tokenData['access_token'];
    $refreshToken = isset($tokenData['refresh_token']) ? $tokenData['refresh_token'] : null;
    $tokenExpiresUTC = null;
    if (!empty($tokenData['expires_in'])) {
        $tokenExpiresUTC = gmdate('Y-m-d H:i:s', time() + (int) $tokenData['expires_in']);
    }

    // Fetch Discord user data
    $userUrl = 'https://discord.com/api/users/@me';
    $options = [
        'http' => [
            'header' => "Authorization: Bearer $accessToken\r\n",
            'method' => 'GET'
        ]
    ];

    $context = stream_context_create($options);
    $userResult = file_get_contents($userUrl, false, $context);
    if ($userResult === FALSE) {
        die('Error fetching user data.');
    }
    $discordUser = json_decode($userResult, true);
    if (!isset($discordUser['id'])) {
        die('Invalid user data response: ' . json_encode($discordUser));
    }

    // Use Discord’s global_name if set, otherwise fall back to username
    if (!empty($discordUser['global_name'])) {
        $displayName = $discordUser['global_name'];
    } else {
        // you can include the discriminator if you want uniqueness:
        $displayName = $discordUser['username']; // . '#' . $discordUser['discriminator'];
    }
    
    // Construct the full avatar URL using Discord's CDN.
    $avatarURL = "https://cdn.discordapp.com/avatars/" . $discordUser['id'] . "/" . $discordUser['avatar'] . ".png";

    // Connect to the SQLite database using $databasePath from CommonFunctions.php
    $pdo = createSqliteConnection($databasePath);

    // Get current UTC timestamp
    $nowUTC = gmdate('Y-m-d H:i:s');

    // Check if this Discord ID already exists in UsersDiscord
    $stmt = $pdo->prepare("SELECT WSGUserID FROM UsersDiscord WHERE DiscordID = ?");
    $stmt->execute([$discordUser['id']]);
    $resultRow = $stmt->fetch(PDO::FETCH_ASSOC);

    if ($resultRow && isset($resultRow['WSGUserID'])) {
        // Existing user found; retrieve the internal user ID
        $wsgUserID = $resultRow['WSGUserID'];
        
        // Update the user's display name, avatar URL, and LastLoginUTC
        $updateStmt = $pdo->prepare("UPDATE Users SET WSGDisplayName = ?, LastLoginUTC = ?, AvatarURL = ? WHERE WSGUserID = ?");
        $updateStmt->execute([$displayName, $nowUTC, $avatarURL, $wsgUserID]);
    } else {
        // New user: create an entry in Users using the global_name and avatar URL, then create UsersDiscord entry.
        $insertStmt = $pdo->prepare("INSERT INTO Users (WSGDisplayName, JoinedUTC, LastLoginUTC, AvatarURL) VALUES (?, ?, ?, ?)");
        $insertStmt->execute([$displayName, $nowUTC, $nowUTC, $avatarURL]);
        $wsgUserID = $pdo->lastInsertId();

        // Create the association in UsersDiscord
        $insertDiscordStmt = $pdo->prepare("INSERT INTO UsersDiscord (DiscordID, WSGUserID) VALUES (?, ?)");
        $insertDiscordStmt->execute([$discordUser['id'], $wsgUserID]);

        // Send email notification for new user
        $to = "guy@siglr.com"; // Replace with your email address.
        $subject = "New User Created on WeSimGlide.org";
        $message = "A new user has been created on WeSimGlide.org.\n\n"
                 . "Display Name: " . $displayName . "\n"
                 . "Joined UTC: " . $nowUTC;
        $headers = "From: no-reply@wesimglide.org\r\n" .
                   "Reply-To: no-reply@wesimglide.org\r\n" .
                   "X-Mailer: PHP/" . phpversion();
        mail($to, $subject, $message, $headers);
    }

    try {
        $tokenExistsStmt = $pdo->prepare("SELECT WSGUserID FROM UsersDiscordTokens WHERE WSGUserID = ?");
        $tokenExistsStmt->execute([$wsgUserID]);
        $tokenRow = $tokenExistsStmt->fetch(PDO::FETCH_ASSOC);

        if ($tokenRow) {
            $updateTokenStmt = $pdo->prepare("
                UPDATE UsersDiscordTokens
                SET DiscordID = ?,
                    AccessToken = ?,
                    RefreshToken = ?,
                    TokenExpiresUTC = ?,
                    LastDiscordSyncUTC = ?
                WHERE WSGUserID = ?
            ");
            $updateTokenStmt->execute([
                $discordUser['id'],
                $accessToken,
                $refreshToken,
                $tokenExpiresUTC,
                $nowUTC,
                $wsgUserID
            ]);
        } else {
            $insertTokenStmt = $pdo->prepare("
                INSERT INTO UsersDiscordTokens (WSGUserID, DiscordID, AccessToken, RefreshToken, TokenExpiresUTC, LastDiscordSyncUTC)
                VALUES (?, ?, ?, ?, ?, ?)
            ");
            $insertTokenStmt->execute([
                $wsgUserID,
                $discordUser['id'],
                $accessToken,
                $refreshToken,
                $tokenExpiresUTC,
                $nowUTC
            ]);
        }
    } catch (Exception $e) {
        logDiscordSyncMessage("Discord token persistence skipped: " . $e->getMessage());
    }

    // Update the session with the internal WSGUserID and minimal user info including AvatarURL
    $_SESSION['WSGUserID'] = $wsgUserID;
    $_SESSION['user'] = [
       'id'          => $wsgUserID,
       'displayName' => $displayName,
       'discordID'   => $discordUser['id'],
       'avatar'      => $avatarURL
    ];

    // Optionally, set a cookie for the WSGUserID (if needed)
    setcookie('WSGUserID', $wsgUserID, time() + (86400 * 30), "/");

    // Redirect to the account tab on your main page
    header('Location: ../index.html?tab=account');
    exit();
}
?>
