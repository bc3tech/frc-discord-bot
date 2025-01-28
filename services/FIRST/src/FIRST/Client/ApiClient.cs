/*
 * FRC Events
 *
 * # Overview  _FIRST_/FMS FRC Events API is a service to return relevant information about the _FIRST_ Robotics Competition (FRC). Information is made available from events operating around the world.  For FRC, information is made available by the Field Management System (FMS) server operating at each event site. The FMS will attempt to sync all data from the event to \"the cloud\" as long as internet is available at the venue. If internet is unavailable, or \"goes down\" during the event, the FMS will automatically sync all data from the time the system was offline as soon as the connection is restored. The API will provide data as soon as it has synced, and we do not add any artificial delays. If you are receiving \"stale\" data, it may be because of connectivity problems at the venue. We recommend you try again later, and post on the FIRST FMS TeamForge site if the problem persists. _(Note: FMS does not sync while a match is running, so data that has to do with a particular match should become available once the score has been revealed to the audience at the event.)_  ### Migration and Program Notes:  Pay close attention to the addresses for calling the various endpoints- as well as the notes regarding endpoints with multiple possible responses (i.e. score details and rankings).  # Documentation Notes  All times are listed in the local time to the event venue. HTTP-date values will show their timezone.  If you specify a parameter, but no value for that parameter, it will be ignored. For example, if you request **URL?teamNumber=** the **teamNumber** parameter would be ignored.  We will continue to support the current version of the API plus one version older. Old APIs are depricated once a version \"two times newer\" is available, at minimum 6 months. For example, version 2.0 and 1.0 are supported right now, but 1.0 will not be supported once 2.1 (or 3.0) is available. Versions may also be retired earlier with prior notice here in the documentation.  The full host address of the API is needed in all calls. The version number is required in each call in order to ensure your requests are made (and responses are returned) in the formats you anticipate. The version number for this documentation is found on the top of the page, in the title. If you call this version number, the API responses will match the formats listed below.  All of the APIs are capable of accepting the **Accept** HTTP header with either **application/xml** or **application/json** based on the desired return type. Any API call that results in an **HTTP Status Code** other than **200 (OK)** will only be shown here as an **application/json** response to save space, but the content is the same regardless of the request type. All response will have a **Content-Length** and **Date** response header, but those are not shown in the documentaiton.  For all APIs that accept a query string in addition to the URI base, the order of parameters do not matter, but the name shown in the documentation must match exactly, as does the associated value format as described in details.  For response codes that are not **HTTP 200 (OK)**, the documentation will show a body message that represents a possible response value. While the \"title\" of the **HTTP Status Code** will match those shown in the response codes documentation section exactly, the body of the response will be a more detailed explanation of why that status code is being returned and may not always be exactly as shown in the examples.  None of the APIs will show possible return here in the documentation of **HTTP 401 (Unauthorized)**, but that code applies to all APIs as a possible response if the request is made without a valid token.  ### Last-Modified, FMS-OnlyModifiedSince, and If-Modified-Since Headers  The FRC Events API utilizes the **Last-Modified** and **If-Modified-Since** Headers to communicate with consumers regarding the age of the data they are requesting. With a couple of exceptions, all calls will return a **Last-Modified** Header set with the time at which the data at that endpoint was last modified. The Header will always be set in the HTTP-date format, as described in the [HTTP Protocol](https://www.w3.org/Protocols/rfc2616/rfc2616-sec14.html). There are two exceptions: the **Last-Modified** Header is not set if the endpoint returns no results (such as a request for a schedule with no matches) and will also not be set if the request was an **HTTP DELETE**.  Consumers should keep track of the **Last-Modified** Header, and return it on subsequent calls to the same endpoint as the **If-Modified-Since**. The server will recognize this request, and will only return a result if the data has been modified since the last request. If no changes have been made, an **HTTP 304** will be returned. If data has been modified, ALL data on that call will be returned (for \"only modified\" data, see below).  The FRC Events API also allows a custom header used to filter the return data to a specific subset. This is done by specifying a **FMS-OnlyModifiedSince** header with each call. As with the **If-Modified-Since** header, consumers should keep track of the **Last-Modified** Header, and return it on subsequent calls to the same endpoint as the **FMS-OnlyModifiedSince** Header. The server will recognize this request, and will only return a result if the data has been modified since the last request, and, if returned, the data will only be those portions modified since the included date. If no changes, have been made, an **HTTP 304** will be returned. Using this method, the server and consumer save processing time by only receiving modified data that is in need of update on the consumer side.  If the Headers are improperly passed (such as the wrong Day of Week for the matching date, or a date in the future), the endpoint will simply ignore the Header and return all results. If both headers are specified, the request will be denied.  # Response Codes  The FRC Events API HTTP Status Codes correspond with the [common codes](https://www.w3.org/Protocols/rfc2616/rfc2616-sec10.html), but occasionally with different \"titles\". The \"title\" used by the API is shown next to each of the below possible response HTTP Status Codes. Throughout the documentation, Apiary may automatically show the common \"title\" in example returns (like \"Not Found\" for 404) but on the production server, the \"title\" will instead match those listed below.  ``` HTTP 200 - \"OK\"   ```  The request has succeeded. An entity corresponding to the requested resource is sent in the response. This will be returned as the HTTP Status Code for all request that succeed, even if the body is empty (such as an event that has no rankings, but with a valid season and event code were used)  ``` HTTP 304 - \"Not Modified\"   ```  When utilizing a Header that allows filtered data returns, such as **If-Modified-Since**, this response indicates that no data meets the request.  ``` HTTP 400 - \"Invalid Season Requested\"/\"Malformed Parameter Format In Request\"/\"Missing Parameter In Request\"/\"Invalid API Version Requested\":   ```  The request could not be understood by the server due to malformed syntax. The client SHOULD NOT repeat the request without modifications. Specifically for this API, a 400 response indicates that the requested URI matches with a valid API, but one or more required parameter was malformed or invalid. Examples include an event code that is too short or team number that contains a letter.  ``` HTTP 401 - \"Unauthorized\"   ```  All requests against the API require authentication via a valid user token. Failing to provide one, or providing an invalid one, will warrant a 401 response. The client MAY repeat the request with a suitable Authorization header field.  ``` HTTP 404 - \"Invalid Event Requested\"   ```  Even though the 404 code usually indicates any not found status, a 404 will only be issued in this API when an event cannot be found for the requested season and event code. If the request didn't match a valid API or there were malformed parameters, the response would not receive a 404 but rather a 400 or 501. If this HTTP code is received, the season was a valid season and the event code matched the acceptable style of an event code, but there were no records of an event matching the combination of that season and event code. For example, HTTP 404 would be issued when the event had a different code in the requested season (the codes can change year to year based on event location).  ``` HTTP 500 - \"Internal Server Error\"   ```  The server encountered an unexpected condition which prevented it from fulfilling the request. This is a code sent directly by the server, and has no special alternate definition specific to this API.  ``` HTTP 501 - \"Request Did Not Match Any Current API Pattern\"   ```  The server does not support the functionality required to fulfill the request. Specifically, the request pattern did not match any of the possible APIs, and thus processing was discontinued. This code is also issued when too many optional parameters were included in a single request and fulfilling it would make the result confusing or misleading. Each API will specify which parameters or combination of parameters can be used at the same time.  ``` HTTP 503 - \"Service Unavailable\"   ```  The server is currently unable to handle the request due to a temporary overloading or maintenance of the server. The implication is that this is a temporary condition which will be alleviated after some delay. If known, the length of the delay MAY be indicated in a Retry-After header. This code will not always appear, sometimes the server may outright refuse the connection instead. This is a code sent directly by the server, and has no special alternate definition specific to this API.  See the notes at the top of this documentation for important information about HTTP Status Codes.  # Authorization  In order to make calls against the FRC Events API, you must include an HTTP Header called **Authorization** with the value set as specified below. If a request is made without this header, processing stops and an **HTTP 401** is issued. All **Authorization** headers follow the same format:  Authorization: Basic 000000000000000000000000000000000000000000000000000000000000  Where the Zeros are replaced by your Token. The Token can be formed by taking your **username** and your **AuthorizationKey** and adding a colon. For example, if your **username** is sampleuser and your **AuthorizationKey** is 7eaa6338-a097-4221-ac04-b6120fcc4d49 you would have this string:  **sampleuser:7eaa6338-a097-4221-ac04-b6120fcc4d49**  This string must then be encoded using Base64 Encoded to form the Token, which will be the same length as the example above, but include letters and numbers. For our example, we would have:  c2FtcGxldXNlcjo3ZWFhNjMzOC1hMDk3LTQyMjEtYWMwNC1iNjEyMGZjYzRkNDk=  **NOTICE**: Publicly distributing an application, code snippet, etc, that has your username and token in it, encoded or not, WILL result in your token being blocked from the API. Each user should apply for their own token.  If you wish to acquire a token for your development, you may do so by requesting a token through our automated system on [this website](https://frc-events.firstinspires.org/services/API).  **AUTOMATED REMOVAL**: If you do not activate your account within 72 hours of making your request for a token, or if you do not make at least one API request every twelve (12) months, your account/token will be marked as disabled for inactivity and subject to being deleted. (This policy does not apply to accounts with special operating agreements with FIRST)  ### HTTP401 and Authorization  Each Token can be individually enabled and disabled by _FIRST_. As such, a normally valid combination of **username** and **AuthorizationToken** could still be rejected. The possible return messages you may see in these instances are:  **Incorrect Token** (You supplied an AuthorizationToken, but it wasn't correct)  **Account Disabled, Contact Support** (You have been disabled for excessive traffic or abuse. Contact support)  **Username Not Found** (A username was found, but didn't match any on file)  **Unable To Determine Authorization Token** (The format of the **Authorization** header was incorrect)  # Webhooks  > **COMING SOON**
 *
 * The version of the OpenAPI document: 1.0.0
 * Generated by: https://github.com/openapitools/openapi-generator.git
 */

