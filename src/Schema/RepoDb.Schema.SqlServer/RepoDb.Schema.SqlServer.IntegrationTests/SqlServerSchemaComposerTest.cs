#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Linq;
using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema.Models;
using RepoDb.Schema.SqlServer.IntegrationTests.Setup;

namespace RepoDb.Schema.SqlServer.IntegrationTests
{
    [TestClass]
    public class SqlServerSchemaComposerTest
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
        public void TestSqlServerSchemaComposerComposedSchemaOfTableWithIdentityAndKey()
        {
            // Act
            Helper.CopyToTarget("Country");

            // Assert
            Helper.AssertSchemaEquality(Helper.GetSourceSchema("Country"), Helper.GetTargetSchema("Country"));
        }

        [TestMethod]
        public void TestSqlServerSchemaComposerComposedSchemaOfTableWithAllTheObjects()
        {
            // Act
            Helper.CopyToTarget("Country", "Person");

            // Assert
            Helper.AssertSchemaEquality(Helper.GetSourceSchema("Person"), Helper.GetTargetSchema("Person"));
        }

        [TestMethod]
        public void TestSqlServerSchemaComposerComposedSchemaOfTableWithCompositePrimaryKey()
        {
            // Act
            Helper.CopyToTarget("OrderLine");

            // Assert
            Helper.AssertSchemaEquality(Helper.GetSourceSchema("OrderLine"), Helper.GetTargetSchema("OrderLine"));
        }

        [TestMethod]
        public void TestSqlServerSchemaComposerComposedSchemaOfTableWithoutKey()
        {
            // Act
            Helper.CopyToTarget("NoKey");

            // Assert
            Helper.AssertSchemaEquality(Helper.GetSourceSchema("NoKey"), Helper.GetTargetSchema("NoKey"));
        }

        [TestMethod]
        public void TestSqlServerSchemaComposerComposedSchemaOfTableInAnotherSchema()
        {
            // Act
            Helper.CopyToTarget("Sales.Invoice");

            // Assert
            Helper.AssertSchemaEquality(Helper.GetSourceSchema("Sales.Invoice"), Helper.GetTargetSchema("Sales.Invoice"));
        }

        [TestMethod]
        public void TestSqlServerSchemaComposerComposedSchemaCreatesTheTableInTheTarget()
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
        public void TestSqlServerSchemaComposerComposedSchemaKeepsTheIdentitySeedAndIncrement()
        {
            // Setup
            Helper.CopyToTarget("Country", "Person");

            using (var connection = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Act
                connection.ExecuteNonQuery("INSERT INTO [dbo].[Person] ([Name]) VALUES (N'First'), (N'Second');");
                var ids = connection.ExecuteQuery<long>("SELECT [Id] FROM [dbo].[Person] ORDER BY [Id];").ToArray();

                // Assert
                CollectionAssert.AreEqual(new[] { 10L, 15L }, ids);
            }
        }

        [TestMethod]
        public void TestSqlServerSchemaComposerComposedSchemaKeepsTheDefaultValues()
        {
            // Setup
            Helper.CopyToTarget("Country", "Person");

            using (var connection = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Act
                connection.ExecuteNonQuery("INSERT INTO [dbo].[Person] ([Name]) VALUES (N'First');");
                var age = connection.ExecuteScalar<int>("SELECT [Age] FROM [dbo].[Person];");

                // Assert
                Assert.AreEqual(0, age);
            }
        }

        [TestMethod]
        public void TestSqlServerSchemaComposerComposedSchemaKeepsTheComputedColumn()
        {
            // Setup
            Helper.CopyToTarget("Country", "Person");

            using (var connection = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Act
                connection.ExecuteNonQuery("INSERT INTO [dbo].[Person] ([Name]) VALUES (N'first');");
                var nameUpper = connection.ExecuteScalar<string>("SELECT [NameUpper] FROM [dbo].[Person];");

                // Assert
                Assert.AreEqual("FIRST", nameUpper, StringComparer.Ordinal);
            }
        }

