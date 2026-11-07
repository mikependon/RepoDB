#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema.Enumerations;

namespace RepoDb.Schema.ClickHouse.UnitTests.Dialects
{
    [TestClass]
    public class ClickHouseSchemaDialectTest
    {
        #region Public Methods

        [TestMethod]
        public void TestClickHouseSchemaDialectFamily()
        {
            // Act
            var family = ClickHouseSchemaDialect.Instance.Family;

            // Assert
            Assert.AreEqual(@"ClickHouse", family);
        }

        [TestMethod]
        public void TestClickHouseSchemaDialectIdentifierQuotesStartWithTheFirstOpening()
        {
            // Act
            var quote = ClickHouseSchemaDialect.Instance.IdentifierQuotes[0];

            // Assert
            Assert.AreEqual('`', quote.Open);
        }

        [TestMethod]
        public void TestClickHouseSchemaDialectIdentifierQuotesStartWithTheFirstClosing()
        {
            // Act
            var quote = ClickHouseSchemaDialect.Instance.IdentifierQuotes[0];

            // Assert
            Assert.AreEqual('`', quote.Close);
        }

        [TestMethod]
        public void TestClickHouseSchemaDialectIdentifierQuotesCount()
        {
            // Act
            var quotes = ClickHouseSchemaDialect.Instance.IdentifierQuotes;

            // Assert
            Assert.AreEqual(2, quotes.Count);
        }

        [TestMethod]
        public void TestClickHouseSchemaDialectNonDeterministicFunctionsHaveNow()
        {
            // Act
            var functions = ClickHouseSchemaDialect.Instance.NonDeterministicFunctions;

            // Assert
            CollectionAssert.Contains(functions.ToList(), "now");
        }

        [TestMethod]
        public void TestClickHouseSchemaDialectHasNoNonPortableOperators()
        {
            // Act
            var operators = ClickHouseSchemaDialect.Instance.NonPortableOperators;

            // Assert
            Assert.AreEqual(0, operators.Count);
        }

        [TestMethod]
        public void TestClickHouseSchemaDialectClassifiesTheCurrentTimestampOfNow64()
        {
            // Act
            var kind = SchemaDefaultValue.Classify(@"now64(3)",
                ClickHouseSchemaDialect.Instance);

            // Assert
            Assert.AreEqual(ColumnDefaultValue.CurrentTimestamp, kind);
        }

        [TestMethod]
        public void TestClickHouseSchemaDialectClassifiesTheCurrentDateOfToday()
        {
            // Act
            var kind = SchemaDefaultValue.Classify(@"today()",
                ClickHouseSchemaDialect.Instance);

            // Assert
            Assert.AreEqual(ColumnDefaultValue.CurrentDate, kind);
        }

        [TestMethod]
        public void TestClickHouseSchemaDialectClassifiesTheNewGuid()
        {
            // Act
            var kind = SchemaDefaultValue.Classify(@"generateUUIDv4()",
                ClickHouseSchemaDialect.Instance);

            // Assert
            Assert.AreEqual(ColumnDefaultValue.NewGuid, kind);
        }

        [TestMethod]
        public void TestClickHouseSchemaDialectIsDateOnlyOfADate()
        {
            // Act
            var actual = ClickHouseSchemaDialect.Instance.IsDateOnly("date");

            // Assert
            Assert.IsTrue(actual);
        }

        #endregion
    }
}
