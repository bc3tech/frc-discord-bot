# Statbotics.Api.MatchApi

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
using Statbotics.Api;
using Statbotics.Client;
using Statbotics.Model;

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

[[Back to top]](#) [[Back to API list]](../../README.md#documentation-for-api-endpoints) [[Back to Model list]](../../README.md#documentation-for-models) [[Back to README]](../../README.md)

<a id="readmatchesv3matchesget"></a>
# **ReadMatchesV3MatchesGet**
> Collection&lt;Object&gt; ReadMatchesV3MatchesGet (bool? ascending = null, bool? elim = null, int? limit = null, string? metric = null, bool? offseason = null, int? offset = null, string? team = null, string? varEvent = null, int? week = null, int? year = null)

Query multiple matches

Returns up to 1000 matches at a time. Specify limit and offset to page through results.

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using Statbotics.Api;
using Statbotics.Client;
using Statbotics.Model;

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
            var ascending = true;  // bool? | Whether to sort the returned values in ascending order. Default is ascending (optional) 
            var elim = true;  // bool? | Whether the match is an elimination match. (optional) 
            var limit = 56;  // int? | Maximum number of events to return. Default is 1000. (optional) 
            var metric = "metric_example";  // string? | How to sort the returned values. Any column in the table is valid. (optional) 
            var offseason = true;  // bool? | Whether the event is an offseason event. (optional) 
            var offset = 56;  // int? | Offset from the first result to return. (optional) 
            var team = "team_example";  // string? | Team number (no prefix), e.g. `5511`. (optional) 
            var varEvent = "varEvent_example";  // string? | Event key, e.g. `2019ncwak`. (optional) 
            var week = 56;  // int? | Week of the competition season. 0 is preseason, 8 is CMP, 9 is offseason. (optional) 
            var year = 56;  // int? | Four-digit year (optional) 

            try
            {
                // Query multiple matches
                Collection<Object> result = apiInstance.ReadMatchesV3MatchesGet(ascending, elim, limit, metric, offseason, offset, team, varEvent, week, year);
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
    ApiResponse<Collection<Object>> response = apiInstance.ReadMatchesV3MatchesGetWithHttpInfo(ascending, elim, limit, metric, offseason, offset, team, varEvent, week, year);
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
| **ascending** | **bool?** | Whether to sort the returned values in ascending order. Default is ascending | [optional]  |
| **elim** | **bool?** | Whether the match is an elimination match. | [optional]  |
| **limit** | **int?** | Maximum number of events to return. Default is 1000. | [optional]  |
| **metric** | **string?** | How to sort the returned values. Any column in the table is valid. | [optional]  |
| **offseason** | **bool?** | Whether the event is an offseason event. | [optional]  |
| **offset** | **int?** | Offset from the first result to return. | [optional]  |
| **team** | **string?** | Team number (no prefix), e.g. &#x60;5511&#x60;. | [optional]  |
| **varEvent** | **string?** | Event key, e.g. &#x60;2019ncwak&#x60;. | [optional]  |
| **week** | **int?** | Week of the competition season. 0 is preseason, 8 is CMP, 9 is offseason. | [optional]  |
| **year** | **int?** | Four-digit year | [optional]  |

### Return type

**Collection<Object>**

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

[[Back to top]](#) [[Back to API list]](../../README.md#documentation-for-api-endpoints) [[Back to Model list]](../../README.md#documentation-for-models) [[Back to README]](../../README.md)

