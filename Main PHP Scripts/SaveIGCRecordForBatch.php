<?php
require __DIR__ . '/CommonFunctions.php';
header('Content-Type: application/json');

try {
    // 1) Required POST fields (parsed‐results fields are now optional)
    $required = [
        'IGCKey','EntrySeqID','IGCRecordDateTimeUTC','IGCUploadDateTimeUTC',
        'LocalDate','LocalTime','BeginTimeUTC','Pilot','GliderType','GliderID',
        'CompetitionID','CompetitionClass','NB21Version','Sim'
    ];
    foreach ($required as $f) {
        if (!isset($_POST[$f]) || trim($_POST[$f]) === '') {
            throw new Exception("Missing required field: $f");
        }
    }
    if (!isset($_FILES['igcFile']) || $_FILES['igcFile']['error'] !== UPLOAD_ERR_OK) {
        throw new Exception("IGC file upload failed");
    }

    // 2) Pull the base parameters
    $IGCKey               = trim($_POST['IGCKey']);
    $EntrySeqID           = (int) $_POST['EntrySeqID'];
    $IGCRecordDateTimeUTC = trim($_POST['IGCRecordDateTimeUTC']);
    $IGCUploadDateTimeUTC = trim($_POST['IGCUploadDateTimeUTC']);
    $LocalDate            = trim($_POST['LocalDate']);
    $LocalTime            = trim($_POST['LocalTime']);
    $BeginTimeUTC         = trim($_POST['BeginTimeUTC']);
    $Pilot                = trim($_POST['Pilot']);
    $GliderType           = trim($_POST['GliderType']);
    $GliderID             = trim($_POST['GliderID']);
    $CompetitionID        = trim($_POST['CompetitionID']);
    $CompetitionClass     = trim($_POST['CompetitionClass']);
    $NB21Version          = trim($_POST['NB21Version']);
    $Sim                  = trim($_POST['Sim']);
    // Optional user ID; skip UsersTasks if zero or absent
    $WSGUserID            = isset($_POST['WSGUserID']) ? (int) $_POST['WSGUserID'] : 0;

    // 3) Optional parsed‐results fields
    $TaskCompletedInt     = isset($_POST['TaskCompleted']) 
        ? (int) $_POST['TaskCompleted'] 
        : 0;
    $PenaltiesInt         = isset($_POST['Penalties']) 
        ? (int) $_POST['Penalties'] 
        : 0;
    $DurationSeconds      = isset($_POST['Duration']) 
        ? (int) $_POST['Duration'] 
        : 0; // seconds
    $DistanceFloat        = (isset($_POST['Distance']) && trim($_POST['Distance']) !== '') 
        ? (float) $_POST['Distance'] 
        : null;
    $SpeedFloat           = (isset($_POST['Speed']) && trim($_POST['Speed']) !== '') 
        ? (float) $_POST['Speed'] 
        : null;
    $IGCValidInt          = isset($_POST['IGCValid']) 
        ? (int) $_POST['IGCValid'] 
        : 0;
    $TPVersion            = isset($_POST['TPVersion']) 
        ? trim($_POST['TPVersion']) 
        : null;

    // 4) Move uploaded IGC file into the per‐task folder
    $destFolder = rtrim($taskBrowserPath, '/\\') . '/IGCFiles/' . $EntrySeqID;
    if (!is_dir($destFolder) && !mkdir($destFolder, 0755, true)) {
        throw new Exception("Failed to create folder: $destFolder");
    }
    $destPath = "$destFolder/$IGCKey.igc";
    if (!move_uploaded_file($_FILES['igcFile']['tmp_name'], $destPath)) {
        throw new Exception("Failed to move IGC file to destination");
    }

    // 5) Insert into IGCRecords
    $pdo = new PDO("sqlite:$databasePath");
    $pdo->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);

    // Duplicate check
    $dupQ = "SELECT 1 FROM IGCRecords WHERE IGCKey = :IGCKey";
    $stmt = $pdo->prepare($dupQ);
    $stmt->execute([':IGCKey' => $IGCKey]);
    if ($stmt->fetch()) {
        echo json_encode(['status'=>'duplicate','message'=>'IGCKey already exists']);
        exit;
    }

    $insertQ = "
      INSERT INTO IGCRecords (
        IGCKey, EntrySeqID, IGCRecordDateTimeUTC, IGCUploadDateTimeUTC,
        LocalTime, BeginTimeUTC, Pilot, GliderType, GliderID, CompetitionID,
        CompetitionClass, NB21Version, Sim, WSGUserID, TaskCompleted, Penalties,
        Duration, Distance, Speed, IGCValid, TPVersion, LocalDate
      ) VALUES (
        :IGCKey, :EntrySeqID, :IGCRecordDateTimeUTC, :IGCUploadDateTimeUTC,
        :LocalTime, :BeginTimeUTC, :Pilot, :GliderType, :GliderID, :CompetitionID,
        :CompetitionClass, :NB21Version, :Sim, :WSGUserID, :TaskCompleted, :Penalties,
        :Duration, :Distance, :Speed, :IGCValid, :TPVersion, :LocalDate
      )
    ";
    $stmt = $pdo->prepare($insertQ);
    $stmt->execute([
      ':IGCKey'               => $IGCKey,
      ':EntrySeqID'           => $EntrySeqID,
      ':IGCRecordDateTimeUTC' => $IGCRecordDateTimeUTC,
      ':IGCUploadDateTimeUTC' => $IGCUploadDateTimeUTC,
      ':LocalTime'            => $LocalTime,
      ':BeginTimeUTC'         => $BeginTimeUTC,
      ':Pilot'                => $Pilot,
      ':GliderType'           => $GliderType,
      ':GliderID'             => $GliderID,
      ':CompetitionID'        => $CompetitionID,
      ':CompetitionClass'     => $CompetitionClass,
      ':NB21Version'          => $NB21Version,
      ':Sim'                  => $Sim,
      ':WSGUserID'            => $WSGUserID,
      ':TaskCompleted'        => $TaskCompletedInt,
      ':Penalties'            => $PenaltiesInt,
      ':Duration'             => $DurationSeconds,
      ':Distance'             => $DistanceFloat,
      ':Speed'                => $SpeedFloat,
      ':IGCValid'             => $IGCValidInt,
      ':TPVersion'            => $TPVersion,
      ':LocalDate'            => $LocalDate
    ]);

    // 6) Update UsersTasks only when WSGUserID > 0
    if ($WSGUserID > 0) {
        // build MarkedFlownDateUTC from IGCRecordDateTimeUTC (YYMMDDHHMMSS)
        if (strlen($IGCRecordDateTimeUTC) === 12) {
            $y  = 2000 + (int)substr($IGCRecordDateTimeUTC, 0, 2);
            $m  = (int)substr($IGCRecordDateTimeUTC, 2, 2);
            $d  = (int)substr($IGCRecordDateTimeUTC, 4, 2);
            $H  = (int)substr($IGCRecordDateTimeUTC, 6, 2);
            $i  = (int)substr($IGCRecordDateTimeUTC, 8, 2);
            $markedDate = sprintf("%04d-%02d-%02d %02d:%02d:00", $y,$m,$d,$H,$i);
        } else {
            $markedDate = gmdate('Y-m-d H:i:s');
        }

        // check existing
        $checkQ = "SELECT MarkedFlownDateUTC FROM UsersTasks
                   WHERE WSGUserID=:u AND EntrySeqID=:e";
        $stmt   = $pdo->prepare($checkQ);
        $stmt->execute([':u'=>$WSGUserID,':e'=>$EntrySeqID]);
        $row    = $stmt->fetch(PDO::FETCH_ASSOC);

        if ($row) {
            if (empty($row['MarkedFlownDateUTC'])
             || strtotime($markedDate) > strtotime($row['MarkedFlownDateUTC'])) {
                $updQ = "UPDATE UsersTasks
                         SET MarkedFlownDateUTC=:md
                         WHERE WSGUserID=:u AND EntrySeqID=:e";
                $uStmt = $pdo->prepare($updQ);
                $uStmt->execute([':md'=>$markedDate,':u'=>$WSGUserID,':e'=>$EntrySeqID]);
            }
        } else {
            $insQ = "INSERT INTO UsersTasks
                      (WSGUserID,EntrySeqID,MarkedFlownDateUTC)
                     VALUES
                      (:u,:e,:md)";
            $iStmt = $pdo->prepare($insQ);
            $iStmt->execute([':u'=>$WSGUserID,':e'=>$EntrySeqID,':md'=>$markedDate]);
        }
    }

    // 7) Success
    echo json_encode([
      'status'  => 'success',
      'message' => 'IGC record saved.',
      'IGCKey'  => $IGCKey
    ]);
}
catch (Exception $e) {
    echo json_encode([
      'status'  => 'error',
      'message' => $e->getMessage()
    ]);
    exit;
}
