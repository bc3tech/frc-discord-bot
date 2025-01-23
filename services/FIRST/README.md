# FIRST - the C# library for the FRC Events

# Overview

_FIRST_/FMS FRC Events API is a service to return relevant information about the _FIRST_ Robotics Competition (FRC). Information is made available from events operating around the world.

For FRC, information is made available by the Field Management System (FMS) server operating at each event site. The FMS will attempt to sync all data from the event to \"the cloud\" as long as internet is available at the venue. If internet is unavailable, or \"goes down\" during the event, the FMS will automatically sync all data from the time the system was offline as soon as the connection is restored. The API will provide data as soon as it has synced, and we do not add any artificial delays. If you are receiving \"stale\" data, it may be because of connectivity problems at the venue. We recommend you try again later, and post on the FIRST FMS TeamForge site if the problem persists. _(Note: FMS does not sync while a match is running, so data that has to do with a particular match should become available once the score has been revealed to the audience at the event.)_

### Migration and Program Notes:

Pay close attention to the addresses for calling the various endpoints- as well as the notes regarding endpoints with multiple possible responses (i.e. score details and rankings).

# Documentation Notes

All times are listed in the local time to the event venue. HTTP-date values will show their timezone.

If you specify a parameter, but no value for that parameter, it will be ignored. For example, if you request **URL?teamNumber=** the **teamNumber** parameter would be ignored.

We will continue to support the current version of the API plus one version older. Old APIs are depricated once a version \"two times newer\" is available, at minimum 6 months. For example, version 2.0 and 1.0 are supported right now, but 1.0 will not be supported once 2.1 (or 3.0) is available. Versions may also be retired earlier with prior notice here in the documentation.

The full host address of the API is needed in all calls. The version number is required in each call in order to ensure your requests are made (and responses are returned) in the formats you anticipate. The version number for this documentation is found on the top of the page, in the title. If you call this version number, the API responses will match the formats listed below.

All of the APIs are capable of accepting the **Accept** HTTP header with either **application/xml** or **application/json** based on the desired return type. Any API call that results in an **HTTP Status Code** other than **200 (OK)** will only be shown here as an **application/json** response to save space, but the content is the same regardless of the request type. All response will have a **Content-Length** and **Date** response header, but those are not shown in the documentaiton.

For all APIs that accept a query string in addition to the URI base, the order of parameters do not matter, but the name shown in the documentation must match exactly, as does the associated value format as described in details.

For response codes that are not **HTTP 200 (OK)**, the documentation will show a body message that represents a possible response value. While the \"title\" of the **HTTP Status Code** will match those shown in the response codes documentation section exactly, the body of the response will be a more detailed explanation of why that status code is being returned and may not always be exactly as shown in the examples.

None of the APIs will show possible return here in the documentation of **HTTP 401 (Unauthorized)**, but that code applies to all APIs as a possible response if the request is made without a valid token.

### Last-Modified, FMS-OnlyModifiedSince, and If-Modified-Since Headers

