// <copyright file="HealthState.cs" company="ALSI">
// Copyright (c) ALSI. All rights reserved.
// </copyright>

namespace ALSI.HealthCheck.Context;

using System;

internal record HealthState(DateTime StartTime, HealthStatus Status)
{
    public TimeSpan Since => DateTime.Now - StartTime;
}
