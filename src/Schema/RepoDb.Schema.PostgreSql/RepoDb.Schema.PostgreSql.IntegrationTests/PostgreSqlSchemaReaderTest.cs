#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.
#endregion

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Npgsql;
using RepoDb.Schema.Enumerations;
using RepoDb.Schema.PostgreSql.IntegrationTests.Setup;
using RepoDb.Schema.Models;

namespace RepoDb.Schema.PostgreSql.IntegrationTests
{
    /// <summary>
    /// Contains the postgre sql schema reader test behavior.
    /// </summary>
    [TestClass]
    public class PostgreSqlSchemaReaderTest
    {
        /// <summary>
        /// Performs initialize.
        /// </summary>
        [TestInitialize]
        public void Initialize() => Database.Initialize();
        #region Private Methods

        /// <summary>
        /// Performs create reader.
        /// </summary>
        private static PostgreSqlSchemaReader CreateReader() => new PostgreSqlSchemaReader();
        #endregion

        #region GetTableSchema

        /// <summary>
        /// Verifies postgre sql schema reader get columns.
        /// </summary>
        [TestMethod]
        public void TestPostgreSqlSchemaReaderGetColumns()
        {
            using var operationConnection = new NpgsqlConnection(Database.ConnectionStringForSource); // Act
            var columns = CreateReader().GetColumns(operationConnection,
                "Person").ToList();
            // Assert
            CollectionAssert.AreEqual(new[] { "Id", "Name", "Age", "Salary", "CreatedAt", "CountryId", "Double", "Token", "Photo", "Active" },
                columns.Select(c => c.Field.Name).ToArray());
            Assert.IsTrue(columns.All(c => c.Field.Provider == "PostgreSql" || c.Field.Provider == null));
            var id = columns[0];
            Assert.AreEqual("bigint",
                id.Field.DatabaseType);
            Assert.IsTrue(id.Field.IsIdentity);
            Assert.IsTrue(id.Field.IsPrimary);
            Assert.IsFalse(id.Field.IsNullable);
            Assert.AreEqual(10,
                id.IdentitySeed);
            Assert.AreEqual(5,
                id.IdentityIncrement);
            Assert.AreEqual(typeof(long),
                id.Field.Type);
            var name=columns[1];
            Assert.AreEqual("character varying",
                name.Field.DatabaseType);
            Assert.AreEqual(50,
                name.Field.Size);
            Assert.AreEqual(typeof(string),
                name.Field.Type);
            var salary = columns[3];
            Assert.AreEqual("numeric",
                salary.Field.DatabaseType);
            Assert.AreEqual(12,
                (int)salary.Field.Precision);
            Assert.AreEqual(2,
                (int)salary.Field.Scale);
            Assert.AreEqual("0",
                salary.DefaultExpression);
            Assert.AreEqual(ColumnDefaultValue.Literal,
                salary.DefaultValue);
            Assert.IsTrue(salary.Field.IsNullable);
            var createdAt = columns[4];
            Assert.AreEqual("timestamp without time zone",
                createdAt.Field.DatabaseType);
            Assert.AreEqual(3,
                (int)createdAt.Field.Scale);
            Assert.AreEqual("now()",
                createdAt.DefaultExpression);
            Assert.AreEqual(ColumnDefaultValue.CurrentTimestamp,
                createdAt.DefaultValue);
            Assert.AreEqual("(\"Age\" * 2)",
                columns[6].ComputedExpression);
            Assert.AreEqual("true",
                columns[9].DefaultExpression);
            Assert.AreEqual(ColumnDefaultValue.BooleanTrue,
                columns[9].DefaultValue);
        }

        /// <summary>
        /// Verifies postgre sql schema reader get columns of serial column is identity.
        /// </summary>
        [TestMethod]
        public void TestPostgreSqlSchemaReaderGetColumnsOfSerialColumnIsIdentity()
        {
            using var operationConnection = new NpgsqlConnection(Database.ConnectionStringForSource); // Act
            var id = CreateReader().GetColumns(operationConnection,
                "ticket").First();
            // Assert
            Assert.IsTrue(id.Field.IsIdentity);
            Assert.IsNull(id.DefaultExpression);
            Assert.AreEqual(ColumnDefaultValue.None,
                id.DefaultValue);
            Assert.AreEqual(1,
                id.IdentitySeed);
            Assert.AreEqual(1,
                id.IdentityIncrement);
        }

        /// <summary>
        /// Verifies postgre sql schema reader get primary key.
        /// </summary>
        [TestMethod]
        public void TestPostgreSqlSchemaReaderGetPrimaryKey()
        {
            using var operationConnection = new NpgsqlConnection(Database.ConnectionStringForSource); // Act
            var primaryKey = CreateReader().GetPrimaryKey(operationConnection,
                "Person");
            // Assert
            Assert.AreEqual("pk_person",
                primaryKey.Name);
            CollectionAssert.AreEqual(new[] { "Id" },
                primaryKey.Columns.ToArray());
            Assert.IsNull(CreateReader().GetPrimaryKey(operationConnection,
                    "no_key"));
        }

