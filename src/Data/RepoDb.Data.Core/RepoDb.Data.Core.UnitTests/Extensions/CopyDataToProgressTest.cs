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
        public void TestCopyDataToItemHasTheRowsAndTheBatchesOfTheBulkInsertAsOneBatch()
        {
            // Setup
            var source = new FakeDbConnection { Data = FakeDbConnection.CreatePersons(3) };

            // Act
            var item = source.CopyDataTo("Person", "Customer", new FakeBulkDbConnection(), batchSize: 2).Items.Single();

            // Assert
            Assert.AreEqual(3, item.RowCount);
            Assert.AreEqual(3L, item.Source.RowCount);
            Assert.AreEqual(1, item.Batches.Count);
            Assert.AreEqual(1, item.Batches[0].Number);
            Assert.AreEqual(3, item.Batches[0].RowCount);
        }

        [TestMethod]
        public void TestCopyDataToItemHasTheBatchesOfTheRowsThatAreInsertedInBatches()
        {
            // Setup
            var source = new FakeDbConnection { Data = FakeDbConnection.CreatePersons(5) };

            // Act
            var actual = source.CopyDataTo("Person", "Customer", new FakeDbConnection(), batchSize: 2);

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
            var actual = await source.CopyDataToAsync("Person", "Customer", new FakeDbConnection(), batchSize: 2);

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
            var item = source.CopyDataTo("Person", "Customer", new FakeDbConnection(), batchSize: 2).Items.Single();

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
            var actual = source.CopyDataTo("Person", "Customer", new FakeDbConnection(), batchSize: 2,
                progressCallback: p =>
                {
                    seen.Add(p);
                    totals.Add((p.RowCount, p.BatchCount, p.CurrentBatch.Number, p.CurrentBatch.RowCount));
                });

            // Assert
            Assert.IsTrue(seen.All(p => ReferenceEquals(p, actual)));
            CollectionAssert.AreEqual(new[] { (2, 1, 1, 2), (4, 2, 2, 2), (5, 3, 3, 1) }, totals);
        }

        [TestMethod]
        public void TestCopyDataToCallsTheCompletedCallbackWhenTheTableHasEndedAfterAllItsBatches()
        {
            // Setup
            var source = new FakeDbConnection { Data = FakeDbConnection.CreatePersons(5) };
            var events = new List<string>();
            var completed = new List<CopyProgressItem>();

            // Act
            var actual = source.CopyDataTo("Person", "Customer", new FakeDbConnection(), batchSize: 2,
                progressCallback: _ => events.Add("batch"),
                completedCallback: item =>
                {
                    events.Add("table");
                    completed.Add(item);
                });

            // Assert
            CollectionAssert.AreEqual(new[] { "batch", "batch", "batch", "table" }, events);
            Assert.AreEqual(1, completed.Count);
            Assert.AreSame(actual.Items[0], completed[0]);
            Assert.IsTrue(completed[0].EndTime >= completed[0].Batches.Last().EndTime);
        }

        [TestMethod]
        public async Task TestCopyDataToAsyncCallsTheCompletedCallbackWhenTheTableHasEnded()
        {
            // Setup
            var source = new FakeDbConnection { Data = FakeDbConnection.CreatePersons(5) };
            var events = new List<string>();

            // Act
            var actual = await source.CopyDataToAsync("Person", "Customer", new FakeDbConnection(), batchSize: 2,
                progressCallback: _ => events.Add("batch"),
                completedCallback: _ => events.Add("table"));

            // Assert
            CollectionAssert.AreEqual(new[] { "batch", "batch", "batch", "table" }, events);
            Assert.AreEqual(1, actual.CompletedTableCount);
        }

        [TestMethod]
        public void TestCopyDataToWithoutCallbacksStillRecordsTheItems()
        {
            // Setup
            var source = new FakeDbConnection { Data = FakeDbConnection.CreatePersons(3) };

            // Act
            var actual = source.CopyDataTo("Person", "Customer", new FakeBulkDbConnection(), progressCallback: null, completedCallback: null);

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
            Assert.AreEqual(CopyLogLevel.Info, item.Logs[0].Level);
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
            var completed = new List<CopyProgressItem>();

            // Act
            DataDbConnection.CopyRows(source, destination, "Person", "Customer", null, 1000, CopyColumnMappingsBehavior.Automatic, null, options, null, completed.Add, null, null, null, null);
            DataDbConnection.CopyRows(source, destination, "Pet", "Animal", null, 1000, CopyColumnMappingsBehavior.Automatic, null, options, null, completed.Add, null, null, null, null);

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
            Assert.AreEqual(2, completed.Count);
            Assert.AreSame(options.Items[0], completed[0]);
            Assert.AreSame(options.Items[1], completed[1]);
        }

        [TestMethod]
        public void TestCopyRowsOfMoreThanOneTableKeepsTheLogsOfEachTableApart()
        {
            // Setup
            var source = new FakeDbConnection { Data = FakeDbConnection.CreatePersons(3) };
            var destination = new FakeBulkDbConnection();
            var options = new CopyProgress { StartTime = DateTime.UtcNow };

            // Act
            DataDbConnection.CopyRows(source, destination, "Person", "Customer", null, 1000, CopyColumnMappingsBehavior.Automatic, null, options, null, null, null, null, null, null);
            DataDbConnection.CopyRows(source, destination, "Pet", "Animal", null, 1000, CopyColumnMappingsBehavior.Automatic, null, options, null, null, null, null, null, null);

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
            await DataDbConnection.CopyRowsAsync(source, destination, "Person", "Customer", null, 1000, CopyColumnMappingsBehavior.Automatic, null, options, null, null, null, null, null, null, CancellationToken.None);
            await DataDbConnection.CopyRowsAsync(source, destination, "Pet", "Animal", null, 1000, CopyColumnMappingsBehavior.Automatic, null, options, null, null, null, null, null, null, CancellationToken.None);

            // Assert
            Assert.AreEqual(2, options.Items.Count);
            Assert.AreEqual(2, options.CompletedTableCount);
            Assert.AreEqual(6, options.RowCount);
            Assert.AreSame(options.Items[1], options.Current);
        }

        #endregion
    }
}
