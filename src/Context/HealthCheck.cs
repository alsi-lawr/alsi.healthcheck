// <copyright file="HealthCheck.cs" company="ALSI">
// Copyright (c) ALSI. All rights reserved.
// </copyright>

namespace ALSI.HealthCheck.Context;

using System;

/// <summary>
/// Represents a health check with a unique name and an asynchronous function
/// that evaluates the health status based on a given <see cref="HealthCheckContext"/>.
/// </summary>
public record HealthCheck(string Name, Func<HealthCheckContext, Task<HealthStatus>> Check);

/// <summary>
/// Represents a strongly-typed health check associated with a specific provider or component type.
/// This generic variant inherits from <see cref="HealthCheck"/> to provide additional type safety.
/// </summary>
/// <typeparam name="T">
/// The type of the provider or component for which this health check is performed.
/// </typeparam>
public record HealthCheck<T> : HealthCheck
{
    /// <summary>
    /// Initialises a new instance of the <see cref="HealthCheck{T}"/> class.
    /// Initializes a new instance of the <see cref="HealthCheck{T}"/> record with the specified name
    /// and health check function.
    /// </summary>
    /// <param name="name">A unique identifier for this health check.</param>
    /// <param name="check">
    /// An asynchronous function that accepts a <see cref="HealthCheckContext"/> and returns a <see cref="HealthStatus"/>.
    /// </param>
    /// <remarks>
    /// This constructor is internal, ensuring that instances of <see cref="HealthCheck{T}"/> can only be created
    /// within the assembly, thereby enforcing controlled instantiation.
    /// </remarks>
    internal HealthCheck(string name, Func<HealthCheckContext, Task<HealthStatus>> check)
        : base(name, check) { }
}
