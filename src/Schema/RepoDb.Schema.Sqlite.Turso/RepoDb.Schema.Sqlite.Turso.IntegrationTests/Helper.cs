#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using Turso.Data.Sqlite;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema.Models;
using RepoDb.Schema.Sqlite.Turso.IntegrationTests.Setup;

namespace RepoDb.Schema.Sqlite.Turso.IntegrationTests
{
    public static class Helper
    {
        /// <summary>
        /// Makes the schema copies stop checking what exists in the destination database, so they always try to create the tables.
        /// The next call of <see cref="Database.Initialize"/> registers the composer again.
        /// </summary>
        public static void DisableExistenceChecks() =>
            SchemaComposerMapper.Add<SqliteConnection>(new NoExistenceCheckSchemaComposer(new TursoSchemaComposer()), force: true);

        /// <summary>
        /// Creates a table in the target database that owns the name of an index, so the statement that creates the index with the same name fails.
        /// </summary>
        /// <param name="indexName">The name of the index.</param>
        public static void BlockIndexName(string indexName)
        {
            using (var connection = Database.CreateTarget())
            {
                connection.ExecuteNonQuery("CREATE TABLE [Blocker] ([Id] INT);");
                connection.ExecuteNonQuery($"CREATE INDEX [{indexName}] ON [Blocker] ([Id]);");
            }
        }

        /// <summary>
        /// Reads the schema of the table from the source database.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <returns>The <see cref="TableSchema"/> of the table.</returns>
        public static TableSchema GetSourceSchema(string tableName)
        {
            using (var connection = Database.CreateSource())
            {
                return new TursoSchemaReader(connection).GetTableSchema(tableName);
            }
        }

        /// <summary>
        /// Reads the schema of the table from the target database.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <returns>The <see cref="TableSchema"/> of the table.</returns>
        public static TableSchema GetTargetSchema(string tableName)
        {
            using (var connection = Database.CreateTarget())
            {
                var schema = new TursoSchemaReader(connection).GetTableSchema(tableName);
                return schema;
            }
        }

        /// <summary>
        /// Checks whether the table exists in the target database.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <returns><c>true</c> if the table exists; otherwise, <c>false</c>.</returns>
        public static bool TargetTableExists(string tableName)
        {
            using (var connection = Database.CreateTarget())
            {
                return new TursoSchemaReader(connection).TableExists(tableName);
            }
        }

        /// <summary>
        /// Reads the schema of the tables from the source database, composes it and creates it in the target database.
        /// The tables must be given in the order that they can be created (the referenced tables first).
        /// </summary>
        /// <param name="tableNames">The names of the tables.</param>
        public static void CopyToTarget(params string[] tableNames)
        {
            var composer = new TursoSchemaComposer();
            using (var source = Database.CreateSource())
            using (var target = Database.CreateTarget())
            {
                var reader = new TursoSchemaReader(source);
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
        /// Reads the schemas of the tables from the source database.
        /// </summary>
        /// <param name="tableNames">The names of the tables.</param>
        /// <returns>The <see cref="TableSchema"/> objects of the tables, in the same order.</returns>
        public static List<TableSchema> GetSourceSchemas(IEnumerable<string> tableNames)
        {
            using (var connection = Database.CreateSource())
            {
                var reader = new TursoSchemaReader(connection);
                return tableNames.Select(reader.GetTableSchema).ToList();
            }
        }

        /// <summary>
        /// Executes the statements, in order, on the target database.
        /// </summary>
        /// <param name="statements">The SQL statements.</param>
        public static void ExecuteOnTarget(IEnumerable<string> statements)
        {
            using (var connection = Database.CreateTarget())
            {
                foreach (var statement in statements)
                {
                    connection.ExecuteNonQuery(statement);
                }
            }
        }

        /// <summary>
        /// Reads the schemas of the tables from the source database, composes all of them together and creates them in the target database.
        /// The tables can be given in any order.
        /// </summary>
        /// <param name="tableNames">The names of the tables.</param>
        public static void CopyAllToTarget(params string[] tableNames) =>
            ExecuteOnTarget(new TursoSchemaComposer().ComposeSchemas(GetSourceSchemas(tableNames)));

        /// <summary>
        /// Asserts the equality of the 2 schemas of a table. The comments of the columns and the names of the unnamed constraints are not compared.
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
                AssertColumnEquality(expected.Columns[i], actual.Columns[i]);
            }

            // Primary key
            if (expected.PrimaryKey == null)
            {
                Assert.IsNull(actual.PrimaryKey);
            }
            else
            {
                Assert.IsNotNull(actual.PrimaryKey);
                Assert.AreEqual(expected.PrimaryKey.Name, actual.PrimaryKey.Name, StringComparer.Ordinal);
                Assert.AreEqual(expected.PrimaryKey.IsClustered, actual.PrimaryKey.IsClustered);
                CollectionAssert.AreEqual(expected.PrimaryKey.Columns.ToArray(), actual.PrimaryKey.Columns.ToArray());
            }

            // Indexes
            Assert.AreEqual(expected.Indexes.Count, actual.Indexes.Count);
            foreach (var index in expected.Indexes)
            {
                var other = actual.Indexes.Single(x => string.Equals(x.Name, index.Name, StringComparison.Ordinal));
                Assert.AreEqual(index.IsUnique, other.IsUnique);
                Assert.AreEqual(index.IsClustered, other.IsClustered);
                Assert.AreEqual(index.Filter, other.Filter, StringComparer.Ordinal);
                CollectionAssert.AreEqual(index.Columns.ToArray(), other.Columns.ToArray());
                CollectionAssert.AreEqual(index.DescendingColumns.ToArray(), other.DescendingColumns.ToArray());
                CollectionAssert.AreEqual(index.IncludedColumns.ToArray(), other.IncludedColumns.ToArray());
            }

            // Foreign keys
            Assert.AreEqual(expected.ForeignKeys.Count, actual.ForeignKeys.Count);
            foreach (var foreignKey in expected.ForeignKeys)
            {
                var other = actual.ForeignKeys.Single(x => string.Equals(x.Name, foreignKey.Name, StringComparison.Ordinal));
                CollectionAssert.AreEqual(foreignKey.Columns.ToArray(), other.Columns.ToArray());
                Assert.AreEqual(foreignKey.ReferencedTable, other.ReferencedTable);
                CollectionAssert.AreEqual(foreignKey.ReferencedColumns.ToArray(), other.ReferencedColumns.ToArray());
                Assert.AreEqual(foreignKey.UpdateRule, other.UpdateRule);
                Assert.AreEqual(foreignKey.DeleteRule, other.DeleteRule);
            }

            // Unique constraints
            Assert.AreEqual(expected.UniqueConstraints.Count, actual.UniqueConstraints.Count);
            foreach (var constraint in expected.UniqueConstraints)
            {
                var other = actual.UniqueConstraints.Single(x => string.Equals(x.Name, constraint.Name, StringComparison.Ordinal));
                CollectionAssert.AreEqual(constraint.Columns.ToArray(), other.Columns.ToArray());
            }

            // Check constraints
            Assert.AreEqual(expected.CheckConstraints.Count, actual.CheckConstraints.Count);
            foreach (var constraint in expected.CheckConstraints)
            {
                var other = actual.CheckConstraints.Single(x => string.Equals(x.Name, constraint.Name, StringComparison.Ordinal));
                Assert.AreEqual(constraint.Expression, other.Expression, StringComparer.Ordinal);
            }
        }

