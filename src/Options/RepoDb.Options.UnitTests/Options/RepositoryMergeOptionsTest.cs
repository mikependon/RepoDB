#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RepoDb.Options.UnitTests.Options
{
    [TestClass]
    public class RepositoryMergeOptionsTest
    {
        [TestMethod]
        public void TestRepositoryMergeOptionsFieldsProperty()
        {
            // Setup
            var options = new RepositoryMergeOptions();

            // Assert
            Assert.IsNull(options.Fields);
        }

        [TestMethod]
        public void TestRepositoryMergeOptionsHintsProperty()
        {
            // Setup
            var options = new RepositoryMergeOptions();

            // Assert
            Assert.IsNull(options.Hints);
        }

        [TestMethod]
        public void TestRepositoryMergeOptionsTraceKeyProperty()
        {
            // Setup
            var options = new RepositoryMergeOptions();

            // Assert
            Assert.AreEqual(TraceKeys.Merge, options.TraceKey);
        }

        [TestMethod]
        public void TestRepositoryMergeOptionsTransactionProperty()
        {
            // Setup
            var options = new RepositoryMergeOptions();

            // Assert
            Assert.IsNull(options.Transaction);
        }
    }
}
