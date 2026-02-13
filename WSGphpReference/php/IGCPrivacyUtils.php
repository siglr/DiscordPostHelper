<?php

function privacyBindNullableString(PDOStatement $stmt, string $parameter, $value): void
{
    if ($value === null) {
        $stmt->bindValue($parameter, null, PDO::PARAM_NULL);
    } else {
        $stmt->bindValue($parameter, $value, PDO::PARAM_STR);
    }
}

function findBestPublicEligiblePerformance(PDO $pdo, int $entrySeqId, string $sim, $competitionClass, $gliderType): ?array
{
    $bestQuery = "
        SELECT
            IGC.IGCKey,
            IGC.Speed,
            IGC.IGCUploadDateTimeUTC
        FROM IGCRecords IGC
        JOIN Tasks T ON IGC.EntrySeqID = T.EntrySeqID
        WHERE IGC.EntrySeqID = :EntrySeqID
          AND IGC.Sim = :Sim
          AND COALESCE(IGC.IsPrivate, 0) = 0
          AND ((IGC.CompetitionClass IS NULL AND :CompetitionClass IS NULL) OR IGC.CompetitionClass = :CompetitionClass)
          AND ((IGC.GliderType IS NULL AND :GliderType IS NULL) OR IGC.GliderType = :GliderType)
          AND IGC.TaskCompleted = 1
          AND IGC.IGCValid = 1
          AND COALESCE(IGC.MarkedAsDesigner, 0) <> 1
          AND abs(strftime('%s', T.SimDateTime) - strftime(
                '%s',
                substr(T.SimDateTime, 1, 4) || '-' ||
                substr(IGC.LocalDate, 6, 5) || ' ' ||
                substr(IGC.LocalTime, 1, 2) || ':' ||
                substr(IGC.LocalTime, 3, 2) || ':' ||
                substr(IGC.LocalTime, 5, 2)
          )) <= 1800
        ORDER BY IGC.Speed DESC, IGC.IGCUploadDateTimeUTC ASC
    ";

    $stmt = $pdo->prepare($bestQuery);
    $stmt->bindValue(':EntrySeqID', $entrySeqId, PDO::PARAM_INT);
    $stmt->bindValue(':Sim', $sim, PDO::PARAM_STR);
    privacyBindNullableString($stmt, ':CompetitionClass', $competitionClass);
    privacyBindNullableString($stmt, ':GliderType', $gliderType);
    $stmt->execute();

    $bestRow = null;
    $bestRoundedSpeed = null;

    while ($row = $stmt->fetch(PDO::FETCH_ASSOC)) {
        if ($row['Speed'] === null) {
            continue;
        }

        $roundedSpeed = (float) number_format((float) $row['Speed'], 1, '.', '');

        if ($bestRow === null || $roundedSpeed > $bestRoundedSpeed) {
            $bestRow = $row;
            $bestRoundedSpeed = $roundedSpeed;
            continue;
        }

        if ($roundedSpeed === $bestRoundedSpeed) {
            $currentUpload = $row['IGCUploadDateTimeUTC'] ?? null;
            $bestUpload = $bestRow['IGCUploadDateTimeUTC'] ?? null;

            if ($currentUpload !== null && ($bestUpload === null || strcmp($currentUpload, $bestUpload) < 0)) {
                $bestRow = $row;
                $bestRoundedSpeed = $roundedSpeed;
            }
        }
    }

    return $bestRow;
}

