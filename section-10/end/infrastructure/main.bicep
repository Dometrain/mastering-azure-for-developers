@description('The department')
param department string

@description('The environment name')
@allowed([
  'dev'
  'test'
  'prod'
])
param env string        

@description('The region for resources')
@allowed([
  'westeurope'
  'northeurope'
  'eastus'
  'westus'
  'centralus'
])
param location string = 'westeurope'

@description('The name of the itineraries function app')
param itinerariesFunctionAppName string 

@description('The name of the storage account')
param storageAccountName string 

@description('The name of the hosting plan')
param hostingPlanName string 

@description('The hosting plan tier for all function apps')
@allowed([
  'FlexConsumption'
  'Basic'
])
param functionHostingPlanTier string = 'FlexConsumption'

@description('The name of the resource group')
param resourceGroupName string 

var locationAbbreviations = {
  westeurope: 'we'
  northeurope: 'ne'
  eastus: 'eus'
  westus: 'wus'
  centralus: 'cus'
}

@description('The database connection string')
@secure()
param databaseConnectionString string

var locationAbbr = locationAbbreviations[location]
var typeSuffix = '${env}-${locationAbbr}'
var fullItinerariesFunctionAppName = '${department}-func-${typeSuffix}-${itinerariesFunctionAppName}'
var fullStorageAccountName = toLower(replace(storageAccountName , '-', ''))
var fullHostingPlanName = '${department}-plan-${typeSuffix}-${hostingPlanName}'
var fullResourceGroupName = '${department}-rg-${typeSuffix}-${resourceGroupName}'

targetScope = 'subscription'

// Create resource group at subscription scope
resource rg 'Microsoft.Resources/resourceGroups@2021-04-01' = {
  name: fullResourceGroupName
  location: location
}

module functionModule 'modules/func-app.bicep' = {
  name: 'itineraries-function-deployment'
  scope: rg
  params: {
    functionAppName: fullItinerariesFunctionAppName
    location: location
    hostingPlanTier: functionHostingPlanTier 
    hostingPlanName: fullHostingPlanName
    storageAccountName: fullStorageAccountName  
  }
}

var itinerariesFunctionAppId = functionModule.outputs.functionAppId


// module functionModuleForTokenEnrichment 'modules/func-app.bicep' = {
//   name: 'token-enrichment-function-deployment'
//   params: {
//     functionAppName: fullTokenEnrichmentFunctionAppName
//     location: location
//     hostingPlanTier: functionHostingPlanTier 
//     hostingPlanName: fullHostingPlanName
//     storageAccountName: fullStorageAccountName  
//   }
// }
