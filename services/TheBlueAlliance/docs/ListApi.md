# TheBlueAlliance.Api.ListApi

All URIs are relative to *https://www.thebluealliance.com/api/v3*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**GetDistrictAwards**](ListApi.md#getdistrictawards) | **GET** /district/{district_key}/awards |  |
| [**GetDistrictEvents**](ListApi.md#getdistrictevents) | **GET** /district/{district_key}/events |  |
| [**GetDistrictEventsKeys**](ListApi.md#getdistricteventskeys) | **GET** /district/{district_key}/events/keys |  |
| [**GetDistrictEventsSimple**](ListApi.md#getdistricteventssimple) | **GET** /district/{district_key}/events/simple |  |
| [**GetDistrictRankings**](ListApi.md#getdistrictrankings) | **GET** /district/{district_key}/rankings |  |
| [**GetDistrictTeams**](ListApi.md#getdistrictteams) | **GET** /district/{district_key}/teams |  |
| [**GetDistrictTeamsKeys**](ListApi.md#getdistrictteamskeys) | **GET** /district/{district_key}/teams/keys |  |
| [**GetDistrictTeamsSimple**](ListApi.md#getdistrictteamssimple) | **GET** /district/{district_key}/teams/simple |  |
| [**GetEventTeams**](ListApi.md#geteventteams) | **GET** /event/{event_key}/teams |  |
| [**GetEventTeamsKeys**](ListApi.md#geteventteamskeys) | **GET** /event/{event_key}/teams/keys |  |
| [**GetEventTeamsSimple**](ListApi.md#geteventteamssimple) | **GET** /event/{event_key}/teams/simple |  |
| [**GetEventTeamsStatuses**](ListApi.md#geteventteamsstatuses) | **GET** /event/{event_key}/teams/statuses |  |
| [**GetEventsByYear**](ListApi.md#geteventsbyyear) | **GET** /events/{year} |  |
| [**GetEventsByYearKeys**](ListApi.md#geteventsbyyearkeys) | **GET** /events/{year}/keys |  |
| [**GetEventsByYearSimple**](ListApi.md#geteventsbyyearsimple) | **GET** /events/{year}/simple |  |
| [**GetInsightsLeaderboardsYear**](ListApi.md#getinsightsleaderboardsyear) | **GET** /insights/leaderboards/{year} |  |
| [**GetInsightsNotablesYear**](ListApi.md#getinsightsnotablesyear) | **GET** /insights/notables/{year} |  |
| [**GetTeamEventsStatusesByYear**](ListApi.md#getteameventsstatusesbyyear) | **GET** /team/{team_key}/events/{year}/statuses |  |
| [**GetTeams**](ListApi.md#getteams) | **GET** /teams/{page_num} |  |
| [**GetTeamsByYear**](ListApi.md#getteamsbyyear) | **GET** /teams/{year}/{page_num} |  |
| [**GetTeamsByYearKeys**](ListApi.md#getteamsbyyearkeys) | **GET** /teams/{year}/{page_num}/keys |  |
| [**GetTeamsByYearSimple**](ListApi.md#getteamsbyyearsimple) | **GET** /teams/{year}/{page_num}/simple |  |
| [**GetTeamsKeys**](ListApi.md#getteamskeys) | **GET** /teams/{page_num}/keys |  |
| [**GetTeamsSimple**](ListApi.md#getteamssimple) | **GET** /teams/{page_num}/simple |  |

<a id="getdistrictawards"></a>
# **GetDistrictAwards**
> Collection&lt;Award&gt; GetDistrictAwards (string districtKey, string? ifNoneMatch = null)



Gets a list of awards in the given district.

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using TheBlueAlliance.Api;
using TheBlueAlliance.Client;
using TheBlueAlliance.Model;

namespace Example


    public class GetDistrictAwardsExample
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
            var apiInstance = new ListApi(httpClient, config, httpClientHandler);
            var districtKey = "districtKey_example";  // string | TBA District Key, eg `2016fim`
            var ifNoneMatch = "ifNoneMatch_example";  // string? | Value of the `ETag` header in the most recently cached response by the client. (optional) 

            try
            {
                Collection<Award> result = apiInstance.GetDistrictAwards(districtKey, ifNoneMatch);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling ListApi.GetDistrictAwards: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the GetDistrictAwardsWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<Collection<Award>> response = apiInstance.GetDistrictAwardsWithHttpInfo(districtKey, ifNoneMatch);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling ListApi.GetDistrictAwardsWithHttpInfo: " + e.Message);
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

<a id="getdistrictevents"></a>
# **GetDistrictEvents**
> Collection&lt;Event&gt; GetDistrictEvents (string districtKey, string? ifNoneMatch = null)



Gets a list of events in the given district.

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using TheBlueAlliance.Api;
using TheBlueAlliance.Client;
using TheBlueAlliance.Model;

namespace Example


    public class GetDistrictEventsExample
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
            var apiInstance = new ListApi(httpClient, config, httpClientHandler);
            var districtKey = "districtKey_example";  // string | TBA District Key, eg `2016fim`
            var ifNoneMatch = "ifNoneMatch_example";  // string? | Value of the `ETag` header in the most recently cached response by the client. (optional) 

            try
            {
                Collection<Event> result = apiInstance.GetDistrictEvents(districtKey, ifNoneMatch);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling ListApi.GetDistrictEvents: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the GetDistrictEventsWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<Collection<Event>> response = apiInstance.GetDistrictEventsWithHttpInfo(districtKey, ifNoneMatch);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling ListApi.GetDistrictEventsWithHttpInfo: " + e.Message);
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

<a id="getdistricteventskeys"></a>
# **GetDistrictEventsKeys**
> Collection&lt;string&gt; GetDistrictEventsKeys (string districtKey, string? ifNoneMatch = null)



Gets a list of event keys for events in the given district.

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using TheBlueAlliance.Api;
using TheBlueAlliance.Client;
using TheBlueAlliance.Model;

namespace Example


    public class GetDistrictEventsKeysExample
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
            var apiInstance = new ListApi(httpClient, config, httpClientHandler);
            var districtKey = "districtKey_example";  // string | TBA District Key, eg `2016fim`
            var ifNoneMatch = "ifNoneMatch_example";  // string? | Value of the `ETag` header in the most recently cached response by the client. (optional) 

            try
            {
                Collection<string> result = apiInstance.GetDistrictEventsKeys(districtKey, ifNoneMatch);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling ListApi.GetDistrictEventsKeys: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the GetDistrictEventsKeysWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<Collection<string>> response = apiInstance.GetDistrictEventsKeysWithHttpInfo(districtKey, ifNoneMatch);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling ListApi.GetDistrictEventsKeysWithHttpInfo: " + e.Message);
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

<a id="getdistricteventssimple"></a>
# **GetDistrictEventsSimple**
> Collection&lt;EventSimple&gt; GetDistrictEventsSimple (string districtKey, string? ifNoneMatch = null)



Gets a short-form list of events in the given district.

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using TheBlueAlliance.Api;
using TheBlueAlliance.Client;
using TheBlueAlliance.Model;

namespace Example


    public class GetDistrictEventsSimpleExample
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
            var apiInstance = new ListApi(httpClient, config, httpClientHandler);
            var districtKey = "districtKey_example";  // string | TBA District Key, eg `2016fim`
            var ifNoneMatch = "ifNoneMatch_example";  // string? | Value of the `ETag` header in the most recently cached response by the client. (optional) 

            try
            {
                Collection<EventSimple> result = apiInstance.GetDistrictEventsSimple(districtKey, ifNoneMatch);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling ListApi.GetDistrictEventsSimple: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the GetDistrictEventsSimpleWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<Collection<EventSimple>> response = apiInstance.GetDistrictEventsSimpleWithHttpInfo(districtKey, ifNoneMatch);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling ListApi.GetDistrictEventsSimpleWithHttpInfo: " + e.Message);
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
            var apiInstance = new ListApi(httpClient, config, httpClientHandler);
            var districtKey = "districtKey_example";  // string | TBA District Key, eg `2016fim`
            var ifNoneMatch = "ifNoneMatch_example";  // string? | Value of the `ETag` header in the most recently cached response by the client. (optional) 

            try
            {
                Collection<DistrictRanking> result = apiInstance.GetDistrictRankings(districtKey, ifNoneMatch);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling ListApi.GetDistrictRankings: " + e.Message);
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
    Debug.Print("Exception when calling ListApi.GetDistrictRankingsWithHttpInfo: " + e.Message);
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
            var apiInstance = new ListApi(httpClient, config, httpClientHandler);
            var districtKey = "districtKey_example";  // string | TBA District Key, eg `2016fim`
            var ifNoneMatch = "ifNoneMatch_example";  // string? | Value of the `ETag` header in the most recently cached response by the client. (optional) 

            try
            {
                Collection<Team> result = apiInstance.GetDistrictTeams(districtKey, ifNoneMatch);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling ListApi.GetDistrictTeams: " + e.Message);
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
    Debug.Print("Exception when calling ListApi.GetDistrictTeamsWithHttpInfo: " + e.Message);
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
            var apiInstance = new ListApi(httpClient, config, httpClientHandler);
            var districtKey = "districtKey_example";  // string | TBA District Key, eg `2016fim`
            var ifNoneMatch = "ifNoneMatch_example";  // string? | Value of the `ETag` header in the most recently cached response by the client. (optional) 

            try
            {
                Collection<string> result = apiInstance.GetDistrictTeamsKeys(districtKey, ifNoneMatch);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling ListApi.GetDistrictTeamsKeys: " + e.Message);
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
    Debug.Print("Exception when calling ListApi.GetDistrictTeamsKeysWithHttpInfo: " + e.Message);
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
            var apiInstance = new ListApi(httpClient, config, httpClientHandler);
            var districtKey = "districtKey_example";  // string | TBA District Key, eg `2016fim`
            var ifNoneMatch = "ifNoneMatch_example";  // string? | Value of the `ETag` header in the most recently cached response by the client. (optional) 

            try
            {
                Collection<TeamSimple> result = apiInstance.GetDistrictTeamsSimple(districtKey, ifNoneMatch);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling ListApi.GetDistrictTeamsSimple: " + e.Message);
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
    Debug.Print("Exception when calling ListApi.GetDistrictTeamsSimpleWithHttpInfo: " + e.Message);
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
            var apiInstance = new ListApi(httpClient, config, httpClientHandler);
            var eventKey = "eventKey_example";  // string | TBA Event Key, eg `2016nytr`
            var ifNoneMatch = "ifNoneMatch_example";  // string? | Value of the `ETag` header in the most recently cached response by the client. (optional) 

            try
            {
                Collection<Team> result = apiInstance.GetEventTeams(eventKey, ifNoneMatch);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling ListApi.GetEventTeams: " + e.Message);
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
    Debug.Print("Exception when calling ListApi.GetEventTeamsWithHttpInfo: " + e.Message);
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
            var apiInstance = new ListApi(httpClient, config, httpClientHandler);
            var eventKey = "eventKey_example";  // string | TBA Event Key, eg `2016nytr`
            var ifNoneMatch = "ifNoneMatch_example";  // string? | Value of the `ETag` header in the most recently cached response by the client. (optional) 

            try
            {
                Collection<string> result = apiInstance.GetEventTeamsKeys(eventKey, ifNoneMatch);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling ListApi.GetEventTeamsKeys: " + e.Message);
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
    Debug.Print("Exception when calling ListApi.GetEventTeamsKeysWithHttpInfo: " + e.Message);
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
            var apiInstance = new ListApi(httpClient, config, httpClientHandler);
            var eventKey = "eventKey_example";  // string | TBA Event Key, eg `2016nytr`
            var ifNoneMatch = "ifNoneMatch_example";  // string? | Value of the `ETag` header in the most recently cached response by the client. (optional) 

            try
            {
                Collection<TeamSimple> result = apiInstance.GetEventTeamsSimple(eventKey, ifNoneMatch);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling ListApi.GetEventTeamsSimple: " + e.Message);
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
    Debug.Print("Exception when calling ListApi.GetEventTeamsSimpleWithHttpInfo: " + e.Message);
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
            var apiInstance = new ListApi(httpClient, config, httpClientHandler);
            var eventKey = "eventKey_example";  // string | TBA Event Key, eg `2016nytr`
            var ifNoneMatch = "ifNoneMatch_example";  // string? | Value of the `ETag` header in the most recently cached response by the client. (optional) 

            try
            {
                Dictionary<string, GetTeamEventsStatusesByYear200ResponseValue> result = apiInstance.GetEventTeamsStatuses(eventKey, ifNoneMatch);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling ListApi.GetEventTeamsStatuses: " + e.Message);
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
    Debug.Print("Exception when calling ListApi.GetEventTeamsStatusesWithHttpInfo: " + e.Message);
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

<a id="geteventsbyyear"></a>
# **GetEventsByYear**
> Collection&lt;Event&gt; GetEventsByYear (int year, string? ifNoneMatch = null)



Gets a list of events in the given year.

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using TheBlueAlliance.Api;
using TheBlueAlliance.Client;
using TheBlueAlliance.Model;

namespace Example


    public class GetEventsByYearExample
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
            var apiInstance = new ListApi(httpClient, config, httpClientHandler);
            var year = 56;  // int | Competition Year (or Season). Must be 4 digits.
            var ifNoneMatch = "ifNoneMatch_example";  // string? | Value of the `ETag` header in the most recently cached response by the client. (optional) 

            try
            {
                Collection<Event> result = apiInstance.GetEventsByYear(year, ifNoneMatch);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling ListApi.GetEventsByYear: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the GetEventsByYearWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<Collection<Event>> response = apiInstance.GetEventsByYearWithHttpInfo(year, ifNoneMatch);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling ListApi.GetEventsByYearWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
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

<a id="geteventsbyyearkeys"></a>
# **GetEventsByYearKeys**
> Collection&lt;string&gt; GetEventsByYearKeys (int year, string? ifNoneMatch = null)



Gets a list of event keys in the given year.

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using TheBlueAlliance.Api;
using TheBlueAlliance.Client;
using TheBlueAlliance.Model;

namespace Example


    public class GetEventsByYearKeysExample
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
            var apiInstance = new ListApi(httpClient, config, httpClientHandler);
            var year = 56;  // int | Competition Year (or Season). Must be 4 digits.
            var ifNoneMatch = "ifNoneMatch_example";  // string? | Value of the `ETag` header in the most recently cached response by the client. (optional) 

            try
            {
                Collection<string> result = apiInstance.GetEventsByYearKeys(year, ifNoneMatch);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling ListApi.GetEventsByYearKeys: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the GetEventsByYearKeysWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<Collection<string>> response = apiInstance.GetEventsByYearKeysWithHttpInfo(year, ifNoneMatch);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling ListApi.GetEventsByYearKeysWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
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

<a id="geteventsbyyearsimple"></a>
# **GetEventsByYearSimple**
> Collection&lt;EventSimple&gt; GetEventsByYearSimple (int year, string? ifNoneMatch = null)



Gets a short-form list of events in the given year.

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using TheBlueAlliance.Api;
using TheBlueAlliance.Client;
using TheBlueAlliance.Model;

namespace Example


    public class GetEventsByYearSimpleExample
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
            var apiInstance = new ListApi(httpClient, config, httpClientHandler);
            var year = 56;  // int | Competition Year (or Season). Must be 4 digits.
            var ifNoneMatch = "ifNoneMatch_example";  // string? | Value of the `ETag` header in the most recently cached response by the client. (optional) 

            try
            {
                Collection<EventSimple> result = apiInstance.GetEventsByYearSimple(year, ifNoneMatch);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling ListApi.GetEventsByYearSimple: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the GetEventsByYearSimpleWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<Collection<EventSimple>> response = apiInstance.GetEventsByYearSimpleWithHttpInfo(year, ifNoneMatch);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling ListApi.GetEventsByYearSimpleWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
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

<a id="getinsightsleaderboardsyear"></a>
# **GetInsightsLeaderboardsYear**
> Collection&lt;LeaderboardInsight&gt; GetInsightsLeaderboardsYear (int year, string? ifNoneMatch = null)



Gets a list of `LeaderboardInsight` objects from a specific year. Use year=0 for overall.

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using TheBlueAlliance.Api;
using TheBlueAlliance.Client;
using TheBlueAlliance.Model;

namespace Example


    public class GetInsightsLeaderboardsYearExample
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
            var apiInstance = new ListApi(httpClient, config, httpClientHandler);
            var year = 56;  // int | Competition Year (or Season). Must be 4 digits.
            var ifNoneMatch = "ifNoneMatch_example";  // string? | Value of the `ETag` header in the most recently cached response by the client. (optional) 

            try
            {
                Collection<LeaderboardInsight> result = apiInstance.GetInsightsLeaderboardsYear(year, ifNoneMatch);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling ListApi.GetInsightsLeaderboardsYear: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the GetInsightsLeaderboardsYearWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<Collection<LeaderboardInsight>> response = apiInstance.GetInsightsLeaderboardsYearWithHttpInfo(year, ifNoneMatch);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling ListApi.GetInsightsLeaderboardsYearWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **year** | **int** | Competition Year (or Season). Must be 4 digits. |  |
| **ifNoneMatch** | **string?** | Value of the &#x60;ETag&#x60; header in the most recently cached response by the client. | [optional]  |

### Return type

[**Collection&lt;LeaderboardInsight&gt;**](LeaderboardInsight.md)

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

<a id="getinsightsnotablesyear"></a>
# **GetInsightsNotablesYear**
> Collection&lt;NotablesInsight&gt; GetInsightsNotablesYear (int year, string? ifNoneMatch = null)



Gets a list of `NotablesInsight` objects from a specific year. Use year=0 for overall.

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using TheBlueAlliance.Api;
using TheBlueAlliance.Client;
using TheBlueAlliance.Model;

namespace Example


    public class GetInsightsNotablesYearExample
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
            var apiInstance = new ListApi(httpClient, config, httpClientHandler);
            var year = 56;  // int | Competition Year (or Season). Must be 4 digits.
            var ifNoneMatch = "ifNoneMatch_example";  // string? | Value of the `ETag` header in the most recently cached response by the client. (optional) 

            try
            {
                Collection<NotablesInsight> result = apiInstance.GetInsightsNotablesYear(year, ifNoneMatch);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling ListApi.GetInsightsNotablesYear: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the GetInsightsNotablesYearWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<Collection<NotablesInsight>> response = apiInstance.GetInsightsNotablesYearWithHttpInfo(year, ifNoneMatch);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling ListApi.GetInsightsNotablesYearWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **year** | **int** | Competition Year (or Season). Must be 4 digits. |  |
| **ifNoneMatch** | **string?** | Value of the &#x60;ETag&#x60; header in the most recently cached response by the client. | [optional]  |

### Return type

[**Collection&lt;NotablesInsight&gt;**](NotablesInsight.md)

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
            var apiInstance = new ListApi(httpClient, config, httpClientHandler);
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
                Debug.Print("Exception when calling ListApi.GetTeamEventsStatusesByYear: " + e.Message);
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
    Debug.Print("Exception when calling ListApi.GetTeamEventsStatusesByYearWithHttpInfo: " + e.Message);
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
            var apiInstance = new ListApi(httpClient, config, httpClientHandler);
            var pageNum = 56;  // int | Page number of results to return, zero-indexed
            var ifNoneMatch = "ifNoneMatch_example";  // string? | Value of the `ETag` header in the most recently cached response by the client. (optional) 

            try
            {
                Collection<Team> result = apiInstance.GetTeams(pageNum, ifNoneMatch);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling ListApi.GetTeams: " + e.Message);
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
    Debug.Print("Exception when calling ListApi.GetTeamsWithHttpInfo: " + e.Message);
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
            var apiInstance = new ListApi(httpClient, config, httpClientHandler);
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
                Debug.Print("Exception when calling ListApi.GetTeamsByYear: " + e.Message);
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
    Debug.Print("Exception when calling ListApi.GetTeamsByYearWithHttpInfo: " + e.Message);
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
            var apiInstance = new ListApi(httpClient, config, httpClientHandler);
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
                Debug.Print("Exception when calling ListApi.GetTeamsByYearKeys: " + e.Message);
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
    Debug.Print("Exception when calling ListApi.GetTeamsByYearKeysWithHttpInfo: " + e.Message);
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
            var apiInstance = new ListApi(httpClient, config, httpClientHandler);
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
                Debug.Print("Exception when calling ListApi.GetTeamsByYearSimple: " + e.Message);
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
    Debug.Print("Exception when calling ListApi.GetTeamsByYearSimpleWithHttpInfo: " + e.Message);
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
            var apiInstance = new ListApi(httpClient, config, httpClientHandler);
            var pageNum = 56;  // int | Page number of results to return, zero-indexed
            var ifNoneMatch = "ifNoneMatch_example";  // string? | Value of the `ETag` header in the most recently cached response by the client. (optional) 

            try
            {
                Collection<string> result = apiInstance.GetTeamsKeys(pageNum, ifNoneMatch);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling ListApi.GetTeamsKeys: " + e.Message);
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
    Debug.Print("Exception when calling ListApi.GetTeamsKeysWithHttpInfo: " + e.Message);
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
            var apiInstance = new ListApi(httpClient, config, httpClientHandler);
            var pageNum = 56;  // int | Page number of results to return, zero-indexed
            var ifNoneMatch = "ifNoneMatch_example";  // string? | Value of the `ETag` header in the most recently cached response by the client. (optional) 

            try
            {
                Collection<TeamSimple> result = apiInstance.GetTeamsSimple(pageNum, ifNoneMatch);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling ListApi.GetTeamsSimple: " + e.Message);
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
    Debug.Print("Exception when calling ListApi.GetTeamsSimpleWithHttpInfo: " + e.Message);
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

