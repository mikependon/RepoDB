#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema.Enumerations;

namespace RepoDb.Schema.Oracle.UnitTests.Dialects
{
    [TestClass]
    public class OracleSchemaDialectTest
    {
        #region Public Methods

        [TestMethod]
        public void TestOracleSchemaDialectFamily()
        {
            // Act
            var family = OracleSchemaDialect.Instance.Family;

            // Assert
            Assert.AreEqual(@"Oracle", family);
        }

        [TestMethod]
        public void TestOracleSchemaDialectIdentifierQuotesAreTheStandardOnes()
        {
            // Act
            var quotes = OracleSchemaDialect.Instance.IdentifierQuotes;

            // Assert
            Assert.AreEqual(1, quotes.Count);
            Assert.AreEqual('"', quotes[0].Open);
        }

        [TestMethod]
        public void TestOracleSchemaDialectNonDeterministicFunctionsHaveSysGuid()
        {
            // Act
            var functions = OracleSchemaDialect.Instance.NonDeterministicFunctions;

            // Assert
            CollectionAssert.Contains(functions.ToList(), "sys_guid");
        }

        [TestMethod]
        public void TestOracleSchemaDialectHasNoNonPortableOperators()
        {
            // Act
            var operators = OracleSchemaDialect.Instance.NonPortableOperators;

            // Assert
            Assert.AreEqual(0, operators.Count);
        }

        [TestMethod]
        public void TestOracleSchemaDialectClassifiesTheCurrentTimestampOfSysdate()
        {
            // Act
            var kind = SchemaDefaultValue.Classify(@"SYSDATE",
                OracleSchemaDialect.Instance);

            // Assert
            Assert.AreEqual(ColumnDefaultValue.CurrentTimestamp, kind);
        }

        [TestMethod]
        public void TestOracleSchemaDialectClassifiesTheCurrentTimestampWithOffset()
        {
            // Act
            var kind = SchemaDefaultValue.Classify(@"SYSTIMESTAMP",
                OracleSchemaDialect.Instance);

            // Assert
            Assert.AreEqual(ColumnDefaultValue.CurrentTimestampWithOffset, kind);
        }

        [TestMethod]
        public void TestOracleSchemaDialectClassifiesTheCurrentTimestampInUtc()
        {
            // Act
            var kind = SchemaDefaultValue.Classify(@"SYS_EXTRACT_UTC(SYSTIMESTAMP)",
                OracleSchemaDialect.Instance);

            // Assert
            Assert.AreEqual(ColumnDefaultValue.CurrentTimestampUtc, kind);
        }

        [TestMethod]
        public void TestOracleSchemaDialectClassifiesTheCurrentDateOfATruncatedSysdate()
        {
            // Act
            var kind = SchemaDefaultValue.Classify(@"TRUNC(SYSDATE)",
                OracleSchemaDialect.Instance);

            // Assert
            Assert.AreEqual(ColumnDefaultValue.CurrentDate, kind);
        }

        [TestMethod]
        public void TestOracleSchemaDialectClassifiesTheNewGuid()
        {
            // Act
            var kind = SchemaDefaultValue.Classify(@"SYS_GUID()",
                OracleSchemaDialect.Instance);

            // Assert
            Assert.AreEqual(ColumnDefaultValue.NewGuid, kind);
        }

        [TestMethod]
        public void TestOracleSchemaDialectClassifiesTheNextValueOfASequence()
        {
            // Act
            var kind = SchemaDefaultValue.Classify(@"ORDER_SEQ.NEXTVAL",
                OracleSchemaDialect.Instance);

            // Assert
            Assert.AreEqual(ColumnDefaultValue.SequenceNextValue, kind);
        }

        [TestMethod]
        public void TestOracleSchemaDialectClassifiesTheCurrentUser()
        {
            // Act
            var kind = SchemaDefaultValue.Classify(@"SYS_CONTEXT('USERENV', 'SESSION_USER')",
                OracleSchemaDialect.Instance);

            // Assert
            Assert.AreEqual(ColumnDefaultValue.CurrentUser, kind);
        }

        [TestMethod]
        public void TestOracleSchemaDialectIsDateOnlyOfADateIsFalseBecauseItHoldsATime()
        {
            // Act
            var actual = OracleSchemaDialect.Instance.IsDateOnly("DATE");

            // Assert
            Assert.IsFalse(actual);
        }

        #endregion
    }
}
