# Initialize variables
$LocalRootFolder = "$PSScriptRoot\Test"
$RemoteRootFolder ='/USQL'
$ADLSAccountName = "modusadls"
$ADLAAccountName = "modusadla"

# Local Build
& "$PSScriptRoot\BuildProject.ps1"

# Upload and Create Assembly
& "$PSScriptRoot\SubmitUSQL.ps1" -LocalRootFolder $LocalRootFolder -RemoteRootFolder $RemoteRootFolder -ADLSAccountName $ADLSAccountName  -ADLAAccountName $ADLAAccountName