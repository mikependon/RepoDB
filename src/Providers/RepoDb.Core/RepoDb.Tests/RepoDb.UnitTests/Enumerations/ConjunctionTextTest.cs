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
    public class ConjunctionTextTest
    {
        [TestMethod]
        public void TestConjunctionAndText()
        {
            // Prepare
            var operation = Conjunction.And;

            // Act
            var text = operation.GetText();

            // Assert
            Assert.AreEqual("AND", text);
        }

        [TestMethod]
        public void TestConjunctionOrText()
        {
            // Prepare
            var operation = Conjunction.Or;

            // Act
            var text = operation.GetText();

            // Assert
            Assert.AreEqual("OR", text);
        }

    }
}
