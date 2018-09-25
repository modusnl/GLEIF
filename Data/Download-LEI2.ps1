# Prepare variables
$fileDate = '20180901'
$url = "https://leidata.gleif.org/api/v1/concatenated-files/lei2/$fileDate/zip"
$outFile = ".\$fileDate-gleif-concatenated-file-lei2.xml.zip"

# Perform download (~30 sec)
$startTime = Get-Date
Invoke-WebRequest -Uri $url -OutFile $outFile
Write-Output "Time taken: $((Get-Date).Subtract($startTime).Seconds) second(s) for Downloading $outFile"

# Extract Zip (~30 sec)
$startTime = Get-Date
Expand-Archive -Path $outFile -DestinationPath '.\'
Write-Output "Time taken: $((Get-Date).Subtract($startTime).Seconds) second(s) for Extracting $outFile"