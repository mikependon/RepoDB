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
using RepoDb.Data.Models;
using RepoDb.Enumerations;
using RepoDb.Interfaces;
using RepoDb.Schema;
using RepoDb.Schema.Enumerations;
using RepoDb.Schema.Models;
using DataDbConnection = RepoDb.Data.DbConnection;

namespace RepoDb.Data.Core.UnitTests.Extensions
{
    [TestClass]
    public class DbConnectionTest
    {
        #region Initialize

        [TestInitialize]
        public void Initialize()
        {
            Cleanup();
        }

        [TestCleanup]
        public void Cleanup()
        {
            SchemaReaderMapper.Clear();
            SchemaComposerMapper.Clear();
        }

        #endregion

        #region Helpers

        private static IDbConnection GetConnection() =>
            new Mock<IDbConnection>().Object;

        private static TableSchema GetSchema(string tableName,
            string schema = "dbo") =>
            new TableSchema(tableName, schema) { Columns = { new ColumnInfo() } };

        private static RelationshipInfo CreateRelationship(TableSchema schema)
        {
            var relationship = new RelationshipInfo();
            typeof(RelationshipInfo).GetProperty(nameof(RelationshipInfo.Schema)).SetValue(relationship, schema);
            return relationship;
        }

        private static Mock<ISchemaReader> MapReader(string[] related,
            params string[] ordered)
        {
            var relationships = ordered.Select(name => CreateRelationship(GetSchema(name))).ToList();
            var reader = new Mock<ISchemaReader>();
            reader.Setup(r => r.GetRelatedTables(It.IsAny<IEnumerable<string>>(), It.IsAny<CopySchemaRelationshipBehavior>())).Returns(related);
            reader.Setup(r => r.GetRelatedTablesAsync(It.IsAny<IEnumerable<string>>(), It.IsAny<CopySchemaRelationshipBehavior>(), It.IsAny<CancellationToken>())).ReturnsAsync(related);
            reader.Setup(r => r.GetDependencyOrder(It.IsAny<IEnumerable<string>>())).Returns(relationships);
            reader.Setup(r => r.GetDependencyOrderAsync(It.IsAny<IEnumerable<string>>(), It.IsAny<CancellationToken>())).ReturnsAsync(relationships);
            reader.Setup(r => r.GetTableSchema(It.IsAny<string>())).Returns<string>(name => GetSchema(name));
            reader.Setup(r => r.GetTableSchemaAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).Returns<string, CancellationToken>((name, _) => Task.FromResult(GetSchema(name)));
            SchemaReaderMapper.Add<FakeDbConnection>(reader.Object, true);
            return reader;
        }

        private static Mock<ISchemaComposer> MapComposer()
        {
            var composer = new Mock<ISchemaComposer>();
            composer.Setup(c => c.ComposeSchemas(It.IsAny<IEnumerable<TableSchema>>())).Returns<IEnumerable<TableSchema>>(schemas => schemas.Select(s => $"CREATE TABLE {s.Table.Name};").ToList());
            composer.Setup(c => c.ComposeSchema(It.IsAny<TableSchema>())).Returns<TableSchema>(s => new[] { $"CREATE TABLE {s.Table.Name};" });
            SchemaComposerMapper.Add<FakeDbConnection>(composer.Object, true);
            return composer;
        }

        #endregion

        #region IsSchemaRequested

        [TestMethod]
        public void TestIsSchemaRequestedIsFalseWithTheDefaultBehaviorsAndNoTargetSchema()
        {
            // Act/Assert
            Assert.IsFalse(DataDbConnection.IsSchemaRequested(CopyDataRelationshipBehavior.TableOnly, CopySchemaExistsBehavior.Skip, null));
            Assert.IsFalse(DataDbConnection.IsSchemaRequested(CopyDataRelationshipBehavior.TableOnly, CopySchemaExistsBehavior.Skip, string.Empty));
            Assert.IsFalse(DataDbConnection.IsSchemaRequested(CopyDataRelationshipBehavior.TableOnly, CopySchemaExistsBehavior.Skip, "  "));
        }

        [TestMethod]
        public void TestIsSchemaRequestedIsTrueWithARelationshipBehaviorOtherThanTableOnly()
        {
            foreach (var behavior in new[] { CopyDataRelationshipBehavior.Parents, CopyDataRelationshipBehavior.Children, CopyDataRelationshipBehavior.ParentsAndChildren })
            {
                // Act/Assert
                Assert.IsTrue(DataDbConnection.IsSchemaRequested(behavior, CopySchemaExistsBehavior.Skip, null));
            }
        }

        [TestMethod]
        public void TestIsSchemaRequestedIsTrueWithATableExistenceBehaviorOtherThanSkip()
        {
            foreach (var behavior in new[] { CopySchemaExistsBehavior.Align, CopySchemaExistsBehavior.Throw, CopySchemaExistsBehavior.Drop })
            {
                // Act/Assert
                Assert.IsTrue(DataDbConnection.IsSchemaRequested(CopyDataRelationshipBehavior.TableOnly, behavior, null));
            }
        }

