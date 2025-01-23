# Statbotics.Api.YearApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**ReadYearV3YearYearGet**](YearApi.md#readyearv3yearyearget) | **GET** /v3/year/{year} | Query a single year |
| [**ReadYearsV3YearsGet**](YearApi.md#readyearsv3yearsget) | **GET** /v3/years | Query multiple years |

<a id="readyearv3yearyearget"></a>
# **ReadYearV3YearYearGet**
> Object ReadYearV3YearYearGet (int year)

Query a single year

Returns a single Year object. Requires a four-digit year, e.g. `2019`.

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using Statbotics.Api;
using Statbotics.Client;
using Statbotics.Model;

namespace Example


    public class ReadYearV3YearYearGetExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // create instances of HttpClient, HttpClientHandler to be reused later with different Api classes
            HttpClient httpClient = new HttpClient();
            HttpClientHandler httpClientHandler = new HttpClientHandler();
            var apiInstance = new YearApi(httpClient, config, httpClientHandler);
            var year = 56;  // int | 

            try
            {
                // Query a single year
                Object result = apiInstance.ReadYearV3YearYearGet(year);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling YearApi.ReadYearV3YearYearGet: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the ReadYearV3YearYearGetWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    // Query a single year
    ApiResponse<Object> response = apiInstance.ReadYearV3YearYearGetWithHttpInfo(year);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling YearApi.ReadYearV3YearYearGetWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
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

<a id="readyearsv3yearsget"></a>
# **ReadYearsV3YearsGet**
> Collection&lt;Object&gt; ReadYearsV3YearsGet (bool? ascending = null, int? limit = null, string? metric = null, int? offset = null)

Query multiple years

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using Statbotics.Api;
using Statbotics.Client;
using Statbotics.Model;

namespace Example


    public class ReadYearsV3YearsGetExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // create instances of HttpClient, HttpClientHandler to be reused later with different Api classes
            HttpClient httpClient = new HttpClient();
            HttpClientHandler httpClientHandler = new HttpClientHandler();
            var apiInstance = new YearApi(httpClient, config, httpClientHandler);
            var ascending = true;  // bool? | Whether to sort the returned values in ascending order. Default is ascending (optional) 
            var limit = 56;  // int? | Maximum number of events to return. Default is 1000. (optional) 
            var metric = "metric_example";  // string? | How to sort the returned values. Any column in the table is valid. (optional) 
            var offset = 56;  // int? | Offset from the first result to return. (optional) 

            try
            {
                // Query multiple years
                Collection<Object> result = apiInstance.ReadYearsV3YearsGet(ascending, limit, metric, offset);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling YearApi.ReadYearsV3YearsGet: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the ReadYearsV3YearsGetWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    // Query multiple years
    ApiResponse<Collection<Object>> response = apiInstance.ReadYearsV3YearsGetWithHttpInfo(ascending, limit, metric, offset);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling YearApi.ReadYearsV3YearsGetWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **ascending** | **bool?** | Whether to sort the returned values in ascending order. Default is ascending | [optional]  |
| **limit** | **int?** | Maximum number of events to return. Default is 1000. | [optional]  |
| **metric** | **string?** | How to sort the returned values. Any column in the table is valid. | [optional]  |
| **offset** | **int?** | Offset from the first result to return. | [optional]  |

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
| **200** | Returns a list of Years since 2002. Older data is not available. |  -  |
| **422** | Validation Error |  -  |

[[Back to top]](#) [[Back to API list]](../../README.md#documentation-for-api-endpoints) [[Back to Model list]](../../README.md#documentation-for-models) [[Back to README]](../../README.md)

