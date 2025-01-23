# FIRST.Api.MatchResultsApi

All URIs are relative to *https://frc-api.firstinspires.org*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**SeasonMatchesEventCodeGet**](MatchResultsApi.md#seasonmatcheseventcodeget) | **GET** /{season}/matches/{eventCode} | Event Match Results |
| [**SeasonScoresEventCodeTournamentLevelGet**](MatchResultsApi.md#seasonscoreseventcodetournamentlevelget) | **GET** /{season}/scores/{eventCode}/{tournamentLevel} | Score Details |

<a id="seasonmatcheseventcodeget"></a>
# **SeasonMatchesEventCodeGet**
> Object SeasonMatchesEventCodeGet (string eventCode, string season, string? end = null, string? ifModifiedSince = null, string? matchNumber = null, string? start = null, string? teamNumber = null, string? tournamentLevel = null)

Event Match Results

The match results API returns the match results for all matches of a particular event in a particular season. Match results are only available once a match has been played, retrieving info about future matches requires the event schedule API. You cannot receive data about a match that is in progress.  If you specify the `matchNumber`, `start` and/or `end` optional parameters, you must also specify a `tournamentLevel`. If you specify the `teamNumber` parameter, you cannot specify a `matchNumber` parameter. If you specify the matchNumber, you cannot define a `start` or `end`.  **Note**: If you specify `start`, and it is higher than the maximum match number at the event, you will not receive any match results in the response. The same is true in reverse for the end parameter.  Starting in the 2015 season, Elimination matches were renamed to Playoff matches. As such, you must request Playoff matches from the API, and \"elim\" will not return any results. In Playoffs, match numbers 1-8 are \"Quarterfinal\" matches, 9-14 are \"Semifinal\" and 15-17 are \"Finals\" matches. The \"level\" response however, will always just show \"Playoff\" regardless of the portion of the Playoff tournament.

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using FIRST.Api;
using FIRST.Client;
using FIRST.Model;

namespace Example


    public class SeasonMatchesEventCodeGetExample
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
            var apiInstance = new MatchResultsApi(httpClient, config, httpClientHandler);
            var eventCode = "eventCode_example";  // string | (Required) **[REQUIRED] (string)** Case insensitive alphanumeric eventCode of the event from which the results are requested. Must be at least 3 characters.
            var season = {{season}};  // string | **[REQUIRED] (int)** Numeric year of the event from which the award listings are requested. Must be 4 digits and greater than or equal to 2015, and less than or equal to the current year.
            var end = "end_example";  // string? | **(int)** Optional end match number for subset of results to return (inclusive). (optional) 
            var ifModifiedSince = "ifModifiedSince_example";  // string? |  (optional) 
            var matchNumber = "matchNumber_example";  // string? | **(int)** Optional specific single matchNumber of result. (optional) 
            var start = "start_example";  // string? | **(int)** Optional start match number for subset of results to return. (optional) 
            var teamNumber = "teamNumber_example";  // string? | **(int)** Optional teamNumber to search for within the results. Only returns match results in which the requested team was a participant. (optional) 
            var tournamentLevel = "tournamentLevel_example";  // string? | **(string)** Optional tournamentLevel of desired match results.  Enum values: ``` 1. None 2. Practice 3. Qualification 4. Playoff ``` (optional) 

            try
            {
                // Event Match Results
                Object result = apiInstance.SeasonMatchesEventCodeGet(eventCode, season, end, ifModifiedSince, matchNumber, start, teamNumber, tournamentLevel);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling MatchResultsApi.SeasonMatchesEventCodeGet: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the SeasonMatchesEventCodeGetWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    // Event Match Results
    ApiResponse<Object> response = apiInstance.SeasonMatchesEventCodeGetWithHttpInfo(eventCode, season, end, ifModifiedSince, matchNumber, start, teamNumber, tournamentLevel);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling MatchResultsApi.SeasonMatchesEventCodeGetWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **eventCode** | **string** | (Required) **[REQUIRED] (string)** Case insensitive alphanumeric eventCode of the event from which the results are requested. Must be at least 3 characters. |  |
| **season** | **string** | **[REQUIRED] (int)** Numeric year of the event from which the award listings are requested. Must be 4 digits and greater than or equal to 2015, and less than or equal to the current year. |  |
| **end** | **string?** | **(int)** Optional end match number for subset of results to return (inclusive). | [optional]  |
| **ifModifiedSince** | **string?** |  | [optional]  |
| **matchNumber** | **string?** | **(int)** Optional specific single matchNumber of result. | [optional]  |
| **start** | **string?** | **(int)** Optional start match number for subset of results to return. | [optional]  |
| **teamNumber** | **string?** | **(int)** Optional teamNumber to search for within the results. Only returns match results in which the requested team was a participant. | [optional]  |
| **tournamentLevel** | **string?** | **(string)** Optional tournamentLevel of desired match results.  Enum values: &#x60;&#x60;&#x60; 1. None 2. Practice 3. Qualification 4. Playoff &#x60;&#x60;&#x60; | [optional]  |

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

<a id="seasonscoreseventcodetournamentlevelget"></a>
# **SeasonScoresEventCodeTournamentLevelGet**
> Object SeasonScoresEventCodeTournamentLevelGet (string eventCode, string season, string tournamentLevel, string? end = null, string? ifModifiedSince = null, string? matchNumber = null, string? start = null)

Score Details

The score details API returns the score detail for all matches of a particular event in a particular season and a particular tournament level. Score details are only available once a match has been played, retrieving info about future matches requires the event schedule API. You cannot receive data about a match that is in progress.  **IMPORTANT: This endpoint returns differently depending on the season requested. The response details are listed multiple times, representing the different seasons possible in the return. Additionally, the scores shown in the example returns are not necessarily realistic- like the points may not add up.**  If you specify the `teamNumber` parameter, you cannot specify a `matchNumber` parameter. If you specify the `matchNumber`, you cannot define a `start` or `end`.  _**Note:**_ If you specify start, and it is higher than the maximum match number at the event, you will not receive any match results in the response. The same is true in reverse for the end parameter.  > _Click the \"20XX Score Details\" drop down in the example request/response pane (to the right or below) to view example score details responses for each of the available seasons._

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using FIRST.Api;
using FIRST.Client;
using FIRST.Model;

namespace Example


    public class SeasonScoresEventCodeTournamentLevelGetExample
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
            var apiInstance = new MatchResultsApi(httpClient, config, httpClientHandler);
            var eventCode = "eventCode_example";  // string | (Required) **[REQUIRED] (string)** Case insensitive alphanumeric eventCode of the event from which the details are requested. Must be at least 3 characters.
            var season = {{season}};  // string | **[REQUIRED] (int)** Numeric year of the event from which the award listings are requested. Must be 4 digits and greater than or equal to 2015, and less than or equal to the current year.
            var tournamentLevel = "tournamentLevel_example";  // string | (Required) **[REQUIRED] (string)** Required tournamentLevel of desired score details.  Enum values: ``` 1. None 2. Practice 3. Qualification 4. Playoff ```
            var end = "end_example";  // string? | **(int)** Optional end match number for subset of results to return (inclusive). (optional) 
            var ifModifiedSince = "ifModifiedSince_example";  // string? |  (optional) 
            var matchNumber = "matchNumber_example";  // string? | **(int)** Optional specific single matchNumber of result. (optional) 
            var start = "start_example";  // string? | **(int)** Optional start match number for subset of results to return (includsive). (optional) 

            try
            {
                // Score Details
                Object result = apiInstance.SeasonScoresEventCodeTournamentLevelGet(eventCode, season, tournamentLevel, end, ifModifiedSince, matchNumber, start);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling MatchResultsApi.SeasonScoresEventCodeTournamentLevelGet: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the SeasonScoresEventCodeTournamentLevelGetWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    // Score Details
    ApiResponse<Object> response = apiInstance.SeasonScoresEventCodeTournamentLevelGetWithHttpInfo(eventCode, season, tournamentLevel, end, ifModifiedSince, matchNumber, start);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling MatchResultsApi.SeasonScoresEventCodeTournamentLevelGetWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **eventCode** | **string** | (Required) **[REQUIRED] (string)** Case insensitive alphanumeric eventCode of the event from which the details are requested. Must be at least 3 characters. |  |
| **season** | **string** | **[REQUIRED] (int)** Numeric year of the event from which the award listings are requested. Must be 4 digits and greater than or equal to 2015, and less than or equal to the current year. |  |
| **tournamentLevel** | **string** | (Required) **[REQUIRED] (string)** Required tournamentLevel of desired score details.  Enum values: &#x60;&#x60;&#x60; 1. None 2. Practice 3. Qualification 4. Playoff &#x60;&#x60;&#x60; |  |
| **end** | **string?** | **(int)** Optional end match number for subset of results to return (inclusive). | [optional]  |
| **ifModifiedSince** | **string?** |  | [optional]  |
| **matchNumber** | **string?** | **(int)** Optional specific single matchNumber of result. | [optional]  |
| **start** | **string?** | **(int)** Optional start match number for subset of results to return (includsive). | [optional]  |

### Return type

**Object**

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json, text/plain


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | OK |  * Last-Modified -  <br>  * Content-Type -  <br>  |
| **500** | Internal Server Error |  * Content-Type -  <br>  |
| **0** | Default response |  -  |

[[Back to top]](#) [[Back to API list]](../../README.md#documentation-for-api-endpoints) [[Back to Model list]](../../README.md#documentation-for-models) [[Back to README]](../../README.md)