        [TestMethod]
        public void TestIsSchemaRequestedIsTrueWithATargetSchema()
        {
            // Act/Assert
            Assert.IsTrue(DataDbConnection.IsSchemaRequested(CopyDataRelationshipBehavior.TableOnly, CopySchemaExistsBehavior.Skip, "Sales"));
        }

        #endregion

        #region ToSchemaBehavior

        [TestMethod]
        public void TestToSchemaBehaviorMapsEveryRelationshipBehavior()
        {
            // Act/Assert
            Assert.AreEqual(CopySchemaRelationshipBehavior.TableOnly, DataDbConnection.ToSchemaBehavior(CopyDataRelationshipBehavior.TableOnly));
            Assert.AreEqual(CopySchemaRelationshipBehavior.Parents, DataDbConnection.ToSchemaBehavior(CopyDataRelationshipBehavior.Parents));
            Assert.AreEqual(CopySchemaRelationshipBehavior.Children, DataDbConnection.ToSchemaBehavior(CopyDataRelationshipBehavior.Children));
            Assert.AreEqual(CopySchemaRelationshipBehavior.ParentsAndChildren, DataDbConnection.ToSchemaBehavior(CopyDataRelationshipBehavior.ParentsAndChildren));
        }

        [TestMethod]
        public void ThrowExceptionOnToSchemaBehaviorIfTheRelationshipBehaviorIsNotDefined()
        {
            // Act/Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => DataDbConnection.ToSchemaBehavior((CopyDataRelationshipBehavior)99));
        }

        #endregion

        #region GetName

        [TestMethod]
        public void TestGetNameQuotesTheSchemaAndTheTable()
        {
            // Act
            var actual = DataDbConnection.GetName(new FakeDbConnection(), "Sales", "Person");

            // Assert
            Assert.AreEqual("[Sales].[Person]", actual);
        }

        [TestMethod]
        public void TestGetNameWithoutASchemaQuotesTheTableOnly()
        {
            // Act/Assert
            Assert.AreEqual("[Person]", DataDbConnection.GetName(new FakeDbConnection(), null, "Person"));
            Assert.AreEqual("[Person]", DataDbConnection.GetName(new FakeDbConnection(), string.Empty, "Person"));
            Assert.AreEqual("[Person]", DataDbConnection.GetName(new FakeDbConnection(), " ", "Person"));
        }

        [TestMethod]
        public void TestGetNameOfACopySchemaResultUsesTheDestinationSchemaAndTheTableName()
        {
            // Setup
            var result = new CopySchemaResult { SourceSchema = "dbo", DestinationSchema = "Sales", TableName = "Person" };

            // Act
            var actual = DataDbConnection.GetName(new FakeDbConnection(), result);

            // Assert
            Assert.AreEqual("[Sales].[Person]", actual);
        }

        #endregion

        #region GetTargetSchema

        [TestMethod]
        public void TestGetTargetSchemaReturnsTheSchemaOfTheTargetTable()
        {
            // Act/Assert
            Assert.AreEqual("Sales", DataDbConnection.GetTargetSchema(new FakeDbConnection(), "Sales.Person"));
            Assert.AreEqual("Sales", DataDbConnection.GetTargetSchema(new FakeDbConnection(), "[Sales].[Person]"));
        }

        [TestMethod]
        public void TestGetTargetSchemaReturnsNullIfTheTargetTableHasNoSchema()
        {
            // Act/Assert
            Assert.IsNull(DataDbConnection.GetTargetSchema(new FakeDbConnection(), "Person"));
            Assert.IsNull(DataDbConnection.GetTargetSchema(new FakeDbConnection(), "[Person]"));
        }

        [TestMethod]
        public void TestGetTargetSchemaReturnsNullIfThereIsNoDestinationConnectionOrNoTargetTable()
        {
            // Act/Assert
            Assert.IsNull(DataDbConnection.GetTargetSchema(null, "Sales.Person"));
            Assert.IsNull(DataDbConnection.GetTargetSchema(new FakeDbConnection(), null));
            Assert.IsNull(DataDbConnection.GetTargetSchema(new FakeDbConnection(), " "));
        }

        #endregion

        #region GetTableName

        [TestMethod]
        public void TestGetTableNameReturnsTheNameWithoutTheSchemaAndTheQuotes()
        {
            // Act/Assert
            Assert.AreEqual("Person", DataDbConnection.GetTableName(new FakeDbConnection(), "Person"));
            Assert.AreEqual("Person", DataDbConnection.GetTableName(new FakeDbConnection(), "[Person]"));
            Assert.AreEqual("Person", DataDbConnection.GetTableName(new FakeDbConnection(), "Sales.Person"));
            Assert.AreEqual("Person", DataDbConnection.GetTableName(new FakeDbConnection(), "[Sales].[Person]"));
        }

        #endregion

