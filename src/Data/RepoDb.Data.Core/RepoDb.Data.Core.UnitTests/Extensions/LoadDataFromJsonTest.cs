#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RepoDb.Data.Core.UnitTests.CustomObjects;
using RepoDb.Data.Core.UnitTests.Fake.BulkOperations;
using RepoDb.Data.Models;
using RepoDb.DbSettings;

namespace RepoDb.Data.Core.UnitTests.Extensions
{
    [TestClass]
    public class LoadDataFromJsonTest
    {
        #region Helpers

        private readonly List<string> _files = new List<string>();

        private string Write(string text)
        {
            var path = Path.Combine(Path.GetTempPath(), "repodb_load_json_" + Guid.NewGuid().ToString("N") + ".json");
            File.WriteAllText(path, text, new UTF8Encoding(false));
            _files.Add(path);
            return path;
        }

        private static string Rows(int count) =>
            "[" + string.Join(",", Enumerable.Range(1, count).Select(i => "{\"Id\":" + i + ",\"Name\":\"Name " + i + "\",\"Birthday\":\"2026-01-02T03:04:05Z\"}")) + "]";

        private static Action<LoadProgress> Snapshot(List<(int Batch, long Loaded, long Total)> seen) =>
            p => seen.Add((p.BatchNumber, p.LoadedRowCount, p.TotalRowCount));

        [TestInitialize]
        public void Initialize()
        {
            DbSettingMapper.Add<FakeBulkDbConnection>(new FakeDbSetting(), true);
            DbHelperMapper.Add<FakeBulkDbConnection>(new FakeDbHelper(), true);
            FakeBulkOperations.Reset();
        }

        [TestCleanup]
        public void Cleanup()
        {
            foreach (var file in _files.Where(File.Exists))
            {
                File.Delete(file);
            }
            _files.Clear();
        }

        #endregion

        #region Arguments

