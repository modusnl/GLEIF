# https://blogs.msdn.microsoft.com/azuredatalake/2017/10/24/continuous-integration-made-easy-with-msbuild-support-for-u-sql-preview/
# https://docs.microsoft.com/en-us/azure/data-lake-analytics/data-lake-analytics-cicd-overview#build-u-sql-project
Param(
    $USQLProject = 'GLEIF.USQL.usqlproj',
    $USQLTargetType = 'SyntaxCheck'
)

Clear-Host

# Initialize USQL variables
$USQLDataRoot = "$env:LOCALAPPDATA\USQLDataRoot"
$USQLSDKPath = "$env:USERPROFILE\.nuget\packages\Microsoft.Azure.DataLake.USQL.SDK\1.4.180926\build\runtime"
$LocalRunHelper = "$USQLSDKPath\LocalRunHelper.exe"
$PackageDeploymentTool = "$USQLSDKPath\PackageDeploymentTool.exe"

# Perform MSBuild
Write-Host "`nMSBuild: Building '$USQLProject'...`n" -ForegroundColor Cyan
$MSBuild = "C:\Program Files (x86)\Microsoft Visual Studio\2017\Enterprise\MSBuild\15.0\Bin\MSBuild.exe"
& $MSBuild $USQLProject /p:USQLSDKPath=$USQLSDKPath /p:USQLTargetType=$USQLTargetType /p:DataRoot=$USQLDataRoot /t:rebuild