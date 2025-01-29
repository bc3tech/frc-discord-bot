/*
 * FRC Events
 *
 * # Overview  _FIRST_/FMS FRC Events API is a service to return relevant information about the _FIRST_ Robotics Competition (FRC). Information is made available from events operating around the world.  For FRC, information is made available by the Field Management System (FMS) server operating at each event site. The FMS will attempt to sync all data from the event to \"the cloud\" as long as internet is available at the venue. If internet is unavailable, or \"goes down\" during the event, the FMS will automatically sync all data from the time the system was offline as soon as the connection is restored. The API will provide data as soon as it has synced, and we do not add any artificial delays. If you are receiving \"stale\" data, it may be because of connectivity problems at the venue. We recommend you try again later, and post on the FIRST FMS TeamForge site if the problem persists. _(Note: FMS does not sync while a match is running, so data that has to do with a particular match should become available once the score has been revealed to the audience at the event.)_  ### Migration and Program Notes:  Pay close attention to the addresses for calling the various endpoints- as well as the notes regarding endpoints with multiple possible responses (i.e. score details and rankings).  # Documentation Notes  All times are listed in the local time to the event venue. HTTP-date values will show their timezone.  If you specify a parameter, but no value for that parameter, it will be ignored. For example, if you request **URL?teamNumber=** the **teamNumber** parameter would be ignored.  We will continue to support the current version of the API plus one version older. Old APIs are depricated once a version \"two times newer\" is available, at minimum 6 months. For example, version 2.0 and 1.0 are supported right now, but 1.0 will not be supported once 2.1 (or 3.0) is available. Versions may also be retired earlier with prior notice here in the documentation.  The full host address of the API is needed in all calls. The version number is required in each call in order to ensure your requests are made (and responses are returned) in the formats you anticipate. The version number for this documentation is found on the top of the page, in the title. If you call this version number, the API responses will match the formats listed below.  All of the APIs are capable of accepting the **Accept** HTTP header with either **application/xml** or **application/json** based on the desired return type. Any API call that results in an **HTTP Status Code** other than **200 (OK)** will only be shown here as an **application/json** response to save space, but the content is the same regardless of the request type. All response will have a **Content-Length** and **Date** response header, but those are not shown in the documentaiton.  For all APIs that accept a query string in addition to the URI base, the order of parameters do not matter, but the name shown in the documentation must match exactly, as does the associated value format as described in details.  For response codes that are not **HTTP 200 (OK)**, the documentation will show a body message that represents a possible response value. While the \"title\" of the **HTTP Status Code** will match those shown in the response codes documentation section exactly, the body of the response will be a more detailed explanation of why that status code is being returned and may not always be exactly as shown in the examples.  None of the APIs will show possible return here in the documentation of **HTTP 401 (Unauthorized)**, but that code applies to all APIs as a possible response if the request is made without a valid token.  ### Last-Modified, FMS-OnlyModifiedSince, and If-Modified-Since Headers  The FRC Events API utilizes the **Last-Modified** and **If-Modified-Since** Headers to communicate with consumers regarding the age of the data they are requesting. With a couple of exceptions, all calls will return a **Last-Modified** Header set with the time at which the data at that endpoint was last modified. The Header will always be set in the HTTP-date format, as described in the [HTTP Protocol](https://www.w3.org/Protocols/rfc2616/rfc2616-sec14.html). There are two exceptions: the **Last-Modified** Header is not set if the endpoint returns no results (such as a request for a schedule with no matches) and will also not be set if the request was an **HTTP DELETE**.  Consumers should keep track of the **Last-Modified** Header, and return it on subsequent calls to the same endpoint as the **If-Modified-Since**. The server will recognize this request, and will only return a result if the data has been modified since the last request. If no changes have been made, an **HTTP 304** will be returned. If data has been modified, ALL data on that call will be returned (for \"only modified\" data, see below).  The FRC Events API also allows a custom header used to filter the return data to a specific subset. This is done by specifying a **FMS-OnlyModifiedSince** header with each call. As with the **If-Modified-Since** header, consumers should keep track of the **Last-Modified** Header, and return it on subsequent calls to the same endpoint as the **FMS-OnlyModifiedSince** Header. The server will recognize this request, and will only return a result if the data has been modified since the last request, and, if returned, the data will only be those portions modified since the included date. If no changes, have been made, an **HTTP 304** will be returned. Using this method, the server and consumer save processing time by only receiving modified data that is in need of update on the consumer side.  If the Headers are improperly passed (such as the wrong Day of Week for the matching date, or a date in the future), the endpoint will simply ignore the Header and return all results. If both headers are specified, the request will be denied.  # Response Codes  The FRC Events API HTTP Status Codes correspond with the [common codes](https://www.w3.org/Protocols/rfc2616/rfc2616-sec10.html), but occasionally with different \"titles\". The \"title\" used by the API is shown next to each of the below possible response HTTP Status Codes. Throughout the documentation, Apiary may automatically show the common \"title\" in example returns (like \"Not Found\" for 404) but on the production server, the \"title\" will instead match those listed below.  ``` HTTP 200 - \"OK\"   ```  The request has succeeded. An entity corresponding to the requested resource is sent in the response. This will be returned as the HTTP Status Code for all request that succeed, even if the body is empty (such as an event that has no rankings, but with a valid season and event code were used)  ``` HTTP 304 - \"Not Modified\"   ```  When utilizing a Header that allows filtered data returns, such as **If-Modified-Since**, this response indicates that no data meets the request.  ``` HTTP 400 - \"Invalid Season Requested\"/\"Malformed Parameter Format In Request\"/\"Missing Parameter In Request\"/\"Invalid API Version Requested\":   ```  The request could not be understood by the server due to malformed syntax. The client SHOULD NOT repeat the request without modifications. Specifically for this API, a 400 response indicates that the requested URI matches with a valid API, but one or more required parameter was malformed or invalid. Examples include an event code that is too short or team number that contains a letter.  ``` HTTP 401 - \"Unauthorized\"   ```  All requests against the API require authentication via a valid user token. Failing to provide one, or providing an invalid one, will warrant a 401 response. The client MAY repeat the request with a suitable Authorization header field.  ``` HTTP 404 - \"Invalid Event Requested\"   ```  Even though the 404 code usually indicates any not found status, a 404 will only be issued in this API when an event cannot be found for the requested season and event code. If the request didn't match a valid API or there were malformed parameters, the response would not receive a 404 but rather a 400 or 501. If this HTTP code is received, the season was a valid season and the event code matched the acceptable style of an event code, but there were no records of an event matching the combination of that season and event code. For example, HTTP 404 would be issued when the event had a different code in the requested season (the codes can change year to year based on event location).  ``` HTTP 500 - \"Internal Server Error\"   ```  The server encountered an unexpected condition which prevented it from fulfilling the request. This is a code sent directly by the server, and has no special alternate definition specific to this API.  ``` HTTP 501 - \"Request Did Not Match Any Current API Pattern\"   ```  The server does not support the functionality required to fulfill the request. Specifically, the request pattern did not match any of the possible APIs, and thus processing was discontinued. This code is also issued when too many optional parameters were included in a single request and fulfilling it would make the result confusing or misleading. Each API will specify which parameters or combination of parameters can be used at the same time.  ``` HTTP 503 - \"Service Unavailable\"   ```  The server is currently unable to handle the request due to a temporary overloading or maintenance of the server. The implication is that this is a temporary condition which will be alleviated after some delay. If known, the length of the delay MAY be indicated in a Retry-After header. This code will not always appear, sometimes the server may outright refuse the connection instead. This is a code sent directly by the server, and has no special alternate definition specific to this API.  See the notes at the top of this documentation for important information about HTTP Status Codes.  # Authorization  In order to make calls against the FRC Events API, you must include an HTTP Header called **Authorization** with the value set as specified below. If a request is made without this header, processing stops and an **HTTP 401** is issued. All **Authorization** headers follow the same format:  Authorization: Basic 000000000000000000000000000000000000000000000000000000000000  Where the Zeros are replaced by your Token. The Token can be formed by taking your **username** and your **AuthorizationKey** and adding a colon. For example, if your **username** is sampleuser and your **AuthorizationKey** is 7eaa6338-a097-4221-ac04-b6120fcc4d49 you would have this string:  **sampleuser:7eaa6338-a097-4221-ac04-b6120fcc4d49**  This string must then be encoded using Base64 Encoded to form the Token, which will be the same length as the example above, but include letters and numbers. For our example, we would have:  c2FtcGxldXNlcjo3ZWFhNjMzOC1hMDk3LTQyMjEtYWMwNC1iNjEyMGZjYzRkNDk=  **NOTICE**: Publicly distributing an application, code snippet, etc, that has your username and token in it, encoded or not, WILL result in your token being blocked from the API. Each user should apply for their own token.  If you wish to acquire a token for your development, you may do so by requesting a token through our automated system on [this website](https://frc-events.firstinspires.org/services/API).  **AUTOMATED REMOVAL**: If you do not activate your account within 72 hours of making your request for a token, or if you do not make at least one API request every twelve (12) months, your account/token will be marked as disabled for inactivity and subject to being deleted. (This policy does not apply to accounts with special operating agreements with FIRST)  ### HTTP401 and Authorization  Each Token can be individually enabled and disabled by _FIRST_. As such, a normally valid combination of **username** and **AuthorizationToken** could still be rejected. The possible return messages you may see in these instances are:  **Incorrect Token** (You supplied an AuthorizationToken, but it wasn't correct)  **Account Disabled, Contact Support** (You have been disabled for excessive traffic or abuse. Contact support)  **Username Not Found** (A username was found, but didn't match any on file)  **Unable To Determine Authorization Token** (The format of the **Authorization** header was incorrect)  # Webhooks  > **COMING SOON**
 *
 * The version of the OpenAPI document: 1.0.0
 * Generated by: https://github.com/openapitools/openapi-generator.git
 */

