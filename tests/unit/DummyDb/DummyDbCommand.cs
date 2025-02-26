// <copyright file="DummyDbCommand.cs" company="ALSI">
// Copyright (c) ALSI. All rights reserved.
// </copyright>

namespace ALSI.HealthCheck.UnitTests.DummyDb;

using System.Data;
using System.Data.Common;

public class DummyDbCommand : DbCommand
{
    /// <inheritdoc/>
    public override string CommandText { get; set; } = string.Empty;

    /// <inheritdoc/>
    public override int CommandTimeout { get; set; }

    /// <inheritdoc/>
    public override CommandType CommandType { get; set; }

    /// <inheritdoc/>
    protected override DbConnection DbConnection { get; set; } =
        new DummyDbConnection<NothingOnDbOpen>();

    /// <inheritdoc/>
    protected override DbParameterCollection DbParameterCollection { get; } =
        new DummyDbParameterCollection();

    /// <inheritdoc/>
    protected override DbTransaction DbTransaction { get; set; } = null!;

    /// <inheritdoc/>
    public override bool DesignTimeVisible { get; set; }

    /// <inheritdoc/>
    public override UpdateRowSource UpdatedRowSource { get; set; }

    /// <inheritdoc/>
    public override void Cancel() { }

    /// <inheritdoc/>
    public override int ExecuteNonQuery() => 1;

    /// <inheritdoc/>
    public override object ExecuteScalar() => 1;

    /// <inheritdoc/>
    public override void Prepare() { }

    /// <inheritdoc/>
    protected override DbParameter CreateDbParameter() => new DummyDbParameter();

    /// <inheritdoc/>
    protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior) => null!;
}
