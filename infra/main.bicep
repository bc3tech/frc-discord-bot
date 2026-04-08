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
