# Initialize variables
$LocalPath = $PSScriptRoot
$RemotePath = '/GLEIF'
$ADLSAccountName = "modusadls"

# Iterate through local Zip & Xml files and upload to DataLake
$items = Get-ChildItem -Path "$LocalPath\*" -Include *zip, *.xml -Filter *lei2*
foreach($item in $items){
    $destination = "$RemotePath/$($item.Name)"
    Import-AzureRmDataLakeStoreItem -AccountName $ADLSAccountName -Path $item.FullName -Destination $destination -Force
}