#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema.Enumerations;

namespace RepoDb.Schema.AuroraDbPostgreSql.UnitTests.Dialects
{
    [TestClass]
    public class AuroraDbPostgreSqlSchemaDialectTest
    {
        #region Public Methods

        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaDialectFamily()
        {
            // Act
            var family = AuroraDbPostgreSqlSchemaDialect.Instance.Family;

            // Assert
            Assert.AreEqual(@"PostgreSql", family);
        }

        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaDialectIdentifierQuotesAreTheStandardOnes()
        {
            // Act
            var quotes = AuroraDbPostgreSqlSchemaDialect.Instance.IdentifierQuotes;

            // Assert
            Assert.AreEqual(1, quotes.Count);
            Assert.AreEqual('"', quotes[0].Open);
        }

        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaDialectNonDeterministicFunctionsHaveNow()
        {
            // Act
            var functions = AuroraDbPostgreSqlSchemaDialect.Instance.NonDeterministicFunctions;

            // Assert
            CollectionAssert.Contains(functions.ToList(), "now");
        }

        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaDialectNonPortableOperatorsHaveTheCast()
        {
            // Act
            var operators = AuroraDbPostgreSqlSchemaDialect.Instance.NonPortableOperators;

            // Assert
            CollectionAssert.Contains(operators.ToList(), "::");
        }

        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaDialectClassifiesTheCurrentTimestampOfNow()
        {
            // Act
            var kind = SchemaDefaultValue.Classify(@"now()",
                AuroraDbPostgreSqlSchemaDialect.Instance);

            // Assert
            Assert.AreEqual(ColumnDefaultValue.CurrentTimestamp, kind);
        }

        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaDialectClassifiesTheCurrentTimestampWithACast()
        {
            // Act
            var kind = SchemaDefaultValue.Classify(@"now()::timestamp without time zone",
                AuroraDbPostgreSqlSchemaDialect.Instance);

            // Assert
            Assert.AreEqual(ColumnDefaultValue.CurrentTimestamp, kind);
        }

        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaDialectClassifiesTheNewGuid()
        {
            // Act
            var kind = SchemaDefaultValue.Classify(@"gen_random_uuid()",
                AuroraDbPostgreSqlSchemaDialect.Instance);

            // Assert
            Assert.AreEqual(ColumnDefaultValue.NewGuid, kind);
        }

        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaDialectClassifiesTheRandomNumber()
        {
            // Act
            var kind = SchemaDefaultValue.Classify(@"random()",
                AuroraDbPostgreSqlSchemaDialect.Instance);

            // Assert
            Assert.AreEqual(ColumnDefaultValue.RandomNumber, kind);
        }

        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaDialectClassifiesTheNextValueOfASequence()
        {
            // Act
            var kind = SchemaDefaultValue.Classify(@"nextval('orders_id_seq'::regclass)",
                AuroraDbPostgreSqlSchemaDialect.Instance);

            // Assert
            Assert.AreEqual(ColumnDefaultValue.SequenceNextValue, kind);
        }

        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaDialectClassifiesTheCurrentDatabase()
        {
            // Act
            var kind = SchemaDefaultValue.Classify(@"current_database()",
                AuroraDbPostgreSqlSchemaDialect.Instance);

            // Assert
            Assert.AreEqual(ColumnDefaultValue.CurrentDatabase, kind);
        }

        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaDialectClassifiesATextWithACast()
        {
            // Act
            var kind = SchemaDefaultValue.Classify(@"'abc'::character varying",
                AuroraDbPostgreSqlSchemaDialect.Instance);

            // Assert
            Assert.AreEqual(ColumnDefaultValue.Literal, kind);
        }

        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaDialectNormalizeLiteralRemovesTheCastOfTheText()
        {
            // Act
            var actual = AuroraDbPostgreSqlSchemaDialect.Instance.NormalizeLiteral(@"'abc'::character varying");

            // Assert
            Assert.AreEqual(@"'abc'", actual);
        }

        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaDialectNormalizeLiteralRewritesAQuotedHexadecimal()
        {
            // Act
            var actual = AuroraDbPostgreSqlSchemaDialect.Instance.NormalizeLiteral(@"'\xDEAD'");

            // Assert
            Assert.AreEqual(@"X'DEAD'", actual);
        }

        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaDialectNormalizeLiteralDoesNotUnderstandAnEscapeText()
        {
            // Act
            var actual = AuroraDbPostgreSqlSchemaDialect.Instance.NormalizeLiteral(@"E'a\nb'");

            // Assert
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void TestAuroraDbPostgreSqlSchemaDialectIsDateOnlyOfADate()
        {
            // Act
            var actual = AuroraDbPostgreSqlSchemaDialect.Instance.IsDateOnly("date");

            // Assert
            Assert.IsTrue(actual);
        }

        #endregion
    }
}
