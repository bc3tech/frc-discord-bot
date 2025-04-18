/*
 * FRC Events
 *
 * # Overview  _FIRST_/FMS FRC Events API is a service to return relevant information about the _FIRST_ Robotics Competition (FRC). Information is made available from events operating around the world.  For FRC, information is made available by the Field Management System (FMS) server operating at each event site. The FMS will attempt to sync all data from the event to \"the cloud\" as long as internet is available at the venue. If internet is unavailable, or \"goes down\" during the event, the FMS will automatically sync all data from the time the system was offline as soon as the connection is restored. The API will provide data as soon as it has synced, and we do not add any artificial delays. If you are receiving \"stale\" data, it may be because of connectivity problems at the venue. We recommend you try again later, and post on the FIRST FMS TeamForge site if the problem persists. _(Note: FMS does not sync while a match is running, so data that has to do with a particular match should become available once the score has been revealed to the audience at the event.)_  ### Migration and Program Notes:  Pay close attention to the addresses for calling the various endpoints- as well as the notes regarding endpoints with multiple possible responses (i.e. score details and rankings).  # Documentation Notes  All times are listed in the local time to the event venue. HTTP-date values will show their timezone.  If you specify a parameter, but no value for that parameter, it will be ignored. For example, if you request **URL?teamNumber=** the **teamNumber** parameter would be ignored.  We will continue to support the current version of the API plus one version older. Old APIs are depricated once a version \"two times newer\" is available, at minimum 6 months. For example, version 2.0 and 1.0 are supported right now, but 1.0 will not be supported once 2.1 (or 3.0) is available. Versions may also be retired earlier with prior notice here in the documentation.  The full host address of the API is needed in all calls. The version number is required in each call in order to ensure your requests are made (and responses are returned) in the formats you anticipate. The version number for this documentation is found on the top of the page, in the title. If you call this version number, the API responses will match the formats listed below.  All of the APIs are capable of accepting the **Accept** HTTP header with either **application/xml** or **application/json** based on the desired return type. Any API call that results in an **HTTP Status Code** other than **200 (OK)** will only be shown here as an **application/json** response to save space, but the content is the same regardless of the request type. All response will have a **Content-Length** and **Date** response header, but those are not shown in the documentaiton.  For all APIs that accept a query string in addition to the URI base, the order of parameters do not matter, but the name shown in the documentation must match exactly, as does the associated value format as described in details.  For response codes that are not **HTTP 200 (OK)**, the documentation will show a body message that represents a possible response value. While the \"title\" of the **HTTP Status Code** will match those shown in the response codes documentation section exactly, the body of the response will be a more detailed explanation of why that status code is being returned and may not always be exactly as shown in the examples.  None of the APIs will show possible return here in the documentation of **HTTP 401 (Unauthorized)**, but that code applies to all APIs as a possible response if the request is made without a valid token.  ### Last-Modified, FMS-OnlyModifiedSince, and If-Modified-Since Headers  The FRC Events API utilizes the **Last-Modified** and **If-Modified-Since** Headers to communicate with consumers regarding the age of the data they are requesting. With a couple of exceptions, all calls will return a **Last-Modified** Header set with the time at which the data at that endpoint was last modified. The Header will always be set in the HTTP-date format, as described in the [HTTP Protocol](https://www.w3.org/Protocols/rfc2616/rfc2616-sec14.html). There are two exceptions: the **Last-Modified** Header is not set if the endpoint returns no results (such as a request for a schedule with no matches) and will also not be set if the request was an **HTTP DELETE**.  Consumers should keep track of the **Last-Modified** Header, and return it on subsequent calls to the same endpoint as the **If-Modified-Since**. The server will recognize this request, and will only return a result if the data has been modified since the last request. If no changes have been made, an **HTTP 304** will be returned. If data has been modified, ALL data on that call will be returned (for \"only modified\" data, see below).  The FRC Events API also allows a custom header used to filter the return data to a specific subset. This is done by specifying a **FMS-OnlyModifiedSince** header with each call. As with the **If-Modified-Since** header, consumers should keep track of the **Last-Modified** Header, and return it on subsequent calls to the same endpoint as the **FMS-OnlyModifiedSince** Header. The server will recognize this request, and will only return a result if the data has been modified since the last request, and, if returned, the data will only be those portions modified since the included date. If no changes, have been made, an **HTTP 304** will be returned. Using this method, the server and consumer save processing time by only receiving modified data that is in need of update on the consumer side.  If the Headers are improperly passed (such as the wrong Day of Week for the matching date, or a date in the future), the endpoint will simply ignore the Header and return all results. If both headers are specified, the request will be denied.  # Response Codes  The FRC Events API HTTP Status Codes correspond with the [common codes](https://www.w3.org/Protocols/rfc2616/rfc2616-sec10.html), but occasionally with different \"titles\". The \"title\" used by the API is shown next to each of the below possible response HTTP Status Codes. Throughout the documentation, Apiary may automatically show the common \"title\" in example returns (like \"Not Found\" for 404) but on the production server, the \"title\" will instead match those listed below.  ``` HTTP 200 - \"OK\"   ```  The request has succeeded. An entity corresponding to the requested resource is sent in the response. This will be returned as the HTTP Status Code for all request that succeed, even if the body is empty (such as an event that has no rankings, but with a valid season and event code were used)  ``` HTTP 304 - \"Not Modified\"   ```  When utilizing a Header that allows filtered data returns, such as **If-Modified-Since**, this response indicates that no data meets the request.  ``` HTTP 400 - \"Invalid Season Requested\"/\"Malformed Parameter Format In Request\"/\"Missing Parameter In Request\"/\"Invalid API Version Requested\":   ```  The request could not be understood by the server due to malformed syntax. The client SHOULD NOT repeat the request without modifications. Specifically for this API, a 400 response indicates that the requested URI matches with a valid API, but one or more required parameter was malformed or invalid. Examples include an event code that is too short or team number that contains a letter.  ``` HTTP 401 - \"Unauthorized\"   ```  All requests against the API require authentication via a valid user token. Failing to provide one, or providing an invalid one, will warrant a 401 response. The client MAY repeat the request with a suitable Authorization header field.  ``` HTTP 404 - \"Invalid Event Requested\"   ```  Even though the 404 code usually indicates any not found status, a 404 will only be issued in this API when an event cannot be found for the requested season and event code. If the request didn't match a valid API or there were malformed parameters, the response would not receive a 404 but rather a 400 or 501. If this HTTP code is received, the season was a valid season and the event code matched the acceptable style of an event code, but there were no records of an event matching the combination of that season and event code. For example, HTTP 404 would be issued when the event had a different code in the requested season (the codes can change year to year based on event location).  ``` HTTP 500 - \"Internal Server Error\"   ```  The server encountered an unexpected condition which prevented it from fulfilling the request. This is a code sent directly by the server, and has no special alternate definition specific to this API.  ``` HTTP 501 - \"Request Did Not Match Any Current API Pattern\"   ```  The server does not support the functionality required to fulfill the request. Specifically, the request pattern did not match any of the possible APIs, and thus processing was discontinued. This code is also issued when too many optional parameters were included in a single request and fulfilling it would make the result confusing or misleading. Each API will specify which parameters or combination of parameters can be used at the same time.  ``` HTTP 503 - \"Service Unavailable\"   ```  The server is currently unable to handle the request due to a temporary overloading or maintenance of the server. The implication is that this is a temporary condition which will be alleviated after some delay. If known, the length of the delay MAY be indicated in a Retry-After header. This code will not always appear, sometimes the server may outright refuse the connection instead. This is a code sent directly by the server, and has no special alternate definition specific to this API.  See the notes at the top of this documentation for important information about HTTP Status Codes.  # Authorization  In order to make calls against the FRC Events API, you must include an HTTP Header called **Authorization** with the value set as specified below. If a request is made without this header, processing stops and an **HTTP 401** is issued. All **Authorization** headers follow the same format:  Authorization: Basic 000000000000000000000000000000000000000000000000000000000000  Where the Zeros are replaced by your Token. The Token can be formed by taking your **username** and your **AuthorizationKey** and adding a colon. For example, if your **username** is sampleuser and your **AuthorizationKey** is 7eaa6338-a097-4221-ac04-b6120fcc4d49 you would have this string:  **sampleuser:7eaa6338-a097-4221-ac04-b6120fcc4d49**  This string must then be encoded using Base64 Encoded to form the Token, which will be the same length as the example above, but include letters and numbers. For our example, we would have:  c2FtcGxldXNlcjo3ZWFhNjMzOC1hMDk3LTQyMjEtYWMwNC1iNjEyMGZjYzRkNDk=  **NOTICE**: Publicly distributing an application, code snippet, etc, that has your username and token in it, encoded or not, WILL result in your token being blocked from the API. Each user should apply for their own token.  If you wish to acquire a token for your development, you may do so by requesting a token through our automated system on [this website](https://frc-events.firstinspires.org/services/API).  **AUTOMATED REMOVAL**: If you do not activate your account within 72 hours of making your request for a token, or if you do not make at least one API request every twelve (12) months, your account/token will be marked as disabled for inactivity and subject to being deleted. (This policy does not apply to accounts with special operating agreements with FIRST)  ### HTTP401 and Authorization  Each Token can be individually enabled and disabled by _FIRST_. As such, a normally valid combination of **username** and **AuthorizationToken** could still be rejected. The possible return messages you may see in these instances are:  **Incorrect Token** (You supplied an AuthorizationToken, but it wasn't correct)  **Account Disabled, Contact Support** (You have been disabled for excessive traffic or abuse. Contact support)  **Username Not Found** (A username was found, but didn't match any on file)  **Unable To Determine Authorization Token** (The format of the **Authorization** header was incorrect)  # Webhooks  > **COMING SOON**
 *
 * The version of the OpenAPI document: 1.0.0
 * Generated by: https://github.com/openapitools/openapi-generator.git
 */

