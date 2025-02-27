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
  public interface ISeasonDataApiSync : IApiAccessor
  {
    #region Synchronous Operations
      /// <summary>
      /// Team Avatar Listings
      /// </summary>
        /// <remarks>
        /// This endpoint applies only to the 2018 or later seasons. Requests for other seasons will result in a &#x60;Bad Season&#x60; error. The team avatar listings API returns all FRC official teams in a particular &#x60;season&#x60; with, if applicable, their Avatar. If specified, the &#x60;teamNumber&#x60; parameter will return only one result with the details of the requested &#x60;teamNumber&#x60;. Alternately, the &#x60;eventCode&#x60; parameter allows sorting of the team list to only those teams attending a particular event in the particular &#x60;season&#x60;. If you specify a &#x60;teamNumber&#x60; parameter, you cannot additionally specify an &#x60;eventCode&#x60; or you will receive an &#x60;HTTP 501&#x60;. If a team does not have an Avatar, the return value will be &#x60;null&#x60;. Please note that the returned Avatar is encoded in the reply, and would need to be properly handled in order to render a PNG image.
        /// </remarks>
      /// <exception cref="FIRST.Client.ApiException">Thrown when fails to make API call</exception>
      /// <param name="season">**[REQUIRED] (int)** Numeric year of the event from which the award listings are requested. Must be 4 digits and greater than or equal to 2015, and less than or equal to the current year.</param>
      /// <param name="eventCode">**(string)** Case insensitive alphanumeric eventCode of the event from which details are requested. (optional)</param>
      /// <param name="ifModifiedSince"> (optional)</param>
      /// <param name="page">**(int)** Numeric page of results to return. If not included, page 1 will be returned. (optional)</param>
      /// <param name="teamNumber">**(int)** Numeric teamNumber of the team about which information is requested. Must be 1 to 4 digits. (optional)</param>
      /// <returns>Object</returns>
      Object? SeasonAvatarsGet(string season, string? eventCode = default, string? ifModifiedSince = default, string? page = default, string? teamNumber = default);

    /// <summary>
    /// Team Avatar Listings
    /// </summary>
    /// <remarks>
    /// This endpoint applies only to the 2018 or later seasons. Requests for other seasons will result in a &#x60;Bad Season&#x60; error. The team avatar listings API returns all FRC official teams in a particular &#x60;season&#x60; with, if applicable, their Avatar. If specified, the &#x60;teamNumber&#x60; parameter will return only one result with the details of the requested &#x60;teamNumber&#x60;. Alternately, the &#x60;eventCode&#x60; parameter allows sorting of the team list to only those teams attending a particular event in the particular &#x60;season&#x60;. If you specify a &#x60;teamNumber&#x60; parameter, you cannot additionally specify an &#x60;eventCode&#x60; or you will receive an &#x60;HTTP 501&#x60;. If a team does not have an Avatar, the return value will be &#x60;null&#x60;. Please note that the returned Avatar is encoded in the reply, and would need to be properly handled in order to render a PNG image.
    /// </remarks>
    /// <exception cref="ApiException">Thrown when fails to make API call</exception>
    /// <param name="season">**[REQUIRED] (int)** Numeric year of the event from which the award listings are requested. Must be 4 digits and greater than or equal to 2015, and less than or equal to the current year.</param>
    /// <param name="eventCode">**(string)** Case insensitive alphanumeric eventCode of the event from which details are requested. (optional)</param>
    /// <param name="ifModifiedSince"> (optional)</param>
    /// <param name="page">**(int)** Numeric page of results to return. If not included, page 1 will be returned. (optional)</param>
    /// <param name="teamNumber">**(int)** Numeric teamNumber of the team about which information is requested. Must be 1 to 4 digits. (optional)</param>
    /// <returns>ApiResponse of Object</returns>
    ApiResponse<Object?> SeasonAvatarsGetWithHttpInfo(string season, string? eventCode = default, string? ifModifiedSince = default, string? page = default, string? teamNumber = default);
      /// <summary>
      /// District Listings
      /// </summary>
        /// <remarks>
        /// The district listings API returns all FRC official districts of a particular season.
        /// </remarks>
      /// <exception cref="FIRST.Client.ApiException">Thrown when fails to make API call</exception>
      /// <param name="season">**[REQUIRED] (int)** Numeric year of the event from which the award listings are requested. Must be 4 digits and greater than or equal to 2015, and less than or equal to the current year.</param>
      /// <param name="ifModifiedSince"> (optional)</param>
      /// <returns>Object</returns>
      Object? SeasonDistrictsGet(string season, string? ifModifiedSince = default);

    /// <summary>
    /// District Listings
    /// </summary>
    /// <remarks>
    /// The district listings API returns all FRC official districts of a particular season.
    /// </remarks>
    /// <exception cref="ApiException">Thrown when fails to make API call</exception>
    /// <param name="season">**[REQUIRED] (int)** Numeric year of the event from which the award listings are requested. Must be 4 digits and greater than or equal to 2015, and less than or equal to the current year.</param>
    /// <param name="ifModifiedSince"> (optional)</param>
    /// <returns>ApiResponse of Object</returns>
    ApiResponse<Object?> SeasonDistrictsGetWithHttpInfo(string season, string? ifModifiedSince = default);
      /// <summary>
      /// Event Listings
      /// </summary>
        /// <remarks>
        /// The event listings API returns all FRC official district and regional events in a particular season. You can specify an &#x60;eventCode&#x60; if you would only like data about one specific event. If you specify an &#x60;eventCode&#x60; you cannot specify any other optional parameters. Alternately, you can specify a &#x60;teamNumber&#x60; to retrieve only the listings of events being attended by the particular team. If you specify a &#x60;teamNumber&#x60; you cannot specify an &#x60;eventCode&#x60;. The &#x60;districtCode&#x60; parameter can be added to filter the response to only thos events from a particular district. If you specify the &#x60;districtCode&#x60; parameter, you cannot specify an &#x60;eventCode&#x60; or the &#x60;excludeDistrict&#x60; parameters. The &#x60;excludeDistrict&#x60; parameter can be used to prevent the response from including district events. If you specify the &#x60;excludeDistrict&#x60; parameter, you cannot specify an &#x60;eventCode&#x60; or &#x60;districtCode&#x60;.  Values on this endpoint are \&quot;pass through\&quot; values from the MyEvents registration system. As such, if the event does not specify a value for a field, it may be presented in the API as &#x60;null&#x60;.  The response for event listings contains a special field called &#x60;divisionCode&#x60;. Starting with the 2015 season, some events are considered to be divisions of others. For example, the FIRST Championship contains four Divisions and each Division contains two Subdivisions. As an example of a reponse, the event listings for a subdivision will contain a &#x60;divisionCode&#x60; of the Division from which they belong. In turn, the Division will then have a &#x60;divisionCode&#x60; that matches the FIRST Championship event code (as they are divisions of that event). This allows you to see the full structure of events, and how they relate to each other.  **_Important Note:_** For events with the &#x60;type&#x60; ChampionshipDivision, you cannot request match results, rankings, schedules or alliances. These event types do not have those results, only Subdivisions. As such, you would receive an &#x60;HTTP 404 (not found)&#x60; if you request results that are not applicable to the event. However, they do have awards, and those can be requested.
        /// </remarks>
      /// <exception cref="FIRST.Client.ApiException">Thrown when fails to make API call</exception>
      /// <param name="season">**[REQUIRED] (int)** Numeric year of the event from which the award listings are requested. Must be 4 digits and greater than or equal to 2015, and less than or equal to the current year.</param>
      /// <param name="districtCode">**(string)** Case insensitive districtCode of the district from which event listings are requested.  District Codes: &#x60;&#x60;&#x60; FMA PNW NE FIN FNC ONT ISR CHS FIT PCH FIM &#x60;&#x60;&#x60; (optional)</param>
      /// <param name="eventCode">**(string)** Case insensitive alphanumeric eventCode of the event about which details are requested. (optional)</param>
      /// <param name="excludeDistrict">**(bool)**  Boolean to specify whether or not to exclude district events in the event listings. true means exclude, but if no value is specified, false will be used (include district events). Excluding district events also excludes district championships. (optional)</param>
      /// <param name="ifModifiedSince"> (optional)</param>
      /// <param name="teamNumber">**(int)** Numeric teamNumber of the team from which the attending event listings are requested. (optional)</param>
      /// <param name="tournamentType">**(string)** Type of event to perform the calculation on.  Enum values: &#x60;&#x60;&#x60; 1. None 2. Regional 3. DistrictEvent 4. DistrictChampionship 5. DistrictChampionshipWithLevels 6. DistrictChampionshipDivision 7. ChampionshipSubdivision 8. ChampionshipDivision 9. Championship 10. OffSeason 11. OffSeasonWithAzureSync &#x60;&#x60;&#x60; (optional)</param>
      /// <param name="weekNumber">**(int)** Week during the FRC season that the event takes place. (optional)</param>
      /// <returns>Object</returns>
      Object? SeasonEventsGet(string season, string? districtCode = default, string? eventCode = default, string? excludeDistrict = default, string? ifModifiedSince = default, string? teamNumber = default, string? tournamentType = default, string? weekNumber = default);

    /// <summary>
    /// Event Listings
    /// </summary>
    /// <remarks>
    /// The event listings API returns all FRC official district and regional events in a particular season. You can specify an &#x60;eventCode&#x60; if you would only like data about one specific event. If you specify an &#x60;eventCode&#x60; you cannot specify any other optional parameters. Alternately, you can specify a &#x60;teamNumber&#x60; to retrieve only the listings of events being attended by the particular team. If you specify a &#x60;teamNumber&#x60; you cannot specify an &#x60;eventCode&#x60;. The &#x60;districtCode&#x60; parameter can be added to filter the response to only thos events from a particular district. If you specify the &#x60;districtCode&#x60; parameter, you cannot specify an &#x60;eventCode&#x60; or the &#x60;excludeDistrict&#x60; parameters. The &#x60;excludeDistrict&#x60; parameter can be used to prevent the response from including district events. If you specify the &#x60;excludeDistrict&#x60; parameter, you cannot specify an &#x60;eventCode&#x60; or &#x60;districtCode&#x60;.  Values on this endpoint are \&quot;pass through\&quot; values from the MyEvents registration system. As such, if the event does not specify a value for a field, it may be presented in the API as &#x60;null&#x60;.  The response for event listings contains a special field called &#x60;divisionCode&#x60;. Starting with the 2015 season, some events are considered to be divisions of others. For example, the FIRST Championship contains four Divisions and each Division contains two Subdivisions. As an example of a reponse, the event listings for a subdivision will contain a &#x60;divisionCode&#x60; of the Division from which they belong. In turn, the Division will then have a &#x60;divisionCode&#x60; that matches the FIRST Championship event code (as they are divisions of that event). This allows you to see the full structure of events, and how they relate to each other.  **_Important Note:_** For events with the &#x60;type&#x60; ChampionshipDivision, you cannot request match results, rankings, schedules or alliances. These event types do not have those results, only Subdivisions. As such, you would receive an &#x60;HTTP 404 (not found)&#x60; if you request results that are not applicable to the event. However, they do have awards, and those can be requested.
    /// </remarks>
    /// <exception cref="ApiException">Thrown when fails to make API call</exception>
    /// <param name="season">**[REQUIRED] (int)** Numeric year of the event from which the award listings are requested. Must be 4 digits and greater than or equal to 2015, and less than or equal to the current year.</param>
    /// <param name="districtCode">**(string)** Case insensitive districtCode of the district from which event listings are requested.  District Codes: &#x60;&#x60;&#x60; FMA PNW NE FIN FNC ONT ISR CHS FIT PCH FIM &#x60;&#x60;&#x60; (optional)</param>
    /// <param name="eventCode">**(string)** Case insensitive alphanumeric eventCode of the event about which details are requested. (optional)</param>
    /// <param name="excludeDistrict">**(bool)**  Boolean to specify whether or not to exclude district events in the event listings. true means exclude, but if no value is specified, false will be used (include district events). Excluding district events also excludes district championships. (optional)</param>
    /// <param name="ifModifiedSince"> (optional)</param>
    /// <param name="teamNumber">**(int)** Numeric teamNumber of the team from which the attending event listings are requested. (optional)</param>
    /// <param name="tournamentType">**(string)** Type of event to perform the calculation on.  Enum values: &#x60;&#x60;&#x60; 1. None 2. Regional 3. DistrictEvent 4. DistrictChampionship 5. DistrictChampionshipWithLevels 6. DistrictChampionshipDivision 7. ChampionshipSubdivision 8. ChampionshipDivision 9. Championship 10. OffSeason 11. OffSeasonWithAzureSync &#x60;&#x60;&#x60; (optional)</param>
    /// <param name="weekNumber">**(int)** Week during the FRC season that the event takes place. (optional)</param>
    /// <returns>ApiResponse of Object</returns>
    ApiResponse<Object?> SeasonEventsGetWithHttpInfo(string season, string? districtCode = default, string? eventCode = default, string? excludeDistrict = default, string? ifModifiedSince = default, string? teamNumber = default, string? tournamentType = default, string? weekNumber = default);
      /// <summary>
      /// Season Summary
      /// </summary>
        /// <remarks>
        /// The season summary API returns a high level glance of a particular FRC season.
        /// </remarks>
      /// <exception cref="FIRST.Client.ApiException">Thrown when fails to make API call</exception>
      /// <param name="season">**[REQUIRED] (int)** Numeric year of the event from which the award listings are requested. Must be 4 digits and greater than or equal to 2015, and less than or equal to the current year.</param>
      /// <param name="ifModifiedSince">(Required)  (optional)</param>
      /// <returns>Object</returns>
      Object? SeasonGet(string season, string? ifModifiedSince = default);

    /// <summary>
    /// Season Summary
    /// </summary>
    /// <remarks>
    /// The season summary API returns a high level glance of a particular FRC season.
    /// </remarks>
    /// <exception cref="ApiException">Thrown when fails to make API call</exception>
    /// <param name="season">**[REQUIRED] (int)** Numeric year of the event from which the award listings are requested. Must be 4 digits and greater than or equal to 2015, and less than or equal to the current year.</param>
    /// <param name="ifModifiedSince">(Required)  (optional)</param>
    /// <returns>ApiResponse of Object</returns>
    ApiResponse<Object?> SeasonGetWithHttpInfo(string season, string? ifModifiedSince = default);
      /// <summary>
      /// Team Listings
      /// </summary>
        /// <remarks>
        /// The team listings API returns all FRC official teams in a particular &#x60;season&#x60;. If specified, the &#x60;teamNumber&#x60; parameter will return only one result with the details of the requested &#x60;teamNumber&#x60;. Alternately, the &#x60;eventCode&#x60; parameter allows sorting of the team list to only those teams attending a particular event in the particular &#x60;season&#x60;. Further, the district parameter allows results to be returned only when the team is a member of the requested district based on the district code. If you specify a &#x60;teamNumber&#x60; parameter, you cannot additionally specify an &#x60;eventCode&#x60; and/or &#x60;districtCode&#x60; and/or state in the same request, or you will receive an &#x60;HTTP 501&#x60;. You can however request a &#x60;districtCode&#x60; and an &#x60;eventCode&#x60; together. If you specify the &#x60;state&#x60; parameter, it should be the full legal name of the US state or international state/prov, such as &#x60;New Hampshire&#x60; or &#x60;Ontario&#x60;. Values on this endpoint are \&quot;pass through\&quot; values from the TIMS registration system. As such, if the team does not specify a value for a field, it may be presented in the API as &#x60;null&#x60;.
        /// </remarks>
      /// <exception cref="FIRST.Client.ApiException">Thrown when fails to make API call</exception>
      /// <param name="season">**[REQUIRED] (int)** Numeric year of the event from which the award listings are requested. Must be 4 digits and greater than or equal to 2015, and less than or equal to the current year.</param>
      /// <param name="districtCode">**(string)** Case insensitive districtCode code of the district from which team listings are requested.  District Codes: &#x60;&#x60;&#x60; FMA PNW NE FIN FNC ONT ISR CHS FIT PCH FIM &#x60;&#x60;&#x60; (optional)</param>
      /// <param name="eventCode">**(string)** Case insensitive alphanumeric eventCode of the event from which details are requested. (optional)</param>
      /// <param name="ifModifiedSince"> (optional)</param>
      /// <param name="page">**(int)** Numeric page of results to return. If not included, page 1 will be returned. (optional)</param>
      /// <param name="state">**(string)** Name of the state or province the desired teams are located. (optional)</param>
      /// <param name="teamNumber">**(int)** Numeric teamNumber of the team about which information is requested. Must be 1 to 4 digits. (optional)</param>
      /// <returns>Object</returns>
      Object? SeasonTeamsGet(string season, string? districtCode = default, string? eventCode = default, string? ifModifiedSince = default, string? page = default, string? state = default, string? teamNumber = default);

    /// <summary>
    /// Team Listings
    /// </summary>
    /// <remarks>
    /// The team listings API returns all FRC official teams in a particular &#x60;season&#x60;. If specified, the &#x60;teamNumber&#x60; parameter will return only one result with the details of the requested &#x60;teamNumber&#x60;. Alternately, the &#x60;eventCode&#x60; parameter allows sorting of the team list to only those teams attending a particular event in the particular &#x60;season&#x60;. Further, the district parameter allows results to be returned only when the team is a member of the requested district based on the district code. If you specify a &#x60;teamNumber&#x60; parameter, you cannot additionally specify an &#x60;eventCode&#x60; and/or &#x60;districtCode&#x60; and/or state in the same request, or you will receive an &#x60;HTTP 501&#x60;. You can however request a &#x60;districtCode&#x60; and an &#x60;eventCode&#x60; together. If you specify the &#x60;state&#x60; parameter, it should be the full legal name of the US state or international state/prov, such as &#x60;New Hampshire&#x60; or &#x60;Ontario&#x60;. Values on this endpoint are \&quot;pass through\&quot; values from the TIMS registration system. As such, if the team does not specify a value for a field, it may be presented in the API as &#x60;null&#x60;.
    /// </remarks>
    /// <exception cref="ApiException">Thrown when fails to make API call</exception>
    /// <param name="season">**[REQUIRED] (int)** Numeric year of the event from which the award listings are requested. Must be 4 digits and greater than or equal to 2015, and less than or equal to the current year.</param>
    /// <param name="districtCode">**(string)** Case insensitive districtCode code of the district from which team listings are requested.  District Codes: &#x60;&#x60;&#x60; FMA PNW NE FIN FNC ONT ISR CHS FIT PCH FIM &#x60;&#x60;&#x60; (optional)</param>
    /// <param name="eventCode">**(string)** Case insensitive alphanumeric eventCode of the event from which details are requested. (optional)</param>
    /// <param name="ifModifiedSince"> (optional)</param>
    /// <param name="page">**(int)** Numeric page of results to return. If not included, page 1 will be returned. (optional)</param>
    /// <param name="state">**(string)** Name of the state or province the desired teams are located. (optional)</param>
    /// <param name="teamNumber">**(int)** Numeric teamNumber of the team about which information is requested. Must be 1 to 4 digits. (optional)</param>
    /// <returns>ApiResponse of Object</returns>
    ApiResponse<Object?> SeasonTeamsGetWithHttpInfo(string season, string? districtCode = default, string? eventCode = default, string? ifModifiedSince = default, string? page = default, string? state = default, string? teamNumber = default);
      #endregion Synchronous Operations
    }
    
      /// <summary>
      /// Represents a collection of functions to interact with the API endpoints
      /// </summary>
      public interface ISeasonDataApiAsync : IApiAccessor
      {
    #region Asynchronous Operations
    /// <summary>
    /// Team Avatar Listings
    /// </summary>
    /// <remarks>
    /// This endpoint applies only to the 2018 or later seasons. Requests for other seasons will result in a &#x60;Bad Season&#x60; error. The team avatar listings API returns all FRC official teams in a particular &#x60;season&#x60; with, if applicable, their Avatar. If specified, the &#x60;teamNumber&#x60; parameter will return only one result with the details of the requested &#x60;teamNumber&#x60;. Alternately, the &#x60;eventCode&#x60; parameter allows sorting of the team list to only those teams attending a particular event in the particular &#x60;season&#x60;. If you specify a &#x60;teamNumber&#x60; parameter, you cannot additionally specify an &#x60;eventCode&#x60; or you will receive an &#x60;HTTP 501&#x60;. If a team does not have an Avatar, the return value will be &#x60;null&#x60;. Please note that the returned Avatar is encoded in the reply, and would need to be properly handled in order to render a PNG image.
    /// </remarks>
    /// <exception cref="ApiException">Thrown when fails to make API call</exception>
    /// <param name="season">**[REQUIRED] (int)** Numeric year of the event from which the award listings are requested. Must be 4 digits and greater than or equal to 2015, and less than or equal to the current year.</param>
    /// <param name="eventCode">**(string)** Case insensitive alphanumeric eventCode of the event from which details are requested. (optional)</param>
    /// <param name="ifModifiedSince"> (optional)</param>
    /// <param name="page">**(int)** Numeric page of results to return. If not included, page 1 will be returned. (optional)</param>
    /// <param name="teamNumber">**(int)** Numeric teamNumber of the team about which information is requested. Must be 1 to 4 digits. (optional)</param>
    /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
    /// <returns>Task of Object</returns>
    Task<Object?> SeasonAvatarsGetAsync(string season, string? eventCode = default, string? ifModifiedSince = default, string? page = default, string? teamNumber = default, CancellationToken cancellationToken = default);

    /// <summary>
    /// Team Avatar Listings
    /// </summary>
    /// <remarks>
    /// This endpoint applies only to the 2018 or later seasons. Requests for other seasons will result in a &#x60;Bad Season&#x60; error. The team avatar listings API returns all FRC official teams in a particular &#x60;season&#x60; with, if applicable, their Avatar. If specified, the &#x60;teamNumber&#x60; parameter will return only one result with the details of the requested &#x60;teamNumber&#x60;. Alternately, the &#x60;eventCode&#x60; parameter allows sorting of the team list to only those teams attending a particular event in the particular &#x60;season&#x60;. If you specify a &#x60;teamNumber&#x60; parameter, you cannot additionally specify an &#x60;eventCode&#x60; or you will receive an &#x60;HTTP 501&#x60;. If a team does not have an Avatar, the return value will be &#x60;null&#x60;. Please note that the returned Avatar is encoded in the reply, and would need to be properly handled in order to render a PNG image.
    /// </remarks>
    /// <exception cref="ApiException">Thrown when fails to make API call</exception>
    /// <param name="season">**[REQUIRED] (int)** Numeric year of the event from which the award listings are requested. Must be 4 digits and greater than or equal to 2015, and less than or equal to the current year.</param>
    /// <param name="eventCode">**(string)** Case insensitive alphanumeric eventCode of the event from which details are requested. (optional)</param>
    /// <param name="ifModifiedSince"> (optional)</param>
    /// <param name="page">**(int)** Numeric page of results to return. If not included, page 1 will be returned. (optional)</param>
    /// <param name="teamNumber">**(int)** Numeric teamNumber of the team about which information is requested. Must be 1 to 4 digits. (optional)</param>
    /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
    /// <returns>Task of ApiResponse (Object)</returns>
    Task<ApiResponse<Object?>> SeasonAvatarsGetWithHttpInfoAsync(string season, string? eventCode = default, string? ifModifiedSince = default, string? page = default, string? teamNumber = default, CancellationToken cancellationToken = default);
    /// <summary>
    /// District Listings
    /// </summary>
    /// <remarks>
    /// The district listings API returns all FRC official districts of a particular season.
    /// </remarks>
    /// <exception cref="ApiException">Thrown when fails to make API call</exception>
    /// <param name="season">**[REQUIRED] (int)** Numeric year of the event from which the award listings are requested. Must be 4 digits and greater than or equal to 2015, and less than or equal to the current year.</param>
    /// <param name="ifModifiedSince"> (optional)</param>
    /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
    /// <returns>Task of Object</returns>
    Task<Object?> SeasonDistrictsGetAsync(string season, string? ifModifiedSince = default, CancellationToken cancellationToken = default);

    /// <summary>
    /// District Listings
    /// </summary>
    /// <remarks>
    /// The district listings API returns all FRC official districts of a particular season.
    /// </remarks>
    /// <exception cref="ApiException">Thrown when fails to make API call</exception>
    /// <param name="season">**[REQUIRED] (int)** Numeric year of the event from which the award listings are requested. Must be 4 digits and greater than or equal to 2015, and less than or equal to the current year.</param>
    /// <param name="ifModifiedSince"> (optional)</param>
    /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
    /// <returns>Task of ApiResponse (Object)</returns>
    Task<ApiResponse<Object?>> SeasonDistrictsGetWithHttpInfoAsync(string season, string? ifModifiedSince = default, CancellationToken cancellationToken = default);
    /// <summary>
    /// Event Listings
    /// </summary>
    /// <remarks>
    /// The event listings API returns all FRC official district and regional events in a particular season. You can specify an &#x60;eventCode&#x60; if you would only like data about one specific event. If you specify an &#x60;eventCode&#x60; you cannot specify any other optional parameters. Alternately, you can specify a &#x60;teamNumber&#x60; to retrieve only the listings of events being attended by the particular team. If you specify a &#x60;teamNumber&#x60; you cannot specify an &#x60;eventCode&#x60;. The &#x60;districtCode&#x60; parameter can be added to filter the response to only thos events from a particular district. If you specify the &#x60;districtCode&#x60; parameter, you cannot specify an &#x60;eventCode&#x60; or the &#x60;excludeDistrict&#x60; parameters. The &#x60;excludeDistrict&#x60; parameter can be used to prevent the response from including district events. If you specify the &#x60;excludeDistrict&#x60; parameter, you cannot specify an &#x60;eventCode&#x60; or &#x60;districtCode&#x60;.  Values on this endpoint are \&quot;pass through\&quot; values from the MyEvents registration system. As such, if the event does not specify a value for a field, it may be presented in the API as &#x60;null&#x60;.  The response for event listings contains a special field called &#x60;divisionCode&#x60;. Starting with the 2015 season, some events are considered to be divisions of others. For example, the FIRST Championship contains four Divisions and each Division contains two Subdivisions. As an example of a reponse, the event listings for a subdivision will contain a &#x60;divisionCode&#x60; of the Division from which they belong. In turn, the Division will then have a &#x60;divisionCode&#x60; that matches the FIRST Championship event code (as they are divisions of that event). This allows you to see the full structure of events, and how they relate to each other.  **_Important Note:_** For events with the &#x60;type&#x60; ChampionshipDivision, you cannot request match results, rankings, schedules or alliances. These event types do not have those results, only Subdivisions. As such, you would receive an &#x60;HTTP 404 (not found)&#x60; if you request results that are not applicable to the event. However, they do have awards, and those can be requested.
    /// </remarks>
    /// <exception cref="ApiException">Thrown when fails to make API call</exception>
    /// <param name="season">**[REQUIRED] (int)** Numeric year of the event from which the award listings are requested. Must be 4 digits and greater than or equal to 2015, and less than or equal to the current year.</param>
    /// <param name="districtCode">**(string)** Case insensitive districtCode of the district from which event listings are requested.  District Codes: &#x60;&#x60;&#x60; FMA PNW NE FIN FNC ONT ISR CHS FIT PCH FIM &#x60;&#x60;&#x60; (optional)</param>
    /// <param name="eventCode">**(string)** Case insensitive alphanumeric eventCode of the event about which details are requested. (optional)</param>
    /// <param name="excludeDistrict">**(bool)**  Boolean to specify whether or not to exclude district events in the event listings. true means exclude, but if no value is specified, false will be used (include district events). Excluding district events also excludes district championships. (optional)</param>
    /// <param name="ifModifiedSince"> (optional)</param>
    /// <param name="teamNumber">**(int)** Numeric teamNumber of the team from which the attending event listings are requested. (optional)</param>
    /// <param name="tournamentType">**(string)** Type of event to perform the calculation on.  Enum values: &#x60;&#x60;&#x60; 1. None 2. Regional 3. DistrictEvent 4. DistrictChampionship 5. DistrictChampionshipWithLevels 6. DistrictChampionshipDivision 7. ChampionshipSubdivision 8. ChampionshipDivision 9. Championship 10. OffSeason 11. OffSeasonWithAzureSync &#x60;&#x60;&#x60; (optional)</param>
    /// <param name="weekNumber">**(int)** Week during the FRC season that the event takes place. (optional)</param>
    /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
    /// <returns>Task of Object</returns>
    Task<Object?> SeasonEventsGetAsync(string season, string? districtCode = default, string? eventCode = default, string? excludeDistrict = default, string? ifModifiedSince = default, string? teamNumber = default, string? tournamentType = default, string? weekNumber = default, CancellationToken cancellationToken = default);

    /// <summary>
    /// Event Listings
    /// </summary>
    /// <remarks>
    /// The event listings API returns all FRC official district and regional events in a particular season. You can specify an &#x60;eventCode&#x60; if you would only like data about one specific event. If you specify an &#x60;eventCode&#x60; you cannot specify any other optional parameters. Alternately, you can specify a &#x60;teamNumber&#x60; to retrieve only the listings of events being attended by the particular team. If you specify a &#x60;teamNumber&#x60; you cannot specify an &#x60;eventCode&#x60;. The &#x60;districtCode&#x60; parameter can be added to filter the response to only thos events from a particular district. If you specify the &#x60;districtCode&#x60; parameter, you cannot specify an &#x60;eventCode&#x60; or the &#x60;excludeDistrict&#x60; parameters. The &#x60;excludeDistrict&#x60; parameter can be used to prevent the response from including district events. If you specify the &#x60;excludeDistrict&#x60; parameter, you cannot specify an &#x60;eventCode&#x60; or &#x60;districtCode&#x60;.  Values on this endpoint are \&quot;pass through\&quot; values from the MyEvents registration system. As such, if the event does not specify a value for a field, it may be presented in the API as &#x60;null&#x60;.  The response for event listings contains a special field called &#x60;divisionCode&#x60;. Starting with the 2015 season, some events are considered to be divisions of others. For example, the FIRST Championship contains four Divisions and each Division contains two Subdivisions. As an example of a reponse, the event listings for a subdivision will contain a &#x60;divisionCode&#x60; of the Division from which they belong. In turn, the Division will then have a &#x60;divisionCode&#x60; that matches the FIRST Championship event code (as they are divisions of that event). This allows you to see the full structure of events, and how they relate to each other.  **_Important Note:_** For events with the &#x60;type&#x60; ChampionshipDivision, you cannot request match results, rankings, schedules or alliances. These event types do not have those results, only Subdivisions. As such, you would receive an &#x60;HTTP 404 (not found)&#x60; if you request results that are not applicable to the event. However, they do have awards, and those can be requested.
    /// </remarks>
    /// <exception cref="ApiException">Thrown when fails to make API call</exception>
    /// <param name="season">**[REQUIRED] (int)** Numeric year of the event from which the award listings are requested. Must be 4 digits and greater than or equal to 2015, and less than or equal to the current year.</param>
    /// <param name="districtCode">**(string)** Case insensitive districtCode of the district from which event listings are requested.  District Codes: &#x60;&#x60;&#x60; FMA PNW NE FIN FNC ONT ISR CHS FIT PCH FIM &#x60;&#x60;&#x60; (optional)</param>
    /// <param name="eventCode">**(string)** Case insensitive alphanumeric eventCode of the event about which details are requested. (optional)</param>
    /// <param name="excludeDistrict">**(bool)**  Boolean to specify whether or not to exclude district events in the event listings. true means exclude, but if no value is specified, false will be used (include district events). Excluding district events also excludes district championships. (optional)</param>
    /// <param name="ifModifiedSince"> (optional)</param>
    /// <param name="teamNumber">**(int)** Numeric teamNumber of the team from which the attending event listings are requested. (optional)</param>
    /// <param name="tournamentType">**(string)** Type of event to perform the calculation on.  Enum values: &#x60;&#x60;&#x60; 1. None 2. Regional 3. DistrictEvent 4. DistrictChampionship 5. DistrictChampionshipWithLevels 6. DistrictChampionshipDivision 7. ChampionshipSubdivision 8. ChampionshipDivision 9. Championship 10. OffSeason 11. OffSeasonWithAzureSync &#x60;&#x60;&#x60; (optional)</param>
    /// <param name="weekNumber">**(int)** Week during the FRC season that the event takes place. (optional)</param>
    /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
    /// <returns>Task of ApiResponse (Object)</returns>
    Task<ApiResponse<Object?>> SeasonEventsGetWithHttpInfoAsync(string season, string? districtCode = default, string? eventCode = default, string? excludeDistrict = default, string? ifModifiedSince = default, string? teamNumber = default, string? tournamentType = default, string? weekNumber = default, CancellationToken cancellationToken = default);
    /// <summary>
    /// Season Summary
    /// </summary>
    /// <remarks>
    /// The season summary API returns a high level glance of a particular FRC season.
    /// </remarks>
    /// <exception cref="ApiException">Thrown when fails to make API call</exception>
    /// <param name="season">**[REQUIRED] (int)** Numeric year of the event from which the award listings are requested. Must be 4 digits and greater than or equal to 2015, and less than or equal to the current year.</param>
    /// <param name="ifModifiedSince">(Required)  (optional)</param>
    /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
    /// <returns>Task of Object</returns>
    Task<Object?> SeasonGetAsync(string season, string? ifModifiedSince = default, CancellationToken cancellationToken = default);

    /// <summary>
    /// Season Summary
    /// </summary>
    /// <remarks>
    /// The season summary API returns a high level glance of a particular FRC season.
    /// </remarks>
    /// <exception cref="ApiException">Thrown when fails to make API call</exception>
    /// <param name="season">**[REQUIRED] (int)** Numeric year of the event from which the award listings are requested. Must be 4 digits and greater than or equal to 2015, and less than or equal to the current year.</param>
    /// <param name="ifModifiedSince">(Required)  (optional)</param>
    /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
    /// <returns>Task of ApiResponse (Object)</returns>
    Task<ApiResponse<Object?>> SeasonGetWithHttpInfoAsync(string season, string? ifModifiedSince = default, CancellationToken cancellationToken = default);
    /// <summary>
    /// Team Listings
    /// </summary>
    /// <remarks>
    /// The team listings API returns all FRC official teams in a particular &#x60;season&#x60;. If specified, the &#x60;teamNumber&#x60; parameter will return only one result with the details of the requested &#x60;teamNumber&#x60;. Alternately, the &#x60;eventCode&#x60; parameter allows sorting of the team list to only those teams attending a particular event in the particular &#x60;season&#x60;. Further, the district parameter allows results to be returned only when the team is a member of the requested district based on the district code. If you specify a &#x60;teamNumber&#x60; parameter, you cannot additionally specify an &#x60;eventCode&#x60; and/or &#x60;districtCode&#x60; and/or state in the same request, or you will receive an &#x60;HTTP 501&#x60;. You can however request a &#x60;districtCode&#x60; and an &#x60;eventCode&#x60; together. If you specify the &#x60;state&#x60; parameter, it should be the full legal name of the US state or international state/prov, such as &#x60;New Hampshire&#x60; or &#x60;Ontario&#x60;. Values on this endpoint are \&quot;pass through\&quot; values from the TIMS registration system. As such, if the team does not specify a value for a field, it may be presented in the API as &#x60;null&#x60;.
    /// </remarks>
    /// <exception cref="ApiException">Thrown when fails to make API call</exception>
    /// <param name="season">**[REQUIRED] (int)** Numeric year of the event from which the award listings are requested. Must be 4 digits and greater than or equal to 2015, and less than or equal to the current year.</param>
    /// <param name="districtCode">**(string)** Case insensitive districtCode code of the district from which team listings are requested.  District Codes: &#x60;&#x60;&#x60; FMA PNW NE FIN FNC ONT ISR CHS FIT PCH FIM &#x60;&#x60;&#x60; (optional)</param>
    /// <param name="eventCode">**(string)** Case insensitive alphanumeric eventCode of the event from which details are requested. (optional)</param>
    /// <param name="ifModifiedSince"> (optional)</param>
    /// <param name="page">**(int)** Numeric page of results to return. If not included, page 1 will be returned. (optional)</param>
    /// <param name="state">**(string)** Name of the state or province the desired teams are located. (optional)</param>
    /// <param name="teamNumber">**(int)** Numeric teamNumber of the team about which information is requested. Must be 1 to 4 digits. (optional)</param>
    /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
    /// <returns>Task of Object</returns>
    Task<Object?> SeasonTeamsGetAsync(string season, string? districtCode = default, string? eventCode = default, string? ifModifiedSince = default, string? page = default, string? state = default, string? teamNumber = default, CancellationToken cancellationToken = default);

    /// <summary>
    /// Team Listings
    /// </summary>
    /// <remarks>
    /// The team listings API returns all FRC official teams in a particular &#x60;season&#x60;. If specified, the &#x60;teamNumber&#x60; parameter will return only one result with the details of the requested &#x60;teamNumber&#x60;. Alternately, the &#x60;eventCode&#x60; parameter allows sorting of the team list to only those teams attending a particular event in the particular &#x60;season&#x60;. Further, the district parameter allows results to be returned only when the team is a member of the requested district based on the district code. If you specify a &#x60;teamNumber&#x60; parameter, you cannot additionally specify an &#x60;eventCode&#x60; and/or &#x60;districtCode&#x60; and/or state in the same request, or you will receive an &#x60;HTTP 501&#x60;. You can however request a &#x60;districtCode&#x60; and an &#x60;eventCode&#x60; together. If you specify the &#x60;state&#x60; parameter, it should be the full legal name of the US state or international state/prov, such as &#x60;New Hampshire&#x60; or &#x60;Ontario&#x60;. Values on this endpoint are \&quot;pass through\&quot; values from the TIMS registration system. As such, if the team does not specify a value for a field, it may be presented in the API as &#x60;null&#x60;.
    /// </remarks>
    /// <exception cref="ApiException">Thrown when fails to make API call</exception>
    /// <param name="season">**[REQUIRED] (int)** Numeric year of the event from which the award listings are requested. Must be 4 digits and greater than or equal to 2015, and less than or equal to the current year.</param>
    /// <param name="districtCode">**(string)** Case insensitive districtCode code of the district from which team listings are requested.  District Codes: &#x60;&#x60;&#x60; FMA PNW NE FIN FNC ONT ISR CHS FIT PCH FIM &#x60;&#x60;&#x60; (optional)</param>
    /// <param name="eventCode">**(string)** Case insensitive alphanumeric eventCode of the event from which details are requested. (optional)</param>
    /// <param name="ifModifiedSince"> (optional)</param>
    /// <param name="page">**(int)** Numeric page of results to return. If not included, page 1 will be returned. (optional)</param>
    /// <param name="state">**(string)** Name of the state or province the desired teams are located. (optional)</param>
    /// <param name="teamNumber">**(int)** Numeric teamNumber of the team about which information is requested. Must be 1 to 4 digits. (optional)</param>
    /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
    /// <returns>Task of ApiResponse (Object)</returns>
    Task<ApiResponse<Object?>> SeasonTeamsGetWithHttpInfoAsync(string season, string? districtCode = default, string? eventCode = default, string? ifModifiedSince = default, string? page = default, string? state = default, string? teamNumber = default, CancellationToken cancellationToken = default);
          #endregion Asynchronous Operations
        }
      
      /// <summary>
      /// Represents a collection of functions to interact with the API endpoints
      /// </summary>
      public interface ISeasonDataApi : ISeasonDataApiSync, ISeasonDataApiAsync { }
      
      /// <summary>
      /// Represents a collection of functions to interact with the API endpoints
      /// </summary>
      public sealed partial class SeasonDataApi : ISeasonDataApi
      {
        private ExceptionFactory? _exceptionFactory = (name, response) => null;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="SeasonDataApi"/> class.
        /// **IMPORTANT** This will also create an instance of HttpClient, which is less than ideal.
        /// It's better to reuse the <see href="https://docs.microsoft.com/en-us/dotnet/architecture/microservices/implement-resilient-applications/use-httpclientfactory-to-implement-resilient-http-requests#issues-with-the-original-httpclient-class-available-in-net">HttpClient and HttpClientHandler</see>.
        /// </summary>
        /// <returns></returns>
        public SeasonDataApi() : this(basePath: default) { }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="SeasonDataApi"/> class.
        /// **IMPORTANT** This will also create an instance of HttpClient, which is less than ideal.
        /// It's better to reuse the <see href="https://docs.microsoft.com/en-us/dotnet/architecture/microservices/implement-resilient-applications/use-httpclientfactory-to-implement-resilient-http-requests#issues-with-the-original-httpclient-class-available-in-net">HttpClient and HttpClientHandler</see>.
        /// </summary>
        /// <param name="basePath">The target service's base path in URL format.</param>
        /// <exception cref="ArgumentException"></exception>
        /// <returns></returns>
        public SeasonDataApi(string? basePath)
        {
          this.Configuration = FIRST.Client.Configuration.MergeConfigurations(GlobalConfiguration.Instance, new Configuration { BasePath = basePath });
          this.ApiClient = new ApiClient(this.Configuration.BasePath);
          this.Client = this.ApiClient;
            this.AsynchronousClient = this.ApiClient;
          
          this.ExceptionFactory = FIRST.Client.Configuration.DefaultExceptionFactory;
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="SeasonDataApi"/> class using Configuration object.
        /// **IMPORTANT** This will also create an instance of HttpClient, which is less than ideal.
        /// It's better to reuse the <see href="https://docs.microsoft.com/en-us/dotnet/architecture/microservices/implement-resilient-applications/use-httpclientfactory-to-implement-resilient-http-requests#issues-with-the-original-httpclient-class-available-in-net">HttpClient and HttpClientHandler</see>.
        /// </summary>
        /// <param name="configuration">An instance of Configuration.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public SeasonDataApi(Configuration configuration)
        {
          ArgumentNullException.ThrowIfNull(configuration);
          
          this.Configuration = FIRST.Client.Configuration.MergeConfigurations(GlobalConfiguration.Instance, configuration);
          this.ApiClient = new ApiClient(this.Configuration.BasePath);
          this.Client = this.ApiClient;
            this.AsynchronousClient = this.ApiClient;
          
          this.ExceptionFactory = FIRST.Client.Configuration.DefaultExceptionFactory;
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="SeasonDataApi"/> class.
        /// </summary>
        /// <param name="client">An instance of HttpClient.</param>
        /// <param name="handler">An optional instance of HttpClientHandler that is used by HttpClient.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        /// <remarks>
        /// Some configuration settings will not be applied without passing an HttpClientHandler.
        /// The features affected are: Setting and Retrieving Cookies, Client Certificates, Proxy settings.
        /// </remarks>
        public SeasonDataApi(HttpClient client, HttpClientHandler? handler = null) : this(client, basePath: default, handler: handler) { }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="SeasonDataApi"/> class.
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
        public SeasonDataApi(HttpClient client, string? basePath, HttpClientHandler? handler = null)
        {
          ArgumentNullException.ThrowIfNull(client);
          
          this.Configuration = FIRST.Client.Configuration.MergeConfigurations(GlobalConfiguration.Instance, new Configuration { BasePath = basePath });
          this.ApiClient = new ApiClient(client, this.Configuration.BasePath, handler);
          this.Client = this.ApiClient;
            this.AsynchronousClient = this.ApiClient;
          
          this.ExceptionFactory = FIRST.Client.Configuration.DefaultExceptionFactory;
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="SeasonDataApi"/> class using Configuration object.
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
        public SeasonDataApi(HttpClient client, Configuration configuration, HttpClientHandler? handler = null)
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
        /// Initializes a new instance of the <see cref="SeasonDataApi"/> class
        /// using a Configuration object and client instance.
        /// </summary>
        /// <param name="client">The client interface for synchronous API access.</param>
        /// <param name="asyncClient">The client interface for asynchronous API access.</param>
        /// <param name="configuration">The configuration object.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public SeasonDataApi(ISynchronousClient client, IAsynchronousClient asyncClient, IReadableConfiguration configuration)
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
    /// Team Avatar Listings This endpoint applies only to the 2018 or later seasons. Requests for other seasons will result in a &#x60;Bad Season&#x60; error. The team avatar listings API returns all FRC official teams in a particular &#x60;season&#x60; with, if applicable, their Avatar. If specified, the &#x60;teamNumber&#x60; parameter will return only one result with the details of the requested &#x60;teamNumber&#x60;. Alternately, the &#x60;eventCode&#x60; parameter allows sorting of the team list to only those teams attending a particular event in the particular &#x60;season&#x60;. If you specify a &#x60;teamNumber&#x60; parameter, you cannot additionally specify an &#x60;eventCode&#x60; or you will receive an &#x60;HTTP 501&#x60;. If a team does not have an Avatar, the return value will be &#x60;null&#x60;. Please note that the returned Avatar is encoded in the reply, and would need to be properly handled in order to render a PNG image.
    /// </summary>
    /// <exception cref="ApiException">Thrown when fails to make API call</exception>
    /// <param name="season">**[REQUIRED] (int)** Numeric year of the event from which the award listings are requested. Must be 4 digits and greater than or equal to 2015, and less than or equal to the current year.</param>
    /// <param name="eventCode">**(string)** Case insensitive alphanumeric eventCode of the event from which details are requested. (optional)</param>
    /// <param name="ifModifiedSince"> (optional)</param>
    /// <param name="page">**(int)** Numeric page of results to return. If not included, page 1 will be returned. (optional)</param>
    /// <param name="teamNumber">**(int)** Numeric teamNumber of the team about which information is requested. Must be 1 to 4 digits. (optional)</param>
    /// <returns>Object</returns>
    public Object? SeasonAvatarsGet(string season, string? eventCode = default, string? ifModifiedSince = default, string? page = default, string? teamNumber = default)
          {
        ApiResponse<object?> localVarResponse = SeasonAvatarsGetWithHttpInfo(season, eventCode, ifModifiedSince, page, teamNumber);
              return localVarResponse.Data;
            }

    /// <summary>
    /// Team Avatar Listings This endpoint applies only to the 2018 or later seasons. Requests for other seasons will result in a &#x60;Bad Season&#x60; error. The team avatar listings API returns all FRC official teams in a particular &#x60;season&#x60; with, if applicable, their Avatar. If specified, the &#x60;teamNumber&#x60; parameter will return only one result with the details of the requested &#x60;teamNumber&#x60;. Alternately, the &#x60;eventCode&#x60; parameter allows sorting of the team list to only those teams attending a particular event in the particular &#x60;season&#x60;. If you specify a &#x60;teamNumber&#x60; parameter, you cannot additionally specify an &#x60;eventCode&#x60; or you will receive an &#x60;HTTP 501&#x60;. If a team does not have an Avatar, the return value will be &#x60;null&#x60;. Please note that the returned Avatar is encoded in the reply, and would need to be properly handled in order to render a PNG image.
    /// </summary>
    /// <exception cref="ApiException">Thrown when fails to make API call</exception>
    /// <param name="season">**[REQUIRED] (int)** Numeric year of the event from which the award listings are requested. Must be 4 digits and greater than or equal to 2015, and less than or equal to the current year.</param>
    /// <param name="eventCode">**(string)** Case insensitive alphanumeric eventCode of the event from which details are requested. (optional)</param>
    /// <param name="ifModifiedSince"> (optional)</param>
    /// <param name="page">**(int)** Numeric page of results to return. If not included, page 1 will be returned. (optional)</param>
    /// <param name="teamNumber">**(int)** Numeric teamNumber of the team about which information is requested. Must be 1 to 4 digits. (optional)</param>
    /// <returns>ApiResponse of Object</returns>
    public ApiResponse<Object?> SeasonAvatarsGetWithHttpInfo(string season, string? eventCode = default, string? ifModifiedSince = default, string? page = default, string? teamNumber = default)
            {
                    // verify the required parameter 'season' is set
                    if (season is null)
                    {
                      throw new ApiException(400, "Missing required parameter 'season' when calling SeasonDataApi->SeasonAvatarsGet");
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
                  if (eventCode is not null)
                  {
                      localVarRequestOptions.QueryParameters.Add(ClientUtils.ParameterToMultiMap("", "eventCode", eventCode));
                  }
                  
                  if (page is not null)
                  {
                      localVarRequestOptions.QueryParameters.Add(ClientUtils.ParameterToMultiMap("", "page", page));
                  }
                  
                  if (teamNumber is not null)
                  {
                      localVarRequestOptions.QueryParameters.Add(ClientUtils.ParameterToMultiMap("", "teamNumber", teamNumber));
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
              var localVarResponse = this.Client.Get<Object?>("/{season}/avatars", localVarRequestOptions, this.Configuration);
              
              if (this.ExceptionFactory is not null)
              {
                var _exception = this.ExceptionFactory("SeasonAvatarsGet", localVarResponse);
                if (_exception is not null)
                {
                  throw _exception;
                }
              }
              
              return localVarResponse;
            }

    /// <summary>
    /// Team Avatar Listings This endpoint applies only to the 2018 or later seasons. Requests for other seasons will result in a &#x60;Bad Season&#x60; error. The team avatar listings API returns all FRC official teams in a particular &#x60;season&#x60; with, if applicable, their Avatar. If specified, the &#x60;teamNumber&#x60; parameter will return only one result with the details of the requested &#x60;teamNumber&#x60;. Alternately, the &#x60;eventCode&#x60; parameter allows sorting of the team list to only those teams attending a particular event in the particular &#x60;season&#x60;. If you specify a &#x60;teamNumber&#x60; parameter, you cannot additionally specify an &#x60;eventCode&#x60; or you will receive an &#x60;HTTP 501&#x60;. If a team does not have an Avatar, the return value will be &#x60;null&#x60;. Please note that the returned Avatar is encoded in the reply, and would need to be properly handled in order to render a PNG image.
    /// </summary>
    /// <exception cref="ApiException">Thrown when fails to make API call</exception>
    /// <param name="season">**[REQUIRED] (int)** Numeric year of the event from which the award listings are requested. Must be 4 digits and greater than or equal to 2015, and less than or equal to the current year.</param>
    /// <param name="eventCode">**(string)** Case insensitive alphanumeric eventCode of the event from which details are requested. (optional)</param>
    /// <param name="ifModifiedSince"> (optional)</param>
    /// <param name="page">**(int)** Numeric page of results to return. If not included, page 1 will be returned. (optional)</param>
    /// <param name="teamNumber">**(int)** Numeric teamNumber of the team about which information is requested. Must be 1 to 4 digits. (optional)</param>
    /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
    /// <returns>Task of Object</returns>
    public async Task<Object?> SeasonAvatarsGetAsync(string season, string? eventCode = default, string? ifModifiedSince = default, string? page = default, string? teamNumber = default, CancellationToken cancellationToken = default)
            {
        ApiResponse<object?> localVarResponse = await SeasonAvatarsGetWithHttpInfoAsync(season, eventCode, ifModifiedSince, page, teamNumber, cancellationToken).ConfigureAwait(false);
                return localVarResponse.Data;
              }

    /// <summary>
    /// Team Avatar Listings This endpoint applies only to the 2018 or later seasons. Requests for other seasons will result in a &#x60;Bad Season&#x60; error. The team avatar listings API returns all FRC official teams in a particular &#x60;season&#x60; with, if applicable, their Avatar. If specified, the &#x60;teamNumber&#x60; parameter will return only one result with the details of the requested &#x60;teamNumber&#x60;. Alternately, the &#x60;eventCode&#x60; parameter allows sorting of the team list to only those teams attending a particular event in the particular &#x60;season&#x60;. If you specify a &#x60;teamNumber&#x60; parameter, you cannot additionally specify an &#x60;eventCode&#x60; or you will receive an &#x60;HTTP 501&#x60;. If a team does not have an Avatar, the return value will be &#x60;null&#x60;. Please note that the returned Avatar is encoded in the reply, and would need to be properly handled in order to render a PNG image.
    /// </summary>
    /// <exception cref="ApiException">Thrown when fails to make API call</exception>
    /// <param name="season">**[REQUIRED] (int)** Numeric year of the event from which the award listings are requested. Must be 4 digits and greater than or equal to 2015, and less than or equal to the current year.</param>
    /// <param name="eventCode">**(string)** Case insensitive alphanumeric eventCode of the event from which details are requested. (optional)</param>
    /// <param name="ifModifiedSince"> (optional)</param>
    /// <param name="page">**(int)** Numeric page of results to return. If not included, page 1 will be returned. (optional)</param>
    /// <param name="teamNumber">**(int)** Numeric teamNumber of the team about which information is requested. Must be 1 to 4 digits. (optional)</param>
    /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
    /// <returns>Task of ApiResponse (Object)</returns>
    public async Task<ApiResponse<object?>> SeasonAvatarsGetWithHttpInfoAsync(string season, string? eventCode = default, string? ifModifiedSince = default, string? page = default, string? teamNumber = default, CancellationToken cancellationToken = default)
              {
                      // verify the required parameter 'season' is set
                      if (season is null)
                      {
                        throw new ApiException(400, "Missing required parameter 'season' when calling SeasonDataApi->SeasonAvatarsGet");
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
                    if (eventCode is not null)
                    {
                      localVarRequestOptions.QueryParameters.Add(ClientUtils.ParameterToMultiMap("", "eventCode", eventCode));
                    }
                    
                    if (page is not null)
                    {
                      localVarRequestOptions.QueryParameters.Add(ClientUtils.ParameterToMultiMap("", "page", page));
                    }
                    
                    if (teamNumber is not null)
                    {
                      localVarRequestOptions.QueryParameters.Add(ClientUtils.ParameterToMultiMap("", "teamNumber", teamNumber));
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
                var localVarResponse = await this.AsynchronousClient.GetAsync<Object?>("/{season}/avatars", localVarRequestOptions, this.Configuration, cancellationToken).ConfigureAwait(false);
                
                if (this.ExceptionFactory is not null)
                {
                  var _exception = this.ExceptionFactory("SeasonAvatarsGet", localVarResponse);
                  if (_exception is not null)
                  {
                    throw _exception;
                  }
                }
                
                return localVarResponse;
              }          /// <summary>
                         /// District Listings The district listings API returns all FRC official districts of a particular season.
                         /// </summary>
                         /// <exception cref="ApiException">Thrown when fails to make API call</exception>
                         /// <param name="season">**[REQUIRED] (int)** Numeric year of the event from which the award listings are requested. Must be 4 digits and greater than or equal to 2015, and less than or equal to the current year.</param>
                         /// <param name="ifModifiedSince"> (optional)</param>
                         /// <returns>Object</returns>
    public Object? SeasonDistrictsGet(string season, string? ifModifiedSince = default)
          {
        ApiResponse<object?> localVarResponse = SeasonDistrictsGetWithHttpInfo(season, ifModifiedSince);
              return localVarResponse.Data;
            }

    /// <summary>
    /// District Listings The district listings API returns all FRC official districts of a particular season.
    /// </summary>
    /// <exception cref="ApiException">Thrown when fails to make API call</exception>
    /// <param name="season">**[REQUIRED] (int)** Numeric year of the event from which the award listings are requested. Must be 4 digits and greater than or equal to 2015, and less than or equal to the current year.</param>
    /// <param name="ifModifiedSince"> (optional)</param>
    /// <returns>ApiResponse of Object</returns>
    public ApiResponse<Object?> SeasonDistrictsGetWithHttpInfo(string season, string? ifModifiedSince = default)
            {
                    // verify the required parameter 'season' is set
                    if (season is null)
                    {
                      throw new ApiException(400, "Missing required parameter 'season' when calling SeasonDataApi->SeasonDistrictsGet");
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
              var localVarResponse = this.Client.Get<Object?>("/{season}/districts", localVarRequestOptions, this.Configuration);
              
              if (this.ExceptionFactory is not null)
              {
                var _exception = this.ExceptionFactory("SeasonDistrictsGet", localVarResponse);
                if (_exception is not null)
                {
                  throw _exception;
                }
              }
              
              return localVarResponse;
            }

    /// <summary>
    /// District Listings The district listings API returns all FRC official districts of a particular season.
    /// </summary>
    /// <exception cref="ApiException">Thrown when fails to make API call</exception>
    /// <param name="season">**[REQUIRED] (int)** Numeric year of the event from which the award listings are requested. Must be 4 digits and greater than or equal to 2015, and less than or equal to the current year.</param>
    /// <param name="ifModifiedSince"> (optional)</param>
    /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
    /// <returns>Task of Object</returns>
    public async Task<Object?> SeasonDistrictsGetAsync(string season, string? ifModifiedSince = default, CancellationToken cancellationToken = default)
            {
        ApiResponse<object?> localVarResponse = await SeasonDistrictsGetWithHttpInfoAsync(season, ifModifiedSince, cancellationToken).ConfigureAwait(false);
                return localVarResponse.Data;
              }

    /// <summary>
    /// District Listings The district listings API returns all FRC official districts of a particular season.
    /// </summary>
    /// <exception cref="ApiException">Thrown when fails to make API call</exception>
    /// <param name="season">**[REQUIRED] (int)** Numeric year of the event from which the award listings are requested. Must be 4 digits and greater than or equal to 2015, and less than or equal to the current year.</param>
    /// <param name="ifModifiedSince"> (optional)</param>
    /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
    /// <returns>Task of ApiResponse (Object)</returns>
    public async Task<ApiResponse<object?>> SeasonDistrictsGetWithHttpInfoAsync(string season, string? ifModifiedSince = default, CancellationToken cancellationToken = default)
              {
                      // verify the required parameter 'season' is set
                      if (season is null)
                      {
                        throw new ApiException(400, "Missing required parameter 'season' when calling SeasonDataApi->SeasonDistrictsGet");
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
                var localVarResponse = await this.AsynchronousClient.GetAsync<Object?>("/{season}/districts", localVarRequestOptions, this.Configuration, cancellationToken).ConfigureAwait(false);
                
                if (this.ExceptionFactory is not null)
                {
                  var _exception = this.ExceptionFactory("SeasonDistrictsGet", localVarResponse);
                  if (_exception is not null)
                  {
                    throw _exception;
                  }
                }
                
                return localVarResponse;
              }          /// <summary>
                         /// Event Listings The event listings API returns all FRC official district and regional events in a particular season. You can specify an &#x60;eventCode&#x60; if you would only like data about one specific event. If you specify an &#x60;eventCode&#x60; you cannot specify any other optional parameters. Alternately, you can specify a &#x60;teamNumber&#x60; to retrieve only the listings of events being attended by the particular team. If you specify a &#x60;teamNumber&#x60; you cannot specify an &#x60;eventCode&#x60;. The &#x60;districtCode&#x60; parameter can be added to filter the response to only thos events from a particular district. If you specify the &#x60;districtCode&#x60; parameter, you cannot specify an &#x60;eventCode&#x60; or the &#x60;excludeDistrict&#x60; parameters. The &#x60;excludeDistrict&#x60; parameter can be used to prevent the response from including district events. If you specify the &#x60;excludeDistrict&#x60; parameter, you cannot specify an &#x60;eventCode&#x60; or &#x60;districtCode&#x60;.  Values on this endpoint are \&quot;pass through\&quot; values from the MyEvents registration system. As such, if the event does not specify a value for a field, it may be presented in the API as &#x60;null&#x60;.  The response for event listings contains a special field called &#x60;divisionCode&#x60;. Starting with the 2015 season, some events are considered to be divisions of others. For example, the FIRST Championship contains four Divisions and each Division contains two Subdivisions. As an example of a reponse, the event listings for a subdivision will contain a &#x60;divisionCode&#x60; of the Division from which they belong. In turn, the Division will then have a &#x60;divisionCode&#x60; that matches the FIRST Championship event code (as they are divisions of that event). This allows you to see the full structure of events, and how they relate to each other.  **_Important Note:_** For events with the &#x60;type&#x60; ChampionshipDivision, you cannot request match results, rankings, schedules or alliances. These event types do not have those results, only Subdivisions. As such, you would receive an &#x60;HTTP 404 (not found)&#x60; if you request results that are not applicable to the event. However, they do have awards, and those can be requested.
                         /// </summary>
                         /// <exception cref="ApiException">Thrown when fails to make API call</exception>
                         /// <param name="season">**[REQUIRED] (int)** Numeric year of the event from which the award listings are requested. Must be 4 digits and greater than or equal to 2015, and less than or equal to the current year.</param>
                         /// <param name="districtCode">**(string)** Case insensitive districtCode of the district from which event listings are requested.  District Codes: &#x60;&#x60;&#x60; FMA PNW NE FIN FNC ONT ISR CHS FIT PCH FIM &#x60;&#x60;&#x60; (optional)</param>
                         /// <param name="eventCode">**(string)** Case insensitive alphanumeric eventCode of the event about which details are requested. (optional)</param>
                         /// <param name="excludeDistrict">**(bool)**  Boolean to specify whether or not to exclude district events in the event listings. true means exclude, but if no value is specified, false will be used (include district events). Excluding district events also excludes district championships. (optional)</param>
                         /// <param name="ifModifiedSince"> (optional)</param>
                         /// <param name="teamNumber">**(int)** Numeric teamNumber of the team from which the attending event listings are requested. (optional)</param>
                         /// <param name="tournamentType">**(string)** Type of event to perform the calculation on.  Enum values: &#x60;&#x60;&#x60; 1. None 2. Regional 3. DistrictEvent 4. DistrictChampionship 5. DistrictChampionshipWithLevels 6. DistrictChampionshipDivision 7. ChampionshipSubdivision 8. ChampionshipDivision 9. Championship 10. OffSeason 11. OffSeasonWithAzureSync &#x60;&#x60;&#x60; (optional)</param>
                         /// <param name="weekNumber">**(int)** Week during the FRC season that the event takes place. (optional)</param>
                         /// <returns>Object</returns>
    public Object? SeasonEventsGet(string season, string? districtCode = default, string? eventCode = default, string? excludeDistrict = default, string? ifModifiedSince = default, string? teamNumber = default, string? tournamentType = default, string? weekNumber = default)
          {
        ApiResponse<object?> localVarResponse = SeasonEventsGetWithHttpInfo(season, districtCode, eventCode, excludeDistrict, ifModifiedSince, teamNumber, tournamentType, weekNumber);
              return localVarResponse.Data;
            }

    /// <summary>
    /// Event Listings The event listings API returns all FRC official district and regional events in a particular season. You can specify an &#x60;eventCode&#x60; if you would only like data about one specific event. If you specify an &#x60;eventCode&#x60; you cannot specify any other optional parameters. Alternately, you can specify a &#x60;teamNumber&#x60; to retrieve only the listings of events being attended by the particular team. If you specify a &#x60;teamNumber&#x60; you cannot specify an &#x60;eventCode&#x60;. The &#x60;districtCode&#x60; parameter can be added to filter the response to only thos events from a particular district. If you specify the &#x60;districtCode&#x60; parameter, you cannot specify an &#x60;eventCode&#x60; or the &#x60;excludeDistrict&#x60; parameters. The &#x60;excludeDistrict&#x60; parameter can be used to prevent the response from including district events. If you specify the &#x60;excludeDistrict&#x60; parameter, you cannot specify an &#x60;eventCode&#x60; or &#x60;districtCode&#x60;.  Values on this endpoint are \&quot;pass through\&quot; values from the MyEvents registration system. As such, if the event does not specify a value for a field, it may be presented in the API as &#x60;null&#x60;.  The response for event listings contains a special field called &#x60;divisionCode&#x60;. Starting with the 2015 season, some events are considered to be divisions of others. For example, the FIRST Championship contains four Divisions and each Division contains two Subdivisions. As an example of a reponse, the event listings for a subdivision will contain a &#x60;divisionCode&#x60; of the Division from which they belong. In turn, the Division will then have a &#x60;divisionCode&#x60; that matches the FIRST Championship event code (as they are divisions of that event). This allows you to see the full structure of events, and how they relate to each other.  **_Important Note:_** For events with the &#x60;type&#x60; ChampionshipDivision, you cannot request match results, rankings, schedules or alliances. These event types do not have those results, only Subdivisions. As such, you would receive an &#x60;HTTP 404 (not found)&#x60; if you request results that are not applicable to the event. However, they do have awards, and those can be requested.
    /// </summary>
    /// <exception cref="ApiException">Thrown when fails to make API call</exception>
    /// <param name="season">**[REQUIRED] (int)** Numeric year of the event from which the award listings are requested. Must be 4 digits and greater than or equal to 2015, and less than or equal to the current year.</param>
    /// <param name="districtCode">**(string)** Case insensitive districtCode of the district from which event listings are requested.  District Codes: &#x60;&#x60;&#x60; FMA PNW NE FIN FNC ONT ISR CHS FIT PCH FIM &#x60;&#x60;&#x60; (optional)</param>
    /// <param name="eventCode">**(string)** Case insensitive alphanumeric eventCode of the event about which details are requested. (optional)</param>
    /// <param name="excludeDistrict">**(bool)**  Boolean to specify whether or not to exclude district events in the event listings. true means exclude, but if no value is specified, false will be used (include district events). Excluding district events also excludes district championships. (optional)</param>
    /// <param name="ifModifiedSince"> (optional)</param>
    /// <param name="teamNumber">**(int)** Numeric teamNumber of the team from which the attending event listings are requested. (optional)</param>
    /// <param name="tournamentType">**(string)** Type of event to perform the calculation on.  Enum values: &#x60;&#x60;&#x60; 1. None 2. Regional 3. DistrictEvent 4. DistrictChampionship 5. DistrictChampionshipWithLevels 6. DistrictChampionshipDivision 7. ChampionshipSubdivision 8. ChampionshipDivision 9. Championship 10. OffSeason 11. OffSeasonWithAzureSync &#x60;&#x60;&#x60; (optional)</param>
    /// <param name="weekNumber">**(int)** Week during the FRC season that the event takes place. (optional)</param>
    /// <returns>ApiResponse of Object</returns>
    public ApiResponse<Object?> SeasonEventsGetWithHttpInfo(string season, string? districtCode = default, string? eventCode = default, string? excludeDistrict = default, string? ifModifiedSince = default, string? teamNumber = default, string? tournamentType = default, string? weekNumber = default)
            {
                    // verify the required parameter 'season' is set
                    if (season is null)
                    {
                      throw new ApiException(400, "Missing required parameter 'season' when calling SeasonDataApi->SeasonEventsGet");
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
                  
                  if (eventCode is not null)
                  {
                      localVarRequestOptions.QueryParameters.Add(ClientUtils.ParameterToMultiMap("", "eventCode", eventCode));
                  }
                  
                  if (excludeDistrict is not null)
                  {
                      localVarRequestOptions.QueryParameters.Add(ClientUtils.ParameterToMultiMap("", "excludeDistrict", excludeDistrict));
                  }
                  
                  if (teamNumber is not null)
                  {
                      localVarRequestOptions.QueryParameters.Add(ClientUtils.ParameterToMultiMap("", "teamNumber", teamNumber));
                  }
                  
                  if (tournamentType is not null)
                  {
                      localVarRequestOptions.QueryParameters.Add(ClientUtils.ParameterToMultiMap("", "tournamentType", tournamentType));
                  }
                  
                  if (weekNumber is not null)
                  {
                      localVarRequestOptions.QueryParameters.Add(ClientUtils.ParameterToMultiMap("", "weekNumber", weekNumber));
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
              var localVarResponse = this.Client.Get<Object?>("/{season}/events", localVarRequestOptions, this.Configuration);
              
              if (this.ExceptionFactory is not null)
              {
                var _exception = this.ExceptionFactory("SeasonEventsGet", localVarResponse);
                if (_exception is not null)
                {
                  throw _exception;
                }
              }
              
              return localVarResponse;
            }

    /// <summary>
    /// Event Listings The event listings API returns all FRC official district and regional events in a particular season. You can specify an &#x60;eventCode&#x60; if you would only like data about one specific event. If you specify an &#x60;eventCode&#x60; you cannot specify any other optional parameters. Alternately, you can specify a &#x60;teamNumber&#x60; to retrieve only the listings of events being attended by the particular team. If you specify a &#x60;teamNumber&#x60; you cannot specify an &#x60;eventCode&#x60;. The &#x60;districtCode&#x60; parameter can be added to filter the response to only thos events from a particular district. If you specify the &#x60;districtCode&#x60; parameter, you cannot specify an &#x60;eventCode&#x60; or the &#x60;excludeDistrict&#x60; parameters. The &#x60;excludeDistrict&#x60; parameter can be used to prevent the response from including district events. If you specify the &#x60;excludeDistrict&#x60; parameter, you cannot specify an &#x60;eventCode&#x60; or &#x60;districtCode&#x60;.  Values on this endpoint are \&quot;pass through\&quot; values from the MyEvents registration system. As such, if the event does not specify a value for a field, it may be presented in the API as &#x60;null&#x60;.  The response for event listings contains a special field called &#x60;divisionCode&#x60;. Starting with the 2015 season, some events are considered to be divisions of others. For example, the FIRST Championship contains four Divisions and each Division contains two Subdivisions. As an example of a reponse, the event listings for a subdivision will contain a &#x60;divisionCode&#x60; of the Division from which they belong. In turn, the Division will then have a &#x60;divisionCode&#x60; that matches the FIRST Championship event code (as they are divisions of that event). This allows you to see the full structure of events, and how they relate to each other.  **_Important Note:_** For events with the &#x60;type&#x60; ChampionshipDivision, you cannot request match results, rankings, schedules or alliances. These event types do not have those results, only Subdivisions. As such, you would receive an &#x60;HTTP 404 (not found)&#x60; if you request results that are not applicable to the event. However, they do have awards, and those can be requested.
    /// </summary>
    /// <exception cref="ApiException">Thrown when fails to make API call</exception>
    /// <param name="season">**[REQUIRED] (int)** Numeric year of the event from which the award listings are requested. Must be 4 digits and greater than or equal to 2015, and less than or equal to the current year.</param>
    /// <param name="districtCode">**(string)** Case insensitive districtCode of the district from which event listings are requested.  District Codes: &#x60;&#x60;&#x60; FMA PNW NE FIN FNC ONT ISR CHS FIT PCH FIM &#x60;&#x60;&#x60; (optional)</param>
    /// <param name="eventCode">**(string)** Case insensitive alphanumeric eventCode of the event about which details are requested. (optional)</param>
    /// <param name="excludeDistrict">**(bool)**  Boolean to specify whether or not to exclude district events in the event listings. true means exclude, but if no value is specified, false will be used (include district events). Excluding district events also excludes district championships. (optional)</param>
    /// <param name="ifModifiedSince"> (optional)</param>
    /// <param name="teamNumber">**(int)** Numeric teamNumber of the team from which the attending event listings are requested. (optional)</param>
    /// <param name="tournamentType">**(string)** Type of event to perform the calculation on.  Enum values: &#x60;&#x60;&#x60; 1. None 2. Regional 3. DistrictEvent 4. DistrictChampionship 5. DistrictChampionshipWithLevels 6. DistrictChampionshipDivision 7. ChampionshipSubdivision 8. ChampionshipDivision 9. Championship 10. OffSeason 11. OffSeasonWithAzureSync &#x60;&#x60;&#x60; (optional)</param>
    /// <param name="weekNumber">**(int)** Week during the FRC season that the event takes place. (optional)</param>
    /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
    /// <returns>Task of Object</returns>
    public async Task<Object?> SeasonEventsGetAsync(string season, string? districtCode = default, string? eventCode = default, string? excludeDistrict = default, string? ifModifiedSince = default, string? teamNumber = default, string? tournamentType = default, string? weekNumber = default, CancellationToken cancellationToken = default)
            {
        ApiResponse<object?> localVarResponse = await SeasonEventsGetWithHttpInfoAsync(season, districtCode, eventCode, excludeDistrict, ifModifiedSince, teamNumber, tournamentType, weekNumber, cancellationToken).ConfigureAwait(false);
                return localVarResponse.Data;
              }

    /// <summary>
    /// Event Listings The event listings API returns all FRC official district and regional events in a particular season. You can specify an &#x60;eventCode&#x60; if you would only like data about one specific event. If you specify an &#x60;eventCode&#x60; you cannot specify any other optional parameters. Alternately, you can specify a &#x60;teamNumber&#x60; to retrieve only the listings of events being attended by the particular team. If you specify a &#x60;teamNumber&#x60; you cannot specify an &#x60;eventCode&#x60;. The &#x60;districtCode&#x60; parameter can be added to filter the response to only thos events from a particular district. If you specify the &#x60;districtCode&#x60; parameter, you cannot specify an &#x60;eventCode&#x60; or the &#x60;excludeDistrict&#x60; parameters. The &#x60;excludeDistrict&#x60; parameter can be used to prevent the response from including district events. If you specify the &#x60;excludeDistrict&#x60; parameter, you cannot specify an &#x60;eventCode&#x60; or &#x60;districtCode&#x60;.  Values on this endpoint are \&quot;pass through\&quot; values from the MyEvents registration system. As such, if the event does not specify a value for a field, it may be presented in the API as &#x60;null&#x60;.  The response for event listings contains a special field called &#x60;divisionCode&#x60;. Starting with the 2015 season, some events are considered to be divisions of others. For example, the FIRST Championship contains four Divisions and each Division contains two Subdivisions. As an example of a reponse, the event listings for a subdivision will contain a &#x60;divisionCode&#x60; of the Division from which they belong. In turn, the Division will then have a &#x60;divisionCode&#x60; that matches the FIRST Championship event code (as they are divisions of that event). This allows you to see the full structure of events, and how they relate to each other.  **_Important Note:_** For events with the &#x60;type&#x60; ChampionshipDivision, you cannot request match results, rankings, schedules or alliances. These event types do not have those results, only Subdivisions. As such, you would receive an &#x60;HTTP 404 (not found)&#x60; if you request results that are not applicable to the event. However, they do have awards, and those can be requested.
    /// </summary>
    /// <exception cref="ApiException">Thrown when fails to make API call</exception>
    /// <param name="season">**[REQUIRED] (int)** Numeric year of the event from which the award listings are requested. Must be 4 digits and greater than or equal to 2015, and less than or equal to the current year.</param>
    /// <param name="districtCode">**(string)** Case insensitive districtCode of the district from which event listings are requested.  District Codes: &#x60;&#x60;&#x60; FMA PNW NE FIN FNC ONT ISR CHS FIT PCH FIM &#x60;&#x60;&#x60; (optional)</param>
    /// <param name="eventCode">**(string)** Case insensitive alphanumeric eventCode of the event about which details are requested. (optional)</param>
    /// <param name="excludeDistrict">**(bool)**  Boolean to specify whether or not to exclude district events in the event listings. true means exclude, but if no value is specified, false will be used (include district events). Excluding district events also excludes district championships. (optional)</param>
    /// <param name="ifModifiedSince"> (optional)</param>
    /// <param name="teamNumber">**(int)** Numeric teamNumber of the team from which the attending event listings are requested. (optional)</param>
    /// <param name="tournamentType">**(string)** Type of event to perform the calculation on.  Enum values: &#x60;&#x60;&#x60; 1. None 2. Regional 3. DistrictEvent 4. DistrictChampionship 5. DistrictChampionshipWithLevels 6. DistrictChampionshipDivision 7. ChampionshipSubdivision 8. ChampionshipDivision 9. Championship 10. OffSeason 11. OffSeasonWithAzureSync &#x60;&#x60;&#x60; (optional)</param>
    /// <param name="weekNumber">**(int)** Week during the FRC season that the event takes place. (optional)</param>
    /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
    /// <returns>Task of ApiResponse (Object)</returns>
    public async Task<ApiResponse<object?>> SeasonEventsGetWithHttpInfoAsync(string season, string? districtCode = default, string? eventCode = default, string? excludeDistrict = default, string? ifModifiedSince = default, string? teamNumber = default, string? tournamentType = default, string? weekNumber = default, CancellationToken cancellationToken = default)
              {
                      // verify the required parameter 'season' is set
                      if (season is null)
                      {
                        throw new ApiException(400, "Missing required parameter 'season' when calling SeasonDataApi->SeasonEventsGet");
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
                    
                    if (eventCode is not null)
                    {
                      localVarRequestOptions.QueryParameters.Add(ClientUtils.ParameterToMultiMap("", "eventCode", eventCode));
                    }
                    
                    if (excludeDistrict is not null)
                    {
                      localVarRequestOptions.QueryParameters.Add(ClientUtils.ParameterToMultiMap("", "excludeDistrict", excludeDistrict));
                    }
                    
                    if (teamNumber is not null)
                    {
                      localVarRequestOptions.QueryParameters.Add(ClientUtils.ParameterToMultiMap("", "teamNumber", teamNumber));
                    }
                    
                    if (tournamentType is not null)
                    {
                      localVarRequestOptions.QueryParameters.Add(ClientUtils.ParameterToMultiMap("", "tournamentType", tournamentType));
                    }
                    
                    if (weekNumber is not null)
                    {
                      localVarRequestOptions.QueryParameters.Add(ClientUtils.ParameterToMultiMap("", "weekNumber", weekNumber));
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
                var localVarResponse = await this.AsynchronousClient.GetAsync<Object?>("/{season}/events", localVarRequestOptions, this.Configuration, cancellationToken).ConfigureAwait(false);
                
                if (this.ExceptionFactory is not null)
                {
                  var _exception = this.ExceptionFactory("SeasonEventsGet", localVarResponse);
                  if (_exception is not null)
                  {
                    throw _exception;
                  }
                }
                
                return localVarResponse;
              }          /// <summary>
                         /// Season Summary The season summary API returns a high level glance of a particular FRC season.
                         /// </summary>
                         /// <exception cref="ApiException">Thrown when fails to make API call</exception>
                         /// <param name="season">**[REQUIRED] (int)** Numeric year of the event from which the award listings are requested. Must be 4 digits and greater than or equal to 2015, and less than or equal to the current year.</param>
                         /// <param name="ifModifiedSince">(Required)  (optional)</param>
                         /// <returns>Object</returns>
    public Object? SeasonGet(string season, string? ifModifiedSince = default)
          {
        ApiResponse<object?> localVarResponse = SeasonGetWithHttpInfo(season, ifModifiedSince);
              return localVarResponse.Data;
            }

    /// <summary>
    /// Season Summary The season summary API returns a high level glance of a particular FRC season.
    /// </summary>
    /// <exception cref="ApiException">Thrown when fails to make API call</exception>
    /// <param name="season">**[REQUIRED] (int)** Numeric year of the event from which the award listings are requested. Must be 4 digits and greater than or equal to 2015, and less than or equal to the current year.</param>
    /// <param name="ifModifiedSince">(Required)  (optional)</param>
    /// <returns>ApiResponse of Object</returns>
    public ApiResponse<Object?> SeasonGetWithHttpInfo(string season, string? ifModifiedSince = default)
            {
                    // verify the required parameter 'season' is set
                    if (season is null)
                    {
                      throw new ApiException(400, "Missing required parameter 'season' when calling SeasonDataApi->SeasonGet");
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
              var localVarResponse = this.Client.Get<Object?>("/{season}", localVarRequestOptions, this.Configuration);
              
              if (this.ExceptionFactory is not null)
              {
                var _exception = this.ExceptionFactory("SeasonGet", localVarResponse);
                if (_exception is not null)
                {
                  throw _exception;
                }
              }
              
              return localVarResponse;
            }

    /// <summary>
    /// Season Summary The season summary API returns a high level glance of a particular FRC season.
    /// </summary>
    /// <exception cref="ApiException">Thrown when fails to make API call</exception>
    /// <param name="season">**[REQUIRED] (int)** Numeric year of the event from which the award listings are requested. Must be 4 digits and greater than or equal to 2015, and less than or equal to the current year.</param>
    /// <param name="ifModifiedSince">(Required)  (optional)</param>
    /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
    /// <returns>Task of Object</returns>
    public async Task<Object?> SeasonGetAsync(string season, string? ifModifiedSince = default, CancellationToken cancellationToken = default)
            {
        ApiResponse<object?> localVarResponse = await SeasonGetWithHttpInfoAsync(season, ifModifiedSince, cancellationToken).ConfigureAwait(false);
                return localVarResponse.Data;
              }

    /// <summary>
    /// Season Summary The season summary API returns a high level glance of a particular FRC season.
    /// </summary>
    /// <exception cref="ApiException">Thrown when fails to make API call</exception>
    /// <param name="season">**[REQUIRED] (int)** Numeric year of the event from which the award listings are requested. Must be 4 digits and greater than or equal to 2015, and less than or equal to the current year.</param>
    /// <param name="ifModifiedSince">(Required)  (optional)</param>
    /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
    /// <returns>Task of ApiResponse (Object)</returns>
    public async Task<ApiResponse<object?>> SeasonGetWithHttpInfoAsync(string season, string? ifModifiedSince = default, CancellationToken cancellationToken = default)
              {
                      // verify the required parameter 'season' is set
                      if (season is null)
                      {
                        throw new ApiException(400, "Missing required parameter 'season' when calling SeasonDataApi->SeasonGet");
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
                var localVarResponse = await this.AsynchronousClient.GetAsync<Object?>("/{season}", localVarRequestOptions, this.Configuration, cancellationToken).ConfigureAwait(false);
                
                if (this.ExceptionFactory is not null)
                {
                  var _exception = this.ExceptionFactory("SeasonGet", localVarResponse);
                  if (_exception is not null)
                  {
                    throw _exception;
                  }
                }
                
                return localVarResponse;
              }          /// <summary>
                         /// Team Listings The team listings API returns all FRC official teams in a particular &#x60;season&#x60;. If specified, the &#x60;teamNumber&#x60; parameter will return only one result with the details of the requested &#x60;teamNumber&#x60;. Alternately, the &#x60;eventCode&#x60; parameter allows sorting of the team list to only those teams attending a particular event in the particular &#x60;season&#x60;. Further, the district parameter allows results to be returned only when the team is a member of the requested district based on the district code. If you specify a &#x60;teamNumber&#x60; parameter, you cannot additionally specify an &#x60;eventCode&#x60; and/or &#x60;districtCode&#x60; and/or state in the same request, or you will receive an &#x60;HTTP 501&#x60;. You can however request a &#x60;districtCode&#x60; and an &#x60;eventCode&#x60; together. If you specify the &#x60;state&#x60; parameter, it should be the full legal name of the US state or international state/prov, such as &#x60;New Hampshire&#x60; or &#x60;Ontario&#x60;. Values on this endpoint are \&quot;pass through\&quot; values from the TIMS registration system. As such, if the team does not specify a value for a field, it may be presented in the API as &#x60;null&#x60;.
                         /// </summary>
                         /// <exception cref="ApiException">Thrown when fails to make API call</exception>
                         /// <param name="season">**[REQUIRED] (int)** Numeric year of the event from which the award listings are requested. Must be 4 digits and greater than or equal to 2015, and less than or equal to the current year.</param>
                         /// <param name="districtCode">**(string)** Case insensitive districtCode code of the district from which team listings are requested.  District Codes: &#x60;&#x60;&#x60; FMA PNW NE FIN FNC ONT ISR CHS FIT PCH FIM &#x60;&#x60;&#x60; (optional)</param>
                         /// <param name="eventCode">**(string)** Case insensitive alphanumeric eventCode of the event from which details are requested. (optional)</param>
                         /// <param name="ifModifiedSince"> (optional)</param>
                         /// <param name="page">**(int)** Numeric page of results to return. If not included, page 1 will be returned. (optional)</param>
                         /// <param name="state">**(string)** Name of the state or province the desired teams are located. (optional)</param>
                         /// <param name="teamNumber">**(int)** Numeric teamNumber of the team about which information is requested. Must be 1 to 4 digits. (optional)</param>
                         /// <returns>Object</returns>
    public Object? SeasonTeamsGet(string season, string? districtCode = default, string? eventCode = default, string? ifModifiedSince = default, string? page = default, string? state = default, string? teamNumber = default)
          {
        ApiResponse<object?> localVarResponse = SeasonTeamsGetWithHttpInfo(season, districtCode, eventCode, ifModifiedSince, page, state, teamNumber);
              return localVarResponse.Data;
            }

    /// <summary>
    /// Team Listings The team listings API returns all FRC official teams in a particular &#x60;season&#x60;. If specified, the &#x60;teamNumber&#x60; parameter will return only one result with the details of the requested &#x60;teamNumber&#x60;. Alternately, the &#x60;eventCode&#x60; parameter allows sorting of the team list to only those teams attending a particular event in the particular &#x60;season&#x60;. Further, the district parameter allows results to be returned only when the team is a member of the requested district based on the district code. If you specify a &#x60;teamNumber&#x60; parameter, you cannot additionally specify an &#x60;eventCode&#x60; and/or &#x60;districtCode&#x60; and/or state in the same request, or you will receive an &#x60;HTTP 501&#x60;. You can however request a &#x60;districtCode&#x60; and an &#x60;eventCode&#x60; together. If you specify the &#x60;state&#x60; parameter, it should be the full legal name of the US state or international state/prov, such as &#x60;New Hampshire&#x60; or &#x60;Ontario&#x60;. Values on this endpoint are \&quot;pass through\&quot; values from the TIMS registration system. As such, if the team does not specify a value for a field, it may be presented in the API as &#x60;null&#x60;.
    /// </summary>
    /// <exception cref="ApiException">Thrown when fails to make API call</exception>
    /// <param name="season">**[REQUIRED] (int)** Numeric year of the event from which the award listings are requested. Must be 4 digits and greater than or equal to 2015, and less than or equal to the current year.</param>
    /// <param name="districtCode">**(string)** Case insensitive districtCode code of the district from which team listings are requested.  District Codes: &#x60;&#x60;&#x60; FMA PNW NE FIN FNC ONT ISR CHS FIT PCH FIM &#x60;&#x60;&#x60; (optional)</param>
    /// <param name="eventCode">**(string)** Case insensitive alphanumeric eventCode of the event from which details are requested. (optional)</param>
    /// <param name="ifModifiedSince"> (optional)</param>
    /// <param name="page">**(int)** Numeric page of results to return. If not included, page 1 will be returned. (optional)</param>
    /// <param name="state">**(string)** Name of the state or province the desired teams are located. (optional)</param>
    /// <param name="teamNumber">**(int)** Numeric teamNumber of the team about which information is requested. Must be 1 to 4 digits. (optional)</param>
    /// <returns>ApiResponse of Object</returns>
    public ApiResponse<Object?> SeasonTeamsGetWithHttpInfo(string season, string? districtCode = default, string? eventCode = default, string? ifModifiedSince = default, string? page = default, string? state = default, string? teamNumber = default)
            {
                    // verify the required parameter 'season' is set
                    if (season is null)
                    {
                      throw new ApiException(400, "Missing required parameter 'season' when calling SeasonDataApi->SeasonTeamsGet");
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
                  
                  if (eventCode is not null)
                  {
                      localVarRequestOptions.QueryParameters.Add(ClientUtils.ParameterToMultiMap("", "eventCode", eventCode));
                  }
                  
                  if (page is not null)
                  {
                      localVarRequestOptions.QueryParameters.Add(ClientUtils.ParameterToMultiMap("", "page", page));
                  }
                  
                  if (state is not null)
                  {
                      localVarRequestOptions.QueryParameters.Add(ClientUtils.ParameterToMultiMap("", "state", state));
                  }
                  
                  if (teamNumber is not null)
                  {
                      localVarRequestOptions.QueryParameters.Add(ClientUtils.ParameterToMultiMap("", "teamNumber", teamNumber));
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
              var localVarResponse = this.Client.Get<Object?>("/{season}/teams", localVarRequestOptions, this.Configuration);
              
              if (this.ExceptionFactory is not null)
              {
                var _exception = this.ExceptionFactory("SeasonTeamsGet", localVarResponse);
                if (_exception is not null)
                {
                  throw _exception;
                }
              }
              
              return localVarResponse;
            }

    /// <summary>
    /// Team Listings The team listings API returns all FRC official teams in a particular &#x60;season&#x60;. If specified, the &#x60;teamNumber&#x60; parameter will return only one result with the details of the requested &#x60;teamNumber&#x60;. Alternately, the &#x60;eventCode&#x60; parameter allows sorting of the team list to only those teams attending a particular event in the particular &#x60;season&#x60;. Further, the district parameter allows results to be returned only when the team is a member of the requested district based on the district code. If you specify a &#x60;teamNumber&#x60; parameter, you cannot additionally specify an &#x60;eventCode&#x60; and/or &#x60;districtCode&#x60; and/or state in the same request, or you will receive an &#x60;HTTP 501&#x60;. You can however request a &#x60;districtCode&#x60; and an &#x60;eventCode&#x60; together. If you specify the &#x60;state&#x60; parameter, it should be the full legal name of the US state or international state/prov, such as &#x60;New Hampshire&#x60; or &#x60;Ontario&#x60;. Values on this endpoint are \&quot;pass through\&quot; values from the TIMS registration system. As such, if the team does not specify a value for a field, it may be presented in the API as &#x60;null&#x60;.
    /// </summary>
    /// <exception cref="ApiException">Thrown when fails to make API call</exception>
    /// <param name="season">**[REQUIRED] (int)** Numeric year of the event from which the award listings are requested. Must be 4 digits and greater than or equal to 2015, and less than or equal to the current year.</param>
    /// <param name="districtCode">**(string)** Case insensitive districtCode code of the district from which team listings are requested.  District Codes: &#x60;&#x60;&#x60; FMA PNW NE FIN FNC ONT ISR CHS FIT PCH FIM &#x60;&#x60;&#x60; (optional)</param>
    /// <param name="eventCode">**(string)** Case insensitive alphanumeric eventCode of the event from which details are requested. (optional)</param>
    /// <param name="ifModifiedSince"> (optional)</param>
    /// <param name="page">**(int)** Numeric page of results to return. If not included, page 1 will be returned. (optional)</param>
    /// <param name="state">**(string)** Name of the state or province the desired teams are located. (optional)</param>
    /// <param name="teamNumber">**(int)** Numeric teamNumber of the team about which information is requested. Must be 1 to 4 digits. (optional)</param>
    /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
    /// <returns>Task of Object</returns>
    public async Task<Object?> SeasonTeamsGetAsync(string season, string? districtCode = default, string? eventCode = default, string? ifModifiedSince = default, string? page = default, string? state = default, string? teamNumber = default, CancellationToken cancellationToken = default)
            {
        ApiResponse<object?> localVarResponse = await SeasonTeamsGetWithHttpInfoAsync(season, districtCode, eventCode, ifModifiedSince, page, state, teamNumber, cancellationToken).ConfigureAwait(false);
                return localVarResponse.Data;
              }

    /// <summary>
    /// Team Listings The team listings API returns all FRC official teams in a particular &#x60;season&#x60;. If specified, the &#x60;teamNumber&#x60; parameter will return only one result with the details of the requested &#x60;teamNumber&#x60;. Alternately, the &#x60;eventCode&#x60; parameter allows sorting of the team list to only those teams attending a particular event in the particular &#x60;season&#x60;. Further, the district parameter allows results to be returned only when the team is a member of the requested district based on the district code. If you specify a &#x60;teamNumber&#x60; parameter, you cannot additionally specify an &#x60;eventCode&#x60; and/or &#x60;districtCode&#x60; and/or state in the same request, or you will receive an &#x60;HTTP 501&#x60;. You can however request a &#x60;districtCode&#x60; and an &#x60;eventCode&#x60; together. If you specify the &#x60;state&#x60; parameter, it should be the full legal name of the US state or international state/prov, such as &#x60;New Hampshire&#x60; or &#x60;Ontario&#x60;. Values on this endpoint are \&quot;pass through\&quot; values from the TIMS registration system. As such, if the team does not specify a value for a field, it may be presented in the API as &#x60;null&#x60;.
    /// </summary>
    /// <exception cref="ApiException">Thrown when fails to make API call</exception>
    /// <param name="season">**[REQUIRED] (int)** Numeric year of the event from which the award listings are requested. Must be 4 digits and greater than or equal to 2015, and less than or equal to the current year.</param>
    /// <param name="districtCode">**(string)** Case insensitive districtCode code of the district from which team listings are requested.  District Codes: &#x60;&#x60;&#x60; FMA PNW NE FIN FNC ONT ISR CHS FIT PCH FIM &#x60;&#x60;&#x60; (optional)</param>
    /// <param name="eventCode">**(string)** Case insensitive alphanumeric eventCode of the event from which details are requested. (optional)</param>
    /// <param name="ifModifiedSince"> (optional)</param>
    /// <param name="page">**(int)** Numeric page of results to return. If not included, page 1 will be returned. (optional)</param>
    /// <param name="state">**(string)** Name of the state or province the desired teams are located. (optional)</param>
    /// <param name="teamNumber">**(int)** Numeric teamNumber of the team about which information is requested. Must be 1 to 4 digits. (optional)</param>
    /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
    /// <returns>Task of ApiResponse (Object)</returns>
    public async Task<ApiResponse<object?>> SeasonTeamsGetWithHttpInfoAsync(string season, string? districtCode = default, string? eventCode = default, string? ifModifiedSince = default, string? page = default, string? state = default, string? teamNumber = default, CancellationToken cancellationToken = default)
              {
                      // verify the required parameter 'season' is set
                      if (season is null)
                      {
                        throw new ApiException(400, "Missing required parameter 'season' when calling SeasonDataApi->SeasonTeamsGet");
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
                    
                    if (eventCode is not null)
                    {
                      localVarRequestOptions.QueryParameters.Add(ClientUtils.ParameterToMultiMap("", "eventCode", eventCode));
                    }
                    
                    if (page is not null)
                    {
                      localVarRequestOptions.QueryParameters.Add(ClientUtils.ParameterToMultiMap("", "page", page));
                    }
                    
                    if (state is not null)
                    {
                      localVarRequestOptions.QueryParameters.Add(ClientUtils.ParameterToMultiMap("", "state", state));
                    }
                    
                    if (teamNumber is not null)
                    {
                      localVarRequestOptions.QueryParameters.Add(ClientUtils.ParameterToMultiMap("", "teamNumber", teamNumber));
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
                var localVarResponse = await this.AsynchronousClient.GetAsync<Object?>("/{season}/teams", localVarRequestOptions, this.Configuration, cancellationToken).ConfigureAwait(false);
                
                if (this.ExceptionFactory is not null)
                {
                  var _exception = this.ExceptionFactory("SeasonTeamsGet", localVarResponse);
                  if (_exception is not null)
                  {
                    throw _exception;
                  }
                }
                
                return localVarResponse;
              }
            }
