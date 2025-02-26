[![NuGet Version](https://img.shields.io/nuget/v/ALSI.HealthCheck.svg?style=flat)](https://www.nuget.org/packages/ALSI.HealthCheck/)
[![Build Status](https://github.com/alsi-lawr/alsi.healthcheck/actions/workflows/deploy-nuget.yml/badge.svg)](https://github.com/alsi-lawr/ALSI.HealthCheck/actions)
[![Downloads](https://img.shields.io/nuget/dt/ALSI.HealthCheck.svg?logo=nuget&logoSize=auto)](https://www.nuget.org/packages/ALSI.HealthCheck)
[![codecov](https://codecov.io/gh/alsi-lawr/alsi.healthcheck/graph/badge.svg)](https://codecov.io/gh/alsi-lawr/alsi.healthcheck)

**ALSI.HealthCheck** is a lightweight, modular health check library for .NET applications. It provides a fluent, builder-based API to register health checks for external APIs, databases, and custom background services. The library includes a hosted background service that exposes a configurable HTTP endpoint to collate and report health check results. Out-of-the-box, it supports multiple response formats via context serializers (JSON, plain text, etc.) and is fully configurable using dependency injection and strongly typed options.

## Features

- **Modular Health Checks:** Easily register checks for APIs, database connections, and custom services.
- **Fluent Builder API:** Use a fluent builder pattern to configure and combine multiple health checks.
- **Hosted Service:** A background service (`HostedHealthChecker`) listens on an HTTP endpoint and collates health check results.
- **Flexible Serialisation:** Supports multiple output formats via implementations of the `IContextSerializer` interface (e.g. JSON and plain text).
- **Strong Configuration:** Configure options via `IOptions<T>` using the `HealthCheckOptions` record.
- **Extensibility:** Easily add custom health checks, providers, and serializers.

## Installation

Install the package via NuGet:

```bash
dotnet add package ALSI.HealthCheck
```

Alternatively, search for **ALSI.HealthCheck** in your NuGet Package Manager.

## Configuration

The health check endpoint is configured using the `HealthCheckOptions` record. By default, the configuration is:

- **Hostname:** `"*"` (binds to all network interfaces)
- **Port:** `8080`
- **UrlPath:** `"/health"` (the base path for health checks)

You can override these defaults via your configuration file (e.g., `appsettings.json`):

```json
{
  "HealthCheck": {
    "Hostname": "localhost",
    "Port": 8081,
    "UrlPath": "/health"
  }
}
```

And register them in your DI container:

```csharp
builder.Services.Configure<HealthCheckOptions>(builder.Configuration.GetSection("HealthCheck"));
```

## Usage

### Registering Health Checks

Use the extension methods provided by the library to register your health checks. For example, in your `Program.cs`:

```csharp
using ALSI.HealthCheck;
using ALSI.HealthCheck.Context;

var builder = Host.CreateApplicationBuilder(args);

// Register your health checks using the fluent builder API.
builder.Services.AddHealthChecker(healthBuilder =>
    healthBuilder
        .AddCheck("Basic Flip Flop", ctx =>
            Task.FromResult(/* some condition */ HealthStatus.Healthy))
        .AddCheck("AlwaysHealthy", _ =>
            Task.FromResult(HealthStatus.Healthy))
        .AddMonitoredService<YourMonitoredService>()
        .AddMonitoredDatabaseConnection(
            "DatabaseCheck",
            () => new YourDbConnection(),  // Your connection factory
            "SELECT 1;"
        )
        .AddMonitoredApiRoute(
            "ApiCheck",
            () => new HttpClient(),
            "http://example.com/api/health",
            httpMethod: HttpMethod.Get,
            expectedStatusCode: HttpStatusCode.OK
        )
);

// Register the hosted health checker using your preferred context serializer.
builder.Services.AddHostedService<HostedHealthChecker<JsonContextSerializer>>();

var host = builder.Build();
host.Run();
```

### Hosted Health Checker

The `HostedHealthChecker<TContextSerializer>` is a background service that:
- Listens on an HTTP endpoint (configured by `HealthCheckOptions`).
- Executes all registered health checks.
- Collates their statuses and serializes the results using the specified serializer (e.g. `JsonContextSerializer` or `PlainContextSerializer`).

When a GET request is made to the health check URL (e.g., `http://localhost:8081/health`), the service runs the checks and returns a response similar to:

```json
{
  "Status": "Healthy",
  "Since": "00:00:06.8265131",
  "Results": {
    "Basic Flip Flop": "Healthy",
    "AlwaysHealthy": "Healthy",
    "DatabaseCheck": "Healthy",
    "ApiCheck": "Healthy"
  }
}
```

### Context Serializers

The package includes implementations of `IContextSerializer`:

- **JsonContextSerializer:** Returns detailed structured JSON output.
- **PlainContextSerializer:** Returns a simple plain text response.

You can also implement your own serializer by implementing `IContextSerializer` if you require a custom output format.

## Extensibility

ALSI.HealthCheck is designed to be extensible:
- **Custom Health Checks:** Define your own health check functions using the signature `Func<HealthCheckContext, Task<HealthStatus>>` and register them via the builder.
- **Custom Providers:** Add health checks for any dependency by creating a factory method that returns a `HealthCheck`.
- **Custom Serializers:** Implement the `IContextSerializer` interface to create a custom serialization strategy.

## Running and Monitoring

After configuring your health checks and registering the hosted service, run your host application. The health check endpoint will be available at the URL formed by combining the `Hostname`, `Port`, and `UrlPath` (e.g., `http://localhost:8081/health`). You can query this endpoint using a web browser or tools like `curl` or Postman.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

## Contributing

Contributions are welcome! If you have suggestions or bug fixes, please submit a pull request or open an issue on GitHub.

