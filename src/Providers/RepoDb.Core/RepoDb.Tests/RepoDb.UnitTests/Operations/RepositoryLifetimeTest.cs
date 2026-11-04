#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System.Threading.Tasks;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Enumerations;
using RepoDb.UnitTests.CustomObjects;

namespace RepoDb.UnitTests.Operations
{
    [TestClass]
    [DoNotParallelize]
    public class RepositoryLifetimeTest : RepositoryOperationTestBase
    {
        [TestMethod]
        public void TestCreateConnectionInstance()
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            using var first = repository.CreateConnection();

            // Act
            using var actual = repository.CreateConnection();

            // Assert
            Assert.AreSame(first, actual);
        }

        [TestMethod]
        public void TestForceNewConnectionInstance()
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            using var first = repository.CreateConnection();

            // Act
            using var actual = repository.CreateConnection(true);

            // Assert
            Assert.AreNotSame(first, actual);
            Assert.AreEqual("UnitTest", actual.ConnectionString, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDisposeConnectionInstance()
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            using var first = repository.CreateConnection();

            // Act
            repository.DisposeConnectionForPerCall(first);

            // Assert
            Assert.AreEqual(0, first.DisposeCount);
        }

        [TestMethod]
        public void TestPreserveTransactionConnectionInstance()
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            using var first = repository.CreateConnection();
            using var transaction = first.BeginTransaction();

            // Act
            repository.DisposeConnectionForPerCall(first, transaction);

