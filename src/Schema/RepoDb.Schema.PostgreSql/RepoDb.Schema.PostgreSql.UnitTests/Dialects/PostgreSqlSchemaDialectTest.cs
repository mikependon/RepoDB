#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema.Enumerations;

namespace RepoDb.Schema.PostgreSql.UnitTests.Dialects
{
    [TestClass]
    public class PostgreSqlSchemaDialectTest
    {
        #region Public Methods

        [TestMethod]
        public void TestPostgreSqlSchemaDialectFamily()
        {
            // Act
            var family = PostgreSqlSchemaDialect.Instance.Family;

            // Assert
            Assert.AreEqual(@"PostgreSql", family);
        }

        [TestMethod]
        public void TestPostgreSqlSchemaDialectIdentifierQuotesAreTheStandardOnes()
        {
            // Act
            var quotes = PostgreSqlSchemaDialect.Instance.IdentifierQuotes;

            // Assert
            Assert.AreEqual(1, quotes.Count);
            Assert.AreEqual('"', quotes[0].Open);
        }

        [TestMethod]
        public void TestPostgreSqlSchemaDialectNonDeterministicFunctionsHaveNow()
        {
            // Act
            var functions = PostgreSqlSchemaDialect.Instance.NonDeterministicFunctions;

            // Assert
            CollectionAssert.Contains(functions.ToList(), "now");
        }

        [TestMethod]
        public void TestPostgreSqlSchemaDialectNonPortableOperatorsHaveTheCast()
        {
            // Act
            var operators = PostgreSqlSchemaDialect.Instance.NonPortableOperators;

            // Assert
            CollectionAssert.Contains(operators.ToList(), "::");
        }

        [TestMethod]
        public void TestPostgreSqlSchemaDialectClassifiesTheCurrentTimestampOfNow()
        {
            // Act
            var kind = SchemaDefaultValue.Classify(@"now()",
                PostgreSqlSchemaDialect.Instance);

            // Assert
            Assert.AreEqual(ColumnDefaultValue.CurrentTimestamp, kind);
        }

        [TestMethod]
        public void TestPostgreSqlSchemaDialectClassifiesTheCurrentTimestampWithACast()
        {
            // Act
            var kind = SchemaDefaultValue.Classify(@"now()::timestamp without time zone",
                PostgreSqlSchemaDialect.Instance);

            // Assert
            Assert.AreEqual(ColumnDefaultValue.CurrentTimestamp, kind);
        }

        [TestMethod]
        public void TestPostgreSqlSchemaDialectClassifiesTheNewGuid()
        {
            // Act
            var kind = SchemaDefaultValue.Classify(@"gen_random_uuid()",
                PostgreSqlSchemaDialect.Instance);

            // Assert
            Assert.AreEqual(ColumnDefaultValue.NewGuid, kind);
        }

        [TestMethod]
        public void TestPostgreSqlSchemaDialectClassifiesTheRandomNumber()
        {
            // Act
            var kind = SchemaDefaultValue.Classify(@"random()",
                PostgreSqlSchemaDialect.Instance);

            // Assert
            Assert.AreEqual(ColumnDefaultValue.RandomNumber, kind);
        }

        [TestMethod]
        public void TestPostgreSqlSchemaDialectClassifiesTheNextValueOfASequence()
        {
            // Act
            var kind = SchemaDefaultValue.Classify(@"nextval('orders_id_seq'::regclass)",
                PostgreSqlSchemaDialect.Instance);

            // Assert
            Assert.AreEqual(ColumnDefaultValue.SequenceNextValue, kind);
        }

        [TestMethod]
        public void TestPostgreSqlSchemaDialectClassifiesTheCurrentDatabase()
        {
            // Act
            var kind = SchemaDefaultValue.Classify(@"current_database()",
                PostgreSqlSchemaDialect.Instance);

            // Assert
            Assert.AreEqual(ColumnDefaultValue.CurrentDatabase, kind);
        }

        [TestMethod]
        public void TestPostgreSqlSchemaDialectClassifiesATextWithACast()
        {
            // Act
            var kind = SchemaDefaultValue.Classify(@"'abc'::character varying",
                PostgreSqlSchemaDialect.Instance);

            // Assert
            Assert.AreEqual(ColumnDefaultValue.Literal, kind);
        }

        [TestMethod]
        public void TestPostgreSqlSchemaDialectNormalizeLiteralRemovesTheCastOfTheText()
        {
            // Act
            var actual = PostgreSqlSchemaDialect.Instance.NormalizeLiteral(@"'abc'::character varying");

            // Assert
            Assert.AreEqual(@"'abc'", actual);
        }

        [TestMethod]
        public void TestPostgreSqlSchemaDialectNormalizeLiteralRewritesAQuotedHexadecimal()
        {
            // Act
            var actual = PostgreSqlSchemaDialect.Instance.NormalizeLiteral(@"'\xDEAD'");

            // Assert
            Assert.AreEqual(@"X'DEAD'", actual);
        }

        [TestMethod]
        public void TestPostgreSqlSchemaDialectNormalizeLiteralDoesNotUnderstandAnEscapeText()
        {
            // Act
            var actual = PostgreSqlSchemaDialect.Instance.NormalizeLiteral(@"E'a\nb'");

            // Assert
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void TestPostgreSqlSchemaDialectIsDateOnlyOfADate()
        {
            // Act
            var actual = PostgreSqlSchemaDialect.Instance.IsDateOnly("date");

            // Assert
            Assert.IsTrue(actual);
        }

        #endregion
    }
}
