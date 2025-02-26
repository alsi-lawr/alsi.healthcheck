// <copyright file="DependencyInjection.cs" company="ALSI">
// Copyright (c) ALSI. All rights reserved.
// </copyright>

namespace ALSI.HealthCheck;

using ALSI.HealthCheck.Context;
using ALSI.HealthCheck.Monitoring;
using ALSI.HealthCheck.Options;
using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Provides extension methods for configuring health check services in the dependency injection container.
/// </summary>
/// <remarks>
/// By default, if no custom HealthCheckOptions are provided, the service uses the following configuration:
/// - Hostname: "*"
/// - Port: 8080
/// - BasePath: "/health".
/// </remarks>
public static class DependencyInjection
{
    /// <summary>
    /// Adds health check services to the service collection using a configuration function for the <see cref="HealthCheckBuilder"/>.
    /// </summary>
    /// <param name="services">The service collection to add the health check services to.</param>
    /// <param name="configureBuilder">
    /// A function that configures the <see cref="HealthCheckBuilder"/> instance.
    /// </param>
    /// <returns>The updated service collection.</returns>
    /// <remarks>
    /// This overload uses the default HealthCheckOptions (Hostname = "*", Port = 8080, BasePath = "/health") when no options are explicitly configured.
    /// </remarks>
    public static IServiceCollection AddHealthChecker(
        this IServiceCollection services,
        Func<HealthCheckBuilder, HealthCheckBuilder> configureBuilder
    ) =>
        services.AddHealthChecker(
            (IServiceProvider _, HealthCheckBuilder builder) => configureBuilder(builder),
            _ => { }
        );

    /// <summary>
    /// Adds health check services to the service collection using configuration functions for both the <see cref="HealthCheckBuilder"/> and the <see cref="HealthCheckOptions"/>.
    /// </summary>
    /// <param name="services">The service collection to add the health check services to.</param>
    /// <param name="configureBuilder">
    /// A function that configures the <see cref="HealthCheckBuilder"/> instance.
    /// </param>
    /// <param name="configureOptions">
    /// An action to configure the <see cref="HealthCheckOptions"/>.
    /// </param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddHealthChecker(
        this IServiceCollection services,
        Func<HealthCheckBuilder, HealthCheckBuilder> configureBuilder,
        Action<HealthCheckOptions> configureOptions
    ) =>
        services.AddHealthChecker(
            (IServiceProvider _, HealthCheckBuilder builder) => configureBuilder(builder),
            configureOptions
        );

    /// <summary>
    /// Adds health check services to the service collection using a configuration function that receives the <see cref="IServiceProvider"/>
    /// along with a <see cref="HealthCheckBuilder"/>, plus an action to configure the <see cref="HealthCheckOptions"/>.
    /// </summary>
    /// <param name="services">The service collection to add the health check services to.</param>
    /// <param name="configureBuilderWithProvider">
    /// A function that configures a <see cref="HealthCheckBuilder"/> instance, with access to the service provider.
    /// </param>
    /// <param name="configureOptions">
    /// An action to configure the <see cref="HealthCheckOptions"/>.
    /// </param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddHealthChecker(
        this IServiceCollection services,
        Func<IServiceProvider, HealthCheckBuilder, HealthCheckBuilder> configureBuilderWithProvider,
        Action<HealthCheckOptions> configureOptions
    ) =>
        services
            .AddSingleton(sp => configureBuilderWithProvider(sp, new HealthCheckBuilder()).Build())
            .AddSingleton<IServiceHealthRegistry, ServiceHealthRegistry>()
            .Configure(configureOptions)
            .AddHostedService<HostedHealthChecker<JsonContextSerializer>>();
}
