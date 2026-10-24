#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RepoDb.Data.Core.UnitTests.CustomObjects;
using RepoDb.Data.Core.UnitTests.Fake.BulkOperations;
using RepoDb.Data.Enumerations;
using RepoDb.Data.Interfaces;
using RepoDb.Data.Models;
using RepoDb.DbSettings;
using RepoDb.Enumerations;
using RepoDb.Exceptions;
using RepoDb.Schema;
using RepoDb.Schema.Enumerations;
using RepoDb.Schema.Models;

namespace RepoDb.Data.Core.UnitTests.Extensions
{
    [TestClass]
    public class CopyDataToTest
    {
        #region Helpers

        private static IDbConnection GetConnection() =>
            new Mock<IDbConnection>().Object;

        private static readonly CopyRelationshipBehavior[] RelationshipBehaviors =
        {
            CopyRelationshipBehavior.Parents,
            CopyRelationshipBehavior.Children,
            CopyRelationshipBehavior.ParentsAndChildren
        };

        private static readonly TableExistsBehavior[] ExistsBehaviors =
        {
            TableExistsBehavior.Align,
            TableExistsBehavior.Throw,
            TableExistsBehavior.Drop
        };

        #endregion

        #region Arguments

        [TestMethod]
        public void ThrowExceptionOnCopyDataToIfTheConnectionIsNull()
        {
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => ((IDbConnection)null).CopyDataTo("Person", "Person", GetConnection()));
        }

