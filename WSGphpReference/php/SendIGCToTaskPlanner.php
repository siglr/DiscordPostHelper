<?php
require_once __DIR__ . '/CommonFunctions.php';

header('Content-Type: application/json');

try {
    // Check required POST parameters.
    $required = ['EntrySeqID', 'TaskID', 'PLNFilename', 'WPRFilename'];
    foreach ($required as $field) {
        if (!isset($_POST[$field]) || trim($_POST[$field]) === "") {
            throw new Exception("Missing required field: $field");
        }
    }
    
    $entrySeqID = (int) $_POST['EntrySeqID'];
    $taskID = trim($_POST['TaskID']);
    // Strip any directory paths; keep only the filenames.
    $plnFilename = basename(str_replace('\\', '/', trim($_POST['PLNFilename'])));
    $wprFilename = basename(str_replace('\\', '/', trim($_POST['WPRFilename'])));
    
    // Create a temporary folder (used to store the comp file)
    $tempDir = __DIR__ . '/DPHXTemp';
    $randomFolder = uniqid("igc_", true);
    $destFolder = $tempDir . '/' . $randomFolder;
    if (!mkdir($destFolder, 0755, true)) {
        throw new Exception("Failed to create temporary folder: $destFolder");
    }
    
    // Determine mode:
    // - Pre-uploaded mode if 'tempIGCKey' exists and is not empty.
    // - Otherwise, expect a list of IGC keys in POST parameter 'igcKeys'.
    $usePreUploaded = false;
    if (isset($_POST['tempIGCKey']) && trim($_POST['tempIGCKey']) !== "") {
        $usePreUploaded = true;
        $tempIGCKey = trim($_POST['tempIGCKey']);
    } elseif (isset($_POST['igcKeys']) && trim($_POST['igcKeys']) !== "") {
        $usePreUploaded = false;
    } else {
        throw new Exception("No pre-uploaded IGC file key provided and no IGC key list provided.");
    }
    
    if ($usePreUploaded) {
        // --- PRE-UPLOADED MODE ---
        // The IGC file has been saved previously under tempIGCKey subfolder.
        // Compute the expected file path.
        $destFilePath = $tempDir . '/' . $tempIGCKey . '/' . $tempIGCKey . '.igc';
        if (!file_exists($destFilePath)) {
            throw new Exception("Pre-uploaded IGC file not found in expected folder.");
        }
        // Build the URL to access the pre-uploaded IGC file.
        $igcBasePath = __DIR__ . '/DPHXTemp';
        if (strpos($igcBasePath, '/home3/siglr3/soaring.siglr.com/') === 0) {
            $igcBasePath = str_replace('/home3/siglr3/soaring.siglr.com/', 'soaring.siglr.com/', $igcBasePath);
        } elseif (strpos($igcBasePath, '/home3/siglr3/wesimglide/') === 0) {
            $igcBasePath = str_replace('/home3/siglr3/wesimglide/', 'wesimglide.org/', $igcBasePath);
        }
        $igcFileUrl = $igcBasePath . '/' . $tempIGCKey . '/' . $tempIGCKey . '.igc';
        $igcFileUrlNoProtocol = preg_replace('/^https?:\/\//', '', $igcFileUrl);
    }
    
    // Call the PrepareSendToB21OnlineTaskPlanner.php script to obtain the task folder.
    ob_start();
    $_GET['taskID'] = $taskID;
    include __DIR__ . '/PrepareSendToB21OnlineTaskPlanner.php';
    $prepareResponse = ob_get_clean();
    if ($prepareResponse === false) {
        throw new Exception("Failed to capture output from PrepareSendToB21OnlineTaskPlanner.php");
    }
    $prepareData = json_decode($prepareResponse, true);
    if (!$prepareData || $prepareData['status'] !== 'success') {
        throw new Exception("PrepareSendToB21OnlineTaskPlanner error: " . ($prepareData['message'] ?? 'Unknown error'));
    }
    // $taskFolder is expected to be a URL path like:
    // soaring.siglr.com/php/DPHXTemp/P-83263a91-88dd-4507-97e5-983f8dc4b082
    $taskFolder = $prepareData['taskFolder'];
    
    // Build URLs for the PLN and WPR files (used in both modes)
    $plnFileUrl = $taskFolder . "/" . $plnFilename;
    $wprFileUrl = $taskFolder . "/" . $wprFilename;
    
    // Build comp file content.
    // First two lines are always the PLN and WPR file URLs (with "https://")
    $compLines = [];
    $compLines[] = 'https://' . $plnFileUrl;
    $compLines[] = 'https://' . $wprFileUrl;
    
    if ($usePreUploaded) {
        // In pre-uploaded mode, add the pre-uploaded IGC file URL.
        $compLines[] = 'https://' . $igcFileUrlNoProtocol;
    } else {
        // --- IGC KEY LIST MODE ---
        // Expect a POST parameter 'igcKeys' containing either an array or a comma-separated string.
        $igcKeys = $_POST['igcKeys'];
        if (!is_array($igcKeys)) {
            $igcKeys = array_map('trim', explode(',', $igcKeys));
        }
        // We assume that $taskBrowserPathHTTPS is defined (e.g. in CommonFunctions.php)
        if (!isset($taskBrowserPathHTTPS)) {
            throw new Exception("\$taskBrowserPathHTTPS is not defined.");
        }
        foreach ($igcKeys as $key) {
            // Build URL: $taskBrowserPathHTTPS/IGCFiles/EntrySeqID/key
            $igcKeyUrl = rtrim($taskBrowserPathHTTPS, '/') . '/IGCFiles/' . $entrySeqID . '/' . $key . '.igc';
            // Ensure the URL starts with "https://"
            if (stripos($igcKeyUrl, 'http') !== 0) {
                $igcKeyUrl = 'https://' . $igcKeyUrl;
            }
            $compLines[] = $igcKeyUrl;
        }
    }
    
    // Join all lines with a newline and write the comp file.
    $compFileContent = implode("\n", $compLines);
    $compFilePath = $destFolder . '/listoffiles.comp';
    if (file_put_contents($compFilePath, $compFileContent) === false) {
        throw new Exception("Failed to create comp file: $compFilePath");
    }
    
    // Build the comp file URL (without protocol) for the planner URL.
    // (We use the same base transformation as before.)
    $igcBasePath = __DIR__ . '/DPHXTemp';
    if (strpos($igcBasePath, '/home3/siglr3/soaring.siglr.com/') === 0) {
        $igcBasePath = str_replace('/home3/siglr3/soaring.siglr.com/', 'soaring.siglr.com/', $igcBasePath);
    } elseif (strpos($igcBasePath, '/home3/siglr3/wesimglide/') === 0) {
        $igcBasePath = str_replace('/home3/siglr3/wesimglide/', 'wesimglide.org/', $igcBasePath);
    }
    $compFileUrl = $igcBasePath . '/' . $randomFolder . '/listoffiles.comp';
    
    // Build the final planner URL.
    // The planner URL will pass:
    // - wpr: the URL for the WPR file (without protocol)
    // - comp: the URL for the comp file (without protocol)
    $wprParam = rawurlencode($wprFileUrl);
    $compParam = rawurlencode($compFileUrl);
    
    $plannerUrl = "xp-soaring.github.io/tasks/b21_task_planner/index.html?wpr={$wprParam}&comp={$compParam}";
    
    echo json_encode([
        'status'     => 'success',
        'plannerUrl' => $plannerUrl,
        'tempFolder' => $randomFolder
    ]);
    
} catch (Exception $e) {
    echo json_encode(['status' => 'error', 'message' => $e->getMessage()]);
}
?>
