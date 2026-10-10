#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Data.Enumerations;

namespace RepoDb.Data.Core.UnitTests.Enumerations
{
    [TestClass]
    public class CopyInterceptionLevelTest
    {
        [TestMethod]
        public void TestCopyInterceptionLevelDefaultValue()
        {
            // Act
            var actual = default(CopyInterceptionLevel);
            var expected = CopyInterceptionLevel.Row;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestCopyInterceptionLevelValues()
        {
            // Act
            var actual = Enum.GetNames(typeof(CopyInterceptionLevel));
            var expected = new[] { "Row", "Table", "RowAndTable" };

            // Assert
            CollectionAssert.AreEqual(expected, actual);
        }
    }
}
