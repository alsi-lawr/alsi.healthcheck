// <copyright file="HealthResponse.cs" company="ALSI">
// Copyright (c) ALSI. All rights reserved.
// </copyright>

namespace ALSI.HealthCheck.Context;

using System.Net;

/// <summary>
/// Health response.
/// </summary>
/// <param name="Body">Body of the health response.</param>
/// <param name="StatusCode">The http status code.</param>
public record HealthResponse(byte[] Body, HttpStatusCode StatusCode)
{
    public static implicit operator HealthResponse(Tuple<byte[], HttpStatusCode> tuple) => tuple;
}
