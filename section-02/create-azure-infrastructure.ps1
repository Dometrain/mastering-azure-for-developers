param(
    [string]$Department = "appdev",
    [string]$Environment = "dev",
    [string]$Region = "westeurope",
    [string]$RegionShort = "we",    
    [string]$FunctionalGroup = "ti", # ti for TravelInspiration
    [string]$UniqueIdentifier = "i68e3",  # Optional parameter
    [string]$PublisherName = "Marvin", # Used for APIM, replace with your own
    [string]$PublisherEmail = "kevin.dockx@gmail.com", # Used for APIM, replace with your own
    [string]$SqlAdminUser = "kevin", # Used for SQL access, optionally replace this    
    [string]$SqlAdminPassword = "HRZc8atfjZbrkb2", # Used for SQL access, optionally replace this
    [string]$EntraAdminDisplayName = "INPUT-YOUR-OWN", # Replace with your own Entra ID display name
    [string]$EntraAdminObjectId = "INPUT-YOUR-OWN" # Replace with your own Entra ID user object identifier
)

# Function to check if resource group exists
function Test-ResourceGroupExists {
    param([string]$ResourceGroupName)
    
    $result = az group exists --name $ResourceGroupName 2>$null
    return $result -eq "true"
}

# Validate and prepare UniqueIdentifier
$UniqueSuffix = ""
if (-not [string]::IsNullOrWhiteSpace($UniqueIdentifier)) {
    # Validate UniqueIdentifier format (alphanumeric, max 8 chars for Azure naming limits)
    if ($UniqueIdentifier -notmatch '^[a-zA-Z0-9]{1,8}$') {
        throw "UniqueIdentifier must be alphanumeric and maximum 8 characters long"
    }
    $UniqueSuffix = "-$UniqueIdentifier"
    Write-Host "Using UniqueIdentifier: $UniqueIdentifier" -ForegroundColor Yellow
} else {
    Write-Host "No UniqueIdentifier provided - using default resource names" -ForegroundColor Yellow
}

# Construct resource names with conditional user suffix
$ResourceGroupMain = "$Department-rg-$Environment-$RegionShort$UniqueSuffix"
$ResourceGroupTI = "$Department-rg-$Environment-$RegionShort-$FunctionalGroup$UniqueSuffix"
$LogAnalyticsWorkspace = "$Department-log-$Environment-$RegionShort$UniqueSuffix"
$ApplicationInsights = "$Department-appi-$Environment-$RegionShort$UniqueSuffix"
$ApiManagement = "$Department-apim-$Environment-$RegionShort$UniqueSuffix"
$AppServicePlan = "$Department-plan-$Environment-$RegionShort-$FunctionalGroup$UniqueSuffix"
$WebAppClient = "$Department-app-$Environment-$RegionShort-ticlient$UniqueSuffix"
$WebAppClientJS = "$Department-app-$Environment-$RegionShort-ticlientjs$UniqueSuffix"
$WebAppDestinationsAPI = "$Department-app-$Environment-$RegionShort-tidestapi$UniqueSuffix"
$StorageAccount = "$Department`st$Environment$RegionShort$FunctionalGroup$UniqueIdentifier"  # No dash for storage account
$FunctionApp = "$Department-func-$Environment-$RegionShort-tiitinapi$UniqueSuffix"
$SqlServer = "$Department-sql-$Environment-$RegionShort-$FunctionalGroup$UniqueSuffix"
$SqlDatabase = "$Department-sqldb-$Environment-$RegionShort-$FunctionalGroup$UniqueSuffix"

Write-Host "Starting Azure resource deployment with the following configuration:" -ForegroundColor Green
Write-Host "Environment: $Environment" -ForegroundColor Yellow
Write-Host "Region: $Region" -ForegroundColor Yellow
Write-Host "Main Resource Group: $ResourceGroupMain" -ForegroundColor Yellow
Write-Host "TI Resource Group: $ResourceGroupTI" -ForegroundColor Yellow

# Login to Azure (if not already logged in)
Write-Host "Ensuring Azure login..." -ForegroundColor Blue
az login

# Delete existing resource groups only if they exist
Write-Host "Checking for existing resource groups from previous courses, or that were created by previously running this script..." -ForegroundColor Blue

