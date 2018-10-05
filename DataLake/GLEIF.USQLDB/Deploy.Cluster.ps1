# https://docs.microsoft.com/en-us/azure/data-lake-analytics/data-lake-analytics-data-lake-tools-develop-usql-database#deploy-u-sql-database
# https://docs.microsoft.com/nl-nl/azure/data-lake-analytics/data-lake-analytics-cicd-overview#deploy-u-sql-database-through-azure-pipelines
Param(
    $USQLProject = 'GLEIF.USQLDB.usqldbproj',
    $USQLDatabase = "GLEIF.USQLDB",
    $PackagePath = "$PSScriptRoot\bin\Debug\GLEIF.USQLDB.usqldbpack"
)

Clear-Host

# Initialize USQL variables
$USQLSDKPath = "$env:USERPROFILE\.nuget\packages\Microsoft.Azure.DataLake.USQL.SDK\1.4.180926\build\runtime"
$AzureSDKPath = "C:\Program Files (x86)\Microsoft Visual Studio\2017\Enterprise\common7\ide\extensions\microsoft\adl tools\2.3.5001.1"
$LocalRunHelper = "$AzureSDKPath\U-SQLSDK\LocalRunHelper.exe"
$PackageDeploymentTool = "$AzureSDKPath\U-SQLSDK\PackageDeploymentTool.exe"

# Build 
$MSBuild = "C:\Program Files (x86)\Microsoft Visual Studio\2017\Enterprise\MSBuild\15.0\Bin\MSBuild.exe";
#& $MSBuild $USQLProject /p:USQLSDKPath=$USQLSDKPath

# Drop existing database
#Write-Host "`nLocalRunHelper: Dropping Database '$USQLDatabase'..." -ForegroundColor Cyan
#$Script = "$PSScriptRoot\Drop-Database.usql"
#& $LocalRunHelper run -Script $Script -DataRoot $USQLDataRoot #-Verbose

# Execute clean Deployment
Write-Host "`nPackageDeploymentTool: Depolying Database '$USQLDatabase'..." -ForegroundColor Cyan
$AccountADLA = 'modusadla'
$ResourceGroup = 'Modus-ADLS'
$SubscriptionId = '947b192a-5c69-46ef-8378-3349c449a553'
$Tenant = '60eb35ed-e512-4f48-9a6d-622bc1d9a525'
$JobPrefix = "DeployDatabase_$USQLDatabase" 

# > Interactive mode seeems to fail...
Write-Host "& $PackageDeploymentTool deploycluster -Package $PackagePath -Database $USQLDatabase -JobPrefix $JobPrefix -AzureSDKPath $AzureSDKPath -Account $AccountADLA -ResourceGroup $ResourceGroup -SubscriptionId $SubscriptionId -Tenant $Tenant -Interactive -LogLevel Verbose" -ForegroundColor Cyan
            & $PackageDeploymentTool deploycluster -Package $PackagePath -Database $USQLDatabase -JobPrefix $JobPrefix -AzureSDKPath $AzureSDKPath -Account $AccountADLA -ResourceGroup $ResourceGroup -SubscriptionId $SubscriptionId -Tenant $Tenant -Interactive -LogLevel Verbose

#ERROR:
<#
Deployment failed with Could not load file or assembly 'Microsoft.Rest.ClientRuntime.Azure.Authentication, Version=2.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35' or one of its dependencies. The system c
annot find the file specified.
System.IO.FileNotFoundException: Could not load file or assembly 'Microsoft.Rest.ClientRuntime.Azure.Authentication, Version=2.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35' or one of its dependencies. Th
e system cannot find the file specified.
File name: 'Microsoft.Rest.ClientRuntime.Azure.Authentication, Version=2.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35'
   at Microsoft.Analytics.CICD.Deploy.Program.GetCredsInteractivePopup(String domain, Uri tokenAudience, String clientId, PromptBehavior promptBehavior)
   at Microsoft.Analytics.CICD.Deploy.Program.DoClusterDeployment()
   at Microsoft.Analytics.CICD.Deploy.Program.Main(String[] args)

WRN: Assembly binding logging is turned OFF.
To enable assembly bind failure logging, set the registry value [HKLM\Software\Microsoft\Fusion!EnableLog] (DWORD) to 1.
Note: There is some performance penalty associated with assembly bind failure logging.
To turn this feature off, remove the registry value [HKLM\Software\Microsoft\Fusion!EnableLog].
#>