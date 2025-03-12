/*
 * The Blue Alliance API v3
 *
 * # Overview    Information and statistics about FIRST Robotics Competition teams and events.   # Authentication   All endpoints require an Auth Key to be passed in the header `X-TBA-Auth-Key`. If you do not have an auth key yet, you can obtain one from your [Account Page](/account).
 *
 * The version of the OpenAPI document: 3.9.11
 * Generated by: https://github.com/openapitools/openapi-generator.git
 */

namespace TheBlueAlliance.Api;

using System;
using System.Net.Http;
using System.Threading.Tasks;

using TheBlueAlliance.Client;

using TheBlueAlliance.Model;
/// <summary>
/// Represents a collection of functions to interact with the API endpoints
/// </summary>
public interface IDefaultApiSync : IApiAccessor
{
    #region Synchronous Operations
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// Gets a large blob of data that is used on the frontend for searching. May change without notice.
    /// </remarks>
    /// <exception cref="ApiException">Thrown when fails to make API call</exception>
    /// <param name="ifNoneMatch">Value of the &#x60;ETag&#x60; header in the most recently cached response by the client. (optional)</param>
    /// <returns>SearchIndex</returns>
    SearchIndex? GetSearchIndex(string? ifNoneMatch = default);

    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// Gets a large blob of data that is used on the frontend for searching. May change without notice.
    /// </remarks>
    /// <exception cref="ApiException">Thrown when fails to make API call</exception>
    /// <param name="ifNoneMatch">Value of the &#x60;ETag&#x60; header in the most recently cached response by the client. (optional)</param>
    /// <returns>ApiResponse of SearchIndex</returns>
    ApiResponse<SearchIndex?> GetSearchIndexWithHttpInfo(string? ifNoneMatch = default);
    #endregion Synchronous Operations
}

