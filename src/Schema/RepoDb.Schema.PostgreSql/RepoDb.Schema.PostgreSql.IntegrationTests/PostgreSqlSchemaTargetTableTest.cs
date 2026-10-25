#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Npgsql;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema.Models;
using RepoDb.Schema.PostgreSql.IntegrationTests.Setup;

namespace RepoDb.Schema.PostgreSql.IntegrationTests
{
    [TestClass]
    public class PostgreSqlSchemaTargetTableTest
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

        private static void MapSchemaReaderConnection(NpgsqlConnection source) =>
            SchemaReaderMapper.Add<NpgsqlConnection>(new PostgreSqlSchemaReader(source), true);

        private static string[] Names(TableSchema schema) =>
            schema.Columns.Select(c => c.Field.Name).ToArray();

        #endregion

        #region Target Table

        [TestMethod]
        public void TestCopyTableSchemaToWithTargetTableCreatesTheTableByTheNewName()
        {
            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);

                // Act
                var result = source.CopyTableSchemaTo("country", target, targetTable: "nation").Items.Single();

                // Assert
                Assert.IsTrue(Helper.TargetTableExists("nation"));
                Assert.IsFalse(Helper.TargetTableExists("country"));
                Assert.AreEqual("country", result.Source.Table.Table.Name, StringComparer.Ordinal);
                Assert.AreEqual("nation", result.Target.Table.Table.Name, StringComparer.Ordinal);
                CollectionAssert.AreEqual(Names(Helper.GetSourceSchema("country")), Names(Helper.GetTargetSchema("nation")));
            }
        }

        [TestMethod]
        public async Task TestCopyTableSchemaToAsyncWithTargetTableCreatesTheTableByTheNewName()
        {
            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);

                // Act
                var result = (await source.CopyTableSchemaToAsync("country", target, targetTable: "nation")).Items.Single();

                // Assert
                Assert.IsTrue(Helper.TargetTableExists("nation"));
                Assert.IsFalse(Helper.TargetTableExists("country"));
                Assert.AreEqual("nation", result.Target.Table.Table.Name, StringComparer.Ordinal);
            }
        }

        [TestMethod]
        public void TestCopyTableSchemaToWithTargetTableAndColumnMappingsCreatesTheTableByTheNewNameWithTheNewColumns()
        {
            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                var mappings = new List<ColumnMapping> { new ColumnMapping("id", "NationId"), new ColumnMapping("name", "NationName") };

                // Act
                source.CopyTableSchemaTo("country", target, mappings, targetTable: "nation");

                // Assert
                Assert.IsTrue(Helper.TargetTableExists("nation"));
                CollectionAssert.AreEqual(new[] { "NationId", "NationName" }, Names(Helper.GetTargetSchema("nation")));
            }
        }

        [TestMethod]
        public void TestCopyTableSchemaToWithTableMappingsRenamesTheParentAndTheForeignKeyFollowsIt()
        {
            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                var mappings = new List<TableMapping>
                {
                    new TableMapping("country", "nation", new[] { new ColumnMapping("id"), new ColumnMapping("name") }),
                    new TableMapping("Person", null, new[] { new ColumnMapping("Id"), new ColumnMapping("Name"), new ColumnMapping("CountryId") })
                };

                // Act
                source.CopyTableSchemaTo(new[] { "country", "Person" }, target, mappings);

                // Assert
                Assert.IsTrue(Helper.TargetTableExists("nation"));
                Assert.IsTrue(Helper.TargetTableExists("Person"));
                Assert.IsFalse(Helper.TargetTableExists("country"));
                var foreignKey = Helper.GetTargetSchema("Person").ForeignKeys.Single();
                Assert.AreEqual("nation", foreignKey.ReferencedTable.Name, StringComparer.Ordinal);
                CollectionAssert.AreEqual(new[] { "CountryId" }, foreignKey.Columns.ToArray());
            }
        }

        [TestMethod]
        public void TestCopyTableSchemaToWithTargetTableRenamesTheRequestedTableAndTheChildTablesFollowIt()
        {
            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);

                // Act
                source.CopyTableSchemaTo("country", target, targetTable: "nation", relationshipBehavior: Enumerations.RelatedTableBehavior.Children);

                // Assert
                Assert.IsTrue(Helper.TargetTableExists("nation"));
                Assert.IsTrue(Helper.TargetTableExists("Person"));
                Assert.AreEqual("nation", Helper.GetTargetSchema("Person").ForeignKeys.Single().ReferencedTable.Name, StringComparer.Ordinal);
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCopyTableSchemaToWithMappingsOfATableThatIsNotCopiedAndListTheTablesFound()
        {
            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                var mappings = new List<TableMapping> { new TableMapping("nation", null, new[] { new ColumnMapping("id") }) };

                // Act
                var exception = Assert.Throws<ArgumentException>(() => source.CopyTableSchemaTo(new[] { "country" }, target, mappings));

                // Assert
                StringAssert.Contains(exception.Message, "The table 'nation' is not one of the tables that are copied.");
                StringAssert.Contains(exception.Message, "country");
            }
        }

        #endregion
    }
}
