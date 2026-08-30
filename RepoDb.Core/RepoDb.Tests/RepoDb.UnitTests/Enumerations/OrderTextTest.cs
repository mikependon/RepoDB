#region Copyright Attributions

// Copyright (c) 2018 Michael Camara Pendon.
// Portions copyright their respective RepoDB contributors.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Enumerations;
using RepoDb.Extensions;

namespace RepoDb.UnitTests.Enumerations
{
    [TestClass]
    public class OrderTextTest
    {
        [TestMethod]
        public void TestOrderAscendingText()
        {
            // Prepare
            var operation = Order.Ascending;

            // Act
            var text = operation.GetText();

            // Assert
            Assert.AreEqual("ASC", text);
        }

        [TestMethod]
        public void TestOrderDescendingText()
        {
            // Prepare
            var operation = Order.Descending;

            // Act
            var text = operation.GetText();

            // Assert
            Assert.AreEqual("DESC", text);
        }
    }
}
