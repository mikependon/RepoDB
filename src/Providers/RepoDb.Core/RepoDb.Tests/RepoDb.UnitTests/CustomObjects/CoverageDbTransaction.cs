#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System.Data.Common;
using System.Data;

namespace RepoDb.UnitTests.CustomObjects
{
    public sealed class CoverageDbTransaction : DbTransaction
    {
        private readonly DbConnection connection;

        public CoverageDbTransaction(DbConnection connection, IsolationLevel isolationLevel)
        {
            this.connection = connection;
            IsolationLevel = isolationLevel;
        }

        public override IsolationLevel IsolationLevel { get; }

        protected override DbConnection DbConnection => connection;

        public override void Commit() { }

        public override void Rollback() { }
    }
}
