#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema.Enumerations;
using RepoDb.Schema.SqlServer.IntegrationTests.Setup;

namespace RepoDb.Schema.SqlServer.IntegrationTests
{
    [TestClass]
    public class CopyTableSchemaToTest
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

        private static void MapSchemaReaderConnection(SqlConnection source) =>
            SchemaReaderMapper.Add<SqlConnection>(new SqlServerSchemaReader(source), true);

        #endregion

        #region CopyTableSchemaTo

        #region Sync

        [TestMethod]
        public void TestCopyTableSchemaTo()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);

                // Act
                var result = source.CopyTableSchemaTo("Country", target).Items.Single();

                // Assert
                Assert.IsTrue(Helper.TargetTableExists("Country"));
                Helper.AssertSchemaEquality(Helper.GetSourceSchema("Country"), Helper.GetTargetSchema("Country"));
                Assert.AreEqual("Country", result.Target.Table.Table.Name, StringComparer.Ordinal);
            }
        }

        [TestMethod]
        public void TestCopyTableSchemaToResult()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                source.CopyTableSchemaTo("Country", target);

                // Act
                var run = source.CopyTableSchemaTo("Person", target);
                var result = run.Items.Single();

                // Assert
                Assert.AreEqual("Person", result.Target.Table.Table.Name, StringComparer.Ordinal);
                Assert.AreEqual("dbo", result.Source.Table.Table.Schema, StringComparer.Ordinal);
                Assert.AreEqual("RepoDb_Schema_Source", result.Source.Database, StringComparer.Ordinal);
                Assert.AreEqual("RepoDb_Schema_Target", result.Target.Database, StringComparer.Ordinal);
                Assert.AreEqual(nameof(SqlConnection), result.Source.DatabaseType, StringComparer.Ordinal);
                Assert.AreEqual(nameof(SqlConnection), result.Target.DatabaseType, StringComparer.Ordinal);
                Assert.AreEqual(TableOutcome.Created, result.Outcome);
                Assert.AreEqual(TableExistsBehavior.Skip, result.Action);
                Assert.AreEqual(7, result.ColumnCount);
                Assert.AreEqual(1, result.IndexCount);
                Assert.AreEqual(1, result.ForeignKeyCount);
                Assert.AreEqual(0, result.UniqueConstraintCount);
                Assert.AreEqual(1, result.CheckConstraintCount);
                StringAssert.Contains(result.Script, "CREATE TABLE [dbo].[Person]", StringComparison.Ordinal);
                StringAssert.Contains(result.Script, "CREATE INDEX [IX_Person_Name]", StringComparison.Ordinal);
                StringAssert.Contains(result.Script, "FK_Person_Country", StringComparison.Ordinal);
                Assert.IsTrue(run.EndTime >= run.StartTime);
            }
        }

        [TestMethod]
        public void TestCopyTableSchemaToWithAllTheObjects()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                source.CopyTableSchemaTo("Country", target);

                // Act
                source.CopyTableSchemaTo("Person", target);

                // Assert
                Helper.AssertSchemaEquality(Helper.GetSourceSchema("Person"), Helper.GetTargetSchema("Person"));
            }
        }

        [TestMethod]
        public void TestCopyTableSchemaToOfTableInAnotherSchema()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);

                // Act
                var result = source.CopyTableSchemaTo("Sales.Invoice", target, "Sales").Items.Single();

                // Assert
                Assert.AreEqual("Sales", result.Source.Table.Table.Schema, StringComparer.Ordinal);
                Assert.AreEqual("Sales", result.Target.Table.Table.Schema, StringComparer.Ordinal);
                Helper.AssertSchemaEquality(Helper.GetSourceSchema("Sales.Invoice"), Helper.GetTargetSchema("Sales.Invoice"));
            }
        }

        [TestMethod]
        public void TestCopyTableSchemaToWithTheRequestedExistsBehavior()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);

                // Act
                var result = source.CopyTableSchemaTo("NoKey", target, tableExistenceBehavior: TableExistsBehavior.Drop).Items.Single();

                // Assert
                Assert.AreEqual(TableExistsBehavior.Drop, result.Action);
            }
        }

        [TestMethod]
        public void TestCopyTableSchemaToWithTransaction()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget).EnsureOpen())
            {
                // Setup
                MapSchemaReaderConnection(source);

                // Act
                using (var transaction = target.BeginTransaction())
                {
                    source.CopyTableSchemaTo("NoKey", target, transaction: transaction);
                    transaction.Rollback();
                }

                // Assert
                Assert.IsFalse(Helper.TargetTableExists("NoKey"));
            }
        }

        [TestMethod]
        public void TestCopyTableSchemaToViaTheSameConnectionMapping()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);

                // Act
                source.CopyTableSchemaTo("OrderLine", target);
                source.CopyTableSchemaTo("NoKey", target);

                // Assert
                Assert.IsTrue(Helper.TargetTableExists("OrderLine"));
                Assert.IsTrue(Helper.TargetTableExists("NoKey"));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCopyTableSchemaToIfTheTargetTableAlreadyExists()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                source.CopyTableSchemaTo("NoKey", target);

                // Act/Assert
                Assert.Throws<InvalidOperationException>(() => source.CopyTableSchemaTo("NoKey", target, tableExistenceBehavior: TableExistsBehavior.Throw));
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestCopyTableSchemaToAsync()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);

                // Act
                var result = (await source.CopyTableSchemaToAsync("Country", target)).Items.Single();

                // Assert
                Assert.IsTrue(Helper.TargetTableExists("Country"));
                Helper.AssertSchemaEquality(Helper.GetSourceSchema("Country"), Helper.GetTargetSchema("Country"));
                Assert.AreEqual("Country", result.Target.Table.Table.Name, StringComparer.Ordinal);
            }
        }

        [TestMethod]
        public async Task TestCopyTableSchemaToAsyncWithAllTheObjects()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                await source.CopyTableSchemaToAsync("Country", target);

                // Act
                var result = (await source.CopyTableSchemaToAsync("Person", target)).Items.Single();

                // Assert
                Helper.AssertSchemaEquality(Helper.GetSourceSchema("Person"), Helper.GetTargetSchema("Person"));
                Assert.AreEqual(7, result.ColumnCount);
                Assert.AreEqual(TableOutcome.Created, result.Outcome);
            }
        }

        [TestMethod]
        public async Task TestCopyTableSchemaToAsyncOfTableInAnotherSchema()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);

                // Act
                await source.CopyTableSchemaToAsync("Sales.Invoice", target, "Sales");

                // Assert
                Helper.AssertSchemaEquality(Helper.GetSourceSchema("Sales.Invoice"), Helper.GetTargetSchema("Sales.Invoice"));
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnCopyTableSchemaToAsyncIfTheTargetTableAlreadyExists()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                await source.CopyTableSchemaToAsync("NoKey", target);

                // Act/Assert
                await Assert.ThrowsAsync<InvalidOperationException>(() => source.CopyTableSchemaToAsync("NoKey", target, tableExistenceBehavior: TableExistsBehavior.Throw));
            }
        }

        #endregion

        #endregion
    }
}
