#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Linq;
using MySql.Data.MySqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema.Models;
using RepoDb.Schema.MySql.IntegrationTests.Setup;

namespace RepoDb.Schema.MySql.IntegrationTests
{
    [TestClass]
    public class MySqlSchemaComposerTest
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
        public void TestMySqlSchemaComposerComposedSchemaOfTableWithIdentityAndKey()
        {
            // Act
            Helper.CopyToTarget("Country");

            // Assert
            Helper.AssertSchemaEquality(Helper.GetSourceSchema("Country"), Helper.GetTargetSchema("Country"));
        }

        [TestMethod]
        public void TestMySqlSchemaComposerComposedSchemaOfTableWithAllTheObjects()
        {
            // Act
            Helper.CopyToTarget("Country", "Person");

            // Assert
            Helper.AssertSchemaEquality(Helper.GetSourceSchema("Person"), Helper.GetTargetSchema("Person"));
        }

        [TestMethod]
        public void TestMySqlSchemaComposerComposedSchemaOfTableWithCompositePrimaryKey()
        {
            // Act
            Helper.CopyToTarget("OrderLine");

            // Assert
            Helper.AssertSchemaEquality(Helper.GetSourceSchema("OrderLine"), Helper.GetTargetSchema("OrderLine"));
        }

        [TestMethod]
        public void TestMySqlSchemaComposerComposedSchemaOfTableWithoutKey()
        {
            // Act
            Helper.CopyToTarget("NoKey");

            // Assert
            Helper.AssertSchemaEquality(Helper.GetSourceSchema("NoKey"), Helper.GetTargetSchema("NoKey"));
        }

        [TestMethod]
        public void TestMySqlSchemaComposerComposedSchemaOfTableInAnotherSchema()
        {
            // Act
            Helper.CopyToTarget($"{Database.SourceSalesName}.Invoice");

            // Assert
            Helper.AssertSchemaEquality(Helper.GetSourceSchema($"{Database.SourceSalesName}.Invoice"), Helper.GetTargetSchema($"{Database.SourceSalesName}.Invoice"));
        }

        [TestMethod]
        public void TestMySqlSchemaComposerComposedSchemaCreatesTheTableInTheTarget()
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
        public void TestMySqlSchemaComposerComposedSchemaKeepsTheAutoIncrement()
        {
            // Setup
            Helper.CopyToTarget("Country", "Person");

            using (var connection = new MySqlConnection(Database.ConnectionStringForTarget))
            {
                // Act
                connection.ExecuteNonQuery("INSERT INTO `Person` (`Name`) VALUES ('First'), ('Second');");
                var ids = connection.ExecuteQuery<long>("SELECT `Id` FROM `Person` ORDER BY `Id`;").ToArray();

                // Assert
                CollectionAssert.AreEqual(new[] { 1L, 2L }, ids);
            }
        }

        [TestMethod]
        public void TestMySqlSchemaComposerComposedSchemaKeepsTheDefaultValues()
        {
            // Setup
            Helper.CopyToTarget("Country", "Person");

            using (var connection = new MySqlConnection(Database.ConnectionStringForTarget))
            {
                // Act
                connection.ExecuteNonQuery("INSERT INTO `Person` (`Name`) VALUES ('First');");
                var age = connection.ExecuteScalar<int>("SELECT `Age` FROM `Person`;");

                // Assert
                Assert.AreEqual(0, age);
            }
        }

        [TestMethod]
        public void TestMySqlSchemaComposerComposedSchemaKeepsTheComputedColumn()
        {
            // Setup
            Helper.CopyToTarget("Country", "Person");

            using (var connection = new MySqlConnection(Database.ConnectionStringForTarget))
            {
                // Act
                connection.ExecuteNonQuery("INSERT INTO `Person` (`Name`) VALUES ('first');");
                var nameUpper = connection.ExecuteScalar<string>("SELECT `NameUpper` FROM `Person`;");

                // Assert
                Assert.AreEqual("FIRST", nameUpper, StringComparer.Ordinal);
            }
        }

