<?php
$file = 'DPHGetVersion.txt';
$date = date('Y-m-d H:i:s');

if (isset($_GET['param'])) {
    $param = $_GET['param'];
} else {
    $param = '';
}

$log = $date . ' ' . $param . "\n";
file_put_contents($file, $log, FILE_APPEND);

echo 'Logged: ' . $date;
?>
