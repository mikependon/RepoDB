#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema.Models;
using RepoDb.Schema.SqlServer.IntegrationTests.Setup;

namespace RepoDb.Schema.SqlServer.IntegrationTests
{
    /// <summary>
    /// Contains the sql server schema target table test behavior.
    /// </summary>
    [TestClass]
    public class SqlServerSchemaTargetTableTest
    {
        /// <summary>
        /// Performs initialize.
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            Database.Initialize();
            Cleanup();
        }

        /// <summary>
        /// Performs cleanup.
        /// </summary>
        [TestCleanup]
        public void Cleanup()
        {
            SchemaReaderMapper.Clear();
            Database.Cleanup();
        }

        #region Private Methods

        /// <summary>
        /// Performs map schema reader connection.
        /// </summary>
        /// <param name="source">The source.</param>
        private static void MapSchemaReaderConnection(SqlConnection source) => SchemaReaderMapper.Add<SqlConnection>(new SqlServerSchemaReader(),
            true);

        /// <summary>
        /// Performs names.
        /// </summary>
        /// <param name="schema">The schema.</param>
        private static string[] Names(TableSchema schema) => schema.Columns.Select(c => c.Field.Name).ToArray();
        #endregion

        #region Target Table

        /// <summary>
        /// Verifies copy table schema to with target table creates the table by the new name.
        /// </summary>
        [TestMethod]
        public void TestCopyTableSchemaToWithTargetTableCreatesTheTableByTheNewName()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                // Act
                var result = source.CopyTableSchemaTo("Country",
                    target,
                    targetTable: "Nation").Items.Single();
                // Assert
                Assert.IsTrue(Helper.TargetTableExists("Nation"));
                Assert.IsFalse(Helper.TargetTableExists("Country"));
                Assert.AreEqual("Country",
                    result.Source.Table.Table.Name,
                    StringComparer.Ordinal);
                Assert.AreEqual("Nation",
                    result.Target.Table.Table.Name,
                    StringComparer.Ordinal);
                CollectionAssert.AreEqual(Names(Helper.GetSourceSchema("Country")),
                    Names(Helper.GetTargetSchema("Nation")));
            }
        }

        /// <summary>
        /// Verifies copy table schema to async with target table creates the table by the new name.
        /// </summary>
        [TestMethod]
        public async Task TestCopyTableSchemaToAsyncWithTargetTableCreatesTheTableByTheNewName()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                // Act
                var result = (await source.CopyTableSchemaToAsync("Country",
                    target,
                    targetTable: "Nation")).Items.Single();
                // Assert
                Assert.IsTrue(Helper.TargetTableExists("Nation"));
                Assert.IsFalse(Helper.TargetTableExists("Country"));
                Assert.AreEqual("Nation",
                    result.Target.Table.Table.Name,
                    StringComparer.Ordinal);
            }
        }

        /// <summary>
        /// Verifies copy table schema to with target table and column mappings creates the table by the new name with the new columns.
        /// </summary>
        [TestMethod]
        public void TestCopyTableSchemaToWithTargetTableAndColumnMappingsCreatesTheTableByTheNewNameWithTheNewColumns()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                var mappings = new List<ColumnMapping>
                {
                    new ColumnMapping("Id",
                        "NationId"),
                    new ColumnMapping("Name",
                        "NationName")
                };
                // Act
                source.CopyTableSchemaTo("Country",
                    target,
                    mappings,
                    targetTable: "Nation");
                // Assert
                Assert.IsTrue(Helper.TargetTableExists("Nation"));
                CollectionAssert.AreEqual(new[] { "NationId", "NationName" },
                    Names(Helper.GetTargetSchema("Nation")));
            }
        }

        /// <summary>
        /// Verifies copy table schema to with table mappings renames the parent and the foreign key follows it.
        /// </summary>
        [TestMethod]
        public void TestCopyTableSchemaToWithTableMappingsRenamesTheParentAndTheForeignKeyFollowsIt()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                var mappings = new List<TableMapping>
                {
                    new TableMapping("Country",
                        "Nation",
                        new[] { new ColumnMapping("Id"), new ColumnMapping("Name") }),
                    new TableMapping("Person",
                        null,
                        new[] { new ColumnMapping("Id"), new ColumnMapping("Name"), new ColumnMapping("CountryId") })
                };
                // Act
                source.CopyTableSchemaTo(new[] { "Country", "Person" },
                    target,
                    mappings);
                // Assert
                Assert.IsTrue(Helper.TargetTableExists("Nation"));
                Assert.IsTrue(Helper.TargetTableExists("Person"));
                Assert.IsFalse(Helper.TargetTableExists("Country"));
                var foreignKey = Helper.GetTargetSchema("Person").ForeignKeys.Single();
                Assert.AreEqual("Nation",
                    foreignKey.ReferencedTable.Name,
                    StringComparer.Ordinal);
                CollectionAssert.AreEqual(new[] { "CountryId" },
                    foreignKey.Columns.ToArray());
            }
        }

        /// <summary>
        /// Verifies copy table schema to with target table renames the requested table and the child tables follow it.
        /// </summary>
        [TestMethod]
        public void TestCopyTableSchemaToWithTargetTableRenamesTheRequestedTableAndTheChildTablesFollowIt()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                // Act
                source.CopyTableSchemaTo("Country",
                    target,
                    targetTable: "Nation",
                    relationshipBehavior: Enumerations.RelatedTableBehavior.Children);
                // Assert
                Assert.IsTrue(Helper.TargetTableExists("Nation"));
                Assert.IsTrue(Helper.TargetTableExists("Person"));
                Assert.AreEqual("Nation",
                    Helper.GetTargetSchema("Person").ForeignKeys.Single().ReferencedTable.Name,
                    StringComparer.Ordinal);
            }
        }

        /// <summary>
        /// Verifies throw exception on copy table schema to with mappings of atable that is not copied and list the tables found.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnCopyTableSchemaToWithMappingsOfATableThatIsNotCopiedAndListTheTablesFound()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                var mappings = new List<TableMapping>
                {
                    new TableMapping("Nation",
                        null,
                        new[] { new ColumnMapping("Id") })
                };
                // Act
                var exception = Assert.Throws<ArgumentException>(() => source.CopyTableSchemaTo(new[] { "Country" },
                        target,
                        mappings));
                // Assert
                StringAssert.Contains(exception.Message,
                    "The table 'Nation' is not one of the tables that are copied.");
                StringAssert.Contains(exception.Message,
                    "Country");
            }
        }
        #endregion
    }
}
