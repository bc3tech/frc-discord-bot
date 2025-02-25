/*
 * The Blue Alliance API v3
 *
 * # Overview    Information and statistics about FIRST Robotics Competition teams and events.   # Authentication   All endpoints require an Auth Key to be passed in the header `X-TBA-Auth-Key`. If you do not have an auth key yet, you can obtain one from your [Account Page](/account).
 *
 * The version of the OpenAPI document: 3.9.11
 * Generated by: https://github.com/openapitools/openapi-generator.git
 */

namespace TheBlueAlliance.Client;

using Polly;
  using System.Net.Http;

/// <summary>
/// Configuration class to set the polly retry policies to be applied to the requests.
/// </summary>
public static class RetryConfiguration
{
    /// <summary>
    /// Retry policy
    /// </summary>
    public static Policy<HttpResponseMessage>? RetryPolicy { get; set; }
    
    /// <summary>
    /// Async retry policy
    /// </summary>
    public static AsyncPolicy<HttpResponseMessage>? AsyncRetryPolicy { get; set; }
}