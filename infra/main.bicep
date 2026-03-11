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

param appExists bool

// Tags that should be applied to all resources.
// 
// Note that 'azd-service-name' tags should be applied separately to service host resources.
// Example usage:
//   tags: union(tags, { 'azd-service-name': <service name in azure.yaml> })
var tags = {
  'azd-env-name': environmentName
}

// Organize resources in a resource group
resource rg 'Microsoft.Resources/resourceGroups@2021-04-01' = {
  name: 'rg-discordbot-${environmentName}'
  location: location
  tags: tags
}

module resources 'resources.bicep' = {
  scope: rg
  name: 'resources'
  params: {
    location: location
    foundryLocation: foundryLocation
    tags: tags
    discordToken: discordToken
    firstPassword: firstPassword
    tbaApiKey: tbaApiKey
    appExists: appExists
  }
}
output AZURE_CONTAINER_REGISTRY_ENDPOINT string = resources.outputs.AZURE_CONTAINER_REGISTRY_ENDPOINT
output AZURE_RESOURCE_APP_ID string = resources.outputs.AZURE_RESOURCE_APP_ID
output AZURE_AI_PROJECT_ENDPOINT string = resources.outputs.AZURE_AI_PROJECT_ENDPOINT
