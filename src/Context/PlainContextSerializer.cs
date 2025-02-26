// <copyright file="PlainContextSerializer.cs" company="ALSI">
// Copyright (c) ALSI. All rights reserved.
// </copyright>

namespace ALSI.HealthCheck.Context;

using System.Collections.Immutable;
using System.Net;
using System.Text;

/// <summary>
/// Plain context serialiser that only returns "Healthy" or "Unhealthy".
/// </summary>
public record struct PlainContextSerializer : IContextSerializer
{
#if NET6_0_OR_GREATER
    /// <inheritdoc/>
    public static string ContentType => "text/plain";

    /// <inheritdoc/>
    public static HealthResponse Healthy(
        ImmutableDictionary<string, HealthStatus> results,
        TimeSpan since
    ) => new(Encoding.UTF8.GetBytes($"Healthy since {since}"), HttpStatusCode.OK);

    /// <inheritdoc/>
    public static HealthResponse Unhealthy(
        ImmutableDictionary<string, HealthStatus> results,
        TimeSpan since
    ) => new(Encoding.UTF8.GetBytes($"Unhealthy since {since}"), HttpStatusCode.Conflict);
#else
    /// <inheritdoc/>
    public readonly string ContentType => "text/plain";

    /// <inheritdoc/>
    public readonly HealthResponse Healthy(
        ImmutableDictionary<string, HealthStatus> results,
        TimeSpan since
    ) => new(Encoding.UTF8.GetBytes($"Healthy since {since}"), HttpStatusCode.OK);

    /// <inheritdoc/>
    public readonly HealthResponse Unhealthy(
        ImmutableDictionary<string, HealthStatus> results,
        TimeSpan since
    ) => new(Encoding.UTF8.GetBytes($"Unhealthy since {since}"), HttpStatusCode.Conflict);
#endif
}
