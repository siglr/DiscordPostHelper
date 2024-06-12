<?php

function saveTextFile($textContent, $folderName, $fileNameWithExtension) {
    // First, get the text content from POST data and then perform operations
    $textContent = utf8_encode($textContent); // Get and encode the text content

    // Replace Windows-style line endings with Unix-style
    $textContent = str_replace("\r\n", "\n", $textContent);

    // Trim any trailing newline characters
    $textContent = rtrim($textContent, "\n");

    // Sanitize folder name and file name
    $folderName = preg_replace('/[^A-Za-z0-9\_\.\-]/', '', $folderName);
    $fileName = urldecode($fileNameWithExtension); // Decode the entire filename with extension
    $reservedChars = ['\\', '/', ':', '*', '?', '"', '<', '>', '|'];
    $fileName = str_replace($reservedChars, '', $fileName);

    // Define the file path
    $baseDir = __DIR__ . '/FlightPlans/';
    $folderPath = $baseDir . $folderName;
    $filePath = $folderPath . "/" . $fileName; // Include the extension

    // Create folder if it doesn't exist
    if (!file_exists($folderPath)) {
        mkdir($folderPath, 0777, true);
    }

    // Write the content to the file
    $file = fopen($filePath, "w");
    fwrite($file, $textContent);
    fclose($file);

    echo "Text file saved to: " . $filePath;
}

function deleteOldFolders() {
    $baseDir = __DIR__ . '/FlightPlans/';
    $currentTime = time();

    foreach (new DirectoryIterator($baseDir) as $fileInfo) {
        if ($fileInfo->isDot() || !$fileInfo->isDir()) {
            continue;
        }
        if ($currentTime - $fileInfo->getMTime() > 3600) { // 3600 seconds = 60 minutes
            $folderPath = $baseDir . $fileInfo->getFilename();
            // Use recursive function to delete directories and their contents
            deleteDirectory($folderPath);
        }
    }
}

function deleteDirectory($dirPath) {
    if (!is_dir($dirPath)) {
        throw new InvalidArgumentException("$dirPath must be a directory");
    }
    if (substr($dirPath, strlen($dirPath) - 1, 1) != '/') {
        $dirPath .= '/';
    }
    $files = glob($dirPath . '*', GLOB_MARK);
    foreach ($files as $file) {
        if (is_dir($file)) {
            deleteDirectory($file);
        } else {
            unlink($file);
        }
    }
    rmdir($dirPath);
}

// Receive POST data
$textContent = $_POST["textContent"];
$folderName = $_POST["folderName"];
$fileNameWithExtension = $_POST["fileName"]; // Include the file extension here

saveTextFile($textContent, $folderName, $fileNameWithExtension);
deleteOldFolders();

?>