The FRC Events API utilizes the **Last-Modified** and **If-Modified-Since** Headers to communicate with consumers regarding the age of the data they are requesting. With a couple of exceptions, all calls will return a **Last-Modified** Header set with the time at which the data at that endpoint was last modified. The Header will always be set in the HTTP-date format, as described in the [HTTP Protocol](https://www.w3.org/Protocols/rfc2616/rfc2616-sec14.html). There are two exceptions: the **Last-Modified** Header is not set if the endpoint returns no results (such as a request for a schedule with no matches) and will also not be set if the request was an **HTTP DELETE**.

Consumers should keep track of the **Last-Modified** Header, and return it on subsequent calls to the same endpoint as the **If-Modified-Since**. The server will recognize this request, and will only return a result if the data has been modified since the last request. If no changes have been made, an **HTTP 304** will be returned. If data has been modified, ALL data on that call will be returned (for \"only modified\" data, see below).

The FRC Events API also allows a custom header used to filter the return data to a specific subset. This is done by specifying a **FMS-OnlyModifiedSince** header with each call. As with the **If-Modified-Since** header, consumers should keep track of the **Last-Modified** Header, and return it on subsequent calls to the same endpoint as the **FMS-OnlyModifiedSince** Header. The server will recognize this request, and will only return a result if the data has been modified since the last request, and, if returned, the data will only be those portions modified since the included date. If no changes, have been made, an **HTTP 304** will be returned. Using this method, the server and consumer save processing time by only receiving modified data that is in need of update on the consumer side.

If the Headers are improperly passed (such as the wrong Day of Week for the matching date, or a date in the future), the endpoint will simply ignore the Header and return all results. If both headers are specified, the request will be denied.

# Response Codes

The FRC Events API HTTP Status Codes correspond with the [common codes](https://www.w3.org/Protocols/rfc2616/rfc2616-sec10.html), but occasionally with different \"titles\". The \"title\" used by the API is shown next to each of the below possible response HTTP Status Codes. Throughout the documentation, Apiary may automatically show the common \"title\" in example returns (like \"Not Found\" for 404) but on the production server, the \"title\" will instead match those listed below.

```
HTTP 200 - \"OK\"

 ```

The request has succeeded. An entity corresponding to the requested resource is sent in the response. This will be returned as the HTTP Status Code for all request that succeed, even if the body is empty (such as an event that has no rankings, but with a valid season and event code were used)

```
HTTP 304 - \"Not Modified\"

 ```

When utilizing a Header that allows filtered data returns, such as **If-Modified-Since**, this response indicates that no data meets the request.

```
HTTP 400 - \"Invalid Season Requested\"/\"Malformed Parameter Format In Request\"/\"Missing Parameter In Request\"/\"Invalid API Version Requested\":

 ```

The request could not be understood by the server due to malformed syntax. The client SHOULD NOT repeat the request without modifications. Specifically for this API, a 400 response indicates that the requested URI matches with a valid API, but one or more required parameter was malformed or invalid. Examples include an event code that is too short or team number that contains a letter.

```
HTTP 401 - \"Unauthorized\"

 ```

All requests against the API require authentication via a valid user token. Failing to provide one, or providing an invalid one, will warrant a 401 response. The client MAY repeat the request with a suitable Authorization header field.

```
HTTP 404 - \"Invalid Event Requested\"

 ```

Even though the 404 code usually indicates any not found status, a 404 will only be issued in this API when an event cannot be found for the requested season and event code. If the request didn't match a valid API or there were malformed parameters, the response would not receive a 404 but rather a 400 or 501. If this HTTP code is received, the season was a valid season and the event code matched the acceptable style of an event code, but there were no records of an event matching the combination of that season and event code. For example, HTTP 404 would be issued when the event had a different code in the requested season (the codes can change year to year based on event location).

```
HTTP 500 - \"Internal Server Error\"

 ```

The server encountered an unexpected condition which prevented it from fulfilling the request. This is a code sent directly by the server, and has no special alternate definition specific to this API.

```
HTTP 501 - \"Request Did Not Match Any Current API Pattern\"

 ```

The server does not support the functionality required to fulfill the request. Specifically, the request pattern did not match any of the possible APIs, and thus processing was discontinued. This code is also issued when too many optional parameters were included in a single request and fulfilling it would make the result confusing or misleading. Each API will specify which parameters or combination of parameters can be used at the same time.

```
HTTP 503 - \"Service Unavailable\"

 ```

The server is currently unable to handle the request due to a temporary overloading or maintenance of the server. The implication is that this is a temporary condition which will be alleviated after some delay. If known, the length of the delay MAY be indicated in a Retry-After header. This code will not always appear, sometimes the server may outright refuse the connection instead. This is a code sent directly by the server, and has no special alternate definition specific to this API.

See the notes at the top of this documentation for important information about HTTP Status Codes.

# Authorization

In order to make calls against the FRC Events API, you must include an HTTP Header called **Authorization** with the value set as specified below. If a request is made without this header, processing stops and an **HTTP 401** is issued. All **Authorization** headers follow the same format:

Authorization: Basic 000000000000000000000000000000000000000000000000000000000000

Where the Zeros are replaced by your Token. The Token can be formed by taking your **username** and your **AuthorizationKey** and adding a colon. For example, if your **username** is sampleuser and your **AuthorizationKey** is 7eaa6338-a097-4221-ac04-b6120fcc4d49 you would have this string:

**sampleuser:7eaa6338-a097-4221-ac04-b6120fcc4d49**

This string must then be encoded using Base64 Encoded to form the Token, which will be the same length as the example above, but include letters and numbers. For our example, we would have:

c2FtcGxldXNlcjo3ZWFhNjMzOC1hMDk3LTQyMjEtYWMwNC1iNjEyMGZjYzRkNDk=

**NOTICE**: Publicly distributing an application, code snippet, etc, that has your username and token in it, encoded or not, WILL result in your token being blocked from the API. Each user should apply for their own token.

If you wish to acquire a token for your development, you may do so by requesting a token through our automated system on [this website](https://frc-events.firstinspires.org/services/API).

**AUTOMATED REMOVAL**: If you do not activate your account within 72 hours of making your request for a token, or if you do not make at least one API request every twelve (12) months, your account/token will be marked as disabled for inactivity and subject to being deleted. (This policy does not apply to accounts with special operating agreements with FIRST)

### HTTP401 and Authorization

Each Token can be individually enabled and disabled by _FIRST_. As such, a normally valid combination of **username** and **AuthorizationToken** could still be rejected. The possible return messages you may see in these instances are:

**Incorrect Token** (You supplied an AuthorizationToken, but it wasn't correct)

**Account Disabled, Contact Support** (You have been disabled for excessive traffic or abuse. Contact support)

**Username Not Found** (A username was found, but didn't match any on file)

**Unable To Determine Authorization Token** (The format of the **Authorization** header was incorrect)

# Webhooks

> **COMING SOON**

This C# SDK is automatically generated by the [OpenAPI Generator](https://openapi-generator.tech) project:

- API version: 1.0.0
- SDK version: 1.0.0
- Build package: org.openapitools.codegen.languages.CSharpClientCodegen

<a id="frameworks-supported"></a>
## Frameworks supported

<a id="dependencies"></a>
## Dependencies

- [Json.NET](https://www.nuget.org/packages/System.Text.Json/) - 13.0.2 or later
- [JsonSubTypes](https://www.nuget.org/packages/JsonSubTypes/) - 1.8.0 or later

The DLLs included in the package may not be the latest version. We recommend using [NuGet](https://docs.nuget.org/consume/installing-nuget) to obtain the latest version of the packages:
```
Install-Package System.Text.Json
Install-Package JsonSubTypes
```
<a id="installation"></a>
## Installation
Run the following command to generate the DLL
- [Mac/Linux] `/bin/sh build.sh`
- [Windows] `build.bat`

Then include the DLL (under the `bin` folder) in the C# project, and use the namespaces:
```csharp
using FIRST.Api;
using FIRST.Client;
using FIRST.Model;
```
<a id="packaging"></a>
## Packaging

A `.nuspec` is included with the project. You can follow the Nuget quickstart to [create](https://docs.microsoft.com/en-us/nuget/quickstart/create-and-publish-a-package#create-the-package) and [publish](https://docs.microsoft.com/en-us/nuget/quickstart/create-and-publish-a-package#publish-the-package) packages.

This `.nuspec` uses placeholders from the `.csproj`, so build the `.csproj` directly:

```
nuget pack -Build -OutputDirectory out FIRST.csproj
```

Then, publish to a [local feed](https://docs.microsoft.com/en-us/nuget/hosting-packages/local-feeds) or [other host](https://docs.microsoft.com/en-us/nuget/hosting-packages/overview) and consume the new package via Nuget as usual.

<a id="usage"></a>
## Usage

To use the API client with a HTTP proxy, setup a `System.Net.WebProxy`
```csharp
Configuration c = new Configuration();
System.Net.WebProxy webProxy = new System.Net.WebProxy("http://myProxyUrl:80/");
webProxy.Credentials = System.Net.CredentialCache.DefaultCredentials;
c.Proxy = webProxy;
```

### Connections
Each ApiClass (properly the ApiClient inside it) will create an instance of HttpClient. It will use that for the entire lifecycle and dispose it when called the Dispose method.

To better manager the connections it's a common practice to reuse the HttpClient and HttpClientHandler (see [here](https://docs.microsoft.com/en-us/dotnet/architecture/microservices/implement-resilient-applications/use-httpclientfactory-to-implement-resilient-http-requests#issues-with-the-original-httpclient-class-available-in-net) for details). To use your own HttpClient instance just pass it to the ApiClass constructor.

```csharp
HttpClientHandler yourHandler = new HttpClientHandler();
HttpClient yourHttpClient = new HttpClient(yourHandler);
var api = new YourApiClass(yourHttpClient, yourHandler);
```

If you want to use an HttpClient and don't have access to the handler, for example in a DI context in Asp.net Core when using IHttpClientFactory.

```csharp
HttpClient yourHttpClient = new HttpClient();
var api = new YourApiClass(yourHttpClient);
```
You'll loose some configuration settings, the features affected are: Setting and Retrieving Cookies, Client Certificates, Proxy settings. You need to either manually handle those in your setup of the HttpClient or they won't be available.

Here an example of DI setup in a sample web project:

```csharp
services.AddHttpClient<YourApiClass>(httpClient =>
   new PetApi(httpClient));
```


<a id="getting-started"></a>
## Getting Started

```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using FIRST.Api;
using FIRST.Client;
using FIRST.Model;

namespace Example


    public class Example
    {
        public static void Main()
        {

            Configuration config = new Configuration();
            config.BasePath = "https://frc-api.firstinspires.org";
            // Configure HTTP basic authorization: basicAuth
            config.Username = "YOUR_USERNAME";
            config.Password = "YOUR_PASSWORD";

            // create instances of HttpClient, HttpClientHandler to be reused later with different Api classes
            HttpClient httpClient = new HttpClient();
            HttpClientHandler httpClientHandler = new HttpClientHandler();
            var apiInstance = new AllianceSelectionApi(httpClient, config, httpClientHandler);
            var eventCode = "eventCode_example";  // string | **[REQUIRED] (string)** Case insensitive alphanumeric eventCode of the event from which the alliance selection results are requested. Must be at least 3 characters. 
            var season = {{season}};  // string | **[REQUIRED] (int)** Numeric year of the event from which the event alliances are requested. Must be 4 digits and greater than or equal to 2015, and less than or equal to the current year. 

            try
            {
                // Event Alliances
                apiInstance.SeasonAlliancesEventCodeGet(eventCode, season);
            }
            catch (ApiException e)
            {
                Debug.Print("Exception when calling AllianceSelectionApi.SeasonAlliancesEventCodeGet: " + e.Message );
                Debug.Print("Status Code: "+ e.ErrorCode);
                Debug.Print(e.StackTrace);
            }

        }
    }
}
```

<a id="documentation-for-api-endpoints"></a>
## Documentation for API Endpoints

All URIs are relative to *https://frc-api.firstinspires.org*

Class | Method | HTTP request | Description
------------ | ------------- | ------------- | -------------
*AllianceSelectionApi* | [**SeasonAlliancesEventCodeGet**](docs/AllianceSelectionApi.md#seasonallianceseventcodeget) | **GET** /{season}/alliances/{eventCode} | Event Alliances
*AncillaryApi* | [**RootGet**](docs/AncillaryApi.md#rootget) | **GET** / | API Index
*AwardsApi* | [**SeasonAwardsEventEventCodeGet**](docs/AwardsApi.md#seasonawardseventeventcodeget) | **GET** /{season}/awards/event/{eventCode} | Event Awards
*AwardsApi* | [**SeasonAwardsEventteamEventCodeTeamNumberGet**](docs/AwardsApi.md#seasonawardseventteameventcodeteamnumberget) | **GET** /{season}/awards/eventteam/{eventCode}/{teamNumber} | Event Team Awards
*AwardsApi* | [**SeasonAwardsListGet**](docs/AwardsApi.md#seasonawardslistget) | **GET** /{season}/awards/list | Awards Listings
*AwardsApi* | [**SeasonAwardsTeamTeamNumberGet**](docs/AwardsApi.md#seasonawardsteamteamnumberget) | **GET** /{season}/awards/team/{teamNumber} | Team Awards
*MatchResultsApi* | [**SeasonMatchesEventCodeGet**](docs/MatchResultsApi.md#seasonmatcheseventcodeget) | **GET** /{season}/matches/{eventCode} | Event Match Results
*MatchResultsApi* | [**SeasonScoresEventCodeTournamentLevelGet**](docs/MatchResultsApi.md#seasonscoreseventcodetournamentlevelget) | **GET** /{season}/scores/{eventCode}/{tournamentLevel} | Score Details
*RankingsApi* | [**SeasonRankingsDistrictGet**](docs/RankingsApi.md#seasonrankingsdistrictget) | **GET** /{season}/rankings/district | District Rankings
*RankingsApi* | [**SeasonRankingsEventCodeGet**](docs/RankingsApi.md#seasonrankingseventcodeget) | **GET** /{season}/rankings/{eventCode} | Event Rankings
*RankingsDistrictRankingCalculationApi* | [**SeasonRankingsDistrictAllianceSelectionCalculationGet**](docs/RankingsDistrictRankingCalculationApi.md#seasonrankingsdistrictallianceselectioncalculationget) | **GET** /{season}/rankings/district/allianceSelectionCalculation | Alliance Selection Points
*RankingsDistrictRankingCalculationApi* | [**SeasonRankingsDistrictPlayoffAdvancementCalculationGet**](docs/RankingsDistrictRankingCalculationApi.md#seasonrankingsdistrictplayoffadvancementcalculationget) | **GET** /{season}/rankings/district/playoffAdvancementCalculation | Playoff Advancement Points
*RankingsDistrictRankingCalculationApi* | [**SeasonRankingsDistrictQualPerformanceCalculationGet**](docs/RankingsDistrictRankingCalculationApi.md#seasonrankingsdistrictqualperformancecalculationget) | **GET** /{season}/rankings/district/qualPerformanceCalculation | Qual Performance Points
*ScheduleApi* | [**SeasonScheduleEventCodeGet**](docs/ScheduleApi.md#seasonscheduleeventcodeget) | **GET** /{season}/schedule/{eventCode} | Event Schedule
*SeasonDataApi* | [**SeasonAvatarsGet**](docs/SeasonDataApi.md#seasonavatarsget) | **GET** /{season}/avatars | Team Avatar Listings
*SeasonDataApi* | [**SeasonDistrictsGet**](docs/SeasonDataApi.md#seasondistrictsget) | **GET** /{season}/districts | District Listings
*SeasonDataApi* | [**SeasonEventsGet**](docs/SeasonDataApi.md#seasoneventsget) | **GET** /{season}/events | Event Listings
*SeasonDataApi* | [**SeasonGet**](docs/SeasonDataApi.md#seasonget) | **GET** /{season} | Season Summary
*SeasonDataApi* | [**SeasonTeamsGet**](docs/SeasonDataApi.md#seasonteamsget) | **GET** /{season}/teams | Team Listings


<a id="documentation-for-models"></a>
## Documentation for Models



<a id="documentation-for-authorization"></a>
## Documentation for Authorization


Authentication schemes defined for the API:
<a id="noauthAuth"></a>
### noauthAuth

<a id="basicAuth"></a>
### basicAuth

- **Type**: HTTP basic authentication
