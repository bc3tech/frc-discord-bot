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

The FRC Discord Bot is developed as an Azure Function using .NET 8.0. Setting up your dev environment is as simple as installing [Visual Studio](https://visualstudio.com) 2022+ with the Azure workload, cloning the repo, and hitting `Build`.

## Infrastructure components

The infrastructure necessary to run the bot includes:

- An Azure Function Plan (Consumption is adequate for most installations)
- Azure Storage account
- (Optional) Azure App Insights (telemetry & logging)
- (Optional) Azure OpenAI ("chatbot" functionality)

## Code Structure

- `/app` - This contains all of the application code. It is organized into
  - `/Apis` - simpler APIs that have a very small amount of code associated with them
  - `/ChatBot` - implementation & detail for the GPT-Chat capability
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

Simply deploying the Function App will make the bot live and ready for installation on any Discord Guild. Once live, follow the instructions for creating a Discord App to get the Discord Token used to link the deployed bot with a Discord application.

### Secrets

The following secret values must be set as user secrets or environment variables to configure the deployed bot to interact with Discord and the relevant APIs:

|App Setting|Environment Variable Name|Description|Example value|
|-|:-:|-|-:|
|`TbaApiKey`|`TbaApiKey`|API key for The Blue Alliance; available on your TBA Account Page|`aB3dE5fG7hI9jK1lM2...`|
|`Discord.Token`|`Discord__Token`|The auth token for the Discord app you will create for the bot|`Q6rS8tU0vW!xYz@2#4$6^8&0*C8D0E...`|
|`FIRST.Username`|`FIRST__Username`|Username for the FIRST API|`myuser`|
|`FIRST.Password`|`FIRST__Password`|Password for the FIRST API|`123e4567-e89b-12d3-a456-426614174000`|
|`Azure.AI.ApiKey`|`Azure__AI__ApiKey`|The API Key for an Azure OpenAI instance to use for ChatBot features|`1F3G5H7I9JkLmN...`|
|`Azure.AI.Project.ConnectionString`|`Azure__AI__Project__ConnectionString`|The Connection string to an Azure AI Foundry project that will house the AI Agent used for ChatBot features|`westus.api.azureml.ms;f47ac10b-58cc-4372-a567-0e02b2c3d479;my-ai-hub;my-ai-project`|
|`Azure.AI.Project.Credentials.ClientId`|`Azure__AI__Project__Credentials__ClientId`|The ClientID of the Service Principal with inference access to the AI Project|`550e8400-e29b-41d4-a716-446655440000`|
|`Azure.AI.Project.Credentials.ClientSecret`|`Azure__AI__Project__Credentials__ClientSecret`|The Client Secret of the Service Principal with inference access to the AI Project|`9jK1lM2nO4pQ6rS8tU0vW!...`|
|`Azure.AI.Project.Credentials.TenantId`|`Azure__AI__Project__Credentials__TenantId`|The TenantId of the Service Principal with inference access to the AI Project|`d3c4e5f6-7890-1234-abcd-ef0123456789`|

For example:

```json
{
  "TbaApiKey": "...",
  "Discord": {
    "Token": "..."
  },
  "FIRST": {
    "Username": "bc3tech",
    "Password": "..."
  },
  "Azure": {
    "AI": {
      "ApiKey": "...",
      "Project": {
        "ConnectionString": "westus.api.azureml.ms;...;my-ai-hub;my-ai-project",
        "Credentials": {
          "ClientId": "0fba27f4-998c-4f7a-b3ed-d0d435b8eb05",
          "ClientSecret": "...",
          "TenantId": "..."
        }
      }
    }
  }
}
```

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
