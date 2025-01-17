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
namespace Common.Statbotics.Api.V3.Team_matches
{
    /// <summary>
    /// Builds and executes requests for operations under \v3\team_matches
    /// </summary>
    [global::System.CodeDom.Compiler.GeneratedCode("Kiota", "1.0.0")]
    public partial class Team_matchesRequestBuilder : BaseRequestBuilder
    {
        /// <summary>
        /// Instantiates a new <see cref="global::Common.Statbotics.Api.V3.Team_matches.Team_matchesRequestBuilder"/> and sets the default values.
        /// </summary>
        /// <param name="pathParameters">Path parameters for the request</param>
        /// <param name="requestAdapter">The request adapter to use to execute the requests.</param>
        public Team_matchesRequestBuilder(Dictionary<string, object> pathParameters, IRequestAdapter requestAdapter) : base(requestAdapter, "{+baseurl}/v3/team_matches{?ascending*,elim*,event*,limit*,match*,metric*,offseason*,offset*,team*,week*,year*}", pathParameters)
        {
        }
        /// <summary>
        /// Instantiates a new <see cref="global::Common.Statbotics.Api.V3.Team_matches.Team_matchesRequestBuilder"/> and sets the default values.
        /// </summary>
        /// <param name="rawUrl">The raw URL to use for the request builder.</param>
        /// <param name="requestAdapter">The request adapter to use to execute the requests.</param>
        public Team_matchesRequestBuilder(string rawUrl, IRequestAdapter requestAdapter) : base(requestAdapter, "{+baseurl}/v3/team_matches{?ascending*,elim*,event*,limit*,match*,metric*,offseason*,offset*,team*,week*,year*}", rawUrl)
        {
        }
        /// <summary>
        /// Returns up to 1000 team matches at a time. Specify limit and offset to page through results.
        /// </summary>
        /// <returns>A List&lt;global::Common.Statbotics.Api.V3.Team_matches.Team_matches&gt;</returns>
        /// <param name="cancellationToken">Cancellation token to use when cancelling requests</param>
        /// <param name="requestConfiguration">Configuration for the request such as headers, query parameters, and middleware options.</param>
        /// <exception cref="global::Common.Statbotics.Api.Models.HTTPValidationError">When receiving a 422 status code</exception>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public async Task<List<global::Common.Statbotics.Api.V3.Team_matches.Team_matches>?> GetAsync(Action<RequestConfiguration<global::Common.Statbotics.Api.V3.Team_matches.Team_matchesRequestBuilder.Team_matchesRequestBuilderGetQueryParameters>>? requestConfiguration = default, CancellationToken cancellationToken = default)
        {
#nullable restore
#else
        public async Task<List<global::Common.Statbotics.Api.V3.Team_matches.Team_matches>> GetAsync(Action<RequestConfiguration<global::Common.Statbotics.Api.V3.Team_matches.Team_matchesRequestBuilder.Team_matchesRequestBuilderGetQueryParameters>> requestConfiguration = default, CancellationToken cancellationToken = default)
        {
#endif
            var requestInfo = ToGetRequestInformation(requestConfiguration);
            var errorMapping = new Dictionary<string, ParsableFactory<IParsable>>
            {
                { "422", global::Common.Statbotics.Api.Models.HTTPValidationError.CreateFromDiscriminatorValue },
            };
            var collectionResult = await RequestAdapter.SendCollectionAsync<global::Common.Statbotics.Api.V3.Team_matches.Team_matches>(requestInfo, global::Common.Statbotics.Api.V3.Team_matches.Team_matches.CreateFromDiscriminatorValue, errorMapping, cancellationToken).ConfigureAwait(false);
            return collectionResult?.AsList();
        }
        /// <summary>
        /// Returns up to 1000 team matches at a time. Specify limit and offset to page through results.
        /// </summary>
        /// <returns>A <see cref="RequestInformation"/></returns>
        /// <param name="requestConfiguration">Configuration for the request such as headers, query parameters, and middleware options.</param>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public RequestInformation ToGetRequestInformation(Action<RequestConfiguration<global::Common.Statbotics.Api.V3.Team_matches.Team_matchesRequestBuilder.Team_matchesRequestBuilderGetQueryParameters>>? requestConfiguration = default)
        {
#nullable restore
#else
        public RequestInformation ToGetRequestInformation(Action<RequestConfiguration<global::Common.Statbotics.Api.V3.Team_matches.Team_matchesRequestBuilder.Team_matchesRequestBuilderGetQueryParameters>> requestConfiguration = default)
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
        /// <returns>A <see cref="global::Common.Statbotics.Api.V3.Team_matches.Team_matchesRequestBuilder"/></returns>
        /// <param name="rawUrl">The raw URL to use for the request builder.</param>
        public global::Common.Statbotics.Api.V3.Team_matches.Team_matchesRequestBuilder WithUrl(string rawUrl)
        {
            return new global::Common.Statbotics.Api.V3.Team_matches.Team_matchesRequestBuilder(rawUrl, RequestAdapter);
        }
        /// <summary>
        /// Returns up to 1000 team matches at a time. Specify limit and offset to page through results.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCode("Kiota", "1.0.0")]
        public partial class Team_matchesRequestBuilderGetQueryParameters 
        {
            /// <summary>Whether to sort the returned values in ascending order. Default is ascending</summary>
            [QueryParameter("ascending")]
            public bool? Ascending { get; set; }
            /// <summary>Whether the match is an elimination match.</summary>
            [QueryParameter("elim")]
            public bool? Elim { get; set; }
            /// <summary>Event key, e.g. `2019ncwak`.</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
            [QueryParameter("event")]
            public string? Event { get; set; }
#nullable restore
#else
            [QueryParameter("event")]
            public string Event { get; set; }
#endif
            /// <summary>Maximum number of events to return. Default is 1000.</summary>
            [QueryParameter("limit")]
            public int? Limit { get; set; }
            /// <summary>Match key, e.g. `2019ncwak_f1m1`.</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
            [QueryParameter("match")]
            public string? Match { get; set; }
#nullable restore
#else
            [QueryParameter("match")]
            public string Match { get; set; }
#endif
            /// <summary>How to sort the returned values. Any column in the table is valid.</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
            [QueryParameter("metric")]
            public string? Metric { get; set; }
#nullable restore
#else
            [QueryParameter("metric")]
            public string Metric { get; set; }
#endif
            /// <summary>Whether the event is an offseason event.</summary>
            [QueryParameter("offseason")]
            public bool? Offseason { get; set; }
            /// <summary>Offset from the first result to return.</summary>
            [QueryParameter("offset")]
            public int? Offset { get; set; }
            /// <summary>Team number (no prefix), e.g. `5511`.</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
            [QueryParameter("team")]
            public string? Team { get; set; }
#nullable restore
#else
            [QueryParameter("team")]
            public string Team { get; set; }
#endif
            /// <summary>Week of the competition season. 0 is preseason, 8 is CMP, 9 is offseason.</summary>
            [QueryParameter("week")]
            public int? Week { get; set; }
            /// <summary>Four-digit year</summary>
            [QueryParameter("year")]
            public int? Year { get; set; }
        }
        /// <summary>
        /// Configuration for the request such as headers, query parameters, and middleware options.
        /// </summary>
        [Obsolete("This class is deprecated. Please use the generic RequestConfiguration class generated by the generator.")]
        [global::System.CodeDom.Compiler.GeneratedCode("Kiota", "1.0.0")]
        public partial class Team_matchesRequestBuilderGetRequestConfiguration : RequestConfiguration<global::Common.Statbotics.Api.V3.Team_matches.Team_matchesRequestBuilder.Team_matchesRequestBuilderGetQueryParameters>
        {
        }
    }
}
#pragma warning restore CS0618
