#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System.Data.Common;
using System.Data;

namespace RepoDb.UnitTests.CustomObjects
{
    /// <summary>
    /// Supplies deterministic command results and records connection ownership without a database.
    /// </summary>
    public class CoverageDbConnection : CustomTracedDbConnection
    {
        private readonly DataSet results = new DataSet();
        private ConnectionState state;

        public CoverageDbConnection()
        {
            RowsAffected = 1;
            ScalarValue = 1;
            for (var index = 0; index < 7; index++)
            {
                var table = new DataTable();
                table.Columns.Add("Id", typeof(int));
                table.Columns.Add("Name", typeof(string));
                table.Rows.Add(1, "Result");
                results.Tables.Add(table);
            }
            ReaderFactory = () => results.CreateDataReader();
        }

        public override ConnectionState State => state;

        public int DisposeCount { get; private set; }

        public void UseScalarResults()
        {
            foreach (DataTable table in results.Tables)
            {
                table.Columns.Remove("Name");
            }
        }

        public void UseBatchResults()
        {
            RowsAffected = 2;
            ReaderFactory = () => results.Tables[0].CreateDataReader();
            foreach (DataTable table in results.Tables)
            {
                table.Columns.Remove("Name");
                table.Columns.Add("Position", typeof(int));
                table.Rows[0]["Position"] = 0;
                table.Rows.Add(2, 1);
            }
        }

        public override void Open()
        {
            base.Open();
            state = ConnectionState.Open;
        }

        public override void Close()
        {
            state = ConnectionState.Closed;
        }

        protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel) =>
            new CoverageDbTransaction(this, isolationLevel);

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                DisposeCount++;
                results.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
