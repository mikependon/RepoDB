#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.UnitTests.CustomObjects;

namespace RepoDb.UnitTests.Operations
{
    [TestClass]
    [DoNotParallelize]
    public class QueryMultipleFailureTest : RepositoryOperationTestBase
    {
        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void TestDynamic2ResultsPreservesCommandFailure(bool multipleStatements)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var connection = new CoverageDbConnection();
            var cache = new MemoryCache();
            connection.UseScalarResults();
            var failure = new InvalidOperationException("Command failed.");
            connection.Failure = failure;

            // Act
            Action act = () => connection.QueryMultiple(tableName1: "CoverageEntity",
                where1: new QueryGroup(new QueryField("Id", 1)),
                cacheKey1: "result1",
                tableName2: "CoverageEntity",
                where2: new QueryGroup(new QueryField("Id", 1)),
                cacheKey2: "result2",
                cache: cache);

            // Assert
            var actual = Assert.ThrowsExactly<InvalidOperationException>(act);
            Assert.AreSame(failure, actual);
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public async Task TestDynamic2ResultsAsyncPreservesCommandFailure(bool multipleStatements)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var connection = new CoverageDbConnection();
            var cache = new MemoryCache();
            connection.UseScalarResults();
            var failure = new InvalidOperationException("Command failed.");
            connection.Failure = failure;

            // Act
            Func<Task> act = async () => await connection.QueryMultipleAsync(tableName1: "CoverageEntity",
                where1: new QueryGroup(new QueryField("Id", 1)),
                cacheKey1: "result1",
                tableName2: "CoverageEntity",
                where2: new QueryGroup(new QueryField("Id", 1)),
                cacheKey2: "result2",
                cache: cache).ConfigureAwait(false);

            // Assert
            var actual = await Assert.ThrowsExactlyAsync<InvalidOperationException>(act).ConfigureAwait(false);
            Assert.AreSame(failure, actual);
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public async Task TestDynamic2ResultsAsyncObservesCancellation(bool multipleStatements)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var connection = new CoverageDbConnection();
            var cache = new MemoryCache();
            connection.UseScalarResults();
            using var cancellation = new CancellationTokenSource();
            cancellation.Cancel();

            // Act
            Func<Task> act = async () => await connection.QueryMultipleAsync(tableName1: "CoverageEntity",
                where1: new QueryGroup(new QueryField("Id", 1)),
                cacheKey1: "result1",
                tableName2: "CoverageEntity",
                where2: new QueryGroup(new QueryField("Id", 1)),
                cacheKey2: "result2",
                cache: cache,
                cancellationToken: cancellation.Token).ConfigureAwait(false);

            // Assert
            await Assert.ThrowsAsync<OperationCanceledException>(act).ConfigureAwait(false);
            Assert.AreEqual(0, connection.ExecutionCount);
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void TestTyped2ResultsPreservesCommandFailure(bool multipleStatements)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var connection = new CoverageDbConnection();
            var cache = new MemoryCache();
            var failure = new InvalidOperationException("Command failed.");
            connection.Failure = failure;

            // Act
            Action act = () => connection.QueryMultiple<CoverageEntity, CoverageEntity>(where1: new QueryGroup(new QueryField("Id", 1)),
                cacheKey1: "result1",
                where2: new QueryGroup(new QueryField("Id", 1)),
                cacheKey2: "result2",
                cache: cache);

            // Assert
            var actual = Assert.ThrowsExactly<InvalidOperationException>(act);
            Assert.AreSame(failure, actual);
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public async Task TestTyped2ResultsAsyncPreservesCommandFailure(bool multipleStatements)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var connection = new CoverageDbConnection();
            var cache = new MemoryCache();
            var failure = new InvalidOperationException("Command failed.");
            connection.Failure = failure;

            // Act
            Func<Task> act = async () => await connection.QueryMultipleAsync<CoverageEntity, CoverageEntity>(where1: new QueryGroup(new QueryField("Id", 1)),
                cacheKey1: "result1",
                where2: new QueryGroup(new QueryField("Id", 1)),
                cacheKey2: "result2",
                cache: cache).ConfigureAwait(false);

            // Assert
            var actual = await Assert.ThrowsExactlyAsync<InvalidOperationException>(act).ConfigureAwait(false);
            Assert.AreSame(failure, actual);
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public async Task TestTyped2ResultsAsyncObservesCancellation(bool multipleStatements)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var connection = new CoverageDbConnection();
            var cache = new MemoryCache();
            using var cancellation = new CancellationTokenSource();
            cancellation.Cancel();

            // Act
            Func<Task> act = async () => await connection.QueryMultipleAsync<CoverageEntity, CoverageEntity>(where1: new QueryGroup(new QueryField("Id", 1)),
                cacheKey1: "result1",
                where2: new QueryGroup(new QueryField("Id", 1)),
                cacheKey2: "result2",
                cache: cache,
                cancellationToken: cancellation.Token).ConfigureAwait(false);

