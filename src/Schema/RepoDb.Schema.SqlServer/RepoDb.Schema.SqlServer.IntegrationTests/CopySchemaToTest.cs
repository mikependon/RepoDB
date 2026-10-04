#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema.Enumerations;
using RepoDb.Schema.SqlServer.IntegrationTests.Setup;

namespace RepoDb.Schema.SqlServer.IntegrationTests
{
    [TestClass]
    public class CopySchemaToTest
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

        // The reader owns the connection it reads from, so it is mapped for the connection that is used as the source of the copy.
        private static void MapSchemaReaderConnection(SqlConnection source) =>
            SchemaReaderMapper.Add<SqlConnection>(new SqlServerSchemaReader(source), true);

        #endregion

        #region CopySchemaTo

        #region Sync

        [TestMethod]
        public void TestCopySchemaTo()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);

                // Act
                var result = source.CopySchemaTo("Country", target);

                // Assert
                Assert.IsTrue(Helper.TargetTableExists("Country"));
                Helper.AssertSchemaEquality(Helper.GetSourceSchema("Country"), Helper.GetTargetSchema("Country"));
                Assert.AreEqual("Country", result.TableName, StringComparer.Ordinal);
            }
        }

        [TestMethod]
        public void TestCopySchemaToResult()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                source.CopySchemaTo("Country", target);

                // Act
                var result = source.CopySchemaTo("Person", target);

                // Assert
                Assert.AreEqual("Person", result.TableName, StringComparer.Ordinal);
                Assert.AreEqual("dbo", result.SourceSchema, StringComparer.Ordinal);
                Assert.AreEqual("RepoDb_Schema_Source", result.SourceDatabase, StringComparer.Ordinal);
                Assert.AreEqual("RepoDb_Schema_Target", result.DestinationDatabase, StringComparer.Ordinal);
                Assert.AreEqual(nameof(SqlConnection), result.SourceDatabaseType, StringComparer.Ordinal);
                Assert.AreEqual(nameof(SqlConnection), result.DestinationDatabaseType, StringComparer.Ordinal);
                Assert.AreEqual(CopySchemaOutcome.Created, result.Outcome);
                Assert.AreEqual(CopySchemaExistsBehavior.Skip, result.Action);
                Assert.AreEqual(7, result.ColumnCount);
                Assert.AreEqual(1, result.IndexCount);
                Assert.AreEqual(1, result.ForeignKeyCount);
                Assert.AreEqual(0, result.UniqueConstraintCount);
                Assert.AreEqual(1, result.CheckConstraintCount);
                StringAssert.Contains(result.Script, "CREATE TABLE [dbo].[Person]", StringComparison.Ordinal);
                StringAssert.Contains(result.Script, "CREATE INDEX [IX_Person_Name]", StringComparison.Ordinal);
                StringAssert.Contains(result.Script, "FK_Person_Country", StringComparison.Ordinal);
                Assert.IsTrue(result.EndTime >= result.StartTime);
            }
        }

        [TestMethod]
        public void TestCopySchemaToWithAllTheObjects()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                source.CopySchemaTo("Country", target);

                // Act
                source.CopySchemaTo("Person", target);

                // Assert
                Helper.AssertSchemaEquality(Helper.GetSourceSchema("Person"), Helper.GetTargetSchema("Person"));
            }
        }

        [TestMethod]
        public void TestCopySchemaToOfTableInAnotherSchema()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);

                // Act
                var result = source.CopySchemaTo("Sales.Invoice", target);

                // Assert
                Assert.AreEqual("Sales", result.SourceSchema, StringComparer.Ordinal);
                Helper.AssertSchemaEquality(Helper.GetSourceSchema("Sales.Invoice"), Helper.GetTargetSchema("Sales.Invoice"));
            }
        }

        [TestMethod]
        public void TestCopySchemaToWithTheRequestedExistsBehavior()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);

                // Act
                var result = source.CopySchemaTo("NoKey", target, CopySchemaExistsBehavior.Drop);

                // Assert
                Assert.AreEqual(CopySchemaExistsBehavior.Drop, result.Action);
            }
        }

        [TestMethod]
        public void TestCopySchemaToWithTransaction()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget).EnsureOpen())
            {
                // Setup
                MapSchemaReaderConnection(source);

                // Act
                using (var transaction = target.BeginTransaction())
                {
                    source.CopySchemaTo("NoKey", target, transaction: transaction);
                    transaction.Rollback();
                }

                // Assert
                Assert.IsFalse(Helper.TargetTableExists("NoKey"));
            }
        }

        [TestMethod]
        public void TestCopySchemaToViaTheSameConnectionMapping()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);

                // Act
                source.CopySchemaTo("OrderLine", target);
                source.CopySchemaTo("NoKey", target);

                // Assert
                Assert.IsTrue(Helper.TargetTableExists("OrderLine"));
                Assert.IsTrue(Helper.TargetTableExists("NoKey"));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCopySchemaToIfTheTargetTableAlreadyExists()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                source.CopySchemaTo("NoKey", target);

                // Act/Assert (the existence behavior is not enforced yet, so the creation of the existing table fails)
                Assert.Throws<SqlException>(() => source.CopySchemaTo("NoKey", target));
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestCopySchemaToAsync()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);

                // Act
                var result = await source.CopySchemaToAsync("Country", target);

                // Assert
                Assert.IsTrue(Helper.TargetTableExists("Country"));
                Helper.AssertSchemaEquality(Helper.GetSourceSchema("Country"), Helper.GetTargetSchema("Country"));
                Assert.AreEqual("Country", result.TableName, StringComparer.Ordinal);
            }
        }

        [TestMethod]
        public async Task TestCopySchemaToAsyncWithAllTheObjects()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                await source.CopySchemaToAsync("Country", target);

                // Act
                var result = await source.CopySchemaToAsync("Person", target);

                // Assert
                Helper.AssertSchemaEquality(Helper.GetSourceSchema("Person"), Helper.GetTargetSchema("Person"));
                Assert.AreEqual(7, result.ColumnCount);
                Assert.AreEqual(CopySchemaOutcome.Created, result.Outcome);
            }
        }

        [TestMethod]
        public async Task TestCopySchemaToAsyncOfTableInAnotherSchema()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);

                // Act
                await source.CopySchemaToAsync("Sales.Invoice", target);

                // Assert
                Helper.AssertSchemaEquality(Helper.GetSourceSchema("Sales.Invoice"), Helper.GetTargetSchema("Sales.Invoice"));
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnCopySchemaToAsyncIfTheTargetTableAlreadyExists()
        {
            using (var source = new SqlConnection(Database.ConnectionStringForSource))
            using (var target = new SqlConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                MapSchemaReaderConnection(source);
                await source.CopySchemaToAsync("NoKey", target);

                // Act/Assert (the existence behavior is not enforced yet, so the creation of the existing table fails)
                await Assert.ThrowsAsync<SqlException>(() => source.CopySchemaToAsync("NoKey", target));
            }
        }

        #endregion

        #endregion
    }
}
