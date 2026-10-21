#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RepoDb.Schema.Core.UnitTests
{
    [TestClass]
    public class SchemaExpressionTest
    {
        #region Helpers

        private static readonly string[] Functions = { "now", "getdate", "rand" };

        private static readonly string[] Keywords = { "current_timestamp", "current_date" };

        #endregion

        #region FindUsage

        [TestMethod]
        public void TestSchemaExpressionFindUsageFindsAFunctionThatIsCalled()
        {
            // Act/Assert
            Assert.AreEqual("now", SchemaExpression.FindUsage("BirthDate < now()", Functions, Keywords));
            Assert.AreEqual("getdate", SchemaExpression.FindUsage("([BirthDate]<getdate())", Functions, Keywords));
            Assert.AreEqual("rand", SchemaExpression.FindUsage("Score < RAND ()", Functions, Keywords));
        }

        [TestMethod]
        public void TestSchemaExpressionFindUsageFindsAKeywordWithoutParentheses()
        {
            // Act/Assert
            Assert.AreEqual("current_timestamp", SchemaExpression.FindUsage("BirthDate < CURRENT_TIMESTAMP", Functions, Keywords));
            Assert.AreEqual("current_timestamp", SchemaExpression.FindUsage("BirthDate < current_timestamp(6)", Functions, Keywords));
            Assert.AreEqual("current_date", SchemaExpression.FindUsage("Day = Current_Date", Functions, Keywords));
        }

        [TestMethod]
        public void TestSchemaExpressionFindUsageDoesNotMatchAFunctionThatIsNotCalled()
        {
            // Act/Assert
            Assert.IsNull(SchemaExpression.FindUsage("now > 0", Functions, Keywords));
            Assert.IsNull(SchemaExpression.FindUsage("Rand >= 1", Functions, Keywords));
        }

        [TestMethod]
        public void TestSchemaExpressionFindUsageDoesNotMatchAQuotedOrQualifiedName()
        {
            // Act/Assert
            Assert.IsNull(SchemaExpression.FindUsage("[now] > 0", Functions, Keywords));
            Assert.IsNull(SchemaExpression.FindUsage("`current_date` > 0", Functions, Keywords));
            Assert.IsNull(SchemaExpression.FindUsage("\"current_timestamp\" > 0", Functions, Keywords));
            Assert.IsNull(SchemaExpression.FindUsage("t.current_date > 0", Functions, Keywords));
        }

        [TestMethod]
        public void TestSchemaExpressionFindUsageMatchesWholeWordsOnly()
        {
            // Act/Assert
            Assert.IsNull(SchemaExpression.FindUsage("snow() > 0 AND random() > 0", Functions, Keywords));
            Assert.IsNull(SchemaExpression.FindUsage("current_date_value > 0", Functions, Keywords));
        }

        [TestMethod]
        public void TestSchemaExpressionFindUsageReturnsNullIfTheExpressionIsNullOrWhiteSpace()
        {
            // Act/Assert
            Assert.IsNull(SchemaExpression.FindUsage(null, Functions, Keywords));
            Assert.IsNull(SchemaExpression.FindUsage(" ", Functions, Keywords));
        }

        [TestMethod]
        public void TestSchemaExpressionFindUsageAcceptsNullLists()
        {
            // Act/Assert
            Assert.IsNull(SchemaExpression.FindUsage("now()", null, null));
            Assert.AreEqual("now", SchemaExpression.FindUsage("now()", Functions, null));
            Assert.AreEqual("current_date", SchemaExpression.FindUsage("current_date", null, Keywords));
        }

        #endregion
    }
}