            // Assert
            await Assert.ThrowsAsync<OperationCanceledException>(act).ConfigureAwait(false);
            Assert.AreEqual(0, connection.ExecutionCount);
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void TestDynamic3ResultsPreservesCommandFailure(bool multipleStatements)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var connection = new CoverageDbConnection();
            var cache = new MemoryCache();
            connection.UseScalarResults();
            var failure = new InvalidOperationException("Command failed.");
            connection.Failure = failure;

            // Act
            Action act = () => connection.QueryMultiple(tableName1: "CoverageEntity",
                where1: new QueryGroup(new QueryField("Id", 1)),
                cacheKey1: "result1",
                tableName2: "CoverageEntity",
                where2: new QueryGroup(new QueryField("Id", 1)),
                cacheKey2: "result2",
                tableName3: "CoverageEntity",
                where3: new QueryGroup(new QueryField("Id", 1)),
                cacheKey3: "result3",
                cache: cache);

            // Assert
            var actual = Assert.ThrowsExactly<InvalidOperationException>(act);
            Assert.AreSame(failure, actual);
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public async Task TestDynamic3ResultsAsyncPreservesCommandFailure(bool multipleStatements)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var connection = new CoverageDbConnection();
            var cache = new MemoryCache();
            connection.UseScalarResults();
            var failure = new InvalidOperationException("Command failed.");
            connection.Failure = failure;

            // Act
            Func<Task> act = async () => await connection.QueryMultipleAsync(tableName1: "CoverageEntity",
                where1: new QueryGroup(new QueryField("Id", 1)),
                cacheKey1: "result1",
                tableName2: "CoverageEntity",
                where2: new QueryGroup(new QueryField("Id", 1)),
                cacheKey2: "result2",
                tableName3: "CoverageEntity",
                where3: new QueryGroup(new QueryField("Id", 1)),
                cacheKey3: "result3",
                cache: cache).ConfigureAwait(false);

            // Assert
            var actual = await Assert.ThrowsExactlyAsync<InvalidOperationException>(act).ConfigureAwait(false);
            Assert.AreSame(failure, actual);
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public async Task TestDynamic3ResultsAsyncObservesCancellation(bool multipleStatements)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var connection = new CoverageDbConnection();
            var cache = new MemoryCache();
            connection.UseScalarResults();
            using var cancellation = new CancellationTokenSource();
            cancellation.Cancel();

            // Act
            Func<Task> act = async () => await connection.QueryMultipleAsync(tableName1: "CoverageEntity",
                where1: new QueryGroup(new QueryField("Id", 1)),
                cacheKey1: "result1",
                tableName2: "CoverageEntity",
                where2: new QueryGroup(new QueryField("Id", 1)),
                cacheKey2: "result2",
                tableName3: "CoverageEntity",
                where3: new QueryGroup(new QueryField("Id", 1)),
                cacheKey3: "result3",
                cache: cache,
                cancellationToken: cancellation.Token).ConfigureAwait(false);

            // Assert
            await Assert.ThrowsAsync<OperationCanceledException>(act).ConfigureAwait(false);
            Assert.AreEqual(0, connection.ExecutionCount);
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void TestTyped3ResultsPreservesCommandFailure(bool multipleStatements)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var connection = new CoverageDbConnection();
            var cache = new MemoryCache();
            var failure = new InvalidOperationException("Command failed.");
            connection.Failure = failure;

            // Act
            Action act = () => connection.QueryMultiple<CoverageEntity, CoverageEntity, CoverageEntity>(where1: new QueryGroup(new QueryField("Id", 1)),
                cacheKey1: "result1",
                where2: new QueryGroup(new QueryField("Id", 1)),
                cacheKey2: "result2",
                where3: new QueryGroup(new QueryField("Id", 1)),
                cacheKey3: "result3",
                cache: cache);

            // Assert
            var actual = Assert.ThrowsExactly<InvalidOperationException>(act);
            Assert.AreSame(failure, actual);
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public async Task TestTyped3ResultsAsyncPreservesCommandFailure(bool multipleStatements)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var connection = new CoverageDbConnection();
            var cache = new MemoryCache();
            var failure = new InvalidOperationException("Command failed.");
            connection.Failure = failure;

            // Act
            Func<Task> act = async () => await connection.QueryMultipleAsync<CoverageEntity, CoverageEntity, CoverageEntity>(where1: new QueryGroup(new QueryField("Id", 1)),
                cacheKey1: "result1",
                where2: new QueryGroup(new QueryField("Id", 1)),
                cacheKey2: "result2",
                where3: new QueryGroup(new QueryField("Id", 1)),
                cacheKey3: "result3",
                cache: cache).ConfigureAwait(false);

            // Assert
            var actual = await Assert.ThrowsExactlyAsync<InvalidOperationException>(act).ConfigureAwait(false);
            Assert.AreSame(failure, actual);
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public async Task TestTyped3ResultsAsyncObservesCancellation(bool multipleStatements)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var connection = new CoverageDbConnection();
            var cache = new MemoryCache();
            using var cancellation = new CancellationTokenSource();
            cancellation.Cancel();

