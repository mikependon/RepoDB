#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Linq;
using Microsoft.Data.Sqlite;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema.Models;
using RepoDb.Schema.Sqlite.IntegrationTests.Setup;

namespace RepoDb.Schema.Sqlite.IntegrationTests
{
    [TestClass]
    public class SqliteSchemaComposerTest
    {
        [TestInitialize]
        public void Initialize()
        {
            Database.Initialize();
            Cleanup();
        }

        [TestCleanup]
        public void Cleanup()
        {
            Database.Cleanup();
        }

        #region ComposeSchema

        [TestMethod]
        public void TestSqliteSchemaComposerComposedSchemaOfTableWithIdentityAndKey()
        {
            // Act
            Helper.CopyToTarget("Country");

            // Assert
            Helper.AssertSchemaEquality(Helper.GetSourceSchema("Country"), Helper.GetTargetSchema("Country"));
        }

        [TestMethod]
        public void TestSqliteSchemaComposerComposedSchemaOfTableWithAllTheObjects()
        {
            // Act
            Helper.CopyToTarget("Country", "Person");

            // Assert
            Helper.AssertSchemaEquality(Helper.GetSourceSchema("Person"), Helper.GetTargetSchema("Person"));
        }

        [TestMethod]
        public void TestSqliteSchemaComposerComposedSchemaOfTableWithCompositePrimaryKey()
        {
            // Act
            Helper.CopyToTarget("OrderLine");

            // Assert
            Helper.AssertSchemaEquality(Helper.GetSourceSchema("OrderLine"), Helper.GetTargetSchema("OrderLine"));
        }

        [TestMethod]
        public void TestSqliteSchemaComposerComposedSchemaOfTableWithoutKey()
        {
            // Act
            Helper.CopyToTarget("NoKey");

            // Assert
            Helper.AssertSchemaEquality(Helper.GetSourceSchema("NoKey"), Helper.GetTargetSchema("NoKey"));
        }

        [TestMethod]
        public void TestSqliteSchemaComposerComposedSchemaOfTableInAnotherSchema()
        {
            // Act
            Helper.CopyToTarget($"{Database.SalesSchema}.Invoice");

            // Assert
            Helper.AssertSchemaEquality(Helper.GetSourceSchema($"{Database.SalesSchema}.Invoice"), Helper.GetTargetSchema($"{Database.SalesSchema}.Invoice"));
        }

        [TestMethod]
        public void TestSqliteSchemaComposerComposedSchemaCreatesTheTableInTheTarget()
        {
            // Setup
            Assert.IsFalse(Helper.TargetTableExists("Person"));

            // Act
            Helper.CopyToTarget("Country", "Person");

            // Assert
            Assert.IsTrue(Helper.TargetTableExists("Person"));
            Assert.IsTrue(Helper.TargetTableExists("Country"));
        }

        [TestMethod]
        public void TestSqliteSchemaComposerComposedSchemaKeepsTheAutoIncrement()
        {
            // Setup
            Helper.CopyToTarget("Country", "Person");

            using (var connection = Database.CreateTarget())
            {
                // Act
                connection.ExecuteNonQuery("INSERT INTO [Person] ([Name]) VALUES ('First'), ('Second');");
                var ids = connection.ExecuteQuery<long>("SELECT [Id] FROM [Person] ORDER BY [Id];").ToArray();

                // Assert
                CollectionAssert.AreEqual(new[] { 1L, 2L }, ids);
            }
        }

        [TestMethod]
        public void TestSqliteSchemaComposerComposedSchemaKeepsTheDefaultValues()
        {
            // Setup
            Helper.CopyToTarget("Country", "Person");

            using (var connection = Database.CreateTarget())
            {
                // Act
                connection.ExecuteNonQuery("INSERT INTO [Person] ([Name]) VALUES ('First');");
                var age = connection.ExecuteScalar<int>("SELECT [Age] FROM [Person];");

                // Assert
                Assert.AreEqual(0, age);
            }
        }

        [TestMethod]
        public void TestSqliteSchemaComposerComposedSchemaKeepsTheComputedColumn()
        {
            // Setup
            Helper.CopyToTarget("Country", "Person");

            using (var connection = Database.CreateTarget())
            {
                // Act
                connection.ExecuteNonQuery("INSERT INTO [Person] ([Name]) VALUES ('first');");
                var nameUpper = connection.ExecuteScalar<string>("SELECT [NameUpper] FROM [Person];");

                // Assert
                Assert.AreEqual("FIRST", nameUpper, StringComparer.Ordinal);
            }
        }

