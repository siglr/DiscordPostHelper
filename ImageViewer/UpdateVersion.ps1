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

# Specify the path to the AssemblyInfo.cs file for the Release configuration
$assemblyInfoPath = "H:\DiscordHelper - 4.8.1\DiscordHelper\ImageViewer\Properties\AssemblyInfo.cs"

# Read the existing AssemblyVersion and AssemblyFileVersion from AssemblyInfo.cs
$assemblyInfo = Get-Content $assemblyInfoPath

# Define a regular expression pattern to match the AssemblyVersion and AssemblyFileVersion lines
$pattern = 'AssemblyVersion\("(\d{2}\.\d{1,2}\.\d{1,2})\.(\d+)"\)'
$matches = [regex]::Matches($assemblyInfo, $pattern)

# Check if any matches were found
if ($matches.Count -gt 0) {
    # Get the existing version and sequence number
    $existingVersion = $matches[0].Groups[1].Value
    $sequenceNumber = [int]$matches[0].Groups[2].Value

    # Check if the current date matches the existing date
    if ($existingVersion -eq $currentDate) {
        # Increment the sequence number
        $sequenceNumber++
    } else {
        # If the current date is different, reset the sequence number to 1
        $sequenceNumber = 1
    }

    # Update the version with the new sequence number
    $newVersion = "$currentDate.$sequenceNumber"

    # Replace the AssemblyVersion and AssemblyFileVersion lines in the AssemblyInfo.cs file
    $assemblyInfo = $assemblyInfo -replace $pattern, "AssemblyVersion(`"$newVersion`")"
    $assemblyInfo = $assemblyInfo -replace 'AssemblyFileVersion\("(\d+\.\d+\.\d+\.\d+)"\)', "AssemblyFileVersion(`"$newVersion`")"

    # Save the updated AssemblyInfo.cs file
    $assemblyInfo | Set-Content $assemblyInfoPath
}
