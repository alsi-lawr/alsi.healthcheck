// <copyright file="HealthStatus.cs" company="ALSI">
// Copyright (c) ALSI. All rights reserved.
// </copyright>

namespace ALSI.HealthCheck.Context;

/// <summary>
/// The status of the health check.
/// </summary>
[Flags]
public enum HealthStatus
{
    Unknown = 0,
    Healthy = 1 << 0,
    Unhealthy = 1 << 1,
    Recoverable = 1 << 2,
    Lost = 1 << 3,
}
