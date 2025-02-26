// <copyright file="HealthCheckContextTests.cs" company="ALSI">
// Copyright (c) ALSI. All rights reserved.
// </copyright>

namespace ALSI.HealthCheck.UnitTests.Context;

using System;
using System.Net;
using System.Text;
using System.Threading;
using ALSI.HealthCheck.Context;
using ALSI.HealthCheck.Monitoring;
using Shouldly;
using Xunit;

public class HealthCheckContextTests
{
    [Fact]
    public void SetItem_Should_AddOrUpdate_HealthCheckResult()
    {
        // Arrange
        var registry = new ServiceHealthRegistry();
        var token = CancellationToken.None;
        var initialState = new HealthState(DateTime.UtcNow, HealthStatus.Healthy);
        var context = new HealthCheckContext(registry, token) { State = initialState };

        // Act
        var updatedContext = context.SetItem("TestCheck", HealthStatus.Unhealthy);

        // Assert: Since Unhealthy is present, HasAnyUnhealthyOrLost should be true.
        updatedContext.HasAnyUnhealthyOrLost.ShouldBeTrue();
    }

    [Fact]
    public void GetServiceStatus_Should_Return_Status_From_Registry_OrDefaultToLost()
    {
        // Arrange
        var registry = new ServiceHealthRegistry();

        // Set DummyService status to Healthy.
        registry.SetStatus(nameof(DummyService), true);
        var token = CancellationToken.None;
        var context = new HealthCheckContext(registry, token)
        {
            State = new HealthState(DateTime.UtcNow, HealthStatus.Healthy),
        };

        // Act
        var statusForDummy = context.GetServiceStatus<DummyService>();
        var statusForNonexistent = context.GetServiceStatus<HealthCheckContextTests>();

        // Assert
        statusForDummy.ShouldBe(HealthStatus.Healthy);
        statusForNonexistent.ShouldBe(HealthStatus.Lost);
    }

    [Fact]
    public void Serialize_Should_ReturnHealthyResponse_When_NoProblems()
    {
        // Arrange
        var registry = new ServiceHealthRegistry();
        var token = CancellationToken.None;
        DateTime stateSince = DateTime.UtcNow;
        var context = new HealthCheckContext(registry, token)
        {
            State = new HealthState(stateSince, HealthStatus.Healthy),
        };

        var response = context.Serialize<PlainContextSerializer>();

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        Encoding.UTF8.GetString(response.Body).ShouldContain("Healthy");
    }

    [Fact]
    public void Serialize_Should_ReturnUnhealthyResponse_When_ProblemExists()
    {
        // Arrange
        var registry = new ServiceHealthRegistry();
        var token = CancellationToken.None;
        DateTime stateSince = DateTime.UtcNow;
        var context = new HealthCheckContext(registry, token)
        {
            State = new HealthState(stateSince, HealthStatus.Healthy),
        };

        context = context.SetItem("TestCheck", HealthStatus.Unhealthy);
        var response = context.Serialize<PlainContextSerializer>();
        var responseBody = Encoding.UTF8.GetString(response.Body);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Conflict);
        responseBody.ShouldContain("Unhealthy");
    }
}