        #region ToTables

        [TestMethod]
        public void TestToTablesKeepsTheSourceNameAndTheFilterOfTheRequestedTable()
        {
            // Setup
            var where = new QueryGroup(new QueryField("Id", 1));
            var relationships = new[] { CreateRelationship(GetSchema("Person")) };
            var results = new[] { new CopySchemaResult { TableName = "Person", SourceSchema = "dbo", DestinationSchema = "Sales" } };

            // Act
            var actual = DataDbConnection.ToTables(relationships, new TableInfo("Person", "dbo"), ("Person", "Customer", where), results, new FakeDbConnection(), new FakeDbConnection());

            // Assert
            Assert.AreEqual(1, actual.Count);
            Assert.AreEqual("Person", actual[0].Source);
            Assert.AreEqual("[Sales].[Person]", actual[0].Target);
            Assert.AreSame(where, actual[0].Where);
        }

        [TestMethod]
        public void TestToTablesCopiesTheRelatedTablesAsAWholeAndInTheGivenOrder()
        {
            // Setup
            var where = new QueryGroup(new QueryField("Id", 1));
            var relationships = new[]
            {
                CreateRelationship(GetSchema("Country")),
                CreateRelationship(GetSchema("Person"))
            };
            var results = new[]
            {
                new CopySchemaResult { TableName = "Country", SourceSchema = "dbo", DestinationSchema = "Sales" },
                new CopySchemaResult { TableName = "Person", SourceSchema = "dbo", DestinationSchema = "Sales" }
            };

            // Act
            var actual = DataDbConnection.ToTables(relationships, new TableInfo("Person", "dbo"), ("Person", "Person", where), results, new FakeDbConnection(), new FakeDbConnection());

            // Assert
            Assert.AreEqual(2, actual.Count);
            Assert.AreEqual("[dbo].[Country]", actual[0].Source);
            Assert.AreEqual("[Sales].[Country]", actual[0].Target);
            Assert.IsNull(actual[0].Where);
            Assert.AreEqual("Person", actual[1].Source);
            Assert.AreEqual("[Sales].[Person]", actual[1].Target);
            Assert.AreSame(where, actual[1].Where);
        }

        [TestMethod]
        public void TestToTablesMatchesTheResultOfATableByItsSchemaAndItsName()
        {
            // Setup
            var relationships = new[]
            {
                CreateRelationship(GetSchema("Person", "Sales")),
                CreateRelationship(GetSchema("Person", "Hr"))
            };
            var results = new[]
            {
                new CopySchemaResult { TableName = "Person", SourceSchema = "Sales", DestinationSchema = "Target1" },
                new CopySchemaResult { TableName = "Person", SourceSchema = "Hr", DestinationSchema = "Target2" }
            };

            // Act
            var actual = DataDbConnection.ToTables(relationships, new TableInfo("Person", "Sales"), ("Person", "Person", null), results, new FakeDbConnection(), new FakeDbConnection());

            // Assert
            Assert.AreEqual("[Target1].[Person]", actual[0].Target);
            Assert.AreEqual("[Target2].[Person]", actual[1].Target);
            Assert.AreEqual("[Hr].[Person]", actual[1].Source);
        }

        #endregion

        #region GetTables

        [TestMethod]
        public void TestGetTablesWithoutTheSchemaRequestedReturnsTheRequestedTableOnly()
        {
            // Setup
            var where = new QueryGroup(new QueryField("Id", 1));

            // Act
            var actual = DataDbConnection.GetTables(GetConnection(), GetConnection(), "Person", "Customer", null, where, CopyDataRelationshipBehavior.TableOnly, CopySchemaExistsBehavior.Skip, null, null, null);

            // Assert
            Assert.AreEqual(1, actual.Count);
            Assert.AreEqual("Person", actual[0].Source);
            Assert.AreEqual("Customer", actual[0].Target);
            Assert.AreSame(where, actual[0].Where);
        }

        [TestMethod]
        public async Task TestGetTablesAsyncWithoutTheSchemaRequestedReturnsTheRequestedTableOnly()
        {
            // Setup
            var where = new QueryGroup(new QueryField("Id", 1));

            // Act
            var actual = await DataDbConnection.GetTablesAsync(GetConnection(), GetConnection(), "Person", "Customer", null, where, CopyDataRelationshipBehavior.TableOnly, CopySchemaExistsBehavior.Skip, null, null, null, CancellationToken.None);

            // Assert
            Assert.AreEqual(1, actual.Count);
            Assert.AreEqual("Person", actual[0].Source);
            Assert.AreEqual("Customer", actual[0].Target);
            Assert.AreSame(where, actual[0].Where);
        }