            // Act
            Func<Task> act = async () => await connection.QueryMultipleAsync<CoverageEntity, CoverageEntity, CoverageEntity>(where1: new QueryGroup(new QueryField("Id", 1)),
                cacheKey1: "result1",
                where2: new QueryGroup(new QueryField("Id", 1)),
                cacheKey2: "result2",
                where3: new QueryGroup(new QueryField("Id", 1)),
                cacheKey3: "result3",
                cache: cache,
                cancellationToken: cancellation.Token).ConfigureAwait(false);

            // Assert
            await Assert.ThrowsAsync<OperationCanceledException>(act).ConfigureAwait(false);
            Assert.AreEqual(0, connection.ExecutionCount);
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void TestDynamic4ResultsPreservesCommandFailure(bool multipleStatements)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var connection = new CoverageDbConnection();
            var cache = new MemoryCache();
            connection.UseScalarResults();
            var failure = new InvalidOperationException("Command failed.");
            connection.Failure = failure;

            // Act
            Action act = () => connection.QueryMultiple(tableName1: "CoverageEntity",
                where1: new QueryGroup(new QueryField("Id", 1)),
                cacheKey1: "result1",
                tableName2: "CoverageEntity",
                where2: new QueryGroup(new QueryField("Id", 1)),
                cacheKey2: "result2",
                tableName3: "CoverageEntity",
                where3: new QueryGroup(new QueryField("Id", 1)),
                cacheKey3: "result3",
                tableName4: "CoverageEntity",
                where4: new QueryGroup(new QueryField("Id", 1)),
                cacheKey4: "result4",
                cache: cache);

            // Assert
            var actual = Assert.ThrowsExactly<InvalidOperationException>(act);
            Assert.AreSame(failure, actual);
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public async Task TestDynamic4ResultsAsyncPreservesCommandFailure(bool multipleStatements)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var connection = new CoverageDbConnection();
            var cache = new MemoryCache();
            connection.UseScalarResults();
            var failure = new InvalidOperationException("Command failed.");
            connection.Failure = failure;

            // Act
            Func<Task> act = async () => await connection.QueryMultipleAsync(tableName1: "CoverageEntity",
                where1: new QueryGroup(new QueryField("Id", 1)),
                cacheKey1: "result1",
                tableName2: "CoverageEntity",
                where2: new QueryGroup(new QueryField("Id", 1)),
                cacheKey2: "result2",
                tableName3: "CoverageEntity",
                where3: new QueryGroup(new QueryField("Id", 1)),
                cacheKey3: "result3",
                tableName4: "CoverageEntity",
                where4: new QueryGroup(new QueryField("Id", 1)),
                cacheKey4: "result4",
                cache: cache).ConfigureAwait(false);

            // Assert
            var actual = await Assert.ThrowsExactlyAsync<InvalidOperationException>(act).ConfigureAwait(false);
            Assert.AreSame(failure, actual);
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public async Task TestDynamic4ResultsAsyncObservesCancellation(bool multipleStatements)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var connection = new CoverageDbConnection();
            var cache = new MemoryCache();
            connection.UseScalarResults();
            using var cancellation = new CancellationTokenSource();
            cancellation.Cancel();

            // Act
            Func<Task> act = async () => await connection.QueryMultipleAsync(tableName1: "CoverageEntity",
                where1: new QueryGroup(new QueryField("Id", 1)),
                cacheKey1: "result1",
                tableName2: "CoverageEntity",
                where2: new QueryGroup(new QueryField("Id", 1)),
                cacheKey2: "result2",
                tableName3: "CoverageEntity",
                where3: new QueryGroup(new QueryField("Id", 1)),
                cacheKey3: "result3",
                tableName4: "CoverageEntity",
                where4: new QueryGroup(new QueryField("Id", 1)),
                cacheKey4: "result4",
                cache: cache,
                cancellationToken: cancellation.Token).ConfigureAwait(false);

            // Assert
            await Assert.ThrowsAsync<OperationCanceledException>(act).ConfigureAwait(false);
            Assert.AreEqual(0, connection.ExecutionCount);
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void TestTyped4ResultsPreservesCommandFailure(bool multipleStatements)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var connection = new CoverageDbConnection();
            var cache = new MemoryCache();
            var failure = new InvalidOperationException("Command failed.");
            connection.Failure = failure;

            // Act
            Action act = () => connection.QueryMultiple<CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity>(where1: new QueryGroup(new QueryField("Id", 1)),
                cacheKey1: "result1",
                where2: new QueryGroup(new QueryField("Id", 1)),
                cacheKey2: "result2",
                where3: new QueryGroup(new QueryField("Id", 1)),
                cacheKey3: "result3",
                where4: new QueryGroup(new QueryField("Id", 1)),
                cacheKey4: "result4",
                cache: cache);

