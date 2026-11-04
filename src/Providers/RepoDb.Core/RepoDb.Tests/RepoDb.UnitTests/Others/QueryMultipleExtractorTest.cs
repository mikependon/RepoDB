#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System.Collections.Generic;
using System.Data.Common;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq.Protected;
using Moq;
using RepoDb.Interfaces;
using RepoDb.Reflection;

namespace RepoDb.UnitTests.Others
{
    [TestClass]
    public class QueryMultipleExtractorTest
    {
        [TestMethod]
        public void TestUntypedScalar()
        {
            // Setup
            using var table = CreateTable(42);
            using var reader = table.CreateDataReader();
            using var extractor = new QueryMultipleExtractor(reader: reader);

            // Act
            var actual = extractor.Scalar();

            // Assert
            Assert.AreEqual(42, actual);
            Assert.AreEqual(-1, extractor.Position);
        }

        [TestMethod]
        public void TestUntypedScalarPreservesPosition()
        {
            // Setup
            using var table = CreateTable(42);
            using var reader = table.CreateDataReader();
            using var extractor = new QueryMultipleExtractor(reader: reader);

            // Act
            var actual = extractor.Scalar(false);

            // Assert
            Assert.AreEqual(42, actual);
            Assert.AreEqual(0, extractor.Position);
        }

