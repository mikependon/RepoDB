#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema.Models;
using RepoDb.Schema.MySql.IntegrationTests.Setup;

namespace RepoDb.Schema.MySql.IntegrationTests
{
    [TestClass]
    public class MySqlSchemaTargetTableTest
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
            SchemaReaderMapper.Clear();
            Database.Cleanup();
        }

        #region Helpers

        private static void MapSchemaReaderConnection(MySqlConnection source) =>
            SchemaReaderMapper.Add<MySqlConnection>(new MySqlSchemaReader(source), true);

        private static string[] Names(TableSchema schema) =>
            schema.Columns.Select(c => c.Field.Name).ToArray();

        #endregion

        #region Target Table

        [TestMethod]
        public void TestCopyTableSchemaToWithTargetTableCreatesTheTableByTheNewName()
        {
            using (var source = new MySqlConnection(Database.ConnectionStringForSource))
            using (var target = new MySqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);

                // Act
                var result = source.CopyTableSchemaTo("Country", target, targetTable: "Nation").Items.Single();

                // Assert
                Assert.IsTrue(Helper.TargetTableExists("Nation"));
                Assert.IsFalse(Helper.TargetTableExists("Country"));
                Assert.AreEqual("Country", result.Source.Table.Table.Name, StringComparer.Ordinal);
                Assert.AreEqual("Nation", result.Target.Table.Table.Name, StringComparer.Ordinal);
                CollectionAssert.AreEqual(Names(Helper.GetSourceSchema("Country")), Names(Helper.GetTargetSchema("Nation")));
            }
        }

        [TestMethod]
        public async Task TestCopyTableSchemaToAsyncWithTargetTableCreatesTheTableByTheNewName()
        {
            using (var source = new MySqlConnection(Database.ConnectionStringForSource))
            using (var target = new MySqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);

                // Act
                var result = (await source.CopyTableSchemaToAsync("Country", target, targetTable: "Nation")).Items.Single();

                // Assert
                Assert.IsTrue(Helper.TargetTableExists("Nation"));
                Assert.IsFalse(Helper.TargetTableExists("Country"));
                Assert.AreEqual("Nation", result.Target.Table.Table.Name, StringComparer.Ordinal);
            }
        }

        [TestMethod]
        public void TestCopyTableSchemaToWithTargetTableAndColumnMappingsCreatesTheTableByTheNewNameWithTheNewColumns()
        {
            using (var source = new MySqlConnection(Database.ConnectionStringForSource))
            using (var target = new MySqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                var mappings = new List<ColumnMapping> { new ColumnMapping("Id", "NationId"), new ColumnMapping("Name", "NationName") };

                // Act
                source.CopyTableSchemaTo("Country", target, mappings, targetTable: "Nation");

                // Assert
                Assert.IsTrue(Helper.TargetTableExists("Nation"));
                CollectionAssert.AreEqual(new[] { "NationId", "NationName" }, Names(Helper.GetTargetSchema("Nation")));
            }
        }

        [TestMethod]
        public void TestCopyTableSchemaToWithTableMappingsRenamesTheParentAndTheForeignKeyFollowsIt()
        {
            using (var source = new MySqlConnection(Database.ConnectionStringForSource))
            using (var target = new MySqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                var mappings = new List<TableMapping>
                {
                    new TableMapping("Country", "Nation", new[] { new ColumnMapping("Id"), new ColumnMapping("Name") }),
                    new TableMapping("Person", null, new[] { new ColumnMapping("Id"), new ColumnMapping("Name"), new ColumnMapping("CountryId") })
                };

                // Act
                source.CopyTableSchemaTo(new[] { "Country", "Person" }, target, mappings);

                // Assert
                Assert.IsTrue(Helper.TargetTableExists("Nation"));
                Assert.IsTrue(Helper.TargetTableExists("Person"));
                Assert.IsFalse(Helper.TargetTableExists("Country"));
                var foreignKey = Helper.GetTargetSchema("Person").ForeignKeys.Single();
                Assert.AreEqual("Nation", foreignKey.ReferencedTable.Name, StringComparer.Ordinal);
                CollectionAssert.AreEqual(new[] { "CountryId" }, foreignKey.Columns.ToArray());
            }
        }

        [TestMethod]
        public void TestCopyTableSchemaToWithTargetTableRenamesTheRequestedTableAndTheChildTablesFollowIt()
        {
            using (var source = new MySqlConnection(Database.ConnectionStringForSource))
            using (var target = new MySqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);

                // Act
                source.CopyTableSchemaTo("Country", target, targetTable: "Nation", relationshipBehavior: Enumerations.RelatedTableBehavior.Children);

                // Assert
                Assert.IsTrue(Helper.TargetTableExists("Nation"));
                Assert.IsTrue(Helper.TargetTableExists("Person"));
                Assert.AreEqual("Nation", Helper.GetTargetSchema("Person").ForeignKeys.Single().ReferencedTable.Name, StringComparer.Ordinal);
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCopyTableSchemaToWithMappingsOfATableThatIsNotCopiedAndListTheTablesFound()
        {
            using (var source = new MySqlConnection(Database.ConnectionStringForSource))
            using (var target = new MySqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                var mappings = new List<TableMapping> { new TableMapping("Nation", null, new[] { new ColumnMapping("Id") }) };

                // Act
                var exception = Assert.Throws<ArgumentException>(() => source.CopyTableSchemaTo(new[] { "Country" }, target, mappings));

                // Assert
                StringAssert.Contains(exception.Message, "The table 'Nation' is not one of the tables that are copied.");
                StringAssert.Contains(exception.Message, "Country");
            }
        }

        #endregion
    }
}