        [TestMethod]
        public void TestGetTablesWithATargetSchemaCopiesTheSchemaOfTheTableAndReturnsItsNameInTheTargetSchema()
        {
            // Setup
            var reader = MapReader(new[] { "Person" }, "Person");
            var composer = MapComposer();
            var destination = new FakeDbConnection();
            var where = new QueryGroup(new QueryField("Id", 1));

            // Act
            var actual = DataDbConnection.GetTables(new FakeDbConnection(), destination, "Person", "Person", "Sales", where, CopyDataRelationshipBehavior.TableOnly, CopySchemaExistsBehavior.Skip, null, null, null);

            // Assert
            Assert.AreEqual(1, actual.Count);
            Assert.AreEqual("Person", actual[0].Source);
            Assert.AreEqual("[Sales].[Person]", actual[0].Target);
            Assert.AreSame(where, actual[0].Where);
            reader.Verify(r => r.GetRelatedTables(It.IsAny<IEnumerable<string>>(), It.IsAny<CopySchemaRelationshipBehavior>()), Times.Never);
            composer.Verify(c => c.ComposeSchemas(It.IsAny<IEnumerable<TableSchema>>()), Times.Once);
            Assert.IsTrue(destination.Executions.Any(e => e.Kind == "NonQuery" && e.CommandText == "CREATE TABLE Person;"));
        }

        [TestMethod]
        public async Task TestGetTablesAsyncWithATargetSchemaCopiesTheSchemaOfTheTableAndReturnsItsNameInTheTargetSchema()
        {
            // Setup
            MapReader(new[] { "Person" }, "Person");
            MapComposer();
            var destination = new FakeDbConnection();

            // Act
            var actual = await DataDbConnection.GetTablesAsync(new FakeDbConnection(), destination, "Person", "Person", "Sales", null, CopyDataRelationshipBehavior.TableOnly, CopySchemaExistsBehavior.Skip, null, null, null, CancellationToken.None);

            // Assert
            Assert.AreEqual(1, actual.Count);
            Assert.AreEqual("Person", actual[0].Source);
            Assert.AreEqual("[Sales].[Person]", actual[0].Target);
            Assert.IsNull(actual[0].Where);
        }

        [TestMethod]
        public void TestGetTablesWithARelationshipBehaviorReturnsTheRelatedTablesInTheDependencyOrder()
        {
            // Setup
            var reader = MapReader(new[] { "Person", "Country" }, "Country", "Person");
            MapComposer();
            var where = new QueryGroup(new QueryField("Id", 1));

            // Act
            var actual = DataDbConnection.GetTables(new FakeDbConnection(), new FakeDbConnection(), "Person", "Person", null, where, CopyDataRelationshipBehavior.Parents, CopySchemaExistsBehavior.Skip, null, null, null);

            // Assert
            Assert.AreEqual(2, actual.Count);
            Assert.AreEqual("[dbo].[Country]", actual[0].Source);
            Assert.AreEqual("[dbo].[Country]", actual[0].Target);
            Assert.IsNull(actual[0].Where);
            Assert.AreEqual("Person", actual[1].Source);
            Assert.AreEqual("[dbo].[Person]", actual[1].Target);
            Assert.AreSame(where, actual[1].Where);
            reader.Verify(r => r.GetRelatedTables(It.Is<IEnumerable<string>>(n => n.SequenceEqual(new[] { "Person" })), CopySchemaRelationshipBehavior.Parents), Times.AtLeastOnce);
        }

        [TestMethod]
        public async Task TestGetTablesAsyncWithARelationshipBehaviorReturnsTheRelatedTablesInTheDependencyOrder()
        {
            // Setup
            var reader = MapReader(new[] { "Person", "Country" }, "Country", "Person");
            MapComposer();
            var where = new QueryGroup(new QueryField("Id", 1));

            // Act
            var actual = await DataDbConnection.GetTablesAsync(new FakeDbConnection(), new FakeDbConnection(), "Person", "Person", null, where, CopyDataRelationshipBehavior.Children, CopySchemaExistsBehavior.Skip, null, null, null, CancellationToken.None);

            // Assert
            Assert.AreEqual(2, actual.Count);
            Assert.AreEqual("[dbo].[Country]", actual[0].Source);
            Assert.IsNull(actual[0].Where);
            Assert.AreEqual("Person", actual[1].Source);
            Assert.AreSame(where, actual[1].Where);
            reader.Verify(r => r.GetRelatedTablesAsync(It.IsAny<IEnumerable<string>>(), CopySchemaRelationshipBehavior.Children, It.IsAny<CancellationToken>()), Times.AtLeastOnce);
        }

        [TestMethod]
        public void TestGetTablesPassesTheCommandTimeoutAndTheTraceToTheSchemaCopy()
        {
            // Setup
            MapReader(new[] { "Person" }, "Person");
            MapComposer();
            var destination = new FakeDbConnection();

            // Act
            DataDbConnection.GetTables(new FakeDbConnection(), destination, "Person", "Person", "Sales", null, CopyDataRelationshipBehavior.TableOnly, CopySchemaExistsBehavior.Skip, 77, null, null);

            // Assert
            Assert.IsTrue(destination.Executions.Any(e => e.CommandTimeout == 77));
        }

