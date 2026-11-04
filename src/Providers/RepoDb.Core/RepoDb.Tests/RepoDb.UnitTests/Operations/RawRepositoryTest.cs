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
    public class RawRepositoryTest : RepositoryOperationTestBase
    {
        [TestMethod]
        public void TestExecuteQueryUntypedReturnsCommandResult()
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();

            // Act
            var actual = repository.ExecuteQuery("SELECT Id, Name", param: new { Id = 1 });

            // Assert
            Assert.AreEqual(1, (int)((IDictionary<string, object>)actual.Single())["Id"]);
            Assert.AreEqual(1, connection.ExecutionCount);
        }

        [TestMethod]
        public void TestExecuteQueryUntypedPreservesCommandFailure()
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
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
        public void TestExecuteQueryUntypedUsesCallerTransaction()
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = connection.BeginTransaction();

            // Act
            var actual = repository.ExecuteQuery("SELECT Id, Name", param: new { Id = 1 }, transaction: transaction);

            // Assert
            Assert.AreEqual(1, (int)((IDictionary<string, object>)actual.Single())["Id"]);
            Assert.AreEqual(0, connection.DisposeCount);
        }

        [TestMethod]
        public void TestExecuteQueryUntypedReturnsCachedResult()
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            var first = repository.ExecuteQuery("SELECT Id, Name", param: new { Id = 1 }, cacheKey: "raw-result");
            connection.Failure = new InvalidOperationException("Cache should avoid execution.");

            // Act
            var actual = repository.ExecuteQuery("SELECT Id, Name", param: new { Id = 1 }, cacheKey: "raw-result");

            // Assert
            Assert.AreEqual(1, (int)((IDictionary<string, object>)actual.Single())["Id"]);
            Assert.AreEqual(1, connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestExecuteQueryAsyncUntypedReturnsCommandResult()
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();

            // Act
            var actual = await repository.ExecuteQueryAsync("SELECT Id, Name", param: new { Id = 1 });

            // Assert
            Assert.AreEqual(1, (int)((IDictionary<string, object>)actual.Single())["Id"]);
            Assert.AreEqual(1, connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestExecuteQueryAsyncUntypedPreservesCommandFailure()
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
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
        public async Task TestExecuteQueryAsyncUntypedUsesCallerTransaction()
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = connection.BeginTransaction();

            // Act
            var actual = await repository.ExecuteQueryAsync("SELECT Id, Name", param: new { Id = 1 }, transaction: transaction);

            // Assert
            Assert.AreEqual(1, (int)((IDictionary<string, object>)actual.Single())["Id"]);
            Assert.AreEqual(0, connection.DisposeCount);
        }

        [TestMethod]
        public async Task TestExecuteQueryAsyncUntypedObservesCancellation()
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
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
        public async Task TestExecuteQueryAsyncUntypedReturnsCachedResult()
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            var first = await repository.ExecuteQueryAsync("SELECT Id, Name", param: new { Id = 1 }, cacheKey: "raw-result");
            connection.Failure = new InvalidOperationException("Cache should avoid execution.");

            // Act
            var actual = await repository.ExecuteQueryAsync("SELECT Id, Name", param: new { Id = 1 }, cacheKey: "raw-result");

            // Assert
            Assert.AreEqual(1, (int)((IDictionary<string, object>)actual.Single())["Id"]);
            Assert.AreEqual(1, connection.ExecutionCount);
        }

        [TestMethod]
        public void TestExecuteQueryTypedReturnsCommandResult()
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();

            // Act
            var actual = repository.ExecuteQuery<CoverageEntity>("SELECT Id, Name", param: new { Id = 1 });

            // Assert
            Assert.AreEqual(1, actual.Single().Id);
            Assert.AreEqual(1, connection.ExecutionCount);
        }

        [TestMethod]
        public void TestExecuteQueryTypedPreservesCommandFailure()
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            var failure = new InvalidOperationException("Command failed.");
            connection.Failure = failure;

            // Act
            Action act = () => repository.ExecuteQuery<CoverageEntity>("SELECT Id, Name", param: new { Id = 1 });

            // Assert
            var actual = Assert.ThrowsExactly<InvalidOperationException>(act);
            Assert.AreSame(failure, actual);
        }

        [TestMethod]
        public void TestExecuteQueryTypedUsesCallerTransaction()
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = connection.BeginTransaction();

            // Act
            var actual = repository.ExecuteQuery<CoverageEntity>("SELECT Id, Name", param: new { Id = 1 }, transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Single().Id);
            Assert.AreEqual(0, connection.DisposeCount);
        }

        [TestMethod]
        public void TestExecuteQueryTypedReturnsCachedResult()
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            var first = repository.ExecuteQuery<CoverageEntity>("SELECT Id, Name", param: new { Id = 1 }, cacheKey: "raw-result");
            connection.Failure = new InvalidOperationException("Cache should avoid execution.");

            // Act
            var actual = repository.ExecuteQuery<CoverageEntity>("SELECT Id, Name", param: new { Id = 1 }, cacheKey: "raw-result");

            // Assert
            Assert.AreEqual(1, actual.Single().Id);
            Assert.AreEqual(1, connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestExecuteQueryAsyncTypedReturnsCommandResult()
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();

            // Act
            var actual = await repository.ExecuteQueryAsync<CoverageEntity>("SELECT Id, Name", param: new { Id = 1 });

            // Assert
            Assert.AreEqual(1, actual.Single().Id);
            Assert.AreEqual(1, connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestExecuteQueryAsyncTypedPreservesCommandFailure()
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            var failure = new InvalidOperationException("Command failed.");
            connection.Failure = failure;

            // Act
            Func<Task> act = async () => await repository.ExecuteQueryAsync<CoverageEntity>("SELECT Id, Name", param: new { Id = 1 }).ConfigureAwait(false);

            // Assert
            var actual = await Assert.ThrowsExactlyAsync<InvalidOperationException>(act).ConfigureAwait(false);
            Assert.AreSame(failure, actual);
        }

        [TestMethod]
        public async Task TestExecuteQueryAsyncTypedUsesCallerTransaction()
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = connection.BeginTransaction();

            // Act
            var actual = await repository.ExecuteQueryAsync<CoverageEntity>("SELECT Id, Name", param: new { Id = 1 }, transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Single().Id);
            Assert.AreEqual(0, connection.DisposeCount);
        }

        [TestMethod]
        public async Task TestExecuteQueryAsyncTypedObservesCancellation()
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var cancellation = new CancellationTokenSource();
            cancellation.Cancel();

            // Act
            Func<Task> act = async () => await repository.ExecuteQueryAsync<CoverageEntity>("SELECT Id, Name", param: new { Id = 1 }, cancellationToken: cancellation.Token).ConfigureAwait(false);

            // Assert
            await Assert.ThrowsAsync<OperationCanceledException>(act).ConfigureAwait(false);
            Assert.AreEqual(0, connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestExecuteQueryAsyncTypedReturnsCachedResult()
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            var first = await repository.ExecuteQueryAsync<CoverageEntity>("SELECT Id, Name", param: new { Id = 1 }, cacheKey: "raw-result");
            connection.Failure = new InvalidOperationException("Cache should avoid execution.");

            // Act
            var actual = await repository.ExecuteQueryAsync<CoverageEntity>("SELECT Id, Name", param: new { Id = 1 }, cacheKey: "raw-result");

            // Assert
            Assert.AreEqual(1, actual.Single().Id);
            Assert.AreEqual(1, connection.ExecutionCount);
        }

        [TestMethod]
        public void TestExecuteQueryFirstUntypedReturnsCommandResult()
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();

            // Act
            var actual = repository.ExecuteQueryFirst("SELECT Id, Name", param: new { Id = 1 });

            // Assert
            Assert.AreEqual(1, (int)((IDictionary<string, object>)actual)["Id"]);
            Assert.AreEqual(1, connection.ExecutionCount);
        }

        [TestMethod]
        public void TestExecuteQueryFirstUntypedPreservesCommandFailure()
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
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
        public void TestExecuteQueryFirstUntypedUsesCallerTransaction()
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = connection.BeginTransaction();

            // Act
            var actual = repository.ExecuteQueryFirst("SELECT Id, Name", param: new { Id = 1 }, transaction: transaction);

            // Assert
            Assert.AreEqual(1, (int)((IDictionary<string, object>)actual)["Id"]);
            Assert.AreEqual(0, connection.DisposeCount);
        }

        [TestMethod]
        public void TestExecuteQueryFirstUntypedReturnsCachedResult()
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            var first = repository.ExecuteQueryFirst("SELECT Id, Name", param: new { Id = 1 }, cacheKey: "raw-result");
            connection.Failure = new InvalidOperationException("Cache should avoid execution.");

            // Act
            var actual = repository.ExecuteQueryFirst("SELECT Id, Name", param: new { Id = 1 }, cacheKey: "raw-result");

            // Assert
            Assert.AreEqual(1, (int)((IDictionary<string, object>)actual)["Id"]);
            Assert.AreEqual(1, connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestExecuteQueryFirstAsyncUntypedReturnsCommandResult()
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();

            // Act
            var actual = await repository.ExecuteQueryFirstAsync("SELECT Id, Name", param: new { Id = 1 });

            // Assert
            Assert.AreEqual(1, (int)((IDictionary<string, object>)actual)["Id"]);
            Assert.AreEqual(1, connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestExecuteQueryFirstAsyncUntypedPreservesCommandFailure()
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
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
        public async Task TestExecuteQueryFirstAsyncUntypedUsesCallerTransaction()
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = connection.BeginTransaction();

            // Act
            var actual = await repository.ExecuteQueryFirstAsync("SELECT Id, Name", param: new { Id = 1 }, transaction: transaction);

            // Assert
            Assert.AreEqual(1, (int)((IDictionary<string, object>)actual)["Id"]);
            Assert.AreEqual(0, connection.DisposeCount);
        }

        [TestMethod]
        public async Task TestExecuteQueryFirstAsyncUntypedObservesCancellation()
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
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
        public async Task TestExecuteQueryFirstAsyncUntypedReturnsCachedResult()
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            var first = await repository.ExecuteQueryFirstAsync("SELECT Id, Name", param: new { Id = 1 }, cacheKey: "raw-result");
            connection.Failure = new InvalidOperationException("Cache should avoid execution.");

            // Act
            var actual = await repository.ExecuteQueryFirstAsync("SELECT Id, Name", param: new { Id = 1 }, cacheKey: "raw-result");

            // Assert
            Assert.AreEqual(1, (int)((IDictionary<string, object>)actual)["Id"]);
            Assert.AreEqual(1, connection.ExecutionCount);
        }

        [TestMethod]
        public void TestExecuteQueryFirstTypedReturnsCommandResult()
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();

            // Act
            var actual = repository.ExecuteQueryFirst<CoverageEntity>("SELECT Id, Name", param: new { Id = 1 });

            // Assert
            Assert.AreEqual(1, actual.Id);
            Assert.AreEqual(1, connection.ExecutionCount);
        }

        [TestMethod]
        public void TestExecuteQueryFirstTypedPreservesCommandFailure()
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            var failure = new InvalidOperationException("Command failed.");
            connection.Failure = failure;

            // Act
            Action act = () => repository.ExecuteQueryFirst<CoverageEntity>("SELECT Id, Name", param: new { Id = 1 });

            // Assert
            var actual = Assert.ThrowsExactly<InvalidOperationException>(act);
            Assert.AreSame(failure, actual);
        }

        [TestMethod]
        public void TestExecuteQueryFirstTypedUsesCallerTransaction()
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = connection.BeginTransaction();

            // Act
            var actual = repository.ExecuteQueryFirst<CoverageEntity>("SELECT Id, Name", param: new { Id = 1 }, transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Id);
            Assert.AreEqual(0, connection.DisposeCount);
        }

        [TestMethod]
        public void TestExecuteQueryFirstTypedReturnsCachedResult()
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            var first = repository.ExecuteQueryFirst<CoverageEntity>("SELECT Id, Name", param: new { Id = 1 }, cacheKey: "raw-result");
            connection.Failure = new InvalidOperationException("Cache should avoid execution.");

            // Act
            var actual = repository.ExecuteQueryFirst<CoverageEntity>("SELECT Id, Name", param: new { Id = 1 }, cacheKey: "raw-result");

            // Assert
            Assert.AreEqual(1, actual.Id);
            Assert.AreEqual(1, connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestExecuteQueryFirstAsyncTypedReturnsCommandResult()
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();

            // Act
            var actual = await repository.ExecuteQueryFirstAsync<CoverageEntity>("SELECT Id, Name", param: new { Id = 1 });

            // Assert
            Assert.AreEqual(1, actual.Id);
            Assert.AreEqual(1, connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestExecuteQueryFirstAsyncTypedPreservesCommandFailure()
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            var failure = new InvalidOperationException("Command failed.");
            connection.Failure = failure;

            // Act
            Func<Task> act = async () => await repository.ExecuteQueryFirstAsync<CoverageEntity>("SELECT Id, Name", param: new { Id = 1 }).ConfigureAwait(false);

            // Assert
            var actual = await Assert.ThrowsExactlyAsync<InvalidOperationException>(act).ConfigureAwait(false);
            Assert.AreSame(failure, actual);
        }

        [TestMethod]
        public async Task TestExecuteQueryFirstAsyncTypedUsesCallerTransaction()
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = connection.BeginTransaction();

            // Act
            var actual = await repository.ExecuteQueryFirstAsync<CoverageEntity>("SELECT Id, Name", param: new { Id = 1 }, transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Id);
            Assert.AreEqual(0, connection.DisposeCount);
        }

        [TestMethod]
        public async Task TestExecuteQueryFirstAsyncTypedObservesCancellation()
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var cancellation = new CancellationTokenSource();
            cancellation.Cancel();

            // Act
            Func<Task> act = async () => await repository.ExecuteQueryFirstAsync<CoverageEntity>("SELECT Id, Name", param: new { Id = 1 }, cancellationToken: cancellation.Token).ConfigureAwait(false);

            // Assert
            await Assert.ThrowsAsync<OperationCanceledException>(act).ConfigureAwait(false);
            Assert.AreEqual(0, connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestExecuteQueryFirstAsyncTypedReturnsCachedResult()
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            var first = await repository.ExecuteQueryFirstAsync<CoverageEntity>("SELECT Id, Name", param: new { Id = 1 }, cacheKey: "raw-result");
            connection.Failure = new InvalidOperationException("Cache should avoid execution.");

            // Act
            var actual = await repository.ExecuteQueryFirstAsync<CoverageEntity>("SELECT Id, Name", param: new { Id = 1 }, cacheKey: "raw-result");

            // Assert
            Assert.AreEqual(1, actual.Id);
            Assert.AreEqual(1, connection.ExecutionCount);
        }

        [TestMethod]
        public void TestExecuteQuerySingleUntypedReturnsCommandResult()
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();

            // Act
            var actual = repository.ExecuteQuerySingle("SELECT Id, Name", param: new { Id = 1 });

            // Assert
            Assert.AreEqual(1, (int)((IDictionary<string, object>)actual)["Id"]);
            Assert.AreEqual(1, connection.ExecutionCount);
        }

        [TestMethod]
        public void TestExecuteQuerySingleUntypedPreservesCommandFailure()
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
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
        public void TestExecuteQuerySingleUntypedUsesCallerTransaction()
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = connection.BeginTransaction();

            // Act
            var actual = repository.ExecuteQuerySingle("SELECT Id, Name", param: new { Id = 1 }, transaction: transaction);

            // Assert
            Assert.AreEqual(1, (int)((IDictionary<string, object>)actual)["Id"]);
            Assert.AreEqual(0, connection.DisposeCount);
        }

        [TestMethod]
        public void TestExecuteQuerySingleUntypedReturnsCachedResult()
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            var first = repository.ExecuteQuerySingle("SELECT Id, Name", param: new { Id = 1 }, cacheKey: "raw-result");
            connection.Failure = new InvalidOperationException("Cache should avoid execution.");

            // Act
            var actual = repository.ExecuteQuerySingle("SELECT Id, Name", param: new { Id = 1 }, cacheKey: "raw-result");

            // Assert
            Assert.AreEqual(1, (int)((IDictionary<string, object>)actual)["Id"]);
            Assert.AreEqual(1, connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestExecuteQuerySingleAsyncUntypedReturnsCommandResult()
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();

            // Act
            var actual = await repository.ExecuteQuerySingleAsync("SELECT Id, Name", param: new { Id = 1 });

            // Assert
            Assert.AreEqual(1, (int)((IDictionary<string, object>)actual)["Id"]);
            Assert.AreEqual(1, connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestExecuteQuerySingleAsyncUntypedPreservesCommandFailure()
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
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
        public async Task TestExecuteQuerySingleAsyncUntypedUsesCallerTransaction()
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = connection.BeginTransaction();

            // Act
            var actual = await repository.ExecuteQuerySingleAsync("SELECT Id, Name", param: new { Id = 1 }, transaction: transaction);

            // Assert
            Assert.AreEqual(1, (int)((IDictionary<string, object>)actual)["Id"]);
            Assert.AreEqual(0, connection.DisposeCount);
        }

        [TestMethod]
        public async Task TestExecuteQuerySingleAsyncUntypedObservesCancellation()
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
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
        public async Task TestExecuteQuerySingleAsyncUntypedReturnsCachedResult()
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            var first = await repository.ExecuteQuerySingleAsync("SELECT Id, Name", param: new { Id = 1 }, cacheKey: "raw-result");
            connection.Failure = new InvalidOperationException("Cache should avoid execution.");

            // Act
            var actual = await repository.ExecuteQuerySingleAsync("SELECT Id, Name", param: new { Id = 1 }, cacheKey: "raw-result");

            // Assert
            Assert.AreEqual(1, (int)((IDictionary<string, object>)actual)["Id"]);
            Assert.AreEqual(1, connection.ExecutionCount);
        }

        [TestMethod]
        public void TestExecuteQuerySingleTypedReturnsCommandResult()
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();

            // Act
            var actual = repository.ExecuteQuerySingle<CoverageEntity>("SELECT Id, Name", param: new { Id = 1 });

            // Assert
            Assert.AreEqual(1, actual.Id);
            Assert.AreEqual(1, connection.ExecutionCount);
        }

        [TestMethod]
        public void TestExecuteQuerySingleTypedPreservesCommandFailure()
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            var failure = new InvalidOperationException("Command failed.");
            connection.Failure = failure;

            // Act
            Action act = () => repository.ExecuteQuerySingle<CoverageEntity>("SELECT Id, Name", param: new { Id = 1 });

            // Assert
            var actual = Assert.ThrowsExactly<InvalidOperationException>(act);
            Assert.AreSame(failure, actual);
        }

        [TestMethod]
        public void TestExecuteQuerySingleTypedUsesCallerTransaction()
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = connection.BeginTransaction();

            // Act
            var actual = repository.ExecuteQuerySingle<CoverageEntity>("SELECT Id, Name", param: new { Id = 1 }, transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Id);
            Assert.AreEqual(0, connection.DisposeCount);
        }

        [TestMethod]
        public void TestExecuteQuerySingleTypedReturnsCachedResult()
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            var first = repository.ExecuteQuerySingle<CoverageEntity>("SELECT Id, Name", param: new { Id = 1 }, cacheKey: "raw-result");
            connection.Failure = new InvalidOperationException("Cache should avoid execution.");

            // Act
            var actual = repository.ExecuteQuerySingle<CoverageEntity>("SELECT Id, Name", param: new { Id = 1 }, cacheKey: "raw-result");

            // Assert
            Assert.AreEqual(1, actual.Id);
            Assert.AreEqual(1, connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestExecuteQuerySingleAsyncTypedReturnsCommandResult()
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();

            // Act
            var actual = await repository.ExecuteQuerySingleAsync<CoverageEntity>("SELECT Id, Name", param: new { Id = 1 });

            // Assert
            Assert.AreEqual(1, actual.Id);
            Assert.AreEqual(1, connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestExecuteQuerySingleAsyncTypedPreservesCommandFailure()
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            var failure = new InvalidOperationException("Command failed.");
            connection.Failure = failure;

            // Act
            Func<Task> act = async () => await repository.ExecuteQuerySingleAsync<CoverageEntity>("SELECT Id, Name", param: new { Id = 1 }).ConfigureAwait(false);

            // Assert
            var actual = await Assert.ThrowsExactlyAsync<InvalidOperationException>(act).ConfigureAwait(false);
            Assert.AreSame(failure, actual);
        }

        [TestMethod]
        public async Task TestExecuteQuerySingleAsyncTypedUsesCallerTransaction()
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = connection.BeginTransaction();

            // Act
            var actual = await repository.ExecuteQuerySingleAsync<CoverageEntity>("SELECT Id, Name", param: new { Id = 1 }, transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Id);
            Assert.AreEqual(0, connection.DisposeCount);
        }

        [TestMethod]
        public async Task TestExecuteQuerySingleAsyncTypedObservesCancellation()
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var cancellation = new CancellationTokenSource();
            cancellation.Cancel();

            // Act
            Func<Task> act = async () => await repository.ExecuteQuerySingleAsync<CoverageEntity>("SELECT Id, Name", param: new { Id = 1 }, cancellationToken: cancellation.Token).ConfigureAwait(false);

            // Assert
            await Assert.ThrowsAsync<OperationCanceledException>(act).ConfigureAwait(false);
            Assert.AreEqual(0, connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestExecuteQuerySingleAsyncTypedReturnsCachedResult()
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            var first = await repository.ExecuteQuerySingleAsync<CoverageEntity>("SELECT Id, Name", param: new { Id = 1 }, cacheKey: "raw-result");
            connection.Failure = new InvalidOperationException("Cache should avoid execution.");

            // Act
            var actual = await repository.ExecuteQuerySingleAsync<CoverageEntity>("SELECT Id, Name", param: new { Id = 1 }, cacheKey: "raw-result");

            // Assert
            Assert.AreEqual(1, actual.Id);
            Assert.AreEqual(1, connection.ExecutionCount);
        }

        [TestMethod]
        public void TestExecuteNonQueryUntypedReturnsCommandResult()
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
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
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
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
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
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
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
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
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
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
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
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
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
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
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
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
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
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
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
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
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
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
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
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
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
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
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
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
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
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
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
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
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
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
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
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
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
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
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
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
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
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
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
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
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
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
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
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
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            var first = await repository.ExecuteScalarAsync<int>("SELECT Id, Name", param: new { Id = 1 }, cacheKey: "raw-result");
            connection.Failure = new InvalidOperationException("Cache should avoid execution.");

            // Act
            var actual = await repository.ExecuteScalarAsync<int>("SELECT Id, Name", param: new { Id = 1 }, cacheKey: "raw-result");

            // Assert
            Assert.AreEqual(1, actual);
            Assert.AreEqual(1, connection.ExecutionCount);
        }

        [TestMethod]
        public void TestExecuteQueryMultipleUntypedReturnsCommandResult()
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();

            // Act
            using var actual = repository.ExecuteQueryMultiple("SELECT Id, Name", param: new { Id = 1 });

            // Assert
            Assert.AreEqual(1, actual.Extract<CoverageEntity>(false).Single().Id);
            Assert.AreEqual(1, connection.ExecutionCount);
        }

        [TestMethod]
        public void TestExecuteQueryMultipleUntypedPreservesCommandFailure()
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            var failure = new InvalidOperationException("Command failed.");
            connection.Failure = failure;

            // Act
            Action act = () => repository.ExecuteQueryMultiple("SELECT Id, Name", param: new { Id = 1 });

            // Assert
            var actual = Assert.ThrowsExactly<InvalidOperationException>(act);
            Assert.AreSame(failure, actual);
        }

        [TestMethod]
        public void TestExecuteQueryMultipleUntypedUsesCallerTransaction()
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = connection.BeginTransaction();

            // Act
            using var actual = repository.ExecuteQueryMultiple("SELECT Id, Name", param: new { Id = 1 }, transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Extract<CoverageEntity>(false).Single().Id);
            Assert.AreEqual(0, connection.DisposeCount);
        }

        [TestMethod]
        public async Task TestExecuteQueryMultipleAsyncUntypedReturnsCommandResult()
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();

            // Act
            using var actual = await repository.ExecuteQueryMultipleAsync("SELECT Id, Name", param: new { Id = 1 });

            // Assert
            Assert.AreEqual(1, actual.Extract<CoverageEntity>(false).Single().Id);
            Assert.AreEqual(1, connection.ExecutionCount);
        }

        [TestMethod]
        public async Task TestExecuteQueryMultipleAsyncUntypedPreservesCommandFailure()
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            var failure = new InvalidOperationException("Command failed.");
            connection.Failure = failure;

            // Act
            Func<Task> act = async () => await repository.ExecuteQueryMultipleAsync("SELECT Id, Name", param: new { Id = 1 }).ConfigureAwait(false);

            // Assert
            var actual = await Assert.ThrowsExactlyAsync<InvalidOperationException>(act).ConfigureAwait(false);
            Assert.AreSame(failure, actual);
        }

        [TestMethod]
        public async Task TestExecuteQueryMultipleAsyncUntypedUsesCallerTransaction()
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var transaction = connection.BeginTransaction();

            // Act
            using var actual = await repository.ExecuteQueryMultipleAsync("SELECT Id, Name", param: new { Id = 1 }, transaction: transaction);

            // Assert
            Assert.AreEqual(1, actual.Extract<CoverageEntity>(false).Single().Id);
            Assert.AreEqual(0, connection.DisposeCount);
        }

        [TestMethod]
        public async Task TestExecuteQueryMultipleAsyncUntypedObservesCancellation()
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            var connection = repository.CreateConnection();
            using var cancellation = new CancellationTokenSource();
            cancellation.Cancel();

            // Act
            Func<Task> act = async () => await repository.ExecuteQueryMultipleAsync("SELECT Id, Name", param: new { Id = 1 }, cancellationToken: cancellation.Token).ConfigureAwait(false);

            // Assert
            await Assert.ThrowsAsync<OperationCanceledException>(act).ConfigureAwait(false);
            Assert.AreEqual(0, connection.ExecutionCount);
        }

    }
}
