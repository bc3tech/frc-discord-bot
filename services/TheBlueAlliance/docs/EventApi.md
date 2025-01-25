# TheBlueAlliance.Api.EventApi

All URIs are relative to *https://www.thebluealliance.com/api/v3*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**GetDistrictAwards**](EventApi.md#getdistrictawards) | **GET** /district/{district_key}/awards |  |
| [**GetDistrictEvents**](EventApi.md#getdistrictevents) | **GET** /district/{district_key}/events |  |
| [**GetDistrictEventsKeys**](EventApi.md#getdistricteventskeys) | **GET** /district/{district_key}/events/keys |  |
| [**GetDistrictEventsSimple**](EventApi.md#getdistricteventssimple) | **GET** /district/{district_key}/events/simple |  |
| [**GetEvent**](EventApi.md#getevent) | **GET** /event/{event_key} |  |
| [**GetEventAlliances**](EventApi.md#geteventalliances) | **GET** /event/{event_key}/alliances |  |
| [**GetEventAwards**](EventApi.md#geteventawards) | **GET** /event/{event_key}/awards |  |
| [**GetEventCOPRs**](EventApi.md#geteventcoprs) | **GET** /event/{event_key}/coprs |  |
| [**GetEventDistrictPoints**](EventApi.md#geteventdistrictpoints) | **GET** /event/{event_key}/district_points |  |
| [**GetEventInsights**](EventApi.md#geteventinsights) | **GET** /event/{event_key}/insights |  |
| [**GetEventMatchTimeseries**](EventApi.md#geteventmatchtimeseries) | **GET** /event/{event_key}/matches/timeseries |  |
| [**GetEventMatches**](EventApi.md#geteventmatches) | **GET** /event/{event_key}/matches |  |
| [**GetEventMatchesKeys**](EventApi.md#geteventmatcheskeys) | **GET** /event/{event_key}/matches/keys |  |
| [**GetEventMatchesSimple**](EventApi.md#geteventmatchessimple) | **GET** /event/{event_key}/matches/simple |  |
| [**GetEventOPRs**](EventApi.md#geteventoprs) | **GET** /event/{event_key}/oprs |  |
| [**GetEventPredictions**](EventApi.md#geteventpredictions) | **GET** /event/{event_key}/predictions |  |
| [**GetEventRankings**](EventApi.md#geteventrankings) | **GET** /event/{event_key}/rankings |  |
| [**GetEventSimple**](EventApi.md#geteventsimple) | **GET** /event/{event_key}/simple |  |
| [**GetEventTeamMedia**](EventApi.md#geteventteammedia) | **GET** /event/{event_key}/team_media |  |
| [**GetEventTeams**](EventApi.md#geteventteams) | **GET** /event/{event_key}/teams |  |
| [**GetEventTeamsKeys**](EventApi.md#geteventteamskeys) | **GET** /event/{event_key}/teams/keys |  |
| [**GetEventTeamsSimple**](EventApi.md#geteventteamssimple) | **GET** /event/{event_key}/teams/simple |  |
| [**GetEventTeamsStatuses**](EventApi.md#geteventteamsstatuses) | **GET** /event/{event_key}/teams/statuses |  |
| [**GetEventsByYear**](EventApi.md#geteventsbyyear) | **GET** /events/{year} |  |
| [**GetEventsByYearKeys**](EventApi.md#geteventsbyyearkeys) | **GET** /events/{year}/keys |  |
| [**GetEventsByYearSimple**](EventApi.md#geteventsbyyearsimple) | **GET** /events/{year}/simple |  |
| [**GetTeamEventAwards**](EventApi.md#getteameventawards) | **GET** /team/{team_key}/event/{event_key}/awards |  |
| [**GetTeamEventMatches**](EventApi.md#getteameventmatches) | **GET** /team/{team_key}/event/{event_key}/matches |  |
| [**GetTeamEventMatchesKeys**](EventApi.md#getteameventmatcheskeys) | **GET** /team/{team_key}/event/{event_key}/matches/keys |  |
| [**GetTeamEventMatchesSimple**](EventApi.md#getteameventmatchessimple) | **GET** /team/{team_key}/event/{event_key}/matches/simple |  |
| [**GetTeamEventStatus**](EventApi.md#getteameventstatus) | **GET** /team/{team_key}/event/{event_key}/status |  |
| [**GetTeamEvents**](EventApi.md#getteamevents) | **GET** /team/{team_key}/events |  |
| [**GetTeamEventsByYear**](EventApi.md#getteameventsbyyear) | **GET** /team/{team_key}/events/{year} |  |
| [**GetTeamEventsByYearKeys**](EventApi.md#getteameventsbyyearkeys) | **GET** /team/{team_key}/events/{year}/keys |  |
| [**GetTeamEventsByYearSimple**](EventApi.md#getteameventsbyyearsimple) | **GET** /team/{team_key}/events/{year}/simple |  |
| [**GetTeamEventsKeys**](EventApi.md#getteameventskeys) | **GET** /team/{team_key}/events/keys |  |
| [**GetTeamEventsSimple**](EventApi.md#getteameventssimple) | **GET** /team/{team_key}/events/simple |  |
| [**GetTeamEventsStatusesByYear**](EventApi.md#getteameventsstatusesbyyear) | **GET** /team/{team_key}/events/{year}/statuses |  |

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
            var apiInstance = new EventApi(httpClient, config, httpClientHandler);
            var districtKey = "districtKey_example";  // string | TBA District Key, eg `2016fim`
            var ifNoneMatch = "ifNoneMatch_example";  // string? | Value of the `ETag` header in the most recently cached response by the client. (optional) 

            try
            {
                Collection<Award> result = apiInstance.GetDistrictAwards(districtKey, ifNoneMatch);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling EventApi.GetDistrictAwards: " + e.Message);
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
    Debug.Print("Exception when calling EventApi.GetDistrictAwardsWithHttpInfo: " + e.Message);
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
            var apiInstance = new EventApi(httpClient, config, httpClientHandler);
            var districtKey = "districtKey_example";  // string | TBA District Key, eg `2016fim`
            var ifNoneMatch = "ifNoneMatch_example";  // string? | Value of the `ETag` header in the most recently cached response by the client. (optional) 

            try
            {
                Collection<Event> result = apiInstance.GetDistrictEvents(districtKey, ifNoneMatch);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling EventApi.GetDistrictEvents: " + e.Message);
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
    Debug.Print("Exception when calling EventApi.GetDistrictEventsWithHttpInfo: " + e.Message);
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
            var apiInstance = new EventApi(httpClient, config, httpClientHandler);
            var districtKey = "districtKey_example";  // string | TBA District Key, eg `2016fim`
            var ifNoneMatch = "ifNoneMatch_example";  // string? | Value of the `ETag` header in the most recently cached response by the client. (optional) 

            try
            {
                Collection<string> result = apiInstance.GetDistrictEventsKeys(districtKey, ifNoneMatch);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling EventApi.GetDistrictEventsKeys: " + e.Message);
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
    Debug.Print("Exception when calling EventApi.GetDistrictEventsKeysWithHttpInfo: " + e.Message);
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
            var apiInstance = new EventApi(httpClient, config, httpClientHandler);
            var districtKey = "districtKey_example";  // string | TBA District Key, eg `2016fim`
            var ifNoneMatch = "ifNoneMatch_example";  // string? | Value of the `ETag` header in the most recently cached response by the client. (optional) 

            try
            {
                Collection<EventSimple> result = apiInstance.GetDistrictEventsSimple(districtKey, ifNoneMatch);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling EventApi.GetDistrictEventsSimple: " + e.Message);
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
    Debug.Print("Exception when calling EventApi.GetDistrictEventsSimpleWithHttpInfo: " + e.Message);
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

<a id="getevent"></a>
# **GetEvent**
> Event GetEvent (string eventKey, string? ifNoneMatch = null)



Gets an Event.

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using TheBlueAlliance.Api;
using TheBlueAlliance.Client;
using TheBlueAlliance.Model;

namespace Example


    public class GetEventExample
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
            var apiInstance = new EventApi(httpClient, config, httpClientHandler);
            var eventKey = "eventKey_example";  // string | TBA Event Key, eg `2016nytr`
            var ifNoneMatch = "ifNoneMatch_example";  // string? | Value of the `ETag` header in the most recently cached response by the client. (optional) 

            try
            {
                Event result = apiInstance.GetEvent(eventKey, ifNoneMatch);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling EventApi.GetEvent: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the GetEventWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<Event> response = apiInstance.GetEventWithHttpInfo(eventKey, ifNoneMatch);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling EventApi.GetEventWithHttpInfo: " + e.Message);
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

[**Event**](Event.md)

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

<a id="geteventalliances"></a>
# **GetEventAlliances**
> Collection&lt;EliminationAlliance&gt; GetEventAlliances (string eventKey, string? ifNoneMatch = null)



Gets a list of Elimination Alliances for the given Event.

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using TheBlueAlliance.Api;
using TheBlueAlliance.Client;
using TheBlueAlliance.Model;

namespace Example


    public class GetEventAlliancesExample
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
            var apiInstance = new EventApi(httpClient, config, httpClientHandler);
            var eventKey = "eventKey_example";  // string | TBA Event Key, eg `2016nytr`
            var ifNoneMatch = "ifNoneMatch_example";  // string? | Value of the `ETag` header in the most recently cached response by the client. (optional) 

            try
            {
                Collection<EliminationAlliance> result = apiInstance.GetEventAlliances(eventKey, ifNoneMatch);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling EventApi.GetEventAlliances: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the GetEventAlliancesWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<Collection<EliminationAlliance>> response = apiInstance.GetEventAlliancesWithHttpInfo(eventKey, ifNoneMatch);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling EventApi.GetEventAlliancesWithHttpInfo: " + e.Message);
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

[**Collection&lt;EliminationAlliance&gt;**](EliminationAlliance.md)

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

<a id="geteventawards"></a>
# **GetEventAwards**
> Collection&lt;Award&gt; GetEventAwards (string eventKey, string? ifNoneMatch = null)



Gets a list of awards from the given event.

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using TheBlueAlliance.Api;
using TheBlueAlliance.Client;
using TheBlueAlliance.Model;

namespace Example


    public class GetEventAwardsExample
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
            var apiInstance = new EventApi(httpClient, config, httpClientHandler);
            var eventKey = "eventKey_example";  // string | TBA Event Key, eg `2016nytr`
            var ifNoneMatch = "ifNoneMatch_example";  // string? | Value of the `ETag` header in the most recently cached response by the client. (optional) 

            try
            {
                Collection<Award> result = apiInstance.GetEventAwards(eventKey, ifNoneMatch);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling EventApi.GetEventAwards: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the GetEventAwardsWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<Collection<Award>> response = apiInstance.GetEventAwardsWithHttpInfo(eventKey, ifNoneMatch);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling EventApi.GetEventAwardsWithHttpInfo: " + e.Message);
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

<a id="geteventcoprs"></a>
# **GetEventCOPRs**
> Dictionary&lt;string, Dictionary&lt;string, decimal&gt;&gt; GetEventCOPRs (string eventKey, string? ifNoneMatch = null)



Gets a set of Event Component OPRs for the given Event.

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using TheBlueAlliance.Api;
using TheBlueAlliance.Client;
using TheBlueAlliance.Model;

namespace Example


    public class GetEventCOPRsExample
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
            var apiInstance = new EventApi(httpClient, config, httpClientHandler);
            var eventKey = "eventKey_example";  // string | TBA Event Key, eg `2016nytr`
            var ifNoneMatch = "ifNoneMatch_example";  // string? | Value of the `ETag` header in the most recently cached response by the client. (optional) 

            try
            {
                Dictionary<string, Dictionary<string, decimal>> result = apiInstance.GetEventCOPRs(eventKey, ifNoneMatch);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling EventApi.GetEventCOPRs: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the GetEventCOPRsWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<Dictionary<string, Dictionary<string, decimal>>> response = apiInstance.GetEventCOPRsWithHttpInfo(eventKey, ifNoneMatch);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling EventApi.GetEventCOPRsWithHttpInfo: " + e.Message);
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

**Dictionary<string, Dictionary<string, decimal>>**

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

<a id="geteventdistrictpoints"></a>
# **GetEventDistrictPoints**
> EventDistrictPoints GetEventDistrictPoints (string eventKey, string? ifNoneMatch = null)



Gets a list of team rankings for the Event.

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using TheBlueAlliance.Api;
using TheBlueAlliance.Client;
using TheBlueAlliance.Model;

namespace Example


    public class GetEventDistrictPointsExample
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
            var apiInstance = new EventApi(httpClient, config, httpClientHandler);
            var eventKey = "eventKey_example";  // string | TBA Event Key, eg `2016nytr`
            var ifNoneMatch = "ifNoneMatch_example";  // string? | Value of the `ETag` header in the most recently cached response by the client. (optional) 

            try
            {
                EventDistrictPoints result = apiInstance.GetEventDistrictPoints(eventKey, ifNoneMatch);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling EventApi.GetEventDistrictPoints: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the GetEventDistrictPointsWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<EventDistrictPoints> response = apiInstance.GetEventDistrictPointsWithHttpInfo(eventKey, ifNoneMatch);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling EventApi.GetEventDistrictPointsWithHttpInfo: " + e.Message);
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

[**EventDistrictPoints**](EventDistrictPoints.md)

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

<a id="geteventinsights"></a>
# **GetEventInsights**
> EventInsights GetEventInsights (string eventKey, string? ifNoneMatch = null)



Gets a set of Event-specific insights for the given Event.

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using TheBlueAlliance.Api;
using TheBlueAlliance.Client;
using TheBlueAlliance.Model;

namespace Example


    public class GetEventInsightsExample
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
            var apiInstance = new EventApi(httpClient, config, httpClientHandler);
            var eventKey = "eventKey_example";  // string | TBA Event Key, eg `2016nytr`
            var ifNoneMatch = "ifNoneMatch_example";  // string? | Value of the `ETag` header in the most recently cached response by the client. (optional) 

            try
            {
                EventInsights result = apiInstance.GetEventInsights(eventKey, ifNoneMatch);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling EventApi.GetEventInsights: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the GetEventInsightsWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<EventInsights> response = apiInstance.GetEventInsightsWithHttpInfo(eventKey, ifNoneMatch);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling EventApi.GetEventInsightsWithHttpInfo: " + e.Message);
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

[**EventInsights**](EventInsights.md)

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

<a id="geteventmatchtimeseries"></a>
# **GetEventMatchTimeseries**
> Collection&lt;string&gt; GetEventMatchTimeseries (string eventKey, string? ifNoneMatch = null)



Gets an array of Match Keys for the given event key that have timeseries data. Returns an empty array if no matches have timeseries data. *WARNING:* This is *not* official data, and is subject to a significant possibility of error, or missing data. Do not rely on this data for any purpose. In fact, pretend we made it up. *WARNING:* This endpoint and corresponding data models are under *active development* and may change at any time, including in breaking ways.

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using TheBlueAlliance.Api;
using TheBlueAlliance.Client;
using TheBlueAlliance.Model;

namespace Example


    public class GetEventMatchTimeseriesExample
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
            var apiInstance = new EventApi(httpClient, config, httpClientHandler);
            var eventKey = "eventKey_example";  // string | TBA Event Key, eg `2016nytr`
            var ifNoneMatch = "ifNoneMatch_example";  // string? | Value of the `ETag` header in the most recently cached response by the client. (optional) 

            try
            {
                Collection<string> result = apiInstance.GetEventMatchTimeseries(eventKey, ifNoneMatch);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling EventApi.GetEventMatchTimeseries: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the GetEventMatchTimeseriesWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<Collection<string>> response = apiInstance.GetEventMatchTimeseriesWithHttpInfo(eventKey, ifNoneMatch);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling EventApi.GetEventMatchTimeseriesWithHttpInfo: " + e.Message);
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

<a id="geteventmatches"></a>
# **GetEventMatches**
> Collection&lt;Match&gt; GetEventMatches (string eventKey, string? ifNoneMatch = null)



Gets a list of matches for the given event.

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using TheBlueAlliance.Api;
using TheBlueAlliance.Client;
using TheBlueAlliance.Model;

namespace Example


    public class GetEventMatchesExample
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
            var apiInstance = new EventApi(httpClient, config, httpClientHandler);
            var eventKey = "eventKey_example";  // string | TBA Event Key, eg `2016nytr`
            var ifNoneMatch = "ifNoneMatch_example";  // string? | Value of the `ETag` header in the most recently cached response by the client. (optional) 

            try
            {
                Collection<Match> result = apiInstance.GetEventMatches(eventKey, ifNoneMatch);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling EventApi.GetEventMatches: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the GetEventMatchesWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<Collection<Match>> response = apiInstance.GetEventMatchesWithHttpInfo(eventKey, ifNoneMatch);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling EventApi.GetEventMatchesWithHttpInfo: " + e.Message);
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

<a id="geteventmatcheskeys"></a>
# **GetEventMatchesKeys**
> Collection&lt;string&gt; GetEventMatchesKeys (string eventKey, string? ifNoneMatch = null)



Gets a list of match keys for the given event.

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using TheBlueAlliance.Api;
using TheBlueAlliance.Client;
using TheBlueAlliance.Model;

namespace Example


    public class GetEventMatchesKeysExample
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
            var apiInstance = new EventApi(httpClient, config, httpClientHandler);
            var eventKey = "eventKey_example";  // string | TBA Event Key, eg `2016nytr`
            var ifNoneMatch = "ifNoneMatch_example";  // string? | Value of the `ETag` header in the most recently cached response by the client. (optional) 

            try
            {
                Collection<string> result = apiInstance.GetEventMatchesKeys(eventKey, ifNoneMatch);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling EventApi.GetEventMatchesKeys: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the GetEventMatchesKeysWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<Collection<string>> response = apiInstance.GetEventMatchesKeysWithHttpInfo(eventKey, ifNoneMatch);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling EventApi.GetEventMatchesKeysWithHttpInfo: " + e.Message);
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

<a id="geteventmatchessimple"></a>
# **GetEventMatchesSimple**
> Collection&lt;MatchSimple&gt; GetEventMatchesSimple (string eventKey, string? ifNoneMatch = null)



Gets a short-form list of matches for the given event.

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using TheBlueAlliance.Api;
using TheBlueAlliance.Client;
using TheBlueAlliance.Model;

namespace Example


    public class GetEventMatchesSimpleExample
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
            var apiInstance = new EventApi(httpClient, config, httpClientHandler);
            var eventKey = "eventKey_example";  // string | TBA Event Key, eg `2016nytr`
            var ifNoneMatch = "ifNoneMatch_example";  // string? | Value of the `ETag` header in the most recently cached response by the client. (optional) 

            try
            {
                Collection<MatchSimple> result = apiInstance.GetEventMatchesSimple(eventKey, ifNoneMatch);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling EventApi.GetEventMatchesSimple: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the GetEventMatchesSimpleWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<Collection<MatchSimple>> response = apiInstance.GetEventMatchesSimpleWithHttpInfo(eventKey, ifNoneMatch);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling EventApi.GetEventMatchesSimpleWithHttpInfo: " + e.Message);
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

<a id="geteventoprs"></a>
# **GetEventOPRs**
> EventOPRs GetEventOPRs (string eventKey, string? ifNoneMatch = null)



Gets a set of Event OPRs (including OPR, DPR, and CCWM) for the given Event.

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using TheBlueAlliance.Api;
using TheBlueAlliance.Client;
using TheBlueAlliance.Model;

namespace Example


    public class GetEventOPRsExample
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
            var apiInstance = new EventApi(httpClient, config, httpClientHandler);
            var eventKey = "eventKey_example";  // string | TBA Event Key, eg `2016nytr`
            var ifNoneMatch = "ifNoneMatch_example";  // string? | Value of the `ETag` header in the most recently cached response by the client. (optional) 

            try
            {
                EventOPRs result = apiInstance.GetEventOPRs(eventKey, ifNoneMatch);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling EventApi.GetEventOPRs: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the GetEventOPRsWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<EventOPRs> response = apiInstance.GetEventOPRsWithHttpInfo(eventKey, ifNoneMatch);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling EventApi.GetEventOPRsWithHttpInfo: " + e.Message);
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

[**EventOPRs**](EventOPRs.md)

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

<a id="geteventpredictions"></a>
# **GetEventPredictions**
> Object GetEventPredictions (string eventKey, string? ifNoneMatch = null)



Gets information on TBA-generated predictions for the given Event. Contains year-specific information. *WARNING* This endpoint is currently under development and may change at any time.

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using TheBlueAlliance.Api;
using TheBlueAlliance.Client;
using TheBlueAlliance.Model;

namespace Example


    public class GetEventPredictionsExample
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
            var apiInstance = new EventApi(httpClient, config, httpClientHandler);
            var eventKey = "eventKey_example";  // string | TBA Event Key, eg `2016nytr`
            var ifNoneMatch = "ifNoneMatch_example";  // string? | Value of the `ETag` header in the most recently cached response by the client. (optional) 

            try
            {
                Object result = apiInstance.GetEventPredictions(eventKey, ifNoneMatch);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling EventApi.GetEventPredictions: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the GetEventPredictionsWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<Object> response = apiInstance.GetEventPredictionsWithHttpInfo(eventKey, ifNoneMatch);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling EventApi.GetEventPredictionsWithHttpInfo: " + e.Message);
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

**Object**

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

<a id="geteventrankings"></a>
# **GetEventRankings**
> EventRanking GetEventRankings (string eventKey, string? ifNoneMatch = null)



Gets a list of team rankings for the Event.

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using TheBlueAlliance.Api;
using TheBlueAlliance.Client;
using TheBlueAlliance.Model;

namespace Example


    public class GetEventRankingsExample
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
            var apiInstance = new EventApi(httpClient, config, httpClientHandler);
            var eventKey = "eventKey_example";  // string | TBA Event Key, eg `2016nytr`
            var ifNoneMatch = "ifNoneMatch_example";  // string? | Value of the `ETag` header in the most recently cached response by the client. (optional) 

            try
            {
                EventRanking result = apiInstance.GetEventRankings(eventKey, ifNoneMatch);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling EventApi.GetEventRankings: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the GetEventRankingsWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<EventRanking> response = apiInstance.GetEventRankingsWithHttpInfo(eventKey, ifNoneMatch);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling EventApi.GetEventRankingsWithHttpInfo: " + e.Message);
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

[**EventRanking**](EventRanking.md)

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

<a id="geteventsimple"></a>
# **GetEventSimple**
> EventSimple GetEventSimple (string eventKey, string? ifNoneMatch = null)



Gets a short-form Event.

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using TheBlueAlliance.Api;
using TheBlueAlliance.Client;
using TheBlueAlliance.Model;

namespace Example


    public class GetEventSimpleExample
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
            var apiInstance = new EventApi(httpClient, config, httpClientHandler);
            var eventKey = "eventKey_example";  // string | TBA Event Key, eg `2016nytr`
            var ifNoneMatch = "ifNoneMatch_example";  // string? | Value of the `ETag` header in the most recently cached response by the client. (optional) 

            try
            {
                EventSimple result = apiInstance.GetEventSimple(eventKey, ifNoneMatch);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling EventApi.GetEventSimple: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the GetEventSimpleWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<EventSimple> response = apiInstance.GetEventSimpleWithHttpInfo(eventKey, ifNoneMatch);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling EventApi.GetEventSimpleWithHttpInfo: " + e.Message);
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

[**EventSimple**](EventSimple.md)

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

<a id="geteventteammedia"></a>
# **GetEventTeamMedia**
> Collection&lt;Media&gt; GetEventTeamMedia (string eventKey, string? ifNoneMatch = null)



Gets a list of media objects that correspond to teams at this event.

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using TheBlueAlliance.Api;
using TheBlueAlliance.Client;
using TheBlueAlliance.Model;

namespace Example


    public class GetEventTeamMediaExample
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
            var apiInstance = new EventApi(httpClient, config, httpClientHandler);
            var eventKey = "eventKey_example";  // string | TBA Event Key, eg `2016nytr`
            var ifNoneMatch = "ifNoneMatch_example";  // string? | Value of the `ETag` header in the most recently cached response by the client. (optional) 

            try
            {
                Collection<Media> result = apiInstance.GetEventTeamMedia(eventKey, ifNoneMatch);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling EventApi.GetEventTeamMedia: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the GetEventTeamMediaWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<Collection<Media>> response = apiInstance.GetEventTeamMediaWithHttpInfo(eventKey, ifNoneMatch);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling EventApi.GetEventTeamMediaWithHttpInfo: " + e.Message);
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
            var apiInstance = new EventApi(httpClient, config, httpClientHandler);
            var eventKey = "eventKey_example";  // string | TBA Event Key, eg `2016nytr`
            var ifNoneMatch = "ifNoneMatch_example";  // string? | Value of the `ETag` header in the most recently cached response by the client. (optional) 

            try
            {
                Collection<Team> result = apiInstance.GetEventTeams(eventKey, ifNoneMatch);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling EventApi.GetEventTeams: " + e.Message);
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
    Debug.Print("Exception when calling EventApi.GetEventTeamsWithHttpInfo: " + e.Message);
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
            var apiInstance = new EventApi(httpClient, config, httpClientHandler);
            var eventKey = "eventKey_example";  // string | TBA Event Key, eg `2016nytr`
            var ifNoneMatch = "ifNoneMatch_example";  // string? | Value of the `ETag` header in the most recently cached response by the client. (optional) 

            try
            {
                Collection<string> result = apiInstance.GetEventTeamsKeys(eventKey, ifNoneMatch);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling EventApi.GetEventTeamsKeys: " + e.Message);
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
    Debug.Print("Exception when calling EventApi.GetEventTeamsKeysWithHttpInfo: " + e.Message);
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
            var apiInstance = new EventApi(httpClient, config, httpClientHandler);
            var eventKey = "eventKey_example";  // string | TBA Event Key, eg `2016nytr`
            var ifNoneMatch = "ifNoneMatch_example";  // string? | Value of the `ETag` header in the most recently cached response by the client. (optional) 

            try
            {
                Collection<TeamSimple> result = apiInstance.GetEventTeamsSimple(eventKey, ifNoneMatch);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling EventApi.GetEventTeamsSimple: " + e.Message);
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
    Debug.Print("Exception when calling EventApi.GetEventTeamsSimpleWithHttpInfo: " + e.Message);
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
            var apiInstance = new EventApi(httpClient, config, httpClientHandler);
            var eventKey = "eventKey_example";  // string | TBA Event Key, eg `2016nytr`
            var ifNoneMatch = "ifNoneMatch_example";  // string? | Value of the `ETag` header in the most recently cached response by the client. (optional) 

            try
            {
                Dictionary<string, GetTeamEventsStatusesByYear200ResponseValue> result = apiInstance.GetEventTeamsStatuses(eventKey, ifNoneMatch);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling EventApi.GetEventTeamsStatuses: " + e.Message);
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
    Debug.Print("Exception when calling EventApi.GetEventTeamsStatusesWithHttpInfo: " + e.Message);
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
            var apiInstance = new EventApi(httpClient, config, httpClientHandler);
            var year = 56;  // int | Competition Year (or Season). Must be 4 digits.
            var ifNoneMatch = "ifNoneMatch_example";  // string? | Value of the `ETag` header in the most recently cached response by the client. (optional) 

            try
            {
                Collection<Event> result = apiInstance.GetEventsByYear(year, ifNoneMatch);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling EventApi.GetEventsByYear: " + e.Message);
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
    Debug.Print("Exception when calling EventApi.GetEventsByYearWithHttpInfo: " + e.Message);
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
            var apiInstance = new EventApi(httpClient, config, httpClientHandler);
            var year = 56;  // int | Competition Year (or Season). Must be 4 digits.
            var ifNoneMatch = "ifNoneMatch_example";  // string? | Value of the `ETag` header in the most recently cached response by the client. (optional) 

            try
            {
                Collection<string> result = apiInstance.GetEventsByYearKeys(year, ifNoneMatch);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling EventApi.GetEventsByYearKeys: " + e.Message);
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
    Debug.Print("Exception when calling EventApi.GetEventsByYearKeysWithHttpInfo: " + e.Message);
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
            var apiInstance = new EventApi(httpClient, config, httpClientHandler);
            var year = 56;  // int | Competition Year (or Season). Must be 4 digits.
            var ifNoneMatch = "ifNoneMatch_example";  // string? | Value of the `ETag` header in the most recently cached response by the client. (optional) 

            try
            {
                Collection<EventSimple> result = apiInstance.GetEventsByYearSimple(year, ifNoneMatch);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling EventApi.GetEventsByYearSimple: " + e.Message);
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
    Debug.Print("Exception when calling EventApi.GetEventsByYearSimpleWithHttpInfo: " + e.Message);
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
            var apiInstance = new EventApi(httpClient, config, httpClientHandler);
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
                Debug.Print("Exception when calling EventApi.GetTeamEventAwards: " + e.Message);
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
    Debug.Print("Exception when calling EventApi.GetTeamEventAwardsWithHttpInfo: " + e.Message);
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
            var apiInstance = new EventApi(httpClient, config, httpClientHandler);
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
                Debug.Print("Exception when calling EventApi.GetTeamEventMatches: " + e.Message);
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
    Debug.Print("Exception when calling EventApi.GetTeamEventMatchesWithHttpInfo: " + e.Message);
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
            var apiInstance = new EventApi(httpClient, config, httpClientHandler);
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
                Debug.Print("Exception when calling EventApi.GetTeamEventMatchesKeys: " + e.Message);
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
    Debug.Print("Exception when calling EventApi.GetTeamEventMatchesKeysWithHttpInfo: " + e.Message);
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
            var apiInstance = new EventApi(httpClient, config, httpClientHandler);
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
                Debug.Print("Exception when calling EventApi.GetTeamEventMatchesSimple: " + e.Message);
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
    Debug.Print("Exception when calling EventApi.GetTeamEventMatchesSimpleWithHttpInfo: " + e.Message);
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
            var apiInstance = new EventApi(httpClient, config, httpClientHandler);
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
                Debug.Print("Exception when calling EventApi.GetTeamEventStatus: " + e.Message);
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
    Debug.Print("Exception when calling EventApi.GetTeamEventStatusWithHttpInfo: " + e.Message);
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
            var apiInstance = new EventApi(httpClient, config, httpClientHandler);
            var teamKey = "teamKey_example";  // string | TBA Team Key, eg `frc254`
            var ifNoneMatch = "ifNoneMatch_example";  // string? | Value of the `ETag` header in the most recently cached response by the client. (optional) 

            try
            {
                Collection<Event> result = apiInstance.GetTeamEvents(teamKey, ifNoneMatch);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling EventApi.GetTeamEvents: " + e.Message);
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
    Debug.Print("Exception when calling EventApi.GetTeamEventsWithHttpInfo: " + e.Message);
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
            var apiInstance = new EventApi(httpClient, config, httpClientHandler);
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
                Debug.Print("Exception when calling EventApi.GetTeamEventsByYear: " + e.Message);
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
    Debug.Print("Exception when calling EventApi.GetTeamEventsByYearWithHttpInfo: " + e.Message);
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
            var apiInstance = new EventApi(httpClient, config, httpClientHandler);
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
                Debug.Print("Exception when calling EventApi.GetTeamEventsByYearKeys: " + e.Message);
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
    Debug.Print("Exception when calling EventApi.GetTeamEventsByYearKeysWithHttpInfo: " + e.Message);
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
            var apiInstance = new EventApi(httpClient, config, httpClientHandler);
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
                Debug.Print("Exception when calling EventApi.GetTeamEventsByYearSimple: " + e.Message);
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
    Debug.Print("Exception when calling EventApi.GetTeamEventsByYearSimpleWithHttpInfo: " + e.Message);
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
            var apiInstance = new EventApi(httpClient, config, httpClientHandler);
            var teamKey = "teamKey_example";  // string | TBA Team Key, eg `frc254`
            var ifNoneMatch = "ifNoneMatch_example";  // string? | Value of the `ETag` header in the most recently cached response by the client. (optional) 

            try
            {
                Collection<string> result = apiInstance.GetTeamEventsKeys(teamKey, ifNoneMatch);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling EventApi.GetTeamEventsKeys: " + e.Message);
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
    Debug.Print("Exception when calling EventApi.GetTeamEventsKeysWithHttpInfo: " + e.Message);
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
            var apiInstance = new EventApi(httpClient, config, httpClientHandler);
            var teamKey = "teamKey_example";  // string | TBA Team Key, eg `frc254`
            var ifNoneMatch = "ifNoneMatch_example";  // string? | Value of the `ETag` header in the most recently cached response by the client. (optional) 

            try
            {
                Collection<EventSimple> result = apiInstance.GetTeamEventsSimple(teamKey, ifNoneMatch);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling EventApi.GetTeamEventsSimple: " + e.Message);
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
    Debug.Print("Exception when calling EventApi.GetTeamEventsSimpleWithHttpInfo: " + e.Message);
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
            var apiInstance = new EventApi(httpClient, config, httpClientHandler);
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
                Debug.Print("Exception when calling EventApi.GetTeamEventsStatusesByYear: " + e.Message);
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
    Debug.Print("Exception when calling EventApi.GetTeamEventsStatusesByYearWithHttpInfo: " + e.Message);
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

