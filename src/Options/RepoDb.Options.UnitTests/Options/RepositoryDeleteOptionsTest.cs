#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RepoDb.Options.UnitTests.Options
{
    [TestClass]
    public class RepositoryDeleteOptionsTest
    {
        [TestMethod]
        public void TestRepositoryDeleteOptionsHintsProperty()
        {
            // Setup
            var options = new RepositoryDeleteOptions();

            // Assert
            Assert.IsNull(options.Hints);
        }

        [TestMethod]
        public void TestRepositoryDeleteOptionsTraceKeyProperty()
        {
            // Setup
            var options = new RepositoryDeleteOptions();

            // Assert
            Assert.AreEqual(TraceKeys.Delete, options.TraceKey);
        }

        [TestMethod]
        public void TestRepositoryDeleteOptionsTransactionProperty()
        {
            // Setup
            var options = new RepositoryDeleteOptions();

            // Assert
            Assert.IsNull(options.Transaction);
        }
    }
}
