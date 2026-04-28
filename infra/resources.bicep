type DeploymentConfig = {
  location: string
  foundryLocation: string
  tags: object
  appExists: bool
}

type AiFeatureConfig = {
  chatBotAgentId: string
  chatBotAgent365Enabled: string
  chatBotAgent365TenantId: string
  chatBotAgent365BlueprintClientId: string
  chatBotAgent365ManagedIdentityClientId: string
  chatBotAgent365AgentIdentityClientId: string
  chatBotAgent365AutoCreateIdentity: string
  chatBotAgent365AgentIdentityDisplayName: string
  chatBotAgent365Sponsors: string
  chatBotAgent365TokenExchangeAudience: string
  chatBotAgent365ProbeScope: string
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
var chatBotAgent365Enabled = ai.chatBotAgent365Enabled
var chatBotAgent365TenantId = empty(ai.chatBotAgent365TenantId) ? tenant().tenantId : ai.chatBotAgent365TenantId
var chatBotAgent365BlueprintClientId = ai.chatBotAgent365BlueprintClientId
var chatBotAgent365ManagedIdentityClientId = ai.chatBotAgent365ManagedIdentityClientId
var chatBotAgent365AgentIdentityClientId = ai.chatBotAgent365AgentIdentityClientId
var chatBotAgent365AutoCreateIdentity = ai.chatBotAgent365AutoCreateIdentity
var chatBotAgent365AgentIdentityDisplayName = ai.chatBotAgent365AgentIdentityDisplayName
var chatBotAgent365Sponsors = ai.chatBotAgent365Sponsors
var chatBotAgent365TokenExchangeAudience = ai.chatBotAgent365TokenExchangeAudience
var chatBotAgent365ProbeScope = ai.chatBotAgent365ProbeScope
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
        name: 'AI__Agent365__Enabled'
        value: chatBotAgent365Enabled
      }
      {
        name: 'AI__Agent365__TenantId'
        value: chatBotAgent365TenantId
      }
      {
        name: 'AI__Agent365__BlueprintClientId'
        value: chatBotAgent365BlueprintClientId
      }
      {
        name: 'AI__Agent365__ManagedIdentityClientId'
        value: empty(chatBotAgent365ManagedIdentityClientId) ? platformCore.outputs.appIdentityClientId : chatBotAgent365ManagedIdentityClientId
      }
      {
        name: 'AI__Agent365__AgentIdentityClientId'
        value: chatBotAgent365AgentIdentityClientId
      }
      {
        name: 'AI__Agent365__AutoCreateIdentity'
        value: chatBotAgent365AutoCreateIdentity
      }
      {
        name: 'AI__Agent365__AgentIdentityDisplayName'
        value: chatBotAgent365AgentIdentityDisplayName
      }
      {
        name: 'AI__Agent365__Sponsors'
        value: chatBotAgent365Sponsors
      }
      {
        name: 'AI__Agent365__TokenExchangeAudience'
        value: chatBotAgent365TokenExchangeAudience
      }
      {
        name: 'AI__Agent365__ProbeScope'
        value: chatBotAgent365ProbeScope
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
