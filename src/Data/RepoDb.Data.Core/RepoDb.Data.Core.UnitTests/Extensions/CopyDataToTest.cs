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
using ColumnMapping = RepoDb.Data.Models.ColumnMapping;
using RepoDb.DbSettings;
using RepoDb.Enumerations;
using RepoDb.Exceptions;
using RepoDb.Schema;
using RepoDb.Schema.Enumerations;
using RepoDb.Schema.Models;

namespace RepoDb.Data.Core.UnitTests.Extensions
{
    /// <summary>
    /// Contains the copy data to test behavior.
    /// </summary>
    [TestClass]
    public class CopyDataToTest
    {
        #region Private Methods

        /// <summary>
        /// Performs get connection.
        /// </summary>
        private static IDbConnection GetConnection() => new Mock<IDbConnection>().Object;
        private static readonly RelationshipBehavior[] RelationshipBehaviors =
        {
            RelationshipBehavior.Parents,
            RelationshipBehavior.Children,
            RelationshipBehavior.ParentsAndChildren
        };
        private static readonly TableExistsBehavior[] ExistsBehaviors =
        {
            TableExistsBehavior.Align,
            TableExistsBehavior.Throw,
            TableExistsBehavior.Drop
        };
        #endregion

        #region Arguments

        /// <summary>
        /// Verifies throw exception on copy data to if the connection is null.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnCopyDataToIfTheConnectionIsNull()
        {
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => ((IDbConnection)null).CopyDataTo("Person",
                    "Person",
                    GetConnection()));
        }

