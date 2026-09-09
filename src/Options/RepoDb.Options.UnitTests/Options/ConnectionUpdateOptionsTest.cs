#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RepoDb.Options.UnitTests.Options
{
    [TestClass]
    public class ConnectionUpdateOptionsTest
    {
        [TestMethod]
        public void TestConnectionUpdateOptionsFieldsProperty()
        {
            // Setup
            var options = new ConnectionUpdateOptions();

            // Assert
            Assert.IsNull(options.Fields);
        }

        [TestMethod]
        public void TestConnectionUpdateOptionsHintsProperty()
        {
            // Setup
            var options = new ConnectionUpdateOptions();

            // Assert
            Assert.IsNull(options.Hints);
        }

        [TestMethod]
        public void TestConnectionUpdateOptionsCommandTimeoutProperty()
        {
            // Setup
            var options = new ConnectionUpdateOptions();

            // Assert
            Assert.IsNull(options.CommandTimeout);
        }

        [TestMethod]
        public void TestConnectionUpdateOptionsTraceKeyProperty()
        {
            // Setup
            var options = new ConnectionUpdateOptions();

            // Assert
            Assert.AreEqual(TraceKeys.Update, options.TraceKey);
        }

        [TestMethod]
        public void TestConnectionUpdateOptionsTransactionProperty()
        {
            // Setup
            var options = new ConnectionUpdateOptions();

            // Assert
            Assert.IsNull(options.Transaction);
        }

        [TestMethod]
        public void TestConnectionUpdateOptionsTraceProperty()
        {
            // Setup
            var options = new ConnectionUpdateOptions();

            // Assert
            Assert.IsNull(options.Trace);
        }

        [TestMethod]
        public void TestConnectionUpdateOptionsStatementBuilderProperty()
        {
            // Setup
            var options = new ConnectionUpdateOptions();

            // Assert
            Assert.IsNull(options.StatementBuilder);
        }
    }
}
