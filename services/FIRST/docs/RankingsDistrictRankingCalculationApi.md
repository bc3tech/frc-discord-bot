# FIRST.Api.RankingsDistrictRankingCalculationApi

All URIs are relative to *https://frc-api.firstinspires.org*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**SeasonRankingsDistrictAllianceSelectionCalculationGet**](RankingsDistrictRankingCalculationApi.md#seasonrankingsdistrictallianceselectioncalculationget) | **GET** /{season}/rankings/district/allianceSelectionCalculation | Alliance Selection Points |
| [**SeasonRankingsDistrictPlayoffAdvancementCalculationGet**](RankingsDistrictRankingCalculationApi.md#seasonrankingsdistrictplayoffadvancementcalculationget) | **GET** /{season}/rankings/district/playoffAdvancementCalculation | Playoff Advancement Points |
| [**SeasonRankingsDistrictQualPerformanceCalculationGet**](RankingsDistrictRankingCalculationApi.md#seasonrankingsdistrictqualperformancecalculationget) | **GET** /{season}/rankings/district/qualPerformanceCalculation | Qual Performance Points |

<a id="seasonrankingsdistrictallianceselectioncalculationget"></a>
# **SeasonRankingsDistrictAllianceSelectionCalculationGet**
> Object SeasonRankingsDistrictAllianceSelectionCalculationGet (string season)

Alliance Selection Points

Alliance Selection Points is one of three endpoints to assist teams in figuring out how to improve their performance to achieve the desired district ranking placement. It is to determine the Event Total points.   `tournamentType`, `sizeType`, `allianceNumber`, and `allianceRole` are all required parameters for the calculation to occur.

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using FIRST.Api;
using FIRST.Client;
using FIRST.Model;

namespace Example


    public class SeasonRankingsDistrictAllianceSelectionCalculationGetExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "https://frc-api.firstinspires.org";

            // create instances of HttpClient, HttpClientHandler to be reused later with different Api classes
            HttpClient httpClient = new HttpClient();
            HttpClientHandler httpClientHandler = new HttpClientHandler();
            var apiInstance = new RankingsDistrictRankingCalculationApi(httpClient, config, httpClientHandler);
            var season = {{season}};  // string | **[REQUIRED] (int)** Numeric year of the event from which the award listings are requested. Must be 4 digits and greater than or equal to 2015, and less than or equal to the current year.

            try
            {
                // Alliance Selection Points
                Object result = apiInstance.SeasonRankingsDistrictAllianceSelectionCalculationGet(season);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling RankingsDistrictRankingCalculationApi.SeasonRankingsDistrictAllianceSelectionCalculationGet: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the SeasonRankingsDistrictAllianceSelectionCalculationGetWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    // Alliance Selection Points
    ApiResponse<Object> response = apiInstance.SeasonRankingsDistrictAllianceSelectionCalculationGetWithHttpInfo(season);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling RankingsDistrictRankingCalculationApi.SeasonRankingsDistrictAllianceSelectionCalculationGetWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **season** | **string** | **[REQUIRED] (int)** Numeric year of the event from which the award listings are requested. Must be 4 digits and greater than or equal to 2015, and less than or equal to the current year. |  |

### Return type

**Object**

### Authorization

[noauthAuth](../README.md#noauthAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | OK |  * Transfer-Encoding -  <br>  * Content-Type -  <br>  * Server -  <br>  * Date -  <br>  |

[[Back to top]](#) [[Back to API list]](../../README.md#documentation-for-api-endpoints) [[Back to Model list]](../../README.md#documentation-for-models) [[Back to README]](../../README.md)

<a id="seasonrankingsdistrictplayoffadvancementcalculationget"></a>
# **SeasonRankingsDistrictPlayoffAdvancementCalculationGet**
> Object SeasonRankingsDistrictPlayoffAdvancementCalculationGet (string season)

Playoff Advancement Points

Playoff Advancement Points is one of three endpoints to assist teams in figuring out how to improve their performance to achieve the desired district ranking placement. It is to determine the Event Total points.  `tournamentType`, `quarterFinalWins`, `semiFinalWins`, and `finalWins` are all required parameters for the calculation to occur.

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using FIRST.Api;
using FIRST.Client;
using FIRST.Model;

namespace Example


    public class SeasonRankingsDistrictPlayoffAdvancementCalculationGetExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "https://frc-api.firstinspires.org";

            // create instances of HttpClient, HttpClientHandler to be reused later with different Api classes
            HttpClient httpClient = new HttpClient();
            HttpClientHandler httpClientHandler = new HttpClientHandler();
            var apiInstance = new RankingsDistrictRankingCalculationApi(httpClient, config, httpClientHandler);
            var season = {{season}};  // string | **[REQUIRED] (int)** Numeric year of the event from which the award listings are requested. Must be 4 digits and greater than or equal to 2015, and less than or equal to the current year.

            try
            {
                // Playoff Advancement Points
                Object result = apiInstance.SeasonRankingsDistrictPlayoffAdvancementCalculationGet(season);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling RankingsDistrictRankingCalculationApi.SeasonRankingsDistrictPlayoffAdvancementCalculationGet: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the SeasonRankingsDistrictPlayoffAdvancementCalculationGetWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    // Playoff Advancement Points
    ApiResponse<Object> response = apiInstance.SeasonRankingsDistrictPlayoffAdvancementCalculationGetWithHttpInfo(season);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling RankingsDistrictRankingCalculationApi.SeasonRankingsDistrictPlayoffAdvancementCalculationGetWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **season** | **string** | **[REQUIRED] (int)** Numeric year of the event from which the award listings are requested. Must be 4 digits and greater than or equal to 2015, and less than or equal to the current year. |  |

### Return type

**Object**

### Authorization

[noauthAuth](../README.md#noauthAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | OK |  * Transfer-Encoding -  <br>  * Content-Type -  <br>  * Server -  <br>  * Date -  <br>  |

[[Back to top]](#) [[Back to API list]](../../README.md#documentation-for-api-endpoints) [[Back to Model list]](../../README.md#documentation-for-models) [[Back to README]](../../README.md)

<a id="seasonrankingsdistrictqualperformancecalculationget"></a>
# **SeasonRankingsDistrictQualPerformanceCalculationGet**
> Object SeasonRankingsDistrictQualPerformanceCalculationGet (string season)

Qual Performance Points

Qual Performance Points is one of three endpoints to assist teams in figuring out how to improve their performance to achieve the desired district ranking placement. It is to determine the Event Total points.  `tournamentType`, `sizeType`, `allianceNumber`, and `allianceRole` are all required parameters for the calculation to occur.

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using FIRST.Api;
using FIRST.Client;
using FIRST.Model;

namespace Example


    public class SeasonRankingsDistrictQualPerformanceCalculationGetExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "https://frc-api.firstinspires.org";

            // create instances of HttpClient, HttpClientHandler to be reused later with different Api classes
            HttpClient httpClient = new HttpClient();
            HttpClientHandler httpClientHandler = new HttpClientHandler();
            var apiInstance = new RankingsDistrictRankingCalculationApi(httpClient, config, httpClientHandler);
            var season = {{season}};  // string | **[REQUIRED] (int)** Numeric year of the event from which the award listings are requested. Must be 4 digits and greater than or equal to 2015, and less than or equal to the current year.

            try
            {
                // Qual Performance Points
                Object result = apiInstance.SeasonRankingsDistrictQualPerformanceCalculationGet(season);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling RankingsDistrictRankingCalculationApi.SeasonRankingsDistrictQualPerformanceCalculationGet: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the SeasonRankingsDistrictQualPerformanceCalculationGetWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    // Qual Performance Points
    ApiResponse<Object> response = apiInstance.SeasonRankingsDistrictQualPerformanceCalculationGetWithHttpInfo(season);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling RankingsDistrictRankingCalculationApi.SeasonRankingsDistrictQualPerformanceCalculationGetWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **season** | **string** | **[REQUIRED] (int)** Numeric year of the event from which the award listings are requested. Must be 4 digits and greater than or equal to 2015, and less than or equal to the current year. |  |

### Return type

**Object**

### Authorization

[noauthAuth](../README.md#noauthAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | OK |  * Transfer-Encoding -  <br>  * Content-Type -  <br>  * Server -  <br>  * Date -  <br>  |

[[Back to top]](#) [[Back to API list]](../../README.md#documentation-for-api-endpoints) [[Back to Model list]](../../README.md#documentation-for-models) [[Back to README]](../../README.md)

