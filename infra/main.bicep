targetScope = 'subscription'

@minLength(1)
@maxLength(64)
@description('Name of the environment that can be used as part of naming resource convention')
param environmentName string

@minLength(1)
@description('Primary location for all resources')
param location string

@minLength(1)
@description('Location for Azure AI Foundry resources. Defaults to the primary deployment location.')
param foundryLocation string = location

@secure()
@description('Discord bot token injected into the container app as a secret.')
param discordToken string

@secure()
@description('FIRST API password injected into the container app as a secret.')
param firstPassword string

@secure()
@description('The Blue Alliance API key injected into the container app as a secret.')
param tbaApiKey string

@description('Optional Azure AI Foundry agent ID injected into the container app only when set. Leave empty to keep hosted chat settings disabled.')
param chatBotAgentId string = ''

@description('Optional toggle for Agent365 observability and agent identity wiring. Set to true to enable.')
param chatBotAgent365Enabled string = ''

@description('Optional Microsoft Entra tenant ID used for Agent365 identity flows. Defaults to the deployment tenant when empty.')
param chatBotAgent365TenantId string = ''

@description('Optional Agent Identity Blueprint app (client) ID used for Agent365 token exchange.')
param chatBotAgent365BlueprintClientId string = ''

@description('Optional managed identity client ID used for Agent365 token exchange. Defaults to the function app managed identity when empty.')
param chatBotAgent365ManagedIdentityClientId string = ''

@description('Optional pre-created Agent Identity app (client) ID. Leave empty to rely on auto-creation when enabled.')
param chatBotAgent365AgentIdentityClientId string = ''

@description('Optional toggle controlling Agent365 identity auto-creation when no agent identity client ID is supplied.')
param chatBotAgent365AutoCreateIdentity string = ''

@description('Optional display name used when Agent365 identity auto-creation is enabled.')
param chatBotAgent365AgentIdentityDisplayName string = ''

@description('Optional Agent365 sponsor identifiers as a delimited string (comma, semicolon, or newline).')
param chatBotAgent365Sponsors string = ''

@description('Optional Agent365 token exchange audience. Defaults to api://AzureADTokenExchange when empty.')
param chatBotAgent365TokenExchangeAudience string = ''

@description('Optional Agent365 probe scope used to validate identity token acquisition. Defaults to Microsoft Graph .default scope when empty.')
param chatBotAgent365ProbeScope string = ''

@description('Optional Azure AI Search service name used by the Foundry knowledge-base integration. Leave empty to skip provisioning the search service.')
param searchServiceName string = ''

@description('Location for the optional Azure AI Search service. Defaults to the Foundry location.')
param searchLocation string = foundryLocation

param appExists bool

var deploymentConfig = {
  location: location
  foundryLocation: foundryLocation
  tags: tags
  appExists: appExists
}

var appSecrets = {
  discordToken: discordToken
  firstPassword: firstPassword
  tbaApiKey: tbaApiKey
}

var aiFeatureConfig = {
  chatBotAgentId: chatBotAgentId
  chatBotAgent365Enabled: chatBotAgent365Enabled
  chatBotAgent365TenantId: chatBotAgent365TenantId
  chatBotAgent365BlueprintClientId: chatBotAgent365BlueprintClientId
  chatBotAgent365ManagedIdentityClientId: chatBotAgent365ManagedIdentityClientId
  chatBotAgent365AgentIdentityClientId: chatBotAgent365AgentIdentityClientId
  chatBotAgent365AutoCreateIdentity: chatBotAgent365AutoCreateIdentity
  chatBotAgent365AgentIdentityDisplayName: chatBotAgent365AgentIdentityDisplayName
  chatBotAgent365Sponsors: chatBotAgent365Sponsors
  chatBotAgent365TokenExchangeAudience: chatBotAgent365TokenExchangeAudience
  chatBotAgent365ProbeScope: chatBotAgent365ProbeScope
  searchServiceName: searchServiceName
  searchLocation: searchLocation
}

// Tags that should be applied to all resources.
// 
// Note that 'azd-service-name' tags should be applied separately to service host resources.
// Example usage:
//   tags: union(tags, { 'azd-service-name': <service name in azure.yaml> })
var tags = {
  'azd-env-name': environmentName
}

// Organize resources in a resource group
resource rg 'Microsoft.Resources/resourceGroups@2025-04-01' = {
  name: 'rg-discordbot-${environmentName}'
  location: location
  tags: tags
}

module resources 'resources.bicep' = {
  scope: rg
  name: 'resources'
  params: {
    deployment: deploymentConfig
    secrets: appSecrets
    ai: aiFeatureConfig
  }
}
output AZURE_CONTAINER_REGISTRY_ENDPOINT string = resources.outputs.AZURE_CONTAINER_REGISTRY_ENDPOINT
output AZURE_RESOURCE_APP_ID string = resources.outputs.AZURE_RESOURCE_APP_ID
output AZURE_AI_PROJECT_ENDPOINT string = resources.outputs.AZURE_AI_PROJECT_ENDPOINT
output AZURE_AI_SEARCH_ENDPOINT string = resources.outputs.AZURE_AI_SEARCH_ENDPOINT
