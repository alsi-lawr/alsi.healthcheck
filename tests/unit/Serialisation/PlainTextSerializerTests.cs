// <copyright file="PlainTextSerializerTests.cs" company="ALSI">
// Copyright (c) ALSI. All rights reserved.
// </copyright>

namespace ALSI.HealthCheck.UnitTests.Serialisation;

using System;
using System.Collections.Immutable;
using System.Net;
using System.Text;
using ALSI.HealthCheck.Context;
using Shouldly;
using Xunit;

public class PlainContextSerializerTests
{
    [Fact]
    public void Healthy_Should_Return_CorrectPlainTextResponse()
    {
        // Arrange
        var results = ImmutableDictionary<string, HealthStatus>.Empty.Add(
            "TestCheck",
            HealthStatus.Healthy
        );
        var since = TimeSpan.FromSeconds(10);

        // Act
        HealthResponse response = PlainContextSerializer.Healthy(results, since);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        string body = Encoding.UTF8.GetString(response.Body);
        body.ShouldBe($"Healthy since {since}");
    }

    [Fact]
    public void Unhealthy_Should_Return_CorrectPlainTextResponse()
    {
        // Arrange
        var results = ImmutableDictionary<string, HealthStatus>.Empty.Add(
            "TestCheck",
            HealthStatus.Unhealthy
        );
        var since = TimeSpan.FromSeconds(20);

        // Act
        HealthResponse response = PlainContextSerializer.Unhealthy(results, since);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Conflict);
        string body = Encoding.UTF8.GetString(response.Body);
        body.ShouldBe($"Unhealthy since {since}");
    }
}