namespace FIRST.Client;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Text.Json;
using System.Net.Http;
using System.Net.Http.Headers;
  using Polly;

/// <summary>
/// To Serialize/Deserialize JSON using our custom logic, but only when ContentType is JSON.
/// </summary>
internal sealed partial class CustomJsonCodec
{
  private readonly IReadableConfiguration _configuration;
  private static readonly string _contentType = "application/json";
  private readonly JsonSerializerOptions _serializerSettings = new()
  {
    // OpenAPI generated types generally hide default constructors.
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    DictionaryKeyPolicy = JsonNamingPolicy.CamelCase
  };
  
  public CustomJsonCodec(IReadableConfiguration configuration)
  {
    _configuration = configuration;
  }
  
  public CustomJsonCodec(JsonSerializerOptions serializerSettings, IReadableConfiguration configuration)
  {
    _serializerSettings = serializerSettings;
    _configuration = configuration;
  }
  
  /// <summary>
  /// Serialize the object into a JSON string.
  /// </summary>
  /// <param name="obj">object to be serialized.</param>
  /// <returns>A JSON string.</returns>
  public string Serialize(object obj)
  {
    if (obj is FIRST.Model.AbstractOpenAPISchema absObj)
    {
      // the object to be serialized is an oneOf/anyOf schema
      return absObj.ToJson();
    }
    else
    {
      return JsonSerializer.Serialize(obj, _serializerSettings);
    }
  }
  
