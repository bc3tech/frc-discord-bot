# Statbotics.Api.V2TeamMatchApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**ReadTeamMatchV2TeamMatchTeamMatchGet**](V2TeamMatchApi.md#readteammatchv2teammatchteammatchget) | **GET** /v2/team_match/{team}/{match} | Read Team Match |
| [**ReadTeamMatchesEventV2TeamMatchesEventEventGet**](V2TeamMatchApi.md#readteammatcheseventv2teammatcheseventeventget) | **GET** /v2/team_matches/event/{event} | Read Team Matches Event |
| [**ReadTeamMatchesTeamEventV2TeamMatchesTeamTeamEventEventGet**](V2TeamMatchApi.md#readteammatchesteameventv2teammatchesteamteameventeventget) | **GET** /v2/team_matches/team/{team}/event/{event} | Read Team Matches Team Event |
| [**ReadTeamMatchesTeamYearV2TeamMatchesTeamTeamYearYearGet**](V2TeamMatchApi.md#readteammatchesteamyearv2teammatchesteamteamyearyearget) | **GET** /v2/team_matches/team/{team}/year/{year} | Read Team Matches Team Year |
| [**ReadTeamMatchesV2TeamMatchesGet**](V2TeamMatchApi.md#readteammatchesv2teammatchesget) | **GET** /v2/team_matches | Read Team Matches |

<a id="readteammatchv2teammatchteammatchget"></a>
# **ReadTeamMatchV2TeamMatchTeamMatchGet**
> Object ReadTeamMatchV2TeamMatchTeamMatchGet (string match, int team)

Read Team Match

Get a single Team Match object containing team and EPA predictions. Specify team number and match key ex: 5511, 2019ncwak_f1m1

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using Statbotics.Api;
using Statbotics.Client;
using Statbotics.Model;

namespace Example


    public class ReadTeamMatchV2TeamMatchTeamMatchGetExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // create instances of HttpClient, HttpClientHandler to be reused later with different Api classes
            HttpClient httpClient = new HttpClient();
            HttpClientHandler httpClientHandler = new HttpClientHandler();
            var apiInstance = new V2TeamMatchApi(httpClient, config, httpClientHandler);
            var match = "match_example";  // string | 
            var team = 56;  // int | 

            try
            {
                // Read Team Match
                Object result = apiInstance.ReadTeamMatchV2TeamMatchTeamMatchGet(match, team);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling V2TeamMatchApi.ReadTeamMatchV2TeamMatchTeamMatchGet: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the ReadTeamMatchV2TeamMatchTeamMatchGetWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    // Read Team Match
    ApiResponse<Object> response = apiInstance.ReadTeamMatchV2TeamMatchTeamMatchGetWithHttpInfo(match, team);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling V2TeamMatchApi.ReadTeamMatchV2TeamMatchTeamMatchGetWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **match** | **string** |  |  |
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
| **200** | A Team Match object. |  -  |
| **422** | Validation Error |  -  |

[[Back to top]](#) [[Back to API list]](../../README.md#documentation-for-api-endpoints) [[Back to Model list]](../../README.md#documentation-for-models) [[Back to README]](../../README.md)

<a id="readteammatcheseventv2teammatcheseventeventget"></a>
# **ReadTeamMatchesEventV2TeamMatchesEventEventGet**
> Collection&lt;Object&gt; ReadTeamMatchesEventV2TeamMatchesEventEventGet (string varEvent)

Read Team Matches Event

Get a list of Team Match objects for a single event. Specify event key ex: 2019ncwak

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using Statbotics.Api;
using Statbotics.Client;
using Statbotics.Model;

namespace Example


    public class ReadTeamMatchesEventV2TeamMatchesEventEventGetExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // create instances of HttpClient, HttpClientHandler to be reused later with different Api classes
            HttpClient httpClient = new HttpClient();
            HttpClientHandler httpClientHandler = new HttpClientHandler();
            var apiInstance = new V2TeamMatchApi(httpClient, config, httpClientHandler);
            var varEvent = "varEvent_example";  // string | 

            try
            {
                // Read Team Matches Event
                Collection<Object> result = apiInstance.ReadTeamMatchesEventV2TeamMatchesEventEventGet(varEvent);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling V2TeamMatchApi.ReadTeamMatchesEventV2TeamMatchesEventEventGet: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the ReadTeamMatchesEventV2TeamMatchesEventEventGetWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    // Read Team Matches Event
    ApiResponse<Collection<Object>> response = apiInstance.ReadTeamMatchesEventV2TeamMatchesEventEventGetWithHttpInfo(varEvent);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling V2TeamMatchApi.ReadTeamMatchesEventV2TeamMatchesEventEventGetWithHttpInfo: " + e.Message);
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
| **200** | A list of Team Match objects. See /team_match/{team}/{match} for more information. |  -  |
| **422** | Validation Error |  -  |

[[Back to top]](#) [[Back to API list]](../../README.md#documentation-for-api-endpoints) [[Back to Model list]](../../README.md#documentation-for-models) [[Back to README]](../../README.md)

<a id="readteammatchesteameventv2teammatchesteamteameventeventget"></a>
# **ReadTeamMatchesTeamEventV2TeamMatchesTeamTeamEventEventGet**
> Collection&lt;Object&gt; ReadTeamMatchesTeamEventV2TeamMatchesTeamTeamEventEventGet (int team, string varEvent)

Read Team Matches Team Event

Get a list of Team Match objects for a single team and event. Specify team number and event key ex: 5511, 2019ncwak

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using Statbotics.Api;
using Statbotics.Client;
using Statbotics.Model;

namespace Example


    public class ReadTeamMatchesTeamEventV2TeamMatchesTeamTeamEventEventGetExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // create instances of HttpClient, HttpClientHandler to be reused later with different Api classes
            HttpClient httpClient = new HttpClient();
            HttpClientHandler httpClientHandler = new HttpClientHandler();
            var apiInstance = new V2TeamMatchApi(httpClient, config, httpClientHandler);
            var team = 56;  // int | 
            var varEvent = "varEvent_example";  // string | 

            try
            {
                // Read Team Matches Team Event
                Collection<Object> result = apiInstance.ReadTeamMatchesTeamEventV2TeamMatchesTeamTeamEventEventGet(team, varEvent);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling V2TeamMatchApi.ReadTeamMatchesTeamEventV2TeamMatchesTeamTeamEventEventGet: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the ReadTeamMatchesTeamEventV2TeamMatchesTeamTeamEventEventGetWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    // Read Team Matches Team Event
    ApiResponse<Collection<Object>> response = apiInstance.ReadTeamMatchesTeamEventV2TeamMatchesTeamTeamEventEventGetWithHttpInfo(team, varEvent);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling V2TeamMatchApi.ReadTeamMatchesTeamEventV2TeamMatchesTeamTeamEventEventGetWithHttpInfo: " + e.Message);
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

**Collection<Object>**

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | A list of Team Match objects. See /team_match/{team}/{match} for more information. |  -  |
| **422** | Validation Error |  -  |

[[Back to top]](#) [[Back to API list]](../../README.md#documentation-for-api-endpoints) [[Back to Model list]](../../README.md#documentation-for-models) [[Back to README]](../../README.md)

<a id="readteammatchesteamyearv2teammatchesteamteamyearyearget"></a>
# **ReadTeamMatchesTeamYearV2TeamMatchesTeamTeamYearYearGet**
> Collection&lt;Object&gt; ReadTeamMatchesTeamYearV2TeamMatchesTeamTeamYearYearGet (int team, int year)

Read Team Matches Team Year

Get a list of Team Match objects for a single team and year. Specify team number and year ex: 5511, 2019. Note, includes offseason events.

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using Statbotics.Api;
using Statbotics.Client;
using Statbotics.Model;

namespace Example


    public class ReadTeamMatchesTeamYearV2TeamMatchesTeamTeamYearYearGetExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // create instances of HttpClient, HttpClientHandler to be reused later with different Api classes
            HttpClient httpClient = new HttpClient();
            HttpClientHandler httpClientHandler = new HttpClientHandler();
            var apiInstance = new V2TeamMatchApi(httpClient, config, httpClientHandler);
            var team = 56;  // int | 
            var year = 56;  // int | 

            try
            {
                // Read Team Matches Team Year
                Collection<Object> result = apiInstance.ReadTeamMatchesTeamYearV2TeamMatchesTeamTeamYearYearGet(team, year);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling V2TeamMatchApi.ReadTeamMatchesTeamYearV2TeamMatchesTeamTeamYearYearGet: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the ReadTeamMatchesTeamYearV2TeamMatchesTeamTeamYearYearGetWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    // Read Team Matches Team Year
    ApiResponse<Collection<Object>> response = apiInstance.ReadTeamMatchesTeamYearV2TeamMatchesTeamTeamYearYearGetWithHttpInfo(team, year);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling V2TeamMatchApi.ReadTeamMatchesTeamYearV2TeamMatchesTeamTeamYearYearGetWithHttpInfo: " + e.Message);
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
| **200** | A list of Team Match objects. See /team_match/{team}/{match} for more information. |  -  |
| **422** | Validation Error |  -  |

[[Back to top]](#) [[Back to API list]](../../README.md#documentation-for-api-endpoints) [[Back to Model list]](../../README.md#documentation-for-models) [[Back to README]](../../README.md)

<a id="readteammatchesv2teammatchesget"></a>
# **ReadTeamMatchesV2TeamMatchesGet**
> Collection&lt;Object&gt; ReadTeamMatchesV2TeamMatchesGet (bool? ascending = null, bool? elims = null, int? limit = null, string? match = null, string? metric = null, bool? offseason = null, int? offset = null, int? team = null, string? varEvent = null, int? week = null, int? year = null)

Read Team Matches

Get a list of Team Match objects with optional filters

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using Statbotics.Api;
using Statbotics.Client;
using Statbotics.Model;

namespace Example


    public class ReadTeamMatchesV2TeamMatchesGetExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // create instances of HttpClient, HttpClientHandler to be reused later with different Api classes
            HttpClient httpClient = new HttpClient();
            HttpClientHandler httpClientHandler = new HttpClientHandler();
            var apiInstance = new V2TeamMatchApi(httpClient, config, httpClientHandler);
            var ascending = true;  // bool? |  (optional) 
            var elims = true;  // bool? |  (optional) 
            var limit = 56;  // int? |  (optional) 
            var match = "match_example";  // string? |  (optional) 
            var metric = "metric_example";  // string? |  (optional) 
            var offseason = true;  // bool? |  (optional) 
            var offset = 56;  // int? |  (optional) 
            var team = 56;  // int? |  (optional) 
            var varEvent = "varEvent_example";  // string? |  (optional) 
            var week = 56;  // int? |  (optional) 
            var year = 56;  // int? |  (optional) 

            try
            {
                // Read Team Matches
                Collection<Object> result = apiInstance.ReadTeamMatchesV2TeamMatchesGet(ascending, elims, limit, match, metric, offseason, offset, team, varEvent, week, year);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling V2TeamMatchApi.ReadTeamMatchesV2TeamMatchesGet: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the ReadTeamMatchesV2TeamMatchesGetWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    // Read Team Matches
    ApiResponse<Collection<Object>> response = apiInstance.ReadTeamMatchesV2TeamMatchesGetWithHttpInfo(ascending, elims, limit, match, metric, offseason, offset, team, varEvent, week, year);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling V2TeamMatchApi.ReadTeamMatchesV2TeamMatchesGetWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **ascending** | **bool?** |  | [optional]  |
| **elims** | **bool?** |  | [optional]  |
| **limit** | **int?** |  | [optional]  |
| **match** | **string?** |  | [optional]  |
| **metric** | **string?** |  | [optional]  |
| **offseason** | **bool?** |  | [optional]  |
| **offset** | **int?** |  | [optional]  |
| **team** | **int?** |  | [optional]  |
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
| **200** | A list of Team Match objects. See /team_match/{team}/{match} for more information. |  -  |
| **422** | Validation Error |  -  |

[[Back to top]](#) [[Back to API list]](../../README.md#documentation-for-api-endpoints) [[Back to Model list]](../../README.md#documentation-for-models) [[Back to README]](../../README.md)

