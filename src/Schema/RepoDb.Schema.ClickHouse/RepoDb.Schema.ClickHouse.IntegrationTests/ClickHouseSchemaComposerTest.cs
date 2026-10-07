#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Linq;
using ClickHouse.Driver;
using ClickHouse.Driver.ADO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema.Models;
using RepoDb.Schema.ClickHouse.IntegrationTests.Setup;

namespace RepoDb.Schema.ClickHouse.IntegrationTests
{
    [TestClass]
    public class ClickHouseSchemaComposerTest
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
        public void TestClickHouseSchemaComposerComposedSchemaOfTableWithAllTheObjects()
        {
            // Act
            Helper.CopyToTarget("Country", "Person");

            // Assert
            Helper.AssertSchemaEquality(Helper.GetSourceSchema("Person"), Helper.GetTargetSchema("Person"));
        }

        [TestMethod]
        public void TestClickHouseSchemaComposerComposedSchemaOfTableWithCompositePrimaryKey()
        {
            // Act
            Helper.CopyToTarget("OrderLine");

            // Assert
            Helper.AssertSchemaEquality(Helper.GetSourceSchema("OrderLine"), Helper.GetTargetSchema("OrderLine"));
        }

        [TestMethod]
        public void TestClickHouseSchemaComposerComposedSchemaOfTableInAnotherSchema()
        {
            // Act
            Helper.CopyToTarget($"{Database.SourceSalesName}.Invoice");

            // Assert
            Helper.AssertSchemaEquality(Helper.GetSourceSchema($"{Database.SourceSalesName}.Invoice"), Helper.GetTargetSchema($"{Database.SourceSalesName}.Invoice"));
        }

        [TestMethod]
        public void TestClickHouseSchemaComposerComposedSchemaCreatesTheTableInTheTarget()
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
        public void TestClickHouseSchemaComposerComposedSchemaKeepsTheDefaultValues()
        {
            // Setup
            Helper.CopyToTarget("Country", "Person");

            using (var connection = new ClickHouseConnection(Database.ConnectionStringForTarget))
            {
                // Act
                connection.ExecuteNonQuery("INSERT INTO `Person` (`Name`) VALUES ('First');");
                var age = connection.ExecuteScalar<int>("SELECT `Age` FROM `Person`;");

                // Assert
                Assert.AreEqual(0, age);
            }
        }

        [TestMethod]
        public void TestClickHouseSchemaComposerComposedSchemaKeepsTheComputedColumn()
        {
            // Setup
            Helper.CopyToTarget("Country", "Person");

            using (var connection = new ClickHouseConnection(Database.ConnectionStringForTarget))
            {
                // Act
                connection.ExecuteNonQuery("INSERT INTO `Person` (`Name`) VALUES ('first');");
                var nameUpper = connection.ExecuteScalar<string>("SELECT `NameUpper` FROM `Person`;");

                // Assert
                Assert.AreEqual("FIRST", nameUpper, StringComparer.Ordinal);
            }
        }

        [TestMethod]
        public void TestClickHouseSchemaComposerComposedSchemaKeepsTheCheckConstraint()
        {
            // Setup
            Helper.CopyToTarget("Country", "Person");

            using (var connection = new ClickHouseConnection(Database.ConnectionStringForTarget))
            {
                // Act/Assert
                Assert.Throws<ClickHouseServerException>(() =>
                    connection.ExecuteNonQuery("INSERT INTO `Person` (`Name`, `Age`) VALUES ('First', -1);"));
            }
        }

        #endregion

        #region ComposeDropTable

        [TestMethod]
        public void TestClickHouseSchemaComposerComposedDropTable()
        {
            // Setup
            Helper.CopyToTarget("NoKey");
            var composer = new ClickHouseSchemaComposer();

            // Act
            using (var connection = new ClickHouseConnection(Database.ConnectionStringForTarget))
            {
                connection.ExecuteNonQuery(composer.ComposeDropTable("NoKey"));
            }

            // Assert
            Assert.IsFalse(Helper.TargetTableExists("NoKey"));
        }

        [TestMethod]
        public void TestClickHouseSchemaComposerComposedDropTableOfMissingTable()
        {
            // Setup
            var composer = new ClickHouseSchemaComposer();

            // Act/Assert
            using (var connection = new ClickHouseConnection(Database.ConnectionStringForTarget))
            {
                connection.ExecuteNonQuery(composer.ComposeDropTable("MissingTable"));
            }
        }

        #endregion

        #region ComposeAddColumn

        [TestMethod]
        public void TestClickHouseSchemaComposerComposedAddColumn()
        {
            // Setup
            Helper.CopyToTarget("NoKey");
            var composer = new ClickHouseSchemaComposer();
            var column = Helper.GetSourceSchema("Person").Columns.Single(c => c.Field.Name == "Salary");

            // Act
            using (var connection = new ClickHouseConnection(Database.ConnectionStringForTarget))
            {
                connection.ExecuteNonQuery(composer.ComposeAddColumn("NoKey", column));
            }

            // Assert
            var actual = Helper.GetTargetSchema("NoKey").Columns.Single(c => c.Field.Name == "Salary");
            Assert.AreEqual("Decimal", actual.Field.DatabaseType, StringComparer.Ordinal);
            Assert.AreEqual((byte)18, actual.Field.Precision);
            Assert.AreEqual((byte)2, actual.Field.Scale);
        }

        #endregion

        #region ComposeCreateIndex / ComposeAddForeignKey

        #endregion
    }
}
