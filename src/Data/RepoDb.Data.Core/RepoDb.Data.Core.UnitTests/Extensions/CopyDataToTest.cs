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

        private static readonly CopyDataRelationshipBehavior[] RelationshipBehaviors =
        {
            CopyDataRelationshipBehavior.Parents,
            CopyDataRelationshipBehavior.Children,
            CopyDataRelationshipBehavior.ParentsAndChildren
        };

        private static readonly CopySchemaExistsBehavior[] ExistsBehaviors =
        {
            CopySchemaExistsBehavior.Align,
            CopySchemaExistsBehavior.Throw,
            CopySchemaExistsBehavior.Drop
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
        public void ThrowExceptionOnCopyDataToIfTheBatchSizeIsNotGreaterThanZero()
        {
            // Act/Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => GetConnection().CopyDataTo("Person", "Person", GetConnection(), batchSize: 0));
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
        public async Task ThrowExceptionOnCopyDataToAsyncIfTheBatchSizeIsNotGreaterThanZero()
        {
            // Act/Assert
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => GetConnection().CopyDataToAsync("Person", "Person", GetConnection(), batchSize: 0));
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
            var actual = new FakeDbConnection().CopyDataTo<Person>(name);

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
            var actual = new FakeDbConnection().CopyDataTo<Person>("Customer", name);

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
            var actual = new FakeDbConnection().CopyDataTo("Person", name);

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
            var actual = source.CopyDataTo("Person", "Customer", name);

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
            var actual = new FakeDbConnection().CopyDataTo<Person>(new FakeBulkDbConnection());

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
            var actual = new FakeDbConnection().CopyDataTo<Person>("Customer", new FakeBulkDbConnection());

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
            var actual = new FakeDbConnection().CopyDataTo("Person", new FakeBulkDbConnection());

            // Assert
            Assert.AreEqual(3, actual);
            Assert.AreEqual("Person", Call().TableName);
        }

        [TestMethod]
        public void TestCopyDataToWithTheBatchSizeTheTimeoutAndTheTransactionGivesThemToTheBulkInsert()
        {
            // Setup
            var transaction = new Mock<IDbTransaction>().Object;
            var source = new FakeDbConnection();

            // Act
            source.CopyDataTo("Person", "Customer", new FakeBulkDbConnection(), batchSize: 250, commandTimeout: 12, transaction: transaction);

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
                progressCallback: p => progress.Add((p.BatchNumber, p.RowCount, p.TotalCopiedRowCount, p.StartTime, p.EndTime)));

            // Assert
            Assert.AreEqual(3, actual);
            Assert.AreEqual(1, progress.Count);
            Assert.AreEqual((1, 3, 3), (progress[0].Batch, progress[0].Rows, progress[0].Total));
            Assert.AreNotEqual(default(DateTime), progress[0].Start);
            Assert.IsTrue(progress[0].End >= progress[0].Start);
        }

        [TestMethod]
        public void TestCopyDataToWithoutABulkInsertInsertsTheRowsInBatchesAndReturnsTheNumberOfRows()
        {
            // Setup
            var source = new FakeDbConnection { Data = FakeDbConnection.CreatePersons(5) };
            var destination = new FakeDbConnection();
            var batches = new List<int>();

            // Act
            var actual = source.CopyDataTo("Person", "Customer", destination, batchSize: 2, progressCallback: p => batches.Add(p.RowCount));

            // Assert
            Assert.AreEqual(5, actual);
            CollectionAssert.AreEqual(new[] { 2, 2, 1 }, batches);
        }

        [TestMethod]
        public void TestCopyDataToWithATargetTableOfAnotherSchemaCopiesTheSchemaAndTheRowsIntoThatSchema()
        {
            // Setup
            var composer = new Mock<ISchemaComposer>();
            composer.Setup(c => c.ComposeSchemas(It.IsAny<IEnumerable<TableSchema>>())).Returns<IEnumerable<TableSchema>>(schemas => schemas.Select(s => $"CREATE TABLE {s.Table.Schema}.{s.Table.Name};").ToList());
            SchemaComposerMapper.Add<FakeDbConnection>(composer.Object, true);
            var reader = new Mock<ISchemaReader>();
            var schema = new TableSchema("Person", "dbo") { Columns = { new ColumnInfo() } };
            var relationship = new RelationshipInfo();
            typeof(RelationshipInfo).GetProperty(nameof(RelationshipInfo.Schema)).SetValue(relationship, schema);
            reader.Setup(r => r.GetDependencyOrder(It.IsAny<IEnumerable<string>>())).Returns(new[] { relationship });
            SchemaReaderMapper.Add<FakeDbConnection>(reader.Object, true);
            var destination = new FakeDbConnection();

            // Act
            var actual = new FakeDbConnection().CopyDataTo("Person", "Sales.Person", destination);

            // Assert
            Assert.AreEqual(3, actual);
            Assert.AreEqual("CREATE TABLE Sales.Person;", destination.Executions.First(e => e.Kind == "NonQuery").CommandText);
            StringAssert.Contains(destination.Executions.Last(e => e.Kind == "Scalar").CommandText, "[Sales].[Person]");
        }

        [TestMethod]
        public void TestCopyDataToWithARelationshipBehaviorCopiesTheRowsOfTheRelatedTablesFirst()
        {
            // Setup
            var composer = new Mock<ISchemaComposer>();
            composer.Setup(c => c.ComposeSchemas(It.IsAny<IEnumerable<TableSchema>>())).Returns<IEnumerable<TableSchema>>(schemas => schemas.Select(s => $"CREATE TABLE {s.Table.Name};").ToList());
            SchemaComposerMapper.Add<FakeDbConnection>(composer.Object, true);
            var relationships = new[] { "Country", "Person" }.Select(name =>
            {
                var relationship = new RelationshipInfo();
                typeof(RelationshipInfo).GetProperty(nameof(RelationshipInfo.Schema)).SetValue(relationship, new TableSchema(name, "dbo") { Columns = { new ColumnInfo() } });
                return relationship;
            }).ToList();
            var reader = new Mock<ISchemaReader>();
            reader.Setup(r => r.GetRelatedTables(It.IsAny<IEnumerable<string>>(), It.IsAny<CopySchemaRelationshipBehavior>())).Returns(new[] { "Person", "Country" });
            reader.Setup(r => r.GetDependencyOrder(It.IsAny<IEnumerable<string>>())).Returns(relationships);
            reader.Setup(r => r.GetTableSchema(It.IsAny<string>())).Returns<string>(name => new TableSchema(name, "dbo") { Columns = { new ColumnInfo() } });
            SchemaReaderMapper.Add<FakeDbConnection>(reader.Object, true);
            var destination = new FakeDbConnection();

            // Act
            var actual = new FakeDbConnection().CopyDataTo("Person", "Person", destination, relationshipBehavior: CopyDataRelationshipBehavior.Parents);

            // Assert
            Assert.AreEqual(6, actual);
            var inserts = destination.Executions.Where(e => e.Kind == "Scalar").Select(e => e.CommandText).ToList();
            Assert.IsTrue(inserts.Take(3).All(t => t.Contains("[Country]")));
            Assert.IsTrue(inserts.Skip(3).All(t => t.Contains("[Person]")));
        }

        #endregion

        #region CopyDataTo (Registered Connection, Async)

        [TestMethod]
        public async Task TestCopyDataToAsyncOfAnEntityToARegisteredConnectionCopiesTheTableOfTheEntity()
        {
            // Setup
            var name = Register(new FakeBulkDbConnection());

            // Act
            var actual = await new FakeDbConnection().CopyDataToAsync<Person>(name);

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
            var actual = await new FakeDbConnection().CopyDataToAsync<Person>("Customer", name);

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
            var actual = await new FakeDbConnection().CopyDataToAsync("Person", name);

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
            var actual = await new FakeDbConnection().CopyDataToAsync("Person", "Customer", name);

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
            var actual = await new FakeDbConnection().CopyDataToAsync<Person>(new FakeBulkDbConnection());

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
            var actual = await new FakeDbConnection().CopyDataToAsync<Person>("Customer", new FakeBulkDbConnection());

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
            var actual = await new FakeDbConnection().CopyDataToAsync("Person", new FakeBulkDbConnection());

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
        public async Task TestCopyDataToAsyncWithTheBatchSizeTheTimeoutTheTransactionAndTheCancellationTokenGivesThemToTheBulkInsert()
        {
            // Setup
            var transaction = new Mock<IDbTransaction>().Object;
            using (var tokenSource = new CancellationTokenSource())
            {
                // Act
                await new FakeDbConnection().CopyDataToAsync("Person", "Customer", new FakeBulkDbConnection(), batchSize: 250, commandTimeout: 12, transaction: transaction, cancellationToken: tokenSource.Token);

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
            var actual = await new FakeDbConnection().CopyDataToAsync("Person", "Customer", new FakeBulkDbConnection(),
                progressCallback: p => progress.Add((p.BatchNumber, p.RowCount, p.TotalCopiedRowCount)));

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
            var actual = await source.CopyDataToAsync("Person", "Customer", new FakeDbConnection(), batchSize: 2, progressCallback: p => batches.Add(p.RowCount));

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
            SchemaComposerMapper.Add<FakeDbConnection>(composer.Object, true);
            var relationships = new[] { "Country", "Person" }.Select(name =>
            {
                var relationship = new RelationshipInfo();
                typeof(RelationshipInfo).GetProperty(nameof(RelationshipInfo.Schema)).SetValue(relationship, new TableSchema(name, "dbo") { Columns = { new ColumnInfo() } });
                return relationship;
            }).ToList();
            var reader = new Mock<ISchemaReader>();
            reader.Setup(r => r.GetRelatedTablesAsync(It.IsAny<IEnumerable<string>>(), It.IsAny<CopySchemaRelationshipBehavior>(), It.IsAny<CancellationToken>())).ReturnsAsync(new[] { "Person", "Country" });
            reader.Setup(r => r.GetDependencyOrder(It.IsAny<IEnumerable<string>>())).Returns(relationships);
            reader.Setup(r => r.GetDependencyOrderAsync(It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>())).ReturnsAsync(relationships);
            reader.Setup(r => r.GetTableSchemaAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).Returns<string, CancellationToken>((name, _) => Task.FromResult(new TableSchema(name, "dbo") { Columns = { new ColumnInfo() } }));
            SchemaReaderMapper.Add<FakeDbConnection>(reader.Object, true);
            var destination = new FakeDbConnection();

            // Act
            var actual = await new FakeDbConnection().CopyDataToAsync("Person", "Person", destination, relationshipBehavior: CopyDataRelationshipBehavior.ParentsAndChildren);

            // Assert
            Assert.AreEqual(6, actual);
            var inserts = destination.Executions.Where(e => e.Kind == "Scalar").Select(e => e.CommandText).ToList();
            Assert.IsTrue(inserts.Take(3).All(t => t.Contains("[Country]")));
            Assert.IsTrue(inserts.Skip(3).All(t => t.Contains("[Person]")));
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
    }
}
