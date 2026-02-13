<?php
session_start();

// Unset all session variables
$_SESSION = array();

// Destroy the session
session_destroy();

// Delete cookies by setting their expiration time in the past
setcookie('WSGUserID', '', time() - 3600, "/");

// Optionally clear any legacy cookies
setcookie('user_id', '', time() - 3600, "/");
setcookie('username', '', time() - 3600, "/");
setcookie('avatar', '', time() - 3600, "/");

// Redirect to the account tab on your main page
header('Location: ../index.html?tab=account');
exit();
?>