  public async Task<T?> DeserializeAsync<T>(HttpResponseMessage response, CancellationToken cancellationToken = default)
  {
    var result = (T?)await DeserializeAsync(response, typeof(T), cancellationToken).ConfigureAwait(false);
    return result;
  }
  
  /// <summary>
  /// Deserialize the JSON string into a proper object.
  /// </summary>
  /// <param name="response">The HTTP response.</param>
  /// <param name="type">object type.</param>
  /// <param name="cancellationToken"></param>
  /// <returns>object representation of the JSON string.</returns>
  internal async Task<object?> DeserializeAsync(HttpResponseMessage response, Type type, CancellationToken cancellationToken = default)
  {
    IList<string> headers = [.. response.Headers.Select(x => x.Key + "=" + x.Value)];
    
    if (type == typeof(byte[])) // return byte array
    {
      return await response.Content.ReadAsByteArrayAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
    }
    else if (type == typeof(FileParameter))
    {
      return new FileParameter(await response.Content.ReadAsStreamAsync(cancellationToken: cancellationToken).ConfigureAwait(false));
    }
    
    cancellationToken.ThrowIfCancellationRequested();
    
    // TODO: ? if (type.IsAssignableFrom(typeof(Stream)))
    if (type == typeof(Stream))
    {
      var bytes = await response.Content.ReadAsByteArrayAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
      if (headers is not null)
      {
        var filePath = string.IsNullOrEmpty(_configuration.TempFolderPath)
        ? Path.GetTempPath()
        : _configuration.TempFolderPath;
        var regex = FilenameContentDetector();
        foreach (var header in headers)
        {
          cancellationToken.ThrowIfCancellationRequested();
          var match = regex.Match(header.ToString());
          if (match.Success)
          {
            string fileName = filePath + ClientUtils.SanitizeFilename(match.Groups[1].Value.Replace("\"", "").Replace("'", ""));
            File.WriteAllBytes(fileName, bytes);
            return new FileStream(fileName, FileMode.Open);
          }
        }
      }
      
      var stream = new MemoryStream(bytes);
      return stream;
    }
    
    cancellationToken.ThrowIfCancellationRequested();
    
    if (type.Name.StartsWith("System.Nullable`1[[System.DateTime", StringComparison.Ordinal)) // return a datetime object
    {
      return DateTime.Parse(await response.Content.ReadAsStringAsync(cancellationToken: cancellationToken).ConfigureAwait(false), null, System.Globalization.DateTimeStyles.RoundtripKind);
    }
    
    if (type == typeof(string) || type.Name.StartsWith("System.Nullable", StringComparison.Ordinal)) // return primitive type
    {
      return Convert.ChangeType(await response.Content.ReadAsStringAsync(cancellationToken: cancellationToken).ConfigureAwait(false), type);
    }
    
    // at this point, it must be a model (json)
    try
    {
      return JsonSerializer.Deserialize(await response.Content.ReadAsStringAsync(cancellationToken: cancellationToken).ConfigureAwait(false), type, _serializerSettings);
    }
    catch (Exception e)
    {
      throw new ApiException(500, e.Message);
    }
  }
  
  public string? RootElement { get; set; }
  public string? Namespace { get; set; }
  public string? DateFormat { get; set; }
  
  public static string ContentType
  {
    get { return _contentType; }
    set { throw new InvalidOperationException("Not allowed to set content type."); }
  }
  
