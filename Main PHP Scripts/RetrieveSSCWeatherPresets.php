<?php
// RetrieveSSCWeatherPresets.php
header('Content-Type: application/json');
require __DIR__ . '/CommonFunctions.php';

try {
    // Open DB
    $pdo = new PDO("sqlite:$databasePath");
    $pdo->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);

    // RootPath is hardcoded
    $rootPath = 'WeatherPresets';

    // NEW schema: build paths using stored filenames (no parameters table)
    $stmt = $pdo->query("
        SELECT
            PresetID,
            PresetDescriptiveName,
            FileName2020,
            Title2020,
            FileName2024,
            Title2024
        FROM SSCWeatherPresets
        ORDER BY PresetID
    ");

    $presets = [];
    foreach ($stmt->fetchAll(PDO::FETCH_ASSOC) as $r) {
        $presets[] = [
            'PresetID'              => (int)$r['PresetID'],
            'PresetDescriptiveName' => (string)$r['PresetDescriptiveName'],

            'PresetMSFSTitle2020'   => (string)$r['Title2020'],
            'PresetMSFSTitle2024'   => (string)$r['Title2024'],

            // Paths are now deterministic:
            'PresetFile2020'        => $rootPath . '/SSC2020/' . (string)$r['FileName2020'],
            'PresetFile2024'        => $rootPath . '/SSC2024/' . (string)$r['FileName2024'],
        ];
    }

    echo json_encode([
        'status'  => 'success',
        'presets' => $presets
    ]);

} catch (Exception $e) {
    echo json_encode(['status' => 'error', 'message' => $e->getMessage()]);
}
