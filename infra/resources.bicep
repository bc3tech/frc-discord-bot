@description('The location used for all deployed resources')
param location string = resourceGroup().location

@description('The location used for Azure AI Foundry resources')
param foundryLocation string = location

@description('Tags that will be applied to all resources')
param tags object = {
  'solution-name': 'frc-discord-bot'
}

@secure()
@description('Discord bot token injected into the container app as a secret')
param discordToken string

@secure()
@description('FIRST API password injected into the container app as a secret')
param firstPassword string

@secure()
@description('The Blue Alliance API key injected into the container app as a secret')
param tbaApiKey string

param appExists bool

var abbrs = loadJsonContent('./abbreviations.json')
var resourceToken = uniqueString(subscription().id, resourceGroup().id, location)
var appIdentityName = '${abbrs.managedIdentityUserAssignedIdentities}discordbot-${resourceToken}'
var appIdentityResourceId = resourceId('Microsoft.ManagedIdentity/userAssignedIdentities', appIdentityName)
var storageAccountName = '${abbrs.storageStorageAccounts}discord${take(resourceToken, 15)}'
var storageBlobDataContributorRoleDefinitionId = 'ba92f5b4-2d11-453d-a403-e96b0029c9fe'
var storageQueueDataContributorRoleDefinitionId = '974c5e8b-45b9-4653-ba55-5f855dd0fb88'
var storageTableDataContributorRoleDefinitionId = '0a9a7e1f-b9d0-4cc4-a60d-0319b160aaa3'

// Monitor application with Azure Monitor
module monitoring 'br/public:avm/ptn/azd/monitoring:0.1.0' = {
  name: 'monitoring'
  params: {
    logAnalyticsName: '${abbrs.operationalInsightsWorkspaces}${resourceToken}'
    applicationInsightsName: '${abbrs.insightsComponents}${resourceToken}'
    applicationInsightsDashboardName: '${abbrs.portalDashboards}${resourceToken}'
    location: location
    tags: tags
  }
}
// Container registry
module containerRegistry 'br/public:avm/res/container-registry/registry:0.1.1' = {
  name: 'registry'
  params: {
    name: '${abbrs.containerRegistryRegistries}${resourceToken}'
    location: location
    tags: tags
    publicNetworkAccess: 'Enabled'
    roleAssignments:[
      {
        principalId: appIdentity.outputs.principalId
        principalType: 'ServicePrincipal'
        roleDefinitionIdOrName: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', '7f951dda-4ed3-4680-a7ca-43fe172d538d')
      }
    ]
  }
}

// Container apps environment
module containerAppsEnvironment 'br/public:avm/res/app/managed-environment:0.4.5' = {
  name: 'container-apps-environment'
  params: {
    logAnalyticsWorkspaceResourceId: monitoring.outputs.logAnalyticsWorkspaceResourceId
    name: '${abbrs.appManagedEnvironments}${resourceToken}'
    location: location
    zoneRedundant: false
  }
}

module appIdentity 'br/public:avm/res/managed-identity/user-assigned-identity:0.2.1' = {
  name: 'appidentity'
  params: {
    name: appIdentityName
    location: location
  }
}

resource storageAccount 'Microsoft.Storage/storageAccounts@2025-06-01' = {
  name: storageAccountName
  location: location
  kind: 'StorageV2'
  sku: {
    name: 'Standard_LRS'
  }
  tags: tags
  properties: {
    accessTier: 'Hot'
    allowBlobPublicAccess: false
    allowSharedKeyAccess: false
    publicNetworkAccess: 'Enabled'
    minimumTlsVersion: 'TLS1_2'
    supportsHttpsTrafficOnly: true
    networkAcls: {
      bypass: 'AzureServices'
      defaultAction: 'Allow'
    }
  }
}

resource storageBlobDataContributorAssignment 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(storageAccount.id, appIdentityName, storageBlobDataContributorRoleDefinitionId)
  scope: storageAccount
  properties: {
    principalId: appIdentity.outputs.principalId
    principalType: 'ServicePrincipal'
    roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', storageBlobDataContributorRoleDefinitionId)
  }
}

resource storageQueueDataContributorAssignment 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(storageAccount.id, appIdentityName, storageQueueDataContributorRoleDefinitionId)
  scope: storageAccount
  properties: {
    principalId: appIdentity.outputs.principalId
    principalType: 'ServicePrincipal'
    roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', storageQueueDataContributorRoleDefinitionId)
  }
}

