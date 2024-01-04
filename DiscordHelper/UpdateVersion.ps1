# Custom function to format the date without leading zeros
function Get-FormattedDate {
    $date = Get-Date
    $year = $date.Year - 2000
    $month = $date.Month
    $day = $date.Day
    return "$year.$month.$day"
}

# Get the current formatted date
$currentDate = Get-FormattedDate
Write-Output "Current Formatted Date: $currentDate"

# Specify the path to the AssemblyInfo.vb file for the Release configuration
$assemblyInfoPath = "H:\DiscordHelper - 4.8.1\DiscordHelper\DiscordHelper\My Project\AssemblyInfo.vb"
Write-Output "AssemblyInfo.vb Path: $assemblyInfoPath"

# Read the existing AssemblyVersion and AssemblyFileVersion from AssemblyInfo.vb
$assemblyInfo = Get-Content $assemblyInfoPath

# Define a regular expression pattern to match the AssemblyVersion and AssemblyFileVersion lines
$pattern = 'AssemblyVersion\("(\d{2}\.\d{1,2}\.\d{1,2})\.(\d+)"\)'
$matches = [regex]::Matches($assemblyInfo, $pattern)

# Check if any matches were found
if ($matches.Count -gt 0) {
    # Get the existing version and sequence number
    $existingVersion = $matches[0].Groups[1].Value
    $sequenceNumber = [int]$matches[0].Groups[2].Value
    Write-Output "Existing Version: $existingVersion, Sequence Number: $sequenceNumber"

    # Check if the current date matches the existing date
    if ($existingVersion -eq $currentDate) {
        # Increment the sequence number
        $sequenceNumber++
        Write-Output "Date matches, incrementing sequence number to $sequenceNumber"
    } else {
        # If the current date is different, reset the sequence number to 1
        $sequenceNumber = 1
        Write-Output "Date does not match, resetting sequence number to $sequenceNumber"
    }

    # Update the version with the new sequence number
    $newVersion = "$currentDate.$sequenceNumber"
    Write-Output "New Version: $newVersion"

    # Replace the AssemblyVersion and AssemblyFileVersion lines in the AssemblyInfo.vb file
    $assemblyInfo = $assemblyInfo -replace $pattern, "AssemblyVersion(`"$newVersion`")"
    $assemblyInfo = $assemblyInfo -replace 'AssemblyFileVersion\("(\d+\.\d+\.\d+\.\d+)"\)', "AssemblyFileVersion(`"$newVersion`")"

    # Save the updated AssemblyInfo.vb file
    $assemblyInfo | Set-Content $assemblyInfoPath
} else {
    Write-Output "No matching version found in AssemblyInfo.vb"
}
