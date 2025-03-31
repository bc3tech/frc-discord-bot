# TheBlueAlliance.Api.MatchApi

All URIs are relative to *https://www.thebluealliance.com/api/v3*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**GetEventMatchTimeseries**](MatchApi.md#geteventmatchtimeseries) | **GET** /event/{event_key}/matches/timeseries |  |
| [**GetEventMatches**](MatchApi.md#geteventmatches) | **GET** /event/{event_key}/matches |  |
| [**GetEventMatchesKeys**](MatchApi.md#geteventmatcheskeys) | **GET** /event/{event_key}/matches/keys |  |
| [**GetEventMatchesSimple**](MatchApi.md#geteventmatchessimple) | **GET** /event/{event_key}/matches/simple |  |
| [**GetMatch**](MatchApi.md#getmatch) | **GET** /match/{match_key} |  |
| [**GetMatchSimple**](MatchApi.md#getmatchsimple) | **GET** /match/{match_key}/simple |  |
| [**GetMatchTimeseries**](MatchApi.md#getmatchtimeseries) | **GET** /match/{match_key}/timeseries |  |
| [**GetMatchZebra**](MatchApi.md#getmatchzebra) | **GET** /match/{match_key}/zebra_motionworks |  |
| [**GetTeamEventMatches**](MatchApi.md#getteameventmatches) | **GET** /team/{team_key}/event/{event_key}/matches |  |
| [**GetTeamEventMatchesKeys**](MatchApi.md#getteameventmatcheskeys) | **GET** /team/{team_key}/event/{event_key}/matches/keys |  |
| [**GetTeamEventMatchesSimple**](MatchApi.md#getteameventmatchessimple) | **GET** /team/{team_key}/event/{event_key}/matches/simple |  |
| [**GetTeamMatchesByYear**](MatchApi.md#getteammatchesbyyear) | **GET** /team/{team_key}/matches/{year} |  |
| [**GetTeamMatchesByYearKeys**](MatchApi.md#getteammatchesbyyearkeys) | **GET** /team/{team_key}/matches/{year}/keys |  |
| [**GetTeamMatchesByYearSimple**](MatchApi.md#getteammatchesbyyearsimple) | **GET** /team/{team_key}/matches/{year}/simple |  |

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
            var apiInstance = new MatchApi(httpClient, config, httpClientHandler);
            var eventKey = "eventKey_example";  // string | TBA Event Key, eg `2016nytr`
            var ifNoneMatch = "ifNoneMatch_example";  // string? | Value of the `ETag` header in the most recently cached response by the client. (optional) 

            try
            {
                Collection<string> result = apiInstance.GetEventMatchTimeseries(eventKey, ifNoneMatch);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling MatchApi.GetEventMatchTimeseries: " + e.Message);
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
    Debug.Print("Exception when calling MatchApi.GetEventMatchTimeseriesWithHttpInfo: " + e.Message);
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
            var apiInstance = new MatchApi(httpClient, config, httpClientHandler);
            var eventKey = "eventKey_example";  // string | TBA Event Key, eg `2016nytr`
            var ifNoneMatch = "ifNoneMatch_example";  // string? | Value of the `ETag` header in the most recently cached response by the client. (optional) 

            try
            {
                Collection<Match> result = apiInstance.GetEventMatches(eventKey, ifNoneMatch);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling MatchApi.GetEventMatches: " + e.Message);
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
    Debug.Print("Exception when calling MatchApi.GetEventMatchesWithHttpInfo: " + e.Message);
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
            var apiInstance = new MatchApi(httpClient, config, httpClientHandler);
            var eventKey = "eventKey_example";  // string | TBA Event Key, eg `2016nytr`
            var ifNoneMatch = "ifNoneMatch_example";  // string? | Value of the `ETag` header in the most recently cached response by the client. (optional) 

            try
            {
                Collection<string> result = apiInstance.GetEventMatchesKeys(eventKey, ifNoneMatch);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling MatchApi.GetEventMatchesKeys: " + e.Message);
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
    Debug.Print("Exception when calling MatchApi.GetEventMatchesKeysWithHttpInfo: " + e.Message);
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
            var apiInstance = new MatchApi(httpClient, config, httpClientHandler);
            var eventKey = "eventKey_example";  // string | TBA Event Key, eg `2016nytr`
            var ifNoneMatch = "ifNoneMatch_example";  // string? | Value of the `ETag` header in the most recently cached response by the client. (optional) 

            try
            {
                Collection<MatchSimple> result = apiInstance.GetEventMatchesSimple(eventKey, ifNoneMatch);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling MatchApi.GetEventMatchesSimple: " + e.Message);
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
    Debug.Print("Exception when calling MatchApi.GetEventMatchesSimpleWithHttpInfo: " + e.Message);
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

<a id="getmatch"></a>
# **GetMatch**
> Match GetMatch (string matchKey, string? ifNoneMatch = null)



Gets a `Match` object for the given match key.

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using TheBlueAlliance.Api;
using TheBlueAlliance.Client;
using TheBlueAlliance.Model;

namespace Example


    public class GetMatchExample
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
            var apiInstance = new MatchApi(httpClient, config, httpClientHandler);
            var matchKey = "matchKey_example";  // string | TBA Match Key, eg `2016nytr_qm1`
            var ifNoneMatch = "ifNoneMatch_example";  // string? | Value of the `ETag` header in the most recently cached response by the client. (optional) 

            try
            {
                Match result = apiInstance.GetMatch(matchKey, ifNoneMatch);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling MatchApi.GetMatch: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the GetMatchWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<Match> response = apiInstance.GetMatchWithHttpInfo(matchKey, ifNoneMatch);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling MatchApi.GetMatchWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **matchKey** | **string** | TBA Match Key, eg &#x60;2016nytr_qm1&#x60; |  |
| **ifNoneMatch** | **string?** | Value of the &#x60;ETag&#x60; header in the most recently cached response by the client. | [optional]  |

### Return type

[**Match**](Match.md)

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

<a id="getmatchsimple"></a>
# **GetMatchSimple**
> MatchSimple GetMatchSimple (string matchKey, string? ifNoneMatch = null)



Gets a short-form `Match` object for the given match key.

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using TheBlueAlliance.Api;
using TheBlueAlliance.Client;
using TheBlueAlliance.Model;

namespace Example


    public class GetMatchSimpleExample
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
            var apiInstance = new MatchApi(httpClient, config, httpClientHandler);
            var matchKey = "matchKey_example";  // string | TBA Match Key, eg `2016nytr_qm1`
            var ifNoneMatch = "ifNoneMatch_example";  // string? | Value of the `ETag` header in the most recently cached response by the client. (optional) 

            try
            {
                MatchSimple result = apiInstance.GetMatchSimple(matchKey, ifNoneMatch);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling MatchApi.GetMatchSimple: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the GetMatchSimpleWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<MatchSimple> response = apiInstance.GetMatchSimpleWithHttpInfo(matchKey, ifNoneMatch);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling MatchApi.GetMatchSimpleWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **matchKey** | **string** | TBA Match Key, eg &#x60;2016nytr_qm1&#x60; |  |
| **ifNoneMatch** | **string?** | Value of the &#x60;ETag&#x60; header in the most recently cached response by the client. | [optional]  |

### Return type

[**MatchSimple**](MatchSimple.md)

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

<a id="getmatchtimeseries"></a>
# **GetMatchTimeseries**
> Collection&lt;Object&gt; GetMatchTimeseries (string matchKey, string? ifNoneMatch = null)



Gets an array of game-specific Match Timeseries objects for the given match key or an empty array if not available. *WARNING:* This is *not* official data, and is subject to a significant possibility of error, or missing data. Do not rely on this data for any purpose. In fact, pretend we made it up. *WARNING:* This endpoint and corresponding data models are under *active development* and may change at any time, including in breaking ways.

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using TheBlueAlliance.Api;
using TheBlueAlliance.Client;
using TheBlueAlliance.Model;

namespace Example


    public class GetMatchTimeseriesExample
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
            var apiInstance = new MatchApi(httpClient, config, httpClientHandler);
            var matchKey = "matchKey_example";  // string | TBA Match Key, eg `2016nytr_qm1`
            var ifNoneMatch = "ifNoneMatch_example";  // string? | Value of the `ETag` header in the most recently cached response by the client. (optional) 

            try
            {
                Collection<Object> result = apiInstance.GetMatchTimeseries(matchKey, ifNoneMatch);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling MatchApi.GetMatchTimeseries: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the GetMatchTimeseriesWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<Collection<Object>> response = apiInstance.GetMatchTimeseriesWithHttpInfo(matchKey, ifNoneMatch);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling MatchApi.GetMatchTimeseriesWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **matchKey** | **string** | TBA Match Key, eg &#x60;2016nytr_qm1&#x60; |  |
| **ifNoneMatch** | **string?** | Value of the &#x60;ETag&#x60; header in the most recently cached response by the client. | [optional]  |

### Return type

**Collection<Object>**

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

<a id="getmatchzebra"></a>
# **GetMatchZebra**
> Zebra GetMatchZebra (string matchKey, string? ifNoneMatch = null)



Gets Zebra MotionWorks data for a Match for the given match key.

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using TheBlueAlliance.Api;
using TheBlueAlliance.Client;
using TheBlueAlliance.Model;

namespace Example


    public class GetMatchZebraExample
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
            var apiInstance = new MatchApi(httpClient, config, httpClientHandler);
            var matchKey = "matchKey_example";  // string | TBA Match Key, eg `2016nytr_qm1`
            var ifNoneMatch = "ifNoneMatch_example";  // string? | Value of the `ETag` header in the most recently cached response by the client. (optional) 

            try
            {
                Zebra result = apiInstance.GetMatchZebra(matchKey, ifNoneMatch);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling MatchApi.GetMatchZebra: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the GetMatchZebraWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<Zebra> response = apiInstance.GetMatchZebraWithHttpInfo(matchKey, ifNoneMatch);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling MatchApi.GetMatchZebraWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **matchKey** | **string** | TBA Match Key, eg &#x60;2016nytr_qm1&#x60; |  |
| **ifNoneMatch** | **string?** | Value of the &#x60;ETag&#x60; header in the most recently cached response by the client. | [optional]  |

### Return type

[**Zebra**](Zebra.md)

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
            var apiInstance = new MatchApi(httpClient, config, httpClientHandler);
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
                Debug.Print("Exception when calling MatchApi.GetTeamEventMatches: " + e.Message);
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
    Debug.Print("Exception when calling MatchApi.GetTeamEventMatchesWithHttpInfo: " + e.Message);
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
            var apiInstance = new MatchApi(httpClient, config, httpClientHandler);
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
                Debug.Print("Exception when calling MatchApi.GetTeamEventMatchesKeys: " + e.Message);
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
    Debug.Print("Exception when calling MatchApi.GetTeamEventMatchesKeysWithHttpInfo: " + e.Message);
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
            var apiInstance = new MatchApi(httpClient, config, httpClientHandler);
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
                Debug.Print("Exception when calling MatchApi.GetTeamEventMatchesSimple: " + e.Message);
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
    Debug.Print("Exception when calling MatchApi.GetTeamEventMatchesSimpleWithHttpInfo: " + e.Message);
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
            var apiInstance = new MatchApi(httpClient, config, httpClientHandler);
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
                Debug.Print("Exception when calling MatchApi.GetTeamMatchesByYear: " + e.Message);
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
    Debug.Print("Exception when calling MatchApi.GetTeamMatchesByYearWithHttpInfo: " + e.Message);
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
            var apiInstance = new MatchApi(httpClient, config, httpClientHandler);
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
                Debug.Print("Exception when calling MatchApi.GetTeamMatchesByYearKeys: " + e.Message);
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
    Debug.Print("Exception when calling MatchApi.GetTeamMatchesByYearKeysWithHttpInfo: " + e.Message);
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
            var apiInstance = new MatchApi(httpClient, config, httpClientHandler);
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
                Debug.Print("Exception when calling MatchApi.GetTeamMatchesByYearSimple: " + e.Message);
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
    Debug.Print("Exception when calling MatchApi.GetTeamMatchesByYearSimpleWithHttpInfo: " + e.Message);
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

