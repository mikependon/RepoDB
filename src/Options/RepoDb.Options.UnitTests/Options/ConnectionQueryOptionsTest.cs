#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RepoDb.Options.UnitTests.Options
{
    [TestClass]
    public class ConnectionQueryOptionsTest
    {
        [TestMethod]
        public void TestConnectionQueryOptionsFieldsProperty()
        {
            // Setup
            var options = new ConnectionQueryOptions();

            // Assert
            Assert.IsNull(options.Fields);
        }

        [TestMethod]
        public void TestConnectionQueryOptionsOrderByProperty()
        {
            // Setup
            var options = new ConnectionQueryOptions();

            // Assert
            Assert.IsNull(options.OrderBy);
        }

        [TestMethod]
        public void TestConnectionQueryOptionsTopProperty()
        {
            // Setup
            var options = new ConnectionQueryOptions();

            // Assert
            Assert.AreEqual(0, options.Top);
        }

        [TestMethod]
        public void TestConnectionQueryOptionsHintsProperty()
        {
            // Setup
            var options = new ConnectionQueryOptions();

            // Assert
            Assert.IsNull(options.Hints);
        }

        [TestMethod]
        public void TestConnectionQueryOptionsCacheKeyProperty()
        {
            // Setup
            var options = new ConnectionQueryOptions();

            // Assert
            Assert.IsNull(options.CacheKey);
        }

        [TestMethod]
        public void TestConnectionQueryOptionsCacheItemExpirationProperty()
        {
            // Setup
            var options = new ConnectionQueryOptions();

            // Assert
            Assert.AreEqual(Constant.DefaultCacheItemExpirationInMinutes, options.CacheItemExpiration);
        }

        [TestMethod]
        public void TestConnectionQueryOptionsCommandTimeoutProperty()
        {
            // Setup
            var options = new ConnectionQueryOptions();

            // Assert
            Assert.IsNull(options.CommandTimeout);
        }

        [TestMethod]
        public void TestConnectionQueryOptionsTraceKeyProperty()
        {
            // Setup
            var options = new ConnectionQueryOptions();

            // Assert
            Assert.AreEqual(TraceKeys.Query, options.TraceKey);
        }

        [TestMethod]
        public void TestConnectionQueryOptionsTransactionProperty()
        {
            // Setup
            var options = new ConnectionQueryOptions();

            // Assert
            Assert.IsNull(options.Transaction);
        }

        [TestMethod]
        public void TestConnectionQueryOptionsCacheProperty()
        {
            // Setup
            var options = new ConnectionQueryOptions();

            // Assert
            Assert.IsNull(options.Cache);
        }

        [TestMethod]
        public void TestConnectionQueryOptionsTraceProperty()
        {
            // Setup
            var options = new ConnectionQueryOptions();

            // Assert
            Assert.IsNull(options.Trace);
        }

        [TestMethod]
        public void TestConnectionQueryOptionsStatementBuilderProperty()
        {
            // Setup
            var options = new ConnectionQueryOptions();

            // Assert
            Assert.IsNull(options.StatementBuilder);
        }
    }
}
