/*
 * Statbotics REST API
 *
 * The REST API for Statbotics. Please be nice to our servers! If you are looking to do large-scale data science projects, use the CSV exports on the GitHub repo.
 *
 * The version of the OpenAPI document: 3.0.0
 * Generated by: https://github.com/openapitools/openapi-generator.git
 */

namespace Statbotics.Api;

using Statbotics.Client;
using Statbotics.Model;

using System;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Threading.Tasks;
/// <summary>
/// Represents a collection of functions to interact with the API endpoints
/// </summary>
public interface IMatchApiSync : IApiAccessor
{
    #region Synchronous Operations
    /// <summary>
    /// Query a single match
    /// </summary>
    /// <remarks>
    /// Returns a single Match object. Requires a match key, e.g. &#x60;2019ncwak_f1m1&#x60;.
    /// </remarks>
    /// <exception cref="ApiException">Thrown when fails to make API call</exception>
    /// <param name="match"></param>
    /// <returns>Object</returns>
    Match? ReadMatchV3MatchMatchGet(string match);

    /// <summary>
    /// Query a single match
    /// </summary>
    /// <remarks>
    /// Returns a single Match object. Requires a match key, e.g. &#x60;2019ncwak_f1m1&#x60;.
    /// </remarks>
    /// <exception cref="ApiException">Thrown when fails to make API call</exception>
    /// <param name="match"></param>
    /// <returns>ApiResponse of Object</returns>
    ApiResponse<Match?> ReadMatchV3MatchMatchGetWithHttpInfo(string match);
    /// <summary>
    /// Query multiple matches
    /// </summary>
    /// <remarks>
    /// Returns up to 1000 matches at a time. Specify limit and offset to page through results.
    /// </remarks>
    /// <exception cref="ApiException">Thrown when fails to make API call</exception>
    /// <param name="ascending">Whether to sort the returned values in ascending order. Default is ascending (optional)</param>
    /// <param name="elim">Whether the match is an elimination match. (optional)</param>
    /// <param name="limit">Maximum number of events to return. Default is 1000. (optional)</param>
    /// <param name="metric">How to sort the returned values. Any column in the table is valid. (optional)</param>
    /// <param name="offseason">Whether the event is an offseason event. (optional)</param>
    /// <param name="offset">Offset from the first result to return. (optional)</param>
    /// <param name="team">Team number (no prefix), e.g. &#x60;5511&#x60;. (optional)</param>
    /// <param name="varEvent">Event key, e.g. &#x60;2019ncwak&#x60;. (optional)</param>
    /// <param name="week">Week of the competition season. 0 is preseason, 8 is CMP, 9 is offseason. (optional)</param>
    /// <param name="year">Four-digit year (optional)</param>
    /// <returns>Collection&lt;Object&gt;</returns>
    Collection<Object>? ReadMatchesV3MatchesGet(bool? ascending = default, bool? elim = default, int? limit = default, string? metric = default, bool? offseason = default, int? offset = default, string? team = default, string? varEvent = default, int? week = default, int? year = default);

    /// <summary>
    /// Query multiple matches
    /// </summary>
    /// <remarks>
    /// Returns up to 1000 matches at a time. Specify limit and offset to page through results.
    /// </remarks>
    /// <exception cref="ApiException">Thrown when fails to make API call</exception>
    /// <param name="ascending">Whether to sort the returned values in ascending order. Default is ascending (optional)</param>
    /// <param name="elim">Whether the match is an elimination match. (optional)</param>
    /// <param name="limit">Maximum number of events to return. Default is 1000. (optional)</param>
    /// <param name="metric">How to sort the returned values. Any column in the table is valid. (optional)</param>
    /// <param name="offseason">Whether the event is an offseason event. (optional)</param>
    /// <param name="offset">Offset from the first result to return. (optional)</param>
    /// <param name="team">Team number (no prefix), e.g. &#x60;5511&#x60;. (optional)</param>
    /// <param name="varEvent">Event key, e.g. &#x60;2019ncwak&#x60;. (optional)</param>
    /// <param name="week">Week of the competition season. 0 is preseason, 8 is CMP, 9 is offseason. (optional)</param>
    /// <param name="year">Four-digit year (optional)</param>
    /// <returns>ApiResponse of Collection&lt;Object&gt;</returns>
    ApiResponse<Collection<Object>?> ReadMatchesV3MatchesGetWithHttpInfo(bool? ascending = default, bool? elim = default, int? limit = default, string? metric = default, bool? offseason = default, int? offset = default, string? team = default, string? varEvent = default, int? week = default, int? year = default);
    #endregion Synchronous Operations
}

