// Copyright (c) 2018 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

using System.Data;
using System.Data.Common;

namespace RepoDb.UnitTests.CustomObjects
{
    public class CustomDbConnection : DbConnection
    {
        public override string ConnectionString { get; set; }
        public override string Database { get; }
        public override string DataSource { get; }
        public override string ServerVersion { get; }
        public override ConnectionState State { get; }
        public override void ChangeDatabase(string databaseName)
        {
        }
        public override void Close()
        {
        }
        public override void Open()
        {
        }
        protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel)
        {
            return new CustomDbTransaction();
        }
        protected override DbCommand CreateDbCommand()
        {
            return new CustomDbCommand();
        }
    }
}
