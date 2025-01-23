# Statbotics.Api.V2MatchApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**ReadMatchV2MatchMatchGet**](V2MatchApi.md#readmatchv2matchmatchget) | **GET** /v2/match/{match} | Read Match |
| [**ReadMatchesEventV2MatchesEventEventGet**](V2MatchApi.md#readmatcheseventv2matcheseventeventget) | **GET** /v2/matches/event/{event} | Read Matches Event |
| [**ReadMatchesTeamEventV2MatchesTeamTeamEventEventGet**](V2MatchApi.md#readmatchesteameventv2matchesteamteameventeventget) | **GET** /v2/matches/team/{team}/event/{event} | Read Matches Team Event |
| [**ReadMatchesTeamYearV2MatchesTeamTeamYearYearGet**](V2MatchApi.md#readmatchesteamyearv2matchesteamteamyearyearget) | **GET** /v2/matches/team/{team}/year/{year} | Read Matches Team Year |
| [**ReadMatchesV2MatchesGet**](V2MatchApi.md#readmatchesv2matchesget) | **GET** /v2/matches | Read Matches |

<a id="readmatchv2matchmatchget"></a>
# **ReadMatchV2MatchMatchGet**
> Object ReadMatchV2MatchMatchGet (string match)

Read Match

Get a single Match object containing teams, score prediction, and actual results. Specify match key ex: 2019ncwak_f1m1

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using Statbotics.Api;
using Statbotics.Client;
using Statbotics.Model;

namespace Example


    public class ReadMatchV2MatchMatchGetExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // create instances of HttpClient, HttpClientHandler to be reused later with different Api classes
            HttpClient httpClient = new HttpClient();
            HttpClientHandler httpClientHandler = new HttpClientHandler();
            var apiInstance = new V2MatchApi(httpClient, config, httpClientHandler);
            var match = "match_example";  // string | 

            try
            {
                // Read Match
                Object result = apiInstance.ReadMatchV2MatchMatchGet(match);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling V2MatchApi.ReadMatchV2MatchMatchGet: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the ReadMatchV2MatchMatchGetWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    // Read Match
    ApiResponse<Object> response = apiInstance.ReadMatchV2MatchMatchGetWithHttpInfo(match);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling V2MatchApi.ReadMatchV2MatchMatchGetWithHttpInfo: " + e.Message);
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
| **200** | A Match object. |  -  |
| **422** | Validation Error |  -  |

[[Back to top]](#) [[Back to API list]](../../README.md#documentation-for-api-endpoints) [[Back to Model list]](../../README.md#documentation-for-models) [[Back to README]](../../README.md)

<a id="readmatcheseventv2matcheseventeventget"></a>
# **ReadMatchesEventV2MatchesEventEventGet**
> Collection&lt;Object&gt; ReadMatchesEventV2MatchesEventEventGet (string varEvent)

Read Matches Event

Get a list of Match objects for a single event. Specify event key ex: 2019ncwak, 2022cmptx

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using Statbotics.Api;
using Statbotics.Client;
using Statbotics.Model;

namespace Example


    public class ReadMatchesEventV2MatchesEventEventGetExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // create instances of HttpClient, HttpClientHandler to be reused later with different Api classes
            HttpClient httpClient = new HttpClient();
            HttpClientHandler httpClientHandler = new HttpClientHandler();
            var apiInstance = new V2MatchApi(httpClient, config, httpClientHandler);
            var varEvent = "varEvent_example";  // string | 

            try
            {
                // Read Matches Event
                Collection<Object> result = apiInstance.ReadMatchesEventV2MatchesEventEventGet(varEvent);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling V2MatchApi.ReadMatchesEventV2MatchesEventEventGet: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the ReadMatchesEventV2MatchesEventEventGetWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    // Read Matches Event
    ApiResponse<Collection<Object>> response = apiInstance.ReadMatchesEventV2MatchesEventEventGetWithHttpInfo(varEvent);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling V2MatchApi.ReadMatchesEventV2MatchesEventEventGetWithHttpInfo: " + e.Message);
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
| **200** | A list of Match objects. See /match/{match} for more information. |  -  |
| **422** | Validation Error |  -  |

[[Back to top]](#) [[Back to API list]](../../README.md#documentation-for-api-endpoints) [[Back to Model list]](../../README.md#documentation-for-models) [[Back to README]](../../README.md)

<a id="readmatchesteameventv2matchesteamteameventeventget"></a>
# **ReadMatchesTeamEventV2MatchesTeamTeamEventEventGet**
> Collection&lt;Object&gt; ReadMatchesTeamEventV2MatchesTeamTeamEventEventGet (int team, string varEvent)

Read Matches Team Event

Get a list of Match objects for a single team in a single event. Specify team number and event key, ex: 5511, 2019ncwak

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using Statbotics.Api;
using Statbotics.Client;
using Statbotics.Model;

namespace Example


    public class ReadMatchesTeamEventV2MatchesTeamTeamEventEventGetExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // create instances of HttpClient, HttpClientHandler to be reused later with different Api classes
            HttpClient httpClient = new HttpClient();
            HttpClientHandler httpClientHandler = new HttpClientHandler();
            var apiInstance = new V2MatchApi(httpClient, config, httpClientHandler);
            var team = 56;  // int | 
            var varEvent = "varEvent_example";  // string | 

            try
            {
                // Read Matches Team Event
                Collection<Object> result = apiInstance.ReadMatchesTeamEventV2MatchesTeamTeamEventEventGet(team, varEvent);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling V2MatchApi.ReadMatchesTeamEventV2MatchesTeamTeamEventEventGet: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the ReadMatchesTeamEventV2MatchesTeamTeamEventEventGetWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    // Read Matches Team Event
    ApiResponse<Collection<Object>> response = apiInstance.ReadMatchesTeamEventV2MatchesTeamTeamEventEventGetWithHttpInfo(team, varEvent);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling V2MatchApi.ReadMatchesTeamEventV2MatchesTeamTeamEventEventGetWithHttpInfo: " + e.Message);
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
| **200** | A list of Match objects. See /match/{match} for more information. |  -  |
| **422** | Validation Error |  -  |

[[Back to top]](#) [[Back to API list]](../../README.md#documentation-for-api-endpoints) [[Back to Model list]](../../README.md#documentation-for-models) [[Back to README]](../../README.md)

<a id="readmatchesteamyearv2matchesteamteamyearyearget"></a>
# **ReadMatchesTeamYearV2MatchesTeamTeamYearYearGet**
> Collection&lt;Object&gt; ReadMatchesTeamYearV2MatchesTeamTeamYearYearGet (int team, int year)

Read Matches Team Year

Get a list of Match objects for a single team in a single year. Specify team number and year, ex: 254, 2019

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using Statbotics.Api;
using Statbotics.Client;
using Statbotics.Model;

namespace Example


    public class ReadMatchesTeamYearV2MatchesTeamTeamYearYearGetExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // create instances of HttpClient, HttpClientHandler to be reused later with different Api classes
            HttpClient httpClient = new HttpClient();
            HttpClientHandler httpClientHandler = new HttpClientHandler();
            var apiInstance = new V2MatchApi(httpClient, config, httpClientHandler);
            var team = 56;  // int | 
            var year = 56;  // int | 

            try
            {
                // Read Matches Team Year
                Collection<Object> result = apiInstance.ReadMatchesTeamYearV2MatchesTeamTeamYearYearGet(team, year);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling V2MatchApi.ReadMatchesTeamYearV2MatchesTeamTeamYearYearGet: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the ReadMatchesTeamYearV2MatchesTeamTeamYearYearGetWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    // Read Matches Team Year
    ApiResponse<Collection<Object>> response = apiInstance.ReadMatchesTeamYearV2MatchesTeamTeamYearYearGetWithHttpInfo(team, year);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling V2MatchApi.ReadMatchesTeamYearV2MatchesTeamTeamYearYearGetWithHttpInfo: " + e.Message);
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
| **200** | A list of Match objects. See /match/{match} for more information. |  -  |
| **422** | Validation Error |  -  |

[[Back to top]](#) [[Back to API list]](../../README.md#documentation-for-api-endpoints) [[Back to Model list]](../../README.md#documentation-for-models) [[Back to README]](../../README.md)

<a id="readmatchesv2matchesget"></a>
# **ReadMatchesV2MatchesGet**
> Collection&lt;Object&gt; ReadMatchesV2MatchesGet (bool? ascending = null, bool? elims = null, int? limit = null, string? metric = null, bool? offseason = null, int? offset = null, int? team = null, string? varEvent = null, int? week = null, int? year = null)

Read Matches

Get a list of Matches with optional filters

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using Statbotics.Api;
using Statbotics.Client;
using Statbotics.Model;

namespace Example


    public class ReadMatchesV2MatchesGetExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // create instances of HttpClient, HttpClientHandler to be reused later with different Api classes
            HttpClient httpClient = new HttpClient();
            HttpClientHandler httpClientHandler = new HttpClientHandler();
            var apiInstance = new V2MatchApi(httpClient, config, httpClientHandler);
            var ascending = true;  // bool? |  (optional) 
            var elims = true;  // bool? |  (optional) 
            var limit = 56;  // int? |  (optional) 
            var metric = "metric_example";  // string? |  (optional) 
            var offseason = true;  // bool? |  (optional) 
            var offset = 56;  // int? |  (optional) 
            var team = 56;  // int? |  (optional) 
            var varEvent = "varEvent_example";  // string? |  (optional) 
            var week = 56;  // int? |  (optional) 
            var year = 56;  // int? |  (optional) 

            try
            {
                // Read Matches
                Collection<Object> result = apiInstance.ReadMatchesV2MatchesGet(ascending, elims, limit, metric, offseason, offset, team, varEvent, week, year);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling V2MatchApi.ReadMatchesV2MatchesGet: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the ReadMatchesV2MatchesGetWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    // Read Matches
    ApiResponse<Collection<Object>> response = apiInstance.ReadMatchesV2MatchesGetWithHttpInfo(ascending, elims, limit, metric, offseason, offset, team, varEvent, week, year);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling V2MatchApi.ReadMatchesV2MatchesGetWithHttpInfo: " + e.Message);
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
| **200** | A list of Match objects. See /match/{match} for more information. |  -  |
| **422** | Validation Error |  -  |

[[Back to top]](#) [[Back to API list]](../../README.md#documentation-for-api-endpoints) [[Back to Model list]](../../README.md#documentation-for-models) [[Back to README]](../../README.md)

