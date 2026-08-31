#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Sap.Data.Hana;
using RepoDb.SapHana.BulkOperations.IntegrationTests.Models;
using RepoDb.SapHana.PropertyHandlers;
using System;
using System.Text.RegularExpressions;

namespace RepoDb.IntegrationTests.Setup
{
    /// <summary>
    /// A class used as a startup setup for the RepoDb SAP HANA bulk-operations test database.
    /// </summary>
    public static class Database
    {
        #region Properties

        /// <summary>
        /// Gets the connection string to be used for the SAP HANA database.
        /// </summary>
        public static string ConnectionString { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Initialize the creation of the database.
        /// </summary>
        public static void Initialize()
        {
            ConnectionString =
                Environment.GetEnvironmentVariable("REPODB_SAPHANA_CONSTR_BULK") ??
                Environment.GetEnvironmentVariable("REPODB_SAPHANA_CONSTR") ??
                "Server=localhost:39041;UserID=SYSTEM;Password=RepoDB2026;Current Schema=REPODB;";

            // Initialize SAP HANA
            GlobalConfiguration
                .Setup()
                .UseSapHana();

            PropertyHandlerMapper.Add<BulkOperationIdentityTable, SapHanaGuidToStringPropertyHandler>(
                e => e.RowGuid, new SapHanaGuidToStringPropertyHandler(), true);
            PropertyHandlerMapper.Add<BulkOperationNonIdentityTable, SapHanaGuidToStringPropertyHandler>(
                e => e.RowGuid, new SapHanaGuidToStringPropertyHandler(), true);
            PropertyHandlerMapper.Add<WithExtraFieldsBulkOperationIdentityTable, SapHanaGuidToStringPropertyHandler>(
                e => e.RowGuid, new SapHanaGuidToStringPropertyHandler(), true);
            PropertyHandlerMapper.Add<WithExtraFieldsBulkOperationNonIdentityTable, SapHanaGuidToStringPropertyHandler>(
                e => e.RowGuid, new SapHanaGuidToStringPropertyHandler(), true);
            PropertyHandlerMapper.Add<BulkOperationMappedIdentityTable, SapHanaGuidToStringPropertyHandler>(
                e => e.RowGuidMapped, new SapHanaGuidToStringPropertyHandler(), true);
            PropertyHandlerMapper.Add<BulkOperationMappedNonIdentityTable, SapHanaGuidToStringPropertyHandler>(
                e => e.RowGuidMapped, new SapHanaGuidToStringPropertyHandler(), true);

            // Create the schema
            EnsureSchema();

            // Create the tables
            CreateTables();
        }

        /// <summary>
        /// 
        /// </summary>
        private static void EnsureSchema()
        {
            var schemaMatch = Regex.Match(ConnectionString, @"Current Schema\s*=\s*([^;]+)", RegexOptions.IgnoreCase);
            var schemaName = schemaMatch.Success ? schemaMatch.Groups[1].Value.Trim() : "REPODB";
            var bootstrapConnectionString = Regex.Replace(ConnectionString, @"Current Schema\s*=\s*[^;]*;?", "", RegexOptions.IgnoreCase);

            using var connection = new HanaConnection(bootstrapConnectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = $"SELECT COUNT(*) FROM SYS.SCHEMAS WHERE SCHEMA_NAME = '{schemaName}'";
            var exists = Convert.ToInt32(command.ExecuteScalar()) > 0;

            if (!exists)
            {
                command.CommandText = $"CREATE SCHEMA \"{schemaName}\"";
                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Clean up all the table.
        /// </summary>
        public static void Cleanup()
        {
            using var connection = new HanaConnection(ConnectionString);
            connection.Truncate<BulkOperationIdentityTable>();
            connection.Truncate<BulkOperationNonIdentityTable>();
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

        #region CreateTables

        /// <summary>
        /// Create the necessary tables for testing.
        /// </summary>
        public static void CreateTables()
        {
            CreateBulkOperationIdentityTable();
            CreateBulkOperationNonIdentityTable();
        }

        /// <summary>
        /// 
        /// </summary>
        public static void CreateBulkOperationIdentityTable()
        {
            using var connection = new HanaConnection(ConnectionString);

            if (TableExists(connection, "BulkOperationIdentityTable")) return;

            var commandText = @"
                CREATE TABLE ""BulkOperationIdentityTable""
                (
                    ""Id"" BIGINT GENERATED BY DEFAULT AS IDENTITY NOT NULL,
                    ""RowGuid"" NVARCHAR(36) NOT NULL,
                    ""ColumnBit"" TINYINT NULL,
                    ""ColumnDateTime"" TIMESTAMP NULL,
                    ""ColumnDateTime2"" TIMESTAMP NULL,
                    ""ColumnDecimal"" DECIMAL(18,2) NULL,
                    ""ColumnFloat"" DOUBLE NULL,
                    ""ColumnInt"" INTEGER NULL,
                    ""ColumnNVarChar"" NVARCHAR(2000) NULL,
                    PRIMARY KEY (""Id"")
                );";
            connection.ExecuteNonQuery(commandText);
        }

        /// <summary>
        /// 
        /// </summary>
        public static void CreateBulkOperationNonIdentityTable()
        {
            using var connection = new HanaConnection(ConnectionString);

            if (TableExists(connection, "BulkOperationNonIdentityTable")) return;

            var commandText = @"
                CREATE TABLE ""BulkOperationNonIdentityTable""
                (
                    ""Id"" BIGINT NOT NULL,
                    ""RowGuid"" NVARCHAR(36) NOT NULL,
                    ""ColumnBit"" TINYINT NULL,
                    ""ColumnDateTime"" TIMESTAMP NULL,
                    ""ColumnDateTime2"" TIMESTAMP NULL,
                    ""ColumnDecimal"" DECIMAL(18,2) NULL,
                    ""ColumnFloat"" DOUBLE NULL,
                    ""ColumnInt"" INTEGER NULL,
                    ""ColumnNVarChar"" NVARCHAR(2000) NULL,
                    PRIMARY KEY (""Id"")
                );";
            connection.ExecuteNonQuery(commandText);
        }

        #endregion
    }
}
