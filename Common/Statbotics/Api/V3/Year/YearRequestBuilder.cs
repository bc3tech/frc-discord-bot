// <auto-generated/>
#pragma warning disable CS0618
using Common.Statbotics.Api.V3.Year.Item;
using Microsoft.Kiota.Abstractions.Extensions;
using Microsoft.Kiota.Abstractions;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System;
namespace Common.Statbotics.Api.V3.Year
{
    /// <summary>
    /// Builds and executes requests for operations under \v3\year
    /// </summary>
    [global::System.CodeDom.Compiler.GeneratedCode("Kiota", "1.0.0")]
    public partial class YearRequestBuilder : BaseRequestBuilder
    {
        /// <summary>Gets an item from the Common.Statbotics.Api.v3.year.item collection</summary>
        /// <param name="position">Unique identifier of the item</param>
        /// <returns>A <see cref="global::Common.Statbotics.Api.V3.Year.Item.WithYearItemRequestBuilder"/></returns>
        public global::Common.Statbotics.Api.V3.Year.Item.WithYearItemRequestBuilder this[int position]
        {
            get
            {
                var urlTplParams = new Dictionary<string, object>(PathParameters);
                urlTplParams.Add("year", position);
                return new global::Common.Statbotics.Api.V3.Year.Item.WithYearItemRequestBuilder(urlTplParams, RequestAdapter);
            }
        }
        /// <summary>Gets an item from the Common.Statbotics.Api.v3.year.item collection</summary>
        /// <param name="position">Unique identifier of the item</param>
        /// <returns>A <see cref="global::Common.Statbotics.Api.V3.Year.Item.WithYearItemRequestBuilder"/></returns>
        [Obsolete("This indexer is deprecated and will be removed in the next major version. Use the one with the typed parameter instead.")]
        public global::Common.Statbotics.Api.V3.Year.Item.WithYearItemRequestBuilder this[string position]
        {
            get
            {
                var urlTplParams = new Dictionary<string, object>(PathParameters);
                if (!string.IsNullOrWhiteSpace(position)) urlTplParams.Add("year", position);
                return new global::Common.Statbotics.Api.V3.Year.Item.WithYearItemRequestBuilder(urlTplParams, RequestAdapter);
            }
        }
        /// <summary>
        /// Instantiates a new <see cref="global::Common.Statbotics.Api.V3.Year.YearRequestBuilder"/> and sets the default values.
        /// </summary>
        /// <param name="pathParameters">Path parameters for the request</param>
        /// <param name="requestAdapter">The request adapter to use to execute the requests.</param>
        public YearRequestBuilder(Dictionary<string, object> pathParameters, IRequestAdapter requestAdapter) : base(requestAdapter, "{+baseurl}/v3/year", pathParameters)
        {
        }
        /// <summary>
        /// Instantiates a new <see cref="global::Common.Statbotics.Api.V3.Year.YearRequestBuilder"/> and sets the default values.
        /// </summary>
        /// <param name="rawUrl">The raw URL to use for the request builder.</param>
        /// <param name="requestAdapter">The request adapter to use to execute the requests.</param>
        public YearRequestBuilder(string rawUrl, IRequestAdapter requestAdapter) : base(requestAdapter, "{+baseurl}/v3/year", rawUrl)
        {
        }
    }
}
#pragma warning restore CS0618
