#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema.Core.UnitTests.CustomObjects;
using RepoDb.Schema.Enumerations;

namespace RepoDb.Schema.Core.UnitTests.Dialects
{
    [TestClass]
    public class SchemaDialectTest
    {
        #region Public Methods

        [TestMethod]
        public void TestSchemaDialectWithoutAFamilyThrows()
        {
            // Act/Assert
            Assert.ThrowsExactly<ArgumentNullException>(() =>
            new SchemaDialect(null));
        }

        [TestMethod]
        public void TestSchemaDialectFamily()
        {
            // Act
            var dialect = new SchemaDialect("Made");

            // Assert
            Assert.AreEqual("Made", dialect.Family);
        }

        [TestMethod]
        public void TestSchemaDialectStandardFamily()
        {
            // Act
            var dialect = SchemaDialect.Standard;

            // Assert
            Assert.AreEqual("Standard", dialect.Family);
        }

        [TestMethod]
        public void TestSchemaDialectIdentifierQuotesAreTheDoubleQuote()
        {
            // Act
            var quotes = SchemaDialect.Standard.IdentifierQuotes;

            // Assert
            Assert.AreEqual(1, quotes.Count);
            Assert.AreEqual('"', quotes[0].Open);
        }

        [TestMethod]
        public void TestSchemaDialectHasNoNonDeterministicFunctions()
        {
            // Act
            var functions = SchemaDialect.Standard.NonDeterministicFunctions;

            // Assert
            Assert.AreEqual(0, functions.Count);
        }

        [TestMethod]
        public void TestSchemaDialectHasNoNonPortableOperators()
        {
            // Act
            var operators = SchemaDialect.Standard.NonPortableOperators;

            // Assert
            Assert.AreEqual(0, operators.Count);
        }

        [TestMethod]
        public void TestSchemaDialectNonDeterministicKeywordsAreTheStandardOnes()
        {
            // Act
            var keywords = SchemaDialect.Standard.NonDeterministicKeywords;

            // Assert
            CollectionAssert.Contains(keywords.ToList(), "current_timestamp");
        }

        [TestMethod]
        public void TestSchemaDialectNonDeterministicKeywordsIncludeTheOnesOfTheDialect()
        {
            // Act
            var keywords = CustomSchemaDialect.Instance.NonDeterministicKeywords;

            // Assert
            CollectionAssert.Contains(keywords.ToList(), "sysdate");
            CollectionAssert.Contains(keywords.ToList(), "current_date");
        }

        [TestMethod]
        public void TestSchemaDialectDefaultPatternsAreTheStandardOnes()
        {
            // Act
            var patterns = SchemaDialect.Standard.DefaultPatterns;

            // Assert
            Assert.IsTrue(patterns.Any(item => item.Kind == ColumnDefaultValue.CurrentDate));
        }

        [TestMethod]
        public void TestSchemaDialectDefaultPatternsIncludeTheOnesOfTheDialect()
        {
            // Act
            var patterns = CustomSchemaDialect.Instance.DefaultPatterns;

            // Assert
            Assert.IsTrue(patterns.Any(item => item.Kind == ColumnDefaultValue.NewGuid));
            Assert.IsTrue(patterns.Any(item => item.Kind == ColumnDefaultValue.CurrentDate));
        }

        [TestMethod]
        public void TestSchemaDialectIsDateOnlyOfADate()
        {
            // Act
            var actual = SchemaDialect.Standard.IsDateOnly("DATE");

            // Assert
            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void TestSchemaDialectIsDateOnlyIgnoresTheWhiteSpaces()
        {
            // Act
            var actual = SchemaDialect.Standard.IsDateOnly(" date ");

            // Assert
            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void TestSchemaDialectIsDateOnlyOfADateTime()
        {
            // Act
            var actual = SchemaDialect.Standard.IsDateOnly("datetime");

            // Assert
            Assert.IsFalse(actual);
        }

        [TestMethod]
        public void TestSchemaDialectIsDateOnlyOfANullType()
        {
            // Act
            var actual = SchemaDialect.Standard.IsDateOnly(null);

            // Assert
            Assert.IsFalse(actual);
        }

        [TestMethod]
        public void TestSchemaDialectNormalizeLiteralKeepsTheLiteral()
        {
            // Act
            var actual = SchemaDialect.Standard.NormalizeLiteral("'x'");

            // Assert
            Assert.AreEqual("'x'", actual);
        }

        #endregion
    }
}
