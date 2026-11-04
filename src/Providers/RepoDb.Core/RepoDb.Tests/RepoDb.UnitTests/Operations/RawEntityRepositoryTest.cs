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
using RepoDb.Enumerations;
using RepoDb.UnitTests.CustomObjects;

namespace RepoDb.UnitTests.Operations
{
    [TestClass]
    [DoNotParallelize]
    public class RawEntityRepositoryTest : RepositoryOperationTestBase
    {
        [TestMethod]
        public void TestExecuteQueryTypedReturnsCommandResult()
        {
            // Setup
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();

            // Act
            var actual = repository.ExecuteQuery("SELECT Id, Name", param: new { Id = 1 });

            // Assert
            Assert.AreEqual(1, actual.Single().Id);
            Assert.AreEqual(1, connection.ExecutionCount);
        }

        [TestMethod]
        public void TestExecuteQueryTypedPreservesCommandFailure()
        {
            // Setup
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();
            var failure = new InvalidOperationException("Command failed.");
            connection.Failure = failure;

            // Act
            Action act = () => repository.ExecuteQuery("SELECT Id, Name", param: new { Id = 1 });

            // Assert
            var actual = Assert.ThrowsExactly<InvalidOperationException>(act);
            Assert.AreSame(failure, actual);
        }

        [TestMethod]
        public void TestExecuteQueryTypedUsesCallerTransaction()
        {
            // Setup
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();
            using var transaction = connection.BeginTransaction();

            // Act
            var actual = repository.ExecuteQuery("SELECT Id, Name", param: new { Id = 1 }, transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Single().Id);
            Assert.AreEqual(0, connection.DisposeCount);
        }

