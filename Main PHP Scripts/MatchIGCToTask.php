<?php
require __DIR__ . '/CommonFunctions.php';

/**
 * ─────────────────────────────────────────────────────────────────────────────
 * DEBUG TOGGLE
 * Flip to true to enable verbose logging via your existing logMessage().
 * ─────────────────────────────────────────────────────────────────────────────
 */
$DEBUG_LOG = false;

/**
 * Conditional logger that reuses your existing logMessage() if available.
 * Does nothing when $DEBUG_LOG is false.
 */
if (!function_exists('dbg')) {
    function dbg($msg) {
        global $DEBUG_LOG;
        if ($DEBUG_LOG && function_exists('logMessage')) {
            // Prefix with script name for easier grepping
            logMessage('[SearchTaskByIGC] ' . $msg);
        }
    }
}

try {
    dbg('=== START Request ===');
    // 1) Grab POST data
    $data = $_POST;
    dbg('Raw POST keys: ' . implode(',', array_keys($data ?? [])));
    if (empty($data)) {
        dbg('No POST data received.');
        throw new Exception("No POST data received.");
    }

    // Decode waypoints if needed
    if (isset($data['igcWaypoints']) && is_string($data['igcWaypoints'])) {
        dbg('igcWaypoints provided as JSON string. Decoding…');
        $data['igcWaypoints'] = json_decode($data['igcWaypoints'], true);
        dbg('igcWaypoints decoded. Count=' . (is_array($data['igcWaypoints']) ? count($data['igcWaypoints']) : 0));
    }

    // 3) Open DB
    dbg('Opening database connection…');
    $pdo = new PDO("sqlite:$databasePath");
    $pdo->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);
    dbg('Database opened OK.');

    $foundTask = null;

    // If EntrySeqID has been forced - retrieve the PLNXML and nothing else
    $entrySeqID = isset($data['entrySeqID']) ? (int)$data['entrySeqID'] : 0;
    dbg('Forced EntrySeqID (0 = none): ' . $entrySeqID);
    if ($entrySeqID > 0) {
        dbg("Forced lookup by EntrySeqID={$entrySeqID}");
        $stmt = $pdo->prepare("
            SELECT EntrySeqID, Title, PLNXML, SimDateTime
              FROM Tasks
             WHERE EntrySeqID = :EntrySeqID
        ");
        $stmt->bindValue(':EntrySeqID', $entrySeqID, PDO::PARAM_INT);
        $stmt->execute();
        $foundTask = $stmt->fetch(PDO::FETCH_ASSOC);
        dbg($foundTask ? "Forced task found: EntrySeqID={$foundTask['EntrySeqID']}" : 'Forced task NOT FOUND');
        if (!$foundTask) {
            throw new Exception("Forced task does not exist!");
        }
    }

    //
    // === STEP 1: Search by Title ===
    //
    if (!$foundTask) {
        dbg('STEP 1: Search by Title');
        if (!isset($data['igcTitle']) || !isset($data['igcWaypoints'])) {
            dbg('Missing required parameters igcTitle and/or igcWaypoints');
            throw new Exception("Required parameters missing: igcTitle and igcWaypoints.");
        }
        $igcTitle     = trim($data['igcTitle']);
        $igcWaypoints = $data['igcWaypoints'];
        dbg("igcTitle='{$igcTitle}', waypointsCount=" . count((array)$igcWaypoints));

        $stmt = $pdo->prepare(
            "SELECT EntrySeqID, Title, PLNXML, SimDateTime FROM Tasks WHERE PLNXML LIKE :titleClause"
        );
        $titleClause = '%<Title>' . $igcTitle . '</Title>%';
        $stmt->bindParam(':titleClause', $titleClause, PDO::PARAM_STR);
        dbg("Title LIKE clause: {$titleClause}");
        $stmt->execute();
        $titleResults = $stmt->fetchAll(PDO::FETCH_ASSOC);
        dbg('Title search results count: ' . count($titleResults));

        if (!empty($titleResults)) {
            foreach ($titleResults as $cand) {
                dbg("Validating candidate by title: EntrySeqID={$cand['EntrySeqID']}");
                if (validateCandidate($cand, $igcWaypoints)) {
                    $foundTask = $cand;
                    dbg("Candidate matched by title: EntrySeqID={$cand['EntrySeqID']}");
                    break;
                } else {
                    dbg("Candidate failed by title: EntrySeqID={$cand['EntrySeqID']}");
                }
            }
        }
    }

    //
    // === STEP 2: If no title match, search by ALL waypoint IDs ===
    //
    if (!$foundTask) {
        dbg('STEP 2: Search by ALL waypoint IDs');
        $ids = array_map(fn($w) => normId($w['id']), $igcWaypoints);
        dbg('IGC waypoint IDs: ' . implode(', ', $ids));
        $clauses = $params = [];
        foreach ($ids as $wpID) {
            $clauses[] = "(PLNXML LIKE ? OR PLNXML LIKE ? OR PLNXML LIKE ? OR PLNXML LIKE ?)";
            $params[]  = '%<ATCWaypoint id="' . $wpID . '">%';    // exact
            $params[]  = '%<ATCWaypoint id=" ' . $wpID . '">%';   // leading space
            $params[]  = '%<ATCWaypoint id="' . $wpID . ' ">%';   // trailing space
            $params[]  = '%<ATCWaypoint id=" ' . $wpID . ' ">%';  // both sides
        }
        if (!empty($clauses)) {
            $sql  = "SELECT EntrySeqID, Title, PLNXML, SimDateTime FROM Tasks WHERE " . implode(' AND ', $clauses);
            dbg('Waypoint SQL (ALL IDs): ' . $sql);
            dbg('Params: ' . implode(' | ', $params));
            $stmt = $pdo->prepare($sql);
            $stmt->execute($params);
            $wpResults = $stmt->fetchAll(PDO::FETCH_ASSOC);
            dbg('ALL-ID search results count: ' . count($wpResults));

            foreach ($wpResults as $cand) {
                dbg("Validating candidate by ALL IDs: EntrySeqID={$cand['EntrySeqID']}");
                if (validateCandidate($cand, $igcWaypoints)) {
                    $foundTask = $cand;
                    dbg("Candidate matched by ALL IDs: EntrySeqID={$cand['EntrySeqID']}");
                    break;
                } else {
                    dbg("Candidate failed by ALL IDs: EntrySeqID={$cand['EntrySeqID']}");
                }
            }
        } else {
            dbg('No waypoint IDs available for ALL-ID search.');
        }
    }

    //
    // === STEP 3: If still not found, match by INTERIOR IDs only ===
    //
    if (!$foundTask) {
        dbg('STEP 3: Search by INTERIOR waypoint IDs');
        $ids = array_map(fn($w) => normId($w['id']), $igcWaypoints);
        if (count($ids) > 2) {
            $interior = array_slice($ids, 1, -1);
            dbg('Interior IDs: ' . implode(', ', $interior));
            $clauses = $params = [];
            foreach ($interior as $wpID) {
                $clauses[] = "(PLNXML LIKE ? OR PLNXML LIKE ? OR PLNXML LIKE ? OR PLNXML LIKE ?)";
                $params[]  = '%<ATCWaypoint id="' . $wpID . '">%';    // exact
                $params[]  = '%<ATCWaypoint id=" ' . $wpID . '">%';   // leading space
                $params[]  = '%<ATCWaypoint id="' . $wpID . ' ">%';   // trailing space
                $params[]  = '%<ATCWaypoint id=" ' . $wpID . ' ">%';  // both sides
            }
            $sql  = "SELECT EntrySeqID, Title, PLNXML, SimDateTime FROM Tasks WHERE " . implode(' AND ', $clauses);
            dbg('Waypoint SQL (INTERIOR): ' . $sql);
            dbg('Params: ' . implode(' | ', $params));
            $stmt = $pdo->prepare($sql);
            $stmt->execute($params);
            $cands = $stmt->fetchAll(PDO::FETCH_ASSOC);
            dbg('INTERIOR-ID search results count: ' . count($cands));

            foreach ($cands as $cand) {
                dbg("Validating candidate by INTERIOR IDs: EntrySeqID={$cand['EntrySeqID']}");
                if (validateCandidate($cand, $igcWaypoints)) {
                    $foundTask = $cand;
                    dbg("Candidate matched by INTERIOR IDs: EntrySeqID={$cand['EntrySeqID']}");
                    break;
                } else {
                    dbg("Candidate failed by INTERIOR IDs: EntrySeqID={$cand['EntrySeqID']}");
                }
            }
        } else {
            dbg('Not enough waypoints to perform INTERIOR-ID search.');
        }
    }

    // 4) Return JSON
    if ($foundTask) {
        dbg("FOUND: EntrySeqID={$foundTask['EntrySeqID']} Title='{$foundTask['Title']}' SimDateTime='{$foundTask['SimDateTime']}'");
        echo json_encode([
            'status'     => 'found',
            'EntrySeqID' => $foundTask['EntrySeqID'],
            'Title' => $foundTask['Title'],
            'PLNXML' => $foundTask['PLNXML'],
            'SimDateTime'  => $foundTask['SimDateTime']
        ]);
    } else {
        dbg('NOT FOUND — returning status=not_found');
        echo json_encode([
            'status' => 'not_found'
        ]);
    }

    dbg('=== END Request ===');
}
catch (Exception $e) {
    dbg('ERROR: ' . $e->getMessage());
    echo json_encode([
        'status'  => 'error',
        'message' => $e->getMessage()
    ]);
    exit;
}

