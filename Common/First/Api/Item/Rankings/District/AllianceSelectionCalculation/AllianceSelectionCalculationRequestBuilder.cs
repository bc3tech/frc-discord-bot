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
namespace Common.First.Api.Item.Rankings.District.AllianceSelectionCalculation
{
    /// <summary>
    /// Builds and executes requests for operations under \{season}\rankings\district\allianceSelectionCalculation
    /// </summary>
    [global::System.CodeDom.Compiler.GeneratedCode("Kiota", "1.0.0")]
    public partial class AllianceSelectionCalculationRequestBuilder : BaseRequestBuilder
    {
        /// <summary>
        /// Instantiates a new <see cref="global::Common.First.Api.Item.Rankings.District.AllianceSelectionCalculation.AllianceSelectionCalculationRequestBuilder"/> and sets the default values.
        /// </summary>
        /// <param name="pathParameters">Path parameters for the request</param>
        /// <param name="requestAdapter">The request adapter to use to execute the requests.</param>
        public AllianceSelectionCalculationRequestBuilder(Dictionary<string, object> pathParameters, IRequestAdapter requestAdapter) : base(requestAdapter, "{+baseurl}/{season}/rankings/district/allianceSelectionCalculation", pathParameters)
        {
        }
        /// <summary>
        /// Instantiates a new <see cref="global::Common.First.Api.Item.Rankings.District.AllianceSelectionCalculation.AllianceSelectionCalculationRequestBuilder"/> and sets the default values.
        /// </summary>
        /// <param name="rawUrl">The raw URL to use for the request builder.</param>
        /// <param name="requestAdapter">The request adapter to use to execute the requests.</param>
        public AllianceSelectionCalculationRequestBuilder(string rawUrl, IRequestAdapter requestAdapter) : base(requestAdapter, "{+baseurl}/{season}/rankings/district/allianceSelectionCalculation", rawUrl)
        {
        }
        /// <summary>
        /// Alliance Selection Points is one of three endpoints to assist teams in figuring out how to improve their performance to achieve the desired district ranking placement. It is to determine the Event Total points. `tournamentType`, `sizeType`, `allianceNumber`, and `allianceRole` are all required parameters for the calculation to occur.
        /// </summary>
        /// <returns>A <see cref="global::Common.First.Api.Item.Rankings.District.AllianceSelectionCalculation.AllianceSelectionCalculationGetResponse"/></returns>
        /// <param name="cancellationToken">Cancellation token to use when cancelling requests</param>
        /// <param name="requestConfiguration">Configuration for the request such as headers, query parameters, and middleware options.</param>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public async Task<global::Common.First.Api.Item.Rankings.District.AllianceSelectionCalculation.AllianceSelectionCalculationGetResponse?> GetAsAllianceSelectionCalculationGetResponseAsync(Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = default, CancellationToken cancellationToken = default)
        {
#nullable restore
#else
        public async Task<global::Common.First.Api.Item.Rankings.District.AllianceSelectionCalculation.AllianceSelectionCalculationGetResponse> GetAsAllianceSelectionCalculationGetResponseAsync(Action<RequestConfiguration<DefaultQueryParameters>> requestConfiguration = default, CancellationToken cancellationToken = default)
        {
#endif
            var requestInfo = ToGetRequestInformation(requestConfiguration);
            return await RequestAdapter.SendAsync<global::Common.First.Api.Item.Rankings.District.AllianceSelectionCalculation.AllianceSelectionCalculationGetResponse>(requestInfo, global::Common.First.Api.Item.Rankings.District.AllianceSelectionCalculation.AllianceSelectionCalculationGetResponse.CreateFromDiscriminatorValue, default, cancellationToken).ConfigureAwait(false);
        }
        /// <summary>
        /// Alliance Selection Points is one of three endpoints to assist teams in figuring out how to improve their performance to achieve the desired district ranking placement. It is to determine the Event Total points. `tournamentType`, `sizeType`, `allianceNumber`, and `allianceRole` are all required parameters for the calculation to occur.
        /// </summary>
        /// <returns>A <see cref="global::Common.First.Api.Item.Rankings.District.AllianceSelectionCalculation.AllianceSelectionCalculationResponse"/></returns>
        /// <param name="cancellationToken">Cancellation token to use when cancelling requests</param>
        /// <param name="requestConfiguration">Configuration for the request such as headers, query parameters, and middleware options.</param>
        [Obsolete("This method is obsolete. Use GetAsAllianceSelectionCalculationGetResponseAsync instead.")]
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public async Task<global::Common.First.Api.Item.Rankings.District.AllianceSelectionCalculation.AllianceSelectionCalculationResponse?> GetAsync(Action<RequestConfiguration<DefaultQueryParameters>>? requestConfiguration = default, CancellationToken cancellationToken = default)
        {
#nullable restore
#else
        public async Task<global::Common.First.Api.Item.Rankings.District.AllianceSelectionCalculation.AllianceSelectionCalculationResponse> GetAsync(Action<RequestConfiguration<DefaultQueryParameters>> requestConfiguration = default, CancellationToken cancellationToken = default)
        {
#endif
            var requestInfo = ToGetRequestInformation(requestConfiguration);
            return await RequestAdapter.SendAsync<global::Common.First.Api.Item.Rankings.District.AllianceSelectionCalculation.AllianceSelectionCalculationResponse>(requestInfo, global::Common.First.Api.Item.Rankings.District.AllianceSelectionCalculation.AllianceSelectionCalculationResponse.CreateFromDiscriminatorValue, default, cancellationToken).ConfigureAwait(false);
        }
        /// <summary>
        /// Alliance Selection Points is one of three endpoints to assist teams in figuring out how to improve their performance to achieve the desired district ranking placement. It is to determine the Event Total points. `tournamentType`, `sizeType`, `allianceNumber`, and `allianceRole` are all required parameters for the calculation to occur.
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
        /// <returns>A <see cref="global::Common.First.Api.Item.Rankings.District.AllianceSelectionCalculation.AllianceSelectionCalculationRequestBuilder"/></returns>
        /// <param name="rawUrl">The raw URL to use for the request builder.</param>
        public global::Common.First.Api.Item.Rankings.District.AllianceSelectionCalculation.AllianceSelectionCalculationRequestBuilder WithUrl(string rawUrl)
        {
            return new global::Common.First.Api.Item.Rankings.District.AllianceSelectionCalculation.AllianceSelectionCalculationRequestBuilder(rawUrl, RequestAdapter);
        }
        /// <summary>
        /// Configuration for the request such as headers, query parameters, and middleware options.
        /// </summary>
        [Obsolete("This class is deprecated. Please use the generic RequestConfiguration class generated by the generator.")]
        [global::System.CodeDom.Compiler.GeneratedCode("Kiota", "1.0.0")]
        public partial class AllianceSelectionCalculationRequestBuilderGetRequestConfiguration : RequestConfiguration<DefaultQueryParameters>
        {
        }
    }
}
#pragma warning restore CS0618
