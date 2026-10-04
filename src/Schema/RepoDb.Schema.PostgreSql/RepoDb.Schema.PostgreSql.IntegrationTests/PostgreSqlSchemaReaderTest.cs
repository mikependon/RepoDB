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

namespace RepoDb.Schema.PostgreSql.IntegrationTests
{
    [TestClass]
    public class PostgreSqlSchemaReaderTest
    {
        [TestInitialize]
        public void Initialize() =>
            Database.Initialize();

        #region Helpers

        private static PostgreSqlSchemaReader CreateReader() =>
            new PostgreSqlSchemaReader(new NpgsqlConnection(Database.ConnectionStringForSource));

        #endregion

        #region GetTableSchema

        [TestMethod]
        public void TestPostgreSqlSchemaReaderGetColumns()
        {
            // Act
            var columns = CreateReader().GetColumns("Person").ToList();

            // Assert
            CollectionAssert.AreEqual(new[] { "Id", "Name", "Age", "Salary", "CreatedAt", "CountryId", "Double", "Token", "Photo", "Active" }, columns.Select(c => c.Field.Name).ToArray());
            Assert.IsTrue(columns.All(c => c.Field.Provider == "PostgreSql" || c.Field.Provider == null));

            var id = columns[0];
            Assert.AreEqual("bigint", id.Field.DatabaseType);
            Assert.IsTrue(id.Field.IsIdentity);
            Assert.IsTrue(id.Field.IsPrimary);
            Assert.IsFalse(id.Field.IsNullable);
            Assert.AreEqual(10, id.IdentitySeed);
            Assert.AreEqual(5, id.IdentityIncrement);
            Assert.AreEqual(typeof(long), id.Field.Type);

            var name = columns[1];
            Assert.AreEqual("character varying", name.Field.DatabaseType);
            Assert.AreEqual(50, name.Field.Size);
            Assert.AreEqual(typeof(string), name.Field.Type);

            var salary = columns[3];
            Assert.AreEqual("numeric", salary.Field.DatabaseType);
            Assert.AreEqual(12, (int)salary.Field.Precision);
            Assert.AreEqual(2, (int)salary.Field.Scale);
            Assert.AreEqual("0", salary.DefaultExpression);
            Assert.IsTrue(salary.Field.IsNullable);

            var createdAt = columns[4];
            Assert.AreEqual("timestamp without time zone", createdAt.Field.DatabaseType);
            Assert.AreEqual(3, (int)createdAt.Field.Scale);
            Assert.AreEqual("now()", createdAt.DefaultExpression);

            Assert.AreEqual("(\"Age\" * 2)", columns[6].ComputedExpression);
            Assert.AreEqual("true", columns[9].DefaultExpression);
        }

        [TestMethod]
        public void TestPostgreSqlSchemaReaderGetColumnsOfSerialColumnIsIdentity()
        {
            // Act
            var id = CreateReader().GetColumns("ticket").First();

            // Assert (the sequence is not copied, so the column is an identity column without a default)
            Assert.IsTrue(id.Field.IsIdentity);
            Assert.IsNull(id.DefaultExpression);
            Assert.AreEqual(1, id.IdentitySeed);
            Assert.AreEqual(1, id.IdentityIncrement);
        }

        [TestMethod]
        public void TestPostgreSqlSchemaReaderGetPrimaryKey()
        {
            // Act
            var primaryKey = CreateReader().GetPrimaryKey("Person");

            // Assert
            Assert.AreEqual("pk_person", primaryKey.Name);
            CollectionAssert.AreEqual(new[] { "Id" }, primaryKey.Columns.ToArray());
            Assert.IsNull(CreateReader().GetPrimaryKey("no_key"));
        }

        [TestMethod]
        public void TestPostgreSqlSchemaReaderGetIndexes()
        {
            // Act
            var indexes = CreateReader().GetIndexes("Person").ToList();

            // Assert (the primary key is not an index)
            Assert.AreEqual(2, indexes.Count);
            var name = indexes.Single(i => i.Name == "ix_person_name");
            Assert.IsFalse(name.IsUnique);
            CollectionAssert.AreEqual(new[] { "Name", "Age" }, name.Columns.ToArray());
            CollectionAssert.AreEqual(new[] { "Name" }, name.DescendingColumns.ToArray());
            CollectionAssert.AreEqual(new[] { "Salary" }, name.IncludedColumns.ToArray());
            Assert.AreEqual("\"Active\"", name.Filter);
            var token = indexes.Single(i => i.Name == "ux_person_token");
            Assert.IsTrue(token.IsUnique);
            Assert.AreEqual("(\"Token\" IS NOT NULL)", token.Filter);
        }

