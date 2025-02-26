// <copyright file="DatabaseHealthCheck.cs" company="ALSI">
// Copyright (c) ALSI. All rights reserved.
// </copyright>

namespace ALSI.HealthCheck.Monitoring;

using System;
using System.Data.Common;
using ALSI.HealthCheck.Context;

/// <summary>
/// Provides factory methods for constructing database health checks.
/// </summary>
/// <remarks>
/// These methods create health checks that are provider-agnostic; they work with any database
/// provider that derives from <see cref="DbConnection"/> and supports asynchronous operations.
/// The health check attempts to open a connection and execute a test query (defaulting to "SELECT 1;").
/// If the operation succeeds, the check returns a healthy status; otherwise, it returns an unhealthy status.
/// </remarks>
public static class DatabaseHealthCheck
{
    /// <summary>
    /// Creates a health check for a database provider.
    /// </summary>
    /// <typeparam name="TConnection">
    /// The specific type of <see cref="DbConnection"/> used by the database provider.
    /// </typeparam>
    /// <param name="name">A unique name for the health check.</param>
    /// <param name="connectionFactory">
    /// A delegate that creates a new instance of <typeparamref name="TConnection"/> when invoked.
    /// </param>
    /// <param name="testQuery">
    /// The SQL query to execute as the health check test. Defaults to <c>"SELECT 1;"</c>.
    /// </param>
    /// <returns>
    /// A <see cref="HealthCheck"/> instance that encapsulates the logic for validating the database connection.
    /// </returns>
    /// <remarks>
    /// The returned health check opens a connection using the provided factory, executes the specified test query,
    /// and returns <see cref="HealthStatus.Healthy"/> if the operation completes successfully.
    /// Any exception encountered during the process results in a <see cref="HealthStatus.Unhealthy"/> status.
    /// </remarks>
    public static HealthCheck Create<TConnection>(
        string name,
        Func<TConnection> connectionFactory,
        string testQuery = "SELECT 1;"
    )
        where TConnection : DbConnection =>
        new(
            name,
            async context =>
            {
                try
                {
                    await using TConnection connection = connectionFactory();
                    await connection.OpenAsync(context.CancellationToken);

                    await using var command = connection.CreateCommand();
                    command.CommandText = testQuery;

                    await command.ExecuteScalarAsync(context.CancellationToken);

                    return HealthStatus.Healthy;
                }
                catch (Exception)
                {
                    return HealthStatus.Unhealthy;
                }
            }
        );
}
