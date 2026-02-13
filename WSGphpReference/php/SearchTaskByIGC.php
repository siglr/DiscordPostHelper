<?php
require __DIR__ . '/CommonFunctions.php';
require_once __DIR__ . '/session_restore.php';

header('Content-Type: application/json');

// Ensure the user is logged in; if not, return an error response.
if (!isset($_SESSION['user']) || !isset($_SESSION['user']['id'])) {
    http_response_code(401);
    echo json_encode(["error" => "User not authenticated"]);
    exit;
}

$logEnabled = false;

if (!function_exists('dbg')) {
    function dbg(string $msg): void
    {
        global $logEnabled;
        if ($logEnabled && function_exists('logMessage')) {
            logMessage('[SearchTaskByIGC] ' . $msg);
        }
    }
}

try {

    if ($logEnabled) logMessage("SearchTaskByIGC.php: Script started.");

    // Grab all POST data
    $data = $_POST;
    if (empty($data)) {
        throw new Exception("No POST data received.");
    }

    // if JS only sent EntrySeqID, treat it as a forced match
    if (isset($data['EntrySeqID']) && !isset($data['forcedEntrySeqID'])) {
        $data['forcedEntrySeqID'] = $data['EntrySeqID'];
    }

    // Detect forced-match override
    $forced = isset($data['forcedEntrySeqID'])
           && trim($data['forcedEntrySeqID']) !== '';

    // These fields are always required, even in forced mode:
    $alwaysRequired = [
        'pilot',
        'gliderType',
        'competitionID',
        'IGCRecordDateTimeUTC'
    ];
    foreach ($alwaysRequired as $field) {
        if (!isset($data[$field]) || trim($data[$field]) === '') {
            throw new Exception("Missing required field: $field");
        }
    }

    // Only when NOT forced do we also require title + waypoints:
    if (! $forced) {
        if (!isset($data['igcTitle']) || trim($data['igcTitle']) === '') {
            throw new Exception("Missing required field: igcTitle");
        }
        if (!isset($data['igcWaypoints']) || trim($data['igcWaypoints']) === '') {
            throw new Exception("Missing required field: igcWaypoints");
        }
        // decode waypoints JSON
        if (is_string($data['igcWaypoints'])) {
            $data['igcWaypoints'] = json_decode($data['igcWaypoints'], true);
            if (json_last_error() !== JSON_ERROR_NONE) {
                throw new Exception("Could not decode igcWaypoints JSON: " . json_last_error_msg());
            }
        }
    }

    // now we can safely pull out our variables:
    $pilot                = trim($data['pilot']);
    $gliderType           = trim($data['gliderType']);
    $competitionID        = trim($data['competitionID']);
    $IGCRecordDateTimeUTC = trim($data['IGCRecordDateTimeUTC']);
    $igcLocalDate         = trim($data['LocalDate'] ?? '');
    $igcLocalTime         = trim($data['LocalTime'] ?? '');
    if (! $forced) {
        $igcTitle     = trim($data['igcTitle']);
        $igcWaypoints = $data['igcWaypoints'];
    }

    // Validate upload
    if (!isset($_FILES['igcFile']) || $_FILES['igcFile']['error'] !== UPLOAD_ERR_OK) {
        throw new Exception("IGC file not provided in the upload.");
    }

    // Open database, etc.
    $pdo = new PDO("sqlite:$databasePath");
    $pdo->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);

    $foundTask = null;
    $matchedCandidates = [];
    $candidateMeta = [];

    // ▶ Forced‐match override: if client passed forcedEntrySeqID, load that task and skip all searches
    if ($forced) {
        $forcedId = (int) trim($data['forcedEntrySeqID']);
        dbg("Forced lookup EntrySeqID={$forcedId}");
        $stmtForce = $pdo->prepare("SELECT EntrySeqID, SimDateTime, Title, PLNXML, WPRXML, LastUpdate FROM Tasks WHERE EntrySeqID = :eid");
        $stmtForce->bindValue(':eid', $forcedId, PDO::PARAM_INT);
        $stmtForce->execute();
        $foundTask = $stmtForce->fetch(PDO::FETCH_ASSOC);

        if (! $foundTask) {
            echo json_encode([
              'status'  => 'not_found',
              'message' => "Forced task ID {$forcedId} not found."
            ]);
            exit;
        }
    }

    // === STEP 1: Search by Title
    if (!$foundTask && isset($igcTitle, $igcWaypoints)) {
        dbg('STEP 1: search by Title');
        $stmt = $pdo->prepare("SELECT EntrySeqID, SimDateTime, Title, PLNXML, WPRXML, LastUpdate FROM Tasks WHERE PLNXML LIKE :titleClause");
        $titleClause = '%<Title>' . $igcTitle . '</Title>%';
        $stmt->bindParam(':titleClause', $titleClause, PDO::PARAM_STR);
        $stmt->execute();
        $titleResults = $stmt->fetchAll(PDO::FETCH_ASSOC);
        dbg('Title results count: ' . count($titleResults));

        if (!empty($titleResults)) {
            [$matchedCandidates, $candidateMeta] = accumulateMatches($titleResults, $igcWaypoints, $matchedCandidates, $candidateMeta);
        }
    }

    // === STEP 2: Search by ALL waypoint IDs
    if (!$foundTask && isset($igcWaypoints)) {
        $ids = array_map(fn($w) => normId($w['id']), $igcWaypoints);
        $clauses = $params = [];
        foreach ($ids as $wpID) {
            $clauses[] = "(PLNXML LIKE ? OR PLNXML LIKE ? OR PLNXML LIKE ? OR PLNXML LIKE ?)";
            $params[]  = '%<ATCWaypoint id="' . $wpID . '">%';
            $params[]  = '%<ATCWaypoint id=" ' . $wpID . '">%';
            $params[]  = '%<ATCWaypoint id="' . $wpID . ' ">%';
            $params[]  = '%<ATCWaypoint id=" ' . $wpID . ' ">%';
        }

        if (!empty($clauses)) {
            $sql  = "SELECT EntrySeqID, SimDateTime, Title, PLNXML, WPRXML, LastUpdate FROM Tasks WHERE " . implode(' AND ', $clauses);
            dbg('ALL IDs SQL: ' . $sql);
            $stmt = $pdo->prepare($sql);
            $stmt->execute($params);
            $wpResults = $stmt->fetchAll(PDO::FETCH_ASSOC);
            dbg('ALL IDs results count: ' . count($wpResults));

            [$matchedCandidates, $candidateMeta] = accumulateMatches($wpResults, $igcWaypoints, $matchedCandidates, $candidateMeta);
        }
    }

    // === STEP 3: Search by INTERIOR waypoint IDs
    if (!$foundTask && isset($igcWaypoints)) {
        $ids = array_map(fn($w) => normId($w['id']), $igcWaypoints);
        if (count($ids) > 2) {
            $interior = array_slice($ids, 1, -1);
            $clauses = $params = [];
            foreach ($interior as $wpID) {
                $clauses[] = "(PLNXML LIKE ? OR PLNXML LIKE ? OR PLNXML LIKE ? OR PLNXML LIKE ?)";
                $params[]  = '%<ATCWaypoint id="' . $wpID . '">%';
                $params[]  = '%<ATCWaypoint id=" ' . $wpID . '">%';
                $params[]  = '%<ATCWaypoint id="' . $wpID . ' ">%';
                $params[]  = '%<ATCWaypoint id=" ' . $wpID . ' ">%';
            }
            $sql  = "SELECT EntrySeqID, SimDateTime, Title, PLNXML, WPRXML, LastUpdate FROM Tasks WHERE " . implode(' AND ', $clauses);
            dbg('INTERIOR IDs SQL: ' . $sql);
            $stmt = $pdo->prepare($sql);
            $stmt->execute($params);
            $cands = $stmt->fetchAll(PDO::FETCH_ASSOC);
            dbg('INTERIOR results count: ' . count($cands));

            [$matchedCandidates, $candidateMeta] = accumulateMatches($cands, $igcWaypoints, $matchedCandidates, $candidateMeta);
        }
    }

    if (!$foundTask && !empty($matchedCandidates)) {
        $igcMeta = buildIgcWaypointMeta($igcWaypoints);

        $filteredByConstraints = filterByConstraints($matchedCandidates, $candidateMeta, $igcMeta);
        if (!empty($filteredByConstraints)) {
            $matchedCandidates = $filteredByConstraints;
        }

        $filteredByLocalTime = filterByLocalDateTime($matchedCandidates, $igcLocalDate, $igcLocalTime);
        if (!empty($filteredByLocalTime)) {
            $matchedCandidates = $filteredByLocalTime;
        }

        if (count($matchedCandidates) === 1) {
            $foundTask = array_values($matchedCandidates)[0];
        }
    }

    if (!$foundTask && !empty($matchedCandidates)) {
        $candidateList = [];
        foreach ($matchedCandidates as $cand) {
            $candidateList[] = [
                'EntrySeqID' => $cand['EntrySeqID'],
                'Title' => $cand['Title'],
                'PLNXML' => $cand['PLNXML'],
                'SimDateTime' => $cand['SimDateTime'],
                'LastUpdate' => $cand['LastUpdate'] ?? '',
                'WPRXML' => $cand['WPRXML'] ?? '',
                'WeatherPresetName' => extractWeatherPresetName($cand['WPRXML'] ?? '')
            ];
        }

        echo json_encode([
            'status' => 'multiple',
            'candidates' => $candidateList
        ]);
        exit;
    }

    if ($foundTask) {
        // Build the IGCKey using the new format:
        // EntrySeqID_CompetitionID_GliderType_IGCRecordDateTimeUTC
        $entrySeqID = $foundTask['EntrySeqID'];
        $simDateTime = $foundTask['SimDateTime'];
        $competitionID = trim($data['competitionID']);
        $gliderType = trim($data['gliderType']);
        $recordDateTimeUTC = trim($data['IGCRecordDateTimeUTC']); // expected in YYMMDDHHMMSS format
        
        $IGCKey = strtoupper($entrySeqID . "_" . $competitionID . "_" . $gliderType . "_" . $recordDateTimeUTC);

        if ($logEnabled) logMessage("Constructed IGCKey: " . $IGCKey);
        
        // Check the IGCRecords table for a previous entry with the same key
        $checkQuery = "SELECT IGCKey FROM IGCRecords WHERE UPPER(IGCKey) = :igcKey";
        $stmt = $pdo->prepare($checkQuery);
        $stmt->bindParam(':igcKey', $IGCKey, PDO::PARAM_STR);
        $stmt->execute();
        $existingRecord = $stmt->fetch(PDO::FETCH_ASSOC);
        
        if ($existingRecord) {
            if ($logEnabled) logMessage("Duplicate IGC record found for key: " . $IGCKey);
            echo json_encode([
                'status' => 'duplicate',
                'message' => 'An IGC record with this key already exists.',
                'IGCKey' => $IGCKey
            ]);
        } else {
            if ($logEnabled) logMessage("Found matching task: EntrySeqID = " . $foundTask['EntrySeqID'] . ", Title = " . $foundTask['Title']);
            // Save the IGC file under the temporary folder under the igckey subfolder
            $tempDir = __DIR__ . '/DPHXTemp';
            if (!is_dir($tempDir)) {
                mkdir($tempDir, 0755, true);
            }
            $igcKeyDir = $tempDir . '/' . $IGCKey;
            if (!is_dir($igcKeyDir)) {
                mkdir($igcKeyDir, 0755, true);
            }
            $targetFile = $igcKeyDir . '/' . $IGCKey . '.igc';
            
            if (!move_uploaded_file($_FILES['igcFile']['tmp_name'], $targetFile)) {
                throw new Exception("Failed to save the uploaded IGC file.");
            }

            $plnFile = $igcKeyDir . '/' . $foundTask['EntrySeqID'] . '.pln';
            if (file_put_contents($plnFile, $foundTask['PLNXML']) === false) {
                throw new Exception("Failed to write temporary PLN file to $plnFile");
            }
            
            // --- Begin Browserless Call Integration ---
            if (isset($blesstok) && !empty($blesstok)) {
                // Remove the protocol from $wsgRoot (e.g., "https://wesimglide.org" becomes "wesimglide.org")
                $rootWithoutProtocol = preg_replace('#^https?://#', '', $wsgRoot);

                // Build the URL without including "https://"
                $igcFileUrl = $rootWithoutProtocol . "/php/DPHXTemp/{$IGCKey}/" . $IGCKey . '.igc';
                $plnFileUrl = $rootWithoutProtocol . "/php/DPHXTemp/{$IGCKey}/" . $foundTask['EntrySeqID'] . '.pln';

                $flow = $forced ? 'forced' : 'normal';
                $browserlessResponse = browserlessExtractTracklogsOnly($igcFileUrl, $plnFileUrl, $flow);

                if (!empty($browserlessResponse['data'])) {
                    $browserlessResult = $browserlessResponse['data'];
                    if (!$browserlessResponse['ok']) {
                        $browserlessResult['error'] = $browserlessResponse['error'] ?? 'Browserless request failed.';
                    }
                } else {
                    $browserlessResult = [
                        'error' => $browserlessResponse['error'] ?? 'Browserless did not return data.'
                    ];
                }
            } else {
                // Fake Browserless response for testing: read from a local file.
                $fakeResponseFile = __DIR__ . '/fake_browserless_response.txt';
                if (file_exists($fakeResponseFile)) {
                    $bl_response = file_get_contents($fakeResponseFile);
                    $decoded = json_decode($bl_response, true);

                    if (json_last_error() === JSON_ERROR_NONE
                        && isset($decoded['data']['tracklogsHTML']['html'])
                    ) {
                        // Make sure plannerVersion is defined, even if empty
                        if (!isset($decoded['data']['plannerVersion']['html'])) {
                            $decoded['data']['plannerVersion'] = ['html' => ''];
                        }
                        $browserlessResult = $decoded;
                    } else {
                        // Raw HTML fallback: wrap in the expected structure, including plannerVersion
                        $browserlessResult = [
                            'data' => [
                                'tracklogsHTML'   => ['html' => $bl_response],
                                'plannerVersion'  => ['html' => '']
                            ]
                        ];
                    }
                } else {
                    $browserlessResult = [
                        'error' => 'Browserless token not configured and fake response file not found.'
                    ];
                }
            }

            $browserlessResult = parseBrowserlessTracklogs($browserlessResult, $igcKeyDir, $logEnabled);
            if (empty($browserlessResult['parsedResults'])) {
                $error = $browserlessResult['error'] ?? 'Failed to parse Browserless results.';
                if ($logEnabled) logMessage("Browserless parse failure: $error");
                echo json_encode([
                    'status' => 'found',
                    'EntrySeqID' => $foundTask['EntrySeqID'],
                    'SimDateTime' => $foundTask['SimDateTime'],
                    'Title' => $foundTask['Title'],
                    'browserless' => $browserlessResult
                ]);
                return;
            }
    
            // ————————————————
            // Identify the WSGUserID from the Users table
            // ————————————————
            $wsgUserId = 0;
            // 1) Try exact PilotName + CompID
            $stmt = $pdo->prepare("
                SELECT WSGUserID
                  FROM Users
                 WHERE PilotName = :pilot
                   AND CompID    = :comp
                 LIMIT 1
            ");
            $stmt->execute([
                ':pilot' => $pilot,
                ':comp'  => $competitionID
            ]);
            if ($row = $stmt->fetch(PDO::FETCH_ASSOC)) {
                $wsgUserId = $row['WSGUserID'];
            } else {
                // 2) Fallback: single-match on CompID or PilotName
                $compStmt  = $pdo->prepare("SELECT WSGUserID FROM Users WHERE CompID    = :comp");
                $pilotStmt = $pdo->prepare("SELECT WSGUserID FROM Users WHERE PilotName = :pilot");

                $compStmt->execute([':comp'  => $competitionID]);
                $pilotStmt->execute([':pilot' => $pilot]);

                $compRows  = $compStmt->fetchAll(PDO::FETCH_COLUMN, 0);
                $pilotRows = $pilotStmt->fetchAll(PDO::FETCH_COLUMN, 0);

                if (count($compRows) === 1 && count($pilotRows) === 0) {
                    $wsgUserId = $compRows[0];
                }
                elseif (count($pilotRows) === 1 && count($compRows) === 0) {
                    $wsgUserId = $pilotRows[0];
                }
                // otherwise leave at 0
            }

            // —————————————
            // Send JSON response
            // —————————————
            echo json_encode([
                'status'      => 'found',
                'EntrySeqID'  => $foundTask['EntrySeqID'],
                'SimDateTime' => $foundTask['SimDateTime'],
                'Title'       => $foundTask['Title'],
                'WSGUserID'   => $wsgUserId,
                'browserless' => $browserlessResult
            ]);

        }
    } else {
        if ($logEnabled) logMessage("No matching task found.");
        echo json_encode([
            'status' => 'not_found',
            'WSGUserID' => 0,
            'message' => 'No matching task was found.'
        ]);
    }

} catch (Exception $e) {
    if ($logEnabled) logMessage("Error: " . $e->getMessage());
    echo json_encode(['error' => $e->getMessage()]);
    exit;
}

