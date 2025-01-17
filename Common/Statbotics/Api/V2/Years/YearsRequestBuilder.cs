// <auto-generated/>
#pragma warning disable CS0618
using Common.Statbotics.Api.Models;
using Microsoft.Kiota.Abstractions.Extensions;
using Microsoft.Kiota.Abstractions.Serialization;
using Microsoft.Kiota.Abstractions;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Threading;
using System;
namespace Common.Statbotics.Api.V2.Years
{
    /// <summary>
    /// Builds and executes requests for operations under \v2\years
    /// </summary>
    [global::System.CodeDom.Compiler.GeneratedCode("Kiota", "1.0.0")]
    public partial class YearsRequestBuilder : BaseRequestBuilder
    {
        /// <summary>
        /// Instantiates a new <see cref="global::Common.Statbotics.Api.V2.Years.YearsRequestBuilder"/> and sets the default values.
        /// </summary>
        /// <param name="pathParameters">Path parameters for the request</param>
        /// <param name="requestAdapter">The request adapter to use to execute the requests.</param>
        public YearsRequestBuilder(Dictionary<string, object> pathParameters, IRequestAdapter requestAdapter) : base(requestAdapter, "{+baseurl}/v2/years{?ascending*,limit*,metric*,offset*}", pathParameters)
        {
        }
        /// <summary>
        /// Instantiates a new <see cref="global::Common.Statbotics.Api.V2.Years.YearsRequestBuilder"/> and sets the default values.
        /// </summary>
        /// <param name="rawUrl">The raw URL to use for the request builder.</param>
        /// <param name="requestAdapter">The request adapter to use to execute the requests.</param>
        public YearsRequestBuilder(string rawUrl, IRequestAdapter requestAdapter) : base(requestAdapter, "{+baseurl}/v2/years{?ascending*,limit*,metric*,offset*}", rawUrl)
        {
        }
        /// <summary>
        /// Get a list of Year objects from 2002 to 2023. Specify a four-digit year, ex: 2019
        /// </summary>
        /// <returns>A List&lt;global::Common.Statbotics.Api.V2.Years.Years&gt;</returns>
        /// <param name="cancellationToken">Cancellation token to use when cancelling requests</param>
        /// <param name="requestConfiguration">Configuration for the request such as headers, query parameters, and middleware options.</param>
        /// <exception cref="global::Common.Statbotics.Api.Models.HTTPValidationError">When receiving a 422 status code</exception>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public async Task<List<global::Common.Statbotics.Api.V2.Years.Years>?> GetAsync(Action<RequestConfiguration<global::Common.Statbotics.Api.V2.Years.YearsRequestBuilder.YearsRequestBuilderGetQueryParameters>>? requestConfiguration = default, CancellationToken cancellationToken = default)
        {
#nullable restore
#else
        public async Task<List<global::Common.Statbotics.Api.V2.Years.Years>> GetAsync(Action<RequestConfiguration<global::Common.Statbotics.Api.V2.Years.YearsRequestBuilder.YearsRequestBuilderGetQueryParameters>> requestConfiguration = default, CancellationToken cancellationToken = default)
        {
#endif
            var requestInfo = ToGetRequestInformation(requestConfiguration);
            var errorMapping = new Dictionary<string, ParsableFactory<IParsable>>
            {
                { "422", global::Common.Statbotics.Api.Models.HTTPValidationError.CreateFromDiscriminatorValue },
            };
            var collectionResult = await RequestAdapter.SendCollectionAsync<global::Common.Statbotics.Api.V2.Years.Years>(requestInfo, global::Common.Statbotics.Api.V2.Years.Years.CreateFromDiscriminatorValue, errorMapping, cancellationToken).ConfigureAwait(false);
            return collectionResult?.AsList();
        }
        /// <summary>
        /// Get a list of Year objects from 2002 to 2023. Specify a four-digit year, ex: 2019
        /// </summary>
        /// <returns>A <see cref="RequestInformation"/></returns>
        /// <param name="requestConfiguration">Configuration for the request such as headers, query parameters, and middleware options.</param>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public RequestInformation ToGetRequestInformation(Action<RequestConfiguration<global::Common.Statbotics.Api.V2.Years.YearsRequestBuilder.YearsRequestBuilderGetQueryParameters>>? requestConfiguration = default)
        {
#nullable restore
#else
        public RequestInformation ToGetRequestInformation(Action<RequestConfiguration<global::Common.Statbotics.Api.V2.Years.YearsRequestBuilder.YearsRequestBuilderGetQueryParameters>> requestConfiguration = default)
        {
#endif
            var requestInfo = new RequestInformation(Method.GET, UrlTemplate, PathParameters);
            requestInfo.Configure(requestConfiguration);
            requestInfo.Headers.TryAdd("Accept", "application/json");
            return requestInfo;
        }
        /// <summary>
        /// Returns a request builder with the provided arbitrary URL. Using this method means any other path or query parameters are ignored.
        /// </summary>
        /// <returns>A <see cref="global::Common.Statbotics.Api.V2.Years.YearsRequestBuilder"/></returns>
        /// <param name="rawUrl">The raw URL to use for the request builder.</param>
        public global::Common.Statbotics.Api.V2.Years.YearsRequestBuilder WithUrl(string rawUrl)
        {
            return new global::Common.Statbotics.Api.V2.Years.YearsRequestBuilder(rawUrl, RequestAdapter);
        }
        /// <summary>
        /// Get a list of Year objects from 2002 to 2023. Specify a four-digit year, ex: 2019
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCode("Kiota", "1.0.0")]
        public partial class YearsRequestBuilderGetQueryParameters 
        {
            [QueryParameter("ascending")]
            public bool? Ascending { get; set; }
            [QueryParameter("limit")]
            public int? Limit { get; set; }
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
            [QueryParameter("metric")]
            public string? Metric { get; set; }
#nullable restore
#else
            [QueryParameter("metric")]
            public string Metric { get; set; }
#endif
            [QueryParameter("offset")]
            public int? Offset { get; set; }
        }
        /// <summary>
        /// Configuration for the request such as headers, query parameters, and middleware options.
        /// </summary>
        [Obsolete("This class is deprecated. Please use the generic RequestConfiguration class generated by the generator.")]
        [global::System.CodeDom.Compiler.GeneratedCode("Kiota", "1.0.0")]
        public partial class YearsRequestBuilderGetRequestConfiguration : RequestConfiguration<global::Common.Statbotics.Api.V2.Years.YearsRequestBuilder.YearsRequestBuilderGetQueryParameters>
        {
        }
    }
}
#pragma warning restore CS0618
