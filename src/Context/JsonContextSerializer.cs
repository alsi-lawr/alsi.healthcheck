// <copyright file="JsonContextSerializer.cs" company="ALSI">
// Copyright (c) ALSI. All rights reserved.
// </copyright>

namespace ALSI.HealthCheck.Context;

using System.Collections.Immutable;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

/// <summary>
/// Json context serializer that provides detailed results in structured json.
/// </summary>
public record struct JsonContextSerializer : IContextSerializer
{
    private static JsonSerializerOptions _jsonOptions =>
        new() { Converters = { new JsonStringEnumConverter() } };

    private static HealthResponse Serialize(
        ImmutableDictionary<string, HealthStatus> results,
        string status,
        TimeSpan since,
        HttpStatusCode code
    )
    {
        var json = JsonSerializer.Serialize(
            new
            {
                Status = status,
                Since = since,
                Results = results,
            },
            _jsonOptions
        );
        return new(Encoding.UTF8.GetBytes(json), code);
    }

    /// <inheritdoc/>
#if NET6_0_OR_GREATER
    public static string ContentType => "application/json";

    /// <inheritdoc/>
    public static HealthResponse Healthy(
        ImmutableDictionary<string, HealthStatus> results,
        TimeSpan since
    ) => Serialize(results, "Healthy", since, HttpStatusCode.OK);

    /// <inheritdoc/>
    public static HealthResponse Unhealthy(
        ImmutableDictionary<string, HealthStatus> results,
        TimeSpan since
    ) => Serialize(results, "Unhealthy", since, HttpStatusCode.Conflict);
#else
    public readonly string ContentType => "application/json";

    /// <inheritdoc/>
    public readonly HealthResponse Healthy(
        ImmutableDictionary<string, HealthStatus> results,
        TimeSpan since
    ) => Serialize(results, "Healthy", since, HttpStatusCode.OK);

    /// <inheritdoc/>
    public readonly HealthResponse Unhealthy(
        ImmutableDictionary<string, HealthStatus> results,
        TimeSpan since
    ) => Serialize(results, "Unhealthy", since, HttpStatusCode.Conflict);
#endif
}
