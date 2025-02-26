// <copyright file="IContextSerializer.cs" company="ALSI">
// Copyright (c) ALSI. All rights reserved.
// </copyright>

namespace ALSI.HealthCheck.Context;

using System.Collections.Immutable;

#if NET6_0_OR_GREATER
/// <summary>
/// Defines a serializer for health check results that converts the aggregated results into a response
/// suitable for transmission (e.g. as JSON). Implementations must provide the content type and methods
/// to produce a healthy or unhealthy <see cref="HealthResponse"/> from a set of health check results.
/// </summary>
public interface IContextSerializer
{
    /// <summary>
    /// Gets the MIME content type for the serialized health check response.
    /// </summary>
    public static abstract string ContentType { get; }

    /// <summary>
    /// Serializes the provided health check results into a healthy response.
    /// </summary>
    /// <param name="results">
    /// The collection of health check results, where the key identifies the check and the value its status.
    /// </param>
    /// <param name="since">
    /// A <see cref="TimeSpan"/> indicating how long the system has been in the current (healthy) state.
    /// </param>
    /// <returns>
    /// A <see cref="HealthResponse"/> representing a healthy state, formatted according to the serializer’s rules.
    /// </returns>
    public static abstract HealthResponse Healthy(
        ImmutableDictionary<string, HealthStatus> results,
        TimeSpan since
    );

    /// <summary>
    /// Serializes the provided health check results into an unhealthy response.
    /// </summary>
    /// <param name="results">
    /// The collection of health check results, where the key identifies the check and the value its status.
    /// </param>
    /// <param name="since">
    /// A <see cref="TimeSpan"/> indicating how long the system has been in the current (healthy) state.
    /// </param>
    /// <returns>
    /// A <see cref="HealthResponse"/> representing an unhealthy state, formatted according to the serializer’s rules.
    /// </returns>
    public static abstract HealthResponse Unhealthy(
        ImmutableDictionary<string, HealthStatus> results,
        TimeSpan since
    );
}
#else
/// <summary>
/// Defines a serializer for health check results that converts the aggregated results into a response
/// suitable for transmission (e.g. as JSON). Implementations must provide the content type and methods
/// to produce a healthy or unhealthy <see cref="HealthResponse"/> from a set of health check results.
/// </summary>
public interface IContextSerializer
{
    /// <summary>
    /// Gets the MIME content type for the serialized health check response.
    /// </summary>
    string ContentType { get; }

    /// <summary>
    /// Serializes the provided health check results into a healthy response.
    /// </summary>
    /// <param name="results">
    /// The collection of health check results, where each key identifies a check and the corresponding value indicates its status.
    /// </param>
    /// <param name="since">
    /// A <see cref="TimeSpan"/> indicating how long the system has been in the current (healthy) state.
    /// </param>
    /// <returns>
    /// A <see cref="HealthResponse"/> representing a healthy state, formatted according to the serializer’s rules.
    /// </returns>
    HealthResponse Healthy(ImmutableDictionary<string, HealthStatus> results, TimeSpan since);

    /// <summary>
    /// Serializes the provided health check results into an unhealthy response.
    /// </summary>
    /// <param name="results">
    /// The collection of health check results, where each key identifies a check and the corresponding value indicates its status.
    /// </param>
    /// <param name="since">
    /// A <see cref="TimeSpan"/> indicating how long the system has been in the current (unhealthy) state.
    /// </param>
    /// <returns>
    /// A <see cref="HealthResponse"/> representing an unhealthy state, formatted according to the serializer’s rules.
    /// </returns>
    HealthResponse Unhealthy(ImmutableDictionary<string, HealthStatus> results, TimeSpan since);
}
#endif
