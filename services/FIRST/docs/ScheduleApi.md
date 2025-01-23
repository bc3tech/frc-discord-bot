# FIRST.Api.ScheduleApi

All URIs are relative to *https://frc-api.firstinspires.org*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**SeasonScheduleEventCodeGet**](ScheduleApi.md#seasonscheduleeventcodeget) | **GET** /{season}/schedule/{eventCode} | Event Schedule |

<a id="seasonscheduleeventcodeget"></a>
# **SeasonScheduleEventCodeGet**
> Object SeasonScheduleEventCodeGet (string eventCode, string season, string? end = null, string? ifModifiedSince = null, string? start = null, string? teamNumber = null, string? tournamentLevel = null)

Event Schedule

The schedule API returns the match schedule for the desired tournament level of a particular event in a particular season. You must also specify a `tournamentLevel` from which to return the results. Alternately, you can specify a `teamNumber` to filter the results to only those in which a particular team is participating. There is no validation that the `teamNumber` you request is actually competing at the event, if they are not, the response will be empty. You can also specify the parameters together, but cannot make a request without at least one of the two.

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using FIRST.Api;
using FIRST.Client;
using FIRST.Model;

namespace Example


    public class SeasonScheduleEventCodeGetExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "https://frc-api.firstinspires.org";
            // Configure HTTP basic authorization: basicAuth
            config.Username = "YOUR_USERNAME";
            config.Password = "YOUR_PASSWORD";

            // create instances of HttpClient, HttpClientHandler to be reused later with different Api classes
            HttpClient httpClient = new HttpClient();
            HttpClientHandler httpClientHandler = new HttpClientHandler();
            var apiInstance = new ScheduleApi(httpClient, config, httpClientHandler);
            var eventCode = "eventCode_example";  // string | **[REQUIRED] (string)** Case insensitive alphanumeric eventCode of the event from which the schedule are requested. Must be at least 3 characters.
            var season = {{season}};  // string | **[REQUIRED] (int)** Numeric year of the event from which the award listings are requested. Must be 4 digits and greater than or equal to 2015, and less than or equal to the current year.
            var end = "end_example";  // string? | **(int)** Optional end match number for subset of results to return (inclusive). (optional) 
            var ifModifiedSince = "ifModifiedSince_example";  // string? |  (optional) 
            var start = "start_example";  // string? | **(int)** Optional start match number for subset of results to return (inclusive). (optional) 
            var teamNumber = "teamNumber_example";  // string? | **(int)** Optional teamNumber to search for within the schedule. Only returns matches in which the requested team participated. (optional) 
            var tournamentLevel = "tournamentLevel_example";  // string? | **(string)** tournamentLevel of desired match schedule.  Enum values: ``` 1. None 2. Practice 3. Qualification 4. Playoff ``` (optional) 

            try
            {
                // Event Schedule
                Object result = apiInstance.SeasonScheduleEventCodeGet(eventCode, season, end, ifModifiedSince, start, teamNumber, tournamentLevel);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling ScheduleApi.SeasonScheduleEventCodeGet: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the SeasonScheduleEventCodeGetWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    // Event Schedule
    ApiResponse<Object> response = apiInstance.SeasonScheduleEventCodeGetWithHttpInfo(eventCode, season, end, ifModifiedSince, start, teamNumber, tournamentLevel);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling ScheduleApi.SeasonScheduleEventCodeGetWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **eventCode** | **string** | **[REQUIRED] (string)** Case insensitive alphanumeric eventCode of the event from which the schedule are requested. Must be at least 3 characters. |  |
| **season** | **string** | **[REQUIRED] (int)** Numeric year of the event from which the award listings are requested. Must be 4 digits and greater than or equal to 2015, and less than or equal to the current year. |  |
| **end** | **string?** | **(int)** Optional end match number for subset of results to return (inclusive). | [optional]  |
| **ifModifiedSince** | **string?** |  | [optional]  |
| **start** | **string?** | **(int)** Optional start match number for subset of results to return (inclusive). | [optional]  |
| **teamNumber** | **string?** | **(int)** Optional teamNumber to search for within the schedule. Only returns matches in which the requested team participated. | [optional]  |
| **tournamentLevel** | **string?** | **(string)** tournamentLevel of desired match schedule.  Enum values: &#x60;&#x60;&#x60; 1. None 2. Practice 3. Qualification 4. Playoff &#x60;&#x60;&#x60; | [optional]  |

### Return type

**Object**

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | OK |  * Last-Modified -  <br>  * Content-Type -  <br>  |
| **500** | Internal Server Error |  * Content-Type -  <br>  |

[[Back to top]](#) [[Back to API list]](../../README.md#documentation-for-api-endpoints) [[Back to Model list]](../../README.md#documentation-for-models) [[Back to README]](../../README.md)

