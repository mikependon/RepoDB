#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema.Core.UnitTests.CustomObjects;
using RepoDb.Schema.Enumerations;

namespace RepoDb.Schema.Core.UnitTests
{
    [TestClass]
    public class SchemaDefaultValueTest
    {
        #region Helpers

        private static void AssertAll(ColumnDefaultValue expected,
            params string[] expressions)
        {
            foreach (var expression in expressions)
            {
                Assert.AreEqual(expected, SchemaDefaultValue.Classify(expression,
                    CustomSchemaDialect.Instance), $"'{expression}'");
            }
        }

        #endregion

        #region Classify

        [TestMethod]
        public void TestSchemaDefaultValueClassifyReturnsNoneIfThereIsNoExpression()
        {
            // Act/Assert
            AssertAll(ColumnDefaultValue.None, null, string.Empty, " ", "()", "(())");
        }

        [TestMethod]
        public void TestSchemaDefaultValueClassifyFindsTheLiteralsWithoutTheParenthesesThatWrapThem()
        {
            // Act/Assert
            AssertAll(ColumnDefaultValue.Literal, "((0))", "(0)", "0", "1", "-1", "((3.14))", "(('N/A'))", "('N/A')", "'N/A'", "N'text'", "('it''s')", "''", "(1E5)");
        }

        [TestMethod]
        public void TestSchemaDefaultValueClassifyFindsTheLiteralsOfOtherDatabases()
        {
            // Act/Assert
            AssertAll(ColumnDefaultValue.Literal, "'abc'::character varying", "'abc'::text", "'10'::numeric(10,2)");
        }

        [TestMethod]
        public void TestSchemaDefaultValueClassifyFindsNullAndTheBooleans()
        {
            // Act/Assert
            AssertAll(ColumnDefaultValue.Null, "NULL", "(NULL)", "null");
            AssertAll(ColumnDefaultValue.BooleanTrue, "TRUE", "true", "(true)");
            AssertAll(ColumnDefaultValue.BooleanFalse, "FALSE", "false", "(false)");
        }

        [TestMethod]
        public void TestSchemaDefaultValueClassifyFindsTheCurrentTimestamp()
        {
            // Act/Assert
            AssertAll(ColumnDefaultValue.CurrentTimestamp, "(getdate())", "getdate()", "GETDATE()", "now()", "NOW()", "CURRENT_TIMESTAMP", "current_timestamp()", "CURRENT_TIMESTAMP(6)", "localtimestamp", "now()::timestamp without time zone");
        }

        [TestMethod]
        public void TestSchemaDefaultValueClassifyFindsTheCurrentTimestampInUtc()
        {
            // Act/Assert
            AssertAll(ColumnDefaultValue.CurrentTimestampUtc, "(getutcdate())", "GETUTCDATE()");
        }

        [TestMethod]
        public void TestSchemaDefaultValueClassifyFindsTheCurrentDate()
        {
            // Act/Assert
            AssertAll(ColumnDefaultValue.CurrentDate, "CURRENT_DATE", "current_date::date");
        }

        [TestMethod]
        public void TestSchemaDefaultValueClassifyFindsTheCurrentTime()
        {
            // Act/Assert
            AssertAll(ColumnDefaultValue.CurrentTime, "CURRENT_TIME", "current_time(3)", "localtime", "LOCALTIME(6)");
        }

        [TestMethod]
        public void TestSchemaDefaultValueClassifyFindsTheNewGuids()
        {
            // Act/Assert
            AssertAll(ColumnDefaultValue.NewGuid, "(newid())", "NEWID()", "uuid()", "UUID()", "uuid()::text");
        }

        [TestMethod]
        public void TestSchemaDefaultValueClassifyFindsTheRandomNumbers()
        {
            // Act/Assert
            AssertAll(ColumnDefaultValue.RandomNumber, "(rand())", "RAND()", "random()", "rand(5)");
        }

        [TestMethod]
        public void TestSchemaDefaultValueClassifyFindsTheCurrentUserAndDatabase()
        {
            // Act/Assert
            AssertAll(ColumnDefaultValue.CurrentUser, "SESSION_USER", "current_user", "USER");
            AssertAll(ColumnDefaultValue.CurrentDatabase, "current_catalog");
        }

