# Statbotics.Api.V2TeamApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**ReadTeamV2TeamTeamGet**](V2TeamApi.md#readteamv2teamteamget) | **GET** /v2/team/{team} | Read Team |
| [**ReadTeamsDistrictV2TeamsDistrictDistrictGet**](V2TeamApi.md#readteamsdistrictv2teamsdistrictdistrictget) | **GET** /v2/teams/district/{district} | Read Teams District |
| [**ReadTeamsStateV2TeamsStateStateGet**](V2TeamApi.md#readteamsstatev2teamsstatestateget) | **GET** /v2/teams/state/{state} | Read Teams State |
| [**ReadTeamsV2TeamsGet**](V2TeamApi.md#readteamsv2teamsget) | **GET** /v2/teams | Read Teams |

<a id="readteamv2teamteamget"></a>
# **ReadTeamV2TeamTeamGet**
> Object ReadTeamV2TeamTeamGet (int team)

Read Team

Get a single Team object containing team name, location, normalized EPA statistics, and winrate.

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using Statbotics.Api;
using Statbotics.Client;
using Statbotics.Model;

namespace Example


    public class ReadTeamV2TeamTeamGetExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // create instances of HttpClient, HttpClientHandler to be reused later with different Api classes
            HttpClient httpClient = new HttpClient();
            HttpClientHandler httpClientHandler = new HttpClientHandler();
            var apiInstance = new V2TeamApi(httpClient, config, httpClientHandler);
            var team = 56;  // int | 

            try
            {
                // Read Team
                Object result = apiInstance.ReadTeamV2TeamTeamGet(team);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling V2TeamApi.ReadTeamV2TeamTeamGet: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the ReadTeamV2TeamTeamGetWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    // Read Team
    ApiResponse<Object> response = apiInstance.ReadTeamV2TeamTeamGetWithHttpInfo(team);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling V2TeamApi.ReadTeamV2TeamTeamGetWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **team** | **int** |  |  |

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
| **200** | A Team object. |  -  |
| **422** | Validation Error |  -  |

[[Back to top]](#) [[Back to API list]](../../README.md#documentation-for-api-endpoints) [[Back to Model list]](../../README.md#documentation-for-models) [[Back to README]](../../README.md)

<a id="readteamsdistrictv2teamsdistrictdistrictget"></a>
# **ReadTeamsDistrictV2TeamsDistrictDistrictGet**
> Collection&lt;Object&gt; ReadTeamsDistrictV2TeamsDistrictDistrictGet (string district)

Read Teams District

Get a list of Team objects from a single district. Specify lowercase district abbreviation, ex: fnc, fim

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using Statbotics.Api;
using Statbotics.Client;
using Statbotics.Model;

namespace Example


    public class ReadTeamsDistrictV2TeamsDistrictDistrictGetExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // create instances of HttpClient, HttpClientHandler to be reused later with different Api classes
            HttpClient httpClient = new HttpClient();
            HttpClientHandler httpClientHandler = new HttpClientHandler();
            var apiInstance = new V2TeamApi(httpClient, config, httpClientHandler);
            var district = "district_example";  // string | 

            try
            {
                // Read Teams District
                Collection<Object> result = apiInstance.ReadTeamsDistrictV2TeamsDistrictDistrictGet(district);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling V2TeamApi.ReadTeamsDistrictV2TeamsDistrictDistrictGet: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the ReadTeamsDistrictV2TeamsDistrictDistrictGetWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    // Read Teams District
    ApiResponse<Collection<Object>> response = apiInstance.ReadTeamsDistrictV2TeamsDistrictDistrictGetWithHttpInfo(district);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling V2TeamApi.ReadTeamsDistrictV2TeamsDistrictDistrictGetWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **district** | **string** |  |  |

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
| **200** | A list of Team objects. See /team/{team} for more information. |  -  |
| **422** | Validation Error |  -  |

[[Back to top]](#) [[Back to API list]](../../README.md#documentation-for-api-endpoints) [[Back to Model list]](../../README.md#documentation-for-models) [[Back to README]](../../README.md)

<a id="readteamsstatev2teamsstatestateget"></a>
# **ReadTeamsStateV2TeamsStateStateGet**
> Collection&lt;Object&gt; ReadTeamsStateV2TeamsStateStateGet (string state)

Read Teams State

Get a list of Team objects from a single state. Specify uppercase state abbreviation, ex: NC, CA

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using Statbotics.Api;
using Statbotics.Client;
using Statbotics.Model;

namespace Example


    public class ReadTeamsStateV2TeamsStateStateGetExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // create instances of HttpClient, HttpClientHandler to be reused later with different Api classes
            HttpClient httpClient = new HttpClient();
            HttpClientHandler httpClientHandler = new HttpClientHandler();
            var apiInstance = new V2TeamApi(httpClient, config, httpClientHandler);
            var state = "state_example";  // string | 

            try
            {
                // Read Teams State
                Collection<Object> result = apiInstance.ReadTeamsStateV2TeamsStateStateGet(state);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling V2TeamApi.ReadTeamsStateV2TeamsStateStateGet: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the ReadTeamsStateV2TeamsStateStateGetWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    // Read Teams State
    ApiResponse<Collection<Object>> response = apiInstance.ReadTeamsStateV2TeamsStateStateGetWithHttpInfo(state);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling V2TeamApi.ReadTeamsStateV2TeamsStateStateGetWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **state** | **string** |  |  |

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
| **200** | A list of Team objects. See /team/{team} for more information. |  -  |
| **422** | Validation Error |  -  |

[[Back to top]](#) [[Back to API list]](../../README.md#documentation-for-api-endpoints) [[Back to Model list]](../../README.md#documentation-for-models) [[Back to README]](../../README.md)

<a id="readteamsv2teamsget"></a>
# **ReadTeamsV2TeamsGet**
> Collection&lt;Object&gt; ReadTeamsV2TeamsGet (bool? active = null, bool? ascending = null, string? country = null, string? district = null, int? limit = null, string? metric = null, bool? offseason = null, int? offset = null, string? state = null)

Read Teams

Get a list of Team objects with optional filters.

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using Statbotics.Api;
using Statbotics.Client;
using Statbotics.Model;

namespace Example


    public class ReadTeamsV2TeamsGetExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // create instances of HttpClient, HttpClientHandler to be reused later with different Api classes
            HttpClient httpClient = new HttpClient();
            HttpClientHandler httpClientHandler = new HttpClientHandler();
            var apiInstance = new V2TeamApi(httpClient, config, httpClientHandler);
            var active = true;  // bool? |  (optional) 
            var ascending = true;  // bool? |  (optional)  (default to true)
            var country = "country_example";  // string? |  (optional) 
            var district = "district_example";  // string? |  (optional) 
            var limit = 100;  // int? |  (optional)  (default to 100)
            var metric = "\"team\"";  // string? |  (optional)  (default to "team")
            var offseason = true;  // bool? |  (optional) 
            var offset = 0;  // int? |  (optional)  (default to 0)
            var state = "state_example";  // string? |  (optional) 

            try
            {
                // Read Teams
                Collection<Object> result = apiInstance.ReadTeamsV2TeamsGet(active, ascending, country, district, limit, metric, offseason, offset, state);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling V2TeamApi.ReadTeamsV2TeamsGet: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the ReadTeamsV2TeamsGetWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    // Read Teams
    ApiResponse<Collection<Object>> response = apiInstance.ReadTeamsV2TeamsGetWithHttpInfo(active, ascending, country, district, limit, metric, offseason, offset, state);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling V2TeamApi.ReadTeamsV2TeamsGetWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **active** | **bool?** |  | [optional]  |
| **ascending** | **bool?** |  | [optional] [default to true] |
| **country** | **string?** |  | [optional]  |
| **district** | **string?** |  | [optional]  |
| **limit** | **int?** |  | [optional] [default to 100] |
| **metric** | **string?** |  | [optional] [default to &quot;team&quot;] |
| **offseason** | **bool?** |  | [optional]  |
| **offset** | **int?** |  | [optional] [default to 0] |
| **state** | **string?** |  | [optional]  |

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
| **200** | A list of Team objects. See /team/{team} for more information. |  -  |
| **422** | Validation Error |  -  |

[[Back to top]](#) [[Back to API list]](../../README.md#documentation-for-api-endpoints) [[Back to Model list]](../../README.md#documentation-for-models) [[Back to README]](../../README.md)