namespace FIRST.Api;

using System;
using System.Net.Http;
  using System.Collections.ObjectModel;
  using System.Threading.Tasks;

using FIRST.Client;

  /// <summary>
  /// Represents a collection of functions to interact with the API endpoints
  /// </summary>
  public interface IRankingsApiSync : IApiAccessor
  {
    #region Synchronous Operations
      /// <summary>
      /// District Rankings
      /// </summary>
        /// <remarks>
        /// The district rankings API returns team ranking detail from a particular team in a particular season. You *must* specify a districtCode unless a &#x60;teamNumber&#x60; is being specified. If a &#x60;teamNumber&#x60; is specified, do not include any other paramaters. Optionally, the &#x60;top&#x60; parameter can be added to the query string to request a subset of the rankings based on the highest ranked teams at the time of the request. Alternately, you can specify the &#x60;teamNumber&#x60; parameter to retrieve the ranking on one specific team. You cannot specify both a &#x60;top&#x60; and &#x60;teamNumber&#x60; in the same call. If you specify a &#x60;page&#x60;, you cannot specify a &#x60;top&#x60;.  This endpoint is only updated periodically, and may not reflect final rankings for an event/district until a period of time after a given event is completed. The final authority on teams advancing tournament levels is the District Ranking website and communications from *FIRST*, not this API. *See the FRC Game Manual for more information.*
        /// </remarks>
      /// <exception cref="FIRST.Client.ApiException">Thrown when fails to make API call</exception>
      /// <param name="season">**[REQUIRED] (int)** Numeric year of the event from which the award listings are requested. Must be 4 digits and greater than or equal to 2015, and less than or equal to the current year.</param>
      /// <param name="districtCode">**(string)** Case insensitive alphanumeric districtCode of the district from which the rankings are requested. Must be at least 2 characters.  District Codes: &#x60;&#x60;&#x60; FMA PNW NE FIN FNC ONT ISR CHS FIT PCH FIM &#x60;&#x60;&#x60; (optional)</param>
      /// <param name="ifModifiedSince"> (optional)</param>
      /// <param name="page">**(int)** Numeric page of results to return. If not included, page 1 will be returned. (optional)</param>
      /// <param name="teamNumber">**(int)** Optional team number of the team whose ranking is requested. (optional)</param>
      /// <param name="top">**(int)** Optional number of requested top ranked teams to return in result. (optional)</param>
      /// <returns>Object</returns>
      Object? SeasonRankingsDistrictGet(string season, string? districtCode = default, string? ifModifiedSince = default, string? page = default, string? teamNumber = default, string? top = default);
        
        /// <summary>
        /// District Rankings
        /// </summary>
        /// <remarks>
        /// The district rankings API returns team ranking detail from a particular team in a particular season. You *must* specify a districtCode unless a &#x60;teamNumber&#x60; is being specified. If a &#x60;teamNumber&#x60; is specified, do not include any other paramaters. Optionally, the &#x60;top&#x60; parameter can be added to the query string to request a subset of the rankings based on the highest ranked teams at the time of the request. Alternately, you can specify the &#x60;teamNumber&#x60; parameter to retrieve the ranking on one specific team. You cannot specify both a &#x60;top&#x60; and &#x60;teamNumber&#x60; in the same call. If you specify a &#x60;page&#x60;, you cannot specify a &#x60;top&#x60;.  This endpoint is only updated periodically, and may not reflect final rankings for an event/district until a period of time after a given event is completed. The final authority on teams advancing tournament levels is the District Ranking website and communications from *FIRST*, not this API. *See the FRC Game Manual for more information.*
        /// </remarks>
        /// <exception cref="FIRST.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="season">**[REQUIRED] (int)** Numeric year of the event from which the award listings are requested. Must be 4 digits and greater than or equal to 2015, and less than or equal to the current year.</param>
        /// <param name="districtCode">**(string)** Case insensitive alphanumeric districtCode of the district from which the rankings are requested. Must be at least 2 characters.  District Codes: &#x60;&#x60;&#x60; FMA PNW NE FIN FNC ONT ISR CHS FIT PCH FIM &#x60;&#x60;&#x60; (optional)</param>
        /// <param name="ifModifiedSince"> (optional)</param>
        /// <param name="page">**(int)** Numeric page of results to return. If not included, page 1 will be returned. (optional)</param>
        /// <param name="teamNumber">**(int)** Optional team number of the team whose ranking is requested. (optional)</param>
        /// <param name="top">**(int)** Optional number of requested top ranked teams to return in result. (optional)</param>
        /// <returns>ApiResponse of Object</returns>
        ApiResponse<Object?> SeasonRankingsDistrictGetWithHttpInfo(string season, string? districtCode = default, string? ifModifiedSince = default, string? page = default, string? teamNumber = default, string? top = default);
      /// <summary>
      /// Event Rankings
      /// </summary>
        /// <remarks>
        /// The rankings API returns team ranking detail from a particular event in a particular season. Optionally, the &#x60;top&#x60; parameter can be added to the query string to request a subset of the rankings based on the highest ranked teams at the time of the request. Alternately, you can specify the &#x60;teamNumber&#x60; parameter to retrieve the ranking on one specific team. You cannot specify both a top and &#x60;teamNumber&#x60; in the same call.  **IMPORTANT: This endpoint use to return differently for 2015 vs other seasons. In the fall 2016 updates, this was changed, and all seasons of data now return in the genericized format specified below.**  In all response details, the \&quot;team\&quot; refers to the FRC Team that the ranking represents, as well as their various alliance partners in the matches they have played (i.e. scores in a single match are not calcualted by team, but by alliance). *See the FRC Game Manual for more information.*
        /// </remarks>
      /// <exception cref="FIRST.Client.ApiException">Thrown when fails to make API call</exception>
      /// <param name="eventCode">(Required) **[REQUIRED] (string)** Case insensitive alphanumeric eventCode of the event from which the rankings are requested. Must be at least 3 characters.</param>
      /// <param name="season">**[REQUIRED] (int)** Numeric year of the event from which the award listings are requested. Must be 4 digits and greater than or equal to 2015, and less than or equal to the current year.</param>
      /// <param name="ifModifiedSince"> (optional)</param>
      /// <param name="teamNumber">**(int)** Optional team number of the team whose ranking is requested. (optional)</param>
      /// <param name="top">**(int)** Optional number of requested top ranked teams to return in result. (optional)</param>
      /// <returns>Object</returns>
      Object? SeasonRankingsEventCodeGet(string eventCode, string season, string? ifModifiedSince = default, string? teamNumber = default, string? top = default);
        
        /// <summary>
        /// Event Rankings
        /// </summary>
        /// <remarks>
        /// The rankings API returns team ranking detail from a particular event in a particular season. Optionally, the &#x60;top&#x60; parameter can be added to the query string to request a subset of the rankings based on the highest ranked teams at the time of the request. Alternately, you can specify the &#x60;teamNumber&#x60; parameter to retrieve the ranking on one specific team. You cannot specify both a top and &#x60;teamNumber&#x60; in the same call.  **IMPORTANT: This endpoint use to return differently for 2015 vs other seasons. In the fall 2016 updates, this was changed, and all seasons of data now return in the genericized format specified below.**  In all response details, the \&quot;team\&quot; refers to the FRC Team that the ranking represents, as well as their various alliance partners in the matches they have played (i.e. scores in a single match are not calcualted by team, but by alliance). *See the FRC Game Manual for more information.*
        /// </remarks>
        /// <exception cref="FIRST.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="eventCode">(Required) **[REQUIRED] (string)** Case insensitive alphanumeric eventCode of the event from which the rankings are requested. Must be at least 3 characters.</param>
        /// <param name="season">**[REQUIRED] (int)** Numeric year of the event from which the award listings are requested. Must be 4 digits and greater than or equal to 2015, and less than or equal to the current year.</param>
        /// <param name="ifModifiedSince"> (optional)</param>
        /// <param name="teamNumber">**(int)** Optional team number of the team whose ranking is requested. (optional)</param>
        /// <param name="top">**(int)** Optional number of requested top ranked teams to return in result. (optional)</param>
        /// <returns>ApiResponse of Object</returns>
        ApiResponse<Object?> SeasonRankingsEventCodeGetWithHttpInfo(string eventCode, string season, string? ifModifiedSince = default, string? teamNumber = default, string? top = default);
      #endregion Synchronous Operations
    }
    
      /// <summary>
      /// Represents a collection of functions to interact with the API endpoints
      /// </summary>
      public interface IRankingsApiAsync : IApiAccessor
      {
        #region Asynchronous Operations
          /// <summary>
          /// District Rankings
          /// </summary>
          /// <remarks>
          /// The district rankings API returns team ranking detail from a particular team in a particular season. You *must* specify a districtCode unless a &#x60;teamNumber&#x60; is being specified. If a &#x60;teamNumber&#x60; is specified, do not include any other paramaters. Optionally, the &#x60;top&#x60; parameter can be added to the query string to request a subset of the rankings based on the highest ranked teams at the time of the request. Alternately, you can specify the &#x60;teamNumber&#x60; parameter to retrieve the ranking on one specific team. You cannot specify both a &#x60;top&#x60; and &#x60;teamNumber&#x60; in the same call. If you specify a &#x60;page&#x60;, you cannot specify a &#x60;top&#x60;.  This endpoint is only updated periodically, and may not reflect final rankings for an event/district until a period of time after a given event is completed. The final authority on teams advancing tournament levels is the District Ranking website and communications from *FIRST*, not this API. *See the FRC Game Manual for more information.*
          /// </remarks>
          /// <exception cref="FIRST.Client.ApiException">Thrown when fails to make API call</exception>
            /// <param name="season">**[REQUIRED] (int)** Numeric year of the event from which the award listings are requested. Must be 4 digits and greater than or equal to 2015, and less than or equal to the current year.</param>
            /// <param name="districtCode">**(string)** Case insensitive alphanumeric districtCode of the district from which the rankings are requested. Must be at least 2 characters.  District Codes: &#x60;&#x60;&#x60; FMA PNW NE FIN FNC ONT ISR CHS FIT PCH FIM &#x60;&#x60;&#x60; (optional)</param>
            /// <param name="ifModifiedSince"> (optional)</param>
            /// <param name="page">**(int)** Numeric page of results to return. If not included, page 1 will be returned. (optional)</param>
            /// <param name="teamNumber">**(int)** Optional team number of the team whose ranking is requested. (optional)</param>
            /// <param name="top">**(int)** Optional number of requested top ranked teams to return in result. (optional)</param>
          /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
          /// <returns>Task of Object</returns>
          Task<Object?> SeasonRankingsDistrictGetAsync(string season, string? districtCode = default, string? ifModifiedSince = default, string? page = default, string? teamNumber = default, string? top = default, CancellationToken cancellationToken = default);
            
            /// <summary>
            /// District Rankings
            /// </summary>
            /// <remarks>
            /// The district rankings API returns team ranking detail from a particular team in a particular season. You *must* specify a districtCode unless a &#x60;teamNumber&#x60; is being specified. If a &#x60;teamNumber&#x60; is specified, do not include any other paramaters. Optionally, the &#x60;top&#x60; parameter can be added to the query string to request a subset of the rankings based on the highest ranked teams at the time of the request. Alternately, you can specify the &#x60;teamNumber&#x60; parameter to retrieve the ranking on one specific team. You cannot specify both a &#x60;top&#x60; and &#x60;teamNumber&#x60; in the same call. If you specify a &#x60;page&#x60;, you cannot specify a &#x60;top&#x60;.  This endpoint is only updated periodically, and may not reflect final rankings for an event/district until a period of time after a given event is completed. The final authority on teams advancing tournament levels is the District Ranking website and communications from *FIRST*, not this API. *See the FRC Game Manual for more information.*
            /// </remarks>
            /// <exception cref="FIRST.Client.ApiException">Thrown when fails to make API call</exception>
              /// <param name="season">**[REQUIRED] (int)** Numeric year of the event from which the award listings are requested. Must be 4 digits and greater than or equal to 2015, and less than or equal to the current year.</param>
              /// <param name="districtCode">**(string)** Case insensitive alphanumeric districtCode of the district from which the rankings are requested. Must be at least 2 characters.  District Codes: &#x60;&#x60;&#x60; FMA PNW NE FIN FNC ONT ISR CHS FIT PCH FIM &#x60;&#x60;&#x60; (optional)</param>
              /// <param name="ifModifiedSince"> (optional)</param>
              /// <param name="page">**(int)** Numeric page of results to return. If not included, page 1 will be returned. (optional)</param>
              /// <param name="teamNumber">**(int)** Optional team number of the team whose ranking is requested. (optional)</param>
              /// <param name="top">**(int)** Optional number of requested top ranked teams to return in result. (optional)</param>
            /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
            /// <returns>Task of ApiResponse (Object)</returns>
            Task<ApiResponse<Object?>> SeasonRankingsDistrictGetWithHttpInfoAsync(string season, string? districtCode = default, string? ifModifiedSince = default, string? page = default, string? teamNumber = default, string? top = default, CancellationToken cancellationToken = default);
          /// <summary>
          /// Event Rankings
          /// </summary>
          /// <remarks>
          /// The rankings API returns team ranking detail from a particular event in a particular season. Optionally, the &#x60;top&#x60; parameter can be added to the query string to request a subset of the rankings based on the highest ranked teams at the time of the request. Alternately, you can specify the &#x60;teamNumber&#x60; parameter to retrieve the ranking on one specific team. You cannot specify both a top and &#x60;teamNumber&#x60; in the same call.  **IMPORTANT: This endpoint use to return differently for 2015 vs other seasons. In the fall 2016 updates, this was changed, and all seasons of data now return in the genericized format specified below.**  In all response details, the \&quot;team\&quot; refers to the FRC Team that the ranking represents, as well as their various alliance partners in the matches they have played (i.e. scores in a single match are not calcualted by team, but by alliance). *See the FRC Game Manual for more information.*
          /// </remarks>
          /// <exception cref="FIRST.Client.ApiException">Thrown when fails to make API call</exception>
            /// <param name="eventCode">(Required) **[REQUIRED] (string)** Case insensitive alphanumeric eventCode of the event from which the rankings are requested. Must be at least 3 characters.</param>
            /// <param name="season">**[REQUIRED] (int)** Numeric year of the event from which the award listings are requested. Must be 4 digits and greater than or equal to 2015, and less than or equal to the current year.</param>
            /// <param name="ifModifiedSince"> (optional)</param>
            /// <param name="teamNumber">**(int)** Optional team number of the team whose ranking is requested. (optional)</param>
            /// <param name="top">**(int)** Optional number of requested top ranked teams to return in result. (optional)</param>
          /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
          /// <returns>Task of Object</returns>
          Task<Object?> SeasonRankingsEventCodeGetAsync(string eventCode, string season, string? ifModifiedSince = default, string? teamNumber = default, string? top = default, CancellationToken cancellationToken = default);
            
            /// <summary>
            /// Event Rankings
            /// </summary>
            /// <remarks>
            /// The rankings API returns team ranking detail from a particular event in a particular season. Optionally, the &#x60;top&#x60; parameter can be added to the query string to request a subset of the rankings based on the highest ranked teams at the time of the request. Alternately, you can specify the &#x60;teamNumber&#x60; parameter to retrieve the ranking on one specific team. You cannot specify both a top and &#x60;teamNumber&#x60; in the same call.  **IMPORTANT: This endpoint use to return differently for 2015 vs other seasons. In the fall 2016 updates, this was changed, and all seasons of data now return in the genericized format specified below.**  In all response details, the \&quot;team\&quot; refers to the FRC Team that the ranking represents, as well as their various alliance partners in the matches they have played (i.e. scores in a single match are not calcualted by team, but by alliance). *See the FRC Game Manual for more information.*
            /// </remarks>
            /// <exception cref="FIRST.Client.ApiException">Thrown when fails to make API call</exception>
              /// <param name="eventCode">(Required) **[REQUIRED] (string)** Case insensitive alphanumeric eventCode of the event from which the rankings are requested. Must be at least 3 characters.</param>
              /// <param name="season">**[REQUIRED] (int)** Numeric year of the event from which the award listings are requested. Must be 4 digits and greater than or equal to 2015, and less than or equal to the current year.</param>
              /// <param name="ifModifiedSince"> (optional)</param>
              /// <param name="teamNumber">**(int)** Optional team number of the team whose ranking is requested. (optional)</param>
              /// <param name="top">**(int)** Optional number of requested top ranked teams to return in result. (optional)</param>
            /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
            /// <returns>Task of ApiResponse (Object)</returns>
            Task<ApiResponse<Object?>> SeasonRankingsEventCodeGetWithHttpInfoAsync(string eventCode, string season, string? ifModifiedSince = default, string? teamNumber = default, string? top = default, CancellationToken cancellationToken = default);
          #endregion Asynchronous Operations
        }
      
      /// <summary>
      /// Represents a collection of functions to interact with the API endpoints
      /// </summary>
      public interface IRankingsApi : IRankingsApiSync, IRankingsApiAsync { }
      
      /// <summary>
      /// Represents a collection of functions to interact with the API endpoints
      /// </summary>
      public sealed partial class RankingsApi : IRankingsApi
      {
        private ExceptionFactory? _exceptionFactory = (name, response) => null;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="RankingsApi"/> class.
        /// **IMPORTANT** This will also create an instance of HttpClient, which is less than ideal.
        /// It's better to reuse the <see href="https://docs.microsoft.com/en-us/dotnet/architecture/microservices/implement-resilient-applications/use-httpclientfactory-to-implement-resilient-http-requests#issues-with-the-original-httpclient-class-available-in-net">HttpClient and HttpClientHandler</see>.
        /// </summary>
        /// <returns></returns>
        public RankingsApi() : this(basePath: default) { }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="RankingsApi"/> class.
        /// **IMPORTANT** This will also create an instance of HttpClient, which is less than ideal.
        /// It's better to reuse the <see href="https://docs.microsoft.com/en-us/dotnet/architecture/microservices/implement-resilient-applications/use-httpclientfactory-to-implement-resilient-http-requests#issues-with-the-original-httpclient-class-available-in-net">HttpClient and HttpClientHandler</see>.
        /// </summary>
        /// <param name="basePath">The target service's base path in URL format.</param>
        /// <exception cref="ArgumentException"></exception>
        /// <returns></returns>
        public RankingsApi(string? basePath)
        {
          this.Configuration = FIRST.Client.Configuration.MergeConfigurations(GlobalConfiguration.Instance, new Configuration { BasePath = basePath });
          this.ApiClient = new ApiClient(this.Configuration.BasePath);
          this.Client = this.ApiClient;
            this.AsynchronousClient = this.ApiClient;
          
          this.ExceptionFactory = FIRST.Client.Configuration.DefaultExceptionFactory;
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="RankingsApi"/> class using Configuration object.
        /// **IMPORTANT** This will also create an instance of HttpClient, which is less than ideal.
        /// It's better to reuse the <see href="https://docs.microsoft.com/en-us/dotnet/architecture/microservices/implement-resilient-applications/use-httpclientfactory-to-implement-resilient-http-requests#issues-with-the-original-httpclient-class-available-in-net">HttpClient and HttpClientHandler</see>.
        /// </summary>
        /// <param name="configuration">An instance of Configuration.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public RankingsApi(Configuration configuration)
        {
          ArgumentNullException.ThrowIfNull(configuration);
          
          this.Configuration = FIRST.Client.Configuration.MergeConfigurations(GlobalConfiguration.Instance, configuration);
          this.ApiClient = new ApiClient(this.Configuration.BasePath);
          this.Client = this.ApiClient;
            this.AsynchronousClient = this.ApiClient;
          
          this.ExceptionFactory = FIRST.Client.Configuration.DefaultExceptionFactory;
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="RankingsApi"/> class.
        /// </summary>
        /// <param name="client">An instance of HttpClient.</param>
        /// <param name="handler">An optional instance of HttpClientHandler that is used by HttpClient.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        /// <remarks>
        /// Some configuration settings will not be applied without passing an HttpClientHandler.
        /// The features affected are: Setting and Retrieving Cookies, Client Certificates, Proxy settings.
        /// </remarks>
        public RankingsApi(HttpClient client, HttpClientHandler? handler = null) : this(client, basePath: default, handler: handler) { }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="RankingsApi"/> class.
        /// </summary>
        /// <param name="client">An instance of HttpClient.</param>
        /// <param name="basePath">The target service's base path in URL format.</param>
        /// <param name="handler">An optional instance of HttpClientHandler that is used by HttpClient.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <returns></returns>
        /// <remarks>
        /// Some configuration settings will not be applied without passing an HttpClientHandler.
        /// The features affected are: Setting and Retrieving Cookies, Client Certificates, Proxy settings.
        /// </remarks>
        public RankingsApi(HttpClient client, string? basePath, HttpClientHandler? handler = null)
        {
          ArgumentNullException.ThrowIfNull(client);
          
          this.Configuration = FIRST.Client.Configuration.MergeConfigurations(GlobalConfiguration.Instance, new Configuration { BasePath = basePath });
          this.ApiClient = new ApiClient(client, this.Configuration.BasePath, handler);
          this.Client = this.ApiClient;
            this.AsynchronousClient = this.ApiClient;
          
          this.ExceptionFactory = FIRST.Client.Configuration.DefaultExceptionFactory;
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="RankingsApi"/> class using Configuration object.
        /// </summary>
        /// <param name="client">An instance of HttpClient.</param>
        /// <param name="configuration">An instance of Configuration.</param>
        /// <param name="handler">An optional instance of HttpClientHandler that is used by HttpClient.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        /// <remarks>
        /// Some configuration settings will not be applied without passing an HttpClientHandler.
        /// The features affected are: Setting and Retrieving Cookies, Client Certificates, Proxy settings.
        /// </remarks>
        public RankingsApi(HttpClient client, Configuration configuration, HttpClientHandler? handler = null)
        {
          ArgumentNullException.ThrowIfNull(configuration);
          ArgumentNullException.ThrowIfNull(client);
          
          this.Configuration = FIRST.Client.Configuration.MergeConfigurations(GlobalConfiguration.Instance, configuration);
          this.ApiClient = new ApiClient(client, this.Configuration.BasePath, handler);
          this.Client = this.ApiClient;
            this.AsynchronousClient = this.ApiClient;
          
          this.ExceptionFactory = FIRST.Client.Configuration.DefaultExceptionFactory;
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="RankingsApi"/> class
        /// using a Configuration object and client instance.
        /// </summary>
        /// <param name="client">The client interface for synchronous API access.</param>
        /// <param name="asyncClient">The client interface for asynchronous API access.</param>
        /// <param name="configuration">The configuration object.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public RankingsApi(ISynchronousClient client, IAsynchronousClient asyncClient, IReadableConfiguration configuration)
        {
          ArgumentNullException.ThrowIfNull(client);
          
            ArgumentNullException.ThrowIfNull(asyncClient);
            this.AsynchronousClient = asyncClient;
            
          ArgumentNullException.ThrowIfNull(configuration);
          this.Configuration = configuration;
          
          this.Client = client;
          this.ExceptionFactory = FIRST.Client.Configuration.DefaultExceptionFactory;
        }
        
        /// <summary>
        /// Holds the ApiClient if created
        /// </summary>
        public ApiClient? ApiClient { get; set; }
        
          /// <summary>
          /// The client for accessing this underlying API asynchronously.
          /// </summary>
          public IAsynchronousClient AsynchronousClient { get; set; }
        
        /// <summary>
        /// The client for accessing this underlying API synchronously.
        /// </summary>
        public ISynchronousClient Client { get; set; }
        
        /// <summary>
        /// Gets the base path of the API client.
        /// </summary>
        /// <value>The base path</value>
        public string? GetBasePath() => this.Configuration.BasePath;
        
        /// <summary>
        /// Gets or sets the configuration object
        /// </summary>
        /// <value>An instance of the Configuration</value>
        public IReadableConfiguration Configuration { get; set; }
        
        /// <summary>
        /// Provides a factory method hook for the creation of exceptions.
        /// </summary>
        public ExceptionFactory? ExceptionFactory
        {
          get
          {
            return _exceptionFactory is not null && _exceptionFactory.GetInvocationList().Length > 1
            ? throw new InvalidOperationException("Multicast delegate for ExceptionFactory is unsupported.")
            : _exceptionFactory;
          }
          set => _exceptionFactory = value;
        }
        
          /// <summary>
          /// District Rankings The district rankings API returns team ranking detail from a particular team in a particular season. You *must* specify a districtCode unless a &#x60;teamNumber&#x60; is being specified. If a &#x60;teamNumber&#x60; is specified, do not include any other paramaters. Optionally, the &#x60;top&#x60; parameter can be added to the query string to request a subset of the rankings based on the highest ranked teams at the time of the request. Alternately, you can specify the &#x60;teamNumber&#x60; parameter to retrieve the ranking on one specific team. You cannot specify both a &#x60;top&#x60; and &#x60;teamNumber&#x60; in the same call. If you specify a &#x60;page&#x60;, you cannot specify a &#x60;top&#x60;.  This endpoint is only updated periodically, and may not reflect final rankings for an event/district until a period of time after a given event is completed. The final authority on teams advancing tournament levels is the District Ranking website and communications from *FIRST*, not this API. *See the FRC Game Manual for more information.*
          /// </summary>
          /// <exception cref="FIRST.Client.ApiException">Thrown when fails to make API call</exception>
          /// <param name="season">**[REQUIRED] (int)** Numeric year of the event from which the award listings are requested. Must be 4 digits and greater than or equal to 2015, and less than or equal to the current year.</param>
          /// <param name="districtCode">**(string)** Case insensitive alphanumeric districtCode of the district from which the rankings are requested. Must be at least 2 characters.  District Codes: &#x60;&#x60;&#x60; FMA PNW NE FIN FNC ONT ISR CHS FIT PCH FIM &#x60;&#x60;&#x60; (optional)</param>
          /// <param name="ifModifiedSince"> (optional)</param>
          /// <param name="page">**(int)** Numeric page of results to return. If not included, page 1 will be returned. (optional)</param>
          /// <param name="teamNumber">**(int)** Optional team number of the team whose ranking is requested. (optional)</param>
          /// <param name="top">**(int)** Optional number of requested top ranked teams to return in result. (optional)</param>
          /// <returns>Object</returns>
          public Object? SeasonRankingsDistrictGet(string season, string? districtCode = default, string? ifModifiedSince = default, string? page = default, string? teamNumber = default, string? top = default)
          {
            FIRST.Client.ApiResponse<Object?> localVarResponse = SeasonRankingsDistrictGetWithHttpInfo(season, districtCode, ifModifiedSince, page, teamNumber, top);
              return localVarResponse.Data;
            }
            
            /// <summary>
            /// District Rankings The district rankings API returns team ranking detail from a particular team in a particular season. You *must* specify a districtCode unless a &#x60;teamNumber&#x60; is being specified. If a &#x60;teamNumber&#x60; is specified, do not include any other paramaters. Optionally, the &#x60;top&#x60; parameter can be added to the query string to request a subset of the rankings based on the highest ranked teams at the time of the request. Alternately, you can specify the &#x60;teamNumber&#x60; parameter to retrieve the ranking on one specific team. You cannot specify both a &#x60;top&#x60; and &#x60;teamNumber&#x60; in the same call. If you specify a &#x60;page&#x60;, you cannot specify a &#x60;top&#x60;.  This endpoint is only updated periodically, and may not reflect final rankings for an event/district until a period of time after a given event is completed. The final authority on teams advancing tournament levels is the District Ranking website and communications from *FIRST*, not this API. *See the FRC Game Manual for more information.*
            /// </summary>
            /// <exception cref="FIRST.Client.ApiException">Thrown when fails to make API call</exception>
            /// <param name="season">**[REQUIRED] (int)** Numeric year of the event from which the award listings are requested. Must be 4 digits and greater than or equal to 2015, and less than or equal to the current year.</param>
            /// <param name="districtCode">**(string)** Case insensitive alphanumeric districtCode of the district from which the rankings are requested. Must be at least 2 characters.  District Codes: &#x60;&#x60;&#x60; FMA PNW NE FIN FNC ONT ISR CHS FIT PCH FIM &#x60;&#x60;&#x60; (optional)</param>
            /// <param name="ifModifiedSince"> (optional)</param>
            /// <param name="page">**(int)** Numeric page of results to return. If not included, page 1 will be returned. (optional)</param>
            /// <param name="teamNumber">**(int)** Optional team number of the team whose ranking is requested. (optional)</param>
            /// <param name="top">**(int)** Optional number of requested top ranked teams to return in result. (optional)</param>
            /// <returns>ApiResponse of Object</returns>
            public ApiResponse<Object?> SeasonRankingsDistrictGetWithHttpInfo(string season, string? districtCode = default, string? ifModifiedSince = default, string? page = default, string? teamNumber = default, string? top = default)
            {
                    // verify the required parameter 'season' is set
                    if (season is null)
                    {
                      throw new ApiException(400, "Missing required parameter 'season' when calling RankingsApi->SeasonRankingsDistrictGet");
                    }
                    
              RequestOptions localVarRequestOptions = new();
              
              string[] _contentTypes = [
              ];
              
              // to determine the Accept header
              string[] _accepts = [
                  "application/json"
              ];
              
              var localVarContentType = ClientUtils.SelectHeaderContentType(_contentTypes);
              if (localVarContentType is not null)
              {
                localVarRequestOptions.HeaderParameters.Add("Content-Type", localVarContentType);
              }
              
              var localVarAccept = ClientUtils.SelectHeaderAccept(_accepts);
              if (localVarAccept is not null)
              {
                localVarRequestOptions.HeaderParameters.Add("Accept", localVarAccept);
              }
              
                  localVarRequestOptions.PathParameters.Add("season", ClientUtils.ParameterToString(season)); // path parameter
                  if (districtCode is not null)
                  {
                      localVarRequestOptions.QueryParameters.Add(ClientUtils.ParameterToMultiMap("", "districtCode", districtCode));
                  }
                  
                  if (page is not null)
                  {
                      localVarRequestOptions.QueryParameters.Add(ClientUtils.ParameterToMultiMap("", "page", page));
                  }
                  
                  if (teamNumber is not null)
                  {
                      localVarRequestOptions.QueryParameters.Add(ClientUtils.ParameterToMultiMap("", "teamNumber", teamNumber));
                  }
                  
                  if (top is not null)
                  {
                      localVarRequestOptions.QueryParameters.Add(ClientUtils.ParameterToMultiMap("", "top", top));
                  }
                  
                  if (ifModifiedSince is not null)
                  {
                    localVarRequestOptions.HeaderParameters.Add("If-Modified-Since", ClientUtils.ParameterToString(ifModifiedSince)); // header parameter
                  }
                  
                              // authentication (basicAuth) required
                  // http basic authentication required
                  if (!string.IsNullOrEmpty(this.Configuration.Username) || (!string.IsNullOrEmpty(this.Configuration.Password) && !localVarRequestOptions.HeaderParameters.ContainsKey("Authorization")))
                  {
                    localVarRequestOptions.HeaderParameters.Add("Authorization", "Basic " + ClientUtils.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password));
                  }
                  
              
              // make the HTTP request
              var localVarResponse = this.Client.Get<Object?>("/{season}/rankings/district", localVarRequestOptions, this.Configuration);
              
              if (this.ExceptionFactory is not null)
              {
                var _exception = this.ExceptionFactory("SeasonRankingsDistrictGet", localVarResponse);
                if (_exception is not null)
                {
                  throw _exception;
                }
              }
              
              return localVarResponse;
            }
            
            /// <summary>
            /// District Rankings The district rankings API returns team ranking detail from a particular team in a particular season. You *must* specify a districtCode unless a &#x60;teamNumber&#x60; is being specified. If a &#x60;teamNumber&#x60; is specified, do not include any other paramaters. Optionally, the &#x60;top&#x60; parameter can be added to the query string to request a subset of the rankings based on the highest ranked teams at the time of the request. Alternately, you can specify the &#x60;teamNumber&#x60; parameter to retrieve the ranking on one specific team. You cannot specify both a &#x60;top&#x60; and &#x60;teamNumber&#x60; in the same call. If you specify a &#x60;page&#x60;, you cannot specify a &#x60;top&#x60;.  This endpoint is only updated periodically, and may not reflect final rankings for an event/district until a period of time after a given event is completed. The final authority on teams advancing tournament levels is the District Ranking website and communications from *FIRST*, not this API. *See the FRC Game Manual for more information.*
            /// </summary>
            /// <exception cref="FIRST.Client.ApiException">Thrown when fails to make API call</exception>
              /// <param name="season">**[REQUIRED] (int)** Numeric year of the event from which the award listings are requested. Must be 4 digits and greater than or equal to 2015, and less than or equal to the current year.</param>
              /// <param name="districtCode">**(string)** Case insensitive alphanumeric districtCode of the district from which the rankings are requested. Must be at least 2 characters.  District Codes: &#x60;&#x60;&#x60; FMA PNW NE FIN FNC ONT ISR CHS FIT PCH FIM &#x60;&#x60;&#x60; (optional)</param>
              /// <param name="ifModifiedSince"> (optional)</param>
              /// <param name="page">**(int)** Numeric page of results to return. If not included, page 1 will be returned. (optional)</param>
              /// <param name="teamNumber">**(int)** Optional team number of the team whose ranking is requested. (optional)</param>
              /// <param name="top">**(int)** Optional number of requested top ranked teams to return in result. (optional)</param>
            /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
            /// <returns>Task of Object</returns>
            public async Task<Object?> SeasonRankingsDistrictGetAsync(string season, string? districtCode = default, string? ifModifiedSince = default, string? page = default, string? teamNumber = default, string? top = default, CancellationToken cancellationToken = default)
            {
              FIRST.Client.ApiResponse<Object?> localVarResponse = await SeasonRankingsDistrictGetWithHttpInfoAsync(season, districtCode, ifModifiedSince, page, teamNumber, top, cancellationToken).ConfigureAwait(false);
                return localVarResponse.Data;
              }
              
              /// <summary>
              /// District Rankings The district rankings API returns team ranking detail from a particular team in a particular season. You *must* specify a districtCode unless a &#x60;teamNumber&#x60; is being specified. If a &#x60;teamNumber&#x60; is specified, do not include any other paramaters. Optionally, the &#x60;top&#x60; parameter can be added to the query string to request a subset of the rankings based on the highest ranked teams at the time of the request. Alternately, you can specify the &#x60;teamNumber&#x60; parameter to retrieve the ranking on one specific team. You cannot specify both a &#x60;top&#x60; and &#x60;teamNumber&#x60; in the same call. If you specify a &#x60;page&#x60;, you cannot specify a &#x60;top&#x60;.  This endpoint is only updated periodically, and may not reflect final rankings for an event/district until a period of time after a given event is completed. The final authority on teams advancing tournament levels is the District Ranking website and communications from *FIRST*, not this API. *See the FRC Game Manual for more information.*
              /// </summary>
              /// <exception cref="FIRST.Client.ApiException">Thrown when fails to make API call</exception>
                /// <param name="season">**[REQUIRED] (int)** Numeric year of the event from which the award listings are requested. Must be 4 digits and greater than or equal to 2015, and less than or equal to the current year.</param>
                /// <param name="districtCode">**(string)** Case insensitive alphanumeric districtCode of the district from which the rankings are requested. Must be at least 2 characters.  District Codes: &#x60;&#x60;&#x60; FMA PNW NE FIN FNC ONT ISR CHS FIT PCH FIM &#x60;&#x60;&#x60; (optional)</param>
                /// <param name="ifModifiedSince"> (optional)</param>
                /// <param name="page">**(int)** Numeric page of results to return. If not included, page 1 will be returned. (optional)</param>
                /// <param name="teamNumber">**(int)** Optional team number of the team whose ranking is requested. (optional)</param>
                /// <param name="top">**(int)** Optional number of requested top ranked teams to return in result. (optional)</param>
              /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
              /// <returns>Task of ApiResponse (Object)</returns>
              public async Task<FIRST.Client.ApiResponse<Object?>> SeasonRankingsDistrictGetWithHttpInfoAsync(string season, string? districtCode = default, string? ifModifiedSince = default, string? page = default, string? teamNumber = default, string? top = default, CancellationToken cancellationToken = default)
              {
                      // verify the required parameter 'season' is set
                      if (season is null)
                      {
                        throw new ApiException(400, "Missing required parameter 'season' when calling RankingsApi->SeasonRankingsDistrictGet");
                      }
                      
                RequestOptions localVarRequestOptions = new();
                
                string[] _contentTypes = [
                ];
                
                // to determine the Accept header
                string[] _accepts = [
                    "application/json"
                ];
                
                var localVarContentType = ClientUtils.SelectHeaderContentType(_contentTypes);
                if (localVarContentType is not null)
                {
                  localVarRequestOptions.HeaderParameters.Add("Content-Type", localVarContentType);
                }
                
                var localVarAccept = ClientUtils.SelectHeaderAccept(_accepts);
                if (localVarAccept is not null)
                {
                  localVarRequestOptions.HeaderParameters.Add("Accept", localVarAccept);
                }
                
                    localVarRequestOptions.PathParameters.Add("season", ClientUtils.ParameterToString(season)); // path parameter
                    if (districtCode is not null)
                    {
                      localVarRequestOptions.QueryParameters.Add(ClientUtils.ParameterToMultiMap("", "districtCode", districtCode));
                    }
                    
                    if (page is not null)
                    {
                      localVarRequestOptions.QueryParameters.Add(ClientUtils.ParameterToMultiMap("", "page", page));
                    }
                    
                    if (teamNumber is not null)
                    {
                      localVarRequestOptions.QueryParameters.Add(ClientUtils.ParameterToMultiMap("", "teamNumber", teamNumber));
                    }
                    
                    if (top is not null)
                    {
                      localVarRequestOptions.QueryParameters.Add(ClientUtils.ParameterToMultiMap("", "top", top));
                    }
                    
                    if (ifModifiedSince is not null)
                    {
                      localVarRequestOptions.HeaderParameters.Add("If-Modified-Since", ClientUtils.ParameterToString(ifModifiedSince)); // header parameter
                    }
                    
                                  // authentication (basicAuth) required
                      // http basic authentication required
                      if (!string.IsNullOrEmpty(this.Configuration.Username) || (!string.IsNullOrEmpty(this.Configuration.Password) && !localVarRequestOptions.HeaderParameters.ContainsKey("Authorization")))
                      
                      {
                        localVarRequestOptions.HeaderParameters.Add("Authorization", "Basic " + ClientUtils.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password));
                      }
                      
                // make the HTTP request
                var localVarResponse = await this.AsynchronousClient.GetAsync<Object?>("/{season}/rankings/district", localVarRequestOptions, this.Configuration, cancellationToken).ConfigureAwait(false);
                
                if (this.ExceptionFactory is not null)
                {
                  var _exception = this.ExceptionFactory("SeasonRankingsDistrictGet", localVarResponse);
                  if (_exception is not null)
                  {
                    throw _exception;
                  }
                }
                
                return localVarResponse;
              }          /// <summary>
          /// Event Rankings The rankings API returns team ranking detail from a particular event in a particular season. Optionally, the &#x60;top&#x60; parameter can be added to the query string to request a subset of the rankings based on the highest ranked teams at the time of the request. Alternately, you can specify the &#x60;teamNumber&#x60; parameter to retrieve the ranking on one specific team. You cannot specify both a top and &#x60;teamNumber&#x60; in the same call.  **IMPORTANT: This endpoint use to return differently for 2015 vs other seasons. In the fall 2016 updates, this was changed, and all seasons of data now return in the genericized format specified below.**  In all response details, the \&quot;team\&quot; refers to the FRC Team that the ranking represents, as well as their various alliance partners in the matches they have played (i.e. scores in a single match are not calcualted by team, but by alliance). *See the FRC Game Manual for more information.*
          /// </summary>
          /// <exception cref="FIRST.Client.ApiException">Thrown when fails to make API call</exception>
          /// <param name="eventCode">(Required) **[REQUIRED] (string)** Case insensitive alphanumeric eventCode of the event from which the rankings are requested. Must be at least 3 characters.</param>
          /// <param name="season">**[REQUIRED] (int)** Numeric year of the event from which the award listings are requested. Must be 4 digits and greater than or equal to 2015, and less than or equal to the current year.</param>
          /// <param name="ifModifiedSince"> (optional)</param>
          /// <param name="teamNumber">**(int)** Optional team number of the team whose ranking is requested. (optional)</param>
          /// <param name="top">**(int)** Optional number of requested top ranked teams to return in result. (optional)</param>
          /// <returns>Object</returns>
          public Object? SeasonRankingsEventCodeGet(string eventCode, string season, string? ifModifiedSince = default, string? teamNumber = default, string? top = default)
          {
            FIRST.Client.ApiResponse<Object?> localVarResponse = SeasonRankingsEventCodeGetWithHttpInfo(eventCode, season, ifModifiedSince, teamNumber, top);
              return localVarResponse.Data;
            }
            
            /// <summary>
            /// Event Rankings The rankings API returns team ranking detail from a particular event in a particular season. Optionally, the &#x60;top&#x60; parameter can be added to the query string to request a subset of the rankings based on the highest ranked teams at the time of the request. Alternately, you can specify the &#x60;teamNumber&#x60; parameter to retrieve the ranking on one specific team. You cannot specify both a top and &#x60;teamNumber&#x60; in the same call.  **IMPORTANT: This endpoint use to return differently for 2015 vs other seasons. In the fall 2016 updates, this was changed, and all seasons of data now return in the genericized format specified below.**  In all response details, the \&quot;team\&quot; refers to the FRC Team that the ranking represents, as well as their various alliance partners in the matches they have played (i.e. scores in a single match are not calcualted by team, but by alliance). *See the FRC Game Manual for more information.*
            /// </summary>
            /// <exception cref="FIRST.Client.ApiException">Thrown when fails to make API call</exception>
            /// <param name="eventCode">(Required) **[REQUIRED] (string)** Case insensitive alphanumeric eventCode of the event from which the rankings are requested. Must be at least 3 characters.</param>
            /// <param name="season">**[REQUIRED] (int)** Numeric year of the event from which the award listings are requested. Must be 4 digits and greater than or equal to 2015, and less than or equal to the current year.</param>
            /// <param name="ifModifiedSince"> (optional)</param>
            /// <param name="teamNumber">**(int)** Optional team number of the team whose ranking is requested. (optional)</param>
            /// <param name="top">**(int)** Optional number of requested top ranked teams to return in result. (optional)</param>
            /// <returns>ApiResponse of Object</returns>
            public ApiResponse<Object?> SeasonRankingsEventCodeGetWithHttpInfo(string eventCode, string season, string? ifModifiedSince = default, string? teamNumber = default, string? top = default)
            {
                    // verify the required parameter 'eventCode' is set
                    if (eventCode is null)
                    {
                      throw new ApiException(400, "Missing required parameter 'eventCode' when calling RankingsApi->SeasonRankingsEventCodeGet");
                    }
                    
                    // verify the required parameter 'season' is set
                    if (season is null)
                    {
                      throw new ApiException(400, "Missing required parameter 'season' when calling RankingsApi->SeasonRankingsEventCodeGet");
                    }
                    
              RequestOptions localVarRequestOptions = new();
              
              string[] _contentTypes = [
              ];
              
              // to determine the Accept header
              string[] _accepts = [
                  "application/json"
              ];
              
              var localVarContentType = ClientUtils.SelectHeaderContentType(_contentTypes);
              if (localVarContentType is not null)
              {
                localVarRequestOptions.HeaderParameters.Add("Content-Type", localVarContentType);
              }
              
              var localVarAccept = ClientUtils.SelectHeaderAccept(_accepts);
              if (localVarAccept is not null)
              {
                localVarRequestOptions.HeaderParameters.Add("Accept", localVarAccept);
              }
              
                  localVarRequestOptions.PathParameters.Add("eventCode", ClientUtils.ParameterToString(eventCode)); // path parameter
                  localVarRequestOptions.PathParameters.Add("season", ClientUtils.ParameterToString(season)); // path parameter
                  if (teamNumber is not null)
                  {
                      localVarRequestOptions.QueryParameters.Add(ClientUtils.ParameterToMultiMap("", "teamNumber", teamNumber));
                  }
                  
                  if (top is not null)
                  {
                      localVarRequestOptions.QueryParameters.Add(ClientUtils.ParameterToMultiMap("", "top", top));
                  }
                  
                  if (ifModifiedSince is not null)
                  {
                    localVarRequestOptions.HeaderParameters.Add("If-Modified-Since", ClientUtils.ParameterToString(ifModifiedSince)); // header parameter
                  }
                  
                              // authentication (basicAuth) required
                  // http basic authentication required
                  if (!string.IsNullOrEmpty(this.Configuration.Username) || (!string.IsNullOrEmpty(this.Configuration.Password) && !localVarRequestOptions.HeaderParameters.ContainsKey("Authorization")))
                  {
                    localVarRequestOptions.HeaderParameters.Add("Authorization", "Basic " + ClientUtils.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password));
                  }
                  
              
              // make the HTTP request
              var localVarResponse = this.Client.Get<Object?>("/{season}/rankings/{eventCode}", localVarRequestOptions, this.Configuration);
              
              if (this.ExceptionFactory is not null)
              {
                var _exception = this.ExceptionFactory("SeasonRankingsEventCodeGet", localVarResponse);
                if (_exception is not null)
                {
                  throw _exception;
                }
              }
              
              return localVarResponse;
            }
            
            /// <summary>
            /// Event Rankings The rankings API returns team ranking detail from a particular event in a particular season. Optionally, the &#x60;top&#x60; parameter can be added to the query string to request a subset of the rankings based on the highest ranked teams at the time of the request. Alternately, you can specify the &#x60;teamNumber&#x60; parameter to retrieve the ranking on one specific team. You cannot specify both a top and &#x60;teamNumber&#x60; in the same call.  **IMPORTANT: This endpoint use to return differently for 2015 vs other seasons. In the fall 2016 updates, this was changed, and all seasons of data now return in the genericized format specified below.**  In all response details, the \&quot;team\&quot; refers to the FRC Team that the ranking represents, as well as their various alliance partners in the matches they have played (i.e. scores in a single match are not calcualted by team, but by alliance). *See the FRC Game Manual for more information.*
            /// </summary>
            /// <exception cref="FIRST.Client.ApiException">Thrown when fails to make API call</exception>
              /// <param name="eventCode">(Required) **[REQUIRED] (string)** Case insensitive alphanumeric eventCode of the event from which the rankings are requested. Must be at least 3 characters.</param>
              /// <param name="season">**[REQUIRED] (int)** Numeric year of the event from which the award listings are requested. Must be 4 digits and greater than or equal to 2015, and less than or equal to the current year.</param>
              /// <param name="ifModifiedSince"> (optional)</param>
              /// <param name="teamNumber">**(int)** Optional team number of the team whose ranking is requested. (optional)</param>
              /// <param name="top">**(int)** Optional number of requested top ranked teams to return in result. (optional)</param>
            /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
            /// <returns>Task of Object</returns>
            public async Task<Object?> SeasonRankingsEventCodeGetAsync(string eventCode, string season, string? ifModifiedSince = default, string? teamNumber = default, string? top = default, CancellationToken cancellationToken = default)
            {
              FIRST.Client.ApiResponse<Object?> localVarResponse = await SeasonRankingsEventCodeGetWithHttpInfoAsync(eventCode, season, ifModifiedSince, teamNumber, top, cancellationToken).ConfigureAwait(false);
                return localVarResponse.Data;
              }
              
              /// <summary>
              /// Event Rankings The rankings API returns team ranking detail from a particular event in a particular season. Optionally, the &#x60;top&#x60; parameter can be added to the query string to request a subset of the rankings based on the highest ranked teams at the time of the request. Alternately, you can specify the &#x60;teamNumber&#x60; parameter to retrieve the ranking on one specific team. You cannot specify both a top and &#x60;teamNumber&#x60; in the same call.  **IMPORTANT: This endpoint use to return differently for 2015 vs other seasons. In the fall 2016 updates, this was changed, and all seasons of data now return in the genericized format specified below.**  In all response details, the \&quot;team\&quot; refers to the FRC Team that the ranking represents, as well as their various alliance partners in the matches they have played (i.e. scores in a single match are not calcualted by team, but by alliance). *See the FRC Game Manual for more information.*
              /// </summary>
              /// <exception cref="FIRST.Client.ApiException">Thrown when fails to make API call</exception>
                /// <param name="eventCode">(Required) **[REQUIRED] (string)** Case insensitive alphanumeric eventCode of the event from which the rankings are requested. Must be at least 3 characters.</param>
                /// <param name="season">**[REQUIRED] (int)** Numeric year of the event from which the award listings are requested. Must be 4 digits and greater than or equal to 2015, and less than or equal to the current year.</param>
                /// <param name="ifModifiedSince"> (optional)</param>
                /// <param name="teamNumber">**(int)** Optional team number of the team whose ranking is requested. (optional)</param>
                /// <param name="top">**(int)** Optional number of requested top ranked teams to return in result. (optional)</param>
              /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
              /// <returns>Task of ApiResponse (Object)</returns>
              public async Task<FIRST.Client.ApiResponse<Object?>> SeasonRankingsEventCodeGetWithHttpInfoAsync(string eventCode, string season, string? ifModifiedSince = default, string? teamNumber = default, string? top = default, CancellationToken cancellationToken = default)
              {
                      // verify the required parameter 'eventCode' is set
                      if (eventCode is null)
                      {
                        throw new ApiException(400, "Missing required parameter 'eventCode' when calling RankingsApi->SeasonRankingsEventCodeGet");
                      }
                      
                      // verify the required parameter 'season' is set
                      if (season is null)
                      {
                        throw new ApiException(400, "Missing required parameter 'season' when calling RankingsApi->SeasonRankingsEventCodeGet");
                      }
                      
                RequestOptions localVarRequestOptions = new();
                
                string[] _contentTypes = [
                ];
                
                // to determine the Accept header
                string[] _accepts = [
                    "application/json"
                ];
                
                var localVarContentType = ClientUtils.SelectHeaderContentType(_contentTypes);
                if (localVarContentType is not null)
                {
                  localVarRequestOptions.HeaderParameters.Add("Content-Type", localVarContentType);
                }
                
                var localVarAccept = ClientUtils.SelectHeaderAccept(_accepts);
                if (localVarAccept is not null)
                {
                  localVarRequestOptions.HeaderParameters.Add("Accept", localVarAccept);
                }
                
                    localVarRequestOptions.PathParameters.Add("eventCode", ClientUtils.ParameterToString(eventCode)); // path parameter
                    localVarRequestOptions.PathParameters.Add("season", ClientUtils.ParameterToString(season)); // path parameter
                    if (teamNumber is not null)
                    {
                      localVarRequestOptions.QueryParameters.Add(ClientUtils.ParameterToMultiMap("", "teamNumber", teamNumber));
                    }
                    
                    if (top is not null)
                    {
                      localVarRequestOptions.QueryParameters.Add(ClientUtils.ParameterToMultiMap("", "top", top));
                    }
                    
                    if (ifModifiedSince is not null)
                    {
                      localVarRequestOptions.HeaderParameters.Add("If-Modified-Since", ClientUtils.ParameterToString(ifModifiedSince)); // header parameter
                    }
                    
                                  // authentication (basicAuth) required
                      // http basic authentication required
                      if (!string.IsNullOrEmpty(this.Configuration.Username) || (!string.IsNullOrEmpty(this.Configuration.Password) && !localVarRequestOptions.HeaderParameters.ContainsKey("Authorization")))
                      
                      {
                        localVarRequestOptions.HeaderParameters.Add("Authorization", "Basic " + ClientUtils.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password));
                      }
                      
                // make the HTTP request
                var localVarResponse = await this.AsynchronousClient.GetAsync<Object?>("/{season}/rankings/{eventCode}", localVarRequestOptions, this.Configuration, cancellationToken).ConfigureAwait(false);
                
                if (this.ExceptionFactory is not null)
                {
                  var _exception = this.ExceptionFactory("SeasonRankingsEventCodeGet", localVarResponse);
                  if (_exception is not null)
                  {
                    throw _exception;
                  }
                }
                
                return localVarResponse;
              }
            }
