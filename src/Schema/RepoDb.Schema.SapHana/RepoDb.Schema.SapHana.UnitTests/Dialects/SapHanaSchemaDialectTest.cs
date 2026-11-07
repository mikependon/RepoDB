#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema.Enumerations;

namespace RepoDb.Schema.SapHana.UnitTests.Dialects
{
    [TestClass]
    public class SapHanaSchemaDialectTest
    {
        #region Public Methods

        [TestMethod]
        public void TestSapHanaSchemaDialectFamily()
        {
            // Act
            var family = SapHanaSchemaDialect.Instance.Family;

            // Assert
            Assert.AreEqual(@"SapHana", family);
        }

        [TestMethod]
        public void TestSapHanaSchemaDialectIdentifierQuotesAreTheStandardOnes()
        {
            // Act
            var quotes = SapHanaSchemaDialect.Instance.IdentifierQuotes;

            // Assert
            Assert.AreEqual(1, quotes.Count);
            Assert.AreEqual('"', quotes[0].Open);
        }

        [TestMethod]
        public void TestSapHanaSchemaDialectHasNoNonPortableOperators()
        {
            // Act
            var operators = SapHanaSchemaDialect.Instance.NonPortableOperators;

            // Assert
            Assert.AreEqual(0, operators.Count);
        }

        [TestMethod]
        public void TestSapHanaSchemaDialectIsDateOnlyOfADate()
        {
            // Act
            var actual = SapHanaSchemaDialect.Instance.IsDateOnly("date");

            // Assert
            Assert.IsTrue(actual);
        }

        #endregion
    }
}
