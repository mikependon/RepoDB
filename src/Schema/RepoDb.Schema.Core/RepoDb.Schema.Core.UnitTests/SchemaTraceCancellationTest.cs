#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Exceptions;
using RepoDb.Schema.Core.UnitTests.CustomObjects;

namespace RepoDb.Schema.Core.UnitTests
{
    /// <summary>
    /// Contains the tests of the <c>SchemaTraceCancellation</c> class. The logs come from a trace, as a log cannot be created outside of the library.
    /// </summary>
    [TestClass]
    public class SchemaTraceCancellationTest
    {
        #region Public Methods

        [TestMethod]
        public void TestSchemaTraceCancellationWithALogThatIsNotCancelled()
        {
            // Setup
            var log = CaptureLog();

            // Act
            var actual = SchemaTraceCancellation.IsCancelled(log, "cancelled");

            // Assert
            Assert.IsFalse(actual);
        }

        [TestMethod]
        public void TestSchemaTraceCancellationWithALogThatIsCancelled()
        {
            // Setup
            var log = CaptureLog();
            log.Cancel(false);

            // Act
            var actual = SchemaTraceCancellation.IsCancelled(log, "cancelled");

            // Assert
            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void TestSchemaTraceCancellationWithALogThatIsCancelledWithAnException()
        {
            // Setup
            var log = CaptureLog();
            log.Cancel(true);

            // Act/Assert
            var exception = Assert.ThrowsExactly<CancelledExecutionException>(() => SchemaTraceCancellation.IsCancelled(log, "The operation was cancelled."));
            Assert.AreEqual("The operation was cancelled.", exception.Message, StringComparer.Ordinal);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Gets a log that a trace received, by validating a definition with the trace.
        /// </summary>
        /// <returns>The log of the validation.</returns>
        private static CancellableTraceLog CaptureLog()
        {
            var table = TableSchemaFactory.CreateTable("Child", "Id");
            var trace = new RecordingTrace(false);
            using var connection = new CustomDbConnection();
            connection.ValidateDefinition(TableSchemaFactory.CreateDefinition(table),
                trace: trace);
            return trace.Logs.Single();
        }

        #endregion
    }
}