        [TestMethod]
        public void ThrowExceptionOnCopyDataToIfTheSourceTableIsNull()
        {
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => GetConnection().CopyDataTo(null, "Person", GetConnection()));
        }

        [TestMethod]
        public void ThrowExceptionOnCopyDataToIfTheTargetTableIsWhiteSpace()
        {
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => GetConnection().CopyDataTo("Person", " ", GetConnection()));
        }

        [TestMethod]
        public void ThrowExceptionOnCopyDataToIfTheDestinationConnectionIsNull()
        {
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => GetConnection().CopyDataTo("Person", "Person", (IDbConnection)null));
        }

        [TestMethod]
        public void ThrowExceptionOnCopyDataToIfTheBufferSizeIsNotGreaterThanZero()
        {
            // Act/Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => GetConnection().CopyDataTo("Person", "Person", GetConnection(), bufferSize: 0));
        }

        #endregion

        #region Schema

        [TestMethod]
        public void ThrowExceptionOnCopyDataToIfTheTablesHaveDifferentNamesAndTheRelationshipBehaviorIsNotTableOnly()
        {
            foreach (var behavior in RelationshipBehaviors)
            {
                // Act/Assert
                Assert.Throws<ArgumentException>(() =>
                    GetConnection().CopyDataTo("Person", "Customer", GetConnection(), relationshipBehavior: behavior));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCopyDataToIfTheTablesHaveDifferentNamesAndTheTableExistenceBehaviorIsNotSkip()
        {
            foreach (var behavior in ExistsBehaviors)
            {
                // Act/Assert
                Assert.Throws<ArgumentException>(() =>
                    GetConnection().CopyDataTo("Person", "Customer", GetConnection(), tableExistenceBehavior: behavior));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCopyDataToIfTheTablesHaveDifferentNamesAndTheTargetTableHasASchema()
        {
            // Act/Assert
            Assert.Throws<ArgumentException>(() =>
                GetConnection().CopyDataTo("Person", "Sales.Customer", new CustomDbConnection()));
            Assert.Throws<ArgumentException>(() =>
                GetConnection().CopyDataTo("Person", "[Sales].[Customer]", new CustomDbConnection()));
        }

        [TestMethod]
        public async Task ThrowExceptionOnCopyDataToAsyncIfTheTablesHaveDifferentNamesAndTheTargetTableHasASchema()
        {
            // Act/Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                GetConnection().CopyDataToAsync("Person", "Sales.Customer", new CustomDbConnection()));
        }

        [TestMethod]
        public void TestCopyDataToWithATargetTableOfAnotherSchemaCopiesTheSchema()
        {
            // Act/Assert (the table has the same name, so it is valid and the schema is copied, but there is no schema reader for the connection)
            Assert.Throws<MissingMappingException>(() =>
                GetConnection().CopyDataTo("Person", "Sales.Person", new CustomDbConnection()));
            Assert.Throws<MissingMappingException>(() =>
                GetConnection().CopyDataTo("Person", "[Sales].[Person]", new CustomDbConnection()));
            Assert.Throws<MissingMappingException>(() =>
                new CustomDbConnection().CopyDataTo("[dbo].[Person]", "Sales.Person", new CustomDbConnection()));
        }

        [TestMethod]
        public async Task TestCopyDataToAsyncWithATargetTableOfAnotherSchemaCopiesTheSchema()
        {
            // Act/Assert
            await Assert.ThrowsAsync<MissingMappingException>(() =>
                GetConnection().CopyDataToAsync("Person", "Sales.Person", new CustomDbConnection()));
        }

        [TestMethod]
        public async Task ThrowExceptionOnCopyDataToAsyncIfTheTablesHaveDifferentNamesAndTheRelationshipBehaviorIsNotTableOnly()
        {
            foreach (var behavior in RelationshipBehaviors)
            {
                // Act/Assert
                await Assert.ThrowsAsync<ArgumentException>(() =>
                    GetConnection().CopyDataToAsync("Person", "Customer", GetConnection(), relationshipBehavior: behavior));
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnCopyDataToAsyncIfTheTablesHaveDifferentNamesAndTheTableExistenceBehaviorIsNotSkip()
        {
            foreach (var behavior in ExistsBehaviors)
            {
                // Act/Assert
                await Assert.ThrowsAsync<ArgumentException>(() =>
                    GetConnection().CopyDataToAsync("Person", "Customer", GetConnection(), tableExistenceBehavior: behavior));
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnCopyDataToAsyncIfTheBufferSizeIsNotGreaterThanZero()
        {
            // Act/Assert
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => GetConnection().CopyDataToAsync("Person", "Person", GetConnection(), bufferSize: 0));
        }

        #endregion

        #region Initialize

        [TestInitialize]
        public void Initialize()
        {
            DbSettingMapper.Add<FakeBulkDbConnection>(new FakeDbSetting(), true);
            FakeBulkOperations.Reset();
        }

        [TestCleanup]
        public void Cleanup()
        {
            SchemaReaderMapper.Clear();
            SchemaComposerMapper.Clear();
        }

        #endregion

        #region Classes

        public class Person
        {
            public int Id { get; set; }

            public string Name { get; set; }
        }

        #endregion

        #region Success Helpers

        private static string Register(IDbConnection destination)
        {
            var name = "Destination-" + Guid.NewGuid().ToString("N");
            ConnectionManager.Register(name, destination);
            return name;
        }

        private static FakeBulkCall Call() =>
            FakeBulkOperations.Calls.Single();

        private static QueryGroup Filter() =>
            new QueryGroup(new QueryField("Id", Operation.GreaterThan, 1));

        #endregion

        #region CopyDataTo (Registered Connection, Sync)

        [TestMethod]
        public void TestCopyDataToOfAnEntityToARegisteredConnectionCopiesTheTableOfTheEntity()
        {
            // Setup
            var name = Register(new FakeBulkDbConnection());

            // Act
            var actual = new FakeDbConnection().CopyDataTo<Person>(name).RowCount;

            // Assert
            Assert.AreEqual(3, actual);
            Assert.AreEqual("Person", Call().TableName);
        }

        [TestMethod]
        public void TestCopyDataToOfAnEntityToARegisteredConnectionFiltersTheRowsWithTheExpression()
        {
            // Setup
            var name = Register(new FakeBulkDbConnection());
            var source = new FakeDbConnection();

            // Act
            new FakeDbConnection().CopyDataTo<Person>(name, p => p.Id > 1);
            source.CopyDataTo<Person>(name, p => p.Id > 1);

            // Assert
            Assert.AreEqual(2, FakeBulkOperations.Calls.Length);
            StringAssert.Contains(source.Executions.Single().CommandText, "WHERE");
            Assert.AreEqual(1, source.Executions.Single().Parameters.Values.Single());
        }

        [TestMethod]
        public void TestCopyDataToOfAnEntityWithATargetTableToARegisteredConnectionCopiesIntoTheTargetTable()
        {
            // Setup
            var name = Register(new FakeBulkDbConnection());

            // Act
            var actual = new FakeDbConnection().CopyDataTo<Person>("Customer", name).RowCount;

            // Assert
            Assert.AreEqual(3, actual);
            Assert.AreEqual("Customer", Call().TableName);
        }

        [TestMethod]
        public void TestCopyDataToOfATableToARegisteredConnectionCopiesIntoTheSameTable()
        {
            // Setup
            var name = Register(new FakeBulkDbConnection());

            // Act
            var actual = new FakeDbConnection().CopyDataTo("Person", name).RowCount;

            // Assert
            Assert.AreEqual(3, actual);
            Assert.AreEqual("Person", Call().TableName);
        }

        [TestMethod]
        public void TestCopyDataToOfATableToARegisteredConnectionFiltersTheRowsWithTheQueryGroup()
        {
            // Setup
            var name = Register(new FakeBulkDbConnection());
            var source = new FakeDbConnection();

            // Act
            source.CopyDataTo("Person", name, Filter());

            // Assert
            StringAssert.Contains(source.Executions.Single().CommandText, "WHERE");
        }

        [TestMethod]
        public void TestCopyDataToOfASourceTableToARegisteredConnectionCopiesIntoTheTargetTable()
        {
            // Setup
            var name = Register(new FakeBulkDbConnection());
            var source = new FakeDbConnection();

            // Act
            var actual = source.CopyDataTo("Person", "Customer", name).RowCount;

            // Assert
            Assert.AreEqual(3, actual);
            Assert.AreEqual("Customer", Call().TableName);
            StringAssert.Contains(source.Executions.Single().CommandText, "[Person]");
        }

        [TestMethod]
        public void ThrowExceptionOnCopyDataToIfTheDestinationConnectionIsNotRegistered()
        {
            // Act/Assert
            Assert.Throws<InvalidOperationException>(() => new FakeDbConnection().CopyDataTo("Person", "NotRegistered-" + Guid.NewGuid().ToString("N")));
            Assert.Throws<InvalidOperationException>(() => new FakeDbConnection().CopyDataTo("Person", "Customer", "NotRegistered-" + Guid.NewGuid().ToString("N")));
            Assert.Throws<InvalidOperationException>(() => new FakeDbConnection().CopyDataTo<Person>("NotRegistered-" + Guid.NewGuid().ToString("N")));
            Assert.Throws<InvalidOperationException>(() => new FakeDbConnection().CopyDataTo<Person>("Customer", "NotRegistered-" + Guid.NewGuid().ToString("N")));
        }

        #endregion

        #region CopyDataTo (IDbConnection, Sync)

        [TestMethod]
        public void TestCopyDataToOfAnEntityCopiesTheTableOfTheEntity()
        {
            // Act
            var actual = new FakeDbConnection().CopyDataTo<Person>(new FakeBulkDbConnection()).RowCount;

            // Assert
            Assert.AreEqual(3, actual);
            Assert.AreEqual("Person", Call().TableName);
        }

        [TestMethod]
        public void TestCopyDataToOfAnEntityFiltersTheRowsWithTheExpression()
        {
            // Setup
            var source = new FakeDbConnection();

            // Act
            source.CopyDataTo<Person>(new FakeBulkDbConnection(), p => p.Id > 1);

            // Assert
            StringAssert.Contains(source.Executions.Single().CommandText, "WHERE");
        }

        [TestMethod]
        public void TestCopyDataToOfAnEntityWithATargetTableCopiesIntoTheTargetTable()
        {
            // Act
            var actual = new FakeDbConnection().CopyDataTo<Person>("Customer", new FakeBulkDbConnection()).RowCount;

            // Assert
            Assert.AreEqual(3, actual);
            Assert.AreEqual("Customer", Call().TableName);
        }

        [TestMethod]
        public void TestCopyDataToOfAnEntityWithATargetTableFiltersTheRowsWithTheExpression()
        {
            // Setup
            var source = new FakeDbConnection();

            // Act
            source.CopyDataTo<Person>("Customer", new FakeBulkDbConnection(), p => p.Id > 1);

            // Assert
            StringAssert.Contains(source.Executions.Single().CommandText, "WHERE");
        }

        [TestMethod]
        public void TestCopyDataToOfATableCopiesIntoTheSameTable()
        {
            // Act
            var actual = new FakeDbConnection().CopyDataTo("Person", new FakeBulkDbConnection()).RowCount;

            // Assert
            Assert.AreEqual(3, actual);
            Assert.AreEqual("Person", Call().TableName);
        }

        [TestMethod]
        public void TestCopyDataToWithTheBufferSizeTheTimeoutAndTheTransactionGivesThemToTheBulkInsert()
        {
            // Setup
            var transaction = new Mock<IDbTransaction>().Object;
            var source = new FakeDbConnection();

            // Act
            source.CopyDataTo("Person", "Customer", new FakeBulkDbConnection(), bufferSize: 250, commandTimeout: 12, transaction: transaction);

            // Assert
            var call = Call();
            Assert.AreEqual(250, call.BatchSize);
            Assert.AreEqual(12, call.Timeout);
            Assert.AreSame(transaction, call.Transaction);
            Assert.AreEqual(12, source.Executions.Single().CommandTimeout);
        }

        [TestMethod]
        public void TestCopyDataToCallsTheProgressCallbackWithTheProgressOfTheCopy()
        {
            // Setup
            var progress = new List<(int Batch, int Rows, int Total, DateTime Start, DateTime End)>();

            // Act
            var actual = new FakeDbConnection().CopyDataTo("Person", "Customer", new FakeBulkDbConnection(),
                progressCallback: p => progress.Add((p.BatchCount, p.CurrentBatch.RowCount, p.RowCount, p.StartTime, p.EndTime))).RowCount;

            // Assert
            Assert.AreEqual(3, actual);
            Assert.AreEqual(1, progress.Count);
            Assert.AreEqual((1, 3, 3), (progress[0].Batch, progress[0].Rows, progress[0].Total));
            Assert.AreNotEqual(default(DateTime), progress[0].Start);
            Assert.IsTrue(progress[0].End >= progress[0].Start);
        }

        [TestMethod]
        public void TestCopyDataToWithBypassExistingRowsBulkMergesTheRows()
        {
            // Setup
            var source = new FakeDbConnection();
            FakeBulkOperations.Reset();

            // Act
            var actual = source.CopyDataTo("Person", "Customer", FakeBulkDestination.Create(), bypassExistingRows: true).RowCount;

            // Assert
            var call = FakeBulkOperations.Calls.Single();
            Assert.AreEqual(3, actual);
            Assert.AreEqual("BulkMerge", call.Method);
            CollectionAssert.AreEqual(new[] { "Id" }, call.Qualifiers);
        }

        [TestMethod]
        public async Task TestCopyDataToAsyncWithBypassExistingRowsBulkMergesTheRows()
        {
            // Setup
            var source = new FakeDbConnection();
            FakeBulkOperations.Reset();

            // Act
            var actual = (await source.CopyDataToAsync("Person", "Customer", FakeBulkDestination.Create(), bypassExistingRows: true)).RowCount;

            // Assert
            var call = FakeBulkOperations.Calls.Single();
            Assert.AreEqual(3, actual);
            Assert.AreEqual("BulkMergeAsync", call.Method);
            CollectionAssert.AreEqual(new[] { "Id" }, call.Qualifiers);
        }

        [TestMethod]
        public void TestCopyDataToWithBypassExistingRowsToARegisteredConnectionBulkMergesTheRows()
        {
            // Setup
            var name = Register(FakeBulkDestination.Create());
            FakeBulkOperations.Reset();

            // Act
            new FakeDbConnection().CopyDataTo("Person", name, bypassExistingRows: true);

            // Assert
            Assert.AreEqual("BulkMerge", FakeBulkOperations.Calls.Single().Method);
        }

        [TestMethod]
        public void TestCopyDataToWithBypassExistingRowsAndMappingsBulkMergesTheRows()
        {
            // Setup
            var source = new FakeDbConnection();
            FakeBulkOperations.Reset();

            // Act
            source.CopyDataTo("Person", "Customer", FakeBulkDestination.Create(), new[] { new CopyColumnMapping("Id", "Id"), new CopyColumnMapping("Name", "Name") }, bypassExistingRows: true);

            // Assert
            Assert.AreEqual("BulkMerge", FakeBulkOperations.Calls.Single().Method);
            StringAssert.Contains(source.Executions.Single().CommandText, "[Name] AS [Name]");
        }

        [TestMethod]
        public void ThrowExceptionOnCopyDataToWithBypassExistingRowsIfTheTargetTableHasNoPrimaryKeyAndNoIdentity()
        {
            // Act/Assert
            Assert.Throws<InvalidOperationException>(() =>
                new FakeDbConnection().CopyDataTo("Person", "Customer", FakeBulkDestination.CreateKeyless(), bypassExistingRows: true));
        }

        [TestMethod]
        public void TestCopyDataToWithQualifiersBulkMergesTheRowsWithTheQualifiers()
        {
            // Setup
            FakeBulkOperations.Reset();

            // Act
            new FakeDbConnection().CopyDataTo("Person", "Customer", FakeBulkDestination.Create(), bypassExistingRows: true, qualifiers: new[] { "Name" });

            // Assert
            var call = FakeBulkOperations.Calls.Single();
            Assert.AreEqual("BulkMerge", call.Method);
            CollectionAssert.AreEqual(new[] { "Name" }, call.Qualifiers);
        }

        [TestMethod]
        public async Task TestCopyDataToAsyncWithQualifiersBulkMergesTheRowsWithTheQualifiers()
        {
            // Setup
            FakeBulkOperations.Reset();

            // Act
            await new FakeDbConnection().CopyDataToAsync("Person", "Customer", FakeBulkDestination.Create(), bypassExistingRows: true, qualifiers: new[] { "Id", "Name" });

            // Assert
            var call = FakeBulkOperations.Calls.Single();
            Assert.AreEqual("BulkMergeAsync", call.Method);
            CollectionAssert.AreEqual(new[] { "Id", "Name" }, call.Qualifiers);
        }

        [TestMethod]
        public void TestCopyDataToWithQualifiersToARegisteredConnectionBulkMergesTheRowsWithTheQualifiers()
        {
            // Setup
            var name = Register(FakeBulkDestination.CreateKeyless());
            FakeBulkOperations.Reset();

            // Act
            new FakeDbConnection().CopyDataTo("Person", name, bypassExistingRows: true, qualifiers: new[] { "Id" });

            // Assert
            CollectionAssert.AreEqual(new[] { "Id" }, FakeBulkOperations.Calls.Single().Qualifiers);
        }

        [TestMethod]
        public void TestCopyDataToWithMappingsAndQualifiersBulkMergesTheRowsWithTheQualifiers()
        {
            // Setup
            FakeBulkOperations.Reset();
            var mappings = new[] { new CopyColumnMapping("Id", "Id"), new CopyColumnMapping("Name", "Name") };

            // Act
            new FakeDbConnection().CopyDataTo("Person", "Customer", FakeBulkDestination.Create(), mappings, bypassExistingRows: true, qualifiers: new[] { "Name" });

            // Assert
            CollectionAssert.AreEqual(new[] { "Name" }, FakeBulkOperations.Calls.Single().Qualifiers);
        }

        [TestMethod]
        public void TestCopyDataToIgnoresTheQualifiersIfTheExistingRowsAreNotBypassed()
        {
            // Setup
            FakeBulkOperations.Reset();

            // Act
            new FakeDbConnection().CopyDataTo("Person", "Customer", FakeBulkDestination.Create(), qualifiers: new[] { "Name" });

            // Assert
            Assert.AreEqual("BulkInsert", FakeBulkOperations.Calls.Single().Method);
        }

        [TestMethod]
        public void TestCopyDataToWithBypassExistingRowsMergesTheRowsOneByOneIfTheProviderHasNoBulkMerge()
        {
            // Setup
            var destination = new FakeDbConnection();

            // Act
            var actual = new FakeDbConnection().CopyDataTo("Person", "Customer", destination, bypassExistingRows: true, qualifiers: new[] { "Name" }).RowCount;

            // Assert
            var updates = destination.Executions.Where(e => e.CommandText.StartsWith("UPDATE", StringComparison.Ordinal)).ToList();
            Assert.AreEqual(3, actual);
            Assert.AreEqual(3, updates.Count);
            StringAssert.Contains(updates[0].CommandText, "WHERE [Name] = ");
        }

        [TestMethod]
        public async Task TestCopyDataToAsyncWithBypassExistingRowsMergesTheRowsOneByOneIfTheProviderHasNoBulkMerge()
        {
            // Setup
            var destination = new FakeDbConnection();

            // Act
            await new FakeDbConnection().CopyDataToAsync("Person", "Customer", destination, bypassExistingRows: true);

            // Assert
            var update = destination.Executions.First(e => e.CommandText.StartsWith("UPDATE", StringComparison.Ordinal));
            StringAssert.Contains(update.CommandText, "WHERE [Id] = ");
        }

        [TestMethod]
        public void ThrowExceptionOnCopyDataToWithQualifiersIfAQualifierIsNotAColumnOfTheTargetTable()
        {
            // Act/Assert
            Assert.Throws<ArgumentException>(() =>
                new FakeDbConnection().CopyDataTo("Person", "Customer", FakeBulkDestination.Create(), bypassExistingRows: true, qualifiers: new[] { "Missing" }));
        }

        [TestMethod]
        public void TestCopyDataToSetsTheTablesAndTheDatabasesOfTheProgress()
        {
            // Setup
            var source = new FakeDbConnection { Data = FakeDbConnection.CreatePersons(3) };
            var destination = FakeBulkDestination.Create();
            CopyProgress progress = null;

            // Act
            source.CopyDataTo("Person", "Customer", destination, progressCallback: p => progress = p);

            // Assert
            Assert.AreEqual(3, progress.CurrentBatch.RowCount);
            Assert.AreEqual("FakeDataSource", progress.Current.Source.Server);
            Assert.AreEqual("FakeDatabase", progress.Current.Source.Database);
            Assert.AreEqual("Person", progress.Current.Source.Table.Name);
            Assert.AreEqual("FakeDataSource", progress.Current.Target.Server);
            Assert.AreEqual("FakeDatabase", progress.Current.Target.Database);
            Assert.AreEqual("Customer", progress.Current.Target.Table.Name);
        }

        [TestMethod]
        public void TestCopyDataToSetsTheTimesOfTheBatchesOfTheProgress()
        {
            // Setup
            var source = new FakeDbConnection { Data = FakeDbConnection.CreatePersons(4) };
            var destination = FakeBulkDestination.Create();
            var times = new List<(DateTime Start, DateTime End, DateTime OperationStart, DateTime OperationEnd)>();

            // Act
            source.CopyDataTo("Person", "Customer", destination, bufferSize: 2, progressCallback: p => times.Add((p.CurrentBatch.StartTime, p.CurrentBatch.EndTime, p.StartTime, p.EndTime)));

            // Assert
            Assert.AreEqual(2, times.Count);
            Assert.IsTrue(times.All(t => t.Start != default && t.Start <= t.End));
            Assert.IsTrue(times.All(t => t.OperationStart <= t.Start && t.End == t.OperationEnd));
            Assert.IsTrue(times[0].End <= times[1].Start);
        }

        [TestMethod]
        public void TestCopyDataToWithoutABulkInsertInsertsTheRowsInBatchesAndReturnsTheNumberOfRows()
        {
            // Setup
            var source = new FakeDbConnection { Data = FakeDbConnection.CreatePersons(5) };
            var destination = new FakeDbConnection();
            var batches = new List<int>();

            // Act
            var actual = source.CopyDataTo("Person", "Customer", destination, bufferSize: 2, progressCallback: p => batches.Add(p.CurrentBatch.RowCount)).RowCount;

            // Assert
            Assert.AreEqual(5, actual);
            CollectionAssert.AreEqual(new[] { 2, 2, 1 }, batches);
        }

        [TestMethod]
        public void TestCopyDataToGivesTheColumnMappingsBehaviorToTheBulkInsert()
        {
            // Setup
            DbSettingMapper.Add<FakeBulkSettingDbConnection>(new FakeBulkDbSetting(), true);
            var destination = new FakeBulkSettingDbConnection();

            // Act
            new FakeDbConnection().CopyDataTo("Person", destination, columnMappingsBehavior: LoadColumnMappingBehavior.Strict);
            new FakeDbConnection().CopyDataTo("Person", "Customer", destination, columnMappingsBehavior: LoadColumnMappingBehavior.StrictBypass);
            new FakeDbConnection().CopyDataTo<Person>(destination, columnMappingsBehavior: LoadColumnMappingBehavior.Automatic);

            // Assert
            CollectionAssert.AreEqual(new[] { "Strict", "StrictBypass", "Automatic" }, FakeBulkOperations.Calls.Select(c => c.ColumnMappingsBehavior).ToArray());
        }

        [TestMethod]
        public void TestCopyDataToWithARegisteredConnectionGivesTheColumnMappingsBehaviorToTheBulkInsert()
        {
            // Setup
            DbSettingMapper.Add<FakeBulkSettingDbConnection>(new FakeBulkDbSetting(), true);
            var name = Register(new FakeBulkSettingDbConnection());

            // Act
            new FakeDbConnection().CopyDataTo("Person", name, columnMappingsBehavior: LoadColumnMappingBehavior.Strict);
            new FakeDbConnection().CopyDataTo<Person>("Customer", name, columnMappingsBehavior: LoadColumnMappingBehavior.StrictBypass);

            // Assert
            CollectionAssert.AreEqual(new[] { "Strict", "StrictBypass" }, FakeBulkOperations.Calls.Select(c => c.ColumnMappingsBehavior).ToArray());
        }

        [TestMethod]
        public void TestCopyDataToWithoutABulkInsertIgnoresTheColumnMappingsBehavior()
        {
            // Setup
            var destination = new FakeDbConnection();

            // Act
            var actual = new FakeDbConnection().CopyDataTo("Person", "Customer", destination, columnMappingsBehavior: LoadColumnMappingBehavior.Strict).RowCount;

            // Assert
            Assert.AreEqual(3, actual);
            Assert.AreEqual(3, destination.Executions.Count(e => e.Kind == "NonQuery" && e.CommandText.StartsWith("INSERT", StringComparison.Ordinal)));
        }

        [TestMethod]
        public void ThrowExceptionOnCopyDataToIfTheColumnMappingsBehaviorIsNotDefined()
        {
            // Act/Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => GetConnection().CopyDataTo("Person", "Person", GetConnection(), columnMappingsBehavior: (LoadColumnMappingBehavior)99));
        }

        [TestMethod]
        public void ThrowExceptionOnCopyDataToIfTheColumnMappingsBehaviorIsNotTheDefaultAndTheSettingOfTheBulkInsertHasNoBehavior()
        {
            // Act/Assert
            Assert.Throws<InvalidOperationException>(() => new FakeDbConnection().CopyDataTo("Person", new FakeBulkDbConnection(), columnMappingsBehavior: LoadColumnMappingBehavior.StrictBypass));
        }

        [TestMethod]
        public void TestCopyDataToWithATargetTableOfAnotherSchemaCopiesTheSchemaAndTheRowsIntoThatSchema()
        {
            // Setup
            var composer = new Mock<ISchemaComposer>();
            composer.Setup(c => c.ComposeSchemas(It.IsAny<IEnumerable<TableSchema>>())).Returns<IEnumerable<TableSchema>>(schemas => schemas.Select(s => $"CREATE TABLE {s.Table.Schema}.{s.Table.Name};").ToList());
            SchemaComposerMapper.Add<FakeBulkKeyedDbConnection>(composer.Object, true);
            var reader = new Mock<ISchemaReader>();
            var schema = new TableSchema("Person", "dbo") { Columns = { new ColumnInfo() } };
            var relationship = new RelationshipInfo();
            typeof(RelationshipInfo).GetProperty(nameof(RelationshipInfo.Schema)).SetValue(relationship, schema);
            reader.Setup(r => r.GetDependencyOrder(It.IsAny<IEnumerable<string>>())).Returns(new[] { relationship });
            SchemaReaderMapper.Add<FakeDbConnection>(reader.Object, true);
            var destination = FakeBulkDestination.Create();
            FakeBulkOperations.Reset();

            // Act
            var actual = new FakeDbConnection().CopyDataTo("Person", "Sales.Person", destination).RowCount;

            // Assert
            Assert.AreEqual(3, actual);
            Assert.AreEqual("CREATE TABLE Sales.Person;", FakeBulkOperations.Statements.First().CommandText);
            Assert.AreEqual("[Sales].[Person]", FakeBulkOperations.Calls.Last().TableName);
        }

        [TestMethod]
        public void TestCopyDataToWithARelationshipBehaviorCopiesTheRowsOfTheRelatedTablesFirst()
        {
            // Setup
            var composer = new Mock<ISchemaComposer>();
            composer.Setup(c => c.ComposeSchemas(It.IsAny<IEnumerable<TableSchema>>())).Returns<IEnumerable<TableSchema>>(schemas => schemas.Select(s => $"CREATE TABLE {s.Table.Name};").ToList());
            SchemaComposerMapper.Add<FakeBulkKeyedDbConnection>(composer.Object, true);
            var relationships = new[] { "Country", "Person" }.Select(name =>
            {
                var relationship = new RelationshipInfo();
                typeof(RelationshipInfo).GetProperty(nameof(RelationshipInfo.Schema)).SetValue(relationship, new TableSchema(name, "dbo") { Columns = { new ColumnInfo() } });
                return relationship;
            }).ToList();
            var reader = new Mock<ISchemaReader>();
            reader.Setup(r => r.GetRelatedTables(It.IsAny<IEnumerable<string>>(), It.IsAny<RelatedTableBehavior>())).Returns(new[] { "Person", "Country" });
            reader.Setup(r => r.GetDependencyOrder(It.IsAny<IEnumerable<string>>())).Returns(relationships);
            reader.Setup(r => r.GetTableSchema(It.IsAny<string>())).Returns<string>(name => new TableSchema(name, "dbo") { Columns = { new ColumnInfo() } });
            SchemaReaderMapper.Add<FakeDbConnection>(reader.Object, true);
            var destination = FakeBulkDestination.Create();
            FakeBulkOperations.Reset();

            // Act
            var actual = new FakeDbConnection().CopyDataTo("Person", "Person", destination, relationshipBehavior: CopyRelationshipBehavior.Parents).RowCount;

            // Assert
            Assert.AreEqual(6, actual);
            CollectionAssert.AreEqual(new[] { "[dbo].[Country]", "[dbo].[Person]" }, FakeBulkOperations.Calls.Select(c => c.TableName).ToArray());
        }

        #endregion

        #region CopyDataTo (Registered Connection, Async)

        [TestMethod]
        public async Task TestCopyDataToAsyncGivesTheColumnMappingsBehaviorToTheBulkInsert()
        {
            // Setup
            DbSettingMapper.Add<FakeBulkSettingDbConnection>(new FakeBulkDbSetting(), true);
            var destination = new FakeBulkSettingDbConnection();

            // Act
            await new FakeDbConnection().CopyDataToAsync("Person", destination, columnMappingsBehavior: LoadColumnMappingBehavior.Strict);
            await new FakeDbConnection().CopyDataToAsync("Person", "Customer", destination, columnMappingsBehavior: LoadColumnMappingBehavior.StrictBypass);
            await new FakeDbConnection().CopyDataToAsync<Person>(destination, columnMappingsBehavior: LoadColumnMappingBehavior.Automatic);

            // Assert
            CollectionAssert.AreEqual(new[] { "Strict", "StrictBypass", "Automatic" }, FakeBulkOperations.Calls.Select(c => c.ColumnMappingsBehavior).ToArray());
        }

        [TestMethod]
        public async Task TestCopyDataToAsyncWithARegisteredConnectionGivesTheColumnMappingsBehaviorToTheBulkInsert()
        {
            // Setup
            DbSettingMapper.Add<FakeBulkSettingDbConnection>(new FakeBulkDbSetting(), true);
            var name = Register(new FakeBulkSettingDbConnection());

            // Act
            await new FakeDbConnection().CopyDataToAsync("Person", name, columnMappingsBehavior: LoadColumnMappingBehavior.StrictBypass);
            await new FakeDbConnection().CopyDataToAsync<Person>("Customer", name, columnMappingsBehavior: LoadColumnMappingBehavior.Strict);

            // Assert
            CollectionAssert.AreEqual(new[] { "StrictBypass", "Strict" }, FakeBulkOperations.Calls.Select(c => c.ColumnMappingsBehavior).ToArray());
        }

        [TestMethod]
        public async Task ThrowExceptionOnCopyDataToAsyncIfTheColumnMappingsBehaviorIsNotDefined()
        {
            // Act/Assert
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => GetConnection().CopyDataToAsync("Person", "Person", GetConnection(), columnMappingsBehavior: (LoadColumnMappingBehavior)99));
        }

        [TestMethod]
        public async Task TestCopyDataToAsyncOfAnEntityToARegisteredConnectionCopiesTheTableOfTheEntity()
        {
            // Setup
            var name = Register(new FakeBulkDbConnection());

            // Act
            var actual = (await new FakeDbConnection().CopyDataToAsync<Person>(name)).RowCount;

            // Assert
            Assert.AreEqual(3, actual);
            Assert.AreEqual("Person", Call().TableName);
        }

        [TestMethod]
        public async Task TestCopyDataToAsyncOfAnEntityWithATargetTableToARegisteredConnectionCopiesIntoTheTargetTable()
        {
            // Setup
            var name = Register(new FakeBulkDbConnection());

            // Act
            var actual = (await new FakeDbConnection().CopyDataToAsync<Person>("Customer", name)).RowCount;

            // Assert
            Assert.AreEqual(3, actual);
            Assert.AreEqual("Customer", Call().TableName);
        }

        [TestMethod]
        public async Task TestCopyDataToAsyncOfATableToARegisteredConnectionCopiesIntoTheSameTable()
        {
            // Setup
            var name = Register(new FakeBulkDbConnection());

            // Act
            var actual = (await new FakeDbConnection().CopyDataToAsync("Person", name)).RowCount;

            // Assert
            Assert.AreEqual(3, actual);
            Assert.AreEqual("Person", Call().TableName);
        }

        [TestMethod]
        public async Task TestCopyDataToAsyncOfASourceTableToARegisteredConnectionCopiesIntoTheTargetTable()
        {
            // Setup
            var name = Register(new FakeBulkDbConnection());

            // Act
            var actual = (await new FakeDbConnection().CopyDataToAsync("Person", "Customer", name)).RowCount;

            // Assert
            Assert.AreEqual(3, actual);
            Assert.AreEqual("Customer", Call().TableName);
        }

        [TestMethod]
        public async Task ThrowExceptionOnCopyDataToAsyncIfTheDestinationConnectionIsNotRegistered()
        {
            // Act/Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => new FakeDbConnection().CopyDataToAsync("Person", "NotRegistered-" + Guid.NewGuid().ToString("N")));
            await Assert.ThrowsAsync<InvalidOperationException>(() => new FakeDbConnection().CopyDataToAsync("Person", "Customer", "NotRegistered-" + Guid.NewGuid().ToString("N")));
            await Assert.ThrowsAsync<InvalidOperationException>(() => new FakeDbConnection().CopyDataToAsync<Person>("NotRegistered-" + Guid.NewGuid().ToString("N")));
            await Assert.ThrowsAsync<InvalidOperationException>(() => new FakeDbConnection().CopyDataToAsync<Person>("Customer", "NotRegistered-" + Guid.NewGuid().ToString("N")));
        }

        #endregion

        #region CopyDataTo (IDbConnection, Async)

        [TestMethod]
        public async Task TestCopyDataToAsyncOfAnEntityCopiesTheTableOfTheEntity()
        {
            // Act
            var actual = (await new FakeDbConnection().CopyDataToAsync<Person>(new FakeBulkDbConnection())).RowCount;

            // Assert
            Assert.AreEqual(3, actual);
            Assert.AreEqual("Person", Call().TableName);
        }

        [TestMethod]
        public async Task TestCopyDataToAsyncOfAnEntityFiltersTheRowsWithTheExpression()
        {
            // Setup
            var source = new FakeDbConnection();

            // Act
            await source.CopyDataToAsync<Person>(new FakeBulkDbConnection(), p => p.Id > 1);

            // Assert
            StringAssert.Contains(source.Executions.Single().CommandText, "WHERE");
        }

        [TestMethod]
        public async Task TestCopyDataToAsyncOfAnEntityWithATargetTableCopiesIntoTheTargetTable()
        {
            // Act
            var actual = (await new FakeDbConnection().CopyDataToAsync<Person>("Customer", new FakeBulkDbConnection())).RowCount;

            // Assert
            Assert.AreEqual(3, actual);
            Assert.AreEqual("Customer", Call().TableName);
        }

        [TestMethod]
        public async Task TestCopyDataToAsyncOfAnEntityWithATargetTableFiltersTheRowsWithTheExpression()
        {
            // Setup
            var source = new FakeDbConnection();

            // Act
            await source.CopyDataToAsync<Person>("Customer", new FakeBulkDbConnection(), p => p.Id > 1);

            // Assert
            StringAssert.Contains(source.Executions.Single().CommandText, "WHERE");
        }

        [TestMethod]
        public async Task TestCopyDataToAsyncOfATableCopiesIntoTheSameTable()
        {
            // Act
            var actual = (await new FakeDbConnection().CopyDataToAsync("Person", new FakeBulkDbConnection())).RowCount;

            // Assert
            Assert.AreEqual(3, actual);
            Assert.AreEqual("Person", Call().TableName);
        }

        [TestMethod]
        public async Task TestCopyDataToAsyncOfATableFiltersTheRowsWithTheQueryGroup()
        {
            // Setup
            var source = new FakeDbConnection();

            // Act
            await source.CopyDataToAsync("Person", new FakeBulkDbConnection(), Filter());

            // Assert
            StringAssert.Contains(source.Executions.Single().CommandText, "WHERE");
        }

        [TestMethod]
        public async Task TestCopyDataToAsyncWithTheBufferSizeTheTimeoutTheTransactionAndTheCancellationTokenGivesThemToTheBulkInsert()
        {
            // Setup
            var transaction = new Mock<IDbTransaction>().Object;
            using (var tokenSource = new CancellationTokenSource())
            {
                // Act
                await new FakeDbConnection().CopyDataToAsync("Person", "Customer", new FakeBulkDbConnection(), bufferSize: 250, commandTimeout: 12, transaction: transaction, cancellationToken: tokenSource.Token);

                // Assert
                var call = Call();
                Assert.AreEqual("BulkInsertAsync", call.Method);
                Assert.AreEqual(250, call.BatchSize);
                Assert.AreEqual(12, call.Timeout);
                Assert.AreSame(transaction, call.Transaction);
                Assert.AreEqual(tokenSource.Token, call.CancellationToken);
            }
        }

        [TestMethod]
        public async Task TestCopyDataToAsyncCallsTheProgressCallbackWithTheProgressOfTheCopy()
        {
            // Setup
            var progress = new List<(int Batch, int Rows, int Total)>();

            // Act
            var actual = (await new FakeDbConnection().CopyDataToAsync("Person", "Customer", new FakeBulkDbConnection(),
                progressCallback: p => progress.Add((p.BatchCount, p.CurrentBatch.RowCount, p.RowCount)))).RowCount;

            // Assert
            Assert.AreEqual(3, actual);
            CollectionAssert.AreEqual(new[] { (1, 3, 3) }, progress);
        }

        [TestMethod]
        public async Task TestCopyDataToAsyncWithoutABulkInsertInsertsTheRowsInBatchesAndReturnsTheNumberOfRows()
        {
            // Setup
            var source = new FakeDbConnection { Data = FakeDbConnection.CreatePersons(5) };
            var batches = new List<int>();

            // Act
            var actual = (await source.CopyDataToAsync("Person", "Customer", new FakeDbConnection(), bufferSize: 2, progressCallback: p => batches.Add(p.CurrentBatch.RowCount))).RowCount;

            // Assert
            Assert.AreEqual(5, actual);
            CollectionAssert.AreEqual(new[] { 2, 2, 1 }, batches);
        }

        [TestMethod]
        public async Task TestCopyDataToAsyncWithARelationshipBehaviorCopiesTheRowsOfTheRelatedTablesFirst()
        {
            // Setup
            var composer = new Mock<ISchemaComposer>();
            composer.Setup(c => c.ComposeSchemas(It.IsAny<IEnumerable<TableSchema>>())).Returns<IEnumerable<TableSchema>>(schemas => schemas.Select(s => $"CREATE TABLE {s.Table.Name};").ToList());
            SchemaComposerMapper.Add<FakeBulkKeyedDbConnection>(composer.Object, true);
            var relationships = new[] { "Country", "Person" }.Select(name =>
            {
                var relationship = new RelationshipInfo();
                typeof(RelationshipInfo).GetProperty(nameof(RelationshipInfo.Schema)).SetValue(relationship, new TableSchema(name, "dbo") { Columns = { new ColumnInfo() } });
                return relationship;
            }).ToList();
            var reader = new Mock<ISchemaReader>();
            reader.Setup(r => r.GetRelatedTablesAsync(It.IsAny<IEnumerable<string>>(), It.IsAny<RelatedTableBehavior>(), It.IsAny<CancellationToken>())).ReturnsAsync(new[] { "Person", "Country" });
            reader.Setup(r => r.GetDependencyOrder(It.IsAny<IEnumerable<string>>())).Returns(relationships);
            reader.Setup(r => r.GetDependencyOrderAsync(It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>())).ReturnsAsync(relationships);
            reader.Setup(r => r.GetTableSchemaAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).Returns<string, CancellationToken>((name, _) => Task.FromResult(new TableSchema(name, "dbo") { Columns = { new ColumnInfo() } }));
            SchemaReaderMapper.Add<FakeDbConnection>(reader.Object, true);
            var destination = FakeBulkDestination.Create();
            FakeBulkOperations.Reset();

            // Act
            var actual = (await new FakeDbConnection().CopyDataToAsync("Person", "Person", destination, relationshipBehavior: CopyRelationshipBehavior.ParentsAndChildren)).RowCount;

            // Assert
            Assert.AreEqual(6, actual);
            CollectionAssert.AreEqual(new[] { "[dbo].[Country]", "[dbo].[Person]" }, FakeBulkOperations.Calls.Select(c => c.TableName).ToArray());
        }

        [TestMethod]
        public async Task ThrowExceptionOnCopyDataToAsyncIfTheOperationIsCancelled()
        {
            // Setup
            using (var tokenSource = new CancellationTokenSource())
            {
                tokenSource.Cancel();

                // Act/Assert
                await Assert.ThrowsAsync<OperationCanceledException>(() =>
                    new FakeDbConnection().CopyDataToAsync("Person", "Person", new FakeDbConnection(), cancellationToken: tokenSource.Token));
            }
        }

        #endregion

        #region Mappings

        [TestMethod]
        public void TestCopyDataToWithMappingsSelectsOnlyTheMappedColumnsAliasedWithTheDestinationColumns()
        {
            // Setup
            var source = new FakeDbConnection();
            var mappings = new[] { new CopyColumnMapping("Id", "Id"), new CopyColumnMapping("Name", "Birthday") };

            // Act
            source.CopyDataTo("Person", "Customer", FakeBulkDestination.Create(), mappings: mappings);

            // Assert
            var commandText = source.Executions.Single().CommandText;
            StringAssert.Contains(commandText, "[Id] AS [Id], [Name] AS [Birthday]");
            Assert.IsFalse(commandText.Contains("[Birthday],"));
        }

        [TestMethod]
        public async Task TestCopyDataToAsyncWithMappingsSelectsOnlyTheMappedColumnsAliasedWithTheDestinationColumns()
        {
            // Setup
            var source = new FakeDbConnection();
            var mappings = new[] { new CopyColumnMapping("Name", "Birthday") };

            // Act
            await source.CopyDataToAsync("Person", "Customer", FakeBulkDestination.Create(), mappings: mappings);

            // Assert
            StringAssert.Contains(source.Executions.Single().CommandText, "[Name] AS [Birthday]");
        }

        [TestMethod]
        public void TestCopyDataToWithMappingsSelectsAllTheColumnsIfTheMappingsAreNullOrEmpty()
        {
            // Setup
            var source = new FakeDbConnection();

            // Act
            source.CopyDataTo("Person", "Customer", FakeBulkDestination.Create(), mappings: null);
            source.CopyDataTo("Person", "Customer", FakeBulkDestination.Create(), mappings: new List<CopyColumnMapping>());

            // Assert
            Assert.IsTrue(source.Executions.All(e => !e.CommandText.Contains(" AS ")));
            Assert.IsTrue(source.Executions.All(e => e.CommandText.Contains("[Id]") && e.CommandText.Contains("[Name]") && e.CommandText.Contains("[Birthday]")));
        }

        [TestMethod]
        public void ThrowExceptionOnCopyDataToIfTheMappingsHaveANullItem()
        {
            // Act/Assert
            Assert.Throws<ArgumentException>(() =>
                new FakeDbConnection().CopyDataTo("Person", "Customer", new FakeDbConnection(), mappings: new CopyColumnMapping[] { null }));
        }

        [TestMethod]
        public void ThrowExceptionOnCopyDataToIfAMappingHasAnEmptyColumn()
        {
            // Act/Assert
            Assert.Throws<ArgumentException>(() =>
                new FakeDbConnection().CopyDataTo("Person", "Customer", new FakeDbConnection(), mappings: new[] { new CopyColumnMapping(" ", "Id") }));
            Assert.Throws<ArgumentException>(() =>
                new FakeDbConnection().CopyDataTo("Person", "Customer", new FakeDbConnection(), mappings: new[] { new CopyColumnMapping("Id", null) }));
        }

        [TestMethod]
        public void ThrowExceptionOnCopyDataToIfADestinationColumnIsMappedMoreThanOnce()
        {
            // Act/Assert
            Assert.Throws<ArgumentException>(() =>
                new FakeDbConnection().CopyDataTo("Person", "Customer", new FakeDbConnection(), mappings: new[] { new CopyColumnMapping("Id", "Name"), new CopyColumnMapping("Name", "name") }));
        }

        [TestMethod]
        public void ThrowExceptionOnCopyDataToIfASourceColumnDoesNotExistOnTheSourceTable()
        {
            // Act/Assert
            var ex = Assert.Throws<ArgumentException>(() =>
                new FakeDbConnection().CopyDataTo("Person", "Customer", new FakeDbConnection(), mappings: new[] { new CopyColumnMapping("Missing", "Id") }));
            StringAssert.Contains(ex.Message, "Missing");
        }

        [TestMethod]
        public void ThrowExceptionOnCopyDataToIfADestinationColumnDoesNotExistOnTheTargetTable()
        {
            // Act/Assert
            var ex = Assert.Throws<ArgumentException>(() =>
                new FakeDbConnection().CopyDataTo("Person", "Customer", new FakeDbConnection(), mappings: new[] { new CopyColumnMapping("Id", "Missing") }));
            StringAssert.Contains(ex.Message, "Missing");
        }

        [TestMethod]
        public async Task ThrowExceptionOnCopyDataToAsyncIfASourceColumnDoesNotExistOnTheSourceTable()
        {
            // Act/Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                new FakeDbConnection().CopyDataToAsync("Person", "Customer", new FakeDbConnection(), mappings: new[] { new CopyColumnMapping("Missing", "Id") }));
        }

        [TestMethod]
        public void TestCopyDataToOfAnEntityWithMappingsToARegisteredConnectionSelectsTheMappedColumns()
        {
            // Setup
            var source = new FakeDbConnection();
            var name = Register(FakeBulkDestination.Create());

            // Act
            var actual = source.CopyDataTo<Person>("Customer", name, new[] { new CopyColumnMapping("Name", "Birthday") }).RowCount;

            // Assert
            Assert.AreEqual(3, actual);
            StringAssert.Contains(source.Executions.Single().CommandText, "[Name] AS [Birthday]");
        }

        [TestMethod]
        public void TestCopyDataToOfAnEntityWithMappingsSelectsTheMappedColumns()
        {
            // Setup
            var source = new FakeDbConnection();

            // Act
            var actual = source.CopyDataTo<Person>(FakeBulkDestination.Create(), new[] { new CopyColumnMapping("Name", "Birthday") }).RowCount;

            // Assert
            Assert.AreEqual(3, actual);
            StringAssert.Contains(source.Executions.Single().CommandText, "[Name] AS [Birthday]");
        }

        [TestMethod]
        public void TestCopyDataToOfATableWithMappingsToARegisteredConnectionSelectsTheMappedColumns()
        {
            // Setup
            var source = new FakeDbConnection();
            var name = Register(FakeBulkDestination.Create());

            // Act
            source.CopyDataTo("Person", name, new[] { new CopyColumnMapping("Name", "Birthday") });

            // Assert
            StringAssert.Contains(source.Executions.Single().CommandText, "[Name] AS [Birthday]");
        }

        [TestMethod]
        public void TestCopyDataToWithMappingsAndAFilterSelectsTheMappedColumnsOfTheMatchingRows()
        {
            // Setup
            var source = new FakeDbConnection();

            // Act
            source.CopyDataTo("Person", "Customer", FakeBulkDestination.Create(), new[] { new CopyColumnMapping("Name", "Birthday") }, Filter());

            // Assert
            StringAssert.Contains(source.Executions.Single().CommandText, "[Name] AS [Birthday]");
            StringAssert.Contains(source.Executions.Single().CommandText, "WHERE");
        }

        [TestMethod]
        public async Task TestCopyDataToAsyncOfAnEntityWithMappingsToARegisteredConnectionSelectsTheMappedColumns()
        {
            // Setup
            var source = new FakeDbConnection();
            var name = Register(FakeBulkDestination.Create());

            // Act
            var actual = (await source.CopyDataToAsync<Person>("Customer", name, new[] { new CopyColumnMapping("Name", "Birthday") })).RowCount;

            // Assert
            Assert.AreEqual(3, actual);
            StringAssert.Contains(source.Executions.Single().CommandText, "[Name] AS [Birthday]");
        }

        [TestMethod]
        public async Task TestCopyDataToAsyncOfAnEntityWithMappingsSelectsTheMappedColumns()
        {
            // Setup
            var source = new FakeDbConnection();

            // Act
            var actual = (await source.CopyDataToAsync<Person>(FakeBulkDestination.Create(), new[] { new CopyColumnMapping("Name", "Birthday") })).RowCount;

            // Assert
            Assert.AreEqual(3, actual);
            StringAssert.Contains(source.Executions.Single().CommandText, "[Name] AS [Birthday]");
        }

        [TestMethod]
        public async Task TestCopyDataToAsyncOfATableWithMappingsToARegisteredConnectionSelectsTheMappedColumns()
        {
            // Setup
            var source = new FakeDbConnection();
            var name = Register(FakeBulkDestination.Create());

            // Act
            await source.CopyDataToAsync("Person", name, new[] { new CopyColumnMapping("Name", "Birthday") });

            // Assert
            StringAssert.Contains(source.Executions.Single().CommandText, "[Name] AS [Birthday]");
        }

        [TestMethod]
        public void ThrowExceptionOnCopyDataToWithMappingsToARegisteredConnectionIfASourceColumnDoesNotExist()
        {
            // Setup
            var name = Register(new FakeDbConnection());

            // Act/Assert
            Assert.Throws<ArgumentException>(() =>
                new FakeDbConnection().CopyDataTo<Person>("Customer", name, new[] { new CopyColumnMapping("Missing", "Id") }));
        }

        #endregion

        #region Data Interceptors

        private static IList<IDataInterceptor<CopyRow>> Interceptors(params Func<CopyRow, CopyRow>[] transforms) =>
            transforms.Select(transform => (IDataInterceptor<CopyRow>)new FakeDataInterceptor(transform)).ToList();

        [TestMethod]
        public void TestCopyDataToCopiesTheRowsThatTheInterceptorsGiveInOrder()
        {
            // Setup
            var destination = new FakeDbConnection();
            var interceptors = Interceptors(
                row => { row["Name"] = ((string)row["Name"]).ToUpperInvariant(); return row; },
                row => { row["Name"] = row["Name"] + "!"; return row; });

            // Act
            new FakeDbConnection().CopyDataTo("Person", "Customer", destination, dataInterceptors: interceptors);

            // Assert
            var values = destination.Executions.Where(e => e.CommandText.StartsWith("INSERT INTO", StringComparison.Ordinal)).SelectMany(e => e.Parameters.Values).OfType<string>().ToList();
            CollectionAssert.AreEquivalent(new[] { "NAME 1!", "NAME 2!", "NAME 3!" }, values);
        }

        [TestMethod]
        public async Task TestCopyDataToAsyncCopiesTheRowsThatTheInterceptorsGive()
        {
            // Setup
            var destination = new FakeDbConnection();
            var interceptors = Interceptors(row => { row["Name"] = "Changed"; return row; });

            // Act
            await new FakeDbConnection().CopyDataToAsync("Person", "Customer", destination, dataInterceptors: interceptors);

            // Assert
            var values = destination.Executions.Where(e => e.CommandText.StartsWith("INSERT INTO", StringComparison.Ordinal)).SelectMany(e => e.Parameters.Values).OfType<string>().ToList();
            CollectionAssert.AreEqual(new[] { "Changed", "Changed", "Changed" }, values);
        }

        [TestMethod]
        public void TestCopyDataToBulkInsertsTheColumnsThatTheInterceptorsGive()
        {
            // Setup
            var interceptors = Interceptors(row => row.Add("Extra", typeof(string), "e"), row => { row.Remove("Birthday"); return row; });
            FakeBulkOperations.Reset();

            // Act
            new FakeDbConnection().CopyDataTo("Person", "Customer", FakeBulkDestination.Create(), dataInterceptors: interceptors);

            // Assert
            CollectionAssert.AreEqual(new[] { "Id", "Name", "Extra" }, FakeBulkOperations.LastColumns);
        }

        [TestMethod]
        public void TestCopyDataToGivesTheInterceptorsTheColumnsOfTheMappings()
        {
            // Setup
            var names = new List<string>();
            var interceptors = Interceptors(row => { names.AddRange(row.Columns.Select(c => c.Name)); return row; });
            var mappings = new[] { new CopyColumnMapping("Id", "Id"), new CopyColumnMapping("Name", "Birthday") };

            // Act
            new FakeDbConnection().CopyDataTo("Person", "Customer", FakeBulkDestination.Create(), mappings, dataInterceptors: interceptors);

            // Assert
            Assert.IsTrue(names.Count > 0);
            Assert.IsTrue(names.All(name => name == "Id" || name == "Name" || name == "Birthday"));
        }

        [TestMethod]
        public void TestCopyDataToLeavesTheIdentityColumnOutAfterTheInterceptors()
        {
            // Setup
            var interceptors = Interceptors(row => { row["Id"] = 500; return row; });
            FakeBulkOperations.Reset();

            // Act
            new FakeDbConnection().CopyDataTo("Person", "Customer", FakeBulkDestination.Create(), identityBehavior: LoadIdentityBehavior.Ignore, dataInterceptors: interceptors);

            // Assert
            CollectionAssert.AreEqual(new[] { "Name", "Birthday" }, FakeBulkOperations.LastColumns);
        }

        [TestMethod]
        public void TestCopyDataToWithInterceptorsAndBypassExistingRowsBulkMergesTheRowsWithTheQualifiers()
        {
            // Setup
            var interceptors = Interceptors(row => { row["Name"] = "Changed"; return row; });
            FakeBulkOperations.Reset();

            // Act
            new FakeDbConnection().CopyDataTo("Person", "Customer", FakeBulkDestination.Create(), bypassExistingRows: true, dataInterceptors: interceptors);

            // Assert
            var call = FakeBulkOperations.Calls.Single();
            Assert.AreEqual("BulkMerge", call.Method);
            CollectionAssert.AreEqual(new[] { "Id" }, call.Qualifiers);
        }

        [TestMethod]
        public void ThrowExceptionOnCopyDataToWithBypassExistingRowsIfAnInterceptorRemovesAQualifier()
        {
            // Setup
            var interceptors = Interceptors(row => { row.Remove("Id"); return row; });
            FakeBulkOperations.Reset();

            // Act/Assert
            Assert.Throws<InvalidOperationException>(() => new FakeDbConnection().CopyDataTo("Person", "Customer", FakeBulkDestination.Create(), bypassExistingRows: true, dataInterceptors: interceptors));
            Assert.AreEqual(0, FakeBulkOperations.Calls.Length);
        }

        [TestMethod]
        public void ThrowExceptionOnCopyDataToIfAnInterceptorReturnsNull()
        {
            // Setup
            var interceptors = Interceptors(row => null);

            // Act/Assert
            Assert.Throws<InvalidOperationException>(() => new FakeDbConnection().CopyDataTo("Person", "Customer", FakeBulkDestination.Create(), dataInterceptors: interceptors));
        }

        [TestMethod]
        public void ThrowExceptionOnCopyDataToIfAnInterceptorIsNull()
        {
            // Act/Assert
            Assert.Throws<ArgumentException>(() => new FakeDbConnection().CopyDataTo("Person", "Customer", FakeBulkDestination.Create(), dataInterceptors: new List<IDataInterceptor<CopyRow>> { null }));
        }

        [TestMethod]
        public void TestCopyDataToWithoutInterceptorsCopiesTheRowsAsTheyAre()
        {
            // Setup
            var destination = new FakeDbConnection();

            // Act
            new FakeDbConnection().CopyDataTo("Person", "Customer", destination, dataInterceptors: new List<IDataInterceptor<CopyRow>>());

            // Assert
            var values = destination.Executions.Where(e => e.CommandText.StartsWith("INSERT INTO", StringComparison.Ordinal)).SelectMany(e => e.Parameters.Values).OfType<string>().ToList();
            CollectionAssert.AreEquivalent(new[] { "Name 1", "Name 2", "Name 3" }, values);
        }

        #endregion
    }
}
