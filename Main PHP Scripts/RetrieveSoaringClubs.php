<?php
header('Content-Type: application/json');
require __DIR__ . '/CommonFunctions.php';

try {
    // Open DB
    $pdo = new PDO("sqlite:$databasePath");
    $pdo->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);

    // 1) Load all clubs
    $clubs = [];
    $clubStmt = $pdo->query("
        SELECT
          ClubID, ClubName, ClubFullName, TrackerGroup, Emoji, EmojiID, EventNewsID,
          MSFSServer, VoiceChannel, ZuluDayOfWeek, ZuluTime,
          SummerZuluDayOfWeek, SummerZuluTime, TimeZoneID,
          SyncFlyDelay, LaunchDelay, StartTaskDelay, EligibleAward,
          BeginnerLinkURL, BeginnerLink, ForceSyncFly, ForceLaunch, ForceStartTask,
          DiscordURL
        FROM Clubs
        ORDER BY ClubID
    ");
    $clubRows = $clubStmt->fetchAll(PDO::FETCH_ASSOC);

    // 2) Load publishers once (resolve names through Users)
    $pubStmt = $pdo->query("
        SELECT
          p.ClubID, p.Shared, p.Authorized,
          u.UserRightsName
        FROM ClubPublishers p
        JOIN Users u ON u.WSGUserID = p.WSGUserID
        ORDER BY p.ClubID, u.UserRightsName COLLATE NOCASE
    ");
    $pubRows = $pubStmt->fetchAll(PDO::FETCH_ASSOC);

    // Build quick lookup: clubId => ['shared'=>[], 'authorized'=>[]]
    $pubByClub = [];
    foreach ($pubRows as $r) {
        $cid = $r['ClubID'];
        if (!isset($pubByClub[$cid])) {
            $pubByClub[$cid] = ['shared' => [], 'authorized' => []];
        }
        if (!empty($r['Shared']))     { $pubByClub[$cid]['shared'][]     = (string)$r['UserRightsName']; }
        if (!empty($r['Authorized'])) { $pubByClub[$cid]['authorized'][] = (string)$r['UserRightsName']; }
    }

    // 3) Assemble club payload (exact same fields & booleans as before)
    foreach ($clubRows as $c) {
        $cid = $c['ClubID'];
        $shared     = $pubByClub[$cid]['shared']     ?? [];
        $authorized = $pubByClub[$cid]['authorized'] ?? [];

        $clubs[] = [
            'ClubId'                => (string)$c['ClubID'],
            'ClubName'              => (string)$c['ClubName'],
            'ClubFullName'          => (string)$c['ClubFullName'],
            'TrackerGroup'          => (string)$c['TrackerGroup'],
            'Emoji'                 => (string)$c['Emoji'],
            'EmojiID'               => (string)$c['EmojiID'],
            'EventNewsID'           => (string)$c['EventNewsID'],
            'MSFSServer'            => (string)$c['MSFSServer'],
            'VoiceChannel'          => (string)$c['VoiceChannel'],
            'ZuluDayOfWeek'         => (string)$c['ZuluDayOfWeek'],
            'ZuluTime'              => (string)$c['ZuluTime'],
            'SummerZuluDayOfWeek'   => (string)$c['SummerZuluDayOfWeek'],
            'SummerZuluTime'        => (string)$c['SummerZuluTime'],
            'TimeZoneID'            => (string)$c['TimeZoneID'],
            'SyncFlyDelay'          => (int)$c['SyncFlyDelay'],
            'LaunchDelay'           => (int)$c['LaunchDelay'],
            'StartTaskDelay'        => (int)$c['StartTaskDelay'],
            'EligibleAward'         => (bool)$c['EligibleAward'],
            'BeginnerLink'          => (string)$c['BeginnerLink'],
            'BeginnerLinkURL'       => (string)$c['BeginnerLinkURL'],
            'ForceSyncFly'          => (bool)$c['ForceSyncFly'],
            'ForceLaunch'           => (bool)$c['ForceLaunch'],
            'ForceStartTask'        => (bool)$c['ForceStartTask'],
            'DiscordURL'            => (string)$c['DiscordURL'],
            'SharedPublishers'      => $shared,      // array of names
            'AuthorizedPublishers'  => $authorized   // array of names
        ];
    }

    // 4) Designers: only return those with a DiscordID
    $designersStrings  = [];
    $designersExtended = [];
    
    $desStmt = $pdo->query("
        SELECT
          COALESCE(NULLIF(u.DesignerName,''), u.UserRightsName) AS DesignerDisplay,
          ud.DiscordID
        FROM Users u
        JOIN UsersDiscord ud ON ud.WSGUserID = u.WSGUserID
        WHERE u.KnownDesigner = 1
          AND ud.DiscordID IS NOT NULL
          AND ud.DiscordID <> ''
        ORDER BY DesignerDisplay COLLATE NOCASE
    ");
    foreach ($desStmt->fetchAll(PDO::FETCH_ASSOC) as $r) {
        $name = (string)$r['DesignerDisplay'];
        $designersStrings[] = $name;
        $designersExtended[] = [
            'Name'      => $name,
            'DiscordID' => (string)$r['DiscordID'],
        ];
    }

    echo json_encode([
        'status'            => 'success',
        'clubs'             => $clubs,
        'designers'         => $designersStrings,   // legacy consumers
        'designersExtended' => $designersExtended   // newer consumers
    ]);

} catch (Exception $e) {
    echo json_encode(['status' => 'error', 'message' => $e->getMessage()]);
}
