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
  using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;

/// <summary>
/// Represents a set of configuration settings
/// </summary>
internal class Configuration : IReadableConfiguration
{
  #region Constants
  
  /// <summary>
  /// Version of the package.
  /// </summary>
  /// <value>Version of the package.</value>
  public const string Version = "1.0.0";
  
  /// <summary>
  /// Identifier for ISO 8601 DateTime Format
  /// </summary>
  /// <remarks>See https://msdn.microsoft.com/en-us/library/az4se3k1(v=vs.110).aspx#Anchor_8 for more information.</remarks>
  // ReSharper disable once InconsistentNaming
  public const string ISO8601_DATETIME_FORMAT = "o";
  
  #endregion Constants
  
  #region Static Members
  
  /// <summary>
  /// Default creation of exceptions for a given method name and response object
  /// </summary>
  public static readonly ExceptionFactory DefaultExceptionFactory = (methodName, response) =>
  {
    var status = (int)response.StatusCode;
    return status switch
    {
      >= 400 => new ApiException(status,
      string.Format("Error calling {0}: {1}", methodName, response.RawContent),
      response.RawContent, response.Headers),
        0 => new ApiException(status, string.Format("Error calling {0}: {1}", methodName, response.ErrorText), response.ErrorText),
      _ => null
    };
  };
  
  #endregion Static Members
  
  private string _dateTimeFormat = ISO8601_DATETIME_FORMAT;
  private string _tempFolderPath = Path.GetTempPath();
  
  #region Constructors
  
  /// <summary>
  /// Initializes a new instance of the <see cref="Configuration" /> class
  /// </summary>
  [System.Diagnostics.CodeAnalysis.SuppressMessage("ReSharper", "VirtualMemberCallInConstructor")]
  public Configuration()
  {
    UserAgent = WebUtility.UrlEncode("OpenAPI-Generator/1.0.0/csharp");
    BasePath = "https://frc-api.firstinspires.org";
    DefaultHeaders = new ConcurrentDictionary<string, string>();
    ApiKey = new ConcurrentDictionary<string, string>();
    ApiKeyPrefix = new ConcurrentDictionary<string, string>();
        Servers = new List<IReadOnlyDictionary<string, object>>()
        {
        {
          new Dictionary<string, object> {
            {"url", "https://frc-api.firstinspires.org"},
            {"description", "No description provided"},
          }
        }
        };
    OperationServers = new Dictionary<string, List<IReadOnlyDictionary<string, object>>>()
    {
    };
    
    // Setting Timeout has side effects (forces ApiClient creation).
    Timeout = 100000;
  }
  
  /// <summary>
  /// Initializes a new instance of the <see cref="Configuration" /> class
  /// </summary>
  [System.Diagnostics.CodeAnalysis.SuppressMessage("ReSharper", "VirtualMemberCallInConstructor")]
  public Configuration(IDictionary<string, string> defaultHeaders, IDictionary<string, string> apiKey, IDictionary<string, string> apiKeyPrefix, string basePath = "https://frc-api.firstinspires.org") : this()
  {
    ArgumentException.ThrowIfNullOrWhiteSpace(basePath);
    this.BasePath = basePath;
    
    ArgumentNullException.ThrowIfNull(defaultHeaders);
    this.DefaultHeaders = defaultHeaders;
    
    ArgumentNullException.ThrowIfNull(apiKey);
    this.ApiKey = apiKey;
    
    ArgumentNullException.ThrowIfNull(apiKeyPrefix);
    this.ApiKeyPrefix = apiKeyPrefix;
  }
  
  #endregion Constructors
  
  #region Properties
  
  /// <summary>
  /// Gets or sets the base path for API access.
  /// </summary>
  public virtual string? BasePath { get; set; }
  
  /// <summary>
  /// Determine whether or not the "default credentials" (e.g. the user account under which the current process is running) will be sent along to the server. The default is false.
  /// </summary>
  public virtual bool UseDefaultCredentials { get; set; }
  
  /// <summary>
  /// Gets or sets the default headers.
  /// </summary>
  public virtual IDictionary<string, string> DefaultHeaders { get; set; }
  
