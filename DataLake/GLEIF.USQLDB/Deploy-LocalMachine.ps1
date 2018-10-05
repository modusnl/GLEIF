# https://docs.microsoft.com/en-us/azure/data-lake-analytics/data-lake-analytics-cicd-overview#build-u-sql-database-project
Param(
    $USQLProject = "$PSScriptRoot\GLEIF.USQLDB.usqldbproj",
    $USQLDatabase = "GLEIF.USQLDB",
    $PackagePath = "$PSScriptRoot\bin\Debug\GLEIF.USQLDB.usqldbpack",
    $BuildType = 'Build'
)

Clear-Host

# Initialize USQL variables
$USQLSDKPath = "$env:USERPROFILE\.nuget\packages\Microsoft.Azure.DataLake.USQL.SDK\1.4.180926\build\runtime"
$USQLDataRoot = "$env:LOCALAPPDATA\USQLDataRoot"
$LocalRunHelper = "$USQLSDKPath\LocalRunHelper.exe"
$PackageDeploymentTool = "$USQLSDKPath\PackageDeploymentTool.exe"

# Perform MSBuild
Write-Host "`nMSBuild: Building '$USQLProject'...`n" -ForegroundColor Cyan
$MSBuild = "C:\Program Files (x86)\Microsoft Visual Studio\2017\Enterprise\MSBuild\15.0\Bin\MSBuild.exe"
& $MSBuild $USQLProject /t:$BuildType /p:USQLSDKPath=$USQLSDKPath

# Drop existing database
Write-Host "`nLocalRunHelper: Dropping Database '$USQLDatabase'...`n" -ForegroundColor Cyan
$Script = "$PSScriptRoot\Drop-Database.usql"
& $LocalRunHelper run -Script $Script -DataRoot $USQLDataRoot

# Execute clean Deployment
Write-Host "`nPackageDeploymentTool: Depolying Database '$USQLDatabase'...`n" -ForegroundColor Cyan
& $PackageDeploymentTool deploylocal -Package $PackagePath -Database $USQLDatabase -DataRoot $USQLDataRoot -LocalAccount localcomputeaccount -LogLevel Normal