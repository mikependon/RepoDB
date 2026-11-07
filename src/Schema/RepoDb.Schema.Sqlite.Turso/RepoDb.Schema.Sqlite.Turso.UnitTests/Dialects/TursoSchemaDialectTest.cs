#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema.Enumerations;

namespace RepoDb.Schema.Sqlite.Turso.UnitTests.Dialects
{
    [TestClass]
    public class TursoSchemaDialectTest
    {
        #region Public Methods

        [TestMethod]
        public void TestTursoSchemaDialectFamily()
        {
            // Act
            var family = TursoSchemaDialect.Instance.Family;

            // Assert
            Assert.AreEqual(@"Sqlite", family);
        }

        [TestMethod]
        public void TestTursoSchemaDialectIdentifierQuotesStartWithTheFirstOpening()
        {
            // Act
            var quote = TursoSchemaDialect.Instance.IdentifierQuotes[0];

            // Assert
            Assert.AreEqual('[', quote.Open);
        }

        [TestMethod]
        public void TestTursoSchemaDialectIdentifierQuotesStartWithTheFirstClosing()
        {
            // Act
            var quote = TursoSchemaDialect.Instance.IdentifierQuotes[0];

            // Assert
            Assert.AreEqual(']', quote.Close);
        }

        [TestMethod]
        public void TestTursoSchemaDialectIdentifierQuotesCount()
        {
            // Act
            var quotes = TursoSchemaDialect.Instance.IdentifierQuotes;

            // Assert
            Assert.AreEqual(3, quotes.Count);
        }

        [TestMethod]
        public void TestTursoSchemaDialectNonDeterministicFunctionsHaveRandom()
        {
            // Act
            var functions = TursoSchemaDialect.Instance.NonDeterministicFunctions;

            // Assert
            CollectionAssert.Contains(functions.ToList(), "random");
        }

        [TestMethod]
        public void TestTursoSchemaDialectHasNoNonPortableOperators()
        {
            // Act
            var operators = TursoSchemaDialect.Instance.NonPortableOperators;

            // Assert
            Assert.AreEqual(0, operators.Count);
        }

        [TestMethod]
        public void TestTursoSchemaDialectClassifiesTheCurrentTimestampOfDatetime()
        {
            // Act
            var kind = SchemaDefaultValue.Classify(@"(datetime('now','localtime'))",
                TursoSchemaDialect.Instance);

            // Assert
            Assert.AreEqual(ColumnDefaultValue.CurrentTimestamp, kind);
        }

        [TestMethod]
        public void TestTursoSchemaDialectClassifiesTheCurrentTimestampInUtc()
        {
            // Act
            var kind = SchemaDefaultValue.Classify(@"(datetime('now'))",
                TursoSchemaDialect.Instance);

            // Assert
            Assert.AreEqual(ColumnDefaultValue.CurrentTimestampUtc, kind);
        }

        [TestMethod]
        public void TestTursoSchemaDialectClassifiesTheCurrentDate()
        {
            // Act
            var kind = SchemaDefaultValue.Classify(@"(date('now'))",
                TursoSchemaDialect.Instance);

            // Assert
            Assert.AreEqual(ColumnDefaultValue.CurrentDate, kind);
        }

        [TestMethod]
        public void TestTursoSchemaDialectIsDateOnlyOfADate()
        {
            // Act
            var actual = TursoSchemaDialect.Instance.IsDateOnly("date");

            // Assert
            Assert.IsTrue(actual);
        }

        #endregion
    }
}
