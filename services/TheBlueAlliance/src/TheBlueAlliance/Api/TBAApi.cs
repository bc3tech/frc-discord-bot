/*
 * The Blue Alliance API v3
 *
 * # Overview    Information and statistics about FIRST Robotics Competition teams and events.   # Authentication   All endpoints require an Auth Key to be passed in the header `X-TBA-Auth-Key`. If you do not have an auth key yet, you can obtain one from your [Account Page](/account).
 *
 * The version of the OpenAPI document: 3.9.7
 * Generated by: https://github.com/openapitools/openapi-generator.git
 */

namespace TheBlueAlliance.Api;

using System;
using System.Net.Http;
  using System.Collections.ObjectModel;

using TheBlueAlliance.Client;

using TheBlueAlliance.Model;
  /// <summary>
  /// Represents a collection of functions to interact with the API endpoints
  /// </summary>
  public interface ITBAApiSync : IApiAccessor
  {
    #region Synchronous Operations
      /// <summary>
      /// 
      /// </summary>
        /// <remarks>
        /// Returns API status, and TBA status information.
        /// </remarks>
      /// <exception cref="TheBlueAlliance.Client.ApiException">Thrown when fails to make API call</exception>
      /// <param name="ifNoneMatch">Value of the &#x60;ETag&#x60; header in the most recently cached response by the client. (optional)</param>
      /// <returns>APIStatus</returns>
      APIStatus? GetStatus(string? ifNoneMatch = default);
        
        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// Returns API status, and TBA status information.
        /// </remarks>
        /// <exception cref="TheBlueAlliance.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="ifNoneMatch">Value of the &#x60;ETag&#x60; header in the most recently cached response by the client. (optional)</param>
        /// <returns>ApiResponse of APIStatus</returns>
        ApiResponse<APIStatus?> GetStatusWithHttpInfo(string? ifNoneMatch = default);
      #endregion Synchronous Operations
    }
    
      /// <summary>
      /// Represents a collection of functions to interact with the API endpoints
      /// </summary>
      public interface ITBAApiAsync : IApiAccessor
      {
        #region Asynchronous Operations
          /// <summary>
          /// 
          /// </summary>
          /// <remarks>
          /// Returns API status, and TBA status information.
          /// </remarks>
          /// <exception cref="TheBlueAlliance.Client.ApiException">Thrown when fails to make API call</exception>
            /// <param name="ifNoneMatch">Value of the &#x60;ETag&#x60; header in the most recently cached response by the client. (optional)</param>
          /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
          /// <returns>Task of APIStatus</returns>
          System.Threading.Tasks.Task<APIStatus?> GetStatusAsync(string? ifNoneMatch = default, CancellationToken cancellationToken = default);
            
            /// <summary>
            /// 
            /// </summary>
            /// <remarks>
            /// Returns API status, and TBA status information.
            /// </remarks>
            /// <exception cref="TheBlueAlliance.Client.ApiException">Thrown when fails to make API call</exception>
              /// <param name="ifNoneMatch">Value of the &#x60;ETag&#x60; header in the most recently cached response by the client. (optional)</param>
            /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
            /// <returns>Task of ApiResponse (APIStatus)</returns>
            System.Threading.Tasks.Task<ApiResponse<APIStatus?>> GetStatusWithHttpInfoAsync(string? ifNoneMatch = default, CancellationToken cancellationToken = default);
          #endregion Asynchronous Operations
        }
      
      /// <summary>
      /// Represents a collection of functions to interact with the API endpoints
      /// </summary>
      public interface ITBAApi : ITBAApiSync, ITBAApiAsync { }
      
      /// <summary>
      /// Represents a collection of functions to interact with the API endpoints
      /// </summary>
      public sealed partial class TBAApi : ITBAApi
      {
        private ExceptionFactory? _exceptionFactory = (name, response) => null;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="TBAApi"/> class.
        /// **IMPORTANT** This will also create an instance of HttpClient, which is less than ideal.
        /// It's better to reuse the <see href="https://docs.microsoft.com/en-us/dotnet/architecture/microservices/implement-resilient-applications/use-httpclientfactory-to-implement-resilient-http-requests#issues-with-the-original-httpclient-class-available-in-net">HttpClient and HttpClientHandler</see>.
        /// </summary>
        /// <returns></returns>
        public TBAApi() : this(basePath: default) { }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="TBAApi"/> class.
        /// **IMPORTANT** This will also create an instance of HttpClient, which is less than ideal.
        /// It's better to reuse the <see href="https://docs.microsoft.com/en-us/dotnet/architecture/microservices/implement-resilient-applications/use-httpclientfactory-to-implement-resilient-http-requests#issues-with-the-original-httpclient-class-available-in-net">HttpClient and HttpClientHandler</see>.
        /// </summary>
        /// <param name="basePath">The target service's base path in URL format.</param>
        /// <exception cref="ArgumentException"></exception>
        /// <returns></returns>
        public TBAApi(string? basePath)
        {
          this.Configuration = TheBlueAlliance.Client.Configuration.MergeConfigurations(
          GlobalConfiguration.Instance,
          new Configuration { BasePath = basePath }
          );
          this.ApiClient = new ApiClient(this.Configuration.BasePath);
          this.Client =  this.ApiClient;
            this.AsynchronousClient = this.ApiClient;
          this.ExceptionFactory = TheBlueAlliance.Client.Configuration.DefaultExceptionFactory;
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="TBAApi"/> class using Configuration object.
        /// **IMPORTANT** This will also create an instance of HttpClient, which is less than ideal.
        /// It's better to reuse the <see href="https://docs.microsoft.com/en-us/dotnet/architecture/microservices/implement-resilient-applications/use-httpclientfactory-to-implement-resilient-http-requests#issues-with-the-original-httpclient-class-available-in-net">HttpClient and HttpClientHandler</see>.
        /// </summary>
        /// <param name="configuration">An instance of Configuration.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public TBAApi(TheBlueAlliance.Client.Configuration configuration)
        {
          ArgumentNullException.ThrowIfNull(configuration);
          
          this.Configuration = TheBlueAlliance.Client.Configuration.MergeConfigurations(
          GlobalConfiguration.Instance,
          configuration
          );
          this.ApiClient = new ApiClient(this.Configuration.BasePath);
          this.Client = this.ApiClient;
            this.AsynchronousClient = this.ApiClient;
          ExceptionFactory = TheBlueAlliance.Client.Configuration.DefaultExceptionFactory;
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="TBAApi"/> class.
        /// </summary>
        /// <param name="client">An instance of HttpClient.</param>
        /// <param name="handler">An optional instance of HttpClientHandler that is used by HttpClient.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        /// <remarks>
        /// Some configuration settings will not be applied without passing an HttpClientHandler.
        /// The features affected are: Setting and Retrieving Cookies, Client Certificates, Proxy settings.
        /// </remarks>
        public TBAApi(HttpClient client, HttpClientHandler? handler = null) : this(client, basePath: default, handler: handler) { }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="TBAApi"/> class.
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
        public TBAApi(HttpClient client, string? basePath, HttpClientHandler? handler = null)
        {
          ArgumentNullException.ThrowIfNull(client);
          
          this.Configuration = TheBlueAlliance.Client.Configuration.MergeConfigurations(
          GlobalConfiguration.Instance,
          new Configuration { BasePath = basePath }
          );
          this.ApiClient = new ApiClient(client, this.Configuration.BasePath, handler);
          this.Client =  this.ApiClient;
            this.AsynchronousClient = this.ApiClient;
          this.ExceptionFactory = TheBlueAlliance.Client.Configuration.DefaultExceptionFactory;
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="TBAApi"/> class using Configuration object.
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
        public TBAApi(HttpClient client, Configuration configuration, HttpClientHandler? handler = null)
        {
          ArgumentNullException.ThrowIfNull(configuration);
          ArgumentNullException.ThrowIfNull(client);
          
          this.Configuration = TheBlueAlliance.Client.Configuration.MergeConfigurations(
          GlobalConfiguration.Instance,
          configuration
          );
          this.ApiClient = new ApiClient(client, this.Configuration.BasePath, handler);
          this.Client = this.ApiClient;
            this.AsynchronousClient = this.ApiClient;
          ExceptionFactory = TheBlueAlliance.Client.Configuration.DefaultExceptionFactory;
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="TBAApi"/> class
        /// using a Configuration object and client instance.
        /// </summary>
        /// <param name="client">The client interface for synchronous API access.</param>
        /// <param name="asyncClient">The client interface for asynchronous API access.</param>
        /// <param name="configuration">The configuration object.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public TBAApi(ISynchronousClient client, IAsynchronousClient asyncClient, IReadableConfiguration configuration)
        {
          ArgumentNullException.ThrowIfNull(client);
          
            ArgumentNullException.ThrowIfNull(asyncClient);
            
          ArgumentNullException.ThrowIfNull(configuration);
          
          this.Client = client;
            this.AsynchronousClient = asyncClient;
          this.Configuration = configuration;
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
          ///  Returns API status, and TBA status information.
          /// </summary>
          /// <exception cref="TheBlueAlliance.Client.ApiException">Thrown when fails to make API call</exception>
          /// <param name="ifNoneMatch">Value of the &#x60;ETag&#x60; header in the most recently cached response by the client. (optional)</param>
          /// <returns>APIStatus</returns>
          public APIStatus? GetStatus(string? ifNoneMatch = default)
          {
            TheBlueAlliance.Client.ApiResponse<APIStatus?> localVarResponse = GetStatusWithHttpInfo(ifNoneMatch);
              return localVarResponse.Data;
            }
            
            /// <summary>
            ///  Returns API status, and TBA status information.
            /// </summary>
            /// <exception cref="TheBlueAlliance.Client.ApiException">Thrown when fails to make API call</exception>
            /// <param name="ifNoneMatch">Value of the &#x60;ETag&#x60; header in the most recently cached response by the client. (optional)</param>
            /// <returns>ApiResponse of APIStatus</returns>
            public ApiResponse<APIStatus?> GetStatusWithHttpInfo(string? ifNoneMatch = default)
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
                    if (!string.IsNullOrEmpty(this.Configuration.GetApiKeyWithPrefix("X-TBA-Auth-Key")))
                    {
                      localVarRequestOptions.HeaderParameters.Add("X-TBA-Auth-Key", this.Configuration.GetApiKeyWithPrefix("X-TBA-Auth-Key"));
                    }
                    
              
              // make the HTTP request
              var localVarResponse = this.Client.Get<APIStatus?>("/status", localVarRequestOptions, this.Configuration);
              
              if (this.ExceptionFactory is not null)
              {
                var _exception = this.ExceptionFactory("GetStatus", localVarResponse);
                if (_exception is not null)
                {
                  throw _exception;
                }
              }
              
              return localVarResponse;
            }
            
            /// <summary>
            ///  Returns API status, and TBA status information.
            /// </summary>
            /// <exception cref="TheBlueAlliance.Client.ApiException">Thrown when fails to make API call</exception>
              /// <param name="ifNoneMatch">Value of the &#x60;ETag&#x60; header in the most recently cached response by the client. (optional)</param>
            /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
            /// <returns>Task of APIStatus</returns>
            public async System.Threading.Tasks.Task<APIStatus?> GetStatusAsync(string? ifNoneMatch = default, CancellationToken cancellationToken = default)
            {
              TheBlueAlliance.Client.ApiResponse<APIStatus?> localVarResponse = await GetStatusWithHttpInfoAsync(ifNoneMatch, cancellationToken).ConfigureAwait(false);
                return localVarResponse.Data;
              }
              
              /// <summary>
              ///  Returns API status, and TBA status information.
              /// </summary>
              /// <exception cref="TheBlueAlliance.Client.ApiException">Thrown when fails to make API call</exception>
                /// <param name="ifNoneMatch">Value of the &#x60;ETag&#x60; header in the most recently cached response by the client. (optional)</param>
              /// <param name="cancellationToken">Cancellation Token to cancel the request.</param>
              /// <returns>Task of ApiResponse (APIStatus)</returns>
              public async System.Threading.Tasks.Task<TheBlueAlliance.Client.ApiResponse<APIStatus?>> GetStatusWithHttpInfoAsync(string? ifNoneMatch = default, CancellationToken cancellationToken = default)
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
                      if (!string.IsNullOrEmpty(this.Configuration.GetApiKeyWithPrefix("X-TBA-Auth-Key")))
                      {
                        localVarRequestOptions.HeaderParameters.Add("X-TBA-Auth-Key", this.Configuration.GetApiKeyWithPrefix("X-TBA-Auth-Key"));
                      }
                      
                // make the HTTP request
                var localVarResponse = await this.AsynchronousClient.GetAsync<APIStatus?>("/status", localVarRequestOptions, this.Configuration, cancellationToken).ConfigureAwait(false);
                
                if (this.ExceptionFactory is not null)
                {
                  var _exception = this.ExceptionFactory("GetStatus", localVarResponse);
                  if (_exception is not null)
                  {
                    throw _exception;
                  }
                }
                
                return localVarResponse;
              }
            }
