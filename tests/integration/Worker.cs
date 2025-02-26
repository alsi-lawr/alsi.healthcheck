// <copyright file="Worker.cs" company="ALSI">
// Copyright (c) ALSI. All rights reserved.
// </copyright>

namespace ALSI.HealthCheck.Integration;

using ALSI.HealthCheck.Monitoring;

public class Worker : MonitoredBackgroundService
{
    private readonly ILogger<Worker> _logger;

    /// <summary>
    /// Initialises a new instance of the <see cref="Worker"/> class.
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="registry"></param>
    public Worker(ILogger<Worker> logger, IServiceHealthRegistry registry)
        : base(registry)
    {
        _logger = logger;
    }

    /// <inheritdoc/>
    protected override async Task OnExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            }

            await Task.Delay(1000, stoppingToken);
            break;
        }
    }
}
