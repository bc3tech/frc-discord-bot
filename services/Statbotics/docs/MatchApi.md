# Org.OpenAPITools.Api.MatchApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**ReadMatchV3MatchMatchGet**](MatchApi.md#readmatchv3matchmatchget) | **GET** /v3/match/{match} | Query a single match |
| [**ReadMatchesV3MatchesGet**](MatchApi.md#readmatchesv3matchesget) | **GET** /v3/matches | Query multiple matches |

<a id="readmatchv3matchmatchget"></a>
# **ReadMatchV3MatchMatchGet**
> Object ReadMatchV3MatchMatchGet (string match)

Query a single match

Returns a single Match object. Requires a match key, e.g. `2019ncwak_f1m1`.

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using Org.OpenAPITools.Api;
using Org.OpenAPITools.Client;
using Org.OpenAPITools.Model;

namespace Example


    public class ReadMatchV3MatchMatchGetExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // create instances of HttpClient, HttpClientHandler to be reused later with different Api classes
            HttpClient httpClient = new HttpClient();
            HttpClientHandler httpClientHandler = new HttpClientHandler();
            var apiInstance = new MatchApi(httpClient, config, httpClientHandler);
            var match = "match_example";  // string | 

            try
            {
                // Query a single match
                Object result = apiInstance.ReadMatchV3MatchMatchGet(match);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling MatchApi.ReadMatchV3MatchMatchGet: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the ReadMatchV3MatchMatchGetWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    // Query a single match
    ApiResponse<Object> response = apiInstance.ReadMatchV3MatchMatchGetWithHttpInfo(match);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling MatchApi.ReadMatchV3MatchMatchGetWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **match** | **string** |  |  |

### Return type

**Object**

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | Successful Response |  -  |
| **422** | Validation Error |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="readmatchesv3matchesget"></a>
# **ReadMatchesV3MatchesGet**
> List&lt;Object&gt; ReadMatchesV3MatchesGet (string? team = null, int? year = null, string? varEvent = null, int? week = null, bool? elim = null, bool? offseason = null, string? metric = null, bool? ascending = null, int? limit = null, int? offset = null)

Query multiple matches

Returns up to 1000 matches at a time. Specify limit and offset to page through results.

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using Org.OpenAPITools.Api;
using Org.OpenAPITools.Client;
using Org.OpenAPITools.Model;

namespace Example


    public class ReadMatchesV3MatchesGetExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // create instances of HttpClient, HttpClientHandler to be reused later with different Api classes
            HttpClient httpClient = new HttpClient();
            HttpClientHandler httpClientHandler = new HttpClientHandler();
            var apiInstance = new MatchApi(httpClient, config, httpClientHandler);
            var team = "team_example";  // string? | Team number (no prefix), e.g. `5511`. (optional) 
            var year = 56;  // int? | Four-digit year (optional) 
            var varEvent = "varEvent_example";  // string? | Event key, e.g. `2019ncwak`. (optional) 
            var week = 56;  // int? | Week of the competition season. 0 is preseason, 8 is CMP, 9 is offseason. (optional) 
            var elim = true;  // bool? | Whether the match is an elimination match. (optional) 
            var offseason = true;  // bool? | Whether the event is an offseason event. (optional) 
            var metric = "metric_example";  // string? | How to sort the returned values. Any column in the table is valid. (optional) 
            var ascending = true;  // bool? | Whether to sort the returned values in ascending order. Default is ascending (optional) 
            var limit = 56;  // int? | Maximum number of events to return. Default is 1000. (optional) 
            var offset = 56;  // int? | Offset from the first result to return. (optional) 

            try
            {
                // Query multiple matches
                List<Object> result = apiInstance.ReadMatchesV3MatchesGet(team, year, varEvent, week, elim, offseason, metric, ascending, limit, offset);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling MatchApi.ReadMatchesV3MatchesGet: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the ReadMatchesV3MatchesGetWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    // Query multiple matches
    ApiResponse<List<Object>> response = apiInstance.ReadMatchesV3MatchesGetWithHttpInfo(team, year, varEvent, week, elim, offseason, metric, ascending, limit, offset);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling MatchApi.ReadMatchesV3MatchesGetWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **team** | **string?** | Team number (no prefix), e.g. &#x60;5511&#x60;. | [optional]  |
| **year** | **int?** | Four-digit year | [optional]  |
| **varEvent** | **string?** | Event key, e.g. &#x60;2019ncwak&#x60;. | [optional]  |
| **week** | **int?** | Week of the competition season. 0 is preseason, 8 is CMP, 9 is offseason. | [optional]  |
| **elim** | **bool?** | Whether the match is an elimination match. | [optional]  |
| **offseason** | **bool?** | Whether the event is an offseason event. | [optional]  |
| **metric** | **string?** | How to sort the returned values. Any column in the table is valid. | [optional]  |
| **ascending** | **bool?** | Whether to sort the returned values in ascending order. Default is ascending | [optional]  |
| **limit** | **int?** | Maximum number of events to return. Default is 1000. | [optional]  |
| **offset** | **int?** | Offset from the first result to return. | [optional]  |

### Return type

**List<Object>**

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | Successful Response |  -  |
| **422** | Validation Error |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

