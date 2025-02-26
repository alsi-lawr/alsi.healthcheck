// <copyright file="HostedHealthChecker.cs" company="ALSI">
// Copyright (c) ALSI. All rights reserved.
// </copyright>

namespace ALSI.HealthCheck;

using System;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ALSI.HealthCheck.Context;
using ALSI.HealthCheck.Monitoring;
using ALSI.HealthCheck.Options;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

/// <inheritdoc/>
internal class HostedHealthChecker<TContextSerializer> : BackgroundService
#if !NET6_0_OR_GREATER
    where TContextSerializer : IContextSerializer, new()
#else
    where TContextSerializer : IContextSerializer
#endif
{
    private readonly ILogger<HostedHealthChecker<TContextSerializer>> _logger;
    private readonly IServiceHealthRegistry _healthRegistry;
    private readonly HealthChecks _healthChecks;
    private readonly HealthCheckOptions _config;
    private HealthState _healthState = new(DateTime.Now, HealthStatus.Unknown);

    /// <summary>
    /// Initialises a new instance of the <see cref="HostedHealthChecker{TContextSerializer}"/> class.
    /// </summary>
    /// <param name="logger">Logger.</param>
    /// <param name="healthRegistry">The service health registry.</param>
    /// <param name="healthChecks">Health checks.</param>
    /// <param name="config">Configuration.</param>
    public HostedHealthChecker(
        ILogger<HostedHealthChecker<TContextSerializer>> logger,
        IServiceHealthRegistry healthRegistry,
        HealthChecks healthChecks,
        IOptions<HealthCheckOptions> config
    )
    {
        _logger = logger;
        _healthRegistry = healthRegistry;
        _healthChecks = healthChecks;
        _config = config.Value;
    }

    /// <inheritdoc/>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            HttpListener listener = CreateListener(_config);
            var cancelTask = Task.Delay(Timeout.Infinite, stoppingToken);
            while (!stoppingToken.IsCancellationRequested)
            {
                var httpContextTask = listener.GetContextAsync();
                var completionTask = await Task.WhenAny(httpContextTask, cancelTask)
                    .ConfigureAwait(false);
                if (completionTask == cancelTask)
                {
                    listener.Stop();
                    listener.Close();
                    break;
                }

                _ = ProcessHealthCheck(httpContextTask.Result!, stoppingToken)
                    .ConfigureAwait(false);
            }
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Health check failed to start");
        }
    }

    private async Task ProcessHealthCheck(
        HttpListenerContext client,
        CancellationToken stoppingToken
    )
    {
        var request = client.Request;
        using var response = client.Response;
        if (
            !request.HttpMethod.Equals("GET", StringComparison.InvariantCultureIgnoreCase)
            || (
                !request.Url?.PathAndQuery.Equals(
                    _config.BasePath,
                    StringComparison.InvariantCultureIgnoreCase
                ) ?? true
            )
        )
        {
            response.StatusCode = 404;
            return;
        }

        var ctx = new HealthCheckContext(_healthRegistry, stoppingToken);
        foreach (var check in _healthChecks)
        {
            var status = await check.Check(ctx);
            ctx = ctx.SetItem(check.Name, status);
        }

        _healthState =
            ctx.HasAnyUnhealthyOrLost && _healthState.Status != HealthStatus.Unhealthy
                ? _healthState = new(DateTime.Now, HealthStatus.Unhealthy)
            : !ctx.HasAnyUnhealthyOrLost && _healthState.Status != HealthStatus.Healthy
                ? _healthState = _healthState with
                {
                    StartTime = DateTime.Now,
                    Status = HealthStatus.Healthy,
                }
            : _healthState;

        ctx = ctx with { State = _healthState };

        response.ContentEncoding = Encoding.UTF8;

#if !NET6_0_OR_GREATER
        response.ContentType = new TContextSerializer().ContentType;
#else
        response.ContentType = TContextSerializer.ContentType;
#endif

        var result = ctx.Serialize<TContextSerializer>();
        response.StatusCode = (int)result.StatusCode;
        response.ContentLength64 = result.Body.LongLength;
        await response
            .OutputStream.WriteAsync(result.Body, 0, result.Body.Length, stoppingToken)
            .ConfigureAwait(false);
    }

    private static HttpListener CreateListener(HealthCheckOptions config)
    {
        HttpListener listener = new();
        listener.Prefixes.Add($"http://{config.Hostname}:{config.Port}/");
        listener.Start();
        return listener;
    }
}
