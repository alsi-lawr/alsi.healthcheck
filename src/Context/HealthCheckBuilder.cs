// <copyright file="HealthCheckBuilder.cs" company="ALSI">
// Copyright (c) ALSI. All rights reserved.
// </copyright>

namespace ALSI.HealthCheck.Context;

using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using ALSI.HealthCheck.Monitoring;

/// <summary>
/// Represents a collection of health checks.
/// </summary>
internal class HealthChecks : List<HealthCheck> { }

/// <summary>
/// A builder for constructing a collection of health checks.
/// </summary>
public record HealthCheckBuilder
{
    private readonly HealthChecks _checks = [];

    /// <summary>
    /// Adds a generic health check for a given type.
    /// </summary>
    /// <typeparam name="T">
    /// The identifier type for this health check.
    /// </typeparam>
    /// <param name="check">
    /// An asynchronous function that evaluates health using a <see cref="HealthCheckContext"/>.
    /// </param>
    /// <returns>This builder instance.</returns>
    public HealthCheckBuilder AddCheck<T>(Func<HealthCheckContext, Task<HealthStatus>> check)
        where T : notnull
    {
        _checks.Add(new HealthCheck<T>(typeof(T).Name, (_, ctx) => check(ctx)));
        return this;
    }

    /// <summary>
    /// Adds a generic health check for a given type.
    /// </summary>
    /// <param name="name">Name of the health check.</param>
    /// <param name="check">
    /// An asynchronous function that evaluates health using a <see cref="HealthCheckContext"/>.
    /// </param>
    /// <returns>This builder instance.</returns>
    public HealthCheckBuilder AddCheck(
        string name,
        Func<HealthCheckContext, Task<HealthStatus>> check
    )
    {
        _checks.Add(new HealthCheck(name, (_, ctx) => check(ctx)));
        return this;
    }

    /// <summary>
    /// Adds a generic health check for a given type.
    /// </summary>
    /// <typeparam name="T">
    /// The identifier type for this health check.
    /// </typeparam>
    /// <param name="check">
    /// An asynchronous function that evaluates health using a <see cref="HealthCheckContext"/>.
    /// </param>
    /// <returns>This builder instance.</returns>
    public HealthCheckBuilder AddCheck<T>(
        Func<IServiceProvider, HealthCheckContext, Task<HealthStatus>> check
    )
        where T : notnull
    {
        _checks.Add(new HealthCheck<T>(typeof(T).Name, check));
        return this;
    }

    /// <summary>
    /// Adds a generic health check for a given type.
    /// </summary>
    /// <param name="name">Name of the health check.</param>
    /// <param name="check">
    /// An asynchronous function that evaluates health using a <see cref="HealthCheckContext"/>.
    /// </param>
    /// <returns>This builder instance.</returns>
    public HealthCheckBuilder AddCheck(
        string name,
        Func<IServiceProvider, HealthCheckContext, Task<HealthStatus>> check
    )
    {
        _checks.Add(new HealthCheck(name, check));
        return this;
    }

    /// <summary>
    /// Adds a health check for a monitored background service.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the monitored background service.
    /// </typeparam>
    /// <returns>This builder instance.</returns>
    public HealthCheckBuilder AddMonitoredService<T>()
        where T : MonitoredBackgroundService
    {
        _checks.Add(
            new HealthCheck<T>(
                typeof(T).Name,
                (_, ctx) => Task.FromResult(ctx.GetServiceStatus<T>())
            )
        );
        return this;
    }

