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

    // Convert KnownSoaringClubs to an array
    $clubs = [];
    foreach ($xml->KnownSoaringClubs->KnownSoaringClub as $club) {
        $sharedPublishers = [];
        if ($club->SharedPublishers) {
            foreach ($club->SharedPublishers->Name as $publisher) {
                $sharedPublishers[] = (string)$publisher;
            }
        }

        $clubs[] = [
            'ClubId' => (string)$club->ClubId,
            'ClubName' => (string)$club->ClubName,
            'ClubFullName' => (string)$club->ClubFullName,
            'TrackerGroup' => (string)$club->TrackerGroup,
            'Emoji' => (string)$club->Emoji,
            'EmojiID' => (string)$club->EmojiID,
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
            'BeginnerLinkURL' => (string)$club->BeginnerLinkURL,
            'ForceSyncFly' => filter_var($club->ForceSyncFly, FILTER_VALIDATE_BOOLEAN),
            'ForceLaunch' => filter_var($club->ForceLaunch, FILTER_VALIDATE_BOOLEAN),
            'ForceStartTask' => filter_var($club->ForceStartTask, FILTER_VALIDATE_BOOLEAN),
            'DiscordURL' => (string)$club->DiscordURL,
            'SharedPublishers' => $sharedPublishers
        ];
    }

    // Convert KnownDesigners to an array of strings for backward compatibility,
    // and also build an extended array with extra info.
    $designersAsStrings = [];
    $designersExtended = [];
    foreach ($xml->KnownDesigners->KnownDesigner as $designer) {
        $name = (string)$designer->Name;
        $discordID = (string)$designer->DiscordID;
        $designersAsStrings[] = $name;  // Original behavior: just the name.
        $designersExtended[] = [
            'Name'      => $name,
            'DiscordID' => $discordID
        ];
    }

    echo json_encode([
        'status' => 'success', 
        'clubs' => $clubs, 
        'designers' => $designersAsStrings,      // For current deployments
        'designersExtended' => $designersExtended  // For new versions that need DiscordID
    ]);

} catch (Exception $e) {
    echo json_encode(['status' => 'error', 'message' => $e->getMessage()]);
}
