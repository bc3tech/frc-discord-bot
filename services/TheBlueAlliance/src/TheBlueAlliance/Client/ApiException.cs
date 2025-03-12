/*
 * The Blue Alliance API v3
 *
 * # Overview    Information and statistics about FIRST Robotics Competition teams and events.   # Authentication   All endpoints require an Auth Key to be passed in the header `X-TBA-Auth-Key`. If you do not have an auth key yet, you can obtain one from your [Account Page](/account).
 *
 * The version of the OpenAPI document: 3.9.11
 * Generated by: https://github.com/openapitools/openapi-generator.git
 */

namespace TheBlueAlliance.Client;

using System;

/// <summary>
/// API Exception
/// </summary>
public sealed class ApiException : Exception
{
    /// <summary>
    /// Gets or sets the error code (HTTP status code)
    /// </summary>
    /// <value>The error code (HTTP status code).</value>
    public int ErrorCode { get; set; }

    /// <summary>
    /// Gets or sets the error content (body json object)
    /// </summary>
    /// <value>The error content (Http response body).</value>
    public object? ErrorContent { get; private set; }

    /// <summary>
    /// Gets or sets the HTTP headers
    /// </summary>
    /// <value>HTTP headers</value>
    public Multimap<string, string>? Headers { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ApiException"/> class.
    /// </summary>
    public ApiException() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ApiException"/> class.
    /// </summary>
    /// <param name="errorCode">HTTP status code.</param>
    /// <param name="message">Error message.</param>
    public ApiException(int errorCode, string message) : base(message)
    {
        this.ErrorCode = errorCode;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ApiException"/> class.
    /// </summary>
    /// <param name="errorCode">HTTP status code.</param>
    /// <param name="message">Error message.</param>
    /// <param name="errorContent">Error content.</param>
    /// <param name="headers">HTTP Headers.</param>
    public ApiException(int errorCode, string message, object? errorContent = null, Multimap<string, string>? headers = null) : base(message)
    {
        this.ErrorCode = errorCode;
        this.ErrorContent = errorContent;
        this.Headers = headers;
    }
}