        [TestMethod]
        public void TestUntypedScalarEmptyResult()
        {
            // Setup
            using var table = CreateTable();
            using var reader = table.CreateDataReader();
            using var extractor = new QueryMultipleExtractor(reader: reader);

            // Act
            var actual = extractor.Scalar();

            // Assert
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void TestUntypedScalarUsesCache()
        {
            // Setup
            using var table = CreateTable(99);
            using var reader = table.CreateDataReader();
            var cache = new Mock<ICache>();
            cache.Setup(item => item.Get<object[]>("result", false))
                .Returns(new CacheItem<object[]>("result", new object[] { 42 }));
            using var extractor = new QueryMultipleExtractor(reader: reader, cacheKey: "result", cache: cache.Object);

            // Act
            var actual = extractor.Scalar(false);

            // Assert
            Assert.AreEqual(42, actual);
            Assert.IsTrue(reader.Read());
        }

        [TestMethod]
        public void TestTypedScalar()
        {
            // Setup
            using var table = CreateTable(42);
            using var reader = table.CreateDataReader();
            using var extractor = new QueryMultipleExtractor(reader: reader);

            // Act
            var actual = extractor.Scalar<int>();

            // Assert
            Assert.AreEqual(42, actual);
            Assert.AreEqual(-1, extractor.Position);
        }

        [TestMethod]
        public void TestTypedScalarPreservesPosition()
        {
            // Setup
            using var table = CreateTable(42);
            using var reader = table.CreateDataReader();
            using var extractor = new QueryMultipleExtractor(reader: reader);

            // Act
            var actual = extractor.Scalar<int>(false);

            // Assert
            Assert.AreEqual(42, actual);
            Assert.AreEqual(0, extractor.Position);
        }

        [TestMethod]
        public void TestTypedScalarEmptyResult()
        {
            // Setup
            using var table = CreateTable();
            using var reader = table.CreateDataReader();
            using var extractor = new QueryMultipleExtractor(reader: reader);

            // Act
            var actual = extractor.Scalar<int>();

            // Assert
            Assert.AreEqual(0, actual);
        }

        [TestMethod]
        public void TestTypedScalarUsesCache()
        {
            // Setup
            using var table = CreateTable(99);
            using var reader = table.CreateDataReader();
            var cache = new Mock<ICache>();
            cache.Setup(item => item.Get<object[]>("result", false))
                .Returns(new CacheItem<object[]>("result", new object[] { 42 }));
            using var extractor = new QueryMultipleExtractor(reader: reader, cacheKey: "result", cache: cache.Object);

            // Act
            var actual = extractor.Scalar<int>(false);

            // Assert
            Assert.AreEqual(42, actual);
            Assert.IsTrue(reader.Read());
        }

        [TestMethod]
        public void TestTypedExtract()
        {
            // Setup
            using var table = CreateTable(42, 43);
            using var reader = table.CreateDataReader();
            using var extractor = new QueryMultipleExtractor(reader: reader);

            // Act
            var actual = (extractor.Extract<Row>()).ToList();

            // Assert
            Assert.AreSequenceEqual(new[] { 42, 43 }, actual.Select(item => item.Value));
        }

        [TestMethod]
        public void TestTypedExtractPreservesPosition()
        {
            // Setup
            using var table = CreateTable(42, 43);
            using var reader = table.CreateDataReader();
            using var extractor = new QueryMultipleExtractor(reader: reader);

            // Act
            var actual = (extractor.Extract<Row>(false)).ToList();

            // Assert
            Assert.HasCount(2, actual);
            Assert.AreEqual(0, extractor.Position);
        }

        [TestMethod]
        public void TestTypedExtractEmptyResult()
        {
            // Setup
            using var table = CreateTable();
            using var reader = table.CreateDataReader();
            using var extractor = new QueryMultipleExtractor(reader: reader);

            // Act
            var actual = (extractor.Extract<Row>()).ToList();

            // Assert
            Assert.IsEmpty(actual);
        }

        [TestMethod]
        public void TestTypedExtractUsesCache()
        {
            // Setup
            using var table = CreateTable(99);
            using var reader = table.CreateDataReader();
            var expected = new List<Row> { new Row { Value = 42 } };
            var cache = new Mock<ICache>();
            cache.Setup(item => item.Get<object[]>("result", false))
                .Returns(new CacheItem<object[]>("result", new object[] { expected }));
            using var extractor = new QueryMultipleExtractor(reader: reader, cacheKey: "result", cache: cache.Object);

            // Act
            var actual = extractor.Extract<Row>(false);

            // Assert
            Assert.AreSame(expected, actual);
            Assert.IsTrue(reader.Read());
        }

        [TestMethod]
        public void TestDynamicExtract()
        {
            // Setup
            using var table = CreateTable(42, 43);
            using var reader = table.CreateDataReader();
            using var extractor = new QueryMultipleExtractor(reader: reader);

            // Act
            var actual = (extractor.Extract()).ToList();

            // Assert
            Assert.AreSequenceEqual(new[] { 42, 43 }, actual.Select(item => (int)((IDictionary<string, object>)item)["Value"]));
        }

        [TestMethod]
        public void TestDynamicExtractPreservesPosition()
        {
            // Setup
            using var table = CreateTable(42, 43);
            using var reader = table.CreateDataReader();
            using var extractor = new QueryMultipleExtractor(reader: reader);

            // Act
            var actual = (extractor.Extract(false)).ToList();

            // Assert
            Assert.HasCount(2, actual);
            Assert.AreEqual(0, extractor.Position);
        }

        [TestMethod]
        public void TestDynamicExtractEmptyResult()
        {
            // Setup
            using var table = CreateTable();
            using var reader = table.CreateDataReader();
            using var extractor = new QueryMultipleExtractor(reader: reader);

            // Act
            var actual = (extractor.Extract()).ToList();

            // Assert
            Assert.IsEmpty(actual);
        }

        [TestMethod]
        public void TestDynamicExtractUsesCache()
        {
            // Setup
            using var table = CreateTable(99);
            using var reader = table.CreateDataReader();
            var expected = new List<dynamic> { new Dictionary<string, object>(StringComparer.Ordinal) { ["Value"] = 42 } };
            var cache = new Mock<ICache>();
            cache.Setup(item => item.Get<object[]>("result", false))
                .Returns(new CacheItem<object[]>("result", new object[] { expected }));
            using var extractor = new QueryMultipleExtractor(reader: reader, cacheKey: "result", cache: cache.Object);

            // Act
            var actual = extractor.Extract(false);

            // Assert
            Assert.AreSame(expected, actual);
            Assert.IsTrue(reader.Read());
        }

        [TestMethod]
        public void TestNextResultAdvancesPosition()
        {
            // Setup
            using var first = CreateTable(42);
            using var second = CreateTable(43);
            using var reader = new DataTableReader(new[] { first, second });
            using var extractor = new QueryMultipleExtractor(reader: reader);

            // Act
            var actual = extractor.NextResult();

            // Assert
            Assert.IsTrue(actual);
            Assert.AreEqual(1, extractor.Position);
        }

        [TestMethod]
        public void TestNextResultMarksEnd()
        {
            // Setup
            using var first = CreateTable(42);
            using var second = CreateTable(43);
            using var reader = new DataTableReader(new[] { first, second });
            using var extractor = new QueryMultipleExtractor(reader: reader);
            extractor.NextResult();

            // Act
            var actual = extractor.NextResult();

            // Assert
            Assert.IsFalse(actual);
            Assert.AreEqual(-1, extractor.Position);
        }

        [TestMethod]
        public void TestScalarReadsSuccessiveResults()
        {
            // Setup
            using var first = CreateTable(42);
            using var second = CreateTable(43);
            using var reader = new DataTableReader(new[] { first, second });
            using var extractor = new QueryMultipleExtractor(reader: reader);
            extractor.Scalar<int>();

            // Act
            var actual = extractor.Scalar<int>();

            // Assert
            Assert.AreEqual(43, actual);
        }

        [TestMethod]
        public void TestScalarReadsWhenCacheIsMissing()
        {
            // Setup
            using var table = CreateTable(42);
            using var reader = table.CreateDataReader();
            var cache = new Mock<ICache>();
            using var extractor = new QueryMultipleExtractor(reader: reader, cacheKey: "result", cache: cache.Object);

            // Act
            var actual = extractor.Scalar<int>(false);

            // Assert
            Assert.AreEqual(42, actual);
        }

        [TestMethod]
        public void TestScalarReadsWhenCacheIsEmpty()
        {
            // Setup
            using var table = CreateTable(42);
            using var reader = table.CreateDataReader();
            var cache = new Mock<ICache>();
            var cached = new CacheItem<object[]>("result", Array.Empty<object>());
            cache.Setup(item => item.Get<object[]>("result", false)).Returns(cached);
            using var extractor = new QueryMultipleExtractor(reader: reader, cacheKey: "result", cache: cache.Object);

            // Act
            var actual = extractor.Scalar<int>(false);

            // Assert
            Assert.AreEqual(42, actual);
            Assert.AreEqual(42, cached.Value.Single());
        }

        [TestMethod]
        public void TestScalarReadsWhenCacheIsAbsent()
        {
            // Setup
            using var table = CreateTable(42);
            using var reader = table.CreateDataReader();
            var cache = new Mock<ICache>();
            using var extractor = new QueryMultipleExtractor(reader: reader, cacheKey: "result", cache: null);

            // Act
            var actual = extractor.Scalar<int>(false);

            // Assert
            Assert.AreEqual(42, actual);
        }

        [TestMethod]
        public async Task TestUntypedScalarAsync()
        {
            // Setup
            using var table = CreateTable(42);
            using var reader = table.CreateDataReader();
            using var extractor = new QueryMultipleExtractor(reader: reader);

            // Act
            var actual = await extractor.ScalarAsync();

            // Assert
            Assert.AreEqual(42, actual);
            Assert.AreEqual(-1, extractor.Position);
        }

        [TestMethod]
        public async Task TestUntypedScalarPreservesPositionAsync()
        {
            // Setup
            using var table = CreateTable(42);
            using var reader = table.CreateDataReader();
            using var extractor = new QueryMultipleExtractor(reader: reader);

            // Act
            var actual = await extractor.ScalarAsync(false);

            // Assert
            Assert.AreEqual(42, actual);
            Assert.AreEqual(0, extractor.Position);
        }

        [TestMethod]
        public async Task TestUntypedScalarEmptyResultAsync()
        {
            // Setup
            using var table = CreateTable();
            using var reader = table.CreateDataReader();
            using var extractor = new QueryMultipleExtractor(reader: reader);

            // Act
            var actual = await extractor.ScalarAsync();

            // Assert
            Assert.IsNull(actual);
        }

        [TestMethod]
        public async Task TestUntypedScalarUsesCacheAsync()
        {
            // Setup
            using var table = CreateTable(99);
            using var reader = table.CreateDataReader();
            var cache = new Mock<ICache>();
            cache.Setup(item => item.Get<object[]>("result", false))
                .Returns(new CacheItem<object[]>("result", new object[] { 42 }));
            using var extractor = new QueryMultipleExtractor(reader: reader, cacheKey: "result", cache: cache.Object);

            // Act
            var actual = await extractor.ScalarAsync(false);

            // Assert
            Assert.AreEqual(42, actual);
            Assert.IsTrue(reader.Read());
        }

        [TestMethod]
        public async Task TestTypedScalarAsync()
        {
            // Setup
            using var table = CreateTable(42);
            using var reader = table.CreateDataReader();
            using var extractor = new QueryMultipleExtractor(reader: reader);

            // Act
            var actual = await extractor.ScalarAsync<int>();

            // Assert
            Assert.AreEqual(42, actual);
            Assert.AreEqual(-1, extractor.Position);
        }

        [TestMethod]
        public async Task TestTypedScalarPreservesPositionAsync()
        {
            // Setup
            using var table = CreateTable(42);
            using var reader = table.CreateDataReader();
            using var extractor = new QueryMultipleExtractor(reader: reader);

            // Act
            var actual = await extractor.ScalarAsync<int>(false);

            // Assert
            Assert.AreEqual(42, actual);
            Assert.AreEqual(0, extractor.Position);
        }

        [TestMethod]
        public async Task TestTypedScalarEmptyResultAsync()
        {
            // Setup
            using var table = CreateTable();
            using var reader = table.CreateDataReader();
            using var extractor = new QueryMultipleExtractor(reader: reader);

            // Act
            var actual = await extractor.ScalarAsync<int>();

            // Assert
            Assert.AreEqual(0, actual);
        }

        [TestMethod]
        public async Task TestTypedScalarUsesCacheAsync()
        {
            // Setup
            using var table = CreateTable(99);
            using var reader = table.CreateDataReader();
            var cache = new Mock<ICache>();
            cache.Setup(item => item.Get<object[]>("result", false))
                .Returns(new CacheItem<object[]>("result", new object[] { 42 }));
            using var extractor = new QueryMultipleExtractor(reader: reader, cacheKey: "result", cache: cache.Object);

            // Act
            var actual = await extractor.ScalarAsync<int>(false);

            // Assert
            Assert.AreEqual(42, actual);
            Assert.IsTrue(reader.Read());
        }

        [TestMethod]
        public async Task TestTypedExtractAsync()
        {
            // Setup
            using var table = CreateTable(42, 43);
            using var reader = table.CreateDataReader();
            using var extractor = new QueryMultipleExtractor(reader: reader);

            // Act
            var actual = (await extractor.ExtractAsync<Row>()).ToList();

            // Assert
            Assert.AreSequenceEqual(new[] { 42, 43 }, actual.Select(item => item.Value));
        }

        [TestMethod]
        public async Task TestTypedExtractPreservesPositionAsync()
        {
            // Setup
            using var table = CreateTable(42, 43);
            using var reader = table.CreateDataReader();
            using var extractor = new QueryMultipleExtractor(reader: reader);

            // Act
            var actual = (await extractor.ExtractAsync<Row>(false)).ToList();

            // Assert
            Assert.HasCount(2, actual);
            Assert.AreEqual(0, extractor.Position);
        }

        [TestMethod]
        public async Task TestTypedExtractEmptyResultAsync()
        {
            // Setup
            using var table = CreateTable();
            using var reader = table.CreateDataReader();
            using var extractor = new QueryMultipleExtractor(reader: reader);

            // Act
            var actual = (await extractor.ExtractAsync<Row>()).ToList();

            // Assert
            Assert.IsEmpty(actual);
        }

        [TestMethod]
        public async Task TestTypedExtractUsesCacheAsync()
        {
            // Setup
            using var table = CreateTable(99);
            using var reader = table.CreateDataReader();
            var expected = new List<Row> { new Row { Value = 42 } };
            var cache = new Mock<ICache>();
            cache.Setup(item => item.Get<object[]>("result", false))
                .Returns(new CacheItem<object[]>("result", new object[] { expected }));
            using var extractor = new QueryMultipleExtractor(reader: reader, cacheKey: "result", cache: cache.Object);

            // Act
            var actual = await extractor.ExtractAsync<Row>(false);

            // Assert
            Assert.AreSame(expected, actual);
            Assert.IsTrue(reader.Read());
        }

        [TestMethod]
        public async Task TestDynamicExtractAsync()
        {
            // Setup
            using var table = CreateTable(42, 43);
            using var reader = table.CreateDataReader();
            using var extractor = new QueryMultipleExtractor(reader: reader);

            // Act
            var actual = (await extractor.ExtractAsync()).ToList();

            // Assert
            Assert.AreSequenceEqual(new[] { 42, 43 }, actual.Select(item => (int)((IDictionary<string, object>)item)["Value"]));
        }

        [TestMethod]
        public async Task TestDynamicExtractPreservesPositionAsync()
        {
            // Setup
            using var table = CreateTable(42, 43);
            using var reader = table.CreateDataReader();
            using var extractor = new QueryMultipleExtractor(reader: reader);

            // Act
            var actual = (await extractor.ExtractAsync(false)).ToList();

            // Assert
            Assert.HasCount(2, actual);
            Assert.AreEqual(0, extractor.Position);
        }

        [TestMethod]
        public async Task TestDynamicExtractEmptyResultAsync()
        {
            // Setup
            using var table = CreateTable();
            using var reader = table.CreateDataReader();
            using var extractor = new QueryMultipleExtractor(reader: reader);

            // Act
            var actual = (await extractor.ExtractAsync()).ToList();

            // Assert
            Assert.IsEmpty(actual);
        }

        [TestMethod]
        public async Task TestDynamicExtractUsesCacheAsync()
        {
            // Setup
            using var table = CreateTable(99);
            using var reader = table.CreateDataReader();
            var expected = new List<dynamic> { new Dictionary<string, object>(StringComparer.Ordinal) { ["Value"] = 42 } };
            var cache = new Mock<ICache>();
            cache.Setup(item => item.Get<object[]>("result", false))
                .Returns(new CacheItem<object[]>("result", new object[] { expected }));
            using var extractor = new QueryMultipleExtractor(reader: reader, cacheKey: "result", cache: cache.Object);

            // Act
            var actual = await extractor.ExtractAsync(false);

            // Assert
            Assert.AreSame(expected, actual);
            Assert.IsTrue(reader.Read());
        }

        [TestMethod]
        public async Task TestNextResultAdvancesPositionAsync()
        {
            // Setup
            using var first = CreateTable(42);
            using var second = CreateTable(43);
            using var reader = new DataTableReader(new[] { first, second });
            using var extractor = new QueryMultipleExtractor(reader: reader);

            // Act
            var actual = await extractor.NextResultAsync();

            // Assert
            Assert.IsTrue(actual);
            Assert.AreEqual(1, extractor.Position);
        }

        [TestMethod]
        public async Task TestNextResultMarksEndAsync()
        {
            // Setup
            using var first = CreateTable(42);
            using var second = CreateTable(43);
            using var reader = new DataTableReader(new[] { first, second });
            using var extractor = new QueryMultipleExtractor(reader: reader);
            await extractor.NextResultAsync();

            // Act
            var actual = await extractor.NextResultAsync();

            // Assert
            Assert.IsFalse(actual);
            Assert.AreEqual(-1, extractor.Position);
        }

        [TestMethod]
        public async Task TestScalarReadsSuccessiveResultsAsync()
        {
            // Setup
            using var first = CreateTable(42);
            using var second = CreateTable(43);
            using var reader = new DataTableReader(new[] { first, second });
            using var extractor = new QueryMultipleExtractor(reader: reader);
            await extractor.ScalarAsync<int>();

            // Act
            var actual = await extractor.ScalarAsync<int>();

            // Assert
            Assert.AreEqual(43, actual);
        }

        [TestMethod]
        public async Task TestScalarReadsWhenCacheIsMissingAsync()
        {
            // Setup
            using var table = CreateTable(42);
            using var reader = table.CreateDataReader();
            var cache = new Mock<ICache>();
            using var extractor = new QueryMultipleExtractor(reader: reader, cacheKey: "result", cache: cache.Object);

            // Act
            var actual = await extractor.ScalarAsync<int>(false);

            // Assert
            Assert.AreEqual(42, actual);
        }

        [TestMethod]
        public async Task TestScalarReadsWhenCacheIsEmptyAsync()
        {
            // Setup
            using var table = CreateTable(42);
            using var reader = table.CreateDataReader();
            var cache = new Mock<ICache>();
            var cached = new CacheItem<object[]>("result", Array.Empty<object>());
            cache.Setup(item => item.Get<object[]>("result", false)).Returns(cached);
            using var extractor = new QueryMultipleExtractor(reader: reader, cacheKey: "result", cache: cache.Object);

            // Act
            var actual = await extractor.ScalarAsync<int>(false);

            // Assert
            Assert.AreEqual(42, actual);
            Assert.AreEqual(42, cached.Value.Single());
        }

        [TestMethod]
        public async Task TestScalarReadsWhenCacheIsAbsentAsync()
        {
            // Setup
            using var table = CreateTable(42);
            using var reader = table.CreateDataReader();
            var cache = new Mock<ICache>();
            using var extractor = new QueryMultipleExtractor(reader: reader, cacheKey: "result", cache: null);

            // Act
            var actual = await extractor.ScalarAsync<int>(false);

            // Assert
            Assert.AreEqual(42, actual);
        }

        [TestMethod]
        public void TestExtractStoresAllResultsInCache()
        {
            // Setup
            using var first = CreateTable(42);
            using var second = CreateTable(43);
            using var reader = new DataTableReader(new[] { first, second });
            var cache = new Mock<ICache>();
            var cached = new CacheItem<object[]>("result", Array.Empty<object>());
            cache.Setup(item => item.Get<object[]>("result", false)).Returns(cached);
            using var extractor = new QueryMultipleExtractor(reader: reader, cacheKey: "result", cache: cache.Object);
            extractor.Extract<Row>();

            // Act
            extractor.Extract<Row>();

            // Assert
            Assert.HasCount(2, cached.Value);
            Assert.AreEqual(43, ((IEnumerable<Row>)cached.Value[1]).Single().Value);
        }

        [TestMethod]
        public void TestDisposeBorrowedConnection()
        {
            // Setup
            using var table = CreateTable(42);
            using var reader = table.CreateDataReader();
            var connection = new Mock<DbConnection>();
            var extractor = new QueryMultipleExtractor(connection: connection.Object, reader: reader, isDisposeConnection: false);

            // Act
            extractor.Dispose();

            // Assert
            Assert.IsTrue(reader.IsClosed);
            connection.Protected().Verify("Dispose", Times.Never(), new object[] { true });
        }

        [TestMethod]
        public void TestDisposeOwnedConnection()
        {
            // Setup
            using var table = CreateTable(42);
            using var reader = table.CreateDataReader();
            var connection = new Mock<DbConnection>();
            var extractor = new QueryMultipleExtractor(connection: connection.Object, reader: reader, isDisposeConnection: true);

            // Act
            extractor.Dispose();

            // Assert
            Assert.IsTrue(reader.IsClosed);
            connection.Protected().Verify("Dispose", Times.Once(), new object[] { true });
        }

        [TestMethod]
        public void TestDisposeWithoutResources()
        {
            // Setup
            var extractor = new QueryMultipleExtractor();

            // Act
            extractor.Dispose();

            // Assert
            Assert.AreEqual(0, extractor.Position);
        }

        [TestMethod]
        public async Task TestScalarAsyncObservesCancellation()
        {
            // Setup
            using var table = CreateTable(42);
            using var reader = table.CreateDataReader();
            using var cancellation = new CancellationTokenSource();
            cancellation.Cancel();
            using var extractor = new QueryMultipleExtractor(reader: reader, cancellationToken: cancellation.Token);

            // Act
            Func<Task> act = async () => await extractor.ScalarAsync<int>().ConfigureAwait(false);

            // Assert
            await Assert.ThrowsAsync<OperationCanceledException>(act).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task TestExtractAsyncObservesCancellation()
        {
            // Setup
            using var table = CreateTable(42);
            using var reader = table.CreateDataReader();
            using var cancellation = new CancellationTokenSource();
            cancellation.Cancel();
            using var extractor = new QueryMultipleExtractor(reader: reader, cancellationToken: cancellation.Token);

            // Act
            Func<Task> act = async () => await extractor.ExtractAsync<Row>().ConfigureAwait(false);

            // Assert
            await Assert.ThrowsAsync<OperationCanceledException>(act).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task TestNextResultAsyncObservesCancellation()
        {
            // Setup
            using var table = CreateTable(42);
            using var reader = table.CreateDataReader();
            using var cancellation = new CancellationTokenSource();
            cancellation.Cancel();
            using var extractor = new QueryMultipleExtractor(reader: reader, cancellationToken: cancellation.Token);

            // Act
            Func<Task> act = async () => await extractor.NextResultAsync().ConfigureAwait(false);

            // Assert
            await Assert.ThrowsAsync<OperationCanceledException>(act).ConfigureAwait(false);
        }

        private static DataTable CreateTable(params int[] values)
        {
            var table = new DataTable();
            table.Columns.Add("Value", typeof(int));
            foreach (var value in values)
            {
                table.Rows.Add(value);
            }
            return table;
        }

        public class Row
        {
            public int Value { get; set; }
        }
    }
}
