<?php
require __DIR__ . '/CommonFunctions.php';

error_reporting(E_ALL);
ini_set('display_errors', 1);

// Start output buffer with HTML structure
echo "<!DOCTYPE html>
<html lang='en'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Task Processing</title>
    <style>
        body { font-family: Arial, sans-serif; line-height: 1.6; }
        .success { color: green; }
        .error { color: red; }
    </style>
</head>
<body>
<h1>Task Processing Report</h1>
<pre>";

try {
    // Database connection
    $pdo = new PDO("sqlite:$databasePath");
    $pdo->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);

    $query = "
        SELECT TaskID
        FROM Tasks
        WHERE EntrySeqID BETWEEN 1 AND 800
    ";
    $tasks = $pdo->query($query)->fetchAll(PDO::FETCH_ASSOC);

    if (empty($tasks)) {
        echo "<span class='error'>No tasks found with NULL or empty SuppressBaroPressureWarningSymbol.</span>";
        exit;
    }

    foreach ($tasks as $task) {
        $taskId = $task['TaskID'];
        $dphxFilePath = "https://siglr.com/DiscordPostHelper/TaskBrowser/Tasks/$taskId.dphx";

        echo "Processing TaskID: <strong>$taskId</strong>\n";

        // Create temporary paths
        $tempZipFile = __DIR__ . "/TempFolder/$taskId.dphx";
        $tempExtractFolder = __DIR__ . "/TempFolder/$taskId";

        // Ensure the TempFolder exists
        if (!file_exists(__DIR__ . "/TempFolder")) {
            mkdir(__DIR__ . "/TempFolder", 0755, true);
        }

        // Download the file
        $fileContent = @file_get_contents($dphxFilePath);
        if ($fileContent === false) {
            echo "<span class='error'>Failed to download file for TaskID $taskId from $dphxFilePath.</span>\n";
            continue;
        }
        file_put_contents($tempZipFile, $fileContent);

        // Unzip the file
        $zip = new ZipArchive();
        if ($zip->open($tempZipFile) === TRUE) {
            $zip->extractTo($tempExtractFolder);
            $zip->close();
        } else {
            echo "<span class='error'>Failed to extract $tempZipFile for TaskID $taskId.</span>\n";
            unlink($tempZipFile); // Clean up the downloaded zip
            continue;
        }

        // Locate the .DPH file
        $dphFilePath = glob("$tempExtractFolder/*.{dph,DPH}", GLOB_BRACE)[0] ?? null;
        if (!$dphFilePath || !file_exists($dphFilePath)) {
            echo "<span class='error'>DPH file not found for TaskID $taskId in $tempExtractFolder.</span>\n";
            unlink($tempZipFile); // Clean up the downloaded zip
            array_map('unlink', glob("$tempExtractFolder/*")); // Delete extracted files
            rmdir($tempExtractFolder); // Remove the extraction folder
            continue;
        }

        // Load the DPH file as XML
        $xml = simplexml_load_file($dphFilePath);
        if (!$xml) {
            echo "<span class='error'>Failed to parse XML for TaskID $taskId. File: $dphFilePath</span>\n";
            unlink($tempZipFile); // Clean up the downloaded zip
            array_map('unlink', glob("$tempExtractFolder/*")); // Delete extracted files
            rmdir($tempExtractFolder); // Remove the extraction folder
            continue;
        }

    // Extract values from XML
    $suppressBaro = (string)$xml->SuppressBaroPressureWarningSymbol === "true" ? 1 : 0;
    
    // Check and process BaroPressureExtraInfo
    $baroExtraInfo = trim((string)$xml->BaroPressureExtraInfo);
    
    // If the value matches either of the default values, set it to NULL
    if (
        $baroExtraInfo === 'Non standard: Set your altimeter! (Press "B" once in your glider)' || 
        $baroExtraInfo === 'Non standard: Set your altimeter!'
    ) {
        $baroExtraInfo = null;
    }
    
    // Handle RecommendedAddOns
    $recommendedAddOnsList = [];
    if ((int)$xml->RecommendedAddOns->count() > 0) {
        foreach ($xml->RecommendedAddOns as $addOn) {
            $recommendedAddOnsList[] = [
                'Name' => (string)$addOn->Name,
                'URL' => (string)$addOn->URL,
                'Type' => (string)$addOn->Type,
            ];
        }
    }
    $recommendedAddOnsJson = json_encode($recommendedAddOnsList);
    
    // Update the database
    $updateQuery = "
        UPDATE Tasks
        SET SuppressBaroPressureWarningSymbol = :suppressBaro,
            BaroPressureExtraInfo = :baroExtraInfo,
            RecommendedAddOnsList = :recommendedAddOns
        WHERE TaskID = :taskId;
    ";
    $stmt = $pdo->prepare($updateQuery);
    $stmt->execute([
        ':suppressBaro' => $suppressBaro,
        ':baroExtraInfo' => $baroExtraInfo,
        ':recommendedAddOns' => $recommendedAddOnsJson,
        ':taskId' => $taskId,
    ]);
    
    echo "TaskID $taskId updated successfully.\n";

        // Clean up
        unlink($tempZipFile); // Delete the downloaded zip file
        array_map('unlink', glob("$tempExtractFolder/*")); // Delete extracted files
        rmdir($tempExtractFolder); // Remove the extraction folder
        echo "Cleaned up temporary files for TaskID $taskId.\n\n";
    }
} catch (PDOException $e) {
    echo "<span class='error'>Database error: " . $e->getMessage() . "</span>\n";
} catch (Exception $e) {
    echo "<span class='error'>General error: " . $e->getMessage() . "</span>\n";
}

echo "</pre></body></html>";
?>