        /// <summary>
        /// Verifies postgre sql schema reader get indexes.
        /// </summary>
        [TestMethod]
        public void TestPostgreSqlSchemaReaderGetIndexes()
        {
            using var operationConnection = new NpgsqlConnection(Database.ConnectionStringForSource); // Act
            var indexes = CreateReader().GetIndexes(operationConnection,
                "Person").ToList();
            // Assert
            Assert.AreEqual(2,
                indexes.Count);
            var name=indexes.Single(i => i.Name == "ix_person_name");
            Assert.IsFalse(name.IsUnique);
            CollectionAssert.AreEqual(new[] { "Name", "Age" },
                name.Columns.ToArray());
            CollectionAssert.AreEqual(new[] { "Name" },
                name.DescendingColumns.ToArray());
            CollectionAssert.AreEqual(new[] { "Salary" },
                name.IncludedColumns.ToArray());
            Assert.AreEqual("\"Active\"",
                name.Filter);
            var token = indexes.Single(i => i.Name == "ux_person_token");
            Assert.IsTrue(token.IsUnique);
            Assert.AreEqual("(\"Token\" IS NOT NULL)",
                token.Filter);
        }

        /// <summary>
        /// Verifies postgre sql schema reader get foreign keys.
        /// </summary>
        [TestMethod]
        public void TestPostgreSqlSchemaReaderGetForeignKeys()
        {
            using var operationConnection = new NpgsqlConnection(Database.ConnectionStringForSource); // Act
            var foreignKey = CreateReader().GetForeignKeys(operationConnection,
                "Person").Single();
            // Assert
            Assert.AreEqual("fk_person_country",
                foreignKey.Name);
            Assert.AreEqual(new TableInfo("country",
                    "public"),
                foreignKey.ReferencedTable);
            CollectionAssert.AreEqual(new[] { "CountryId" },
                foreignKey.Columns.ToArray());
            CollectionAssert.AreEqual(new[] { "id" },
                foreignKey.ReferencedColumns.ToArray());
            Assert.AreEqual(ForeignKeyRule.Cascade,
                foreignKey.DeleteRule);
            Assert.AreEqual(ForeignKeyRule.SetNull,
                foreignKey.UpdateRule);
        }

        /// <summary>
        /// Verifies postgre sql schema reader get unique and check constraints.
        /// </summary>
        [TestMethod]
        public void TestPostgreSqlSchemaReaderGetUniqueAndCheckConstraints()
        {
            using var operationConnection = new NpgsqlConnection(Database.ConnectionStringForSource); // Act
            var unique = CreateReader().GetUniqueConstraints(operationConnection,
                "country").Single();
            var check = CreateReader().GetCheckConstraints(operationConnection,
                "Person").Single();
            // Assert
            Assert.AreEqual("uq_country_name",
                unique.Name);
            CollectionAssert.AreEqual(new[] { "name" },
                unique.Columns.ToArray());
            Assert.AreEqual("ck_person_age",
                check.Name);
            Assert.AreEqual("(\"Age\" >= 0)",
                check.Expression);
        }

        /// <summary>
        /// Verifies postgre sql schema reader get table schema resolves the schema.
        /// </summary>
        [TestMethod]
        public void TestPostgreSqlSchemaReaderGetTableSchemaResolvesTheSchema()
        {
            using var operationConnection = new NpgsqlConnection(Database.ConnectionStringForSource); // Act
            var reader = CreateReader();
            var orderLine = reader.GetTableSchema(operationConnection,
                "order_line");
            var person = reader.GetTableSchema(operationConnection,
                "public.Person");
            // Assert
            Assert.AreEqual("sales",
                orderLine.Table.Schema);
            Assert.AreEqual("public",
                person.Table.Schema);
            Assert.AreEqual("Person",
                person.Table.Name);
            Assert.AreEqual(new TableInfo("Person",
                    "public"),
                reader.GetForeignKeys(operationConnection,
                    "sales.order_line").Single().ReferencedTable);
        }

        #endregion

        #region TableExists / GetTables / GetDependencyOrder

        /// <summary>
        /// Verifies postgre sql schema reader table exists.
        /// </summary>
        [TestMethod]
        public void TestPostgreSqlSchemaReaderTableExists()
        {
            using var operationConnection = new NpgsqlConnection(Database.ConnectionStringForSource); // Act
            var reader = CreateReader();
            // Assert
            Assert.IsTrue(reader.TableExists(operationConnection,
                    "Person"));
            Assert.IsTrue(reader.TableExists(operationConnection,
                    "sales.order_line"));
            Assert.IsFalse(reader.TableExists(operationConnection,
                    "person"));
            Assert.IsFalse(reader.TableExists(operationConnection,
                    "Missing"));
        }

