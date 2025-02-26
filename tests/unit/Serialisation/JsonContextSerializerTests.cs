// <copyright file="JsonContextSerializerTests.cs" company="ALSI">
// Copyright (c) ALSI. All rights reserved.
// </copyright>

namespace ALSI.HealthCheck.UnitTests.Serialisation;

using System;
using System.Collections.Immutable;
using System.Net;
using System.Text;
using System.Text.Json;
using ALSI.HealthCheck.Context;
using Shouldly;
using Xunit;

public class JsonContextSerializerTests
{
    [Fact]
    public void Healthy_Should_Serialize_Correctly()
    {
        // Arrange
        var results = ImmutableDictionary<string, HealthStatus>.Empty.Add(
            "TestCheck",
            HealthStatus.Healthy
        );
        var since = TimeSpan.FromSeconds(10);

        // Act
        HealthResponse response = JsonContextSerializer.Healthy(results, since);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        string json = Encoding.UTF8.GetString(response.Body);
        using var document = JsonDocument.Parse(json);
        var root = document.RootElement;
        root.GetProperty("Status").GetString().ShouldBe("Healthy");
        root.GetProperty("Since").GetString().ShouldBe(since.ToString());
        root.GetProperty("Results").TryGetProperty("TestCheck", out var checkStatus).ShouldBeTrue();
        checkStatus.GetString().ShouldBe(HealthStatus.Healthy.ToString());
    }

    [Fact]
    public void Unhealthy_Should_Serialize_Correctly()
    {
        // Arrange
        var results = ImmutableDictionary<string, HealthStatus>.Empty.Add(
            "TestCheck",
            HealthStatus.Unhealthy
        );
        var since = TimeSpan.FromSeconds(20);

        // Act
        HealthResponse response = JsonContextSerializer.Unhealthy(results, since);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Conflict);
        string json = Encoding.UTF8.GetString(response.Body);
        using var document = JsonDocument.Parse(json);
        var root = document.RootElement;
        root.GetProperty("Status").GetString().ShouldBe("Unhealthy");
        root.GetProperty("Since").GetString().ShouldBe(since.ToString());
        root.GetProperty("Results").TryGetProperty("TestCheck", out var checkStatus).ShouldBeTrue();
        checkStatus.GetString().ShouldBe(HealthStatus.Unhealthy.ToString());
    }
}