/**
 * Convert a coordinate string (e.g., "N70° 56' 38.94"" or "N70°56'38.94"") to a decimal degree.
 */
function coordinateToDecimal($coord) {
    $coord = trim($coord);
    $coord = str_replace(array("Â°", "°"), "°", $coord);
    $coord = preg_replace('/\s+/', ' ', $coord);
    dbg("coordinateToDecimal normalized: " . $coord);
    $result = sscanf($coord, "%c%d° %d' %f", $hem, $deg, $min, $sec);
    if ($result === 4) {
        $decimal = $deg + ($min / 60) + ($sec / 3600);
        if ($hem === 'S' || $hem === 'W') {
            $decimal = -$decimal;
        }
        return $decimal;
    }
    return null;
}

/**
 * Normalize an XML coordinate string by removing the elevation portion.
 */
function normalizeXmlCoordinate($xmlCoord) {
    $parts = explode(',', $xmlCoord);
    if (count($parts) >= 2) {
        return [ trim($parts[0]), trim($parts[1]) ];
    }
    return null;
}

function parseWaypointMeta(string $rawId): array {
    $id = trim($rawId);
    $meta = [
        'minAlt' => null,
        'maxAlt' => null,
        'aatMinSeconds' => 0
    ];

    $parts = explode('+', $id);
    $endpart = $parts[1] ?? '';

    if ($endpart !== '') {
        if (preg_match('/\|(\d+)/', $endpart, $m)) {
            $meta['maxAlt'] = (int)$m[1];
        }
        if (preg_match('/\/(\d+)/', $endpart, $m)) {
            $meta['minAlt'] = (int)$m[1];
        }
        if (preg_match('/AAT(\d{2}):(\d{2})/i', $endpart, $m)) {
            $hours = (int)$m[1];
            $minutes = (int)$m[2];
            $meta['aatMinSeconds'] = ($hours * 3600) + ($minutes * 60);
        }
    }

    return $meta;
}