        #endregion

        #region CopyRows

        private static (FakeDbConnection Source, FakeDbConnection Destination) GetConnections(int rows = 3)
        {
            var source = new FakeDbConnection { Data = FakeDbConnection.CreatePersons(rows) };
            return (source, new FakeDbConnection());
        }

        [TestMethod]
        public void TestCopyRowsWithABulkInsertReadsTheSourceTableAndBulkInsertsTheRowsAsABatch()
        {
            // Setup
            var source = new FakeDbConnection();
            var destination = new FakeBulkDbConnection();
            var options = new CopyDataProgress();
            var reported = new List<(int Batch, int Rows, int Total)>();
            FakeBulkOperations.Reset();

            // Act
            DataDbConnection.CopyRows(source, destination, "Person", "Customer", null, 500, options, p => reported.Add((p.BatchNumber, p.RowCount, p.TotalCopiedRowCount)), 33, "MyTraceKey", null, null);

            // Assert
            var call = FakeBulkOperations.Calls.Single();
            Assert.AreEqual("BulkInsert", call.Method);
            Assert.AreEqual("Customer", call.TableName);
            Assert.AreEqual(500, call.BatchSize);
            Assert.AreEqual(33, call.Timeout);
            Assert.AreEqual(3, call.RowCount);
            Assert.AreEqual(1, source.Executions.Count);
            Assert.AreEqual("Reader", source.Executions[0].Kind);
            Assert.AreEqual(33, source.Executions[0].CommandTimeout);
            CollectionAssert.AreEqual(new[] { (1, 3, 3) }, reported);
            Assert.AreEqual(1, options.BatchNumber);
            Assert.AreEqual(3, options.RowCount);
            Assert.AreEqual(3, options.TotalCopiedRowCount);
            Assert.AreNotEqual(default(DateTime), options.EndTime);
        }

        [TestMethod]
        public async Task TestCopyRowsAsyncWithABulkInsertReadsTheSourceTableAndBulkInsertsTheRowsAsABatch()
        {
            // Setup
            var source = new FakeDbConnection();
            var destination = new FakeBulkDbConnection();
            var options = new CopyDataProgress();
            var reported = new List<(int Batch, int Rows, int Total)>();
            using (var tokenSource = new CancellationTokenSource())
            {
                FakeBulkOperations.Reset();

                // Act
                await DataDbConnection.CopyRowsAsync(source, destination, "Person", "Customer", null, 500, options, p => reported.Add((p.BatchNumber, p.RowCount, p.TotalCopiedRowCount)), 33, null, null, null, tokenSource.Token);

                // Assert
                var call = FakeBulkOperations.Calls.Single();
                Assert.AreEqual("BulkInsertAsync", call.Method);
                Assert.AreEqual("Customer", call.TableName);
                Assert.AreEqual(500, call.BatchSize);
                Assert.AreEqual(33, call.Timeout);
                Assert.AreEqual(3, call.RowCount);
                Assert.AreEqual(tokenSource.Token, call.CancellationToken);
                Assert.AreEqual("Reader", source.Executions.Single().Kind);
                CollectionAssert.AreEqual(new[] { (1, 3, 3) }, reported);
            }
        }

        [TestMethod]
        public void TestCopyRowsWithoutABulkInsertInsertsTheRowsInBatches()
        {
            // Setup
            var (source, destination) = GetConnections(5);
            var options = new CopyDataProgress();
            var reported = new List<(int Batch, int Rows, int Total)>();

            // Act
            DataDbConnection.CopyRows(source, destination, "Person", "Customer", null, 2, options, p => reported.Add((p.BatchNumber, p.RowCount, p.TotalCopiedRowCount)), null, null, null, null);

            // Assert
            Assert.AreEqual(5, destination.Executions.Count(e => e.Kind == "Scalar"));
            CollectionAssert.AreEqual(new[] { (1, 2, 2), (2, 2, 4), (3, 1, 5) }, reported);
            Assert.AreEqual(3, options.BatchNumber);
            Assert.AreEqual(5, options.TotalCopiedRowCount);
        }

        [TestMethod]
        public async Task TestCopyRowsAsyncWithoutABulkInsertInsertsTheRowsInBatches()
        {
            // Setup
            var (source, destination) = GetConnections(5);
            var options = new CopyDataProgress();
            var reported = new List<(int Batch, int Rows, int Total)>();

            // Act
            await DataDbConnection.CopyRowsAsync(source, destination, "Person", "Customer", null, 2, options, p => reported.Add((p.BatchNumber, p.RowCount, p.TotalCopiedRowCount)), null, null, null, null, CancellationToken.None);

            // Assert
            Assert.AreEqual(5, destination.Executions.Count(e => e.Kind == "Scalar"));
            CollectionAssert.AreEqual(new[] { (1, 2, 2), (2, 2, 4), (3, 1, 5) }, reported);
            Assert.AreEqual(5, options.TotalCopiedRowCount);
        }

