#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using DbConnectionBase = System.Data.Common.DbConnection;

namespace RepoDb.Data.Core.UnitTests.CustomObjects
{
    /// <summary>
    /// A connection whose commands read the rows of <see cref="Data"/> and record what they execute (<see cref="Executions"/>).
    /// It has no BulkInsert extension method, so the rows that are copied into it are inserted with the <c>InsertAll</c> of the library.
    /// </summary>
    public class FakeDbConnection : DbConnectionBase
    {
        static FakeDbConnection()
        {
            DbSettingMapper.Add<FakeDbConnection>(new FakeDbSetting(), true);
            DbHelperMapper.Add<FakeDbConnection>(new FakeDbHelper(), true);
            StatementBuilderMapper.Add<FakeDbConnection>(new FakeStatementBuilder(), true);
        }

        public DataTable Data { get; set; } = CreatePersons();

        public List<FakeExecution> Executions { get; } = new List<FakeExecution>();

        public override string ConnectionString { get; set; } = "FakeDbConnection";

        public override string Database => "FakeDatabase";

        public override string DataSource => "FakeDataSource";

        public override string ServerVersion => "1.0";

        public override ConnectionState State => ConnectionState.Open;

        public override void ChangeDatabase(string databaseName)
        {
        }

        public override void Close()
        {
        }

        public override void Open()
        {
        }

        protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel) =>
            new FakeDbTransaction();

        protected override DbCommand CreateDbCommand() =>
            new FakeDbCommand(this);

        /// <summary>
        /// Creates a table of persons: <c>Id</c> (int), <c>Name</c> (string) and <c>Birthday</c> (date), with the specified number of rows (the birthday of the second row is <c>DBNull</c>).
        /// </summary>
        public static DataTable CreatePersons(int rows = 3)
        {
            var table = new DataTable("Person");
            table.Columns.Add("Id", typeof(int));
            table.Columns.Add("Name", typeof(string));
            table.Columns.Add("Birthday", typeof(DateTime));
            for (var i = 1; i <= rows; i++)
            {
                table.Rows.Add(i, $"Name {i}", i == 2 ? (object)DBNull.Value : new DateTime(2000, 1, i));
            }
            return table;
        }
    }
}
