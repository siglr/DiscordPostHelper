<?php
require __DIR__ . '/CommonFunctions.php';

const VALID_FILE_TYPES = ['pln', 'wpr', 'wpr2', 'zip'];

function sanitizeDownloadFilename($filename) {
    $clean = preg_replace('/[\r\n"]+/', '', $filename);
    return trim($clean);
}

function getBasenameFromPath($filePath) {
    $normalized = str_replace('\\', '/', $filePath);
    return basename($normalized);
}

function buildContentDisposition(string $filename): string {
    $encodedFilename = rawurlencode($filename);
    return 'attachment; filename="' . $filename . '"; filename*=UTF-8\'\'' . $encodedFilename;
}

function isRemoteUrl(string $path): bool {
    $scheme = parse_url($path, PHP_URL_SCHEME);
    return $scheme !== null && $scheme !== '';
}

function incrementDownloadCount(PDO $pdo, int $entrySeqID) {
    try {
        recordUniqueTaskDownload($pdo, $entrySeqID);
    } catch (Exception $error) {
        throw $error;
    }
}

try {
    $entrySeqID = filter_input(INPUT_GET, 'entrySeqID', FILTER_VALIDATE_INT);
    $fileTypeRaw = filter_input(INPUT_GET, 'fileType', FILTER_UNSAFE_RAW);

    if ($entrySeqID === null || $entrySeqID === false) {
        throw new Exception('Missing or invalid parameter: entrySeqID');
    }

    $fileType = $fileTypeRaw ? strtolower(trim($fileTypeRaw)) : null;

    if (!$fileType || !in_array($fileType, VALID_FILE_TYPES, true)) {
        throw new Exception('Missing or invalid parameter: fileType');
    }

    $pdo = new PDO("sqlite:$databasePath");
    $pdo->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);

    $query = "
        SELECT
            EntrySeqID,
            TaskID,
            Title,
            PLNFilename,
            PLNXML,
            WPRFilename,
            WPRXML,
            WPRSecondaryFilename,
            Availability
        FROM Tasks
        WHERE EntrySeqID = :entrySeqID
          AND Status = 99
    ";

    $stmt = $pdo->prepare($query);
    $stmt->bindParam(':entrySeqID', $entrySeqID, PDO::PARAM_INT);
    $stmt->execute();

    $task = $stmt->fetch(PDO::FETCH_ASSOC);

    if (!$task) {
        header('Content-Type: application/json');
        echo json_encode(['status' => 'not_found', 'message' => 'Task not found']);
        exit;
    }

    $nowUTC = (new DateTime('now', new DateTimeZone('UTC')))->getTimestamp();
    $availabilityTimestamp = null;

    if (!empty($task['Availability'])) {
        $availabilityDate = DateTime::createFromFormat('Y-m-d H:i:s', $task['Availability'], new DateTimeZone('UTC'));
        if ($availabilityDate !== false) {
            $availabilityTimestamp = $availabilityDate->getTimestamp();
        }
    }

    if ($availabilityTimestamp !== null && $availabilityTimestamp > $nowUTC) {
        header('Content-Type: application/json');
        echo json_encode([
            'status' => 'unavailable',
            'message' => 'Task is not available yet.',
            'availability' => $task['Availability']
        ]);
        exit;
    }

    if ($fileType === 'pln') {
        $filename = $task['PLNFilename'] ?: 'task.pln';
        $filename = sanitizeDownloadFilename(getBasenameFromPath($filename));
        $xml = prettyPrintXml($task['PLNXML']);

        incrementDownloadCount($pdo, $entrySeqID);

        header('Content-Type: application/octet-stream');
        header('Content-Disposition: ' . buildContentDisposition($filename));
        header('X-Content-Type-Options: nosniff');
        header('Content-Length: ' . strlen($xml));
        echo $xml;
        exit;
    }

    if ($fileType === 'wpr') {
        $filename = $task['WPRFilename'] ?: 'task.wpr';
        $filename = sanitizeDownloadFilename(getBasenameFromPath($filename));
        $xml = prettyPrintXml($task['WPRXML']);

        header('Content-Type: application/octet-stream');
        header('Content-Disposition: ' . buildContentDisposition($filename));
        header('X-Content-Type-Options: nosniff');
        header('Content-Length: ' . strlen($xml));
        echo $xml;
        exit;
    }

    if ($fileType === 'wpr2') {
        if (empty($task['WPRSecondaryFilename'])) {
            header('Content-Type: application/json');
            echo json_encode(['status' => 'not_found', 'message' => 'Secondary weather file not available']);
            exit;
        }

        $filename = sanitizeDownloadFilename(getBasenameFromPath($task['WPRSecondaryFilename']));
        $taskFolder = retrieveAndUnpackDPHX($task['TaskID']);
        $requestedFile = $taskFolder . '/' . $filename;

        if (!file_exists($requestedFile)) {
            header('Content-Type: application/json');
            echo json_encode(['status' => 'not_found', 'message' => 'Secondary file not found']);
            exit;
        }

        header('Content-Description: File Transfer');
        header('Content-Type: application/octet-stream');
        header('Content-Disposition: attachment; filename="' . $filename . '"');
        header('Content-Length: ' . filesize($requestedFile));
        readfile($requestedFile);
        exit;
    }

    if ($fileType === 'zip') {
        $taskID = $task['TaskID'];
        $title = sanitizeDownloadFilename($task['Title']) ?: $taskID;
        $downloadFilename = $title . '.zip';
        $fileUrl = "$taskBrowserPath/Tasks/$taskID.dphx";

        $fileStream = @fopen($fileUrl, 'rb');
        if (!$fileStream) {
            throw new Exception('ZIP file not found');
        }

        incrementDownloadCount($pdo, $entrySeqID);

        $contentLength = null;
        // get_headers only supports URLs; local paths should use filesize instead.
        if (isRemoteUrl($fileUrl)) {
            $headers = get_headers($fileUrl, true);
            if ($headers && isset($headers['Content-Length'])) {
                $contentLength = is_array($headers['Content-Length'])
                    ? end($headers['Content-Length'])
                    : $headers['Content-Length'];
            }
        } elseif (is_file($fileUrl)) {
            $contentLength = filesize($fileUrl);
        }

        header('Content-Description: File Transfer');
        header('Content-Type: application/zip');
        header('Content-Disposition: attachment; filename="' . $downloadFilename . '"');
        if ($contentLength) {
            header('Content-Length: ' . $contentLength);
        }

        fpassthru($fileStream);
        fclose($fileStream);
        exit;
    }
} catch (Exception $e) {
    header('Content-Type: application/json');
    echo json_encode(['error' => $e->getMessage()]);
}
?>