        [TestMethod]
        public void TestCopyRowsWithoutABulkInsertAndAnExactNumberOfBatchesDoesNotInsertAnEmptyBatch()
        {
            // Setup
            var (source, destination) = GetConnections(4);
            var options = new CopyDataProgress();

            // Act
            DataDbConnection.CopyRows(source, destination, "Person", "Customer", null, 2, options, null, null, null, null, null);

            // Assert
            Assert.AreEqual(2, options.BatchNumber);
            Assert.AreEqual(4, options.TotalCopiedRowCount);
        }

        [TestMethod]
        public void TestCopyRowsWithoutRowsInsertsNothing()
        {
            // Setup
            var (source, destination) = GetConnections(0);
            var options = new CopyDataProgress();
            var called = false;

            // Act
            DataDbConnection.CopyRows(source, destination, "Person", "Customer", null, 2, options, p => called = true, null, null, null, null);

            // Assert
            Assert.IsFalse(called);
            Assert.AreEqual(0, options.BatchNumber);
            Assert.AreEqual(0, options.TotalCopiedRowCount);
            Assert.AreEqual(0, destination.Executions.Count);
        }

        [TestMethod]
        public async Task TestCopyRowsAsyncWithoutRowsInsertsNothing()
        {
            // Setup
            var (source, destination) = GetConnections(0);
            var options = new CopyDataProgress();

            // Act
            await DataDbConnection.CopyRowsAsync(source, destination, "Person", "Customer", null, 2, options, null, null, null, null, null, CancellationToken.None);

            // Assert
            Assert.AreEqual(0, options.BatchNumber);
            Assert.AreEqual(0, destination.Executions.Count);
        }

        [TestMethod]
        public void TestCopyRowsWithoutABulkInsertInsertsTheColumnsOfTheRows()
        {
            // Setup
            var (source, destination) = GetConnections(3);

            // Act
            DataDbConnection.CopyRows(source, destination, "Person", "Customer", null, 3, new CopyDataProgress(), null, null, null, null, null);

            // Assert
            var insert = destination.Executions.First(e => e.Kind == "Scalar");
            StringAssert.Contains(insert.CommandText, "[Customer]");
            StringAssert.Contains(insert.CommandText, "[Name]");
            StringAssert.Contains(insert.CommandText, "[Birthday]");
            Assert.IsTrue(insert.Parameters.Values.Contains("Name 1"));
        }

        [TestMethod]
        public void TestCopyRowsWithoutABulkInsertInsertsTheNullValuesOfTheRowsAsNull()
        {
            // Setup
            var (source, destination) = GetConnections(3);

            // Act
            DataDbConnection.CopyRows(source, destination, "Person", "Customer", null, 3, new CopyDataProgress(), null, null, null, null, null);

            // Assert
            var withoutBirthday = destination.Executions.Where(e => e.Kind == "Scalar").Single(e => e.Parameters.Values.Contains("Name 2"));
            Assert.IsFalse(withoutBirthday.Parameters.Values.Any(v => v is DateTime));
        }

        [TestMethod]
        public void TestCopyRowsSelectsTheColumnsOfTheSourceTable()
        {
            // Setup
            var (source, destination) = GetConnections();

            // Act
            DataDbConnection.CopyRows(source, destination, "Person", "Person", null, 1000, new CopyDataProgress(), null, null, null, null, null);

            // Assert
            var select = source.Executions.Single();
            StringAssert.Contains(select.CommandText, "SELECT");
            StringAssert.Contains(select.CommandText, "[Person]");
            StringAssert.Contains(select.CommandText, "[Id]");
            Assert.AreEqual(0, select.Parameters.Count);
        }

        [TestMethod]
        public void TestCopyRowsWithAFilterSelectsTheRowsThatMatchTheFilter()
        {
            // Setup
            var (source, destination) = GetConnections();
            var where = new QueryGroup(new QueryField("Id", Operation.GreaterThan, 1));

            // Act
            DataDbConnection.CopyRows(source, destination, "Person", "Person", where, 1000, new CopyDataProgress(), null, null, null, null, null);

            // Assert
            var select = source.Executions.Single();
            StringAssert.Contains(select.CommandText, "WHERE");
            Assert.AreEqual(1, select.Parameters.Count);
            Assert.AreEqual(1, select.Parameters.Values.Single());
        }

        [TestMethod]
        public void TestCopyRowsWithABetweenFilterGivesTheLeftAndTheRightValues()
        {
            // Setup
            var (source, destination) = GetConnections();
            var where = new QueryGroup(new QueryField("Id", Operation.Between, new[] { 1, 3 }));

            // Act
            DataDbConnection.CopyRows(source, destination, "Person", "Person", where, 1000, new CopyDataProgress(), null, null, null, null, null);

            // Assert
            var parameters = source.Executions.Single().Parameters;
            Assert.AreEqual(2, parameters.Count);
            Assert.AreEqual(1, parameters.Single(p => p.Key.EndsWith("_Left", StringComparison.Ordinal)).Value);
            Assert.AreEqual(3, parameters.Single(p => p.Key.EndsWith("_Right", StringComparison.Ordinal)).Value);
        }