namespace FIRST.Api;

using FIRST.Client;

using System;
using System.Net.Http;
using System.Threading.Tasks;

/// <summary>
/// Represents a collection of functions to interact with the API endpoints
/// </summary>
public interface IAwardsApiSync : IApiAccessor
{
    #region Synchronous Operations
    /// <summary>
    /// Event Awards
    /// </summary>
    /// <remarks>
    /// The event awards API returns details about awards presented at a particular event in a particular season. Return values may contain either &#x60;teamNumber&#x60; or &#x60;person&#x60; values, and if the winner was a &#x60;person&#x60;, and that person is from a team, the &#x60;teamNumber&#x60; value might be set with their &#x60;teamNumber&#x60;.
    /// </remarks>
    /// <exception cref="FIRST.Client.ApiException">Thrown when fails to make API call</exception>
    /// <param name="eventCode">(Required) **(string)** Optional case insensitive alphanumeric eventCode of the event from which the awards are requested. Must be at least 3 characters.</param>
    /// <param name="season">**[REQUIRED] (int)** Numeric year of the event from which the award listings are requested. Must be 4 digits and greater than or equal to 2015, and less than or equal to the current year.</param>
    /// <param name="ifModifiedSince"> (optional)</param>
    /// <returns>Object</returns>
    Object? SeasonAwardsEventEventCodeGet(string eventCode, string season, string? ifModifiedSince = default);

    /// <summary>
    /// Event Awards
    /// </summary>
    /// <remarks>
    /// The event awards API returns details about awards presented at a particular event in a particular season. Return values may contain either &#x60;teamNumber&#x60; or &#x60;person&#x60; values, and if the winner was a &#x60;person&#x60;, and that person is from a team, the &#x60;teamNumber&#x60; value might be set with their &#x60;teamNumber&#x60;.
    /// </remarks>
    /// <exception cref="ApiException">Thrown when fails to make API call</exception>
    /// <param name="eventCode">(Required) **(string)** Optional case insensitive alphanumeric eventCode of the event from which the awards are requested. Must be at least 3 characters.</param>
    /// <param name="season">**[REQUIRED] (int)** Numeric year of the event from which the award listings are requested. Must be 4 digits and greater than or equal to 2015, and less than or equal to the current year.</param>
    /// <param name="ifModifiedSince"> (optional)</param>
    /// <returns>ApiResponse of Object</returns>
    ApiResponse<Object?> SeasonAwardsEventEventCodeGetWithHttpInfo(string eventCode, string season, string? ifModifiedSince = default);
    /// <summary>
    /// Event Team Awards
    /// </summary>
    /// <remarks>
    /// The event team awards API returns details about awards presented at a particular event in a particular season for a particular team. Return values may contain either &#x60;teamNumber&#x60; or &#x60;person&#x60; values, and if the winner was a &#x60;person&#x60;, and that person is from a team, the &#x60;teamNumber&#x60; value might be set with their &#x60;teamNumber&#x60;.
    /// </remarks>
    /// <exception cref="FIRST.Client.ApiException">Thrown when fails to make API call</exception>
    /// <param name="eventCode">**[Required] (string)** Optional case insensitive alphanumeric eventCode of the event from which the awards are requested. Must be at least 3 characters.</param>
    /// <param name="season">**[REQUIRED] (int)** Numeric year of the team and event from which the award listings are requested. Must be 4 digits and greater than or equal to 2015, and less than or equal to the current year.</param>
    /// <param name="teamNumber">**[REQUIRED] (int)** Numeric teamNumber of the team about which information is requested. Must be 1 to 4 digits.</param>
    /// <returns>Object</returns>
    Object? SeasonAwardsEventteamEventCodeTeamNumberGet(string eventCode, string season, string teamNumber);

    /// <summary>
    /// Event Team Awards
    /// </summary>
    /// <remarks>
    /// The event team awards API returns details about awards presented at a particular event in a particular season for a particular team. Return values may contain either &#x60;teamNumber&#x60; or &#x60;person&#x60; values, and if the winner was a &#x60;person&#x60;, and that person is from a team, the &#x60;teamNumber&#x60; value might be set with their &#x60;teamNumber&#x60;.
    /// </remarks>
    /// <exception cref="ApiException">Thrown when fails to make API call</exception>
    /// <param name="eventCode">**[Required] (string)** Optional case insensitive alphanumeric eventCode of the event from which the awards are requested. Must be at least 3 characters.</param>
    /// <param name="season">**[REQUIRED] (int)** Numeric year of the team and event from which the award listings are requested. Must be 4 digits and greater than or equal to 2015, and less than or equal to the current year.</param>
    /// <param name="teamNumber">**[REQUIRED] (int)** Numeric teamNumber of the team about which information is requested. Must be 1 to 4 digits.</param>
    /// <returns>ApiResponse of Object</returns>
    ApiResponse<Object?> SeasonAwardsEventteamEventCodeTeamNumberGetWithHttpInfo(string eventCode, string season, string teamNumber);
    /// <summary>
    /// Awards Listings
    /// </summary>
    /// <remarks>
    /// The award listings API returns a listing of the various awards that can be distributed in the requested season. This is especially useful in order to avoid having to use the &#x60;name&#x60; field of the event awards API to know which award was won. Instead the &#x60;awardId&#x60; field can be matched between the two APIs.
    /// </remarks>
    /// <exception cref="FIRST.Client.ApiException">Thrown when fails to make API call</exception>
    /// <param name="season">**[REQUIRED] (int)** Numeric year of the event from which the award listings are requested. Must be 4 digits and greater than or equal to 2015, and less than or equal to the current year.</param>
    /// <param name="ifModifiedSince"> (optional)</param>
    /// <returns>Object</returns>
    Object? SeasonAwardsListGet(string season, string? ifModifiedSince = default);