function buildIgcWaypointMeta(array $igcWaypoints): array {
    $meta = [];
    foreach ($igcWaypoints as $i => $wp) {
        $meta[$i] = parseWaypointMeta((string)($wp['id'] ?? ''));
    }
    return $meta;
}

function filterByConstraints(array $candidates, array $candidateMeta, array $igcMeta): array {
    $filtered = [];
    foreach ($candidates as $cand) {
        $entrySeq = $cand['EntrySeqID'];
        $plnMeta = $candidateMeta[$entrySeq] ?? null;
        if (!$plnMeta || !isset($plnMeta['waypoints'])) {
            $filtered[$entrySeq] = $cand;
            continue;
        }

        $plnWps = $plnMeta['waypoints'];
        $matches = true;
        foreach ($plnWps as $idx => $meta) {
            $igcWp = $igcMeta[$idx] ?? null;
            if (!$igcWp) { continue; }

            if ($meta['minAlt'] !== null && $igcWp['minAlt'] !== null && $meta['minAlt'] !== $igcWp['minAlt']) {
                $matches = false;
                break;
            }
            if ($meta['maxAlt'] !== null && $igcWp['maxAlt'] !== null && $meta['maxAlt'] !== $igcWp['maxAlt']) {
                $matches = false;
                break;
            }
        }

        $plnAat = 0;
        foreach ($plnWps as $meta) {
            if (!empty($meta['aatMinSeconds'])) {
                $plnAat = $meta['aatMinSeconds'];
                break;
            }
        }
        $igcAat = 0;
        foreach ($igcMeta as $meta) {
            if (!empty($meta['aatMinSeconds'])) {
                $igcAat = $meta['aatMinSeconds'];
                break;
            }
        }
        if ($plnAat > 0 && $igcAat > 0 && $plnAat !== $igcAat) {
            $matches = false;
        }

        if ($matches) {
            $filtered[$entrySeq] = $cand;
        }
    }
    return $filtered;
}

