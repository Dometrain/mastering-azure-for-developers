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

@description('The name of the web application')
param webAppName string 

@description('The App Service Plan SKU')
@allowed([
  'F1'
  'B1'
  'S1'
  'P1v2'
])
param appServicePlanSku string = env == 'prod' ? 'P1v2' : 'F1'
 
var locationAbbreviations = {
  westeurope: 'we'
  northeurope: 'ne'
  eastus: 'eus'
  westus: 'wus'
  centralus: 'cus'
}

var locationAbbr = locationAbbreviations[location]
var typeSuffix = '${env}-${locationAbbr}'
var fullWebAppResourceName = '${department}-app-${typeSuffix}-${webAppName}'
var appServicePlanName = '${webAppName}-plan' 

resource appServicePlan 'Microsoft.Web/serverfarms@2023-01-01' = {
  name: appServicePlanName
  location: location
  sku: {
    name: appServicePlanSku
  }
  kind: 'app'
}

resource webApp 'Microsoft.Web/sites@2023-01-01' = {
  name: fullWebAppResourceName
  location: location
  kind: 'app'
  properties: {
    serverFarmId: appServicePlan.id
    httpsOnly: true
    siteConfig: {
      minTlsVersion: '1.2'
      ftpsState: 'Disabled'
      netFrameworkVersion: 'v9.0'
    }
  }
}

output webAppName string = webApp.name
output webAppId string = webApp.id
output webAppDefaultHostName string = webApp.properties.defaultHostName
output webAppUrl string = 'https://${webApp.properties.defaultHostName}'
output appServicePlanId string = appServicePlan.id