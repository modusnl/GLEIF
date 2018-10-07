# https://docs.microsoft.com/en-us/azure/data-lake-analytics/data-lake-analytics-cicd-overview#build-a-u-sql-project
Param(
    $USQLProject = "$PSScriptRoot\GLEIF.USQL.usqlproj",
    $USQLTargetType = 'SyntaxCheck',
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
& $MSBuild $USQLProject /t:$BuildType /p:USQLSDKPath=$USQLSDKPath /p:USQLTargetType=$USQLTargetType /p:DataRoot=$USQLDataRoot /p:EnableDeployment=True