        /// <summary>
        /// Asserts the equality of the 2 columns. The comments are not compared.
        /// </summary>
        /// <param name="expected">The expected column.</param>
        /// <param name="actual">The actual column.</param>
        public static void AssertColumnEquality(ColumnInfo expected,
            ColumnInfo actual)
        {
            var name = expected.Field.Name;
            Assert.AreEqual(expected.Field.Name, actual.Field.Name, StringComparer.Ordinal);
            Assert.AreEqual(expected.Ordinal, actual.Ordinal, $"Ordinal of '{name}'.");
            Assert.AreEqual(expected.Field.DatabaseType, actual.Field.DatabaseType, StringComparer.Ordinal, $"Type of '{name}'.");
            Assert.AreEqual(expected.Field.Size, actual.Field.Size, $"Size of '{name}'.");
            Assert.AreEqual(expected.Field.Precision, actual.Field.Precision, $"Precision of '{name}'.");
            Assert.AreEqual(expected.Field.Scale, actual.Field.Scale, $"Scale of '{name}'.");
            Assert.AreEqual(expected.Field.IsNullable, actual.Field.IsNullable, $"Nullability of '{name}'.");
            Assert.AreEqual(expected.Field.IsIdentity, actual.Field.IsIdentity, $"Identity of '{name}'.");
            Assert.AreEqual(expected.Field.IsPrimary, actual.Field.IsPrimary, $"Primary of '{name}'.");
            Assert.AreEqual(expected.IdentitySeed, actual.IdentitySeed, $"Identity seed of '{name}'.");
            Assert.AreEqual(expected.IdentityIncrement, actual.IdentityIncrement, $"Identity increment of '{name}'.");
            Assert.AreEqual(expected.DefaultExpression, actual.DefaultExpression, StringComparer.Ordinal, $"Default of '{name}'.");
            Assert.AreEqual(expected.ComputedExpression, actual.ComputedExpression, StringComparer.Ordinal, $"Computed expression of '{name}'.");
            Assert.AreEqual(expected.Collation, actual.Collation, StringComparer.Ordinal, $"Collation of '{name}'.");
        }

        /// <summary>
        /// Asserts that the table of the target database has the same schema as the table of the source database.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        public static void AssertTargetMatchesSource(string tableName) =>
            AssertSchemaEquality(GetSourceSchema(tableName), GetTargetSchema(tableName));

        /// <summary>
        /// Gets the names (<c>schema.table</c>) of the tables of the relationships, in the same order.
        /// </summary>
        /// <param name="relationships">The relationships.</param>
        /// <returns>The names of the tables.</returns>
        public static string[] GetTableNames(IEnumerable<RelationshipInfo> relationships) =>
            relationships.Select(r => FormatName(r.Schema.Table.Schema, r.Schema.Table.Name)).ToArray();

        /// <summary>
        /// Formats the name of a table like the reader does: the plain identifiers are kept as they are (<c>Person</c>),
        /// and the other ones are quoted (<c>sales."Order Details"</c>).
        /// </summary>
        /// <param name="schema">The name of the schema.</param>
        /// <param name="table">The name of the table.</param>
        /// <returns>The formatted name of the table.</returns>
        public static string FormatName(string schema, string table)
        {
            string Part(string name) =>
                name.Length > 0 && !char.IsDigit(name[0]) && name.All(c => char.IsLetterOrDigit(c) || c == '_')
                    ? name
                    : $"[{name}]";
            return schema == null ? Part(table) : $"{Part(schema)}.{Part(table)}";
        }

        /// <summary>
        /// Gets the names of the columns.
        /// </summary>
        /// <param name="columns">The columns.</param>
        /// <returns>The names of the columns.</returns>
        public static string[] GetNames(IEnumerable<ColumnInfo> columns) =>
            columns.Select(c => c.Field.Name).ToArray();
    }
}
