<?php

if (isset($_GET['folder'])) {
    $folderName = $_GET['folder'];
    $folderName = preg_replace('/[^A-Za-z0-9\_\.\-]/', '', $folderName);
    $baseDir = __DIR__ . '/FlightPlans/';
    $folder = $baseDir . $folderName;
    if (is_dir($folder)) {
        // Delete all the files in the directory
        $files = glob($folder . '/*');
        foreach ($files as $file) {
            if (is_file($file)) {
                unlink($file);
            }
        }
        // Delete the directory
        rmdir($folder);
        echo "Folder '$folderName' and its contents were successfully deleted.";
    } else {
        echo "Folder '$folderName' does not exist.";
    }
} else {
    echo "Please specify a folder to delete.";
}

?>