    /// <summary>
    /// Adds a health check for a database provider.
    /// </summary>
    /// <typeparam name="TConnection">
    /// The specific type of <see cref="DbConnection"/> used by the provider.
    /// </typeparam>
    /// <param name="name">
    /// A unique name for the health check.
    /// </param>
    /// <param name="connectionFactory">
    /// A delegate that creates a new instance of <typeparamref name="TConnection"/>.
    /// </param>
    /// <param name="testQuery">
    /// The SQL query used to verify connectivity (default is <c>"SELECT 1;"</c>).
    /// </param>
    /// <returns>This builder instance.</returns>
    public HealthCheckBuilder AddMonitoredDatabaseConnection<TConnection>(
        string name,
        Func<TConnection> connectionFactory,
        string testQuery = "SELECT 1;"
    )
        where TConnection : DbConnection
    {
        _checks.Add(DatabaseHealthCheck.Create(name, connectionFactory, testQuery));
        return this;
    }

    /// <summary>
    /// Adds a health check for an external API endpoint.
    /// </summary>
    /// <param name="name">
    /// A unique name for the health check.
    /// </param>
    /// <param name="clientFactory">
    /// A delegate that provides an instance of <see cref="HttpClient"/>.
    /// </param>
    /// <param name="requestUri">
    /// The URI of the API endpoint to test.
    /// </param>
    /// <param name="httpMethod">
    /// The HTTP method to use (defaults to GET if not specified).
    /// </param>
    /// <param name="expectedStatusCode">
    /// The status code indicating a healthy API (defaults to 200 OK).
    /// </param>
    /// <returns>This builder instance.</returns>
    public HealthCheckBuilder AddMonitoredApiRoute(
        string name,
        Func<HttpClient> clientFactory,
        string requestUri,
        HttpMethod? httpMethod = null,
        HttpStatusCode expectedStatusCode = HttpStatusCode.OK
    )
    {
        _checks.Add(
            ApiHealthCheck.Create(name, clientFactory, requestUri, httpMethod, expectedStatusCode)
        );
        return this;
    }

    /// <summary>
    /// Adds a health check for a database provider.
    /// </summary>
    /// <typeparam name="TConnection">
    /// The specific type of <see cref="DbConnection"/> used by the provider.
    /// </typeparam>
    /// <param name="name">
    /// A unique name for the health check.
    /// </param>
    /// <param name="connectionFactory">
    /// A delegate that creates a new instance of <typeparamref name="TConnection"/>.
    /// </param>
    /// <param name="testQuery">
    /// The SQL query used to verify connectivity (default is <c>"SELECT 1;"</c>).
    /// </param>
    /// <returns>This builder instance.</returns>
    public HealthCheckBuilder AddMonitoredDatabaseConnection<TConnection>(
        string name,
        Func<IServiceProvider, TConnection> connectionFactory,
        string testQuery = "SELECT 1;"
    )
        where TConnection : DbConnection
    {
        _checks.Add(DatabaseHealthCheck.Create(name, connectionFactory, testQuery));
        return this;
    }

    /// <summary>
    /// Adds a health check for an external API endpoint.
    /// </summary>
    /// <param name="name">
    /// A unique name for the health check.
    /// </param>
    /// <param name="clientFactory">
    /// A delegate that provides an instance of <see cref="HttpClient"/>.
    /// </param>
    /// <param name="requestUri">
    /// The URI of the API endpoint to test.
    /// </param>
    /// <param name="httpMethod">
    /// The HTTP method to use (defaults to GET if not specified).
    /// </param>
    /// <param name="expectedStatusCode">
    /// The status code indicating a healthy API (defaults to 200 OK).
    /// </param>
    /// <returns>This builder instance.</returns>
    public HealthCheckBuilder AddMonitoredApiRoute(
        string name,
        Func<IServiceProvider, HttpClient> clientFactory,
        string requestUri,
        HttpMethod? httpMethod = null,
        HttpStatusCode expectedStatusCode = HttpStatusCode.OK
    )
    {
        _checks.Add(
            ApiHealthCheck.Create(name, clientFactory, requestUri, httpMethod, expectedStatusCode)
        );
        return this;
    }

    /// <summary>
    /// Finalizes and returns the collection of configured health checks.
    /// </summary>
    /// <returns>A <see cref="HealthChecks"/> collection containing all registered checks.</returns>
    internal HealthChecks Build() => _checks;
}
