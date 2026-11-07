#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema.Enumerations;

namespace RepoDb.Schema.CockroachDb.UnitTests.Dialects
{
    [TestClass]
    public class CockroachDbSchemaDialectTest
    {
        #region Public Methods

        [TestMethod]
        public void TestCockroachDbSchemaDialectFamily()
        {
            // Act
            var family = CockroachDbSchemaDialect.Instance.Family;

            // Assert
            Assert.AreEqual(@"PostgreSql", family);
        }

        [TestMethod]
        public void TestCockroachDbSchemaDialectIdentifierQuotesAreTheStandardOnes()
        {
            // Act
            var quotes = CockroachDbSchemaDialect.Instance.IdentifierQuotes;

            // Assert
            Assert.AreEqual(1, quotes.Count);
            Assert.AreEqual('"', quotes[0].Open);
        }

        [TestMethod]
        public void TestCockroachDbSchemaDialectNonDeterministicFunctionsHaveNow()
        {
            // Act
            var functions = CockroachDbSchemaDialect.Instance.NonDeterministicFunctions;

            // Assert
            CollectionAssert.Contains(functions.ToList(), "now");
        }

        [TestMethod]
        public void TestCockroachDbSchemaDialectNonPortableOperatorsHaveTheCast()
        {
            // Act
            var operators = CockroachDbSchemaDialect.Instance.NonPortableOperators;

            // Assert
            CollectionAssert.Contains(operators.ToList(), "::");
        }

        [TestMethod]
        public void TestCockroachDbSchemaDialectClassifiesTheCurrentTimestampOfNow()
        {
            // Act
            var kind = SchemaDefaultValue.Classify(@"now()",
                CockroachDbSchemaDialect.Instance);

            // Assert
            Assert.AreEqual(ColumnDefaultValue.CurrentTimestamp, kind);
        }

        [TestMethod]
        public void TestCockroachDbSchemaDialectClassifiesTheCurrentTimestampWithACast()
        {
            // Act
            var kind = SchemaDefaultValue.Classify(@"now()::timestamp without time zone",
                CockroachDbSchemaDialect.Instance);

            // Assert
            Assert.AreEqual(ColumnDefaultValue.CurrentTimestamp, kind);
        }

        [TestMethod]
        public void TestCockroachDbSchemaDialectClassifiesTheNewGuid()
        {
            // Act
            var kind = SchemaDefaultValue.Classify(@"gen_random_uuid()",
                CockroachDbSchemaDialect.Instance);

            // Assert
            Assert.AreEqual(ColumnDefaultValue.NewGuid, kind);
        }

        [TestMethod]
        public void TestCockroachDbSchemaDialectClassifiesTheRandomNumber()
        {
            // Act
            var kind = SchemaDefaultValue.Classify(@"random()",
                CockroachDbSchemaDialect.Instance);

            // Assert
            Assert.AreEqual(ColumnDefaultValue.RandomNumber, kind);
        }

        [TestMethod]
        public void TestCockroachDbSchemaDialectClassifiesTheNextValueOfASequence()
        {
            // Act
            var kind = SchemaDefaultValue.Classify(@"nextval('orders_id_seq'::regclass)",
                CockroachDbSchemaDialect.Instance);

            // Assert
            Assert.AreEqual(ColumnDefaultValue.SequenceNextValue, kind);
        }

        [TestMethod]
        public void TestCockroachDbSchemaDialectClassifiesTheCurrentDatabase()
        {
            // Act
            var kind = SchemaDefaultValue.Classify(@"current_database()",
                CockroachDbSchemaDialect.Instance);

            // Assert
            Assert.AreEqual(ColumnDefaultValue.CurrentDatabase, kind);
        }

        [TestMethod]
        public void TestCockroachDbSchemaDialectClassifiesATextWithACast()
        {
            // Act
            var kind = SchemaDefaultValue.Classify(@"'abc'::character varying",
                CockroachDbSchemaDialect.Instance);

            // Assert
            Assert.AreEqual(ColumnDefaultValue.Literal, kind);
        }

        [TestMethod]
        public void TestCockroachDbSchemaDialectNormalizeLiteralRemovesTheCastOfTheText()
        {
            // Act
            var actual = CockroachDbSchemaDialect.Instance.NormalizeLiteral(@"'abc'::character varying");

            // Assert
            Assert.AreEqual(@"'abc'", actual);
        }

        [TestMethod]
        public void TestCockroachDbSchemaDialectNormalizeLiteralRewritesAQuotedHexadecimal()
        {
            // Act
            var actual = CockroachDbSchemaDialect.Instance.NormalizeLiteral(@"'\xDEAD'");

            // Assert
            Assert.AreEqual(@"X'DEAD'", actual);
        }

        [TestMethod]
        public void TestCockroachDbSchemaDialectNormalizeLiteralDoesNotUnderstandAnEscapeText()
        {
            // Act
            var actual = CockroachDbSchemaDialect.Instance.NormalizeLiteral(@"E'a\nb'");

            // Assert
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void TestCockroachDbSchemaDialectIsDateOnlyOfADate()
        {
            // Act
            var actual = CockroachDbSchemaDialect.Instance.IsDateOnly("date");

            // Assert
            Assert.IsTrue(actual);
        }

        #endregion
    }
}