        [TestMethod]
        public void TestSqlServerSchemaComposerComposedSchemaKeepsTheCheckConstraint()
        {
            // Setup
            Helper.CopyToTarget("Country", "Person");

            using (var connection = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Act/Assert
                Assert.Throws<SqlException>(() =>
                    connection.ExecuteNonQuery("INSERT INTO [dbo].[Person] ([Name], [Age]) VALUES (N'First', -1);"));
            }
        }

        [TestMethod]
        public void TestSqlServerSchemaComposerComposedSchemaKeepsTheForeignKeyRules()
        {
            // Setup
            Helper.CopyToTarget("Country", "Person");

            using (var connection = new SqlConnection(Database.ConnectionStringForTarget))
            {
                connection.ExecuteNonQuery("INSERT INTO [dbo].[Country] ([Name]) VALUES (N'Philippines');");
                connection.ExecuteNonQuery("INSERT INTO [dbo].[Person] ([Name], [CountryId]) VALUES (N'First', 1);");

                // Act (ON DELETE SET NULL)
                connection.ExecuteNonQuery("DELETE FROM [dbo].[Country];");
                var countryId = connection.ExecuteScalar<int?>("SELECT [CountryId] FROM [dbo].[Person];");

                // Assert
                Assert.IsNull(countryId);
            }
        }

        #endregion

        #region ComposeDropTable

        [TestMethod]
        public void TestSqlServerSchemaComposerComposedDropTable()
        {
            // Setup
            Helper.CopyToTarget("NoKey");
            var composer = new SqlServerSchemaComposer();

            // Act
            using (var connection = new SqlConnection(Database.ConnectionStringForTarget))
            {
                connection.ExecuteNonQuery(composer.ComposeDropTable("NoKey"));
            }

            // Assert
            Assert.IsFalse(Helper.TargetTableExists("NoKey"));
        }

        [TestMethod]
        public void TestSqlServerSchemaComposerComposedDropTableOfMissingTable()
        {
            // Setup
            var composer = new SqlServerSchemaComposer();

            // Act/Assert
            using (var connection = new SqlConnection(Database.ConnectionStringForTarget))
            {
                connection.ExecuteNonQuery(composer.ComposeDropTable("dbo.MissingTable"));
            }
        }

        #endregion

        #region ComposeAddColumn

        [TestMethod]
        public void TestSqlServerSchemaComposerComposedAddColumn()
        {
            // Setup
            Helper.CopyToTarget("NoKey");
            var composer = new SqlServerSchemaComposer();
            var column = Helper.GetSourceSchema("Person").Columns.Single(c => c.Field.Name == "Salary");

            // Act
            using (var connection = new SqlConnection(Database.ConnectionStringForTarget))
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
        public void TestSqlServerSchemaComposerComposedCreateIndexAndAddForeignKey()
        {
            // Setup
            var source = Helper.GetSourceSchema("Person");
            var composer = new SqlServerSchemaComposer();
            Helper.CopyToTarget("Country");

            using (var connection = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Act
                var withoutIndexesAndForeignKeys = new TableSchema
                {
                    Table = new TableInfo { Name = source.Table.Name, Schema = source.Table.Schema },
                    PrimaryKey = source.PrimaryKey,
                    Columns = source.Columns,
                    CheckConstraints = source.CheckConstraints
                };
                connection.ExecuteNonQuery(composer.ComposeCreateTable(withoutIndexesAndForeignKeys));
                connection.ExecuteNonQuery(composer.ComposeCreateIndex("dbo.Person", source.Indexes.Single()));
                connection.ExecuteNonQuery(composer.ComposeAddForeignKey("dbo.Person", source.ForeignKeys.Single()));
            }

            // Assert
            Helper.AssertSchemaEquality(source, Helper.GetTargetSchema("Person"));
        }

        #endregion
    }
}
