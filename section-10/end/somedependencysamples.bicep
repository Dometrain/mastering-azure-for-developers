// param location string

// resource appServicePlan 'Microsoft.Web/serverfarms@2023-01-01' = {
//   name: 'my-plan'
//   location: location
//   sku: {
//     name: 'B1'
//   }
// }

// resource webApp 'Microsoft.Web/sites@2023-01-01' = {
//   name: 'my-webapp'
//   location: location  
//   properties: {
//     // This creates an implicit dependency
//     // webApp depends on appServicePlan
//     serverFarmId: appServicePlan.id
//   }
// }

param location string

resource storageAccount 'Microsoft.Storage/storageAccounts@2023-01-01' = {
  name: 'mystorageaccount'
  location: location
  kind: 'StorageV2'
  sku: {
    name: 'Standard_LRS'
  }
}

resource deploymentScript 'Microsoft.Resources/deploymentScripts@2020-10-01' = {
  name: 'initialize-storage'
  location: location
  kind: 'AzureCLI'
  properties: {
    azCliVersion: '2.40.0'
    scriptContent: '''
      az storage container create --name data --account-name mystorageaccount
    '''
    retentionInterval: 'PT1H'
  }
  // Explicit dependency needed - the script references the storage account
  // by name string, not by property, so Bicep can't detect the dependency
  dependsOn: [
    storageAccount
  ]
}