param webapp_name string = 'appdev-app-dev-we-ticlient-i9e5m2-new-willbeoverridden'
param webapp_location string = 'West Europe'
param webapp_plan_serverfarm_externalid string = '/subscriptions/979324e5-17ea-450e-bed2-26d05ec4d43e/resourceGroups/appdev-rg-dev-we-ti-i9e5m2/providers/Microsoft.Web/serverfarms/appdev-plan-dev-we-ti-i9e5m2'

resource webapp_name_resource 'Microsoft.Web/sites@2023-01-01' = {
  name: webapp_name
  location: webapp_location
  kind: 'app'
  properties: {
    serverFarmId: webapp_plan_serverfarm_externalid
    httpsOnly: true
    siteConfig: {
      minTlsVersion: '1.2'
      ftpsState: 'Disabled'
      netFrameworkVersion: 'v9.0'
    }
  }
}
