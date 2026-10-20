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
using RepoDb.Schema.Enumerations;
using RepoDb.Schema.AuroraDbPostgreSql.IntegrationTests.Setup;

namespace RepoDb.Schema.AuroraDbPostgreSql.IntegrationTests
{
    [TestClass]
    public class AuroraDbPostgreSqlCopySchemaToTest
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

        #region CopySchemaTo

        [TestMethod]
        public void TestAuroraDbPostgreSqlCopySchemaTo()
        {
            using (var source = new AuroraDbConnection(Database.ConnectionStringForSource))
            using (var target = new AuroraDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                source.CopySchemaTo("country", target);

                // Act
                var result = source.CopySchemaTo("Person", target);

                // Assert
                Helper.AssertTargetMatchesSource("country");
                Helper.AssertTargetMatchesSource("Person");
                Assert.AreEqual("Person", result.TableName);
                Assert.AreEqual(CopySchemaOutcome.Created, result.Outcome);
                Assert.AreEqual(10, result.ColumnCount);
                Assert.AreEqual(2, result.IndexCount);
                Assert.AreEqual(1, result.ForeignKeyCount);
            }
        }

        [TestMethod]
        public void TestAuroraDbPostgreSqlCopySchemaToSerialAndWithoutKey()
        {
            using (var source = new AuroraDbConnection(Database.ConnectionStringForSource))
            using (var target = new AuroraDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);

                // Act
                source.CopySchemaTo("ticket", target);
                source.CopySchemaTo("no_key", target);

                // Assert
                Helper.AssertTargetMatchesSource("ticket");
                Helper.AssertTargetMatchesSource("no_key");
            }
        }

        [TestMethod]
        public void TestAuroraDbPostgreSqlCopySchemaToAcceptsTheSchemaOfTheTable()
        {
            using (var source = new AuroraDbConnection(Database.ConnectionStringForSource))
            using (var target = new AuroraDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                source.CopySchemaTo("country", target);
                source.CopySchemaTo("Person", target);

                // Act
                var result = source.CopySchemaTo("sales.order_line", target);

                // Assert
                Assert.AreEqual("sales", result.SourceSchema);
                Assert.AreEqual("public", result.DestinationSchema);
                Assert.IsTrue(Helper.TargetTableExists("public.order_line"));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnAuroraDbPostgreSqlCopySchemaToIfTheTableAlreadyExists()
        {
            using (var source = new AuroraDbConnection(Database.ConnectionStringForSource))
            using (var target = new AuroraDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                source.CopySchemaTo("country", target);

                // Act/Assert
                Assert.Throws<InvalidOperationException>(() => source.CopySchemaTo("country", target, tableExistenceBehavior: CopySchemaExistsBehavior.Throw));
            }
        }

        #endregion

        #region CopySchemaTo (Multiple Tables)

        [TestMethod]
        public void TestAuroraDbPostgreSqlCopySchemaToWithTheTablesInAnyOrderAndWithCycles()
        {
            using (var source = new AuroraDbConnection(Database.ConnectionStringForSource))
            using (var target = new AuroraDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                var tables = new[] { "sales.order_line", "node_b", "Person", "node_a", "country" };
                var created = new List<string>();

                // Act
                source.CopySchemaTo(tables, target, createdCallback: r => created.Add(r.TableName));

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
        public void TestAuroraDbPostgreSqlCopySchemaToWithErrorCallbackContinuesWithTheOtherTables()
        {
            Helper.DisableExistenceChecks();

            using (var source = new AuroraDbConnection(Database.ConnectionStringForSource))
            using (var target = new AuroraDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                source.CopySchemaTo("no_key", target);
                var errors = new List<CopySchemaError>();
                var created = new List<CopySchemaResult>();

                // Act
                source.CopySchemaTo(new[] { "no_key", "country", "Person" }, target, createdCallback: created.Add, errorCallback: errors.Add);

                // Assert
                Assert.AreEqual(1, errors.Count);
                Assert.AreEqual("no_key", errors[0].TableName);
                Assert.IsInstanceOfType<AuroraDbException>(errors[0].Exception);
                Assert.AreEqual(3, created.Count);
                Assert.AreSame(errors[0], created.Single(r => r.TableName == "no_key").Errors.Single());
                Assert.AreEqual(CopySchemaOutcome.Failed, created.Single(r => r.TableName == "no_key").Outcome);
                Helper.AssertTargetMatchesSource("country");
                Helper.AssertTargetMatchesSource("Person");
            }
        }

        [TestMethod]
        public async Task TestAuroraDbPostgreSqlCopySchemaToAsync()
        {
            using (var source = new AuroraDbConnection(Database.ConnectionStringForSource))
            using (var target = new AuroraDbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);

                // Act
                var single = await source.CopySchemaToAsync("country", target);
                await source.CopySchemaToAsync(new[] { "Person", "node_a", "node_b" }, target);

                // Assert
                Assert.AreEqual("country", single.TableName);
                foreach (var table in new[] { "country", "Person", "node_a", "node_b" })
                {
                    Helper.AssertTargetMatchesSource(table);
                }
            }
        }

        #endregion
    }
}
