# FIRST.Api.RankingsApi

All URIs are relative to *https://frc-api.firstinspires.org*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**SeasonRankingsDistrictGet**](RankingsApi.md#seasonrankingsdistrictget) | **GET** /{season}/rankings/district | District Rankings |
| [**SeasonRankingsEventCodeGet**](RankingsApi.md#seasonrankingseventcodeget) | **GET** /{season}/rankings/{eventCode} | Event Rankings |

<a id="seasonrankingsdistrictget"></a>
# **SeasonRankingsDistrictGet**
> Object SeasonRankingsDistrictGet (string season, string? districtCode = null, string? ifModifiedSince = null, string? page = null, string? teamNumber = null, string? top = null)

District Rankings

The district rankings API returns team ranking detail from a particular team in a particular season. You *must* specify a districtCode unless a `teamNumber` is being specified. If a `teamNumber` is specified, do not include any other paramaters. Optionally, the `top` parameter can be added to the query string to request a subset of the rankings based on the highest ranked teams at the time of the request. Alternately, you can specify the `teamNumber` parameter to retrieve the ranking on one specific team. You cannot specify both a `top` and `teamNumber` in the same call. If you specify a `page`, you cannot specify a `top`.  This endpoint is only updated periodically, and may not reflect final rankings for an event/district until a period of time after a given event is completed. The final authority on teams advancing tournament levels is the District Ranking website and communications from *FIRST*, not this API. *See the FRC Game Manual for more information.*

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using FIRST.Api;
using FIRST.Client;
using FIRST.Model;

namespace Example


    public class SeasonRankingsDistrictGetExample
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
            var apiInstance = new RankingsApi(httpClient, config, httpClientHandler);
            var season = {{season}};  // string | **[REQUIRED] (int)** Numeric year of the event from which the award listings are requested. Must be 4 digits and greater than or equal to 2015, and less than or equal to the current year.
            var districtCode = "districtCode_example";  // string? | **(string)** Case insensitive alphanumeric districtCode of the district from which the rankings are requested. Must be at least 2 characters.  District Codes: ``` FMA PNW NE FIN FNC ONT ISR CHS FIT PCH FIM ``` (optional) 
            var ifModifiedSince = "ifModifiedSince_example";  // string? |  (optional) 
            var page = "page_example";  // string? | **(int)** Numeric page of results to return. If not included, page 1 will be returned. (optional) 
            var teamNumber = "teamNumber_example";  // string? | **(int)** Optional team number of the team whose ranking is requested. (optional) 
            var top = "top_example";  // string? | **(int)** Optional number of requested top ranked teams to return in result. (optional) 

            try
            {
                // District Rankings
                Object result = apiInstance.SeasonRankingsDistrictGet(season, districtCode, ifModifiedSince, page, teamNumber, top);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling RankingsApi.SeasonRankingsDistrictGet: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the SeasonRankingsDistrictGetWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    // District Rankings
    ApiResponse<Object> response = apiInstance.SeasonRankingsDistrictGetWithHttpInfo(season, districtCode, ifModifiedSince, page, teamNumber, top);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling RankingsApi.SeasonRankingsDistrictGetWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **season** | **string** | **[REQUIRED] (int)** Numeric year of the event from which the award listings are requested. Must be 4 digits and greater than or equal to 2015, and less than or equal to the current year. |  |
| **districtCode** | **string?** | **(string)** Case insensitive alphanumeric districtCode of the district from which the rankings are requested. Must be at least 2 characters.  District Codes: &#x60;&#x60;&#x60; FMA PNW NE FIN FNC ONT ISR CHS FIT PCH FIM &#x60;&#x60;&#x60; | [optional]  |
| **ifModifiedSince** | **string?** |  | [optional]  |
| **page** | **string?** | **(int)** Numeric page of results to return. If not included, page 1 will be returned. | [optional]  |
| **teamNumber** | **string?** | **(int)** Optional team number of the team whose ranking is requested. | [optional]  |
| **top** | **string?** | **(int)** Optional number of requested top ranked teams to return in result. | [optional]  |

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

<a id="seasonrankingseventcodeget"></a>
# **SeasonRankingsEventCodeGet**
> Object SeasonRankingsEventCodeGet (string eventCode, string season, string? ifModifiedSince = null, string? teamNumber = null, string? top = null)

Event Rankings

The rankings API returns team ranking detail from a particular event in a particular season. Optionally, the `top` parameter can be added to the query string to request a subset of the rankings based on the highest ranked teams at the time of the request. Alternately, you can specify the `teamNumber` parameter to retrieve the ranking on one specific team. You cannot specify both a top and `teamNumber` in the same call.  **IMPORTANT: This endpoint use to return differently for 2015 vs other seasons. In the fall 2016 updates, this was changed, and all seasons of data now return in the genericized format specified below.**  In all response details, the \"team\" refers to the FRC Team that the ranking represents, as well as their various alliance partners in the matches they have played (i.e. scores in a single match are not calcualted by team, but by alliance). *See the FRC Game Manual for more information.*

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using FIRST.Api;
using FIRST.Client;
using FIRST.Model;

namespace Example


    public class SeasonRankingsEventCodeGetExample
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
            var apiInstance = new RankingsApi(httpClient, config, httpClientHandler);
            var eventCode = "eventCode_example";  // string | (Required) **[REQUIRED] (string)** Case insensitive alphanumeric eventCode of the event from which the rankings are requested. Must be at least 3 characters.
            var season = {{season}};  // string | **[REQUIRED] (int)** Numeric year of the event from which the award listings are requested. Must be 4 digits and greater than or equal to 2015, and less than or equal to the current year.
            var ifModifiedSince = "ifModifiedSince_example";  // string? |  (optional) 
            var teamNumber = "teamNumber_example";  // string? | **(int)** Optional team number of the team whose ranking is requested. (optional) 
            var top = "top_example";  // string? | **(int)** Optional number of requested top ranked teams to return in result. (optional) 

            try
            {
                // Event Rankings
                Object result = apiInstance.SeasonRankingsEventCodeGet(eventCode, season, ifModifiedSince, teamNumber, top);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling RankingsApi.SeasonRankingsEventCodeGet: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the SeasonRankingsEventCodeGetWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    // Event Rankings
    ApiResponse<Object> response = apiInstance.SeasonRankingsEventCodeGetWithHttpInfo(eventCode, season, ifModifiedSince, teamNumber, top);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling RankingsApi.SeasonRankingsEventCodeGetWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **eventCode** | **string** | (Required) **[REQUIRED] (string)** Case insensitive alphanumeric eventCode of the event from which the rankings are requested. Must be at least 3 characters. |  |
| **season** | **string** | **[REQUIRED] (int)** Numeric year of the event from which the award listings are requested. Must be 4 digits and greater than or equal to 2015, and less than or equal to the current year. |  |
| **ifModifiedSince** | **string?** |  | [optional]  |
| **teamNumber** | **string?** | **(int)** Optional team number of the team whose ranking is requested. | [optional]  |
| **top** | **string?** | **(int)** Optional number of requested top ranked teams to return in result. | [optional]  |

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