$ResourceGroupsToDelete = @('appdev-rg-dev-we', 'appdev-rg-dev-we-travelinspiration', $ResourceGroupMain, $ResourceGroupTI)

foreach ($ResourceGroup in $ResourceGroupsToDelete) {
    if (Test-ResourceGroupExists -ResourceGroupName $ResourceGroup) {
        Write-Host "Deleting existing resource group: $ResourceGroup" -ForegroundColor Yellow
        az group delete --name $ResourceGroup --yes --no-wait
    } else {
        Write-Host "Resource group does not exist, skipping: $ResourceGroup" -ForegroundColor Gray
    }
}

# Wait for all deletions to complete before proceeding
Write-Host "Waiting for resource group deletions to complete..." -ForegroundColor Blue
foreach ($ResourceGroup in $ResourceGroupsToDelete) {
    while (Test-ResourceGroupExists -ResourceGroupName $ResourceGroup) {
        Write-Host "Still waiting for deletion of: $ResourceGroup" -ForegroundColor Gray
        Start-Sleep -Seconds 30
    }
}
Write-Host "All resource group deletions completed." -ForegroundColor Green 
 

# Create the main resource group
Write-Host "Creating main resource group: $ResourceGroupMain" -ForegroundColor Blue
az group create --name $ResourceGroupMain --location $Region

# Create Log Analytics workspace
Write-Host "Creating Log Analytics workspace: $LogAnalyticsWorkspace" -ForegroundColor Blue
az monitor log-analytics workspace create --resource-group $ResourceGroupMain --name $LogAnalyticsWorkspace --location $Region

# Create Application Insights connected to the Log Analytics workspace
Write-Host "Creating Application Insights: $ApplicationInsights" -ForegroundColor Blue
az monitor app-insights component create --app $ApplicationInsights --location $Region --resource-group $ResourceGroupMain --workspace $LogAnalyticsWorkspace --kind web --application-type web

# Create the APIM instance in Developer mode
Write-Host "Creating API Management: $ApiManagement" -ForegroundColor Blue
az apim create --name $ApiManagement --resource-group $ResourceGroupMain --location $Region --publisher-name $PublisherName --publisher-email $PublisherEmail --sku-name Developer

# Create the TI resource group
Write-Host "Creating TI resource group: $ResourceGroupTI" -ForegroundColor Blue
az group create --name $ResourceGroupTI --location $Region

# Create App Service Plan 
Write-Host "Creating App Service Plan: $AppServicePlan" -ForegroundColor Blue
az appservice plan create --name $AppServicePlan --resource-group $ResourceGroupTI --location $Region --sku B1

# Create frontend client web app
Write-Host "Creating client web app: $WebAppClient" -ForegroundColor Blue
az webapp create --name $WebAppClient --resource-group $ResourceGroupTI --plan $AppServicePlan --runtime "dotnet:9"

# Create frontend client web app for JavaScript-based client 
Write-Host "Creating JavaScript client web app: $WebAppClientJS" -ForegroundColor Blue
az webapp create --name $WebAppClientJS --resource-group $ResourceGroupTI --plan $AppServicePlan --runtime "dotnet:9"

# Create destination search API web app
Write-Host "Creating destinations API web app: $WebAppDestinationsAPI" -ForegroundColor Blue
az webapp create --name $WebAppDestinationsAPI --resource-group $ResourceGroupTI --plan $AppServicePlan --runtime "dotnet:9"

# Create a storage account for the function app
Write-Host "Creating storage account: $StorageAccount" -ForegroundColor Blue
az storage account create --name $StorageAccount --resource-group $ResourceGroupTI --location $Region --sku Standard_LRS --kind StorageV2 --access-tier Hot --https-only true --min-tls-version TLS1_2
    
# Create function app for the itineraries API
Write-Host "Creating function app: $FunctionApp" -ForegroundColor Blue
az functionapp create --name $FunctionApp --resource-group $ResourceGroupTI --plan $AppServicePlan --storage-account $StorageAccount --runtime dotnet-isolated --runtime-version 9 --functions-version 4

# Create SQL Server
Write-Host "Creating SQL Server: $SqlServer" -ForegroundColor Blue
az sql server create --name $SqlServer --resource-group $ResourceGroupTI --location $Region --admin-user $SqlAdminUser --admin-password $SqlAdminPassword

