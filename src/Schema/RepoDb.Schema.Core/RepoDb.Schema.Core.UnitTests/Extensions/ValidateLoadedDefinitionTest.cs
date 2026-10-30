#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Exceptions;
using RepoDb.Interfaces;
using RepoDb.Schema.Core.UnitTests.CustomObjects;
using RepoDb.Schema.Models;

namespace RepoDb.Schema.Core.UnitTests.Extensions
{
    /// <summary>
    /// Contains the tests of the <c>ValidateDefinition</c> extension methods, which validate a definition that is already loaded.
    /// </summary>
    [TestClass]
    public class ValidateLoadedDefinitionTest
    {
        #region Public Methods

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task ValidateReportsAValidDefinition(bool asynchronous)
        {
            // Setup
            var definition = CreateValidDefinition();

            // Act
            var result = await Validate(definition, asynchronous).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(result.IsValid);
            Assert.AreSame(definition, result.Definition);
            Assert.AreEqual(0, result.Errors.Count);
            Assert.AreEqual(0, result.Warnings.Count);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task ValidateReportsTheStructuralErrors(bool asynchronous)
        {
            // Setup
            var definition = CreateValidDefinition();
            TableSchemaFactory.SetPrimaryKey(definition.Tables[0], "PK_Child", "Missing");
            definition.Tables[0].Indexes.Add(new IndexInfo("IX_Child") { Columns = new[] { "AlsoMissing" } });

            // Act
            var result = await Validate(definition, asynchronous).ConfigureAwait(false);

            // Assert
            Assert.IsFalse(result.IsValid);
            Assert.IsNull(result.Definition);
            Assert.AreEqual(2, result.Errors.Count);
            Assert.IsTrue(result.Errors.Any(error => error.Contains("primary key 'PK_Child'", StringComparison.Ordinal)));
            Assert.IsTrue(result.Errors.Any(error => error.Contains("index 'IX_Child'", StringComparison.Ordinal)));
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task ValidateReportsTheStrictErrors(bool asynchronous)
        {
            // Setup
            var definition = CreateValidDefinition();
            definition.Tables[0].Indexes.Add(new IndexInfo("IX_Same") { Columns = new[] { "Id" } });
            definition.Tables[0].Indexes.Add(new IndexInfo("IX_Same") { Columns = new[] { "Name" } });

            // Act
            var result = await Validate(definition, asynchronous).ConfigureAwait(false);

            // Assert
            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            StringAssert.Contains(result.Errors[0], "index name 'IX_Same' is used more than once", StringComparison.Ordinal);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task ValidateReportsTheWarningsOfAValidDefinition(bool asynchronous)
        {
            // Setup
            var definition = CreateValidDefinition();
            definition.Tables[0].Columns.Add(TableSchemaFactory.CreateColumn("Code", typeof(string), 3, isNullable: true));
            TableSchemaFactory.SetPrimaryKey(definition.Tables[0], "PK_Child", "Code");

            // Act
            var result = await Validate(definition, asynchronous).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(result.IsValid);
            Assert.AreEqual(1, result.Warnings.Count);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task ValidateCanBeCalledMoreThanOnceOnTheSameDefinition(bool asynchronous)
        {
            // Setup
            var definition = CreateValidDefinition();
            TableSchemaFactory.SetPrimaryKey(definition.Tables[0], "PK_Child", "Missing");

            // Act
            var first = await Validate(definition, asynchronous).ConfigureAwait(false);
            var second = await Validate(definition, asynchronous).ConfigureAwait(false);

            // Assert
            CollectionAssert.AreEqual(first.Errors.ToList(), second.Errors.ToList());
            Assert.AreNotSame(first, second);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task ValidateThrowsForANullDefinition(bool asynchronous)
        {
            // Act/Assert
            await Assert.ThrowsExactlyAsync<ArgumentNullException>(() => Validate(null, asynchronous)).ConfigureAwait(false);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task ValidateThrowsForANullConnection(bool asynchronous)
        {
            // Setup
            var definition = CreateValidDefinition();

            // Act/Assert
            if (asynchronous)
            {
                await Assert.ThrowsExactlyAsync<ArgumentNullException>(() => ((IDbConnection)null).ValidateDefinitionAsync(definition)).ConfigureAwait(false);
            }
            else
            {
                Assert.ThrowsExactly<ArgumentNullException>(() => ((IDbConnection)null).ValidateDefinition(definition));
            }
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task ValidateNeverOpensOrQueriesTheConnection(bool asynchronous)
        {
            // Setup
            var definition = CreateValidDefinition();
            using var connection = new CustomDbConnection();

            // Act
            var result = asynchronous
                ? await connection.ValidateDefinitionAsync(definition).ConfigureAwait(false)
                : connection.ValidateDefinition(definition);

            // Assert
            Assert.IsTrue(result.IsValid);
            Assert.AreEqual(ConnectionState.Closed, connection.State);
            Assert.AreEqual(0, connection.Details.Count);
            Assert.AreEqual(0, connection.ExecutedCommands.Count);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task ValidateForwardsTheTraceKeyAndTellsTheTraceThatItCompleted(bool asynchronous)
        {
            // Setup
            var definition = CreateValidDefinition();
            var trace = new RecordingTrace(asynchronous);

            // Act
            await Validate(definition, asynchronous, traceKey: "validate-loaded-test", trace: trace).ConfigureAwait(false);

            // Assert
            Assert.AreEqual(1, trace.Keys.Count);
            Assert.AreEqual("validate-loaded-test", trace.Keys.Single());
            Assert.AreEqual(1, trace.AfterCount);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task ValidateUsesTheDefaultTraceKey(bool asynchronous)
        {
            // Setup
            var definition = CreateValidDefinition();
            var trace = new RecordingTrace(asynchronous);

            // Act
            await Validate(definition, asynchronous, trace: trace).ConfigureAwait(false);

            // Assert
            Assert.AreEqual("ValidateDefinition", trace.Keys.Single(), StringComparer.Ordinal);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task ValidateTellsTheTraceThatItCompletedEvenWhenTheDefinitionIsInvalid(bool asynchronous)
        {
            // Setup
            var definition = CreateValidDefinition();
            TableSchemaFactory.SetPrimaryKey(definition.Tables[0], "PK_Child", "Missing");
            var trace = new RecordingTrace(asynchronous);

            // Act
            var result = await Validate(definition, asynchronous, trace: trace).ConfigureAwait(false);

            // Assert
            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, trace.AfterCount);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task ValidateReturnsNullWhenTheTraceCancels(bool asynchronous)
        {
            // Setup
            var definition = CreateValidDefinition();
            var trace = new RecordingTrace(asynchronous) { Cancel = true };

            // Act
            var result = await Validate(definition, asynchronous, trace: trace).ConfigureAwait(false);

            // Assert
            Assert.IsNull(result);
            Assert.AreEqual(0, trace.AfterCount);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task ValidateThrowsWhenTheTraceCancelsWithAnException(bool asynchronous)
        {
            // Setup
            var definition = CreateValidDefinition();
            var trace = new RecordingTrace(asynchronous) { CancelWithException = true };

            // Act/Assert
            await Assert.ThrowsExactlyAsync<CancelledExecutionException>(() => Validate(definition, asynchronous, trace: trace)).ConfigureAwait(false);
            Assert.AreEqual(0, trace.AfterCount);
        }

        [TestMethod]
        public async Task ValidateAsyncThrowsWhenTheTokenIsCancelled()
        {
            // Setup
            var definition = CreateValidDefinition();
            using var source = new CancellationTokenSource();
            source.Cancel();

            // Act/Assert
            await Assert.ThrowsExactlyAsync<OperationCanceledException>(() => Validate(definition, true, cancellationToken: source.Token)).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task ValidateAsyncThrowsWhenTheTokenIsCancelledAndThereIsATrace()
        {
            // Setup
            var definition = CreateValidDefinition();
            using var source = new CancellationTokenSource();
            source.Cancel();

            // Act/Assert
            await Assert.ThrowsExactlyAsync<OperationCanceledException>(() => Validate(definition, true, trace: new RecordingTrace(true), cancellationToken: source.Token)).ConfigureAwait(false);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task ValidateReportsTheErrorsInTheResultWhenThrowOnErrorIsFalse(bool asynchronous)
        {
            // Setup
            var definition = CreateValidDefinition();
            TableSchemaFactory.SetPrimaryKey(definition.Tables[0], "PK_Child", "Missing");

            // Act
            var result = await Validate(definition, asynchronous, throwOnError: false).ConfigureAwait(false);

            // Assert
            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task ValidateThrowsTheFirstErrorWhenThrowOnErrorIsTrue(bool asynchronous)
        {
            // Setup
            var definition = CreateValidDefinition();
            TableSchemaFactory.SetPrimaryKey(definition.Tables[0], "PK_Child", "Missing");
            definition.Tables[0].Indexes.Add(new IndexInfo("IX_Child") { Columns = new[] { "AlsoMissing" } });

            // Act/Assert
            var exception = await Assert.ThrowsExactlyAsync<InvalidDataException>(() => Validate(definition, asynchronous, throwOnError: true)).ConfigureAwait(false);
            StringAssert.Contains(exception.Message, "primary key 'PK_Child'", StringComparison.Ordinal);
            Assert.IsFalse(exception.Message.Contains("IX_Child", StringComparison.Ordinal));
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task ValidateThrowsAStrictErrorWhenThrowOnErrorIsTrue(bool asynchronous)
        {
            // Setup
            var definition = CreateValidDefinition();
            definition.Tables[0].CheckConstraints.Add(new CheckConstraintInfo("CK_Child") { Expression = null });

            // Act/Assert
            var exception = await Assert.ThrowsExactlyAsync<InvalidDataException>(() => Validate(definition, asynchronous, throwOnError: true)).ConfigureAwait(false);
            StringAssert.Contains(exception.Message, "no expression", StringComparison.Ordinal);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task ValidateReturnsAValidDefinitionWhenThrowOnErrorIsTrue(bool asynchronous)
        {
            // Setup
            var definition = CreateValidDefinition();

            // Act
            var result = await Validate(definition, asynchronous, throwOnError: true).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(result.IsValid);
            Assert.AreSame(definition, result.Definition);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task ValidateDoesNotThrowForWarningsWhenThrowOnErrorIsTrue(bool asynchronous)
        {
            // Setup
            var definition = CreateValidDefinition();
            definition.Tables[0].Columns.Add(TableSchemaFactory.CreateColumn("Code", typeof(string), 3, isNullable: true));
            TableSchemaFactory.SetPrimaryKey(definition.Tables[0], "PK_Child", "Code");

            // Act
            var result = await Validate(definition, asynchronous, throwOnError: true).ConfigureAwait(false);

            // Assert
            Assert.IsTrue(result.IsValid);
            Assert.AreEqual(1, result.Warnings.Count);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task ValidateThrowsBeforeTheTraceIsToldThatItCompleted(bool asynchronous)
        {
            // Setup
            var definition = CreateValidDefinition();
            TableSchemaFactory.SetPrimaryKey(definition.Tables[0], "PK_Child", "Missing");
            var trace = new RecordingTrace(asynchronous);

            // Act/Assert
            await Assert.ThrowsExactlyAsync<InvalidDataException>(() => Validate(definition, asynchronous, trace: trace, throwOnError: true)).ConfigureAwait(false);
            Assert.AreEqual(1, trace.Keys.Count);
            Assert.AreEqual(0, trace.AfterCount);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task ValidateReturnsNullWhenTheTraceCancelsEvenIfThrowOnErrorIsTrue(bool asynchronous)
        {
            // Setup
            var definition = CreateValidDefinition();
            TableSchemaFactory.SetPrimaryKey(definition.Tables[0], "PK_Child", "Missing");
            var trace = new RecordingTrace(asynchronous) { Cancel = true };

            // Act
            var result = await Validate(definition, asynchronous, trace: trace, throwOnError: true).ConfigureAwait(false);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public async Task ValidateStillThrowsForANullDefinitionWhenThrowOnErrorIsTrue(bool asynchronous)
        {
            // Act/Assert
            await Assert.ThrowsExactlyAsync<ArgumentNullException>(() => Validate(null, asynchronous, throwOnError: true)).ConfigureAwait(false);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Creates a definition of one table that has a primary key, and no problems.
        /// </summary>
        /// <returns>The definition.</returns>
        private static SchemaDefinitionInfo CreateValidDefinition()
        {
            var table = TableSchemaFactory.CreateTable("Child", "Id", "Name");
            TableSchemaFactory.SetPrimaryKey(table, "PK_Child", "Id");
            return TableSchemaFactory.CreateDefinition(table);
        }

        /// <summary>
        /// Validates the definition with the sync or the async extension method of a connection. The default tracking key is used if no tracking key is given.
        /// </summary>
        /// <param name="definition">The definition to be validated.</param>
        /// <param name="asynchronous">Whether the asynchronous extension method is used.</param>
        /// <param name="traceKey">The tracking key (the default one of the extension method if it is <c>null</c>).</param>
        /// <param name="trace">The trace object to be used.</param>
        /// <param name="cancellationToken">The cancellation token of the asynchronous extension method.</param>
        /// <param name="throwOnError">Whether the first error is thrown instead of being reported in the result.</param>
        /// <returns>A task that represents the operation. The task result is the outcome of the validation.</returns>
        private static async Task<ValidateDefinitionResult> Validate(SchemaDefinitionInfo definition,
            bool asynchronous,
            string traceKey = null,
            ITrace trace = null,
            CancellationToken cancellationToken = default,
            bool throwOnError = false)
        {
            using var connection = new CustomDbConnection();
            if (traceKey == null)
            {
                return asynchronous
                    ? await connection.ValidateDefinitionAsync(definition, throwOnError, trace: trace, cancellationToken: cancellationToken).ConfigureAwait(false)
                    : connection.ValidateDefinition(definition, throwOnError, trace: trace);
            }
            return asynchronous
                ? await connection.ValidateDefinitionAsync(definition, throwOnError, traceKey, trace, cancellationToken).ConfigureAwait(false)
                : connection.ValidateDefinition(definition, throwOnError, traceKey, trace);
        }

        #endregion
    }
}
