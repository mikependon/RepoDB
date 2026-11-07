#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema.Enumerations;

namespace RepoDb.Schema.Vertica.UnitTests.Dialects
{
    [TestClass]
    public class VerticaSchemaDialectTest
    {
        #region Public Methods

        [TestMethod]
        public void TestVerticaSchemaDialectFamily()
        {
            // Act
            var family = VerticaSchemaDialect.Instance.Family;

            // Assert
            Assert.AreEqual(@"Vertica", family);
        }

        [TestMethod]
        public void TestVerticaSchemaDialectIdentifierQuotesAreTheStandardOnes()
        {
            // Act
            var quotes = VerticaSchemaDialect.Instance.IdentifierQuotes;

            // Assert
            Assert.AreEqual(1, quotes.Count);
            Assert.AreEqual('"', quotes[0].Open);
        }

        [TestMethod]
        public void TestVerticaSchemaDialectNonPortableOperatorsHaveTheCast()
        {
            // Act
            var operators = VerticaSchemaDialect.Instance.NonPortableOperators;

            // Assert
            CollectionAssert.Contains(operators.ToList(), "::");
        }

        [TestMethod]
        public void TestVerticaSchemaDialectNormalizeLiteralRemovesTheCastOfTheText()
        {
            // Act
            var actual = VerticaSchemaDialect.Instance.NormalizeLiteral(@"'abc'::varchar");

            // Assert
            Assert.AreEqual(@"'abc'", actual);
        }

        [TestMethod]
        public void TestVerticaSchemaDialectIsDateOnlyOfADate()
        {
            // Act
            var actual = VerticaSchemaDialect.Instance.IsDateOnly("date");

            // Assert
            Assert.IsTrue(actual);
        }

        #endregion
    }
}
