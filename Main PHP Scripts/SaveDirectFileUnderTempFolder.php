<?php

function saveFile($folderName, $fileNameWithExtension) {
    // Sanitize the folder name
    $folderName = preg_replace('/[^A-Za-z0-9\_\.\-]/', '', $folderName);
    $fileName = urldecode($fileNameWithExtension); // Decode the entire filename with extension
    $reservedChars = ['\\', '/', ':', '*', '?', '"', '<', '>', '|'];
    $fileName = str_replace($reservedChars, '', $fileName);
    $baseDir = __DIR__ . '/FlightPlans/';
    $folderPath = $baseDir . $folderName;
    $filePath = $folderPath . "/" . $fileName; // Include the extension

    if (!file_exists($folderPath)) {
        mkdir($folderPath, 0777, true);
    }

    // Move the uploaded file to the desired folder
    if (move_uploaded_file($_FILES['file']['tmp_name'], $filePath)) {
        echo "File uploaded successfully: " . $filePath;
    } else {
        echo "Error uploading file";
    }
}

$folderName = $_POST["folderName"];
$fileNameWithExtension = $_POST["fileName"]; // Include the file extension here

saveFile($folderName, $fileNameWithExtension);

?>
