/*
 * Statbotics REST API
 *
 * The REST API for Statbotics. Please be nice to our servers! If you are looking to do large-scale data science projects, use the CSV exports on the GitHub repo.
 *
 * The version of the OpenAPI document: 3.0.0
 * Generated by: https://github.com/openapitools/openapi-generator.git
 */

namespace Statbotics.Client;

using System;

/// <summary>
/// A delegate to ExceptionFactory method
/// </summary>
/// <param name="methodName">Method name</param>
/// <param name="response">Response</param>
/// <returns>Exceptions</returns>
internal delegate Exception? ExceptionFactory(string methodName, IApiResponse response);