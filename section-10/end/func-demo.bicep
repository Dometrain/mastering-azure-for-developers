@description('The department')
param department string

@description('The environment name')
@allowed([
  'dev'
  'test'
  'prod'
])
param env string

@description('The region for the function app')
@allowed([
  'westeurope'
  'northeurope'
  'eastus'
  'westus'
  'centralus'
])
param location string = 'westeurope'

@description('The name of the function app')
param functionAppName string 

@description('The hosting plan tier')
@allowed([
  'FlexConsumption'
  'Basic'
])
param hostingPlanTier string = 'FlexConsumption'

var locationAbbreviations = {
  westeurope: 'we'
  northeurope: 'ne'
  eastus: 'eus'
  westus: 'wus'
  centralus: 'cus'
}

var locationAbbr = locationAbbreviations[location]
var typeSuffix = '${env}-${locationAbbr}'
var fullFunctionAppName = '${department}-func-${typeSuffix}-${functionAppName }'
var storageAccountName = toLower(replace(functionAppName , '-', ''))
var hostingPlanName = '${fullFunctionAppName}-plan'

resource storageAccount 'Microsoft.Storage/storageAccounts@2023-01-01' = {
  name: storageAccountName
  location: location
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'StorageV2'
  properties: {
    supportsHttpsTrafficOnly: true
    minimumTlsVersion: 'TLS1_2'
    allowBlobPublicAccess: false
  }
}

resource hostingPlan 'Microsoft.Web/serverfarms@2023-12-01' = {
  name: hostingPlanName
  location: location
  sku: hostingPlanTier == 'FlexConsumption' ? {
    name: 'FC1'
    tier: 'FlexConsumption'
  } : {
    name: 'B1'
    tier: 'Basic'
  }
  properties: {
    reserved: true // Required for Linux
  }
}

// Common app settings used by both hosting plan types
var commonAppSettings = [
  {
    name: 'AzureWebJobsStorage'
    value: 'DefaultEndpointsProtocol=https;AccountName=${storageAccount.name};AccountKey=${storageAccount.listKeys().keys[0].value};EndpointSuffix=${environment().suffixes.storage}'
  }
]
  
// Additional app settings for Basic plan only
var basicPlanAppSettings = [
  {
    name: 'WEBSITE_CONTENTAZUREFILECONNECTIONSTRING'
    value: 'DefaultEndpointsProtocol=https;AccountName=${storageAccount.name};AccountKey=${storageAccount.listKeys().keys[0].value};EndpointSuffix=${environment().suffixes.storage}'
  }
  {
    name: 'WEBSITE_CONTENTSHARE'
    value: toLower(functionAppName)
  }
  {
    name: 'FUNCTIONS_WORKER_RUNTIME'
    value: 'dotnet-isolated'
  }
  {
    name: 'FUNCTIONS_EXTENSION_VERSION'
    value: '~4'
  }
]

resource functionApp 'Microsoft.Web/sites@2023-12-01' = {
  name: fullFunctionAppName
  location: location
  kind: hostingPlanTier == 'FlexConsumption' 
    ? 'functionapp,linux,container,azurecontainerapps' : 'functionapp,linux'
  properties: {
    serverFarmId: hostingPlan.id
    httpsOnly: true
    siteConfig: {
      linuxFxVersion: hostingPlanTier == 'FlexConsumption' ? null : 'DOTNET-ISOLATED|9.0'
      appSettings: hostingPlanTier == 'FlexConsumption' ? commonAppSettings : concat(commonAppSettings, basicPlanAppSettings)

    }    
    functionAppConfig: hostingPlanTier == 'FlexConsumption' ? {
      deployment: {
        storage: {
          type: 'blobContainer'
          value: '${storageAccount.properties.primaryEndpoints.blob}deploymentpackage'
          authentication: {
            type: 'StorageAccountConnectionString'
            storageAccountConnectionStringName: 'AzureWebJobsStorage'
          }
        }
      }  
      scaleAndConcurrency: {
           maximumInstanceCount: 100
           instanceMemoryMB: 2048
         }
      runtime: {
        name: 'dotnet-isolated'
        version: '9.0'
      }
    } : null
  }
}


output functionAppName string = functionApp.name
output functionAppId string = functionApp.id
output functionAppDefaultHostName string = functionApp.properties.defaultHostName