<?php
// ─────────────────────────────────────────────────────────────────────────────
// Port of Task/Waypoint/Coordinate/Line + Nav helpers from your C# code.
// Parity preserved per your confirmations (1–10). See notes at bottom.
// ─────────────────────────────────────────────────────────────────────────────

class Coordinate {
    public float $Latitude = 0.0;
    public float $Longitude = 0.0;
    /** [m] MSL */
    public float $Altitude = 0.0;
}

class Line {
    public Coordinate $A;
    public Coordinate $B;
    public function __construct(Coordinate $A, Coordinate $B) {
        $this->A = $A; $this->B = $B;
    }
}

class Waypoint {
    public string $Name = '';
    public string $Type = '';
    public string $ICAO = '';
    public string $Runway = '';
    public Coordinate $Location;
    /** AAT min location */
    public Coordinate $LocationMin;
    /** AAT max location */
    public Coordinate $LocationMax;
    /** real turnpoint for AAT tasks */
    public Coordinate $LocationReal;
    /** [m] */
    public float $Elevation = 0.0;
    /** [m] */
    public float $Radius = 0.0;
    /** [m] */
    public float $MinAlt = 0.0;
    /** [m] */
    public float $MaxAlt = 0.0;
    public bool $IsStart = false;
    public bool $IsFinish = false;
    public bool $IsAATWP = false;
    public bool $IsInAATRadius = false;
    public bool $IsDone = false;
    /** Distance to this WP [m] */
    public float $DistanceTo = 0.0;
    /** Bearing to this WP [°] */
    public float $BearingTo = 0.0;

    public function __construct() {
        $this->Location = new Coordinate();
        $this->LocationMin = new Coordinate();
        $this->LocationMax = new Coordinate();
        $this->LocationReal = new Coordinate();
    }
}

final class Nav {
    public static float $EarthRadiusMeters = 6366710.0; // keep mismatch per C# (GetPointAtBearing uses 6378137)

    /** convert degree,minutes,seconds coordinate to decimal coordinate */
    public static function ConvertDMSToDecimal(string $dms): float {
        try {
            // Split on ° ' "
            $parts = preg_split('/[°\'"]/u', $dms, -1, PREG_SPLIT_NO_EMPTY);
            if (!$parts || count($parts) < 3) return 0.0;

            $deg = (float)preg_replace('/[^\d.]/', '', $parts[0]);
            $min = (float)preg_replace('/[^\d.]/', '', $parts[1]);
            $sec = (float)trim($parts[2]);

            $decimal = $deg + ($min / 60.0) + ($sec / 3600.0);
            $hem = mb_substr($dms, 0, 1, 'UTF-8');
            if ($hem === 'S' || $hem === 'W') $decimal *= -1.0;
            return $decimal;
        } catch (\Throwable $e) {
            return 0.0;
        }
    }

    /** Parse first digits (with optional decimals) from string */
    public static function GetFirstDigits(?string $text): float {
        if (!$text || trim($text) === '') return 0.0;
        if (preg_match('/^\d+(\.\d+)?/', $text, $m)) {
            return (float)$m[0];
        }
        return 0.0;
    }

    /** ft to m */
    public static function ftToM(float $ft): float { return $ft / 3.28084; }

    public static function DegreesToRadians(float $deg): float { return $deg * M_PI / 180.0; }
    public static function RadiansToDegrees(float $rad): float { return $rad * (180.0 / M_PI); }

    /** returns seconds from time string like "3:44" (no seconds part by spec) */
    public static function GetSecondsFromTime(?string $text): int {
        if (!$text || trim($text) === '') return 0;
        $p = explode(':', $text);
        $h = 0; $m = 0;
        $h = (int)($p[0] ?? 0);
        if (count($p) > 1) $m = (int)$p[1];
        return $h * 3600 + $m * 60;
    }

    /** [m] Haversine distance */
    public static function GetDistanceM(Coordinate $p1, Coordinate $p2): float {
        try {
            $dLat = self::DegreesToRadians($p2->Latitude - $p1->Latitude);
            $dLon = self::DegreesToRadians($p2->Longitude - $p1->Longitude);
            $lat1 = self::DegreesToRadians($p1->Latitude);
            $lat2 = self::DegreesToRadians($p2->Latitude);

            $a = sin($dLat/2)*sin($dLat/2)
               + cos($lat1)*cos($lat2)*sin($dLon/2)*sin($dLon/2);
            $c = 2 * atan2(sqrt($a), sqrt(1-$a));
            return self::$EarthRadiusMeters * $c;
        } catch (\Throwable $e) {
            return 0.0;
        }
    }

