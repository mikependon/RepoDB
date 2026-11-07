#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema.Enumerations;

namespace RepoDb.Schema.AuroraDbMySqlConnector.UnitTests.Dialects
{
    [TestClass]
    public class AuroraDbMySqlConnectorSchemaDialectTest
    {
        #region Public Methods

        [TestMethod]
        public void TestAuroraDbMySqlConnectorSchemaDialectFamily()
        {
            // Act
            var family = AuroraDbMySqlConnectorSchemaDialect.Instance.Family;

            // Assert
            Assert.AreEqual(@"MySql", family);
        }

        [TestMethod]
        public void TestAuroraDbMySqlConnectorSchemaDialectIdentifierQuotesStartWithTheFirstOpening()
        {
            // Act
            var quote = AuroraDbMySqlConnectorSchemaDialect.Instance.IdentifierQuotes[0];

            // Assert
            Assert.AreEqual('`', quote.Open);
        }

        [TestMethod]
        public void TestAuroraDbMySqlConnectorSchemaDialectIdentifierQuotesStartWithTheFirstClosing()
        {
            // Act
            var quote = AuroraDbMySqlConnectorSchemaDialect.Instance.IdentifierQuotes[0];

            // Assert
            Assert.AreEqual('`', quote.Close);
        }

        [TestMethod]
        public void TestAuroraDbMySqlConnectorSchemaDialectIdentifierQuotesCount()
        {
            // Act
            var quotes = AuroraDbMySqlConnectorSchemaDialect.Instance.IdentifierQuotes;

            // Assert
            Assert.AreEqual(1, quotes.Count);
        }

        [TestMethod]
        public void TestAuroraDbMySqlConnectorSchemaDialectNonDeterministicFunctionsHaveNow()
        {
            // Act
            var functions = AuroraDbMySqlConnectorSchemaDialect.Instance.NonDeterministicFunctions;

            // Assert
            CollectionAssert.Contains(functions.ToList(), "now");
        }

        [TestMethod]
        public void TestAuroraDbMySqlConnectorSchemaDialectHasNoNonPortableOperators()
        {
            // Act
            var operators = AuroraDbMySqlConnectorSchemaDialect.Instance.NonPortableOperators;

            // Assert
            Assert.AreEqual(0, operators.Count);
        }

        [TestMethod]
        public void TestAuroraDbMySqlConnectorSchemaDialectClassifiesTheCurrentTimestampOfNow()
        {
            // Act
            var kind = SchemaDefaultValue.Classify(@"now()",
                AuroraDbMySqlConnectorSchemaDialect.Instance);

            // Assert
            Assert.AreEqual(ColumnDefaultValue.CurrentTimestamp, kind);
        }

        [TestMethod]
        public void TestAuroraDbMySqlConnectorSchemaDialectClassifiesTheCurrentTimestampInUtc()
        {
            // Act
            var kind = SchemaDefaultValue.Classify(@"utc_timestamp()",
                AuroraDbMySqlConnectorSchemaDialect.Instance);

            // Assert
            Assert.AreEqual(ColumnDefaultValue.CurrentTimestampUtc, kind);
        }

        [TestMethod]
        public void TestAuroraDbMySqlConnectorSchemaDialectClassifiesTheCurrentDateOfCurdate()
        {
            // Act
            var kind = SchemaDefaultValue.Classify(@"curdate()",
                AuroraDbMySqlConnectorSchemaDialect.Instance);

            // Assert
            Assert.AreEqual(ColumnDefaultValue.CurrentDate, kind);
        }

        [TestMethod]
        public void TestAuroraDbMySqlConnectorSchemaDialectClassifiesTheCurrentTimeOfCurtime()
        {
            // Act
            var kind = SchemaDefaultValue.Classify(@"curtime()",
                AuroraDbMySqlConnectorSchemaDialect.Instance);

            // Assert
            Assert.AreEqual(ColumnDefaultValue.CurrentTime, kind);
        }

        [TestMethod]
        public void TestAuroraDbMySqlConnectorSchemaDialectClassifiesTheNewGuidOfUuid()
        {
            // Act
            var kind = SchemaDefaultValue.Classify(@"uuid()",
                AuroraDbMySqlConnectorSchemaDialect.Instance);

            // Assert
            Assert.AreEqual(ColumnDefaultValue.NewGuid, kind);
        }

        [TestMethod]
        public void TestAuroraDbMySqlConnectorSchemaDialectClassifiesTheRandomNumber()
        {
            // Act
            var kind = SchemaDefaultValue.Classify(@"rand()",
                AuroraDbMySqlConnectorSchemaDialect.Instance);

            // Assert
            Assert.AreEqual(ColumnDefaultValue.RandomNumber, kind);
        }

        [TestMethod]
        public void TestAuroraDbMySqlConnectorSchemaDialectClassifiesTheCurrentDatabase()
        {
            // Act
            var kind = SchemaDefaultValue.Classify(@"database()",
                AuroraDbMySqlConnectorSchemaDialect.Instance);

            // Assert
            Assert.AreEqual(ColumnDefaultValue.CurrentDatabase, kind);
        }

        [TestMethod]
        public void TestAuroraDbMySqlConnectorSchemaDialectClassifiesATextWithACharacterSetIntroducer()
        {
            // Act
            var kind = SchemaDefaultValue.Classify(@"_utf8mb4'abc'",
                AuroraDbMySqlConnectorSchemaDialect.Instance);

            // Assert
            Assert.AreEqual(ColumnDefaultValue.Literal, kind);
        }

        [TestMethod]
        public void TestAuroraDbMySqlConnectorSchemaDialectNormalizeLiteralRemovesTheCharacterSetIntroducer()
        {
            // Act
            var actual = AuroraDbMySqlConnectorSchemaDialect.Instance.NormalizeLiteral(@"_utf8mb4'abc'");

            // Assert
            Assert.AreEqual(@"'abc'", actual);
        }

        [TestMethod]
        public void TestAuroraDbMySqlConnectorSchemaDialectNormalizeLiteralRewritesAHexadecimalNumber()
        {
            // Act
            var actual = AuroraDbMySqlConnectorSchemaDialect.Instance.NormalizeLiteral(@"0xDEAD");

            // Assert
            Assert.AreEqual(@"X'DEAD'", actual);
        }

        [TestMethod]
        public void TestAuroraDbMySqlConnectorSchemaDialectIsDateOnlyOfADate()
        {
            // Act
            var actual = AuroraDbMySqlConnectorSchemaDialect.Instance.IsDateOnly("date");

            // Assert
            Assert.IsTrue(actual);
        }

        #endregion
    }
}