            // Assert
            var actual = Assert.ThrowsExactly<InvalidOperationException>(act);
            Assert.AreSame(failure, actual);
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public async Task TestTyped4ResultsAsyncPreservesCommandFailure(bool multipleStatements)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var connection = new CoverageDbConnection();
            var cache = new MemoryCache();
            var failure = new InvalidOperationException("Command failed.");
            connection.Failure = failure;

            // Act
            Func<Task> act = async () => await connection.QueryMultipleAsync<CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity>(where1: new QueryGroup(new QueryField("Id", 1)),
                cacheKey1: "result1",
                where2: new QueryGroup(new QueryField("Id", 1)),
                cacheKey2: "result2",
                where3: new QueryGroup(new QueryField("Id", 1)),
                cacheKey3: "result3",
                where4: new QueryGroup(new QueryField("Id", 1)),
                cacheKey4: "result4",
                cache: cache).ConfigureAwait(false);

            // Assert
            var actual = await Assert.ThrowsExactlyAsync<InvalidOperationException>(act).ConfigureAwait(false);
            Assert.AreSame(failure, actual);
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public async Task TestTyped4ResultsAsyncObservesCancellation(bool multipleStatements)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var connection = new CoverageDbConnection();
            var cache = new MemoryCache();
            using var cancellation = new CancellationTokenSource();
            cancellation.Cancel();

            // Act
            Func<Task> act = async () => await connection.QueryMultipleAsync<CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity>(where1: new QueryGroup(new QueryField("Id", 1)),
                cacheKey1: "result1",
                where2: new QueryGroup(new QueryField("Id", 1)),
                cacheKey2: "result2",
                where3: new QueryGroup(new QueryField("Id", 1)),
                cacheKey3: "result3",
                where4: new QueryGroup(new QueryField("Id", 1)),
                cacheKey4: "result4",
                cache: cache,
                cancellationToken: cancellation.Token).ConfigureAwait(false);

            // Assert
            await Assert.ThrowsAsync<OperationCanceledException>(act).ConfigureAwait(false);
            Assert.AreEqual(0, connection.ExecutionCount);
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void TestDynamic5ResultsPreservesCommandFailure(bool multipleStatements)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var connection = new CoverageDbConnection();
            var cache = new MemoryCache();
            connection.UseScalarResults();
            var failure = new InvalidOperationException("Command failed.");
            connection.Failure = failure;

            // Act
            Action act = () => connection.QueryMultiple(tableName1: "CoverageEntity",
                where1: new QueryGroup(new QueryField("Id", 1)),
                cacheKey1: "result1",
                tableName2: "CoverageEntity",
                where2: new QueryGroup(new QueryField("Id", 1)),
                cacheKey2: "result2",
                tableName3: "CoverageEntity",
                where3: new QueryGroup(new QueryField("Id", 1)),
                cacheKey3: "result3",
                tableName4: "CoverageEntity",
                where4: new QueryGroup(new QueryField("Id", 1)),
                cacheKey4: "result4",
                tableName5: "CoverageEntity",
                where5: new QueryGroup(new QueryField("Id", 1)),
                cacheKey5: "result5",
                cache: cache);

            // Assert
            var actual = Assert.ThrowsExactly<InvalidOperationException>(act);
            Assert.AreSame(failure, actual);
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public async Task TestDynamic5ResultsAsyncPreservesCommandFailure(bool multipleStatements)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var connection = new CoverageDbConnection();
            var cache = new MemoryCache();
            connection.UseScalarResults();
            var failure = new InvalidOperationException("Command failed.");
            connection.Failure = failure;

            // Act
            Func<Task> act = async () => await connection.QueryMultipleAsync(tableName1: "CoverageEntity",
                where1: new QueryGroup(new QueryField("Id", 1)),
                cacheKey1: "result1",
                tableName2: "CoverageEntity",
                where2: new QueryGroup(new QueryField("Id", 1)),
                cacheKey2: "result2",
                tableName3: "CoverageEntity",
                where3: new QueryGroup(new QueryField("Id", 1)),
                cacheKey3: "result3",
                tableName4: "CoverageEntity",
                where4: new QueryGroup(new QueryField("Id", 1)),
                cacheKey4: "result4",
                tableName5: "CoverageEntity",
                where5: new QueryGroup(new QueryField("Id", 1)),
                cacheKey5: "result5",
                cache: cache).ConfigureAwait(false);

            // Assert
            var actual = await Assert.ThrowsExactlyAsync<InvalidOperationException>(act).ConfigureAwait(false);
            Assert.AreSame(failure, actual);
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public async Task TestDynamic5ResultsAsyncObservesCancellation(bool multipleStatements)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var connection = new CoverageDbConnection();
            var cache = new MemoryCache();
            connection.UseScalarResults();
            using var cancellation = new CancellationTokenSource();
            cancellation.Cancel();

