<?php
require __DIR__ . '/CommonFunctions.php'; // for $databasePath

header('Content-Type: application/json');

try {
    // DB
    $pdo = new PDO("sqlite:$databasePath");
    $pdo->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);

    // Pull only the fields this endpoint returns
    $stmt = $pdo->query("
        SELECT
          ClubID, ClubName, ClubFullName,
          EventNewsID,
          ZuluDayOfWeek, ZuluTime,
          SummerZuluDayOfWeek, SummerZuluTime,
          TimeZoneID,
          MeetMessage, SyncMessage, NoSyncMessage,
          LaunchMessage, StartMessage
        FROM Clubs
        ORDER BY ClubID
    ");
    $rows = $stmt->fetchAll(PDO::FETCH_ASSOC);

    // Build same structure as XML version: array keyed by EventNewsID
    $clubs = [];
    foreach ($rows as $c) {
        $eventNewsId = (string)$c['EventNewsID'];
        if ($eventNewsId === '') {
            continue; // skip if not set
        }
        $clubs[$eventNewsId] = [
            'ClubId'              => (string)$c['ClubID'],
            'ClubName'            => (string)$c['ClubName'],
            'ClubFullName'        => (string)$c['ClubFullName'],
            'EventNewsID'         => $eventNewsId,
            'ZuluDayOfWeek'       => (string)$c['ZuluDayOfWeek'],
            'ZuluTime'            => (string)$c['ZuluTime'],
            'SummerZuluDayOfWeek' => (string)$c['SummerZuluDayOfWeek'],
            'SummerZuluTime'      => (string)$c['SummerZuluTime'],
            'TimeZoneID'          => (string)$c['TimeZoneID'],
            'MeetMessage'         => (string)$c['MeetMessage'],
            'SyncMessage'         => (string)$c['SyncMessage'],
            'NoSyncMessage'       => (string)$c['NoSyncMessage'],
            'LaunchMessage'       => (string)$c['LaunchMessage'],
            'StartMessage'        => (string)$c['StartMessage'],
        ];
    }

    echo json_encode(['status' => 'success', 'clubs' => $clubs], JSON_UNESCAPED_SLASHES | JSON_UNESCAPED_UNICODE);

} catch (Exception $e) {
    echo json_encode(['status' => 'error', 'message' => $e->getMessage()]);
}
