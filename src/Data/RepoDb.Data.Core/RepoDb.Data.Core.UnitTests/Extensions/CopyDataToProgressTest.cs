#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Data.Core.UnitTests.CustomObjects;
using RepoDb.Data.Core.UnitTests.Fake.BulkOperations;
using RepoDb.Data.Enumerations;
using RepoDb.Data.Models;
using RepoDb.DbSettings;
using DataDbConnection = RepoDb.Data.DbConnection;

namespace RepoDb.Data.Core.UnitTests.Extensions
{
    [TestClass]
    public class CopyDataToProgressTest
    {
        [TestMethod]
        [DataRow(false, false, false)]
        [DataRow(true, false, false)]
        [DataRow(false, true, false)]
        [DataRow(true, true, false)]
        [DataRow(false, false, true)]
        [DataRow(true, false, true)]
        [DataRow(false, true, true)]
        [DataRow(true, true, true)]
        public async Task CountsSourcesThenReportsInMemoryRowsAndBytes(bool asynchronous, bool bulk, bool merge)
        {
            var source = new FakeDbConnection { Data = FakeDbConnection.CreatePersons(5) };
            var bulkDestination = FakeBulkDestination.Create();
            var rowDestination = new FakeDbConnection();
            System.Data.IDbConnection destination = bulk ? bulkDestination : rowDestination;
            var seen = new List<(long Total, int Rows, long Bytes)>();
            Action<CopyProgress> callback = p =>
            {
                seen.Add((p.TotalRowCount, p.RowCount, p.CopiedBytes));
                if (p.Items.Count == 0)
                {
                    Assert.AreEqual(0, p.Logs.Count);
                    Assert.IsNull(p.Current);
                    Assert.IsNull(p.CurrentBatch);
                    Assert.AreEqual(0, p.BatchCount);
                    Assert.AreEqual(0, p.CompletedTableCount);
                    Assert.AreEqual(0, source.Executions.Count(e => e.Kind == "Reader"));
                }
                else
                {
                    Assert.AreEqual(5L, p.Current.TotalRowCount);
                    Assert.AreEqual(p.RowCount, p.Current.RowCount);
                    Assert.AreEqual(p.CopiedBytes, p.Current.CopiedBytes);
                }
            };
            var result = asynchronous
                ? await source.CopyDataToAsync("Person", "Customer", destination, bufferSize: 2, bypassExistingRows: merge, progressCallback: callback)
                : source.CopyDataTo("Person", "Customer", destination, bufferSize: 2, bypassExistingRows: merge, progressCallback: callback);

            CollectionAssert.AreEqual(new[] { (5L, 0, 0L), (5L, 2, 40L), (5L, 4, 88L), (5L, 5, 112L) }, seen);
            Assert.AreEqual(112L, result.Items.Single().Batches.Sum(b => b.CopiedBytes));
            Assert.AreEqual(0, bulkDestination.ScalarExecutions);
            Assert.AreEqual(0, rowDestination.Executions.Count(e => e.Kind == "Scalar"));
            Assert.IsNull(result.Items.Single().Target.RowCount);
            Assert.IsNull(result.Items.Single().SkippedRowCount);
            CollectionAssert.AreEqual(new[] { "Scalar", "Reader" }, source.Executions.Select(e => e.Kind).ToArray());
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task CountsEverySourceBeforeCreatingItemsAndPreservesFilters(bool asynchronous)
        {
            var source = new FakeDbConnection { Data = FakeDbConnection.CreatePersons(3) };
            var filter = new QueryGroup(new QueryField("Id", RepoDb.Enumerations.Operation.GreaterThan, 1));
            var tables = new List<(string Source, string Target, QueryGroup Where)>
            {
                ("Person", "Customer", filter), ("Pet", "Animal", null)
            };
            var progress = new CopyProgress { TableCount = 2 };
            var totals = new List<long>();
            Action<CopyProgress> callback = p =>
            {
                totals.Add(p.TotalRowCount);
                Assert.AreEqual(0, p.Items.Count);
                Assert.AreEqual(0, p.Logs.Count);
                Assert.AreEqual(0, p.RowCount);
                Assert.AreEqual(0L, p.CopiedBytes);
                Assert.IsNull(p.Current);
            };
            if (asynchronous)
                await DataDbConnection.CountSourceRowsAsync(source, tables, progress, callback, 17, null, CancellationToken.None);
            else
                DataDbConnection.CountSourceRows(source, tables, progress, callback, 17, null);

            // The fake does not evaluate filters; verify the count query and its parameters instead.
            CollectionAssert.AreEqual(new[] { 3L, 6L }, totals);
            Assert.IsTrue(source.Executions.All(e => e.Kind == "Scalar" && e.CommandTimeout == 17));
            StringAssert.Contains(source.Executions[0].CommandText, "WHERE");
            Assert.AreEqual(1, source.Executions[0].Parameters.Values.Single());
            Assert.IsFalse(source.Executions[1].CommandText.Contains("WHERE", StringComparison.Ordinal));
            Assert.AreEqual(3L, progress.SourceRowCounts["Person"]);
            Assert.AreEqual(3L, progress.SourceRowCounts["Pet"]);
        }

        [TestMethod]
        public async Task CancellationDuringCountingStopsBeforeCopying()
        {
            var source = new FakeDbConnection();
            using var cancellation = new CancellationTokenSource();
            var tables = new List<(string Source, string Target, QueryGroup Where)>
            {
                ("Person", "Customer", null), ("Pet", "Animal", null)
            };
            var progress = new CopyProgress();
            await Assert.ThrowsExactlyAsync<OperationCanceledException>(() => DataDbConnection.CountSourceRowsAsync(
                source, tables, progress, _ => cancellation.Cancel(), null, null, cancellation.Token));
            Assert.AreEqual(1, source.Executions.Count);
            Assert.AreEqual(0, progress.Items.Count);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task EmptySourcesStillReportTheirTotal(bool asynchronous)
        {
            var source = new FakeDbConnection { Data = FakeDbConnection.CreatePersons(0) };
            var callbacks = 0;
            Action<CopyProgress> callback = p =>
            {
                callbacks++;
                Assert.AreEqual(0L, p.TotalRowCount);
                Assert.AreEqual(0, p.RowCount);
                Assert.AreEqual(0L, p.CopiedBytes);
            };
            var result = asynchronous
                ? await source.CopyDataToAsync("Person", "Customer", new FakeDbConnection(), progressCallback: callback)
                : source.CopyDataTo("Person", "Customer", new FakeDbConnection(), progressCallback: callback);
            Assert.AreEqual(1, callbacks);
            Assert.AreEqual(1, result.CompletedTableCount);
        }

        [TestMethod]
        [DataRow(false, false)]
        [DataRow(true, false)]
        [DataRow(false, true)]
        [DataRow(true, true)]
        public async Task CopiedBytesUsesTransformedValuesAndExcludesGeneratedIdentity(bool asynchronous, bool bulk)
        {
            var source = new FakeDbConnection { Data = FakeDbConnection.CreatePersons(3) };
            System.Data.IDbConnection destination = bulk ? FakeBulkDestination.Create() : new FakeDbConnection();
            var interceptor = new FakeDataInterceptor(row =>
            {
                row["Name"] = "X";
                return row;
            });
            var interceptors = new List<RepoDb.Data.Interfaces.IDataInterceptor<Row>> { interceptor };
            var result = asynchronous
                ? await source.CopyDataToAsync("Person", "Customer", destination, identityBehavior: IdentityBehavior.Ignore, dataInterceptors: interceptors)
                : source.CopyDataTo("Person", "Customer", destination, identityBehavior: IdentityBehavior.Ignore, dataInterceptors: interceptors);
            // Three one-character strings and two non-null dates; identity values are excluded.
            Assert.AreEqual(22L, result.CopiedBytes);
            Assert.AreEqual(22L, result.Items.Single().CopiedBytes);
            Assert.AreEqual(3L, result.TotalRowCount);
        }

        #region Initialize

        [TestInitialize]
        public void Initialize()
        {
            DbSettingMapper.Add<FakeBulkDbConnection>(new FakeDbSetting(), true);
            FakeBulkOperations.Reset();
        }

        #endregion

        #region The Progress Of The Operation

        [TestMethod]
        public void TestCopyDataToReturnsTheProgressWithTheItemOfTheTable()
        {
            // Setup
            var source = new FakeDbConnection { Data = FakeDbConnection.CreatePersons(3) };

            // Act
            var actual = source.CopyDataTo("Person", "Customer", new FakeBulkDbConnection());

            // Assert
            Assert.AreEqual(1, actual.TableCount);
            Assert.AreEqual(1, actual.CompletedTableCount);
            Assert.AreEqual(3, actual.RowCount);
            Assert.AreEqual(1, actual.BatchCount);
            Assert.AreEqual(1, actual.Items.Count);
            Assert.AreSame(actual.Items[0], actual.Current);
            Assert.IsTrue(actual.EndTime >= actual.StartTime);
            Assert.AreNotEqual(default(DateTime), actual.StartTime);
        }

        [TestMethod]
        public async Task TestCopyDataToAsyncReturnsTheProgressWithTheItemOfTheTable()
        {
            // Setup
            var source = new FakeDbConnection { Data = FakeDbConnection.CreatePersons(3) };

            // Act
            var actual = await source.CopyDataToAsync("Person", "Customer", new FakeBulkDbConnection());

            // Assert
            Assert.AreEqual(1, actual.TableCount);
            Assert.AreEqual(1, actual.CompletedTableCount);
            Assert.AreEqual(3, actual.RowCount);
            Assert.AreEqual(1, actual.BatchCount);
            Assert.AreEqual(1, actual.Items.Count);
            Assert.AreSame(actual.Items[0], actual.Current);
        }

        [TestMethod]
        public void TestCopyDataToItemHasTheSourceAndTheTargetOfTheTable()
        {
            // Setup
            var source = new FakeDbConnection { Data = FakeDbConnection.CreatePersons(3) };
            var destination = new FakeBulkDbConnection();

            // Act
            var item = source.CopyDataTo("Person", "Customer", destination).Items.Single();

            // Assert
            Assert.AreEqual("Person", item.Source.Table.Name);
            Assert.AreEqual("Customer", item.Target.Table.Name);
            Assert.IsNull(item.Source.Table.Schema);
            Assert.IsNull(item.Target.Table.Schema);
            Assert.AreEqual(source.Database, item.Source.Database);
            Assert.AreEqual(source.Database, item.Source.Table.Database);
            Assert.AreEqual(destination.Database, item.Target.Database);
            Assert.AreEqual(destination.DataSource, item.Target.Server);
            Assert.AreEqual(nameof(FakeDbConnection), item.Source.DatabaseType);
            Assert.AreEqual(nameof(FakeBulkDbConnection), item.Target.DatabaseType);
            Assert.AreNotSame(item.Source, item.Target);
        }

        [TestMethod]
        public void TestCopyDataToItemHasTheRowsAndTheBatchesOfTheBulkInsertPerBufferSize()
        {
            // Setup
            var source = new FakeDbConnection { Data = FakeDbConnection.CreatePersons(5) };

            // Act
            var item = source.CopyDataTo("Person", "Customer", new FakeBulkDbConnection(), bufferSize: 2).Items.Single();

            // Assert
            Assert.AreEqual(5, item.RowCount);
            Assert.AreEqual(5L, item.Source.RowCount);
            CollectionAssert.AreEqual(new[] { 1, 2, 3 }, item.Batches.Select(b => b.Number).ToArray());
            CollectionAssert.AreEqual(new[] { 2, 2, 1 }, item.Batches.Select(b => b.RowCount).ToArray());
            CollectionAssert.AreEqual(new[] { 2, 2, 1 }, FakeBulkOperations.Calls.Select(c => c.RowCount).ToArray());
        }

        [TestMethod]
        public void TestCopyDataToCallsTheProgressCallbackRightAfterEveryBatchOfTheBulkInsert()
        {
            // Setup
            var source = new FakeDbConnection { Data = FakeDbConnection.CreatePersons(5) };
            var seen = new List<(long Rows, int Calls)>();

            // Act
            source.CopyDataTo("Person", "Customer", new FakeBulkDbConnection(), bufferSize: 2, progressCallback: p => seen.Add((p.RowCount, FakeBulkOperations.Calls.Length)));

            // Assert
            CollectionAssert.AreEqual(new[] { (0L, 0), (2L, 1), (4L, 2), (5L, 3) }, seen);
        }

        [TestMethod]
        public async Task TestCopyDataToAsyncCallsTheProgressCallbackRightAfterEveryBatchOfTheBulkInsert()
        {
            // Setup
            var source = new FakeDbConnection { Data = FakeDbConnection.CreatePersons(5) };
            var seen = new List<(long Rows, int Calls)>();

            // Act
            await source.CopyDataToAsync("Person", "Customer", new FakeBulkDbConnection(), bufferSize: 2, progressCallback: p => seen.Add((p.RowCount, FakeBulkOperations.Calls.Length)));

            // Assert
            CollectionAssert.AreEqual(new[] { (0L, 0), (2L, 1), (4L, 2), (5L, 3) }, seen);
        }

        [TestMethod]
        public void TestCopyDataToReportsOneBatchWithNoRowForATableWithoutRows()
        {
            // Setup
            var source = new FakeDbConnection { Data = FakeDbConnection.CreatePersons(0) };

            // Act
            var item = source.CopyDataTo("Person", "Customer", new FakeBulkDbConnection(), bufferSize: 2).Items.Single();

            // Assert
            Assert.AreEqual(0, item.RowCount);
            Assert.AreEqual(1, item.Batches.Count);
            Assert.AreEqual(0, item.Batches[0].RowCount);
            Assert.AreEqual(0, FakeBulkOperations.Calls.Length);
        }

        [TestMethod]
        public void TestCopyDataToItemHasTheBatchesOfTheRowsThatAreInsertedInBatches()
        {
            // Setup
            var source = new FakeDbConnection { Data = FakeDbConnection.CreatePersons(5) };

            // Act
            var actual = source.CopyDataTo("Person", "Customer", FakeBulkDestination.Create(), bufferSize: 2);

            // Assert
            var item = actual.Items.Single();
            CollectionAssert.AreEqual(new[] { 1, 2, 3 }, item.Batches.Select(b => b.Number).ToArray());
            CollectionAssert.AreEqual(new[] { 2, 2, 1 }, item.Batches.Select(b => b.RowCount).ToArray());
            Assert.AreEqual(5, item.RowCount);
            Assert.AreEqual(5L, item.Source.RowCount);
            Assert.AreEqual(3, actual.BatchCount);
            Assert.AreEqual(5, actual.RowCount);
        }

        [TestMethod]
        public async Task TestCopyDataToAsyncItemHasTheBatchesOfTheRowsThatAreInsertedInBatches()
        {
            // Setup
            var source = new FakeDbConnection { Data = FakeDbConnection.CreatePersons(5) };

            // Act
            var actual = await source.CopyDataToAsync("Person", "Customer", FakeBulkDestination.Create(), bufferSize: 2);

            // Assert
            var item = actual.Items.Single();
            CollectionAssert.AreEqual(new[] { 1, 2, 3 }, item.Batches.Select(b => b.Number).ToArray());
            CollectionAssert.AreEqual(new[] { 2, 2, 1 }, item.Batches.Select(b => b.RowCount).ToArray());
            Assert.AreEqual(3, actual.BatchCount);
            Assert.AreEqual(5, actual.RowCount);
        }

        [TestMethod]
        public void TestCopyDataToBatchesFollowOneAnotherInTime()
        {
            // Setup
            var source = new FakeDbConnection { Data = FakeDbConnection.CreatePersons(5) };

            // Act
            var item = source.CopyDataTo("Person", "Customer", FakeBulkDestination.Create(), bufferSize: 2).Items.Single();

            // Assert
            Assert.IsTrue(item.Batches.All(b => b.StartTime >= item.StartTime && b.StartTime <= b.EndTime && b.EndTime <= item.EndTime));
            Assert.IsTrue(item.Batches[0].EndTime <= item.Batches[1].StartTime);
            Assert.IsTrue(item.Batches[1].EndTime <= item.Batches[2].StartTime);
            Assert.IsTrue(item.EndTime >= item.StartTime);
        }

        [TestMethod]
        public void TestCopyDataToItemOfARowlessTableHasNoBatchIfTheRowsAreInsertedInBatches()
        {
            // Setup
            var source = new FakeDbConnection { Data = FakeDbConnection.CreatePersons(0) };

            // Act
            var actual = source.CopyDataTo("Person", "Customer", new FakeDbConnection());

            // Assert
            var item = actual.Items.Single();
            Assert.AreEqual(0, item.Batches.Count);
            Assert.AreEqual(0, item.RowCount);
            Assert.AreEqual(0L, item.Source.RowCount);
            Assert.AreEqual(1, actual.CompletedTableCount);
            Assert.IsNull(actual.CurrentBatch);
        }

        #endregion

        #region The Callbacks

        [TestMethod]
        public void TestCopyDataToCallsTheProgressCallbackWithTheSameProgressAndTheTotalsSoFar()
        {
            // Setup
            var source = new FakeDbConnection { Data = FakeDbConnection.CreatePersons(5) };
            var seen = new List<CopyProgress>();
            var totals = new List<(int Rows, int Batches, int Number, int BatchRows)>();

            // Act
            var actual = source.CopyDataTo("Person", "Customer", FakeBulkDestination.Create(), bufferSize: 2,
                progressCallback: p =>
                {
                    seen.Add(p);
                    if (p.CurrentBatch != null) totals.Add((p.RowCount, p.BatchCount, p.CurrentBatch.Number, p.CurrentBatch.RowCount));
                });

            // Assert
            Assert.IsTrue(seen.All(p => ReferenceEquals(p, actual)));
            CollectionAssert.AreEqual(new[] { (2, 1, 1, 2), (4, 2, 2, 2), (5, 3, 3, 1) }, totals);
        }

        [TestMethod]
        public void TestCopyDataToMarksTheEndOfTheTableAfterAllItsBatches()
        {
            // Setup
            var source = new FakeDbConnection { Data = FakeDbConnection.CreatePersons(5) };

            // Act
            var actual = source.CopyDataTo("Person", "Customer", FakeBulkDestination.Create(), bufferSize: 2);

            // Assert
            Assert.AreEqual(1, actual.CompletedTableCount);
            Assert.IsTrue(actual.Items[0].EndTime >= actual.Items[0].Batches.Last().EndTime);
        }

        [TestMethod]
        public async Task TestCopyDataToAsyncMarksTheEndOfTheTable()
        {
            // Setup
            var source = new FakeDbConnection { Data = FakeDbConnection.CreatePersons(5) };

            // Act
            var actual = await source.CopyDataToAsync("Person", "Customer", FakeBulkDestination.Create(), bufferSize: 2);

            // Assert
            Assert.AreEqual(1, actual.CompletedTableCount);
        }

        [TestMethod]
        public void TestCopyDataToWithoutTheCallbackStillRecordsTheItems()
        {
            // Setup
            var source = new FakeDbConnection { Data = FakeDbConnection.CreatePersons(3) };

            // Act
            var actual = source.CopyDataTo("Person", "Customer", new FakeBulkDbConnection(), progressCallback: null);

            // Assert
            Assert.AreEqual(1, actual.Items.Count);
            Assert.AreEqual(3, actual.Items[0].RowCount);
        }

        #endregion

        #region The Logs

        [TestMethod]
        public void TestCopyDataToAddsTheLogsOfTheBulkInsertToTheItemAndToTheProgress()
        {
            // Setup
            var source = new FakeDbConnection { Data = FakeDbConnection.CreatePersons(3) };

            // Act
            var actual = source.CopyDataTo("Person", "Customer", new FakeBulkDbConnection());

            // Assert
            var item = actual.Items.Single();
            Assert.IsTrue(item.Logs.Count >= 1);
            StringAssert.Contains(item.Logs[0].Text, "Customer", StringComparison.Ordinal);
            Assert.AreEqual(LogLevel.Info, item.Logs[0].Level);
            Assert.AreEqual(item.Logs.Count, actual.Logs.Count);
            for (var i = 0; i < item.Logs.Count; i++)
            {
                Assert.AreSame(item.Logs[i], actual.Logs[i]);
            }
        }

        #endregion

        #region More Than One Table

        [TestMethod]
        public void TestCopyRowsOfMoreThanOneTableKeepsAnItemForEachTable()
        {
            // Setup
            var source = new FakeDbConnection { Data = FakeDbConnection.CreatePersons(3) };
            var destination = new FakeBulkDbConnection();
            var options = new CopyProgress { StartTime = DateTime.UtcNow };

            // Act
            DataDbConnection.CopyRows(source, destination, "Person", "Customer", null, 1000, false, null, ColumnMappingBehavior.Automatic, IdentityBehavior.KeepIdentity, null, null, options, null, null, null, null, null);
            DataDbConnection.CopyRows(source, destination, "Pet", "Animal", null, 1000, false, null, ColumnMappingBehavior.Automatic, IdentityBehavior.KeepIdentity, null, null, options, null, null, null, null, null);

            // Assert
            Assert.AreEqual(2, options.Items.Count);
            Assert.AreEqual(2, options.CompletedTableCount);
            Assert.AreEqual(6, options.RowCount);
            Assert.AreEqual(2, options.BatchCount);
            Assert.AreSame(options.Items[1], options.Current);
            CollectionAssert.AreEqual(new[] { "Person", "Pet" }, options.Items.Select(i => i.Source.Table.Name).ToArray());
            CollectionAssert.AreEqual(new[] { "Customer", "Animal" }, options.Items.Select(i => i.Target.Table.Name).ToArray());
            CollectionAssert.AreEqual(new[] { 3, 3 }, options.Items.Select(i => i.RowCount).ToArray());
            Assert.IsTrue(options.Items.All(i => i.Batches.Single().Number == 1));
        }

        [TestMethod]
        public void TestCopyRowsOfMoreThanOneTableKeepsTheLogsOfEachTableApart()
        {
            // Setup
            var source = new FakeDbConnection { Data = FakeDbConnection.CreatePersons(3) };
            var destination = new FakeBulkDbConnection();
            var options = new CopyProgress { StartTime = DateTime.UtcNow };

            // Act
            DataDbConnection.CopyRows(source, destination, "Person", "Customer", null, 1000, false, null, ColumnMappingBehavior.Automatic, IdentityBehavior.KeepIdentity, null, null, options, null, null, null, null, null);
            DataDbConnection.CopyRows(source, destination, "Pet", "Animal", null, 1000, false, null, ColumnMappingBehavior.Automatic, IdentityBehavior.KeepIdentity, null, null, options, null, null, null, null, null);

            // Assert
            Assert.IsTrue(options.Items[0].Logs.All(l => l.Text.Contains("Customer")));
            Assert.IsTrue(options.Items[1].Logs.All(l => l.Text.Contains("Animal")));
            Assert.AreEqual(options.Items[0].Logs.Count + options.Items[1].Logs.Count, options.Logs.Count);
        }

        [TestMethod]
        public async Task TestCopyRowsAsyncOfMoreThanOneTableKeepsAnItemForEachTable()
        {
            // Setup
            var source = new FakeDbConnection { Data = FakeDbConnection.CreatePersons(3) };
            var destination = new FakeBulkDbConnection();
            var options = new CopyProgress { StartTime = DateTime.UtcNow };

            // Act
            await DataDbConnection.CopyRowsAsync(source, destination, "Person", "Customer", null, 1000, false, null, ColumnMappingBehavior.Automatic, IdentityBehavior.KeepIdentity, null, null, options, null, null, null, null, null, CancellationToken.None);
            await DataDbConnection.CopyRowsAsync(source, destination, "Pet", "Animal", null, 1000, false, null, ColumnMappingBehavior.Automatic, IdentityBehavior.KeepIdentity, null, null, options, null, null, null, null, null, CancellationToken.None);

            // Assert
            Assert.AreEqual(2, options.Items.Count);
            Assert.AreEqual(2, options.CompletedTableCount);
            Assert.AreEqual(6, options.RowCount);
            Assert.AreSame(options.Items[1], options.Current);
        }

        #endregion
    }
}