        [TestMethod]
        public void TestExecuteQueryTypedReturnsCachedResult()
        {
            // Setup
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();
            var first = repository.ExecuteQuery("SELECT Id, Name", param: new { Id = 1 }, cacheKey: "raw-result");
            connection.Failure = new InvalidOperationException("Cache should avoid execution.");

            // Act
            var actual = repository.ExecuteQuery("SELECT Id, Name", param: new { Id = 1 }, cacheKey: "raw-result");

            // Assert
            Assert.AreEqual(1, actual.Single().Id);
            Assert.AreEqual(1, connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestExecuteQueryAsyncTypedReturnsCommandResult()
        {
            // Setup
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();

            // Act
            var actual = await repository.ExecuteQueryAsync("SELECT Id, Name", param: new { Id = 1 });

            // Assert
            Assert.AreEqual(1, actual.Single().Id);
            Assert.AreEqual(1, connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestExecuteQueryAsyncTypedPreservesCommandFailure()
        {
            // Setup
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();
            var failure = new InvalidOperationException("Command failed.");
            connection.Failure = failure;

            // Act
            Func<Task> act = async () => await repository.ExecuteQueryAsync("SELECT Id, Name", param: new { Id = 1 }).ConfigureAwait(false);

            // Assert
            var actual = await Assert.ThrowsExactlyAsync<InvalidOperationException>(act).ConfigureAwait(false);
            Assert.AreSame(failure, actual);
        }

        [TestMethod]
        public async Task TestExecuteQueryAsyncTypedUsesCallerTransaction()
        {
            // Setup
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();
            using var transaction = connection.BeginTransaction();

            // Act
            var actual = await repository.ExecuteQueryAsync("SELECT Id, Name", param: new { Id = 1 }, transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Single().Id);
            Assert.AreEqual(0, connection.DisposeCount);
        }

        [TestMethod]
        public async Task TestExecuteQueryAsyncTypedObservesCancellation()
        {
            // Setup
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();
            using var cancellation = new CancellationTokenSource();
            cancellation.Cancel();

            // Act
            Func<Task> act = async () => await repository.ExecuteQueryAsync("SELECT Id, Name", param: new { Id = 1 }, cancellationToken: cancellation.Token).ConfigureAwait(false);

            // Assert
            await Assert.ThrowsAsync<OperationCanceledException>(act).ConfigureAwait(false);
            Assert.AreEqual(0, connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestExecuteQueryAsyncTypedReturnsCachedResult()
        {
            // Setup
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();
            var first = await repository.ExecuteQueryAsync("SELECT Id, Name", param: new { Id = 1 }, cacheKey: "raw-result");
            connection.Failure = new InvalidOperationException("Cache should avoid execution.");

            // Act
            var actual = await repository.ExecuteQueryAsync("SELECT Id, Name", param: new { Id = 1 }, cacheKey: "raw-result");

            // Assert
            Assert.AreEqual(1, actual.Single().Id);
            Assert.AreEqual(1, connection.ExecutionCount);
        }

        [TestMethod]
        public void TestExecuteQueryFirstTypedReturnsCommandResult()
        {
            // Setup
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();

            // Act
            var actual = repository.ExecuteQueryFirst("SELECT Id, Name", param: new { Id = 1 });

            // Assert
            Assert.AreEqual(1, actual.Id);
            Assert.AreEqual(1, connection.ExecutionCount);
        }

        [TestMethod]
        public void TestExecuteQueryFirstTypedPreservesCommandFailure()
        {
            // Setup
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();
            var failure = new InvalidOperationException("Command failed.");
            connection.Failure = failure;

            // Act
            Action act = () => repository.ExecuteQueryFirst("SELECT Id, Name", param: new { Id = 1 });

            // Assert
            var actual = Assert.ThrowsExactly<InvalidOperationException>(act);
            Assert.AreSame(failure, actual);
        }

        [TestMethod]
        public void TestExecuteQueryFirstTypedUsesCallerTransaction()
        {
            // Setup
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();
            using var transaction = connection.BeginTransaction();

            // Act
            var actual = repository.ExecuteQueryFirst("SELECT Id, Name", param: new { Id = 1 }, transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Id);
            Assert.AreEqual(0, connection.DisposeCount);
        }

        [TestMethod]
        public void TestExecuteQueryFirstTypedReturnsCachedResult()
        {
            // Setup
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();
            var first = repository.ExecuteQueryFirst("SELECT Id, Name", param: new { Id = 1 }, cacheKey: "raw-result");
            connection.Failure = new InvalidOperationException("Cache should avoid execution.");

            // Act
            var actual = repository.ExecuteQueryFirst("SELECT Id, Name", param: new { Id = 1 }, cacheKey: "raw-result");

            // Assert
            Assert.AreEqual(1, actual.Id);
            Assert.AreEqual(1, connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestExecuteQueryFirstAsyncTypedReturnsCommandResult()
        {
            // Setup
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();

            // Act
            var actual = await repository.ExecuteQueryFirstAsync("SELECT Id, Name", param: new { Id = 1 });

            // Assert
            Assert.AreEqual(1, actual.Id);
            Assert.AreEqual(1, connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestExecuteQueryFirstAsyncTypedPreservesCommandFailure()
        {
            // Setup
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();
            var failure = new InvalidOperationException("Command failed.");
            connection.Failure = failure;

            // Act
            Func<Task> act = async () => await repository.ExecuteQueryFirstAsync("SELECT Id, Name", param: new { Id = 1 }).ConfigureAwait(false);

            // Assert
            var actual = await Assert.ThrowsExactlyAsync<InvalidOperationException>(act).ConfigureAwait(false);
            Assert.AreSame(failure, actual);
        }

        [TestMethod]
        public async Task TestExecuteQueryFirstAsyncTypedUsesCallerTransaction()
        {
            // Setup
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();
            using var transaction = connection.BeginTransaction();

            // Act
            var actual = await repository.ExecuteQueryFirstAsync("SELECT Id, Name", param: new { Id = 1 }, transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Id);
            Assert.AreEqual(0, connection.DisposeCount);
        }

        [TestMethod]
        public async Task TestExecuteQueryFirstAsyncTypedObservesCancellation()
        {
            // Setup
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();
            using var cancellation = new CancellationTokenSource();
            cancellation.Cancel();

            // Act
            Func<Task> act = async () => await repository.ExecuteQueryFirstAsync("SELECT Id, Name", param: new { Id = 1 }, cancellationToken: cancellation.Token).ConfigureAwait(false);

            // Assert
            await Assert.ThrowsAsync<OperationCanceledException>(act).ConfigureAwait(false);
            Assert.AreEqual(0, connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestExecuteQueryFirstAsyncTypedReturnsCachedResult()
        {
            // Setup
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();
            var first = await repository.ExecuteQueryFirstAsync("SELECT Id, Name", param: new { Id = 1 }, cacheKey: "raw-result");
            connection.Failure = new InvalidOperationException("Cache should avoid execution.");

            // Act
            var actual = await repository.ExecuteQueryFirstAsync("SELECT Id, Name", param: new { Id = 1 }, cacheKey: "raw-result");

            // Assert
            Assert.AreEqual(1, actual.Id);
            Assert.AreEqual(1, connection.ExecutionCount);
        }

        [TestMethod]
        public void TestExecuteQuerySingleTypedReturnsCommandResult()
        {
            // Setup
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();

            // Act
            var actual = repository.ExecuteQuerySingle("SELECT Id, Name", param: new { Id = 1 });

            // Assert
            Assert.AreEqual(1, actual.Id);
            Assert.AreEqual(1, connection.ExecutionCount);
        }

        [TestMethod]
        public void TestExecuteQuerySingleTypedPreservesCommandFailure()
        {
            // Setup
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();
            var failure = new InvalidOperationException("Command failed.");
            connection.Failure = failure;

            // Act
            Action act = () => repository.ExecuteQuerySingle("SELECT Id, Name", param: new { Id = 1 });

            // Assert
            var actual = Assert.ThrowsExactly<InvalidOperationException>(act);
            Assert.AreSame(failure, actual);
        }

        [TestMethod]
        public void TestExecuteQuerySingleTypedUsesCallerTransaction()
        {
            // Setup
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();
            using var transaction = connection.BeginTransaction();

            // Act
            var actual = repository.ExecuteQuerySingle("SELECT Id, Name", param: new { Id = 1 }, transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Id);
            Assert.AreEqual(0, connection.DisposeCount);
        }

        [TestMethod]
        public void TestExecuteQuerySingleTypedReturnsCachedResult()
        {
            // Setup
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();
            var first = repository.ExecuteQuerySingle("SELECT Id, Name", param: new { Id = 1 }, cacheKey: "raw-result");
            connection.Failure = new InvalidOperationException("Cache should avoid execution.");

            // Act
            var actual = repository.ExecuteQuerySingle("SELECT Id, Name", param: new { Id = 1 }, cacheKey: "raw-result");

            // Assert
            Assert.AreEqual(1, actual.Id);
            Assert.AreEqual(1, connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestExecuteQuerySingleAsyncTypedReturnsCommandResult()
        {
            // Setup
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();

            // Act
            var actual = await repository.ExecuteQuerySingleAsync("SELECT Id, Name", param: new { Id = 1 });

            // Assert
            Assert.AreEqual(1, actual.Id);
            Assert.AreEqual(1, connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestExecuteQuerySingleAsyncTypedPreservesCommandFailure()
        {
            // Setup
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();
            var failure = new InvalidOperationException("Command failed.");
            connection.Failure = failure;

            // Act
            Func<Task> act = async () => await repository.ExecuteQuerySingleAsync("SELECT Id, Name", param: new { Id = 1 }).ConfigureAwait(false);

            // Assert
            var actual = await Assert.ThrowsExactlyAsync<InvalidOperationException>(act).ConfigureAwait(false);
            Assert.AreSame(failure, actual);
        }

        [TestMethod]
        public async Task TestExecuteQuerySingleAsyncTypedUsesCallerTransaction()
        {
            // Setup
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();
            using var transaction = connection.BeginTransaction();

            // Act
            var actual = await repository.ExecuteQuerySingleAsync("SELECT Id, Name", param: new { Id = 1 }, transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Id);
            Assert.AreEqual(0, connection.DisposeCount);
        }

        [TestMethod]
        public async Task TestExecuteQuerySingleAsyncTypedObservesCancellation()
        {
            // Setup
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();
            using var cancellation = new CancellationTokenSource();
            cancellation.Cancel();

            // Act
            Func<Task> act = async () => await repository.ExecuteQuerySingleAsync("SELECT Id, Name", param: new { Id = 1 }, cancellationToken: cancellation.Token).ConfigureAwait(false);

            // Assert
            await Assert.ThrowsAsync<OperationCanceledException>(act).ConfigureAwait(false);
            Assert.AreEqual(0, connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestExecuteQuerySingleAsyncTypedReturnsCachedResult()
        {
            // Setup
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();
            var first = await repository.ExecuteQuerySingleAsync("SELECT Id, Name", param: new { Id = 1 }, cacheKey: "raw-result");
            connection.Failure = new InvalidOperationException("Cache should avoid execution.");

            // Act
            var actual = await repository.ExecuteQuerySingleAsync("SELECT Id, Name", param: new { Id = 1 }, cacheKey: "raw-result");

            // Assert
            Assert.AreEqual(1, actual.Id);
            Assert.AreEqual(1, connection.ExecutionCount);
        }

        [TestMethod]
        public void TestExecuteNonQueryUntypedReturnsCommandResult()
        {
            // Setup
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();

            // Act
            var actual = repository.ExecuteNonQuery("SELECT Id, Name", param: new { Id = 1 });

            // Assert
            Assert.AreEqual(1, actual);
            Assert.AreEqual(1, connection.ExecutionCount);
        }

        [TestMethod]
        public void TestExecuteNonQueryUntypedPreservesCommandFailure()
        {
            // Setup
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();
            var failure = new InvalidOperationException("Command failed.");
            connection.Failure = failure;

            // Act
            Action act = () => repository.ExecuteNonQuery("SELECT Id, Name", param: new { Id = 1 });

            // Assert
            var actual = Assert.ThrowsExactly<InvalidOperationException>(act);
            Assert.AreSame(failure, actual);
        }

        [TestMethod]
        public void TestExecuteNonQueryUntypedUsesCallerTransaction()
        {
            // Setup
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();
            using var transaction = connection.BeginTransaction();

            // Act
            var actual = repository.ExecuteNonQuery("SELECT Id, Name", param: new { Id = 1 }, transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.AreEqual(0, connection.DisposeCount);
        }

        [TestMethod]
        public async Task TestExecuteNonQueryAsyncUntypedReturnsCommandResult()
        {
            // Setup
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();

            // Act
            var actual = await repository.ExecuteNonQueryAsync("SELECT Id, Name", param: new { Id = 1 });

            // Assert
            Assert.AreEqual(1, actual);
            Assert.AreEqual(1, connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestExecuteNonQueryAsyncUntypedPreservesCommandFailure()
        {
            // Setup
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();
            var failure = new InvalidOperationException("Command failed.");
            connection.Failure = failure;

            // Act
            Func<Task> act = async () => await repository.ExecuteNonQueryAsync("SELECT Id, Name", param: new { Id = 1 }).ConfigureAwait(false);

            // Assert
            var actual = await Assert.ThrowsExactlyAsync<InvalidOperationException>(act).ConfigureAwait(false);
            Assert.AreSame(failure, actual);
        }

        [TestMethod]
        public async Task TestExecuteNonQueryAsyncUntypedUsesCallerTransaction()
        {
            // Setup
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();
            using var transaction = connection.BeginTransaction();

            // Act
            var actual = await repository.ExecuteNonQueryAsync("SELECT Id, Name", param: new { Id = 1 }, transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.AreEqual(0, connection.DisposeCount);
        }

        [TestMethod]
        public async Task TestExecuteNonQueryAsyncUntypedObservesCancellation()
        {
            // Setup
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();
            using var cancellation = new CancellationTokenSource();
            cancellation.Cancel();

            // Act
            Func<Task> act = async () => await repository.ExecuteNonQueryAsync("SELECT Id, Name", param: new { Id = 1 }, cancellationToken: cancellation.Token).ConfigureAwait(false);

            // Assert
            await Assert.ThrowsAsync<OperationCanceledException>(act).ConfigureAwait(false);
            Assert.AreEqual(0, connection.ExecutionCount);
        }

        [TestMethod]
        public void TestExecuteScalarUntypedReturnsCommandResult()
        {
            // Setup
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();

            // Act
            var actual = repository.ExecuteScalar("SELECT Id, Name", param: new { Id = 1 });

            // Assert
            Assert.AreEqual(1, actual);
            Assert.AreEqual(1, connection.ExecutionCount);
        }

        [TestMethod]
        public void TestExecuteScalarUntypedPreservesCommandFailure()
        {
            // Setup
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();
            var failure = new InvalidOperationException("Command failed.");
            connection.Failure = failure;

            // Act
            Action act = () => repository.ExecuteScalar("SELECT Id, Name", param: new { Id = 1 });

            // Assert
            var actual = Assert.ThrowsExactly<InvalidOperationException>(act);
            Assert.AreSame(failure, actual);
        }

        [TestMethod]
        public void TestExecuteScalarUntypedUsesCallerTransaction()
        {
            // Setup
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();
            using var transaction = connection.BeginTransaction();

            // Act
            var actual = repository.ExecuteScalar("SELECT Id, Name", param: new { Id = 1 }, transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.AreEqual(0, connection.DisposeCount);
        }

        [TestMethod]
        public void TestExecuteScalarUntypedReturnsCachedResult()
        {
            // Setup
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();
            var first = repository.ExecuteScalar("SELECT Id, Name", param: new { Id = 1 }, cacheKey: "raw-result");
            connection.Failure = new InvalidOperationException("Cache should avoid execution.");

            // Act
            var actual = repository.ExecuteScalar("SELECT Id, Name", param: new { Id = 1 }, cacheKey: "raw-result");

            // Assert
            Assert.AreEqual(1, actual);
            Assert.AreEqual(1, connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestExecuteScalarAsyncUntypedReturnsCommandResult()
        {
            // Setup
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();

            // Act
            var actual = await repository.ExecuteScalarAsync("SELECT Id, Name", param: new { Id = 1 });

            // Assert
            Assert.AreEqual(1, actual);
            Assert.AreEqual(1, connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestExecuteScalarAsyncUntypedPreservesCommandFailure()
        {
            // Setup
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();
            var failure = new InvalidOperationException("Command failed.");
            connection.Failure = failure;

            // Act
            Func<Task> act = async () => await repository.ExecuteScalarAsync("SELECT Id, Name", param: new { Id = 1 }).ConfigureAwait(false);

            // Assert
            var actual = await Assert.ThrowsExactlyAsync<InvalidOperationException>(act).ConfigureAwait(false);
            Assert.AreSame(failure, actual);
        }

        [TestMethod]
        public async Task TestExecuteScalarAsyncUntypedUsesCallerTransaction()
        {
            // Setup
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();
            using var transaction = connection.BeginTransaction();

            // Act
            var actual = await repository.ExecuteScalarAsync("SELECT Id, Name", param: new { Id = 1 }, transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.AreEqual(0, connection.DisposeCount);
        }

        [TestMethod]
        public async Task TestExecuteScalarAsyncUntypedObservesCancellation()
        {
            // Setup
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();
            using var cancellation = new CancellationTokenSource();
            cancellation.Cancel();

            // Act
            Func<Task> act = async () => await repository.ExecuteScalarAsync("SELECT Id, Name", param: new { Id = 1 }, cancellationToken: cancellation.Token).ConfigureAwait(false);

            // Assert
            await Assert.ThrowsAsync<OperationCanceledException>(act).ConfigureAwait(false);
            Assert.AreEqual(0, connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestExecuteScalarAsyncUntypedReturnsCachedResult()
        {
            // Setup
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();
            var first = await repository.ExecuteScalarAsync("SELECT Id, Name", param: new { Id = 1 }, cacheKey: "raw-result");
            connection.Failure = new InvalidOperationException("Cache should avoid execution.");

            // Act
            var actual = await repository.ExecuteScalarAsync("SELECT Id, Name", param: new { Id = 1 }, cacheKey: "raw-result");

            // Assert
            Assert.AreEqual(1, actual);
            Assert.AreEqual(1, connection.ExecutionCount);
        }

        [TestMethod]
        public void TestExecuteScalarTypedReturnsCommandResult()
        {
            // Setup
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();

            // Act
            var actual = repository.ExecuteScalar<int>("SELECT Id, Name", param: new { Id = 1 });

            // Assert
            Assert.AreEqual(1, actual);
            Assert.AreEqual(1, connection.ExecutionCount);
        }

        [TestMethod]
        public void TestExecuteScalarTypedPreservesCommandFailure()
        {
            // Setup
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();
            var failure = new InvalidOperationException("Command failed.");
            connection.Failure = failure;

            // Act
            Action act = () => repository.ExecuteScalar<int>("SELECT Id, Name", param: new { Id = 1 });

            // Assert
            var actual = Assert.ThrowsExactly<InvalidOperationException>(act);
            Assert.AreSame(failure, actual);
        }

        [TestMethod]
        public void TestExecuteScalarTypedUsesCallerTransaction()
        {
            // Setup
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();
            using var transaction = connection.BeginTransaction();

            // Act
            var actual = repository.ExecuteScalar<int>("SELECT Id, Name", param: new { Id = 1 }, transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.AreEqual(0, connection.DisposeCount);
        }

        [TestMethod]
        public void TestExecuteScalarTypedReturnsCachedResult()
        {
            // Setup
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();
            var first = repository.ExecuteScalar<int>("SELECT Id, Name", param: new { Id = 1 }, cacheKey: "raw-result");
            connection.Failure = new InvalidOperationException("Cache should avoid execution.");

            // Act
            var actual = repository.ExecuteScalar<int>("SELECT Id, Name", param: new { Id = 1 }, cacheKey: "raw-result");

            // Assert
            Assert.AreEqual(1, actual);
            Assert.AreEqual(1, connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestExecuteScalarAsyncTypedReturnsCommandResult()
        {
            // Setup
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();

            // Act
            var actual = await repository.ExecuteScalarAsync<int>("SELECT Id, Name", param: new { Id = 1 });

            // Assert
            Assert.AreEqual(1, actual);
            Assert.AreEqual(1, connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestExecuteScalarAsyncTypedPreservesCommandFailure()
        {
            // Setup
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();
            var failure = new InvalidOperationException("Command failed.");
            connection.Failure = failure;

            // Act
            Func<Task> act = async () => await repository.ExecuteScalarAsync<int>("SELECT Id, Name", param: new { Id = 1 }).ConfigureAwait(false);

            // Assert
            var actual = await Assert.ThrowsExactlyAsync<InvalidOperationException>(act).ConfigureAwait(false);
            Assert.AreSame(failure, actual);
        }

        [TestMethod]
        public async Task TestExecuteScalarAsyncTypedUsesCallerTransaction()
        {
            // Setup
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();
            using var transaction = connection.BeginTransaction();

            // Act
            var actual = await repository.ExecuteScalarAsync<int>("SELECT Id, Name", param: new { Id = 1 }, transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.AreEqual(0, connection.DisposeCount);
        }

        [TestMethod]
        public async Task TestExecuteScalarAsyncTypedObservesCancellation()
        {
            // Setup
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();
            using var cancellation = new CancellationTokenSource();
            cancellation.Cancel();

            // Act
            Func<Task> act = async () => await repository.ExecuteScalarAsync<int>("SELECT Id, Name", param: new { Id = 1 }, cancellationToken: cancellation.Token).ConfigureAwait(false);

            // Assert
            await Assert.ThrowsAsync<OperationCanceledException>(act).ConfigureAwait(false);
            Assert.AreEqual(0, connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestExecuteScalarAsyncTypedReturnsCachedResult()
        {
            // Setup
            using var repository = new CoverageRepository();
            var connection = repository.CreateConnection();
            var first = await repository.ExecuteScalarAsync<int>("SELECT Id, Name", param: new { Id = 1 }, cacheKey: "raw-result");
            connection.Failure = new InvalidOperationException("Cache should avoid execution.");

            // Act
            var actual = await repository.ExecuteScalarAsync<int>("SELECT Id, Name", param: new { Id = 1 }, cacheKey: "raw-result");

            // Assert
            Assert.AreEqual(1, actual);
            Assert.AreEqual(1, connection.ExecutionCount);
        }

    }
}
