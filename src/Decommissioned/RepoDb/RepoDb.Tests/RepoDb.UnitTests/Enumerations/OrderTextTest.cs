#region Copyright Attributions

// Copyright (c) 2018 Michael Camara Pendon.
// Portions copyright their respective RepoDB contributors.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Attributes;
using RepoDb.Enumerations;
using System.Linq;
using System.Reflection;

namespace RepoDb.UnitTests.Enumerations
{
    [TestClass]
    public class OrderTextTest
    {
        private TextAttribute GetOrderTextAttribute(Order order)
        {
            return typeof(Order)
                .GetMembers()
                .First(member => member.Name.ToLowerInvariant() == order.ToString().ToLowerInvariant())
                .GetCustomAttribute<TextAttribute>();
        }

        [TestMethod]
        public void TestOrderAscendingText()
        {
            // Prepare
            var operation = Order.Ascending;

            // Act
            var parsed = GetOrderTextAttribute(operation);

            // Assert
            Assert.AreEqual("ASC", parsed.Text);
        }

        [TestMethod]
        public void TestOrderDescendingText()
        {
            // Prepare
            var operation = Order.Descending;

            // Act
            var parsed = GetOrderTextAttribute(operation);

            // Assert
            Assert.AreEqual("DESC", parsed.Text);
        }
    }
}
