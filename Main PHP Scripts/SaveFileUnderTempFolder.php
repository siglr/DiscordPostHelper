<?php

function saveXML($xmlString, $folderName, $fileNameWithExtension) {
    $xmlString = utf8_encode($xmlString);
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

    $file = fopen($filePath, "w");
    fwrite($file, $xmlString);
    fclose($file);

    echo "XML string saved to file: " . $filePath;
}

$xmlString = $_POST["xmlString"];
$folderName = $_POST["folderName"];
$fileNameWithExtension = $_POST["fileName"]; // Include the file extension here

saveXML($xmlString, $folderName, $fileNameWithExtension);

?>