        [TestMethod]
        public void TestMySqlSchemaComposerComposedSchemaKeepsTheCheckConstraint()
        {
            // Setup
            Helper.CopyToTarget("Country", "Person");

            using (var connection = new MySqlConnection(Database.ConnectionStringForTarget))
            {
                // Act/Assert
                Assert.Throws<MySqlException>(() =>
                    connection.ExecuteNonQuery("INSERT INTO `Person` (`Name`, `Age`) VALUES ('First', -1);"));
            }
        }

        [TestMethod]
        public void TestMySqlSchemaComposerComposedSchemaKeepsTheForeignKeyRules()
        {
            // Setup
            Helper.CopyToTarget("Country", "Person");

            using (var connection = new MySqlConnection(Database.ConnectionStringForTarget))
            {
                connection.ExecuteNonQuery("INSERT INTO `Country` (`Name`) VALUES ('Philippines');");
                connection.ExecuteNonQuery("INSERT INTO `Person` (`Name`, `CountryId`) VALUES ('First', 1);");

                // Act
                connection.ExecuteNonQuery("DELETE FROM `Country`;");
                var countryId = connection.ExecuteScalar<int?>("SELECT `CountryId` FROM `Person`;");

                // Assert
                Assert.IsNull(countryId);
            }
        }

        #endregion

        #region ComposeDropTable

        [TestMethod]
        public void TestMySqlSchemaComposerComposedDropTable()
        {
            // Setup
            Helper.CopyToTarget("NoKey");
            var composer = new MySqlSchemaComposer();

            // Act
            using (var connection = new MySqlConnection(Database.ConnectionStringForTarget))
            {
                connection.ExecuteNonQuery(composer.ComposeDropTable("NoKey"));
            }

            // Assert
            Assert.IsFalse(Helper.TargetTableExists("NoKey"));
        }

        [TestMethod]
        public void TestMySqlSchemaComposerComposedDropTableOfMissingTable()
        {
            // Setup
            var composer = new MySqlSchemaComposer();

            // Act/Assert
            using (var connection = new MySqlConnection(Database.ConnectionStringForTarget))
            {
                connection.ExecuteNonQuery(composer.ComposeDropTable("MissingTable"));
            }
        }

        #endregion

        #region ComposeAddColumn

        [TestMethod]
        public void TestMySqlSchemaComposerComposedAddColumn()
        {
            // Setup
            Helper.CopyToTarget("NoKey");
            var composer = new MySqlSchemaComposer();
            var column = Helper.GetSourceSchema("Person").Columns.Single(c => c.Field.Name == "Salary");

            // Act
            using (var connection = new MySqlConnection(Database.ConnectionStringForTarget))
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
        public void TestMySqlSchemaComposerComposedCreateIndexAndAddForeignKey()
        {
            // Setup
            var source = Helper.GetSourceSchema("Person");
            var composer = new MySqlSchemaComposer();
            Helper.CopyToTarget("Country");

            using (var connection = new MySqlConnection(Database.ConnectionStringForTarget))
            {
                // Act
                var withoutIndexesAndForeignKeys = new TableSchema(source.Table.Name, source.Table.Schema)
                {
                    PrimaryKey = source.PrimaryKey,
                    Columns = source.Columns,
                    CheckConstraints = source.CheckConstraints
                };
                connection.ExecuteNonQuery(composer.ComposeCreateTable(withoutIndexesAndForeignKeys));
                connection.ExecuteNonQuery(composer.ComposeCreateIndex("Person", source.Indexes.Single()));
                connection.ExecuteNonQuery(composer.ComposeAddForeignKey("Person", source.ForeignKeys.Single()));
            }

            // Assert
            Helper.AssertSchemaEquality(source, Helper.GetTargetSchema("Person"));
        }

        #endregion
    }
}
