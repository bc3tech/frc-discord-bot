@description('The Foundry account name that owns the project connection')
param foundryAccountName string

@description('The Foundry project name that owns the Bing grounding connection')
param foundryProjectName string

@description('The token appended to resource names for uniqueness')
param resourceToken string

var bingGroundingName = 'bingsearch-${resourceToken}'
var bingConnectionName = '${foundryAccountName}-bingsearchconnection'

resource foundryAccount 'Microsoft.CognitiveServices/accounts@2025-06-01' existing = {
  name: foundryAccountName
}

resource foundryProject 'Microsoft.CognitiveServices/accounts/projects@2025-06-01' existing = {
  parent: foundryAccount
  name: foundryProjectName
}

resource bingGrounding 'Microsoft.Bing/accounts@2020-06-10' = {
  name: bingGroundingName
  location: 'global'
  sku: {
    name: 'G1'
  }
  kind: 'Bing.Grounding'
}

resource bingConnection 'Microsoft.CognitiveServices/accounts/projects/connections@2025-06-01' = {
  parent: foundryProject
  name: bingConnectionName
  properties: {
    category: 'ApiKey'
    target: 'https://api.bing.microsoft.com/'
    authType: 'ApiKey'
    credentials: {
      key: bingGrounding.listKeys().key1
    }
    isSharedToAll: true
    metadata: {
      ApiType: 'Azure'
      Location: bingGrounding.location
      ResourceId: bingGrounding.id
    }
  }
}

output connectionId string = bingConnection.id
output connectionName string = bingConnection.name
output resourceId string = bingGrounding.id