resource storageTableDataContributorAssignment 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(storageAccount.id, appIdentityName, storageTableDataContributorRoleDefinitionId)
  scope: storageAccount
  properties: {
    principalId: appIdentity.outputs.principalId
    principalType: 'ServicePrincipal'
    roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', storageTableDataContributorRoleDefinitionId)
  }
}

module foundry './modules/foundry.bicep' = {
  name: 'foundry'
  params: {
    location: foundryLocation
    tags: tags
    appIdentityPrincipalId: appIdentity.outputs.principalId
    resourceToken: resourceToken
  }
}

var containerAppName = '${abbrs.appContainerApps}discordbot-${resourceToken}'
module appFetchLatestImage './modules/fetch-container-image.bicep' = {
  name: 'app-fetch-image'
  params: {
    exists: appExists
    name: containerAppName
  }
}

resource app 'Microsoft.App/containerApps@2025-10-02-preview' = {
  name: containerAppName
  kind: 'functionapp'
  location: location
  tags: union(tags, { 'azd-service-name': 'app' })
  identity: {
    type: 'UserAssigned'
    userAssignedIdentities: {
      '${appIdentityResourceId}': {}
    }
  }
  properties: {
    managedEnvironmentId: containerAppsEnvironment.outputs.resourceId
    configuration: {
      activeRevisionsMode: 'Single'
      ingress: {
        allowInsecure: true
        external: true
        targetPort: 80
        transport: 'Auto'
      }
      registries: [
        {
          server: containerRegistry.outputs.loginServer
          identity: appIdentityResourceId
        }
      ]
      secrets: [
        {
          name: 'discord-token'
          value: discordToken
        }
        {
          name: 'first-password'
          value: firstPassword
        }
        {
          name: 'tba-api-key'
          value: tbaApiKey
        }
      ]
    }
    template: {
      containers: [
        {
          image: appFetchLatestImage.outputs.?containers[?0].?image ?? 'mcr.microsoft.com/azuredocs/containerapps-helloworld:latest'
          name: 'main'
          resources: {
            cpu: json('0.5')
            memory: '1.0Gi'
          }
          env: [
            {
              name: 'APPLICATIONINSIGHTS_CONNECTION_STRING'
              value: monitoring.outputs.applicationInsightsConnectionString
            }
            {
              name: 'Discord__Token'
              secretRef: 'discord-token'
            }
            {
              name: 'FIRST__Password'
              secretRef: 'first-password'
            }
            {
              name: 'FIRST__Username'
              value: 'bc3tech'
            }
            {
              name: 'TbaApiKey'
              secretRef: 'tba-api-key'
            }
            {
              name: 'AZURE_CLIENT_ID'
              value: appIdentity.outputs.clientId
            }
            {
              name: 'AzureWebJobsStorage__accountName'
              value: storageAccount.name
            }
            {
              name: 'AzureWebJobsStorage__blobServiceUri'
              value: storageAccount.properties.primaryEndpoints.blob
            }
            {
              name: 'AzureWebJobsStorage__queueServiceUri'
              value: storageAccount.properties.primaryEndpoints.queue
            }
            {
              name: 'AzureWebJobsStorage__tableServiceUri'
              value: storageAccount.properties.primaryEndpoints.table
            }
            {
              name: 'AzureWebJobsStorage__credential'
              value: 'managedidentity'
            }
            {
              name: 'AzureWebJobsStorage__clientId'
              value: appIdentity.outputs.clientId
            }
            {
              name: 'AZURE_TENANT_ID'
              value: tenant().tenantId
            }
            {
              name: 'Azure__Storage__BlobsEndpoint'
              value: storageAccount.properties.primaryEndpoints.blob
            }
            {
              name: 'Azure__Storage__TableEndpoint'
              value: storageAccount.properties.primaryEndpoints.table
            }
            {
              name: 'Azure__AI__Project__Endpoint'
              value: foundry.outputs.projectEndpoint
            }
            {
              name: 'Azure__ClientId'
              value: appIdentity.outputs.clientId
            }
            {
              name: 'Azure__TenantId'
              value: tenant().tenantId
            }
          ]
        }
      ]
      scale: {
        minReplicas: 0
        maxReplicas: 1
      }
    }
  }
}
output AZURE_CONTAINER_REGISTRY_ENDPOINT string = containerRegistry.outputs.loginServer
output AZURE_RESOURCE_APP_ID string = app.id
output AZURE_AI_PROJECT_ENDPOINT string = foundry.outputs.projectEndpoint
