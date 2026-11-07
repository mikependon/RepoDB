#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema.Enumerations;

namespace RepoDb.Schema.MariaDb.UnitTests.Dialects
{
    [TestClass]
    public class MariaDbSchemaDialectTest
    {
        #region Public Methods

        [TestMethod]
        public void TestMariaDbSchemaDialectFamily()
        {
            // Act
            var family = MariaDbSchemaDialect.Instance.Family;

            // Assert
            Assert.AreEqual(@"MySql", family);
        }

        [TestMethod]
        public void TestMariaDbSchemaDialectIdentifierQuotesStartWithTheFirstOpening()
        {
            // Act
            var quote = MariaDbSchemaDialect.Instance.IdentifierQuotes[0];

            // Assert
            Assert.AreEqual('`', quote.Open);
        }

        [TestMethod]
        public void TestMariaDbSchemaDialectIdentifierQuotesStartWithTheFirstClosing()
        {
            // Act
            var quote = MariaDbSchemaDialect.Instance.IdentifierQuotes[0];

            // Assert
            Assert.AreEqual('`', quote.Close);
        }

        [TestMethod]
        public void TestMariaDbSchemaDialectIdentifierQuotesCount()
        {
            // Act
            var quotes = MariaDbSchemaDialect.Instance.IdentifierQuotes;

            // Assert
            Assert.AreEqual(1, quotes.Count);
        }

        [TestMethod]
        public void TestMariaDbSchemaDialectNonDeterministicFunctionsHaveNow()
        {
            // Act
            var functions = MariaDbSchemaDialect.Instance.NonDeterministicFunctions;

            // Assert
            CollectionAssert.Contains(functions.ToList(), "now");
        }

        [TestMethod]
        public void TestMariaDbSchemaDialectHasNoNonPortableOperators()
        {
            // Act
            var operators = MariaDbSchemaDialect.Instance.NonPortableOperators;

            // Assert
            Assert.AreEqual(0, operators.Count);
        }

        [TestMethod]
        public void TestMariaDbSchemaDialectClassifiesTheCurrentTimestampOfNow()
        {
            // Act
            var kind = SchemaDefaultValue.Classify(@"now()",
                MariaDbSchemaDialect.Instance);

            // Assert
            Assert.AreEqual(ColumnDefaultValue.CurrentTimestamp, kind);
        }

        [TestMethod]
        public void TestMariaDbSchemaDialectClassifiesTheCurrentTimestampInUtc()
        {
            // Act
            var kind = SchemaDefaultValue.Classify(@"utc_timestamp()",
                MariaDbSchemaDialect.Instance);

            // Assert
            Assert.AreEqual(ColumnDefaultValue.CurrentTimestampUtc, kind);
        }

        [TestMethod]
        public void TestMariaDbSchemaDialectClassifiesTheCurrentDateOfCurdate()
        {
            // Act
            var kind = SchemaDefaultValue.Classify(@"curdate()",
                MariaDbSchemaDialect.Instance);

            // Assert
            Assert.AreEqual(ColumnDefaultValue.CurrentDate, kind);
        }

        [TestMethod]
        public void TestMariaDbSchemaDialectClassifiesTheCurrentTimeOfCurtime()
        {
            // Act
            var kind = SchemaDefaultValue.Classify(@"curtime()",
                MariaDbSchemaDialect.Instance);

            // Assert
            Assert.AreEqual(ColumnDefaultValue.CurrentTime, kind);
        }

        [TestMethod]
        public void TestMariaDbSchemaDialectClassifiesTheNewGuidOfUuid()
        {
            // Act
            var kind = SchemaDefaultValue.Classify(@"uuid()",
                MariaDbSchemaDialect.Instance);

            // Assert
            Assert.AreEqual(ColumnDefaultValue.NewGuid, kind);
        }

        [TestMethod]
        public void TestMariaDbSchemaDialectClassifiesTheRandomNumber()
        {
            // Act
            var kind = SchemaDefaultValue.Classify(@"rand()",
                MariaDbSchemaDialect.Instance);

            // Assert
            Assert.AreEqual(ColumnDefaultValue.RandomNumber, kind);
        }

        [TestMethod]
        public void TestMariaDbSchemaDialectClassifiesTheCurrentDatabase()
        {
            // Act
            var kind = SchemaDefaultValue.Classify(@"database()",
                MariaDbSchemaDialect.Instance);

            // Assert
            Assert.AreEqual(ColumnDefaultValue.CurrentDatabase, kind);
        }

        [TestMethod]
        public void TestMariaDbSchemaDialectClassifiesATextWithACharacterSetIntroducer()
        {
            // Act
            var kind = SchemaDefaultValue.Classify(@"_utf8mb4'abc'",
                MariaDbSchemaDialect.Instance);

            // Assert
            Assert.AreEqual(ColumnDefaultValue.Literal, kind);
        }

        [TestMethod]
        public void TestMariaDbSchemaDialectNormalizeLiteralRemovesTheCharacterSetIntroducer()
        {
            // Act
            var actual = MariaDbSchemaDialect.Instance.NormalizeLiteral(@"_utf8mb4'abc'");

            // Assert
            Assert.AreEqual(@"'abc'", actual);
        }

        [TestMethod]
        public void TestMariaDbSchemaDialectNormalizeLiteralRewritesAHexadecimalNumber()
        {
            // Act
            var actual = MariaDbSchemaDialect.Instance.NormalizeLiteral(@"0xDEAD");

            // Assert
            Assert.AreEqual(@"X'DEAD'", actual);
        }

        [TestMethod]
        public void TestMariaDbSchemaDialectIsDateOnlyOfADate()
        {
            // Act
            var actual = MariaDbSchemaDialect.Instance.IsDateOnly("date");

            // Assert
            Assert.IsTrue(actual);
        }

        #endregion
    }
}