        [TestMethod]
        public void TestCopyRowsWithABetweenFilterThatHasASingleValueGivesANullRightValue()
        {
            // Setup
            var (source, destination) = GetConnections();
            var where = new QueryGroup(new QueryField("Id", Operation.Between, new[] { 1 }));

            // Act
            DataDbConnection.CopyRows(source, destination, "Person", "Person", where, 1000, new CopyDataProgress(), null, null, null, null, null);

            // Assert
            var parameters = source.Executions.Single().Parameters;
            Assert.AreEqual(1, parameters.Single(p => p.Key.EndsWith("_Left", StringComparison.Ordinal)).Value);
            Assert.IsTrue(parameters.Single(p => p.Key.EndsWith("_Right", StringComparison.Ordinal)).Value == null ||
                parameters.Single(p => p.Key.EndsWith("_Right", StringComparison.Ordinal)).Value is DBNull);
        }

        [TestMethod]
        public void TestCopyRowsWithAnInFilterGivesAParameterPerValue()
        {
            // Setup
            var (source, destination) = GetConnections();
            var where = new QueryGroup(new QueryField("Id", Operation.In, new[] { 1, 2, 3 }));

            // Act
            DataDbConnection.CopyRows(source, destination, "Person", "Person", where, 1000, new CopyDataProgress(), null, null, null, null, null);

            // Assert
            var parameters = source.Executions.Single().Parameters;
            Assert.AreEqual(3, parameters.Count);
            CollectionAssert.AreEquivalent(new object[] { 1, 2, 3 }, parameters.Values.ToArray());
            Assert.IsTrue(parameters.Keys.All(k => k.Contains("_In_")));
        }

        [TestMethod]
        public void TestCopyRowsWithAFilterOfASingleValueGivesTheValueByItsParameterName()
        {
            // Setup
            var (source, destination) = GetConnections();
            var where = new QueryGroup(new QueryField("Name", "Name 2"));

            // Act
            DataDbConnection.CopyRows(source, destination, "Person", "Person", where, 1000, new CopyDataProgress(), null, null, null, null, null);

            // Assert
            var parameters = source.Executions.Single().Parameters;
            Assert.AreEqual(1, parameters.Count);
            Assert.AreEqual("Name 2", parameters.Values.Single());
        }

        [TestMethod]
        public async Task TestCopyRowsAsyncWithAFilterSelectsTheRowsThatMatchTheFilter()
        {
            // Setup
            var (source, destination) = GetConnections();
            var where = new QueryGroup(new QueryField("Id", Operation.In, new[] { 1, 2 }));

            // Act
            await DataDbConnection.CopyRowsAsync(source, destination, "Person", "Person", where, 1000, new CopyDataProgress(), null, null, null, null, null, CancellationToken.None);

            // Assert
            var parameters = source.Executions.First().Parameters;
            Assert.AreEqual(2, parameters.Count);
        }

        [TestMethod]
        public void TestCopyRowsWithoutAProgressCallbackStillUpdatesTheProgress()
        {
            // Setup
            var (source, destination) = GetConnections();
            var options = new CopyDataProgress();

            // Act
            DataDbConnection.CopyRows(source, destination, "Person", "Person", null, 1000, options, null, null, null, null, null);

            // Assert
            Assert.AreEqual(1, options.BatchNumber);
            Assert.AreEqual(3, options.TotalCopiedRowCount);
        }

        [TestMethod]
        public void ThrowExceptionOnCopyRowsIfThereIsNoStatementBuilderForTheSourceConnection()
        {
            // Act/Assert
            Assert.Throws<InvalidOperationException>(() =>
                DataDbConnection.CopyRows(new Mock<IDbConnection>().Object, new FakeDbConnection(), "Person", "Person", null, 1000, new CopyDataProgress(), null, null, null, null, null));
        }

        [TestMethod]
        public async Task ThrowExceptionOnCopyRowsAsyncIfThereIsNoStatementBuilderForTheSourceConnection()
        {
            // Act/Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                DataDbConnection.CopyRowsAsync(new Mock<IDbConnection>().Object, new FakeDbConnection(), "Person", "Person", null, 1000, new CopyDataProgress(), null, null, null, null, null, CancellationToken.None));
        }

        [TestMethod]
        public async Task ThrowExceptionOnCopyRowsAsyncIfTheOperationIsCancelledBeforeTheRowsAreRead()
        {
            // Setup
            var (source, destination) = GetConnections();
            using (var tokenSource = new CancellationTokenSource())
            {
                tokenSource.Cancel();

                // Act/Assert
                await Assert.ThrowsAsync<OperationCanceledException>(() =>
                    DataDbConnection.CopyRowsAsync(source, destination, "Person", "Person", null, 1000, new CopyDataProgress(), null, null, null, null, null, tokenSource.Token));
            }
        }

