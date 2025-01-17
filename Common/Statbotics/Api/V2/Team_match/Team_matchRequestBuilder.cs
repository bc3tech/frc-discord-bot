// <auto-generated/>
#pragma warning disable CS0618
using Common.Statbotics.Api.V2.Team_match.Item;
using Microsoft.Kiota.Abstractions.Extensions;
using Microsoft.Kiota.Abstractions;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System;
namespace Common.Statbotics.Api.V2.Team_match
{
    /// <summary>
    /// Builds and executes requests for operations under \v2\team_match
    /// </summary>
    [global::System.CodeDom.Compiler.GeneratedCode("Kiota", "1.0.0")]
    public partial class Team_matchRequestBuilder : BaseRequestBuilder
    {
        /// <summary>Gets an item from the Common.Statbotics.Api.v2.team_match.item collection</summary>
        /// <param name="position">Unique identifier of the item</param>
        /// <returns>A <see cref="global::Common.Statbotics.Api.V2.Team_match.Item.WithTeamItemRequestBuilder"/></returns>
        public global::Common.Statbotics.Api.V2.Team_match.Item.WithTeamItemRequestBuilder this[int position]
        {
            get
            {
                var urlTplParams = new Dictionary<string, object>(PathParameters);
                urlTplParams.Add("team", position);
                return new global::Common.Statbotics.Api.V2.Team_match.Item.WithTeamItemRequestBuilder(urlTplParams, RequestAdapter);
            }
        }
        /// <summary>Gets an item from the Common.Statbotics.Api.v2.team_match.item collection</summary>
        /// <param name="position">Unique identifier of the item</param>
        /// <returns>A <see cref="global::Common.Statbotics.Api.V2.Team_match.Item.WithTeamItemRequestBuilder"/></returns>
        [Obsolete("This indexer is deprecated and will be removed in the next major version. Use the one with the typed parameter instead.")]
        public global::Common.Statbotics.Api.V2.Team_match.Item.WithTeamItemRequestBuilder this[string position]
        {
            get
            {
                var urlTplParams = new Dictionary<string, object>(PathParameters);
                if (!string.IsNullOrWhiteSpace(position)) urlTplParams.Add("team", position);
                return new global::Common.Statbotics.Api.V2.Team_match.Item.WithTeamItemRequestBuilder(urlTplParams, RequestAdapter);
            }
        }
        /// <summary>
        /// Instantiates a new <see cref="global::Common.Statbotics.Api.V2.Team_match.Team_matchRequestBuilder"/> and sets the default values.
        /// </summary>
        /// <param name="pathParameters">Path parameters for the request</param>
        /// <param name="requestAdapter">The request adapter to use to execute the requests.</param>
        public Team_matchRequestBuilder(Dictionary<string, object> pathParameters, IRequestAdapter requestAdapter) : base(requestAdapter, "{+baseurl}/v2/team_match", pathParameters)
        {
        }
        /// <summary>
        /// Instantiates a new <see cref="global::Common.Statbotics.Api.V2.Team_match.Team_matchRequestBuilder"/> and sets the default values.
        /// </summary>
        /// <param name="rawUrl">The raw URL to use for the request builder.</param>
        /// <param name="requestAdapter">The request adapter to use to execute the requests.</param>
        public Team_matchRequestBuilder(string rawUrl, IRequestAdapter requestAdapter) : base(requestAdapter, "{+baseurl}/v2/team_match", rawUrl)
        {
        }
    }
}
#pragma warning restore CS0618
