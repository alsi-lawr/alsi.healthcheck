// <copyright file="MonitoredBackgroundService.cs" company="ALSI">
// Copyright (c) ALSI. All rights reserved.
// </copyright>

namespace ALSI.HealthCheck.Monitoring;

using Microsoft.Extensions.Hosting;

/// <summary>
/// Represents a background service whose running status is automatically monitored and recorded.
/// </summary>
/// <remarks>
/// Derived services automatically register their status in an <see cref="IServiceHealthRegistry"/>.
/// Override <see cref="OnStartAsync"/> and <see cref="OnStopAsync"/> to add custom startup or shutdown logic
/// without interfering with the health status registration. The default <see cref="ServiceName"/>
/// is set to the runtime type name but can be overridden if needed.
/// </remarks>
public abstract class MonitoredBackgroundService : BackgroundService
{
    private readonly IServiceHealthRegistry _healthRegistry;

    /// <summary>
    /// Gets the name of the monitored service.
    /// </summary>
    /// <remarks>
    /// Defaults to the runtime type name. Override this property to specify a custom service name.
    /// </remarks>
    protected virtual string ServiceName => GetType().Name;

    /// <summary>
    /// Initialises a new instance of the <see cref="MonitoredBackgroundService"/> class.
    /// </summary>
    /// <param name="healthRegistry">The service health registry used to track the service's status.</param>
    public MonitoredBackgroundService(IServiceHealthRegistry healthRegistry)
    {
        _healthRegistry = healthRegistry;
    }

    /// <summary>
    /// Starts the service and registers its status as running.
    /// </summary>
    /// <param name="cancellationToken">A token that can signal the operation should be canceled.</param>
    /// <returns>A task representing the asynchronous start operation.</returns>
    /// <remarks>
    /// This method is sealed to ensure that the service health status is always updated.
    /// Override <see cref="OnStartAsync"/> to execute additional startup logic.
    /// </remarks>
    public sealed override async Task StartAsync(CancellationToken cancellationToken)
    {
        _healthRegistry.SetStatus(ServiceName, true);
        await OnStartAsync(cancellationToken);
        await base.StartAsync(cancellationToken);
    }

    /// <summary>
    /// Stops the service and updates its status to indicate it is no longer running.
    /// </summary>
    /// <param name="cancellationToken">A token that can signal the operation should be canceled.</param>
    /// <returns>A task representing the asynchronous stop operation.</returns>
    /// <remarks>
    /// This method is sealed to ensure that the service health status is consistently updated.
    /// Override <see cref="OnStopAsync"/> to execute additional shutdown logic.
    /// </remarks>
    public sealed override async Task StopAsync(CancellationToken cancellationToken)
    {
        _healthRegistry.SetStatus(ServiceName, false);
        await OnStopAsync(cancellationToken);
        await base.StopAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public override void Dispose()
    {
        _healthRegistry.SetStatus(ServiceName, false);
        base.Dispose();
    }

    /// <inheritdoc/>
    protected sealed override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        _healthRegistry.SetStatus(ServiceName, false);
        await OnExecuteAsync(cancellationToken);
    }

    /// <summary>
    /// Executes additional startup tasks for the service.
    /// </summary>
    /// <param name="cancellationToken">A token that can signal the operation should be canceled.</param>
    /// <returns>A task representing any custom startup operations.</returns>
    /// <remarks>
    /// Override this method to add startup logic while preserving the automatic health registration.
    /// </remarks>
    protected virtual Task OnStartAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    /// <summary>
    /// Executes additional shutdown tasks for the service.
    /// </summary>
    /// <param name="cancellationToken">A token that can signal the operation should be canceled.</param>
    /// <returns>A task representing any custom shutdown operations.</returns>
    /// <remarks>
    /// Override this method to add shutdown logic while preserving the automatic health update.
    /// </remarks>
    protected virtual Task OnStopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    /// <summary>
    /// Executes the tasks for the service.
    /// </summary>
    /// <param name="cancellationToken">A token that can signal the operation should be canceled.</param>
    /// <returns>A task representing the background service operations.</returns>
    /// <remarks>
    /// Override this method to add tasks to the service.
    /// </remarks>
    protected virtual Task OnExecuteAsync(CancellationToken cancellationToken) =>
        Task.CompletedTask;
}
