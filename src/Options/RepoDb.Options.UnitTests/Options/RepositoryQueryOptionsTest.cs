#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RepoDb.Options.UnitTests.Options
{
    [TestClass]
    public class RepositoryQueryOptionsTest
    {
        [TestMethod]
        public void TestRepositoryQueryOptionsFieldsProperty()
        {
            // Setup
            var options = new RepositoryQueryOptions();

            // Assert
            Assert.IsNull(options.Fields);
        }

        [TestMethod]
        public void TestRepositoryQueryOptionsOrderByProperty()
        {
            // Setup
            var options = new RepositoryQueryOptions();

            // Assert
            Assert.IsNull(options.OrderBy);
        }

        [TestMethod]
        public void TestRepositoryQueryOptionsTopProperty()
        {
            // Setup
            var options = new RepositoryQueryOptions();

            // Assert
            Assert.AreEqual(0, options.Top);
        }

        [TestMethod]
        public void TestRepositoryQueryOptionsHintsProperty()
        {
            // Setup
            var options = new RepositoryQueryOptions();

            // Assert
            Assert.IsNull(options.Hints);
        }

        [TestMethod]
        public void TestRepositoryQueryOptionsCacheKeyProperty()
        {
            // Setup
            var options = new RepositoryQueryOptions();

            // Assert
            Assert.IsNull(options.CacheKey);
        }

        [TestMethod]
        public void TestRepositoryQueryOptionsTraceKeyProperty()
        {
            // Setup
            var options = new RepositoryQueryOptions();

            // Assert
            Assert.AreEqual(TraceKeys.Query, options.TraceKey);
        }

        [TestMethod]
        public void TestRepositoryQueryOptionsTransactionProperty()
        {
            // Setup
            var options = new RepositoryQueryOptions();

            // Assert
            Assert.IsNull(options.Transaction);
        }
    }
}