        /// <summary>
        /// Verifies postgre sql schema reader get schemas.
        /// </summary>
        [TestMethod]
        public void TestPostgreSqlSchemaReaderGetSchemas()
        {
            using var operationConnection = new NpgsqlConnection(Database.ConnectionStringForSource); // Act
            var actual = CreateReader().GetSchemas(operationConnection).Select(schema => schema.Name).ToList();
            // Assert
            Assert.IsTrue(actual.Count > 0);
            CollectionAssert.Contains(actual,
                "public");
            CollectionAssert.Contains(actual,
                "sales");
            CollectionAssert.DoesNotContain(actual,
                "information_schema");
            CollectionAssert.DoesNotContain(actual,
                "pg_catalog");
        }

        /// <summary>
        /// Verifies postgre sql schema reader get schemas async.
        /// </summary>
        [TestMethod]
        public async Task TestPostgreSqlSchemaReaderGetSchemasAsync()
        {
            using var operationConnection = new NpgsqlConnection(Database.ConnectionStringForSource); // Act
            var actual = (await CreateReader().GetSchemasAsync(operationConnection)).Select(schema => schema.Name).ToList();
            // Assert
            Assert.IsTrue(actual.Count > 0);
            CollectionAssert.Contains(actual,
                "public");
            CollectionAssert.Contains(actual,
                "sales");
            CollectionAssert.DoesNotContain(actual,
                "information_schema");
            CollectionAssert.DoesNotContain(actual,
                "pg_catalog");
        }

        /// <summary>
        /// Verifies postgre sql schema reader get tables.
        /// </summary>
        [TestMethod]
        public void TestPostgreSqlSchemaReaderGetTables()
        {
            using var operationConnection = new NpgsqlConnection(Database.ConnectionStringForSource); // Act
            var all = CreateReader().GetTables(operationConnection).ToList();
            var sales = CreateReader().GetTables(operationConnection,
                "sales").ToList();
            // Assert
            CollectionAssert.IsSubsetOf(new[] { "public.\"Person\"", "public.country", "public.no_key", "public.node_a", "public.node_b", "public.ticket", "sales.order_line", "sales.item" },
                all);
            CollectionAssert.AreEquivalent(new[] { "sales.order_line", "sales.item" },
                sales);
        }

        /// <summary>
        /// Verifies postgre sql schema reader get dependency order.
        /// </summary>
        [TestMethod]
        public void TestPostgreSqlSchemaReaderGetDependencyOrder()
        {
            using var operationConnection = new NpgsqlConnection(Database.ConnectionStringForSource); // Act
            var order = CreateReader().GetDependencyOrder(operationConnection,
                new[] { "sales.order_line", "Person", "country" }).Select(r => r.Schema.Table.Name).ToArray();
            // Assert
            CollectionAssert.AreEqual(new[] { "country", "Person", "order_line" },
                order);
        }

        /// <summary>
        /// Verifies postgre sql schema reader get dependency order keeps the tables of acycle together.
        /// </summary>
        [TestMethod]
        public void TestPostgreSqlSchemaReaderGetDependencyOrderKeepsTheTablesOfACycleTogether()
        {
            using var operationConnection = new NpgsqlConnection(Database.ConnectionStringForSource); // Act
            var relationships = CreateReader().GetDependencyOrder(operationConnection,
                new[] { "node_b", "country", "node_a" }).ToList();
            // Assert
            CollectionAssert.AreEqual(new[] { "node_b", "node_a", "country" },
                relationships.Select(r => r.Schema.Table.Name).ToArray());
            Assert.AreEqual(1,
                relationships[0].Parents.Count);
        }

        #endregion

        #region Async

        /// <summary>
        /// Verifies postgre sql schema reader get table schema async.
        /// </summary>
        [TestMethod]
        public async Task TestPostgreSqlSchemaReaderGetTableSchemaAsync()
        {
            using var operationConnection = new NpgsqlConnection(Database.ConnectionStringForSource); // Act
            var reader = CreateReader();
            var actual = await reader.GetTableSchemaAsync(operationConnection,
                "Person");
            // Assert
            Helper.AssertSchemaEquality(reader.GetTableSchema(operationConnection,
                    "Person"),
                actual);
            Assert.IsTrue(await reader.TableExistsAsync(operationConnection,
                    "Person"));
            Assert.AreEqual("sales",
                await reader.GetSchemaNameAsync(operationConnection,
                    "order_line"));
            Assert.AreEqual(2,
                (await reader.GetTablesAsync(operationConnection,
                    "sales")).Count());
            Assert.AreEqual(3,
                (await reader.GetDependencyOrderAsync(operationConnection,
                    new[] { "order_line", "Person", "country" })).Count());
        }
        #endregion
    }
}