# Add Entra ID admin
Write-Host "Adding Entra ID admin: $EntraAdminDisplayName" -ForegroundColor Blue
az sql server ad-admin create --resource-group $ResourceGroupTI --server-name $SqlServer --display-name $EntraAdminDisplayName --object-id $EntraAdminObjectId
 
# Create SQL Database with DTU-based model (Basic tier with 5 DTUs)
Write-Host "Creating SQL Database: $SqlDatabase" -ForegroundColor Blue
az sql db create --name $SqlDatabase --resource-group $ResourceGroupTI --server $SqlServer --service-objective Basic --max-size 2GB

# Create firewall rule to allow Azure services to connect to SQL Server
Write-Host "Creating SQL Server firewall rule" -ForegroundColor Blue
az sql server firewall-rule create --name AllowAllAzureServices --resource-group $ResourceGroupTI --server $SqlServer --start-ip-address 0.0.0.0 --end-ip-address 0.0.0.0

# Add SQL connection string to function app
Write-Host "Configuring SQL connection string for function app" -ForegroundColor Blue
$ConnectionString = "Server=$SqlServer.database.windows.net,1433;Initial Catalog=$SqlDatabase;User Id=$SqlAdminUser;Password=$SqlAdminPassword;"
az webapp config connection-string set --name $FunctionApp --resource-group $ResourceGroupTI --settings TravelInspirationDbConnection=$ConnectionString --connection-string-type SQLAzure 
 
# Get the Application Insights instrumentation key
Write-Host "Retrieving Application Insights instrumentation key" -ForegroundColor Blue
$APPINSIGHTS_KEY = az resource show --resource-group $ResourceGroupMain --name $ApplicationInsights --resource-type "microsoft.insights/components" --query properties.InstrumentationKey --output tsv

# Configure Application Insights for client web app
Write-Host "Configuring Application Insights for client web app" -ForegroundColor Blue
az webapp config appsettings set --name $WebAppClient --resource-group $ResourceGroupTI --settings APPLICATIONINSIGHTS_CONNECTION_STRING="InstrumentationKey=$APPINSIGHTS_KEY" ApplicationInsightsAgent_EXTENSION_VERSION="~3"

# Configure Application Insights for JS-based client web app
Write-Host "Configuring Application Insights for JavaScript client web app" -ForegroundColor Blue
az webapp config appsettings set --name $WebAppClientJS --resource-group $ResourceGroupTI --settings APPLICATIONINSIGHTS_CONNECTION_STRING="InstrumentationKey=$APPINSIGHTS_KEY" ApplicationInsightsAgent_EXTENSION_VERSION="~3"

# Configure Application Insights for destinations API web app
Write-Host "Configuring Application Insights for destinations API web app" -ForegroundColor Blue
az webapp config appsettings set --name $WebAppDestinationsAPI --resource-group $ResourceGroupTI --settings APPLICATIONINSIGHTS_CONNECTION_STRING="InstrumentationKey=$APPINSIGHTS_KEY" ApplicationInsightsAgent_EXTENSION_VERSION="~3"

# Configure Application Insights for itineraries API function app
Write-Host "Configuring Application Insights for itineraries API function app" -ForegroundColor Blue
az webapp config appsettings set --name $FunctionApp --resource-group $ResourceGroupTI --settings APPLICATIONINSIGHTS_CONNECTION_STRING="InstrumentationKey=$APPINSIGHTS_KEY" ApplicationInsightsAgent_EXTENSION_VERSION="~3"

# Output the Application Insights connection string
Write-Host "Retrieving Application Insights connection string for development configuration" -ForegroundColor Green
$ConnectionString = az resource show --resource-group $ResourceGroupMain --name $ApplicationInsights --resource-type "microsoft.insights/components" --query properties.ConnectionString --output json

Write-Host "`nDeployment completed successfully!" -ForegroundColor Green
Write-Host "`nApplication Insights Connection String for development:" -ForegroundColor Yellow
Write-Host $ConnectionString -ForegroundColor Cyan
Write-Host "`nPlease note down this connection string for use in your application settings during development." -ForegroundColor Yellow
