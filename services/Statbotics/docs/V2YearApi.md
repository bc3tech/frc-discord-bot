# Statbotics.Api.V2YearApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**ReadYearV2YearYearGet**](V2YearApi.md#readyearv2yearyearget) | **GET** /v2/year/{year} | Read Year |
| [**ReadYearsV2YearsGet**](V2YearApi.md#readyearsv2yearsget) | **GET** /v2/years | Read Years |

<a id="readyearv2yearyearget"></a>
# **ReadYearV2YearYearGet**
> Object ReadYearV2YearYearGet (int year)

Read Year

Get a single Year object containing EPA percentiles, Week 1 match score statistics, and prediction accuracy. After 2016, separated into components and ranking points included.

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using Statbotics.Api;
using Statbotics.Client;
using Statbotics.Model;

namespace Example


    public class ReadYearV2YearYearGetExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // create instances of HttpClient, HttpClientHandler to be reused later with different Api classes
            HttpClient httpClient = new HttpClient();
            HttpClientHandler httpClientHandler = new HttpClientHandler();
            var apiInstance = new V2YearApi(httpClient, config, httpClientHandler);
            var year = 56;  // int | 

            try
            {
                // Read Year
                Object result = apiInstance.ReadYearV2YearYearGet(year);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling V2YearApi.ReadYearV2YearYearGet: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the ReadYearV2YearYearGetWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    // Read Year
    ApiResponse<Object> response = apiInstance.ReadYearV2YearYearGetWithHttpInfo(year);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling V2YearApi.ReadYearV2YearYearGetWithHttpInfo: " + e.Message);
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
| **200** | A Year object. |  -  |
| **422** | Validation Error |  -  |

[[Back to top]](#) [[Back to API list]](../../README.md#documentation-for-api-endpoints) [[Back to Model list]](../../README.md#documentation-for-models) [[Back to README]](../../README.md)

<a id="readyearsv2yearsget"></a>
# **ReadYearsV2YearsGet**
> Collection&lt;Object&gt; ReadYearsV2YearsGet (bool? ascending = null, int? limit = null, string? metric = null, int? offset = null)

Read Years

Get a list of Year objects from 2002 to 2023. Specify a four-digit year, ex: 2019

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using Statbotics.Api;
using Statbotics.Client;
using Statbotics.Model;

namespace Example


    public class ReadYearsV2YearsGetExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // create instances of HttpClient, HttpClientHandler to be reused later with different Api classes
            HttpClient httpClient = new HttpClient();
            HttpClientHandler httpClientHandler = new HttpClientHandler();
            var apiInstance = new V2YearApi(httpClient, config, httpClientHandler);
            var ascending = true;  // bool? |  (optional) 
            var limit = 56;  // int? |  (optional) 
            var metric = "metric_example";  // string? |  (optional) 
            var offset = 56;  // int? |  (optional) 

            try
            {
                // Read Years
                Collection<Object> result = apiInstance.ReadYearsV2YearsGet(ascending, limit, metric, offset);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling V2YearApi.ReadYearsV2YearsGet: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the ReadYearsV2YearsGetWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    // Read Years
    ApiResponse<Collection<Object>> response = apiInstance.ReadYearsV2YearsGetWithHttpInfo(ascending, limit, metric, offset);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling V2YearApi.ReadYearsV2YearsGetWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **ascending** | **bool?** |  | [optional]  |
| **limit** | **int?** |  | [optional]  |
| **metric** | **string?** |  | [optional]  |
| **offset** | **int?** |  | [optional]  |

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
| **200** | A list of Year objects. See /year/{year} for more information. |  -  |
| **422** | Validation Error |  -  |

[[Back to top]](#) [[Back to API list]](../../README.md#documentation-for-api-endpoints) [[Back to Model list]](../../README.md#documentation-for-models) [[Back to README]](../../README.md)

