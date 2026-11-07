#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema.Enumerations;

namespace RepoDb.Schema.Db2.UnitTests.Dialects
{
    [TestClass]
    public class Db2SchemaDialectTest
    {
        #region Public Methods

        [TestMethod]
        public void TestDb2SchemaDialectFamily()
        {
            // Act
            var family = Db2SchemaDialect.Instance.Family;

            // Assert
            Assert.AreEqual(@"Db2", family);
        }

        [TestMethod]
        public void TestDb2SchemaDialectIdentifierQuotesAreTheStandardOnes()
        {
            // Act
            var quotes = Db2SchemaDialect.Instance.IdentifierQuotes;

            // Assert
            Assert.AreEqual(1, quotes.Count);
            Assert.AreEqual('"', quotes[0].Open);
        }

        [TestMethod]
        public void TestDb2SchemaDialectHasNoNonPortableOperators()
        {
            // Act
            var operators = Db2SchemaDialect.Instance.NonPortableOperators;

            // Assert
            Assert.AreEqual(0, operators.Count);
        }

        [TestMethod]
        public void TestDb2SchemaDialectClassifiesTheCurrentTimestampWithASpace()
        {
            // Act
            var kind = SchemaDefaultValue.Classify(@"CURRENT TIMESTAMP",
                Db2SchemaDialect.Instance);

            // Assert
            Assert.AreEqual(ColumnDefaultValue.CurrentTimestamp, kind);
        }

        [TestMethod]
        public void TestDb2SchemaDialectClassifiesTheCurrentDateWithASpace()
        {
            // Act
            var kind = SchemaDefaultValue.Classify(@"current date",
                Db2SchemaDialect.Instance);

            // Assert
            Assert.AreEqual(ColumnDefaultValue.CurrentDate, kind);
        }

        [TestMethod]
        public void TestDb2SchemaDialectClassifiesTheCurrentTimeWithASpace()
        {
            // Act
            var kind = SchemaDefaultValue.Classify(@"current time",
                Db2SchemaDialect.Instance);

            // Assert
            Assert.AreEqual(ColumnDefaultValue.CurrentTime, kind);
        }

        [TestMethod]
        public void TestDb2SchemaDialectIsDateOnlyOfADate()
        {
            // Act
            var actual = Db2SchemaDialect.Instance.IsDateOnly("date");

            // Assert
            Assert.IsTrue(actual);
        }

        #endregion
    }
}
