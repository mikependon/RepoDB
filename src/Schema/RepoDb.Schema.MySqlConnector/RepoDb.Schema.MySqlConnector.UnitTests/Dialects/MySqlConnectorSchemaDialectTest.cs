#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema.Enumerations;

namespace RepoDb.Schema.MySqlConnector.UnitTests.Dialects
{
    [TestClass]
    public class MySqlConnectorSchemaDialectTest
    {
        #region Public Methods

        [TestMethod]
        public void TestMySqlConnectorSchemaDialectFamily()
        {
            // Act
            var family = MySqlConnectorSchemaDialect.Instance.Family;

            // Assert
            Assert.AreEqual(@"MySql", family);
        }

        [TestMethod]
        public void TestMySqlConnectorSchemaDialectIdentifierQuotesStartWithTheFirstOpening()
        {
            // Act
            var quote = MySqlConnectorSchemaDialect.Instance.IdentifierQuotes[0];

            // Assert
            Assert.AreEqual('`', quote.Open);
        }

        [TestMethod]
        public void TestMySqlConnectorSchemaDialectIdentifierQuotesStartWithTheFirstClosing()
        {
            // Act
            var quote = MySqlConnectorSchemaDialect.Instance.IdentifierQuotes[0];

            // Assert
            Assert.AreEqual('`', quote.Close);
        }

        [TestMethod]
        public void TestMySqlConnectorSchemaDialectIdentifierQuotesCount()
        {
            // Act
            var quotes = MySqlConnectorSchemaDialect.Instance.IdentifierQuotes;

            // Assert
            Assert.AreEqual(1, quotes.Count);
        }

        [TestMethod]
        public void TestMySqlConnectorSchemaDialectNonDeterministicFunctionsHaveNow()
        {
            // Act
            var functions = MySqlConnectorSchemaDialect.Instance.NonDeterministicFunctions;

            // Assert
            CollectionAssert.Contains(functions.ToList(), "now");
        }

        [TestMethod]
        public void TestMySqlConnectorSchemaDialectHasNoNonPortableOperators()
        {
            // Act
            var operators = MySqlConnectorSchemaDialect.Instance.NonPortableOperators;

            // Assert
            Assert.AreEqual(0, operators.Count);
        }

        [TestMethod]
        public void TestMySqlConnectorSchemaDialectClassifiesTheCurrentTimestampOfNow()
        {
            // Act
            var kind = SchemaDefaultValue.Classify(@"now()",
                MySqlConnectorSchemaDialect.Instance);

            // Assert
            Assert.AreEqual(ColumnDefaultValue.CurrentTimestamp, kind);
        }

        [TestMethod]
        public void TestMySqlConnectorSchemaDialectClassifiesTheCurrentTimestampInUtc()
        {
            // Act
            var kind = SchemaDefaultValue.Classify(@"utc_timestamp()",
                MySqlConnectorSchemaDialect.Instance);

            // Assert
            Assert.AreEqual(ColumnDefaultValue.CurrentTimestampUtc, kind);
        }

        [TestMethod]
        public void TestMySqlConnectorSchemaDialectClassifiesTheCurrentDateOfCurdate()
        {
            // Act
            var kind = SchemaDefaultValue.Classify(@"curdate()",
                MySqlConnectorSchemaDialect.Instance);

            // Assert
            Assert.AreEqual(ColumnDefaultValue.CurrentDate, kind);
        }

        [TestMethod]
        public void TestMySqlConnectorSchemaDialectClassifiesTheCurrentTimeOfCurtime()
        {
            // Act
            var kind = SchemaDefaultValue.Classify(@"curtime()",
                MySqlConnectorSchemaDialect.Instance);

            // Assert
            Assert.AreEqual(ColumnDefaultValue.CurrentTime, kind);
        }

        [TestMethod]
        public void TestMySqlConnectorSchemaDialectClassifiesTheNewGuidOfUuid()
        {
            // Act
            var kind = SchemaDefaultValue.Classify(@"uuid()",
                MySqlConnectorSchemaDialect.Instance);

            // Assert
            Assert.AreEqual(ColumnDefaultValue.NewGuid, kind);
        }

        [TestMethod]
        public void TestMySqlConnectorSchemaDialectClassifiesTheRandomNumber()
        {
            // Act
            var kind = SchemaDefaultValue.Classify(@"rand()",
                MySqlConnectorSchemaDialect.Instance);

            // Assert
            Assert.AreEqual(ColumnDefaultValue.RandomNumber, kind);
        }

        [TestMethod]
        public void TestMySqlConnectorSchemaDialectClassifiesTheCurrentDatabase()
        {
            // Act
            var kind = SchemaDefaultValue.Classify(@"database()",
                MySqlConnectorSchemaDialect.Instance);

            // Assert
            Assert.AreEqual(ColumnDefaultValue.CurrentDatabase, kind);
        }

        [TestMethod]
        public void TestMySqlConnectorSchemaDialectClassifiesATextWithACharacterSetIntroducer()
        {
            // Act
            var kind = SchemaDefaultValue.Classify(@"_utf8mb4'abc'",
                MySqlConnectorSchemaDialect.Instance);

            // Assert
            Assert.AreEqual(ColumnDefaultValue.Literal, kind);
        }

        [TestMethod]
        public void TestMySqlConnectorSchemaDialectNormalizeLiteralRemovesTheCharacterSetIntroducer()
        {
            // Act
            var actual = MySqlConnectorSchemaDialect.Instance.NormalizeLiteral(@"_utf8mb4'abc'");

            // Assert
            Assert.AreEqual(@"'abc'", actual);
        }

        [TestMethod]
        public void TestMySqlConnectorSchemaDialectNormalizeLiteralRewritesAHexadecimalNumber()
        {
            // Act
            var actual = MySqlConnectorSchemaDialect.Instance.NormalizeLiteral(@"0xDEAD");

            // Assert
            Assert.AreEqual(@"X'DEAD'", actual);
        }

        [TestMethod]
        public void TestMySqlConnectorSchemaDialectIsDateOnlyOfADate()
        {
            // Act
            var actual = MySqlConnectorSchemaDialect.Instance.IsDateOnly("date");

            // Assert
            Assert.IsTrue(actual);
        }

        #endregion
    }
}