    /** Bearing in degrees from p1 -> p2 (0..360) */
    public static function GetBearingDeg(Coordinate $p1, Coordinate $p2): float {
        $lat1 = self::DegreesToRadians($p1->Latitude);
        $lon1 = self::DegreesToRadians($p1->Longitude);
        $lat2 = self::DegreesToRadians($p2->Latitude);
        $lon2 = self::DegreesToRadians($p2->Longitude);

        $dLon = $lon2 - $lon1;
        $y = sin($dLon) * cos($lat2);
        $x = cos($lat1)*sin($lat2) - sin($lat1)*cos($lat2)*cos($dLon);
        $bearing = self::RadiansToDegrees(atan2($y, $x));
        return fmod(($bearing + 360.0), 360.0);
    }

    /** Given inbound/outbound bearings, returns bisector pointing AWAY from the WP */
    public static function GetBisector(float $track_in, float $track_out): float {
        $bis = ($track_in + $track_out) / 2.0 + 90.0;
        $offset = ($bis > $track_in) ? ($bis - $track_in) : ($track_in - $bis);
        if ($offset > 90.0 && $offset < 270.0) $bis += 180.0;
        if ($bis >= 360.0) $bis -= 360.0;
        return $bis;
    }

    /** Perpendicular distance of point P to segment [A,B] (meters) */
    public static function GetDistanceToLine_m(Coordinate $P, Line $line): float {
        $R = self::GetEarthRadius_m($P->Latitude);
        $bearing_AP = self::GetBearingDeg($line->A, $P);
        $bearing_AB = self::GetBearingDeg($line->A, $line->B);
        $bearing_BP = self::GetBearingDeg($line->B, $P);

        $angle_BAP = fmod(($bearing_AP - $bearing_AB + 360.0), 360.0);
        if ($angle_BAP > 90.0 && $angle_BAP < 270.0) return self::GetDistanceM($line->A, $P);

        $angle_ABP = fmod((180.0 - $bearing_BP + $bearing_AB + 360.0), 360.0);
        if ($angle_ABP > 90.0 && $angle_ABP < 270.0) return self::GetDistanceM($line->B, $P);

        $distance_AB = self::GetDistanceM($line->A, $P);
        $d = abs(asin(sin($distance_AB / $R) * sin(self::DegreesToRadians($bearing_AP - $bearing_AB))) * $R);
        return $d;
    }

    /** Earth radius estimate for given latitude (meters) */
    public static function GetEarthRadius_m(float $latitude): float {
        $R1 = 6378.137; // km equator
        $R2 = 6356.752; // km poles
        $B  = self::DegreesToRadians($latitude);
        $sinB = sin($B); $cosB = cos($B);
        $term1 = pow(pow($R1,2)*$cosB, 2) + pow(pow($R2,2)*$sinB, 2);
        $term2 = pow($R1*$cosB, 2) + pow($R2*$sinB, 2);
        return sqrt($term1 / $term2) * 1000.0;
    }

    /** Destination point given start lat/lon, distance (m), bearing (deg) */
    public static function GetPointAtBearing(float $latitude, float $longitude, float $distance, float $bearing): Coordinate {
        $EarthRadius = 6378137.0; // keep as in C#
        $radHeading = self::DegreesToRadians($bearing);
        $radLat = self::DegreesToRadians($latitude);
        $radLon = self::DegreesToRadians($longitude);

        $newLat = asin(sin($radLat) * cos($distance / $EarthRadius)
                 + cos($radLat) * sin($distance / $EarthRadius) * cos($radHeading));

        $newLon = $radLon + atan2(
            sin($radHeading) * sin($distance / $EarthRadius) * cos($radLat),
            cos($distance / $EarthRadius) - sin($radLat) * sin($newLat)
        );

        $c = new Coordinate();
        $c->Latitude  = self::RadiansToDegrees($newLat);
        $c->Longitude = self::RadiansToDegrees($newLon);
        return $c;
    }
}

