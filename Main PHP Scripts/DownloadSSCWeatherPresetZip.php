<?php
// DownloadSSCWeatherPresetZip.php
// Usage: DownloadSSCWeatherPresetZip.php?presetId=123
//
// Returns a ZIP containing the 2020 + 2024 .WPR files.
// Filenames inside the ZIP are EXACTLY the source filenames.

require __DIR__ . '/CommonFunctions.php';

function json_error($msg, $code = 400) {
    http_response_code($code);
    header('Content-Type: application/json');
    echo json_encode(['status' => 'error', 'message' => $msg]);
    exit;
}

$presetId = $_GET['presetId'] ?? '';
if (!preg_match('/^\d+$/', (string)$presetId)) {
    json_error('Missing or invalid presetId.');
}

try {
    // Open DB
    $pdo = new PDO("sqlite:$databasePath");
    $pdo->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);

    // NEW schema: filenames are stored explicitly
    $presetStmt = $pdo->prepare("
        SELECT
            PresetID,
            FileName2020,
            FileName2024
        FROM SSCWeatherPresets
        WHERE PresetID = :id
        LIMIT 1
    ");
    $presetStmt->execute([':id' => (int)$presetId]);
    $preset = $presetStmt->fetch(PDO::FETCH_ASSOC);
    if (!$preset) {
        json_error("Preset not found (PresetID=$presetId).", 404);
    }

    $file2020Name = (string)$preset['FileName2020'];
    $file2024Name = (string)$preset['FileName2024'];

    if ($file2020Name === '' || $file2024Name === '') {
        json_error("Preset filenames are missing in DB for PresetID=$presetId.", 500);
    }

    // Find app root that contains WeatherPresets
    $candidate1 = __DIR__;
    $candidate2 = dirname(__DIR__);
    if (is_dir($candidate1 . DIRECTORY_SEPARATOR . 'WeatherPresets')) {
        $appRoot = $candidate1;
    } elseif (is_dir($candidate2 . DIRECTORY_SEPARATOR . 'WeatherPresets')) {
        $appRoot = $candidate2;
    } else {
        json_error("WeatherPresets folder not found relative to this script.", 500);
    }

    // Build absolute paths
    $file2020Path = $appRoot . DIRECTORY_SEPARATOR . 'WeatherPresets' . DIRECTORY_SEPARATOR . 'SSC2020' . DIRECTORY_SEPARATOR . $file2020Name;
    $file2024Path = $appRoot . DIRECTORY_SEPARATOR . 'WeatherPresets' . DIRECTORY_SEPARATOR . 'SSC2024' . DIRECTORY_SEPARATOR . $file2024Name;

    if (!is_file($file2020Path)) json_error("2020 preset file not found: $file2020Name", 404);
    if (!is_file($file2024Path)) json_error("2024 preset file not found: $file2024Name", 404);

    // Create ZIP in temp
    $zipPath = tempnam(sys_get_temp_dir(), 'sscwp_');
    if ($zipPath === false) json_error("Unable to create temp file.", 500);

    $zipRealPath = $zipPath . '.zip';
    @rename($zipPath, $zipRealPath);

    $zip = new ZipArchive();
    if ($zip->open($zipRealPath, ZipArchive::CREATE | ZipArchive::OVERWRITE) !== true) {
        @unlink($zipRealPath);
        json_error("Unable to create ZIP archive.", 500);
    }

    // Keep exact source filenames inside ZIP
    $zip->addFile($file2020Path, basename($file2020Path));
    $zip->addFile($file2024Path, basename($file2024Path));
    $zip->close();

    // Stream ZIP
    $downloadName = 'SSCWeatherPreset_' . (int)$presetId . '.zip';
    header('Content-Type: application/zip');
    header('Content-Disposition: attachment; filename="' . $downloadName . '"');
    header('Content-Length: ' . filesize($zipRealPath));
    header('Cache-Control: no-store, no-cache, must-revalidate, max-age=0');
    header('Pragma: no-cache');

    readfile($zipRealPath);
    @unlink($zipRealPath);
    exit;

} catch (Exception $e) {
    json_error($e->getMessage(), 500);
}
