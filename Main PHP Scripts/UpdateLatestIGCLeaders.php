<?php
require_once __DIR__ . '/CommonFunctions.php';

// Resolve otherdata/ from $homeLeaderboardCacheDir (which points to .../otherdata/homeleaderboardcache/)
$otherdataDir = rtrim(dirname(rtrim($homeLeaderboardCacheDir, '/')), '/') . '/';

try {
    $pdo = new PDO("sqlite:$databasePath");
    $pdo->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);
} catch (Exception $e) {
    echo "❌ Error (DB connect): " . $e->getMessage() . "\n";
    exit(1);
}

// === Top Contributors (Last 7 Days) ===
try {
    $stmt = $pdo->query("
      SELECT
        Pilot,
        COUNT(*)                   AS UploadCount,
        MAX(IGCUploadDateTimeUTC) AS LastUpload
      FROM IGCRecords
      WHERE
        IGCUploadDateTimeUTC >= datetime('now', '-7 days')
        AND Pilot IS NOT NULL
        AND TRIM(Pilot) <> ''
        AND COALESCE(IsPrivate, 0) = 0
        AND COALESCE(MarkedAsDesigner, 0) <> 1
      GROUP BY Pilot
      ORDER BY
        UploadCount DESC,
        LastUpload  DESC,
        Pilot       ASC
      LIMIT 5
    ");
    $contribResults = $stmt->fetchAll(PDO::FETCH_ASSOC);

    $jsonPathContrib = $otherdataDir . 'topIGCContributors7Days.json';
    file_put_contents($jsonPathContrib, json_encode($contribResults, JSON_PRETTY_PRINT | JSON_UNESCAPED_UNICODE));
    echo "✔ Top contributors (7d) written to topIGCContributors7Days.json\n";
} catch (Exception $e) {
    echo "❌ Error (Top Contributors): " . $e->getMessage() . "\n";
}

// === Top Gliders (Last 7 Days) ===
try {
    $stmt = $pdo->query("
        SELECT
            GliderType,
            COUNT(*) AS FlightCount
        FROM IGCRecords
        WHERE
            IGCUploadDateTimeUTC >= datetime('now', '-7 days')
            AND GliderType IS NOT NULL
            AND TRIM(GliderType) <> ''
            AND COALESCE(IsPrivate, 0) = 0
            AND COALESCE(MarkedAsDesigner, 0) <> 1
        GROUP BY GliderType
        ORDER BY FlightCount DESC, GliderType ASC
        LIMIT 5
    ");
    $gliderResults = $stmt->fetchAll(PDO::FETCH_ASSOC);

    $jsonPathGliders = $otherdataDir . 'topGliders7Days.json';
    file_put_contents($jsonPathGliders, json_encode($gliderResults, JSON_PRETTY_PRINT | JSON_UNESCAPED_UNICODE));
    echo "✔ Top gliders (7d) written to topGliders7Days.json\n";
} catch (Exception $e) {
    echo "❌ Error (Top Gliders): " . $e->getMessage() . "\n";
}

// === MSFS Versions (Last 7 Days) ===
try {
    $stmt = $pdo->query("
        SELECT
            Sim AS Version,
            COUNT(*) AS VersionCount
        FROM IGCRecords
        WHERE
            IGCUploadDateTimeUTC >= datetime('now', '-7 days')
            AND Sim IS NOT NULL
            AND TRIM(Sim) <> ''
            AND COALESCE(IsPrivate, 0) = 0
            AND COALESCE(MarkedAsDesigner, 0) <> 1
        GROUP BY Sim
        ORDER BY VersionCount DESC, Version ASC
        LIMIT 5
    ");
    $versionResults = $stmt->fetchAll(PDO::FETCH_ASSOC);

    $jsonPathVersions = $otherdataDir . 'msfsVersions7Days.json';
    file_put_contents($jsonPathVersions, json_encode($versionResults, JSON_PRETTY_PRINT | JSON_UNESCAPED_UNICODE));
    echo "✔ MSFS versions (7d) written to msfsVersions7Days.json\n";
} catch (Exception $e) {
    echo "❌ Error (MSFS Versions): " . $e->getMessage() . "\n";
}