class Task {
    /** @var Waypoint[] */
    public array $Waypoints = [];
    public int $DefaultWaypointRadius = 500;
    public string $Name = '';
    public string $GN = '';
    public int $StartWPIndex = -1;
    public int $NextWPIndex  = 0;
    public int $FinishWPIndex = -1;
    public bool $IsAATTask = false;
    public int $TimeMin = 0;
    public float $DistanceAATMin = 0.0;
    public float $DistanceAATMax = 0.0;
    public float $Distance = 0.0;
    public string $DepartureID = '';
    public string $DepartureName = '';
    public string $DeparturePosition = '';
    public string $DestinationID = '';
    public string $DestinationName = '';
    public bool $IsEmpty = true;

    public function __construct(string $TaskData, string $GroupName) {
        $this->Clear();
        $this->Waypoints = [];
        $this->ParseFlightPlan($TaskData);
        $this->GN = $GroupName;
    }

    public function Clear(): void {
        $this->Waypoints = [];
        $this->Name = '';
        $this->GN = '';
        $this->IsAATTask = false;
        $this->DepartureID = '';
        $this->DepartureName = '';
        $this->DeparturePosition = '';
        $this->DestinationID = '';
        $this->DestinationName = '';
        $this->Distance = 0.0;
        $this->DistanceAATMax = 0.0;
        $this->DistanceAATMin = 0.0;
        $this->IsEmpty = true;
        $this->StartWPIndex = -1;
        $this->NextWPIndex = 0;
        $this->FinishWPIndex = -1;
        $this->TimeMin = 0;
        $this->IsAATTask = false;
    }