        [TestMethod]
        public void ThrowExceptionOnLoadDataFromJsonIfTheConnectionIsNull()
        {
            // Setup
            var path = Write("[]");

            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => ((IDbConnection)null).LoadDataFromJson("Customer", path));
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("  ")]
        public void ThrowExceptionOnLoadDataFromJsonIfTheTableNameIsNullOrWhiteSpace(string tableName)
        {
            // Setup
            var path = Write("[]");

            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => new FakeDbConnection().LoadDataFromJson(tableName, path));
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("  ")]
        public void ThrowExceptionOnLoadDataFromJsonIfTheFileNameIsNullOrWhiteSpace(string filename)
        {
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => new FakeDbConnection().LoadDataFromJson("Customer", filename));
        }

        [TestMethod]
        public void ThrowExceptionOnLoadDataFromJsonIfTheFileDoesNotExist()
        {
            // Act/Assert
            Assert.Throws<FileNotFoundException>(() => new FakeDbConnection().LoadDataFromJson("Customer", Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N") + ".json")));
        }

        [TestMethod]
        public void ThrowExceptionOnLoadDataFromJsonIfTheCutoffSizeIsNegative()
        {
            // Setup
            var path = Write("[]");

            // Act/Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => new FakeDbConnection().LoadDataFromJson("Customer", path, cutoffSize: -1));
        }

        [DataTestMethod]
        [DataRow(0)]
        [DataRow(-5)]
        public void ThrowExceptionOnLoadDataFromJsonIfTheBatchBufferSizeIsLessThanOne(int size)
        {
            // Setup
            var path = Write("[]");

            // Act/Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => new FakeDbConnection().LoadDataFromJson("Customer", path, batchBufferSize: size));
        }

        [DataTestMethod]
        [DataRow(0)]
        [DataRow(-5)]
        public void ThrowExceptionOnLoadDataFromJsonIfTheBulkBufferSizeIsLessThanOne(int size)
        {
            // Setup
            var path = Write("[]");

            // Act/Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => new FakeDbConnection().LoadDataFromJson("Customer", path, bulkBufferSize: size));
        }

        [TestMethod]
        public async Task ThrowExceptionOnLoadDataFromJsonAsyncIfTheConnectionIsNull()
        {
            // Setup
            var path = Write("[]");

            // Act/Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => ((IDbConnection)null).LoadDataFromJsonAsync("Customer", path));
        }

        [TestMethod]
        public async Task ThrowExceptionOnLoadDataFromJsonAsyncIfTheFileDoesNotExist()
        {
            // Act/Assert
            await Assert.ThrowsAsync<FileNotFoundException>(() => new FakeDbConnection().LoadDataFromJsonAsync("Customer", Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N") + ".json")));
        }

        [TestMethod]
        public async Task ThrowExceptionOnLoadDataFromJsonAsyncIfTheBufferSizesAreInvalid()
        {
            // Setup
            var path = Write("[]");

            // Act/Assert
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => new FakeDbConnection().LoadDataFromJsonAsync("Customer", path, cutoffSize: -1));
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => new FakeDbConnection().LoadDataFromJsonAsync("Customer", path, batchBufferSize: 0));
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => new FakeDbConnection().LoadDataFromJsonAsync("Customer", path, bulkBufferSize: 0));
        }

        #endregion

        #region Batch Operations

        [TestMethod]
        public void TestLoadDataFromJsonBelowTheCutoffSizeInsertsTheRowsInBatches()
        {
            // Setup
            var path = Write(Rows(5));
            var destination = new FakeBulkDbConnection();
            var seen = new List<(int Batch, long Loaded, long Total)>();

            // Act
            var actual = destination.LoadDataFromJson("Customer", path, cutoffSize: 1000, batchBufferSize: 2, progressCallback: Snapshot(seen));

            // Assert
            Assert.AreEqual(0, FakeBulkOperations.Calls.Length);
            Assert.AreEqual(5, FakeBulkOperations.Statements.Count(s => s.CommandText.StartsWith("INSERT INTO", StringComparison.Ordinal)));
            Assert.AreEqual(5L, actual.TotalRowCount);
            Assert.AreEqual(5L, actual.LoadedRowCount);
            Assert.AreEqual(3, actual.BatchNumber);
            CollectionAssert.AreEqual(new[] { (0, 0L, 5L), (1, 2L, 5L), (2, 4L, 5L), (3, 5L, 5L), (3, 5L, 5L) }, seen);
        }

        [TestMethod]
        public void TestLoadDataFromJsonInsertsTheValuesConvertedIntoTheTypesOfTheColumns()
        {
            // Setup
            var path = Write("[{\"Id\":7,\"Name\":\"Seven\",\"Birthday\":\"2026-01-02T03:04:05Z\"}]");
            var destination = new FakeDbConnection();

            // Act
            destination.LoadDataFromJson("Customer", path);

            // Assert
            var execution = destination.Executions.Single(e => e.CommandText.StartsWith("INSERT INTO", StringComparison.Ordinal));
            var values = execution.Parameters.Values.ToArray();
            CollectionAssert.AreEquivalent(new object[] { 7, "Seven", new DateTime(2026, 1, 2, 3, 4, 5, DateTimeKind.Utc) }, values);
        }

        [TestMethod]
        public void TestLoadDataFromJsonCallsTheProgressCallbackWithTheSameInstanceThatItReturns()
        {
            // Setup
            var path = Write(Rows(3));
            var seen = new List<LoadProgress>();

            // Act
            var actual = new FakeDbConnection().LoadDataFromJson("Customer", path, batchBufferSize: 1, progressCallback: seen.Add);

            // Assert
            Assert.IsTrue(seen.Count >= 2);
            Assert.IsTrue(seen.All(p => ReferenceEquals(p, actual)));
        }

        [TestMethod]
        public void TestLoadDataFromJsonCallsTheCallbackAfterTheRowsAreCountedAndWhenItEnds()
        {
            // Setup
            var path = Write(Rows(2));
            var seen = new List<(int Batch, long Loaded, long Total)>();

            // Act
            new FakeDbConnection().LoadDataFromJson("Customer", path, batchBufferSize: 50, progressCallback: p => seen.Add((p.BatchNumber, p.LoadedRowCount, p.TotalRowCount)));

            // Assert
            CollectionAssert.AreEqual(new[] { (0, 0L, 2L), (1, 2L, 2L), (1, 2L, 2L) }, seen);
        }

        [TestMethod]
        public void TestLoadDataFromJsonSetsTheStartInUtc()
        {
            // Setup
            var path = Write(Rows(1));
            var before = DateTime.UtcNow;

            // Act
            var actual = new FakeDbConnection().LoadDataFromJson("Customer", path);

            // Assert
            Assert.AreEqual(DateTimeKind.Utc, actual.Start.Kind);
            Assert.IsTrue(actual.Start >= before);
            Assert.IsTrue(actual.Start <= DateTime.UtcNow);
        }

        [TestMethod]
        public void TestLoadDataFromJsonOfAFileWithoutRowsCallsTheCallbackTwiceAndInsertsNothing()
        {
            // Setup
            var path = Write("[]");
            var destination = new FakeBulkDbConnection();
            var seen = new List<(int Batch, long Loaded, long Total)>();

            // Act
            var actual = destination.LoadDataFromJson("Customer", path, progressCallback: p => seen.Add((p.BatchNumber, p.LoadedRowCount, p.TotalRowCount)));

            // Assert
            CollectionAssert.AreEqual(new[] { (0, 0L, 0L), (0, 0L, 0L) }, seen);
            Assert.AreEqual(0, FakeBulkOperations.Statements.Length);
            Assert.AreEqual(0, FakeBulkOperations.Calls.Length);
            Assert.AreEqual(0L, actual.LoadedRowCount);
        }

        [TestMethod]
        public void TestLoadDataFromJsonWithoutACallbackStillLoadsTheRows()
        {
            // Setup
            var path = Write(Rows(4));
            var destination = new FakeDbConnection();

            // Act
            var actual = destination.LoadDataFromJson("Customer", path, batchBufferSize: 3, progressCallback: null);

            // Assert
            Assert.AreEqual(4L, actual.LoadedRowCount);
            Assert.AreEqual(2, actual.BatchNumber);
        }

        [TestMethod]
        public void TestLoadDataFromJsonPassesTheCommandTimeoutOfTheBatchOperation()
        {
            // Setup
            var path = Write(Rows(2));
            var destination = new FakeDbConnection();

            // Act
            destination.LoadDataFromJson("Customer", path, commandTimeout: 33);

            // Assert
            Assert.IsTrue(destination.Executions.Where(e => e.Kind == "NonQuery").All(e => e.CommandTimeout == 33));
        }

        [TestMethod]
        public void TestLoadDataFromJsonIgnoresTheColumnsOfTheFileThatTheTableDoesNotHave()
        {
            // Setup
            var path = Write("[{\"Id\":1,\"Name\":\"A\",\"Unknown\":\"x\"},{\"Id\":2,\"Name\":\"B\",\"Unknown\":\"y\"}]");
            var destination = new FakeDbConnection();

            // Act
            var actual = destination.LoadDataFromJson("Customer", path);

            // Assert
            Assert.AreEqual(2L, actual.LoadedRowCount);
            var execution = destination.Executions.First(e => e.CommandText.StartsWith("INSERT INTO", StringComparison.Ordinal));
            Assert.AreEqual(2, execution.Parameters.Count);
            Assert.IsFalse(execution.CommandText.Contains("Unknown"));
        }

        [TestMethod]
        public void TestLoadDataFromJsonInsertsTheMissingValuesAsNull()
        {
            // Setup
            var path = Write("[{\"Id\":1,\"Name\":\"A\",\"Birthday\":null},{\"Id\":2,\"Name\":\"B\"}]");
            var destination = new FakeDbConnection();

            // Act
            var actual = destination.LoadDataFromJson("Customer", path);

            // Assert
            Assert.AreEqual(2L, actual.LoadedRowCount);
        }

        #endregion

        #region Bulk Operations

        [TestMethod]
        public void TestLoadDataFromJsonFromTheCutoffSizeUsesTheBulkInsertInBlocksOfTheBulkBufferSize()
        {
            // Setup
            var path = Write(Rows(10));
            var destination = new FakeBulkDbConnection();
            var seen = new List<(int Batch, long Loaded, long Total)>();

            // Act
            var actual = destination.LoadDataFromJson("Customer", path, cutoffSize: 5, batchBufferSize: 2, bulkBufferSize: 4, progressCallback: Snapshot(seen));

            // Assert
            var calls = FakeBulkOperations.Calls;
            CollectionAssert.AreEqual(new[] { 4, 4, 2 }, calls.Select(c => c.RowCount).ToArray());
            Assert.IsTrue(calls.All(c => c.TableName == "Customer" && c.Method == "BulkInsert"));
            Assert.AreEqual(0, FakeBulkOperations.Statements.Length);
            Assert.AreEqual(10L, actual.LoadedRowCount);
            Assert.AreEqual(3, actual.BatchNumber);
            CollectionAssert.AreEqual(new[] { (0, 0L, 10L), (1, 4L, 10L), (2, 8L, 10L), (3, 10L, 10L), (3, 10L, 10L) }, seen);
        }

        [TestMethod]
        public void TestLoadDataFromJsonWithTheSameNumberOfRowsAsTheCutoffSizeUsesTheBulkInsert()
        {
            // Setup
            var path = Write(Rows(5));

            // Act
            new FakeBulkDbConnection().LoadDataFromJson("Customer", path, cutoffSize: 5);

            // Assert
            Assert.AreEqual(1, FakeBulkOperations.Calls.Length);
        }

        [TestMethod]
        public void TestLoadDataFromJsonWithOneRowLessThanTheCutoffSizeUsesTheBatchInsert()
        {
            // Setup
            var path = Write(Rows(4));

            // Act
            new FakeBulkDbConnection().LoadDataFromJson("Customer", path, cutoffSize: 5);

            // Assert
            Assert.AreEqual(0, FakeBulkOperations.Calls.Length);
        }

        [TestMethod]
        public void TestLoadDataFromJsonWithACutoffSizeOfZeroAlwaysUsesTheBulkInsert()
        {
            // Setup
            var path = Write(Rows(1));

            // Act
            new FakeBulkDbConnection().LoadDataFromJson("Customer", path, cutoffSize: 0);

            // Assert
            Assert.AreEqual(1, FakeBulkOperations.Calls.Length);
        }

        [TestMethod]
        public void TestLoadDataFromJsonUsesTheBatchInsertIfTheProviderHasNoBulkInsert()
        {
            // Setup
            var path = Write(Rows(10));
            var destination = new FakeDbConnection();

            // Act
            var actual = destination.LoadDataFromJson("Customer", path, cutoffSize: 5, batchBufferSize: 4, bulkBufferSize: 100);

            // Assert
            Assert.AreEqual(0, FakeBulkOperations.Calls.Length);
            Assert.AreEqual(10L, actual.LoadedRowCount);
            Assert.AreEqual(3, actual.BatchNumber);
        }

        [TestMethod]
        public void TestLoadDataFromJsonPassesTheArgumentsToTheBulkInsert()
        {
            // Setup
            var path = Write(Rows(6));
            var transaction = new Mock<IDbTransaction>().Object;

            // Act
            new FakeBulkDbConnection().LoadDataFromJson("Customer", path, cutoffSize: 1, bulkBufferSize: 6, commandTimeout: 12, transaction: transaction);

            // Assert
            var call = FakeBulkOperations.Calls.Single();
            Assert.AreEqual(6, call.BatchSize);
            Assert.AreEqual(12, call.Timeout);
            Assert.AreSame(transaction, call.Transaction);
            Assert.AreEqual(DataTraceKeys.LoadDataFromJson, call.TraceKey);
        }

        [TestMethod]
        public void TestLoadDataFromJsonReadsTheBulkRowsWithTheTypesOfTheColumns()
        {
            // Setup
            var path = Write(Rows(3));

            // Act
            var actual = new FakeBulkDbConnection().LoadDataFromJson("Customer", path, cutoffSize: 1);

            // Assert
            Assert.AreEqual(3L, actual.LoadedRowCount);
            Assert.AreEqual(3, FakeBulkOperations.Calls.Single().RowCount);
        }

        #endregion

        #region Shapes

        [TestMethod]
        public void TestLoadDataFromJsonLoadsAnObjectThatHoldsTheRows()
        {
            // Setup
            var path = Write("{\"meta\":{\"x\":1},\"rows\":" + Rows(3) + "}");

            // Act
            var actual = new FakeDbConnection().LoadDataFromJson("Customer", path);

            // Assert
            Assert.AreEqual(3L, actual.TotalRowCount);
            Assert.AreEqual(3L, actual.LoadedRowCount);
        }

        [TestMethod]
        public void TestLoadDataFromJsonLoadsASequenceOfObjects()
        {
            // Setup
            var path = Write(string.Join("\n", Enumerable.Range(1, 4).Select(i => "{\"Id\":" + i + ",\"Name\":\"Name " + i + "\"}")));

            // Act
            var actual = new FakeDbConnection().LoadDataFromJson("Customer", path, batchBufferSize: 3);

            // Assert
            Assert.AreEqual(4L, actual.TotalRowCount);
            Assert.AreEqual(4L, actual.LoadedRowCount);
            Assert.AreEqual(2, actual.BatchNumber);
        }

        [TestMethod]
        public void TestLoadDataFromJsonLoadsAValueWithItsType()
        {
            // Setup
            var path = Write("[{\"Id\":{\"type\":\"Int32\",\"value\":9},\"Name\":{\"type\":\"String\",\"value\":\"Nine\"}}]");
            var destination = new FakeDbConnection();

            // Act
            destination.LoadDataFromJson("Customer", path);

            // Assert
            var values = destination.Executions.Single(e => e.CommandText.StartsWith("INSERT INTO", StringComparison.Ordinal)).Parameters.Values.ToArray();
            CollectionAssert.AreEquivalent(new object[] { 9, "Nine" }, values);
        }

        #endregion

        #region Errors

        [TestMethod]
        public void ThrowExceptionOnLoadDataFromJsonIfNoColumnOfTheFileMatchesAColumnOfTheTable()
        {
            // Setup
            var path = Write("[{\"A\":1,\"B\":2}]");

            // Act/Assert
            var exception = Assert.Throws<InvalidOperationException>(() => new FakeDbConnection().LoadDataFromJson("Customer", path));
            StringAssert.Contains(exception.Message, "Customer", StringComparison.Ordinal);
        }

        [TestMethod]
        public void ThrowExceptionOnLoadDataFromJsonIfAValueCannotBeConvertedIntoTheTypeOfItsColumn()
        {
            // Setup
            var path = Write("[{\"Id\":1,\"Name\":\"A\"},{\"Id\":1.5,\"Name\":\"B\"}]");

            // Act/Assert
            var exception = Assert.Throws<FormatException>(() => new FakeDbConnection().LoadDataFromJson("Customer", path));
            StringAssert.Contains(exception.Message, "Row 2", StringComparison.Ordinal);
            StringAssert.Contains(exception.Message, "Id", StringComparison.Ordinal);
        }

        [TestMethod]
        public void ThrowExceptionOnLoadDataFromJsonIfTheFileIsNotValidJson()
        {
            // Setup
            var path = Write("[{\"Id\":1},");

            // Act/Assert
            Assert.Throws<FormatException>(() => new FakeDbConnection().LoadDataFromJson("Customer", path));
        }

        [TestMethod]
        public void TestLoadDataFromJsonStopsAtTheBatchThatFailsAndKeepsTheProgressOfTheOnesThatSucceeded()
        {
            // Setup
            var path = Write(Rows(6));
            var seen = new List<LoadProgress>();
            var destination = new FakeDbConnection();
            var calls = 0;

            // Act/Assert
            Assert.Throws<InvalidOperationException>(() =>
                destination.LoadDataFromJson("Customer", path, batchBufferSize: 2, progressCallback: p =>
                {
                    seen.Add(p);
                    if (++calls == 3)
                    {
                        throw new InvalidOperationException("stop");
                    }
                }));
            Assert.AreEqual(3, seen.Count);
            Assert.AreEqual(2, seen.Last().BatchNumber);
            Assert.AreEqual(4L, seen.Last().LoadedRowCount);
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestLoadDataFromJsonAsyncBelowTheCutoffSizeInsertsTheRowsInBatches()
        {
            // Setup
            var path = Write(Rows(5));
            var destination = new FakeBulkDbConnection();
            var seen = new List<(int Batch, long Loaded, long Total)>();

            // Act
            var actual = await destination.LoadDataFromJsonAsync("Customer", path, cutoffSize: 1000, batchBufferSize: 2, progressCallback: Snapshot(seen));

            // Assert
            Assert.AreEqual(0, FakeBulkOperations.Calls.Length);
            Assert.AreEqual(5L, actual.LoadedRowCount);
            Assert.AreEqual(3, actual.BatchNumber);
            CollectionAssert.AreEqual(new[] { (0, 0L, 5L), (1, 2L, 5L), (2, 4L, 5L), (3, 5L, 5L), (3, 5L, 5L) }, seen);
        }

        [TestMethod]
        public async Task TestLoadDataFromJsonAsyncFromTheCutoffSizeUsesTheBulkInsert()
        {
            // Setup
            var path = Write(Rows(10));
            var destination = new FakeBulkDbConnection();

            // Act
            var actual = await destination.LoadDataFromJsonAsync("Customer", path, cutoffSize: 5, bulkBufferSize: 4);

            // Assert
            var calls = FakeBulkOperations.Calls;
            CollectionAssert.AreEqual(new[] { 4, 4, 2 }, calls.Select(c => c.RowCount).ToArray());
            Assert.IsTrue(calls.All(c => c.Method == "BulkInsertAsync"));
            Assert.AreEqual(10L, actual.LoadedRowCount);
            Assert.AreEqual(3, actual.BatchNumber);
        }

        [TestMethod]
        public async Task TestLoadDataFromJsonAsyncLoadsASequenceOfObjects()
        {
            // Setup
            var path = Write(string.Join("\n", Enumerable.Range(1, 4).Select(i => "{\"Id\":" + i + ",\"Name\":\"Name " + i + "\"}")));

            // Act
            var actual = await new FakeDbConnection().LoadDataFromJsonAsync("Customer", path, batchBufferSize: 3);

            // Assert
            Assert.AreEqual(4L, actual.LoadedRowCount);
            Assert.AreEqual(2, actual.BatchNumber);
        }

        [TestMethod]
        public async Task TestLoadDataFromJsonAsyncOfAFileWithoutRowsCallsTheCallbackTwice()
        {
            // Setup
            var path = Write("[]");
            var count = 0;

            // Act
            var actual = await new FakeDbConnection().LoadDataFromJsonAsync("Customer", path, progressCallback: _ => count++);

            // Assert
            Assert.AreEqual(2, count);
            Assert.AreEqual(0L, actual.TotalRowCount);
        }

        [TestMethod]
        public async Task ThrowExceptionOnLoadDataFromJsonAsyncIfNoColumnOfTheFileMatchesAColumnOfTheTable()
        {
            // Setup
            var path = Write("[{\"A\":1}]");

            // Act/Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => new FakeDbConnection().LoadDataFromJsonAsync("Customer", path));
        }

        [TestMethod]
        public async Task ThrowExceptionOnLoadDataFromJsonAsyncIfAValueCannotBeConvertedIntoTheTypeOfItsColumn()
        {
            // Setup
            var path = Write("[{\"Id\":\"abc\"}]");

            // Act/Assert
            await Assert.ThrowsAsync<FormatException>(() => new FakeDbConnection().LoadDataFromJsonAsync("Customer", path));
        }

        [TestMethod]
        public async Task ThrowExceptionOnLoadDataFromJsonAsyncIfTheTokenIsCancelled()
        {
            // Setup
            var path = Write(Rows(5));
            using (var source = new CancellationTokenSource())
            {
                source.Cancel();

                // Act/Assert
                await Assert.ThrowsAsync<OperationCanceledException>(() => new FakeDbConnection().LoadDataFromJsonAsync("Customer", path, cancellationToken: source.Token));
            }
        }

        #endregion
    }
}
