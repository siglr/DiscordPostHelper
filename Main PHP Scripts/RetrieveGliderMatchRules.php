<?php
header('Content-Type: application/json');
require __DIR__ . '/CommonFunctions.php';

try {
    $pdo = new PDO("sqlite:$databasePath");
    $pdo->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);

    $stmt = $pdo->query("SELECT
            r.RuleID,
            r.RuleType,
            r.Pattern,
            r.Priority,
            g.GliderKey,
            g.DisplayName
        FROM GliderMatchRules r
        INNER JOIN Gliders g ON g.GliderID = r.GliderID
        WHERE r.IsActive = 1
          AND g.IsActive = 1
        ORDER BY r.Priority ASC, r.RuleID ASC");

    $rules = [];
    foreach ($stmt->fetchAll(PDO::FETCH_ASSOC) as $row) {
        $rules[] = [
            'RuleID' => (int)$row['RuleID'],
            'RuleType' => (string)$row['RuleType'],
            'Pattern' => (string)$row['Pattern'],
            'Priority' => (int)$row['Priority'],
            'GliderKey' => (string)$row['GliderKey'],
            'DisplayName' => (string)$row['DisplayName']
        ];
    }

    echo json_encode([
        'status' => 'success',
        'rules' => $rules
    ]);
} catch (Exception $e) {
    echo json_encode([
        'status' => 'error',
        'message' => $e->getMessage()
    ]);
}
