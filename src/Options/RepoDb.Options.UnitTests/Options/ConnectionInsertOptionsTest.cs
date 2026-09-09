#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RepoDb.Options.UnitTests.Options
{
    [TestClass]
    public class ConnectionInsertOptionsTest
    {
        [TestMethod]
        public void TestConnectionInsertOptionsFieldsProperty()
        {
            // Setup
            var options = new ConnectionInsertOptions();

            // Assert
            Assert.IsNull(options.Fields);
        }

        [TestMethod]
        public void TestConnectionInsertOptionsHintsProperty()
        {
            // Setup
            var options = new ConnectionInsertOptions();

            // Assert
            Assert.IsNull(options.Hints);
        }

        [TestMethod]
        public void TestConnectionInsertOptionsCommandTimeoutProperty()
        {
            // Setup
            var options = new ConnectionInsertOptions();

            // Assert
            Assert.IsNull(options.CommandTimeout);
        }

        [TestMethod]
        public void TestConnectionInsertOptionsTraceKeyProperty()
        {
            // Setup
            var options = new ConnectionInsertOptions();

            // Assert
            Assert.AreEqual(TraceKeys.Insert, options.TraceKey);
        }

        [TestMethod]
        public void TestConnectionInsertOptionsTransactionProperty()
        {
            // Setup
            var options = new ConnectionInsertOptions();

            // Assert
            Assert.IsNull(options.Transaction);
        }

        [TestMethod]
        public void TestConnectionInsertOptionsTraceProperty()
        {
            // Setup
            var options = new ConnectionInsertOptions();

            // Assert
            Assert.IsNull(options.Trace);
        }

        [TestMethod]
        public void TestConnectionInsertOptionsStatementBuilderProperty()
        {
            // Setup
            var options = new ConnectionInsertOptions();

            // Assert
            Assert.IsNull(options.StatementBuilder);
        }
    }
}
