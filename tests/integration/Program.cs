// <copyright file="Program.cs" company="ALSI">
// Copyright (c) ALSI. All rights reserved.
// </copyright>

using ALSI.HealthCheck;
using ALSI.HealthCheck.Integration;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddLogging();
builder.Services.AddHostedService<Worker>();

builder.Services.AddHealthChecker(
    builder =>
        builder
            .AddMonitoredService<Worker>()
            .AddMonitoredApiRoute(
                "Failing api",
                () => new HttpClient(),
                "https://completelymadeup.co.uk/"
            ),
    opts =>
    {
        opts.Hostname = "localhost";
    }
);
var host = builder.Build();
host.Run();
