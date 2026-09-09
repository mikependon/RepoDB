#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RepoDb.Options.UnitTests.Options
{
    [TestClass]
    public class ConnectionMergeOptionsTest
    {
        [TestMethod]
        public void TestConnectionMergeOptionsFieldsProperty()
        {
            // Setup
            var options = new ConnectionMergeOptions();

            // Assert
            Assert.IsNull(options.Fields);
        }

        [TestMethod]
        public void TestConnectionMergeOptionsHintsProperty()
        {
            // Setup
            var options = new ConnectionMergeOptions();

            // Assert
            Assert.IsNull(options.Hints);
        }

        [TestMethod]
        public void TestConnectionMergeOptionsCommandTimeoutProperty()
        {
            // Setup
            var options = new ConnectionMergeOptions();

            // Assert
            Assert.IsNull(options.CommandTimeout);
        }

        [TestMethod]
        public void TestConnectionMergeOptionsTraceKeyProperty()
        {
            // Setup
            var options = new ConnectionMergeOptions();

            // Assert
            Assert.AreEqual(TraceKeys.Merge, options.TraceKey);
        }

        [TestMethod]
        public void TestConnectionMergeOptionsTransactionProperty()
        {
            // Setup
            var options = new ConnectionMergeOptions();

            // Assert
            Assert.IsNull(options.Transaction);
        }

        [TestMethod]
        public void TestConnectionMergeOptionsTraceProperty()
        {
            // Setup
            var options = new ConnectionMergeOptions();

            // Assert
            Assert.IsNull(options.Trace);
        }

        [TestMethod]
        public void TestConnectionMergeOptionsStatementBuilderProperty()
        {
            // Setup
            var options = new ConnectionMergeOptions();

            // Assert
            Assert.IsNull(options.StatementBuilder);
        }
    }
}