  [GeneratedRegex(@"Content-Disposition=.*filename=['""]?([^'""\s]+)['""]?$")]
  private static partial Regex FilenameContentDetector();
}
/// <summary>
/// Provides a default implementation of an Api client (both synchronous and asynchronous implementations),
/// encapsulating general REST accessor use cases.
/// </summary>
public sealed partial class ApiClient : ISynchronousClient, IAsynchronousClient
{
  private readonly string _baseUrl;
  
  private readonly HttpClientHandler? _httpClientHandler;
  private readonly HttpClient _httpClient;
  
  /// <summary>
  /// Specifies the settings on a <see cref="JsonSerializer" /> object.
  /// These settings can be adjusted to accommodate custom serialization rules.
  /// </summary>
  public JsonSerializerOptions SerializerSettings { get; set; } = new()
  {
    // OpenAPI generated types generally hide default constructors.
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    DictionaryKeyPolicy = JsonNamingPolicy.CamelCase
  };
  
  /// <summary>
  /// Initializes a new instance of the <see cref="ApiClient" />, defaulting to the global configurations' base url.
  /// **IMPORTANT** This will also create an instance of HttpClient, which is less than ideal.
  /// It's better to reuse the <see href="https://docs.microsoft.com/en-us/dotnet/architecture/microservices/implement-resilient-applications/use-httpclientfactory-to-implement-resilient-http-requests#issues-with-the-original-httpclient-class-available-in-net">HttpClient and HttpClientHandler</see>.
  /// </summary>
  public ApiClient() : this(GlobalConfiguration.Instance.BasePath) { }
  
  /// <summary>
  /// Initializes a new instance of the <see cref="ApiClient" />.
  /// **IMPORTANT** This will also create an instance of HttpClient, which is less than ideal.
  /// It's better to reuse the <see href="https://docs.microsoft.com/en-us/dotnet/architecture/microservices/implement-resilient-applications/use-httpclientfactory-to-implement-resilient-http-requests#issues-with-the-original-httpclient-class-available-in-net">HttpClient and HttpClientHandler</see>.
  /// </summary>
  /// <param name="basePath">The target service's base path in URL format.</param>
  /// <exception cref="ArgumentException"></exception>
  public ApiClient(string? basePath)
  {
    ArgumentException.ThrowIfNullOrEmpty(basePath);
    
    _httpClientHandler = new HttpClientHandler();
    _httpClient = new HttpClient(_httpClientHandler, true);
    _baseUrl = basePath;
  }
  
  /// <summary>
  /// Initializes a new instance of the <see cref="ApiClient" />, defaulting to the global configurations' base url.
  /// </summary>
  /// <param name="client">An instance of HttpClient.</param>
  /// <param name="handler">An optional instance of HttpClientHandler that is used by HttpClient.</param>
  /// <exception cref="ArgumentNullException"></exception>
  /// <remarks>
  /// Some configuration settings will not be applied without passing an HttpClientHandler.
  /// The features affected are: Setting and Retrieving Cookies, Client Certificates, Proxy settings.
  /// </remarks>
  public ApiClient(HttpClient client, HttpClientHandler? handler = null) : this(client, GlobalConfiguration.Instance.BasePath, handler) { }
  
  /// <summary>
  /// Initializes a new instance of the <see cref="ApiClient" />.
  /// </summary>
  /// <param name="client">An instance of HttpClient.</param>
  /// <param name="basePath">The target service's base path in URL format.</param>
  /// <param name="handler">An optional instance of HttpClientHandler that is used by HttpClient.</param>
  /// <exception cref="ArgumentNullException"></exception>
  /// <exception cref="ArgumentException"></exception>
  /// <remarks>
  /// Some configuration settings will not be applied without passing an HttpClientHandler.
  /// The features affected are: Setting and Retrieving Cookies, Client Certificates, Proxy settings.
  /// </remarks>
  public ApiClient(HttpClient client, string? basePath, HttpClientHandler? handler = null)
  {
    ArgumentNullException.ThrowIfNull(client);
    ArgumentException.ThrowIfNullOrEmpty(basePath);
    
    _httpClientHandler = handler;
    _httpClient = client;
    _baseUrl = basePath;
  }
  
  /// Prepares multipart/form-data content
  static MultipartFormDataContent PrepareMultipartFormDataContent(RequestOptions options)
  {
    string boundary = "---------" + Guid.NewGuid().ToString().ToUpperInvariant();
    var multipartContent = new MultipartFormDataContent(boundary);
    foreach (var formParameter in options.FormParameters.Where(p => p.Value is not null))
    {
      multipartContent.Add(new StringContent(formParameter.Value!), formParameter.Key);
    }
    
    return multipartContent;
  }
  
