<?php
header('Content-Type: application/json');
require __DIR__ . '/CommonFunctions.php';

try {
    // Open DB
    $pdo = new PDO("sqlite:$databasePath");
    $pdo->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);

    // Load single parameters row
    $paramsStmt = $pdo->query("
        SELECT Prefix2024, Prefix2020, Folder2020, Folder2024
        FROM SSCWeatherPresetsParameters
        LIMIT 1
    ");
    $params = $paramsStmt->fetch(PDO::FETCH_ASSOC);
    if (!$params) {
        throw new Exception("SSCWeatherPresetsParameters is empty.");
    }

    // RootPath is hardcoded
    $rootPath = 'WeatherPresets';

    // Load presets + compute paths
    $stmt = $pdo->prepare("
        SELECT
            p.PresetID,
            p.PresetDescriptiveName,
            p.PresetMSFSTitle,
            (? || '/' || ? || '/' || ? || ' ' || p.PresetMSFSTitle || '.WPR') AS PresetFile2020,
            (? || '/' || ? || '/' || ? || ' ' || p.PresetMSFSTitle || '.WPR') AS PresetFile2024
        FROM SSCWeatherPresets p
        ORDER BY p.PresetID
    ");

    $stmt->execute([
        $rootPath, $params['Folder2020'], $params['Prefix2020'],
        $rootPath, $params['Folder2024'], $params['Prefix2024'],
    ]);

    $presets = [];
    foreach ($stmt->fetchAll(PDO::FETCH_ASSOC) as $r) {
        $presets[] = [
            'PresetID'              => (int)$r['PresetID'],
            'PresetDescriptiveName' => (string)$r['PresetDescriptiveName'],
            'PresetMSFSTitle'       => (string)$r['PresetMSFSTitle'],
            'PresetFile2020'        => (string)$r['PresetFile2020'],
            'PresetFile2024'        => (string)$r['PresetFile2024'],
        ];
    }

    echo json_encode([
        'status'  => 'success',
        'presets' => $presets
    ]);

} catch (Exception $e) {
    echo json_encode(['status' => 'error', 'message' => $e->getMessage()]);
}
