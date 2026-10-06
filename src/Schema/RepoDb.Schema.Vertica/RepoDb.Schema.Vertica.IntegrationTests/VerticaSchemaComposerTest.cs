#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Linq;
using Vertica.Data.VerticaClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema.Models;
using RepoDb.Schema.Vertica.IntegrationTests.Setup;

namespace RepoDb.Schema.Vertica.IntegrationTests
{
    [TestClass]
    public class VerticaSchemaComposerTest
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
        public void TestVerticaSchemaComposerComposedSchemaOfTableWithIdentityAndKey()
        {
            // Act
            Helper.CopyToTarget("Country");

            // Assert
            Helper.AssertSchemaEquality(Helper.GetSourceSchema("Country"), Helper.GetTargetSchema("Country"));
        }

        [TestMethod]
        public void TestVerticaSchemaComposerComposedSchemaOfTableWithAllTheObjects()
        {
            // Act
            Helper.CopyToTarget("Country", "Person");

            // Assert
            Helper.AssertSchemaEquality(Helper.GetSourceSchema("Person"), Helper.GetTargetSchema("Person"));
        }

        [TestMethod]
        public void TestVerticaSchemaComposerComposedSchemaOfTableWithCompositePrimaryKey()
        {
            // Act
            Helper.CopyToTarget("OrderLine");

            // Assert
            Helper.AssertSchemaEquality(Helper.GetSourceSchema("OrderLine"), Helper.GetTargetSchema("OrderLine"));
        }

        [TestMethod]
        public void TestVerticaSchemaComposerComposedSchemaOfTableWithoutKey()
        {
            // Act
            Helper.CopyToTarget("NoKey");

            // Assert
            Helper.AssertSchemaEquality(Helper.GetSourceSchema("NoKey"), Helper.GetTargetSchema("NoKey"));
        }

        [TestMethod]
        public void TestVerticaSchemaComposerComposedSchemaOfTableInAnotherSchema()
        {
            // Act
            Helper.CopyToTarget($"{Database.SourceSalesName}.Invoice");

            // Assert
            Helper.AssertSchemaEquality(Helper.GetSourceSchema($"{Database.SourceSalesName}.Invoice"), Helper.GetTargetSchema($"{Database.SourceSalesName}.Invoice"));
        }

        [TestMethod]
        public void TestVerticaSchemaComposerComposedSchemaCreatesTheTableInTheTarget()
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
        public void TestVerticaSchemaComposerComposedSchemaKeepsTheIdentitySeedAndIncrement()
        {
            // Setup
            Helper.CopyToTarget("Country", "Person");

            using (var connection = new VerticaConnection(Database.ConnectionStringForTarget))
            {
                // Act
                connection.ExecuteNonQuery("INSERT INTO \"Person\" (\"Name\") VALUES ('First'), ('Second')");
                var ids = connection.ExecuteQuery<long>("SELECT \"Id\" FROM \"Person\" ORDER BY \"Id\"").ToArray();

                // Assert
                CollectionAssert.AreEqual(new[] { 10L, 15L }, ids);
            }
        }

        [TestMethod]
        public void TestVerticaSchemaComposerComposedSchemaKeepsTheDefaultValues()
        {
            // Setup
            Helper.CopyToTarget("Country", "Person");

            using (var connection = new VerticaConnection(Database.ConnectionStringForTarget))
            {
                // Act
                connection.ExecuteNonQuery("INSERT INTO \"Person\" (\"Name\") VALUES ('First')");
                var age = connection.ExecuteScalar<int>("SELECT \"Age\" FROM \"Person\"");

                // Assert
                Assert.AreEqual(0, age);
            }
        }

        [TestMethod]
        public void TestVerticaSchemaComposerComposedSchemaKeepsTheCheckConstraint()
        {
            // Setup
            Helper.CopyToTarget("Country", "Person");

            using (var connection = new VerticaConnection(Database.ConnectionStringForTarget))
            {
                // Act/Assert
                Assert.Throws<VerticaException>(() =>
                    connection.ExecuteNonQuery("INSERT INTO \"Person\" (\"Name\", \"Age\") VALUES ('First', -1)"));
            }
        }

        #endregion

        #region ComposeDropTable

        [TestMethod]
        public void TestVerticaSchemaComposerComposedDropTable()
        {
            // Setup
            Helper.CopyToTarget("NoKey");
            var composer = new VerticaSchemaComposer();

            // Act
            using (var connection = new VerticaConnection(Database.ConnectionStringForTarget))
            {
                connection.ExecuteNonQuery(composer.ComposeDropTable("NoKey"));
            }

            // Assert
            Assert.IsFalse(Helper.TargetTableExists("NoKey"));
        }

        [TestMethod]
        public void TestVerticaSchemaComposerComposedDropTableOfMissingTable()
        {
            // Setup
            var composer = new VerticaSchemaComposer();

            // Act/Assert
            using (var connection = new VerticaConnection(Database.ConnectionStringForTarget))
            {
                connection.ExecuteNonQuery(composer.ComposeDropTable("MissingTable"));
            }
        }

        #endregion

        #region ComposeAddColumn

        [TestMethod]
        public void TestVerticaSchemaComposerComposedAddColumn()
        {
            // Setup
            Helper.CopyToTarget("NoKey");
            var composer = new VerticaSchemaComposer();
            var column = Helper.GetSourceSchema("Person").Columns.Single(c => c.Field.Name == "Salary");

            // Act
            using (var connection = new VerticaConnection(Database.ConnectionStringForTarget))
            {
                connection.ExecuteNonQuery(composer.ComposeAddColumn("NoKey", column));
            }

            // Assert
            var actual = Helper.GetTargetSchema("NoKey").Columns.Single(c => c.Field.Name == "Salary");
            Assert.AreEqual("numeric", actual.Field.DatabaseType, StringComparer.Ordinal);
            Assert.AreEqual((byte)18, actual.Field.Precision);
            Assert.AreEqual((byte)2, actual.Field.Scale);
        }

        #endregion

        #region ComposeCreateIndex / ComposeAddForeignKey

        #endregion
    }
}
