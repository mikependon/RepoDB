#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RepoDb.Options.UnitTests.Options
{
    [TestClass]
    public class RepositoryUpdateOptionsTest
    {
        [TestMethod]
        public void TestRepositoryUpdateOptionsFieldsProperty()
        {
            // Setup
            var options = new RepositoryUpdateOptions();

            // Assert
            Assert.IsNull(options.Fields);
        }

        [TestMethod]
        public void TestRepositoryUpdateOptionsHintsProperty()
        {
            // Setup
            var options = new RepositoryUpdateOptions();

            // Assert
            Assert.IsNull(options.Hints);
        }

        [TestMethod]
        public void TestRepositoryUpdateOptionsTraceKeyProperty()
        {
            // Setup
            var options = new RepositoryUpdateOptions();

            // Assert
            Assert.AreEqual(TraceKeys.Update, options.TraceKey);
        }

        [TestMethod]
        public void TestRepositoryUpdateOptionsTransactionProperty()
        {
            // Setup
            var options = new RepositoryUpdateOptions();

            // Assert
            Assert.IsNull(options.Transaction);
        }
    }
}