/// <summary>
/// Represents a collection of functions to interact with the API endpoints
/// </summary>
public interface IDefaultApiAsync : IApiAccessor
{
    #region Asynchronous Operations
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// Gets a large blob of data that is used on the frontend for searching. May change without notice.
    /// </remarks>
    /// <exception cref="ApiException">Thrown when fails to make API call</exception>
    /// <param name="ifNoneMatch">Value of the &#x60;ETag&#x60; header in the most recently cached response by the client. (optional)</param>
    /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
    /// <returns>Task of SearchIndex</returns>
    Task<SearchIndex?> GetSearchIndexAsync(string? ifNoneMatch = default, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// Gets a large blob of data that is used on the frontend for searching. May change without notice.
    /// </remarks>
    /// <exception cref="ApiException">Thrown when fails to make API call</exception>
    /// <param name="ifNoneMatch">Value of the &#x60;ETag&#x60; header in the most recently cached response by the client. (optional)</param>
    /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
    /// <returns>Task of ApiResponse (SearchIndex)</returns>
    Task<ApiResponse<SearchIndex?>> GetSearchIndexWithHttpInfoAsync(string? ifNoneMatch = default, CancellationToken cancellationToken = default);
    #endregion Asynchronous Operations
}

/// <summary>
/// Represents a collection of functions to interact with the API endpoints
/// </summary>
public interface IDefaultApi : IDefaultApiSync, IDefaultApiAsync { }

/// <summary>
/// Represents a collection of functions to interact with the API endpoints
/// </summary>
public sealed partial class DefaultApi : IDefaultApi
{
    private ExceptionFactory? _exceptionFactory = (name, response) => null;

    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultApi"/> class.
    /// **IMPORTANT** This will also create an instance of HttpClient, which is less than ideal.
    /// It's better to reuse the <see href="https://docs.microsoft.com/en-us/dotnet/architecture/microservices/implement-resilient-applications/use-httpclientfactory-to-implement-resilient-http-requests#issues-with-the-original-httpclient-class-available-in-net">HttpClient and HttpClientHandler</see>.
    /// </summary>
    /// <returns></returns>
    public DefaultApi() : this(basePath: default) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultApi"/> class.
    /// **IMPORTANT** This will also create an instance of HttpClient, which is less than ideal.
    /// It's better to reuse the <see href="https://docs.microsoft.com/en-us/dotnet/architecture/microservices/implement-resilient-applications/use-httpclientfactory-to-implement-resilient-http-requests#issues-with-the-original-httpclient-class-available-in-net">HttpClient and HttpClientHandler</see>.
    /// </summary>
    /// <param name="basePath">The target service's base path in URL format.</param>
    /// <exception cref="ArgumentException"></exception>
    /// <returns></returns>
    public DefaultApi(string? basePath)
    {
        this.Configuration = TheBlueAlliance.Client.Configuration.MergeConfigurations(GlobalConfiguration.Instance, new Configuration { BasePath = basePath });
        this.ApiClient = new ApiClient(this.Configuration.BasePath);
        this.Client = this.ApiClient;
        this.AsynchronousClient = this.ApiClient;

        this.ExceptionFactory = TheBlueAlliance.Client.Configuration.DefaultExceptionFactory;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultApi"/> class using Configuration object.
    /// **IMPORTANT** This will also create an instance of HttpClient, which is less than ideal.
    /// It's better to reuse the <see href="https://docs.microsoft.com/en-us/dotnet/architecture/microservices/implement-resilient-applications/use-httpclientfactory-to-implement-resilient-http-requests#issues-with-the-original-httpclient-class-available-in-net">HttpClient and HttpClientHandler</see>.
    /// </summary>
    /// <param name="configuration">An instance of Configuration.</param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <returns></returns>
    public DefaultApi(Configuration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        this.Configuration = TheBlueAlliance.Client.Configuration.MergeConfigurations(GlobalConfiguration.Instance, configuration);
        this.ApiClient = new ApiClient(this.Configuration.BasePath);
        this.Client = this.ApiClient;
        this.AsynchronousClient = this.ApiClient;

        this.ExceptionFactory = TheBlueAlliance.Client.Configuration.DefaultExceptionFactory;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultApi"/> class.
    /// </summary>
    /// <param name="client">An instance of HttpClient.</param>
    /// <param name="handler">An optional instance of HttpClientHandler that is used by HttpClient.</param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <returns></returns>
    /// <remarks>
    /// Some configuration settings will not be applied without passing an HttpClientHandler.
    /// The features affected are: Setting and Retrieving Cookies, Client Certificates, Proxy settings.
    /// </remarks>
    public DefaultApi(HttpClient client, HttpClientHandler? handler = null) : this(client, basePath: default, handler: handler) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultApi"/> class.
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
    public DefaultApi(HttpClient client, string? basePath, HttpClientHandler? handler = null)
    {
        ArgumentNullException.ThrowIfNull(client);

        this.Configuration = TheBlueAlliance.Client.Configuration.MergeConfigurations(GlobalConfiguration.Instance, new Configuration { BasePath = basePath });
        this.ApiClient = new ApiClient(client, this.Configuration.BasePath, handler);
        this.Client = this.ApiClient;
        this.AsynchronousClient = this.ApiClient;

        this.ExceptionFactory = TheBlueAlliance.Client.Configuration.DefaultExceptionFactory;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultApi"/> class using Configuration object.
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
    public DefaultApi(HttpClient client, Configuration configuration, HttpClientHandler? handler = null)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentNullException.ThrowIfNull(client);

        this.Configuration = TheBlueAlliance.Client.Configuration.MergeConfigurations(GlobalConfiguration.Instance, configuration);
        this.ApiClient = new ApiClient(client, this.Configuration.BasePath, handler);
        this.Client = this.ApiClient;
        this.AsynchronousClient = this.ApiClient;

        this.ExceptionFactory = TheBlueAlliance.Client.Configuration.DefaultExceptionFactory;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultApi"/> class
    /// using a Configuration object and client instance.
    /// </summary>
    /// <param name="client">The client interface for synchronous API access.</param>
    /// <param name="asyncClient">The client interface for asynchronous API access.</param>
    /// <param name="configuration">The configuration object.</param>
    /// <exception cref="ArgumentNullException"></exception>
    public DefaultApi(ISynchronousClient client, IAsynchronousClient asyncClient, IReadableConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(client);

        ArgumentNullException.ThrowIfNull(asyncClient);
        this.AsynchronousClient = asyncClient;

        ArgumentNullException.ThrowIfNull(configuration);
        this.Configuration = configuration;

        this.Client = client;
        this.ExceptionFactory = TheBlueAlliance.Client.Configuration.DefaultExceptionFactory;
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
    ///  Gets a large blob of data that is used on the frontend for searching. May change without notice.
    /// </summary>
    /// <exception cref="ApiException">Thrown when fails to make API call</exception>
    /// <param name="ifNoneMatch">Value of the &#x60;ETag&#x60; header in the most recently cached response by the client. (optional)</param>
    /// <returns>SearchIndex</returns>
    public SearchIndex? GetSearchIndex(string? ifNoneMatch = default)
    {
        ApiResponse<SearchIndex?> localVarResponse = GetSearchIndexWithHttpInfo(ifNoneMatch);
        return localVarResponse.Data;
    }

    /// <summary>
    ///  Gets a large blob of data that is used on the frontend for searching. May change without notice.
    /// </summary>
    /// <exception cref="ApiException">Thrown when fails to make API call</exception>
    /// <param name="ifNoneMatch">Value of the &#x60;ETag&#x60; header in the most recently cached response by the client. (optional)</param>
    /// <returns>ApiResponse of SearchIndex</returns>
    public ApiResponse<SearchIndex?> GetSearchIndexWithHttpInfo(string? ifNoneMatch = default)
    {
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

        if (ifNoneMatch is not null)
        {
            localVarRequestOptions.HeaderParameters.Add("If-None-Match", ClientUtils.ParameterToString(ifNoneMatch)); // header parameter
        }

        // authentication (apiKey) required
        var apiKeyIfExists = this.Configuration.GetApiKeyWithPrefix("X-TBA-Auth-Key");
        if (!string.IsNullOrEmpty(apiKeyIfExists))
        {
            localVarRequestOptions.HeaderParameters.Add("X-TBA-Auth-Key", apiKeyIfExists);
        }

        // make the HTTP request
        var localVarResponse = this.Client.Get<SearchIndex?>("/search_index", localVarRequestOptions, this.Configuration);

        if (this.ExceptionFactory is not null)
        {
            var _exception = this.ExceptionFactory("GetSearchIndex", localVarResponse);
            if (_exception is not null)
            {
                throw _exception;
            }
        }

        return localVarResponse;
    }

    /// <summary>
    ///  Gets a large blob of data that is used on the frontend for searching. May change without notice.
    /// </summary>
    /// <exception cref="ApiException">Thrown when fails to make API call</exception>
    /// <param name="ifNoneMatch">Value of the &#x60;ETag&#x60; header in the most recently cached response by the client. (optional)</param>
    /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
    /// <returns>Task of SearchIndex</returns>
    public async Task<SearchIndex?> GetSearchIndexAsync(string? ifNoneMatch = default, CancellationToken cancellationToken = default)
    {
        ApiResponse<SearchIndex?> localVarResponse = await GetSearchIndexWithHttpInfoAsync(ifNoneMatch, cancellationToken).ConfigureAwait(false);
        return localVarResponse.Data;
    }

    /// <summary>
    ///  Gets a large blob of data that is used on the frontend for searching. May change without notice.
    /// </summary>
    /// <exception cref="ApiException">Thrown when fails to make API call</exception>
    /// <param name="ifNoneMatch">Value of the &#x60;ETag&#x60; header in the most recently cached response by the client. (optional)</param>
    /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
    /// <returns>Task of ApiResponse (SearchIndex)</returns>
    public async Task<ApiResponse<SearchIndex?>> GetSearchIndexWithHttpInfoAsync(string? ifNoneMatch = default, CancellationToken cancellationToken = default)
    {
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

        if (ifNoneMatch is not null)
        {
            localVarRequestOptions.HeaderParameters.Add("If-None-Match", ClientUtils.ParameterToString(ifNoneMatch)); // header parameter
        }

        // authentication (apiKey) required
        var apiKeyIfExists = this.Configuration.GetApiKeyWithPrefix("X-TBA-Auth-Key");
        if (!string.IsNullOrEmpty(apiKeyIfExists))
        {
            localVarRequestOptions.HeaderParameters.Add("X-TBA-Auth-Key", apiKeyIfExists);
        }

        // make the HTTP request
        var localVarResponse = await this.AsynchronousClient.GetAsync<SearchIndex?>("/search_index", localVarRequestOptions, this.Configuration, cancellationToken).ConfigureAwait(false);

        if (this.ExceptionFactory is not null)
        {
            var _exception = this.ExceptionFactory("GetSearchIndex", localVarResponse);
            if (_exception is not null)
            {
                throw _exception;
            }
        }

        return localVarResponse;
    }
}
