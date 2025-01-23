# FIRST.Api.AllianceSelectionApi

All URIs are relative to *https://frc-api.firstinspires.org*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**SeasonAlliancesEventCodeGet**](AllianceSelectionApi.md#seasonallianceseventcodeget) | **GET** /{season}/alliances/{eventCode} | Event Alliances |

<a id="seasonallianceseventcodeget"></a>
# **SeasonAlliancesEventCodeGet**
> void SeasonAlliancesEventCodeGet (string eventCode, string season)

Event Alliances

The alliances API returns details about alliance selection at a particular event in a particular season.

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using FIRST.Api;
using FIRST.Client;
using FIRST.Model;

namespace Example


    public class SeasonAlliancesEventCodeGetExample
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
            var apiInstance = new AllianceSelectionApi(httpClient, config, httpClientHandler);
            var eventCode = "eventCode_example";  // string | **[REQUIRED] (string)** Case insensitive alphanumeric eventCode of the event from which the alliance selection results are requested. Must be at least 3 characters. 
            var season = {{season}};  // string | **[REQUIRED] (int)** Numeric year of the event from which the event alliances are requested. Must be 4 digits and greater than or equal to 2015, and less than or equal to the current year. 

            try
            {
                // Event Alliances
                apiInstance.SeasonAlliancesEventCodeGet(eventCode, season);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling AllianceSelectionApi.SeasonAlliancesEventCodeGet: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the SeasonAlliancesEventCodeGetWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    // Event Alliances
    apiInstance.SeasonAlliancesEventCodeGetWithHttpInfo(eventCode, season);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling AllianceSelectionApi.SeasonAlliancesEventCodeGetWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **eventCode** | **string** | **[REQUIRED] (string)** Case insensitive alphanumeric eventCode of the event from which the alliance selection results are requested. Must be at least 3 characters.  |  |
| **season** | **string** | **[REQUIRED] (int)** Numeric year of the event from which the event alliances are requested. Must be 4 digits and greater than or equal to 2015, and less than or equal to the current year.  |  |

### Return type

void (empty response body)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | Successful response |  -  |

[[Back to top]](#) [[Back to API list]](../../README.md#documentation-for-api-endpoints) [[Back to Model list]](../../README.md#documentation-for-models) [[Back to README]](../../README.md)