  /// <summary>
  /// Gets or sets the HTTP timeout (milliseconds) of ApiClient. Default to 100000 milliseconds.
  /// </summary>
  public virtual int Timeout { get; set; }
  
  /// <summary>
  /// Gets or sets the proxy
  /// </summary>
  /// <value>Proxy.</value>
  public virtual WebProxy? Proxy { get; set; }
  
  /// <summary>
  /// Gets or sets the HTTP user agent.
  /// </summary>
  /// <value>Http user agent.</value>
  public virtual string UserAgent { get; set; }
  
  /// <summary>
  /// Gets or sets the username (HTTP basic authentication).
  /// </summary>
  /// <value>The username.</value>
  public virtual string? Username { get; set; }
  
  /// <summary>
  /// Gets or sets the password (HTTP basic authentication).
  /// </summary>
  /// <value>The password.</value>
  public virtual string? Password { get; set; }
  
  /// <summary>
  /// Gets the API key with prefix.
  /// </summary>
  /// <param name="apiKeyIdentifier">API key identifier (authentication scheme).</param>
  /// <returns>API key with prefix.</returns>
  public string? GetApiKeyWithPrefix(string apiKeyIdentifier)
  {
    return ApiKey.TryGetValue(apiKeyIdentifier, out var apiKeyValue) && apiKeyValue is not null && ApiKeyPrefix.TryGetValue(apiKeyIdentifier, out var apiKeyPrefix) && apiKeyPrefix is not null
    ? apiKeyPrefix + " " + apiKeyValue
    : null;
  }
  
  /// <summary>
  /// Gets or sets certificate collection to be sent with requests.
  /// </summary>
  /// <value>X509 Certificate collection.</value>
  public X509CertificateCollection? ClientCertificates { get; set; }
  
  /// <summary>
  /// Gets or sets the access token for OAuth2 authentication.
  ///
  /// This helper property simplifies code generation.
  /// </summary>
  /// <value>The access token.</value>
  public virtual string? AccessToken { get; set; }
  
  /// <summary>
  /// Gets or sets the temporary folder path to store the files downloaded from the server.
  /// </summary>
  /// <value>Folder path.</value>
  public virtual string TempFolderPath
  {
    get => _tempFolderPath;
    set
    {
      if (string.IsNullOrEmpty(value))
      {
        _tempFolderPath = Path.GetTempPath();
      }
      else
      {
        // create the directory if it does not exist
        if (!Directory.Exists(value))
        {
          Directory.CreateDirectory(value);
        }
        
        // check if the path contains directory separator at the end
        _tempFolderPath = value[^1] == Path.DirectorySeparatorChar ? value : value + Path.DirectorySeparatorChar;
      }
    }
  }
  
  /// <summary>
  /// Gets or sets the date time format used when serializing in the ApiClient
  /// By default, it's set to ISO 8601 - "o", for others see:
  /// https://msdn.microsoft.com/en-us/library/az4se3k1(v=vs.110).aspx
  /// and https://msdn.microsoft.com/en-us/library/8kb3ddd4(v=vs.110).aspx
  /// No validation is done to ensure that the string you're providing is valid
  /// </summary>
  /// <value>The DateTimeFormat string</value>
  public virtual string DateTimeFormat
  {
    get => _dateTimeFormat;
    set
    {
      if (string.IsNullOrEmpty(value))
      {
        // Never allow a blank or null string, go back to the default
        _dateTimeFormat = ISO8601_DATETIME_FORMAT;
        return;
      }
      
      // Caution, no validation when you choose date time format other than ISO 8601
      // Take a look at the above links
      _dateTimeFormat = value;
    }
  }
  
  /// <summary>
  /// Gets or sets the prefix (e.g. Token) of the API key based on the authentication name.
  ///
  /// Whatever you set here will be prepended to the value defined in AddApiKey.
  ///
  /// An example invocation here might be:
  /// <example>
  /// ApiKeyPrefix["Authorization"] = "Bearer";
  /// </example>
  /// … where ApiKey["Authorization"] would then be used to set the value of your bearer token.
  ///
  /// <remarks>
  /// OAuth2 workflows should set tokens via AccessToken.
  /// </remarks>
  /// </summary>
  /// <value>The prefix of the API key.</value>
  public virtual IDictionary<string, string> ApiKeyPrefix { get; set; }
  
