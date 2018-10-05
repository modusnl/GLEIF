# https://docs.microsoft.com/en-us/azure/data-lake-analytics/data-lake-analytics-data-lake-tools-develop-usql-database#deploy-u-sql-database
Param(
    $USQLProject = 'GLEIF.USQLDB.usqldbproj',
    $USQLDatabase = "GLEIF.USQLDB",
    $PackagePath = "$PSScriptRoot\bin\Debug\GLEIF.USQLDB.usqldbpack"
)

Clear-Host

# Initialize USQL variables
$USQLDataRoot = "$env:LOCALAPPDATA\USQLDataRoot"
$USQLSDKPath = "$env:USERPROFILE\.nuget\packages\Microsoft.Azure.DataLake.USQL.SDK\1.3.180620\build\runtime"
$LocalRunHelper = "$USQLSDKPath\LocalRunHelper.exe"
$PackageDeploymentTool = "$USQLSDKPath\PackageDeploymentTool.exe"

# Perform MSBuild
Write-Host "`nMSBuild: Building '$USQLProject'...`n" -ForegroundColor Cyan
$MSBuild = "C:\Program Files (x86)\Microsoft Visual Studio\2017\Enterprise\MSBuild\15.0\Bin\MSBuild.exe"
& $MSBuild $USQLProject /p:USQLSDKPath=$USQLSDKPath

# Drop existing database
Write-Host "`nLocalRunHelper: Dropping Database '$USQLDatabase'...`n" -ForegroundColor Cyan
$Script = "$PSScriptRoot\Drop-Database.usql"
& $LocalRunHelper run -Script $Script -DataRoot $USQLDataRoot

# Execute clean Deployment
Write-Host "`nPackageDeploymentTool: Depolying Database '$USQLDatabase'...`n" -ForegroundColor Cyan
& $PackageDeploymentTool deploylocal -Package $PackagePath -Database $USQLDatabase -DataRoot $USQLDataRoot -LocalAccount localcomputeaccount -LogLevel Normal