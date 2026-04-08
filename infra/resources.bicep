type DeploymentConfig = {
  location: string
  foundryLocation: string
  tags: object
  appExists: bool
}

type AiFeatureConfig = {
  chatBotAgentId: string
  searchServiceName: string
  searchLocation: string
}

@description('Core deployment settings shared across infrastructure modules.')
param deployment DeploymentConfig

@secure()
@description('Secrets injected into the function container app.')
param secrets object

@description('Optional AI/search feature configuration.')
param ai AiFeatureConfig

var location = deployment.location
var foundryLocation = deployment.foundryLocation
var tags = deployment.tags
var appExists = deployment.appExists
var chatBotAgentId = ai.chatBotAgentId
var searchServiceName = ai.searchServiceName
var searchLocation = ai.searchLocation

var abbrs = loadJsonContent('./abbreviations.json')
var resourceToken = uniqueString(subscription().id, resourceGroup().id, location)
var appIdentityName = '${abbrs.managedIdentityUserAssignedIdentities}discordbot-${resourceToken}'
var storageAccountName = '${abbrs.storageStorageAccounts}discord${take(resourceToken, 15)}'
var defaultFunctionsImage = 'mcr.microsoft.com/azure-functions/dotnet-isolated:4-dotnet-isolated10.0'
var hostedChatEnabled = !empty(chatBotAgentId)
var searchServiceEnabled = !empty(searchServiceName)

module platformCore './modules/platform-core.bicep' = {
  name: 'platform-core'
  params: {
    location: location
    tags: tags
    appIdentityName: appIdentityName
    storageAccountName: storageAccountName
    logAnalyticsName: '${abbrs.operationalInsightsWorkspaces}${resourceToken}'
    applicationInsightsName: '${abbrs.insightsComponents}${resourceToken}'
    applicationInsightsDashboardName: '${abbrs.portalDashboards}${resourceToken}'
    containerRegistryName: '${abbrs.containerRegistryRegistries}${resourceToken}'
    containerAppsEnvironmentName: '${abbrs.appManagedEnvironments}${resourceToken}'
  }
}

module foundry './modules/foundry.bicep' = {
  name: 'foundry'
  params: {
    location: foundryLocation
    tags: tags
    appIdentityPrincipalId: platformCore.outputs.appIdentityPrincipalId
    resourceToken: resourceToken
    searchServiceResourceId: searchServiceEnabled ? searchService!.outputs.resourceId : null
  }
}

module searchService 'br/public:avm/res/search/search-service:0.12.0' = if (searchServiceEnabled) {
  name: 'search-service'
  params: {
    name: searchServiceName
    location: searchLocation
    tags: tags
    managedIdentities: {
      systemAssigned: true
    }
    sku: 'free'
    authOptions: {
      aadOrApiKey: {
        aadAuthFailureMode: 'http401WithBearerChallenge'
      }
    }
    disableLocalAuth: false
    hostingMode: 'Default'
    networkRuleSet: {
      bypass: 'None'
      ipRules: []
    }
    partitionCount: 1
    publicNetworkAccess: 'Enabled'
    replicaCount: 1
    semanticSearch: 'free'
  }
}

var baseContainerEnv = [
  {
    name: 'FUNCTIONS_WORKER_RUNTIME'
    value: 'dotnet-isolated'
  }
  {
    name: 'APPLICATIONINSIGHTS_CONNECTION_STRING'
    value: platformCore.outputs.applicationInsightsConnectionString
  }
  {
    name: 'Discord__Token'
    secretRef: 'discord-token'
  }
  {
    name: 'FRC__Password'
    secretRef: 'first-password'
  }
  {
    name: 'FRC__Username'
    value: 'bc3tech'
  }
  {
    name: 'TbaApiKey'
    secretRef: 'tba-api-key'
  }
  {
    name: 'Azure__ClientId'
    value: platformCore.outputs.appIdentityClientId
  }
  {
    name: 'AzureWebJobsStorage__accountName'
    value: platformCore.outputs.storageAccountName
  }
  {
    name: 'AzureWebJobsStorage__blobServiceUri'
    value: platformCore.outputs.storageBlobEndpoint
  }
  {
    name: 'AzureWebJobsStorage__queueServiceUri'
    value: platformCore.outputs.storageQueueEndpoint
  }
  {
    name: 'AzureWebJobsStorage__tableServiceUri'
    value: platformCore.outputs.storageTableEndpoint
  }
  {
    name: 'AzureWebJobsStorage__credential'
    value: 'managedidentity'
  }
  {
    name: 'AzureWebJobsStorage__clientId'
    value: platformCore.outputs.appIdentityClientId
  }
]

var hostedChatContainerEnv = hostedChatEnabled
  ? [
      {
        name: 'AZURE_CLIENT_ID'
        value: platformCore.outputs.appIdentityClientId
      }
      {
        name: 'AZURE_TENANT_ID'
        value: tenant().tenantId
      }
      {
        name: 'AI__Azure__ProjectEndpoint'
        value: foundry.outputs.projectEndpoint
      }
      {
        name: 'AI__Azure__AgentId'
        value: chatBotAgentId
      }
      {
        name: 'Azure__TenantId'
        value: tenant().tenantId
      }
    ]
  : []

var diagnosticsContainerEnv = [
  {
    name: 'Logging__LogLevel__Default'
    value: 'Debug'
  }
  {
    name: 'AzureWebjobs.Heartbeat.Disabled'
    value: '1'
  }
]

var containerEnv = concat(baseContainerEnv, hostedChatContainerEnv, diagnosticsContainerEnv)
var containerAppName = '${abbrs.appContainerApps}discordbot-${resourceToken}'

module app './modules/function-app.bicep' = {
  name: 'function-app'
  params: {
    name: containerAppName
    location: location
    tags: tags
    appExists: appExists
    managedEnvironmentId: platformCore.outputs.containerAppsEnvironmentResourceId
    containerRegistryServer: platformCore.outputs.containerRegistryLoginServer
    appIdentityResourceId: platformCore.outputs.appIdentityResourceId
    containerEnv: containerEnv
    discordToken: secrets.discordToken
    firstPassword: secrets.firstPassword
    tbaApiKey: secrets.tbaApiKey
    defaultFunctionsImage: defaultFunctionsImage
  }
}

output AZURE_CONTAINER_REGISTRY_ENDPOINT string = platformCore.outputs.containerRegistryLoginServer
output AZURE_RESOURCE_APP_ID string = app.outputs.appResourceId
output AZURE_AI_PROJECT_ENDPOINT string = foundry.outputs.projectEndpoint
output AZURE_AI_SEARCH_ENDPOINT string = searchServiceEnabled ? searchService!.outputs.endpoint : ''