            // Act
            Func<Task> act = async () => await connection.QueryMultipleAsync(tableName1: "CoverageEntity",
                where1: new QueryGroup(new QueryField("Id", 1)),
                cacheKey1: "result1",
                tableName2: "CoverageEntity",
                where2: new QueryGroup(new QueryField("Id", 1)),
                cacheKey2: "result2",
                tableName3: "CoverageEntity",
                where3: new QueryGroup(new QueryField("Id", 1)),
                cacheKey3: "result3",
                tableName4: "CoverageEntity",
                where4: new QueryGroup(new QueryField("Id", 1)),
                cacheKey4: "result4",
                tableName5: "CoverageEntity",
                where5: new QueryGroup(new QueryField("Id", 1)),
                cacheKey5: "result5",
                cache: cache,
                cancellationToken: cancellation.Token).ConfigureAwait(false);

            // Assert
            await Assert.ThrowsAsync<OperationCanceledException>(act).ConfigureAwait(false);
            Assert.AreEqual(0, connection.ExecutionCount);
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void TestTyped5ResultsPreservesCommandFailure(bool multipleStatements)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var connection = new CoverageDbConnection();
            var cache = new MemoryCache();
            var failure = new InvalidOperationException("Command failed.");
            connection.Failure = failure;

            // Act
            Action act = () => connection.QueryMultiple<CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity>(where1: new QueryGroup(new QueryField("Id", 1)),
                cacheKey1: "result1",
                where2: new QueryGroup(new QueryField("Id", 1)),
                cacheKey2: "result2",
                where3: new QueryGroup(new QueryField("Id", 1)),
                cacheKey3: "result3",
                where4: new QueryGroup(new QueryField("Id", 1)),
                cacheKey4: "result4",
                where5: new QueryGroup(new QueryField("Id", 1)),
                cacheKey5: "result5",
                cache: cache);

            // Assert
            var actual = Assert.ThrowsExactly<InvalidOperationException>(act);
            Assert.AreSame(failure, actual);
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public async Task TestTyped5ResultsAsyncPreservesCommandFailure(bool multipleStatements)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var connection = new CoverageDbConnection();
            var cache = new MemoryCache();
            var failure = new InvalidOperationException("Command failed.");
            connection.Failure = failure;

            // Act
            Func<Task> act = async () => await connection.QueryMultipleAsync<CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity>(where1: new QueryGroup(new QueryField("Id", 1)),
                cacheKey1: "result1",
                where2: new QueryGroup(new QueryField("Id", 1)),
                cacheKey2: "result2",
                where3: new QueryGroup(new QueryField("Id", 1)),
                cacheKey3: "result3",
                where4: new QueryGroup(new QueryField("Id", 1)),
                cacheKey4: "result4",
                where5: new QueryGroup(new QueryField("Id", 1)),
                cacheKey5: "result5",
                cache: cache).ConfigureAwait(false);

            // Assert
            var actual = await Assert.ThrowsExactlyAsync<InvalidOperationException>(act).ConfigureAwait(false);
            Assert.AreSame(failure, actual);
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public async Task TestTyped5ResultsAsyncObservesCancellation(bool multipleStatements)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var connection = new CoverageDbConnection();
            var cache = new MemoryCache();
            using var cancellation = new CancellationTokenSource();
            cancellation.Cancel();

            // Act
            Func<Task> act = async () => await connection.QueryMultipleAsync<CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity>(where1: new QueryGroup(new QueryField("Id", 1)),
                cacheKey1: "result1",
                where2: new QueryGroup(new QueryField("Id", 1)),
                cacheKey2: "result2",
                where3: new QueryGroup(new QueryField("Id", 1)),
                cacheKey3: "result3",
                where4: new QueryGroup(new QueryField("Id", 1)),
                cacheKey4: "result4",
                where5: new QueryGroup(new QueryField("Id", 1)),
                cacheKey5: "result5",
                cache: cache,
                cancellationToken: cancellation.Token).ConfigureAwait(false);

            // Assert
            await Assert.ThrowsAsync<OperationCanceledException>(act).ConfigureAwait(false);
            Assert.AreEqual(0, connection.ExecutionCount);
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void TestDynamic6ResultsPreservesCommandFailure(bool multipleStatements)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var connection = new CoverageDbConnection();
            var cache = new MemoryCache();
            connection.UseScalarResults();
            var failure = new InvalidOperationException("Command failed.");
            connection.Failure = failure;

            // Act
            Action act = () => connection.QueryMultiple(tableName1: "CoverageEntity",
                where1: new QueryGroup(new QueryField("Id", 1)),
                cacheKey1: "result1",
                tableName2: "CoverageEntity",
                where2: new QueryGroup(new QueryField("Id", 1)),
                cacheKey2: "result2",
                tableName3: "CoverageEntity",
                where3: new QueryGroup(new QueryField("Id", 1)),
                cacheKey3: "result3",
                tableName4: "CoverageEntity",
                where4: new QueryGroup(new QueryField("Id", 1)),
                cacheKey4: "result4",
                tableName5: "CoverageEntity",
                where5: new QueryGroup(new QueryField("Id", 1)),
                cacheKey5: "result5",
                tableName6: "CoverageEntity",
                where6: new QueryGroup(new QueryField("Id", 1)),
                cacheKey6: "result6",
                cache: cache);

