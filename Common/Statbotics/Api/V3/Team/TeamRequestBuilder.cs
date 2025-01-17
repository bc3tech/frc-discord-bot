// <auto-generated/>
#pragma warning disable CS0618
using Common.Statbotics.Api.V3.Team.Item;
using Microsoft.Kiota.Abstractions.Extensions;
using Microsoft.Kiota.Abstractions;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System;
namespace Common.Statbotics.Api.V3.Team
{
    /// <summary>
    /// Builds and executes requests for operations under \v3\team
    /// </summary>
    [global::System.CodeDom.Compiler.GeneratedCode("Kiota", "1.0.0")]
    public partial class TeamRequestBuilder : BaseRequestBuilder
    {
        /// <summary>Gets an item from the Common.Statbotics.Api.v3.team.item collection</summary>
        /// <param name="position">Unique identifier of the item</param>
        /// <returns>A <see cref="global::Common.Statbotics.Api.V3.Team.Item.WithTeamItemRequestBuilder"/></returns>
        public global::Common.Statbotics.Api.V3.Team.Item.WithTeamItemRequestBuilder this[string position]
        {
            get
            {
                var urlTplParams = new Dictionary<string, object>(PathParameters);
                urlTplParams.Add("team", position);
                return new global::Common.Statbotics.Api.V3.Team.Item.WithTeamItemRequestBuilder(urlTplParams, RequestAdapter);
            }
        }
        /// <summary>
        /// Instantiates a new <see cref="global::Common.Statbotics.Api.V3.Team.TeamRequestBuilder"/> and sets the default values.
        /// </summary>
        /// <param name="pathParameters">Path parameters for the request</param>
        /// <param name="requestAdapter">The request adapter to use to execute the requests.</param>
        public TeamRequestBuilder(Dictionary<string, object> pathParameters, IRequestAdapter requestAdapter) : base(requestAdapter, "{+baseurl}/v3/team", pathParameters)
        {
        }
        /// <summary>
        /// Instantiates a new <see cref="global::Common.Statbotics.Api.V3.Team.TeamRequestBuilder"/> and sets the default values.
        /// </summary>
        /// <param name="rawUrl">The raw URL to use for the request builder.</param>
        /// <param name="requestAdapter">The request adapter to use to execute the requests.</param>
        public TeamRequestBuilder(string rawUrl, IRequestAdapter requestAdapter) : base(requestAdapter, "{+baseurl}/v3/team", rawUrl)
        {
        }
    }
}
#pragma warning restore CS0618
