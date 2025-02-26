// <copyright file="HealthCheckOptions.cs" company="ALSI">
// Copyright (c) ALSI. All rights reserved.
// </copyright>

namespace ALSI.HealthCheck.Options;

/// <summary>
/// Represents the configuration settings for the health check endpoint.
/// </summary>
/// <remarks>
/// These options specify the network binding and URL base for health checks. By default:
/// <list type="bullet">
///   <item>
///     <description><see cref="Hostname"/> is set to "*"; the endpoint will bind to all available network interfaces.</description>
///   </item>
///   <item>
///     <description><see cref="Port"/> is set to 8080.</description>
///   </item>
///   <item>
///     <description><see cref="BasePath"/> is set to "/health", indicating the base path for health check requests.</description>
///   </item>
/// </list>
/// </remarks>
public record HealthCheckOptions
{
    /// <summary>
    /// Gets or sets the host name or IP address that the health check endpoint will bind to.
    /// </summary>
    /// <remarks>
    /// A value of "*" indicates that the endpoint will listen on all network interfaces.
    /// </remarks>
    public string Hostname { get; set; } = "*";

    /// <summary>
    /// Gets or sets the port number on which the health check endpoint will listen.
    /// </summary>
    /// <remarks>
    /// The default port is 8080.
    /// </remarks>
    public int Port { get; set; } = 8080;

    /// <summary>
    /// Gets or sets the URL path base for health check requests.
    /// </summary>
    /// <remarks>
    /// The default path is "/health".
    /// </remarks>
    public string BasePath { get; set; } = "/health";
}