        [TestMethod]
        public void TestSchemaDefaultValueClassifyFindsTheNextValueOfASequence()
        {
            // Act/Assert
            AssertAll(ColumnDefaultValue.SequenceNextValue, "(NEXT VALUE FOR [dbo].[OrderSeq])", "NEXT VALUE FOR OrderSeq");
        }

        [TestMethod]
        public void TestSchemaDefaultValueClassifyFindsTheExpressionsThatHaveNoFunction()
        {
            // Act/Assert
            AssertAll(ColumnDefaultValue.Expression, "1 + 2", "(5 * 3)", "-(1)", "'a' || 'b'");
        }

        [TestMethod]
        public void TestSchemaDefaultValueClassifyFindsTheDefaultsThatAreNotKnown()
        {
            // Act/Assert
            AssertAll(ColumnDefaultValue.Unsupported, "LAST_INSERT_ID()", "(lower('X'))", "([dbo].[fnDefaultStatus]())", "SomeColumn", "(Qty * Price)", "now() + interval '1 day'");
        }

        [TestMethod]
        public void TestSchemaDefaultValueClassifyIgnoresTheWordsInsideTheTexts()
        {
            // Act/Assert
            AssertAll(ColumnDefaultValue.Literal, "'getdate() is not called'", "('now()')");
        }

        [TestMethod]
        public void TestSchemaDefaultValueClassifyIgnoresTheBracketsOfTheNames()
        {
            // Act/Assert
            AssertAll(ColumnDefaultValue.CurrentTimestamp, "([getdate]())");
        }

        #endregion

        #region GetLiteral

        [TestMethod]
        public void TestSchemaDefaultValueGetLiteralRemovesTheParenthesesThatWrapThem()
        {
            // Act/Assert
            Assert.AreEqual("0", SchemaDefaultValue.GetLiteral("((0))"));
            Assert.AreEqual("'N/A'", SchemaDefaultValue.GetLiteral("(('N/A'))"));
            Assert.AreEqual("3.14", SchemaDefaultValue.GetLiteral("(3.14)"));
        }

        [TestMethod]
        public void TestSchemaDefaultValueGetLiteralRemovesThePrefixOfTheText()
        {
            // Act/Assert
            Assert.AreEqual("'text'", SchemaDefaultValue.GetLiteral("N'text'"));
        }

        [TestMethod]
        public void TestSchemaDefaultValueGetLiteralKeepsTheDoubledQuotes()
        {
            // Act/Assert
            Assert.AreEqual("'it''s'", SchemaDefaultValue.GetLiteral("('it''s')"));
        }

        [TestMethod]
        public void TestSchemaDefaultValueGetLiteralReturnsAnEmptyTextIfThereIsNoExpression()
        {
            // Act/Assert
            Assert.AreEqual(string.Empty, SchemaDefaultValue.GetLiteral(null));
            Assert.AreEqual(string.Empty, SchemaDefaultValue.GetLiteral(" "));
        }

        #endregion

        #region Unwrap

        [TestMethod]
        public void TestSchemaDefaultValueUnwrapRemovesTheParenthesesThatWrapTheWholeExpression()
        {
            // Act/Assert
            Assert.AreEqual("0", SchemaDefaultValue.Unwrap("((0))"));
            Assert.AreEqual("getdate()", SchemaDefaultValue.Unwrap(" (getdate()) "));
        }

        [TestMethod]
        public void TestSchemaDefaultValueUnwrapKeepsTheParenthesesThatDoNotWrapTheWholeExpression()
        {
            // Act/Assert
            Assert.AreEqual("(a) + (b)", SchemaDefaultValue.Unwrap("(a) + (b)"));
            Assert.AreEqual("f(1)", SchemaDefaultValue.Unwrap("f(1)"));
        }

        [TestMethod]
        public void TestSchemaDefaultValueUnwrapDoesNotCountTheParenthesesInsideTheTexts()
        {
            // Act/Assert
            Assert.AreEqual("'(' + ')'", SchemaDefaultValue.Unwrap("('(' + ')')"));
            Assert.AreEqual("')('", SchemaDefaultValue.Unwrap("(')(')"));
        }

        [TestMethod]
        public void TestSchemaDefaultValueUnwrapReturnsAnEmptyTextIfThereIsNoExpression()
        {
            // Act/Assert
            Assert.AreEqual(string.Empty, SchemaDefaultValue.Unwrap(null));
            Assert.AreEqual(string.Empty, SchemaDefaultValue.Unwrap("  "));
        }

        #endregion
    }
}
