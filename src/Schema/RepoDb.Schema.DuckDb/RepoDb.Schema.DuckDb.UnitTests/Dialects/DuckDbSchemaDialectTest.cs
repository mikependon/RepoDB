#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema.Enumerations;

namespace RepoDb.Schema.DuckDb.UnitTests.Dialects
{
    [TestClass]
    public class DuckDbSchemaDialectTest
    {
        #region Public Methods

        [TestMethod]
        public void TestDuckDbSchemaDialectFamily()
        {
            // Act
            var family = DuckDbSchemaDialect.Instance.Family;

            // Assert
            Assert.AreEqual(@"DuckDb", family);
        }

        [TestMethod]
        public void TestDuckDbSchemaDialectIdentifierQuotesAreTheStandardOnes()
        {
            // Act
            var quotes = DuckDbSchemaDialect.Instance.IdentifierQuotes;

            // Assert
            Assert.AreEqual(1, quotes.Count);
            Assert.AreEqual('"', quotes[0].Open);
        }

        [TestMethod]
        public void TestDuckDbSchemaDialectNonDeterministicFunctionsHaveRandom()
        {
            // Act
            var functions = DuckDbSchemaDialect.Instance.NonDeterministicFunctions;

            // Assert
            CollectionAssert.Contains(functions.ToList(), "random");
        }

        [TestMethod]
        public void TestDuckDbSchemaDialectNonPortableOperatorsHaveTheCast()
        {
            // Act
            var operators = DuckDbSchemaDialect.Instance.NonPortableOperators;

            // Assert
            CollectionAssert.Contains(operators.ToList(), "::");
        }

        [TestMethod]
        public void TestDuckDbSchemaDialectClassifiesTheNewGuid()
        {
            // Act
            var kind = SchemaDefaultValue.Classify(@"uuid()",
                DuckDbSchemaDialect.Instance);

            // Assert
            Assert.AreEqual(ColumnDefaultValue.NewGuid, kind);
        }

        [TestMethod]
        public void TestDuckDbSchemaDialectNormalizeLiteralRemovesTheCastOfTheText()
        {
            // Act
            var actual = DuckDbSchemaDialect.Instance.NormalizeLiteral(@"'abc'::VARCHAR");

            // Assert
            Assert.AreEqual(@"'abc'", actual);
        }

        [TestMethod]
        public void TestDuckDbSchemaDialectIsDateOnlyOfADate()
        {
            // Act
            var actual = DuckDbSchemaDialect.Instance.IsDateOnly("date");

            // Assert
            Assert.IsTrue(actual);
        }

        #endregion
    }
}
