#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RepoDb.Interfaces;
using RepoDb.Schema.Core.UnitTests.CustomObjects;

namespace RepoDb.Schema.Core.UnitTests
{
    [TestClass]
    public class CreateSchemaExtensionTest
    {
        #region Helpers

        private const string ExistsSql = "SELECT COUNT(*) FROM schemas WHERE name = {0}";
        private const string CreateSql = "CREATE SCHEMA {0}";

        private static string Quote(string name) =>
            "[" + name.Replace("]", "]]") + "]";

        private static CustomDbConnection Connection(object count)
        {
            return new CustomDbConnection { ScalarResult = _ => count };
        }

        private static bool Create(CustomDbConnection connection,
            string schemaName,
            int? commandTimeout = null,
            string traceKey = "MyTraceKey",
            ITrace trace = null,
            System.Data.IDbTransaction transaction = null) =>
            CreateSchemaExtension.CreateSchema(connection, schemaName, "@", ExistsSql, CreateSql, Quote, commandTimeout, traceKey, trace, transaction);

        private static Task<bool> CreateAsync(CustomDbConnection connection,
            string schemaName,
            CancellationToken cancellationToken = default,
            int? commandTimeout = null,
            string traceKey = "MyTraceKey",
            ITrace trace = null,
            System.Data.IDbTransaction transaction = null) =>
            CreateSchemaExtension.CreateSchemaAsync(connection, schemaName, "@", ExistsSql, CreateSql, Quote, commandTimeout, traceKey, trace, transaction, cancellationToken);

        #endregion

        #region Create

        [TestMethod]
        public void TestCreateCreatesTheSchemaIfItDoesNotExist()
        {
            // Setup
            var connection = Connection(0);

            // Act
            var actual = Create(connection, "Sales");

            // Assert
            Assert.IsTrue(actual);
            CollectionAssert.AreEqual(new[] { "CREATE SCHEMA [Sales]" }, connection.ExecutedCommands);
        }

        [TestMethod]
        public void TestCreateDoesNotCreateTheSchemaIfItExists()
        {
            // Setup
            var connection = Connection(1);

            // Act
            var actual = Create(connection, "Sales");

            // Assert
            Assert.IsFalse(actual);
            Assert.AreEqual(0, connection.ExecutedCommands.Count);
        }

        [DataTestMethod]
        [DataRow(1)]
        [DataRow(1L)]
        [DataRow(3)]
        [DataRow("2")]
        public void TestCreateTakesAnyPositiveCountAsExisting(object count)
        {
            // Setup
            var connection = Connection(count);

            // Act/Assert
            Assert.IsFalse(Create(connection, "Sales"));
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow(0)]
        [DataRow(0L)]
        public void TestCreateTakesNoRowsOrAZeroCountAsNotExisting(object count)
        {
            // Setup
            var connection = Connection(count);

            // Act/Assert
            Assert.IsTrue(Create(connection, "Sales"));
        }

        [TestMethod]
        public void TestCreateChecksTheNameWithAParameterWithThePrefixOfTheProvider()
        {
            // Setup
            var connection = Connection(0);

            // Act
            Create(connection, "Sales");

            // Assert
            var probe = connection.Details.First();
            Assert.AreEqual("SELECT COUNT(*) FROM schemas WHERE name = @SchemaName", probe.CommandText);
            Assert.AreEqual("Sales", probe.Parameters.Single().Value);
        }

        [TestMethod]
        public void TestCreateQuotesTheNameOfTheSchemaAndEscapesTheQuotesInIt()
        {
            // Setup
            var connection = Connection(0);

            // Act
            Create(connection, "Sa]les; DROP TABLE x");

            // Assert
            CollectionAssert.AreEqual(new[] { "CREATE SCHEMA [Sa]]les; DROP TABLE x]" }, connection.ExecutedCommands);
            Assert.AreEqual("Sa]les; DROP TABLE x", connection.Details.First().Parameters.Single().Value);
        }

        [TestMethod]
        public void TestCreateUsesTheCommandTimeoutAndTheTransactionForBothStatements()
        {
            // Setup
            var connection = Connection(0);
            var transaction = connection.BeginTransaction();

            // Act
            Create(connection, "Sales", commandTimeout: 45, transaction: transaction);

            // Assert
            Assert.AreEqual(2, connection.Details.Count);
            Assert.IsTrue(connection.Details.All(detail => detail.CommandTimeout == 45));
            Assert.IsTrue(connection.Details.All(detail => ReferenceEquals(detail.Transaction, transaction)));
        }