    /// <summary>
    /// Awards Listings
    /// </summary>
    /// <remarks>
    /// The award listings API returns a listing of the various awards that can be distributed in the requested season. This is especially useful in order to avoid having to use the &#x60;name&#x60; field of the event awards API to know which award was won. Instead the &#x60;awardId&#x60; field can be matched between the two APIs.
    /// </remarks>
    /// <exception cref="ApiException">Thrown when fails to make API call</exception>
    /// <param name="season">**[REQUIRED] (int)** Numeric year of the event from which the award listings are requested. Must be 4 digits and greater than or equal to 2015, and less than or equal to the current year.</param>
    /// <param name="ifModifiedSince"> (optional)</param>
    /// <returns>ApiResponse of Object</returns>
    ApiResponse<Object?> SeasonAwardsListGetWithHttpInfo(string season, string? ifModifiedSince = default);
    /// <summary>
    /// Team Awards
    /// </summary>
    /// <remarks>
    /// The team awards API returns details about awards presented for a particular team in a particular season. Return values may contain either &#x60;teamNumber&#x60; or &#x60;person&#x60; values, and if the winner was a &#x60;person&#x60;, and that person is from a team, the &#x60;teamNumber&#x60; value might be set with their &#x60;teamNumber&#x60;.
    /// </remarks>
    /// <exception cref="FIRST.Client.ApiException">Thrown when fails to make API call</exception>
    /// <param name="season">**[REQUIRED] (int)** Numeric year of the team from which the award listings are requested. Must be 4 digits and greater than or equal to 2015, and less than or equal to the current year.</param>
    /// <param name="teamNumber">**[REQUIRED] (int)** Numeric teamNumber of the team about which information is requested. Must be 1 to 4 digits.</param>
    /// <returns>Object</returns>
    Object? SeasonAwardsTeamTeamNumberGet(string season, string teamNumber);

    /// <summary>
    /// Team Awards
    /// </summary>
    /// <remarks>
    /// The team awards API returns details about awards presented for a particular team in a particular season. Return values may contain either &#x60;teamNumber&#x60; or &#x60;person&#x60; values, and if the winner was a &#x60;person&#x60;, and that person is from a team, the &#x60;teamNumber&#x60; value might be set with their &#x60;teamNumber&#x60;.
    /// </remarks>
    /// <exception cref="ApiException">Thrown when fails to make API call</exception>
    /// <param name="season">**[REQUIRED] (int)** Numeric year of the team from which the award listings are requested. Must be 4 digits and greater than or equal to 2015, and less than or equal to the current year.</param>
    /// <param name="teamNumber">**[REQUIRED] (int)** Numeric teamNumber of the team about which information is requested. Must be 1 to 4 digits.</param>
    /// <returns>ApiResponse of Object</returns>
    ApiResponse<Object?> SeasonAwardsTeamTeamNumberGetWithHttpInfo(string season, string teamNumber);
    #endregion Synchronous Operations
}

/// <summary>
/// Represents a collection of functions to interact with the API endpoints
/// </summary>
public interface IAwardsApiAsync : IApiAccessor
{
    #region Asynchronous Operations
    /// <summary>
    /// Event Awards
    /// </summary>
    /// <remarks>
    /// The event awards API returns details about awards presented at a particular event in a particular season. Return values may contain either &#x60;teamNumber&#x60; or &#x60;person&#x60; values, and if the winner was a &#x60;person&#x60;, and that person is from a team, the &#x60;teamNumber&#x60; value might be set with their &#x60;teamNumber&#x60;.
    /// </remarks>
    /// <exception cref="ApiException">Thrown when fails to make API call</exception>
    /// <param name="eventCode">(Required) **(string)** Optional case insensitive alphanumeric eventCode of the event from which the awards are requested. Must be at least 3 characters.</param>
    /// <param name="season">**[REQUIRED] (int)** Numeric year of the event from which the award listings are requested. Must be 4 digits and greater than or equal to 2015, and less than or equal to the current year.</param>
    /// <param name="ifModifiedSince"> (optional)</param>
    /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
    /// <returns>Task of Object</returns>
    Task<Object?> SeasonAwardsEventEventCodeGetAsync(string eventCode, string season, string? ifModifiedSince = default, CancellationToken cancellationToken = default);

    /// <summary>
    /// Event Awards
    /// </summary>
    /// <remarks>
    /// The event awards API returns details about awards presented at a particular event in a particular season. Return values may contain either &#x60;teamNumber&#x60; or &#x60;person&#x60; values, and if the winner was a &#x60;person&#x60;, and that person is from a team, the &#x60;teamNumber&#x60; value might be set with their &#x60;teamNumber&#x60;.
    /// </remarks>
    /// <exception cref="ApiException">Thrown when fails to make API call</exception>
    /// <param name="eventCode">(Required) **(string)** Optional case insensitive alphanumeric eventCode of the event from which the awards are requested. Must be at least 3 characters.</param>
    /// <param name="season">**[REQUIRED] (int)** Numeric year of the event from which the award listings are requested. Must be 4 digits and greater than or equal to 2015, and less than or equal to the current year.</param>
    /// <param name="ifModifiedSince"> (optional)</param>
    /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
    /// <returns>Task of ApiResponse (Object)</returns>
    Task<ApiResponse<Object?>> SeasonAwardsEventEventCodeGetWithHttpInfoAsync(string eventCode, string season, string? ifModifiedSince = default, CancellationToken cancellationToken = default);
    /// <summary>
    /// Event Team Awards
    /// </summary>
    /// <remarks>
    /// The event team awards API returns details about awards presented at a particular event in a particular season for a particular team. Return values may contain either &#x60;teamNumber&#x60; or &#x60;person&#x60; values, and if the winner was a &#x60;person&#x60;, and that person is from a team, the &#x60;teamNumber&#x60; value might be set with their &#x60;teamNumber&#x60;.
    /// </remarks>
    /// <exception cref="ApiException">Thrown when fails to make API call</exception>
    /// <param name="eventCode">**[Required] (string)** Optional case insensitive alphanumeric eventCode of the event from which the awards are requested. Must be at least 3 characters.</param>
    /// <param name="season">**[REQUIRED] (int)** Numeric year of the team and event from which the award listings are requested. Must be 4 digits and greater than or equal to 2015, and less than or equal to the current year.</param>
    /// <param name="teamNumber">**[REQUIRED] (int)** Numeric teamNumber of the team about which information is requested. Must be 1 to 4 digits.</param>
    /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
    /// <returns>Task of Object</returns>
    Task<Object?> SeasonAwardsEventteamEventCodeTeamNumberGetAsync(string eventCode, string season, string teamNumber, CancellationToken cancellationToken = default);

