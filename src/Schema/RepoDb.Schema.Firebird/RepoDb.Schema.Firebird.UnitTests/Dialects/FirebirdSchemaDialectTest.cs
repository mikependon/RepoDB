#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema.Enumerations;

namespace RepoDb.Schema.Firebird.UnitTests.Dialects
{
    [TestClass]
    public class FirebirdSchemaDialectTest
    {
        #region Public Methods

        [TestMethod]
        public void TestFirebirdSchemaDialectFamily()
        {
            // Act
            var family = FirebirdSchemaDialect.Instance.Family;

            // Assert
            Assert.AreEqual(@"Firebird", family);
        }

        [TestMethod]
        public void TestFirebirdSchemaDialectIdentifierQuotesAreTheStandardOnes()
        {
            // Act
            var quotes = FirebirdSchemaDialect.Instance.IdentifierQuotes;

            // Assert
            Assert.AreEqual(1, quotes.Count);
            Assert.AreEqual('"', quotes[0].Open);
        }

        [TestMethod]
        public void TestFirebirdSchemaDialectHasNoNonPortableOperators()
        {
            // Act
            var operators = FirebirdSchemaDialect.Instance.NonPortableOperators;

            // Assert
            Assert.AreEqual(0, operators.Count);
        }

        [TestMethod]
        public void TestFirebirdSchemaDialectClassifiesTheNextValueOfAGenerator()
        {
            // Act
            var kind = SchemaDefaultValue.Classify(@"gen_id(gen_orders, 1)",
                FirebirdSchemaDialect.Instance);

            // Assert
            Assert.AreEqual(ColumnDefaultValue.SequenceNextValue, kind);
        }

        [TestMethod]
        public void TestFirebirdSchemaDialectIsDateOnlyOfADate()
        {
            // Act
            var actual = FirebirdSchemaDialect.Instance.IsDateOnly("date");

            // Assert
            Assert.IsTrue(actual);
        }

        #endregion
    }
}