        [TestMethod]
        public void TestCreateTracesBothStatementsWithTheTraceKey()
        {
            // Setup
            var connection = Connection(0);
            var trace = new Mock<ITrace>();

            // Act
            Create(connection, "Sales", trace: trace.Object);

            // Assert
            trace.Verify(t => t.BeforeExecution(It.Is<CancellableTraceLog>(l => l.Key == "MyTraceKey")), Times.Exactly(2));
        }

        [TestMethod]
        public void TestCreateTracesOnlyTheCheckIfTheSchemaExists()
        {
            // Setup
            var connection = Connection(1);
            var trace = new Mock<ITrace>();

            // Act
            Create(connection, "Sales", trace: trace.Object);

            // Assert
            trace.Verify(t => t.BeforeExecution(It.IsAny<CancellableTraceLog>()), Times.Once);
        }

        [TestMethod]
        public void ThrowExceptionOnCreateIfTheConnectionIsNull()
        {
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => CreateSchemaExtension.CreateSchema(null, "Sales", "@", ExistsSql, CreateSql, Quote, null, "Key", null, null));
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("   ")]
        public void ThrowExceptionOnCreateIfTheNameOfTheSchemaIsNullOrWhiteSpace(string schemaName)
        {
            // Setup
            var connection = Connection(0);

            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => Create(connection, schemaName));
            Assert.AreEqual(0, connection.Details.Count);
        }

        [TestMethod]
        public void ThrowExceptionOnCreateIfTheCreateStatementFails()
        {
            // Setup
            var connection = Connection(0);
            connection.FailWhen = text => text.StartsWith("CREATE", StringComparison.Ordinal);

            // Act/Assert
            Assert.Throws<InvalidOperationException>(() => Create(connection, "Sales"));
        }

        #endregion

        #region Create Async

        [TestMethod]
        public async Task TestCreateAsyncCreatesTheSchemaIfItDoesNotExist()
        {
            // Setup
            var connection = Connection(0);

            // Act
            var actual = await CreateAsync(connection, "Sales");

            // Assert
            Assert.IsTrue(actual);
            CollectionAssert.AreEqual(new[] { "CREATE SCHEMA [Sales]" }, connection.ExecutedCommands);
        }

        [TestMethod]
        public async Task TestCreateAsyncDoesNotCreateTheSchemaIfItExists()
        {
            // Setup
            var connection = Connection(1);

            // Act
            var actual = await CreateAsync(connection, "Sales");

            // Assert
            Assert.IsFalse(actual);
            Assert.AreEqual(0, connection.ExecutedCommands.Count);
        }

        [TestMethod]
        public async Task TestCreateAsyncUsesTheCommandTimeoutAndTheTransactionForBothStatements()
        {
            // Setup
            var connection = Connection(0);
            var transaction = connection.BeginTransaction();

            // Act
            await CreateAsync(connection, "Sales", commandTimeout: 12, transaction: transaction);

            // Assert
            Assert.AreEqual(2, connection.Details.Count);
            Assert.IsTrue(connection.Details.All(detail => detail.CommandTimeout == 12 && ReferenceEquals(detail.Transaction, transaction)));
        }

        [TestMethod]
        public async Task ThrowExceptionOnCreateAsyncIfTheConnectionIsNullOrTheNameIsEmpty()
        {
            // Act/Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => CreateSchemaExtension.CreateSchemaAsync(null, "Sales", "@", ExistsSql, CreateSql, Quote, null, "Key", null, null, default));
            await Assert.ThrowsAsync<ArgumentNullException>(() => CreateAsync(Connection(0), " "));
        }

        [TestMethod]
        public async Task ThrowExceptionOnCreateAsyncIfTheTokenIsCancelled()
        {
            // Setup
            var connection = Connection(0);
            using (var source = new CancellationTokenSource())
            {
                source.Cancel();

                // Act/Assert
                await Assert.ThrowsAsync<OperationCanceledException>(() => CreateAsync(connection, "Sales", source.Token));
                Assert.AreEqual(0, connection.ExecutedCommands.Count);
            }
        }

        #endregion
    }
}
