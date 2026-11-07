#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema.Enumerations;

namespace RepoDb.Schema.SqlServer.UnitTests.Dialects
{
    [TestClass]
    public class SqlServerSchemaDialectTest
    {
        #region Public Methods

        [TestMethod]
        public void TestSqlServerSchemaDialectFamily()
        {
            // Act
            var family = SqlServerSchemaDialect.Instance.Family;

            // Assert
            Assert.AreEqual(@"SqlServer", family);
        }

        [TestMethod]
        public void TestSqlServerSchemaDialectIdentifierQuotesStartWithTheFirstOpening()
        {
            // Act
            var quote = SqlServerSchemaDialect.Instance.IdentifierQuotes[0];

            // Assert
            Assert.AreEqual('[', quote.Open);
        }

        [TestMethod]
        public void TestSqlServerSchemaDialectIdentifierQuotesStartWithTheFirstClosing()
        {
            // Act
            var quote = SqlServerSchemaDialect.Instance.IdentifierQuotes[0];

            // Assert
            Assert.AreEqual(']', quote.Close);
        }

        [TestMethod]
        public void TestSqlServerSchemaDialectIdentifierQuotesCount()
        {
            // Act
            var quotes = SqlServerSchemaDialect.Instance.IdentifierQuotes;

            // Assert
            Assert.AreEqual(2, quotes.Count);
        }

        [TestMethod]
        public void TestSqlServerSchemaDialectNonDeterministicFunctionsHaveGetdate()
        {
            // Act
            var functions = SqlServerSchemaDialect.Instance.NonDeterministicFunctions;

            // Assert
            CollectionAssert.Contains(functions.ToList(), "getdate");
        }

        [TestMethod]
        public void TestSqlServerSchemaDialectHasNoNonPortableOperators()
        {
            // Act
            var operators = SqlServerSchemaDialect.Instance.NonPortableOperators;

            // Assert
            Assert.AreEqual(0, operators.Count);
        }

        [TestMethod]
        public void TestSqlServerSchemaDialectClassifiesTheCurrentTimestampOfGetdate()
        {
            // Act
            var kind = SchemaDefaultValue.Classify(@"getdate()",
                SqlServerSchemaDialect.Instance);

            // Assert
            Assert.AreEqual(ColumnDefaultValue.CurrentTimestamp, kind);
        }

        [TestMethod]
        public void TestSqlServerSchemaDialectClassifiesTheCurrentTimestampInUtc()
        {
            // Act
            var kind = SchemaDefaultValue.Classify(@"getutcdate()",
                SqlServerSchemaDialect.Instance);

            // Assert
            Assert.AreEqual(ColumnDefaultValue.CurrentTimestampUtc, kind);
        }

        [TestMethod]
        public void TestSqlServerSchemaDialectClassifiesTheCurrentTimestampWithOffset()
        {
            // Act
            var kind = SchemaDefaultValue.Classify(@"sysdatetimeoffset()",
                SqlServerSchemaDialect.Instance);

            // Assert
            Assert.AreEqual(ColumnDefaultValue.CurrentTimestampWithOffset, kind);
        }

        [TestMethod]
        public void TestSqlServerSchemaDialectClassifiesTheCurrentDateOfAConvert()
        {
            // Act
            var kind = SchemaDefaultValue.Classify(@"(CONVERT([date],getdate()))",
                SqlServerSchemaDialect.Instance);

            // Assert
            Assert.AreEqual(ColumnDefaultValue.CurrentDate, kind);
        }

        [TestMethod]
        public void TestSqlServerSchemaDialectClassifiesTheNewGuid()
        {
            // Act
            var kind = SchemaDefaultValue.Classify(@"newid()",
                SqlServerSchemaDialect.Instance);

            // Assert
            Assert.AreEqual(ColumnDefaultValue.NewGuid, kind);
        }

        [TestMethod]
        public void TestSqlServerSchemaDialectClassifiesTheNewSequentialGuid()
        {
            // Act
            var kind = SchemaDefaultValue.Classify(@"newsequentialid()",
                SqlServerSchemaDialect.Instance);

            // Assert
            Assert.AreEqual(ColumnDefaultValue.NewSequentialGuid, kind);
        }

        [TestMethod]
        public void TestSqlServerSchemaDialectClassifiesTheRandomNumber()
        {
            // Act
            var kind = SchemaDefaultValue.Classify(@"rand()",
                SqlServerSchemaDialect.Instance);

            // Assert
            Assert.AreEqual(ColumnDefaultValue.RandomNumber, kind);
        }

        [TestMethod]
        public void TestSqlServerSchemaDialectClassifiesTheCurrentUser()
        {
            // Act
            var kind = SchemaDefaultValue.Classify(@"suser_sname()",
                SqlServerSchemaDialect.Instance);

            // Assert
            Assert.AreEqual(ColumnDefaultValue.CurrentUser, kind);
        }

        [TestMethod]
        public void TestSqlServerSchemaDialectClassifiesTheCurrentDatabase()
        {
            // Act
            var kind = SchemaDefaultValue.Classify(@"db_name()",
                SqlServerSchemaDialect.Instance);

            // Assert
            Assert.AreEqual(ColumnDefaultValue.CurrentDatabase, kind);
        }

        [TestMethod]
        public void TestSqlServerSchemaDialectClassifiesTheNextValueOfASequence()
        {
            // Act
            var kind = SchemaDefaultValue.Classify(@"NEXT VALUE FOR [dbo].[OrderSeq]",
                SqlServerSchemaDialect.Instance);

            // Assert
            Assert.AreEqual(ColumnDefaultValue.SequenceNextValue, kind);
        }

        [TestMethod]
        public void TestSqlServerSchemaDialectClassifiesAWrappedNumber()
        {
            // Act
            var kind = SchemaDefaultValue.Classify(@"((0))",
                SqlServerSchemaDialect.Instance);

            // Assert
            Assert.AreEqual(ColumnDefaultValue.Literal, kind);
        }

        [TestMethod]
        public void TestSqlServerSchemaDialectClassifiesAUnicodeText()
        {
            // Act
            var kind = SchemaDefaultValue.Classify(@"N'text'",
                SqlServerSchemaDialect.Instance);

            // Assert
            Assert.AreEqual(ColumnDefaultValue.Literal, kind);
        }

        [TestMethod]
        public void TestSqlServerSchemaDialectNormalizeLiteralRewritesAHexadecimalNumber()
        {
            // Act
            var actual = SqlServerSchemaDialect.Instance.NormalizeLiteral(@"0xDEAD");

            // Assert
            Assert.AreEqual(@"X'DEAD'", actual);
        }

        [TestMethod]
        public void TestSqlServerSchemaDialectIsDateOnlyOfADate()
        {
            // Act
            var actual = SqlServerSchemaDialect.Instance.IsDateOnly("date");

            // Assert
            Assert.IsTrue(actual);
        }

        #endregion
    }
}
