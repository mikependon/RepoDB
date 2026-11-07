#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RepoDb.Schema.Core.UnitTests.CustomObjects;
using RepoDb.Interfaces;
using RepoDb.Schema.Models;

namespace RepoDb.Schema.Core.UnitTests.Helpers
{
    [TestClass]
    public class SchemaExpressionTranslatorTest
    {
        #region Private Methods

        private static IDbSetting CreateSetting(string opening,
            string closing)
        {
            var setting = new Mock<IDbSetting>();
            setting.SetupGet(item => item.OpeningQuote).Returns(opening);
            setting.SetupGet(item => item.ClosingQuote).Returns(closing);
            return setting.Object;
        }

        private static IList<SchemaExpressionToken> Tokenize(string expression) =>
            SchemaExpressionTokenizer.Tokenize(expression,
                CustomSchemaDialect.Instance.IdentifierQuotes);

        #endregion

        #region Translate

        [TestMethod]
        public void TestTranslateQuotesTheNamesWithTheDoubleQuotes()
        {
            // Act
            var expression = SchemaExpressionTranslator.Translate(Tokenize("[BirthDate]"),
                CreateSetting("\"", "\""));

            // Assert
            Assert.AreEqual("\"BirthDate\"", expression);
        }

        [TestMethod]
        public void TestTranslateQuotesTheNamesWithTheBackticks()
        {
            // Act
            var expression = SchemaExpressionTranslator.Translate(Tokenize("[BirthDate]"),
                CreateSetting("`", "`"));

            // Assert
            Assert.AreEqual("`BirthDate`", expression);
        }

        [TestMethod]
        public void TestTranslateDoublesTheClosingQuoteInTheName()
        {
            // Act
            var expression = SchemaExpressionTranslator.Translate(Tokenize("[A\"B]"),
                CreateSetting("\"", "\""));

            // Assert
            Assert.AreEqual("\"A\"\"B\"", expression);
        }

        [TestMethod]
        public void TestTranslateDoesNotEscapeTheNameIfTheDestinationHasNoClosingQuote()
        {
            // Act
            var expression = SchemaExpressionTranslator.Translate(Tokenize("[A]"),
                CreateSetting("<",
                    null));

            // Assert
            Assert.AreEqual("<A", expression);
        }

        [TestMethod]
        public void TestTranslateKeepsTheLiteralsTheWordsAndTheOperators()
        {
            // Act
            var expression = SchemaExpressionTranslator.Translate(Tokenize("[A] >= (0) AND 'x[y]' <> [B]"),
                CreateSetting("\"", "\""));

            // Assert
            Assert.AreEqual("\"A\" >= (0) AND 'x[y]' <> \"B\"", expression);
        }

        #endregion

        #region FindNonPortable

        [TestMethod]
        public void TestFindNonPortableOfAnExpressionWithoutFunctionsIsNull()
        {
            // Act
            var usage = SchemaExpressionTranslator.FindNonPortable(Tokenize("([UnitPrice]>=(0) AND [Discount]<=(1))"),
                CustomSchemaDialect.Instance.NonPortableOperators);

            // Assert
            Assert.IsNull(usage);
        }

        [TestMethod]
        public void TestFindNonPortableFindsAFunctionThatIsNotPortable()
        {
            // Act
            var usage = SchemaExpressionTranslator.FindNonPortable(Tokenize("([BirthDate]<getdate())"),
                CustomSchemaDialect.Instance.NonPortableOperators);

            // Assert
            Assert.AreEqual("getdate()", usage);
        }

        [TestMethod]
        public void TestFindNonPortableKeepsTheCaseOfTheFunction()
        {
            // Act
            var usage = SchemaExpressionTranslator.FindNonPortable(Tokenize("[A]<GETDATE()"),
                CustomSchemaDialect.Instance.NonPortableOperators);

            // Assert
            Assert.AreEqual("GETDATE()", usage);
        }

