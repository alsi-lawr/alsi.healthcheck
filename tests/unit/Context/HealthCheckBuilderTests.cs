// <copyright file="HealthCheckBuilderTests.cs" company="ALSI">
// Copyright (c) ALSI. All rights reserved.
// </copyright>

namespace ALSI.HealthCheck.UnitTests.Context;

using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using ALSI.HealthCheck.Context;
using ALSI.HealthCheck.Monitoring;
using ALSI.HealthCheck.UnitTests.DummyDb;
using Shouldly;
using Xunit;

public class HealthCheckBuilderTests
{
    [Fact]
    public void AddCheck_Generic_AddsHealthCheckWithTypeName()
    {
        // Arrange
        var builder = new HealthCheckBuilder();
        Func<HealthCheckContext, Task<HealthStatus>> dummyCheck = ctx =>
            Task.FromResult(HealthStatus.Healthy);

        // Act
        builder.AddCheck<DummyService>(dummyCheck);
        var checks = builder.Build();

        // Assert
        Assert.Single(checks);
        Assert.Equal(nameof(DummyService), checks[0].Name);
    }

    [Fact]
    public void AddCheck_WithName_AddsHealthCheckWithSpecifiedName()
    {
        // Arrange
        var builder = new HealthCheckBuilder();
        Func<HealthCheckContext, Task<HealthStatus>> dummyCheck = ctx =>
            Task.FromResult(HealthStatus.Healthy);
        string expectedName = "CustomCheck";

        // Act
        builder.AddCheck(expectedName, dummyCheck);
        var checks = builder.Build();

        // Assert
        Assert.Single(checks);
        Assert.Equal(expectedName, checks[0].Name);
    }

    [Fact]
    public void AddMonitoredService_AddsHealthCheckForMonitoredService()
    {
        // Arrange
        var builder = new HealthCheckBuilder();

        // Act
        builder.AddMonitoredService<DummyService>();
        var checks = builder.Build();

        // Assert
        checks.ShouldHaveSingleItem();
        checks[0].Name.ShouldBe(nameof(DummyService));
    }

    [Fact]
    public void AddMonitoredDatabaseConnection_AddsDatabaseHealthCheck()
    {
        // Arrange
        var builder = new HealthCheckBuilder();

        // Act
        builder.AddMonitoredDatabaseConnection(
            "DBCheck",
            () => new DummyDbConnection<NothingOnDbOpen>(),
            "SELECT 1;"
        );
        var checks = builder.Build();

        // Assert
        checks.ShouldHaveSingleItem();
        checks[0].Name.ShouldBe("DBCheck");
    }

    [Fact]
    public void AddMonitoredApiRoute_AddsApiHealthCheck()
    {
        // Arrange
        var builder = new HealthCheckBuilder();

        // Act
        builder.AddMonitoredApiRoute(
            "ApiCheck",
            () => new HttpClient(),
            "http://example.com/api",
            HttpMethod.Get,
            HttpStatusCode.OK
        );
        var checks = builder.Build();

        // Assert
        checks.ShouldHaveSingleItem();
        checks[0].Name.ShouldBe("ApiCheck");
    }
}

// Dummy classes for testing purposes
public class DummyService : MonitoredBackgroundService
{
    /// <summary>
    /// Initialises a new instance of the <see cref="DummyService"/> class.
    /// </summary>
    /// <param name="healthRegistry"></param>
    public DummyService(IServiceHealthRegistry healthRegistry)
        : base(healthRegistry) { }
}