  /// <summary>
  /// Provides all logic for constructing a new HttpRequestMessage.
  /// At this point, all information for querying the service is known. Here, it is simply
  /// mapped into the a HttpRequestMessage.
  /// </summary>
  /// <param name="method">The http verb.</param>
  /// <param name="path">The target path (or resource).</param>
  /// <param name="options">The additional request options.</param>
  /// <param name="configuration">A per-request configuration object. It is assumed that any merge with
  /// GlobalConfiguration has been done before calling this method.</param>
  /// <returns>[private] A new HttpRequestMessage instance.</returns>
  /// <exception cref="ArgumentNullException"></exception>
  private HttpRequestMessage NewRequest(HttpMethod method, string path, RequestOptions options, IReadableConfiguration configuration)
  {
    ArgumentNullException.ThrowIfNull(path);
    ArgumentNullException.ThrowIfNull(options);
    ArgumentNullException.ThrowIfNull(configuration);
    
    WebRequestPathBuilder builder = new(_baseUrl, path);
    builder.AddPathParameters(options.PathParameters);
    builder.AddQueryParameters(options.QueryParameters);
    
    HttpRequestMessage request = new(method, builder.GetFullUri());
    
    if (configuration.UserAgent is not null)
    {
      request.Headers.TryAddWithoutValidation("User-Agent", configuration.UserAgent);
    }
    
    if (configuration.DefaultHeaders is not null)
    {
      foreach (var headerParam in configuration.DefaultHeaders)
      {
        request.Headers.Add(headerParam.Key, headerParam.Value);
      }
    }
    
    if (options.HeaderParameters is not null)
    {
      foreach (var headerParam in options.HeaderParameters)
      {
        foreach (var value in headerParam.Value)
        {
          // Todo make content headers actually content headers
          request.Headers.TryAddWithoutValidation(headerParam.Key, value);
        }
      }
    }
    
    List<Tuple<HttpContent, string, string>> contentList = [];
    
    string? contentType = null;
    if (options.HeaderParameters is not null && options.HeaderParameters.TryGetValue("Content-Type", out var contentTypes) && contentTypes is not null)
    {
      contentType = contentTypes.FirstOrDefault();
    }
    
    if (contentType is "multipart/form-data")
    {
      request.Content = PrepareMultipartFormDataContent(options);
    }
    else if (contentType is "application/x-www-form-urlencoded")
    {
      request.Content = new FormUrlEncodedContent(options.FormParameters);
    }
    else
    {
      if (options.Data is not null)
      {
        if (options.Data is FileParameter fp)
        {
          contentType ??= "application/octet-stream";
          
          var streamContent = new StreamContent(fp.Content);
          streamContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);
          request.Content = streamContent;
        }
        else
        {
          var serializer = new CustomJsonCodec(SerializerSettings, configuration);
          request.Content = new StringContent(serializer.Serialize(options.Data), Encoding.UTF8, "application/json");
        }
      }
    }
    
    // TODO provide an alternative that allows cookies per request instead of per API client
    if (options.Cookies is not null && options.Cookies.Count > 0)
    {
      request.Options.Set(new HttpRequestOptionsKey<List<Cookie>>("CookieContainer"), options.Cookies);
    }
    
