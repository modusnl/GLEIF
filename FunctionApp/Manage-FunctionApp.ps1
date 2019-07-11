Get-AzSubscription -SubscriptionName "Modus VS Enterprise" | Set-AzContext
$resourceGroup = 'Modus-FA'
$FunctionAppName = 'Modus-FAnet'
$ServicePlanName = 'AppServicePlanWinWEU-EP'
$ConsumptionPlanName = 'WestEuropePlan'
$StorageAccountName = 'modusfa'
$StorageAccountNameGleif = 'modusgleif'
$StorageAccountKeyGleif = (Get-AzStorageAccountKey -ResourceGroupName Modus-Gleif -Name $StorageAccountNameGleif)[0].Value
$StorageAccountConnectionString = "DefaultEndpointsProtocol=https;AccountName=$StorageAccountNameGleif;AccountKey=$StorageAccountKeyGleif;EndpointSuffix=core.windows.net"

# List
Get-AzWebApp -Name $FunctionAppName 
Get-AzAppServicePlan -ResourceGroupName $resourceGroup

# Create
#https://docs.microsoft.com/nl-nl/azure/azure-functions/functions-premium-plan
az login
az account set --subscription "Modus VS Enterprise"
az functionapp plan create -g $resourceGroup -n $ServicePlanName -l westeurope --number-of-workers 1 --sku EP1
#az functionapp plan create -g $resourceGroup -n "ConsumptionPlanWEU" -l westeurope --sku Y1 --help --> Invalid SKU
az functionapp create -g $resourceGroup  -p $ServicePlanName -n $FunctionAppName  -s $StorageAccountName

# Function App Config
Set-AzWebApp -Name $FunctionAppName -ResourceGroupName $resourceGroup -Use32BitWorkerProcess $false -HttpsOnly $true -AssignIdentity $true 

# Change to Consumption Plan
Set-AzWebApp -Name $FunctionAppName  -ResourceGroupName $resourceGroup -AppServicePlan $ConsumptionPlanName

# Change to Service Plan
Set-AzWebApp -Name $FunctionAppName  -ResourceGroupName $resourceGroup -AppServicePlan $ServicePlanName