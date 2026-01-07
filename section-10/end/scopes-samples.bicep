// // storage.bicep - deployed to resource group scope
// param location string = resourceGroup().location
// resource storageAccount 'Microsoft.Storage/storageAccounts@2023-01-01' = {
//   name: 'mystorage'
//   location: location
//   // ...
// }

// az deployment group create --resource-group my-rg --template-file storage.bicep

// targetScope = 'subscription'

// param location string = 'westeurope'

// resource resourceGroup 'Microsoft.Resources/resourceGroups@2021-04-01' = {
//   name: 'my-new-rg'
//   location: location
// }

// az deployment sub create --location westeurope --template-file subscription.bicep

// targetScope = 'subscription'
// param location string = 'westeurope'
// param projectName string
// resource resourceGroup 'Microsoft.Resources/resourceGroups@2021-04-01' = {
//   name: '${projectName}-rg'
//   location: location
// }

// module storage 'storage.bicep' = {
//   name: 'storage-deployment'
//   scope: resourceGroup
//   params: {
//     storageAccountName: '${projectName}storage'
//   }
// }

// targetScope = 'managementGroup'
// // Policy assignment across multiple subscriptions
// resource policyAssignment 'Microsoft.Authorization/policyAssignments@2021-06-01' = {
//   name: 'enforce-tagging'
//   // ...
// }


// targetScope = 'tenant'
// resource mgName_resource 'Microsoft.Management/managementGroups@2024-02-01-preview' = 
// { ... } 
