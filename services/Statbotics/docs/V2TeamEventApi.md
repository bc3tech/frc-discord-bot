# Statbotics.Api.V2TeamEventApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**ReadTeamEventV2TeamEventTeamEventGet**](V2TeamEventApi.md#readteameventv2teameventteameventget) | **GET** /v2/team_event/{team}/{event} | Read Team Event |
| [**ReadTeamEventsEventV2TeamEventsEventEventGet**](V2TeamEventApi.md#readteameventseventv2teameventseventeventget) | **GET** /v2/team_events/event/{event} | Read Team Events Event |
| [**ReadTeamEventsTeamV2TeamEventsTeamTeamGet**](V2TeamEventApi.md#readteameventsteamv2teameventsteamteamget) | **GET** /v2/team_events/team/{team} | Read Team Events Team |
| [**ReadTeamEventsTeamYearV2TeamEventsTeamTeamYearYearGet**](V2TeamEventApi.md#readteameventsteamyearv2teameventsteamteamyearyearget) | **GET** /v2/team_events/team/{team}/year/{year} | Read Team Events Team Year |
| [**ReadTeamEventsV2TeamEventsGet**](V2TeamEventApi.md#readteameventsv2teameventsget) | **GET** /v2/team_events | Read Team Events |
| [**ReadTeamEventsYearDistrictV2TeamEventsYearYearDistrictDistrictGet**](V2TeamEventApi.md#readteameventsyeardistrictv2teameventsyearyeardistrictdistrictget) | **GET** /v2/team_events/year/{year}/district/{district} | Read Team Events Year District |
| [**ReadTeamEventsYearStateV2TeamEventsYearYearStateStateGet**](V2TeamEventApi.md#readteameventsyearstatev2teameventsyearyearstatestateget) | **GET** /v2/team_events/year/{year}/state/{state} | Read Team Events Year State |

<a id="readteameventv2teameventteameventget"></a>
# **ReadTeamEventV2TeamEventTeamEventGet**
> Object ReadTeamEventV2TeamEventTeamEventGet (int team, string varEvent)

Read Team Event

Get a single Team Event object containing event metadata, EPA statistics, and winrate. Specify team number and event key ex: 5511, 2019ncwak

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using Statbotics.Api;
using Statbotics.Client;
using Statbotics.Model;

namespace Example


    public class ReadTeamEventV2TeamEventTeamEventGetExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // create instances of HttpClient, HttpClientHandler to be reused later with different Api classes
            HttpClient httpClient = new HttpClient();
            HttpClientHandler httpClientHandler = new HttpClientHandler();
            var apiInstance = new V2TeamEventApi(httpClient, config, httpClientHandler);
            var team = 56;  // int | 
            var varEvent = "varEvent_example";  // string | 

            try
            {
                // Read Team Event
                Object result = apiInstance.ReadTeamEventV2TeamEventTeamEventGet(team, varEvent);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling V2TeamEventApi.ReadTeamEventV2TeamEventTeamEventGet: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the ReadTeamEventV2TeamEventTeamEventGetWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    // Read Team Event
    ApiResponse<Object> response = apiInstance.ReadTeamEventV2TeamEventTeamEventGetWithHttpInfo(team, varEvent);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling V2TeamEventApi.ReadTeamEventV2TeamEventTeamEventGetWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **team** | **int** |  |  |
| **varEvent** | **string** |  |  |

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
| **200** | A Team Event object. |  -  |
| **422** | Validation Error |  -  |

[[Back to top]](#) [[Back to API list]](../../README.md#documentation-for-api-endpoints) [[Back to Model list]](../../README.md#documentation-for-models) [[Back to README]](../../README.md)

<a id="readteameventseventv2teameventseventeventget"></a>
# **ReadTeamEventsEventV2TeamEventsEventEventGet**
> Collection&lt;Object&gt; ReadTeamEventsEventV2TeamEventsEventEventGet (string varEvent)

Read Team Events Event

Get a list of Team Event objects for a single event. Specify event key, ex: 2019ncwak

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using Statbotics.Api;
using Statbotics.Client;
using Statbotics.Model;

namespace Example


    public class ReadTeamEventsEventV2TeamEventsEventEventGetExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // create instances of HttpClient, HttpClientHandler to be reused later with different Api classes
            HttpClient httpClient = new HttpClient();
            HttpClientHandler httpClientHandler = new HttpClientHandler();
            var apiInstance = new V2TeamEventApi(httpClient, config, httpClientHandler);
            var varEvent = "varEvent_example";  // string | 

            try
            {
                // Read Team Events Event
                Collection<Object> result = apiInstance.ReadTeamEventsEventV2TeamEventsEventEventGet(varEvent);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling V2TeamEventApi.ReadTeamEventsEventV2TeamEventsEventEventGet: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the ReadTeamEventsEventV2TeamEventsEventEventGetWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    // Read Team Events Event
    ApiResponse<Collection<Object>> response = apiInstance.ReadTeamEventsEventV2TeamEventsEventEventGetWithHttpInfo(varEvent);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling V2TeamEventApi.ReadTeamEventsEventV2TeamEventsEventEventGetWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **varEvent** | **string** |  |  |

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
| **200** | A list of Team Event objects. See /team_event/{team}/{event} for more information. |  -  |
| **422** | Validation Error |  -  |

[[Back to top]](#) [[Back to API list]](../../README.md#documentation-for-api-endpoints) [[Back to Model list]](../../README.md#documentation-for-models) [[Back to README]](../../README.md)

<a id="readteameventsteamv2teameventsteamteamget"></a>
# **ReadTeamEventsTeamV2TeamEventsTeamTeamGet**
> Collection&lt;Object&gt; ReadTeamEventsTeamV2TeamEventsTeamTeamGet (int team)

Read Team Events Team

Get a list of Team Event objects for a single team. Specify team number, ex: 5511

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using Statbotics.Api;
using Statbotics.Client;
using Statbotics.Model;

namespace Example


    public class ReadTeamEventsTeamV2TeamEventsTeamTeamGetExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // create instances of HttpClient, HttpClientHandler to be reused later with different Api classes
            HttpClient httpClient = new HttpClient();
            HttpClientHandler httpClientHandler = new HttpClientHandler();
            var apiInstance = new V2TeamEventApi(httpClient, config, httpClientHandler);
            var team = 56;  // int | 

            try
            {
                // Read Team Events Team
                Collection<Object> result = apiInstance.ReadTeamEventsTeamV2TeamEventsTeamTeamGet(team);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling V2TeamEventApi.ReadTeamEventsTeamV2TeamEventsTeamTeamGet: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the ReadTeamEventsTeamV2TeamEventsTeamTeamGetWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    // Read Team Events Team
    ApiResponse<Collection<Object>> response = apiInstance.ReadTeamEventsTeamV2TeamEventsTeamTeamGetWithHttpInfo(team);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling V2TeamEventApi.ReadTeamEventsTeamV2TeamEventsTeamTeamGetWithHttpInfo: " + e.Message);
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
| **200** | A list of Team Event objects. See /team_event/{team}/{event} for more information. |  -  |
| **422** | Validation Error |  -  |

[[Back to top]](#) [[Back to API list]](../../README.md#documentation-for-api-endpoints) [[Back to Model list]](../../README.md#documentation-for-models) [[Back to README]](../../README.md)

<a id="readteameventsteamyearv2teameventsteamteamyearyearget"></a>
# **ReadTeamEventsTeamYearV2TeamEventsTeamTeamYearYearGet**
> Collection&lt;Object&gt; ReadTeamEventsTeamYearV2TeamEventsTeamTeamYearYearGet (int team, int year)

Read Team Events Team Year

Get a list of Team Event objects for a single team and year. Specify team number and year, ex: 5511, 2019

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using Statbotics.Api;
using Statbotics.Client;
using Statbotics.Model;

namespace Example


    public class ReadTeamEventsTeamYearV2TeamEventsTeamTeamYearYearGetExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // create instances of HttpClient, HttpClientHandler to be reused later with different Api classes
            HttpClient httpClient = new HttpClient();
            HttpClientHandler httpClientHandler = new HttpClientHandler();
            var apiInstance = new V2TeamEventApi(httpClient, config, httpClientHandler);
            var team = 56;  // int | 
            var year = 56;  // int | 

            try
            {
                // Read Team Events Team Year
                Collection<Object> result = apiInstance.ReadTeamEventsTeamYearV2TeamEventsTeamTeamYearYearGet(team, year);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling V2TeamEventApi.ReadTeamEventsTeamYearV2TeamEventsTeamTeamYearYearGet: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the ReadTeamEventsTeamYearV2TeamEventsTeamTeamYearYearGetWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    // Read Team Events Team Year
    ApiResponse<Collection<Object>> response = apiInstance.ReadTeamEventsTeamYearV2TeamEventsTeamTeamYearYearGetWithHttpInfo(team, year);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling V2TeamEventApi.ReadTeamEventsTeamYearV2TeamEventsTeamTeamYearYearGetWithHttpInfo: " + e.Message);
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

**Collection<Object>**

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | A list of Team Event objects. See /team_event/{team}/{event} for more information. |  -  |
| **422** | Validation Error |  -  |

[[Back to top]](#) [[Back to API list]](../../README.md#documentation-for-api-endpoints) [[Back to Model list]](../../README.md#documentation-for-models) [[Back to README]](../../README.md)

<a id="readteameventsv2teameventsget"></a>
# **ReadTeamEventsV2TeamEventsGet**
> Collection&lt;Object&gt; ReadTeamEventsV2TeamEventsGet (bool? ascending = null, string? country = null, string? district = null, int? limit = null, string? metric = null, bool? offseason = null, int? offset = null, string? state = null, int? team = null, int? type = null, string? varEvent = null, int? week = null, int? year = null)

Read Team Events

Get a list of all Team Event objects with optional filters.

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using Statbotics.Api;
using Statbotics.Client;
using Statbotics.Model;

namespace Example


    public class ReadTeamEventsV2TeamEventsGetExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // create instances of HttpClient, HttpClientHandler to be reused later with different Api classes
            HttpClient httpClient = new HttpClient();
            HttpClientHandler httpClientHandler = new HttpClientHandler();
            var apiInstance = new V2TeamEventApi(httpClient, config, httpClientHandler);
            var ascending = true;  // bool? |  (optional) 
            var country = "country_example";  // string? |  (optional) 
            var district = "district_example";  // string? |  (optional) 
            var limit = 56;  // int? |  (optional) 
            var metric = "metric_example";  // string? |  (optional) 
            var offseason = true;  // bool? |  (optional) 
            var offset = 56;  // int? |  (optional) 
            var state = "state_example";  // string? |  (optional) 
            var team = 56;  // int? |  (optional) 
            var type = 56;  // int? |  (optional) 
            var varEvent = "varEvent_example";  // string? |  (optional) 
            var week = 56;  // int? |  (optional) 
            var year = 56;  // int? |  (optional) 

            try
            {
                // Read Team Events
                Collection<Object> result = apiInstance.ReadTeamEventsV2TeamEventsGet(ascending, country, district, limit, metric, offseason, offset, state, team, type, varEvent, week, year);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling V2TeamEventApi.ReadTeamEventsV2TeamEventsGet: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the ReadTeamEventsV2TeamEventsGetWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    // Read Team Events
    ApiResponse<Collection<Object>> response = apiInstance.ReadTeamEventsV2TeamEventsGetWithHttpInfo(ascending, country, district, limit, metric, offseason, offset, state, team, type, varEvent, week, year);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling V2TeamEventApi.ReadTeamEventsV2TeamEventsGetWithHttpInfo: " + e.Message);
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
| **type** | **int?** |  | [optional]  |
| **varEvent** | **string?** |  | [optional]  |
| **week** | **int?** |  | [optional]  |
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
| **200** | A list of Team Event objects. See /team_event/{team}/{event} for more information. |  -  |
| **422** | Validation Error |  -  |

[[Back to top]](#) [[Back to API list]](../../README.md#documentation-for-api-endpoints) [[Back to Model list]](../../README.md#documentation-for-models) [[Back to README]](../../README.md)

<a id="readteameventsyeardistrictv2teameventsyearyeardistrictdistrictget"></a>
# **ReadTeamEventsYearDistrictV2TeamEventsYearYearDistrictDistrictGet**
> Collection&lt;Object&gt; ReadTeamEventsYearDistrictV2TeamEventsYearYearDistrictDistrictGet (string district, int year)

Read Team Events Year District

Get a list of Team Event objects for a single year and district. Specify year and district, ex: 2019, fnc

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using Statbotics.Api;
using Statbotics.Client;
using Statbotics.Model;

namespace Example


    public class ReadTeamEventsYearDistrictV2TeamEventsYearYearDistrictDistrictGetExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // create instances of HttpClient, HttpClientHandler to be reused later with different Api classes
            HttpClient httpClient = new HttpClient();
            HttpClientHandler httpClientHandler = new HttpClientHandler();
            var apiInstance = new V2TeamEventApi(httpClient, config, httpClientHandler);
            var district = "district_example";  // string | 
            var year = 56;  // int | 

            try
            {
                // Read Team Events Year District
                Collection<Object> result = apiInstance.ReadTeamEventsYearDistrictV2TeamEventsYearYearDistrictDistrictGet(district, year);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling V2TeamEventApi.ReadTeamEventsYearDistrictV2TeamEventsYearYearDistrictDistrictGet: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the ReadTeamEventsYearDistrictV2TeamEventsYearYearDistrictDistrictGetWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    // Read Team Events Year District
    ApiResponse<Collection<Object>> response = apiInstance.ReadTeamEventsYearDistrictV2TeamEventsYearYearDistrictDistrictGetWithHttpInfo(district, year);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling V2TeamEventApi.ReadTeamEventsYearDistrictV2TeamEventsYearYearDistrictDistrictGetWithHttpInfo: " + e.Message);
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
| **200** | A list of Team Event objects. See /team_event/{team}/{event} for more information. |  -  |
| **422** | Validation Error |  -  |

[[Back to top]](#) [[Back to API list]](../../README.md#documentation-for-api-endpoints) [[Back to Model list]](../../README.md#documentation-for-models) [[Back to README]](../../README.md)

<a id="readteameventsyearstatev2teameventsyearyearstatestateget"></a>
# **ReadTeamEventsYearStateV2TeamEventsYearYearStateStateGet**
> Collection&lt;Object&gt; ReadTeamEventsYearStateV2TeamEventsYearYearStateStateGet (string state, int year)

Read Team Events Year State

Get a list of Team Event objects for a single year and state. Specify year and state, ex: 2019, NC

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using Statbotics.Api;
using Statbotics.Client;
using Statbotics.Model;

namespace Example


    public class ReadTeamEventsYearStateV2TeamEventsYearYearStateStateGetExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // create instances of HttpClient, HttpClientHandler to be reused later with different Api classes
            HttpClient httpClient = new HttpClient();
            HttpClientHandler httpClientHandler = new HttpClientHandler();
            var apiInstance = new V2TeamEventApi(httpClient, config, httpClientHandler);
            var state = "state_example";  // string | 
            var year = 56;  // int | 

            try
            {
                // Read Team Events Year State
                Collection<Object> result = apiInstance.ReadTeamEventsYearStateV2TeamEventsYearYearStateStateGet(state, year);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling V2TeamEventApi.ReadTeamEventsYearStateV2TeamEventsYearYearStateStateGet: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the ReadTeamEventsYearStateV2TeamEventsYearYearStateStateGetWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    // Read Team Events Year State
    ApiResponse<Collection<Object>> response = apiInstance.ReadTeamEventsYearStateV2TeamEventsYearYearStateStateGetWithHttpInfo(state, year);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling V2TeamEventApi.ReadTeamEventsYearStateV2TeamEventsYearYearStateStateGetWithHttpInfo: " + e.Message);
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
| **200** | A list of Team Event objects. See /team_event/{team}/{event} for more information. |  -  |
| **422** | Validation Error |  -  |

[[Back to top]](#) [[Back to API list]](../../README.md#documentation-for-api-endpoints) [[Back to Model list]](../../README.md#documentation-for-models) [[Back to README]](../../README.md)

