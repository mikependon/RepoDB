#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema.Enumerations;

namespace RepoDb.Schema.MySql.UnitTests.Dialects
{
    [TestClass]
    public class MySqlSchemaDialectTest
    {
        #region Public Methods

        [TestMethod]
        public void TestMySqlSchemaDialectFamily()
        {
            // Act
            var family = MySqlSchemaDialect.Instance.Family;

            // Assert
            Assert.AreEqual(@"MySql", family);
        }

        [TestMethod]
        public void TestMySqlSchemaDialectIdentifierQuotesStartWithTheFirstOpening()
        {
            // Act
            var quote = MySqlSchemaDialect.Instance.IdentifierQuotes[0];

            // Assert
            Assert.AreEqual('`', quote.Open);
        }

        [TestMethod]
        public void TestMySqlSchemaDialectIdentifierQuotesStartWithTheFirstClosing()
        {
            // Act
            var quote = MySqlSchemaDialect.Instance.IdentifierQuotes[0];

            // Assert
            Assert.AreEqual('`', quote.Close);
        }

        [TestMethod]
        public void TestMySqlSchemaDialectIdentifierQuotesCount()
        {
            // Act
            var quotes = MySqlSchemaDialect.Instance.IdentifierQuotes;

            // Assert
            Assert.AreEqual(1, quotes.Count);
        }

        [TestMethod]
        public void TestMySqlSchemaDialectNonDeterministicFunctionsHaveNow()
        {
            // Act
            var functions = MySqlSchemaDialect.Instance.NonDeterministicFunctions;

            // Assert
            CollectionAssert.Contains(functions.ToList(), "now");
        }

        [TestMethod]
        public void TestMySqlSchemaDialectHasNoNonPortableOperators()
        {
            // Act
            var operators = MySqlSchemaDialect.Instance.NonPortableOperators;

            // Assert
            Assert.AreEqual(0, operators.Count);
        }

        [TestMethod]
        public void TestMySqlSchemaDialectClassifiesTheCurrentTimestampOfNow()
        {
            // Act
            var kind = SchemaDefaultValue.Classify(@"now()",
                MySqlSchemaDialect.Instance);

            // Assert
            Assert.AreEqual(ColumnDefaultValue.CurrentTimestamp, kind);
        }

        [TestMethod]
        public void TestMySqlSchemaDialectClassifiesTheCurrentTimestampInUtc()
        {
            // Act
            var kind = SchemaDefaultValue.Classify(@"utc_timestamp()",
                MySqlSchemaDialect.Instance);

            // Assert
            Assert.AreEqual(ColumnDefaultValue.CurrentTimestampUtc, kind);
        }

        [TestMethod]
        public void TestMySqlSchemaDialectClassifiesTheCurrentDateOfCurdate()
        {
            // Act
            var kind = SchemaDefaultValue.Classify(@"curdate()",
                MySqlSchemaDialect.Instance);

            // Assert
            Assert.AreEqual(ColumnDefaultValue.CurrentDate, kind);
        }

        [TestMethod]
        public void TestMySqlSchemaDialectClassifiesTheCurrentTimeOfCurtime()
        {
            // Act
            var kind = SchemaDefaultValue.Classify(@"curtime()",
                MySqlSchemaDialect.Instance);

            // Assert
            Assert.AreEqual(ColumnDefaultValue.CurrentTime, kind);
        }

        [TestMethod]
        public void TestMySqlSchemaDialectClassifiesTheNewGuidOfUuid()
        {
            // Act
            var kind = SchemaDefaultValue.Classify(@"uuid()",
                MySqlSchemaDialect.Instance);

            // Assert
            Assert.AreEqual(ColumnDefaultValue.NewGuid, kind);
        }

        [TestMethod]
        public void TestMySqlSchemaDialectClassifiesTheRandomNumber()
        {
            // Act
            var kind = SchemaDefaultValue.Classify(@"rand()",
                MySqlSchemaDialect.Instance);

            // Assert
            Assert.AreEqual(ColumnDefaultValue.RandomNumber, kind);
        }

        [TestMethod]
        public void TestMySqlSchemaDialectClassifiesTheCurrentDatabase()
        {
            // Act
            var kind = SchemaDefaultValue.Classify(@"database()",
                MySqlSchemaDialect.Instance);

            // Assert
            Assert.AreEqual(ColumnDefaultValue.CurrentDatabase, kind);
        }

        [TestMethod]
        public void TestMySqlSchemaDialectClassifiesATextWithACharacterSetIntroducer()
        {
            // Act
            var kind = SchemaDefaultValue.Classify(@"_utf8mb4'abc'",
                MySqlSchemaDialect.Instance);

            // Assert
            Assert.AreEqual(ColumnDefaultValue.Literal, kind);
        }

        [TestMethod]
        public void TestMySqlSchemaDialectNormalizeLiteralRemovesTheCharacterSetIntroducer()
        {
            // Act
            var actual = MySqlSchemaDialect.Instance.NormalizeLiteral(@"_utf8mb4'abc'");

            // Assert
            Assert.AreEqual(@"'abc'", actual);
        }

        [TestMethod]
        public void TestMySqlSchemaDialectNormalizeLiteralRewritesAHexadecimalNumber()
        {
            // Act
            var actual = MySqlSchemaDialect.Instance.NormalizeLiteral(@"0xDEAD");

            // Assert
            Assert.AreEqual(@"X'DEAD'", actual);
        }

        [TestMethod]
        public void TestMySqlSchemaDialectIsDateOnlyOfADate()
        {
            // Act
            var actual = MySqlSchemaDialect.Instance.IsDateOnly("date");

            // Assert
            Assert.IsTrue(actual);
        }

        #endregion
    }
}