    /// <summary>
    /// Event Team Awards
    /// </summary>
    /// <remarks>
    /// The event team awards API returns details about awards presented at a particular event in a particular season for a particular team. Return values may contain either &#x60;teamNumber&#x60; or &#x60;person&#x60; values, and if the winner was a &#x60;person&#x60;, and that person is from a team, the &#x60;teamNumber&#x60; value might be set with their &#x60;teamNumber&#x60;.
    /// </remarks>
    /// <exception cref="ApiException">Thrown when fails to make API call</exception>
    /// <param name="eventCode">**[Required] (string)** Optional case insensitive alphanumeric eventCode of the event from which the awards are requested. Must be at least 3 characters.</param>
    /// <param name="season">**[REQUIRED] (int)** Numeric year of the team and event from which the award listings are requested. Must be 4 digits and greater than or equal to 2015, and less than or equal to the current year.</param>
    /// <param name="teamNumber">**[REQUIRED] (int)** Numeric teamNumber of the team about which information is requested. Must be 1 to 4 digits.</param>
    /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
    /// <returns>Task of ApiResponse (Object)</returns>
    Task<ApiResponse<Object?>> SeasonAwardsEventteamEventCodeTeamNumberGetWithHttpInfoAsync(string eventCode, string season, string teamNumber, CancellationToken cancellationToken = default);
    /// <summary>
    /// Awards Listings
    /// </summary>
    /// <remarks>
    /// The award listings API returns a listing of the various awards that can be distributed in the requested season. This is especially useful in order to avoid having to use the &#x60;name&#x60; field of the event awards API to know which award was won. Instead the &#x60;awardId&#x60; field can be matched between the two APIs.
    /// </remarks>
    /// <exception cref="ApiException">Thrown when fails to make API call</exception>
    /// <param name="season">**[REQUIRED] (int)** Numeric year of the event from which the award listings are requested. Must be 4 digits and greater than or equal to 2015, and less than or equal to the current year.</param>
    /// <param name="ifModifiedSince"> (optional)</param>
    /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
    /// <returns>Task of Object</returns>
    Task<Object?> SeasonAwardsListGetAsync(string season, string? ifModifiedSince = default, CancellationToken cancellationToken = default);

    /// <summary>
    /// Awards Listings
    /// </summary>
    /// <remarks>
    /// The award listings API returns a listing of the various awards that can be distributed in the requested season. This is especially useful in order to avoid having to use the &#x60;name&#x60; field of the event awards API to know which award was won. Instead the &#x60;awardId&#x60; field can be matched between the two APIs.
    /// </remarks>
    /// <exception cref="ApiException">Thrown when fails to make API call</exception>
    /// <param name="season">**[REQUIRED] (int)** Numeric year of the event from which the award listings are requested. Must be 4 digits and greater than or equal to 2015, and less than or equal to the current year.</param>
    /// <param name="ifModifiedSince"> (optional)</param>
    /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
    /// <returns>Task of ApiResponse (Object)</returns>
    Task<ApiResponse<Object?>> SeasonAwardsListGetWithHttpInfoAsync(string season, string? ifModifiedSince = default, CancellationToken cancellationToken = default);
    /// <summary>
    /// Team Awards
    /// </summary>
    /// <remarks>
    /// The team awards API returns details about awards presented for a particular team in a particular season. Return values may contain either &#x60;teamNumber&#x60; or &#x60;person&#x60; values, and if the winner was a &#x60;person&#x60;, and that person is from a team, the &#x60;teamNumber&#x60; value might be set with their &#x60;teamNumber&#x60;.
    /// </remarks>
    /// <exception cref="ApiException">Thrown when fails to make API call</exception>
    /// <param name="season">**[REQUIRED] (int)** Numeric year of the team from which the award listings are requested. Must be 4 digits and greater than or equal to 2015, and less than or equal to the current year.</param>
    /// <param name="teamNumber">**[REQUIRED] (int)** Numeric teamNumber of the team about which information is requested. Must be 1 to 4 digits.</param>
    /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
    /// <returns>Task of Object</returns>
    Task<Object?> SeasonAwardsTeamTeamNumberGetAsync(string season, string teamNumber, CancellationToken cancellationToken = default);

    /// <summary>
    /// Team Awards
    /// </summary>
    /// <remarks>
    /// The team awards API returns details about awards presented for a particular team in a particular season. Return values may contain either &#x60;teamNumber&#x60; or &#x60;person&#x60; values, and if the winner was a &#x60;person&#x60;, and that person is from a team, the &#x60;teamNumber&#x60; value might be set with their &#x60;teamNumber&#x60;.
    /// </remarks>
    /// <exception cref="ApiException">Thrown when fails to make API call</exception>
    /// <param name="season">**[REQUIRED] (int)** Numeric year of the team from which the award listings are requested. Must be 4 digits and greater than or equal to 2015, and less than or equal to the current year.</param>
    /// <param name="teamNumber">**[REQUIRED] (int)** Numeric teamNumber of the team about which information is requested. Must be 1 to 4 digits.</param>
    /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
    /// <returns>Task of ApiResponse (Object)</returns>
    Task<ApiResponse<Object?>> SeasonAwardsTeamTeamNumberGetWithHttpInfoAsync(string season, string teamNumber, CancellationToken cancellationToken = default);
    #endregion Asynchronous Operations
}

/// <summary>
/// Represents a collection of functions to interact with the API endpoints
/// </summary>
public interface IAwardsApi : IAwardsApiSync, IAwardsApiAsync { }

/// <summary>
/// Represents a collection of functions to interact with the API endpoints
/// </summary>
public sealed partial class AwardsApi : IAwardsApi
{
    private ExceptionFactory? _exceptionFactory = (name, response) => null;