        [TestMethod]
        public void TestSqliteSchemaComposerComposedSchemaKeepsTheCheckConstraint()
        {
            // Setup
            Helper.CopyToTarget("Country", "Person");

            using (var connection = Database.CreateTarget())
            {
                // Act/Assert
                Assert.Throws<SqliteException>(() =>
                    connection.ExecuteNonQuery("INSERT INTO [Person] ([Name], [Age]) VALUES ('First', -1);"));
            }
        }

        [TestMethod]
        public void TestSqliteSchemaComposerComposedSchemaKeepsTheForeignKeyRules()
        {
            // Setup
            Helper.CopyToTarget("Country", "Person");

            using (var connection = Database.CreateTarget())
            {
                connection.ExecuteNonQuery("INSERT INTO [Country] ([Name]) VALUES ('Philippines');");
                connection.ExecuteNonQuery("INSERT INTO [Person] ([Name], [CountryId]) VALUES ('First', 1);");

                // Act
                connection.ExecuteNonQuery("DELETE FROM [Country];");
                var countryId = connection.ExecuteScalar<int?>("SELECT [CountryId] FROM [Person];");

                // Assert
                Assert.IsNull(countryId);
            }
        }

        #endregion

        #region ComposeDropTable

        [TestMethod]
        public void TestSqliteSchemaComposerComposedDropTable()
        {
            // Setup
            Helper.CopyToTarget("NoKey");
            var composer = new SqliteSchemaComposer();

            // Act
            using (var connection = Database.CreateTarget())
            {
                connection.ExecuteNonQuery(composer.ComposeDropTable("NoKey"));
            }

            // Assert
            Assert.IsFalse(Helper.TargetTableExists("NoKey"));
        }

        [TestMethod]
        public void TestSqliteSchemaComposerComposedDropTableOfMissingTable()
        {
            // Setup
            var composer = new SqliteSchemaComposer();

            // Act/Assert
            using (var connection = Database.CreateTarget())
            {
                connection.ExecuteNonQuery(composer.ComposeDropTable("MissingTable"));
            }
        }

        #endregion

        #region ComposeAddColumn

        [TestMethod]
        public void TestSqliteSchemaComposerComposedAddColumn()
        {
            // Setup
            Helper.CopyToTarget("NoKey");
            var composer = new SqliteSchemaComposer();
            var column = Helper.GetSourceSchema("Person").Columns.Single(c => c.Field.Name == "Salary");

            // Act
            using (var connection = Database.CreateTarget())
            {
                connection.ExecuteNonQuery(composer.ComposeAddColumn("NoKey", column));
            }

            // Assert
            var actual = Helper.GetTargetSchema("NoKey").Columns.Single(c => c.Field.Name == "Salary");
            Assert.AreEqual("decimal", actual.Field.DatabaseType, StringComparer.Ordinal);
            Assert.AreEqual((byte)18, actual.Field.Precision);
            Assert.AreEqual((byte)2, actual.Field.Scale);
        }

        #endregion

        #region ComposeCreateIndex / ComposeAddForeignKey

        [TestMethod]
        public void TestSqliteSchemaComposerComposedCreateIndexAndTheForeignKeysOfTheTable()
        {
            // Setup
            var source = Helper.GetSourceSchema("Person");
            var composer = new SqliteSchemaComposer();
            Helper.CopyToTarget("Country");

            using (var connection = Database.CreateTarget())
            {
                // Act
                var withoutIndexes = new TableSchema(source.Table.Name, source.Table.Schema)
                {
                    PrimaryKey = source.PrimaryKey,
                    Columns = source.Columns,
                    CheckConstraints = source.CheckConstraints,
                    ForeignKeys = source.ForeignKeys
                };
                connection.ExecuteNonQuery(composer.ComposeCreateTable(withoutIndexes));
                connection.ExecuteNonQuery(composer.ComposeCreateIndex("Person", source.Indexes.Single()));
            }

            // Assert
            Assert.AreEqual(string.Empty, composer.ComposeAddForeignKey("Person", source.ForeignKeys.Single()));
            Helper.AssertSchemaEquality(source, Helper.GetTargetSchema("Person"));
        }

        #endregion
    }
}