        [TestMethod]
        public void TestCopyRowsPassesTheTraceKeyOfTheSourceCommand()
        {
            // Setup
            var (source, destination) = GetConnections();
            var trace = new Mock<ITrace>();

            // Act
            DataDbConnection.CopyRows(source, destination, "Person", "Person", null, 1000, new CopyDataProgress(), null, null, "MyTraceKey", trace.Object, null);

            // Assert
            trace.Verify(t => t.BeforeExecution(It.Is<CancellableTraceLog>(l => l.Key == "MyTraceKey")), Times.AtLeastOnce);
        }

        #endregion

        #region Validate

        [TestMethod]
        public void ThrowExceptionOnValidateIfTheConnectionIsNull()
        {
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() =>
                DataDbConnection.Validate(null, "Person", "Person", GetConnection(), null, 1, CopyDataRelationshipBehavior.TableOnly, CopySchemaExistsBehavior.Skip));
        }

        [TestMethod]
        public void ThrowExceptionOnValidateIfTheSourceTableIsNullOrWhiteSpace()
        {
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() =>
                DataDbConnection.Validate(GetConnection(), null, "Person", GetConnection(), null, 1, CopyDataRelationshipBehavior.TableOnly, CopySchemaExistsBehavior.Skip));
            Assert.Throws<ArgumentNullException>(() =>
                DataDbConnection.Validate(GetConnection(), " ", "Person", GetConnection(), null, 1, CopyDataRelationshipBehavior.TableOnly, CopySchemaExistsBehavior.Skip));
        }

        [TestMethod]
        public void ThrowExceptionOnValidateIfTheTargetTableIsNullOrWhiteSpace()
        {
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() =>
                DataDbConnection.Validate(GetConnection(), "Person", null, GetConnection(), null, 1, CopyDataRelationshipBehavior.TableOnly, CopySchemaExistsBehavior.Skip));
            Assert.Throws<ArgumentNullException>(() =>
                DataDbConnection.Validate(GetConnection(), "Person", " ", GetConnection(), null, 1, CopyDataRelationshipBehavior.TableOnly, CopySchemaExistsBehavior.Skip));
        }

        [TestMethod]
        public void ThrowExceptionOnValidateIfTheDestinationConnectionIsNull()
        {
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() =>
                DataDbConnection.Validate(GetConnection(), "Person", "Person", null, null, 1, CopyDataRelationshipBehavior.TableOnly, CopySchemaExistsBehavior.Skip));
        }

        [TestMethod]
        public void ThrowExceptionOnValidateIfTheBatchSizeIsNotGreaterThanZero()
        {
            // Act/Assert
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                DataDbConnection.Validate(GetConnection(), "Person", "Person", GetConnection(), null, 0, CopyDataRelationshipBehavior.TableOnly, CopySchemaExistsBehavior.Skip));
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                DataDbConnection.Validate(GetConnection(), "Person", "Person", GetConnection(), null, -1, CopyDataRelationshipBehavior.TableOnly, CopySchemaExistsBehavior.Skip));
        }

        [TestMethod]
        public void ThrowExceptionOnValidateIfTheTablesHaveDifferentNamesAndTheSchemaIsRequested()
        {
            // Act/Assert
            Assert.Throws<ArgumentException>(() =>
                DataDbConnection.Validate(new FakeDbConnection(), "Person", "Customer", new FakeDbConnection(), null, 1, CopyDataRelationshipBehavior.Parents, CopySchemaExistsBehavior.Skip));
            Assert.Throws<ArgumentException>(() =>
                DataDbConnection.Validate(new FakeDbConnection(), "Person", "Customer", new FakeDbConnection(), null, 1, CopyDataRelationshipBehavior.TableOnly, CopySchemaExistsBehavior.Drop));
            Assert.Throws<ArgumentException>(() =>
                DataDbConnection.Validate(new FakeDbConnection(), "Person", "Customer", new FakeDbConnection(), "Sales", 1, CopyDataRelationshipBehavior.TableOnly, CopySchemaExistsBehavior.Skip));
        }

        [TestMethod]
        public void TestValidateAcceptsTablesWithDifferentNamesIfTheSchemaIsNotRequested()
        {
            // Act/Assert
            DataDbConnection.Validate(new FakeDbConnection(), "Person", "Customer", new FakeDbConnection(), null, 1, CopyDataRelationshipBehavior.TableOnly, CopySchemaExistsBehavior.Skip);
        }

        [TestMethod]
        public void TestValidateAcceptsTablesWithTheSameNameInDifferentSchemasIfTheSchemaIsRequested()
        {
            // Act/Assert
            DataDbConnection.Validate(new FakeDbConnection(), "[dbo].[Person]", "[Sales].[Person]", new FakeDbConnection(), "Sales", 1, CopyDataRelationshipBehavior.Parents, CopySchemaExistsBehavior.Drop);
        }

        #endregion
    }
}
