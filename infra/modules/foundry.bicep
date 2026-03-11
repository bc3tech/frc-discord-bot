@description('The location used for Azure AI Foundry resources')
param location string

@description('Tags that will be applied to Foundry resources')
param tags object

@description('The principal ID of the app managed identity receiving Foundry RBAC')
param appIdentityPrincipalId string

@description('The token appended to resource names for uniqueness')
param resourceToken string

var foundryAccountName = 'aifdiscordbot-${resourceToken}'
var foundryProjectName = 'discordbot'
var foundryProjectEndpoint = 'https://${foundryAccountName}.services.ai.azure.com/api/projects/${foundryProjectName}'
var azureAiUserRoleDefinitionId = subscriptionResourceId(
  'Microsoft.Authorization/roleDefinitions',
  '53ca6127-db72-4b80-b1b0-d745d6d5456d'
)
var cognitiveServicesUserRoleDefinitionId = subscriptionResourceId(
  'Microsoft.Authorization/roleDefinitions',
  'a97b65f3-24c7-4388-baec-2e87135dc908'
)

module foundryAccount 'br/public:avm/ptn/ai-ml/ai-foundry:0.6.0' = {
  name: foundryAccountName
  params: {
    baseName: 'ai-${substring(resourceToken, 0, 9)}'
    location: location
    tags: tags
    enableTelemetry: true
    aiFoundryConfiguration: {
      accountName: foundryAccountName
      location: location
      sku: 'S0'
      disableLocalAuth: true
      project: {
        displayName: 'Discord Bot'
      }
      roleAssignments: [
        {
          principalId: appIdentityPrincipalId
          roleDefinitionIdOrName: azureAiUserRoleDefinitionId
        }
        {
          principalId: appIdentityPrincipalId
          roleDefinitionIdOrName: cognitiveServicesUserRoleDefinitionId
        }
      ]
    }
    aiModelDeployments: [
      {
        // Model deployments use deployment-type SKUs such as GlobalStandard, not the parent account SKU (S0).
        name: 'gpt-5.3-chat'
        sku: {
          name: 'GlobalStandard'
          capacity: 100
        }
        model: {
          name: 'gpt-5.3-chat'
          format: 'OpenAI'
          version: '2026-03-03'
        }
      }
    ]
  }
}

// module bingGrounding './bing-grounding.bicep' = {
//   name: 'bing-grounding'
//   params: {
//     foundryAccountName: foundryAccountName
//     foundryProjectName: foundryProjectName
//     resourceToken: resourceToken
//   }
//   dependsOn: [
//     foundryAccount
//   ]
// }

output accountName string = foundryAccount.name
output projectEndpoint string = foundryProjectEndpoint
output projectName string = foundryAccount.outputs.aiProjectName
