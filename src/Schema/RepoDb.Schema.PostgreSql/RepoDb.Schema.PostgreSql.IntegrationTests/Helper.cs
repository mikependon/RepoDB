#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Npgsql;
using RepoDb.Schema.Models;
using RepoDb.Schema.PostgreSql.IntegrationTests.Setup;

namespace RepoDb.Schema.PostgreSql.IntegrationTests
{
    public static class Helper
    {
        /// <summary>
        /// Reads the schema of the table from the source database.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <returns>The <see cref="TableSchema"/> of the table.</returns>
        public static TableSchema GetSourceSchema(string tableName)
        {
            using (var connection = new NpgsqlConnection(Database.ConnectionStringForSource))
            {
                return new PostgreSqlSchemaReader(connection).GetTableSchema(tableName);
            }
        }

        /// <summary>
        /// Reads the schema of the table from the target database.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <returns>The <see cref="TableSchema"/> of the table.</returns>
        public static TableSchema GetTargetSchema(string tableName)
        {
            using (var connection = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                return new PostgreSqlSchemaReader(connection).GetTableSchema(tableName);
            }
        }

        /// <summary>
        /// Checks whether the table exists in the target database.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <returns><c>true</c> if the table exists; otherwise, <c>false</c>.</returns>
        public static bool TargetTableExists(string tableName)
        {
            using (var connection = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                return new PostgreSqlSchemaReader(connection).TableExists(tableName);
            }
        }

        /// <summary>
        /// Asserts that the table of the target database has the same schema as the table of the source database.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        public static void AssertTargetMatchesSource(string tableName) =>
            AssertSchemaEquality(GetSourceSchema(tableName), GetTargetSchema(tableName));

        /// <summary>
        /// Reads the schemas of the tables from the source database.
        /// </summary>
        /// <param name="tableNames">The names of the tables.</param>
        /// <returns>The <see cref="TableSchema"/> objects of the tables, in the same order.</returns>
        public static List<TableSchema> GetSourceSchemas(IEnumerable<string> tableNames)
        {
            using (var connection = new NpgsqlConnection(Database.ConnectionStringForSource))
            {
                var reader = new PostgreSqlSchemaReader(connection);
                return tableNames.Select(reader.GetTableSchema).ToList();
            }
        }

        /// <summary>
        /// Executes the statements, in order, on the target database.
        /// </summary>
        /// <param name="statements">The SQL statements.</param>
        public static void ExecuteOnTarget(IEnumerable<string> statements)
        {
            using (var connection = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                foreach (var statement in statements)
                {
                    connection.ExecuteNonQuery(statement);
                }
            }
        }

        /// <summary>
        /// Reads the schemas of the tables from the source database, composes each one on its own and creates it in the target database.
        /// The tables must be given in the order that they can be created (the referenced tables first).
        /// </summary>
        /// <param name="tableNames">The names of the tables.</param>
        public static void CopyToTarget(params string[] tableNames)
        {
            var composer = new PostgreSqlSchemaComposer();
            foreach (var schema in GetSourceSchemas(tableNames))
            {
                ExecuteOnTarget(composer.ComposeSchema(schema));
            }
        }

        /// <summary>
        /// Reads the schemas of the tables from the source database, composes all of them together and creates them in the target database.
        /// The tables can be given in any order.
        /// </summary>
        /// <param name="tableNames">The names of the tables.</param>
        public static void CopyAllToTarget(params string[] tableNames) =>
            ExecuteOnTarget(new PostgreSqlSchemaComposer().ComposeSchemas(GetSourceSchemas(tableNames)));

        /// <summary>
        /// Gets the names of the tables of the source database (<c>schema.table</c>).
        /// </summary>
        /// <returns>The names of the tables.</returns>
        public static List<string> GetSourceTables()
        {
            using (var connection = new NpgsqlConnection(Database.ConnectionStringForSource))
            {
                return new PostgreSqlSchemaReader(connection).GetTables().ToList();
            }
        }

        /// <summary>
        /// Gets the names of the tables of the target database (<c>schema.table</c>).
        /// </summary>
        /// <returns>The names of the tables.</returns>
        public static List<string> GetTargetTables()
        {
            using (var connection = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                return new PostgreSqlSchemaReader(connection).GetTables().ToList();
            }
        }

        /// <summary>
        /// Gets the names of the tables of the relationships, in the same order.
        /// </summary>
        /// <param name="relationships">The relationships.</param>
        /// <returns>The names of the tables.</returns>
        public static string[] GetTableNames(IEnumerable<RelationshipInfo> relationships) =>
            relationships.Select(r => r.Schema.Table.Name).ToArray();

