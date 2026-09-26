#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using RepoDb.Connector.CockroachDb;
using RepoDb.CockroachDb.IntegrationTests.Models;

namespace RepoDb.CockroachDb.IntegrationTests.Setup
{
    public static class Database
    {
        #region Properties

        /// <summary>
        /// Gets or sets the connection string to be used for the default administrative database.
        /// </summary>
        public static string ConnectionStringForSystem { get; private set; }

        /// <summary>
        /// Gets or sets the connection string to be used.
        /// </summary>
        public static string ConnectionString { get; private set; }

        #endregion

        #region Methods

        public static void Initialize()
        {
            // Master connection (targets the CockroachDB default database)
            ConnectionStringForSystem =
                Environment.GetEnvironmentVariable("REPODB_COCKROACHDB_CONSTR_SYSTEM") ??
                "Server=127.0.0.1;Port=26257;Database=defaultdb;User Id=root;";

            // RepoDb connection
            ConnectionString =
                Environment.GetEnvironmentVariable("REPODB_COCKROACHDB_CONSTR") ??
                "Server=127.0.0.1;Port=26257;Database=RepoDb;User Id=root;";

            // Initialize CockroachDB
            GlobalConfiguration
                .Setup()
                .UseCockroachDb();

            // Create databases
            CreateDatabase();

            // Create tables
            CreateTables();
        }

