<#
    This script can be used to submit ADLA (Azure data lake analytics) Job with given usql script file.
    This will go through given specific subdirectory (e.g. Config, Schema, StoredProcedures) and find out 
    usql files and then submit job and wait for completion. Script will take care of handling any environment specific
    file execution (naming convention of file should be <script name>.<environmentname>.usql e.g. CustomConfig.Dev.usql
    
    Note: Currently we are going to deploy from specific folders (Config, Schema etc) and order of execution is important.
          So schema, database should be deployed first before any stored procedures.
    
    Usage :
          1. Submit all usql script related to create assembly,schema, tables, stored procedures
          2. Submit environment specific usql scripts.
          
    Example :
        Deploy-AzureDataLakeAnalyticsJob.ps1 -ResourceGroupName "upp-adl" -ADLAAccountName "uppcalcadlppe" -RemoteRootFolder "C:\AmitOm\UST\UPP.Calc.Processor\ET" -EnvironmentName "dev"
#>

param(
	[Parameter(Mandatory=$true)][string]$ADLSAccountName,  #ADLS (Store) account name
    [Parameter(Mandatory=$true)][string]$ADLAAccountName,  #ADLA (Analytics) account name
    [Parameter(Mandatory=$true)][string]$LocalRootFolder,  #Root folder (e.g. local artifacts  folder)
    [Parameter(Mandatory=$true)][string]$RemoteRootFolder, #Root folder (e.g. remote artifacts root folder)
    [Parameter(Mandatory=$false)][string]$DegreeOfParallelism = 1
)

# Trigger USQL deployment
Function Main()
{
    LogInputData 

    Write-Output "Starting USQL script deployment..."

	# Submit ADLA jobs with usql scripts in given sub-folder.
    # Order is important here. Scripts with least dependency goes first followed 
    # by scripts which more dependencies.
    
	UploadResources

	SubmitAnalyticsJob "Assembly"

	SubmitAnalyticsJob "Procedure"

	SubmitAnalyticsJob "Query"

	SubmitAnalyticsJob "Test"

    Write-Host "***********************************************************************"
    Write-Output "Finished deployment..."
}

Function UploadResources()
{
	Write-Host "************************************************************************"
    Write-Host "Uploading files to '$ADLSAccountName'"
    Write-Host "***********************************************************************"

    $files = @(Get-ChildItem $LocalRootFolder -recurse -File)
    foreach($file in $files)
    {    
        switch([System.IO.Path]::GetExtension($file)){
            ".dll"{$destination = "$RemoteRootFolder/Assembly/$file"}
            ".pdb"{$destination = "$RemoteRootFolder/Assembly/$file"}
            ".usql"{$destination = "$RemoteRootFolder/$($file.Directory.Name)/$file"}
            default {$destination = "$RemoteRootFolder/$file"}
        }

        Write-Host "Uploading file: $($file.Name) to $destination"
        $upload = Import-AzureRmDataLakeStoreItem -AccountName $ADLSAccountName -Path $file.FullName -Destination $destination -Force -Recurse
    }
}

# Submit all usql scripts in given subdirectory and deploy
# any environment specific scripts if any.
# We follow convention that all environment specific script should follow
# naming pattern like <script>.<environmenname>.usql

Function SubmitAnalyticsJob($folderName)
{
    $usqlFiles = GetLocalUsqlFiles($folderName)

    # submit each usql script and wait for completion before moving ahead.
    foreach ($usqlFile in $usqlFiles)
    {
        $scriptName = "[Release].[$([System.IO.Path]::GetFileNameWithoutExtension($usqlFile.fullname))]"

        Write-Output "Submitting job for '{$usqlFile}'"

        $jobToSubmit = Submit-AzureRmDataLakeAnalyticsJob -Account $ADLAAccountName -Name $scriptName -ScriptPath $usqlFile -DegreeOfParallelism $DegreeOfParallelism
        LogJobInformation $jobToSubmit
        
        Write-Output "waiting for job to complete. Job ID:'{$($jobToSubmit.JobId)}', Name: '$($jobToSubmit.Name)' "
        $jobResult = Wait-AzureRmDataLakeAnalyticsJob -Account $ADLAAccountName -JobId $jobToSubmit.JobId  
        LogJobInformation $jobResult
        
		# ProcessResult $jobResult
    }
}

# Finds all usql script in given subdirectory and if environment name is passed then
# finds all scripts for that given environment
Function GetLocalUsqlFiles($subFolder)
{
    $files = Get-ChildItem -Path $LocalRootFolder -Include *.usql -File -Recurse -ErrorAction SilentlyContinue | Where-Object {$_.DirectoryName -match $subFolder}

    return $files
}

Function ProcessResult($jobResult)
{
	if ($jobResult.Result -eq "Failed")
	{
		Write-Error "Job Failed. Job Id: $($jobResult.JobId), Job Name: $($jobResult.Name), Log: $($jobResult.LogFolder)"
	}
	else
	{
		Write-Output "Job Succeeded. Job Id: $($jobResult.JobId), Job Name: $($jobResult.Name)"
	}
}

Function LogInputData()
{
	Write-Output "[INPUT#] : Azure data-lake store account name : {$ADLSAccountName}"
    Write-Output "[INPUT#] : Azure data-lake analytics account name : {$ADLAAccountName}"
    Write-Output "[INPUT#] : Root folder for all deploy resources : {$LocalRootFolder}"
    Write-Output "[INPUT#] : Root folder for all usql script files : {$RemoteRootFolder}"
    Write-Output "[INPUT#] : Maximum allowable parallelism of the job : {$DegreeOfParallelism}"

}

Function LogJobInformation($jobInfo)
{
    Write-Output "************************************************************************"
    Write-Output ([string]::Format("Job Id: {0}", $(DefaultIfNull $jobInfo.JobId)))
    Write-Output ([string]::Format("Job Name: {0}", $(DefaultIfNull $jobInfo.Name)))
    Write-Output ([string]::Format("Job State: {0}", $(DefaultIfNull $jobInfo.State)))
    Write-Output ([string]::Format("Job Started at: {0}", $(DefaultIfNull  $jobInfo.StartTime)))
    Write-Output ([string]::Format("Job Ended at: {0}", $(DefaultIfNull  $jobInfo.EndTime)))
    Write-Output ([string]::Format("Job Result: {0}", $(DefaultIfNull $jobInfo.Result)))
    Write-Output "************************************************************************"
}

Function DefaultIfNull($item)
{
    if ($item -ne $null)
	{
        return $item
    }
    return ""
}

# Trigger execution
Main