/// <summary>
/// Represents a collection of functions to interact with the API endpoints
/// </summary>
public interface IMatchApiAsync : IApiAccessor
{
    #region Asynchronous Operations
    /// <summary>
    /// Query a single match
    /// </summary>
    /// <remarks>
    /// Returns a single Match object. Requires a match key, e.g. &#x60;2019ncwak_f1m1&#x60;.
    /// </remarks>
    /// <exception cref="ApiException">Thrown when fails to make API call</exception>
    /// <param name="match"></param>
    /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
    /// <returns>Task of Object</returns>
    Task<Match?> ReadMatchV3MatchMatchGetAsync(string match, CancellationToken cancellationToken = default);

    /// <summary>
    /// Query a single match
    /// </summary>
    /// <remarks>
    /// Returns a single Match object. Requires a match key, e.g. &#x60;2019ncwak_f1m1&#x60;.
    /// </remarks>
    /// <exception cref="ApiException">Thrown when fails to make API call</exception>
    /// <param name="match"></param>
    /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
    /// <returns>Task of ApiResponse (Object)</returns>
    Task<ApiResponse<Match?>> ReadMatchV3MatchMatchGetWithHttpInfoAsync(string match, CancellationToken cancellationToken = default);
    /// <summary>
    /// Query multiple matches
    /// </summary>
    /// <remarks>
    /// Returns up to 1000 matches at a time. Specify limit and offset to page through results.
    /// </remarks>
    /// <exception cref="ApiException">Thrown when fails to make API call</exception>
    /// <param name="ascending">Whether to sort the returned values in ascending order. Default is ascending (optional)</param>
    /// <param name="elim">Whether the match is an elimination match. (optional)</param>
    /// <param name="limit">Maximum number of events to return. Default is 1000. (optional)</param>
    /// <param name="metric">How to sort the returned values. Any column in the table is valid. (optional)</param>
    /// <param name="offseason">Whether the event is an offseason event. (optional)</param>
    /// <param name="offset">Offset from the first result to return. (optional)</param>
    /// <param name="team">Team number (no prefix), e.g. &#x60;5511&#x60;. (optional)</param>
    /// <param name="varEvent">Event key, e.g. &#x60;2019ncwak&#x60;. (optional)</param>
    /// <param name="week">Week of the competition season. 0 is preseason, 8 is CMP, 9 is offseason. (optional)</param>
    /// <param name="year">Four-digit year (optional)</param>
    /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
    /// <returns>Task of Collection&lt;Object&gt;</returns>
    Task<Collection<Object>?> ReadMatchesV3MatchesGetAsync(bool? ascending = default, bool? elim = default, int? limit = default, string? metric = default, bool? offseason = default, int? offset = default, string? team = default, string? varEvent = default, int? week = default, int? year = default, CancellationToken cancellationToken = default);

    /// <summary>
    /// Query multiple matches
    /// </summary>
    /// <remarks>
    /// Returns up to 1000 matches at a time. Specify limit and offset to page through results.
    /// </remarks>
    /// <exception cref="ApiException">Thrown when fails to make API call</exception>
    /// <param name="ascending">Whether to sort the returned values in ascending order. Default is ascending (optional)</param>
    /// <param name="elim">Whether the match is an elimination match. (optional)</param>
    /// <param name="limit">Maximum number of events to return. Default is 1000. (optional)</param>
    /// <param name="metric">How to sort the returned values. Any column in the table is valid. (optional)</param>
    /// <param name="offseason">Whether the event is an offseason event. (optional)</param>
    /// <param name="offset">Offset from the first result to return. (optional)</param>
    /// <param name="team">Team number (no prefix), e.g. &#x60;5511&#x60;. (optional)</param>
    /// <param name="varEvent">Event key, e.g. &#x60;2019ncwak&#x60;. (optional)</param>
    /// <param name="week">Week of the competition season. 0 is preseason, 8 is CMP, 9 is offseason. (optional)</param>
    /// <param name="year">Four-digit year (optional)</param>
    /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
    /// <returns>Task of ApiResponse (Collection&lt;Object&gt;)</returns>
    Task<ApiResponse<Collection<Object>?>> ReadMatchesV3MatchesGetWithHttpInfoAsync(bool? ascending = default, bool? elim = default, int? limit = default, string? metric = default, bool? offseason = default, int? offset = default, string? team = default, string? varEvent = default, int? week = default, int? year = default, CancellationToken cancellationToken = default);
    #endregion Asynchronous Operations
}

