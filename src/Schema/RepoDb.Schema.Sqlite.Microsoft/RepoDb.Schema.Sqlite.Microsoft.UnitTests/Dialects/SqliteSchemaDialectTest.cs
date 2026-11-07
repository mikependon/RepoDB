#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema.Enumerations;

namespace RepoDb.Schema.Sqlite.Microsoft.UnitTests.Dialects
{
    [TestClass]
    public class SqliteSchemaDialectTest
    {
        #region Public Methods

        [TestMethod]
        public void TestSqliteSchemaDialectFamily()
        {
            // Act
            var family = SqliteSchemaDialect.Instance.Family;

            // Assert
            Assert.AreEqual(@"Sqlite", family);
        }

        [TestMethod]
        public void TestSqliteSchemaDialectIdentifierQuotesStartWithTheFirstOpening()
        {
            // Act
            var quote = SqliteSchemaDialect.Instance.IdentifierQuotes[0];

            // Assert
            Assert.AreEqual('[', quote.Open);
        }

        [TestMethod]
        public void TestSqliteSchemaDialectIdentifierQuotesStartWithTheFirstClosing()
        {
            // Act
            var quote = SqliteSchemaDialect.Instance.IdentifierQuotes[0];

            // Assert
            Assert.AreEqual(']', quote.Close);
        }

        [TestMethod]
        public void TestSqliteSchemaDialectIdentifierQuotesCount()
        {
            // Act
            var quotes = SqliteSchemaDialect.Instance.IdentifierQuotes;

            // Assert
            Assert.AreEqual(3, quotes.Count);
        }

        [TestMethod]
        public void TestSqliteSchemaDialectNonDeterministicFunctionsHaveRandom()
        {
            // Act
            var functions = SqliteSchemaDialect.Instance.NonDeterministicFunctions;

            // Assert
            CollectionAssert.Contains(functions.ToList(), "random");
        }

        [TestMethod]
        public void TestSqliteSchemaDialectHasNoNonPortableOperators()
        {
            // Act
            var operators = SqliteSchemaDialect.Instance.NonPortableOperators;

            // Assert
            Assert.AreEqual(0, operators.Count);
        }

        [TestMethod]
        public void TestSqliteSchemaDialectClassifiesTheCurrentTimestampOfDatetime()
        {
            // Act
            var kind = SchemaDefaultValue.Classify(@"(datetime('now','localtime'))",
                SqliteSchemaDialect.Instance);

            // Assert
            Assert.AreEqual(ColumnDefaultValue.CurrentTimestamp, kind);
        }

        [TestMethod]
        public void TestSqliteSchemaDialectClassifiesTheCurrentTimestampInUtc()
        {
            // Act
            var kind = SchemaDefaultValue.Classify(@"(datetime('now'))",
                SqliteSchemaDialect.Instance);

            // Assert
            Assert.AreEqual(ColumnDefaultValue.CurrentTimestampUtc, kind);
        }

        [TestMethod]
        public void TestSqliteSchemaDialectClassifiesTheCurrentDate()
        {
            // Act
            var kind = SchemaDefaultValue.Classify(@"(date('now'))",
                SqliteSchemaDialect.Instance);

            // Assert
            Assert.AreEqual(ColumnDefaultValue.CurrentDate, kind);
        }

        [TestMethod]
        public void TestSqliteSchemaDialectIsDateOnlyOfADate()
        {
            // Act
            var actual = SqliteSchemaDialect.Instance.IsDateOnly("date");

            // Assert
            Assert.IsTrue(actual);
        }

        #endregion
    }
}
