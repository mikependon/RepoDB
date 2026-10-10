#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System.Data;
using System.Data.Common;
using DbConnectionBase = System.Data.Common.DbConnection;
using System.Linq;

namespace RepoDb.Data.Core.UnitTests.CustomObjects
{
    public class FakeDbCommand : DbCommand
    {
        public FakeDbCommand(FakeDbConnection connection)
        {
            DbConnection = connection;
            DbParameterCollection = new FakeDbParameterCollection();
        }

        public override string CommandText { get; set; }

        public override int CommandTimeout { get; set; }

        public override CommandType CommandType { get; set; }

        public override bool DesignTimeVisible { get; set; }

        public override UpdateRowSource UpdatedRowSource { get; set; }

        protected override DbConnectionBase DbConnection { get; set; }

        protected override DbParameterCollection DbParameterCollection { get; }

        protected override DbTransaction DbTransaction { get; set; }

        public override void Cancel()
        {
        }

        public override int ExecuteNonQuery()
        {
            Record("NonQuery");
            return 1;
        }

        public override object ExecuteScalar()
        {
            Record("Scalar");
            return null;
        }

        public override void Prepare()
        {
        }

        protected override DbParameter CreateDbParameter() =>
            new FakeDbParameter();

        protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
        {
            Record("Reader");
            return ((FakeDbConnection)DbConnection).Data.CreateDataReader();
        }

        private void Record(string kind)
        {
            ((FakeDbConnection)DbConnection).Executions.Add(new FakeExecution
            {
                CommandText = CommandText,
                Kind = kind,
                CommandTimeout = CommandTimeout,
                Parameters = DbParameterCollection.Cast<DbParameter>().ToDictionary(p => p.ParameterName, p => p.Value)
            });
        }
    }
}