        public static void Cleanup()
        {
            using (var connection = new CockroachDbConnection(ConnectionString))
            {
                connection.Truncate<CompleteTable>();
                connection.Truncate<NonIdentityCompleteTable>();
                connection.Truncate<EnumTable>();
                connection.Truncate("PropertyHandler");

                // The TRUNCATE statement does not reset the identity columns
                connection.ExecuteNonQuery(@"
                    ALTER TABLE public.""CompleteTable"" ALTER COLUMN ""Id"" RESTART WITH 1;
                    ALTER TABLE public.""PropertyHandler"" ALTER COLUMN ""Id"" RESTART WITH 1;");
            }
        }

        #endregion

        #region CreateDatabases

        private static void CreateDatabase()
        {
            using (var connection = new CockroachDbConnection(ConnectionStringForSystem))
            {
                connection.ExecuteNonQuery(@"CREATE DATABASE IF NOT EXISTS ""RepoDb"";");
            }
        }

        #endregion

        #region CreateTables

        private static void CreateTables()
        {
            CreateCompleteTable();
            CreateNonIdentityCompleteTable();
            CreateEnumTable();
            CreatePropertyHandlerTable();
        }

        private static void CreateCompleteTable()
        {
            using (var connection = new CockroachDbConnection(ConnectionString))
            {
                connection.ExecuteNonQuery(@"CREATE TABLE IF NOT EXISTS public.""CompleteTable""
                    (
                        ""Id"" INT8 GENERATED ALWAYS AS IDENTITY,
                        ""ColumnChar"" ""char"",
                        ""ColumnBigInt"" INT8,
                        ""ColumnBigIntAsArray"" INT8[],
                        ""ColumnBigSerial"" INT8,
                        ""ColumnBit"" BIT(1),
                        ""ColumnBitVarying"" VARBIT,
                        ""ColumnBoolean"" BOOL,
                        ""ColumnBooleanAsArray"" BOOL[],
                        ""ColumnByteA"" BYTES,
                        ""ColumnByteAAsArray"" BYTES[],
                        ""ColumnCharacter"" CHAR(1),
                        ""ColumnCharacterVarying"" VARCHAR,
                        ""ColumnCharacterVaryingAsArray"" VARCHAR[],
                        ""ColumnDate"" DATE,
                        ""ColumnDateAsArray"" DATE[],
                        ""ColumnDoublePrecision"" FLOAT8,
                        ""ColumnDoublePrecisionAsArray"" FLOAT8[],
                        ""ColumnInet"" INET,
                        ""ColumnInetAsArray"" INET[],
                        ""ColumnInteger"" INT4,
                        ""ColumnIntegerAsArray"" INT4[],
                        ""ColumnInterval"" INTERVAL,
                        ""ColumnIntervalAsArray"" INTERVAL[],
                        ""ColumnJson"" JSON,
                        ""ColumnJsonB"" JSONB,
                        ""ColumnName"" NAME,
                        ""ColumnNumeric"" DECIMAL,
                        ""ColumnNumericAsArray"" DECIMAL[],
                        ""ColumnOId"" OID,
                        ""ColumnReal"" FLOAT4,
                        ""ColumnRealAsArray"" FLOAT4[],
                        ""ColumnSmallInt"" INT2,
                        ""ColumnSmallIntAsArray"" INT2[],
                        ""ColumnText"" STRING,
                        ""ColumnTextAsArray"" STRING[],
                        ""ColumnTimeWithTimeZone"" TIMETZ,
                        ""ColumnTimeWithTimeZoneAsArray"" TIMETZ[],
                        ""ColumnTimeWithoutTimeZone"" TIME,
                        ""ColumnTimeWithoutTimeZoneAsArray"" TIME[],
                        ""ColumnTimestampWithTimeZone"" TIMESTAMPTZ,
                        ""ColumnTimestampWithTimeZoneAsArray"" TIMESTAMPTZ[],
                        ""ColumnTimestampWithoutTimeZone"" TIMESTAMP,
                        ""ColumnTimestampWithoutTimeZoneAsArray"" TIMESTAMP[],
                        ""ColumnTSQuery"" TSQUERY,
                        ""ColumnTSVector"" TSVECTOR,
                        ""ColumnUUID"" UUID,
                        ""ColumnUUIDAsArray"" UUID[],
                        CONSTRAINT ""CompleteTable_pkey"" PRIMARY KEY (""Id"")
                    );");
            }
        }

        private static void CreateNonIdentityCompleteTable()
        {
            using (var connection = new CockroachDbConnection(ConnectionString))
            {
                connection.ExecuteNonQuery(@"CREATE TABLE IF NOT EXISTS public.""NonIdentityCompleteTable""
                    (
                        ""Id"" INT8 NOT NULL,
                        ""ColumnChar"" ""char"",
                        ""ColumnBigInt"" INT8,
                        ""ColumnBigIntAsArray"" INT8[],
                        ""ColumnBigSerial"" INT8,
                        ""ColumnBit"" BIT(1),
                        ""ColumnBitVarying"" VARBIT,
                        ""ColumnBoolean"" BOOL,
                        ""ColumnBooleanAsArray"" BOOL[],
                        ""ColumnByteA"" BYTES,
                        ""ColumnByteAAsArray"" BYTES[],
                        ""ColumnCharacter"" CHAR(1),
                        ""ColumnCharacterVarying"" VARCHAR,
                        ""ColumnCharacterVaryingAsArray"" VARCHAR[],
                        ""ColumnDate"" DATE,
                        ""ColumnDateAsArray"" DATE[],
                        ""ColumnDoublePrecision"" FLOAT8,
                        ""ColumnDoublePrecisionAsArray"" FLOAT8[],
                        ""ColumnInet"" INET,
                        ""ColumnInetAsArray"" INET[],
                        ""ColumnInteger"" INT4,
                        ""ColumnIntegerAsArray"" INT4[],
                        ""ColumnInterval"" INTERVAL,
                        ""ColumnIntervalAsArray"" INTERVAL[],
                        ""ColumnJson"" JSON,
                        ""ColumnJsonB"" JSONB,
                        ""ColumnName"" NAME,
                        ""ColumnNumeric"" DECIMAL,
                        ""ColumnNumericAsArray"" DECIMAL[],
                        ""ColumnOId"" OID,
                        ""ColumnReal"" FLOAT4,
                        ""ColumnRealAsArray"" FLOAT4[],
                        ""ColumnSmallInt"" INT2,
                        ""ColumnSmallIntAsArray"" INT2[],
                        ""ColumnText"" STRING,
                        ""ColumnTextAsArray"" STRING[],
                        ""ColumnTimeWithTimeZone"" TIMETZ,
                        ""ColumnTimeWithTimeZoneAsArray"" TIMETZ[],
                        ""ColumnTimeWithoutTimeZone"" TIME,
                        ""ColumnTimeWithoutTimeZoneAsArray"" TIME[],
                        ""ColumnTimestampWithTimeZone"" TIMESTAMPTZ,
                        ""ColumnTimestampWithTimeZoneAsArray"" TIMESTAMPTZ[],
                        ""ColumnTimestampWithoutTimeZone"" TIMESTAMP,
                        ""ColumnTimestampWithoutTimeZoneAsArray"" TIMESTAMP[],
                        ""ColumnTSQuery"" TSQUERY,
                        ""ColumnTSVector"" TSVECTOR,
                        ""ColumnUUID"" UUID,
                        ""ColumnUUIDAsArray"" UUID[],
                        CONSTRAINT ""NonIdentityCompleteTable_pkey"" PRIMARY KEY (""Id"")
                    );");
            }
        }

        #endregion

        #region CompleteTable

        public static IEnumerable<CompleteTable> CreateCompleteTables(int count)
        {
            using (var connection = new CockroachDbConnection(ConnectionString))
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
            using (var connection = new CockroachDbConnection(ConnectionString))
            {
                var tables = Helper.CreateNonIdentityCompleteTables(count);
                connection.InsertAll(tables);
                return tables;
            }
        }

        #endregion

        #region PropertyHandler

        private static void CreatePropertyHandlerTable()
        {
            using (var connection = new CockroachDbConnection(ConnectionString))
            {
                connection.ExecuteNonQuery(@"CREATE TABLE IF NOT EXISTS public.""PropertyHandler""
                    (
                        ""Id"" INT8 GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
                        ""ColumnTsVector"" TSVECTOR,
                        ""ColumnTsQuery"" TSQUERY
                    );");
            }
        }

        private static void CreateEnumTable()
        {
            using (var connection = new CockroachDbConnection(ConnectionString))
            {
                connection.ExecuteNonQuery(@"
                    CREATE TYPE IF NOT EXISTS hand AS ENUM ('Unidentified', 'Left', 'Right');

                    CREATE TABLE IF NOT EXISTS public.""EnumTable""
                    (
                        ""Id"" INT8 PRIMARY KEY,
                        ""ColumnEnumHand"" hand NULL
                    );");
            }
        }

        #endregion
    }
}
