// <copyright file="ApiHealthCheck.cs" company="ALSI">
// Copyright (c) ALSI. All rights reserved.
// </copyright>

namespace ALSI.HealthCheck.UnitTests.Checks;

using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ALSI.HealthCheck.Context;
using ALSI.HealthCheck.Monitoring;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

public class ApiHealthCheckTests
{
    /// <summary>
    ///
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
    [Fact]
    public async Task Create_Should_Return_Healthy_Status_When_ResponseMatchesExpected()
    {
        // Arrange
        Func<HttpClient> clientFactory = () => CreateHttpClient(HttpStatusCode.OK);
        string requestUri = "http://dummyapi.com/health";
        var healthCheck = ApiHealthCheck.Create("ApiTest", clientFactory, requestUri);
        var dummyRegistry = new ServiceHealthRegistry();
        var context = new HealthCheckContext(dummyRegistry, CancellationToken.None)
        {
            State = new HealthState(DateTime.UtcNow, HealthStatus.Healthy),
        };
        IServiceProvider serviceProvider = new ServiceCollection().BuildServiceProvider();

        // Act
        HealthStatus result = await healthCheck.Check(serviceProvider, context);

        // Assert
        result.ShouldBe(HealthStatus.Healthy);
    }

    /// <summary>
    ///
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
    [Fact]
    public async Task Create_Should_Return_Unhealthy_Status_When_ResponseDoesNotMatchExpected()
    {
        // Arrange
        // Simulate an endpoint that returns 404 instead of 200.
        Func<HttpClient> clientFactory = () => CreateHttpClient(HttpStatusCode.NotFound);
        string requestUri = "http://dummyapi.com/health";
        var healthCheck = ApiHealthCheck.Create("ApiTest", clientFactory, requestUri);
        var dummyRegistry = new ServiceHealthRegistry();
        var context = new HealthCheckContext(dummyRegistry, CancellationToken.None)
        {
            State = new HealthState(DateTime.UtcNow, HealthStatus.Healthy),
        };
        IServiceProvider serviceProvider = new ServiceCollection().BuildServiceProvider();

        // Act
        HealthStatus result = await healthCheck.Check(serviceProvider, context);

        // Assert
        result.ShouldBe(HealthStatus.Unhealthy);
    }

    /// <summary>
    ///
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
    [Fact]
    public async Task Create_Should_Return_Unhealthy_Status_When_ExceptionOccurs()
    {
        // Arrange
        // Simulate an exception during the API call.
        Func<HttpClient> clientFactory = () =>
            new HttpClient(
                new FakeHttpMessageHandler(
                    (req, token) =>
                    {
                        throw new HttpRequestException("Simulated failure");
                    }
                )
            );
        string requestUri = "http://dummyapi.com/health";
        var healthCheck = ApiHealthCheck.Create("ApiTest", clientFactory, requestUri);
        var dummyRegistry = new ServiceHealthRegistry();
        var context = new HealthCheckContext(dummyRegistry, CancellationToken.None)
        {
            State = new HealthState(DateTime.UtcNow, HealthStatus.Healthy),
        };
        IServiceProvider serviceProvider = new ServiceCollection().BuildServiceProvider();

        // Act
        HealthStatus result = await healthCheck.Check(serviceProvider, context);

        // Assert
        result.ShouldBe(HealthStatus.Unhealthy);
    }

    private static HttpClient CreateHttpClient(HttpStatusCode statusCode)
    {
        var handler = new FakeHttpMessageHandler(
            (request, cancellationToken) =>
            {
                var response = new HttpResponseMessage(statusCode);
                return Task.FromResult(response);
            }
        );
        return new HttpClient(handler);
    }
}

// Simple fake HttpMessageHandler to simulate HTTP responses.
public class FakeHttpMessageHandler : HttpMessageHandler
{
    private readonly Func<
        HttpRequestMessage,
        CancellationToken,
        Task<HttpResponseMessage>
    > _sendAsync;

    /// <summary>
    /// Initialises a new instance of the <see cref="FakeHttpMessageHandler"/> class.
    /// </summary>
    /// <param name="sendAsync"></param>
    public FakeHttpMessageHandler(
        Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> sendAsync
    )
    {
        _sendAsync = sendAsync;
    }

    /// <inheritdoc/>
    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken
    )
    {
        return _sendAsync(request, cancellationToken);
    }
}
