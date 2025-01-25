# Statbotics.Api.TeamMatchApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**ReadTeamMatchV3TeamMatchTeamMatchGet**](TeamMatchApi.md#readteammatchv3teammatchteammatchget) | **GET** /v3/team_match/{team}/{match} | Query a single team match |
| [**ReadTeamMatchesV3TeamMatchesGet**](TeamMatchApi.md#readteammatchesv3teammatchesget) | **GET** /v3/team_matches | Query multiple team matches |

<a id="readteammatchv3teammatchteammatchget"></a>
# **ReadTeamMatchV3TeamMatchTeamMatchGet**
> Object ReadTeamMatchV3TeamMatchTeamMatchGet (string match, string team)

Query a single team match

Returns a single Team Match object. Requires a team number and match key, e.g. `5511` and `2019ncwak_f1m1`.

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using Statbotics.Api;
using Statbotics.Client;
using Statbotics.Model;

namespace Example


    public class ReadTeamMatchV3TeamMatchTeamMatchGetExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // create instances of HttpClient, HttpClientHandler to be reused later with different Api classes
            HttpClient httpClient = new HttpClient();
            HttpClientHandler httpClientHandler = new HttpClientHandler();
            var apiInstance = new TeamMatchApi(httpClient, config, httpClientHandler);
            var match = "match_example";  // string | 
            var team = "team_example";  // string | 

            try
            {
                // Query a single team match
                Object result = apiInstance.ReadTeamMatchV3TeamMatchTeamMatchGet(match, team);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling TeamMatchApi.ReadTeamMatchV3TeamMatchTeamMatchGet: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the ReadTeamMatchV3TeamMatchTeamMatchGetWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    // Query a single team match
    ApiResponse<Object> response = apiInstance.ReadTeamMatchV3TeamMatchTeamMatchGetWithHttpInfo(match, team);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling TeamMatchApi.ReadTeamMatchV3TeamMatchTeamMatchGetWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **match** | **string** |  |  |
| **team** | **string** |  |  |

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

<a id="readteammatchesv3teammatchesget"></a>
# **ReadTeamMatchesV3TeamMatchesGet**
> Collection&lt;Object&gt; ReadTeamMatchesV3TeamMatchesGet (bool? ascending = null, bool? elim = null, int? limit = null, string? match = null, string? metric = null, bool? offseason = null, int? offset = null, string? team = null, string? varEvent = null, int? week = null, int? year = null)

Query multiple team matches

Returns up to 1000 team matches at a time. Specify limit and offset to page through results.

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using Statbotics.Api;
using Statbotics.Client;
using Statbotics.Model;

namespace Example


    public class ReadTeamMatchesV3TeamMatchesGetExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // create instances of HttpClient, HttpClientHandler to be reused later with different Api classes
            HttpClient httpClient = new HttpClient();
            HttpClientHandler httpClientHandler = new HttpClientHandler();
            var apiInstance = new TeamMatchApi(httpClient, config, httpClientHandler);
            var ascending = true;  // bool? | Whether to sort the returned values in ascending order. Default is ascending (optional) 
            var elim = true;  // bool? | Whether the match is an elimination match. (optional) 
            var limit = 56;  // int? | Maximum number of events to return. Default is 1000. (optional) 
            var match = "match_example";  // string? | Match key, e.g. `2019ncwak_f1m1`. (optional) 
            var metric = "metric_example";  // string? | How to sort the returned values. Any column in the table is valid. (optional) 
            var offseason = true;  // bool? | Whether the event is an offseason event. (optional) 
            var offset = 56;  // int? | Offset from the first result to return. (optional) 
            var team = "team_example";  // string? | Team number (no prefix), e.g. `5511`. (optional) 
            var varEvent = "varEvent_example";  // string? | Event key, e.g. `2019ncwak`. (optional) 
            var week = 56;  // int? | Week of the competition season. 0 is preseason, 8 is CMP, 9 is offseason. (optional) 
            var year = 56;  // int? | Four-digit year (optional) 

            try
            {
                // Query multiple team matches
                Collection<Object> result = apiInstance.ReadTeamMatchesV3TeamMatchesGet(ascending, elim, limit, match, metric, offseason, offset, team, varEvent, week, year);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling TeamMatchApi.ReadTeamMatchesV3TeamMatchesGet: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the ReadTeamMatchesV3TeamMatchesGetWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    // Query multiple team matches
    ApiResponse<Collection<Object>> response = apiInstance.ReadTeamMatchesV3TeamMatchesGetWithHttpInfo(ascending, elim, limit, match, metric, offseason, offset, team, varEvent, week, year);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling TeamMatchApi.ReadTeamMatchesV3TeamMatchesGetWithHttpInfo: " + e.Message);
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
| **match** | **string?** | Match key, e.g. &#x60;2019ncwak_f1m1&#x60;. | [optional]  |
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

