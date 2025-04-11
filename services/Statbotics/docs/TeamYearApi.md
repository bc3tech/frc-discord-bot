# Statbotics.Api.TeamYearApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**ReadTeamYearV3TeamYearTeamYearGet**](TeamYearApi.md#readteamyearv3teamyearteamyearget) | **GET** /v3/team_year/{team}/{year} | Query a single team year |
| [**ReadTeamYearsV3TeamYearsGet**](TeamYearApi.md#readteamyearsv3teamyearsget) | **GET** /v3/team_years | Query multiple team years |

<a id="readteamyearv3teamyearteamyearget"></a>
# **ReadTeamYearV3TeamYearTeamYearGet**
> Object ReadTeamYearV3TeamYearTeamYearGet (string team, int year)

Query a single team year

Returns a single Team Year object. Requires a team number and year.

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using Statbotics.Api;
using Statbotics.Client;
using Statbotics.Model;

namespace Example


    public class ReadTeamYearV3TeamYearTeamYearGetExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // create instances of HttpClient, HttpClientHandler to be reused later with different Api classes
            HttpClient httpClient = new HttpClient();
            HttpClientHandler httpClientHandler = new HttpClientHandler();
            var apiInstance = new TeamYearApi(httpClient, config, httpClientHandler);
            var team = "team_example";  // string | 
            var year = 56;  // int | 

            try
            {
                // Query a single team year
                Object result = apiInstance.ReadTeamYearV3TeamYearTeamYearGet(team, year);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling TeamYearApi.ReadTeamYearV3TeamYearTeamYearGet: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the ReadTeamYearV3TeamYearTeamYearGetWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    // Query a single team year
    ApiResponse<Object> response = apiInstance.ReadTeamYearV3TeamYearTeamYearGetWithHttpInfo(team, year);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling TeamYearApi.ReadTeamYearV3TeamYearTeamYearGetWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **team** | **string** |  |  |
| **year** | **int** |  |  |

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

<a id="readteamyearsv3teamyearsget"></a>
# **ReadTeamYearsV3TeamYearsGet**
> Collection&lt;Object&gt; ReadTeamYearsV3TeamYearsGet (bool? ascending = null, string? country = null, string? district = null, int? limit = null, string? metric = null, bool? offseason = null, int? offset = null, string? state = null, string? team = null, int? year = null)

Query multiple team years

Returns up to 1000 team years at a time. Specify limit and offset to page through results.

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using Statbotics.Api;
using Statbotics.Client;
using Statbotics.Model;

namespace Example


    public class ReadTeamYearsV3TeamYearsGetExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // create instances of HttpClient, HttpClientHandler to be reused later with different Api classes
            HttpClient httpClient = new HttpClient();
            HttpClientHandler httpClientHandler = new HttpClientHandler();
            var apiInstance = new TeamYearApi(httpClient, config, httpClientHandler);
            var ascending = true;  // bool? | Whether to sort the returned values in ascending order. Default is ascending (optional) 
            var country = "country_example";  // string? | Capitalized country name, e.g. `USA` or `Canada`. (optional) 
            var district = "district_example";  // string? | One of [`fma`, `fnc`, `fit`, `fin`, `fim`, `ne`, `chs`, `ont`, `pnw`, `pch`, `isr`] (optional) 
            var limit = 56;  // int? | Maximum number of events to return. Default is 1000. (optional) 
            var metric = "metric_example";  // string? | How to sort the returned values. Any column in the table is valid. (optional) 
            var offseason = true;  // bool? | Whether the event is an offseason event. (optional) 
            var offset = 56;  // int? | Offset from the first result to return. (optional) 
            var state = "state_example";  // string? | Capitalized two-letter state code, e.g. `NC`. (optional) 
            var team = "team_example";  // string? | Team number (no prefix), e.g. `5511`. (optional) 
            var year = 56;  // int? | Four-digit year (optional) 

            try
            {
                // Query multiple team years
                Collection<Object> result = apiInstance.ReadTeamYearsV3TeamYearsGet(ascending, country, district, limit, metric, offseason, offset, state, team, year);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling TeamYearApi.ReadTeamYearsV3TeamYearsGet: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the ReadTeamYearsV3TeamYearsGetWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    // Query multiple team years
    ApiResponse<Collection<Object>> response = apiInstance.ReadTeamYearsV3TeamYearsGetWithHttpInfo(ascending, country, district, limit, metric, offseason, offset, state, team, year);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling TeamYearApi.ReadTeamYearsV3TeamYearsGetWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **ascending** | **bool?** | Whether to sort the returned values in ascending order. Default is ascending | [optional]  |
| **country** | **string?** | Capitalized country name, e.g. &#x60;USA&#x60; or &#x60;Canada&#x60;. | [optional]  |
| **district** | **string?** | One of [&#x60;fma&#x60;, &#x60;fnc&#x60;, &#x60;fit&#x60;, &#x60;fin&#x60;, &#x60;fim&#x60;, &#x60;ne&#x60;, &#x60;chs&#x60;, &#x60;ont&#x60;, &#x60;pnw&#x60;, &#x60;pch&#x60;, &#x60;isr&#x60;] | [optional]  |
| **limit** | **int?** | Maximum number of events to return. Default is 1000. | [optional]  |
| **metric** | **string?** | How to sort the returned values. Any column in the table is valid. | [optional]  |
| **offseason** | **bool?** | Whether the event is an offseason event. | [optional]  |
| **offset** | **int?** | Offset from the first result to return. | [optional]  |
| **state** | **string?** | Capitalized two-letter state code, e.g. &#x60;NC&#x60;. | [optional]  |
| **team** | **string?** | Team number (no prefix), e.g. &#x60;5511&#x60;. | [optional]  |
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

