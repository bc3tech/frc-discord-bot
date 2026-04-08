@description('Container App name for the function workload.')
param name string

@description('Azure region for the function container app.')
param location string

@description('Tags applied to the function container app.')
param tags object

@description('Resource ID of the Container Apps managed environment.')
param managedEnvironmentId string

@description('Azure Container Registry server name (for example contoso.azurecr.io).')
param containerRegistryServer string

@description('User-assigned managed identity resource ID used by the app.')
param appIdentityResourceId string

@description('Whether the target app already exists (used to keep current image on infra-only updates).')
param appExists bool

@secure()
@description('Discord bot token injected as a container app secret.')
param discordToken string

@secure()
@description('FIRST API password injected as a container app secret.')
param firstPassword string

@secure()
@description('The Blue Alliance API key injected as a container app secret.')
param tbaApiKey string

@description('Container environment variables passed to the main function container.')
param containerEnv array

@description('Fallback image used until the service image is available in ACR.')
param defaultFunctionsImage string = 'mcr.microsoft.com/azure-functions/dotnet-isolated:4-dotnet-isolated10.0'

module appFetchLatestImage './fetch-container-image.bicep' = {
  name: 'app-fetch-image'
  params: {
    exists: appExists
    name: name
  }
}

module app 'br/public:avm/res/app/container-app:0.21.0' = {
  name: 'function-container-app'
  params: {
    name: name
    location: location
    kind: 'functionapp'
    tags: union(tags, { 'azd-service-name': 'app' })
    environmentResourceId: managedEnvironmentId
    managedIdentities: {
      userAssignedResourceIds: [
        appIdentityResourceId
      ]
    }
    activeRevisionsMode: 'Single'
    ingressAllowInsecure: true
    ingressExternal: true
    ingressTargetPort: 80
    ingressTransport: 'auto'
    registries: [
      {
        server: containerRegistryServer
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
    containers: [
      {
        image: appFetchLatestImage.outputs.?containers[?0].?image ?? defaultFunctionsImage
        name: 'main'
        resources: {
          cpu: json('0.5')
          memory: '1.0Gi'
        }
        env: containerEnv
      }
    ]
    scaleSettings: {
      minReplicas: 1
      maxReplicas: 1
    }
  }
}

output appResourceId string = app.outputs.resourceId
