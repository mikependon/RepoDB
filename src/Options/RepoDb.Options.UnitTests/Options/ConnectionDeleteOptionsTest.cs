#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RepoDb.Options.UnitTests.Options
{
    [TestClass]
    public class ConnectionDeleteOptionsTest
    {
        [TestMethod]
        public void TestConnectionDeleteOptionsHintsProperty()
        {
            // Setup
            var options = new ConnectionDeleteOptions();

            // Assert
            Assert.IsNull(options.Hints);
        }

        [TestMethod]
        public void TestConnectionDeleteOptionsCommandTimeoutProperty()
        {
            // Setup
            var options = new ConnectionDeleteOptions();

            // Assert
            Assert.IsNull(options.CommandTimeout);
        }

        [TestMethod]
        public void TestConnectionDeleteOptionsTraceKeyProperty()
        {
            // Setup
            var options = new ConnectionDeleteOptions();

            // Assert
            Assert.AreEqual(TraceKeys.Delete, options.TraceKey);
        }

        [TestMethod]
        public void TestConnectionDeleteOptionsTransactionProperty()
        {
            // Setup
            var options = new ConnectionDeleteOptions();

            // Assert
            Assert.IsNull(options.Transaction);
        }

        [TestMethod]
        public void TestConnectionDeleteOptionsTraceProperty()
        {
            // Setup
            var options = new ConnectionDeleteOptions();

            // Assert
            Assert.IsNull(options.Trace);
        }

        [TestMethod]
        public void TestConnectionDeleteOptionsStatementBuilderProperty()
        {
            // Setup
            var options = new ConnectionDeleteOptions();

            // Assert
            Assert.IsNull(options.StatementBuilder);
        }
    }
}
