# FIRST.Api.SeasonDataApi

All URIs are relative to *https://frc-api.firstinspires.org*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**SeasonAvatarsGet**](SeasonDataApi.md#seasonavatarsget) | **GET** /{season}/avatars | Team Avatar Listings |
| [**SeasonDistrictsGet**](SeasonDataApi.md#seasondistrictsget) | **GET** /{season}/districts | District Listings |
| [**SeasonEventsGet**](SeasonDataApi.md#seasoneventsget) | **GET** /{season}/events | Event Listings |
| [**SeasonGet**](SeasonDataApi.md#seasonget) | **GET** /{season} | Season Summary |
| [**SeasonTeamsGet**](SeasonDataApi.md#seasonteamsget) | **GET** /{season}/teams | Team Listings |

<a id="seasonavatarsget"></a>
# **SeasonAvatarsGet**
> Object SeasonAvatarsGet (string season, string? eventCode = null, string? ifModifiedSince = null, string? page = null, string? teamNumber = null)

Team Avatar Listings

This endpoint applies only to the 2018 or later seasons. Requests for other seasons will result in a `Bad Season` error. The team avatar listings API returns all FRC official teams in a particular `season` with, if applicable, their Avatar. If specified, the `teamNumber` parameter will return only one result with the details of the requested `teamNumber`. Alternately, the `eventCode` parameter allows sorting of the team list to only those teams attending a particular event in the particular `season`. If you specify a `teamNumber` parameter, you cannot additionally specify an `eventCode` or you will receive an `HTTP 501`. If a team does not have an Avatar, the return value will be `null`. Please note that the returned Avatar is encoded in the reply, and would need to be properly handled in order to render a PNG image.

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using FIRST.Api;
using FIRST.Client;
using FIRST.Model;

namespace Example


    public class SeasonAvatarsGetExample
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
            var apiInstance = new SeasonDataApi(httpClient, config, httpClientHandler);
            var season = {{season}};  // string | **[REQUIRED] (int)** Numeric year of the event from which the award listings are requested. Must be 4 digits and greater than or equal to 2015, and less than or equal to the current year.
            var eventCode = "eventCode_example";  // string? | **(string)** Case insensitive alphanumeric eventCode of the event from which details are requested. (optional) 
            var ifModifiedSince = "ifModifiedSince_example";  // string? |  (optional) 
            var page = "page_example";  // string? | **(int)** Numeric page of results to return. If not included, page 1 will be returned. (optional) 
            var teamNumber = "teamNumber_example";  // string? | **(int)** Numeric teamNumber of the team about which information is requested. Must be 1 to 4 digits. (optional) 

            try
            {
                // Team Avatar Listings
                Object result = apiInstance.SeasonAvatarsGet(season, eventCode, ifModifiedSince, page, teamNumber);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling SeasonDataApi.SeasonAvatarsGet: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the SeasonAvatarsGetWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    // Team Avatar Listings
    ApiResponse<Object> response = apiInstance.SeasonAvatarsGetWithHttpInfo(season, eventCode, ifModifiedSince, page, teamNumber);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling SeasonDataApi.SeasonAvatarsGetWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **season** | **string** | **[REQUIRED] (int)** Numeric year of the event from which the award listings are requested. Must be 4 digits and greater than or equal to 2015, and less than or equal to the current year. |  |
| **eventCode** | **string?** | **(string)** Case insensitive alphanumeric eventCode of the event from which details are requested. | [optional]  |
| **ifModifiedSince** | **string?** |  | [optional]  |
| **page** | **string?** | **(int)** Numeric page of results to return. If not included, page 1 will be returned. | [optional]  |
| **teamNumber** | **string?** | **(int)** Numeric teamNumber of the team about which information is requested. Must be 1 to 4 digits. | [optional]  |

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

<a id="seasondistrictsget"></a>
# **SeasonDistrictsGet**
> Object SeasonDistrictsGet (string season, string? ifModifiedSince = null)

District Listings

The district listings API returns all FRC official districts of a particular season.

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using FIRST.Api;
using FIRST.Client;
using FIRST.Model;

namespace Example


    public class SeasonDistrictsGetExample
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
            var apiInstance = new SeasonDataApi(httpClient, config, httpClientHandler);
            var season = {{season}};  // string | **[REQUIRED] (int)** Numeric year of the event from which the award listings are requested. Must be 4 digits and greater than or equal to 2015, and less than or equal to the current year.
            var ifModifiedSince = "ifModifiedSince_example";  // string? |  (optional) 

            try
            {
                // District Listings
                Object result = apiInstance.SeasonDistrictsGet(season, ifModifiedSince);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling SeasonDataApi.SeasonDistrictsGet: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the SeasonDistrictsGetWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    // District Listings
    ApiResponse<Object> response = apiInstance.SeasonDistrictsGetWithHttpInfo(season, ifModifiedSince);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling SeasonDataApi.SeasonDistrictsGetWithHttpInfo: " + e.Message);
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

<a id="seasoneventsget"></a>
# **SeasonEventsGet**
> Object SeasonEventsGet (string season, string? districtCode = null, string? eventCode = null, string? excludeDistrict = null, string? ifModifiedSince = null, string? teamNumber = null, string? tournamentType = null, string? weekNumber = null)

Event Listings

The event listings API returns all FRC official district and regional events in a particular season. You can specify an `eventCode` if you would only like data about one specific event. If you specify an `eventCode` you cannot specify any other optional parameters. Alternately, you can specify a `teamNumber` to retrieve only the listings of events being attended by the particular team. If you specify a `teamNumber` you cannot specify an `eventCode`. The `districtCode` parameter can be added to filter the response to only thos events from a particular district. If you specify the `districtCode` parameter, you cannot specify an `eventCode` or the `excludeDistrict` parameters. The `excludeDistrict` parameter can be used to prevent the response from including district events. If you specify the `excludeDistrict` parameter, you cannot specify an `eventCode` or `districtCode`.  Values on this endpoint are \"pass through\" values from the MyEvents registration system. As such, if the event does not specify a value for a field, it may be presented in the API as `null`.  The response for event listings contains a special field called `divisionCode`. Starting with the 2015 season, some events are considered to be divisions of others. For example, the FIRST Championship contains four Divisions and each Division contains two Subdivisions. As an example of a reponse, the event listings for a subdivision will contain a `divisionCode` of the Division from which they belong. In turn, the Division will then have a `divisionCode` that matches the FIRST Championship event code (as they are divisions of that event). This allows you to see the full structure of events, and how they relate to each other.  **_Important Note:_** For events with the `type` ChampionshipDivision, you cannot request match results, rankings, schedules or alliances. These event types do not have those results, only Subdivisions. As such, you would receive an `HTTP 404 (not found)` if you request results that are not applicable to the event. However, they do have awards, and those can be requested.

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using FIRST.Api;
using FIRST.Client;
using FIRST.Model;

namespace Example


    public class SeasonEventsGetExample
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
            var apiInstance = new SeasonDataApi(httpClient, config, httpClientHandler);
            var season = {{season}};  // string | **[REQUIRED] (int)** Numeric year of the event from which the award listings are requested. Must be 4 digits and greater than or equal to 2015, and less than or equal to the current year.
            var districtCode = "districtCode_example";  // string? | **(string)** Case insensitive districtCode of the district from which event listings are requested.  District Codes: ``` FMA PNW NE FIN FNC ONT ISR CHS FIT PCH FIM ``` (optional) 
            var eventCode = "eventCode_example";  // string? | **(string)** Case insensitive alphanumeric eventCode of the event about which details are requested. (optional) 
            var excludeDistrict = "excludeDistrict_example";  // string? | **(bool)**  Boolean to specify whether or not to exclude district events in the event listings. true means exclude, but if no value is specified, false will be used (include district events). Excluding district events also excludes district championships. (optional) 
            var ifModifiedSince = "ifModifiedSince_example";  // string? |  (optional) 
            var teamNumber = "teamNumber_example";  // string? | **(int)** Numeric teamNumber of the team from which the attending event listings are requested. (optional) 
            var tournamentType = "tournamentType_example";  // string? | **(string)** Type of event to perform the calculation on.  Enum values: ``` 1. None 2. Regional 3. DistrictEvent 4. DistrictChampionship 5. DistrictChampionshipWithLevels 6. DistrictChampionshipDivision 7. ChampionshipSubdivision 8. ChampionshipDivision 9. Championship 10. OffSeason 11. OffSeasonWithAzureSync ``` (optional) 
            var weekNumber = "weekNumber_example";  // string? | **(int)** Week during the FRC season that the event takes place. (optional) 

            try
            {
                // Event Listings
                Object result = apiInstance.SeasonEventsGet(season, districtCode, eventCode, excludeDistrict, ifModifiedSince, teamNumber, tournamentType, weekNumber);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling SeasonDataApi.SeasonEventsGet: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the SeasonEventsGetWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    // Event Listings
    ApiResponse<Object> response = apiInstance.SeasonEventsGetWithHttpInfo(season, districtCode, eventCode, excludeDistrict, ifModifiedSince, teamNumber, tournamentType, weekNumber);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling SeasonDataApi.SeasonEventsGetWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **season** | **string** | **[REQUIRED] (int)** Numeric year of the event from which the award listings are requested. Must be 4 digits and greater than or equal to 2015, and less than or equal to the current year. |  |
| **districtCode** | **string?** | **(string)** Case insensitive districtCode of the district from which event listings are requested.  District Codes: &#x60;&#x60;&#x60; FMA PNW NE FIN FNC ONT ISR CHS FIT PCH FIM &#x60;&#x60;&#x60; | [optional]  |
| **eventCode** | **string?** | **(string)** Case insensitive alphanumeric eventCode of the event about which details are requested. | [optional]  |
| **excludeDistrict** | **string?** | **(bool)**  Boolean to specify whether or not to exclude district events in the event listings. true means exclude, but if no value is specified, false will be used (include district events). Excluding district events also excludes district championships. | [optional]  |
| **ifModifiedSince** | **string?** |  | [optional]  |
| **teamNumber** | **string?** | **(int)** Numeric teamNumber of the team from which the attending event listings are requested. | [optional]  |
| **tournamentType** | **string?** | **(string)** Type of event to perform the calculation on.  Enum values: &#x60;&#x60;&#x60; 1. None 2. Regional 3. DistrictEvent 4. DistrictChampionship 5. DistrictChampionshipWithLevels 6. DistrictChampionshipDivision 7. ChampionshipSubdivision 8. ChampionshipDivision 9. Championship 10. OffSeason 11. OffSeasonWithAzureSync &#x60;&#x60;&#x60; | [optional]  |
| **weekNumber** | **string?** | **(int)** Week during the FRC season that the event takes place. | [optional]  |

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

<a id="seasonget"></a>
# **SeasonGet**
> Object SeasonGet (string season, string? ifModifiedSince = null)

Season Summary

The season summary API returns a high level glance of a particular FRC season.

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using FIRST.Api;
using FIRST.Client;
using FIRST.Model;

namespace Example


    public class SeasonGetExample
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
            var apiInstance = new SeasonDataApi(httpClient, config, httpClientHandler);
            var season = {{season}};  // string | **[REQUIRED] (int)** Numeric year of the event from which the award listings are requested. Must be 4 digits and greater than or equal to 2015, and less than or equal to the current year.
            var ifModifiedSince = "ifModifiedSince_example";  // string? | (Required)  (optional) 

            try
            {
                // Season Summary
                Object result = apiInstance.SeasonGet(season, ifModifiedSince);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling SeasonDataApi.SeasonGet: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the SeasonGetWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    // Season Summary
    ApiResponse<Object> response = apiInstance.SeasonGetWithHttpInfo(season, ifModifiedSince);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling SeasonDataApi.SeasonGetWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **season** | **string** | **[REQUIRED] (int)** Numeric year of the event from which the award listings are requested. Must be 4 digits and greater than or equal to 2015, and less than or equal to the current year. |  |
| **ifModifiedSince** | **string?** | (Required)  | [optional]  |

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

<a id="seasonteamsget"></a>
# **SeasonTeamsGet**
> Object SeasonTeamsGet (string season, string? districtCode = null, string? eventCode = null, string? ifModifiedSince = null, string? page = null, string? state = null, string? teamNumber = null)

Team Listings

The team listings API returns all FRC official teams in a particular `season`. If specified, the `teamNumber` parameter will return only one result with the details of the requested `teamNumber`. Alternately, the `eventCode` parameter allows sorting of the team list to only those teams attending a particular event in the particular `season`. Further, the district parameter allows results to be returned only when the team is a member of the requested district based on the district code. If you specify a `teamNumber` parameter, you cannot additionally specify an `eventCode` and/or `districtCode` and/or state in the same request, or you will receive an `HTTP 501`. You can however request a `districtCode` and an `eventCode` together. If you specify the `state` parameter, it should be the full legal name of the US state or international state/prov, such as `New Hampshire` or `Ontario`. Values on this endpoint are \"pass through\" values from the TIMS registration system. As such, if the team does not specify a value for a field, it may be presented in the API as `null`.

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using FIRST.Api;
using FIRST.Client;
using FIRST.Model;

namespace Example


    public class SeasonTeamsGetExample
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
            var apiInstance = new SeasonDataApi(httpClient, config, httpClientHandler);
            var season = {{season}};  // string | **[REQUIRED] (int)** Numeric year of the event from which the award listings are requested. Must be 4 digits and greater than or equal to 2015, and less than or equal to the current year.
            var districtCode = "districtCode_example";  // string? | **(string)** Case insensitive districtCode code of the district from which team listings are requested.  District Codes: ``` FMA PNW NE FIN FNC ONT ISR CHS FIT PCH FIM ``` (optional) 
            var eventCode = "eventCode_example";  // string? | **(string)** Case insensitive alphanumeric eventCode of the event from which details are requested. (optional) 
            var ifModifiedSince = "ifModifiedSince_example";  // string? |  (optional) 
            var page = "page_example";  // string? | **(int)** Numeric page of results to return. If not included, page 1 will be returned. (optional) 
            var state = "state_example";  // string? | **(string)** Name of the state or province the desired teams are located. (optional) 
            var teamNumber = "teamNumber_example";  // string? | **(int)** Numeric teamNumber of the team about which information is requested. Must be 1 to 4 digits. (optional) 

            try
            {
                // Team Listings
                Object result = apiInstance.SeasonTeamsGet(season, districtCode, eventCode, ifModifiedSince, page, state, teamNumber);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling SeasonDataApi.SeasonTeamsGet: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the SeasonTeamsGetWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    // Team Listings
    ApiResponse<Object> response = apiInstance.SeasonTeamsGetWithHttpInfo(season, districtCode, eventCode, ifModifiedSince, page, state, teamNumber);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling SeasonDataApi.SeasonTeamsGetWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **season** | **string** | **[REQUIRED] (int)** Numeric year of the event from which the award listings are requested. Must be 4 digits and greater than or equal to 2015, and less than or equal to the current year. |  |
| **districtCode** | **string?** | **(string)** Case insensitive districtCode code of the district from which team listings are requested.  District Codes: &#x60;&#x60;&#x60; FMA PNW NE FIN FNC ONT ISR CHS FIT PCH FIM &#x60;&#x60;&#x60; | [optional]  |
| **eventCode** | **string?** | **(string)** Case insensitive alphanumeric eventCode of the event from which details are requested. | [optional]  |
| **ifModifiedSince** | **string?** |  | [optional]  |
| **page** | **string?** | **(int)** Numeric page of results to return. If not included, page 1 will be returned. | [optional]  |
| **state** | **string?** | **(string)** Name of the state or province the desired teams are located. | [optional]  |
| **teamNumber** | **string?** | **(int)** Numeric teamNumber of the team about which information is requested. Must be 1 to 4 digits. | [optional]  |

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

