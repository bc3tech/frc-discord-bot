/*
 * The Blue Alliance API v3
 *
 * # Overview    Information and statistics about FIRST Robotics Competition teams and events.   # Authentication   All endpoints require an Auth Key to be passed in the header `X-TBA-Auth-Key`. If you do not have an auth key yet, you can obtain one from your [Account Page](/account).
 *
 * The version of the OpenAPI document: 3.9.7
 * Generated by: https://github.com/openapitools/openapi-generator.git
 */

namespace TheBlueAlliance.Client;

using System;

/// <summary>
/// A delegate to ExceptionFactory method
/// </summary>
/// <param name="methodName">Method name</param>
/// <param name="response">Response</param>
/// <returns>Exceptions</returns>
public delegate Exception? ExceptionFactory(string methodName, IApiResponse response);