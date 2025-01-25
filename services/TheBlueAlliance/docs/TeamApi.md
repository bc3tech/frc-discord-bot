# TheBlueAlliance.Api.TeamApi

All URIs are relative to *https://www.thebluealliance.com/api/v3*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**GetDistrictRankings**](TeamApi.md#getdistrictrankings) | **GET** /district/{district_key}/rankings |  |
| [**GetDistrictTeams**](TeamApi.md#getdistrictteams) | **GET** /district/{district_key}/teams |  |
| [**GetDistrictTeamsKeys**](TeamApi.md#getdistrictteamskeys) | **GET** /district/{district_key}/teams/keys |  |
| [**GetDistrictTeamsSimple**](TeamApi.md#getdistrictteamssimple) | **GET** /district/{district_key}/teams/simple |  |
| [**GetEventTeams**](TeamApi.md#geteventteams) | **GET** /event/{event_key}/teams |  |
| [**GetEventTeamsKeys**](TeamApi.md#geteventteamskeys) | **GET** /event/{event_key}/teams/keys |  |
| [**GetEventTeamsSimple**](TeamApi.md#geteventteamssimple) | **GET** /event/{event_key}/teams/simple |  |
| [**GetEventTeamsStatuses**](TeamApi.md#geteventteamsstatuses) | **GET** /event/{event_key}/teams/statuses |  |
| [**GetTeam**](TeamApi.md#getteam) | **GET** /team/{team_key} |  |
| [**GetTeamAwards**](TeamApi.md#getteamawards) | **GET** /team/{team_key}/awards |  |
| [**GetTeamAwardsByYear**](TeamApi.md#getteamawardsbyyear) | **GET** /team/{team_key}/awards/{year} |  |
| [**GetTeamDistricts**](TeamApi.md#getteamdistricts) | **GET** /team/{team_key}/districts |  |
| [**GetTeamEventAwards**](TeamApi.md#getteameventawards) | **GET** /team/{team_key}/event/{event_key}/awards |  |
| [**GetTeamEventMatches**](TeamApi.md#getteameventmatches) | **GET** /team/{team_key}/event/{event_key}/matches |  |
| [**GetTeamEventMatchesKeys**](TeamApi.md#getteameventmatcheskeys) | **GET** /team/{team_key}/event/{event_key}/matches/keys |  |
| [**GetTeamEventMatchesSimple**](TeamApi.md#getteameventmatchessimple) | **GET** /team/{team_key}/event/{event_key}/matches/simple |  |
| [**GetTeamEventStatus**](TeamApi.md#getteameventstatus) | **GET** /team/{team_key}/event/{event_key}/status |  |
| [**GetTeamEvents**](TeamApi.md#getteamevents) | **GET** /team/{team_key}/events |  |
| [**GetTeamEventsByYear**](TeamApi.md#getteameventsbyyear) | **GET** /team/{team_key}/events/{year} |  |
| [**GetTeamEventsByYearKeys**](TeamApi.md#getteameventsbyyearkeys) | **GET** /team/{team_key}/events/{year}/keys |  |
| [**GetTeamEventsByYearSimple**](TeamApi.md#getteameventsbyyearsimple) | **GET** /team/{team_key}/events/{year}/simple |  |
| [**GetTeamEventsKeys**](TeamApi.md#getteameventskeys) | **GET** /team/{team_key}/events/keys |  |
| [**GetTeamEventsSimple**](TeamApi.md#getteameventssimple) | **GET** /team/{team_key}/events/simple |  |
| [**GetTeamEventsStatusesByYear**](TeamApi.md#getteameventsstatusesbyyear) | **GET** /team/{team_key}/events/{year}/statuses |  |
| [**GetTeamHistory**](TeamApi.md#getteamhistory) | **GET** /team/{team_key}/history |  |
| [**GetTeamMatchesByYear**](TeamApi.md#getteammatchesbyyear) | **GET** /team/{team_key}/matches/{year} |  |
| [**GetTeamMatchesByYearKeys**](TeamApi.md#getteammatchesbyyearkeys) | **GET** /team/{team_key}/matches/{year}/keys |  |
| [**GetTeamMatchesByYearSimple**](TeamApi.md#getteammatchesbyyearsimple) | **GET** /team/{team_key}/matches/{year}/simple |  |
| [**GetTeamMediaByTag**](TeamApi.md#getteammediabytag) | **GET** /team/{team_key}/media/tag/{media_tag} |  |
| [**GetTeamMediaByTagYear**](TeamApi.md#getteammediabytagyear) | **GET** /team/{team_key}/media/tag/{media_tag}/{year} |  |
| [**GetTeamMediaByYear**](TeamApi.md#getteammediabyyear) | **GET** /team/{team_key}/media/{year} |  |
| [**GetTeamRobots**](TeamApi.md#getteamrobots) | **GET** /team/{team_key}/robots |  |
| [**GetTeamSimple**](TeamApi.md#getteamsimple) | **GET** /team/{team_key}/simple |  |
| [**GetTeamSocialMedia**](TeamApi.md#getteamsocialmedia) | **GET** /team/{team_key}/social_media |  |
| [**GetTeamYearsParticipated**](TeamApi.md#getteamyearsparticipated) | **GET** /team/{team_key}/years_participated |  |
| [**GetTeams**](TeamApi.md#getteams) | **GET** /teams/{page_num} |  |
| [**GetTeamsByYear**](TeamApi.md#getteamsbyyear) | **GET** /teams/{year}/{page_num} |  |
| [**GetTeamsByYearKeys**](TeamApi.md#getteamsbyyearkeys) | **GET** /teams/{year}/{page_num}/keys |  |
| [**GetTeamsByYearSimple**](TeamApi.md#getteamsbyyearsimple) | **GET** /teams/{year}/{page_num}/simple |  |
| [**GetTeamsKeys**](TeamApi.md#getteamskeys) | **GET** /teams/{page_num}/keys |  |
| [**GetTeamsSimple**](TeamApi.md#getteamssimple) | **GET** /teams/{page_num}/simple |  |

<a id="getdistrictrankings"></a>
# **GetDistrictRankings**
> Collection&lt;DistrictRanking&gt; GetDistrictRankings (string districtKey, string? ifNoneMatch = null)



Gets a list of team district rankings for the given district.

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using TheBlueAlliance.Api;
using TheBlueAlliance.Client;
using TheBlueAlliance.Model;

namespace Example


    public class GetDistrictRankingsExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "https://www.thebluealliance.com/api/v3";
            // Configure API key authorization: apiKey
            config.AddApiKey("X-TBA-Auth-Key", "YOUR_API_KEY");
            // Uncomment below to setup prefix (e.g. Bearer) for API key, if needed
            // config.AddApiKeyPrefix("X-TBA-Auth-Key", "Bearer");

            // create instances of HttpClient, HttpClientHandler to be reused later with different Api classes
            HttpClient httpClient = new HttpClient();
            HttpClientHandler httpClientHandler = new HttpClientHandler();
            var apiInstance = new TeamApi(httpClient, config, httpClientHandler);
            var districtKey = "districtKey_example";  // string | TBA District Key, eg `2016fim`
            var ifNoneMatch = "ifNoneMatch_example";  // string? | Value of the `ETag` header in the most recently cached response by the client. (optional) 

            try
            {
                Collection<DistrictRanking> result = apiInstance.GetDistrictRankings(districtKey, ifNoneMatch);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling TeamApi.GetDistrictRankings: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the GetDistrictRankingsWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<Collection<DistrictRanking>> response = apiInstance.GetDistrictRankingsWithHttpInfo(districtKey, ifNoneMatch);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling TeamApi.GetDistrictRankingsWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **districtKey** | **string** | TBA District Key, eg &#x60;2016fim&#x60; |  |
| **ifNoneMatch** | **string?** | Value of the &#x60;ETag&#x60; header in the most recently cached response by the client. | [optional]  |

### Return type

[**Collection&lt;DistrictRanking&gt;**](DistrictRanking.md)

### Authorization

[apiKey](../README.md#apiKey)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | Successful response |  * Cache-Control - The &#x60;Cache-Control&#x60; header, in particular the &#x60;max-age&#x60; value, contains the number of seconds the result should be considered valid for. During this time subsequent calls should return from the local cache directly. <br>  * ETag - Specifies the version of the most recent response. Used by clients in the &#x60;If-None-Match&#x60; request header. <br>  |
| **304** | Not Modified - Use Local Cached Value |  -  |
| **401** | Authorization information is missing or invalid. |  -  |
| **404** | Not Found |  -  |

[[Back to top]](#) [[Back to API list]](../../README.md#documentation-for-api-endpoints) [[Back to Model list]](../../README.md#documentation-for-models) [[Back to README]](../../README.md)

<a id="getdistrictteams"></a>
# **GetDistrictTeams**
> Collection&lt;Team&gt; GetDistrictTeams (string districtKey, string? ifNoneMatch = null)



Gets a list of `Team` objects that competed in events in the given district.

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using TheBlueAlliance.Api;
using TheBlueAlliance.Client;
using TheBlueAlliance.Model;

namespace Example


    public class GetDistrictTeamsExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "https://www.thebluealliance.com/api/v3";
            // Configure API key authorization: apiKey
            config.AddApiKey("X-TBA-Auth-Key", "YOUR_API_KEY");
            // Uncomment below to setup prefix (e.g. Bearer) for API key, if needed
            // config.AddApiKeyPrefix("X-TBA-Auth-Key", "Bearer");

            // create instances of HttpClient, HttpClientHandler to be reused later with different Api classes
            HttpClient httpClient = new HttpClient();
            HttpClientHandler httpClientHandler = new HttpClientHandler();
            var apiInstance = new TeamApi(httpClient, config, httpClientHandler);
            var districtKey = "districtKey_example";  // string | TBA District Key, eg `2016fim`
            var ifNoneMatch = "ifNoneMatch_example";  // string? | Value of the `ETag` header in the most recently cached response by the client. (optional) 

            try
            {
                Collection<Team> result = apiInstance.GetDistrictTeams(districtKey, ifNoneMatch);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling TeamApi.GetDistrictTeams: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the GetDistrictTeamsWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<Collection<Team>> response = apiInstance.GetDistrictTeamsWithHttpInfo(districtKey, ifNoneMatch);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling TeamApi.GetDistrictTeamsWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **districtKey** | **string** | TBA District Key, eg &#x60;2016fim&#x60; |  |
| **ifNoneMatch** | **string?** | Value of the &#x60;ETag&#x60; header in the most recently cached response by the client. | [optional]  |

### Return type

[**Collection&lt;Team&gt;**](Team.md)

### Authorization

[apiKey](../README.md#apiKey)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | Successful response |  * Cache-Control - The &#x60;Cache-Control&#x60; header, in particular the &#x60;max-age&#x60; value, contains the number of seconds the result should be considered valid for. During this time subsequent calls should return from the local cache directly. <br>  * ETag - Specifies the version of the most recent response. Used by clients in the &#x60;If-None-Match&#x60; request header. <br>  |
| **304** | Not Modified - Use Local Cached Value |  -  |
| **401** | Authorization information is missing or invalid. |  -  |
| **404** | Not Found |  -  |

[[Back to top]](#) [[Back to API list]](../../README.md#documentation-for-api-endpoints) [[Back to Model list]](../../README.md#documentation-for-models) [[Back to README]](../../README.md)

<a id="getdistrictteamskeys"></a>
# **GetDistrictTeamsKeys**
> Collection&lt;string&gt; GetDistrictTeamsKeys (string districtKey, string? ifNoneMatch = null)



Gets a list of `Team` objects that competed in events in the given district.

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using TheBlueAlliance.Api;
using TheBlueAlliance.Client;
using TheBlueAlliance.Model;

namespace Example


    public class GetDistrictTeamsKeysExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "https://www.thebluealliance.com/api/v3";
            // Configure API key authorization: apiKey
            config.AddApiKey("X-TBA-Auth-Key", "YOUR_API_KEY");
            // Uncomment below to setup prefix (e.g. Bearer) for API key, if needed
            // config.AddApiKeyPrefix("X-TBA-Auth-Key", "Bearer");

            // create instances of HttpClient, HttpClientHandler to be reused later with different Api classes
            HttpClient httpClient = new HttpClient();
            HttpClientHandler httpClientHandler = new HttpClientHandler();
            var apiInstance = new TeamApi(httpClient, config, httpClientHandler);
            var districtKey = "districtKey_example";  // string | TBA District Key, eg `2016fim`
            var ifNoneMatch = "ifNoneMatch_example";  // string? | Value of the `ETag` header in the most recently cached response by the client. (optional) 

            try
            {
                Collection<string> result = apiInstance.GetDistrictTeamsKeys(districtKey, ifNoneMatch);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling TeamApi.GetDistrictTeamsKeys: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the GetDistrictTeamsKeysWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<Collection<string>> response = apiInstance.GetDistrictTeamsKeysWithHttpInfo(districtKey, ifNoneMatch);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling TeamApi.GetDistrictTeamsKeysWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **districtKey** | **string** | TBA District Key, eg &#x60;2016fim&#x60; |  |
| **ifNoneMatch** | **string?** | Value of the &#x60;ETag&#x60; header in the most recently cached response by the client. | [optional]  |

### Return type

**Collection<string>**

### Authorization

[apiKey](../README.md#apiKey)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | Successful response |  * Cache-Control - The &#x60;Cache-Control&#x60; header, in particular the &#x60;max-age&#x60; value, contains the number of seconds the result should be considered valid for. During this time subsequent calls should return from the local cache directly. <br>  * ETag - Specifies the version of the most recent response. Used by clients in the &#x60;If-None-Match&#x60; request header. <br>  |
| **304** | Not Modified - Use Local Cached Value |  -  |
| **401** | Authorization information is missing or invalid. |  -  |
| **404** | Not Found |  -  |

[[Back to top]](#) [[Back to API list]](../../README.md#documentation-for-api-endpoints) [[Back to Model list]](../../README.md#documentation-for-models) [[Back to README]](../../README.md)

<a id="getdistrictteamssimple"></a>
# **GetDistrictTeamsSimple**
> Collection&lt;TeamSimple&gt; GetDistrictTeamsSimple (string districtKey, string? ifNoneMatch = null)



Gets a short-form list of `Team` objects that competed in events in the given district.

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using TheBlueAlliance.Api;
using TheBlueAlliance.Client;
using TheBlueAlliance.Model;

namespace Example


    public class GetDistrictTeamsSimpleExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "https://www.thebluealliance.com/api/v3";
            // Configure API key authorization: apiKey
            config.AddApiKey("X-TBA-Auth-Key", "YOUR_API_KEY");
            // Uncomment below to setup prefix (e.g. Bearer) for API key, if needed
            // config.AddApiKeyPrefix("X-TBA-Auth-Key", "Bearer");

            // create instances of HttpClient, HttpClientHandler to be reused later with different Api classes
            HttpClient httpClient = new HttpClient();
            HttpClientHandler httpClientHandler = new HttpClientHandler();
            var apiInstance = new TeamApi(httpClient, config, httpClientHandler);
            var districtKey = "districtKey_example";  // string | TBA District Key, eg `2016fim`
            var ifNoneMatch = "ifNoneMatch_example";  // string? | Value of the `ETag` header in the most recently cached response by the client. (optional) 

            try
            {
                Collection<TeamSimple> result = apiInstance.GetDistrictTeamsSimple(districtKey, ifNoneMatch);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling TeamApi.GetDistrictTeamsSimple: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the GetDistrictTeamsSimpleWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<Collection<TeamSimple>> response = apiInstance.GetDistrictTeamsSimpleWithHttpInfo(districtKey, ifNoneMatch);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling TeamApi.GetDistrictTeamsSimpleWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **districtKey** | **string** | TBA District Key, eg &#x60;2016fim&#x60; |  |
| **ifNoneMatch** | **string?** | Value of the &#x60;ETag&#x60; header in the most recently cached response by the client. | [optional]  |

### Return type

[**Collection&lt;TeamSimple&gt;**](TeamSimple.md)

### Authorization

[apiKey](../README.md#apiKey)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | Successful response |  * Cache-Control - The &#x60;Cache-Control&#x60; header, in particular the &#x60;max-age&#x60; value, contains the number of seconds the result should be considered valid for. During this time subsequent calls should return from the local cache directly. <br>  * ETag - Specifies the version of the most recent response. Used by clients in the &#x60;If-None-Match&#x60; request header. <br>  |
| **304** | Not Modified - Use Local Cached Value |  -  |
| **401** | Authorization information is missing or invalid. |  -  |
| **404** | Not Found |  -  |

[[Back to top]](#) [[Back to API list]](../../README.md#documentation-for-api-endpoints) [[Back to Model list]](../../README.md#documentation-for-models) [[Back to README]](../../README.md)

<a id="geteventteams"></a>
# **GetEventTeams**
> Collection&lt;Team&gt; GetEventTeams (string eventKey, string? ifNoneMatch = null)



Gets a list of `Team` objects that competed in the given event.

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using TheBlueAlliance.Api;
using TheBlueAlliance.Client;
using TheBlueAlliance.Model;

namespace Example


    public class GetEventTeamsExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "https://www.thebluealliance.com/api/v3";
            // Configure API key authorization: apiKey
            config.AddApiKey("X-TBA-Auth-Key", "YOUR_API_KEY");
            // Uncomment below to setup prefix (e.g. Bearer) for API key, if needed
            // config.AddApiKeyPrefix("X-TBA-Auth-Key", "Bearer");

            // create instances of HttpClient, HttpClientHandler to be reused later with different Api classes
            HttpClient httpClient = new HttpClient();
            HttpClientHandler httpClientHandler = new HttpClientHandler();
            var apiInstance = new TeamApi(httpClient, config, httpClientHandler);
            var eventKey = "eventKey_example";  // string | TBA Event Key, eg `2016nytr`
            var ifNoneMatch = "ifNoneMatch_example";  // string? | Value of the `ETag` header in the most recently cached response by the client. (optional) 

            try
            {
                Collection<Team> result = apiInstance.GetEventTeams(eventKey, ifNoneMatch);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling TeamApi.GetEventTeams: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the GetEventTeamsWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<Collection<Team>> response = apiInstance.GetEventTeamsWithHttpInfo(eventKey, ifNoneMatch);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling TeamApi.GetEventTeamsWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **eventKey** | **string** | TBA Event Key, eg &#x60;2016nytr&#x60; |  |
| **ifNoneMatch** | **string?** | Value of the &#x60;ETag&#x60; header in the most recently cached response by the client. | [optional]  |

### Return type

[**Collection&lt;Team&gt;**](Team.md)

### Authorization

[apiKey](../README.md#apiKey)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | Successful response |  * Cache-Control - The &#x60;Cache-Control&#x60; header, in particular the &#x60;max-age&#x60; value, contains the number of seconds the result should be considered valid for. During this time subsequent calls should return from the local cache directly. <br>  * ETag - Specifies the version of the most recent response. Used by clients in the &#x60;If-None-Match&#x60; request header. <br>  |
| **304** | Not Modified - Use Local Cached Value |  -  |
| **401** | Authorization information is missing or invalid. |  -  |
| **404** | Not Found |  -  |

[[Back to top]](#) [[Back to API list]](../../README.md#documentation-for-api-endpoints) [[Back to Model list]](../../README.md#documentation-for-models) [[Back to README]](../../README.md)

<a id="geteventteamskeys"></a>
# **GetEventTeamsKeys**
> Collection&lt;string&gt; GetEventTeamsKeys (string eventKey, string? ifNoneMatch = null)



Gets a list of `Team` keys that competed in the given event.

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using TheBlueAlliance.Api;
using TheBlueAlliance.Client;
using TheBlueAlliance.Model;

namespace Example


    public class GetEventTeamsKeysExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "https://www.thebluealliance.com/api/v3";
            // Configure API key authorization: apiKey
            config.AddApiKey("X-TBA-Auth-Key", "YOUR_API_KEY");
            // Uncomment below to setup prefix (e.g. Bearer) for API key, if needed
            // config.AddApiKeyPrefix("X-TBA-Auth-Key", "Bearer");

            // create instances of HttpClient, HttpClientHandler to be reused later with different Api classes
            HttpClient httpClient = new HttpClient();
            HttpClientHandler httpClientHandler = new HttpClientHandler();
            var apiInstance = new TeamApi(httpClient, config, httpClientHandler);
            var eventKey = "eventKey_example";  // string | TBA Event Key, eg `2016nytr`
            var ifNoneMatch = "ifNoneMatch_example";  // string? | Value of the `ETag` header in the most recently cached response by the client. (optional) 

            try
            {
                Collection<string> result = apiInstance.GetEventTeamsKeys(eventKey, ifNoneMatch);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling TeamApi.GetEventTeamsKeys: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the GetEventTeamsKeysWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<Collection<string>> response = apiInstance.GetEventTeamsKeysWithHttpInfo(eventKey, ifNoneMatch);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling TeamApi.GetEventTeamsKeysWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **eventKey** | **string** | TBA Event Key, eg &#x60;2016nytr&#x60; |  |
| **ifNoneMatch** | **string?** | Value of the &#x60;ETag&#x60; header in the most recently cached response by the client. | [optional]  |

### Return type

**Collection<string>**

### Authorization

[apiKey](../README.md#apiKey)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | Successful response |  * Cache-Control - The &#x60;Cache-Control&#x60; header, in particular the &#x60;max-age&#x60; value, contains the number of seconds the result should be considered valid for. During this time subsequent calls should return from the local cache directly. <br>  * ETag - Specifies the version of the most recent response. Used by clients in the &#x60;If-None-Match&#x60; request header. <br>  |
| **304** | Not Modified - Use Local Cached Value |  -  |
| **401** | Authorization information is missing or invalid. |  -  |
| **404** | Not Found |  -  |

[[Back to top]](#) [[Back to API list]](../../README.md#documentation-for-api-endpoints) [[Back to Model list]](../../README.md#documentation-for-models) [[Back to README]](../../README.md)

<a id="geteventteamssimple"></a>
# **GetEventTeamsSimple**
> Collection&lt;TeamSimple&gt; GetEventTeamsSimple (string eventKey, string? ifNoneMatch = null)



Gets a short-form list of `Team` objects that competed in the given event.

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using TheBlueAlliance.Api;
using TheBlueAlliance.Client;
using TheBlueAlliance.Model;

namespace Example


    public class GetEventTeamsSimpleExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "https://www.thebluealliance.com/api/v3";
            // Configure API key authorization: apiKey
            config.AddApiKey("X-TBA-Auth-Key", "YOUR_API_KEY");
            // Uncomment below to setup prefix (e.g. Bearer) for API key, if needed
            // config.AddApiKeyPrefix("X-TBA-Auth-Key", "Bearer");

            // create instances of HttpClient, HttpClientHandler to be reused later with different Api classes
            HttpClient httpClient = new HttpClient();
            HttpClientHandler httpClientHandler = new HttpClientHandler();
            var apiInstance = new TeamApi(httpClient, config, httpClientHandler);
            var eventKey = "eventKey_example";  // string | TBA Event Key, eg `2016nytr`
            var ifNoneMatch = "ifNoneMatch_example";  // string? | Value of the `ETag` header in the most recently cached response by the client. (optional) 

            try
            {
                Collection<TeamSimple> result = apiInstance.GetEventTeamsSimple(eventKey, ifNoneMatch);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling TeamApi.GetEventTeamsSimple: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the GetEventTeamsSimpleWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<Collection<TeamSimple>> response = apiInstance.GetEventTeamsSimpleWithHttpInfo(eventKey, ifNoneMatch);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling TeamApi.GetEventTeamsSimpleWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **eventKey** | **string** | TBA Event Key, eg &#x60;2016nytr&#x60; |  |
| **ifNoneMatch** | **string?** | Value of the &#x60;ETag&#x60; header in the most recently cached response by the client. | [optional]  |

### Return type

[**Collection&lt;TeamSimple&gt;**](TeamSimple.md)

### Authorization

[apiKey](../README.md#apiKey)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | Successful response |  * Cache-Control - The &#x60;Cache-Control&#x60; header, in particular the &#x60;max-age&#x60; value, contains the number of seconds the result should be considered valid for. During this time subsequent calls should return from the local cache directly. <br>  * ETag - Specifies the version of the most recent response. Used by clients in the &#x60;If-None-Match&#x60; request header. <br>  |
| **304** | Not Modified - Use Local Cached Value |  -  |
| **401** | Authorization information is missing or invalid. |  -  |
| **404** | Not Found |  -  |

[[Back to top]](#) [[Back to API list]](../../README.md#documentation-for-api-endpoints) [[Back to Model list]](../../README.md#documentation-for-models) [[Back to README]](../../README.md)

<a id="geteventteamsstatuses"></a>
# **GetEventTeamsStatuses**
> Dictionary&lt;string, GetTeamEventsStatusesByYear200ResponseValue&gt; GetEventTeamsStatuses (string eventKey, string? ifNoneMatch = null)



Gets a key-value list of the event statuses for teams competing at the given event.

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using TheBlueAlliance.Api;
using TheBlueAlliance.Client;
using TheBlueAlliance.Model;

namespace Example


    public class GetEventTeamsStatusesExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "https://www.thebluealliance.com/api/v3";
            // Configure API key authorization: apiKey
            config.AddApiKey("X-TBA-Auth-Key", "YOUR_API_KEY");
            // Uncomment below to setup prefix (e.g. Bearer) for API key, if needed
            // config.AddApiKeyPrefix("X-TBA-Auth-Key", "Bearer");

            // create instances of HttpClient, HttpClientHandler to be reused later with different Api classes
            HttpClient httpClient = new HttpClient();
            HttpClientHandler httpClientHandler = new HttpClientHandler();
            var apiInstance = new TeamApi(httpClient, config, httpClientHandler);
            var eventKey = "eventKey_example";  // string | TBA Event Key, eg `2016nytr`
            var ifNoneMatch = "ifNoneMatch_example";  // string? | Value of the `ETag` header in the most recently cached response by the client. (optional) 

            try
            {
                Dictionary<string, GetTeamEventsStatusesByYear200ResponseValue> result = apiInstance.GetEventTeamsStatuses(eventKey, ifNoneMatch);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling TeamApi.GetEventTeamsStatuses: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the GetEventTeamsStatusesWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<Dictionary<string, GetTeamEventsStatusesByYear200ResponseValue>> response = apiInstance.GetEventTeamsStatusesWithHttpInfo(eventKey, ifNoneMatch);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling TeamApi.GetEventTeamsStatusesWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **eventKey** | **string** | TBA Event Key, eg &#x60;2016nytr&#x60; |  |
| **ifNoneMatch** | **string?** | Value of the &#x60;ETag&#x60; header in the most recently cached response by the client. | [optional]  |

### Return type

[**Dictionary&lt;string, GetTeamEventsStatusesByYear200ResponseValue&gt;**](GetTeamEventsStatusesByYear200ResponseValue.md)

### Authorization

[apiKey](../README.md#apiKey)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | Successful response |  * Cache-Control - The &#x60;Cache-Control&#x60; header, in particular the &#x60;max-age&#x60; value, contains the number of seconds the result should be considered valid for. During this time subsequent calls should return from the local cache directly. <br>  * ETag - Specifies the version of the most recent response. Used by clients in the &#x60;If-None-Match&#x60; request header. <br>  |
| **304** | Not Modified - Use Local Cached Value |  -  |
| **401** | Authorization information is missing or invalid. |  -  |
| **404** | Not Found |  -  |

[[Back to top]](#) [[Back to API list]](../../README.md#documentation-for-api-endpoints) [[Back to Model list]](../../README.md#documentation-for-models) [[Back to README]](../../README.md)

<a id="getteam"></a>
# **GetTeam**
> Team GetTeam (string teamKey, string? ifNoneMatch = null)



Gets a `Team` object for the team referenced by the given key.

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using TheBlueAlliance.Api;
using TheBlueAlliance.Client;
using TheBlueAlliance.Model;

namespace Example


    public class GetTeamExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "https://www.thebluealliance.com/api/v3";
            // Configure API key authorization: apiKey
            config.AddApiKey("X-TBA-Auth-Key", "YOUR_API_KEY");
            // Uncomment below to setup prefix (e.g. Bearer) for API key, if needed
            // config.AddApiKeyPrefix("X-TBA-Auth-Key", "Bearer");

            // create instances of HttpClient, HttpClientHandler to be reused later with different Api classes
            HttpClient httpClient = new HttpClient();
            HttpClientHandler httpClientHandler = new HttpClientHandler();
            var apiInstance = new TeamApi(httpClient, config, httpClientHandler);
            var teamKey = "teamKey_example";  // string | TBA Team Key, eg `frc254`
            var ifNoneMatch = "ifNoneMatch_example";  // string? | Value of the `ETag` header in the most recently cached response by the client. (optional) 

            try
            {
                Team result = apiInstance.GetTeam(teamKey, ifNoneMatch);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling TeamApi.GetTeam: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the GetTeamWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<Team> response = apiInstance.GetTeamWithHttpInfo(teamKey, ifNoneMatch);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling TeamApi.GetTeamWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **teamKey** | **string** | TBA Team Key, eg &#x60;frc254&#x60; |  |
| **ifNoneMatch** | **string?** | Value of the &#x60;ETag&#x60; header in the most recently cached response by the client. | [optional]  |

### Return type

[**Team**](Team.md)

### Authorization

[apiKey](../README.md#apiKey)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | Successful response |  * Cache-Control - The &#x60;Cache-Control&#x60; header, in particular the &#x60;max-age&#x60; value, contains the number of seconds the result should be considered valid for. During this time subsequent calls should return from the local cache directly. <br>  * ETag - Specifies the version of the most recent response. Used by clients in the &#x60;If-None-Match&#x60; request header. <br>  |
| **304** | Not Modified - Use Local Cached Value |  -  |
| **401** | Authorization information is missing or invalid. |  -  |
| **404** | Not Found |  -  |

[[Back to top]](#) [[Back to API list]](../../README.md#documentation-for-api-endpoints) [[Back to Model list]](../../README.md#documentation-for-models) [[Back to README]](../../README.md)

<a id="getteamawards"></a>
# **GetTeamAwards**
> Collection&lt;Award&gt; GetTeamAwards (string teamKey, string? ifNoneMatch = null)



Gets a list of awards the given team has won.

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using TheBlueAlliance.Api;
using TheBlueAlliance.Client;
using TheBlueAlliance.Model;

namespace Example


    public class GetTeamAwardsExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "https://www.thebluealliance.com/api/v3";
            // Configure API key authorization: apiKey
            config.AddApiKey("X-TBA-Auth-Key", "YOUR_API_KEY");
            // Uncomment below to setup prefix (e.g. Bearer) for API key, if needed
            // config.AddApiKeyPrefix("X-TBA-Auth-Key", "Bearer");

            // create instances of HttpClient, HttpClientHandler to be reused later with different Api classes
            HttpClient httpClient = new HttpClient();
            HttpClientHandler httpClientHandler = new HttpClientHandler();
            var apiInstance = new TeamApi(httpClient, config, httpClientHandler);
            var teamKey = "teamKey_example";  // string | TBA Team Key, eg `frc254`
            var ifNoneMatch = "ifNoneMatch_example";  // string? | Value of the `ETag` header in the most recently cached response by the client. (optional) 

            try
            {
                Collection<Award> result = apiInstance.GetTeamAwards(teamKey, ifNoneMatch);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling TeamApi.GetTeamAwards: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the GetTeamAwardsWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<Collection<Award>> response = apiInstance.GetTeamAwardsWithHttpInfo(teamKey, ifNoneMatch);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling TeamApi.GetTeamAwardsWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **teamKey** | **string** | TBA Team Key, eg &#x60;frc254&#x60; |  |
| **ifNoneMatch** | **string?** | Value of the &#x60;ETag&#x60; header in the most recently cached response by the client. | [optional]  |

### Return type

[**Collection&lt;Award&gt;**](Award.md)

### Authorization

[apiKey](../README.md#apiKey)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | Successful response |  * Cache-Control - The &#x60;Cache-Control&#x60; header, in particular the &#x60;max-age&#x60; value, contains the number of seconds the result should be considered valid for. During this time subsequent calls should return from the local cache directly. <br>  * ETag - Specifies the version of the most recent response. Used by clients in the &#x60;If-None-Match&#x60; request header. <br>  |
| **304** | Not Modified - Use Local Cached Value |  -  |
| **401** | Authorization information is missing or invalid. |  -  |
| **404** | Not Found |  -  |

[[Back to top]](#) [[Back to API list]](../../README.md#documentation-for-api-endpoints) [[Back to Model list]](../../README.md#documentation-for-models) [[Back to README]](../../README.md)

<a id="getteamawardsbyyear"></a>
# **GetTeamAwardsByYear**
> Collection&lt;Award&gt; GetTeamAwardsByYear (string teamKey, int year, string? ifNoneMatch = null)



Gets a list of awards the given team has won in a given year.

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using TheBlueAlliance.Api;
using TheBlueAlliance.Client;
using TheBlueAlliance.Model;

namespace Example


    public class GetTeamAwardsByYearExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "https://www.thebluealliance.com/api/v3";
            // Configure API key authorization: apiKey
            config.AddApiKey("X-TBA-Auth-Key", "YOUR_API_KEY");
            // Uncomment below to setup prefix (e.g. Bearer) for API key, if needed
            // config.AddApiKeyPrefix("X-TBA-Auth-Key", "Bearer");

            // create instances of HttpClient, HttpClientHandler to be reused later with different Api classes
            HttpClient httpClient = new HttpClient();
            HttpClientHandler httpClientHandler = new HttpClientHandler();
            var apiInstance = new TeamApi(httpClient, config, httpClientHandler);
            var teamKey = "teamKey_example";  // string | TBA Team Key, eg `frc254`
            var year = 56;  // int | Competition Year (or Season). Must be 4 digits.
            var ifNoneMatch = "ifNoneMatch_example";  // string? | Value of the `ETag` header in the most recently cached response by the client. (optional) 

            try
            {
                Collection<Award> result = apiInstance.GetTeamAwardsByYear(teamKey, year, ifNoneMatch);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling TeamApi.GetTeamAwardsByYear: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the GetTeamAwardsByYearWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<Collection<Award>> response = apiInstance.GetTeamAwardsByYearWithHttpInfo(teamKey, year, ifNoneMatch);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling TeamApi.GetTeamAwardsByYearWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **teamKey** | **string** | TBA Team Key, eg &#x60;frc254&#x60; |  |
| **year** | **int** | Competition Year (or Season). Must be 4 digits. |  |
| **ifNoneMatch** | **string?** | Value of the &#x60;ETag&#x60; header in the most recently cached response by the client. | [optional]  |

### Return type

[**Collection&lt;Award&gt;**](Award.md)

### Authorization

[apiKey](../README.md#apiKey)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | Successful response |  * Cache-Control - The &#x60;Cache-Control&#x60; header, in particular the &#x60;max-age&#x60; value, contains the number of seconds the result should be considered valid for. During this time subsequent calls should return from the local cache directly. <br>  * ETag - Specifies the version of the most recent response. Used by clients in the &#x60;If-None-Match&#x60; request header. <br>  |
| **304** | Not Modified - Use Local Cached Value |  -  |
| **401** | Authorization information is missing or invalid. |  -  |
| **404** | Not Found |  -  |

[[Back to top]](#) [[Back to API list]](../../README.md#documentation-for-api-endpoints) [[Back to Model list]](../../README.md#documentation-for-models) [[Back to README]](../../README.md)

<a id="getteamdistricts"></a>
# **GetTeamDistricts**
> Collection&lt;DistrictList&gt; GetTeamDistricts (string teamKey, string? ifNoneMatch = null)



Gets an array of districts representing each year the team was in a district. Will return an empty array if the team was never in a district.

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using TheBlueAlliance.Api;
using TheBlueAlliance.Client;
using TheBlueAlliance.Model;

namespace Example


    public class GetTeamDistrictsExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "https://www.thebluealliance.com/api/v3";
            // Configure API key authorization: apiKey
            config.AddApiKey("X-TBA-Auth-Key", "YOUR_API_KEY");
            // Uncomment below to setup prefix (e.g. Bearer) for API key, if needed
            // config.AddApiKeyPrefix("X-TBA-Auth-Key", "Bearer");

            // create instances of HttpClient, HttpClientHandler to be reused later with different Api classes
            HttpClient httpClient = new HttpClient();
            HttpClientHandler httpClientHandler = new HttpClientHandler();
            var apiInstance = new TeamApi(httpClient, config, httpClientHandler);
            var teamKey = "teamKey_example";  // string | TBA Team Key, eg `frc254`
            var ifNoneMatch = "ifNoneMatch_example";  // string? | Value of the `ETag` header in the most recently cached response by the client. (optional) 

            try
            {
                Collection<DistrictList> result = apiInstance.GetTeamDistricts(teamKey, ifNoneMatch);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling TeamApi.GetTeamDistricts: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the GetTeamDistrictsWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<Collection<DistrictList>> response = apiInstance.GetTeamDistrictsWithHttpInfo(teamKey, ifNoneMatch);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling TeamApi.GetTeamDistrictsWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **teamKey** | **string** | TBA Team Key, eg &#x60;frc254&#x60; |  |
| **ifNoneMatch** | **string?** | Value of the &#x60;ETag&#x60; header in the most recently cached response by the client. | [optional]  |

### Return type

[**Collection&lt;DistrictList&gt;**](DistrictList.md)

### Authorization

[apiKey](../README.md#apiKey)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | Successful response |  * Cache-Control - The &#x60;Cache-Control&#x60; header, in particular the &#x60;max-age&#x60; value, contains the number of seconds the result should be considered valid for. During this time subsequent calls should return from the local cache directly. <br>  * ETag - Specifies the version of the most recent response. Used by clients in the &#x60;If-None-Match&#x60; request header. <br>  |
| **304** | Not Modified - Use Local Cached Value |  -  |
| **401** | Authorization information is missing or invalid. |  -  |
| **404** | Not Found |  -  |

[[Back to top]](#) [[Back to API list]](../../README.md#documentation-for-api-endpoints) [[Back to Model list]](../../README.md#documentation-for-models) [[Back to README]](../../README.md)

<a id="getteameventawards"></a>
# **GetTeamEventAwards**
> Collection&lt;Award&gt; GetTeamEventAwards (string eventKey, string teamKey, string? ifNoneMatch = null)



Gets a list of awards the given team won at the given event.

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using TheBlueAlliance.Api;
using TheBlueAlliance.Client;
using TheBlueAlliance.Model;

namespace Example


    public class GetTeamEventAwardsExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "https://www.thebluealliance.com/api/v3";
            // Configure API key authorization: apiKey
            config.AddApiKey("X-TBA-Auth-Key", "YOUR_API_KEY");
            // Uncomment below to setup prefix (e.g. Bearer) for API key, if needed
            // config.AddApiKeyPrefix("X-TBA-Auth-Key", "Bearer");

            // create instances of HttpClient, HttpClientHandler to be reused later with different Api classes
            HttpClient httpClient = new HttpClient();
            HttpClientHandler httpClientHandler = new HttpClientHandler();
            var apiInstance = new TeamApi(httpClient, config, httpClientHandler);
            var eventKey = "eventKey_example";  // string | TBA Event Key, eg `2016nytr`
            var teamKey = "teamKey_example";  // string | TBA Team Key, eg `frc254`
            var ifNoneMatch = "ifNoneMatch_example";  // string? | Value of the `ETag` header in the most recently cached response by the client. (optional) 

            try
            {
                Collection<Award> result = apiInstance.GetTeamEventAwards(eventKey, teamKey, ifNoneMatch);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling TeamApi.GetTeamEventAwards: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the GetTeamEventAwardsWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<Collection<Award>> response = apiInstance.GetTeamEventAwardsWithHttpInfo(eventKey, teamKey, ifNoneMatch);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling TeamApi.GetTeamEventAwardsWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **eventKey** | **string** | TBA Event Key, eg &#x60;2016nytr&#x60; |  |
| **teamKey** | **string** | TBA Team Key, eg &#x60;frc254&#x60; |  |
| **ifNoneMatch** | **string?** | Value of the &#x60;ETag&#x60; header in the most recently cached response by the client. | [optional]  |

### Return type

[**Collection&lt;Award&gt;**](Award.md)

### Authorization

[apiKey](../README.md#apiKey)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | Successful response |  * Cache-Control - The &#x60;Cache-Control&#x60; header, in particular the &#x60;max-age&#x60; value, contains the number of seconds the result should be considered valid for. During this time subsequent calls should return from the local cache directly. <br>  * ETag - Specifies the version of the most recent response. Used by clients in the &#x60;If-None-Match&#x60; request header. <br>  |
| **304** | Not Modified - Use Local Cached Value |  -  |
| **401** | Authorization information is missing or invalid. |  -  |
| **404** | Not Found |  -  |

[[Back to top]](#) [[Back to API list]](../../README.md#documentation-for-api-endpoints) [[Back to Model list]](../../README.md#documentation-for-models) [[Back to README]](../../README.md)

<a id="getteameventmatches"></a>
# **GetTeamEventMatches**
> Collection&lt;Match&gt; GetTeamEventMatches (string eventKey, string teamKey, string? ifNoneMatch = null)



Gets a list of matches for the given team and event.

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using TheBlueAlliance.Api;
using TheBlueAlliance.Client;
using TheBlueAlliance.Model;

namespace Example


    public class GetTeamEventMatchesExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "https://www.thebluealliance.com/api/v3";
            // Configure API key authorization: apiKey
            config.AddApiKey("X-TBA-Auth-Key", "YOUR_API_KEY");
            // Uncomment below to setup prefix (e.g. Bearer) for API key, if needed
            // config.AddApiKeyPrefix("X-TBA-Auth-Key", "Bearer");

            // create instances of HttpClient, HttpClientHandler to be reused later with different Api classes
            HttpClient httpClient = new HttpClient();
            HttpClientHandler httpClientHandler = new HttpClientHandler();
            var apiInstance = new TeamApi(httpClient, config, httpClientHandler);
            var eventKey = "eventKey_example";  // string | TBA Event Key, eg `2016nytr`
            var teamKey = "teamKey_example";  // string | TBA Team Key, eg `frc254`
            var ifNoneMatch = "ifNoneMatch_example";  // string? | Value of the `ETag` header in the most recently cached response by the client. (optional) 

            try
            {
                Collection<Match> result = apiInstance.GetTeamEventMatches(eventKey, teamKey, ifNoneMatch);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling TeamApi.GetTeamEventMatches: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the GetTeamEventMatchesWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<Collection<Match>> response = apiInstance.GetTeamEventMatchesWithHttpInfo(eventKey, teamKey, ifNoneMatch);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling TeamApi.GetTeamEventMatchesWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **eventKey** | **string** | TBA Event Key, eg &#x60;2016nytr&#x60; |  |
| **teamKey** | **string** | TBA Team Key, eg &#x60;frc254&#x60; |  |
| **ifNoneMatch** | **string?** | Value of the &#x60;ETag&#x60; header in the most recently cached response by the client. | [optional]  |

### Return type

[**Collection&lt;Match&gt;**](Match.md)

### Authorization

[apiKey](../README.md#apiKey)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | Successful response |  * Cache-Control - The &#x60;Cache-Control&#x60; header, in particular the &#x60;max-age&#x60; value, contains the number of seconds the result should be considered valid for. During this time subsequent calls should return from the local cache directly. <br>  * ETag - Specifies the version of the most recent response. Used by clients in the &#x60;If-None-Match&#x60; request header. <br>  |
| **304** | Not Modified - Use Local Cached Value |  -  |
| **401** | Authorization information is missing or invalid. |  -  |
| **404** | Not Found |  -  |

[[Back to top]](#) [[Back to API list]](../../README.md#documentation-for-api-endpoints) [[Back to Model list]](../../README.md#documentation-for-models) [[Back to README]](../../README.md)

<a id="getteameventmatcheskeys"></a>
# **GetTeamEventMatchesKeys**
> Collection&lt;string&gt; GetTeamEventMatchesKeys (string eventKey, string teamKey, string? ifNoneMatch = null)



Gets a list of match keys for matches for the given team and event.

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using TheBlueAlliance.Api;
using TheBlueAlliance.Client;
using TheBlueAlliance.Model;

namespace Example


    public class GetTeamEventMatchesKeysExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "https://www.thebluealliance.com/api/v3";
            // Configure API key authorization: apiKey
            config.AddApiKey("X-TBA-Auth-Key", "YOUR_API_KEY");
            // Uncomment below to setup prefix (e.g. Bearer) for API key, if needed
            // config.AddApiKeyPrefix("X-TBA-Auth-Key", "Bearer");

            // create instances of HttpClient, HttpClientHandler to be reused later with different Api classes
            HttpClient httpClient = new HttpClient();
            HttpClientHandler httpClientHandler = new HttpClientHandler();
            var apiInstance = new TeamApi(httpClient, config, httpClientHandler);
            var eventKey = "eventKey_example";  // string | TBA Event Key, eg `2016nytr`
            var teamKey = "teamKey_example";  // string | TBA Team Key, eg `frc254`
            var ifNoneMatch = "ifNoneMatch_example";  // string? | Value of the `ETag` header in the most recently cached response by the client. (optional) 

            try
            {
                Collection<string> result = apiInstance.GetTeamEventMatchesKeys(eventKey, teamKey, ifNoneMatch);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling TeamApi.GetTeamEventMatchesKeys: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the GetTeamEventMatchesKeysWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<Collection<string>> response = apiInstance.GetTeamEventMatchesKeysWithHttpInfo(eventKey, teamKey, ifNoneMatch);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling TeamApi.GetTeamEventMatchesKeysWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **eventKey** | **string** | TBA Event Key, eg &#x60;2016nytr&#x60; |  |
| **teamKey** | **string** | TBA Team Key, eg &#x60;frc254&#x60; |  |
| **ifNoneMatch** | **string?** | Value of the &#x60;ETag&#x60; header in the most recently cached response by the client. | [optional]  |

### Return type

**Collection<string>**

### Authorization

[apiKey](../README.md#apiKey)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | Successful response |  * Cache-Control - The &#x60;Cache-Control&#x60; header, in particular the &#x60;max-age&#x60; value, contains the number of seconds the result should be considered valid for. During this time subsequent calls should return from the local cache directly. <br>  * ETag - Specifies the version of the most recent response. Used by clients in the &#x60;If-None-Match&#x60; request header. <br>  |
| **304** | Not Modified - Use Local Cached Value |  -  |
| **401** | Authorization information is missing or invalid. |  -  |
| **404** | Not Found |  -  |

[[Back to top]](#) [[Back to API list]](../../README.md#documentation-for-api-endpoints) [[Back to Model list]](../../README.md#documentation-for-models) [[Back to README]](../../README.md)

<a id="getteameventmatchessimple"></a>
# **GetTeamEventMatchesSimple**
> Collection&lt;Match&gt; GetTeamEventMatchesSimple (string eventKey, string teamKey, string? ifNoneMatch = null)



Gets a short-form list of matches for the given team and event.

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using TheBlueAlliance.Api;
using TheBlueAlliance.Client;
using TheBlueAlliance.Model;

namespace Example


    public class GetTeamEventMatchesSimpleExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "https://www.thebluealliance.com/api/v3";
            // Configure API key authorization: apiKey
            config.AddApiKey("X-TBA-Auth-Key", "YOUR_API_KEY");
            // Uncomment below to setup prefix (e.g. Bearer) for API key, if needed
            // config.AddApiKeyPrefix("X-TBA-Auth-Key", "Bearer");

            // create instances of HttpClient, HttpClientHandler to be reused later with different Api classes
            HttpClient httpClient = new HttpClient();
            HttpClientHandler httpClientHandler = new HttpClientHandler();
            var apiInstance = new TeamApi(httpClient, config, httpClientHandler);
            var eventKey = "eventKey_example";  // string | TBA Event Key, eg `2016nytr`
            var teamKey = "teamKey_example";  // string | TBA Team Key, eg `frc254`
            var ifNoneMatch = "ifNoneMatch_example";  // string? | Value of the `ETag` header in the most recently cached response by the client. (optional) 

            try
            {
                Collection<Match> result = apiInstance.GetTeamEventMatchesSimple(eventKey, teamKey, ifNoneMatch);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling TeamApi.GetTeamEventMatchesSimple: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the GetTeamEventMatchesSimpleWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<Collection<Match>> response = apiInstance.GetTeamEventMatchesSimpleWithHttpInfo(eventKey, teamKey, ifNoneMatch);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling TeamApi.GetTeamEventMatchesSimpleWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **eventKey** | **string** | TBA Event Key, eg &#x60;2016nytr&#x60; |  |
| **teamKey** | **string** | TBA Team Key, eg &#x60;frc254&#x60; |  |
| **ifNoneMatch** | **string?** | Value of the &#x60;ETag&#x60; header in the most recently cached response by the client. | [optional]  |

### Return type

[**Collection&lt;Match&gt;**](Match.md)

### Authorization

[apiKey](../README.md#apiKey)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | Successful response |  * Cache-Control - The &#x60;Cache-Control&#x60; header, in particular the &#x60;max-age&#x60; value, contains the number of seconds the result should be considered valid for. During this time subsequent calls should return from the local cache directly. <br>  * ETag - Specifies the version of the most recent response. Used by clients in the &#x60;If-None-Match&#x60; request header. <br>  |
| **304** | Not Modified - Use Local Cached Value |  -  |
| **401** | Authorization information is missing or invalid. |  -  |
| **404** | Not Found |  -  |

[[Back to top]](#) [[Back to API list]](../../README.md#documentation-for-api-endpoints) [[Back to Model list]](../../README.md#documentation-for-models) [[Back to README]](../../README.md)

<a id="getteameventstatus"></a>
# **GetTeamEventStatus**
> TeamEventStatus GetTeamEventStatus (string eventKey, string teamKey, string? ifNoneMatch = null)



Gets the competition rank and status of the team at the given event.

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using TheBlueAlliance.Api;
using TheBlueAlliance.Client;
using TheBlueAlliance.Model;

namespace Example


    public class GetTeamEventStatusExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "https://www.thebluealliance.com/api/v3";
            // Configure API key authorization: apiKey
            config.AddApiKey("X-TBA-Auth-Key", "YOUR_API_KEY");
            // Uncomment below to setup prefix (e.g. Bearer) for API key, if needed
            // config.AddApiKeyPrefix("X-TBA-Auth-Key", "Bearer");

            // create instances of HttpClient, HttpClientHandler to be reused later with different Api classes
            HttpClient httpClient = new HttpClient();
            HttpClientHandler httpClientHandler = new HttpClientHandler();
            var apiInstance = new TeamApi(httpClient, config, httpClientHandler);
            var eventKey = "eventKey_example";  // string | TBA Event Key, eg `2016nytr`
            var teamKey = "teamKey_example";  // string | TBA Team Key, eg `frc254`
            var ifNoneMatch = "ifNoneMatch_example";  // string? | Value of the `ETag` header in the most recently cached response by the client. (optional) 

            try
            {
                TeamEventStatus result = apiInstance.GetTeamEventStatus(eventKey, teamKey, ifNoneMatch);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling TeamApi.GetTeamEventStatus: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the GetTeamEventStatusWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<TeamEventStatus> response = apiInstance.GetTeamEventStatusWithHttpInfo(eventKey, teamKey, ifNoneMatch);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling TeamApi.GetTeamEventStatusWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **eventKey** | **string** | TBA Event Key, eg &#x60;2016nytr&#x60; |  |
| **teamKey** | **string** | TBA Team Key, eg &#x60;frc254&#x60; |  |
| **ifNoneMatch** | **string?** | Value of the &#x60;ETag&#x60; header in the most recently cached response by the client. | [optional]  |

### Return type

[**TeamEventStatus**](TeamEventStatus.md)

### Authorization

[apiKey](../README.md#apiKey)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | Successful response |  * Cache-Control - The &#x60;Cache-Control&#x60; header, in particular the &#x60;max-age&#x60; value, contains the number of seconds the result should be considered valid for. During this time subsequent calls should return from the local cache directly. <br>  * ETag - Specifies the version of the most recent response. Used by clients in the &#x60;If-None-Match&#x60; request header. <br>  |
| **304** | Not Modified - Use Local Cached Value |  -  |
| **401** | Authorization information is missing or invalid. |  -  |
| **404** | Not Found |  -  |

[[Back to top]](#) [[Back to API list]](../../README.md#documentation-for-api-endpoints) [[Back to Model list]](../../README.md#documentation-for-models) [[Back to README]](../../README.md)

<a id="getteamevents"></a>
# **GetTeamEvents**
> Collection&lt;Event&gt; GetTeamEvents (string teamKey, string? ifNoneMatch = null)



Gets a list of all events this team has competed at.

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using TheBlueAlliance.Api;
using TheBlueAlliance.Client;
using TheBlueAlliance.Model;

namespace Example


    public class GetTeamEventsExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "https://www.thebluealliance.com/api/v3";
            // Configure API key authorization: apiKey
            config.AddApiKey("X-TBA-Auth-Key", "YOUR_API_KEY");
            // Uncomment below to setup prefix (e.g. Bearer) for API key, if needed
            // config.AddApiKeyPrefix("X-TBA-Auth-Key", "Bearer");

            // create instances of HttpClient, HttpClientHandler to be reused later with different Api classes
            HttpClient httpClient = new HttpClient();
            HttpClientHandler httpClientHandler = new HttpClientHandler();
            var apiInstance = new TeamApi(httpClient, config, httpClientHandler);
            var teamKey = "teamKey_example";  // string | TBA Team Key, eg `frc254`
            var ifNoneMatch = "ifNoneMatch_example";  // string? | Value of the `ETag` header in the most recently cached response by the client. (optional) 

            try
            {
                Collection<Event> result = apiInstance.GetTeamEvents(teamKey, ifNoneMatch);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling TeamApi.GetTeamEvents: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the GetTeamEventsWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<Collection<Event>> response = apiInstance.GetTeamEventsWithHttpInfo(teamKey, ifNoneMatch);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling TeamApi.GetTeamEventsWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **teamKey** | **string** | TBA Team Key, eg &#x60;frc254&#x60; |  |
| **ifNoneMatch** | **string?** | Value of the &#x60;ETag&#x60; header in the most recently cached response by the client. | [optional]  |

### Return type

[**Collection&lt;Event&gt;**](Event.md)

### Authorization

[apiKey](../README.md#apiKey)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | Successful response |  * Cache-Control - The &#x60;Cache-Control&#x60; header, in particular the &#x60;max-age&#x60; value, contains the number of seconds the result should be considered valid for. During this time subsequent calls should return from the local cache directly. <br>  * ETag - Specifies the version of the most recent response. Used by clients in the &#x60;If-None-Match&#x60; request header. <br>  |
| **304** | Not Modified - Use Local Cached Value |  -  |
| **401** | Authorization information is missing or invalid. |  -  |
| **404** | Not Found |  -  |

[[Back to top]](#) [[Back to API list]](../../README.md#documentation-for-api-endpoints) [[Back to Model list]](../../README.md#documentation-for-models) [[Back to README]](../../README.md)

<a id="getteameventsbyyear"></a>
# **GetTeamEventsByYear**
> Collection&lt;Event&gt; GetTeamEventsByYear (string teamKey, int year, string? ifNoneMatch = null)



Gets a list of events this team has competed at in the given year.

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using TheBlueAlliance.Api;
using TheBlueAlliance.Client;
using TheBlueAlliance.Model;

namespace Example


    public class GetTeamEventsByYearExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "https://www.thebluealliance.com/api/v3";
            // Configure API key authorization: apiKey
            config.AddApiKey("X-TBA-Auth-Key", "YOUR_API_KEY");
            // Uncomment below to setup prefix (e.g. Bearer) for API key, if needed
            // config.AddApiKeyPrefix("X-TBA-Auth-Key", "Bearer");

            // create instances of HttpClient, HttpClientHandler to be reused later with different Api classes
            HttpClient httpClient = new HttpClient();
            HttpClientHandler httpClientHandler = new HttpClientHandler();
            var apiInstance = new TeamApi(httpClient, config, httpClientHandler);
            var teamKey = "teamKey_example";  // string | TBA Team Key, eg `frc254`
            var year = 56;  // int | Competition Year (or Season). Must be 4 digits.
            var ifNoneMatch = "ifNoneMatch_example";  // string? | Value of the `ETag` header in the most recently cached response by the client. (optional) 

            try
            {
                Collection<Event> result = apiInstance.GetTeamEventsByYear(teamKey, year, ifNoneMatch);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling TeamApi.GetTeamEventsByYear: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the GetTeamEventsByYearWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<Collection<Event>> response = apiInstance.GetTeamEventsByYearWithHttpInfo(teamKey, year, ifNoneMatch);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling TeamApi.GetTeamEventsByYearWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **teamKey** | **string** | TBA Team Key, eg &#x60;frc254&#x60; |  |
| **year** | **int** | Competition Year (or Season). Must be 4 digits. |  |
| **ifNoneMatch** | **string?** | Value of the &#x60;ETag&#x60; header in the most recently cached response by the client. | [optional]  |

### Return type

[**Collection&lt;Event&gt;**](Event.md)

### Authorization

[apiKey](../README.md#apiKey)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | Successful response |  * Cache-Control - The &#x60;Cache-Control&#x60; header, in particular the &#x60;max-age&#x60; value, contains the number of seconds the result should be considered valid for. During this time subsequent calls should return from the local cache directly. <br>  * ETag - Specifies the version of the most recent response. Used by clients in the &#x60;If-None-Match&#x60; request header. <br>  |
| **304** | Not Modified - Use Local Cached Value |  -  |
| **401** | Authorization information is missing or invalid. |  -  |
| **404** | Not Found |  -  |

[[Back to top]](#) [[Back to API list]](../../README.md#documentation-for-api-endpoints) [[Back to Model list]](../../README.md#documentation-for-models) [[Back to README]](../../README.md)

<a id="getteameventsbyyearkeys"></a>
# **GetTeamEventsByYearKeys**
> Collection&lt;string&gt; GetTeamEventsByYearKeys (string teamKey, int year, string? ifNoneMatch = null)



Gets a list of the event keys for events this team has competed at in the given year.

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using TheBlueAlliance.Api;
using TheBlueAlliance.Client;
using TheBlueAlliance.Model;

namespace Example


    public class GetTeamEventsByYearKeysExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "https://www.thebluealliance.com/api/v3";
            // Configure API key authorization: apiKey
            config.AddApiKey("X-TBA-Auth-Key", "YOUR_API_KEY");
            // Uncomment below to setup prefix (e.g. Bearer) for API key, if needed
            // config.AddApiKeyPrefix("X-TBA-Auth-Key", "Bearer");

            // create instances of HttpClient, HttpClientHandler to be reused later with different Api classes
            HttpClient httpClient = new HttpClient();
            HttpClientHandler httpClientHandler = new HttpClientHandler();
            var apiInstance = new TeamApi(httpClient, config, httpClientHandler);
            var teamKey = "teamKey_example";  // string | TBA Team Key, eg `frc254`
            var year = 56;  // int | Competition Year (or Season). Must be 4 digits.
            var ifNoneMatch = "ifNoneMatch_example";  // string? | Value of the `ETag` header in the most recently cached response by the client. (optional) 

            try
            {
                Collection<string> result = apiInstance.GetTeamEventsByYearKeys(teamKey, year, ifNoneMatch);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling TeamApi.GetTeamEventsByYearKeys: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the GetTeamEventsByYearKeysWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<Collection<string>> response = apiInstance.GetTeamEventsByYearKeysWithHttpInfo(teamKey, year, ifNoneMatch);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling TeamApi.GetTeamEventsByYearKeysWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **teamKey** | **string** | TBA Team Key, eg &#x60;frc254&#x60; |  |
| **year** | **int** | Competition Year (or Season). Must be 4 digits. |  |
| **ifNoneMatch** | **string?** | Value of the &#x60;ETag&#x60; header in the most recently cached response by the client. | [optional]  |

### Return type

**Collection<string>**

### Authorization

[apiKey](../README.md#apiKey)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | Successful response |  * Cache-Control - The &#x60;Cache-Control&#x60; header, in particular the &#x60;max-age&#x60; value, contains the number of seconds the result should be considered valid for. During this time subsequent calls should return from the local cache directly. <br>  * ETag - Specifies the version of the most recent response. Used by clients in the &#x60;If-None-Match&#x60; request header. <br>  |
| **304** | Not Modified - Use Local Cached Value |  -  |
| **401** | Authorization information is missing or invalid. |  -  |
| **404** | Not Found |  -  |

[[Back to top]](#) [[Back to API list]](../../README.md#documentation-for-api-endpoints) [[Back to Model list]](../../README.md#documentation-for-models) [[Back to README]](../../README.md)

<a id="getteameventsbyyearsimple"></a>
# **GetTeamEventsByYearSimple**
> Collection&lt;EventSimple&gt; GetTeamEventsByYearSimple (string teamKey, int year, string? ifNoneMatch = null)



Gets a short-form list of events this team has competed at in the given year.

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using TheBlueAlliance.Api;
using TheBlueAlliance.Client;
using TheBlueAlliance.Model;

namespace Example


    public class GetTeamEventsByYearSimpleExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "https://www.thebluealliance.com/api/v3";
            // Configure API key authorization: apiKey
            config.AddApiKey("X-TBA-Auth-Key", "YOUR_API_KEY");
            // Uncomment below to setup prefix (e.g. Bearer) for API key, if needed
            // config.AddApiKeyPrefix("X-TBA-Auth-Key", "Bearer");

            // create instances of HttpClient, HttpClientHandler to be reused later with different Api classes
            HttpClient httpClient = new HttpClient();
            HttpClientHandler httpClientHandler = new HttpClientHandler();
            var apiInstance = new TeamApi(httpClient, config, httpClientHandler);
            var teamKey = "teamKey_example";  // string | TBA Team Key, eg `frc254`
            var year = 56;  // int | Competition Year (or Season). Must be 4 digits.
            var ifNoneMatch = "ifNoneMatch_example";  // string? | Value of the `ETag` header in the most recently cached response by the client. (optional) 

            try
            {
                Collection<EventSimple> result = apiInstance.GetTeamEventsByYearSimple(teamKey, year, ifNoneMatch);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling TeamApi.GetTeamEventsByYearSimple: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the GetTeamEventsByYearSimpleWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<Collection<EventSimple>> response = apiInstance.GetTeamEventsByYearSimpleWithHttpInfo(teamKey, year, ifNoneMatch);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling TeamApi.GetTeamEventsByYearSimpleWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **teamKey** | **string** | TBA Team Key, eg &#x60;frc254&#x60; |  |
| **year** | **int** | Competition Year (or Season). Must be 4 digits. |  |
| **ifNoneMatch** | **string?** | Value of the &#x60;ETag&#x60; header in the most recently cached response by the client. | [optional]  |

### Return type

[**Collection&lt;EventSimple&gt;**](EventSimple.md)

### Authorization

[apiKey](../README.md#apiKey)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | Successful response |  * Cache-Control - The &#x60;Cache-Control&#x60; header, in particular the &#x60;max-age&#x60; value, contains the number of seconds the result should be considered valid for. During this time subsequent calls should return from the local cache directly. <br>  * ETag - Specifies the version of the most recent response. Used by clients in the &#x60;If-None-Match&#x60; request header. <br>  |
| **304** | Not Modified - Use Local Cached Value |  -  |
| **401** | Authorization information is missing or invalid. |  -  |
| **404** | Not Found |  -  |

[[Back to top]](#) [[Back to API list]](../../README.md#documentation-for-api-endpoints) [[Back to Model list]](../../README.md#documentation-for-models) [[Back to README]](../../README.md)

<a id="getteameventskeys"></a>
# **GetTeamEventsKeys**
> Collection&lt;string&gt; GetTeamEventsKeys (string teamKey, string? ifNoneMatch = null)



Gets a list of the event keys for all events this team has competed at.

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using TheBlueAlliance.Api;
using TheBlueAlliance.Client;
using TheBlueAlliance.Model;

namespace Example


    public class GetTeamEventsKeysExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "https://www.thebluealliance.com/api/v3";
            // Configure API key authorization: apiKey
            config.AddApiKey("X-TBA-Auth-Key", "YOUR_API_KEY");
            // Uncomment below to setup prefix (e.g. Bearer) for API key, if needed
            // config.AddApiKeyPrefix("X-TBA-Auth-Key", "Bearer");

            // create instances of HttpClient, HttpClientHandler to be reused later with different Api classes
            HttpClient httpClient = new HttpClient();
            HttpClientHandler httpClientHandler = new HttpClientHandler();
            var apiInstance = new TeamApi(httpClient, config, httpClientHandler);
            var teamKey = "teamKey_example";  // string | TBA Team Key, eg `frc254`
            var ifNoneMatch = "ifNoneMatch_example";  // string? | Value of the `ETag` header in the most recently cached response by the client. (optional) 

            try
            {
                Collection<string> result = apiInstance.GetTeamEventsKeys(teamKey, ifNoneMatch);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling TeamApi.GetTeamEventsKeys: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the GetTeamEventsKeysWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<Collection<string>> response = apiInstance.GetTeamEventsKeysWithHttpInfo(teamKey, ifNoneMatch);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling TeamApi.GetTeamEventsKeysWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **teamKey** | **string** | TBA Team Key, eg &#x60;frc254&#x60; |  |
| **ifNoneMatch** | **string?** | Value of the &#x60;ETag&#x60; header in the most recently cached response by the client. | [optional]  |

### Return type

**Collection<string>**

### Authorization

[apiKey](../README.md#apiKey)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | Successful response |  * Cache-Control - The &#x60;Cache-Control&#x60; header, in particular the &#x60;max-age&#x60; value, contains the number of seconds the result should be considered valid for. During this time subsequent calls should return from the local cache directly. <br>  * ETag - Specifies the version of the most recent response. Used by clients in the &#x60;If-None-Match&#x60; request header. <br>  |
| **304** | Not Modified - Use Local Cached Value |  -  |
| **401** | Authorization information is missing or invalid. |  -  |
| **404** | Not Found |  -  |

[[Back to top]](#) [[Back to API list]](../../README.md#documentation-for-api-endpoints) [[Back to Model list]](../../README.md#documentation-for-models) [[Back to README]](../../README.md)

<a id="getteameventssimple"></a>
# **GetTeamEventsSimple**
> Collection&lt;EventSimple&gt; GetTeamEventsSimple (string teamKey, string? ifNoneMatch = null)



Gets a short-form list of all events this team has competed at.

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using TheBlueAlliance.Api;
using TheBlueAlliance.Client;
using TheBlueAlliance.Model;

namespace Example


    public class GetTeamEventsSimpleExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "https://www.thebluealliance.com/api/v3";
            // Configure API key authorization: apiKey
            config.AddApiKey("X-TBA-Auth-Key", "YOUR_API_KEY");
            // Uncomment below to setup prefix (e.g. Bearer) for API key, if needed
            // config.AddApiKeyPrefix("X-TBA-Auth-Key", "Bearer");

            // create instances of HttpClient, HttpClientHandler to be reused later with different Api classes
            HttpClient httpClient = new HttpClient();
            HttpClientHandler httpClientHandler = new HttpClientHandler();
            var apiInstance = new TeamApi(httpClient, config, httpClientHandler);
            var teamKey = "teamKey_example";  // string | TBA Team Key, eg `frc254`
            var ifNoneMatch = "ifNoneMatch_example";  // string? | Value of the `ETag` header in the most recently cached response by the client. (optional) 

            try
            {
                Collection<EventSimple> result = apiInstance.GetTeamEventsSimple(teamKey, ifNoneMatch);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling TeamApi.GetTeamEventsSimple: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the GetTeamEventsSimpleWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<Collection<EventSimple>> response = apiInstance.GetTeamEventsSimpleWithHttpInfo(teamKey, ifNoneMatch);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling TeamApi.GetTeamEventsSimpleWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **teamKey** | **string** | TBA Team Key, eg &#x60;frc254&#x60; |  |
| **ifNoneMatch** | **string?** | Value of the &#x60;ETag&#x60; header in the most recently cached response by the client. | [optional]  |

### Return type

[**Collection&lt;EventSimple&gt;**](EventSimple.md)

### Authorization

[apiKey](../README.md#apiKey)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | Successful response |  * Cache-Control - The &#x60;Cache-Control&#x60; header, in particular the &#x60;max-age&#x60; value, contains the number of seconds the result should be considered valid for. During this time subsequent calls should return from the local cache directly. <br>  * ETag - Specifies the version of the most recent response. Used by clients in the &#x60;If-None-Match&#x60; request header. <br>  |
| **304** | Not Modified - Use Local Cached Value |  -  |
| **401** | Authorization information is missing or invalid. |  -  |
| **404** | Not Found |  -  |

[[Back to top]](#) [[Back to API list]](../../README.md#documentation-for-api-endpoints) [[Back to Model list]](../../README.md#documentation-for-models) [[Back to README]](../../README.md)

<a id="getteameventsstatusesbyyear"></a>
# **GetTeamEventsStatusesByYear**
> Dictionary&lt;string, GetTeamEventsStatusesByYear200ResponseValue&gt; GetTeamEventsStatusesByYear (string teamKey, int year, string? ifNoneMatch = null)



Gets a key-value list of the event statuses for events this team has competed at in the given year.

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using TheBlueAlliance.Api;
using TheBlueAlliance.Client;
using TheBlueAlliance.Model;

namespace Example


    public class GetTeamEventsStatusesByYearExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "https://www.thebluealliance.com/api/v3";
            // Configure API key authorization: apiKey
            config.AddApiKey("X-TBA-Auth-Key", "YOUR_API_KEY");
            // Uncomment below to setup prefix (e.g. Bearer) for API key, if needed
            // config.AddApiKeyPrefix("X-TBA-Auth-Key", "Bearer");

            // create instances of HttpClient, HttpClientHandler to be reused later with different Api classes
            HttpClient httpClient = new HttpClient();
            HttpClientHandler httpClientHandler = new HttpClientHandler();
            var apiInstance = new TeamApi(httpClient, config, httpClientHandler);
            var teamKey = "teamKey_example";  // string | TBA Team Key, eg `frc254`
            var year = 56;  // int | Competition Year (or Season). Must be 4 digits.
            var ifNoneMatch = "ifNoneMatch_example";  // string? | Value of the `ETag` header in the most recently cached response by the client. (optional) 

            try
            {
                Dictionary<string, GetTeamEventsStatusesByYear200ResponseValue> result = apiInstance.GetTeamEventsStatusesByYear(teamKey, year, ifNoneMatch);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling TeamApi.GetTeamEventsStatusesByYear: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the GetTeamEventsStatusesByYearWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<Dictionary<string, GetTeamEventsStatusesByYear200ResponseValue>> response = apiInstance.GetTeamEventsStatusesByYearWithHttpInfo(teamKey, year, ifNoneMatch);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling TeamApi.GetTeamEventsStatusesByYearWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **teamKey** | **string** | TBA Team Key, eg &#x60;frc254&#x60; |  |
| **year** | **int** | Competition Year (or Season). Must be 4 digits. |  |
| **ifNoneMatch** | **string?** | Value of the &#x60;ETag&#x60; header in the most recently cached response by the client. | [optional]  |

### Return type

[**Dictionary&lt;string, GetTeamEventsStatusesByYear200ResponseValue&gt;**](GetTeamEventsStatusesByYear200ResponseValue.md)

### Authorization

[apiKey](../README.md#apiKey)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | Successful response |  * Cache-Control - The &#x60;Cache-Control&#x60; header, in particular the &#x60;max-age&#x60; value, contains the number of seconds the result should be considered valid for. During this time subsequent calls should return from the local cache directly. <br>  * ETag - Specifies the version of the most recent response. Used by clients in the &#x60;If-None-Match&#x60; request header. <br>  |
| **304** | Not Modified - Use Local Cached Value |  -  |
| **401** | Authorization information is missing or invalid. |  -  |
| **404** | Not Found |  -  |

[[Back to top]](#) [[Back to API list]](../../README.md#documentation-for-api-endpoints) [[Back to Model list]](../../README.md#documentation-for-models) [[Back to README]](../../README.md)

<a id="getteamhistory"></a>
# **GetTeamHistory**
> History GetTeamHistory (string teamKey, string? ifNoneMatch = null)



Gets the history for the team referenced by the given key, including their events and awards.

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using TheBlueAlliance.Api;
using TheBlueAlliance.Client;
using TheBlueAlliance.Model;

namespace Example


    public class GetTeamHistoryExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "https://www.thebluealliance.com/api/v3";
            // Configure API key authorization: apiKey
            config.AddApiKey("X-TBA-Auth-Key", "YOUR_API_KEY");
            // Uncomment below to setup prefix (e.g. Bearer) for API key, if needed
            // config.AddApiKeyPrefix("X-TBA-Auth-Key", "Bearer");

            // create instances of HttpClient, HttpClientHandler to be reused later with different Api classes
            HttpClient httpClient = new HttpClient();
            HttpClientHandler httpClientHandler = new HttpClientHandler();
            var apiInstance = new TeamApi(httpClient, config, httpClientHandler);
            var teamKey = "teamKey_example";  // string | TBA Team Key, eg `frc254`
            var ifNoneMatch = "ifNoneMatch_example";  // string? | Value of the `ETag` header in the most recently cached response by the client. (optional) 

            try
            {
                History result = apiInstance.GetTeamHistory(teamKey, ifNoneMatch);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling TeamApi.GetTeamHistory: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the GetTeamHistoryWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<History> response = apiInstance.GetTeamHistoryWithHttpInfo(teamKey, ifNoneMatch);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling TeamApi.GetTeamHistoryWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **teamKey** | **string** | TBA Team Key, eg &#x60;frc254&#x60; |  |
| **ifNoneMatch** | **string?** | Value of the &#x60;ETag&#x60; header in the most recently cached response by the client. | [optional]  |

### Return type

[**History**](History.md)

### Authorization

[apiKey](../README.md#apiKey)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | Successful response with team&#39;s history including events and awards. |  * Cache-Control - The &#x60;Cache-Control&#x60; header, in particular the &#x60;max-age&#x60; value, contains the number of seconds the result should be considered valid for. During this time subsequent calls should return from the local cache directly. <br>  * ETag - Specifies the version of the most recent response. Used by clients in the &#x60;If-None-Match&#x60; request header. <br>  |
| **304** | Not Modified - Use Local Cached Value |  -  |
| **401** | Authorization information is missing or invalid. |  -  |
| **404** | Not Found |  -  |

[[Back to top]](#) [[Back to API list]](../../README.md#documentation-for-api-endpoints) [[Back to Model list]](../../README.md#documentation-for-models) [[Back to README]](../../README.md)

<a id="getteammatchesbyyear"></a>
# **GetTeamMatchesByYear**
> Collection&lt;Match&gt; GetTeamMatchesByYear (string teamKey, int year, string? ifNoneMatch = null)



Gets a list of matches for the given team and year.

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using TheBlueAlliance.Api;
using TheBlueAlliance.Client;
using TheBlueAlliance.Model;

namespace Example


    public class GetTeamMatchesByYearExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "https://www.thebluealliance.com/api/v3";
            // Configure API key authorization: apiKey
            config.AddApiKey("X-TBA-Auth-Key", "YOUR_API_KEY");
            // Uncomment below to setup prefix (e.g. Bearer) for API key, if needed
            // config.AddApiKeyPrefix("X-TBA-Auth-Key", "Bearer");

            // create instances of HttpClient, HttpClientHandler to be reused later with different Api classes
            HttpClient httpClient = new HttpClient();
            HttpClientHandler httpClientHandler = new HttpClientHandler();
            var apiInstance = new TeamApi(httpClient, config, httpClientHandler);
            var teamKey = "teamKey_example";  // string | TBA Team Key, eg `frc254`
            var year = 56;  // int | Competition Year (or Season). Must be 4 digits.
            var ifNoneMatch = "ifNoneMatch_example";  // string? | Value of the `ETag` header in the most recently cached response by the client. (optional) 

            try
            {
                Collection<Match> result = apiInstance.GetTeamMatchesByYear(teamKey, year, ifNoneMatch);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling TeamApi.GetTeamMatchesByYear: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the GetTeamMatchesByYearWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<Collection<Match>> response = apiInstance.GetTeamMatchesByYearWithHttpInfo(teamKey, year, ifNoneMatch);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling TeamApi.GetTeamMatchesByYearWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **teamKey** | **string** | TBA Team Key, eg &#x60;frc254&#x60; |  |
| **year** | **int** | Competition Year (or Season). Must be 4 digits. |  |
| **ifNoneMatch** | **string?** | Value of the &#x60;ETag&#x60; header in the most recently cached response by the client. | [optional]  |

### Return type

[**Collection&lt;Match&gt;**](Match.md)

### Authorization

[apiKey](../README.md#apiKey)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | Successful response |  * Cache-Control - The &#x60;Cache-Control&#x60; header, in particular the &#x60;max-age&#x60; value, contains the number of seconds the result should be considered valid for. During this time subsequent calls should return from the local cache directly. <br>  * ETag - Specifies the version of the most recent response. Used by clients in the &#x60;If-None-Match&#x60; request header. <br>  |
| **304** | Not Modified - Use Local Cached Value |  -  |
| **401** | Authorization information is missing or invalid. |  -  |
| **404** | Not Found |  -  |

[[Back to top]](#) [[Back to API list]](../../README.md#documentation-for-api-endpoints) [[Back to Model list]](../../README.md#documentation-for-models) [[Back to README]](../../README.md)

<a id="getteammatchesbyyearkeys"></a>
# **GetTeamMatchesByYearKeys**
> Collection&lt;string&gt; GetTeamMatchesByYearKeys (string teamKey, int year, string? ifNoneMatch = null)



Gets a list of match keys for matches for the given team and year.

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using TheBlueAlliance.Api;
using TheBlueAlliance.Client;
using TheBlueAlliance.Model;

namespace Example


    public class GetTeamMatchesByYearKeysExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "https://www.thebluealliance.com/api/v3";
            // Configure API key authorization: apiKey
            config.AddApiKey("X-TBA-Auth-Key", "YOUR_API_KEY");
            // Uncomment below to setup prefix (e.g. Bearer) for API key, if needed
            // config.AddApiKeyPrefix("X-TBA-Auth-Key", "Bearer");

            // create instances of HttpClient, HttpClientHandler to be reused later with different Api classes
            HttpClient httpClient = new HttpClient();
            HttpClientHandler httpClientHandler = new HttpClientHandler();
            var apiInstance = new TeamApi(httpClient, config, httpClientHandler);
            var teamKey = "teamKey_example";  // string | TBA Team Key, eg `frc254`
            var year = 56;  // int | Competition Year (or Season). Must be 4 digits.
            var ifNoneMatch = "ifNoneMatch_example";  // string? | Value of the `ETag` header in the most recently cached response by the client. (optional) 

            try
            {
                Collection<string> result = apiInstance.GetTeamMatchesByYearKeys(teamKey, year, ifNoneMatch);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling TeamApi.GetTeamMatchesByYearKeys: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the GetTeamMatchesByYearKeysWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<Collection<string>> response = apiInstance.GetTeamMatchesByYearKeysWithHttpInfo(teamKey, year, ifNoneMatch);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling TeamApi.GetTeamMatchesByYearKeysWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **teamKey** | **string** | TBA Team Key, eg &#x60;frc254&#x60; |  |
| **year** | **int** | Competition Year (or Season). Must be 4 digits. |  |
| **ifNoneMatch** | **string?** | Value of the &#x60;ETag&#x60; header in the most recently cached response by the client. | [optional]  |

### Return type

**Collection<string>**

### Authorization

[apiKey](../README.md#apiKey)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | Successful response |  * Cache-Control - The &#x60;Cache-Control&#x60; header, in particular the &#x60;max-age&#x60; value, contains the number of seconds the result should be considered valid for. During this time subsequent calls should return from the local cache directly. <br>  * ETag - Specifies the version of the most recent response. Used by clients in the &#x60;If-None-Match&#x60; request header. <br>  |
| **304** | Not Modified - Use Local Cached Value |  -  |
| **401** | Authorization information is missing or invalid. |  -  |
| **404** | Not Found |  -  |

[[Back to top]](#) [[Back to API list]](../../README.md#documentation-for-api-endpoints) [[Back to Model list]](../../README.md#documentation-for-models) [[Back to README]](../../README.md)

<a id="getteammatchesbyyearsimple"></a>
# **GetTeamMatchesByYearSimple**
> Collection&lt;MatchSimple&gt; GetTeamMatchesByYearSimple (string teamKey, int year, string? ifNoneMatch = null)



Gets a short-form list of matches for the given team and year.

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using TheBlueAlliance.Api;
using TheBlueAlliance.Client;
using TheBlueAlliance.Model;

namespace Example


    public class GetTeamMatchesByYearSimpleExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "https://www.thebluealliance.com/api/v3";
            // Configure API key authorization: apiKey
            config.AddApiKey("X-TBA-Auth-Key", "YOUR_API_KEY");
            // Uncomment below to setup prefix (e.g. Bearer) for API key, if needed
            // config.AddApiKeyPrefix("X-TBA-Auth-Key", "Bearer");

            // create instances of HttpClient, HttpClientHandler to be reused later with different Api classes
            HttpClient httpClient = new HttpClient();
            HttpClientHandler httpClientHandler = new HttpClientHandler();
            var apiInstance = new TeamApi(httpClient, config, httpClientHandler);
            var teamKey = "teamKey_example";  // string | TBA Team Key, eg `frc254`
            var year = 56;  // int | Competition Year (or Season). Must be 4 digits.
            var ifNoneMatch = "ifNoneMatch_example";  // string? | Value of the `ETag` header in the most recently cached response by the client. (optional) 

            try
            {
                Collection<MatchSimple> result = apiInstance.GetTeamMatchesByYearSimple(teamKey, year, ifNoneMatch);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling TeamApi.GetTeamMatchesByYearSimple: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the GetTeamMatchesByYearSimpleWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<Collection<MatchSimple>> response = apiInstance.GetTeamMatchesByYearSimpleWithHttpInfo(teamKey, year, ifNoneMatch);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling TeamApi.GetTeamMatchesByYearSimpleWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **teamKey** | **string** | TBA Team Key, eg &#x60;frc254&#x60; |  |
| **year** | **int** | Competition Year (or Season). Must be 4 digits. |  |
| **ifNoneMatch** | **string?** | Value of the &#x60;ETag&#x60; header in the most recently cached response by the client. | [optional]  |

### Return type

[**Collection&lt;MatchSimple&gt;**](MatchSimple.md)

### Authorization

[apiKey](../README.md#apiKey)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | Successful response |  * Cache-Control - The &#x60;Cache-Control&#x60; header, in particular the &#x60;max-age&#x60; value, contains the number of seconds the result should be considered valid for. During this time subsequent calls should return from the local cache directly. <br>  * ETag - Specifies the version of the most recent response. Used by clients in the &#x60;If-None-Match&#x60; request header. <br>  |
| **304** | Not Modified - Use Local Cached Value |  -  |
| **401** | Authorization information is missing or invalid. |  -  |
| **404** | Not Found |  -  |

[[Back to top]](#) [[Back to API list]](../../README.md#documentation-for-api-endpoints) [[Back to Model list]](../../README.md#documentation-for-models) [[Back to README]](../../README.md)

<a id="getteammediabytag"></a>
# **GetTeamMediaByTag**
> Collection&lt;Media&gt; GetTeamMediaByTag (string mediaTag, string teamKey, string? ifNoneMatch = null)



Gets a list of Media (videos / pictures) for the given team and tag.

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using TheBlueAlliance.Api;
using TheBlueAlliance.Client;
using TheBlueAlliance.Model;

namespace Example


    public class GetTeamMediaByTagExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "https://www.thebluealliance.com/api/v3";
            // Configure API key authorization: apiKey
            config.AddApiKey("X-TBA-Auth-Key", "YOUR_API_KEY");
            // Uncomment below to setup prefix (e.g. Bearer) for API key, if needed
            // config.AddApiKeyPrefix("X-TBA-Auth-Key", "Bearer");

            // create instances of HttpClient, HttpClientHandler to be reused later with different Api classes
            HttpClient httpClient = new HttpClient();
            HttpClientHandler httpClientHandler = new HttpClientHandler();
            var apiInstance = new TeamApi(httpClient, config, httpClientHandler);
            var mediaTag = "mediaTag_example";  // string | Media Tag which describes the Media.
            var teamKey = "teamKey_example";  // string | TBA Team Key, eg `frc254`
            var ifNoneMatch = "ifNoneMatch_example";  // string? | Value of the `ETag` header in the most recently cached response by the client. (optional) 

            try
            {
                Collection<Media> result = apiInstance.GetTeamMediaByTag(mediaTag, teamKey, ifNoneMatch);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling TeamApi.GetTeamMediaByTag: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the GetTeamMediaByTagWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<Collection<Media>> response = apiInstance.GetTeamMediaByTagWithHttpInfo(mediaTag, teamKey, ifNoneMatch);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling TeamApi.GetTeamMediaByTagWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **mediaTag** | **string** | Media Tag which describes the Media. |  |
| **teamKey** | **string** | TBA Team Key, eg &#x60;frc254&#x60; |  |
| **ifNoneMatch** | **string?** | Value of the &#x60;ETag&#x60; header in the most recently cached response by the client. | [optional]  |

### Return type

[**Collection&lt;Media&gt;**](Media.md)

### Authorization

[apiKey](../README.md#apiKey)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | Successful response |  * Cache-Control - The &#x60;Cache-Control&#x60; header, in particular the &#x60;max-age&#x60; value, contains the number of seconds the result should be considered valid for. During this time subsequent calls should return from the local cache directly. <br>  * ETag - Specifies the version of the most recent response. Used by clients in the &#x60;If-None-Match&#x60; request header. <br>  |
| **304** | Not Modified - Use Local Cached Value |  -  |
| **401** | Authorization information is missing or invalid. |  -  |
| **404** | Not Found |  -  |

[[Back to top]](#) [[Back to API list]](../../README.md#documentation-for-api-endpoints) [[Back to Model list]](../../README.md#documentation-for-models) [[Back to README]](../../README.md)

<a id="getteammediabytagyear"></a>
# **GetTeamMediaByTagYear**
> Collection&lt;Media&gt; GetTeamMediaByTagYear (string mediaTag, string teamKey, int year, string? ifNoneMatch = null)



Gets a list of Media (videos / pictures) for the given team, tag and year.

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using TheBlueAlliance.Api;
using TheBlueAlliance.Client;
using TheBlueAlliance.Model;

namespace Example


    public class GetTeamMediaByTagYearExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "https://www.thebluealliance.com/api/v3";
            // Configure API key authorization: apiKey
            config.AddApiKey("X-TBA-Auth-Key", "YOUR_API_KEY");
            // Uncomment below to setup prefix (e.g. Bearer) for API key, if needed
            // config.AddApiKeyPrefix("X-TBA-Auth-Key", "Bearer");

            // create instances of HttpClient, HttpClientHandler to be reused later with different Api classes
            HttpClient httpClient = new HttpClient();
            HttpClientHandler httpClientHandler = new HttpClientHandler();
            var apiInstance = new TeamApi(httpClient, config, httpClientHandler);
            var mediaTag = "mediaTag_example";  // string | Media Tag which describes the Media.
            var teamKey = "teamKey_example";  // string | TBA Team Key, eg `frc254`
            var year = 56;  // int | Competition Year (or Season). Must be 4 digits.
            var ifNoneMatch = "ifNoneMatch_example";  // string? | Value of the `ETag` header in the most recently cached response by the client. (optional) 

            try
            {
                Collection<Media> result = apiInstance.GetTeamMediaByTagYear(mediaTag, teamKey, year, ifNoneMatch);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling TeamApi.GetTeamMediaByTagYear: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the GetTeamMediaByTagYearWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<Collection<Media>> response = apiInstance.GetTeamMediaByTagYearWithHttpInfo(mediaTag, teamKey, year, ifNoneMatch);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling TeamApi.GetTeamMediaByTagYearWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **mediaTag** | **string** | Media Tag which describes the Media. |  |
| **teamKey** | **string** | TBA Team Key, eg &#x60;frc254&#x60; |  |
| **year** | **int** | Competition Year (or Season). Must be 4 digits. |  |
| **ifNoneMatch** | **string?** | Value of the &#x60;ETag&#x60; header in the most recently cached response by the client. | [optional]  |

### Return type

[**Collection&lt;Media&gt;**](Media.md)

### Authorization

[apiKey](../README.md#apiKey)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | Successful response |  * Cache-Control - The &#x60;Cache-Control&#x60; header, in particular the &#x60;max-age&#x60; value, contains the number of seconds the result should be considered valid for. During this time subsequent calls should return from the local cache directly. <br>  * ETag - Specifies the version of the most recent response. Used by clients in the &#x60;If-None-Match&#x60; request header. <br>  |
| **304** | Not Modified - Use Local Cached Value |  -  |
| **401** | Authorization information is missing or invalid. |  -  |
| **404** | Not Found |  -  |

[[Back to top]](#) [[Back to API list]](../../README.md#documentation-for-api-endpoints) [[Back to Model list]](../../README.md#documentation-for-models) [[Back to README]](../../README.md)

<a id="getteammediabyyear"></a>
# **GetTeamMediaByYear**
> Collection&lt;Media&gt; GetTeamMediaByYear (string teamKey, int year, string? ifNoneMatch = null)



Gets a list of Media (videos / pictures) for the given team and year.

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using TheBlueAlliance.Api;
using TheBlueAlliance.Client;
using TheBlueAlliance.Model;

namespace Example


    public class GetTeamMediaByYearExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "https://www.thebluealliance.com/api/v3";
            // Configure API key authorization: apiKey
            config.AddApiKey("X-TBA-Auth-Key", "YOUR_API_KEY");
            // Uncomment below to setup prefix (e.g. Bearer) for API key, if needed
            // config.AddApiKeyPrefix("X-TBA-Auth-Key", "Bearer");

            // create instances of HttpClient, HttpClientHandler to be reused later with different Api classes
            HttpClient httpClient = new HttpClient();
            HttpClientHandler httpClientHandler = new HttpClientHandler();
            var apiInstance = new TeamApi(httpClient, config, httpClientHandler);
            var teamKey = "teamKey_example";  // string | TBA Team Key, eg `frc254`
            var year = 56;  // int | Competition Year (or Season). Must be 4 digits.
            var ifNoneMatch = "ifNoneMatch_example";  // string? | Value of the `ETag` header in the most recently cached response by the client. (optional) 

            try
            {
                Collection<Media> result = apiInstance.GetTeamMediaByYear(teamKey, year, ifNoneMatch);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling TeamApi.GetTeamMediaByYear: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the GetTeamMediaByYearWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<Collection<Media>> response = apiInstance.GetTeamMediaByYearWithHttpInfo(teamKey, year, ifNoneMatch);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling TeamApi.GetTeamMediaByYearWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **teamKey** | **string** | TBA Team Key, eg &#x60;frc254&#x60; |  |
| **year** | **int** | Competition Year (or Season). Must be 4 digits. |  |
| **ifNoneMatch** | **string?** | Value of the &#x60;ETag&#x60; header in the most recently cached response by the client. | [optional]  |

### Return type

[**Collection&lt;Media&gt;**](Media.md)

### Authorization

[apiKey](../README.md#apiKey)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | Successful response |  * Cache-Control - The &#x60;Cache-Control&#x60; header, in particular the &#x60;max-age&#x60; value, contains the number of seconds the result should be considered valid for. During this time subsequent calls should return from the local cache directly. <br>  * ETag - Specifies the version of the most recent response. Used by clients in the &#x60;If-None-Match&#x60; request header. <br>  |
| **304** | Not Modified - Use Local Cached Value |  -  |
| **401** | Authorization information is missing or invalid. |  -  |
| **404** | Not Found |  -  |

[[Back to top]](#) [[Back to API list]](../../README.md#documentation-for-api-endpoints) [[Back to Model list]](../../README.md#documentation-for-models) [[Back to README]](../../README.md)

<a id="getteamrobots"></a>
# **GetTeamRobots**
> Collection&lt;TeamRobot&gt; GetTeamRobots (string teamKey, string? ifNoneMatch = null)



Gets a list of year and robot name pairs for each year that a robot name was provided. Will return an empty array if the team has never named a robot.

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using TheBlueAlliance.Api;
using TheBlueAlliance.Client;
using TheBlueAlliance.Model;

namespace Example


    public class GetTeamRobotsExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "https://www.thebluealliance.com/api/v3";
            // Configure API key authorization: apiKey
            config.AddApiKey("X-TBA-Auth-Key", "YOUR_API_KEY");
            // Uncomment below to setup prefix (e.g. Bearer) for API key, if needed
            // config.AddApiKeyPrefix("X-TBA-Auth-Key", "Bearer");

            // create instances of HttpClient, HttpClientHandler to be reused later with different Api classes
            HttpClient httpClient = new HttpClient();
            HttpClientHandler httpClientHandler = new HttpClientHandler();
            var apiInstance = new TeamApi(httpClient, config, httpClientHandler);
            var teamKey = "teamKey_example";  // string | TBA Team Key, eg `frc254`
            var ifNoneMatch = "ifNoneMatch_example";  // string? | Value of the `ETag` header in the most recently cached response by the client. (optional) 

            try
            {
                Collection<TeamRobot> result = apiInstance.GetTeamRobots(teamKey, ifNoneMatch);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling TeamApi.GetTeamRobots: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the GetTeamRobotsWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<Collection<TeamRobot>> response = apiInstance.GetTeamRobotsWithHttpInfo(teamKey, ifNoneMatch);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling TeamApi.GetTeamRobotsWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **teamKey** | **string** | TBA Team Key, eg &#x60;frc254&#x60; |  |
| **ifNoneMatch** | **string?** | Value of the &#x60;ETag&#x60; header in the most recently cached response by the client. | [optional]  |

### Return type

[**Collection&lt;TeamRobot&gt;**](TeamRobot.md)

### Authorization

[apiKey](../README.md#apiKey)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | Successful response |  * Cache-Control - The &#x60;Cache-Control&#x60; header, in particular the &#x60;max-age&#x60; value, contains the number of seconds the result should be considered valid for. During this time subsequent calls should return from the local cache directly. <br>  * ETag - Specifies the version of the most recent response. Used by clients in the &#x60;If-None-Match&#x60; request header. <br>  |
| **304** | Not Modified - Use Local Cached Value |  -  |
| **401** | Authorization information is missing or invalid. |  -  |
| **404** | Not Found |  -  |

[[Back to top]](#) [[Back to API list]](../../README.md#documentation-for-api-endpoints) [[Back to Model list]](../../README.md#documentation-for-models) [[Back to README]](../../README.md)

<a id="getteamsimple"></a>
# **GetTeamSimple**
> TeamSimple GetTeamSimple (string teamKey, string? ifNoneMatch = null)



Gets a `Team_Simple` object for the team referenced by the given key.

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using TheBlueAlliance.Api;
using TheBlueAlliance.Client;
using TheBlueAlliance.Model;

namespace Example


    public class GetTeamSimpleExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "https://www.thebluealliance.com/api/v3";
            // Configure API key authorization: apiKey
            config.AddApiKey("X-TBA-Auth-Key", "YOUR_API_KEY");
            // Uncomment below to setup prefix (e.g. Bearer) for API key, if needed
            // config.AddApiKeyPrefix("X-TBA-Auth-Key", "Bearer");

            // create instances of HttpClient, HttpClientHandler to be reused later with different Api classes
            HttpClient httpClient = new HttpClient();
            HttpClientHandler httpClientHandler = new HttpClientHandler();
            var apiInstance = new TeamApi(httpClient, config, httpClientHandler);
            var teamKey = "teamKey_example";  // string | TBA Team Key, eg `frc254`
            var ifNoneMatch = "ifNoneMatch_example";  // string? | Value of the `ETag` header in the most recently cached response by the client. (optional) 

            try
            {
                TeamSimple result = apiInstance.GetTeamSimple(teamKey, ifNoneMatch);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling TeamApi.GetTeamSimple: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the GetTeamSimpleWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<TeamSimple> response = apiInstance.GetTeamSimpleWithHttpInfo(teamKey, ifNoneMatch);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling TeamApi.GetTeamSimpleWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **teamKey** | **string** | TBA Team Key, eg &#x60;frc254&#x60; |  |
| **ifNoneMatch** | **string?** | Value of the &#x60;ETag&#x60; header in the most recently cached response by the client. | [optional]  |

### Return type

[**TeamSimple**](TeamSimple.md)

### Authorization

[apiKey](../README.md#apiKey)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | Successful response |  * Cache-Control - The &#x60;Cache-Control&#x60; header, in particular the &#x60;max-age&#x60; value, contains the number of seconds the result should be considered valid for. During this time subsequent calls should return from the local cache directly. <br>  * ETag - Specifies the version of the most recent response. Used by clients in the &#x60;If-None-Match&#x60; request header. <br>  |
| **304** | Not Modified - Use Local Cached Value |  -  |
| **401** | Authorization information is missing or invalid. |  -  |
| **404** | Not Found |  -  |

[[Back to top]](#) [[Back to API list]](../../README.md#documentation-for-api-endpoints) [[Back to Model list]](../../README.md#documentation-for-models) [[Back to README]](../../README.md)

<a id="getteamsocialmedia"></a>
# **GetTeamSocialMedia**
> Collection&lt;Media&gt; GetTeamSocialMedia (string teamKey, string? ifNoneMatch = null)



Gets a list of Media (social media) for the given team.

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using TheBlueAlliance.Api;
using TheBlueAlliance.Client;
using TheBlueAlliance.Model;

namespace Example


    public class GetTeamSocialMediaExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "https://www.thebluealliance.com/api/v3";
            // Configure API key authorization: apiKey
            config.AddApiKey("X-TBA-Auth-Key", "YOUR_API_KEY");
            // Uncomment below to setup prefix (e.g. Bearer) for API key, if needed
            // config.AddApiKeyPrefix("X-TBA-Auth-Key", "Bearer");

            // create instances of HttpClient, HttpClientHandler to be reused later with different Api classes
            HttpClient httpClient = new HttpClient();
            HttpClientHandler httpClientHandler = new HttpClientHandler();
            var apiInstance = new TeamApi(httpClient, config, httpClientHandler);
            var teamKey = "teamKey_example";  // string | TBA Team Key, eg `frc254`
            var ifNoneMatch = "ifNoneMatch_example";  // string? | Value of the `ETag` header in the most recently cached response by the client. (optional) 

            try
            {
                Collection<Media> result = apiInstance.GetTeamSocialMedia(teamKey, ifNoneMatch);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling TeamApi.GetTeamSocialMedia: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the GetTeamSocialMediaWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<Collection<Media>> response = apiInstance.GetTeamSocialMediaWithHttpInfo(teamKey, ifNoneMatch);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling TeamApi.GetTeamSocialMediaWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **teamKey** | **string** | TBA Team Key, eg &#x60;frc254&#x60; |  |
| **ifNoneMatch** | **string?** | Value of the &#x60;ETag&#x60; header in the most recently cached response by the client. | [optional]  |

### Return type

[**Collection&lt;Media&gt;**](Media.md)

### Authorization

[apiKey](../README.md#apiKey)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | Successful response |  * Cache-Control - The &#x60;Cache-Control&#x60; header, in particular the &#x60;max-age&#x60; value, contains the number of seconds the result should be considered valid for. During this time subsequent calls should return from the local cache directly. <br>  * ETag - Specifies the version of the most recent response. Used by clients in the &#x60;If-None-Match&#x60; request header. <br>  |
| **304** | Not Modified - Use Local Cached Value |  -  |
| **401** | Authorization information is missing or invalid. |  -  |
| **404** | Not Found |  -  |

[[Back to top]](#) [[Back to API list]](../../README.md#documentation-for-api-endpoints) [[Back to Model list]](../../README.md#documentation-for-models) [[Back to README]](../../README.md)

<a id="getteamyearsparticipated"></a>
# **GetTeamYearsParticipated**
> Collection&lt;int&gt; GetTeamYearsParticipated (string teamKey, string? ifNoneMatch = null)



Gets a list of years in which the team participated in at least one competition.

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using TheBlueAlliance.Api;
using TheBlueAlliance.Client;
using TheBlueAlliance.Model;

namespace Example


    public class GetTeamYearsParticipatedExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "https://www.thebluealliance.com/api/v3";
            // Configure API key authorization: apiKey
            config.AddApiKey("X-TBA-Auth-Key", "YOUR_API_KEY");
            // Uncomment below to setup prefix (e.g. Bearer) for API key, if needed
            // config.AddApiKeyPrefix("X-TBA-Auth-Key", "Bearer");

            // create instances of HttpClient, HttpClientHandler to be reused later with different Api classes
            HttpClient httpClient = new HttpClient();
            HttpClientHandler httpClientHandler = new HttpClientHandler();
            var apiInstance = new TeamApi(httpClient, config, httpClientHandler);
            var teamKey = "teamKey_example";  // string | TBA Team Key, eg `frc254`
            var ifNoneMatch = "ifNoneMatch_example";  // string? | Value of the `ETag` header in the most recently cached response by the client. (optional) 

            try
            {
                Collection<int> result = apiInstance.GetTeamYearsParticipated(teamKey, ifNoneMatch);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling TeamApi.GetTeamYearsParticipated: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the GetTeamYearsParticipatedWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<Collection<int>> response = apiInstance.GetTeamYearsParticipatedWithHttpInfo(teamKey, ifNoneMatch);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling TeamApi.GetTeamYearsParticipatedWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **teamKey** | **string** | TBA Team Key, eg &#x60;frc254&#x60; |  |
| **ifNoneMatch** | **string?** | Value of the &#x60;ETag&#x60; header in the most recently cached response by the client. | [optional]  |

### Return type

**Collection<int>**

### Authorization

[apiKey](../README.md#apiKey)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | Successful response |  * Cache-Control - The &#x60;Cache-Control&#x60; header, in particular the &#x60;max-age&#x60; value, contains the number of seconds the result should be considered valid for. During this time subsequent calls should return from the local cache directly. <br>  * ETag - Specifies the version of the most recent response. Used by clients in the &#x60;If-None-Match&#x60; request header. <br>  |
| **304** | Not Modified - Use Local Cached Value |  -  |
| **401** | Authorization information is missing or invalid. |  -  |
| **404** | Not Found |  -  |

[[Back to top]](#) [[Back to API list]](../../README.md#documentation-for-api-endpoints) [[Back to Model list]](../../README.md#documentation-for-models) [[Back to README]](../../README.md)

<a id="getteams"></a>
# **GetTeams**
> Collection&lt;Team&gt; GetTeams (int pageNum, string? ifNoneMatch = null)



Gets a list of `Team` objects, paginated in groups of 500.

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using TheBlueAlliance.Api;
using TheBlueAlliance.Client;
using TheBlueAlliance.Model;

namespace Example


    public class GetTeamsExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "https://www.thebluealliance.com/api/v3";
            // Configure API key authorization: apiKey
            config.AddApiKey("X-TBA-Auth-Key", "YOUR_API_KEY");
            // Uncomment below to setup prefix (e.g. Bearer) for API key, if needed
            // config.AddApiKeyPrefix("X-TBA-Auth-Key", "Bearer");

            // create instances of HttpClient, HttpClientHandler to be reused later with different Api classes
            HttpClient httpClient = new HttpClient();
            HttpClientHandler httpClientHandler = new HttpClientHandler();
            var apiInstance = new TeamApi(httpClient, config, httpClientHandler);
            var pageNum = 56;  // int | Page number of results to return, zero-indexed
            var ifNoneMatch = "ifNoneMatch_example";  // string? | Value of the `ETag` header in the most recently cached response by the client. (optional) 

            try
            {
                Collection<Team> result = apiInstance.GetTeams(pageNum, ifNoneMatch);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling TeamApi.GetTeams: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the GetTeamsWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<Collection<Team>> response = apiInstance.GetTeamsWithHttpInfo(pageNum, ifNoneMatch);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling TeamApi.GetTeamsWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **pageNum** | **int** | Page number of results to return, zero-indexed |  |
| **ifNoneMatch** | **string?** | Value of the &#x60;ETag&#x60; header in the most recently cached response by the client. | [optional]  |

### Return type

[**Collection&lt;Team&gt;**](Team.md)

### Authorization

[apiKey](../README.md#apiKey)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | Successful response |  * Cache-Control - The &#x60;Cache-Control&#x60; header, in particular the &#x60;max-age&#x60; value, contains the number of seconds the result should be considered valid for. During this time subsequent calls should return from the local cache directly. <br>  * ETag - Specifies the version of the most recent response. Used by clients in the &#x60;If-None-Match&#x60; request header. <br>  |
| **304** | Not Modified - Use Local Cached Value |  -  |
| **401** | Authorization information is missing or invalid. |  -  |
| **404** | Not Found |  -  |

[[Back to top]](#) [[Back to API list]](../../README.md#documentation-for-api-endpoints) [[Back to Model list]](../../README.md#documentation-for-models) [[Back to README]](../../README.md)

<a id="getteamsbyyear"></a>
# **GetTeamsByYear**
> Collection&lt;Team&gt; GetTeamsByYear (int pageNum, int year, string? ifNoneMatch = null)



Gets a list of `Team` objects that competed in the given year, paginated in groups of 500.

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using TheBlueAlliance.Api;
using TheBlueAlliance.Client;
using TheBlueAlliance.Model;

namespace Example


    public class GetTeamsByYearExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "https://www.thebluealliance.com/api/v3";
            // Configure API key authorization: apiKey
            config.AddApiKey("X-TBA-Auth-Key", "YOUR_API_KEY");
            // Uncomment below to setup prefix (e.g. Bearer) for API key, if needed
            // config.AddApiKeyPrefix("X-TBA-Auth-Key", "Bearer");

            // create instances of HttpClient, HttpClientHandler to be reused later with different Api classes
            HttpClient httpClient = new HttpClient();
            HttpClientHandler httpClientHandler = new HttpClientHandler();
            var apiInstance = new TeamApi(httpClient, config, httpClientHandler);
            var pageNum = 56;  // int | Page number of results to return, zero-indexed
            var year = 56;  // int | Competition Year (or Season). Must be 4 digits.
            var ifNoneMatch = "ifNoneMatch_example";  // string? | Value of the `ETag` header in the most recently cached response by the client. (optional) 

            try
            {
                Collection<Team> result = apiInstance.GetTeamsByYear(pageNum, year, ifNoneMatch);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling TeamApi.GetTeamsByYear: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the GetTeamsByYearWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<Collection<Team>> response = apiInstance.GetTeamsByYearWithHttpInfo(pageNum, year, ifNoneMatch);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling TeamApi.GetTeamsByYearWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **pageNum** | **int** | Page number of results to return, zero-indexed |  |
| **year** | **int** | Competition Year (or Season). Must be 4 digits. |  |
| **ifNoneMatch** | **string?** | Value of the &#x60;ETag&#x60; header in the most recently cached response by the client. | [optional]  |

### Return type

[**Collection&lt;Team&gt;**](Team.md)

### Authorization

[apiKey](../README.md#apiKey)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | Successful response |  * Cache-Control - The &#x60;Cache-Control&#x60; header, in particular the &#x60;max-age&#x60; value, contains the number of seconds the result should be considered valid for. During this time subsequent calls should return from the local cache directly. <br>  * ETag - Specifies the version of the most recent response. Used by clients in the &#x60;If-None-Match&#x60; request header. <br>  |
| **304** | Not Modified - Use Local Cached Value |  -  |
| **401** | Authorization information is missing or invalid. |  -  |
| **404** | Not Found |  -  |

[[Back to top]](#) [[Back to API list]](../../README.md#documentation-for-api-endpoints) [[Back to Model list]](../../README.md#documentation-for-models) [[Back to README]](../../README.md)

<a id="getteamsbyyearkeys"></a>
# **GetTeamsByYearKeys**
> Collection&lt;string&gt; GetTeamsByYearKeys (int pageNum, int year, string? ifNoneMatch = null)



Gets a list Team Keys that competed in the given year, paginated in groups of 500.

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using TheBlueAlliance.Api;
using TheBlueAlliance.Client;
using TheBlueAlliance.Model;

namespace Example


    public class GetTeamsByYearKeysExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "https://www.thebluealliance.com/api/v3";
            // Configure API key authorization: apiKey
            config.AddApiKey("X-TBA-Auth-Key", "YOUR_API_KEY");
            // Uncomment below to setup prefix (e.g. Bearer) for API key, if needed
            // config.AddApiKeyPrefix("X-TBA-Auth-Key", "Bearer");

            // create instances of HttpClient, HttpClientHandler to be reused later with different Api classes
            HttpClient httpClient = new HttpClient();
            HttpClientHandler httpClientHandler = new HttpClientHandler();
            var apiInstance = new TeamApi(httpClient, config, httpClientHandler);
            var pageNum = 56;  // int | Page number of results to return, zero-indexed
            var year = 56;  // int | Competition Year (or Season). Must be 4 digits.
            var ifNoneMatch = "ifNoneMatch_example";  // string? | Value of the `ETag` header in the most recently cached response by the client. (optional) 

            try
            {
                Collection<string> result = apiInstance.GetTeamsByYearKeys(pageNum, year, ifNoneMatch);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling TeamApi.GetTeamsByYearKeys: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the GetTeamsByYearKeysWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<Collection<string>> response = apiInstance.GetTeamsByYearKeysWithHttpInfo(pageNum, year, ifNoneMatch);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling TeamApi.GetTeamsByYearKeysWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **pageNum** | **int** | Page number of results to return, zero-indexed |  |
| **year** | **int** | Competition Year (or Season). Must be 4 digits. |  |
| **ifNoneMatch** | **string?** | Value of the &#x60;ETag&#x60; header in the most recently cached response by the client. | [optional]  |

### Return type

**Collection<string>**

### Authorization

[apiKey](../README.md#apiKey)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | Successful response |  * Cache-Control - The &#x60;Cache-Control&#x60; header, in particular the &#x60;max-age&#x60; value, contains the number of seconds the result should be considered valid for. During this time subsequent calls should return from the local cache directly. <br>  * ETag - Specifies the version of the most recent response. Used by clients in the &#x60;If-None-Match&#x60; request header. <br>  |
| **304** | Not Modified - Use Local Cached Value |  -  |
| **401** | Authorization information is missing or invalid. |  -  |
| **404** | Not Found |  -  |

[[Back to top]](#) [[Back to API list]](../../README.md#documentation-for-api-endpoints) [[Back to Model list]](../../README.md#documentation-for-models) [[Back to README]](../../README.md)

<a id="getteamsbyyearsimple"></a>
# **GetTeamsByYearSimple**
> Collection&lt;TeamSimple&gt; GetTeamsByYearSimple (int pageNum, int year, string? ifNoneMatch = null)



Gets a list of short form `Team_Simple` objects that competed in the given year, paginated in groups of 500.

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using TheBlueAlliance.Api;
using TheBlueAlliance.Client;
using TheBlueAlliance.Model;

namespace Example


    public class GetTeamsByYearSimpleExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "https://www.thebluealliance.com/api/v3";
            // Configure API key authorization: apiKey
            config.AddApiKey("X-TBA-Auth-Key", "YOUR_API_KEY");
            // Uncomment below to setup prefix (e.g. Bearer) for API key, if needed
            // config.AddApiKeyPrefix("X-TBA-Auth-Key", "Bearer");

            // create instances of HttpClient, HttpClientHandler to be reused later with different Api classes
            HttpClient httpClient = new HttpClient();
            HttpClientHandler httpClientHandler = new HttpClientHandler();
            var apiInstance = new TeamApi(httpClient, config, httpClientHandler);
            var pageNum = 56;  // int | Page number of results to return, zero-indexed
            var year = 56;  // int | Competition Year (or Season). Must be 4 digits.
            var ifNoneMatch = "ifNoneMatch_example";  // string? | Value of the `ETag` header in the most recently cached response by the client. (optional) 

            try
            {
                Collection<TeamSimple> result = apiInstance.GetTeamsByYearSimple(pageNum, year, ifNoneMatch);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling TeamApi.GetTeamsByYearSimple: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the GetTeamsByYearSimpleWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<Collection<TeamSimple>> response = apiInstance.GetTeamsByYearSimpleWithHttpInfo(pageNum, year, ifNoneMatch);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling TeamApi.GetTeamsByYearSimpleWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **pageNum** | **int** | Page number of results to return, zero-indexed |  |
| **year** | **int** | Competition Year (or Season). Must be 4 digits. |  |
| **ifNoneMatch** | **string?** | Value of the &#x60;ETag&#x60; header in the most recently cached response by the client. | [optional]  |

### Return type

[**Collection&lt;TeamSimple&gt;**](TeamSimple.md)

### Authorization

[apiKey](../README.md#apiKey)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | Successful response |  * Cache-Control - The &#x60;Cache-Control&#x60; header, in particular the &#x60;max-age&#x60; value, contains the number of seconds the result should be considered valid for. During this time subsequent calls should return from the local cache directly. <br>  * ETag - Specifies the version of the most recent response. Used by clients in the &#x60;If-None-Match&#x60; request header. <br>  |
| **304** | Not Modified - Use Local Cached Value |  -  |
| **401** | Authorization information is missing or invalid. |  -  |
| **404** | Not Found |  -  |

[[Back to top]](#) [[Back to API list]](../../README.md#documentation-for-api-endpoints) [[Back to Model list]](../../README.md#documentation-for-models) [[Back to README]](../../README.md)

<a id="getteamskeys"></a>
# **GetTeamsKeys**
> Collection&lt;string&gt; GetTeamsKeys (int pageNum, string? ifNoneMatch = null)



Gets a list of Team keys, paginated in groups of 500. (Note, each page will not have 500 teams, but will include the teams within that range of 500.)

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using TheBlueAlliance.Api;
using TheBlueAlliance.Client;
using TheBlueAlliance.Model;

namespace Example


    public class GetTeamsKeysExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "https://www.thebluealliance.com/api/v3";
            // Configure API key authorization: apiKey
            config.AddApiKey("X-TBA-Auth-Key", "YOUR_API_KEY");
            // Uncomment below to setup prefix (e.g. Bearer) for API key, if needed
            // config.AddApiKeyPrefix("X-TBA-Auth-Key", "Bearer");

            // create instances of HttpClient, HttpClientHandler to be reused later with different Api classes
            HttpClient httpClient = new HttpClient();
            HttpClientHandler httpClientHandler = new HttpClientHandler();
            var apiInstance = new TeamApi(httpClient, config, httpClientHandler);
            var pageNum = 56;  // int | Page number of results to return, zero-indexed
            var ifNoneMatch = "ifNoneMatch_example";  // string? | Value of the `ETag` header in the most recently cached response by the client. (optional) 

            try
            {
                Collection<string> result = apiInstance.GetTeamsKeys(pageNum, ifNoneMatch);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling TeamApi.GetTeamsKeys: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the GetTeamsKeysWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<Collection<string>> response = apiInstance.GetTeamsKeysWithHttpInfo(pageNum, ifNoneMatch);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling TeamApi.GetTeamsKeysWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **pageNum** | **int** | Page number of results to return, zero-indexed |  |
| **ifNoneMatch** | **string?** | Value of the &#x60;ETag&#x60; header in the most recently cached response by the client. | [optional]  |

### Return type

**Collection<string>**

### Authorization

[apiKey](../README.md#apiKey)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | Successful response |  * Cache-Control - The &#x60;Cache-Control&#x60; header, in particular the &#x60;max-age&#x60; value, contains the number of seconds the result should be considered valid for. During this time subsequent calls should return from the local cache directly. <br>  * ETag - Specifies the version of the most recent response. Used by clients in the &#x60;If-None-Match&#x60; request header. <br>  |
| **304** | Not Modified - Use Local Cached Value |  -  |
| **401** | Authorization information is missing or invalid. |  -  |
| **404** | Not Found |  -  |

[[Back to top]](#) [[Back to API list]](../../README.md#documentation-for-api-endpoints) [[Back to Model list]](../../README.md#documentation-for-models) [[Back to README]](../../README.md)

<a id="getteamssimple"></a>
# **GetTeamsSimple**
> Collection&lt;TeamSimple&gt; GetTeamsSimple (int pageNum, string? ifNoneMatch = null)



Gets a list of short form `Team_Simple` objects, paginated in groups of 500.

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using TheBlueAlliance.Api;
using TheBlueAlliance.Client;
using TheBlueAlliance.Model;

namespace Example


    public class GetTeamsSimpleExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "https://www.thebluealliance.com/api/v3";
            // Configure API key authorization: apiKey
            config.AddApiKey("X-TBA-Auth-Key", "YOUR_API_KEY");
            // Uncomment below to setup prefix (e.g. Bearer) for API key, if needed
            // config.AddApiKeyPrefix("X-TBA-Auth-Key", "Bearer");

            // create instances of HttpClient, HttpClientHandler to be reused later with different Api classes
            HttpClient httpClient = new HttpClient();
            HttpClientHandler httpClientHandler = new HttpClientHandler();
            var apiInstance = new TeamApi(httpClient, config, httpClientHandler);
            var pageNum = 56;  // int | Page number of results to return, zero-indexed
            var ifNoneMatch = "ifNoneMatch_example";  // string? | Value of the `ETag` header in the most recently cached response by the client. (optional) 

            try
            {
                Collection<TeamSimple> result = apiInstance.GetTeamsSimple(pageNum, ifNoneMatch);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling TeamApi.GetTeamsSimple: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the GetTeamsSimpleWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<Collection<TeamSimple>> response = apiInstance.GetTeamsSimpleWithHttpInfo(pageNum, ifNoneMatch);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling TeamApi.GetTeamsSimpleWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **pageNum** | **int** | Page number of results to return, zero-indexed |  |
| **ifNoneMatch** | **string?** | Value of the &#x60;ETag&#x60; header in the most recently cached response by the client. | [optional]  |

### Return type

[**Collection&lt;TeamSimple&gt;**](TeamSimple.md)

### Authorization

[apiKey](../README.md#apiKey)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | Successful response |  * Cache-Control - The &#x60;Cache-Control&#x60; header, in particular the &#x60;max-age&#x60; value, contains the number of seconds the result should be considered valid for. During this time subsequent calls should return from the local cache directly. <br>  * ETag - Specifies the version of the most recent response. Used by clients in the &#x60;If-None-Match&#x60; request header. <br>  |
| **304** | Not Modified - Use Local Cached Value |  -  |
| **401** | Authorization information is missing or invalid. |  -  |
| **404** | Not Found |  -  |

[[Back to top]](#) [[Back to API list]](../../README.md#documentation-for-api-endpoints) [[Back to Model list]](../../README.md#documentation-for-models) [[Back to README]](../../README.md)

