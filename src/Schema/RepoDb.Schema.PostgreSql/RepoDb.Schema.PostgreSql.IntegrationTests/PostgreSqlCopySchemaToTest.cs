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
using RepoDb.Schema.Enumerations;
using RepoDb.Schema.PostgreSql.IntegrationTests.Setup;

namespace RepoDb.Schema.PostgreSql.IntegrationTests
{
    [TestClass]
    public class PostgreSqlCopySchemaToTest
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

        #endregion

        #region CopySchemaTo

        [TestMethod]
        public void TestPostgreSqlCopySchemaTo()
        {
            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
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
        public void TestPostgreSqlCopySchemaToSerialAndWithoutKey()
        {
            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
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
        public void TestPostgreSqlCopySchemaToAcceptsTheSchemaOfTheTable()
        {
            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                source.CopySchemaTo("country", target);
                source.CopySchemaTo("Person", target);

                // Act
                source.CopySchemaTo("sales.order_line", target);

                // Assert
                Helper.AssertTargetMatchesSource("sales.order_line");
            }
        }

        [TestMethod]
        public void ThrowExceptionOnPostgreSqlCopySchemaToIfTheTableAlreadyExists()
        {
            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                source.CopySchemaTo("country", target);

                // Act/Assert
                Assert.Throws<InvalidOperationException>(() => source.CopySchemaTo("country", target, CopySchemaExistsBehavior.Throw));
            }
        }

        #endregion

        #region CopySchemaTo (Multiple Tables)

        [TestMethod]
        public void TestPostgreSqlCopySchemaToWithTheTablesInAnyOrderAndWithCycles()
        {
            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                var tables = new[] { "sales.order_line", "node_b", "Person", "node_a", "country" };
                var created = new List<string>();

                // Act
                source.CopySchemaTo(tables, target, createdCallback: r => created.Add(r.TableName));

                // Assert
                CollectionAssert.AreEquivalent(new[] { "order_line", "node_a", "node_b", "Person", "country" }, created);
                foreach (var table in tables)
                {
                    Helper.AssertTargetMatchesSource(table);
                }
            }
        }

        [TestMethod]
        public void TestPostgreSqlCopySchemaToWithErrorCallbackContinuesWithTheOtherTables()
        {
            Helper.DisableExistenceChecks();

            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
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
                Assert.IsInstanceOfType<PostgresException>(errors[0].Exception);
                Assert.AreEqual(3, created.Count);
                Assert.AreSame(errors[0], created.Single(r => r.TableName == "no_key").Errors.Single());
                Assert.AreEqual(CopySchemaOutcome.Failed, created.Single(r => r.TableName == "no_key").Outcome);
                Helper.AssertTargetMatchesSource("country");
                Helper.AssertTargetMatchesSource("Person");
            }
        }

        [TestMethod]
        public async Task TestPostgreSqlCopySchemaToAsync()
        {
            using (var source = new NpgsqlConnection(Database.ConnectionStringForSource))
            using (var target = new NpgsqlConnection(Database.ConnectionStringForTarget))
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