function buildComparableLocalDateTimeFromSim(string $simDateTime): ?DateTimeImmutable {
    if (empty($simDateTime)) { return null; }
    try {
        $sim = new DateTimeImmutable($simDateTime);
        $baseYear = 2000;
        return $sim->setDate($baseYear, (int)$sim->format('m'), (int)$sim->format('d'));
    } catch (Exception $e) {
        return null;
    }
}

function buildComparableLocalDateTime(string $datePart, string $timePart): ?DateTimeImmutable {
    if (!preg_match('/^(\d{4})-(\d{2})-(\d{2})$/', $datePart, $m)) { return null; }
    if (!preg_match('/^(\d{2})(\d{2})(\d{2})$/', $timePart, $t)) { return null; }
    $month = (int)$m[2];
    $day = (int)$m[3];
    $hour = (int)$t[1];
    $minute = (int)$t[2];
    $second = (int)$t[3];
    $baseYear = 2000;
    try {
        return new DateTimeImmutable(sprintf('%04d-%02d-%02d %02d:%02d:%02d', $baseYear, $month, $day, $hour, $minute, $second));
    } catch (Exception $e) {
        return null;
    }
}

function filterByLocalDateTime(array $candidates, string $igcDate, string $igcTime): array {
    if (empty($igcDate) || empty($igcTime)) {
        return [];
    }

    $igcDt = buildComparableLocalDateTime($igcDate, $igcTime);
    if (!$igcDt) {
        return [];
    }

    $filtered = [];
    foreach ($candidates as $cand) {
        $sim = buildComparableLocalDateTimeFromSim($cand['SimDateTime'] ?? '');
        if (!$sim) {
            continue;
        }

        $sim0     = $sim;
        $simPlus  = $sim->modify('+1 day');
        $simMinus = $sim->modify('-1 day');

        $deltaMinutes = min(
            abs($igcDt->getTimestamp() - $sim0->getTimestamp()) / 60,
            abs($igcDt->getTimestamp() - $simPlus->getTimestamp()) / 60,
            abs($igcDt->getTimestamp() - $simMinus->getTimestamp()) / 60
        );

        if ($deltaMinutes <= 60) {
            $filtered[$cand['EntrySeqID']] = $cand;
        }
    }

    return $filtered;
}

