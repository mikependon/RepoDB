#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Npgsql;
using RepoDb.Connector.AuroraDb.Npgsql;
using RepoDb.Schema.Models;
using RepoDb.Schema.Enumerations;
using RepoDb.Schema.AuroraDbPostgreSql.IntegrationTests.Setup;

namespace RepoDb.Schema.AuroraDbPostgreSql.IntegrationTests
{
    [TestClass]
    public class AuroraDbPostgreSqlCopyTableSchemaToTest
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

        private static void MapSchemaReaderConnection(AuroraDbConnection source) =>
            SchemaReaderMapper.Add<AuroraDbConnection>(new AuroraDbPostgreSqlSchemaReader(source), true);

        #endregion

        #region CopyTableSchemaTo

        [TestMethod]
        public void TestAuroraDbPostgreSqlCopyTableSchemaTo()
        {
            using (var source = new AuroraDbConnection(Database.ConnectionStringForSource))
            using (var target = new AuroraDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                source.CopyTableSchemaTo("country", target);

                // Act
                var result = source.CopyTableSchemaTo("Person", target).Items.Single();

                // Assert
                Helper.AssertTargetMatchesSource("country");
                Helper.AssertTargetMatchesSource("Person");
                Assert.AreEqual("Person", result.Target.Table.Table.Name);
                Assert.AreEqual(CopyTableSchemaOutcome.Created, result.Outcome);
                Assert.AreEqual(10, result.ColumnCount);
                Assert.AreEqual(2, result.IndexCount);
                Assert.AreEqual(1, result.ForeignKeyCount);
            }
        }

        [TestMethod]
        public void TestAuroraDbPostgreSqlCopyTableSchemaToSerialAndWithoutKey()
        {
            using (var source = new AuroraDbConnection(Database.ConnectionStringForSource))
            using (var target = new AuroraDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);

                // Act
                source.CopyTableSchemaTo("ticket", target);
                source.CopyTableSchemaTo("no_key", target);

                // Assert
                Helper.AssertTargetMatchesSource("ticket");
                Helper.AssertTargetMatchesSource("no_key");
            }
        }

        [TestMethod]
        public void TestAuroraDbPostgreSqlCopyTableSchemaToAcceptsTheSchemaOfTheTable()
        {
            using (var source = new AuroraDbConnection(Database.ConnectionStringForSource))
            using (var target = new AuroraDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                source.CopyTableSchemaTo("country", target);
                source.CopyTableSchemaTo("Person", target);

                // Act
                var result = source.CopyTableSchemaTo("sales.order_line", target).Items.Single();

                // Assert
                Assert.AreEqual("sales", result.Source.Table.Table.Schema);
                Assert.AreEqual("public", result.Target.Table.Table.Schema);
                Assert.IsTrue(Helper.TargetTableExists("public.order_line"));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnAuroraDbPostgreSqlCopyTableSchemaToIfTheTableAlreadyExists()
        {
            using (var source = new AuroraDbConnection(Database.ConnectionStringForSource))
            using (var target = new AuroraDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                source.CopyTableSchemaTo("country", target);

                // Act/Assert
                Assert.Throws<InvalidOperationException>(() => source.CopyTableSchemaTo("country", target, tableExistenceBehavior: CopyTableSchemaExistsBehavior.Throw));
            }
        }

        #endregion

        #region CopyTableSchemaTo (Multiple Tables)

        [TestMethod]
        public void TestAuroraDbPostgreSqlCopyTableSchemaToWithTheTablesInAnyOrderAndWithCycles()
        {
            using (var source = new AuroraDbConnection(Database.ConnectionStringForSource))
            using (var target = new AuroraDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                var tables = new[] { "sales.order_line", "node_b", "Person", "node_a", "country" };
                var created = new List<string>();

                // Act
                source.CopyTableSchemaTo(tables, target, createdCallback: r => created.Add(r.Target.Table.Table.Name));

                // Assert
                CollectionAssert.AreEquivalent(new[] { "order_line", "node_a", "node_b", "Person", "country" }, created);
                foreach (var table in tables.Where(table => table != "sales.order_line"))
                {
                    Helper.AssertTargetMatchesSource(table);
                }
                Assert.IsTrue(Helper.TargetTableExists("public.order_line"));
            }
        }

        [TestMethod]
        public void TestAuroraDbPostgreSqlCopyTableSchemaToWithErrorCallbackContinuesWithTheOtherTables()
        {
            Helper.DisableExistenceChecks();

            using (var source = new AuroraDbConnection(Database.ConnectionStringForSource))
            using (var target = new AuroraDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                source.CopyTableSchemaTo("no_key", target);
                var errors = new List<ExecutionError>();
                var created = new List<CopyTableSchemaResultItem>();

                // Act
                source.CopyTableSchemaTo(new[] { "no_key", "country", "Person" }, target, createdCallback: created.Add, errorCallback: errors.Add);

                // Assert
                Assert.AreEqual(1, errors.Count);
                Assert.AreEqual("no_key", errors[0].TableName);
                Assert.IsInstanceOfType<AuroraDbException>(errors[0].Exception);
                Assert.AreEqual(3, created.Count);
                Assert.AreSame(errors[0], created.Single(r => r.Target.Table.Table.Name == "no_key").Errors.Single());
                Assert.AreEqual(CopyTableSchemaOutcome.Failed, created.Single(r => r.Target.Table.Table.Name == "no_key").Outcome);
                Helper.AssertTargetMatchesSource("country");
                Helper.AssertTargetMatchesSource("Person");
            }
        }

        [TestMethod]
        public async Task TestAuroraDbPostgreSqlCopyTableSchemaToAsync()
        {
            using (var source = new AuroraDbConnection(Database.ConnectionStringForSource))
            using (var target = new AuroraDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);

                // Act
                var single = (await source.CopyTableSchemaToAsync("country", target)).Items.Single();
                await source.CopyTableSchemaToAsync(new[] { "Person", "node_a", "node_b" }, target);

                // Assert
                Assert.AreEqual("country", single.Target.Table.Table.Name);
                foreach (var table in new[] { "country", "Person", "node_a", "node_b" })
                {
                    Helper.AssertTargetMatchesSource(table);
                }
            }
        }

        #endregion
    }
}