/// <summary>
/// Represents a collection of functions to interact with the API endpoints
/// </summary>
public interface IMatchApi : IMatchApiSync, IMatchApiAsync { }

/// <summary>
/// Represents a collection of functions to interact with the API endpoints
/// </summary>
public sealed partial class MatchApi : IMatchApi
{
    private ExceptionFactory? _exceptionFactory = (name, response) => null;

    /// <summary>
    /// Initializes a new instance of the <see cref="MatchApi"/> class.
    /// **IMPORTANT** This will also create an instance of HttpClient, which is less than ideal.
    /// It's better to reuse the <see href="https://docs.microsoft.com/en-us/dotnet/architecture/microservices/implement-resilient-applications/use-httpclientfactory-to-implement-resilient-http-requests#issues-with-the-original-httpclient-class-available-in-net">HttpClient and HttpClientHandler</see>.
    /// </summary>
    /// <returns></returns>
    public MatchApi() : this(basePath: default) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="MatchApi"/> class.
    /// **IMPORTANT** This will also create an instance of HttpClient, which is less than ideal.
    /// It's better to reuse the <see href="https://docs.microsoft.com/en-us/dotnet/architecture/microservices/implement-resilient-applications/use-httpclientfactory-to-implement-resilient-http-requests#issues-with-the-original-httpclient-class-available-in-net">HttpClient and HttpClientHandler</see>.
    /// </summary>
    /// <param name="basePath">The target service's base path in URL format.</param>
    /// <exception cref="ArgumentException"></exception>
    /// <returns></returns>
    public MatchApi(string? basePath)
    {
        this.Configuration = Statbotics.Client.Configuration.MergeConfigurations(GlobalConfiguration.Instance, new Configuration { BasePath = basePath });
        this.ApiClient = new ApiClient(this.Configuration.BasePath);
        this.Client = this.ApiClient;
        this.AsynchronousClient = this.ApiClient;

        this.ExceptionFactory = Statbotics.Client.Configuration.DefaultExceptionFactory;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MatchApi"/> class using Configuration object.
    /// **IMPORTANT** This will also create an instance of HttpClient, which is less than ideal.
    /// It's better to reuse the <see href="https://docs.microsoft.com/en-us/dotnet/architecture/microservices/implement-resilient-applications/use-httpclientfactory-to-implement-resilient-http-requests#issues-with-the-original-httpclient-class-available-in-net">HttpClient and HttpClientHandler</see>.
    /// </summary>
    /// <param name="configuration">An instance of Configuration.</param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <returns></returns>
    public MatchApi(Configuration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        this.Configuration = Statbotics.Client.Configuration.MergeConfigurations(GlobalConfiguration.Instance, configuration);
        this.ApiClient = new ApiClient(this.Configuration.BasePath);
        this.Client = this.ApiClient;
        this.AsynchronousClient = this.ApiClient;

        this.ExceptionFactory = Statbotics.Client.Configuration.DefaultExceptionFactory;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MatchApi"/> class.
    /// </summary>
    /// <param name="client">An instance of HttpClient.</param>
    /// <param name="handler">An optional instance of HttpClientHandler that is used by HttpClient.</param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <returns></returns>
    /// <remarks>
    /// Some configuration settings will not be applied without passing an HttpClientHandler.
    /// The features affected are: Setting and Retrieving Cookies, Client Certificates, Proxy settings.
    /// </remarks>
    public MatchApi(HttpClient client, HttpClientHandler? handler = null) : this(client, basePath: default, handler: handler) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="MatchApi"/> class.
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
    public MatchApi(HttpClient client, string? basePath, HttpClientHandler? handler = null)
    {
        ArgumentNullException.ThrowIfNull(client);

        this.Configuration = Statbotics.Client.Configuration.MergeConfigurations(GlobalConfiguration.Instance, new Configuration { BasePath = basePath });
        this.ApiClient = new ApiClient(client, this.Configuration.BasePath, handler);
        this.Client = this.ApiClient;
        this.AsynchronousClient = this.ApiClient;

        this.ExceptionFactory = Statbotics.Client.Configuration.DefaultExceptionFactory;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MatchApi"/> class using Configuration object.
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
    public MatchApi(HttpClient client, Configuration configuration, HttpClientHandler? handler = null)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentNullException.ThrowIfNull(client);

        this.Configuration = Statbotics.Client.Configuration.MergeConfigurations(GlobalConfiguration.Instance, configuration);
        this.ApiClient = new ApiClient(client, this.Configuration.BasePath, handler);
        this.Client = this.ApiClient;
        this.AsynchronousClient = this.ApiClient;

        this.ExceptionFactory = Statbotics.Client.Configuration.DefaultExceptionFactory;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MatchApi"/> class
    /// using a Configuration object and client instance.
    /// </summary>
    /// <param name="client">The client interface for synchronous API access.</param>
    /// <param name="asyncClient">The client interface for asynchronous API access.</param>
    /// <param name="configuration">The configuration object.</param>
    /// <exception cref="ArgumentNullException"></exception>
    public MatchApi(ISynchronousClient client, IAsynchronousClient asyncClient, IReadableConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(client);

        ArgumentNullException.ThrowIfNull(asyncClient);
        this.AsynchronousClient = asyncClient;

        ArgumentNullException.ThrowIfNull(configuration);
        this.Configuration = configuration;

        this.Client = client;
        this.ExceptionFactory = Statbotics.Client.Configuration.DefaultExceptionFactory;
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
    /// Query a single match Returns a single Match object. Requires a match key, e.g. &#x60;2019ncwak_f1m1&#x60;.
    /// </summary>
    /// <exception cref="ApiException">Thrown when fails to make API call</exception>
    /// <param name="match"></param>
    /// <returns>Object</returns>
    public Match? ReadMatchV3MatchMatchGet(string match)
    {
        ApiResponse<Match?> localVarResponse = ReadMatchV3MatchMatchGetWithHttpInfo(match);
        return localVarResponse.Data;
    }

    /// <summary>
    /// Query a single match Returns a single Match object. Requires a match key, e.g. &#x60;2019ncwak_f1m1&#x60;.
    /// </summary>
    /// <exception cref="ApiException">Thrown when fails to make API call</exception>
    /// <param name="match"></param>
    /// <returns>ApiResponse of Object</returns>
    public ApiResponse<Match?> ReadMatchV3MatchMatchGetWithHttpInfo(string match)
    {
        // verify the required parameter 'match' is set
        if (match is null)
        {
            throw new ApiException(400, "Missing required parameter 'match' when calling MatchApi->ReadMatchV3MatchMatchGet");
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

        localVarRequestOptions.PathParameters.Add("match", ClientUtils.ParameterToString(match)); // path parameter

        // make the HTTP request
        var localVarResponse = this.Client.Get<Match?>("/v3/match/{match}", localVarRequestOptions, this.Configuration);

        if (this.ExceptionFactory is not null)
        {
            var _exception = this.ExceptionFactory("ReadMatchV3MatchMatchGet", localVarResponse);
            if (_exception is not null)
            {
                throw _exception;
            }
        }

        return localVarResponse;
    }

    /// <summary>
    /// Query a single match Returns a single Match object. Requires a match key, e.g. &#x60;2019ncwak_f1m1&#x60;.
    /// </summary>
    /// <exception cref="ApiException">Thrown when fails to make API call</exception>
    /// <param name="match"></param>
    /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
    /// <returns>Task of Object</returns>
    public async Task<Match?> ReadMatchV3MatchMatchGetAsync(string match, CancellationToken cancellationToken = default)
    {
        ApiResponse<Match?> localVarResponse = await ReadMatchV3MatchMatchGetWithHttpInfoAsync(match, cancellationToken).ConfigureAwait(false);
        return localVarResponse.Data;
    }

    /// <summary>
    /// Query a single match Returns a single Match object. Requires a match key, e.g. &#x60;2019ncwak_f1m1&#x60;.
    /// </summary>
    /// <exception cref="ApiException">Thrown when fails to make API call</exception>
    /// <param name="match"></param>
    /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
    /// <returns>Task of ApiResponse (Object)</returns>
    public async Task<ApiResponse<Match?>> ReadMatchV3MatchMatchGetWithHttpInfoAsync(string match, CancellationToken cancellationToken = default)
    {
        // verify the required parameter 'match' is set
        if (match is null)
        {
            throw new ApiException(400, "Missing required parameter 'match' when calling MatchApi->ReadMatchV3MatchMatchGet");
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

        localVarRequestOptions.PathParameters.Add("match", ClientUtils.ParameterToString(match)); // path parameter
                                                                                                  // make the HTTP request
        var localVarResponse = await this.AsynchronousClient.GetAsync<Match?>("/v3/match/{match}", localVarRequestOptions, this.Configuration, cancellationToken).ConfigureAwait(false);

        if (this.ExceptionFactory is not null)
        {
            var _exception = this.ExceptionFactory("ReadMatchV3MatchMatchGet", localVarResponse);
            if (_exception is not null)
            {
                if (_exception is ApiException)
                {
                    return new ApiResponse<Match?>(System.Net.HttpStatusCode.NoContent, null);
                }

                throw _exception;
            }
        }

        return localVarResponse;
    }          /// <summary>
               /// Query multiple matches Returns up to 1000 matches at a time. Specify limit and offset to page through results.
               /// </summary>
               /// <exception cref="ApiException">Thrown when fails to make API call</exception>
               /// <param name="ascending">Whether to sort the returned values in ascending order. Default is ascending (optional)</param>
               /// <param name="elim">Whether the match is an elimination match. (optional)</param>
               /// <param name="limit">Maximum number of events to return. Default is 1000. (optional)</param>
               /// <param name="metric">How to sort the returned values. Any column in the table is valid. (optional)</param>
               /// <param name="offseason">Whether the event is an offseason event. (optional)</param>
               /// <param name="offset">Offset from the first result to return. (optional)</param>
               /// <param name="team">Team number (no prefix), e.g. &#x60;5511&#x60;. (optional)</param>
               /// <param name="varEvent">Event key, e.g. &#x60;2019ncwak&#x60;. (optional)</param>
               /// <param name="week">Week of the competition season. 0 is preseason, 8 is CMP, 9 is offseason. (optional)</param>
               /// <param name="year">Four-digit year (optional)</param>
               /// <returns>Collection&lt;Object&gt;</returns>
    public Collection<Object>? ReadMatchesV3MatchesGet(bool? ascending = default, bool? elim = default, int? limit = default, string? metric = default, bool? offseason = default, int? offset = default, string? team = default, string? varEvent = default, int? week = default, int? year = default)
    {
        ApiResponse<Collection<object>?> localVarResponse = ReadMatchesV3MatchesGetWithHttpInfo(ascending, elim, limit, metric, offseason, offset, team, varEvent, week, year);
        return localVarResponse.Data;
    }

    /// <summary>
    /// Query multiple matches Returns up to 1000 matches at a time. Specify limit and offset to page through results.
    /// </summary>
    /// <exception cref="ApiException">Thrown when fails to make API call</exception>
    /// <param name="ascending">Whether to sort the returned values in ascending order. Default is ascending (optional)</param>
    /// <param name="elim">Whether the match is an elimination match. (optional)</param>
    /// <param name="limit">Maximum number of events to return. Default is 1000. (optional)</param>
    /// <param name="metric">How to sort the returned values. Any column in the table is valid. (optional)</param>
    /// <param name="offseason">Whether the event is an offseason event. (optional)</param>
    /// <param name="offset">Offset from the first result to return. (optional)</param>
    /// <param name="team">Team number (no prefix), e.g. &#x60;5511&#x60;. (optional)</param>
    /// <param name="varEvent">Event key, e.g. &#x60;2019ncwak&#x60;. (optional)</param>
    /// <param name="week">Week of the competition season. 0 is preseason, 8 is CMP, 9 is offseason. (optional)</param>
    /// <param name="year">Four-digit year (optional)</param>
    /// <returns>ApiResponse of Collection&lt;Object&gt;</returns>
    public ApiResponse<Collection<Object>?> ReadMatchesV3MatchesGetWithHttpInfo(bool? ascending = default, bool? elim = default, int? limit = default, string? metric = default, bool? offseason = default, int? offset = default, string? team = default, string? varEvent = default, int? week = default, int? year = default)
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

        if (ascending is not null)
        {
            localVarRequestOptions.QueryParameters.Add(ClientUtils.ParameterToMultiMap("", "ascending", ascending));
        }

        if (elim is not null)
        {
            localVarRequestOptions.QueryParameters.Add(ClientUtils.ParameterToMultiMap("", "elim", elim));
        }

        if (limit is not null)
        {
            localVarRequestOptions.QueryParameters.Add(ClientUtils.ParameterToMultiMap("", "limit", limit));
        }

        if (metric is not null)
        {
            localVarRequestOptions.QueryParameters.Add(ClientUtils.ParameterToMultiMap("", "metric", metric));
        }

        if (offseason is not null)
        {
            localVarRequestOptions.QueryParameters.Add(ClientUtils.ParameterToMultiMap("", "offseason", offseason));
        }

        if (offset is not null)
        {
            localVarRequestOptions.QueryParameters.Add(ClientUtils.ParameterToMultiMap("", "offset", offset));
        }

        if (team is not null)
        {
            localVarRequestOptions.QueryParameters.Add(ClientUtils.ParameterToMultiMap("", "team", team));
        }

        if (varEvent is not null)
        {
            localVarRequestOptions.QueryParameters.Add(ClientUtils.ParameterToMultiMap("", "event", varEvent));
        }

        if (week is not null)
        {
            localVarRequestOptions.QueryParameters.Add(ClientUtils.ParameterToMultiMap("", "week", week));
        }

        if (year is not null)
        {
            localVarRequestOptions.QueryParameters.Add(ClientUtils.ParameterToMultiMap("", "year", year));
        }

        // make the HTTP request
        var localVarResponse = this.Client.Get<Collection<Object>?>("/v3/matches", localVarRequestOptions, this.Configuration);

        if (this.ExceptionFactory is not null)
        {
            var _exception = this.ExceptionFactory("ReadMatchesV3MatchesGet", localVarResponse);
            if (_exception is not null)
            {
                throw _exception;
            }
        }

        return localVarResponse;
    }

    /// <summary>
    /// Query multiple matches Returns up to 1000 matches at a time. Specify limit and offset to page through results.
    /// </summary>
    /// <exception cref="ApiException">Thrown when fails to make API call</exception>
    /// <param name="ascending">Whether to sort the returned values in ascending order. Default is ascending (optional)</param>
    /// <param name="elim">Whether the match is an elimination match. (optional)</param>
    /// <param name="limit">Maximum number of events to return. Default is 1000. (optional)</param>
    /// <param name="metric">How to sort the returned values. Any column in the table is valid. (optional)</param>
    /// <param name="offseason">Whether the event is an offseason event. (optional)</param>
    /// <param name="offset">Offset from the first result to return. (optional)</param>
    /// <param name="team">Team number (no prefix), e.g. &#x60;5511&#x60;. (optional)</param>
    /// <param name="varEvent">Event key, e.g. &#x60;2019ncwak&#x60;. (optional)</param>
    /// <param name="week">Week of the competition season. 0 is preseason, 8 is CMP, 9 is offseason. (optional)</param>
    /// <param name="year">Four-digit year (optional)</param>
    /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
    /// <returns>Task of Collection&lt;Object&gt;</returns>
    public async Task<Collection<Object>?> ReadMatchesV3MatchesGetAsync(bool? ascending = default, bool? elim = default, int? limit = default, string? metric = default, bool? offseason = default, int? offset = default, string? team = default, string? varEvent = default, int? week = default, int? year = default, CancellationToken cancellationToken = default)
    {
        ApiResponse<Collection<object>?> localVarResponse = await ReadMatchesV3MatchesGetWithHttpInfoAsync(ascending, elim, limit, metric, offseason, offset, team, varEvent, week, year, cancellationToken).ConfigureAwait(false);
        return localVarResponse.Data;
    }

    /// <summary>
    /// Query multiple matches Returns up to 1000 matches at a time. Specify limit and offset to page through results.
    /// </summary>
    /// <exception cref="ApiException">Thrown when fails to make API call</exception>
    /// <param name="ascending">Whether to sort the returned values in ascending order. Default is ascending (optional)</param>
    /// <param name="elim">Whether the match is an elimination match. (optional)</param>
    /// <param name="limit">Maximum number of events to return. Default is 1000. (optional)</param>
    /// <param name="metric">How to sort the returned values. Any column in the table is valid. (optional)</param>
    /// <param name="offseason">Whether the event is an offseason event. (optional)</param>
    /// <param name="offset">Offset from the first result to return. (optional)</param>
    /// <param name="team">Team number (no prefix), e.g. &#x60;5511&#x60;. (optional)</param>
    /// <param name="varEvent">Event key, e.g. &#x60;2019ncwak&#x60;. (optional)</param>
    /// <param name="week">Week of the competition season. 0 is preseason, 8 is CMP, 9 is offseason. (optional)</param>
    /// <param name="year">Four-digit year (optional)</param>
    /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
    /// <returns>Task of ApiResponse (Collection&lt;Object&gt;)</returns>
    public async Task<ApiResponse<Collection<object>?>> ReadMatchesV3MatchesGetWithHttpInfoAsync(bool? ascending = default, bool? elim = default, int? limit = default, string? metric = default, bool? offseason = default, int? offset = default, string? team = default, string? varEvent = default, int? week = default, int? year = default, CancellationToken cancellationToken = default)
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

        if (ascending is not null)
        {
            localVarRequestOptions.QueryParameters.Add(ClientUtils.ParameterToMultiMap("", "ascending", ascending));
        }

        if (elim is not null)
        {
            localVarRequestOptions.QueryParameters.Add(ClientUtils.ParameterToMultiMap("", "elim", elim));
        }

        if (limit is not null)
        {
            localVarRequestOptions.QueryParameters.Add(ClientUtils.ParameterToMultiMap("", "limit", limit));
        }

        if (metric is not null)
        {
            localVarRequestOptions.QueryParameters.Add(ClientUtils.ParameterToMultiMap("", "metric", metric));
        }

        if (offseason is not null)
        {
            localVarRequestOptions.QueryParameters.Add(ClientUtils.ParameterToMultiMap("", "offseason", offseason));
        }

        if (offset is not null)
        {
            localVarRequestOptions.QueryParameters.Add(ClientUtils.ParameterToMultiMap("", "offset", offset));
        }

        if (team is not null)
        {
            localVarRequestOptions.QueryParameters.Add(ClientUtils.ParameterToMultiMap("", "team", team));
        }

        if (varEvent is not null)
        {
            localVarRequestOptions.QueryParameters.Add(ClientUtils.ParameterToMultiMap("", "event", varEvent));
        }

        if (week is not null)
        {
            localVarRequestOptions.QueryParameters.Add(ClientUtils.ParameterToMultiMap("", "week", week));
        }

        if (year is not null)
        {
            localVarRequestOptions.QueryParameters.Add(ClientUtils.ParameterToMultiMap("", "year", year));
        }

        // make the HTTP request
        var localVarResponse = await this.AsynchronousClient.GetAsync<Collection<Object>?>("/v3/matches", localVarRequestOptions, this.Configuration, cancellationToken).ConfigureAwait(false);

        if (this.ExceptionFactory is not null)
        {
            var _exception = this.ExceptionFactory("ReadMatchesV3MatchesGet", localVarResponse);
            if (_exception is not null)
            {
                throw _exception;
            }
        }

        return localVarResponse;
    }
}
