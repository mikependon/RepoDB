// Copyright (c) 2019 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

using System.Data;
using System.Data.Common;

namespace RepoDb.UnitTests.CustomObjects
{
    public class CustomDbTransaction : DbTransaction, IDbTransaction
    {
        public override IsolationLevel IsolationLevel { get; }

        protected override DbConnection DbConnection { get; }

        public override void Commit()
        {
            /* do nothing */
        }

        public new void Dispose()
        {
            /* do nothing */
        }

        public override void Rollback()
        {
            /* do nothing */
        }
    }
}