    return request;
  }
  
  partial void InterceptRequest(HttpRequestMessage req);
  partial void InterceptResponse(HttpRequestMessage req, HttpResponseMessage response);
  
  private async Task<ApiResponse<T>> ToApiResponseAsync<T>(HttpResponseMessage response, T? responseData, Uri? uri, CancellationToken cancellationToken = default)
  {
    string rawContent = await response.Content.ReadAsStringAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
    
    var transformed = new ApiResponse<T>(response.StatusCode, [], responseData, rawContent)
    {
      ErrorText = response.ReasonPhrase,
      Cookies = []
    };
    
    // process response headers, e.g. Access-Control-Allow-Methods
    if (response.Headers is not null)
    {
      foreach (var responseHeader in response.Headers)
      {
        cancellationToken.ThrowIfCancellationRequested();
        transformed.Headers.Add(responseHeader.Key, ClientUtils.ParameterToString(responseHeader.Value));
      }
    }
    
    // process response content headers, e.g. Content-Type
    if (response.Content.Headers is not null)
    {
      foreach (var responseHeader in response.Content.Headers)
      {
        cancellationToken.ThrowIfCancellationRequested();
        transformed.Headers.Add(responseHeader.Key, ClientUtils.ParameterToString(responseHeader.Value));
      }
    }
    
    if (_httpClientHandler is not null && response is not null && uri is not null)
    {
      try
      {
        foreach (Cookie cookie in _httpClientHandler.CookieContainer.GetCookies(uri))
        {
          cancellationToken.ThrowIfCancellationRequested();
          transformed.Cookies.Add(cookie);
        }
      }
      catch (PlatformNotSupportedException) {}
    }
    
    return transformed;
  }
  
  [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "VSTHRD002:Avoid problematic synchronous waits", Justification = "YOLO")]
  private ApiResponse<T> Exec<T>(HttpRequestMessage req, IReadableConfiguration configuration) => ExecAsync<T>(req, configuration).GetAwaiter().GetResult();
  
  private async Task<ApiResponse<T>> ExecAsync<T>(HttpRequestMessage req, IReadableConfiguration configuration, CancellationToken cancellationToken = default)
  {
    CancellationTokenSource? timeoutTokenSource = null;
    CancellationTokenSource? finalTokenSource = null;
    var deserializer = new CustomJsonCodec(SerializerSettings, configuration);
    var finalToken = cancellationToken;
    
    try
    {
      if (configuration.Timeout > 0)
      {
        timeoutTokenSource = new CancellationTokenSource(configuration.Timeout);
        finalTokenSource = CancellationTokenSource.CreateLinkedTokenSource(finalToken, timeoutTokenSource.Token);
        finalToken = finalTokenSource.Token;
      }
      
      if (configuration.Proxy is not null)
      {
        if(_httpClientHandler is null)
        {
          throw new InvalidOperationException("Configuration `Proxy` not supported when the client is explicitly created without an HttpClientHandler, use the proper constructor.");
        }
        
        _httpClientHandler.Proxy = configuration.Proxy;
      }
      
      if (configuration.ClientCertificates is not null)
      {
        if(_httpClientHandler is null)
        {
          throw new InvalidOperationException("Configuration `ClientCertificates` not supported when the client is explicitly created without an HttpClientHandler, use the proper constructor.");
        }
        
        _httpClientHandler.ClientCertificates.AddRange(configuration.ClientCertificates);
      }
      
      if (req.Options.TryGetValue(new HttpRequestOptionsKey<List<Cookie>>("CookieContainer"), out var cookieContainer) && cookieContainer is not null)
      {
        if(_httpClientHandler is null)
        {
          throw new InvalidOperationException("Request property `CookieContainer` not supported when the client is explicitly created without an HttpClientHandler, use the proper constructor.");
        }
        
        foreach (var cookie in cookieContainer)
        {
          _httpClientHandler.CookieContainer.Add(cookie);
        }
      }
      
      InterceptRequest(req);
      
      HttpResponseMessage response;
        if (RetryConfiguration.AsyncRetryPolicy is not null)
        {
          var policy = RetryConfiguration.AsyncRetryPolicy;
          var policyResult = await policy.ExecuteAndCaptureAsync(() => _httpClient.SendAsync(req, finalToken)).ConfigureAwait(false);
          response = (policyResult.Outcome == OutcomeType.Successful)
          ? policyResult.Result
          : new HttpResponseMessage()
          {
            ReasonPhrase = policyResult.FinalException.ToString(),
            RequestMessage = req
          };
        }
        else
        {
        response = await _httpClient.SendAsync(req, finalToken).ConfigureAwait(false);
        }
      
      if (!response.IsSuccessStatusCode)
      {
        return await ToApiResponseAsync<T>(response, default, req.RequestUri, cancellationToken: finalToken).ConfigureAwait(false);
      }
      
      T? responseData = await deserializer.DeserializeAsync<T>(response, cancellationToken: finalToken).ConfigureAwait(false);
      
      // if the response type is oneOf/anyOf, call FromJSON to deserialize the data
      if (typeof(Model.AbstractOpenAPISchema).IsAssignableFrom(typeof(T)))
      {
        responseData = (T?) typeof(T).GetMethod("FromJson")?.Invoke(null, [response.Content]);
      }
      else if (typeof(T).Name is "Stream") // for binary response
      {
        responseData = (T?) (object) await response.Content.ReadAsStreamAsync(cancellationToken: finalToken).ConfigureAwait(false);
      }
      
      InterceptResponse(req, response);
      
      return await ToApiResponseAsync(response, responseData, req.RequestUri, cancellationToken: finalToken).ConfigureAwait(false);
    }
    catch (OperationCanceledException original)
    {
      if (timeoutTokenSource is not null && timeoutTokenSource.IsCancellationRequested)
      {
        throw new TaskCanceledException($"[{req.Method}] {req.RequestUri} was timeout.", new TimeoutException(original.Message, original));
      }
      
      throw;
    }
    finally
    {
      timeoutTokenSource?.Dispose();
      finalTokenSource?.Dispose();
    }
  }
  
    #region IAsynchronousClient
    /// <summary>
    /// Make a HTTP GET request (async).
    /// </summary>
    /// <param name="path">The target path (or resource).</param>
    /// <param name="options">The additional request options.</param>
    /// <param name="configuration">A per-request configuration object. It is assumed that any merge with
    /// GlobalConfiguration has been done before calling this method.</param>
    /// <param name="cancellationToken">Token that enables callers to cancel the request.</param>
    /// <returns>A Task containing ApiResponse</returns>
    public Task<ApiResponse<T>> GetAsync<T>(string path, RequestOptions options, IReadableConfiguration? configuration = null, CancellationToken cancellationToken = default)
    {
      var config = configuration ?? GlobalConfiguration.Instance;
      return ExecAsync<T>(NewRequest(HttpMethod.Get, path, options, config), config, cancellationToken);
    }
    
    /// <summary>
    /// Make a HTTP POST request (async).
    /// </summary>
    /// <param name="path">The target path (or resource).</param>
    /// <param name="options">The additional request options.</param>
    /// <param name="configuration">A per-request configuration object. It is assumed that any merge with
    /// GlobalConfiguration has been done before calling this method.</param>
    /// <param name="cancellationToken">Token that enables callers to cancel the request.</param>
    /// <returns>A Task containing ApiResponse</returns>
    public Task<ApiResponse<T>> PostAsync<T>(string path, RequestOptions options, IReadableConfiguration? configuration = null, CancellationToken cancellationToken = default)
    {
      var config = configuration ?? GlobalConfiguration.Instance;
      return ExecAsync<T>(NewRequest(HttpMethod.Post, path, options, config), config, cancellationToken);
    }
    
    /// <summary>
    /// Make a HTTP PUT request (async).
    /// </summary>
    /// <param name="path">The target path (or resource).</param>
    /// <param name="options">The additional request options.</param>
    /// <param name="configuration">A per-request configuration object. It is assumed that any merge with
    /// GlobalConfiguration has been done before calling this method.</param>
    /// <param name="cancellationToken">Token that enables callers to cancel the request.</param>
    /// <returns>A Task containing ApiResponse</returns>
    public Task<ApiResponse<T>> PutAsync<T>(string path, RequestOptions options, IReadableConfiguration? configuration = null, CancellationToken cancellationToken = default)
    {
      var config = configuration ?? GlobalConfiguration.Instance;
      return ExecAsync<T>(NewRequest(HttpMethod.Put, path, options, config), config, cancellationToken);
    }
    
    /// <summary>
    /// Make a HTTP DELETE request (async).
    /// </summary>
    /// <param name="path">The target path (or resource).</param>
    /// <param name="options">The additional request options.</param>
    /// <param name="configuration">A per-request configuration object. It is assumed that any merge with
    /// GlobalConfiguration has been done before calling this method.</param>
    /// <param name="cancellationToken">Token that enables callers to cancel the request.</param>
    /// <returns>A Task containing ApiResponse</returns>
    public Task<ApiResponse<T>> DeleteAsync<T>(string path, RequestOptions options, IReadableConfiguration? configuration = null, CancellationToken cancellationToken = default)
    {
      var config = configuration ?? GlobalConfiguration.Instance;
      return ExecAsync<T>(NewRequest(HttpMethod.Delete, path, options, config), config, cancellationToken);
    }
    
    /// <summary>
    /// Make a HTTP HEAD request (async).
    /// </summary>
    /// <param name="path">The target path (or resource).</param>
    /// <param name="options">The additional request options.</param>
    /// <param name="configuration">A per-request configuration object. It is assumed that any merge with
    /// GlobalConfiguration has been done before calling this method.</param>
    /// <param name="cancellationToken">Token that enables callers to cancel the request.</param>
    /// <returns>A Task containing ApiResponse</returns>
    public Task<ApiResponse<T>> HeadAsync<T>(string path, RequestOptions options, IReadableConfiguration? configuration = null, CancellationToken cancellationToken = default)
    {
      var config = configuration ?? GlobalConfiguration.Instance;
      return ExecAsync<T>(NewRequest(HttpMethod.Head, path, options, config), config, cancellationToken);
    }
    
    /// <summary>
    /// Make a HTTP OPTION request (async).
    /// </summary>
    /// <param name="path">The target path (or resource).</param>
    /// <param name="options">The additional request options.</param>
    /// <param name="configuration">A per-request configuration object. It is assumed that any merge with
    /// GlobalConfiguration has been done before calling this method.</param>
    /// <param name="cancellationToken">Token that enables callers to cancel the request.</param>
    /// <returns>A Task containing ApiResponse</returns>
    public Task<ApiResponse<T>> OptionsAsync<T>(string path, RequestOptions options, IReadableConfiguration? configuration = null, CancellationToken cancellationToken = default)
    {
      var config = configuration ?? GlobalConfiguration.Instance;
      return ExecAsync<T>(NewRequest(HttpMethod.Options, path, options, config), config, cancellationToken);
    }
    
    /// <summary>
    /// Make a HTTP PATCH request (async).
    /// </summary>
    /// <param name="path">The target path (or resource).</param>
    /// <param name="options">The additional request options.</param>
    /// <param name="configuration">A per-request configuration object. It is assumed that any merge with
    /// GlobalConfiguration has been done before calling this method.</param>
    /// <param name="cancellationToken">Token that enables callers to cancel the request.</param>
    /// <returns>A Task containing ApiResponse</returns>
    public Task<ApiResponse<T>> PatchAsync<T>(string path, RequestOptions options, IReadableConfiguration? configuration = null, CancellationToken cancellationToken = default)
    {
      var config = configuration ?? GlobalConfiguration.Instance;
      return ExecAsync<T>(NewRequest(new HttpMethod("PATCH"), path, options, config), config, cancellationToken);
    }
    #endregion IAsynchronousClient
  
  #region ISynchronousClient
  /// <summary>
  /// Make a HTTP GET request (synchronous).
  /// </summary>
  /// <param name="path">The target path (or resource).</param>
  /// <param name="options">The additional request options.</param>
  /// <param name="configuration">A per-request configuration object. It is assumed that any merge with
  /// GlobalConfiguration has been done before calling this method.</param>
  /// <returns>A Task containing ApiResponse</returns>
  public ApiResponse<T> Get<T>(string path, RequestOptions options, IReadableConfiguration? configuration = null)
  {
    var config = configuration ?? GlobalConfiguration.Instance;
    return Exec<T>(NewRequest(HttpMethod.Get, path, options, config), config);
  }
  
  /// <summary>
  /// Make a HTTP POST request (synchronous).
  /// </summary>
  /// <param name="path">The target path (or resource).</param>
  /// <param name="options">The additional request options.</param>
  /// <param name="configuration">A per-request configuration object. It is assumed that any merge with
  /// GlobalConfiguration has been done before calling this method.</param>
  /// <returns>A Task containing ApiResponse</returns>
  public ApiResponse<T> Post<T>(string path, RequestOptions options, IReadableConfiguration? configuration = null)
  {
    var config = configuration ?? GlobalConfiguration.Instance;
    return Exec<T>(NewRequest(HttpMethod.Post, path, options, config), config);
  }
  
  /// <summary>
  /// Make a HTTP PUT request (synchronous).
  /// </summary>
  /// <param name="path">The target path (or resource).</param>
  /// <param name="options">The additional request options.</param>
  /// <param name="configuration">A per-request configuration object. It is assumed that any merge with
  /// GlobalConfiguration has been done before calling this method.</param>
  /// <returns>A Task containing ApiResponse</returns>
  public ApiResponse<T> Put<T>(string path, RequestOptions options, IReadableConfiguration? configuration = null)
  {
    var config = configuration ?? GlobalConfiguration.Instance;
    return Exec<T>(NewRequest(HttpMethod.Put, path, options, config), config);
  }
  
  /// <summary>
  /// Make a HTTP DELETE request (synchronous).
  /// </summary>
  /// <param name="path">The target path (or resource).</param>
  /// <param name="options">The additional request options.</param>
  /// <param name="configuration">A per-request configuration object. It is assumed that any merge with
  /// GlobalConfiguration has been done before calling this method.</param>
  /// <returns>A Task containing ApiResponse</returns>
  public ApiResponse<T> Delete<T>(string path, RequestOptions options, IReadableConfiguration? configuration = null)
  {
    var config = configuration ?? GlobalConfiguration.Instance;
    return Exec<T>(NewRequest(HttpMethod.Delete, path, options, config), config);
  }
  
  /// <summary>
  /// Make a HTTP HEAD request (synchronous).
  /// </summary>
  /// <param name="path">The target path (or resource).</param>
  /// <param name="options">The additional request options.</param>
  /// <param name="configuration">A per-request configuration object. It is assumed that any merge with
  /// GlobalConfiguration has been done before calling this method.</param>
  /// <returns>A Task containing ApiResponse</returns>
  public ApiResponse<T> Head<T>(string path, RequestOptions options, IReadableConfiguration? configuration = null)
  {
    var config = configuration ?? GlobalConfiguration.Instance;
    return Exec<T>(NewRequest(HttpMethod.Head, path, options, config), config);
  }
  
  /// <summary>
  /// Make a HTTP OPTION request (synchronous).
  /// </summary>
  /// <param name="path">The target path (or resource).</param>
  /// <param name="options">The additional request options.</param>
  /// <param name="configuration">A per-request configuration object. It is assumed that any merge with
  /// GlobalConfiguration has been done before calling this method.</param>
  /// <returns>A Task containing ApiResponse</returns>
  public ApiResponse<T> Options<T>(string path, RequestOptions options, IReadableConfiguration? configuration = null)
  {
    var config = configuration ?? GlobalConfiguration.Instance;
    return Exec<T>(NewRequest(HttpMethod.Options, path, options, config), config);
  }
  
  /// <summary>
  /// Make a HTTP PATCH request (synchronous).
  /// </summary>
  /// <param name="path">The target path (or resource).</param>
  /// <param name="options">The additional request options.</param>
  /// <param name="configuration">A per-request configuration object. It is assumed that any merge with
  /// GlobalConfiguration has been done before calling this method.</param>
  /// <returns>A Task containing ApiResponse</returns>
  public ApiResponse<T> Patch<T>(string path, RequestOptions options, IReadableConfiguration? configuration = null)
  {
    var config = configuration ?? GlobalConfiguration.Instance;
    return Exec<T>(NewRequest(new HttpMethod("PATCH"), path, options, config), config);
  }
  #endregion ISynchronousClient
}