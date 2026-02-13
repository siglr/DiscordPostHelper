<?php

function refreshFastestGliderSpeedsCache(PDO $pdo): bool
{
    global $homeLeaderboardCacheDir;

    $otherdataDir = rtrim(dirname(rtrim((string) $homeLeaderboardCacheDir, '/')), '/') . '/';

    $stmt = $pdo->query(" 
        SELECT
            e.GliderType AS GliderType,
            e.Speed AS Speed,
            e.Pilot AS Pilot,
            e.Sim AS Sim,
            e.EntrySeqID AS EntrySeqID,
            t.Title AS TaskTitle,
            e.IGCUploadDateTimeUTC AS IGCUploadDateTimeUTC
        FROM (
            SELECT
                IGC.IGCKey,
                IGC.EntrySeqID,
                IGC.GliderType,
                IGC.Pilot,
                IGC.Speed,
                IGC.Sim,
                IGC.IGCUploadDateTimeUTC
            FROM IGCRecords IGC
            JOIN Tasks T ON IGC.EntrySeqID = T.EntrySeqID
            WHERE
                IGC.IGCValid = 1
                AND IGC.TaskCompleted = 1
                AND COALESCE(IGC.IsPrivate, 0) = 0
                AND COALESCE(IGC.MarkedAsDesigner, 0) <> 1
                AND IGC.GliderType IS NOT NULL
                AND TRIM(IGC.GliderType) <> ''
                AND IGC.Speed IS NOT NULL
                AND abs(strftime('%s', T.SimDateTime) - strftime(
                        '%s',
                        substr(T.SimDateTime, 1, 4) || '-' ||
                        substr(IGC.LocalDate, 6, 5) || ' ' ||
                        substr(IGC.LocalTime, 1, 2) || ':' ||
                        substr(IGC.LocalTime, 3, 2) || ':' ||
                        substr(IGC.LocalTime, 5, 2)
                )) <= 1800
        ) e
        JOIN Tasks t ON e.EntrySeqID = t.EntrySeqID
        WHERE NOT EXISTS (
            SELECT 1
            FROM (
                SELECT
                    IGC.IGCKey,
                    IGC.EntrySeqID,
                    IGC.GliderType,
                    IGC.Pilot,
                    IGC.Speed,
                    IGC.Sim,
                    IGC.IGCUploadDateTimeUTC
                FROM IGCRecords IGC
                JOIN Tasks T ON IGC.EntrySeqID = T.EntrySeqID
                WHERE
                    IGC.IGCValid = 1
                    AND IGC.TaskCompleted = 1
                    AND COALESCE(IGC.IsPrivate, 0) = 0
                    AND COALESCE(IGC.MarkedAsDesigner, 0) <> 1
                    AND IGC.GliderType IS NOT NULL
                    AND TRIM(IGC.GliderType) <> ''
                    AND IGC.Speed IS NOT NULL
                    AND abs(strftime('%s', T.SimDateTime) - strftime(
                            '%s',
                            substr(T.SimDateTime, 1, 4) || '-' ||
                            substr(IGC.LocalDate, 6, 5) || ' ' ||
                            substr(IGC.LocalTime, 1, 2) || ':' ||
                            substr(IGC.LocalTime, 3, 2) || ':' ||
                            substr(IGC.LocalTime, 5, 2)
                    )) <= 1800
            ) e2
            WHERE e2.GliderType = e.GliderType
              AND e2.Sim = e.Sim
              AND (
                e2.Speed > e.Speed
                OR (e2.Speed = e.Speed AND e2.IGCUploadDateTimeUTC < e.IGCUploadDateTimeUTC)
                OR (e2.Speed = e.Speed AND e2.IGCUploadDateTimeUTC = e.IGCUploadDateTimeUTC AND e2.IGCKey < e.IGCKey)
              )
        )
        ORDER BY e.Speed DESC, e.GliderType COLLATE NOCASE
    ");

    $fastestGliderResults = $stmt->fetchAll(PDO::FETCH_ASSOC);
    $jsonPathFastest = $otherdataDir . 'fastestGliderSpeeds.json';

    return file_put_contents(
        $jsonPathFastest,
        json_encode($fastestGliderResults, JSON_PRETTY_PRINT | JSON_UNESCAPED_UNICODE)
    ) !== false;
}