    public function ParseFlightPlan(string $plnContent): void {
        if (trim($plnContent) === '') return;

        // SimpleXML (no namespaces per your sample)
        $xml = @simplexml_load_string($plnContent);
        if (!$xml) return;

        $fp = $xml->{'FlightPlan.FlightPlan'} ?? null;

        $this->Name             = (string)($fp->Title ?? '');
        $this->DepartureID      = (string)($fp->DepartureID ?? '');
        $this->DepartureName    = (string)($fp->DepartureName ?? '');
        $this->DeparturePosition= (string)($fp->DeparturePosition ?? '');
        $this->DestinationID    = (string)($fp->DestinationID ?? '');
        $this->DestinationName  = (string)($fp->DestinationName ?? '');

        // Collect ATCWaypoint nodes
        $wps = [];
        if ($fp && isset($fp->ATCWaypoint)) {
            foreach ($fp->ATCWaypoint as $wpNode) $wps[] = $wpNode;
        } else {
            // fallback: find anywhere
            foreach ($xml->xpath('//ATCWaypoint') ?: [] as $wpNode) $wps[] = $wpNode;
        }

        foreach ($wps as $wpNode) {
            $newWP = new Waypoint();

            // Defensive default for id
            $id = isset($wpNode['id']) ? (string)$wpNode['id'] : '';

            $newWP->Type   = (string)($wpNode->ATCWaypointType ?? '');
            $newWP->Runway = (string)($wpNode->RunwayNumberFP ?? '');

            $worldPos = (string)($wpNode->WorldPosition ?? '');
            if ($worldPos !== '') {
                $parts = array_map('trim', explode(',', $worldPos));
                if (count($parts) > 1) {
                    $lat = Nav::ConvertDMSToDecimal($parts[0]);
                    $lon = Nav::ConvertDMSToDecimal($parts[1]);

                    $newWP->Location->Latitude  = $lat;
                    $newWP->Location->Longitude = $lon;

                    $newWP->LocationReal->Latitude  = $lat;
                    $newWP->LocationReal->Longitude = $lon;

                    $newWP->LocationMin->Latitude   = $lat;
                    $newWP->LocationMin->Longitude  = $lon;

                    $newWP->LocationMax->Latitude   = $lat;
                    $newWP->LocationMax->Longitude  = $lon;

                    $newWP->Elevation = (count($parts) > 2)
                        ? Nav::ftToM(Nav::GetFirstDigits(str_replace('+', '', $parts[2])))
                        : 0.0;
                }
                // else: TODO Get location with ICAO (skip confirmed)
            }

            // ICAO
            if (isset($wpNode->ICAO->ICAOIdent)) {
                $newWP->ICAO = (string)$wpNode->ICAO->ICAOIdent;
            } else {
                $newWP->ICAO = '';
            }

            // Parse ID (custom grammar)
            if (strlen($id) > 0 && $id[0] === '*') {
                if ($this->StartWPIndex <= 0) {
                    $newWP->IsStart = true;
                    $this->StartWPIndex = count($this->Waypoints);
                    $this->NextWPIndex  = count($this->Waypoints);
                } else {
                    $newWP->IsFinish = true;
                    $this->FinishWPIndex = count($this->Waypoints);
                }
            }

            $p1 = explode('+', $id);
            $newWP->Name = str_replace('*', '', ($p1[0] ?? ''));
            $endpart = '';

            // Elevation
            if (count($p1) > 1) {
                $endpart = $p1[1];
                $newWP->Elevation = Nav::ftToM(Nav::GetFirstDigits($p1[1]));
            }

            // Max Alt (|xxx)
            $p2 = explode('|', $endpart);
            if (count($p2) > 1) {
                $newWP->MaxAlt = Nav::ftToM(Nav::GetFirstDigits($p2[1]));
            }

            // Min Alt (/xxx)
            $p3 = explode('/', $endpart);
            if (count($p3) > 1) {
                $newWP->MinAlt = Nav::ftToM(Nav::GetFirstDigits($p3[1]));
            }

            // Radius (xN) — diameter/2 per your C#
            $p4 = explode('x', strtolower($endpart));
            if (count($p4) > 1) {
                $newWP->Radius = Nav::GetFirstDigits($p4[1]) / 2.0;
            }

            // AAT (;AATmm:ss), seconds not supported by spec (kept)
            $p5 = explode(';', $endpart);
            if (count($p5) > 1) {
                if (strpos(strtoupper($p5[1]), 'AAT') !== false) {
                    $this->IsAATTask = true;
                    if ($newWP->IsStart) {
                        $this->TimeMin = Nav::GetSecondsFromTime(str_replace('AAT', '', strtoupper($p5[1])));
                    } else {
                        $newWP->IsAATWP = true;
                    }
                }
            }

            // Leg distance & bearing from previous
            if (count($this->Waypoints) > 0) {
                $prev = $this->Waypoints[count($this->Waypoints) - 1];
                $newWP->DistanceTo = Nav::GetDistanceM($prev->Location, $newWP->Location);
                $newWP->BearingTo  = Nav::GetBearingDeg($prev->Location, $newWP->Location);
            }

            if (stripos($newWP->Name, 'TIME') !== 0) {
                $this->Waypoints[] = $newWP;
            } else {
                // skip TIME*
            }
        }

        // Default Start
        if ($this->StartWPIndex < 0) {
            $this->StartWPIndex = (count($this->Waypoints) > 2) ? 1 : 0;
            if (isset($this->Waypoints[$this->StartWPIndex])) {
                $this->Waypoints[$this->StartWPIndex]->IsStart = true;
            }
        }

        // Default Finish
        if ($this->FinishWPIndex < 0) {
            $this->FinishWPIndex = (count($this->Waypoints) > 3) ? (count($this->Waypoints) - 2) : (count($this->Waypoints) - 1);
            if ($this->FinishWPIndex >= 0 && isset($this->Waypoints[$this->FinishWPIndex])) {
                $this->Waypoints[$this->FinishWPIndex]->IsFinish = true;
            }
        }

        // Check Radius defaults
        for ($i = $this->StartWPIndex; $i <= $this->FinishWPIndex; $i++) {
            if (!isset($this->Waypoints[$i])) continue;
            if ($this->Waypoints[$i]->Radius <= 0) {
                $this->Waypoints[$i]->Radius = $this->DefaultWaypointRadius;
            }
        }

        $this->updateAatDistance();

        $this->DistanceAATMin = 0.0;
        $this->DistanceAATMax = 0.0;

        // Sum Task distance
        for ($i = $this->StartWPIndex + 1; $i <= $this->FinishWPIndex; $i++) {
            if (!isset($this->Waypoints[$i]) || !isset($this->Waypoints[$i - 1])) continue;

            $this->Distance += $this->Waypoints[$i]->DistanceTo;

            if ($this->IsAATTask) {
                $l1min = $this->Waypoints[$i - 1]->Location;
                $l1max = $this->Waypoints[$i - 1]->Location;
                $l2min = $this->Waypoints[$i]->Location;
                $l2max = $this->Waypoints[$i]->Location;

                if ($this->Waypoints[$i - 1]->IsAATWP) {
                    $l1min = $this->Waypoints[$i - 1]->LocationMin;
                    $l1max = $this->Waypoints[$i - 1]->LocationMax;
                }
                if ($this->Waypoints[$i]->IsAATWP) {
                    $l2min = $this->Waypoints[$i]->LocationMin;
                    $l2max = $this->Waypoints[$i]->LocationMax;
                }

                $this->DistanceAATMin += Nav::GetDistanceM($l1min, $l2min);
                $this->DistanceAATMax += Nav::GetDistanceM($l1max, $l2max);
            }
        }

        // TODO MSFS 2024 Workaround (parity kept, including original condition shape)
        if ($this->StartWPIndex > 0) {
            $prevIdx = $this->StartWPIndex - 1;
            if (isset($this->Waypoints[$prevIdx], $this->Waypoints[$this->StartWPIndex])) {
                if ($this->Waypoints[$prevIdx]->Location->Latitude == 0.0) {
                    $this->Waypoints[$prevIdx]->Location->Latitude  = $this->Waypoints[$this->StartWPIndex]->Location->Latitude;
                    $this->Waypoints[$prevIdx]->Location->Longitude = $this->Waypoints[$this->StartWPIndex]->Location->Longitude;
                    $this->Waypoints[$prevIdx]->LocationReal->Longitude = $this->Waypoints[$this->StartWPIndex]->Location->Longitude;
                    $this->Waypoints[$prevIdx]->LocationReal->Latitude  = $this->Waypoints[$this->StartWPIndex]->Location->Latitude;
                }
            }
        }

        if (count($this->Waypoints) >= $this->FinishWPIndex) {
            $idx = $this->FinishWPIndex + 1;
            if (isset($this->Waypoints[$idx], $this->Waypoints[$this->FinishWPIndex])) {
                if ($this->Waypoints[$idx]->Location->Latitude == 0.0) {
                    $this->Waypoints[$idx]->Location->Latitude  = $this->Waypoints[$this->FinishWPIndex]->Location->Latitude;
                    $this->Waypoints[$idx]->Location->Longitude = $this->Waypoints[$this->FinishWPIndex]->Location->Longitude;
                    $this->Waypoints[$idx]->LocationReal->Longitude = $this->Waypoints[$this->FinishWPIndex]->Location->Longitude;
                    $this->Waypoints[$idx]->LocationReal->Latitude  = $this->Waypoints[$this->FinishWPIndex]->Location->Latitude;
                }
            }
        }

        $this->IsEmpty = (count($this->Waypoints) < 2);
    }

