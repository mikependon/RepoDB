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
    public class InterceptionLevelTest
    {
        [TestMethod]
        public void TestInterceptionLevelDefaultValue()
        {
            // Act
            var actual = default(InterceptionLevel);
            var expected = InterceptionLevel.Row;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestInterceptionLevelValues()
        {
            // Act
            var actual = Enum.GetNames(typeof(InterceptionLevel));
            var expected = new[] { "Row", "Table", "RowAndTable" };

            // Assert
            CollectionAssert.AreEqual(expected, actual);
        }
    }
}
