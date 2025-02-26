// <copyright file="ApiHealthCheck.cs" company="ALSI">
// Copyright (c) ALSI. All rights reserved.
// </copyright>

namespace ALSI.HealthCheck.Monitoring;

using System;
using System.Net;
using System.Net.Http;
using ALSI.HealthCheck.Context;

/// <summary>
/// Provides factory methods for constructing health checks for external APIs.
/// </summary>
/// <remarks>
/// The health check sends an HTTP request to the specified endpoint using an HttpClient.
/// If the response status code matches the expected value (default is 200 OK), the API is
/// considered healthy; otherwise, it is considered unhealthy.
/// </remarks>
public static class ApiHealthCheck
{
    /// <summary>
    /// Creates a health check for an external API endpoint.
    /// </summary>
    /// <param name="name">A unique name for the health check.</param>
    /// <param name="httpClientFactory">
    /// A function that returns an instance of <see cref="HttpClient"/> used to perform the API request.
    /// </param>
    /// <param name="requestUri">The URI of the API endpoint to test.</param>
    /// <param name="httpMethod">
    /// The HTTP method to use for the request. Defaults to GET if not specified.
    /// </param>
    /// <param name="expectedStatusCode">
    /// The HTTP status code that indicates the API is healthy. Defaults to 200 (OK).
    /// </param>
    /// <returns>
    /// A <see cref="HealthCheck"/> that executes the API request and returns the corresponding health status.
    /// </returns>
    public static HealthCheck Create(
        string name,
        Func<IServiceProvider, HttpClient> httpClientFactory,
        string requestUri,
        HttpMethod? httpMethod = null,
        HttpStatusCode expectedStatusCode = HttpStatusCode.OK
    ) =>
        new(
            name,
            async (sp, context) =>
            {
                try
                {
                    using HttpClient client = httpClientFactory(sp);
                    using HttpRequestMessage request =
                        new(httpMethod ?? HttpMethod.Get, requestUri);
                    HttpResponseMessage response = await client
                        .SendAsync(request, context.CancellationToken)
                        .ConfigureAwait(false);

                    return response.StatusCode == expectedStatusCode
                        ? HealthStatus.Healthy
                        : HealthStatus.Unhealthy;
                }
                catch (Exception)
                {
                    return HealthStatus.Unhealthy;
                }
            }
        );

    /// <summary>
    /// Creates a health check for an external API endpoint.
    /// </summary>
    /// <param name="name">A unique name for the health check.</param>
    /// <param name="httpClientFactory">
    /// A function that returns an instance of <see cref="HttpClient"/> used to perform the API request.
    /// </param>
    /// <param name="requestUri">The URI of the API endpoint to test.</param>
    /// <param name="httpMethod">
    /// The HTTP method to use for the request. Defaults to GET if not specified.
    /// </param>
    /// <param name="expectedStatusCode">
    /// The HTTP status code that indicates the API is healthy. Defaults to 200 (OK).
    /// </param>
    /// <returns>
    /// A <see cref="HealthCheck"/> that executes the API request and returns the corresponding health status.
    /// </returns>
    public static HealthCheck Create(
        string name,
        Func<HttpClient> httpClientFactory,
        string requestUri,
        HttpMethod? httpMethod = null,
        HttpStatusCode expectedStatusCode = HttpStatusCode.OK
    ) => Create(name, _ => httpClientFactory(), requestUri, httpMethod, expectedStatusCode);
}
