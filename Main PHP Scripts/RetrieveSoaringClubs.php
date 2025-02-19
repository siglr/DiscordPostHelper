<?php
header('Content-Type: application/json');
require __DIR__ . '/CommonFunctions.php';

try {

    // Check if the XML file exists
    if (!file_exists($soaringClubsPath)) {
        throw new Exception("The XML file does not exist.");
    }

    // Load the XML file
    $xml = simplexml_load_file($soaringClubsPath);
    if ($xml === false) {
        throw new Exception("Failed to load the XML file.");
    }

    // Convert XML to an array
    $clubs = [];
    foreach ($xml->KnownSoaringClub as $club) {
        $clubs[] = [
            'ClubId' => (string)$club->ClubId,
            'ClubName' => (string)$club->ClubName,
            'ClubFullName' => (string)$club->ClubFullName,
            'TrackerGroup' => (string)$club->TrackerGroup,
            'Emoji' => (string)$club->Emoji,
            'EventNewsID' => (string)$club->EventNewsID,
            'MSFSServer' => (string)$club->MSFSServer,
            'VoiceChannel' => (string)$club->VoiceChannel,
            'ZuluDayOfWeek' => (string)$club->ZuluDayOfWeek,
            'ZuluTime' => (string)$club->ZuluTime,
            'SummerZuluDayOfWeek' => (string)$club->SummerZuluDayOfWeek,
            'SummerZuluTime' => (string)$club->SummerZuluTime,
            'TimeZoneID' => (string)$club->TimeZoneID,
            'SyncFlyDelay' => (int)$club->SyncFlyDelay,
            'LaunchDelay' => (int)$club->LaunchDelay,
            'StartTaskDelay' => (int)$club->StartTaskDelay,
            'EligibleAward' => filter_var($club->EligibleAward, FILTER_VALIDATE_BOOLEAN),
            'BeginnerLink' => (string)$club->BeginnerLink,
            'ForceSyncFly' => filter_var($club->ForceSyncFly, FILTER_VALIDATE_BOOLEAN),
            'ForceLaunch' => filter_var($club->ForceLaunch, FILTER_VALIDATE_BOOLEAN),
            'ForceStartTask' => filter_var($club->ForceStartTask, FILTER_VALIDATE_BOOLEAN),
            'DiscordURL' => (string)$club->DiscordURL,
            'SharedPublishers' => array_map('strval', (array)$club->SharedPublishers->Name)
        ];
    }

    echo json_encode(['status' => 'success', 'clubs' => $clubs]);
} catch (Exception $e) {
    echo json_encode(['status' => 'error', 'message' => $e->getMessage()]);
}