        [TestMethod]
        public void TestPostgreSqlSchemaReaderGetForeignKeys()
        {
            // Act
            var foreignKey = CreateReader().GetForeignKeys("Person").Single();

            // Assert
            Assert.AreEqual("fk_person_country", foreignKey.Name);
            Assert.AreEqual("public.country", foreignKey.ReferencedTable);
            CollectionAssert.AreEqual(new[] { "CountryId" }, foreignKey.Columns.ToArray());
            CollectionAssert.AreEqual(new[] { "id" }, foreignKey.ReferencedColumns.ToArray());
            Assert.AreEqual(ForeignKeyRule.Cascade, foreignKey.DeleteRule);
            Assert.AreEqual(ForeignKeyRule.SetNull, foreignKey.UpdateRule);
        }

        [TestMethod]
        public void TestPostgreSqlSchemaReaderGetUniqueAndCheckConstraints()
        {
            // Act
            var unique = CreateReader().GetUniqueConstraints("country").Single();
            var check = CreateReader().GetCheckConstraints("Person").Single();

            // Assert
            Assert.AreEqual("uq_country_name", unique.Name);
            CollectionAssert.AreEqual(new[] { "name" }, unique.Columns.ToArray());
            Assert.AreEqual("ck_person_age", check.Name);
            Assert.AreEqual("(\"Age\" >= 0)", check.Expression);
        }

        [TestMethod]
        public void TestPostgreSqlSchemaReaderGetTableSchemaResolvesTheSchema()
        {
            // Act
            var reader = CreateReader();
            var orderLine = reader.GetTableSchema("order_line");
            var person = reader.GetTableSchema("public.Person");

            // Assert
            Assert.AreEqual("sales", orderLine.Table.Schema);
            Assert.AreEqual("public", person.Table.Schema);
            Assert.AreEqual("Person", person.Table.Name);
            Assert.AreEqual("public.\"Person\"", reader.GetForeignKeys("sales.order_line").Single().ReferencedTable);
        }

        #endregion

        #region TableExists / GetTables / GetDependencyOrder

        [TestMethod]
        public void TestPostgreSqlSchemaReaderTableExists()
        {
            // Act
            var reader = CreateReader();

            // Assert
            Assert.IsTrue(reader.TableExists("Person"));
            Assert.IsTrue(reader.TableExists("sales.order_line"));
            Assert.IsFalse(reader.TableExists("person"));
            Assert.IsFalse(reader.TableExists("Missing"));
        }

        [TestMethod]
        public void TestPostgreSqlSchemaReaderGetTables()
        {
            // Act
            var all = CreateReader().GetTables().ToList();
            var sales = CreateReader().GetTables("sales").ToList();

            // Assert
            CollectionAssert.IsSubsetOf(new[] { "public.\"Person\"", "public.country", "public.no_key", "public.node_a", "public.node_b", "public.ticket", "sales.order_line", "sales.item" }, all);
            CollectionAssert.AreEquivalent(new[] { "sales.order_line", "sales.item" }, sales);
        }

        [TestMethod]
        public void TestPostgreSqlSchemaReaderGetDependencyOrder()
        {
            // Act
            var order = CreateReader().GetDependencyOrder(new[] { "sales.order_line", "Person", "country" }).Select(r => r.Schema.Table.Name).ToArray();

            // Assert
            CollectionAssert.AreEqual(new[] { "country", "Person", "order_line" }, order);
        }

        [TestMethod]
        public void TestPostgreSqlSchemaReaderGetDependencyOrderKeepsTheTablesOfACycleTogether()
        {
            // Act
            var relationships = CreateReader().GetDependencyOrder(new[] { "node_b", "country", "node_a" }).ToList();

            // Assert
            CollectionAssert.AreEqual(new[] { "node_b", "node_a", "country" }, relationships.Select(r => r.Schema.Table.Name).ToArray());
            Assert.AreEqual(1, relationships[0].Parents.Count);
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestPostgreSqlSchemaReaderGetTableSchemaAsync()
        {
            // Act
            var reader = CreateReader();
            var actual = await reader.GetTableSchemaAsync("Person");

            // Assert
            Helper.AssertSchemaEquality(reader.GetTableSchema("Person"), actual);
            Assert.IsTrue(await reader.TableExistsAsync("Person"));
            Assert.AreEqual("sales", await reader.GetSchemaNameAsync("order_line"));
            Assert.AreEqual(2, (await reader.GetTablesAsync("sales")).Count());
            Assert.AreEqual(3, (await reader.GetDependencyOrderAsync(new[] { "order_line", "Person", "country" })).Count());
        }

        #endregion
    }
}
