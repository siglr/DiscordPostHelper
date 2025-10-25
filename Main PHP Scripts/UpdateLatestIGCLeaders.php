<?php
require_once __DIR__ . '/CommonFunctions.php';

try {
    $pdo = new PDO("sqlite:$databasePath");
    $pdo->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);

    // === Top Performances ===
    $sql = "
        SELECT 
            IGC.IGCKey,
            IGC.EntrySeqID,
            IGC.IGCUploadDateTimeUTC,
            IGC.Pilot,
            IGC.GliderID,
            IGC.GliderType,
            IGC.CompetitionClass,
            IGC.Speed,
            IGC.Sim,
            T.Title
        FROM IGCRecords IGC
        JOIN Tasks T ON IGC.EntrySeqID = T.EntrySeqID
        WHERE
            IGC.IGCValid = 1
            AND IGC.TaskCompleted = 1
            AND abs(strftime('%s', T.SimDateTime) - strftime(
                '%s',
                substr(T.SimDateTime, 1, 4) || '-' || 
                substr(IGC.LocalDate, 6, 5) || ' ' || 
                substr(IGC.LocalTime, 1, 2) || ':' || 
                substr(IGC.LocalTime, 3, 2) || ':' || 
                substr(IGC.LocalTime, 5, 2)
            )) <= 1800
            AND IGC.Speed = (
                SELECT MAX(Speed)
                FROM IGCRecords I2
                WHERE 
                    I2.EntrySeqID = IGC.EntrySeqID
                    AND I2.IGCValid = 1
                    AND I2.TaskCompleted = 1
                    AND abs(strftime('%s', T.SimDateTime) - strftime(
                        '%s',
                        substr(T.SimDateTime, 1, 4) || '-' || 
                        substr(I2.LocalDate, 6, 5) || ' ' || 
                        substr(I2.LocalTime, 1, 2) || ':' || 
                        substr(I2.LocalTime, 3, 2) || ':' || 
                        substr(I2.LocalTime, 5, 2)
                    )) <= 1800
            )
        GROUP BY IGC.EntrySeqID
        ORDER BY IGC.IGCUploadDateTimeUTC DESC
        LIMIT 10
    ";
    $stmt = $pdo->prepare($sql);
    $stmt->execute();
    $results = $stmt->fetchAll(PDO::FETCH_ASSOC);

    // Format speed to one decimal place
    foreach ($results as &$row) {
        $row['Speed'] = number_format((float)$row['Speed'], 1, '.', '');
    }
    unset($row);

    $jsonPath = __DIR__ . '/../otherdata/latestTopIGCs.json';
    file_put_contents($jsonPath, json_encode($results, JSON_PRETTY_PRINT | JSON_UNESCAPED_UNICODE));
    echo "✔ Top IGC data written to latestTopIGCs.json\n";
} catch (Exception $e) {
    echo "❌ Error (Top Performances): " . $e->getMessage() . "\n";
}

// === Top Contributors (Last 7 Days) ===
try {
    $stmt = $pdo->query("
      SELECT
        Pilot,
        COUNT(*)              AS UploadCount,
        MAX(IGCUploadDateTimeUTC) AS LastUpload
      FROM IGCRecords
      WHERE
        IGCUploadDateTimeUTC >= datetime('now', '-7 days')
        AND Pilot IS NOT NULL
        AND TRIM(Pilot) <> ''
      GROUP BY Pilot
      ORDER BY
        UploadCount DESC,
        LastUpload  DESC,
        Pilot       ASC
      LIMIT 5
    ");
    $contribResults = $stmt->fetchAll(PDO::FETCH_ASSOC);

    $jsonPathContrib = __DIR__ . '/../otherdata/topIGCContributors7Days.json';
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
        GROUP BY GliderType
        ORDER BY FlightCount DESC, GliderType ASC
        LIMIT 5
    ");
    $gliderResults = $stmt->fetchAll(PDO::FETCH_ASSOC);

    $jsonPathGliders = __DIR__ . '/../otherdata/topGliders7Days.json';
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
        GROUP BY Sim
        ORDER BY VersionCount DESC, Version ASC
        LIMIT 5
    ");
    $versionResults = $stmt->fetchAll(PDO::FETCH_ASSOC);

    $jsonPathVersions = __DIR__ . '/../otherdata/msfsVersions7Days.json';
    file_put_contents($jsonPathVersions, json_encode($versionResults, JSON_PRETTY_PRINT | JSON_UNESCAPED_UNICODE));
    echo "✔ MSFS versions (7d) written to msfsVersions7Days.json\n";
} catch (Exception $e) {
    echo "❌ Error (MSFS Versions): " . $e->getMessage() . "\n";
}
