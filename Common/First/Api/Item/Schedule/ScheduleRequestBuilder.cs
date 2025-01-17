// <auto-generated/>
#pragma warning disable CS0618
using Common.First.Api.Item.Schedule.Item;
using Microsoft.Kiota.Abstractions.Extensions;
using Microsoft.Kiota.Abstractions;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System;
namespace Common.First.Api.Item.Schedule
{
    /// <summary>
    /// Builds and executes requests for operations under \{season}\schedule
    /// </summary>
    [global::System.CodeDom.Compiler.GeneratedCode("Kiota", "1.0.0")]
    public partial class ScheduleRequestBuilder : BaseRequestBuilder
    {
        /// <summary>Gets an item from the Common.First.Api.item.schedule.item collection</summary>
        /// <param name="position">**[REQUIRED] (string)** Case insensitive alphanumeric eventCode of the event from which the schedule are requested. Must be at least 3 characters.</param>
        /// <returns>A <see cref="global::Common.First.Api.Item.Schedule.Item.WithEventCodeItemRequestBuilder"/></returns>
        public global::Common.First.Api.Item.Schedule.Item.WithEventCodeItemRequestBuilder this[string position]
        {
            get
            {
                var urlTplParams = new Dictionary<string, object>(PathParameters);
                urlTplParams.Add("eventCode", position);
                return new global::Common.First.Api.Item.Schedule.Item.WithEventCodeItemRequestBuilder(urlTplParams, RequestAdapter);
            }
        }
        /// <summary>
        /// Instantiates a new <see cref="global::Common.First.Api.Item.Schedule.ScheduleRequestBuilder"/> and sets the default values.
        /// </summary>
        /// <param name="pathParameters">Path parameters for the request</param>
        /// <param name="requestAdapter">The request adapter to use to execute the requests.</param>
        public ScheduleRequestBuilder(Dictionary<string, object> pathParameters, IRequestAdapter requestAdapter) : base(requestAdapter, "{+baseurl}/{season}/schedule", pathParameters)
        {
        }
        /// <summary>
        /// Instantiates a new <see cref="global::Common.First.Api.Item.Schedule.ScheduleRequestBuilder"/> and sets the default values.
        /// </summary>
        /// <param name="rawUrl">The raw URL to use for the request builder.</param>
        /// <param name="requestAdapter">The request adapter to use to execute the requests.</param>
        public ScheduleRequestBuilder(string rawUrl, IRequestAdapter requestAdapter) : base(requestAdapter, "{+baseurl}/{season}/schedule", rawUrl)
        {
        }
    }
}
#pragma warning restore CS0618
