#region Copyright Attributions

// Copyright (c) 2026 Bradley Graigner and Michael Camara Pendon.
// Portions copyright their respective RepoDB contributors.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.IO;
using DuckDB.NET.Data;
using RepoDb.DuckDb.IntegrationTests.Models;

namespace RepoDb.DuckDb.IntegrationTests.Setup
{
    public static class Database
    {
        #region Properties

        /// <summary>
        /// Gets or sets the connection string to be used.
        /// </summary>
        public static string ConnectionString { get; private set; }

        #endregion

        #region Methods

        public static void Initialize()
        {
            // Set the connection string. DuckDB is an embedded engine (no server/database/grant concept
            // like MySQL), so the connection string is either a file path or ':memory:'. A real temp file
            // is used by default (rather than ':memory:') because the test suite opens a fresh
            // DuckDBConnection per operation, and DuckDB's in-memory databases do not persist once every
            // connection referencing them has been closed - unlike a file-backed database.
            ConnectionString =
                Environment.GetEnvironmentVariable("REPODB_DUCKDB_CONSTR") ??
                $"Data Source={GetDefaultDatabaseFilePath()}";

            // Start with a clean file on every run
            DeleteDefaultDatabaseFileIfExists();

            // Initialize DuckDb
            GlobalConfiguration
                .Setup()
                .UseDuckDb();

            // Create tables
            CreateTables();
        }

        public static void Cleanup()
        {
            using (var connection = new DuckDBConnection(ConnectionString))
            {
                connection.Truncate<CompleteTable>();
                connection.Truncate<NonIdentityCompleteTable>();
            }
        }

        private static string GetDefaultDatabaseFilePath() =>
            Path.Combine(Path.GetTempPath(), "repodb_duckdb_tests.db");