function refreshTaskBestPerformanceGroup(PDO $pdo, int $entrySeqId, string $sim, $competitionClass, $gliderType): bool
{
    $selectBestQuery = "
        SELECT IGCKey
        FROM TaskBestPerformances
        WHERE EntrySeqID = :EntrySeqID
          AND Sim = :Sim
          AND ((CompetitionClass IS NULL AND :CompetitionClass IS NULL) OR CompetitionClass = :CompetitionClass)
          AND ((GliderType IS NULL AND :GliderType IS NULL) OR GliderType = :GliderType)
        LIMIT 1
    ";

    $stmt = $pdo->prepare($selectBestQuery);
    $stmt->bindValue(':EntrySeqID', $entrySeqId, PDO::PARAM_INT);
    $stmt->bindValue(':Sim', $sim, PDO::PARAM_STR);
    privacyBindNullableString($stmt, ':CompetitionClass', $competitionClass);
    privacyBindNullableString($stmt, ':GliderType', $gliderType);
    $stmt->execute();
    $existingBest = $stmt->fetch(PDO::FETCH_ASSOC);

    $bestEligible = findBestPublicEligiblePerformance($pdo, $entrySeqId, $sim, $competitionClass, $gliderType);

    if ($bestEligible === null) {
        if (!$existingBest) {
            return false;
        }

        $deleteQuery = "
            DELETE FROM TaskBestPerformances
            WHERE EntrySeqID = :EntrySeqID
              AND Sim = :Sim
              AND ((CompetitionClass IS NULL AND :CompetitionClass IS NULL) OR CompetitionClass = :CompetitionClass)
              AND ((GliderType IS NULL AND :GliderType IS NULL) OR GliderType = :GliderType)
        ";
        $stmt = $pdo->prepare($deleteQuery);
        $stmt->bindValue(':EntrySeqID', $entrySeqId, PDO::PARAM_INT);
        $stmt->bindValue(':Sim', $sim, PDO::PARAM_STR);
        privacyBindNullableString($stmt, ':CompetitionClass', $competitionClass);
        privacyBindNullableString($stmt, ':GliderType', $gliderType);
        $stmt->execute();

        return true;
    }

    $bestKey = $bestEligible['IGCKey'];

    if (!$existingBest) {
        $insertQuery = "
            INSERT INTO TaskBestPerformances (EntrySeqID, Sim, CompetitionClass, GliderType, IGCKey)
            VALUES (:EntrySeqID, :Sim, :CompetitionClass, :GliderType, :IGCKey)
        ";

        $stmt = $pdo->prepare($insertQuery);
        $stmt->bindValue(':EntrySeqID', $entrySeqId, PDO::PARAM_INT);
        $stmt->bindValue(':Sim', $sim, PDO::PARAM_STR);
        privacyBindNullableString($stmt, ':CompetitionClass', $competitionClass);
        privacyBindNullableString($stmt, ':GliderType', $gliderType);
        $stmt->bindValue(':IGCKey', $bestKey, PDO::PARAM_STR);
        $stmt->execute();

        return true;
    }

    if ($existingBest['IGCKey'] === $bestKey) {
        return false;
    }

    $updateQuery = "
        UPDATE TaskBestPerformances
        SET IGCKey = :IGCKey
        WHERE EntrySeqID = :EntrySeqID
          AND Sim = :Sim
          AND ((CompetitionClass IS NULL AND :CompetitionClass IS NULL) OR CompetitionClass = :CompetitionClass)
          AND ((GliderType IS NULL AND :GliderType IS NULL) OR GliderType = :GliderType)
    ";

    $stmt = $pdo->prepare($updateQuery);
    $stmt->bindValue(':IGCKey', $bestKey, PDO::PARAM_STR);
    $stmt->bindValue(':EntrySeqID', $entrySeqId, PDO::PARAM_INT);
    $stmt->bindValue(':Sim', $sim, PDO::PARAM_STR);
    privacyBindNullableString($stmt, ':CompetitionClass', $competitionClass);
    privacyBindNullableString($stmt, ':GliderType', $gliderType);
    $stmt->execute();

    return true;
}

function buildGroupKey(int $entrySeqID, string $sim, $competitionClass, $gliderType): string
{
    $classValue = $competitionClass === null ? '__NULL__' : (string) $competitionClass;
    $gliderValue = $gliderType === null ? '__NULL__' : (string) $gliderType;

    return $entrySeqID . '|' . $sim . '|' . $classValue . '|' . $gliderValue;
}