function accumulateMatches(array $candidates, array $igcWaypoints, array $currentMatches, array $meta): array {
    foreach ($candidates as $cand) {
        $entrySeq = $cand['EntrySeqID'];
        if (isset($currentMatches[$entrySeq])) { continue; }

        dbg("Validating candidate EntrySeqID={$entrySeq}");
        $plnMeta = null;
        if (validateCandidate($cand, $igcWaypoints, $plnMeta)) {
            $currentMatches[$entrySeq] = $cand;
            if ($plnMeta !== null) {
                $meta[$entrySeq] = $plnMeta;
            }
            dbg("Candidate matched: EntrySeqID={$entrySeq}");
        } else {
            dbg("Candidate failed validation: EntrySeqID={$entrySeq}");
        }
    }

    return [$currentMatches, $meta];
}

function extractWeatherPresetName(string $wprXml): string {
    if (empty($wprXml)) { return ''; }
    libxml_use_internal_errors(true);
    $xml = simplexml_load_string($wprXml);
    if (!$xml) { return ''; }
    $name = (string)($xml->{"WeatherPreset.Preset"}->Name ?? '');
    if (!empty($name)) { return $name; }
    $node = $xml->xpath('//Name');
    return $node && isset($node[0]) ? (string)$node[0] : '';
}