function normId($s) {
    return preg_replace('/\s+/', ' ', trim((string)$s));
}

/**
 * Convert a coordinate string (e.g., "N70° 56' 38.94\"" or "N70°56'38.94\"") to a decimal degree.
 * This version normalizes the string using str_replace and preg_replace, then uses sscanf.
 *
 * @param string $coord
 * @return float|null
 */
function coordinateToDecimal($coord) {
    $coord = trim($coord);
    $coord = str_replace(array("\xC2\xB0", "°"), "°", $coord);
    $coord = preg_replace('/\s+/', ' ', $coord);
    // logMessage("coordinateToDecimal normalized: " . $coord);
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
 * For example, from "N70° 56' 38.92\",W8° 39' 8.43\",+000021.00" return an array:
 *   [ "N70° 56' 38.92\"", "W8° 39' 8.43\"" ]
 *
 * @param string $xmlCoord
 * @return array|null
 */
function normalizeXmlCoordinate($xmlCoord) {
    $parts = explode(',', $xmlCoord);
    if (count($parts) >= 2) {
        return [ trim($parts[0]), trim($parts[1]) ];
    }
    return null;
}

/**
 * Compare two coordinate strings allowing for a small tolerance.
 *
 * @param string $coord1
 * @param string $coord2
 * @param float $tolerance (default: 0.001)
 * @return bool
 */
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

/**
 * Validate a candidate TASK against the ordered IGC waypoints.
 * Skips id/name lookup for first & last waypoints, using position by index instead.
 *
 * @param array $candidate    The task row, must include ['PLNXML']
 * @param array $igcWaypoints Numeric array of ['id'=>'…','coord'=>'…']
 * @return bool               True if all waypoints pass tolerance check
 */
function validateCandidate(array $candidate, array $igcWaypoints): bool {
    $entrySeq = $candidate['EntrySeqID'] ?? 'unknown';
    // logMessage("validateCandidate: *** START validation for EntrySeqID: {$entrySeq} ***");
    dbg("validateCandidate: *** START validation for EntrySeqID: {$entrySeq} ***");

    // Build human list of IGC fixes
    $igcList = array_map(fn($w)=> "{$w['id']}=>{$w['coord']}", $igcWaypoints);
    // logMessage("validateCandidate: IGC waypoints: " . implode(', ', $igcList));
    dbg("validateCandidate: IGC waypoints: " . implode(', ', $igcList));

    if (empty($candidate['PLNXML'])) {
        // logMessage("validateCandidate: Missing PLNXML");
        dbg("validateCandidate: Missing PLNXML");
        return false;
    }

    // Parse the PLNXML
    libxml_use_internal_errors(true);
    $xml = simplexml_load_string($candidate['PLNXML']);
    if (!$xml) {
        // foreach (libxml_get_errors() as $err) {
        //     logMessage("validateCandidate: XML parse error: " . trim($err->message));
        // }
        foreach (libxml_get_errors() as $err) {
            dbg("validateCandidate: XML parse error: " . trim($err->message));
        }
        libxml_clear_errors();
        return false;
    }

    // Extract all ATCWaypoint elements in order
    $nodes = $xml->xpath('/SimBase.Document/FlightPlan.FlightPlan/ATCWaypoint');
    if (!$nodes) {
        // logMessage("validateCandidate: No ATCWaypoint elements in PLNXML");
        dbg("validateCandidate: No ATCWaypoint elements in PLNXML");
        return false;
    }

    // Build two structures:
    // 1) $orderedXmlPos[i] = "lat,lon" for waypoint at index i
    // 2) $xmlMapById[name] = "lat,lon" for lookup by id (interior fixes)
    $orderedXmlPos = [];
    $xmlMapById    = [];
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
        $dump[] = "{$i}:{$nameAttr}=>{$pos}";
    }
    // logMessage("validateCandidate: PLNXML waypoints by index|id: " . implode(' | ', $dump));
    dbg("validateCandidate: PLNXML waypoints by index|id: " . implode(' | ', $dump));

    $n = count($igcWaypoints);
    // Compare each IGC fix to the corresponding XML position
    foreach ($igcWaypoints as $i => $wp) {
        $wpID = normId($wp['id']);
        $igcCoord= $wp['coord'];
        // logMessage("validateCandidate: Checking IGC waypoint #{$i} ID='{$wpID}', Coord='{$igcCoord}'");
        dbg("validateCandidate: Checking IGC waypoint #{$i} ID='{$wpID}', Coord='{$igcCoord}'");

        // First or last? use position by index, skip id lookup
        if ($i === 0 || $i === $n - 1) {
            $xmlPos = $orderedXmlPos[$i];
            // logMessage("validateCandidate: First/last fix—using XML index {$i} => {$xmlPos}");
            dbg("validateCandidate: First/last fix—using XML index {$i} => {$xmlPos}");
        } else {
            // interior: must exist by id
            if (!isset($xmlMapById[$wpID])) {
                // logMessage("validateCandidate: Interior waypoint '{$wpID}' not found by id");
                dbg("validateCandidate: Interior waypoint '{$wpID}' not found by id");
                return false;
            }
            $xmlPos = $xmlMapById[$wpID];
            // logMessage("validateCandidate: Matched interior '{$wpID}' => {$xmlPos}");
            dbg("validateCandidate: Matched interior '{$wpID}' => {$xmlPos}");
        }

        // Split lat/lon from XML
        [$xmlLat, $xmlLon] = explode(',', $xmlPos) + [null, null];
        if ($xmlLat === null || $xmlLon === null) {
            // logMessage("validateCandidate: Invalid XML coords for '{$wpID}': {$xmlPos}");
            dbg("validateCandidate: Invalid XML coords for '{$wpID}': {$xmlPos}");
            return false;
        }
        $xmlLat = trim($xmlLat);
        $xmlLon = trim($xmlLon);

        // Parse IGC coords (strip spaces)
        $parts = explode(',', str_replace(' ', '', $igcCoord));
        if (count($parts) < 2) {
            // logMessage("validateCandidate: Invalid IGC coord for '{$wpID}': {$igcCoord}");
            dbg("validateCandidate: Invalid IGC coord for '{$wpID}': {$igcCoord}");
            return false;
        }
        [$igcLat, $igcLon] = array_map('trim', $parts);

        // Compare latitude
        // logMessage("validateCandidate: Comparing LAT '{$wpID}' — IGC={$igcLat} vs XML={$xmlLat}");
        dbg("validateCandidate: Comparing LAT '{$wpID}' — IGC={$igcLat} vs XML={$xmlLat}");
        $latOk = compareCoordinates($igcLat, $xmlLat);

        // Compare longitude
        // logMessage("validateCandidate: Comparing LON '{$wpID}' — IGC={$igcLon} vs XML={$xmlLon}");
        dbg("validateCandidate: Comparing LON '{$wpID}' — IGC={$igcLon} vs XML={$xmlLon}");
        $lonOk = compareCoordinates($igcLon, $xmlLon);

        if (!($latOk && $lonOk)) {
            // logMessage("validateCandidate: Mismatch for '{$wpID}': latOk=" . ($latOk?'true':'false') . ", lonOk=" . ($lonOk?'true':'false'));
            dbg("validateCandidate: Mismatch for '{$wpID}': latOk=" . ($latOk?'true':'false') . ", lonOk=" . ($lonOk?'true':'false'));
            return false;
        }

        // logMessage("validateCandidate: Waypoint #{$i} '{$wpID}' matches successfully");
        dbg("validateCandidate: Waypoint #{$i} '{$wpID}' matches successfully");
    }

    // logMessage("validateCandidate: *** ALL WAYPOINTS MATCH for EntrySeqID {$entrySeq} ***");
    dbg("validateCandidate: *** ALL WAYPOINTS MATCH for EntrySeqID {$entrySeq} ***");
    return true;
}
?>