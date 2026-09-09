#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RepoDb.Options.UnitTests.Options
{
    [TestClass]
    public class RepositoryInsertOptionsTest
    {
        [TestMethod]
        public void TestRepositoryInsertOptionsFieldsProperty()
        {
            // Setup
            var options = new RepositoryInsertOptions();

            // Assert
            Assert.IsNull(options.Fields);
        }

        [TestMethod]
        public void TestRepositoryInsertOptionsHintsProperty()
        {
            // Setup
            var options = new RepositoryInsertOptions();

            // Assert
            Assert.IsNull(options.Hints);
        }

        [TestMethod]
        public void TestRepositoryInsertOptionsTraceKeyProperty()
        {
            // Setup
            var options = new RepositoryInsertOptions();

            // Assert
            Assert.AreEqual(TraceKeys.Insert, options.TraceKey);
        }

        [TestMethod]
        public void TestRepositoryInsertOptionsTransactionProperty()
        {
            // Setup
            var options = new RepositoryInsertOptions();

            // Assert
            Assert.IsNull(options.Transaction);
        }
    }
}
