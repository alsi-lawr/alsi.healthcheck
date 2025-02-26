// <copyright file="IServiceHealthRegistry.cs" company="ALSI">
// Copyright (c) ALSI. All rights reserved.
// </copyright>

namespace ALSI.HealthCheck.Monitoring;

using ALSI.HealthCheck.Context;

/// <summary>
/// The registry for the health of monitored services.
/// </summary>
public interface IServiceHealthRegistry : IDictionary<string, HealthStatus>
{
    internal void SetStatus(string serviceName, bool isRunning);
}
