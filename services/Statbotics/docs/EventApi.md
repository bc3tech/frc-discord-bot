# Statbotics.Api.EventApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**ReadEventV3EventEventGet**](EventApi.md#readeventv3eventeventget) | **GET** /v3/event/{event} | Query a single event |
| [**ReadEventsV3EventsGet**](EventApi.md#readeventsv3eventsget) | **GET** /v3/events | Query multiple events |

<a id="readeventv3eventeventget"></a>
# **ReadEventV3EventEventGet**
> Object ReadEventV3EventEventGet (string varEvent)

Query a single event

Returns a single Event object. Requires an event key, e.g. `2019ncwak`.

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using Statbotics.Api;
using Statbotics.Client;
using Statbotics.Model;

namespace Example


    public class ReadEventV3EventEventGetExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // create instances of HttpClient, HttpClientHandler to be reused later with different Api classes
            HttpClient httpClient = new HttpClient();
            HttpClientHandler httpClientHandler = new HttpClientHandler();
            var apiInstance = new EventApi(httpClient, config, httpClientHandler);
            var varEvent = "varEvent_example";  // string | 

            try
            {
                // Query a single event
                Object result = apiInstance.ReadEventV3EventEventGet(varEvent);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling EventApi.ReadEventV3EventEventGet: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the ReadEventV3EventEventGetWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    // Query a single event
    ApiResponse<Object> response = apiInstance.ReadEventV3EventEventGetWithHttpInfo(varEvent);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling EventApi.ReadEventV3EventEventGetWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
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
| **200** | Successful Response |  -  |
| **422** | Validation Error |  -  |

[[Back to top]](#) [[Back to API list]](../../README.md#documentation-for-api-endpoints) [[Back to Model list]](../../README.md#documentation-for-models) [[Back to README]](../../README.md)

<a id="readeventsv3eventsget"></a>
# **ReadEventsV3EventsGet**
> Collection&lt;Object&gt; ReadEventsV3EventsGet (bool? ascending = null, string? country = null, string? district = null, int? limit = null, string? metric = null, bool? offseason = null, int? offset = null, string? state = null, string? type = null, int? week = null, int? year = null)

Query multiple events

Returns up to 1000 events at a time. Specify limit and offset to page through results.

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using Statbotics.Api;
using Statbotics.Client;
using Statbotics.Model;

namespace Example


    public class ReadEventsV3EventsGetExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // create instances of HttpClient, HttpClientHandler to be reused later with different Api classes
            HttpClient httpClient = new HttpClient();
            HttpClientHandler httpClientHandler = new HttpClientHandler();
            var apiInstance = new EventApi(httpClient, config, httpClientHandler);
            var ascending = true;  // bool? | Whether to sort the returned values in ascending order. Default is ascending (optional) 
            var country = "country_example";  // string? | Capitalized country name, e.g. `USA` or `Canada`. (optional) 
            var district = "district_example";  // string? | One of [`fma`, `fnc`, `fit`, `fin`, `fim`, `ne`, `chs`, `ont`, `pnw`, `pch`, `isr`] (optional) 
            var limit = 56;  // int? | Maximum number of events to return. Default is 1000. (optional) 
            var metric = "metric_example";  // string? | How to sort the returned values. Any column in the table is valid. (optional) 
            var offseason = true;  // bool? | Whether the event is an offseason event. (optional) 
            var offset = 56;  // int? | Offset from the first result to return. (optional) 
            var state = "state_example";  // string? | Capitalized two-letter state code, e.g. `NC`. (optional) 
            var type = "type_example";  // string? | One of [`regional`, `district`, `district_cmp`, `cmp_division`, `cmp_finals`, `offseason`, or `preseason`]. (optional) 
            var week = 56;  // int? | Week of the competition season. 0 is preseason, 8 is CMP, 9 is offseason. (optional) 
            var year = 56;  // int? | Four-digit year (optional) 

            try
            {
                // Query multiple events
                Collection<Object> result = apiInstance.ReadEventsV3EventsGet(ascending, country, district, limit, metric, offseason, offset, state, type, week, year);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling EventApi.ReadEventsV3EventsGet: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the ReadEventsV3EventsGetWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    // Query multiple events
    ApiResponse<Collection<Object>> response = apiInstance.ReadEventsV3EventsGetWithHttpInfo(ascending, country, district, limit, metric, offseason, offset, state, type, week, year);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling EventApi.ReadEventsV3EventsGetWithHttpInfo: " + e.Message);
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
| **type** | **string?** | One of [&#x60;regional&#x60;, &#x60;district&#x60;, &#x60;district_cmp&#x60;, &#x60;cmp_division&#x60;, &#x60;cmp_finals&#x60;, &#x60;offseason&#x60;, or &#x60;preseason&#x60;]. | [optional]  |
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

