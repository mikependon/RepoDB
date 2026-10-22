#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
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
                Assert.AreEqual(expected, SchemaDefaultValue.Classify(expression), $"'{expression}'");
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
        public void TestSchemaDefaultValueClassifyFindsTheLiteralsWithoutTheParenthesesOfSqlServer()
        {
            // Act/Assert
            AssertAll(ColumnDefaultValue.Literal, "((0))", "(0)", "0", "1", "-1", "((3.14))", "(('N/A'))", "('N/A')", "'N/A'", "N'text'", "('it''s')", "''", "(1E5)");
        }

        [TestMethod]
        public void TestSchemaDefaultValueClassifyFindsTheLiteralsOfOtherDatabases()
        {
            // Act/Assert
            AssertAll(ColumnDefaultValue.Literal, "'abc'::character varying", "'abc'::text", "'10'::numeric(10,2)", "_utf8mb4'abc'");
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
            AssertAll(ColumnDefaultValue.CurrentTimestamp, "(getdate())", "getdate()", "GETDATE()", "(sysdatetime())", "now()", "NOW()", "CURRENT_TIMESTAMP", "current_timestamp()", "CURRENT_TIMESTAMP(6)",
                "localtimestamp", "SYSDATE", "now()::timestamp without time zone", "current timestamp", "CURRENT TIMESTAMP", "transaction_timestamp()", "(datetime('now','localtime'))", "now64(3)", "now64()");
        }

        [TestMethod]
        public void TestSchemaDefaultValueClassifyFindsTheCurrentTimestampInUtc()
        {
            // Act/Assert
            AssertAll(ColumnDefaultValue.CurrentTimestampUtc, "(getutcdate())", "GETUTCDATE()", "sysutcdatetime()", "utc_timestamp()", "UTC_TIMESTAMP(6)", "timezone('utc', now())",
                "(now() AT TIME ZONE 'utc')", "SYS_EXTRACT_UTC(SYSTIMESTAMP)", "(datetime('now'))");
        }

        [TestMethod]
        public void TestSchemaDefaultValueClassifyFindsTheCurrentTimestampWithOffset()
        {
            // Act/Assert
            AssertAll(ColumnDefaultValue.CurrentTimestampWithOffset, "(sysdatetimeoffset())", "SYSDATETIMEOFFSET()", "SYSTIMESTAMP", "systimestamp(6)");
        }

        [TestMethod]
        public void TestSchemaDefaultValueClassifyFindsTheCurrentDate()
        {
            // Act/Assert
            AssertAll(ColumnDefaultValue.CurrentDate, "CURRENT_DATE", "current_date()", "curdate()", "current date", "(CONVERT([date],getdate()))", "(CONVERT(date, getdate(), 101))",
                "CAST(getdate() AS date)", "CAST(now() AS date)", "TRUNC(SYSDATE)", "(date('now'))", "CURRENT_DATE::date", "today()");
        }

        [TestMethod]
        public void TestSchemaDefaultValueClassifyFindsTheCurrentTime()
        {
            // Act/Assert
            AssertAll(ColumnDefaultValue.CurrentTime, "CURRENT_TIME", "current_time(3)", "curtime()", "current time", "localtime", "LOCALTIME(6)");
        }

        [TestMethod]
        public void TestSchemaDefaultValueClassifyFindsTheNewGuids()
        {
            // Act/Assert
            AssertAll(ColumnDefaultValue.NewGuid, "(newid())", "NEWID()", "uuid()", "UUID()", "gen_random_uuid()", "uuid_generate_v4()", "SYS_GUID()", "random_uuid()", "generateUUIDv4()", "gen_random_uuid()::text");
            AssertAll(ColumnDefaultValue.NewSequentialGuid, "(newsequentialid())", "NEWSEQUENTIALID()");
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
            AssertAll(ColumnDefaultValue.CurrentUser, "(suser_sname())", "SUSER_SNAME()", "system_user", "SESSION_USER", "current_user", "CURRENT_USER()", "USER", "user()", "SYS_CONTEXT('USERENV', 'SESSION_USER')");
            AssertAll(ColumnDefaultValue.CurrentDatabase, "(db_name())", "DB_NAME()", "database()", "current_database()", "current_catalog");
        }

        [TestMethod]
        public void TestSchemaDefaultValueClassifyFindsTheNextValueOfASequence()
        {
            // Act/Assert
            AssertAll(ColumnDefaultValue.SequenceNextValue, "nextval('orders_id_seq'::regclass)", "(NEXT VALUE FOR [dbo].[OrderSeq])", "NEXT VALUE FOR OrderSeq", "ORDER_SEQ.NEXTVAL", "\"HR\".\"SEQ\".nextval", "gen_id(gen_orders, 1)");
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
        public void TestSchemaDefaultValueGetLiteralRemovesTheParenthesesOfSqlServer()
        {
            // Act/Assert
            Assert.AreEqual("0", SchemaDefaultValue.GetLiteral("((0))"));
            Assert.AreEqual("'N/A'", SchemaDefaultValue.GetLiteral("(('N/A'))"));
            Assert.AreEqual("3.14", SchemaDefaultValue.GetLiteral("(3.14)"));
        }

        [TestMethod]
        public void TestSchemaDefaultValueGetLiteralRemovesThePrefixAndTheTypeCastOfTheText()
        {
            // Act/Assert
            Assert.AreEqual("'text'", SchemaDefaultValue.GetLiteral("N'text'"));
            Assert.AreEqual("'abc'", SchemaDefaultValue.GetLiteral("'abc'::character varying"));
            Assert.AreEqual("'abc'", SchemaDefaultValue.GetLiteral("_utf8mb4'abc'"));
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
