# Org.OpenAPITools.Api.TeamApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**ReadTeamV3TeamTeamGet**](TeamApi.md#readteamv3teamteamget) | **GET** /v3/team/{team} | Query a single team |
| [**ReadTeamsV3TeamsGet**](TeamApi.md#readteamsv3teamsget) | **GET** /v3/teams | Query multiple teams |

<a id="readteamv3teamteamget"></a>
# **ReadTeamV3TeamTeamGet**
> Object ReadTeamV3TeamTeamGet (string team)

Query a single team

Returns a single Team object. Requires a team number (no prefix).

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using Org.OpenAPITools.Api;
using Org.OpenAPITools.Client;
using Org.OpenAPITools.Model;

namespace Example


    public class ReadTeamV3TeamTeamGetExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // create instances of HttpClient, HttpClientHandler to be reused later with different Api classes
            HttpClient httpClient = new HttpClient();
            HttpClientHandler httpClientHandler = new HttpClientHandler();
            var apiInstance = new TeamApi(httpClient, config, httpClientHandler);
            var team = "team_example";  // string | 

            try
            {
                // Query a single team
                Object result = apiInstance.ReadTeamV3TeamTeamGet(team);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling TeamApi.ReadTeamV3TeamTeamGet: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the ReadTeamV3TeamTeamGetWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    // Query a single team
    ApiResponse<Object> response = apiInstance.ReadTeamV3TeamTeamGetWithHttpInfo(team);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling TeamApi.ReadTeamV3TeamTeamGetWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
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

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a id="readteamsv3teamsget"></a>
# **ReadTeamsV3TeamsGet**
> List&lt;Object&gt; ReadTeamsV3TeamsGet (string? country = null, string? state = null, string? district = null, bool? active = null, bool? offseason = null, string? metric = null, bool? ascending = null, int? limit = null, int? offset = null)

Query multiple teams

Returns up to 1000 teams at a time. Specify limit and offset to page through results.

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using Org.OpenAPITools.Api;
using Org.OpenAPITools.Client;
using Org.OpenAPITools.Model;

namespace Example


    public class ReadTeamsV3TeamsGetExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // create instances of HttpClient, HttpClientHandler to be reused later with different Api classes
            HttpClient httpClient = new HttpClient();
            HttpClientHandler httpClientHandler = new HttpClientHandler();
            var apiInstance = new TeamApi(httpClient, config, httpClientHandler);
            var country = "country_example";  // string? | Capitalized country name, e.g. `USA` or `Canada`. (optional) 
            var state = "state_example";  // string? | Capitalized two-letter state code, e.g. `NC`. (optional) 
            var district = "district_example";  // string? | One of [`fma`, `fnc`, `fit`, `fin`, `fim`, `ne`, `chs`, `ont`, `pnw`, `pch`, `isr`] (optional) 
            var active = true;  // bool? | Whether the team has played in the last year. (optional) 
            var offseason = true;  // bool? | Whether the event is an offseason event. (optional) 
            var metric = "metric_example";  // string? | How to sort the returned values. Any column in the table is valid. (optional) 
            var ascending = true;  // bool? | Whether to sort the returned values in ascending order. Default is ascending (optional) 
            var limit = 56;  // int? | Maximum number of events to return. Default is 1000. (optional) 
            var offset = 56;  // int? | Offset from the first result to return. (optional) 

            try
            {
                // Query multiple teams
                List<Object> result = apiInstance.ReadTeamsV3TeamsGet(country, state, district, active, offseason, metric, ascending, limit, offset);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling TeamApi.ReadTeamsV3TeamsGet: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the ReadTeamsV3TeamsGetWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    // Query multiple teams
    ApiResponse<List<Object>> response = apiInstance.ReadTeamsV3TeamsGetWithHttpInfo(country, state, district, active, offseason, metric, ascending, limit, offset);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling TeamApi.ReadTeamsV3TeamsGetWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **country** | **string?** | Capitalized country name, e.g. &#x60;USA&#x60; or &#x60;Canada&#x60;. | [optional]  |
| **state** | **string?** | Capitalized two-letter state code, e.g. &#x60;NC&#x60;. | [optional]  |
| **district** | **string?** | One of [&#x60;fma&#x60;, &#x60;fnc&#x60;, &#x60;fit&#x60;, &#x60;fin&#x60;, &#x60;fim&#x60;, &#x60;ne&#x60;, &#x60;chs&#x60;, &#x60;ont&#x60;, &#x60;pnw&#x60;, &#x60;pch&#x60;, &#x60;isr&#x60;] | [optional]  |
| **active** | **bool?** | Whether the team has played in the last year. | [optional]  |
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