        private static void DeleteDefaultDatabaseFileIfExists()
        {
            var path = GetDefaultDatabaseFilePath();
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        #endregion

        #region CompleteTable

        public static IEnumerable<CompleteTable> CreateCompleteTables(int count)
        {
            using (var connection = new DuckDBConnection(ConnectionString))
            {
                var tables = Helper.CreateCompleteTables(count);
                connection.InsertAll(tables);
                return tables;
            }
        }

        #endregion

        #region NonIdentityCompleteTable

        public static IEnumerable<NonIdentityCompleteTable> CreateNonIdentityCompleteTables(int count)
        {
            using (var connection = new DuckDBConnection(ConnectionString))
            {
                var tables = Helper.CreateNonIdentityCompleteTables(count);
                connection.InsertAll(tables);
                return tables;
            }
        }

        #endregion

        #region CreateTables

        private static void CreateTables()
        {
            CreateCompleteTable();
            CreateNonIdentityCompleteTable();
        }

        private static void CreateCompleteTable()
        {
            using (var connection = new DuckDBConnection(ConnectionString))
            {
                // DuckDB has no native AUTO_INCREMENT; a sequence + DEFAULT nextval(...) is the closest
                // analog (see DuckDbDbHelper's identity-detection query, which looks for this pattern).
                connection.ExecuteNonQuery(@"CREATE SEQUENCE IF NOT EXISTS seq_completetable_id START 1;");
                connection.ExecuteNonQuery(@"CREATE TABLE IF NOT EXISTS ""CompleteTable""
                    (
                        ""Id"" BIGINT DEFAULT nextval('seq_completetable_id') PRIMARY KEY,
                        ""ColumnVarchar"" VARCHAR(256) DEFAULT NULL,
                        ""ColumnInt"" INTEGER DEFAULT NULL,
                        ""ColumnDecimal2"" DECIMAL(18,2) DEFAULT NULL,
                        ""ColumnDateTime"" TIMESTAMP DEFAULT NULL,
                        ""ColumnBlob"" BLOB DEFAULT NULL,
                        ""ColumnBlobAsArray"" BLOB DEFAULT NULL,
                        ""ColumnBinary"" BLOB DEFAULT NULL,
                        ""ColumnLongBlob"" BLOB DEFAULT NULL,
                        ""ColumnMediumBlob"" BLOB DEFAULT NULL,
                        ""ColumnTinyBlob"" BLOB DEFAULT NULL,
                        ""ColumnVarBinary"" BLOB DEFAULT NULL,
                        ""ColumnDate"" DATE DEFAULT NULL,
                        ""ColumnDateTime2"" TIMESTAMP DEFAULT NULL,
                        ""ColumnTime"" TIME DEFAULT NULL,
                        ""ColumnTimeStamp"" TIMESTAMP DEFAULT NULL,
                        ""ColumnYear"" SMALLINT DEFAULT NULL,
                        ""ColumnBigint"" BIGINT DEFAULT NULL,
                        ""ColumnDecimal"" DECIMAL(10,0) DEFAULT NULL,
                        ""ColumnDouble"" DOUBLE DEFAULT NULL,
                        ""ColumnFloat"" FLOAT DEFAULT NULL,
                        ""ColumnInt2"" INTEGER DEFAULT NULL,
                        ""ColumnMediumInt"" INTEGER DEFAULT NULL,
                        ""ColumnReal"" DOUBLE DEFAULT NULL,
                        ""ColumnSmallInt"" SMALLINT DEFAULT NULL,
                        ""ColumnTinyInt"" TINYINT DEFAULT NULL,
                        ""ColumnChar"" VARCHAR(1) DEFAULT NULL,
                        ""ColumnJson"" JSON DEFAULT NULL,
                        ""ColumnNChar"" VARCHAR(16) DEFAULT NULL,
                        ""ColumnNVarChar"" VARCHAR(256) DEFAULT NULL,
                        ""ColumnLongText"" VARCHAR DEFAULT NULL,
                        ""ColumnMediumText"" VARCHAR DEFAULT NULL,
                        ""ColumnText"" VARCHAR DEFAULT NULL,
                        ""ColumnTinyText"" VARCHAR DEFAULT NULL,
                        ""ColumnBit"" BIT DEFAULT NULL
                    );");
            }
        }

        private static void CreateNonIdentityCompleteTable()
        {
            using (var connection = new DuckDBConnection(ConnectionString))
            {
                connection.ExecuteNonQuery(@"CREATE TABLE IF NOT EXISTS ""NonIdentityCompleteTable""
                    (
                        ""Id"" BIGINT NOT NULL PRIMARY KEY,
                        ""ColumnVarchar"" VARCHAR(256) DEFAULT NULL,
                        ""ColumnInt"" INTEGER DEFAULT NULL,
                        ""ColumnDecimal2"" DECIMAL(18,2) DEFAULT NULL,
                        ""ColumnDateTime"" TIMESTAMP DEFAULT NULL,
                        ""ColumnBlob"" BLOB DEFAULT NULL,
                        ""ColumnBlobAsArray"" BLOB DEFAULT NULL,
                        ""ColumnBinary"" BLOB DEFAULT NULL,
                        ""ColumnLongBlob"" BLOB DEFAULT NULL,
                        ""ColumnMediumBlob"" BLOB DEFAULT NULL,
                        ""ColumnTinyBlob"" BLOB DEFAULT NULL,
                        ""ColumnVarBinary"" BLOB DEFAULT NULL,
                        ""ColumnDate"" DATE DEFAULT NULL,
                        ""ColumnDateTime2"" TIMESTAMP DEFAULT NULL,
                        ""ColumnTime"" TIME DEFAULT NULL,
                        ""ColumnTimeStamp"" TIMESTAMP DEFAULT NULL,
                        ""ColumnYear"" SMALLINT DEFAULT NULL,
                        ""ColumnBigint"" BIGINT DEFAULT NULL,
                        ""ColumnDecimal"" DECIMAL(10,0) DEFAULT NULL,
                        ""ColumnDouble"" DOUBLE DEFAULT NULL,
                        ""ColumnFloat"" FLOAT DEFAULT NULL,
                        ""ColumnInt2"" INTEGER DEFAULT NULL,
                        ""ColumnMediumInt"" INTEGER DEFAULT NULL,
                        ""ColumnReal"" DOUBLE DEFAULT NULL,
                        ""ColumnSmallInt"" SMALLINT DEFAULT NULL,
                        ""ColumnTinyInt"" TINYINT DEFAULT NULL,
                        ""ColumnChar"" VARCHAR(1) DEFAULT NULL,
                        ""ColumnJson"" JSON DEFAULT NULL,
                        ""ColumnNChar"" VARCHAR(16) DEFAULT NULL,
                        ""ColumnNVarChar"" VARCHAR(256) DEFAULT NULL,
                        ""ColumnLongText"" VARCHAR DEFAULT NULL,
                        ""ColumnMediumText"" VARCHAR DEFAULT NULL,
                        ""ColumnText"" VARCHAR DEFAULT NULL,
                        ""ColumnTinyText"" VARCHAR DEFAULT NULL,
                        ""ColumnBit"" BIT DEFAULT NULL
                    );");
            }
        }

        #endregion
    }
}