        /// <summary>
        /// Verifies throw exception on copy data to if the source table is null.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnCopyDataToIfTheSourceTableIsNull()
        {
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => GetConnection().CopyDataTo(null,
                    "Person",
                    GetConnection()));
        }

        /// <summary>
        /// Verifies throw exception on copy data to if the target table is white space.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnCopyDataToIfTheTargetTableIsWhiteSpace()
        {
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => GetConnection().CopyDataTo("Person",
                    " ",
                    GetConnection()));
        }

        /// <summary>
        /// Verifies throw exception on copy data to if the destination connection is null.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnCopyDataToIfTheDestinationConnectionIsNull()
        {
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => GetConnection().CopyDataTo("Person",
                    "Person",
                    (IDbConnection)null));
        }

        /// <summary>
        /// Verifies throw exception on copy data to if the buffer size is not greater than zero.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnCopyDataToIfTheBufferSizeIsNotGreaterThanZero()
        {
            // Act/Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => GetConnection().CopyDataTo("Person",
                    "Person",
                    GetConnection(),
                    bufferSize: 0));
        }

        #endregion

        #region Schema

        /// <summary>
        /// Verifies throw exception on copy data to if the tables have different names and the relationship behavior is not table only.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnCopyDataToIfTheTablesHaveDifferentNamesAndTheRelationshipBehaviorIsNotTableOnly()
        {
            foreach (var behavior in RelationshipBehaviors)
            {
                // Act/Assert
                Assert.Throws<ArgumentException>(() => GetConnection().CopyDataTo("Person",
                        "Customer",
                        GetConnection(),
                        relationshipBehavior: behavior));
            }
        }

        /// <summary>
        /// Verifies copy data to with different table names and atable existence behavior that is not skip copies the schema with the name of the target table.
        /// </summary>
        [TestMethod]
        public void TestCopyDataToWithDifferentTableNamesAndATableExistenceBehaviorThatIsNotSkipCopiesTheSchemaWithTheNameOfTheTargetTable()
        {
            foreach (var behavior in ExistsBehaviors)
            {
                // Act/Assert (the names are valid, so the schema is copied, but there is no schema reader for the connection)
                Assert.Throws<MissingMappingException>(() => GetConnection().CopyDataTo("Person",
                        "Customer",
                        GetConnection(),
                        tableExistenceBehavior: behavior));
            }
        }

        /// <summary>
        /// Verifies copy data to with different table names and atarget table with aschema copies the schema with the name of the target table.
        /// </summary>
        [TestMethod]
        public void TestCopyDataToWithDifferentTableNamesAndATargetTableWithASchemaCopiesTheSchemaWithTheNameOfTheTargetTable()
        {
            // Act/Assert (the names are valid, so the schema is copied, but there is no schema reader for the connection)
            Assert.Throws<MissingMappingException>(() => GetConnection().CopyDataTo("Person",
                    "Sales.Customer",
                    new CustomDbConnection()));
            Assert.Throws<MissingMappingException>(() => GetConnection().CopyDataTo("Person",
                    "[Sales].[Customer]",
                    new CustomDbConnection()));
        }

        /// <summary>
        /// Verifies copy data to async with different table names and atarget table with aschema copies the schema with the name of the target table.
        /// </summary>
        [TestMethod]
        public async Task TestCopyDataToAsyncWithDifferentTableNamesAndATargetTableWithASchemaCopiesTheSchemaWithTheNameOfTheTargetTable()
        {
            // Act/Assert
            await Assert.ThrowsAsync<MissingMappingException>(() => GetConnection().CopyDataToAsync("Person",
                    "Sales.Customer",
                    new CustomDbConnection()));
        }

        /// <summary>
        /// Verifies copy data to with atarget table of another schema copies the schema.
        /// </summary>
        [TestMethod]
        public void TestCopyDataToWithATargetTableOfAnotherSchemaCopiesTheSchema()
        {
            // Act/Assert (the table has the same name, so it is valid and the schema is copied, but there is no schema reader for the connection)
            Assert.Throws<MissingMappingException>(() => GetConnection().CopyDataTo("Person",
                    "Sales.Person",
                    new CustomDbConnection()));
            Assert.Throws<MissingMappingException>(() => GetConnection().CopyDataTo("Person",
                    "[Sales].[Person]",
                    new CustomDbConnection()));
            Assert.Throws<MissingMappingException>(() => new CustomDbConnection().CopyDataTo("[dbo].[Person]",
                    "Sales.Person",
                    new CustomDbConnection()));
        }

        /// <summary>
        /// Verifies copy data to async with atarget table of another schema copies the schema.
        /// </summary>
        [TestMethod]
        public async Task TestCopyDataToAsyncWithATargetTableOfAnotherSchemaCopiesTheSchema()
        {
            // Act/Assert
            await Assert.ThrowsAsync<MissingMappingException>(() => GetConnection().CopyDataToAsync("Person",
                    "Sales.Person",
                    new CustomDbConnection()));
        }

        /// <summary>
        /// Verifies throw exception on copy data to async if the tables have different names and the relationship behavior is not table only.
        /// </summary>
        [TestMethod]
        public async Task ThrowExceptionOnCopyDataToAsyncIfTheTablesHaveDifferentNamesAndTheRelationshipBehaviorIsNotTableOnly()
        {
            foreach (var behavior in RelationshipBehaviors)
            {
                // Act/Assert
                await Assert.ThrowsAsync<ArgumentException>(() => GetConnection().CopyDataToAsync("Person",
                        "Customer",
                        GetConnection(),
                        relationshipBehavior: behavior));
            }
        }

        /// <summary>
        /// Verifies copy data to async with different table names and atable existence behavior that is not skip copies the schema with the name of the target table.
        /// </summary>
        [TestMethod]
        public async Task TestCopyDataToAsyncWithDifferentTableNamesAndATableExistenceBehaviorThatIsNotSkipCopiesTheSchemaWithTheNameOfTheTargetTable()
        {
            foreach (var behavior in ExistsBehaviors)
            {
                // Act/Assert
                await Assert.ThrowsAsync<MissingMappingException>(() => GetConnection().CopyDataToAsync("Person",
                        "Customer",
                        GetConnection(),
                        tableExistenceBehavior: behavior));
            }
        }

        /// <summary>
        /// Verifies throw exception on copy data to async if the buffer size is not greater than zero.
        /// </summary>
        [TestMethod]
        public async Task ThrowExceptionOnCopyDataToAsyncIfTheBufferSizeIsNotGreaterThanZero()
        {
            // Act/Assert
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => GetConnection().CopyDataToAsync("Person",
                    "Person",
                    GetConnection(),
                    bufferSize: 0));
        }

        #endregion

        #region Initialize

        /// <summary>
        /// Performs initialize.
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            DbSettingMapper.Add<FakeBulkDbConnection>(new FakeDbSetting(),
                true);
            FakeBulkOperations.Reset();
        }

        /// <summary>
        /// Performs cleanup.
        /// </summary>
        [TestCleanup]
        public void Cleanup()
        {
            SchemaReaderMapper.Clear();
            SchemaComposerMapper.Clear();
        }

        #endregion

        #region Classes

        /// <summary>
        /// Contains the person behavior.
        /// </summary>
        public class Person
        {
            /// <summary>
            /// Gets or sets the id.
            /// </summary>
            public int Id { get; set; }
            /// <summary>
            /// Gets or sets the name.
            /// </summary>
            public string Name { get; set; }
        }

        #endregion

        #region Success Helpers

        /// <summary>
        /// Performs call.
        /// </summary>
        private static FakeBulkCall Call() => FakeBulkOperations.Calls.Single();

        /// <summary>
        /// Performs filter.
        /// </summary>
        private static QueryGroup Filter() => new QueryGroup(new QueryField("Id",
                Operation.GreaterThan,
                1));
        #endregion

        #region CopyDataTo (Destination Connection, Sync)

        /// <summary>
        /// Verifies copy data to of an entity to adestination connection copies the table of the entity.
        /// </summary>
        [TestMethod]
        public void TestCopyDataToOfAnEntityToADestinationConnectionCopiesTheTableOfTheEntity()
        {
            // Setup
            var destination = new FakeBulkDbConnection();
            // Act
            var actual = new FakeDbConnection().CopyDataTo<Person>(destination).RowCount;
            // Assert
            Assert.AreEqual(3,
                actual);
            Assert.AreEqual("Person",
                Call().TableName);
        }

        /// <summary>
        /// Verifies copy data to of an entity to adestination connection filters the rows with the expression.
        /// </summary>
        [TestMethod]
        public void TestCopyDataToOfAnEntityToADestinationConnectionFiltersTheRowsWithTheExpression()
        {
            // Setup
            var destination = new FakeBulkDbConnection();
            var source = new FakeDbConnection();
            // Act
            new FakeDbConnection().CopyDataTo<Person>(destination,
                p => p.Id > 1);
            source.CopyDataTo<Person>(destination,
                p => p.Id > 1);
            // Assert
            Assert.AreEqual(2,
                FakeBulkOperations.Calls.Length);
            StringAssert.Contains(source.Executions.Single(e => e.Kind == "Reader").CommandText,
                "WHERE");
            Assert.AreEqual(1,
                source.Executions.Single(e => e.Kind == "Reader").Parameters.Values.Single());
        }

        /// <summary>
        /// Verifies copy data to of an entity with atarget table to adestination connection copies into the target table.
        /// </summary>
        [TestMethod]
        public void TestCopyDataToOfAnEntityWithATargetTableToADestinationConnectionCopiesIntoTheTargetTable()
        {
            // Setup
            var destination = new FakeBulkDbConnection();
            // Act
            var actual = new FakeDbConnection().CopyDataTo<Person>("Customer",
                destination).RowCount;
            // Assert
            Assert.AreEqual(3,
                actual);
            Assert.AreEqual("Customer",
                Call().TableName);
        }

        /// <summary>
        /// Verifies copy data to of atable to adestination connection copies into the same table.
        /// </summary>
        [TestMethod]
        public void TestCopyDataToOfATableToADestinationConnectionCopiesIntoTheSameTable()
        {
            // Setup
            var destination = new FakeBulkDbConnection();
            // Act
            var actual = new FakeDbConnection().CopyDataTo("Person",
                destination).RowCount;
            // Assert
            Assert.AreEqual(3,
                actual);
            Assert.AreEqual("Person",
                Call().TableName);
        }

        /// <summary>
        /// Verifies copy data to of atable to adestination connection filters the rows with the query group.
        /// </summary>
        [TestMethod]
        public void TestCopyDataToOfATableToADestinationConnectionFiltersTheRowsWithTheQueryGroup()
        {
            // Setup
            var destination = new FakeBulkDbConnection();
            var source = new FakeDbConnection();
            // Act
            source.CopyDataTo("Person",
                destination,
                Filter());
            // Assert
            StringAssert.Contains(source.Executions.Single(e => e.Kind == "Reader").CommandText,
                "WHERE");
        }

        /// <summary>
        /// Verifies copy data to of asource table to adestination connection copies into the target table.
        /// </summary>
        [TestMethod]
        public void TestCopyDataToOfASourceTableToADestinationConnectionCopiesIntoTheTargetTable()
        {
            // Setup
            var destination = new FakeBulkDbConnection();
            var source = new FakeDbConnection();
            // Act
            var actual = source.CopyDataTo("Person",
                "Customer",
                destination).RowCount;
            // Assert
            Assert.AreEqual(3,
                actual);
            Assert.AreEqual("Customer",
                Call().TableName);
            StringAssert.Contains(source.Executions.Single(e => e.Kind == "Reader").CommandText,
                "[Person]");
        }

        #endregion

        #region CopyDataTo (IDbConnection, Sync)

        /// <summary>
        /// Verifies copy data to of an entity copies the table of the entity.
        /// </summary>
        [TestMethod]
        public void TestCopyDataToOfAnEntityCopiesTheTableOfTheEntity()
        {
            // Act
            var actual = new FakeDbConnection().CopyDataTo<Person>(new FakeBulkDbConnection()).RowCount;
            // Assert
            Assert.AreEqual(3,
                actual);
            Assert.AreEqual("Person",
                Call().TableName);
        }

        /// <summary>
        /// Verifies copy data to of an entity filters the rows with the expression.
        /// </summary>
        [TestMethod]
        public void TestCopyDataToOfAnEntityFiltersTheRowsWithTheExpression()
        {
            // Setup
            var source = new FakeDbConnection();
            // Act
            source.CopyDataTo<Person>(new FakeBulkDbConnection(),
                p => p.Id > 1);
            // Assert
            StringAssert.Contains(source.Executions.Single(e => e.Kind == "Reader").CommandText,
                "WHERE");
        }

        /// <summary>
        /// Verifies copy data to of an entity with atarget table copies into the target table.
        /// </summary>
        [TestMethod]
        public void TestCopyDataToOfAnEntityWithATargetTableCopiesIntoTheTargetTable()
        {
            // Act
            var actual = new FakeDbConnection().CopyDataTo<Person>("Customer",
                new FakeBulkDbConnection()).RowCount;
            // Assert
            Assert.AreEqual(3,
                actual);
            Assert.AreEqual("Customer",
                Call().TableName);
        }

        /// <summary>
        /// Verifies copy data to of an entity with atarget table filters the rows with the expression.
        /// </summary>
        [TestMethod]
        public void TestCopyDataToOfAnEntityWithATargetTableFiltersTheRowsWithTheExpression()
        {
            // Setup
            var source = new FakeDbConnection();
            // Act
            source.CopyDataTo<Person>("Customer",
                new FakeBulkDbConnection(),
                p => p.Id > 1);
            // Assert
            StringAssert.Contains(source.Executions.Single(e => e.Kind == "Reader").CommandText,
                "WHERE");
        }

        /// <summary>
        /// Verifies copy data to of atable copies into the same table.
        /// </summary>
        [TestMethod]
        public void TestCopyDataToOfATableCopiesIntoTheSameTable()
        {
            // Act
            var actual = new FakeDbConnection().CopyDataTo("Person",
                new FakeBulkDbConnection()).RowCount;
            // Assert
            Assert.AreEqual(3,
                actual);
            Assert.AreEqual("Person",
                Call().TableName);
        }

        /// <summary>
        /// Verifies copy data to with the buffer size the timeout and the transaction gives them to the bulk insert.
        /// </summary>
        [TestMethod]
        public void TestCopyDataToWithTheBufferSizeTheTimeoutAndTheTransactionGivesThemToTheBulkInsert()
        {
            // Setup
            var transaction = new Mock<IDbTransaction>().Object;
            var source = new FakeDbConnection();
            // Act
            source.CopyDataTo("Person",
                "Customer",
                new FakeBulkDbConnection(),
                bufferSize: 250,
                commandTimeout: 12,
                transaction: transaction);
            // Assert
            var call = Call();
            Assert.AreEqual(250,
                call.BatchSize);
            Assert.AreEqual(12,
                call.Timeout);
            Assert.AreSame(transaction,
                call.Transaction);
            Assert.AreEqual(12,
                source.Executions.Single(e => e.Kind == "Reader").CommandTimeout);
        }

        /// <summary>
        /// Verifies copy data to calls the progress callback with the progress of the copy.
        /// </summary>
        [TestMethod]
        public void TestCopyDataToCallsTheProgressCallbackWithTheProgressOfTheCopy()
        {
            // Setup
            var progress = new List<(int Batch, int Rows, int Total, DateTime Start, DateTime End)>();
            // Act
            var actual = new FakeDbConnection().CopyDataTo("Person",
                "Customer",
                new FakeBulkDbConnection(),
                progressCallback: p =>
            {
                if (p.CurrentBatch != null)
                    progress.Add((p.BatchCount, p.CurrentBatch.RowCount, p.RowCount, p.StartTime, p.EndTime));
            }).RowCount;
            // Assert
            Assert.AreEqual(3,
                actual);
            Assert.AreEqual(1,
                progress.Count);
            Assert.AreEqual((1, 3, 3),
                (progress[0].Batch, progress[0].Rows, progress[0].Total));
            Assert.AreNotEqual(default(DateTime),
                progress[0].Start);
            Assert.IsTrue(progress[0].End >= progress[0].Start);
        }

        /// <summary>
        /// Verifies copy data to with bypass existing rows bulk merges the rows.
        /// </summary>
        [TestMethod]
        public void TestCopyDataToWithBypassExistingRowsBulkMergesTheRows()
        {
            // Setup
            var source = new FakeDbConnection();
            FakeBulkOperations.Reset();
            // Act
            var actual = source.CopyDataTo("Person",
                "Customer",
                FakeBulkDestination.Create(),
                mergeExistingRows: true).RowCount;
            // Assert
            var call = FakeBulkOperations.Calls.Single();
            Assert.AreEqual(3,
                actual);
            Assert.AreEqual("BulkMerge",
                call.Method);
            CollectionAssert.AreEqual(new[] { "Id" },
                call.Qualifiers);
        }

        /// <summary>
        /// Verifies copy data to async with bypass existing rows bulk merges the rows.
        /// </summary>
        [TestMethod]
        public async Task TestCopyDataToAsyncWithBypassExistingRowsBulkMergesTheRows()
        {
            // Setup
            var source = new FakeDbConnection();
            FakeBulkOperations.Reset();
            // Act
            var actual = (await source.CopyDataToAsync("Person",
                "Customer",
                FakeBulkDestination.Create(),
                mergeExistingRows: true)).RowCount;
            // Assert
            var call = FakeBulkOperations.Calls.Single();
            Assert.AreEqual(3,
                actual);
            Assert.AreEqual("BulkMergeAsync",
                call.Method);
            CollectionAssert.AreEqual(new[] { "Id" },
                call.Qualifiers);
        }

        /// <summary>
        /// Verifies copy data to with bypass existing rows to adestination connection bulk merges the rows.
        /// </summary>
        [TestMethod]
        public void TestCopyDataToWithBypassExistingRowsToADestinationConnectionBulkMergesTheRows()
        {
            // Setup
            var destination = FakeBulkDestination.Create();
            FakeBulkOperations.Reset();
            // Act
            new FakeDbConnection().CopyDataTo("Person",
                destination,
                mergeExistingRows: true);
            // Assert
            Assert.AreEqual("BulkMerge",
                FakeBulkOperations.Calls.Single().Method);
        }

        /// <summary>
        /// Verifies copy data to with bypass existing rows and mappings bulk merges the rows.
        /// </summary>
        [TestMethod]
        public void TestCopyDataToWithBypassExistingRowsAndMappingsBulkMergesTheRows()
        {
            // Setup
            var source = new FakeDbConnection();
            FakeBulkOperations.Reset();
            // Act
            source.CopyDataTo("Person",
                "Customer",
                FakeBulkDestination.Create(),
                new[] { new ColumnMapping("Id",
                    "Id"), new ColumnMapping("Name",
                    "Name") },
                mergeExistingRows: true);
            // Assert
            Assert.AreEqual("BulkMerge",
                FakeBulkOperations.Calls.Single().Method);
            StringAssert.Contains(source.Executions.Single(e => e.Kind == "Reader").CommandText,
                "[Name] AS [Name]");
        }

        /// <summary>
        /// Verifies throw exception on copy data to with bypass existing rows if the target table has no primary key and no identity.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnCopyDataToWithBypassExistingRowsIfTheTargetTableHasNoPrimaryKeyAndNoIdentity()
        {
            // Act/Assert
            Assert.Throws<InvalidOperationException>(() => new FakeDbConnection().CopyDataTo("Person",
                    "Customer",
                    FakeBulkDestination.CreateKeyless(),
                    mergeExistingRows: true));
        }

        /// <summary>
        /// Verifies copy data to with qualifiers bulk merges the rows with the qualifiers.
        /// </summary>
        [TestMethod]
        public void TestCopyDataToWithQualifiersBulkMergesTheRowsWithTheQualifiers()
        {
            // Setup
            FakeBulkOperations.Reset();
            // Act
            new FakeDbConnection().CopyDataTo("Person",
                "Customer",
                FakeBulkDestination.Create(),
                mergeExistingRows: true,
                qualifiers: new[] { "Name" });
            // Assert
            var call = FakeBulkOperations.Calls.Single();
            Assert.AreEqual("BulkMerge",
                call.Method);
            CollectionAssert.AreEqual(new[] { "Name" },
                call.Qualifiers);
        }

        /// <summary>
        /// Verifies copy data to async with qualifiers bulk merges the rows with the qualifiers.
        /// </summary>
        [TestMethod]
        public async Task TestCopyDataToAsyncWithQualifiersBulkMergesTheRowsWithTheQualifiers()
        {
            // Setup
            FakeBulkOperations.Reset();
            // Act
            await new FakeDbConnection().CopyDataToAsync("Person",
                "Customer",
                FakeBulkDestination.Create(),
                mergeExistingRows: true,
                qualifiers: new[] { "Id", "Name" });
            // Assert
            var call = FakeBulkOperations.Calls.Single();
            Assert.AreEqual("BulkMergeAsync",
                call.Method);
            CollectionAssert.AreEqual(new[] { "Id", "Name" },
                call.Qualifiers);
        }

        /// <summary>
        /// Verifies copy data to with qualifiers to adestination connection bulk merges the rows with the qualifiers.
        /// </summary>
        [TestMethod]
        public void TestCopyDataToWithQualifiersToADestinationConnectionBulkMergesTheRowsWithTheQualifiers()
        {
            // Setup
            var destination = FakeBulkDestination.CreateKeyless();
            FakeBulkOperations.Reset();
            // Act
            new FakeDbConnection().CopyDataTo("Person",
                destination,
                mergeExistingRows: true,
                qualifiers: new[] { "Id" });
            // Assert
            CollectionAssert.AreEqual(new[] { "Id" },
                FakeBulkOperations.Calls.Single().Qualifiers);
        }

        /// <summary>
        /// Verifies copy data to with mappings and qualifiers bulk merges the rows with the qualifiers.
        /// </summary>
        [TestMethod]
        public void TestCopyDataToWithMappingsAndQualifiersBulkMergesTheRowsWithTheQualifiers()
        {
            // Setup
            FakeBulkOperations.Reset();
            var mappings = new[]
            {
                new ColumnMapping("Id",
                    "Id"),
                new ColumnMapping("Name",
                    "Name")
            };
            // Act
            new FakeDbConnection().CopyDataTo("Person",
                "Customer",
                FakeBulkDestination.Create(),
                mappings,
                mergeExistingRows: true,
                qualifiers: new[] { "Name" });
            // Assert
            CollectionAssert.AreEqual(new[] { "Name" },
                FakeBulkOperations.Calls.Single().Qualifiers);
        }

        /// <summary>
        /// Verifies copy data to ignores the qualifiers if the existing rows are not bypassed.
        /// </summary>
        [TestMethod]
        public void TestCopyDataToIgnoresTheQualifiersIfTheExistingRowsAreNotBypassed()
        {
            // Setup
            FakeBulkOperations.Reset();
            // Act
            new FakeDbConnection().CopyDataTo("Person",
                "Customer",
                FakeBulkDestination.Create(),
                qualifiers: new[] { "Name" });
            // Assert
            Assert.AreEqual("BulkInsert",
                FakeBulkOperations.Calls.Single().Method);
        }

        /// <summary>
        /// Verifies copy data to with bypass existing rows merges the rows one by one if the provider has no bulk merge.
        /// </summary>
        [TestMethod]
        public void TestCopyDataToWithBypassExistingRowsMergesTheRowsOneByOneIfTheProviderHasNoBulkMerge()
        {
            // Setup
            var destination = new FakeDbConnection();
            // Act
            var actual = new FakeDbConnection().CopyDataTo("Person",
                "Customer",
                destination,
                mergeExistingRows: true,
                qualifiers: new[] { "Name" }).RowCount;
            // Assert
            var updates = destination.Executions.Where(e => e.CommandText.StartsWith("UPDATE",
                    StringComparison.Ordinal)).ToList();
            Assert.AreEqual(3,
                actual);
            Assert.AreEqual(3,
                updates.Count);
            StringAssert.Contains(updates[0].CommandText,
                "WHERE [Name] = ");
        }

        /// <summary>
        /// Verifies copy data to async with bypass existing rows merges the rows one by one if the provider has no bulk merge.
        /// </summary>
        [TestMethod]
        public async Task TestCopyDataToAsyncWithBypassExistingRowsMergesTheRowsOneByOneIfTheProviderHasNoBulkMerge()
        {
            // Setup
            var destination = new FakeDbConnection();
            // Act
            await new FakeDbConnection().CopyDataToAsync("Person",
                "Customer",
                destination,
                mergeExistingRows: true);
            // Assert
            var update = destination.Executions.First(e => e.CommandText.StartsWith("UPDATE",
                    StringComparison.Ordinal));
            StringAssert.Contains(update.CommandText,
                "WHERE [Id] = ");
        }

        /// <summary>
        /// Verifies throw exception on copy data to with qualifiers if aqualifier is not acolumn of the target table.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnCopyDataToWithQualifiersIfAQualifierIsNotAColumnOfTheTargetTable()
        {
            // Act/Assert
            Assert.Throws<ArgumentException>(() => new FakeDbConnection().CopyDataTo("Person",
                    "Customer",
                    FakeBulkDestination.Create(),
                    mergeExistingRows: true,
                    qualifiers: new[] { "Missing" }));
        }

        /// <summary>
        /// Verifies copy data to sets the tables and the databases of the progress.
        /// </summary>
        [TestMethod]
        public void TestCopyDataToSetsTheTablesAndTheDatabasesOfTheProgress()
        {
            // Setup
            var source = new FakeDbConnection
            {
                Data = FakeDbConnection.CreatePersons(3)
            };
            var destination = FakeBulkDestination.Create();
            CopyProgress progress = null;
            // Act
            source.CopyDataTo("Person",
                "Customer",
                destination,
                progressCallback: p => progress = p);
            // Assert
            Assert.AreEqual(3,
                progress.CurrentBatch.RowCount);
            Assert.AreEqual("FakeDataSource",
                progress.Current.Source.Server);
            Assert.AreEqual("FakeDatabase",
                progress.Current.Source.Database);
            Assert.AreEqual("Person",
                progress.Current.Source.Table.Name);
            Assert.AreEqual("FakeDataSource",
                progress.Current.Target.Server);
            Assert.AreEqual("FakeDatabase",
                progress.Current.Target.Database);
            Assert.AreEqual("Customer",
                progress.Current.Target.Table.Name);
        }

        /// <summary>
        /// Verifies copy data to sets the times of the batches of the progress.
        /// </summary>
        [TestMethod]
        public void TestCopyDataToSetsTheTimesOfTheBatchesOfTheProgress()
        {
            // Setup
            var source = new FakeDbConnection
            {
                Data = FakeDbConnection.CreatePersons(4)
            };
            var destination = FakeBulkDestination.Create();
            var times = new List<(DateTime Start, DateTime End, DateTime OperationStart, DateTime OperationEnd)>();
            // Act
            source.CopyDataTo("Person",
                "Customer",
                destination,
                bufferSize: 2,
                progressCallback: p =>
            {
                if (p.CurrentBatch != null)
                    times.Add((p.CurrentBatch.StartTime, p.CurrentBatch.EndTime, p.StartTime, p.EndTime));
            });
            // Assert
            Assert.AreEqual(2,
                times.Count);
            Assert.IsTrue(times.All(t => t.Start != default && t.Start <= t.End));
            Assert.IsTrue(times.All(t => t.OperationStart <= t.Start && t.End == t.OperationEnd));
            Assert.IsTrue(times[0].End <= times[1].Start);
        }

        /// <summary>
        /// Verifies copy data to without abulk insert inserts the rows in batches and returns the number of rows.
        /// </summary>
        [TestMethod]
        public void TestCopyDataToWithoutABulkInsertInsertsTheRowsInBatchesAndReturnsTheNumberOfRows()
        {
            // Setup
            var source = new FakeDbConnection
            {
                Data = FakeDbConnection.CreatePersons(5)
            };
            var destination = new FakeDbConnection();
            var batches = new List<int>();
            // Act
            var actual = source.CopyDataTo("Person",
                "Customer",
                destination,
                bufferSize: 2,
                progressCallback: p =>
            {
                if (p.CurrentBatch != null)
                    batches.Add(p.CurrentBatch.RowCount);
            }).RowCount;
            // Assert
            Assert.AreEqual(5,
                actual);
            CollectionAssert.AreEqual(new[] { 2, 2, 1 },
                batches);
        }

        /// <summary>
        /// Verifies copy data to gives the column mappings behavior to the bulk insert.
        /// </summary>
        [TestMethod]
        public void TestCopyDataToGivesTheColumnMappingsBehaviorToTheBulkInsert()
        {
            // Setup
            DbSettingMapper.Add<FakeBulkSettingDbConnection>(new FakeBulkDbSetting(),
                true);
            var destination = new FakeBulkSettingDbConnection();
            // Act
            new FakeDbConnection().CopyDataTo("Person",
                destination,
                columnMappingsBehavior: ColumnMappingBehavior.Strict);
            new FakeDbConnection().CopyDataTo("Person",
                "Customer",
                destination,
                columnMappingsBehavior: ColumnMappingBehavior.StrictBypass);
            new FakeDbConnection().CopyDataTo<Person>(destination,
                columnMappingsBehavior: ColumnMappingBehavior.Automatic);
            // Assert
            CollectionAssert.AreEqual(new[] { "Strict", "StrictBypass", "Automatic" },
                FakeBulkOperations.Calls.Select(c => c.ColumnMappingsBehavior).ToArray());
        }

        /// <summary>
        /// Verifies copy data to with adestination connection gives the column mappings behavior to the bulk insert.
        /// </summary>
        [TestMethod]
        public void TestCopyDataToWithADestinationConnectionGivesTheColumnMappingsBehaviorToTheBulkInsert()
        {
            // Setup
            DbSettingMapper.Add<FakeBulkSettingDbConnection>(new FakeBulkDbSetting(),
                true);
            var destination = new FakeBulkSettingDbConnection();
            // Act
            new FakeDbConnection().CopyDataTo("Person",
                destination,
                columnMappingsBehavior: ColumnMappingBehavior.Strict);
            new FakeDbConnection().CopyDataTo<Person>("Customer",
                destination,
                columnMappingsBehavior: ColumnMappingBehavior.StrictBypass);
            // Assert
            CollectionAssert.AreEqual(new[] { "Strict", "StrictBypass" },
                FakeBulkOperations.Calls.Select(c => c.ColumnMappingsBehavior).ToArray());
        }

        /// <summary>
        /// Verifies copy data to without abulk insert ignores the column mappings behavior.
        /// </summary>
        [TestMethod]
        public void TestCopyDataToWithoutABulkInsertIgnoresTheColumnMappingsBehavior()
        {
            // Setup
            var destination = new FakeDbConnection();
            // Act
            var actual = new FakeDbConnection().CopyDataTo("Person",
                "Customer",
                destination,
                columnMappingsBehavior: ColumnMappingBehavior.Strict).RowCount;
            // Assert
            Assert.AreEqual(3,
                actual);
            Assert.AreEqual(3,
                destination.Executions.Count(e => e.Kind == "NonQuery" && e.CommandText.StartsWith("INSERT",
                        StringComparison.Ordinal)));
        }

        /// <summary>
        /// Verifies throw exception on copy data to if the column mappings behavior is not defined.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnCopyDataToIfTheColumnMappingsBehaviorIsNotDefined()
        {
            // Act/Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => GetConnection().CopyDataTo("Person",
                    "Person",
                    GetConnection(),
                    columnMappingsBehavior: (ColumnMappingBehavior)99));
        }

        /// <summary>
        /// Verifies throw exception on copy data to if the column mappings behavior is not the default and the setting of the bulk insert has no behavior.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnCopyDataToIfTheColumnMappingsBehaviorIsNotTheDefaultAndTheSettingOfTheBulkInsertHasNoBehavior()
        {
            // Act/Assert
            Assert.Throws<InvalidOperationException>(() => new FakeDbConnection().CopyDataTo("Person",
                    new FakeBulkDbConnection(),
                    columnMappingsBehavior: ColumnMappingBehavior.StrictBypass));
        }

        /// <summary>
        /// Verifies copy data to with atarget table of another schema copies the schema and the rows into that schema.
        /// </summary>
        [TestMethod]
        public void TestCopyDataToWithATargetTableOfAnotherSchemaCopiesTheSchemaAndTheRowsIntoThatSchema()
        {
            // Setup
            var composer = new Mock<ISchemaComposer>();
            composer.Setup(c => c.ComposeSchemas(It.IsAny<IEnumerable<TableSchema>>())).Returns<IEnumerable<TableSchema>>(schemas => schemas.Select(s => $"CREATE TABLE {s.Table.Schema}.{s.Table.Name};").ToList());
            SchemaComposerMapper.Add<FakeBulkKeyedDbConnection>(composer.Object,
                true);
            var reader = new Mock<ISchemaReader>();
            var schema = new TableSchema("Person",
                "dbo")
            {
                Columns =
                {
                    new ColumnInfo()
                }
            };
            var relationship = new RelationshipInfo();
            typeof(RelationshipInfo).GetProperty(nameof(RelationshipInfo.Schema)).SetValue(relationship,
                schema);
            reader.Setup(r => r.GetDependencyOrder(It.IsAny<System.Data.IDbConnection>(),
                    It.IsAny<IEnumerable<string>>(),
                    It.IsAny<System.Data.IDbTransaction>())).Returns(new[] { relationship });
            SchemaReaderMapper.Add<FakeDbConnection>(reader.Object,
                true);
            var destination = FakeBulkDestination.Create();
            FakeBulkOperations.Reset();
            // Act
            var actual = new FakeDbConnection().CopyDataTo("Person",
                "Sales.Person",
                destination).RowCount;
            // Assert
            Assert.AreEqual(3,
                actual);
            Assert.AreEqual("CREATE TABLE Sales.Person;",
                FakeBulkOperations.Statements.First().CommandText);
            Assert.AreEqual("[Sales].[Person]",
                FakeBulkOperations.Calls.Last().TableName);
        }

        /// <summary>
        /// Verifies copy data to with arelationship behavior copies the rows of the related tables first.
        /// </summary>
        [TestMethod]
        public void TestCopyDataToWithARelationshipBehaviorCopiesTheRowsOfTheRelatedTablesFirst()
        {
            // Setup
            var composer = new Mock<ISchemaComposer>();
            composer.Setup(c => c.ComposeSchemas(It.IsAny<IEnumerable<TableSchema>>())).Returns<IEnumerable<TableSchema>>(schemas => schemas.Select(s => $"CREATE TABLE {s.Table.Name};").ToList());
            SchemaComposerMapper.Add<FakeBulkKeyedDbConnection>(composer.Object,
                true);
            var relationships = new[]
            {
                "Country",
                "Person"
            }.Select(name =>
            {
                var relationship = new RelationshipInfo();
                typeof(RelationshipInfo).GetProperty(nameof(RelationshipInfo.Schema)).SetValue(relationship,
                        new TableSchema(name,
                            "dbo") { Columns = { new ColumnInfo() } });
                return relationship;
            }).ToList();
            var reader = new Mock<ISchemaReader>();
            reader.Setup(r => r.GetRelatedTables(It.IsAny<System.Data.IDbConnection>(),
                    It.IsAny<IEnumerable<string>>(),
                    It.IsAny<RelatedTableBehavior>(),
                    It.IsAny<System.Data.IDbTransaction>())).Returns(new[] { "Person", "Country" });
            reader.Setup(r => r.GetDependencyOrder(It.IsAny<System.Data.IDbConnection>(),
                    It.IsAny<IEnumerable<string>>(),
                    It.IsAny<System.Data.IDbTransaction>())).Returns(relationships);
            reader.Setup(r => r.GetTableSchema(It.IsAny<System.Data.IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<System.Data.IDbTransaction>())).Returns<System.Data.IDbConnection, string, System.Data.IDbTransaction>((operationConnection,
                    name,
                    operationTransaction) => new TableSchema(name,
                    "dbo") { Columns = { new ColumnInfo() } });
            SchemaReaderMapper.Add<FakeDbConnection>(reader.Object,
                true);
            var destination = FakeBulkDestination.Create();
            FakeBulkOperations.Reset();
            // Act
            var actual = new FakeDbConnection().CopyDataTo("Person",
                "Person",
                destination,
                relationshipBehavior: RelationshipBehavior.Parents).RowCount;
            // Assert
            Assert.AreEqual(6,
                actual);
            CollectionAssert.AreEqual(new[] { "[dbo].[Country]", "[dbo].[Person]" },
                FakeBulkOperations.Calls.Select(c => c.TableName).ToArray());
        }

        #endregion

        #region CopyDataTo (Destination Connection, Async)

        /// <summary>
        /// Verifies copy data to async gives the column mappings behavior to the bulk insert.
        /// </summary>
        [TestMethod]
        public async Task TestCopyDataToAsyncGivesTheColumnMappingsBehaviorToTheBulkInsert()
        {
            // Setup
            DbSettingMapper.Add<FakeBulkSettingDbConnection>(new FakeBulkDbSetting(),
                true);
            var destination = new FakeBulkSettingDbConnection();
            // Act
            await new FakeDbConnection().CopyDataToAsync("Person",
                destination,
                columnMappingsBehavior: ColumnMappingBehavior.Strict);
            await new FakeDbConnection().CopyDataToAsync("Person",
                "Customer",
                destination,
                columnMappingsBehavior: ColumnMappingBehavior.StrictBypass);
            await new FakeDbConnection().CopyDataToAsync<Person>(destination,
                columnMappingsBehavior: ColumnMappingBehavior.Automatic);
            // Assert
            CollectionAssert.AreEqual(new[] { "Strict", "StrictBypass", "Automatic" },
                FakeBulkOperations.Calls.Select(c => c.ColumnMappingsBehavior).ToArray());
        }

        /// <summary>
        /// Verifies copy data to async with adestination connection gives the column mappings behavior to the bulk insert.
        /// </summary>
        [TestMethod]
        public async Task TestCopyDataToAsyncWithADestinationConnectionGivesTheColumnMappingsBehaviorToTheBulkInsert()
        {
            // Setup
            DbSettingMapper.Add<FakeBulkSettingDbConnection>(new FakeBulkDbSetting(),
                true);
            var destination = new FakeBulkSettingDbConnection();
            // Act
            await new FakeDbConnection().CopyDataToAsync("Person",
                destination,
                columnMappingsBehavior: ColumnMappingBehavior.StrictBypass);
            await new FakeDbConnection().CopyDataToAsync<Person>("Customer",
                destination,
                columnMappingsBehavior: ColumnMappingBehavior.Strict);
            // Assert
            CollectionAssert.AreEqual(new[] { "StrictBypass", "Strict" },
                FakeBulkOperations.Calls.Select(c => c.ColumnMappingsBehavior).ToArray());
        }

        /// <summary>
        /// Verifies throw exception on copy data to async if the column mappings behavior is not defined.
        /// </summary>
        [TestMethod]
        public async Task ThrowExceptionOnCopyDataToAsyncIfTheColumnMappingsBehaviorIsNotDefined()
        {
            // Act/Assert
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => GetConnection().CopyDataToAsync("Person",
                    "Person",
                    GetConnection(),
                    columnMappingsBehavior: (ColumnMappingBehavior)99));
        }

        /// <summary>
        /// Verifies copy data to async of an entity to adestination connection copies the table of the entity.
        /// </summary>
        [TestMethod]
        public async Task TestCopyDataToAsyncOfAnEntityToADestinationConnectionCopiesTheTableOfTheEntity()
        {
            // Setup
            var destination = new FakeBulkDbConnection();
            // Act
            var actual = (await new FakeDbConnection().CopyDataToAsync<Person>(destination)).RowCount;
            // Assert
            Assert.AreEqual(3,
                actual);
            Assert.AreEqual("Person",
                Call().TableName);
        }

        /// <summary>
        /// Verifies copy data to async of an entity with atarget table to adestination connection copies into the target table.
        /// </summary>
        [TestMethod]
        public async Task TestCopyDataToAsyncOfAnEntityWithATargetTableToADestinationConnectionCopiesIntoTheTargetTable()
        {
            // Setup
            var destination = new FakeBulkDbConnection();
            // Act
            var actual = (await new FakeDbConnection().CopyDataToAsync<Person>("Customer",
                destination)).RowCount;
            // Assert
            Assert.AreEqual(3,
                actual);
            Assert.AreEqual("Customer",
                Call().TableName);
        }

        /// <summary>
        /// Verifies copy data to async of atable to adestination connection copies into the same table.
        /// </summary>
        [TestMethod]
        public async Task TestCopyDataToAsyncOfATableToADestinationConnectionCopiesIntoTheSameTable()
        {
            // Setup
            var destination = new FakeBulkDbConnection();
            // Act
            var actual = (await new FakeDbConnection().CopyDataToAsync("Person",
                destination)).RowCount;
            // Assert
            Assert.AreEqual(3,
                actual);
            Assert.AreEqual("Person",
                Call().TableName);
        }

        /// <summary>
        /// Verifies copy data to async of asource table to adestination connection copies into the target table.
        /// </summary>
        [TestMethod]
        public async Task TestCopyDataToAsyncOfASourceTableToADestinationConnectionCopiesIntoTheTargetTable()
        {
            // Setup
            var destination = new FakeBulkDbConnection();
            // Act
            var actual = (await new FakeDbConnection().CopyDataToAsync("Person",
                "Customer",
                destination)).RowCount;
            // Assert
            Assert.AreEqual(3,
                actual);
            Assert.AreEqual("Customer",
                Call().TableName);
        }

        #endregion

        #region CopyDataTo (IDbConnection, Async)

        /// <summary>
        /// Verifies copy data to async of an entity copies the table of the entity.
        /// </summary>
        [TestMethod]
        public async Task TestCopyDataToAsyncOfAnEntityCopiesTheTableOfTheEntity()
        {
            // Act
            var actual = (await new FakeDbConnection().CopyDataToAsync<Person>(new FakeBulkDbConnection())).RowCount;
            // Assert
            Assert.AreEqual(3,
                actual);
            Assert.AreEqual("Person",
                Call().TableName);
        }

        /// <summary>
        /// Verifies copy data to async of an entity filters the rows with the expression.
        /// </summary>
        [TestMethod]
        public async Task TestCopyDataToAsyncOfAnEntityFiltersTheRowsWithTheExpression()
        {
            // Setup
            var source = new FakeDbConnection();
            // Act
            await source.CopyDataToAsync<Person>(new FakeBulkDbConnection(),
                p => p.Id > 1);
            // Assert
            StringAssert.Contains(source.Executions.Single(e => e.Kind == "Reader").CommandText,
                "WHERE");
        }

        /// <summary>
        /// Verifies copy data to async of an entity with atarget table copies into the target table.
        /// </summary>
        [TestMethod]
        public async Task TestCopyDataToAsyncOfAnEntityWithATargetTableCopiesIntoTheTargetTable()
        {
            // Act
            var actual = (await new FakeDbConnection().CopyDataToAsync<Person>("Customer",
                new FakeBulkDbConnection())).RowCount;
            // Assert
            Assert.AreEqual(3,
                actual);
            Assert.AreEqual("Customer",
                Call().TableName);
        }

        /// <summary>
        /// Verifies copy data to async of an entity with atarget table filters the rows with the expression.
        /// </summary>
        [TestMethod]
        public async Task TestCopyDataToAsyncOfAnEntityWithATargetTableFiltersTheRowsWithTheExpression()
        {
            // Setup
            var source = new FakeDbConnection();
            // Act
            await source.CopyDataToAsync<Person>("Customer",
                new FakeBulkDbConnection(),
                p => p.Id > 1);
            // Assert
            StringAssert.Contains(source.Executions.Single(e => e.Kind == "Reader").CommandText,
                "WHERE");
        }

        /// <summary>
        /// Verifies copy data to async of atable copies into the same table.
        /// </summary>
        [TestMethod]
        public async Task TestCopyDataToAsyncOfATableCopiesIntoTheSameTable()
        {
            // Act
            var actual = (await new FakeDbConnection().CopyDataToAsync("Person",
                new FakeBulkDbConnection())).RowCount;
            // Assert
            Assert.AreEqual(3,
                actual);
            Assert.AreEqual("Person",
                Call().TableName);
        }

        /// <summary>
        /// Verifies copy data to async of atable filters the rows with the query group.
        /// </summary>
        [TestMethod]
        public async Task TestCopyDataToAsyncOfATableFiltersTheRowsWithTheQueryGroup()
        {
            // Setup
            var source = new FakeDbConnection();
            // Act
            await source.CopyDataToAsync("Person",
                new FakeBulkDbConnection(),
                Filter());
            // Assert
            StringAssert.Contains(source.Executions.Single(e => e.Kind == "Reader").CommandText,
                "WHERE");
        }

        /// <summary>
        /// Verifies copy data to async with the buffer size the timeout the transaction and the cancellation token gives them to the bulk insert.
        /// </summary>
        [TestMethod]
        public async Task TestCopyDataToAsyncWithTheBufferSizeTheTimeoutTheTransactionAndTheCancellationTokenGivesThemToTheBulkInsert()
        {
            // Setup
            var transaction = new Mock<IDbTransaction>().Object;
            using (var tokenSource = new CancellationTokenSource())
            {
                // Act
                await new FakeDbConnection().CopyDataToAsync("Person",
                    "Customer",
                    new FakeBulkDbConnection(),
                    bufferSize: 250,
                    commandTimeout: 12,
                    transaction: transaction,
                    cancellationToken: tokenSource.Token);
                // Assert
                var call = Call();
                Assert.AreEqual("BulkInsertAsync",
                    call.Method);
                Assert.AreEqual(250,
                    call.BatchSize);
                Assert.AreEqual(12,
                    call.Timeout);
                Assert.AreSame(transaction,
                    call.Transaction);
                Assert.AreEqual(tokenSource.Token,
                    call.CancellationToken);
            }
        }

        /// <summary>
        /// Verifies copy data to async calls the progress callback with the progress of the copy.
        /// </summary>
        [TestMethod]
        public async Task TestCopyDataToAsyncCallsTheProgressCallbackWithTheProgressOfTheCopy()
        {
            // Setup
            var progress = new List<(int Batch, int Rows, int Total)>();
            // Act
            var actual = (await new FakeDbConnection().CopyDataToAsync("Person",
                "Customer",
                new FakeBulkDbConnection(),
                progressCallback: p =>
            {
                if (p.CurrentBatch != null)
                    progress.Add((p.BatchCount, p.CurrentBatch.RowCount, p.RowCount));
            })).RowCount;
            // Assert
            Assert.AreEqual(3,
                actual);
            CollectionAssert.AreEqual(new[] { (1, 3, 3) },
                progress);
        }

        /// <summary>
        /// Verifies copy data to async without abulk insert inserts the rows in batches and returns the number of rows.
        /// </summary>
        [TestMethod]
        public async Task TestCopyDataToAsyncWithoutABulkInsertInsertsTheRowsInBatchesAndReturnsTheNumberOfRows()
        {
            // Setup
            var source = new FakeDbConnection
            {
                Data = FakeDbConnection.CreatePersons(5)
            };
            var batches = new List<int>();
            // Act
            var actual = (await source.CopyDataToAsync("Person",
                "Customer",
                new FakeDbConnection(),
                bufferSize: 2,
                progressCallback: p =>
            {
                if (p.CurrentBatch != null)
                    batches.Add(p.CurrentBatch.RowCount);
            })).RowCount;
            // Assert
            Assert.AreEqual(5,
                actual);
            CollectionAssert.AreEqual(new[] { 2, 2, 1 },
                batches);
        }

        /// <summary>
        /// Verifies copy data to async with arelationship behavior copies the rows of the related tables first.
        /// </summary>
        [TestMethod]
        public async Task TestCopyDataToAsyncWithARelationshipBehaviorCopiesTheRowsOfTheRelatedTablesFirst()
        {
            // Setup
            var composer = new Mock<ISchemaComposer>();
            composer.Setup(c => c.ComposeSchemas(It.IsAny<IEnumerable<TableSchema>>())).Returns<IEnumerable<TableSchema>>(schemas => schemas.Select(s => $"CREATE TABLE {s.Table.Name};").ToList());
            SchemaComposerMapper.Add<FakeBulkKeyedDbConnection>(composer.Object,
                true);
            var relationships = new[]
            {
                "Country",
                "Person"
            }.Select(name =>
            {
                var relationship = new RelationshipInfo();
                typeof(RelationshipInfo).GetProperty(nameof(RelationshipInfo.Schema)).SetValue(relationship,
                        new TableSchema(name,
                            "dbo") { Columns = { new ColumnInfo() } });
                return relationship;
            }).ToList();
            var reader = new Mock<ISchemaReader>();
            reader.Setup(r => r.GetRelatedTablesAsync(It.IsAny<System.Data.IDbConnection>(),
                    It.IsAny<IEnumerable<string>>(),
                    It.IsAny<RelatedTableBehavior>(),
                    It.IsAny<System.Data.IDbTransaction>(),
                    It.IsAny<CancellationToken>())).ReturnsAsync(new[] { "Person", "Country" });
            reader.Setup(r => r.GetDependencyOrder(It.IsAny<System.Data.IDbConnection>(),
                    It.IsAny<IEnumerable<string>>(),
                    It.IsAny<System.Data.IDbTransaction>())).Returns(relationships);
            reader.Setup(r => r.GetDependencyOrderAsync(It.IsAny<System.Data.IDbConnection>(),
                    It.IsAny<IEnumerable<string>>(),
                    It.IsAny<System.Data.IDbTransaction>(),
                    It.IsAny<CancellationToken>())).ReturnsAsync(relationships);
            reader.Setup(r => r.GetTableSchemaAsync(It.IsAny<System.Data.IDbConnection>(),
                    It.IsAny<string>(),
                    It.IsAny<System.Data.IDbTransaction>(),
                    It.IsAny<CancellationToken>())).Returns<System.Data.IDbConnection, string, System.Data.IDbTransaction, CancellationToken>((operationConnection,
                    name,
                    operationTransaction,
                    _) => Task.FromResult(new TableSchema(name,
                        "dbo") { Columns = { new ColumnInfo() } }));
            SchemaReaderMapper.Add<FakeDbConnection>(reader.Object,
                true);
            var destination = FakeBulkDestination.Create();
            FakeBulkOperations.Reset();
            // Act
            var actual = (await new FakeDbConnection().CopyDataToAsync("Person",
                "Person",
                destination,
                relationshipBehavior: RelationshipBehavior.ParentsAndChildren)).RowCount;
            // Assert
            Assert.AreEqual(6,
                actual);
            CollectionAssert.AreEqual(new[] { "[dbo].[Country]", "[dbo].[Person]" },
                FakeBulkOperations.Calls.Select(c => c.TableName).ToArray());
        }

        /// <summary>
        /// Verifies throw exception on copy data to async if the operation is cancelled.
        /// </summary>
        [TestMethod]
        public async Task ThrowExceptionOnCopyDataToAsyncIfTheOperationIsCancelled()
        {
            // Setup
            using (var tokenSource = new CancellationTokenSource())
            {
                tokenSource.Cancel();
                // Act/Assert
                await Assert.ThrowsAsync<OperationCanceledException>(() => new FakeDbConnection().CopyDataToAsync("Person",
                        "Person",
                        new FakeDbConnection(),
                        cancellationToken: tokenSource.Token));
            }
        }

        #endregion

        #region Mappings

        /// <summary>
        /// Verifies copy data to with mappings selects only the mapped columns aliased with the destination columns.
        /// </summary>
        [TestMethod]
        public void TestCopyDataToWithMappingsSelectsOnlyTheMappedColumnsAliasedWithTheDestinationColumns()
        {
            // Setup
            var source = new FakeDbConnection();
            var mappings = new[]
            {
                new ColumnMapping("Id",
                    "Id"),
                new ColumnMapping("Name",
                    "Birthday")
            };
            // Act
            source.CopyDataTo("Person",
                "Customer",
                FakeBulkDestination.Create(),
                mappings: mappings);
            // Assert
            var commandText = source.Executions.Single(e => e.Kind == "Reader").CommandText;
            StringAssert.Contains(commandText,
                "[Id] AS [Id], [Name] AS [Birthday]");
            Assert.IsFalse(commandText.Contains("[Birthday],"));
        }

        /// <summary>
        /// Verifies copy data to async with mappings selects only the mapped columns aliased with the destination columns.
        /// </summary>
        [TestMethod]
        public async Task TestCopyDataToAsyncWithMappingsSelectsOnlyTheMappedColumnsAliasedWithTheDestinationColumns()
        {
            // Setup
            var source = new FakeDbConnection();
            var mappings = new[]
            {
                new ColumnMapping("Name",
                    "Birthday")
            };
            // Act
            await source.CopyDataToAsync("Person",
                "Customer",
                FakeBulkDestination.Create(),
                mappings: mappings);
            // Assert
            StringAssert.Contains(source.Executions.Single(e => e.Kind == "Reader").CommandText,
                "[Name] AS [Birthday]");
        }

        /// <summary>
        /// Verifies copy data to with mappings selects all the columns if the mappings are null or empty.
        /// </summary>
        [TestMethod]
        public void TestCopyDataToWithMappingsSelectsAllTheColumnsIfTheMappingsAreNullOrEmpty()
        {
            // Setup
            var source = new FakeDbConnection();
            // Act
            source.CopyDataTo("Person",
                "Customer",
                FakeBulkDestination.Create(),
                mappings: null);
            source.CopyDataTo("Person",
                "Customer",
                FakeBulkDestination.Create(),
                mappings: new List<ColumnMapping>());
            // Assert
            Assert.IsTrue(source.Executions.Where(e => e.Kind == "Reader").All(e => !e.CommandText.Contains(" AS ")));
            Assert.IsTrue(source.Executions.Where(e => e.Kind == "Reader").All(e => e.CommandText.Contains("[Id]") && e.CommandText.Contains("[Name]") && e.CommandText.Contains("[Birthday]")));
        }

        /// <summary>
        /// Verifies throw exception on copy data to if the mappings have anull item.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnCopyDataToIfTheMappingsHaveANullItem()
        {
            // Act/Assert
            Assert.Throws<ArgumentException>(() => new FakeDbConnection().CopyDataTo("Person",
                    "Customer",
                    new FakeDbConnection(),
                    mappings: new ColumnMapping[] { null }));
        }

        /// <summary>
        /// Verifies throw exception on copy data to if amapping has an empty column.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnCopyDataToIfAMappingHasAnEmptyColumn()
        {
            // Act/Assert
            Assert.Throws<ArgumentException>(() => new FakeDbConnection().CopyDataTo("Person",
                    "Customer",
                    new FakeDbConnection(),
                    mappings: new[] { new ColumnMapping(" ",
                        "Id") }));
            Assert.Throws<ArgumentException>(() => new FakeDbConnection().CopyDataTo("Person",
                    "Customer",
                    new FakeDbConnection(),
                    mappings: new[] { new ColumnMapping("Id",
                        null) }));
        }

        /// <summary>
        /// Verifies throw exception on copy data to if adestination column is mapped more than once.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnCopyDataToIfADestinationColumnIsMappedMoreThanOnce()
        {
            // Act/Assert
            Assert.Throws<ArgumentException>(() => new FakeDbConnection().CopyDataTo("Person",
                    "Customer",
                    new FakeDbConnection(),
                    mappings: new[] { new ColumnMapping("Id",
                        "Name"), new ColumnMapping("Name",
                        "name") }));
        }

        /// <summary>
        /// Verifies throw exception on copy data to if asource column does not exist on the source table.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnCopyDataToIfASourceColumnDoesNotExistOnTheSourceTable()
        {
            // Act/Assert
            var ex = Assert.Throws<ArgumentException>(() => new FakeDbConnection().CopyDataTo("Person",
                    "Customer",
                    new FakeDbConnection(),
                    mappings: new[] { new ColumnMapping("Missing",
                        "Id") }));
            StringAssert.Contains(ex.Message,
                "Missing");
        }

        /// <summary>
        /// Verifies throw exception on copy data to if adestination column does not exist on the target table.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnCopyDataToIfADestinationColumnDoesNotExistOnTheTargetTable()
        {
            // Act/Assert
            var ex = Assert.Throws<ArgumentException>(() => new FakeDbConnection().CopyDataTo("Person",
                    "Customer",
                    new FakeDbConnection(),
                    mappings: new[] { new ColumnMapping("Id",
                        "Missing") }));
            StringAssert.Contains(ex.Message,
                "Missing");
        }

        /// <summary>
        /// Verifies throw exception on copy data to async if asource column does not exist on the source table.
        /// </summary>
        [TestMethod]
        public async Task ThrowExceptionOnCopyDataToAsyncIfASourceColumnDoesNotExistOnTheSourceTable()
        {
            // Act/Assert
            await Assert.ThrowsAsync<ArgumentException>(() => new FakeDbConnection().CopyDataToAsync("Person",
                    "Customer",
                    new FakeDbConnection(),
                    mappings: new[] { new ColumnMapping("Missing",
                        "Id") }));
        }

        /// <summary>
        /// Verifies copy data to of an entity with mappings to adestination connection selects the mapped columns.
        /// </summary>
        [TestMethod]
        public void TestCopyDataToOfAnEntityWithMappingsToADestinationConnectionSelectsTheMappedColumns()
        {
            // Setup
            var source = new FakeDbConnection();
            var destination = FakeBulkDestination.Create();
            // Act
            var actual = source.CopyDataTo<Person>("Customer",
                destination,
                new[] { new ColumnMapping("Name",
                    "Birthday") }).RowCount;
            // Assert
            Assert.AreEqual(3,
                actual);
            StringAssert.Contains(source.Executions.Single(e => e.Kind == "Reader").CommandText,
                "[Name] AS [Birthday]");
        }

        /// <summary>
        /// Verifies copy data to of an entity with mappings selects the mapped columns.
        /// </summary>
        [TestMethod]
        public void TestCopyDataToOfAnEntityWithMappingsSelectsTheMappedColumns()
        {
            // Setup
            var source = new FakeDbConnection();
            // Act
            var actual = source.CopyDataTo<Person>(FakeBulkDestination.Create(),
                new[] { new ColumnMapping("Name",
                    "Birthday") }).RowCount;
            // Assert
            Assert.AreEqual(3,
                actual);
            StringAssert.Contains(source.Executions.Single(e => e.Kind == "Reader").CommandText,
                "[Name] AS [Birthday]");
        }

        /// <summary>
        /// Verifies copy data to of atable with mappings to adestination connection selects the mapped columns.
        /// </summary>
        [TestMethod]
        public void TestCopyDataToOfATableWithMappingsToADestinationConnectionSelectsTheMappedColumns()
        {
            // Setup
            var source = new FakeDbConnection();
            var destination = FakeBulkDestination.Create();
            // Act
            source.CopyDataTo("Person",
                destination,
                new[] { new ColumnMapping("Name",
                    "Birthday") });
            // Assert
            StringAssert.Contains(source.Executions.Single(e => e.Kind == "Reader").CommandText,
                "[Name] AS [Birthday]");
        }

        /// <summary>
        /// Verifies copy data to with mappings and afilter selects the mapped columns of the matching rows.
        /// </summary>
        [TestMethod]
        public void TestCopyDataToWithMappingsAndAFilterSelectsTheMappedColumnsOfTheMatchingRows()
        {
            // Setup
            var source = new FakeDbConnection();
            // Act
            source.CopyDataTo("Person",
                "Customer",
                FakeBulkDestination.Create(),
                new[] { new ColumnMapping("Name",
                    "Birthday") },
                Filter());
            // Assert
            StringAssert.Contains(source.Executions.Single(e => e.Kind == "Reader").CommandText,
                "[Name] AS [Birthday]");
            StringAssert.Contains(source.Executions.Single(e => e.Kind == "Reader").CommandText,
                "WHERE");
        }

        /// <summary>
        /// Verifies copy data to async of an entity with mappings to adestination connection selects the mapped columns.
        /// </summary>
        [TestMethod]
        public async Task TestCopyDataToAsyncOfAnEntityWithMappingsToADestinationConnectionSelectsTheMappedColumns()
        {
            // Setup
            var source = new FakeDbConnection();
            var destination = FakeBulkDestination.Create();
            // Act
            var actual = (await source.CopyDataToAsync<Person>("Customer",
                destination,
                new[] { new ColumnMapping("Name",
                    "Birthday") })).RowCount;
            // Assert
            Assert.AreEqual(3,
                actual);
            StringAssert.Contains(source.Executions.Single(e => e.Kind == "Reader").CommandText,
                "[Name] AS [Birthday]");
        }

        /// <summary>
        /// Verifies copy data to async of an entity with mappings selects the mapped columns.
        /// </summary>
        [TestMethod]
        public async Task TestCopyDataToAsyncOfAnEntityWithMappingsSelectsTheMappedColumns()
        {
            // Setup
            var source = new FakeDbConnection();
            // Act
            var actual = (await source.CopyDataToAsync<Person>(FakeBulkDestination.Create(),
                new[] { new ColumnMapping("Name",
                    "Birthday") })).RowCount;
            // Assert
            Assert.AreEqual(3,
                actual);
            StringAssert.Contains(source.Executions.Single(e => e.Kind == "Reader").CommandText,
                "[Name] AS [Birthday]");
        }

        /// <summary>
        /// Verifies copy data to async of atable with mappings to adestination connection selects the mapped columns.
        /// </summary>
        [TestMethod]
        public async Task TestCopyDataToAsyncOfATableWithMappingsToADestinationConnectionSelectsTheMappedColumns()
        {
            // Setup
            var source = new FakeDbConnection();
            var destination = FakeBulkDestination.Create();
            // Act
            await source.CopyDataToAsync("Person",
                destination,
                new[] { new ColumnMapping("Name",
                    "Birthday") });
            // Assert
            StringAssert.Contains(source.Executions.Single(e => e.Kind == "Reader").CommandText,
                "[Name] AS [Birthday]");
        }

        /// <summary>
        /// Verifies throw exception on copy data to with mappings to adestination connection if asource column does not exist.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnCopyDataToWithMappingsToADestinationConnectionIfASourceColumnDoesNotExist()
        {
            // Setup
            var destination = new FakeDbConnection();
            // Act/Assert
            Assert.Throws<ArgumentException>(() => new FakeDbConnection().CopyDataTo<Person>("Customer",
                    destination,
                    new[] { new ColumnMapping("Missing",
                        "Id") }));
        }

        #endregion

        #region Data Interceptors

        /// <summary>
        /// Performs interceptors.
        /// </summary>
        /// <param name="transforms">The transforms.</param>
        private static IList<IDataInterceptor<Row>> Interceptors(params Func<Row, Row>[] transforms) => transforms.Select(transform => (IDataInterceptor<Row>)new FakeDataInterceptor(transform)).ToList();

        /// <summary>
        /// Verifies copy data to copies the rows that the interceptors give in order.
        /// </summary>
        [TestMethod]
        public void TestCopyDataToCopiesTheRowsThatTheInterceptorsGiveInOrder()
        {
            // Setup
            var destination = new FakeDbConnection();
            var interceptors = Interceptors(row =>
            {
                row["Name"] = ((string)row["Name"]).ToUpperInvariant();
                return row;
            },
                row =>
            {
                row["Name"] = row["Name"] + "!";
                return row;
            });
            // Act
            new FakeDbConnection().CopyDataTo("Person",
                "Customer",
                destination,
                dataInterceptors: interceptors);
            // Assert
            var values = destination.Executions.Where(e => e.CommandText.StartsWith("INSERT INTO",
                    StringComparison.Ordinal)).SelectMany(e => e.Parameters.Values).OfType<string>().ToList();
            CollectionAssert.AreEquivalent(new[] { "NAME 1!", "NAME 2!", "NAME 3!" },
                values);
        }

        /// <summary>
        /// Verifies copy data to async copies the rows that the interceptors give.
        /// </summary>
        [TestMethod]
        public async Task TestCopyDataToAsyncCopiesTheRowsThatTheInterceptorsGive()
        {
            // Setup
            var destination = new FakeDbConnection();
            var interceptors = Interceptors(row =>
            {
                row["Name"] = "Changed";
                return row;
            });
            // Act
            await new FakeDbConnection().CopyDataToAsync("Person",
                "Customer",
                destination,
                dataInterceptors: interceptors);
            // Assert
            var values = destination.Executions.Where(e => e.CommandText.StartsWith("INSERT INTO",
                    StringComparison.Ordinal)).SelectMany(e => e.Parameters.Values).OfType<string>().ToList();
            CollectionAssert.AreEqual(new[] { "Changed", "Changed", "Changed" },
                values);
        }

        /// <summary>
        /// Verifies copy data to bulk inserts the columns that the interceptors give.
        /// </summary>
        [TestMethod]
        public void TestCopyDataToBulkInsertsTheColumnsThatTheInterceptorsGive()
        {
            // Setup
            var interceptors = Interceptors(row => row.Add("Extra",
                    typeof(string),
                    "e"),
                row =>
            {
                row.Remove("Birthday");
                return row;
            });
            FakeBulkOperations.Reset();
            // Act
            new FakeDbConnection().CopyDataTo("Person",
                "Customer",
                FakeBulkDestination.Create(),
                dataInterceptors: interceptors);
            // Assert
            CollectionAssert.AreEqual(new[] { "Id", "Name", "Extra" },
                FakeBulkOperations.LastColumns);
        }

        /// <summary>
        /// Verifies copy data to gives the interceptors the columns of the mappings.
        /// </summary>
        [TestMethod]
        public void TestCopyDataToGivesTheInterceptorsTheColumnsOfTheMappings()
        {
            // Setup
            var names = new List<string>();
            var interceptors = Interceptors(row =>
            {
                names.AddRange(row.Columns.Select(c => c.Name));
                return row;
            });
            var mappings = new[]
            {
                new ColumnMapping("Id",
                    "Id"),
                new ColumnMapping("Name",
                    "Birthday")
            };
            // Act
            new FakeDbConnection().CopyDataTo("Person",
                "Customer",
                FakeBulkDestination.Create(),
                mappings,
                dataInterceptors: interceptors);
            // Assert
            Assert.IsTrue(names.Count > 0);
            Assert.IsTrue(names.All(name => name == "Id" || name == "Name" || name == "Birthday"));
        }

        /// <summary>
        /// Verifies copy data to leaves the identity column out after the interceptors.
        /// </summary>
        [TestMethod]
        public void TestCopyDataToLeavesTheIdentityColumnOutAfterTheInterceptors()
        {
            // Setup
            var interceptors = Interceptors(row =>
            {
                row["Id"] = 500;
                return row;
            });
            FakeBulkOperations.Reset();
            // Act
            new FakeDbConnection().CopyDataTo("Person",
                "Customer",
                FakeBulkDestination.Create(),
                identityBehavior: IdentityBehavior.Ignore,
                dataInterceptors: interceptors);
            // Assert
            CollectionAssert.AreEqual(new[] { "Name", "Birthday" },
                FakeBulkOperations.LastColumns);
        }

        /// <summary>
        /// Verifies copy data to with interceptors and bypass existing rows bulk merges the rows with the qualifiers.
        /// </summary>
        [TestMethod]
        public void TestCopyDataToWithInterceptorsAndBypassExistingRowsBulkMergesTheRowsWithTheQualifiers()
        {
            // Setup
            var interceptors = Interceptors(row =>
            {
                row["Name"] = "Changed";
                return row;
            });
            FakeBulkOperations.Reset();
            // Act
            new FakeDbConnection().CopyDataTo("Person",
                "Customer",
                FakeBulkDestination.Create(),
                mergeExistingRows: true,
                dataInterceptors: interceptors);
            // Assert
            var call = FakeBulkOperations.Calls.Single();
            Assert.AreEqual("BulkMerge",
                call.Method);
            CollectionAssert.AreEqual(new[] { "Id" },
                call.Qualifiers);
        }

        /// <summary>
        /// Verifies throw exception on copy data to with bypass existing rows if an interceptor removes aqualifier.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnCopyDataToWithBypassExistingRowsIfAnInterceptorRemovesAQualifier()
        {
            // Setup
            var interceptors = Interceptors(row =>
            {
                row.Remove("Id");
                return row;
            });
            FakeBulkOperations.Reset();
            // Act/Assert
            Assert.Throws<InvalidOperationException>(() => new FakeDbConnection().CopyDataTo("Person",
                    "Customer",
                    FakeBulkDestination.Create(),
                    mergeExistingRows: true,
                    dataInterceptors: interceptors));
            Assert.AreEqual(0,
                FakeBulkOperations.Calls.Length);
        }

        /// <summary>
        /// Verifies throw exception on copy data to if an interceptor returns null.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnCopyDataToIfAnInterceptorReturnsNull()
        {
            // Setup
            var interceptors = Interceptors(row => null);
            // Act/Assert
            Assert.Throws<InvalidOperationException>(() => new FakeDbConnection().CopyDataTo("Person",
                    "Customer",
                    FakeBulkDestination.Create(),
                    dataInterceptors: interceptors));
        }

        /// <summary>
        /// Verifies throw exception on copy data to if an interceptor is null.
        /// </summary>
        [TestMethod]
        public void ThrowExceptionOnCopyDataToIfAnInterceptorIsNull()
        {
            // Act/Assert
            Assert.Throws<ArgumentException>(() => new FakeDbConnection().CopyDataTo("Person",
                    "Customer",
                    FakeBulkDestination.Create(),
                    dataInterceptors: new List<IDataInterceptor<Row>> { null }));
        }

        /// <summary>
        /// Verifies copy data to without interceptors copies the rows as they are.
        /// </summary>
        [TestMethod]
        public void TestCopyDataToWithoutInterceptorsCopiesTheRowsAsTheyAre()
        {
            // Setup
            var destination = new FakeDbConnection();
            // Act
            new FakeDbConnection().CopyDataTo("Person",
                "Customer",
                destination,
                dataInterceptors: new List<IDataInterceptor<Row>>());
            // Assert
            var values = destination.Executions.Where(e => e.CommandText.StartsWith("INSERT INTO",
                    StringComparison.Ordinal)).SelectMany(e => e.Parameters.Values).OfType<string>().ToList();
            CollectionAssert.AreEquivalent(new[] { "Name 1", "Name 2", "Name 3" },
                values);
        }
        #endregion
    }
}
