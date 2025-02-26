// <copyright file="DummyDbConnection.cs" company="ALSI">
// Copyright (c) ALSI. All rights reserved.
// </copyright>

namespace ALSI.HealthCheck.UnitTests.DummyDb;

using System.Data;
using System.Data.Common;

internal interface IOnDbOpen
{
    internal static abstract void OnOpen();
}

internal record struct ErrorOnDbOpen : IOnDbOpen
{
    static void IOnDbOpen.OnOpen()
    {
        throw new NotImplementedException();
    }
}

internal record struct NothingOnDbOpen : IOnDbOpen
{
    static void IOnDbOpen.OnOpen() { }
}

internal class DummyDbConnection<TOnDbOpen> : DbConnection
    where TOnDbOpen : IOnDbOpen
{
    /// <inheritdoc/>
    public override string ConnectionString { get; set; } = string.Empty;

    /// <inheritdoc/>
    public override string Database => "Dummy";

    /// <inheritdoc/>
    public override string DataSource => "Dummy";

    /// <inheritdoc/>
    public override string ServerVersion => "1.0";

    /// <inheritdoc/>
    public override ConnectionState State => ConnectionState.Closed;

    /// <inheritdoc/>
    public override void ChangeDatabase(string databaseName) { }

    /// <inheritdoc/>
    public override void Close() { }

    /// <inheritdoc/>
    public override void Open() => TOnDbOpen.OnOpen();

    /// <inheritdoc/>
    protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel) => null!;

    /// <inheritdoc/>
    protected override DbCommand CreateDbCommand() => new DummyDbCommand();
}