  /// <summary>
  /// Gets or sets the API key based on the authentication name.
  /// </summary>
  /// <value>The API key.</value>
  public virtual IDictionary<string, string> ApiKey { get; set; }
    
    /// <summary>
    /// Gets or sets the servers.
    /// </summary>
    /// <value>The servers.</value>
    public virtual IList<IReadOnlyDictionary<string, object>> Servers { get; set; }
    
    /// <summary>
    /// Gets or sets the operation servers.
    /// </summary>
    /// <value>The operation servers.</value>
    public virtual IReadOnlyDictionary<string, List<IReadOnlyDictionary<string, object>>> OperationServers { get; set; }
    
    /// <summary>
    /// Returns URL based on server settings without providing values
    /// for the variables
    /// </summary>
    /// <param name="index">Array index of the server settings.</param>
    /// <return>The server URL.</return>
    public string GetServerUrl(int index) => GetServerUrl(Servers, index, null);
    
    /// <summary>
    /// Returns URL based on server settings.
    /// </summary>
    /// <param name="index">Array index of the server settings.</param>
    /// <param name="inputVariables">Dictionary of the variables and the corresponding values.</param>
    /// <return>The server URL.</return>
    public string GetServerUrl(int index, Dictionary<string, string>? inputVariables) => GetServerUrl(Servers, index, inputVariables);
    
    /// <summary>
    /// Returns URL based on operation server settings.
    /// </summary>
    /// <param name="operation">Operation associated with the request path.</param>
    /// <param name="index">Array index of the server settings.</param>
    /// <return>The operation server URL.</return>
    public string? GetOperationServerUrl(string operation, int index) => GetOperationServerUrl(operation, index, null);
    
    /// <summary>
    /// Returns URL based on operation server settings.
    /// </summary>
    /// <param name="operation">Operation associated with the request path.</param>
    /// <param name="index">Array index of the server settings.</param>
    /// <param name="inputVariables">Dictionary of the variables and the corresponding values.</param>
    /// <return>The operation server URL.</return>
    public string? GetOperationServerUrl(string operation, int index, Dictionary<string, string>? inputVariables)
    {
      return operation switch
      {
        not null when OperationServers.TryGetValue(operation, out var operationServer) => GetServerUrl(operationServer, index, inputVariables),
        _ => null
      };
    }
    
    /// <summary>
    /// Returns URL based on server settings.
    /// </summary>
    /// <param name="servers">Dictionary of server settings.</param>
    /// <param name="index">Array index of the server settings.</param>
    /// <param name="inputVariables">Dictionary of the variables and the corresponding values.</param>
    /// <return>The server URL.</return>
    private static string GetServerUrl(IList<IReadOnlyDictionary<string, object>> servers, int index, Dictionary<string, string>? inputVariables)
    {
      if (index < 0 || index >= servers.Count)
      {
        throw new InvalidOperationException($"Invalid index {index} when selecting the server. Must be less than {servers.Count}.");
      }
      
      inputVariables ??= [];
      
      IReadOnlyDictionary<string, object> server = servers[index];
      string url = (string)server["url"];
      
      if (server.ContainsKey("variables"))
      {
        // go through each variable and assign a value
        foreach (KeyValuePair<string, object> variable in (IReadOnlyDictionary<string, object>)server["variables"])
        {
          IReadOnlyDictionary<string, object> serverVariables = (IReadOnlyDictionary<string, object>)variable.Value;
          
          if (inputVariables.ContainsKey(variable.Key))
          {
            if (((List<string>)serverVariables["enum_values"]).Contains(inputVariables[variable.Key]))
            {
              url = url.Replace("{" + variable.Key + "}", inputVariables[variable.Key]);
            }
            else
            {
              throw new InvalidOperationException($"The variable `{variable.Key}` in the server URL has invalid value #{inputVariables[variable.Key]}. Must be {(List<string>)serverVariables["enum_values"]}");
            }
          }
          else
          {
            // use default value
            url = url.Replace("{" + variable.Key + "}", (string)serverVariables["default_value"]);
          }
        }
      }
      
      return url;
    }
  
