// <auto-generated/>
#pragma warning disable CS0618
using Microsoft.Kiota.Abstractions.Extensions;
using Microsoft.Kiota.Abstractions.Serialization;
using Microsoft.Kiota.Abstractions;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Threading;
using System;
namespace Common.First.Api.Item.Matches.Item
{
    /// <summary>
    /// Builds and executes requests for operations under \{season}\matches\{eventCode}
    /// </summary>
    [global::System.CodeDom.Compiler.GeneratedCode("Kiota", "1.0.0")]
    public partial class WithEventCodeItemRequestBuilder : BaseRequestBuilder
    {
        /// <summary>
        /// Instantiates a new <see cref="global::Common.First.Api.Item.Matches.Item.WithEventCodeItemRequestBuilder"/> and sets the default values.
        /// </summary>
        /// <param name="pathParameters">Path parameters for the request</param>
        /// <param name="requestAdapter">The request adapter to use to execute the requests.</param>
        public WithEventCodeItemRequestBuilder(Dictionary<string, object> pathParameters, IRequestAdapter requestAdapter) : base(requestAdapter, "{+baseurl}/{season}/matches/{eventCode}{?end*,matchNumber*,start*,teamNumber*,tournamentLevel*}", pathParameters)
        {
        }
        /// <summary>
        /// Instantiates a new <see cref="global::Common.First.Api.Item.Matches.Item.WithEventCodeItemRequestBuilder"/> and sets the default values.
        /// </summary>
        /// <param name="rawUrl">The raw URL to use for the request builder.</param>
        /// <param name="requestAdapter">The request adapter to use to execute the requests.</param>
        public WithEventCodeItemRequestBuilder(string rawUrl, IRequestAdapter requestAdapter) : base(requestAdapter, "{+baseurl}/{season}/matches/{eventCode}{?end*,matchNumber*,start*,teamNumber*,tournamentLevel*}", rawUrl)
        {
        }
        /// <summary>
        /// The match results API returns the match results for all matches of a particular event in a particular season. Match results are only available once a match has been played, retrieving info about future matches requires the event schedule API. You cannot receive data about a match that is in progress.If you specify the `matchNumber`, `start` and/or `end` optional parameters, you must also specify a `tournamentLevel`. If you specify the `teamNumber` parameter, you cannot specify a `matchNumber` parameter. If you specify the matchNumber, you cannot define a `start` or `end`.**Note**: If you specify `start`, and it is higher than the maximum match number at the event, you will not receive any match results in the response. The same is true in reverse for the end parameter.Starting in the 2015 season, Elimination matches were renamed to Playoff matches. As such, you must request Playoff matches from the API, and &quot;elim&quot; will not return any results. In Playoffs, match numbers 1-8 are &quot;Quarterfinal&quot; matches, 9-14 are &quot;Semifinal&quot; and 15-17 are &quot;Finals&quot; matches. The &quot;level&quot; response however, will always just show &quot;Playoff&quot; regardless of the portion of the Playoff tournament.
        /// </summary>
        /// <returns>A <see cref="global::Common.First.Api.Item.Matches.Item.WithEventCodeGetResponse"/></returns>
        /// <param name="cancellationToken">Cancellation token to use when cancelling requests</param>
        /// <param name="requestConfiguration">Configuration for the request such as headers, query parameters, and middleware options.</param>
        /// <exception cref="global::Common.First.Api.Item.Matches.Item.WithEventCode500Error">When receiving a 500 status code</exception>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public async Task<global::Common.First.Api.Item.Matches.Item.WithEventCodeGetResponse?> GetAsWithEventCodeGetResponseAsync(Action<RequestConfiguration<global::Common.First.Api.Item.Matches.Item.WithEventCodeItemRequestBuilder.WithEventCodeItemRequestBuilderGetQueryParameters>>? requestConfiguration = default, CancellationToken cancellationToken = default)
        {
#nullable restore
#else
        public async Task<global::Common.First.Api.Item.Matches.Item.WithEventCodeGetResponse> GetAsWithEventCodeGetResponseAsync(Action<RequestConfiguration<global::Common.First.Api.Item.Matches.Item.WithEventCodeItemRequestBuilder.WithEventCodeItemRequestBuilderGetQueryParameters>> requestConfiguration = default, CancellationToken cancellationToken = default)
        {
#endif
            var requestInfo = ToGetRequestInformation(requestConfiguration);
            var errorMapping = new Dictionary<string, ParsableFactory<IParsable>>
            {
                { "500", global::Common.First.Api.Item.Matches.Item.WithEventCode500Error.CreateFromDiscriminatorValue },
            };
            return await RequestAdapter.SendAsync<global::Common.First.Api.Item.Matches.Item.WithEventCodeGetResponse>(requestInfo, global::Common.First.Api.Item.Matches.Item.WithEventCodeGetResponse.CreateFromDiscriminatorValue, errorMapping, cancellationToken).ConfigureAwait(false);
        }
        /// <summary>
        /// The match results API returns the match results for all matches of a particular event in a particular season. Match results are only available once a match has been played, retrieving info about future matches requires the event schedule API. You cannot receive data about a match that is in progress.If you specify the `matchNumber`, `start` and/or `end` optional parameters, you must also specify a `tournamentLevel`. If you specify the `teamNumber` parameter, you cannot specify a `matchNumber` parameter. If you specify the matchNumber, you cannot define a `start` or `end`.**Note**: If you specify `start`, and it is higher than the maximum match number at the event, you will not receive any match results in the response. The same is true in reverse for the end parameter.Starting in the 2015 season, Elimination matches were renamed to Playoff matches. As such, you must request Playoff matches from the API, and &quot;elim&quot; will not return any results. In Playoffs, match numbers 1-8 are &quot;Quarterfinal&quot; matches, 9-14 are &quot;Semifinal&quot; and 15-17 are &quot;Finals&quot; matches. The &quot;level&quot; response however, will always just show &quot;Playoff&quot; regardless of the portion of the Playoff tournament.
        /// </summary>
        /// <returns>A <see cref="global::Common.First.Api.Item.Matches.Item.WithEventCodeResponse"/></returns>
        /// <param name="cancellationToken">Cancellation token to use when cancelling requests</param>
        /// <param name="requestConfiguration">Configuration for the request such as headers, query parameters, and middleware options.</param>
        /// <exception cref="global::Common.First.Api.Item.Matches.Item.WithEventCode500Error">When receiving a 500 status code</exception>
        [Obsolete("This method is obsolete. Use GetAsWithEventCodeGetResponseAsync instead.")]
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public async Task<global::Common.First.Api.Item.Matches.Item.WithEventCodeResponse?> GetAsync(Action<RequestConfiguration<global::Common.First.Api.Item.Matches.Item.WithEventCodeItemRequestBuilder.WithEventCodeItemRequestBuilderGetQueryParameters>>? requestConfiguration = default, CancellationToken cancellationToken = default)
        {
#nullable restore
#else
        public async Task<global::Common.First.Api.Item.Matches.Item.WithEventCodeResponse> GetAsync(Action<RequestConfiguration<global::Common.First.Api.Item.Matches.Item.WithEventCodeItemRequestBuilder.WithEventCodeItemRequestBuilderGetQueryParameters>> requestConfiguration = default, CancellationToken cancellationToken = default)
        {
#endif
            var requestInfo = ToGetRequestInformation(requestConfiguration);
            var errorMapping = new Dictionary<string, ParsableFactory<IParsable>>
            {
                { "500", global::Common.First.Api.Item.Matches.Item.WithEventCode500Error.CreateFromDiscriminatorValue },
            };
            return await RequestAdapter.SendAsync<global::Common.First.Api.Item.Matches.Item.WithEventCodeResponse>(requestInfo, global::Common.First.Api.Item.Matches.Item.WithEventCodeResponse.CreateFromDiscriminatorValue, errorMapping, cancellationToken).ConfigureAwait(false);
        }
        /// <summary>
        /// The match results API returns the match results for all matches of a particular event in a particular season. Match results are only available once a match has been played, retrieving info about future matches requires the event schedule API. You cannot receive data about a match that is in progress.If you specify the `matchNumber`, `start` and/or `end` optional parameters, you must also specify a `tournamentLevel`. If you specify the `teamNumber` parameter, you cannot specify a `matchNumber` parameter. If you specify the matchNumber, you cannot define a `start` or `end`.**Note**: If you specify `start`, and it is higher than the maximum match number at the event, you will not receive any match results in the response. The same is true in reverse for the end parameter.Starting in the 2015 season, Elimination matches were renamed to Playoff matches. As such, you must request Playoff matches from the API, and &quot;elim&quot; will not return any results. In Playoffs, match numbers 1-8 are &quot;Quarterfinal&quot; matches, 9-14 are &quot;Semifinal&quot; and 15-17 are &quot;Finals&quot; matches. The &quot;level&quot; response however, will always just show &quot;Playoff&quot; regardless of the portion of the Playoff tournament.
        /// </summary>
        /// <returns>A <see cref="RequestInformation"/></returns>
        /// <param name="requestConfiguration">Configuration for the request such as headers, query parameters, and middleware options.</param>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public RequestInformation ToGetRequestInformation(Action<RequestConfiguration<global::Common.First.Api.Item.Matches.Item.WithEventCodeItemRequestBuilder.WithEventCodeItemRequestBuilderGetQueryParameters>>? requestConfiguration = default)
        {
#nullable restore
#else
        public RequestInformation ToGetRequestInformation(Action<RequestConfiguration<global::Common.First.Api.Item.Matches.Item.WithEventCodeItemRequestBuilder.WithEventCodeItemRequestBuilderGetQueryParameters>> requestConfiguration = default)
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
        /// <returns>A <see cref="global::Common.First.Api.Item.Matches.Item.WithEventCodeItemRequestBuilder"/></returns>
        /// <param name="rawUrl">The raw URL to use for the request builder.</param>
        public global::Common.First.Api.Item.Matches.Item.WithEventCodeItemRequestBuilder WithUrl(string rawUrl)
        {
            return new global::Common.First.Api.Item.Matches.Item.WithEventCodeItemRequestBuilder(rawUrl, RequestAdapter);
        }
        /// <summary>
        /// The match results API returns the match results for all matches of a particular event in a particular season. Match results are only available once a match has been played, retrieving info about future matches requires the event schedule API. You cannot receive data about a match that is in progress.If you specify the `matchNumber`, `start` and/or `end` optional parameters, you must also specify a `tournamentLevel`. If you specify the `teamNumber` parameter, you cannot specify a `matchNumber` parameter. If you specify the matchNumber, you cannot define a `start` or `end`.**Note**: If you specify `start`, and it is higher than the maximum match number at the event, you will not receive any match results in the response. The same is true in reverse for the end parameter.Starting in the 2015 season, Elimination matches were renamed to Playoff matches. As such, you must request Playoff matches from the API, and &quot;elim&quot; will not return any results. In Playoffs, match numbers 1-8 are &quot;Quarterfinal&quot; matches, 9-14 are &quot;Semifinal&quot; and 15-17 are &quot;Finals&quot; matches. The &quot;level&quot; response however, will always just show &quot;Playoff&quot; regardless of the portion of the Playoff tournament.
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCode("Kiota", "1.0.0")]
        public partial class WithEventCodeItemRequestBuilderGetQueryParameters 
        {
            /// <summary>**(int)** Optional end match number for subset of results to return (inclusive).</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
            [QueryParameter("end")]
            public string? End { get; set; }
#nullable restore
#else
            [QueryParameter("end")]
            public string End { get; set; }
#endif
            /// <summary>**(int)** Optional specific single matchNumber of result.</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
            [QueryParameter("matchNumber")]
            public string? MatchNumber { get; set; }
#nullable restore
#else
            [QueryParameter("matchNumber")]
            public string MatchNumber { get; set; }
#endif
            /// <summary>**(int)** Optional start match number for subset of results to return.</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
            [QueryParameter("start")]
            public string? Start { get; set; }
#nullable restore
#else
            [QueryParameter("start")]
            public string Start { get; set; }
#endif
            /// <summary>**(int)** Optional teamNumber to search for within the results. Only returns match results in which the requested team was a participant.</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
            [QueryParameter("teamNumber")]
            public string? TeamNumber { get; set; }
#nullable restore
#else
            [QueryParameter("teamNumber")]
            public string TeamNumber { get; set; }
#endif
            /// <summary>**(string)** Optional tournamentLevel of desired match results.Enum values:```1. None2. Practice3. Qualification4. Playoff```</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
            [QueryParameter("tournamentLevel")]
            public string? TournamentLevel { get; set; }
#nullable restore
#else
            [QueryParameter("tournamentLevel")]
            public string TournamentLevel { get; set; }
#endif
        }
        /// <summary>
        /// Configuration for the request such as headers, query parameters, and middleware options.
        /// </summary>
        [Obsolete("This class is deprecated. Please use the generic RequestConfiguration class generated by the generator.")]
        [global::System.CodeDom.Compiler.GeneratedCode("Kiota", "1.0.0")]
        public partial class WithEventCodeItemRequestBuilderGetRequestConfiguration : RequestConfiguration<global::Common.First.Api.Item.Matches.Item.WithEventCodeItemRequestBuilder.WithEventCodeItemRequestBuilderGetQueryParameters>
        {
        }
    }
}
#pragma warning restore CS0618