    /// <summary>
    /// Initializes a new instance of the <see cref="AwardsApi"/> class.
    /// **IMPORTANT** This will also create an instance of HttpClient, which is less than ideal.
    /// It's better to reuse the <see href="https://docs.microsoft.com/en-us/dotnet/architecture/microservices/implement-resilient-applications/use-httpclientfactory-to-implement-resilient-http-requests#issues-with-the-original-httpclient-class-available-in-net">HttpClient and HttpClientHandler</see>.
    /// </summary>
    /// <returns></returns>
    public AwardsApi() : this(basePath: default) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="AwardsApi"/> class.
    /// **IMPORTANT** This will also create an instance of HttpClient, which is less than ideal.
    /// It's better to reuse the <see href="https://docs.microsoft.com/en-us/dotnet/architecture/microservices/implement-resilient-applications/use-httpclientfactory-to-implement-resilient-http-requests#issues-with-the-original-httpclient-class-available-in-net">HttpClient and HttpClientHandler</see>.
    /// </summary>
    /// <param name="basePath">The target service's base path in URL format.</param>
    /// <exception cref="ArgumentException"></exception>
    /// <returns></returns>
    public AwardsApi(string? basePath)
    {
        this.Configuration = FIRST.Client.Configuration.MergeConfigurations(GlobalConfiguration.Instance, new Configuration { BasePath = basePath });
        this.ApiClient = new ApiClient(this.Configuration.BasePath);
        this.Client = this.ApiClient;
        this.AsynchronousClient = this.ApiClient;

        this.ExceptionFactory = FIRST.Client.Configuration.DefaultExceptionFactory;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AwardsApi"/> class using Configuration object.
    /// **IMPORTANT** This will also create an instance of HttpClient, which is less than ideal.
    /// It's better to reuse the <see href="https://docs.microsoft.com/en-us/dotnet/architecture/microservices/implement-resilient-applications/use-httpclientfactory-to-implement-resilient-http-requests#issues-with-the-original-httpclient-class-available-in-net">HttpClient and HttpClientHandler</see>.
    /// </summary>
    /// <param name="configuration">An instance of Configuration.</param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <returns></returns>
    public AwardsApi(Configuration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        this.Configuration = FIRST.Client.Configuration.MergeConfigurations(GlobalConfiguration.Instance, configuration);
        this.ApiClient = new ApiClient(this.Configuration.BasePath);
        this.Client = this.ApiClient;
        this.AsynchronousClient = this.ApiClient;

        this.ExceptionFactory = FIRST.Client.Configuration.DefaultExceptionFactory;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AwardsApi"/> class.
    /// </summary>
    /// <param name="client">An instance of HttpClient.</param>
    /// <param name="handler">An optional instance of HttpClientHandler that is used by HttpClient.</param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <returns></returns>
    /// <remarks>
    /// Some configuration settings will not be applied without passing an HttpClientHandler.
    /// The features affected are: Setting and Retrieving Cookies, Client Certificates, Proxy settings.
    /// </remarks>
    public AwardsApi(HttpClient client, HttpClientHandler? handler = null) : this(client, basePath: default, handler: handler) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="AwardsApi"/> class.
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
    public AwardsApi(HttpClient client, string? basePath, HttpClientHandler? handler = null)
    {
        ArgumentNullException.ThrowIfNull(client);

        this.Configuration = FIRST.Client.Configuration.MergeConfigurations(GlobalConfiguration.Instance, new Configuration { BasePath = basePath });
        this.ApiClient = new ApiClient(client, this.Configuration.BasePath, handler);
        this.Client = this.ApiClient;
        this.AsynchronousClient = this.ApiClient;

        this.ExceptionFactory = FIRST.Client.Configuration.DefaultExceptionFactory;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AwardsApi"/> class using Configuration object.
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
    public AwardsApi(HttpClient client, Configuration configuration, HttpClientHandler? handler = null)
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
    /// Initializes a new instance of the <see cref="AwardsApi"/> class
    /// using a Configuration object and client instance.
    /// </summary>
    /// <param name="client">The client interface for synchronous API access.</param>
    /// <param name="asyncClient">The client interface for asynchronous API access.</param>
    /// <param name="configuration">The configuration object.</param>
    /// <exception cref="ArgumentNullException"></exception>
    public AwardsApi(ISynchronousClient client, IAsynchronousClient asyncClient, IReadableConfiguration configuration)
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
    /// Event Awards The event awards API returns details about awards presented at a particular event in a particular season. Return values may contain either &#x60;teamNumber&#x60; or &#x60;person&#x60; values, and if the winner was a &#x60;person&#x60;, and that person is from a team, the &#x60;teamNumber&#x60; value might be set with their &#x60;teamNumber&#x60;.
    /// </summary>
    /// <exception cref="ApiException">Thrown when fails to make API call</exception>
    /// <param name="eventCode">(Required) **(string)** Optional case insensitive alphanumeric eventCode of the event from which the awards are requested. Must be at least 3 characters.</param>
    /// <param name="season">**[REQUIRED] (int)** Numeric year of the event from which the award listings are requested. Must be 4 digits and greater than or equal to 2015, and less than or equal to the current year.</param>
    /// <param name="ifModifiedSince"> (optional)</param>
    /// <returns>Object</returns>
    public Object? SeasonAwardsEventEventCodeGet(string eventCode, string season, string? ifModifiedSince = default)
    {
        ApiResponse<object?> localVarResponse = SeasonAwardsEventEventCodeGetWithHttpInfo(eventCode, season, ifModifiedSince);
        return localVarResponse.Data;
    }

    /// <summary>
    /// Event Awards The event awards API returns details about awards presented at a particular event in a particular season. Return values may contain either &#x60;teamNumber&#x60; or &#x60;person&#x60; values, and if the winner was a &#x60;person&#x60;, and that person is from a team, the &#x60;teamNumber&#x60; value might be set with their &#x60;teamNumber&#x60;.
    /// </summary>
    /// <exception cref="ApiException">Thrown when fails to make API call</exception>
    /// <param name="eventCode">(Required) **(string)** Optional case insensitive alphanumeric eventCode of the event from which the awards are requested. Must be at least 3 characters.</param>
    /// <param name="season">**[REQUIRED] (int)** Numeric year of the event from which the award listings are requested. Must be 4 digits and greater than or equal to 2015, and less than or equal to the current year.</param>
    /// <param name="ifModifiedSince"> (optional)</param>
    /// <returns>ApiResponse of Object</returns>
    public ApiResponse<Object?> SeasonAwardsEventEventCodeGetWithHttpInfo(string eventCode, string season, string? ifModifiedSince = default)
    {
        // verify the required parameter 'eventCode' is set
        if (eventCode is null)
        {
            throw new ApiException(400, "Missing required parameter 'eventCode' when calling AwardsApi->SeasonAwardsEventEventCodeGet");
        }

        // verify the required parameter 'season' is set
        if (season is null)
        {
            throw new ApiException(400, "Missing required parameter 'season' when calling AwardsApi->SeasonAwardsEventEventCodeGet");
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
        var localVarResponse = this.Client.Get<Object?>("/{season}/awards/event/{eventCode}", localVarRequestOptions, this.Configuration);

        if (this.ExceptionFactory is not null)
        {
            var _exception = this.ExceptionFactory("SeasonAwardsEventEventCodeGet", localVarResponse);
            if (_exception is not null)
            {
                throw _exception;
            }
        }

        return localVarResponse;
    }

    /// <summary>
    /// Event Awards The event awards API returns details about awards presented at a particular event in a particular season. Return values may contain either &#x60;teamNumber&#x60; or &#x60;person&#x60; values, and if the winner was a &#x60;person&#x60;, and that person is from a team, the &#x60;teamNumber&#x60; value might be set with their &#x60;teamNumber&#x60;.
    /// </summary>
    /// <exception cref="ApiException">Thrown when fails to make API call</exception>
    /// <param name="eventCode">(Required) **(string)** Optional case insensitive alphanumeric eventCode of the event from which the awards are requested. Must be at least 3 characters.</param>
    /// <param name="season">**[REQUIRED] (int)** Numeric year of the event from which the award listings are requested. Must be 4 digits and greater than or equal to 2015, and less than or equal to the current year.</param>
    /// <param name="ifModifiedSince"> (optional)</param>
    /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
    /// <returns>Task of Object</returns>
    public async Task<Object?> SeasonAwardsEventEventCodeGetAsync(string eventCode, string season, string? ifModifiedSince = default, CancellationToken cancellationToken = default)
    {
        ApiResponse<object?> localVarResponse = await SeasonAwardsEventEventCodeGetWithHttpInfoAsync(eventCode, season, ifModifiedSince, cancellationToken).ConfigureAwait(false);
        return localVarResponse.Data;
    }

    /// <summary>
    /// Event Awards The event awards API returns details about awards presented at a particular event in a particular season. Return values may contain either &#x60;teamNumber&#x60; or &#x60;person&#x60; values, and if the winner was a &#x60;person&#x60;, and that person is from a team, the &#x60;teamNumber&#x60; value might be set with their &#x60;teamNumber&#x60;.
    /// </summary>
    /// <exception cref="ApiException">Thrown when fails to make API call</exception>
    /// <param name="eventCode">(Required) **(string)** Optional case insensitive alphanumeric eventCode of the event from which the awards are requested. Must be at least 3 characters.</param>
    /// <param name="season">**[REQUIRED] (int)** Numeric year of the event from which the award listings are requested. Must be 4 digits and greater than or equal to 2015, and less than or equal to the current year.</param>
    /// <param name="ifModifiedSince"> (optional)</param>
    /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
    /// <returns>Task of ApiResponse (Object)</returns>
    public async Task<ApiResponse<object?>> SeasonAwardsEventEventCodeGetWithHttpInfoAsync(string eventCode, string season, string? ifModifiedSince = default, CancellationToken cancellationToken = default)
    {
        // verify the required parameter 'eventCode' is set
        if (eventCode is null)
        {
            throw new ApiException(400, "Missing required parameter 'eventCode' when calling AwardsApi->SeasonAwardsEventEventCodeGet");
        }

        // verify the required parameter 'season' is set
        if (season is null)
        {
            throw new ApiException(400, "Missing required parameter 'season' when calling AwardsApi->SeasonAwardsEventEventCodeGet");
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
        var localVarResponse = await this.AsynchronousClient.GetAsync<Object?>("/{season}/awards/event/{eventCode}", localVarRequestOptions, this.Configuration, cancellationToken).ConfigureAwait(false);

        if (this.ExceptionFactory is not null)
        {
            var _exception = this.ExceptionFactory("SeasonAwardsEventEventCodeGet", localVarResponse);
            if (_exception is not null)
            {
                throw _exception;
            }
        }

        return localVarResponse;
    }          /// <summary>
               /// Event Team Awards The event team awards API returns details about awards presented at a particular event in a particular season for a particular team. Return values may contain either &#x60;teamNumber&#x60; or &#x60;person&#x60; values, and if the winner was a &#x60;person&#x60;, and that person is from a team, the &#x60;teamNumber&#x60; value might be set with their &#x60;teamNumber&#x60;.
               /// </summary>
               /// <exception cref="ApiException">Thrown when fails to make API call</exception>
               /// <param name="eventCode">**[Required] (string)** Optional case insensitive alphanumeric eventCode of the event from which the awards are requested. Must be at least 3 characters.</param>
               /// <param name="season">**[REQUIRED] (int)** Numeric year of the team and event from which the award listings are requested. Must be 4 digits and greater than or equal to 2015, and less than or equal to the current year.</param>
               /// <param name="teamNumber">**[REQUIRED] (int)** Numeric teamNumber of the team about which information is requested. Must be 1 to 4 digits.</param>
               /// <returns>Object</returns>
    public Object? SeasonAwardsEventteamEventCodeTeamNumberGet(string eventCode, string season, string teamNumber)
    {
        ApiResponse<object?> localVarResponse = SeasonAwardsEventteamEventCodeTeamNumberGetWithHttpInfo(eventCode, season, teamNumber);
        return localVarResponse.Data;
    }

    /// <summary>
    /// Event Team Awards The event team awards API returns details about awards presented at a particular event in a particular season for a particular team. Return values may contain either &#x60;teamNumber&#x60; or &#x60;person&#x60; values, and if the winner was a &#x60;person&#x60;, and that person is from a team, the &#x60;teamNumber&#x60; value might be set with their &#x60;teamNumber&#x60;.
    /// </summary>
    /// <exception cref="ApiException">Thrown when fails to make API call</exception>
    /// <param name="eventCode">**[Required] (string)** Optional case insensitive alphanumeric eventCode of the event from which the awards are requested. Must be at least 3 characters.</param>
    /// <param name="season">**[REQUIRED] (int)** Numeric year of the team and event from which the award listings are requested. Must be 4 digits and greater than or equal to 2015, and less than or equal to the current year.</param>
    /// <param name="teamNumber">**[REQUIRED] (int)** Numeric teamNumber of the team about which information is requested. Must be 1 to 4 digits.</param>
    /// <returns>ApiResponse of Object</returns>
    public ApiResponse<Object?> SeasonAwardsEventteamEventCodeTeamNumberGetWithHttpInfo(string eventCode, string season, string teamNumber)
    {
        // verify the required parameter 'eventCode' is set
        if (eventCode is null)
        {
            throw new ApiException(400, "Missing required parameter 'eventCode' when calling AwardsApi->SeasonAwardsEventteamEventCodeTeamNumberGet");
        }

        // verify the required parameter 'season' is set
        if (season is null)
        {
            throw new ApiException(400, "Missing required parameter 'season' when calling AwardsApi->SeasonAwardsEventteamEventCodeTeamNumberGet");
        }

        // verify the required parameter 'teamNumber' is set
        if (teamNumber is null)
        {
            throw new ApiException(400, "Missing required parameter 'teamNumber' when calling AwardsApi->SeasonAwardsEventteamEventCodeTeamNumberGet");
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
        localVarRequestOptions.PathParameters.Add("teamNumber", ClientUtils.ParameterToString(teamNumber)); // path parameter
                                                                                                            // authentication (basicAuth) required
                                                                                                            // http basic authentication required
        if (!string.IsNullOrEmpty(this.Configuration.Username) || (!string.IsNullOrEmpty(this.Configuration.Password) && !localVarRequestOptions.HeaderParameters.ContainsKey("Authorization")))
        {
            localVarRequestOptions.HeaderParameters.Add("Authorization", "Basic " + ClientUtils.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password));
        }

        // make the HTTP request
        var localVarResponse = this.Client.Get<Object?>("/{season}/awards/eventteam/{eventCode}/{teamNumber}", localVarRequestOptions, this.Configuration);

        if (this.ExceptionFactory is not null)
        {
            var _exception = this.ExceptionFactory("SeasonAwardsEventteamEventCodeTeamNumberGet", localVarResponse);
            if (_exception is not null)
            {
                throw _exception;
            }
        }

        return localVarResponse;
    }

    /// <summary>
    /// Event Team Awards The event team awards API returns details about awards presented at a particular event in a particular season for a particular team. Return values may contain either &#x60;teamNumber&#x60; or &#x60;person&#x60; values, and if the winner was a &#x60;person&#x60;, and that person is from a team, the &#x60;teamNumber&#x60; value might be set with their &#x60;teamNumber&#x60;.
    /// </summary>
    /// <exception cref="ApiException">Thrown when fails to make API call</exception>
    /// <param name="eventCode">**[Required] (string)** Optional case insensitive alphanumeric eventCode of the event from which the awards are requested. Must be at least 3 characters.</param>
    /// <param name="season">**[REQUIRED] (int)** Numeric year of the team and event from which the award listings are requested. Must be 4 digits and greater than or equal to 2015, and less than or equal to the current year.</param>
    /// <param name="teamNumber">**[REQUIRED] (int)** Numeric teamNumber of the team about which information is requested. Must be 1 to 4 digits.</param>
    /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
    /// <returns>Task of Object</returns>
    public async Task<Object?> SeasonAwardsEventteamEventCodeTeamNumberGetAsync(string eventCode, string season, string teamNumber, CancellationToken cancellationToken = default)
    {
        ApiResponse<object?> localVarResponse = await SeasonAwardsEventteamEventCodeTeamNumberGetWithHttpInfoAsync(eventCode, season, teamNumber, cancellationToken).ConfigureAwait(false);
        return localVarResponse.Data;
    }

    /// <summary>
    /// Event Team Awards The event team awards API returns details about awards presented at a particular event in a particular season for a particular team. Return values may contain either &#x60;teamNumber&#x60; or &#x60;person&#x60; values, and if the winner was a &#x60;person&#x60;, and that person is from a team, the &#x60;teamNumber&#x60; value might be set with their &#x60;teamNumber&#x60;.
    /// </summary>
    /// <exception cref="ApiException">Thrown when fails to make API call</exception>
    /// <param name="eventCode">**[Required] (string)** Optional case insensitive alphanumeric eventCode of the event from which the awards are requested. Must be at least 3 characters.</param>
    /// <param name="season">**[REQUIRED] (int)** Numeric year of the team and event from which the award listings are requested. Must be 4 digits and greater than or equal to 2015, and less than or equal to the current year.</param>
    /// <param name="teamNumber">**[REQUIRED] (int)** Numeric teamNumber of the team about which information is requested. Must be 1 to 4 digits.</param>
    /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
    /// <returns>Task of ApiResponse (Object)</returns>
    public async Task<ApiResponse<object?>> SeasonAwardsEventteamEventCodeTeamNumberGetWithHttpInfoAsync(string eventCode, string season, string teamNumber, CancellationToken cancellationToken = default)
    {
        // verify the required parameter 'eventCode' is set
        if (eventCode is null)
        {
            throw new ApiException(400, "Missing required parameter 'eventCode' when calling AwardsApi->SeasonAwardsEventteamEventCodeTeamNumberGet");
        }

        // verify the required parameter 'season' is set
        if (season is null)
        {
            throw new ApiException(400, "Missing required parameter 'season' when calling AwardsApi->SeasonAwardsEventteamEventCodeTeamNumberGet");
        }

        // verify the required parameter 'teamNumber' is set
        if (teamNumber is null)
        {
            throw new ApiException(400, "Missing required parameter 'teamNumber' when calling AwardsApi->SeasonAwardsEventteamEventCodeTeamNumberGet");
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
        localVarRequestOptions.PathParameters.Add("teamNumber", ClientUtils.ParameterToString(teamNumber)); // path parameter
                                                                                                            // authentication (basicAuth) required
                                                                                                            // http basic authentication required
        if (!string.IsNullOrEmpty(this.Configuration.Username) || (!string.IsNullOrEmpty(this.Configuration.Password) && !localVarRequestOptions.HeaderParameters.ContainsKey("Authorization")))

        {
            localVarRequestOptions.HeaderParameters.Add("Authorization", "Basic " + ClientUtils.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password));
        }

        // make the HTTP request
        var localVarResponse = await this.AsynchronousClient.GetAsync<Object?>("/{season}/awards/eventteam/{eventCode}/{teamNumber}", localVarRequestOptions, this.Configuration, cancellationToken).ConfigureAwait(false);

        if (this.ExceptionFactory is not null)
        {
            var _exception = this.ExceptionFactory("SeasonAwardsEventteamEventCodeTeamNumberGet", localVarResponse);
            if (_exception is not null)
            {
                throw _exception;
            }
        }

        return localVarResponse;
    }          /// <summary>
               /// Awards Listings The award listings API returns a listing of the various awards that can be distributed in the requested season. This is especially useful in order to avoid having to use the &#x60;name&#x60; field of the event awards API to know which award was won. Instead the &#x60;awardId&#x60; field can be matched between the two APIs.
               /// </summary>
               /// <exception cref="ApiException">Thrown when fails to make API call</exception>
               /// <param name="season">**[REQUIRED] (int)** Numeric year of the event from which the award listings are requested. Must be 4 digits and greater than or equal to 2015, and less than or equal to the current year.</param>
               /// <param name="ifModifiedSince"> (optional)</param>
               /// <returns>Object</returns>
    public Object? SeasonAwardsListGet(string season, string? ifModifiedSince = default)
    {
        ApiResponse<object?> localVarResponse = SeasonAwardsListGetWithHttpInfo(season, ifModifiedSince);
        return localVarResponse.Data;
    }

    /// <summary>
    /// Awards Listings The award listings API returns a listing of the various awards that can be distributed in the requested season. This is especially useful in order to avoid having to use the &#x60;name&#x60; field of the event awards API to know which award was won. Instead the &#x60;awardId&#x60; field can be matched between the two APIs.
    /// </summary>
    /// <exception cref="ApiException">Thrown when fails to make API call</exception>
    /// <param name="season">**[REQUIRED] (int)** Numeric year of the event from which the award listings are requested. Must be 4 digits and greater than or equal to 2015, and less than or equal to the current year.</param>
    /// <param name="ifModifiedSince"> (optional)</param>
    /// <returns>ApiResponse of Object</returns>
    public ApiResponse<Object?> SeasonAwardsListGetWithHttpInfo(string season, string? ifModifiedSince = default)
    {
        // verify the required parameter 'season' is set
        if (season is null)
        {
            throw new ApiException(400, "Missing required parameter 'season' when calling AwardsApi->SeasonAwardsListGet");
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
        var localVarResponse = this.Client.Get<Object?>("/{season}/awards/list", localVarRequestOptions, this.Configuration);

        if (this.ExceptionFactory is not null)
        {
            var _exception = this.ExceptionFactory("SeasonAwardsListGet", localVarResponse);
            if (_exception is not null)
            {
                throw _exception;
            }
        }

        return localVarResponse;
    }

    /// <summary>
    /// Awards Listings The award listings API returns a listing of the various awards that can be distributed in the requested season. This is especially useful in order to avoid having to use the &#x60;name&#x60; field of the event awards API to know which award was won. Instead the &#x60;awardId&#x60; field can be matched between the two APIs.
    /// </summary>
    /// <exception cref="ApiException">Thrown when fails to make API call</exception>
    /// <param name="season">**[REQUIRED] (int)** Numeric year of the event from which the award listings are requested. Must be 4 digits and greater than or equal to 2015, and less than or equal to the current year.</param>
    /// <param name="ifModifiedSince"> (optional)</param>
    /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
    /// <returns>Task of Object</returns>
    public async Task<Object?> SeasonAwardsListGetAsync(string season, string? ifModifiedSince = default, CancellationToken cancellationToken = default)
    {
        ApiResponse<object?> localVarResponse = await SeasonAwardsListGetWithHttpInfoAsync(season, ifModifiedSince, cancellationToken).ConfigureAwait(false);
        return localVarResponse.Data;
    }

    /// <summary>
    /// Awards Listings The award listings API returns a listing of the various awards that can be distributed in the requested season. This is especially useful in order to avoid having to use the &#x60;name&#x60; field of the event awards API to know which award was won. Instead the &#x60;awardId&#x60; field can be matched between the two APIs.
    /// </summary>
    /// <exception cref="ApiException">Thrown when fails to make API call</exception>
    /// <param name="season">**[REQUIRED] (int)** Numeric year of the event from which the award listings are requested. Must be 4 digits and greater than or equal to 2015, and less than or equal to the current year.</param>
    /// <param name="ifModifiedSince"> (optional)</param>
    /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
    /// <returns>Task of ApiResponse (Object)</returns>
    public async Task<ApiResponse<object?>> SeasonAwardsListGetWithHttpInfoAsync(string season, string? ifModifiedSince = default, CancellationToken cancellationToken = default)
    {
        // verify the required parameter 'season' is set
        if (season is null)
        {
            throw new ApiException(400, "Missing required parameter 'season' when calling AwardsApi->SeasonAwardsListGet");
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
        var localVarResponse = await this.AsynchronousClient.GetAsync<Object?>("/{season}/awards/list", localVarRequestOptions, this.Configuration, cancellationToken).ConfigureAwait(false);

        if (this.ExceptionFactory is not null)
        {
            var _exception = this.ExceptionFactory("SeasonAwardsListGet", localVarResponse);
            if (_exception is not null)
            {
                throw _exception;
            }
        }

        return localVarResponse;
    }          /// <summary>
               /// Team Awards The team awards API returns details about awards presented for a particular team in a particular season. Return values may contain either &#x60;teamNumber&#x60; or &#x60;person&#x60; values, and if the winner was a &#x60;person&#x60;, and that person is from a team, the &#x60;teamNumber&#x60; value might be set with their &#x60;teamNumber&#x60;.
               /// </summary>
               /// <exception cref="ApiException">Thrown when fails to make API call</exception>
               /// <param name="season">**[REQUIRED] (int)** Numeric year of the team from which the award listings are requested. Must be 4 digits and greater than or equal to 2015, and less than or equal to the current year.</param>
               /// <param name="teamNumber">**[REQUIRED] (int)** Numeric teamNumber of the team about which information is requested. Must be 1 to 4 digits.</param>
               /// <returns>Object</returns>
    public Object? SeasonAwardsTeamTeamNumberGet(string season, string teamNumber)
    {
        ApiResponse<object?> localVarResponse = SeasonAwardsTeamTeamNumberGetWithHttpInfo(season, teamNumber);
        return localVarResponse.Data;
    }

    /// <summary>
    /// Team Awards The team awards API returns details about awards presented for a particular team in a particular season. Return values may contain either &#x60;teamNumber&#x60; or &#x60;person&#x60; values, and if the winner was a &#x60;person&#x60;, and that person is from a team, the &#x60;teamNumber&#x60; value might be set with their &#x60;teamNumber&#x60;.
    /// </summary>
    /// <exception cref="ApiException">Thrown when fails to make API call</exception>
    /// <param name="season">**[REQUIRED] (int)** Numeric year of the team from which the award listings are requested. Must be 4 digits and greater than or equal to 2015, and less than or equal to the current year.</param>
    /// <param name="teamNumber">**[REQUIRED] (int)** Numeric teamNumber of the team about which information is requested. Must be 1 to 4 digits.</param>
    /// <returns>ApiResponse of Object</returns>
    public ApiResponse<Object?> SeasonAwardsTeamTeamNumberGetWithHttpInfo(string season, string teamNumber)
    {
        // verify the required parameter 'season' is set
        if (season is null)
        {
            throw new ApiException(400, "Missing required parameter 'season' when calling AwardsApi->SeasonAwardsTeamTeamNumberGet");
        }

        // verify the required parameter 'teamNumber' is set
        if (teamNumber is null)
        {
            throw new ApiException(400, "Missing required parameter 'teamNumber' when calling AwardsApi->SeasonAwardsTeamTeamNumberGet");
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
        localVarRequestOptions.PathParameters.Add("teamNumber", ClientUtils.ParameterToString(teamNumber)); // path parameter
                                                                                                            // authentication (basicAuth) required
                                                                                                            // http basic authentication required
        if (!string.IsNullOrEmpty(this.Configuration.Username) || (!string.IsNullOrEmpty(this.Configuration.Password) && !localVarRequestOptions.HeaderParameters.ContainsKey("Authorization")))
        {
            localVarRequestOptions.HeaderParameters.Add("Authorization", "Basic " + ClientUtils.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password));
        }

        // make the HTTP request
        var localVarResponse = this.Client.Get<Object?>("/{season}/awards/team/{teamNumber}", localVarRequestOptions, this.Configuration);

        if (this.ExceptionFactory is not null)
        {
            var _exception = this.ExceptionFactory("SeasonAwardsTeamTeamNumberGet", localVarResponse);
            if (_exception is not null)
            {
                throw _exception;
            }
        }

        return localVarResponse;
    }

    /// <summary>
    /// Team Awards The team awards API returns details about awards presented for a particular team in a particular season. Return values may contain either &#x60;teamNumber&#x60; or &#x60;person&#x60; values, and if the winner was a &#x60;person&#x60;, and that person is from a team, the &#x60;teamNumber&#x60; value might be set with their &#x60;teamNumber&#x60;.
    /// </summary>
    /// <exception cref="ApiException">Thrown when fails to make API call</exception>
    /// <param name="season">**[REQUIRED] (int)** Numeric year of the team from which the award listings are requested. Must be 4 digits and greater than or equal to 2015, and less than or equal to the current year.</param>
    /// <param name="teamNumber">**[REQUIRED] (int)** Numeric teamNumber of the team about which information is requested. Must be 1 to 4 digits.</param>
    /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
    /// <returns>Task of Object</returns>
    public async Task<Object?> SeasonAwardsTeamTeamNumberGetAsync(string season, string teamNumber, CancellationToken cancellationToken = default)
    {
        ApiResponse<object?> localVarResponse = await SeasonAwardsTeamTeamNumberGetWithHttpInfoAsync(season, teamNumber, cancellationToken).ConfigureAwait(false);
        return localVarResponse.Data;
    }

    /// <summary>
    /// Team Awards The team awards API returns details about awards presented for a particular team in a particular season. Return values may contain either &#x60;teamNumber&#x60; or &#x60;person&#x60; values, and if the winner was a &#x60;person&#x60;, and that person is from a team, the &#x60;teamNumber&#x60; value might be set with their &#x60;teamNumber&#x60;.
    /// </summary>
    /// <exception cref="ApiException">Thrown when fails to make API call</exception>
    /// <param name="season">**[REQUIRED] (int)** Numeric year of the team from which the award listings are requested. Must be 4 digits and greater than or equal to 2015, and less than or equal to the current year.</param>
    /// <param name="teamNumber">**[REQUIRED] (int)** Numeric teamNumber of the team about which information is requested. Must be 1 to 4 digits.</param>
    /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
    /// <returns>Task of ApiResponse (Object)</returns>
    public async Task<ApiResponse<object?>> SeasonAwardsTeamTeamNumberGetWithHttpInfoAsync(string season, string teamNumber, CancellationToken cancellationToken = default)
    {
        // verify the required parameter 'season' is set
        if (season is null)
        {
            throw new ApiException(400, "Missing required parameter 'season' when calling AwardsApi->SeasonAwardsTeamTeamNumberGet");
        }

        // verify the required parameter 'teamNumber' is set
        if (teamNumber is null)
        {
            throw new ApiException(400, "Missing required parameter 'teamNumber' when calling AwardsApi->SeasonAwardsTeamTeamNumberGet");
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
        localVarRequestOptions.PathParameters.Add("teamNumber", ClientUtils.ParameterToString(teamNumber)); // path parameter
                                                                                                            // authentication (basicAuth) required
                                                                                                            // http basic authentication required
        if (!string.IsNullOrEmpty(this.Configuration.Username) || (!string.IsNullOrEmpty(this.Configuration.Password) && !localVarRequestOptions.HeaderParameters.ContainsKey("Authorization")))

        {
            localVarRequestOptions.HeaderParameters.Add("Authorization", "Basic " + ClientUtils.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password));
        }

        // make the HTTP request
        var localVarResponse = await this.AsynchronousClient.GetAsync<Object?>("/{season}/awards/team/{teamNumber}", localVarRequestOptions, this.Configuration, cancellationToken).ConfigureAwait(false);

        if (this.ExceptionFactory is not null)
        {
            var _exception = this.ExceptionFactory("SeasonAwardsTeamTeamNumberGet", localVarResponse);
            if (_exception is not null)
            {
                throw _exception;
            }
        }

        return localVarResponse;
    }
}
