#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using RepoDb.Connector.MariaDbConnector;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Data.MariaDbConnector.IntegrationTests.Setup;
using RepoDb.Schema.MariaDbConnector;
using RepoDb.Schema;

namespace RepoDb.Data.MariaDbConnector.IntegrationTests
{
    public static class Helper
    {
        /// <summary>
        /// The schema of a table is its database in MySQL: the <c>Sales</c> schema of the tests is a database of the source and another one of the target.
        /// </summary>
        /// <param name="connectionString">The connection string of the database (the source or the target).</param>
        /// <param name="commandText">The statement, with the tables of the Sales schema as <c>`Sales`.`Table`</c>.</param>
        public static string Qualify(string connectionString,
            string commandText) =>
            commandText.Replace("`Sales`.", "`" + (connectionString == Database.ConnectionStringForSource ? Database.SourceSalesName : Database.TargetSalesName) + "`.");

        /// <summary>
        /// Reads the rows of the query as dictionaries of the column names and the values (<c>DBNull</c> is read as <c>null</c>).
        /// </summary>
        /// <param name="connectionString">The connection string of the database.</param>
        /// <param name="commandText">The query.</param>
        /// <returns>The rows.</returns>
        public static List<Dictionary<string, object>> Read(string connectionString,
            string commandText)
        {
            var rows = new List<Dictionary<string, object>>();
            using (var connection = new MariaDbConnection(connectionString).EnsureOpen())
            using (var command = connection.CreateCommand())
            {
                command.CommandText = Qualify(connectionString, commandText);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var row = new Dictionary<string, object>(StringComparer.Ordinal);
                        for (var i = 0; i < reader.FieldCount; i++)
                        {
                            row[reader.GetName(i)] = reader.IsDBNull(i) ? null : reader.GetValue(i);
                        }
                        rows.Add(row);
                    }
                }
            }
            return rows;
        }

        /// <summary>
        /// Reads the rows of the table from the source database.
        /// </summary>
        /// <param name="table">The quoted name of the table (i.e.: <c>[dbo].[Person]</c>).</param>
        /// <param name="orderBy">The order of the rows (i.e.: <c>[Id]</c>).</param>
        /// <param name="where">The filter of the rows, without the <c>WHERE</c> keyword.</param>
        public static List<Dictionary<string, object>> SourceRows(string table,
            string orderBy,
            string where = null) =>
            Read(Database.ConnectionStringForSource, $"SELECT * FROM {table}{(where == null ? string.Empty : " WHERE " + where)} ORDER BY {orderBy}");

        /// <summary>
        /// Reads the rows of the table from the target database.
        /// </summary>
        /// <param name="table">The quoted name of the table (i.e.: <c>[dbo].[Person]</c>).</param>
        /// <param name="orderBy">The order of the rows (i.e.: <c>[Id]</c>).</param>
        /// <param name="where">The filter of the rows, without the <c>WHERE</c> keyword.</param>
        public static List<Dictionary<string, object>> TargetRows(string table,
            string orderBy,
            string where = null) =>
            Read(Database.ConnectionStringForTarget, $"SELECT * FROM {table}{(where == null ? string.Empty : " WHERE " + where)} ORDER BY {orderBy}");

        /// <summary>
        /// Counts the rows of the table in the source database.
        /// </summary>
        public static int SourceCount(string table,
            string where = null) =>
            Convert.ToInt32(Read(Database.ConnectionStringForSource, $"SELECT COUNT(*) AS `Count` FROM {table}{(where == null ? string.Empty : " WHERE " + where)}")[0]["Count"]);

        /// <summary>
        /// Counts the rows of the table in the target database.
        /// </summary>
        public static int TargetCount(string table,
            string where = null) =>
            Convert.ToInt32(Read(Database.ConnectionStringForTarget, $"SELECT COUNT(*) AS `Count` FROM {table}{(where == null ? string.Empty : " WHERE " + where)}")[0]["Count"]);

        /// <summary>
        /// Checks whether the table exists in the target database.
        /// </summary>
        /// <param name="tableName">The name of the table (i.e.: <c>Person</c> or <c>Sales.Invoice</c>).</param>
        public static bool TargetTableExists(string tableName)
        {
            using (var connection = new MariaDbConnection(Database.ConnectionStringForTarget))
            {
                return new MariaDbConnectorSchemaReader(connection).TableExists(tableName.StartsWith("Sales.", StringComparison.Ordinal) ? Database.TargetSalesName + tableName.Substring("Sales".Length) : tableName);
            }
        }

        /// <summary>
        /// Gets the names of the (user) tables of the target database, in the <c>[schema].[table]</c> form, in alphabetical order.
        /// </summary>
        public static List<string> TargetTables() =>
            Read(Database.ConnectionStringForTarget, $"SELECT CASE WHEN table_schema = '{Database.TargetName}' THEN CONCAT('`', table_name, '`') ELSE CONCAT('`Sales', '`.`', table_name, '`') END AS `Name` FROM information_schema.tables WHERE table_type = 'BASE TABLE' AND table_schema IN ('{Database.TargetName}', '{Database.TargetSalesName}')")
                .Select(row => (string)row["Name"])
                .OrderBy(name => name.Replace("`", string.Empty), StringComparer.Ordinal)
                .ToList();

        /// <summary>
        /// Maps the schema reader of the source connection, which the copy of the tables that are related to the requested table needs.
        /// </summary>
        /// <param name="source">The source connection.</param>
        public static void MapSchemaReader(MariaDbConnection source) =>
            SchemaReaderMapper.Add<MariaDbConnection>(new MariaDbConnectorSchemaReader(source), true);

        /// <summary>
        /// Creates the tables (without their rows) in the target database, with the schema that they have in the source database.
        /// The tables must be given in the order that they can be created (the referenced tables first).
        /// </summary>
        /// <param name="tableNames">The names of the tables.</param>
        public static void CreateTargetTables(params string[] tableNames)
        {
            var composer = new MariaDbConnectorSchemaComposer();
            using (var source = new MariaDbConnection(Database.ConnectionStringForSource))
            using (var target = new MariaDbConnection(Database.ConnectionStringForTarget))
            {
                var reader = new MariaDbConnectorSchemaReader(source);
                foreach (var tableName in tableNames)
                {
                    foreach (var statement in composer.ComposeSchema(reader.GetTableSchema(tableName)))
                    {
                        target.ExecuteNonQuery(statement);
                    }
                }
            }
        }

        /// <summary>
        /// Counts the rows of the child table (of the target database) that reference a row that does not exist in the parent table.
        /// </summary>
        /// <param name="childTable">The quoted name of the child table.</param>
        /// <param name="childColumn">The quoted name of the column of the foreign key.</param>
        /// <param name="parentTable">The quoted name of the parent table.</param>
        /// <param name="parentColumn">The quoted name of the referenced column.</param>
        public static int TargetOrphans(string childTable,
            string childColumn,
            string parentTable,
            string parentColumn) =>
            Convert.ToInt32(Read(Database.ConnectionStringForTarget,
                $"SELECT COUNT(*) AS `Count` FROM {childTable} c LEFT JOIN {parentTable} p ON c.{childColumn} = p.{parentColumn} WHERE c.{childColumn} IS NOT NULL AND p.{parentColumn} IS NULL")[0]["Count"]);

        /// <summary>
        /// Executes the statement on the source database.
        /// </summary>
        public static void ExecuteOnSource(string commandText)
        {
            using (var connection = new MariaDbConnection(Database.ConnectionStringForSource))
            {
                connection.ExecuteNonQuery(Qualify(Database.ConnectionStringForSource, commandText));
            }
        }

        /// <summary>
        /// Executes the statement on the target database.
        /// </summary>
        public static void ExecuteOnTarget(string commandText)
        {
            using (var connection = new MariaDbConnection(Database.ConnectionStringForTarget))
            {
                connection.ExecuteNonQuery(Qualify(Database.ConnectionStringForTarget, commandText));
            }
        }

        /// <summary>
        /// Asserts that the rows are the same: the same number of rows, with the same values in the same order.
        /// </summary>
        /// <param name="expected">The expected rows.</param>
        /// <param name="actual">The actual rows.</param>
        /// <param name="columns">The columns to be compared. The default is all the columns of the expected rows.</param>
        public static void AssertRowsEqual(IList<Dictionary<string, object>> expected,
            IList<Dictionary<string, object>> actual,
            params string[] columns)
        {
            Assert.AreEqual(expected.Count, actual.Count, "The number of rows is different.");
            for (var i = 0; i < expected.Count; i++)
            {
                foreach (var column in columns.Length > 0 ? columns : expected[i].Keys.ToArray())
                {
                    Assert.IsTrue(actual[i].ContainsKey(column), $"The column '{column}' does not exist in the row {i}.");
                    AssertValuesEqual(expected[i][column], actual[i][column], $"Row {i}, column '{column}'");
                }
            }
        }

        private static void AssertValuesEqual(object expected,
            object actual,
            string message)
        {
            if (expected is byte[] expectedBytes)
            {
                Assert.IsInstanceOfType<byte[]>(actual, message);
                CollectionAssert.AreEqual(expectedBytes, (byte[])actual, message);
                return;
            }
            // TINYINT(1) is read as a boolean, while the columns that are created in the target (TINYINT) are read as numbers
            if (expected is bool expectedBool && actual is sbyte actualSByte)
            {
                Assert.AreEqual(expectedBool, actualSByte != 0, message);
                return;
            }
            Assert.AreEqual(expected, actual, message);
        }
    }
}
