#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema.Enumerations;

namespace RepoDb.Schema.Sqlite.Ahtola.UnitTests.Dialects
{
    [TestClass]
    public class AhtolaSchemaDialectTest
    {
        #region Public Methods

        [TestMethod]
        public void TestAhtolaSchemaDialectFamily()
        {
            // Act
            var family = AhtolaSchemaDialect.Instance.Family;

            // Assert
            Assert.AreEqual(@"Sqlite", family);
        }

        [TestMethod]
        public void TestAhtolaSchemaDialectIdentifierQuotesStartWithTheFirstOpening()
        {
            // Act
            var quote = AhtolaSchemaDialect.Instance.IdentifierQuotes[0];

            // Assert
            Assert.AreEqual('[', quote.Open);
        }

        [TestMethod]
        public void TestAhtolaSchemaDialectIdentifierQuotesStartWithTheFirstClosing()
        {
            // Act
            var quote = AhtolaSchemaDialect.Instance.IdentifierQuotes[0];

            // Assert
            Assert.AreEqual(']', quote.Close);
        }

        [TestMethod]
        public void TestAhtolaSchemaDialectIdentifierQuotesCount()
        {
            // Act
            var quotes = AhtolaSchemaDialect.Instance.IdentifierQuotes;

            // Assert
            Assert.AreEqual(3, quotes.Count);
        }

        [TestMethod]
        public void TestAhtolaSchemaDialectNonDeterministicFunctionsHaveRandom()
        {
            // Act
            var functions = AhtolaSchemaDialect.Instance.NonDeterministicFunctions;

            // Assert
            CollectionAssert.Contains(functions.ToList(), "random");
        }

        [TestMethod]
        public void TestAhtolaSchemaDialectHasNoNonPortableOperators()
        {
            // Act
            var operators = AhtolaSchemaDialect.Instance.NonPortableOperators;

            // Assert
            Assert.AreEqual(0, operators.Count);
        }

        [TestMethod]
        public void TestAhtolaSchemaDialectClassifiesTheCurrentTimestampOfDatetime()
        {
            // Act
            var kind = SchemaDefaultValue.Classify(@"(datetime('now','localtime'))",
                AhtolaSchemaDialect.Instance);

            // Assert
            Assert.AreEqual(ColumnDefaultValue.CurrentTimestamp, kind);
        }

        [TestMethod]
        public void TestAhtolaSchemaDialectClassifiesTheCurrentTimestampInUtc()
        {
            // Act
            var kind = SchemaDefaultValue.Classify(@"(datetime('now'))",
                AhtolaSchemaDialect.Instance);

            // Assert
            Assert.AreEqual(ColumnDefaultValue.CurrentTimestampUtc, kind);
        }

        [TestMethod]
        public void TestAhtolaSchemaDialectClassifiesTheCurrentDate()
        {
            // Act
            var kind = SchemaDefaultValue.Classify(@"(date('now'))",
                AhtolaSchemaDialect.Instance);

            // Assert
            Assert.AreEqual(ColumnDefaultValue.CurrentDate, kind);
        }

        [TestMethod]
        public void TestAhtolaSchemaDialectIsDateOnlyOfADate()
        {
            // Act
            var actual = AhtolaSchemaDialect.Instance.IsDateOnly("date");

            // Assert
            Assert.IsTrue(actual);
        }

        #endregion
    }
}
