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
namespace Common.Statbotics.Api.V2.Team.Item
{
    /// <summary>
    /// Builds and executes requests for operations under \v2\team\{team}
    /// </summary>
    [global::System.CodeDom.Compiler.GeneratedCode("Kiota", "1.0.0")]
    public partial class WithTeamItemRequestBuilder : BaseRequestBuilder
    {
        /// <summary>
        /// Instantiates a new <see cref="global::Common.Statbotics.Api.V2.Team.Item.WithTeamItemRequestBuilder"/> and sets the default values.
        /// </summary>
        /// <param name="pathParameters">Path parameters for the request</param>
        /// <param name="requestAdapter">The request adapter to use to execute the requests.</param>
        public WithTeamItemRequestBuilder(Dictionary<string, object> pathParameters, IRequestAdapter requestAdapter) : base(requestAdapter, "{+baseurl}/v2/team/{team}", pathParameters)
        {
        }
        /// <summary>
        /// Instantiates a new <see cref="global::Common.Statbotics.Api.V2.Team.Item.WithTeamItemRequestBuilder"/> and sets the default values.
        /// </summary>
        /// <param name="rawUrl">The raw URL to use for the request builder.</param>
        /// <param name="requestAdapter">The request adapter to use to execute the requests.</param>
        public WithTeamItemRequestBuilder(string rawUrl, IRequestAdapter requestAdapter) : base(requestAdapter, "{+baseurl}/v2/team/{team}", rawUrl)
        {
        }
        /// <summary>
        /// Get a single Team object containing team name, location, normalized EPA statistics, and winrate.
        /// </summary>
        /// <returns>A <see cref="global::Common.Statbotics.Api.V2.Team.Item.WithTeamGetResponse"/></returns>
        /// <param name="cancellationToken">Cancellation token to use when cancelling requests</param>
        /// <param name="requestConfiguration">Configuration for the request such as headers, query parameters, and middleware options.</param>
        /// <exception cref="global::Common.Statbotics.Api.Models.HTTPValidationError">When receiving a 422 status code</exception>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public async Task<global::Common.Statbotics.Api.V2.Team.Item.WithTeamGetResponse?> GetAsWithTeamGetResponseAsync(Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = default, CancellationToken cancellationToken = default)
        {
#nullable restore
#else
        public async Task<global::Common.Statbotics.Api.V2.Team.Item.WithTeamGetResponse> GetAsWithTeamGetResponseAsync(Action<RequestConfiguration<DefaultQueryParameters>> requestConfiguration = default, CancellationToken cancellationToken = default)
        {
#endif
            var requestInfo = ToGetRequestInformation(requestConfiguration);
            var errorMapping = new Dictionary<string, ParsableFactory<IParsable>>
            {
                { "422", global::Common.Statbotics.Api.Models.HTTPValidationError.CreateFromDiscriminatorValue },
            };
            return await RequestAdapter.SendAsync<global::Common.Statbotics.Api.V2.Team.Item.WithTeamGetResponse>(requestInfo, global::Common.Statbotics.Api.V2.Team.Item.WithTeamGetResponse.CreateFromDiscriminatorValue, errorMapping, cancellationToken).ConfigureAwait(false);
        }
        /// <summary>
        /// Get a single Team object containing team name, location, normalized EPA statistics, and winrate.
        /// </summary>
        /// <returns>A <see cref="global::Common.Statbotics.Api.V2.Team.Item.WithTeamResponse"/></returns>
        /// <param name="cancellationToken">Cancellation token to use when cancelling requests</param>
        /// <param name="requestConfiguration">Configuration for the request such as headers, query parameters, and middleware options.</param>
        /// <exception cref="global::Common.Statbotics.Api.Models.HTTPValidationError">When receiving a 422 status code</exception>
        [Obsolete("This method is obsolete. Use GetAsWithTeamGetResponseAsync instead.")]
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public async Task<global::Common.Statbotics.Api.V2.Team.Item.WithTeamResponse?> GetAsync(Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = default, CancellationToken cancellationToken = default)
        {
#nullable restore
#else
        public async Task<global::Common.Statbotics.Api.V2.Team.Item.WithTeamResponse> GetAsync(Action<RequestConfiguration<DefaultQueryParameters>> requestConfiguration = default, CancellationToken cancellationToken = default)
        {
#endif
            var requestInfo = ToGetRequestInformation(requestConfiguration);
            var errorMapping = new Dictionary<string, ParsableFactory<IParsable>>
            {
                { "422", global::Common.Statbotics.Api.Models.HTTPValidationError.CreateFromDiscriminatorValue },
            };
            return await RequestAdapter.SendAsync<global::Common.Statbotics.Api.V2.Team.Item.WithTeamResponse>(requestInfo, global::Common.Statbotics.Api.V2.Team.Item.WithTeamResponse.CreateFromDiscriminatorValue, errorMapping, cancellationToken).ConfigureAwait(false);
        }
        /// <summary>
        /// Get a single Team object containing team name, location, normalized EPA statistics, and winrate.
        /// </summary>
        /// <returns>A <see cref="RequestInformation"/></returns>
        /// <param name="requestConfiguration">Configuration for the request such as headers, query parameters, and middleware options.</param>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public RequestInformation ToGetRequestInformation(Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = default)
        {
#nullable restore
#else
        public RequestInformation ToGetRequestInformation(Action<RequestConfiguration<DefaultQueryParameters>> requestConfiguration = default)
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
        /// <returns>A <see cref="global::Common.Statbotics.Api.V2.Team.Item.WithTeamItemRequestBuilder"/></returns>
        /// <param name="rawUrl">The raw URL to use for the request builder.</param>
        public global::Common.Statbotics.Api.V2.Team.Item.WithTeamItemRequestBuilder WithUrl(string rawUrl)
        {
            return new global::Common.Statbotics.Api.V2.Team.Item.WithTeamItemRequestBuilder(rawUrl, RequestAdapter);
        }
        /// <summary>
        /// Configuration for the request such as headers, query parameters, and middleware options.
        /// </summary>
        [Obsolete("This class is deprecated. Please use the generic RequestConfiguration class generated by the generator.")]
        [global::System.CodeDom.Compiler.GeneratedCode("Kiota", "1.0.0")]
        public partial class WithTeamItemRequestBuilderGetRequestConfiguration : RequestConfiguration<DefaultQueryParameters>
        {
        }
    }
}
#pragma warning restore CS0618
