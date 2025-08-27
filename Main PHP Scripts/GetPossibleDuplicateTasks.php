<?php
// GetPossibleDuplicateTasks.php
require __DIR__ . '/CommonFunctions.php';

try {
    // Open DB
    $pdo = new PDO("sqlite:$databasePath");
    $pdo->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);

    // Inputs
    $title   = isset($_GET['Title']) ? trim($_GET['Title']) : '';
    $latMin  = isset($_GET['LatMin']) ? floatval($_GET['LatMin']) : null;
    $latMax  = isset($_GET['LatMax']) ? floatval($_GET['LatMax']) : null;
    $lonMin  = isset($_GET['LongMin']) ? floatval($_GET['LongMin']) : null;
    $lonMax  = isset($_GET['LongMax']) ? floatval($_GET['LongMax']) : null;
    $epsilon = isset($_GET['Epsilon']) ? max(0.0, floatval($_GET['Epsilon'])) : 0.001;

    $haveBBox  = ($latMin !== null && $latMax !== null && $lonMin !== null && $lonMax !== null);
    $haveTitle = ($title !== '');

    if (!$haveBBox && !$haveTitle) {
        header('Content-Type: application/json');
        echo json_encode(['error' => 'Provide Title and/or LatMin,LatMax,LongMin,LongMax']);
        exit;
    }

    $branches = [];
    $params   = [];

    if ($haveBBox) {
        // SAME BOX within epsilon: each candidate edge must be within [value - eps, value + eps]
        $branches[] = "
            SELECT EntrySeqID, TaskID, Title, PLNXML, WPRXML
            FROM Tasks
            WHERE
                LatMin  BETWEEN :latMin_lo  AND :latMin_hi  AND
                LatMax  BETWEEN :latMax_lo  AND :latMax_hi  AND
                LongMin BETWEEN :lonMin_lo  AND :lonMin_hi  AND
                LongMax BETWEEN :lonMax_lo  AND :lonMax_hi
        ";
        $params[':latMin_lo'] = $latMin - $epsilon;
        $params[':latMin_hi'] = $latMin + $epsilon;
        $params[':latMax_lo'] = $latMax - $epsilon;
        $params[':latMax_hi'] = $latMax + $epsilon;
        $params[':lonMin_lo'] = $lonMin - $epsilon;
        $params[':lonMin_hi'] = $lonMin + $epsilon;
        $params[':lonMax_lo'] = $lonMax - $epsilon;
        $params[':lonMax_hi'] = $lonMax + $epsilon;
    }

    if ($haveTitle) {
        $branches[] = "
            SELECT EntrySeqID, TaskID, Title, PLNXML, WPRXML
            FROM Tasks
            WHERE Title = :title COLLATE NOCASE
        ";
        $params[':title'] = $title;
    }

    $sql = "
        SELECT DISTINCT EntrySeqID, TaskID, Title, PLNXML, WPRXML
        FROM (
            " . implode("\nUNION ALL\n", $branches) . "
        ) AS candidates
    ";

    $stmt = $pdo->prepare($sql);
    $stmt->execute($params);
    $tasks = $stmt->fetchAll(PDO::FETCH_ASSOC);

    header('Content-Type: application/json');
    echo json_encode($tasks);

} catch (PDOException $e) {
    logMessage('--- Script running GetPossibleDuplicateTasks (UNION, same-box) ---');
    header('Content-Type: application/json');
    echo json_encode(['error' => 'Connection failed: ' . $e->getMessage()]);
    logMessage('--- End of script GetPossibleDuplicateTasks (UNION, same-box) ---');
}
