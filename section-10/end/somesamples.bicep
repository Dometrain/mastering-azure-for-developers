// param stringParam string 
// param intParam int
// param boolParam bool
// param objectParam object
// param arrayParam array

// @description('The name of the web app')
// @minLength(3)
// @maxLength(60)
// param webAppName string

// @description('The location for all resources')
// @allowed([
//   'eastus'
//   'westeurope'
//   'westus2'
// ])
// param location string = 'westeurope'

// @description('The App Service Plan SKU')
// @allowed([
//   'F1'
//   'B1'
//   'B2'
//   'S1'
//   'P1v2'
// ])
// param appServicePlanSku string = 'F1'

// @minValue(1)
// @maxValue(10)
// param instanceCount int = 1

// @secure()
// param adminPassword string 


// param webAppName string
// param environment string

// var appServicePlanName = '${webAppName}-plan'
// var location = resourceGroup().location
// var tags = {
//   Environment: environment
//   ManagedBy: 'Bicep'
//   CostCenter: 'Engineering'
// }


// param department string
// param environment string
// param location string 

// var typeSuffix = '${environment}-${location}'
// var tiClientWebAppName = '${department}-app-${typeSuffix}-ticlient-i9e5m2-new'
// var tiItinerariesApiFunctionName = '${department}-func-${typeSuffix}-tiitinapi-i9e5m2'

param webAppName string
param location string

resource webApp 'Microsoft.Web/sites@2023-01-01' = {
  name: webAppName
  location: location
  // ... rest of configuration
}

output webAppId string = webApp.id
output webAppName string = webApp.name
output webAppDefaultHostName string = webApp.properties.defaultHostName
output webAppUrl string = 'https://${webApp.properties.defaultHostName}'

 
// In infrastructure.bicep
output webAppNameOutput string = webApp.name

// Then in your deployment script:
// First deploy infrastructure
// Then use output as parameter for app deployment