function normId($s) {
    return preg_replace('/\s+/', ' ', trim((string)$s));
}

function compareCoordinates($coord1, $coord2, $tolerance = 0.001) {
    $dec1 = coordinateToDecimal($coord1);
    $dec2 = coordinateToDecimal($coord2);
    if ($dec1 === null || $dec2 === null) {
        dbg("compareCoordinates: parse fail dec1=" . var_export($dec1, true) . " dec2=" . var_export($dec2, true));
        return false;
    }
    $difference = abs($dec1 - $dec2);
    $ok = $difference <= $tolerance;
    dbg("compareCoordinates: diff={$difference} tol={$tolerance} => " . ($ok ? 'OK' : 'FAIL'));
    return $ok;
}

function validateCandidate(array $candidate, array $igcWaypoints, &$plnMeta = null): bool {
    $entrySeq = $candidate['EntrySeqID'] ?? 'unknown';
    dbg("validateCandidate: *** START validation for EntrySeqID: {$entrySeq} ***");

    $igcList = array_map(fn($w)=> "{$w['id']}=>{$w['coord']}", $igcWaypoints);
    dbg("validateCandidate: IGC waypoints: " . implode(', ', $igcList));

    if (empty($candidate['PLNXML'])) {
        dbg("validateCandidate: Missing PLNXML");
        return false;
    }

    libxml_use_internal_errors(true);
    $xml = simplexml_load_string($candidate['PLNXML']);
    if (!$xml) {
        foreach (libxml_get_errors() as $err) {
            dbg("validateCandidate: XML parse error: " . trim($err->message));
        }
        libxml_clear_errors();
        return false;
    }

    $nodes = $xml->xpath('/SimBase.Document/FlightPlan.FlightPlan/ATCWaypoint');
    if (!$nodes) {
        dbg("validateCandidate: No ATCWaypoint elements in PLNXML");
        return false;
    }

    $orderedXmlPos = [];
    $xmlMapById    = [];
    $plnMetaByIndex = [];
    $dump = [];
    foreach ($nodes as $i => $wp) {
        $nameAttrRaw = (string)$wp['id'];
        $nameAttr = normId($nameAttrRaw);
        if ($nameAttr !== $nameAttrRaw) { dbg("Trimmed XML id '{$nameAttrRaw}' => '{$nameAttr}'"); }

        $rawPos   = trim((string)$wp->WorldPosition);
        $norm     = normalizeXmlCoordinate($rawPos);
        $pos      = $norm ? "{$norm[0]},{$norm[1]}" : $rawPos;

        $orderedXmlPos[$i]   = $pos;
        $xmlMapById[$nameAttr] = $pos;
        $plnMetaByIndex[$i] = parseWaypointMeta($nameAttrRaw);
        $dump[] = "{$i}:{$nameAttr}=>{$pos}";
    }
    dbg("validateCandidate: PLNXML waypoints by index|id: " . implode(' | ', $dump));

    $n = count($igcWaypoints);
    foreach ($igcWaypoints as $i => $wp) {
        $wpID = normId($wp['id']);
        $igcCoord= $wp['coord'];
        dbg("validateCandidate: Checking IGC waypoint #{$i} ID='{$wpID}', Coord='{$igcCoord}'");

        if ($i === 0 || $i === $n - 1) {
            $xmlPos = $orderedXmlPos[$i];
            dbg("validateCandidate: First/last fix—using XML index {$i} => {$xmlPos}");
        } else {
            if (!isset($xmlMapById[$wpID])) {
                dbg("validateCandidate: Interior waypoint '{$wpID}' not found by id");
                return false;
            }
            $xmlPos = $xmlMapById[$wpID];
            dbg("validateCandidate: Matched interior '{$wpID}' => {$xmlPos}");
        }

        [$xmlLat, $xmlLon] = explode(',', $xmlPos) + [null, null];
        if ($xmlLat === null || $xmlLon === null) {
            dbg("validateCandidate: Invalid XML coords for '{$wpID}': {$xmlPos}");
            return false;
        }
        $xmlLat = trim($xmlLat);
        $xmlLon = trim($xmlLon);

        $parts = explode(',', str_replace(' ', '', $igcCoord));
        if (count($parts) < 2) {
            dbg("validateCandidate: Invalid IGC coord for '{$wpID}': {$igcCoord}");
            return false;
        }
        [$igcLat, $igcLon] = array_map('trim', $parts);

        dbg("validateCandidate: Comparing LAT '{$wpID}' — IGC={$igcLat} vs XML={$xmlLat}");
        $latOk = compareCoordinates($igcLat, $xmlLat);

        dbg("validateCandidate: Comparing LON '{$wpID}' — IGC={$igcLon} vs XML={$xmlLon}");
        $lonOk = compareCoordinates($igcLon, $xmlLon);

        if (!($latOk && $lonOk)) {
            dbg("validateCandidate: Mismatch for '{$wpID}': latOk=" . ($latOk?'true':'false') . ", lonOk=" . ($lonOk?'true':'false'));
            return false;
        }

        dbg("validateCandidate: Waypoint #{$i} '{$wpID}' matches successfully");
    }

    dbg("validateCandidate: *** ALL WAYPOINTS MATCH for EntrySeqID {$entrySeq} ***");
    $plnMeta = [
        'waypoints' => $plnMetaByIndex
    ];
    return true;
}

?>