  /// <summary>
  /// Gets and Sets the RemoteCertificateValidationCallback
  /// </summary>
  public RemoteCertificateValidationCallback? RemoteCertificateValidationCallback { get; set; }
  
  #endregion Properties
  
  #region Methods
  
  /// <summary>
  /// Returns a string with essential information for debugging.
  /// </summary>
  public static string ToDebugReport()
  {
    string report = "C# SDK (FIRST) Debug Report:\n";
    report += "    OS: " + System.Environment.OSVersion + "\n";
    report += "    .NET Framework Version: " + System.Environment.Version  + "\n";
    report += "    Version of the API: 1.0.0\n";
    report += "    SDK Package Version: 1.0.0\n";
    
    return report;
  }
  
  /// <summary>
  /// Add Api Key Header.
  /// </summary>
  /// <param name="key">Api Key name.</param>
  /// <param name="value">Api Key value.</param>
  /// <returns></returns>
  public void AddApiKey(string key, string value) => ApiKey[key] = value;
  
  /// <summary>
  /// Sets the API key prefix.
  /// </summary>
  /// <param name="key">Api Key name.</param>
  /// <param name="value">Api Key value.</param>
  public void AddApiKeyPrefix(string key, string value) => ApiKeyPrefix[key] = value;
  
  #endregion Methods
  
  #region Static Members
  /// <summary>
  /// Merge configurations.
  /// </summary>
  /// <param name="first">First configuration.</param>
  /// <param name="second">Second configuration.</param>
  /// <return>Merged configuration.</return>
  public static IReadableConfiguration MergeConfigurations(IReadableConfiguration? first, IReadableConfiguration? second)
  {
    if (second is null)
    {
      return first ?? GlobalConfiguration.Instance;
    }
    else if (first is null)
    {
      return second;
    }
    
    Dictionary<string, string> apiKey = first.ApiKey.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
    Dictionary<string, string> apiKeyPrefix = first.ApiKeyPrefix.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
    Dictionary<string, string> defaultHeaders = first.DefaultHeaders.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
    
    foreach (var kvp in second.ApiKey)
    {
      apiKey[kvp.Key] = kvp.Value;
    }
    
    foreach (var kvp in second.ApiKeyPrefix)
    {
      apiKeyPrefix[kvp.Key] = kvp.Value;
    }
    
    foreach (var kvp in second.DefaultHeaders)
    {
      defaultHeaders[kvp.Key] = kvp.Value;
    }
    
    var config = new Configuration
    {
      ApiKey = apiKey,
      ApiKeyPrefix = apiKeyPrefix,
      DefaultHeaders = defaultHeaders,
      BasePath = string.IsNullOrWhiteSpace(second.BasePath) ? first.BasePath : second.BasePath,
      Timeout = second.Timeout,
      Proxy = second.Proxy ?? first.Proxy,
      UserAgent = string.IsNullOrWhiteSpace(second.UserAgent) ? first.UserAgent : second.UserAgent,
      Username = second.Username ?? first.Username,
      Password = second.Password ?? first.Password,
      AccessToken = second.AccessToken ?? first.AccessToken,
      TempFolderPath = string.IsNullOrWhiteSpace(second.TempFolderPath) ? first.TempFolderPath : second.TempFolderPath,
      DateTimeFormat = string.IsNullOrWhiteSpace(second.DateTimeFormat) ? first.DateTimeFormat : second.DateTimeFormat,
      ClientCertificates = second.ClientCertificates ?? first.ClientCertificates,
      UseDefaultCredentials = second.UseDefaultCredentials,
      RemoteCertificateValidationCallback = second.RemoteCertificateValidationCallback ?? first.RemoteCertificateValidationCallback,
    };
    
    return config;
  }
  #endregion Static Members
}