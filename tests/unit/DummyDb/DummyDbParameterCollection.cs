// <copyright file="DummyDbParameterCollection.cs" company="ALSI">
// Copyright (c) ALSI. All rights reserved.
// </copyright>

namespace ALSI.HealthCheck.UnitTests.DummyDb;

using System;
using System.Collections;
using System.Data.Common;

public class DummyDbParameterCollection : DbParameterCollection
{
    /// <inheritdoc/>
    public override int Count => 0;

    /// <inheritdoc/>
    public override object SyncRoot => new();

    /// <inheritdoc/>
    public override int Add(object value) => 0;

    /// <inheritdoc/>
    public override void AddRange(Array values) { }

    /// <inheritdoc/>
    public override void Clear() { }

    /// <inheritdoc/>
    public override bool Contains(object value) => false;

    /// <inheritdoc/>
    public override bool Contains(string value) => false;

    /// <inheritdoc/>
    public override void CopyTo(Array array, int index) { }

    /// <inheritdoc/>
    public override IEnumerator GetEnumerator() =>
        (IEnumerator<object>)Array.Empty<object>().GetEnumerator();

    /// <inheritdoc/>
    public override int IndexOf(object value) => -1;

    /// <inheritdoc/>
    public override int IndexOf(string parameterName) => -1;

    /// <inheritdoc/>
    public override void Insert(int index, object value) { }

    /// <inheritdoc/>
    public override void Remove(object value) { }

    /// <inheritdoc/>
    public override void RemoveAt(int index) { }

    /// <inheritdoc/>
    public override void RemoveAt(string parameterName) { }

    /// <inheritdoc/>
    protected override DbParameter GetParameter(int index) => new DummyDbParameter();

    /// <inheritdoc/>
    protected override DbParameter GetParameter(string parameterName) => new DummyDbParameter();

    /// <inheritdoc/>
    protected override void SetParameter(int index, DbParameter value) { }

    /// <inheritdoc/>
    protected override void SetParameter(string parameterName, DbParameter value) { }
}
