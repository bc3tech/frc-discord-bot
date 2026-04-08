# First Robotics Competition (FRC) Discord Bot

The FRC Discord Bot comprises functionality specific to the First Robotics Competition, and enables the ability to perform the following functions within a Discord Guild (or as a Discord User):

1. Subscribe to a team, and event, or a team at an event (`/subscriptions`) - getting notifications including (but not limited to)
    - Upcoming matches
    - Schedule Changes
    - Match results
    - Awards
    - and more
1. Get information about an individual FRC team (`/teams`)
1. Get information about an FRC event (`/events`)

## Development Environment

The FRC Discord Bot is developed as a containerized Azure Function using .NET 10.0. Setting up your dev environment is as simple as installing [Visual Studio](https://visualstudio.com) 2022+ with the Azure workload, cloning the repo, and hitting `Build`.

## Infrastructure components

The infrastructure necessary to run the bot includes:

- An Azure Container Apps environment hosting the Function App (`kind: functionapp`)
- An Azure Container Registry to store the function container image
- Azure Storage account
- (Optional) Azure Monitor / Application Insights (telemetry & logging)
- (Optional) Azure AI Foundry project with Microsoft Agent Framework-backed chatbot functionality

### Infra template layout (`infra/`)

- `main.bicep` keeps azd-facing parameters and composes grouped configuration objects.
- `resources.bicep` orchestrates feature-level deployment (platform core, app, Foundry, optional search).
- `modules/platform-core.bicep` provisions shared platform dependencies (monitoring, identity, registry, storage, managed environment) with AVM modules.
- `modules/function-app.bicep` deploys the function container app using AVM and preserves existing image-resolution behavior.

## Code Structure

- `/app` - This contains all of the application code. It is organized into
  - `/Apis` - simpler APIs that have a very small amount of code associated with them
  - `/ChatBot` - implementation & detail for the Azure AI Foundry DM chat capability
  - `/DiscordInterop` - everything related to setting up, communicating with, and generating messages for Discord
  - `/FIRSTInterop` - same, but for the FIRST APIs
  - `/StatboticsInterop` - again same, but for Statbotics
  - `/TbaInterop` - same, but for TheBlueAlliance
  - `/Storage` - code specific to both volatile (in-memory) and non-volatile (persisted) storage
  - `/Subscription` - functionality that handles subscription logic like creation, deletion, and dispatching messages
  - `/Functions` - The implemented Azure Functions that are part of the app

- `/services` - This contains the APIs with which the bot integrates. These APIs are 99% code-generated with any necessary modifications being done in accompanying `partial class` definitions so as not to be overwritten by subsquent executions of the code generator. As of this writing, codegen is done on top of the OpenAPI/Swagger definitions for the following services:
  - [FRC-Events API](https://frc-api-docs.firstinspires.org/)
  - [Statbotics.io](https://www.statbotics.io/docs/rest)
  - [The Blue Alliance](https://www.thebluealliance.com/apidocs)
  - [FRC Colors](https://github.com/jonahsnider/frc-colors.com#api-usage)

### Re/generating API clients, models

To re/generate any of the REST API clients and associated code, we make use of the OpenAPI Generator project with additional customizations to bring the generated .NET code current with the latest advancements in .NET.

First, clone the repo containing the OpenAPI Generator template:

```sh
git clone https://github.com/bc3tech/openapi-generator-csharp-system-text-json.git
```

Next, execute the OpenAPI Generator CLI tool against the API you want to create code for **from within its directory**, using the template:

```sh
apt install -y npm default-jre
cd services/Statbotics
npx @openapitools/openapi-generator-cli generate --library=httpclient -i https://api.statbotics.io/openapi.json -g csharp -c openapitools.json  -t /mnt/e/gh/openapi-generator-csharp-system-text-json/
```

## Core Concepts

- Utilize dependency injection as a means to create and reference services throughout the lifetime of the application
- Make extensive use of logging at the appropriate levels
- Make configurable all things that can/may need to be changed in production to afford different levels/types of troubleshooting & deployment
- No secrets shall be saved into any file within the repo at any time. The application, instead, makes use of `dotnet secrets` to manage user secrets during local execution of the application, and Azure App Settings to manage secrets in deployment.

In addition to the above, the codebase makes use of Centralized Package Managment, Code Analysis, and `.editorconfig` to cohesive and compliant code throughout.

## Deployment

Deploying the containerized Function App will make the bot live and ready for installation on any Discord Guild. Once live, follow the instructions for creating a Discord App to get the Discord Token used to link the deployed bot with a Discord application.

For PowerShell on Windows, use an inline command for this template. The committed `.bicepparam` file intentionally omits required secret values, so Azure CLI will reject it unless you first create your own local params file with those secrets filled in.

```powershell
az deployment sub create --location westus2 --template-file .\infra\main.bicep --parameters environmentName='prod' location='westus2' discordToken='<prod-token>' firstPassword='<password>' tbaApiKey='<tba-key>' chatBotAgentId='<foundry-agent-id>'
```

### Secrets

The following secret values must be set as user secrets or environment variables to configure the deployed bot to interact with Discord and the relevant APIs:

|App Setting|Environment Variable Name|Description|Example value|
|-|:-:|-|-:|
|`TbaApiKey`|`TbaApiKey`|API key for The Blue Alliance; available on your TBA Account Page|`aB3dE5fG7hI9jK1lM2...`|
|`Discord.Token`|`Discord__Token`|The auth token for the Discord app you will create for the bot|`Q6rS8tU0vW!xYz@2#4$6^8&0*C8D0E...`|
|`FRC.Username`|`FRC__Username`|Username for the FRC Events API|`bc3tech`|
|`FRC.Password`|`FRC__Password`|Password for the FRC Events API|`123e4567-e89b-12d3-a456-426614174000`|

For example:

```json
{
  "TbaApiKey": "...",
  "Discord": {
    "Token": "..."
  },
  "FRC": {
    "Username": "bc3tech",
    "Password": "..."
  }
}
```

When deployed with the Bicep templates in `infra\`, the Azure AI Foundry account and its child project are provisioned automatically using the current `Microsoft.CognitiveServices/accounts` + `accounts/projects` model. Local auth is disabled on the Foundry account. The container app managed identity is granted `Cognitive Services User` on the Foundry project for keyless data-plane access and `Azure AI User` on the child project for project-scoped Foundry operations. The Function App receives `AI__Azure__ProjectEndpoint`, `Azure__ClientId`, `Azure__TenantId`, `AZURE_CLIENT_ID`, and `AZURE_TENANT_ID` from infrastructure rather than manual secret configuration.

For DM chat, the app now runs a Microsoft Agent Framework workflow that coordinates:

- a hosted Azure AI Foundry prompt agent for user-facing synthesis plus hosted tools like web search and code interpreter
- a local declarative agent for in-process tools such as TBA, Statbotics, and meal-signup lookups

The DM pipeline expects:

- `AI__Azure__ProjectEndpoint`
- `AI__Azure__AgentId`
- `AI__Azure__LocalAgentModel`
- `DefaultTeamNumber`

The hosted agent definition is checked in at `services\ChatBot\Agents\foundry-agent.yaml` as a reference copy of the Foundry-side configuration. The local declarative agent definition is checked in at `services\ChatBot\Agents\local-agent.yaml` and is instantiated in-process against the configured `AI__Azure__LocalAgentModel`. Semantic evaluator turns (answer/ask-user decision checks) use the Foundry evaluator model. The configured `AI__Azure__AgentId` value can be either a bare agent name such as `2046-discord-bot` to use the latest published version or a versioned Foundry identifier in `<agent-name>:<version>` format such as `2046-discord-bot:2` to pin a specific version. If the Foundry project endpoint, hosted agent id, or local agent model is missing, chat-specific services stay disabled so the rest of the bot can still start normally.

`DefaultTeamNumber` defines the fallback team identity for ambiguous first-person team references in chat. With the default value of `2046`, phrases like we, us, and our resolve to team 2046 unless the turn or grounded conversation context clearly establishes a different team.

Keep `AI__Azure__MealSignupGeniusId` only if you still want the optional meal-signup helper behavior.

For `azd` deployments, set the required secret environment values before running `azd deploy`:

```powershell
azd env set-secret Discord__Token "<your-discord-bot-token>"
azd env set-secret FIRST__Password "<your-first-password>"
azd env set-secret TbaApiKey "<your-tba-api-key>"
azd env set AI__Azure__AgentId "<your-chat-agent-id>"
azd env set SEARCH_SERVICE_NAME "<your-search-service-name>"
azd env set SEARCH_LOCATION "eastus"
azd env set AI__Azure__LocalAgentModel "gpt-5.4-mini"
azd env set DefaultTeamNumber "2046"
```

The function container app receives `Discord__Token`, `FIRST__Password`, and `TbaApiKey` as Container Apps secrets, `FIRST__Username` is set to `bc3tech` as a non-secret app setting, and the default production diagnostics settings are applied through `Logging__LogLevel__Default` and `AzureWebjobs.Heartbeat.Disabled`. Hosted chat settings only flow into the container app when `AI__Azure__AgentId` is set.

If `SEARCH_SERVICE_NAME` is non-empty, `azd` also provisions the Azure AI Search service used by the hosted agent knowledge-base connection. The current production environment keeps that service in `eastus`.

`azd` provisions the app as a native Azure Functions deployment on Azure Container Apps by setting the container app resource kind to `functionapp`, while still using the standard `host: containerapp` service workflow in `azure.yaml`. The service metadata now matches the Functions container listener on port `80`, which keeps the `azure.yaml` service definition aligned with the container app ingress configuration in `infra\resources.bicep`.

Until the first application image is pushed to Azure Container Registry, the infrastructure uses the official Azure Functions .NET isolated base image for .NET 10 (`mcr.microsoft.com/azure-functions/dotnet-isolated:4-dotnet-isolated10.0`) instead of a generic Container Apps sample image. The deployed container app also sets `FUNCTIONS_WORKER_RUNTIME=dotnet-isolated` explicitly so the native Functions host configuration remains correct for the .NET isolated worker.

`azd` also provisions a dedicated Azure Storage account for the app, disables shared-key auth on that account, and grants the container app's user-assigned managed identity the `Storage Blob Data Contributor`, `Storage Queue Data Contributor`, and `Storage Table Data Contributor` roles. The storage account keeps its public service endpoints enabled for the current Container Apps deployment shape, while authentication remains keyless through managed identity and RBAC. The Functions host is configured with identity-based `AzureWebJobsStorage__...` settings, while the app code receives `Azure__Storage__BlobsEndpoint` and `Azure__Storage__TableEndpoint` for its own SDK clients. No storage connection string is required in Azure.

### Other application settings

Other app settings are present in `appsettings.json` which gets loaded during production execution of the bot. These can be changed and re-deployed or overridden with environment variables as needed.

## No LLM Training or Referencing Clause

Per [LICENSE](LICENSE):

The source code in this repository is licensed for use by human developers only. Use of this code, its structure, logic, or documentation for the purposes of training, fine-tuning, or referencing by any machine learning model—including but not limited to large language models (LLMs)—is strictly prohibited.

This includes:

- Direct ingestion of code into datasets used for model training or evaluation
- Embedding or indexing for retrieval-augmented generation (RAG) systems
- Use in prompt engineering, code synthesis, or automated code generation tools

Exceptions may be granted only with explicit, written permission from the repository owner.

Violation of this clause may constitute unauthorized use under applicable copyright law.
