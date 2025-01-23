# Statbotics.Api.V2TeamYearApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**ReadTeamYearV2TeamYearTeamYearGet**](V2TeamYearApi.md#readteamyearv2teamyearteamyearget) | **GET** /v2/team_year/{team}/{year} | Read Team Year |
| [**ReadTeamYearsDistrictV2TeamYearsYearYearDistrictDistrictGet**](V2TeamYearApi.md#readteamyearsdistrictv2teamyearsyearyeardistrictdistrictget) | **GET** /v2/team_years/year/{year}/district/{district} | Read Team Years District |
| [**ReadTeamYearsStateV2TeamYearsYearYearStateStateGet**](V2TeamYearApi.md#readteamyearsstatev2teamyearsyearyearstatestateget) | **GET** /v2/team_years/year/{year}/state/{state} | Read Team Years State |
| [**ReadTeamYearsTeamV2TeamYearsTeamTeamGet**](V2TeamYearApi.md#readteamyearsteamv2teamyearsteamteamget) | **GET** /v2/team_years/team/{team} | Read Team Years Team |
| [**ReadTeamYearsV2TeamYearsGet**](V2TeamYearApi.md#readteamyearsv2teamyearsget) | **GET** /v2/team_years | Read Team Years |

<a id="readteamyearv2teamyearteamyearget"></a>
# **ReadTeamYearV2TeamYearTeamYearGet**
> Object ReadTeamYearV2TeamYearTeamYearGet (int team, int year)

Read Team Year

Get a single TeamYear object containing EPA summary, winrates, and location rankings

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using Statbotics.Api;
using Statbotics.Client;
using Statbotics.Model;

namespace Example


    public class ReadTeamYearV2TeamYearTeamYearGetExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // create instances of HttpClient, HttpClientHandler to be reused later with different Api classes
            HttpClient httpClient = new HttpClient();
            HttpClientHandler httpClientHandler = new HttpClientHandler();
            var apiInstance = new V2TeamYearApi(httpClient, config, httpClientHandler);
            var team = 56;  // int | 
            var year = 56;  // int | 

            try
            {
                // Read Team Year
                Object result = apiInstance.ReadTeamYearV2TeamYearTeamYearGet(team, year);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling V2TeamYearApi.ReadTeamYearV2TeamYearTeamYearGet: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the ReadTeamYearV2TeamYearTeamYearGetWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    // Read Team Year
    ApiResponse<Object> response = apiInstance.ReadTeamYearV2TeamYearTeamYearGetWithHttpInfo(team, year);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling V2TeamYearApi.ReadTeamYearV2TeamYearTeamYearGetWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **team** | **int** |  |  |
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
| **200** | A TeamYear object. |  -  |
| **422** | Validation Error |  -  |

[[Back to top]](#) [[Back to API list]](../../README.md#documentation-for-api-endpoints) [[Back to Model list]](../../README.md#documentation-for-models) [[Back to README]](../../README.md)

<a id="readteamyearsdistrictv2teamyearsyearyeardistrictdistrictget"></a>
# **ReadTeamYearsDistrictV2TeamYearsYearYearDistrictDistrictGet**
> Collection&lt;Object&gt; ReadTeamYearsDistrictV2TeamYearsYearYearDistrictDistrictGet (string district, int year)

Read Team Years District

Get a list of TeamYear objects from a single district. Specify lowercase district abbreviation, ex: fnc, fim

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using Statbotics.Api;
using Statbotics.Client;
using Statbotics.Model;

namespace Example


    public class ReadTeamYearsDistrictV2TeamYearsYearYearDistrictDistrictGetExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // create instances of HttpClient, HttpClientHandler to be reused later with different Api classes
            HttpClient httpClient = new HttpClient();
            HttpClientHandler httpClientHandler = new HttpClientHandler();
            var apiInstance = new V2TeamYearApi(httpClient, config, httpClientHandler);
            var district = "district_example";  // string | 
            var year = 56;  // int | 

            try
            {
                // Read Team Years District
                Collection<Object> result = apiInstance.ReadTeamYearsDistrictV2TeamYearsYearYearDistrictDistrictGet(district, year);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling V2TeamYearApi.ReadTeamYearsDistrictV2TeamYearsYearYearDistrictDistrictGet: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the ReadTeamYearsDistrictV2TeamYearsYearYearDistrictDistrictGetWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    // Read Team Years District
    ApiResponse<Collection<Object>> response = apiInstance.ReadTeamYearsDistrictV2TeamYearsYearYearDistrictDistrictGetWithHttpInfo(district, year);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling V2TeamYearApi.ReadTeamYearsDistrictV2TeamYearsYearYearDistrictDistrictGetWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **district** | **string** |  |  |
| **year** | **int** |  |  |

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
| **200** | A list of TeamYear objects. See /team_year/{team}/{year} for more information. |  -  |
| **422** | Validation Error |  -  |

[[Back to top]](#) [[Back to API list]](../../README.md#documentation-for-api-endpoints) [[Back to Model list]](../../README.md#documentation-for-models) [[Back to README]](../../README.md)

<a id="readteamyearsstatev2teamyearsyearyearstatestateget"></a>
# **ReadTeamYearsStateV2TeamYearsYearYearStateStateGet**
> Collection&lt;Object&gt; ReadTeamYearsStateV2TeamYearsYearYearStateStateGet (string state, int year)

Read Team Years State

Get a list of TeamYear objects from a single state. Specify lowercase state abbreviation, ex: ca, tx

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using Statbotics.Api;
using Statbotics.Client;
using Statbotics.Model;

namespace Example


    public class ReadTeamYearsStateV2TeamYearsYearYearStateStateGetExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // create instances of HttpClient, HttpClientHandler to be reused later with different Api classes
            HttpClient httpClient = new HttpClient();
            HttpClientHandler httpClientHandler = new HttpClientHandler();
            var apiInstance = new V2TeamYearApi(httpClient, config, httpClientHandler);
            var state = "state_example";  // string | 
            var year = 56;  // int | 

            try
            {
                // Read Team Years State
                Collection<Object> result = apiInstance.ReadTeamYearsStateV2TeamYearsYearYearStateStateGet(state, year);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling V2TeamYearApi.ReadTeamYearsStateV2TeamYearsYearYearStateStateGet: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the ReadTeamYearsStateV2TeamYearsYearYearStateStateGetWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    // Read Team Years State
    ApiResponse<Collection<Object>> response = apiInstance.ReadTeamYearsStateV2TeamYearsYearYearStateStateGetWithHttpInfo(state, year);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling V2TeamYearApi.ReadTeamYearsStateV2TeamYearsYearYearStateStateGetWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **state** | **string** |  |  |
| **year** | **int** |  |  |

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
| **200** | A list of TeamYear objects. See /team_year/{team}/{year} for more information. |  -  |
| **422** | Validation Error |  -  |

[[Back to top]](#) [[Back to API list]](../../README.md#documentation-for-api-endpoints) [[Back to Model list]](../../README.md#documentation-for-models) [[Back to README]](../../README.md)

<a id="readteamyearsteamv2teamyearsteamteamget"></a>
# **ReadTeamYearsTeamV2TeamYearsTeamTeamGet**
> Collection&lt;Object&gt; ReadTeamYearsTeamV2TeamYearsTeamTeamGet (int team)

Read Team Years Team

Get a list of TeamYear objects for a single team. Specify team number, ex: 254, 1114

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using Statbotics.Api;
using Statbotics.Client;
using Statbotics.Model;

namespace Example


    public class ReadTeamYearsTeamV2TeamYearsTeamTeamGetExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // create instances of HttpClient, HttpClientHandler to be reused later with different Api classes
            HttpClient httpClient = new HttpClient();
            HttpClientHandler httpClientHandler = new HttpClientHandler();
            var apiInstance = new V2TeamYearApi(httpClient, config, httpClientHandler);
            var team = 56;  // int | 

            try
            {
                // Read Team Years Team
                Collection<Object> result = apiInstance.ReadTeamYearsTeamV2TeamYearsTeamTeamGet(team);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling V2TeamYearApi.ReadTeamYearsTeamV2TeamYearsTeamTeamGet: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the ReadTeamYearsTeamV2TeamYearsTeamTeamGetWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    // Read Team Years Team
    ApiResponse<Collection<Object>> response = apiInstance.ReadTeamYearsTeamV2TeamYearsTeamTeamGetWithHttpInfo(team);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling V2TeamYearApi.ReadTeamYearsTeamV2TeamYearsTeamTeamGetWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **team** | **int** |  |  |

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
| **200** | A list of TeamYear objects. See /team_year/{team}/{year} for more information. |  -  |
| **422** | Validation Error |  -  |

[[Back to top]](#) [[Back to API list]](../../README.md#documentation-for-api-endpoints) [[Back to Model list]](../../README.md#documentation-for-models) [[Back to README]](../../README.md)

<a id="readteamyearsv2teamyearsget"></a>
# **ReadTeamYearsV2TeamYearsGet**
> Collection&lt;Object&gt; ReadTeamYearsV2TeamYearsGet (bool? ascending = null, string? country = null, string? district = null, int? limit = null, string? metric = null, bool? offseason = null, int? offset = null, string? state = null, int? team = null, int? year = null)

Read Team Years

Get a list of TeamYear objects with optional filters.

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using Statbotics.Api;
using Statbotics.Client;
using Statbotics.Model;

namespace Example


    public class ReadTeamYearsV2TeamYearsGetExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // create instances of HttpClient, HttpClientHandler to be reused later with different Api classes
            HttpClient httpClient = new HttpClient();
            HttpClientHandler httpClientHandler = new HttpClientHandler();
            var apiInstance = new V2TeamYearApi(httpClient, config, httpClientHandler);
            var ascending = true;  // bool? |  (optional) 
            var country = "country_example";  // string? |  (optional) 
            var district = "district_example";  // string? |  (optional) 
            var limit = 56;  // int? |  (optional) 
            var metric = "metric_example";  // string? |  (optional) 
            var offseason = true;  // bool? |  (optional) 
            var offset = 56;  // int? |  (optional) 
            var state = "state_example";  // string? |  (optional) 
            var team = 56;  // int? |  (optional) 
            var year = 56;  // int? |  (optional) 

            try
            {
                // Read Team Years
                Collection<Object> result = apiInstance.ReadTeamYearsV2TeamYearsGet(ascending, country, district, limit, metric, offseason, offset, state, team, year);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling V2TeamYearApi.ReadTeamYearsV2TeamYearsGet: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the ReadTeamYearsV2TeamYearsGetWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    // Read Team Years
    ApiResponse<Collection<Object>> response = apiInstance.ReadTeamYearsV2TeamYearsGetWithHttpInfo(ascending, country, district, limit, metric, offseason, offset, state, team, year);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling V2TeamYearApi.ReadTeamYearsV2TeamYearsGetWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **ascending** | **bool?** |  | [optional]  |
| **country** | **string?** |  | [optional]  |
| **district** | **string?** |  | [optional]  |
| **limit** | **int?** |  | [optional]  |
| **metric** | **string?** |  | [optional]  |
| **offseason** | **bool?** |  | [optional]  |
| **offset** | **int?** |  | [optional]  |
| **state** | **string?** |  | [optional]  |
| **team** | **int?** |  | [optional]  |
| **year** | **int?** |  | [optional]  |

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
| **200** | A list of TeamYear objects. See /team_year/{team}/{year} for more information. |  -  |
| **422** | Validation Error |  -  |

[[Back to top]](#) [[Back to API list]](../../README.md#documentation-for-api-endpoints) [[Back to Model list]](../../README.md#documentation-for-models) [[Back to README]](../../README.md)

