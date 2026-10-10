#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Data.Enumerations;
using RepoDb.Data.Options;

namespace RepoDb.Data.Core.UnitTests.Options
{
    [TestClass]
    public class CopyDataInterceptorOptionsTest
    {
        [TestMethod]
        public void TestCopyDataInterceptorOptionsDefaultValues()
        {
            // Act
            var actual = new CopyDataInterceptorOptions();

            // Assert
            Assert.AreEqual(CopyInterceptionLevel.Row, actual.CopyInterceptionLevel);
            Assert.AreEqual(0, actual.RetryCount);
        }

        [TestMethod]
        public void TestCopyDataInterceptorOptionsPropertiesCanBeSet()
        {
            // Act
            var actual = new CopyDataInterceptorOptions
            {
                CopyInterceptionLevel = CopyInterceptionLevel.RowAndTable,
                RetryCount = 3
            };

            // Assert
            Assert.AreEqual(CopyInterceptionLevel.RowAndTable, actual.CopyInterceptionLevel);
            Assert.AreEqual(3, actual.RetryCount);
        }
    }
}
