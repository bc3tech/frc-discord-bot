# Statbotics.Api.V2EventApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**ReadEventV2EventEventGet**](V2EventApi.md#readeventv2eventeventget) | **GET** /v2/event/{event} | Read Event |
| [**ReadEventsDistrictV2EventsDistrictDistrictGet**](V2EventApi.md#readeventsdistrictv2eventsdistrictdistrictget) | **GET** /v2/events/district/{district} | Read Events District |
| [**ReadEventsStateV2EventsStateStateGet**](V2EventApi.md#readeventsstatev2eventsstatestateget) | **GET** /v2/events/state/{state} | Read Events State |
| [**ReadEventsV2EventsGet**](V2EventApi.md#readeventsv2eventsget) | **GET** /v2/events | Read Events |
| [**ReadEventsYearDistrictV2EventsYearYearDistrictDistrictGet**](V2EventApi.md#readeventsyeardistrictv2eventsyearyeardistrictdistrictget) | **GET** /v2/events/year/{year}/district/{district} | Read Events Year District |
| [**ReadEventsYearStateV2EventsYearYearStateStateGet**](V2EventApi.md#readeventsyearstatev2eventsyearyearstatestateget) | **GET** /v2/events/year/{year}/state/{state} | Read Events Year State |
| [**ReadEventsYearV2EventsYearYearGet**](V2EventApi.md#readeventsyearv2eventsyearyearget) | **GET** /v2/events/year/{year} | Read Events Year |

<a id="readeventv2eventeventget"></a>
# **ReadEventV2EventEventGet**
> Object ReadEventV2EventEventGet (string varEvent)

Read Event

Get a single Event object containing event location, dates, EPA stats, prediction stats. Specify event key ex: 2019ncwak, 2022cmptx

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using Statbotics.Api;
using Statbotics.Client;
using Statbotics.Model;

namespace Example


    public class ReadEventV2EventEventGetExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // create instances of HttpClient, HttpClientHandler to be reused later with different Api classes
            HttpClient httpClient = new HttpClient();
            HttpClientHandler httpClientHandler = new HttpClientHandler();
            var apiInstance = new V2EventApi(httpClient, config, httpClientHandler);
            var varEvent = "varEvent_example";  // string | 

            try
            {
                // Read Event
                Object result = apiInstance.ReadEventV2EventEventGet(varEvent);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling V2EventApi.ReadEventV2EventEventGet: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the ReadEventV2EventEventGetWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    // Read Event
    ApiResponse<Object> response = apiInstance.ReadEventV2EventEventGetWithHttpInfo(varEvent);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling V2EventApi.ReadEventV2EventEventGetWithHttpInfo: " + e.Message);
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
| **200** | An Event object. |  -  |
| **422** | Validation Error |  -  |

[[Back to top]](#) [[Back to API list]](../../README.md#documentation-for-api-endpoints) [[Back to Model list]](../../README.md#documentation-for-models) [[Back to README]](../../README.md)

<a id="readeventsdistrictv2eventsdistrictdistrictget"></a>
# **ReadEventsDistrictV2EventsDistrictDistrictGet**
> Collection&lt;Object&gt; ReadEventsDistrictV2EventsDistrictDistrictGet (string district)

Read Events District

Get a list of Event objects for a single district. Specify district as lowercase abbreviation, ex fnc, fim.

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using Statbotics.Api;
using Statbotics.Client;
using Statbotics.Model;

namespace Example


    public class ReadEventsDistrictV2EventsDistrictDistrictGetExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // create instances of HttpClient, HttpClientHandler to be reused later with different Api classes
            HttpClient httpClient = new HttpClient();
            HttpClientHandler httpClientHandler = new HttpClientHandler();
            var apiInstance = new V2EventApi(httpClient, config, httpClientHandler);
            var district = "district_example";  // string | 

            try
            {
                // Read Events District
                Collection<Object> result = apiInstance.ReadEventsDistrictV2EventsDistrictDistrictGet(district);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling V2EventApi.ReadEventsDistrictV2EventsDistrictDistrictGet: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the ReadEventsDistrictV2EventsDistrictDistrictGetWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    // Read Events District
    ApiResponse<Collection<Object>> response = apiInstance.ReadEventsDistrictV2EventsDistrictDistrictGetWithHttpInfo(district);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling V2EventApi.ReadEventsDistrictV2EventsDistrictDistrictGetWithHttpInfo: " + e.Message);
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
| **200** | A list of Event objects. See /event/{event} for more information. |  -  |
| **422** | Validation Error |  -  |

[[Back to top]](#) [[Back to API list]](../../README.md#documentation-for-api-endpoints) [[Back to Model list]](../../README.md#documentation-for-models) [[Back to README]](../../README.md)

<a id="readeventsstatev2eventsstatestateget"></a>
# **ReadEventsStateV2EventsStateStateGet**
> Collection&lt;Object&gt; ReadEventsStateV2EventsStateStateGet (string state)

Read Events State

Get a list of Event objects for a single state. Specify state as uppercase two-letter abbreviation, ex CA, TX.

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using Statbotics.Api;
using Statbotics.Client;
using Statbotics.Model;

namespace Example


    public class ReadEventsStateV2EventsStateStateGetExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // create instances of HttpClient, HttpClientHandler to be reused later with different Api classes
            HttpClient httpClient = new HttpClient();
            HttpClientHandler httpClientHandler = new HttpClientHandler();
            var apiInstance = new V2EventApi(httpClient, config, httpClientHandler);
            var state = "state_example";  // string | 

            try
            {
                // Read Events State
                Collection<Object> result = apiInstance.ReadEventsStateV2EventsStateStateGet(state);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling V2EventApi.ReadEventsStateV2EventsStateStateGet: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the ReadEventsStateV2EventsStateStateGetWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    // Read Events State
    ApiResponse<Collection<Object>> response = apiInstance.ReadEventsStateV2EventsStateStateGetWithHttpInfo(state);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling V2EventApi.ReadEventsStateV2EventsStateStateGetWithHttpInfo: " + e.Message);
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
| **200** | A list of Event objects. See /event/{event} for more information. |  -  |
| **422** | Validation Error |  -  |

[[Back to top]](#) [[Back to API list]](../../README.md#documentation-for-api-endpoints) [[Back to Model list]](../../README.md#documentation-for-models) [[Back to README]](../../README.md)

<a id="readeventsv2eventsget"></a>
# **ReadEventsV2EventsGet**
> Collection&lt;Object&gt; ReadEventsV2EventsGet (bool? ascending = null, string? country = null, string? district = null, int? limit = null, string? metric = null, bool? offseason = null, int? offset = null, string? state = null, int? type = null, int? week = null, int? year = null)

Read Events

Get a list of all Event objects with optional filters.

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using Statbotics.Api;
using Statbotics.Client;
using Statbotics.Model;

namespace Example


    public class ReadEventsV2EventsGetExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // create instances of HttpClient, HttpClientHandler to be reused later with different Api classes
            HttpClient httpClient = new HttpClient();
            HttpClientHandler httpClientHandler = new HttpClientHandler();
            var apiInstance = new V2EventApi(httpClient, config, httpClientHandler);
            var ascending = true;  // bool? |  (optional) 
            var country = "country_example";  // string? |  (optional) 
            var district = "district_example";  // string? |  (optional) 
            var limit = 56;  // int? |  (optional) 
            var metric = "metric_example";  // string? |  (optional) 
            var offseason = true;  // bool? |  (optional) 
            var offset = 56;  // int? |  (optional) 
            var state = "state_example";  // string? |  (optional) 
            var type = 56;  // int? |  (optional) 
            var week = 56;  // int? |  (optional) 
            var year = 56;  // int? |  (optional) 

            try
            {
                // Read Events
                Collection<Object> result = apiInstance.ReadEventsV2EventsGet(ascending, country, district, limit, metric, offseason, offset, state, type, week, year);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling V2EventApi.ReadEventsV2EventsGet: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the ReadEventsV2EventsGetWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    // Read Events
    ApiResponse<Collection<Object>> response = apiInstance.ReadEventsV2EventsGetWithHttpInfo(ascending, country, district, limit, metric, offseason, offset, state, type, week, year);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling V2EventApi.ReadEventsV2EventsGetWithHttpInfo: " + e.Message);
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
| **type** | **int?** |  | [optional]  |
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
| **200** | A list of Event objects. See /event/{event} for more information. |  -  |
| **422** | Validation Error |  -  |

[[Back to top]](#) [[Back to API list]](../../README.md#documentation-for-api-endpoints) [[Back to Model list]](../../README.md#documentation-for-models) [[Back to README]](../../README.md)

<a id="readeventsyeardistrictv2eventsyearyeardistrictdistrictget"></a>
# **ReadEventsYearDistrictV2EventsYearYearDistrictDistrictGet**
> Collection&lt;Object&gt; ReadEventsYearDistrictV2EventsYearYearDistrictDistrictGet (string district, int year)

Read Events Year District

Get a list of Event objects for a single (year, district) pair. Specify year as four-digit number, district as lowercase abbreviation.

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using Statbotics.Api;
using Statbotics.Client;
using Statbotics.Model;

namespace Example


    public class ReadEventsYearDistrictV2EventsYearYearDistrictDistrictGetExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // create instances of HttpClient, HttpClientHandler to be reused later with different Api classes
            HttpClient httpClient = new HttpClient();
            HttpClientHandler httpClientHandler = new HttpClientHandler();
            var apiInstance = new V2EventApi(httpClient, config, httpClientHandler);
            var district = "district_example";  // string | 
            var year = 56;  // int | 

            try
            {
                // Read Events Year District
                Collection<Object> result = apiInstance.ReadEventsYearDistrictV2EventsYearYearDistrictDistrictGet(district, year);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling V2EventApi.ReadEventsYearDistrictV2EventsYearYearDistrictDistrictGet: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the ReadEventsYearDistrictV2EventsYearYearDistrictDistrictGetWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    // Read Events Year District
    ApiResponse<Collection<Object>> response = apiInstance.ReadEventsYearDistrictV2EventsYearYearDistrictDistrictGetWithHttpInfo(district, year);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling V2EventApi.ReadEventsYearDistrictV2EventsYearYearDistrictDistrictGetWithHttpInfo: " + e.Message);
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
| **200** | A list of Event objects. See /event/{event} for more information. |  -  |
| **422** | Validation Error |  -  |

[[Back to top]](#) [[Back to API list]](../../README.md#documentation-for-api-endpoints) [[Back to Model list]](../../README.md#documentation-for-models) [[Back to README]](../../README.md)

<a id="readeventsyearstatev2eventsyearyearstatestateget"></a>
# **ReadEventsYearStateV2EventsYearYearStateStateGet**
> Collection&lt;Object&gt; ReadEventsYearStateV2EventsYearYearStateStateGet (string state, int year)

Read Events Year State

Get a list of Event objects for a single (year, state) pair. Specify year as four-digit number, state as uppercase two-letter abbreviation.

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using Statbotics.Api;
using Statbotics.Client;
using Statbotics.Model;

namespace Example


    public class ReadEventsYearStateV2EventsYearYearStateStateGetExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // create instances of HttpClient, HttpClientHandler to be reused later with different Api classes
            HttpClient httpClient = new HttpClient();
            HttpClientHandler httpClientHandler = new HttpClientHandler();
            var apiInstance = new V2EventApi(httpClient, config, httpClientHandler);
            var state = "state_example";  // string | 
            var year = 56;  // int | 

            try
            {
                // Read Events Year State
                Collection<Object> result = apiInstance.ReadEventsYearStateV2EventsYearYearStateStateGet(state, year);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling V2EventApi.ReadEventsYearStateV2EventsYearYearStateStateGet: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the ReadEventsYearStateV2EventsYearYearStateStateGetWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    // Read Events Year State
    ApiResponse<Collection<Object>> response = apiInstance.ReadEventsYearStateV2EventsYearYearStateStateGetWithHttpInfo(state, year);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling V2EventApi.ReadEventsYearStateV2EventsYearYearStateStateGetWithHttpInfo: " + e.Message);
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
| **200** | A list of Event objects. See /event/{event} for more information. |  -  |
| **422** | Validation Error |  -  |

[[Back to top]](#) [[Back to API list]](../../README.md#documentation-for-api-endpoints) [[Back to Model list]](../../README.md#documentation-for-models) [[Back to README]](../../README.md)

<a id="readeventsyearv2eventsyearyearget"></a>
# **ReadEventsYearV2EventsYearYearGet**
> Collection&lt;Object&gt; ReadEventsYearV2EventsYearYearGet (int year)

Read Events Year

Get a list of Event objects for a single year. Specify year, ex: 2019, 2020

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using Statbotics.Api;
using Statbotics.Client;
using Statbotics.Model;

namespace Example


    public class ReadEventsYearV2EventsYearYearGetExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // create instances of HttpClient, HttpClientHandler to be reused later with different Api classes
            HttpClient httpClient = new HttpClient();
            HttpClientHandler httpClientHandler = new HttpClientHandler();
            var apiInstance = new V2EventApi(httpClient, config, httpClientHandler);
            var year = 56;  // int | 

            try
            {
                // Read Events Year
                Collection<Object> result = apiInstance.ReadEventsYearV2EventsYearYearGet(year);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling V2EventApi.ReadEventsYearV2EventsYearYearGet: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the ReadEventsYearV2EventsYearYearGetWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    // Read Events Year
    ApiResponse<Collection<Object>> response = apiInstance.ReadEventsYearV2EventsYearYearGetWithHttpInfo(year);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling V2EventApi.ReadEventsYearV2EventsYearYearGetWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
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
| **200** | A list of Event objects. See /event/{event} for more information. |  -  |
| **422** | Validation Error |  -  |

[[Back to top]](#) [[Back to API list]](../../README.md#documentation-for-api-endpoints) [[Back to Model list]](../../README.md#documentation-for-models) [[Back to README]](../../README.md)