        /// <summary>
        /// Asserts the equality of the 2 schemas of a table. The comments of the columns are not compared.
        /// </summary>
        /// <param name="expected">The expected schema.</param>
        /// <param name="actual">The actual schema.</param>
        public static void AssertSchemaEquality(TableSchema expected,
            TableSchema actual)
        {
            Assert.AreEqual(expected.Table.Name, actual.Table.Name, StringComparer.Ordinal);
            Assert.AreEqual(expected.Table.Schema, actual.Table.Schema, StringComparer.Ordinal);

            // Columns
            Assert.AreEqual(expected.Columns.Count, actual.Columns.Count);
            for (var i = 0; i < expected.Columns.Count; i++)
            {
                var (e, a) = (expected.Columns[i], actual.Columns[i]);
                var name = e.Field.Name;
                Assert.AreEqual(e.Field.Name, a.Field.Name, StringComparer.Ordinal);
                Assert.AreEqual(e.Ordinal, a.Ordinal, $"Ordinal of '{name}'.");
                Assert.AreEqual(e.Field.DatabaseType, a.Field.DatabaseType, StringComparer.Ordinal, $"Type of '{name}'.");
                Assert.AreEqual(e.Field.Size, a.Field.Size, $"Size of '{name}'.");
                Assert.AreEqual(e.Field.Precision, a.Field.Precision, $"Precision of '{name}'.");
                Assert.AreEqual(e.Field.Scale, a.Field.Scale, $"Scale of '{name}'.");
                Assert.AreEqual(e.Field.IsNullable, a.Field.IsNullable, $"Nullability of '{name}'.");
                Assert.AreEqual(e.Field.IsIdentity, a.Field.IsIdentity, $"Identity of '{name}'.");
                Assert.AreEqual(e.Field.IsPrimary, a.Field.IsPrimary, $"Primary of '{name}'.");
                Assert.AreEqual(e.IdentitySeed, a.IdentitySeed, $"Identity seed of '{name}'.");
                Assert.AreEqual(e.IdentityIncrement, a.IdentityIncrement, $"Identity increment of '{name}'.");
                Assert.AreEqual(e.DefaultExpression, a.DefaultExpression, StringComparer.Ordinal, $"Default of '{name}'.");
                Assert.AreEqual(e.ComputedExpression, a.ComputedExpression, StringComparer.Ordinal, $"Computed expression of '{name}'.");
            }

            // Primary key
            Assert.AreEqual(expected.PrimaryKey?.Name, actual.PrimaryKey?.Name, StringComparer.Ordinal);
            CollectionAssert.AreEqual(expected.PrimaryKey?.Columns.ToArray(), actual.PrimaryKey?.Columns.ToArray());

            // Indexes
            Assert.AreEqual(expected.Indexes.Count, actual.Indexes.Count);
            foreach (var index in expected.Indexes)
            {
                var other = actual.Indexes.Single(x => x.Name == index.Name);
                Assert.AreEqual(index.IsUnique, other.IsUnique);
                Assert.AreEqual(index.Filter, other.Filter, StringComparer.Ordinal);
                CollectionAssert.AreEqual(index.Columns.ToArray(), other.Columns.ToArray());
                CollectionAssert.AreEqual(index.DescendingColumns.ToArray(), other.DescendingColumns.ToArray());
                CollectionAssert.AreEqual(index.IncludedColumns.ToArray(), other.IncludedColumns.ToArray());
            }

            // Foreign keys
            Assert.AreEqual(expected.ForeignKeys.Count, actual.ForeignKeys.Count);
            foreach (var foreignKey in expected.ForeignKeys)
            {
                var other = actual.ForeignKeys.Single(x => x.Name == foreignKey.Name);
                CollectionAssert.AreEqual(foreignKey.Columns.ToArray(), other.Columns.ToArray());
                Assert.AreEqual(foreignKey.ReferencedTable, other.ReferencedTable, StringComparer.Ordinal);
                CollectionAssert.AreEqual(foreignKey.ReferencedColumns.ToArray(), other.ReferencedColumns.ToArray());
                Assert.AreEqual(foreignKey.UpdateRule, other.UpdateRule);
                Assert.AreEqual(foreignKey.DeleteRule, other.DeleteRule);
            }

            // Unique constraints
            Assert.AreEqual(expected.UniqueConstraints.Count, actual.UniqueConstraints.Count);
            foreach (var constraint in expected.UniqueConstraints)
            {
                CollectionAssert.AreEqual(constraint.Columns.ToArray(), actual.UniqueConstraints.Single(x => x.Name == constraint.Name).Columns.ToArray());
            }

            // Check constraints
            Assert.AreEqual(expected.CheckConstraints.Count, actual.CheckConstraints.Count);
            foreach (var constraint in expected.CheckConstraints)
            {
                Assert.AreEqual(constraint.Expression, actual.CheckConstraints.Single(x => x.Name == constraint.Name).Expression, StringComparer.Ordinal);
            }
        }
    }
}
