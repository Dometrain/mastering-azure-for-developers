using '../main.bicep'

param department = 'appdev'
param env = 'dev'
param location = 'westeurope'
param itinerariesFunctionAppName = 'itindemo8d9lm'
param storageAccountName = 'itindemo8d9lmsa'
param hostingPlanName = 'itindemo8d9lm-plan'
param functionHostingPlanTier = 'FlexConsumption'
param resourceGroupName = 'moduledemo'
param databaseConnectionString = getSecret(  
    '979324e5-17ea-450e-bed2-26d05ec4d43e',
    'appdev-rg-dev-we-ti-i9e5m2',
    'appdev-kv-dev-we-i9e5m2',
    'TravelInspirationDbConnection')