#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System.Data;
using System.Data.Common;
using DbConnectionBase = System.Data.Common.DbConnection;

namespace RepoDb.Data.Core.UnitTests.CustomObjects
{
    public class FakeDbTransaction : DbTransaction
    {
        private readonly DbConnectionBase _connection;

        public FakeDbTransaction(DbConnectionBase connection = null)
        {
            _connection = connection;
        }

        public override IsolationLevel IsolationLevel { get; }

        protected override DbConnectionBase DbConnection => _connection;

        public override void Commit()
        {
        }

        public override void Rollback()
        {
        }
    }
}
