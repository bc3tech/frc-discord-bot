# TheBlueAlliance.Api.Api.InsightApi

All URIs are relative to *https://www.thebluealliance.com/api/v3*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**GetInsightsLeaderboardsYear**](InsightApi.md#getinsightsleaderboardsyear) | **GET** /insights/leaderboards/{year} |  |
| [**GetInsightsNotablesYear**](InsightApi.md#getinsightsnotablesyear) | **GET** /insights/notables/{year} |  |

<a id="getinsightsleaderboardsyear"></a>
# **GetInsightsLeaderboardsYear**
> List&lt;LeaderboardInsight&gt; GetInsightsLeaderboardsYear (int year, string? ifNoneMatch = null)



Gets a list of `LeaderboardInsight` objects from a specific year. Use year=0 for overall.

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using TheBlueAlliance.Api.Api;
using TheBlueAlliance.Api.Client;
using TheBlueAlliance.Api.Model;

namespace Example
{
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
            var apiInstance = new InsightApi(httpClient, config, httpClientHandler);
            var year = 56;  // int | Competition Year (or Season). Must be 4 digits.
            var ifNoneMatch = "ifNoneMatch_example";  // string? | Value of the `ETag` header in the most recently cached response by the client. (optional) 

            try
            {
                List<LeaderboardInsight> result = apiInstance.GetInsightsLeaderboardsYear(year, ifNoneMatch);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling InsightApi.GetInsightsLeaderboardsYear: " + e.Message);
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
    ApiResponse<List<LeaderboardInsight>> response = apiInstance.GetInsightsLeaderboardsYearWithHttpInfo(year, ifNoneMatch);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling InsightApi.GetInsightsLeaderboardsYearWithHttpInfo: " + e.Message);
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

[**List&lt;LeaderboardInsight&gt;**](LeaderboardInsight.md)

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

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="getinsightsnotablesyear"></a>
# **GetInsightsNotablesYear**
> List&lt;NotablesInsight&gt; GetInsightsNotablesYear (int year, string? ifNoneMatch = null)



Gets a list of `NotablesInsight` objects from a specific year. Use year=0 for overall.

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using TheBlueAlliance.Api.Api;
using TheBlueAlliance.Api.Client;
using TheBlueAlliance.Api.Model;

namespace Example
{
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
            var apiInstance = new InsightApi(httpClient, config, httpClientHandler);
            var year = 56;  // int | Competition Year (or Season). Must be 4 digits.
            var ifNoneMatch = "ifNoneMatch_example";  // string? | Value of the `ETag` header in the most recently cached response by the client. (optional) 

            try
            {
                List<NotablesInsight> result = apiInstance.GetInsightsNotablesYear(year, ifNoneMatch);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling InsightApi.GetInsightsNotablesYear: " + e.Message);
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
    ApiResponse<List<NotablesInsight>> response = apiInstance.GetInsightsNotablesYearWithHttpInfo(year, ifNoneMatch);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling InsightApi.GetInsightsNotablesYearWithHttpInfo: " + e.Message);
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

[**List&lt;NotablesInsight&gt;**](NotablesInsight.md)

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

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

