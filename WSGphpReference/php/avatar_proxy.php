<?php
declare(strict_types=1);

$rawUrl = filter_input(INPUT_GET, 'url', FILTER_VALIDATE_URL);
if (!$rawUrl) {
    http_response_code(400);
    echo 'Missing or invalid url parameter.';
    exit;
}

$parts = parse_url($rawUrl);
if (!$parts || empty($parts['scheme']) || empty($parts['host'])) {
    http_response_code(400);
    echo 'Invalid url format.';
    exit;
}

$scheme = strtolower($parts['scheme']);
if ($scheme !== 'https') {
    http_response_code(400);
    echo 'Only https URLs are allowed.';
    exit;
}

$host = strtolower($parts['host']);
$allowedHosts = [
    'cdn.discordapp.com',
    'media.discordapp.net',
    'discordapp.com',
    'discordapp.net',
    'discord.com',
];

$isAllowedHost = false;
foreach ($allowedHosts as $allowedHost) {
    if ($host === $allowedHost || str_ends_with($host, '.' . $allowedHost)) {
        $isAllowedHost = true;
        break;
    }
}

if (!$isAllowedHost || filter_var($host, FILTER_VALIDATE_IP)) {
    http_response_code(403);
    echo 'Host not allowed.';
    exit;
}

$maxBytes = 5 * 1024 * 1024;
$data = '';
$headers = [];
$sizeExceeded = false;

$ch = curl_init($rawUrl);
curl_setopt($ch, CURLOPT_RETURNTRANSFER, false);
curl_setopt($ch, CURLOPT_FOLLOWLOCATION, true);
curl_setopt($ch, CURLOPT_MAXREDIRS, 3);
curl_setopt($ch, CURLOPT_CONNECTTIMEOUT, 5);
curl_setopt($ch, CURLOPT_TIMEOUT, 10);
curl_setopt($ch, CURLOPT_USERAGENT, 'WSG-Avatar-Proxy/1.0');
curl_setopt($ch, CURLOPT_SSL_VERIFYPEER, true);
curl_setopt($ch, CURLOPT_SSL_VERIFYHOST, 2);
curl_setopt($ch, CURLOPT_HEADERFUNCTION, static function ($ch, $header) use (&$headers) {
    $length = strlen($header);
    $parts = explode(':', $header, 2);
    if (count($parts) === 2) {
        $headers[strtolower(trim($parts[0]))] = trim($parts[1]);
    }
    return $length;
});
curl_setopt($ch, CURLOPT_WRITEFUNCTION, static function ($ch, $chunk) use (&$data, $maxBytes, &$sizeExceeded) {
    $data .= $chunk;
    if (strlen($data) > $maxBytes) {
        $sizeExceeded = true;
        return 0;
    }
    return strlen($chunk);
});

$success = curl_exec($ch);
$statusCode = curl_getinfo($ch, CURLINFO_HTTP_CODE);
$curlError = curl_errno($ch);
curl_close($ch);

if ($sizeExceeded) {
    http_response_code(413);
    echo 'Avatar exceeds size limit.';
    exit;
}

if ($curlError !== 0 || !$success || $statusCode >= 400) {
    http_response_code(404);
    echo 'Avatar not found.';
    exit;
}

$contentType = $headers['content-type'] ?? '';
if (stripos($contentType, 'image/') !== 0) {
    http_response_code(415);
    echo 'Unsupported content type.';
    exit;
}

header('Content-Type: ' . $contentType);
header('Cache-Control: public, max-age=86400');
echo $data;
