#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using Vertica.Data.VerticaClient;
using RepoDb.PropertyHandlers.Vertica;
using RepoDb.Vertica.IntegrationTests.Models;

namespace RepoDb.Vertica.IntegrationTests.Setup
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
            ConnectionString =
                Environment.GetEnvironmentVariable("REPODB_VERTICA_CONSTR") ??
                @"Host=127.0.0.1;Port=5433;Database=RepoDb;User=dbadmin;Password=RepoDB2026;";

            // Initialize Vertica
            GlobalConfiguration
                .Setup()
                .UseVertica(useInvariantCulture: true);

            // Property Handlers
            PropertyHandlerMapper.Add<CompleteTable, TimeToDateTimePropertyHandler>(
                e => e.ColumnTime, new TimeToDateTimePropertyHandler(), true);
            PropertyHandlerMapper.Add<NonIdentityCompleteTable, TimeToDateTimePropertyHandler>(
                e => e.ColumnTime, new TimeToDateTimePropertyHandler(), true);

            // Create tables
            CreateTables();
        }

        public static void Cleanup()
        {
            using (var connection = new VerticaConnection(ConnectionString))
            {
                connection.Truncate<CompleteTable>();
                connection.Truncate<NonIdentityCompleteTable>();
            }
        }

        #endregion

        #region CompleteTable

        public static IEnumerable<CompleteTable> CreateCompleteTables(int count)
        {
            using (var connection = new VerticaConnection(ConnectionString))
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
            using (var connection = new VerticaConnection(ConnectionString))
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
            using (var connection = new VerticaConnection(ConnectionString))
            {
                // Vertica has no RECREATE TABLE statement, and no multi-statement command text
                // (IsMultiStatementExecutable is false) - drop and create are two separate round trips.
                connection.ExecuteNonQuery(@"DROP TABLE IF EXISTS ""CompleteTable"" CASCADE;");

                // ColumnBinary/ColumnVarBinary/etc. use VARBINARY/LONG VARBINARY rather than CHAR/VARCHAR -
                // Vertica has no CHARACTER SET OCTETS equivalent, and VARBINARY/LONG VARBINARY are its
                // real binary types. ColumnJson/ColumnLongText/ColumnMediumText/ColumnText use LONG VARCHAR
                // (Vertica's large-text type) rather than Firebird's BLOB SUB_TYPE TEXT. Vertica has no
                // NCHAR/NCHAR VARYING - it stores everything as UTF-8, so CHAR/VARCHAR cover both.
                connection.ExecuteNonQuery(@"CREATE TABLE ""CompleteTable""
                    (
                        ""Id"" IDENTITY(1, 1),
                        ""ColumnVarchar"" VARCHAR(256),
                        ""ColumnInt"" INTEGER,
                        ""ColumnDecimal2"" DECIMAL(18,2),
                        ""ColumnDateTime"" TIMESTAMP,
                        ""ColumnBlob"" VARBINARY(65000),
                        ""ColumnBlobAsArray"" VARBINARY(65000),
                        ""ColumnBinary"" VARBINARY(65000),
                        ""ColumnLongBlob"" LONG VARBINARY(1000000),
                        ""ColumnMediumBlob"" LONG VARBINARY(1000000),
                        ""ColumnTinyBlob"" VARBINARY(65000),
                        ""ColumnVarBinary"" VARBINARY(65000),
                        ""ColumnDate"" DATE,
                        ""ColumnDateTime2"" TIMESTAMP,
                        ""ColumnTime"" TIME,
                        ""ColumnTimeStamp"" TIMESTAMP,
                        ""ColumnYear"" SMALLINT,
                        ""ColumnBigint"" BIGINT,
                        ""ColumnDecimal"" DECIMAL(10,0),
                        ""ColumnDouble"" DOUBLE PRECISION,
                        ""ColumnFloat"" FLOAT,
                        ""ColumnInt2"" INTEGER,
                        ""ColumnMediumInt"" INTEGER,
                        ""ColumnReal"" DOUBLE PRECISION,
                        ""ColumnSmallInt"" SMALLINT,
                        ""ColumnTinyInt"" SMALLINT,
                        ""ColumnChar"" CHAR(1),
                        ""ColumnJson"" LONG VARCHAR(1000000),
                        ""ColumnNChar"" CHAR(16),
                        ""ColumnNVarChar"" VARCHAR(256),
                        ""ColumnLongText"" LONG VARCHAR(1000000),
                        ""ColumnMediumText"" LONG VARCHAR(1000000),
                        ""ColumnText"" LONG VARCHAR(1000000),
                        ""ColumnTinyText"" VARCHAR(255),
                        ""ColumnBit"" BOOLEAN,
                        PRIMARY KEY (""Id"")
                    );");
            }
        }

        private static void CreateNonIdentityCompleteTable()
        {
            using (var connection = new VerticaConnection(ConnectionString))
            {
                connection.ExecuteNonQuery(@"DROP TABLE IF EXISTS ""NonIdentityCompleteTable"" CASCADE;");

                connection.ExecuteNonQuery(@"CREATE TABLE ""NonIdentityCompleteTable""
                    (
                        ""Id"" BIGINT NOT NULL,
                        ""ColumnVarchar"" VARCHAR(256),
                        ""ColumnInt"" INTEGER,
                        ""ColumnDecimal2"" DECIMAL(18,2),
                        ""ColumnDateTime"" TIMESTAMP,
                        ""ColumnBlob"" VARBINARY(65000),
                        ""ColumnBlobAsArray"" VARBINARY(65000),
                        ""ColumnBinary"" VARBINARY(65000),
                        ""ColumnLongBlob"" LONG VARBINARY(1000000),
                        ""ColumnMediumBlob"" LONG VARBINARY(1000000),
                        ""ColumnTinyBlob"" VARBINARY(65000),
                        ""ColumnVarBinary"" VARBINARY(65000),
                        ""ColumnDate"" DATE,
                        ""ColumnDateTime2"" TIMESTAMP,
                        ""ColumnTime"" TIME,
                        ""ColumnTimeStamp"" TIMESTAMP,
                        ""ColumnYear"" SMALLINT,
                        ""ColumnBigint"" BIGINT,
                        ""ColumnDecimal"" DECIMAL(10,0),
                        ""ColumnDouble"" DOUBLE PRECISION,
                        ""ColumnFloat"" FLOAT,
                        ""ColumnInt2"" INTEGER,
                        ""ColumnMediumInt"" INTEGER,
                        ""ColumnReal"" DOUBLE PRECISION,
                        ""ColumnSmallInt"" SMALLINT,
                        ""ColumnTinyInt"" SMALLINT,
                        ""ColumnChar"" CHAR(1),
                        ""ColumnJson"" LONG VARCHAR(1000000),
                        ""ColumnNChar"" CHAR(16),
                        ""ColumnNVarChar"" VARCHAR(256),
                        ""ColumnLongText"" LONG VARCHAR(1000000),
                        ""ColumnMediumText"" LONG VARCHAR(1000000),
                        ""ColumnText"" LONG VARCHAR(1000000),
                        ""ColumnTinyText"" VARCHAR(255),
                        ""ColumnBit"" BOOLEAN,
                        PRIMARY KEY (""Id"")
                    );");
            }
        }

        #endregion
    }
}