        [TestMethod]
        public void TestFindNonPortableFindsAFunctionThatIsCalledAfterWhiteSpaces()
        {
            // Act
            var usage = SchemaExpressionTranslator.FindNonPortable(Tokenize("[A]<getdate ()"),
                CustomSchemaDialect.Instance.NonPortableOperators);

            // Assert
            Assert.AreEqual("getdate()", usage);
        }

        [TestMethod]
        public void TestFindNonPortableFindsTheCastOperator()
        {
            // Act
            var usage = SchemaExpressionTranslator.FindNonPortable(Tokenize("[A]::int > 0"),
                CustomSchemaDialect.Instance.NonPortableOperators);

            // Assert
            Assert.AreEqual("::", usage);
        }

        [TestMethod]
        public void TestFindNonPortableTakesAPortableFunctionAsPortable()
        {
            // Act
            var usage = SchemaExpressionTranslator.FindNonPortable(Tokenize("ABS([A]) > 0"),
                CustomSchemaDialect.Instance.NonPortableOperators);

            // Assert
            Assert.IsNull(usage);
        }

        [TestMethod]
        public void TestFindNonPortableTakesAKeywordBeforeAParenthesisAsPortable()
        {
            // Act
            var usage = SchemaExpressionTranslator.FindNonPortable(Tokenize("[A] > 0 AND ([B] IN (1, 2) OR NOT ([C] = 1))"),
                CustomSchemaDialect.Instance.NonPortableOperators);

            // Assert
            Assert.IsNull(usage);
        }

        [TestMethod]
        public void TestFindNonPortableTakesAWordThatIsNotCalledAsPortable()
        {
            // Act
            var usage = SchemaExpressionTranslator.FindNonPortable(Tokenize("Quantity > 0"),
                CustomSchemaDialect.Instance.NonPortableOperators);

            // Assert
            Assert.IsNull(usage);
        }

        [TestMethod]
        public void TestFindNonPortableTakesAWordFollowedByAnotherWordAsPortable()
        {
            // Act
            var usage = SchemaExpressionTranslator.FindNonPortable(Tokenize("Quantity AND [A] = 1"),
                CustomSchemaDialect.Instance.NonPortableOperators);

            // Assert
            Assert.IsNull(usage);
        }

        [TestMethod]
        public void TestFindNonPortableTakesAWordAtTheEndAsPortable()
        {
            // Act
            var usage = SchemaExpressionTranslator.FindNonPortable(Tokenize("[A] = Quantity"),
                CustomSchemaDialect.Instance.NonPortableOperators);

            // Assert
            Assert.IsNull(usage);
        }

        [TestMethod]
        public void TestFindNonPortableTakesAWordAtTheEndAfterWhiteSpacesAsPortable()
        {
            // Act
            var usage = SchemaExpressionTranslator.FindNonPortable(Tokenize("[A] = Quantity   "),
                CustomSchemaDialect.Instance.NonPortableOperators);

            // Assert
            Assert.IsNull(usage);
        }

        [TestMethod]
        public void TestFindNonPortableDoesNotLookInsideALiteral()
        {
            // Act
            var usage = SchemaExpressionTranslator.FindNonPortable(Tokenize("[A] <> 'getdate()'"),
                CustomSchemaDialect.Instance.NonPortableOperators);

            // Assert
            Assert.IsNull(usage);
        }

        [TestMethod]
        public void TestFindNonPortableDoesNotLookInsideAQuotedName()
        {
            // Act
            var usage = SchemaExpressionTranslator.FindNonPortable(Tokenize("[getdate] > 0"),
                CustomSchemaDialect.Instance.NonPortableOperators);

            // Assert
            Assert.IsNull(usage);
        }

        [TestMethod]
        public void TestFindNonPortableFindsTheFirstOneOfManyWhenTheyAreNotPortable()
        {
            // Act
            var usage = SchemaExpressionTranslator.FindNonPortable(Tokenize("len([A]) > 0 AND [B] < getdate()"),
                CustomSchemaDialect.Instance.NonPortableOperators);

            // Assert
            Assert.AreEqual("len()", usage);
        }

        #endregion
    }
}
