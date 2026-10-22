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

        #region FindNonDeterministic

        [TestMethod]
        public void TestSchemaExpressionFindNonDeterministicFindsTheFunctionsOfTheDatabases()
        {
            // Act/Assert
            Assert.AreEqual("now()", SchemaExpression.FindNonDeterministic("BirthDate < NOW()"));
            Assert.AreEqual("getdate()", SchemaExpression.FindNonDeterministic("([BirthDate]<getdate())"));
            Assert.AreEqual("random()", SchemaExpression.FindNonDeterministic("random() > 0.5"));
            Assert.AreEqual("sys_guid()", SchemaExpression.FindNonDeterministic("Id <> SYS_GUID()"));
        }

        [TestMethod]
        public void TestSchemaExpressionFindNonDeterministicFindsTheKeywordsAndTheSpecialRegisters()
        {
            // Act/Assert
            Assert.AreEqual("current_timestamp", SchemaExpression.FindNonDeterministic("BirthDate < CURRENT_TIMESTAMP"));
            Assert.AreEqual("sysdate", SchemaExpression.FindNonDeterministic("HireDate < SYSDATE"));
            Assert.AreEqual("current timestamp", SchemaExpression.FindNonDeterministic("HireDate < CURRENT TIMESTAMP"));
            Assert.AreEqual("systimestamp", SchemaExpression.FindNonDeterministic("HireDate < SYSTIMESTAMP"));
        }

        [TestMethod]
        public void TestSchemaExpressionFindNonDeterministicReturnsNullForADeterministicExpression()
        {
            // Act/Assert
            Assert.IsNull(SchemaExpression.FindNonDeterministic("HireDate >= '1990-01-01' AND length(Name) > 0"));
            Assert.IsNull(SchemaExpression.FindNonDeterministic("[now] > 0 AND [user] <> ''"));
            Assert.IsNull(SchemaExpression.FindNonDeterministic(null));
        }

        [TestMethod]
        public void TestSchemaExpressionGetNonDeterministicReasonNamesTheFunctionAndTheDatabase()
        {
            // Act
            var actual = SchemaExpression.GetNonDeterministicReason(new RepoDb.Schema.Models.CheckConstraintInfo("CK_Birthdate") { Expression = "BirthDate < now()" }, "MySQL");

            // Assert
            Assert.AreEqual("its expression uses now(), which MySQL does not allow in a CHECK constraint", actual);
        }

        [TestMethod]
        public void TestSchemaExpressionGetNonDeterministicReasonReturnsNullIfTheCheckConstraintIsDeterministicOrNull()
        {
            // Act/Assert
            Assert.IsNull(SchemaExpression.GetNonDeterministicReason(new RepoDb.Schema.Models.CheckConstraintInfo("CK_Age") { Expression = "Age >= 0" }, "MySQL"));
            Assert.IsNull(SchemaExpression.GetNonDeterministicReason(null, "MySQL"));
        }

        #endregion
    }
}
