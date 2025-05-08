<?php
require __DIR__ . '/CommonFunctions.php';

try {
    // 1) Grab POST data
    $data = $_POST;
    if (empty($data)) {
        throw new Exception("No POST data received.");
    }

    // Decode waypoints if needed
    if (isset($data['igcWaypoints']) && is_string($data['igcWaypoints'])) {
        $data['igcWaypoints'] = json_decode($data['igcWaypoints'], true);
    }

    // 3) Open DB
    $pdo = new PDO("sqlite:$databasePath");
    $pdo->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);

    $foundTask = null;

    // If EntrySeqID has been forced - retrieve the PLNXML and nothing else
    $entrySeqID = isset($data['entrySeqID']) ? (int)$data['entrySeqID'] : 0;
    if ($entrySeqID > 0) {
        $stmt = $pdo->prepare("
            SELECT EntrySeqID, PLNXML
              FROM Tasks
             WHERE EntrySeqID = :EntrySeqID
        ");
        $stmt->bindValue(':EntrySeqID', $entrySeqID, PDO::PARAM_INT);
        $stmt->execute();
        $foundTask = $stmt->fetch(PDO::FETCH_ASSOC);
        if (!$foundTask) {
            throw new Exception("Forced task does not exist!");
        }
    }

    //
    // === STEP 1: Search by Title ===
    //
    if (!$foundTask) {
        if (!isset($data['igcTitle']) || !isset($data['igcWaypoints'])) {
            throw new Exception("Required parameters missing: igcTitle and igcWaypoints.");
        }
        $igcTitle     = trim($data['igcTitle']);
        $igcWaypoints = $data['igcWaypoints'];

        $stmt = $pdo->prepare(
            "SELECT EntrySeqID, PLNXML FROM Tasks WHERE PLNXML LIKE :titleClause"
        );
        $titleClause = '%<Title>' . $igcTitle . '</Title>%';
        $stmt->bindParam(':titleClause', $titleClause, PDO::PARAM_STR);
        $stmt->execute();
        $titleResults = $stmt->fetchAll(PDO::FETCH_ASSOC);

        if (!empty($titleResults)) {
            foreach ($titleResults as $cand) {
                if (validateCandidate($cand, $igcWaypoints)) {
                    $foundTask = $cand;
                    break;
                }
            }
        }
    }

    //
    // === STEP 2: If no title match, search by ALL waypoint IDs ===
    //
    if (!$foundTask) {
        $ids = array_column($igcWaypoints, 'id');
        $clauses = $params = [];
        foreach ($ids as $wpID) {
            $clauses[] = "PLNXML LIKE ?";
            $params[]  = '%<ATCWaypoint id="' . $wpID . '">%';
        }
        if (!empty($clauses)) {
            $sql  = "SELECT EntrySeqID, PLNXML FROM Tasks WHERE " . implode(' AND ', $clauses);
            $stmt = $pdo->prepare($sql);
            $stmt->execute($params);
            $wpResults = $stmt->fetchAll(PDO::FETCH_ASSOC);

            foreach ($wpResults as $cand) {
                if (validateCandidate($cand, $igcWaypoints)) {
                    $foundTask = $cand;
                    break;
                }
            }
        }
    }

    //
    // === STEP 3: If still not found, match by INTERIOR IDs only ===
    //
    if (!$foundTask) {
        $ids = array_column($igcWaypoints, 'id');
        if (count($ids) > 2) {
            $interior = array_slice($ids, 1, -1);
            $clauses = $params = [];
            foreach ($interior as $wpID) {
                $clauses[] = "PLNXML LIKE ?";
                $params[]  = '%<ATCWaypoint id="' . $wpID . '">%';
            }
            $sql  = "SELECT EntrySeqID, PLNXML FROM Tasks WHERE " . implode(' AND ', $clauses);
            $stmt = $pdo->prepare($sql);
            $stmt->execute($params);
            $cands = $stmt->fetchAll(PDO::FETCH_ASSOC);

            foreach ($cands as $cand) {
                if (validateCandidate($cand, $igcWaypoints)) {
                    $foundTask = $cand;
                    break;
                }
            }
        }
    }

    // 4) Return JSON
    if ($foundTask) {
        echo json_encode([
            'status'     => 'found',
            'EntrySeqID' => $foundTask['EntrySeqID'],
            'PLNXML' => $foundTask['PLNXML']
        ]);
    } else {
        echo json_encode([
            'status' => 'not_found'
        ]);
    }
}
catch (Exception $e) {
    echo json_encode([
        'status'  => 'error',
        'message' => $e->getMessage()
    ]);
    exit;
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
        return false;
    }
    $difference = abs($dec1 - $dec2);
    return $difference <= $tolerance;
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

    // Build human list of IGC fixes
    $igcList = array_map(fn($w)=> "{$w['id']}=>{$w['coord']}", $igcWaypoints);
    // logMessage("validateCandidate: IGC waypoints: " . implode(', ', $igcList));

    if (empty($candidate['PLNXML'])) {
        // logMessage("validateCandidate: Missing PLNXML");
        return false;
    }

    // Parse the PLNXML
    libxml_use_internal_errors(true);
    $xml = simplexml_load_string($candidate['PLNXML']);
    if (!$xml) {
        // foreach (libxml_get_errors() as $err) {
        //     logMessage("validateCandidate: XML parse error: " . trim($err->message));
        // }
        libxml_clear_errors();
        return false;
    }

    // Extract all ATCWaypoint elements in order
    $nodes = $xml->xpath('/SimBase.Document/FlightPlan.FlightPlan/ATCWaypoint');
    if (!$nodes) {
        // logMessage("validateCandidate: No ATCWaypoint elements in PLNXML");
        return false;
    }

    // Build two structures:
    // 1) $orderedXmlPos[i] = "lat,lon" for waypoint at index i
    // 2) $xmlMapById[name] = "lat,lon" for lookup by id (interior fixes)
    $orderedXmlPos = [];
    $xmlMapById    = [];
    $dump = [];
    foreach ($nodes as $i => $wp) {
        $nameAttr = (string)$wp['id'];
        $rawPos   = trim((string)$wp->WorldPosition);
        $norm     = normalizeXmlCoordinate($rawPos);
        $pos      = $norm ? "{$norm[0]},{$norm[1]}" : $rawPos;

        $orderedXmlPos[$i]   = $pos;
        $xmlMapById[$nameAttr] = $pos;
        $dump[] = "{$i}:{$nameAttr}=>{$pos}";
    }
    // logMessage("validateCandidate: PLNXML waypoints by index|id: " . implode(' | ', $dump));

    $n = count($igcWaypoints);
    // Compare each IGC fix to the corresponding XML position
    foreach ($igcWaypoints as $i => $wp) {
        $wpID    = $wp['id'];
        $igcCoord= $wp['coord'];
        // logMessage("validateCandidate: Checking IGC waypoint #{$i} ID='{$wpID}', Coord='{$igcCoord}'");

        // First or last? use position by index, skip id lookup
        if ($i === 0 || $i === $n - 1) {
            $xmlPos = $orderedXmlPos[$i];
            // logMessage("validateCandidate: First/last fix—using XML index {$i} => {$xmlPos}");
        } else {
            // interior: must exist by id
            if (!isset($xmlMapById[$wpID])) {
                // logMessage("validateCandidate: Interior waypoint '{$wpID}' not found by id");
                return false;
            }
            $xmlPos = $xmlMapById[$wpID];
            // logMessage("validateCandidate: Matched interior '{$wpID}' => {$xmlPos}");
        }

        // Split lat/lon from XML
        [$xmlLat, $xmlLon] = explode(',', $xmlPos) + [null, null];
        if ($xmlLat === null || $xmlLon === null) {
            // logMessage("validateCandidate: Invalid XML coords for '{$wpID}': {$xmlPos}");
            return false;
        }
        $xmlLat = trim($xmlLat);
        $xmlLon = trim($xmlLon);

        // Parse IGC coords (strip spaces)
        $parts = explode(',', str_replace(' ', '', $igcCoord));
        if (count($parts) < 2) {
            // logMessage("validateCandidate: Invalid IGC coord for '{$wpID}': {$igcCoord}");
            return false;
        }
        [$igcLat, $igcLon] = array_map('trim', $parts);

        // Compare latitude
        // logMessage("validateCandidate: Comparing LAT '{$wpID}' — IGC={$igcLat} vs XML={$xmlLat}");
        $latOk = compareCoordinates($igcLat, $xmlLat);

        // Compare longitude
        // logMessage("validateCandidate: Comparing LON '{$wpID}' — IGC={$igcLon} vs XML={$xmlLon}");
        $lonOk = compareCoordinates($igcLon, $xmlLon);

        if (!($latOk && $lonOk)) {
            // logMessage("validateCandidate: Mismatch for '{$wpID}': latOk=" . ($latOk?'true':'false') . ", lonOk=" . ($lonOk?'true':'false'));
            return false;
        }

        // logMessage("validateCandidate: Waypoint #{$i} '{$wpID}' matches successfully");
    }

    // logMessage("validateCandidate: *** ALL WAYPOINTS MATCH for EntrySeqID {$entrySeq} ***");
    return true;
}
?>
