@description('Primary deployment location for shared platform resources.')
param location string

@description('Tags applied to all shared platform resources.')
param tags object

@description('Name for the user-assigned managed identity used by container apps.')
param appIdentityName string

@description('Name for the storage account used by the function app.')
param storageAccountName string

@description('Name for the Log Analytics workspace.')
param logAnalyticsName string

@description('Name for the Application Insights resource.')
param applicationInsightsName string

@description('Name for the Application Insights dashboard.')
param applicationInsightsDashboardName string

@description('Name for the Azure Container Registry.')
param containerRegistryName string

@description('Name for the Container Apps managed environment.')
param containerAppsEnvironmentName string

var storageBlobDataContributorRoleDefinitionId = 'ba92f5b4-2d11-453d-a403-e96b0029c9fe'
var storageQueueDataContributorRoleDefinitionId = '974c5e8b-45b9-4653-ba55-5f855dd0fb88'
var storageTableDataContributorRoleDefinitionId = '0a9a7e1f-b9d0-4cc4-a60d-0319b160aaa3'
var storageAccountResourceId = resourceId('Microsoft.Storage/storageAccounts', storageAccountName)

module monitoring 'br/public:avm/ptn/azd/monitoring:0.2.1' = {
  name: 'monitoring'
  params: {
    logAnalyticsName: logAnalyticsName
    applicationInsightsName: applicationInsightsName
    applicationInsightsDashboardName: applicationInsightsDashboardName
    location: location
    tags: tags
  }
}

module appIdentity 'br/public:avm/res/managed-identity/user-assigned-identity:0.5.0' = {
  name: 'appidentity'
  params: {
    name: appIdentityName
    location: location
  }
}

module containerRegistry 'br/public:avm/res/container-registry/registry:0.12.0' = {
  name: 'registry'
  params: {
    name: containerRegistryName
    location: location
    tags: tags
    publicNetworkAccess: 'Enabled'
    roleAssignments: [
      {
        principalId: appIdentity.outputs.principalId
        principalType: 'ServicePrincipal'
        roleDefinitionIdOrName: subscriptionResourceId(
          'Microsoft.Authorization/roleDefinitions',
          '7f951dda-4ed3-4680-a7ca-43fe172d538d'
        )
      }
    ]
  }
}

module containerAppsEnvironment 'br/public:avm/res/app/managed-environment:0.13.1' = {
  name: 'container-apps-environment'
  params: {
    name: containerAppsEnvironmentName
    location: location
    zoneRedundant: false
    appInsightsConnectionString: monitoring.outputs.applicationInsightsConnectionString
  }
}

module storageAccount 'br/public:avm/res/storage/storage-account:0.32.0' = {
  name: 'storage-account'
  params: {
    name: storageAccountName
    location: location
    tags: tags
    kind: 'StorageV2'
    skuName: 'Standard_LRS'
    accessTier: 'Hot'
    allowBlobPublicAccess: false
    allowSharedKeyAccess: true
    publicNetworkAccess: 'Enabled'
    minimumTlsVersion: 'TLS1_2'
    supportsHttpsTrafficOnly: true
    networkAcls: {
      bypass: 'AzureServices'
      defaultAction: 'Allow'
    }
    roleAssignments: [
      {
        name: guid(storageAccountResourceId, appIdentityName, storageBlobDataContributorRoleDefinitionId)
        principalId: appIdentity.outputs.principalId
        principalType: 'ServicePrincipal'
        roleDefinitionIdOrName: subscriptionResourceId(
          'Microsoft.Authorization/roleDefinitions',
          storageBlobDataContributorRoleDefinitionId
        )
      }
      {
        name: guid(storageAccountResourceId, appIdentityName, storageQueueDataContributorRoleDefinitionId)
        principalId: appIdentity.outputs.principalId
        principalType: 'ServicePrincipal'
        roleDefinitionIdOrName: subscriptionResourceId(
          'Microsoft.Authorization/roleDefinitions',
          storageQueueDataContributorRoleDefinitionId
        )
      }
      {
        name: guid(storageAccountResourceId, appIdentityName, storageTableDataContributorRoleDefinitionId)
        principalId: appIdentity.outputs.principalId
        principalType: 'ServicePrincipal'
        roleDefinitionIdOrName: subscriptionResourceId(
          'Microsoft.Authorization/roleDefinitions',
          storageTableDataContributorRoleDefinitionId
        )
      }
    ]
  }
}

var storageServiceEndpoints = storageAccount.outputs.serviceEndpoints

output appIdentityClientId string = appIdentity.outputs.clientId
output appIdentityPrincipalId string = appIdentity.outputs.principalId
output appIdentityResourceId string = appIdentity.outputs.resourceId
output applicationInsightsConnectionString string = monitoring.outputs.applicationInsightsConnectionString
output containerAppsEnvironmentResourceId string = containerAppsEnvironment.outputs.resourceId
output containerRegistryLoginServer string = containerRegistry.outputs.loginServer
output storageAccountName string = storageAccount.outputs.name
output storageBlobEndpoint string = storageAccount.outputs.primaryBlobEndpoint
output storageQueueEndpoint string = storageServiceEndpoints.?queue ?? 'https://${storageAccount.outputs.name}.queue.${environment().suffixes.storage}/'
output storageTableEndpoint string = storageServiceEndpoints.?table ?? 'https://${storageAccount.outputs.name}.table.${environment().suffixes.storage}/'
