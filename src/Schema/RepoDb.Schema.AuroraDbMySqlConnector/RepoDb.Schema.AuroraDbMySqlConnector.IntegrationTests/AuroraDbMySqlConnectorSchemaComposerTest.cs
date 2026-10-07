#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Linq;
using MySqlConnector;
using RepoDb.Connector.AuroraDb.MySqlConnector;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema.Models;
using RepoDb.Schema.AuroraDbMySqlConnector.IntegrationTests.Setup;

namespace RepoDb.Schema.AuroraDbMySqlConnector.IntegrationTests
{
    [TestClass]
    public class AuroraDbMySqlConnectorSchemaComposerTest
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
        public void TestAuroraDbMySqlConnectorSchemaComposerComposedSchemaOfTableWithIdentityAndKey()
        {
            // Act
            Helper.CopyToTarget("Country");

            // Assert
            Helper.AssertSchemaEquality(Helper.GetSourceSchema("Country"), Helper.GetTargetSchema("Country"));
        }

        [TestMethod]
        public void TestAuroraDbMySqlConnectorSchemaComposerComposedSchemaOfTableWithAllTheObjects()
        {
            // Act
            Helper.CopyToTarget("Country", "Person");

            // Assert
            Helper.AssertSchemaEquality(Helper.GetSourceSchema("Person"), Helper.GetTargetSchema("Person"));
        }

        [TestMethod]
        public void TestAuroraDbMySqlConnectorSchemaComposerComposedSchemaOfTableWithCompositePrimaryKey()
        {
            // Act
            Helper.CopyToTarget("OrderLine");

            // Assert
            Helper.AssertSchemaEquality(Helper.GetSourceSchema("OrderLine"), Helper.GetTargetSchema("OrderLine"));
        }

        [TestMethod]
        public void TestAuroraDbMySqlConnectorSchemaComposerComposedSchemaOfTableWithoutKey()
        {
            // Act
            Helper.CopyToTarget("NoKey");

            // Assert
            Helper.AssertSchemaEquality(Helper.GetSourceSchema("NoKey"), Helper.GetTargetSchema("NoKey"));
        }

        [TestMethod]
        public void TestAuroraDbMySqlConnectorSchemaComposerComposedSchemaOfTableInAnotherSchema()
        {
            // Act
            Helper.CopyToTarget($"{Database.SourceSalesName}.Invoice");

            // Assert
            Helper.AssertSchemaEquality(Helper.GetSourceSchema($"{Database.SourceSalesName}.Invoice"), Helper.GetTargetSchema($"{Database.SourceSalesName}.Invoice"));
        }

        [TestMethod]
        public void TestAuroraDbMySqlConnectorSchemaComposerComposedSchemaCreatesTheTableInTheTarget()
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
        public void TestAuroraDbMySqlConnectorSchemaComposerComposedSchemaKeepsTheAutoIncrement()
        {
            // Setup
            Helper.CopyToTarget("Country", "Person");

            using (var connection = new AuroraDbConnection(Database.ConnectionStringForTarget))
            {
                // Act
                connection.ExecuteNonQuery("INSERT INTO `Person` (`Name`) VALUES ('First'), ('Second');");
                var ids = connection.ExecuteQuery<long>("SELECT `Id` FROM `Person` ORDER BY `Id`;").ToArray();

                // Assert
                CollectionAssert.AreEqual(new[] { 1L, 2L }, ids);
            }
        }

        [TestMethod]
        public void TestAuroraDbMySqlConnectorSchemaComposerComposedSchemaKeepsTheDefaultValues()
        {
            // Setup
            Helper.CopyToTarget("Country", "Person");

            using (var connection = new AuroraDbConnection(Database.ConnectionStringForTarget))
            {
                // Act
                connection.ExecuteNonQuery("INSERT INTO `Person` (`Name`) VALUES ('First');");
                var age = connection.ExecuteScalar<int>("SELECT `Age` FROM `Person`;");

                // Assert
                Assert.AreEqual(0, age);
            }
        }

        [TestMethod]
        public void TestAuroraDbMySqlConnectorSchemaComposerComposedSchemaKeepsTheComputedColumn()
        {
            // Setup
            Helper.CopyToTarget("Country", "Person");

            using (var connection = new AuroraDbConnection(Database.ConnectionStringForTarget))
            {
                // Act
                connection.ExecuteNonQuery("INSERT INTO `Person` (`Name`) VALUES ('first');");
                var nameUpper = connection.ExecuteScalar<string>("SELECT `NameUpper` FROM `Person`;");

                // Assert
                Assert.AreEqual("FIRST", nameUpper, StringComparer.Ordinal);
            }
        }

        [TestMethod]
        public void TestAuroraDbMySqlConnectorSchemaComposerComposedSchemaKeepsTheCheckConstraint()
        {
            // Setup
            Helper.CopyToTarget("Country", "Person");

            using (var connection = new AuroraDbConnection(Database.ConnectionStringForTarget))
            {
                // Act/Assert
                Assert.Throws<AuroraDbException>(() =>
                    connection.ExecuteNonQuery("INSERT INTO `Person` (`Name`, `Age`) VALUES ('First', -1);"));
            }
        }

        [TestMethod]
        public void TestAuroraDbMySqlConnectorSchemaComposerComposedSchemaKeepsTheForeignKeyRules()
        {
            // Setup
            Helper.CopyToTarget("Country", "Person");

            using (var connection = new AuroraDbConnection(Database.ConnectionStringForTarget))
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
        public void TestAuroraDbMySqlConnectorSchemaComposerComposedDropTable()
        {
            // Setup
            Helper.CopyToTarget("NoKey");
            var composer = new AuroraDbMySqlConnectorSchemaComposer();

            // Act
            using (var connection = new AuroraDbConnection(Database.ConnectionStringForTarget))
            {
                connection.ExecuteNonQuery(composer.ComposeDropTable("NoKey"));
            }

            // Assert
            Assert.IsFalse(Helper.TargetTableExists("NoKey"));
        }

        [TestMethod]
        public void TestAuroraDbMySqlConnectorSchemaComposerComposedDropTableOfMissingTable()
        {
            // Setup
            var composer = new AuroraDbMySqlConnectorSchemaComposer();

            // Act/Assert
            using (var connection = new AuroraDbConnection(Database.ConnectionStringForTarget))
            {
                connection.ExecuteNonQuery(composer.ComposeDropTable("MissingTable"));
            }
        }

        #endregion

        #region ComposeAddColumn

        [TestMethod]
        public void TestAuroraDbMySqlConnectorSchemaComposerComposedAddColumn()
        {
            // Setup
            Helper.CopyToTarget("NoKey");
            var composer = new AuroraDbMySqlConnectorSchemaComposer();
            var column = Helper.GetSourceSchema("Person").Columns.Single(c => c.Field.Name == "Salary");

            // Act
            using (var connection = new AuroraDbConnection(Database.ConnectionStringForTarget))
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
        public void TestAuroraDbMySqlConnectorSchemaComposerComposedCreateIndexAndAddForeignKey()
        {
            // Setup
            var source = Helper.GetSourceSchema("Person");
            var composer = new AuroraDbMySqlConnectorSchemaComposer();
            Helper.CopyToTarget("Country");

            using (var connection = new AuroraDbConnection(Database.ConnectionStringForTarget))
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
