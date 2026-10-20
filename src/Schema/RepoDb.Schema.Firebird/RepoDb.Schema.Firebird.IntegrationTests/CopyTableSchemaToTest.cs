#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Threading.Tasks;
using FirebirdSql.Data.FirebirdClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema.Enumerations;
using RepoDb.Schema.Firebird.IntegrationTests.Setup;

namespace RepoDb.Schema.Firebird.IntegrationTests
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

        private static void MapSchemaReaderConnection(FbConnection source) =>
            SchemaReaderMapper.Add<FbConnection>(new FirebirdSchemaReader(source), true);

        #endregion

        #region CopyTableSchemaTo

        #region Sync

        [TestMethod]
        public void TestCopyTableSchemaTo()
        {
            using (var source = new FbConnection(Database.ConnectionStringForSource))
            using (var target = new FbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);

                // Act
                var result = source.CopyTableSchemaTo("Country", target);

                // Assert
                Assert.IsTrue(Helper.TargetTableExists("Country"));
                Helper.AssertSchemaEquality(Helper.GetSourceSchema("Country"), Helper.GetTargetSchema("Country"));
                Assert.AreEqual("Country", result.TableName, StringComparer.Ordinal);
            }
        }

        [TestMethod]
        public void TestCopyTableSchemaToResult()
        {
            using (var source = new FbConnection(Database.ConnectionStringForSource))
            using (var target = new FbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                source.CopyTableSchemaTo("Country", target);

                // Act
                var result = source.CopyTableSchemaTo("Person", target);

                // Assert
                Assert.AreEqual("Person", result.TableName, StringComparer.Ordinal);
                Assert.IsNull(result.SourceSchema);
                Assert.AreEqual(source.Database, result.SourceDatabase, StringComparer.Ordinal);
                Assert.AreEqual(target.Database, result.DestinationDatabase, StringComparer.Ordinal);
                Assert.AreEqual(nameof(FbConnection), result.SourceDatabaseType, StringComparer.Ordinal);
                Assert.AreEqual(nameof(FbConnection), result.DestinationDatabaseType, StringComparer.Ordinal);
                Assert.AreEqual(CopyTableSchemaOutcome.Created, result.Outcome);
                Assert.AreEqual(CopyTableSchemaExistsBehavior.Skip, result.Action);
                Assert.AreEqual(7, result.ColumnCount);
                Assert.AreEqual(1, result.IndexCount);
                Assert.AreEqual(1, result.ForeignKeyCount);
                Assert.AreEqual(0, result.UniqueConstraintCount);
                Assert.AreEqual(1, result.CheckConstraintCount);
                StringAssert.Contains(result.Script, "CREATE TABLE \"Person\"", StringComparison.Ordinal);
                StringAssert.Contains(result.Script, "CREATE DESCENDING INDEX \"IX_Person_Name\"", StringComparison.Ordinal);
                StringAssert.Contains(result.Script, "FK_Person_Country", StringComparison.Ordinal);
                Assert.IsTrue(result.EndTime >= result.StartTime);
            }
        }

        [TestMethod]
        public void TestCopyTableSchemaToWithAllTheObjects()
        {
            using (var source = new FbConnection(Database.ConnectionStringForSource))
            using (var target = new FbConnection(Database.ConnectionStringForTarget))
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
        public void TestCopyTableSchemaToWithTheRequestedExistsBehavior()
        {
            using (var source = new FbConnection(Database.ConnectionStringForSource))
            using (var target = new FbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);

                // Act
                var result = source.CopyTableSchemaTo("NoKey", target, tableExistenceBehavior: CopyTableSchemaExistsBehavior.Drop);

                // Assert
                Assert.AreEqual(CopyTableSchemaExistsBehavior.Drop, result.Action);
            }
        }

        [TestMethod]
        public void TestCopyTableSchemaToWithTransactionThatIsRolledBackRollsBackTheTable()
        {
            using (var source = new FbConnection(Database.ConnectionStringForSource))
            using (var target = new FbConnection(Database.ConnectionStringForTarget).EnsureOpen())
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
            using (var source = new FbConnection(Database.ConnectionStringForSource))
            using (var target = new FbConnection(Database.ConnectionStringForTarget))
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
            using (var source = new FbConnection(Database.ConnectionStringForSource))
            using (var target = new FbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                source.CopyTableSchemaTo("NoKey", target);

                // Act/Assert
                Assert.Throws<InvalidOperationException>(() => source.CopyTableSchemaTo("NoKey", target, tableExistenceBehavior: CopyTableSchemaExistsBehavior.Throw));
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestCopyTableSchemaToAsync()
        {
            using (var source = new FbConnection(Database.ConnectionStringForSource))
            using (var target = new FbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);

                // Act
                var result = await source.CopyTableSchemaToAsync("Country", target);

                // Assert
                Assert.IsTrue(Helper.TargetTableExists("Country"));
                Helper.AssertSchemaEquality(Helper.GetSourceSchema("Country"), Helper.GetTargetSchema("Country"));
                Assert.AreEqual("Country", result.TableName, StringComparer.Ordinal);
            }
        }

        [TestMethod]
        public async Task TestCopyTableSchemaToAsyncWithAllTheObjects()
        {
            using (var source = new FbConnection(Database.ConnectionStringForSource))
            using (var target = new FbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                await source.CopyTableSchemaToAsync("Country", target);

                // Act
                var result = await source.CopyTableSchemaToAsync("Person", target);

                // Assert
                Helper.AssertSchemaEquality(Helper.GetSourceSchema("Person"), Helper.GetTargetSchema("Person"));
                Assert.AreEqual(7, result.ColumnCount);
                Assert.AreEqual(CopyTableSchemaOutcome.Created, result.Outcome);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnCopyTableSchemaToAsyncIfTheTargetTableAlreadyExists()
        {
            using (var source = new FbConnection(Database.ConnectionStringForSource))
            using (var target = new FbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                await source.CopyTableSchemaToAsync("NoKey", target);

                // Act/Assert
                await Assert.ThrowsAsync<InvalidOperationException>(() => source.CopyTableSchemaToAsync("NoKey", target, tableExistenceBehavior: CopyTableSchemaExistsBehavior.Throw));
            }
        }

        #endregion

        #endregion
    }
}