            // Assert
            Assert.AreEqual(0, first.DisposeCount);
        }

        [TestMethod]
        public void TestDisposeRepositoryInstance()
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);
            using var first = repository.CreateConnection();

            // Act
            repository.Dispose();

            // Assert
            Assert.AreEqual(1, first.DisposeCount);
        }

        [TestMethod]
        public void TestCreateConnectionPerCall()
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.PerCall);
            using var first = repository.CreateConnection();

            // Act
            using var actual = repository.CreateConnection();

            // Assert
            Assert.AreNotSame(first, actual);
        }

        [TestMethod]
        public void TestForceNewConnectionPerCall()
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.PerCall);
            using var first = repository.CreateConnection();

            // Act
            using var actual = repository.CreateConnection(true);

            // Assert
            Assert.AreNotSame(first, actual);
            Assert.AreEqual("UnitTest", actual.ConnectionString, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDisposeConnectionPerCall()
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.PerCall);
            using var first = repository.CreateConnection();

            // Act
            repository.DisposeConnectionForPerCall(first);

            // Assert
            Assert.AreEqual(1, first.DisposeCount);
        }

        [TestMethod]
        public void TestPreserveTransactionConnectionPerCall()
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.PerCall);
            using var first = repository.CreateConnection();
            using var transaction = first.BeginTransaction();

            // Act
            repository.DisposeConnectionForPerCall(first, transaction);

            // Assert
            Assert.AreEqual(0, first.DisposeCount);
        }

        [TestMethod]
        public void TestDisposeRepositoryPerCall()
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.PerCall);
            using var first = repository.CreateConnection();

            // Act
            repository.Dispose();

            // Assert
            Assert.AreEqual(0, first.DisposeCount);
        }

        [TestMethod]
        public void TestDisposeUnusedRepository()
        {
            // Setup
            using var repository = new DbRepository<CoverageDbConnection>("UnitTest", null, ConnectionPersistency.Instance);

            // Act
            repository.Dispose();

            // Assert
            Assert.AreEqual(ConnectionPersistency.Instance, repository.ConnectionPersistency);
        }

        [TestMethod]
        public void TestPerCallSuccess()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            using var repository = new SuppliedConnectionRepository(connection);

            // Act
            var actual = repository.ExecuteNonQuery("UPDATE Table SET Value = 1");

            // Assert
            Assert.AreEqual(1, actual);
            Assert.AreEqual(1, connection.DisposeCount);
        }

        [TestMethod]
        public void TestPerCallFailure()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            using var repository = new SuppliedConnectionRepository(connection);
            var failure = new InvalidOperationException("Command failed.");
            connection.Failure = failure;

            // Act
            Action act = () => repository.ExecuteNonQuery("UPDATE Table SET Value = 1");

            // Assert
            var actual = Assert.ThrowsExactly<InvalidOperationException>(act);
            Assert.AreSame(failure, actual);
            Assert.AreEqual(1, connection.DisposeCount);
        }

        [TestMethod]
        public async Task TestPerCallSuccessAsync()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            using var repository = new SuppliedConnectionRepository(connection);

            // Act
            var actual = await repository.ExecuteNonQueryAsync("UPDATE Table SET Value = 1");

            // Assert
            Assert.AreEqual(1, actual);
            Assert.AreEqual(1, connection.DisposeCount);
        }

        [TestMethod]
        public async Task TestPerCallFailureAsync()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            using var repository = new SuppliedConnectionRepository(connection);
            var failure = new InvalidOperationException("Command failed.");
            connection.Failure = failure;

            // Act
            Func<Task> act = async () => await repository.ExecuteNonQueryAsync("UPDATE Table SET Value = 1").ConfigureAwait(false);

            // Assert
            var actual = await Assert.ThrowsExactlyAsync<InvalidOperationException>(act).ConfigureAwait(false);
            Assert.AreSame(failure, actual);
            Assert.AreEqual(1, connection.DisposeCount);
        }

        [TestMethod]
        public void TestEntityRepositoryForwardsConnectionString()
        {
            // Setup
            using var repository = new ConfiguredEntityRepository();

            // Act
            var actual = repository.ConnectionString;

            // Assert
            Assert.AreEqual("UnitTest", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestEntityRepositoryForwardsCommandTimeout()
        {
            // Setup
            using var repository = new ConfiguredEntityRepository();

            // Act
            var actual = repository.CommandTimeout;

            // Assert
            Assert.AreEqual(17, actual);
        }

        [TestMethod]
        public void TestEntityRepositoryForwardsCacheItemExpiration()
        {
            // Setup
            using var repository = new ConfiguredEntityRepository();

            // Act
            var actual = repository.CacheItemExpiration;

            // Assert
            Assert.AreEqual(23, actual);
        }

        [TestMethod]
        public void TestEntityRepositoryForwardsConnectionPersistency()
        {
            // Setup
            using var repository = new ConfiguredEntityRepository();

            // Act
            var actual = repository.ConnectionPersistency;

            // Assert
            Assert.AreEqual(ConnectionPersistency.Instance, actual);
        }

        [TestMethod]
        public void TestEntityRepositoryForwardsCache()
        {
            // Setup
            using var repository = new ConfiguredEntityRepository();

            // Act
            var actual = repository.Cache;

            // Assert
            Assert.AreSame(repository.DbRepository.Cache, actual);
        }

        [TestMethod]
        public void TestEntityRepositoryForwardsTrace()
        {
            // Setup
            using var repository = new ConfiguredEntityRepository();

            // Act
            var actual = repository.Trace;

            // Assert
            Assert.AreSame(repository.DbRepository.Trace, actual);
        }

        [TestMethod]
        public void TestEntityRepositoryForwardsStatementBuilder()
        {
            // Setup
            using var repository = new ConfiguredEntityRepository();

            // Act
            var actual = repository.StatementBuilder;

            // Assert
            Assert.AreSame(repository.DbRepository.StatementBuilder, actual);
        }

        [TestMethod]
        public void TestEntityRepositoryForcesNewConnection()
        {
            // Setup
            using var repository = new CoverageRepository();
            using var first = repository.CreateConnection();

            // Act
            using var actual = repository.CreateConnection(true);

            // Assert
            Assert.AreNotSame(first, actual);
        }

        private sealed class SuppliedConnectionRepository : DbRepository<CoverageDbConnection>
        {
            private readonly CoverageDbConnection connection;

            public SuppliedConnectionRepository(CoverageDbConnection connection)
                : base("UnitTest")
            {
                this.connection = connection;
            }

            public override CoverageDbConnection CreateConnection() => connection;
        }

        private sealed class ConfiguredEntityRepository : BaseRepository<CoverageEntity, CoverageDbConnection>
        {
            public ConfiguredEntityRepository()
                : base("UnitTest", 17, ConnectionPersistency.Instance, new MemoryCache(), 23,
                    new RecordingTrace(), new CustomStatementBuilder())
            {
            }
        }
    }
}
