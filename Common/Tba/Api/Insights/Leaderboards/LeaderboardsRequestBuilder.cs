// <auto-generated/>
#pragma warning disable CS0618
using Common.Tba.Api.Insights.Leaderboards.Item;
using Microsoft.Kiota.Abstractions.Extensions;
using Microsoft.Kiota.Abstractions;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System;
namespace Common.Tba.Api.Insights.Leaderboards
{
    /// <summary>
    /// Builds and executes requests for operations under \insights\leaderboards
    /// </summary>
    [global::System.CodeDom.Compiler.GeneratedCode("Kiota", "1.0.0")]
    public partial class LeaderboardsRequestBuilder : BaseRequestBuilder
    {
        /// <summary>Gets an item from the Common.Tba.Api.insights.leaderboards.item collection</summary>
        /// <param name="position">Competition Year (or Season). Must be 4 digits.</param>
        /// <returns>A <see cref="global::Common.Tba.Api.Insights.Leaderboards.Item.WithYearItemRequestBuilder"/></returns>
        public global::Common.Tba.Api.Insights.Leaderboards.Item.WithYearItemRequestBuilder this[int position]
        {
            get
            {
                var urlTplParams = new Dictionary<string, object>(PathParameters);
                urlTplParams.Add("year", position);
                return new global::Common.Tba.Api.Insights.Leaderboards.Item.WithYearItemRequestBuilder(urlTplParams, RequestAdapter);
            }
        }
        /// <summary>Gets an item from the Common.Tba.Api.insights.leaderboards.item collection</summary>
        /// <param name="position">Competition Year (or Season). Must be 4 digits.</param>
        /// <returns>A <see cref="global::Common.Tba.Api.Insights.Leaderboards.Item.WithYearItemRequestBuilder"/></returns>
        [Obsolete("This indexer is deprecated and will be removed in the next major version. Use the one with the typed parameter instead.")]
        public global::Common.Tba.Api.Insights.Leaderboards.Item.WithYearItemRequestBuilder this[string position]
        {
            get
            {
                var urlTplParams = new Dictionary<string, object>(PathParameters);
                if (!string.IsNullOrWhiteSpace(position)) urlTplParams.Add("year", position);
                return new global::Common.Tba.Api.Insights.Leaderboards.Item.WithYearItemRequestBuilder(urlTplParams, RequestAdapter);
            }
        }
        /// <summary>
        /// Instantiates a new <see cref="global::Common.Tba.Api.Insights.Leaderboards.LeaderboardsRequestBuilder"/> and sets the default values.
        /// </summary>
        /// <param name="pathParameters">Path parameters for the request</param>
        /// <param name="requestAdapter">The request adapter to use to execute the requests.</param>
        public LeaderboardsRequestBuilder(Dictionary<string, object> pathParameters, IRequestAdapter requestAdapter) : base(requestAdapter, "{+baseurl}/insights/leaderboards", pathParameters)
        {
        }
        /// <summary>
        /// Instantiates a new <see cref="global::Common.Tba.Api.Insights.Leaderboards.LeaderboardsRequestBuilder"/> and sets the default values.
        /// </summary>
        /// <param name="rawUrl">The raw URL to use for the request builder.</param>
        /// <param name="requestAdapter">The request adapter to use to execute the requests.</param>
        public LeaderboardsRequestBuilder(string rawUrl, IRequestAdapter requestAdapter) : base(requestAdapter, "{+baseurl}/insights/leaderboards", rawUrl)
        {
        }
    }
}
#pragma warning restore CS0618
