// <copyright file="DummyDbParameter.cs" company="ALSI">
// Copyright (c) ALSI. All rights reserved.
// </copyright>

namespace ALSI.HealthCheck.UnitTests.DummyDb;

using System.Data;
using System.Data.Common;

public class DummyDbParameter : DbParameter
{
    /// <inheritdoc/>
    public override DbType DbType { get; set; }

    /// <inheritdoc/>
    public override ParameterDirection Direction { get; set; }

    /// <inheritdoc/>
    public override bool IsNullable { get; set; }

    /// <inheritdoc/>
    public override string ParameterName { get; set; } = string.Empty;

    /// <inheritdoc/>
    public override string SourceColumn { get; set; } = string.Empty;

    /// <inheritdoc/>
    public override object? Value { get; set; }

    /// <inheritdoc/>
    public override bool SourceColumnNullMapping { get; set; }

    /// <inheritdoc/>
    public override int Size { get; set; }

    /// <inheritdoc/>
    public override void ResetDbType() { }
}
