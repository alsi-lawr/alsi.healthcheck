// <copyright file="HealthCheckContext.cs" company="ALSI">
// Copyright (c) ALSI. All rights reserved.
// </copyright>

namespace ALSI.HealthCheck.Context;

using System.Collections.Immutable;
using ALSI.HealthCheck.Monitoring;

/// <summary>
/// The context for the current health check.
/// </summary>
public record HealthCheckContext
{
    private readonly IServiceHealthRegistry _healthRegistry;

    internal HealthCheckContext(
        IServiceHealthRegistry healthRegistry,
        CancellationToken cancellationToken
    )
    {
        _healthRegistry = healthRegistry;
        CancellationToken = cancellationToken;
    }

    /// <summary>
    /// Gets or sets the cancellation token.
    /// </summary>
    public CancellationToken CancellationToken { get; set; } = CancellationToken.None;

    private ImmutableDictionary<string, HealthStatus> Values { get; init; } =
        ImmutableDictionary.Create<string, HealthStatus>();

#if NET6_0_OR_GREATER
    internal HealthResponse Serialize<TContextSerializer>()
        where TContextSerializer : IContextSerializer =>
        this.HasAnyUnhealthyOrLost
            ? TContextSerializer.Unhealthy(this.Values, this.State.Since)
            : TContextSerializer.Healthy(this.Values, this.State.Since);
#else
    internal HealthResponse Serialize<TContextSerializer>()
        where TContextSerializer : IContextSerializer, new() =>
        this.HasAnyUnhealthyOrLost
            ? new TContextSerializer().Unhealthy(this.Values, this.State.Since)
            : new TContextSerializer().Healthy(this.Values, this.State.Since);
#endif

    internal HealthCheckContext SetItem(string key, HealthStatus value) =>
        this with
        {
            Values = this.Values.SetItem(key, value),
        };

    internal bool HasAnyUnhealthyOrLost =>
        Values.Values.Any(status => (status & (HealthStatus.Unhealthy | HealthStatus.Lost)) != 0);

    internal HealthStatus GetServiceStatus<T>() =>
        _healthRegistry.TryGetValue(typeof(T).Name, out HealthStatus value)
            ? value
            : HealthStatus.Lost;

    internal HealthState State { get; init; } = new(DateTime.Now, HealthStatus.Unknown);
}
