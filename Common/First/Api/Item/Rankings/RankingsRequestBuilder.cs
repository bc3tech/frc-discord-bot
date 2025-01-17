// <auto-generated/>
#pragma warning disable CS0618
using Common.First.Api.Item.Rankings.District;
using Common.First.Api.Item.Rankings.Item;
using Microsoft.Kiota.Abstractions.Extensions;
using Microsoft.Kiota.Abstractions;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System;
namespace Common.First.Api.Item.Rankings
{
    /// <summary>
    /// Builds and executes requests for operations under \{season}\rankings
    /// </summary>
    [global::System.CodeDom.Compiler.GeneratedCode("Kiota", "1.0.0")]
    public partial class RankingsRequestBuilder : BaseRequestBuilder
    {
        /// <summary>The district property</summary>
        public global::Common.First.Api.Item.Rankings.District.DistrictRequestBuilder District
        {
            get => new global::Common.First.Api.Item.Rankings.District.DistrictRequestBuilder(PathParameters, RequestAdapter);
        }
        /// <summary>Gets an item from the Common.First.Api.item.rankings.item collection</summary>
        /// <param name="position">(Required) **[REQUIRED] (string)** Case insensitive alphanumeric eventCode of the event from which the rankings are requested. Must be at least 3 characters.</param>
        /// <returns>A <see cref="global::Common.First.Api.Item.Rankings.Item.WithEventCodeItemRequestBuilder"/></returns>
        public global::Common.First.Api.Item.Rankings.Item.WithEventCodeItemRequestBuilder this[string position]
        {
            get
            {
                var urlTplParams = new Dictionary<string, object>(PathParameters);
                urlTplParams.Add("eventCode", position);
                return new global::Common.First.Api.Item.Rankings.Item.WithEventCodeItemRequestBuilder(urlTplParams, RequestAdapter);
            }
        }
        /// <summary>
        /// Instantiates a new <see cref="global::Common.First.Api.Item.Rankings.RankingsRequestBuilder"/> and sets the default values.
        /// </summary>
        /// <param name="pathParameters">Path parameters for the request</param>
        /// <param name="requestAdapter">The request adapter to use to execute the requests.</param>
        public RankingsRequestBuilder(Dictionary<string, object> pathParameters, IRequestAdapter requestAdapter) : base(requestAdapter, "{+baseurl}/{season}/rankings", pathParameters)
        {
        }
        /// <summary>
        /// Instantiates a new <see cref="global::Common.First.Api.Item.Rankings.RankingsRequestBuilder"/> and sets the default values.
        /// </summary>
        /// <param name="rawUrl">The raw URL to use for the request builder.</param>
        /// <param name="requestAdapter">The request adapter to use to execute the requests.</param>
        public RankingsRequestBuilder(string rawUrl, IRequestAdapter requestAdapter) : base(requestAdapter, "{+baseurl}/{season}/rankings", rawUrl)
        {
        }
    }
}
#pragma warning restore CS0618
