<?php
session_start();

// Include the common functions and configuration file
include_once 'CommonFunctions.php';

// Retrieve the Discord credentials from the configuration
$clientId    = $config['discordClientId'];
$redirectUri = $config['discordRedirectUri'];

$oauthUrl = 'https://discord.com/api/oauth2/authorize?client_id=' . $clientId . '&redirect_uri=' . urlencode($redirectUri) . '&response_type=code&scope=identify';

header('Location: ' . $oauthUrl);
exit();
?>