            // Assert
            var actual = Assert.ThrowsExactly<InvalidOperationException>(act);
            Assert.AreSame(failure, actual);
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public async Task TestDynamic6ResultsAsyncPreservesCommandFailure(bool multipleStatements)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var connection = new CoverageDbConnection();
            var cache = new MemoryCache();
            connection.UseScalarResults();
            var failure = new InvalidOperationException("Command failed.");
            connection.Failure = failure;

            // Act
            Func<Task> act = async () => await connection.QueryMultipleAsync(tableName1: "CoverageEntity",
                where1: new QueryGroup(new QueryField("Id", 1)),
                cacheKey1: "result1",
                tableName2: "CoverageEntity",
                where2: new QueryGroup(new QueryField("Id", 1)),
                cacheKey2: "result2",
                tableName3: "CoverageEntity",
                where3: new QueryGroup(new QueryField("Id", 1)),
                cacheKey3: "result3",
                tableName4: "CoverageEntity",
                where4: new QueryGroup(new QueryField("Id", 1)),
                cacheKey4: "result4",
                tableName5: "CoverageEntity",
                where5: new QueryGroup(new QueryField("Id", 1)),
                cacheKey5: "result5",
                tableName6: "CoverageEntity",
                where6: new QueryGroup(new QueryField("Id", 1)),
                cacheKey6: "result6",
                cache: cache).ConfigureAwait(false);

            // Assert
            var actual = await Assert.ThrowsExactlyAsync<InvalidOperationException>(act).ConfigureAwait(false);
            Assert.AreSame(failure, actual);
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public async Task TestDynamic6ResultsAsyncObservesCancellation(bool multipleStatements)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var connection = new CoverageDbConnection();
            var cache = new MemoryCache();
            connection.UseScalarResults();
            using var cancellation = new CancellationTokenSource();
            cancellation.Cancel();

            // Act
            Func<Task> act = async () => await connection.QueryMultipleAsync(tableName1: "CoverageEntity",
                where1: new QueryGroup(new QueryField("Id", 1)),
                cacheKey1: "result1",
                tableName2: "CoverageEntity",
                where2: new QueryGroup(new QueryField("Id", 1)),
                cacheKey2: "result2",
                tableName3: "CoverageEntity",
                where3: new QueryGroup(new QueryField("Id", 1)),
                cacheKey3: "result3",
                tableName4: "CoverageEntity",
                where4: new QueryGroup(new QueryField("Id", 1)),
                cacheKey4: "result4",
                tableName5: "CoverageEntity",
                where5: new QueryGroup(new QueryField("Id", 1)),
                cacheKey5: "result5",
                tableName6: "CoverageEntity",
                where6: new QueryGroup(new QueryField("Id", 1)),
                cacheKey6: "result6",
                cache: cache,
                cancellationToken: cancellation.Token).ConfigureAwait(false);

            // Assert
            await Assert.ThrowsAsync<OperationCanceledException>(act).ConfigureAwait(false);
            Assert.AreEqual(0, connection.ExecutionCount);
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void TestTyped6ResultsPreservesCommandFailure(bool multipleStatements)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var connection = new CoverageDbConnection();
            var cache = new MemoryCache();
            var failure = new InvalidOperationException("Command failed.");
            connection.Failure = failure;

            // Act
            Action act = () => connection.QueryMultiple<CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity>(where1: new QueryGroup(new QueryField("Id", 1)),
                cacheKey1: "result1",
                where2: new QueryGroup(new QueryField("Id", 1)),
                cacheKey2: "result2",
                where3: new QueryGroup(new QueryField("Id", 1)),
                cacheKey3: "result3",
                where4: new QueryGroup(new QueryField("Id", 1)),
                cacheKey4: "result4",
                where5: new QueryGroup(new QueryField("Id", 1)),
                cacheKey5: "result5",
                where6: new QueryGroup(new QueryField("Id", 1)),
                cacheKey6: "result6",
                cache: cache);

            // Assert
            var actual = Assert.ThrowsExactly<InvalidOperationException>(act);
            Assert.AreSame(failure, actual);
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public async Task TestTyped6ResultsAsyncPreservesCommandFailure(bool multipleStatements)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var connection = new CoverageDbConnection();
            var cache = new MemoryCache();
            var failure = new InvalidOperationException("Command failed.");
            connection.Failure = failure;

            // Act
            Func<Task> act = async () => await connection.QueryMultipleAsync<CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity>(where1: new QueryGroup(new QueryField("Id", 1)),
                cacheKey1: "result1",
                where2: new QueryGroup(new QueryField("Id", 1)),
                cacheKey2: "result2",
                where3: new QueryGroup(new QueryField("Id", 1)),
                cacheKey3: "result3",
                where4: new QueryGroup(new QueryField("Id", 1)),
                cacheKey4: "result4",
                where5: new QueryGroup(new QueryField("Id", 1)),
                cacheKey5: "result5",
                where6: new QueryGroup(new QueryField("Id", 1)),
                cacheKey6: "result6",
                cache: cache).ConfigureAwait(false);

