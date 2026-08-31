#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Sap.Data.Hana;
using RepoDb.SapHana.IntegrationTests.Models;

namespace RepoDb.SapHana.IntegrationTests.Setup
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
            // Set the connection string. Port 39041 is the HANA Express tenant ("HXE") database's own SQL
            // port - connecting there directly is required in a Docker setup like this repo's
            // docker-compose.yml: the SYSTEMDB port (39013) redirects clients to the tenant using the
            // container's internal Docker-network address, which isn't reachable from the host.
            ConnectionString =
                Environment.GetEnvironmentVariable("REPODB_SAPHANA_CONSTR") ??
                @"Server=localhost:39041;UserID=SYSTEM;Password=RepoDB2026;Current Schema=REPODB;";

            // Initialize SAP HANA
            GlobalConfiguration
                .Setup()
                .UseSapHana();

            // HANA refuses to even open a connection whose "Current Schema" doesn't exist yet, so the
            // target schema has to be created (if missing) over a connection string with that clause
            // stripped, before ConnectionString itself can be used for anything else.
            EnsureSchema();

            // Create tables
            CreateTables();
        }

        private static void EnsureSchema()
        {
            var schemaMatch = Regex.Match(ConnectionString, @"Current Schema\s*=\s*([^;]+)", RegexOptions.IgnoreCase);
            var schemaName = schemaMatch.Success ? schemaMatch.Groups[1].Value.Trim() : "REPODB";
            var bootstrapConnectionString = Regex.Replace(ConnectionString, @"Current Schema\s*=\s*[^;]*;?", "", RegexOptions.IgnoreCase);

            using (var connection = new HanaConnection(bootstrapConnectionString))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = $"SELECT COUNT(*) FROM SYS.SCHEMAS WHERE SCHEMA_NAME = '{schemaName}'";
                    var exists = Convert.ToInt32(command.ExecuteScalar()) > 0;

                    if (!exists)
                    {
                        command.CommandText = $"CREATE SCHEMA \"{schemaName}\"";
                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        public static void Cleanup()
        {
            using (var connection = new HanaConnection(ConnectionString))
            {
                connection.Truncate<CompleteTable>();
                connection.Truncate<NonIdentityCompleteTable>();
            }
        }

        /// <summary>
        /// HANA has no <c>CREATE TABLE IF NOT EXISTS</c> - unlike its <c>CREATE TABLE ... IF NOT EXISTS</c>
        /// claim, attempting it throws a syntax error - so callers must check for the table first.
        /// </summary>
        private static bool TableExists(HanaConnection connection,
            string tableName)
        {
            return connection.ExecuteScalar<int>(
                $"SELECT COUNT(*) FROM SYS.TABLES WHERE SCHEMA_NAME = CURRENT_SCHEMA AND TABLE_NAME = '{tableName}'") > 0;
        }

        #endregion

        #region CompleteTable

        public static IEnumerable<CompleteTable> CreateCompleteTables(int count)
        {
            using (var connection = new HanaConnection(ConnectionString))
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
            using (var connection = new HanaConnection(ConnectionString))
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

        /// <summary>
        /// Creates the "CompleteTable" test table. Column types are mapped from the original
        /// SapHana version to their closest SAP HANA equivalents:
        /// MySQL's spatial types (GEOMETRY/POINT/LINESTRING/POLYGON and their MULTI* variants) and JSON
        /// have no native HANA counterpart wired up in <see cref="Resolvers.SapHanaDbTypeNameToClientTypeResolver"/>
        /// (HANA does have a separate ST_GEOMETRY spatial-type family, but wiring that up needs an SRS
        /// definition most test instances won't have configured), so those columns are stored as NCLOB
        /// instead - a deliberate simplification, not a like-for-like port. YEAR has no HANA equivalent
        /// and becomes SMALLINT; MEDIUMINT becomes INTEGER; BIT(1) becomes BOOLEAN.
        /// </summary>
        private static void CreateCompleteTable()
        {
            using (var connection = new HanaConnection(ConnectionString))
            {
                if (TableExists(connection, "CompleteTable")) return;

                connection.ExecuteNonQuery(@"CREATE TABLE ""CompleteTable""
                    (
                        ""Id"" BIGINT GENERATED BY DEFAULT AS IDENTITY NOT NULL,
                        ""ColumnVarchar"" NVARCHAR(256) DEFAULT NULL,
                        ""ColumnInt"" INTEGER DEFAULT NULL,
                        ""ColumnDecimal2"" DECIMAL(18,2) DEFAULT NULL,
                        ""ColumnDateTime"" TIMESTAMP DEFAULT NULL,
                        ""ColumnBlob"" BLOB,
                        ""ColumnBlobAsArray"" BLOB,
                        ""ColumnBinary"" VARBINARY(255) DEFAULT NULL,
                        ""ColumnLongBlob"" BLOB,
                        ""ColumnMediumBlob"" BLOB,
                        ""ColumnTinyBlob"" BLOB,
                        ""ColumnVarBinary"" VARBINARY(256) DEFAULT NULL,
                        ""ColumnDate"" DATE DEFAULT NULL,
                        ""ColumnDateTime2"" TIMESTAMP DEFAULT NULL,
                        ""ColumnTime"" TIME DEFAULT NULL,
                        ""ColumnTimeStamp"" TIMESTAMP DEFAULT NULL,
                        ""ColumnYear"" SMALLINT DEFAULT NULL,
                        ""ColumnGeometry"" NCLOB DEFAULT NULL,
                        ""ColumnLineString"" NCLOB DEFAULT NULL,
                        ""ColumnMultiLineString"" NCLOB DEFAULT NULL,
                        ""ColumnMultiPoint"" NCLOB DEFAULT NULL,
                        ""ColumnMultiPolygon"" NCLOB DEFAULT NULL,
                        ""ColumnPoint"" NCLOB DEFAULT NULL,
                        ""ColumnPolygon"" NCLOB DEFAULT NULL,
                        ""ColumnBigint"" BIGINT DEFAULT NULL,
                        ""ColumnDecimal"" DECIMAL(10,0) DEFAULT NULL,
                        ""ColumnDouble"" DOUBLE DEFAULT NULL,
                        ""ColumnFloat"" REAL DEFAULT NULL,
                        ""ColumnInt2"" INTEGER DEFAULT NULL,
                        ""ColumnMediumInt"" INTEGER DEFAULT NULL,
                        ""ColumnReal"" DOUBLE DEFAULT NULL,
                        ""ColumnSmallInt"" SMALLINT DEFAULT NULL,
                        ""ColumnTinyInt"" TINYINT DEFAULT NULL,
                        ""ColumnChar"" CHAR(1) DEFAULT NULL,
                        ""ColumnJson"" NCLOB DEFAULT NULL,
                        ""ColumnNChar"" NCHAR(16) DEFAULT NULL,
                        ""ColumnNVarChar"" NVARCHAR(256) DEFAULT NULL,
                        ""ColumnLongText"" NCLOB,
                        ""ColumnMediumText"" NCLOB,
                        ""ColumnText"" NCLOB,
                        ""ColumnTinyText"" NCLOB,
                        ""ColumnBit"" BOOLEAN DEFAULT NULL,
                        PRIMARY KEY (""Id"")
                    );");
            }
        }

        /// <summary>
        /// Creates the "NonIdentityCompleteTable" test table. See the type-mapping remarks on
        /// <see cref="CreateCompleteTable"/> - identical column set except <c>"Id"</c> is a plain
        /// <c>BIGINT</c> primary key (no <c>GENERATED ... AS IDENTITY</c>).
        /// </summary>
        private static void CreateNonIdentityCompleteTable()
        {
            using (var connection = new HanaConnection(ConnectionString))
            {
                if (TableExists(connection, "NonIdentityCompleteTable")) return;

                connection.ExecuteNonQuery(@"CREATE TABLE ""NonIdentityCompleteTable""
                    (
                        ""Id"" BIGINT NOT NULL,
                        ""ColumnVarchar"" NVARCHAR(256) DEFAULT NULL,
                        ""ColumnInt"" INTEGER DEFAULT NULL,
                        ""ColumnDecimal2"" DECIMAL(18, 2) DEFAULT NULL,
                        ""ColumnDateTime"" TIMESTAMP DEFAULT NULL,
                        ""ColumnBlob"" BLOB,
                        ""ColumnBlobAsArray"" BLOB,
                        ""ColumnBinary"" VARBINARY(255) DEFAULT NULL,
                        ""ColumnLongBlob"" BLOB,
                        ""ColumnMediumBlob"" BLOB,
                        ""ColumnTinyBlob"" BLOB,
                        ""ColumnVarBinary"" VARBINARY(256) DEFAULT NULL,
                        ""ColumnDate"" DATE DEFAULT NULL,
                        ""ColumnDateTime2"" TIMESTAMP DEFAULT NULL,
                        ""ColumnTime"" TIME DEFAULT NULL,
                        ""ColumnTimeStamp"" TIMESTAMP DEFAULT NULL,
                        ""ColumnYear"" SMALLINT DEFAULT NULL,
                        ""ColumnGeometry"" NCLOB DEFAULT NULL,
                        ""ColumnLineString"" NCLOB DEFAULT NULL,
                        ""ColumnMultiLineString"" NCLOB DEFAULT NULL,
                        ""ColumnMultiPoint"" NCLOB DEFAULT NULL,
                        ""ColumnMultiPolygon"" NCLOB DEFAULT NULL,
                        ""ColumnPoint"" NCLOB DEFAULT NULL,
                        ""ColumnPolygon"" NCLOB DEFAULT NULL,
                        ""ColumnBigint"" BIGINT DEFAULT NULL,
                        ""ColumnDecimal"" DECIMAL(10, 0) DEFAULT NULL,
                        ""ColumnDouble"" DOUBLE DEFAULT NULL,
                        ""ColumnFloat"" REAL DEFAULT NULL,
                        ""ColumnInt2"" INTEGER DEFAULT NULL,
                        ""ColumnMediumInt"" INTEGER DEFAULT NULL,
                        ""ColumnReal"" DOUBLE DEFAULT NULL,
                        ""ColumnSmallInt"" SMALLINT DEFAULT NULL,
                        ""ColumnTinyInt"" TINYINT DEFAULT NULL,
                        ""ColumnChar"" CHAR(1) DEFAULT NULL,
                        ""ColumnJson"" NCLOB DEFAULT NULL,
                        ""ColumnNChar"" NCHAR(16) DEFAULT NULL,
                        ""ColumnNVarChar"" NVARCHAR(256) DEFAULT NULL,
                        ""ColumnLongText"" NCLOB,
                        ""ColumnMediumText"" NCLOB,
                        ""ColumnText"" NCLOB,
                        ""ColumnTinyText"" NCLOB,
                        ""ColumnBit"" BOOLEAN DEFAULT NULL,
                        PRIMARY KEY(""Id"")
                    );");
            }
        }

        #endregion
    }
}
