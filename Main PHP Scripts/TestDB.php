<?php
require __DIR__ . '/CommonFunctions.php';

try {
    // Open the database connection
    $pdo = new PDO("sqlite:$databasePath");
    $pdo->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);

    // Check if the WorldMapInfo table exists
    $checkQuery = "
        SELECT name
        FROM sqlite_master
        WHERE type='table' AND name='WorldMapInfo';
    ";
    $result = $pdo->query($checkQuery)->fetch(PDO::FETCH_ASSOC);

    if ($result) {
        // WorldMapInfo table exists, perform the merging operations
        $combineSQL = "
            -- Step 1: Create the new combined table
            CREATE TABLE IF NOT EXISTS Tasks_Combined (
                EntrySeqID INTEGER UNIQUE,
                TaskID TEXT NOT NULL UNIQUE,
                Title TEXT NOT NULL,
                LastUpdate TEXT NOT NULL,
                SimDateTime TEXT NOT NULL,
                IncludeYear INTEGER NOT NULL CHECK(IncludeYear IN (0, 1)),
                SimDateTimeExtraInfo TEXT,
                MainAreaPOI TEXT,
                DepartureName TEXT,
                DepartureICAO TEXT,
                DepartureExtra TEXT,
                ArrivalName TEXT,
                ArrivalICAO TEXT,
                ArrivalExtra TEXT,
                SoaringRidge INTEGER NOT NULL CHECK(SoaringRidge IN (0, 1)),
                SoaringThermals INTEGER NOT NULL CHECK(SoaringThermals IN (0, 1)),
                SoaringWaves INTEGER NOT NULL CHECK(SoaringWaves IN (0, 1)),
                SoaringDynamic INTEGER NOT NULL CHECK(SoaringDynamic IN (0, 1)),
                SoaringExtraInfo TEXT,
                DurationMin INTEGER,
                DurationMax INTEGER,
                DurationExtraInfo TEXT,
                TaskDistance INTEGER NOT NULL,
                TotalDistance INTEGER NOT NULL,
                RecommendedGliders TEXT,
                DifficultyRating TEXT,
                DifficultyExtraInfo TEXT,
                ShortDescription TEXT,
                LongDescription TEXT,
                WeatherSummary TEXT,
                Credits TEXT,
                Countries TEXT,
                RecommendedAddOns INTEGER NOT NULL CHECK(RecommendedAddOns IN (0, 1)),
                MapImage BLOB,
                CoverImage BLOB,
                TotDownloads INTEGER DEFAULT 0,
                LastDownloadUpdate TEXT DEFAULT '2000-01-01 00:00:00',
                DBEntryUpdate TEXT,
                ThreadAccess INTEGER DEFAULT 0,
                PLNFilename TEXT,
                PLNXML TEXT,
                WPRFilename TEXT,
                WPRXML TEXT,
                LatMin REAL NOT NULL,
                LatMax REAL NOT NULL,
                LongMin REAL NOT NULL,
                LongMax REAL NOT NULL,
                PRIMARY KEY(EntrySeqID AUTOINCREMENT)
            );

            -- Step 2: Copy data from old tables to the new combined table
            INSERT INTO Tasks_Combined (
                EntrySeqID, TaskID, Title, LastUpdate, SimDateTime, IncludeYear,
                SimDateTimeExtraInfo, MainAreaPOI, DepartureName, DepartureICAO, 
                DepartureExtra, ArrivalName, ArrivalICAO, ArrivalExtra, SoaringRidge,
                SoaringThermals, SoaringWaves, SoaringDynamic, SoaringExtraInfo,
                DurationMin, DurationMax, DurationExtraInfo, TaskDistance, TotalDistance,
                RecommendedGliders, DifficultyRating, DifficultyExtraInfo, ShortDescription,
                LongDescription, WeatherSummary, Credits, Countries, RecommendedAddOns,
                MapImage, CoverImage, TotDownloads, LastDownloadUpdate, DBEntryUpdate, ThreadAccess,
                PLNFilename, PLNXML, WPRFilename, WPRXML, LatMin, LatMax, LongMin, LongMax
            )
            SELECT 
                t.EntrySeqID, t.TaskID, t.Title, t.LastUpdate, t.SimDateTime, t.IncludeYear,
                t.SimDateTimeExtraInfo, t.MainAreaPOI, t.DepartureName, t.DepartureICAO, 
                t.DepartureExtra, t.ArrivalName, t.ArrivalICAO, t.ArrivalExtra, t.SoaringRidge,
                t.SoaringThermals, t.SoaringWaves, t.SoaringDynamic, t.SoaringExtraInfo,
                t.DurationMin, t.DurationMax, t.DurationExtraInfo, t.TaskDistance, t.TotalDistance,
                t.RecommendedGliders, t.DifficultyRating, t.DifficultyExtraInfo, t.ShortDescription,
                t.LongDescription, t.WeatherSummary, t.Credits, t.Countries, t.RecommendedAddOns,
                t.MapImage, t.CoverImage, t.TotDownloads, t.LastDownloadUpdate, t.DBEntryUpdate, t.ThreadAccess,
                w.PLNFilename, w.PLNXML, w.WPRFilename, w.WPRXML, w.LatMin, w.LatMax, w.LongMin, w.LongMax
            FROM Tasks t
            JOIN WorldMapInfo w ON t.TaskID = w.TaskID;

            -- Step 3: Drop the old tables
            DROP TABLE Tasks;
            DROP TABLE WorldMapInfo;

            -- Step 4: Rename the new combined table to Tasks
            ALTER TABLE Tasks_Combined RENAME TO Tasks;
        ";

        // Execute the SQL to combine tables
        $pdo->exec($combineSQL);
        echo "Tables combined successfully.\n";
    } else {
        echo "Table WorldMapInfo does not exist. No changes made.\n";
    }
} catch (PDOException $e) {
    echo "Update failed: " . $e->getMessage();
}
?>