            // Assert
            var actual = await Assert.ThrowsExactlyAsync<InvalidOperationException>(act).ConfigureAwait(false);
            Assert.AreSame(failure, actual);
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public async Task TestTyped6ResultsAsyncObservesCancellation(bool multipleStatements)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var connection = new CoverageDbConnection();
            var cache = new MemoryCache();
            using var cancellation = new CancellationTokenSource();
            cancellation.Cancel();

            // Act
            Func<Task> act = async () => await connection.QueryMultipleAsync<CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity>(where1: new QueryGroup(new QueryField("Id", 1)),
                cacheKey1: "result1",
                where2: new QueryGroup(new QueryField("Id", 1)),
                cacheKey2: "result2",
                where3: new QueryGroup(new QueryField("Id", 1)),
                cacheKey3: "result3",
                where4: new QueryGroup(new QueryField("Id", 1)),
                cacheKey4: "result4",
                where5: new QueryGroup(new QueryField("Id", 1)),
                cacheKey5: "result5",
                where6: new QueryGroup(new QueryField("Id", 1)),
                cacheKey6: "result6",
                cache: cache,
                cancellationToken: cancellation.Token).ConfigureAwait(false);

            // Assert
            await Assert.ThrowsAsync<OperationCanceledException>(act).ConfigureAwait(false);
            Assert.AreEqual(0, connection.ExecutionCount);
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void TestDynamic7ResultsPreservesCommandFailure(bool multipleStatements)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var connection = new CoverageDbConnection();
            var cache = new MemoryCache();
            connection.UseScalarResults();
            var failure = new InvalidOperationException("Command failed.");
            connection.Failure = failure;

            // Act
            Action act = () => connection.QueryMultiple(tableName1: "CoverageEntity",
                where1: new QueryGroup(new QueryField("Id", 1)),
                cacheKey1: "result1",
                tableName2: "CoverageEntity",
                where2: new QueryGroup(new QueryField("Id", 1)),
                cacheKey2: "result2",
                tableName3: "CoverageEntity",
                where3: new QueryGroup(new QueryField("Id", 1)),
                cacheKey3: "result3",
                tableName4: "CoverageEntity",
                where4: new QueryGroup(new QueryField("Id", 1)),
                cacheKey4: "result4",
                tableName5: "CoverageEntity",
                where5: new QueryGroup(new QueryField("Id", 1)),
                cacheKey5: "result5",
                tableName6: "CoverageEntity",
                where6: new QueryGroup(new QueryField("Id", 1)),
                cacheKey6: "result6",
                tableName7: "CoverageEntity",
                where7: new QueryGroup(new QueryField("Id", 1)),
                cacheKey7: "result7",
                cache: cache);

            // Assert
            var actual = Assert.ThrowsExactly<InvalidOperationException>(act);
            Assert.AreSame(failure, actual);
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public async Task TestDynamic7ResultsAsyncPreservesCommandFailure(bool multipleStatements)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var connection = new CoverageDbConnection();
            var cache = new MemoryCache();
            connection.UseScalarResults();
            var failure = new InvalidOperationException("Command failed.");
            connection.Failure = failure;

            // Act
            Func<Task> act = async () => await connection.QueryMultipleAsync(tableName1: "CoverageEntity",
                where1: new QueryGroup(new QueryField("Id", 1)),
                cacheKey1: "result1",
                tableName2: "CoverageEntity",
                where2: new QueryGroup(new QueryField("Id", 1)),
                cacheKey2: "result2",
                tableName3: "CoverageEntity",
                where3: new QueryGroup(new QueryField("Id", 1)),
                cacheKey3: "result3",
                tableName4: "CoverageEntity",
                where4: new QueryGroup(new QueryField("Id", 1)),
                cacheKey4: "result4",
                tableName5: "CoverageEntity",
                where5: new QueryGroup(new QueryField("Id", 1)),
                cacheKey5: "result5",
                tableName6: "CoverageEntity",
                where6: new QueryGroup(new QueryField("Id", 1)),
                cacheKey6: "result6",
                tableName7: "CoverageEntity",
                where7: new QueryGroup(new QueryField("Id", 1)),
                cacheKey7: "result7",
                cache: cache).ConfigureAwait(false);

            // Assert
            var actual = await Assert.ThrowsExactlyAsync<InvalidOperationException>(act).ConfigureAwait(false);
            Assert.AreSame(failure, actual);
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public async Task TestDynamic7ResultsAsyncObservesCancellation(bool multipleStatements)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var connection = new CoverageDbConnection();
            var cache = new MemoryCache();
            connection.UseScalarResults();
            using var cancellation = new CancellationTokenSource();
            cancellation.Cancel();

