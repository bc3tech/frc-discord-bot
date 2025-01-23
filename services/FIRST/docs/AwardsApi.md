# FIRST.Api.AwardsApi

All URIs are relative to *https://frc-api.firstinspires.org*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**SeasonAwardsEventEventCodeGet**](AwardsApi.md#seasonawardseventeventcodeget) | **GET** /{season}/awards/event/{eventCode} | Event Awards |
| [**SeasonAwardsEventteamEventCodeTeamNumberGet**](AwardsApi.md#seasonawardseventteameventcodeteamnumberget) | **GET** /{season}/awards/eventteam/{eventCode}/{teamNumber} | Event Team Awards |
| [**SeasonAwardsListGet**](AwardsApi.md#seasonawardslistget) | **GET** /{season}/awards/list | Awards Listings |
| [**SeasonAwardsTeamTeamNumberGet**](AwardsApi.md#seasonawardsteamteamnumberget) | **GET** /{season}/awards/team/{teamNumber} | Team Awards |

<a id="seasonawardseventeventcodeget"></a>
# **SeasonAwardsEventEventCodeGet**
> Object SeasonAwardsEventEventCodeGet (string eventCode, string season, string? ifModifiedSince = null)

Event Awards

The event awards API returns details about awards presented at a particular event in a particular season. Return values may contain either `teamNumber` or `person` values, and if the winner was a `person`, and that person is from a team, the `teamNumber` value might be set with their `teamNumber`.

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using FIRST.Api;
using FIRST.Client;
using FIRST.Model;

namespace Example


    public class SeasonAwardsEventEventCodeGetExample
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
            var apiInstance = new AwardsApi(httpClient, config, httpClientHandler);
            var eventCode = "eventCode_example";  // string | (Required) **(string)** Optional case insensitive alphanumeric eventCode of the event from which the awards are requested. Must be at least 3 characters.
            var season = {{season}};  // string | **[REQUIRED] (int)** Numeric year of the event from which the award listings are requested. Must be 4 digits and greater than or equal to 2015, and less than or equal to the current year.
            var ifModifiedSince = "ifModifiedSince_example";  // string? |  (optional) 

            try
            {
                // Event Awards
                Object result = apiInstance.SeasonAwardsEventEventCodeGet(eventCode, season, ifModifiedSince);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling AwardsApi.SeasonAwardsEventEventCodeGet: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the SeasonAwardsEventEventCodeGetWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    // Event Awards
    ApiResponse<Object> response = apiInstance.SeasonAwardsEventEventCodeGetWithHttpInfo(eventCode, season, ifModifiedSince);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling AwardsApi.SeasonAwardsEventEventCodeGetWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **eventCode** | **string** | (Required) **(string)** Optional case insensitive alphanumeric eventCode of the event from which the awards are requested. Must be at least 3 characters. |  |
| **season** | **string** | **[REQUIRED] (int)** Numeric year of the event from which the award listings are requested. Must be 4 digits and greater than or equal to 2015, and less than or equal to the current year. |  |
| **ifModifiedSince** | **string?** |  | [optional]  |

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

<a id="seasonawardseventteameventcodeteamnumberget"></a>
# **SeasonAwardsEventteamEventCodeTeamNumberGet**
> Object SeasonAwardsEventteamEventCodeTeamNumberGet (string eventCode, string season, string teamNumber)

Event Team Awards

The event team awards API returns details about awards presented at a particular event in a particular season for a particular team. Return values may contain either `teamNumber` or `person` values, and if the winner was a `person`, and that person is from a team, the `teamNumber` value might be set with their `teamNumber`.

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using FIRST.Api;
using FIRST.Client;
using FIRST.Model;

namespace Example


    public class SeasonAwardsEventteamEventCodeTeamNumberGetExample
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
            var apiInstance = new AwardsApi(httpClient, config, httpClientHandler);
            var eventCode = "eventCode_example";  // string | **[Required] (string)** Optional case insensitive alphanumeric eventCode of the event from which the awards are requested. Must be at least 3 characters.
            var season = {{season}};  // string | **[REQUIRED] (int)** Numeric year of the team and event from which the award listings are requested. Must be 4 digits and greater than or equal to 2015, and less than or equal to the current year.
            var teamNumber = "teamNumber_example";  // string | **[REQUIRED] (int)** Numeric teamNumber of the team about which information is requested. Must be 1 to 4 digits.

            try
            {
                // Event Team Awards
                Object result = apiInstance.SeasonAwardsEventteamEventCodeTeamNumberGet(eventCode, season, teamNumber);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling AwardsApi.SeasonAwardsEventteamEventCodeTeamNumberGet: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the SeasonAwardsEventteamEventCodeTeamNumberGetWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    // Event Team Awards
    ApiResponse<Object> response = apiInstance.SeasonAwardsEventteamEventCodeTeamNumberGetWithHttpInfo(eventCode, season, teamNumber);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling AwardsApi.SeasonAwardsEventteamEventCodeTeamNumberGetWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **eventCode** | **string** | **[Required] (string)** Optional case insensitive alphanumeric eventCode of the event from which the awards are requested. Must be at least 3 characters. |  |
| **season** | **string** | **[REQUIRED] (int)** Numeric year of the team and event from which the award listings are requested. Must be 4 digits and greater than or equal to 2015, and less than or equal to the current year. |  |
| **teamNumber** | **string** | **[REQUIRED] (int)** Numeric teamNumber of the team about which information is requested. Must be 1 to 4 digits. |  |

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
| **200** | OK |  * Transfer-Encoding -  <br>  * Content-Type -  <br>  * Last-Modified -  <br>  * Server -  <br>  * Date -  <br>  |
| **0** | Unknown |  -  |

[[Back to top]](#) [[Back to API list]](../../README.md#documentation-for-api-endpoints) [[Back to Model list]](../../README.md#documentation-for-models) [[Back to README]](../../README.md)

<a id="seasonawardslistget"></a>
# **SeasonAwardsListGet**
> Object SeasonAwardsListGet (string season, string? ifModifiedSince = null)

Awards Listings

The award listings API returns a listing of the various awards that can be distributed in the requested season. This is especially useful in order to avoid having to use the `name` field of the event awards API to know which award was won. Instead the `awardId` field can be matched between the two APIs.

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using FIRST.Api;
using FIRST.Client;
using FIRST.Model;

namespace Example


    public class SeasonAwardsListGetExample
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
            var apiInstance = new AwardsApi(httpClient, config, httpClientHandler);
            var season = {{season}};  // string | **[REQUIRED] (int)** Numeric year of the event from which the award listings are requested. Must be 4 digits and greater than or equal to 2015, and less than or equal to the current year.
            var ifModifiedSince = "ifModifiedSince_example";  // string? |  (optional) 

            try
            {
                // Awards Listings
                Object result = apiInstance.SeasonAwardsListGet(season, ifModifiedSince);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling AwardsApi.SeasonAwardsListGet: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the SeasonAwardsListGetWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    // Awards Listings
    ApiResponse<Object> response = apiInstance.SeasonAwardsListGetWithHttpInfo(season, ifModifiedSince);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling AwardsApi.SeasonAwardsListGetWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **season** | **string** | **[REQUIRED] (int)** Numeric year of the event from which the award listings are requested. Must be 4 digits and greater than or equal to 2015, and less than or equal to the current year. |  |
| **ifModifiedSince** | **string?** |  | [optional]  |

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

<a id="seasonawardsteamteamnumberget"></a>
# **SeasonAwardsTeamTeamNumberGet**
> Object SeasonAwardsTeamTeamNumberGet (string season, string teamNumber)

Team Awards

The team awards API returns details about awards presented for a particular team in a particular season. Return values may contain either `teamNumber` or `person` values, and if the winner was a `person`, and that person is from a team, the `teamNumber` value might be set with their `teamNumber`.

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using FIRST.Api;
using FIRST.Client;
using FIRST.Model;

namespace Example


    public class SeasonAwardsTeamTeamNumberGetExample
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
            var apiInstance = new AwardsApi(httpClient, config, httpClientHandler);
            var season = {{season}};  // string | **[REQUIRED] (int)** Numeric year of the team from which the award listings are requested. Must be 4 digits and greater than or equal to 2015, and less than or equal to the current year.
            var teamNumber = "teamNumber_example";  // string | **[REQUIRED] (int)** Numeric teamNumber of the team about which information is requested. Must be 1 to 4 digits.

            try
            {
                // Team Awards
                Object result = apiInstance.SeasonAwardsTeamTeamNumberGet(season, teamNumber);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling AwardsApi.SeasonAwardsTeamTeamNumberGet: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the SeasonAwardsTeamTeamNumberGetWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    // Team Awards
    ApiResponse<Object> response = apiInstance.SeasonAwardsTeamTeamNumberGetWithHttpInfo(season, teamNumber);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling AwardsApi.SeasonAwardsTeamTeamNumberGetWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **season** | **string** | **[REQUIRED] (int)** Numeric year of the team from which the award listings are requested. Must be 4 digits and greater than or equal to 2015, and less than or equal to the current year. |  |
| **teamNumber** | **string** | **[REQUIRED] (int)** Numeric teamNumber of the team about which information is requested. Must be 1 to 4 digits. |  |

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
| **200** | OK |  * Transfer-Encoding -  <br>  * Content-Type -  <br>  * Last-Modified -  <br>  * Server -  <br>  * Date -  <br>  |
| **0** | Unknown |  -  |

[[Back to top]](#) [[Back to API list]](../../README.md#documentation-for-api-endpoints) [[Back to Model list]](../../README.md#documentation-for-models) [[Back to README]](../../README.md)

