// <copyright file="ServiceHealthRegistry.cs" company="ALSI">
// Copyright (c) ALSI. All rights reserved.
// </copyright>

namespace ALSI.HealthCheck.Monitoring;

using System.Collections.Concurrent;
using ALSI.HealthCheck.Context;

internal class ServiceHealthRegistry
    : ConcurrentDictionary<string, HealthStatus>,
        IServiceHealthRegistry
{
    /// <inheritdoc/>
    public void SetStatus(string serviceName, bool isRunning) =>
        this[serviceName] = isRunning ? HealthStatus.Healthy : HealthStatus.Lost;
}