            // Act
            Func<Task> act = async () => await connection.QueryMultipleAsync(tableName1: "CoverageEntity",
                where1: new QueryGroup(new QueryField("Id", 1)),
                cacheKey1: "result1",
                tableName2: "CoverageEntity",
                where2: new QueryGroup(new QueryField("Id", 1)),
                cacheKey2: "result2",
                tableName3: "CoverageEntity",
                where3: new QueryGroup(new QueryField("Id", 1)),
                cacheKey3: "result3",
                tableName4: "CoverageEntity",
                where4: new QueryGroup(new QueryField("Id", 1)),
                cacheKey4: "result4",
                tableName5: "CoverageEntity",
                where5: new QueryGroup(new QueryField("Id", 1)),
                cacheKey5: "result5",
                tableName6: "CoverageEntity",
                where6: new QueryGroup(new QueryField("Id", 1)),
                cacheKey6: "result6",
                tableName7: "CoverageEntity",
                where7: new QueryGroup(new QueryField("Id", 1)),
                cacheKey7: "result7",
                cache: cache,
                cancellationToken: cancellation.Token).ConfigureAwait(false);

            // Assert
            await Assert.ThrowsAsync<OperationCanceledException>(act).ConfigureAwait(false);
            Assert.AreEqual(0, connection.ExecutionCount);
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void TestTyped7ResultsPreservesCommandFailure(bool multipleStatements)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var connection = new CoverageDbConnection();
            var cache = new MemoryCache();
            var failure = new InvalidOperationException("Command failed.");
            connection.Failure = failure;

            // Act
            Action act = () => connection.QueryMultiple<CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity>(where1: new QueryGroup(new QueryField("Id", 1)),
                cacheKey1: "result1",
                where2: new QueryGroup(new QueryField("Id", 1)),
                cacheKey2: "result2",
                where3: new QueryGroup(new QueryField("Id", 1)),
                cacheKey3: "result3",
                where4: new QueryGroup(new QueryField("Id", 1)),
                cacheKey4: "result4",
                where5: new QueryGroup(new QueryField("Id", 1)),
                cacheKey5: "result5",
                where6: new QueryGroup(new QueryField("Id", 1)),
                cacheKey6: "result6",
                where7: new QueryGroup(new QueryField("Id", 1)),
                cacheKey7: "result7",
                cache: cache);

            // Assert
            var actual = Assert.ThrowsExactly<InvalidOperationException>(act);
            Assert.AreSame(failure, actual);
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public async Task TestTyped7ResultsAsyncPreservesCommandFailure(bool multipleStatements)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var connection = new CoverageDbConnection();
            var cache = new MemoryCache();
            var failure = new InvalidOperationException("Command failed.");
            connection.Failure = failure;

            // Act
            Func<Task> act = async () => await connection.QueryMultipleAsync<CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity>(where1: new QueryGroup(new QueryField("Id", 1)),
                cacheKey1: "result1",
                where2: new QueryGroup(new QueryField("Id", 1)),
                cacheKey2: "result2",
                where3: new QueryGroup(new QueryField("Id", 1)),
                cacheKey3: "result3",
                where4: new QueryGroup(new QueryField("Id", 1)),
                cacheKey4: "result4",
                where5: new QueryGroup(new QueryField("Id", 1)),
                cacheKey5: "result5",
                where6: new QueryGroup(new QueryField("Id", 1)),
                cacheKey6: "result6",
                where7: new QueryGroup(new QueryField("Id", 1)),
                cacheKey7: "result7",
                cache: cache).ConfigureAwait(false);

            // Assert
            var actual = await Assert.ThrowsExactlyAsync<InvalidOperationException>(act).ConfigureAwait(false);
            Assert.AreSame(failure, actual);
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public async Task TestTyped7ResultsAsyncObservesCancellation(bool multipleStatements)
        {
            // Setup
            DbSettingMapper.Add<CoverageDbConnection>(multipleStatements
                ? new CustomDbSetting()
                : new CustomSingleStatementSupportDbSetting(), true);
            using var connection = new CoverageDbConnection();
            var cache = new MemoryCache();
            using var cancellation = new CancellationTokenSource();
            cancellation.Cancel();

            // Act
            Func<Task> act = async () => await connection.QueryMultipleAsync<CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity, CoverageEntity>(where1: new QueryGroup(new QueryField("Id", 1)),
                cacheKey1: "result1",
                where2: new QueryGroup(new QueryField("Id", 1)),
                cacheKey2: "result2",
                where3: new QueryGroup(new QueryField("Id", 1)),
                cacheKey3: "result3",
                where4: new QueryGroup(new QueryField("Id", 1)),
                cacheKey4: "result4",
                where5: new QueryGroup(new QueryField("Id", 1)),
                cacheKey5: "result5",
                where6: new QueryGroup(new QueryField("Id", 1)),
                cacheKey6: "result6",
                where7: new QueryGroup(new QueryField("Id", 1)),
                cacheKey7: "result7",
                cache: cache,
                cancellationToken: cancellation.Token).ConfigureAwait(false);

            // Assert
            await Assert.ThrowsAsync<OperationCanceledException>(act).ConfigureAwait(false);
            Assert.AreEqual(0, connection.ExecutionCount);
        }

    }
}
