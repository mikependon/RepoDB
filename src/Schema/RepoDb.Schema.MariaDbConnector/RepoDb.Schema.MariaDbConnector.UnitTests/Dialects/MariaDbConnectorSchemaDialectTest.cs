#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema.Enumerations;

namespace RepoDb.Schema.MariaDbConnector.UnitTests.Dialects
{
    [TestClass]
    public class MariaDbConnectorSchemaDialectTest
    {
        #region Public Methods

        [TestMethod]
        public void TestMariaDbConnectorSchemaDialectFamily()
        {
            // Act
            var family = MariaDbConnectorSchemaDialect.Instance.Family;

            // Assert
            Assert.AreEqual(@"MySql", family);
        }

        [TestMethod]
        public void TestMariaDbConnectorSchemaDialectIdentifierQuotesStartWithTheFirstOpening()
        {
            // Act
            var quote = MariaDbConnectorSchemaDialect.Instance.IdentifierQuotes[0];

            // Assert
            Assert.AreEqual('`', quote.Open);
        }

        [TestMethod]
        public void TestMariaDbConnectorSchemaDialectIdentifierQuotesStartWithTheFirstClosing()
        {
            // Act
            var quote = MariaDbConnectorSchemaDialect.Instance.IdentifierQuotes[0];

            // Assert
            Assert.AreEqual('`', quote.Close);
        }

        [TestMethod]
        public void TestMariaDbConnectorSchemaDialectIdentifierQuotesCount()
        {
            // Act
            var quotes = MariaDbConnectorSchemaDialect.Instance.IdentifierQuotes;

            // Assert
            Assert.AreEqual(1, quotes.Count);
        }

        [TestMethod]
        public void TestMariaDbConnectorSchemaDialectNonDeterministicFunctionsHaveNow()
        {
            // Act
            var functions = MariaDbConnectorSchemaDialect.Instance.NonDeterministicFunctions;

            // Assert
            CollectionAssert.Contains(functions.ToList(), "now");
        }

        [TestMethod]
        public void TestMariaDbConnectorSchemaDialectHasNoNonPortableOperators()
        {
            // Act
            var operators = MariaDbConnectorSchemaDialect.Instance.NonPortableOperators;

            // Assert
            Assert.AreEqual(0, operators.Count);
        }

        [TestMethod]
        public void TestMariaDbConnectorSchemaDialectClassifiesTheCurrentTimestampOfNow()
        {
            // Act
            var kind = SchemaDefaultValue.Classify(@"now()",
                MariaDbConnectorSchemaDialect.Instance);

            // Assert
            Assert.AreEqual(ColumnDefaultValue.CurrentTimestamp, kind);
        }

        [TestMethod]
        public void TestMariaDbConnectorSchemaDialectClassifiesTheCurrentTimestampInUtc()
        {
            // Act
            var kind = SchemaDefaultValue.Classify(@"utc_timestamp()",
                MariaDbConnectorSchemaDialect.Instance);

            // Assert
            Assert.AreEqual(ColumnDefaultValue.CurrentTimestampUtc, kind);
        }

        [TestMethod]
        public void TestMariaDbConnectorSchemaDialectClassifiesTheCurrentDateOfCurdate()
        {
            // Act
            var kind = SchemaDefaultValue.Classify(@"curdate()",
                MariaDbConnectorSchemaDialect.Instance);

            // Assert
            Assert.AreEqual(ColumnDefaultValue.CurrentDate, kind);
        }

        [TestMethod]
        public void TestMariaDbConnectorSchemaDialectClassifiesTheCurrentTimeOfCurtime()
        {
            // Act
            var kind = SchemaDefaultValue.Classify(@"curtime()",
                MariaDbConnectorSchemaDialect.Instance);

            // Assert
            Assert.AreEqual(ColumnDefaultValue.CurrentTime, kind);
        }

        [TestMethod]
        public void TestMariaDbConnectorSchemaDialectClassifiesTheNewGuidOfUuid()
        {
            // Act
            var kind = SchemaDefaultValue.Classify(@"uuid()",
                MariaDbConnectorSchemaDialect.Instance);

            // Assert
            Assert.AreEqual(ColumnDefaultValue.NewGuid, kind);
        }

        [TestMethod]
        public void TestMariaDbConnectorSchemaDialectClassifiesTheRandomNumber()
        {
            // Act
            var kind = SchemaDefaultValue.Classify(@"rand()",
                MariaDbConnectorSchemaDialect.Instance);

            // Assert
            Assert.AreEqual(ColumnDefaultValue.RandomNumber, kind);
        }

        [TestMethod]
        public void TestMariaDbConnectorSchemaDialectClassifiesTheCurrentDatabase()
        {
            // Act
            var kind = SchemaDefaultValue.Classify(@"database()",
                MariaDbConnectorSchemaDialect.Instance);

            // Assert
            Assert.AreEqual(ColumnDefaultValue.CurrentDatabase, kind);
        }

        [TestMethod]
        public void TestMariaDbConnectorSchemaDialectClassifiesATextWithACharacterSetIntroducer()
        {
            // Act
            var kind = SchemaDefaultValue.Classify(@"_utf8mb4'abc'",
                MariaDbConnectorSchemaDialect.Instance);

            // Assert
            Assert.AreEqual(ColumnDefaultValue.Literal, kind);
        }

        [TestMethod]
        public void TestMariaDbConnectorSchemaDialectNormalizeLiteralRemovesTheCharacterSetIntroducer()
        {
            // Act
            var actual = MariaDbConnectorSchemaDialect.Instance.NormalizeLiteral(@"_utf8mb4'abc'");

            // Assert
            Assert.AreEqual(@"'abc'", actual);
        }

        [TestMethod]
        public void TestMariaDbConnectorSchemaDialectNormalizeLiteralRewritesAHexadecimalNumber()
        {
            // Act
            var actual = MariaDbConnectorSchemaDialect.Instance.NormalizeLiteral(@"0xDEAD");

            // Assert
            Assert.AreEqual(@"X'DEAD'", actual);
        }

        [TestMethod]
        public void TestMariaDbConnectorSchemaDialectIsDateOnlyOfADate()
        {
            // Act
            var actual = MariaDbConnectorSchemaDialect.Instance.IsDateOnly("date");

            // Assert
            Assert.IsTrue(actual);
        }

        #endregion
    }
}
