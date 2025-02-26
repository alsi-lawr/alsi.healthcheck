// <copyright file="DatabaseHealthCheckTests.cs" company="ALSI">
// Copyright (c) ALSI. All rights reserved.
// </copyright>

namespace ALSI.HealthCheck.UnitTests.Monitoring;

using System;
using System.Threading;
using System.Threading.Tasks;
using ALSI.HealthCheck.Context;
using ALSI.HealthCheck.Monitoring;
using ALSI.HealthCheck.UnitTests.DummyDb;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

public class DatabaseHealthCheckTests
{
    [Fact]
    public async Task Create_Should_ReturnHealthyStatus_WhenConnectionSucceeds()
    {
        // Arrange
        string expectedQuery = "SELECT 1;";

        var healthCheck = DatabaseHealthCheck.Create(
            "TestDB",
            () => new DummyDbConnection<NothingOnDbOpen>(),
            expectedQuery
        );

        var registry = new ServiceHealthRegistry();
        var context = new HealthCheckContext(registry, CancellationToken.None)
        {
            State = new HealthState(DateTime.UtcNow, HealthStatus.Healthy),
        };
        IServiceProvider serviceProvider = new ServiceCollection().BuildServiceProvider();

        // Act
        HealthStatus result = await healthCheck.Check(serviceProvider, context);

        // Assert
        result.ShouldBe(HealthStatus.Healthy);
    }

    [Fact]
    public async Task Create_Should_ReturnUnhealthyStatus_WhenConnectionFails()
    {
        // Arrange
        // Simulate a failure by having the connection factory throw an exception.
        var healthCheck = DatabaseHealthCheck.Create(
            "TestDB",
            () => new DummyDbConnection<ErrorOnDbOpen>(),
            "SELECT 1;"
        );
        var registry = new ServiceHealthRegistry();
        var context = new HealthCheckContext(registry, CancellationToken.None)
        {
            State = new HealthState(DateTime.UtcNow, HealthStatus.Healthy),
        };
        IServiceProvider serviceProvider = new ServiceCollection().BuildServiceProvider();

        // Act
        HealthStatus result = await healthCheck.Check(serviceProvider, context);

        // Assert
        result.ShouldBe(HealthStatus.Unhealthy);
    }
}