    private function updateAatDistance(): void {
        if (!$this->IsAATTask || count($this->Waypoints) < 3) return;

        $MAX_ITERATIONS = 5; // parity
        for ($i = 0; $i < $MAX_ITERATIONS; $i++) {
            for ($wpIndex = $this->StartWPIndex + 1; $wpIndex < $this->FinishWPIndex; $wpIndex++) {
                if (!isset($this->Waypoints[$wpIndex-1], $this->Waypoints[$wpIndex], $this->Waypoints[$wpIndex+1])) continue;

                $wp = $this->Waypoints[$wpIndex];
                if ($wp->IsAATWP) {
                    // MIN
                    $p0 = $this->Waypoints[$wpIndex - 1]->LocationMin;
                    $p1 = $this->Waypoints[$wpIndex + 1]->LocationMin;
                    $track_in = Nav::GetBearingDeg($p0, $wp->Location);
                    $track_out= Nav::GetBearingDeg($wp->Location, $p1);
                    $bisector = fmod(Nav::GetBisector($track_in, $track_out) + 180.0, 360.0);
                    $minDist  = Nav::GetDistanceToLine_m($wp->Location, new Line($p0, $p1));
                    $minDist  = min($minDist, $wp->Radius);
                    $wp->LocationMin = Nav::GetPointAtBearing($wp->Location->Latitude, $wp->Location->Longitude, $minDist, $bisector);

                    // MAX
                    $p0 = $this->Waypoints[$wpIndex - 1]->LocationMax;
                    $p1 = $this->Waypoints[$wpIndex + 1]->LocationMax;
                    $track_in = Nav::GetBearingDeg($p0, $wp->Location);
                    $track_out= Nav::GetBearingDeg($wp->Location, $p1);
                    $bisector = Nav::GetBisector($track_in, $track_out);
                    $wp->LocationMax = Nav::GetPointAtBearing($wp->Location->Latitude, $wp->Location->Longitude, $wp->Radius, $bisector);
                }
            }
        }
    }
}
