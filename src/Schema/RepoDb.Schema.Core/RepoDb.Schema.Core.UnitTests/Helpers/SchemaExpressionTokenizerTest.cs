#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema.Enumerations;
using RepoDb.Schema.Models;

namespace RepoDb.Schema.Core.UnitTests.Helpers
{
    [TestClass]
    public class SchemaExpressionTokenizerTest
    {
        #region Private Variables

        private readonly SchemaIdentifierQuote[] _backticks = { new SchemaIdentifierQuote('`') };
        private readonly SchemaIdentifierQuote[] _brackets = { new SchemaIdentifierQuote('[', ']') };
        private readonly SchemaIdentifierQuote[] _doubleQuotes = { new SchemaIdentifierQuote('"') };

        #endregion

        #region Private Methods

        private SchemaExpressionToken TokenizeSingle(string expression,
            SchemaIdentifierQuote[] quotes) =>
            SchemaExpressionTokenizer.Tokenize(expression,
                quotes).Single();

        #endregion

        #region Public Methods

        [TestMethod]
        public void TestTokenizeAnEmptyExpressionGivesNoTokens()
        {
            // Act
            var tokens = SchemaExpressionTokenizer.Tokenize(string.Empty,
                _brackets);

            // Assert
            Assert.AreEqual(0, tokens.Count);
        }

        [TestMethod]
        public void TestTokenizeABracketIdentifierKind()
        {
            // Act
            var token = TokenizeSingle("[BirthDate]",
                _brackets);

            // Assert
            Assert.AreEqual(SchemaExpressionTokenKind.Identifier, token.Kind);
        }

        [TestMethod]
        public void TestTokenizeABracketIdentifierName()
        {
            // Act
            var token = TokenizeSingle("[BirthDate]",
                _brackets);

            // Assert
            Assert.AreEqual("BirthDate", token.Name);
        }

        [TestMethod]
        public void TestTokenizeABracketIdentifierKeepsItsText()
        {
            // Act
            var token = TokenizeSingle("[BirthDate]",
                _brackets);

            // Assert
            Assert.AreEqual("[BirthDate]", token.Text);
        }

        [TestMethod]
        public void TestTokenizeAnEscapedClosingBracketIsPartOfTheName()
        {
            // Act
            var token = TokenizeSingle("[A]]B]",
                _brackets);

            // Assert
            Assert.AreEqual("A]B", token.Name);
        }

        [TestMethod]
        public void TestTokenizeABacktickIdentifier()
        {
            // Act
            var token = TokenizeSingle("`Order Details`",
                _backticks);

            // Assert
            Assert.AreEqual("Order Details", token.Name);
        }

        [TestMethod]
        public void TestTokenizeADoubleQuoteIdentifier()
        {
            // Act
            var token = TokenizeSingle("\"Order\"",
                _doubleQuotes);

            // Assert
            Assert.AreEqual(SchemaExpressionTokenKind.Identifier, token.Kind);
        }

        [TestMethod]
        public void TestTokenizeADoubleQuoteIsNotAnIdentifierIfTheSourceDoesNotAcceptIt()
        {
            // Act
            var tokens = SchemaExpressionTokenizer.Tokenize("\"x\"",
                _backticks);

            // Assert
            Assert.IsTrue(tokens.All(token => token.Kind != SchemaExpressionTokenKind.Identifier));
        }

        [TestMethod]
        public void TestTokenizeUsesAnyOfTheQuotesOfTheSource()
        {
            // Act
            var tokens = SchemaExpressionTokenizer.Tokenize("[a] + \"b\"",
                new[] { new SchemaIdentifierQuote('[', ']'), new SchemaIdentifierQuote('"') });

            // Assert
            Assert.AreEqual(2, tokens.Count(token => token.Kind == SchemaExpressionTokenKind.Identifier));
        }

        [TestMethod]
        public void TestTokenizeALiteralKind()
        {
            // Act
            var token = TokenizeSingle("'text'",
                _brackets);

            // Assert
            Assert.AreEqual(SchemaExpressionTokenKind.Literal, token.Kind);
        }

        [TestMethod]
        public void TestTokenizeADoubledSingleQuoteIsPartOfTheLiteral()
        {
            // Act
            var token = TokenizeSingle("'a''b'",
                _brackets);

            // Assert
            Assert.AreEqual("a'b", token.Name);
        }

        [TestMethod]
        public void TestTokenizeABracketInsideALiteralIsNotAnIdentifier()
        {
            // Act
            var token = TokenizeSingle("'[x]'",
                _brackets);

            // Assert
            Assert.AreEqual(SchemaExpressionTokenKind.Literal, token.Kind);
        }

        [TestMethod]
        public void TestTokenizeALiteralThatIsNotClosedIsOtherText()
        {
            // Act
            var token = TokenizeSingle("'abc",
                _brackets);

            // Assert
            Assert.AreEqual(SchemaExpressionTokenKind.Other, token.Kind);
        }

        [TestMethod]
        public void TestTokenizeALiteralThatIsNotClosedKeepsItsText()
        {
            // Act
            var token = TokenizeSingle("'abc",
                _brackets);

            // Assert
            Assert.AreEqual("'abc", token.Text);
        }

        [TestMethod]
        public void TestTokenizeALiteralThatEndsWithADoubledQuoteIsNotClosed()
        {
            // Act
            var token = TokenizeSingle("'a''",
                _brackets);

            // Assert
            Assert.AreEqual(SchemaExpressionTokenKind.Other, token.Kind);
        }

        [TestMethod]
        public void TestTokenizeAnIdentifierThatIsNotClosedIsOtherText()
        {
            // Act
            var token = TokenizeSingle("[abc",
                _brackets);

            // Assert
            Assert.AreEqual(SchemaExpressionTokenKind.Other, token.Kind);
        }

        [TestMethod]
        public void TestTokenizeAWordKind()
        {
            // Act
            var token = TokenizeSingle("getdate",
                _brackets);

            // Assert
            Assert.AreEqual(SchemaExpressionTokenKind.Word, token.Kind);
        }

        [TestMethod]
        public void TestTokenizeAWordThatStartsWithAnUnderscoreAndHasDigitsAndSymbols()
        {
            // Act
            var token = TokenizeSingle("_a1$#",
                _brackets);

            // Assert
            Assert.AreEqual("_a1$#", token.Text);
        }

        [TestMethod]
        public void TestTokenizeANumberIsOneOtherToken()
        {
            // Act
            var token = TokenizeSingle("10.5e3",
                _brackets);

            // Assert
            Assert.AreEqual(SchemaExpressionTokenKind.Other, token.Kind);
        }

        [TestMethod]
        public void TestTokenizeANumberKeepsItsText()
        {
            // Act
            var token = TokenizeSingle("10.5e3",
                _brackets);

            // Assert
            Assert.AreEqual("10.5e3", token.Text);
        }

        [TestMethod]
        public void TestTokenizeTheWhiteSpacesAreOneToken()
        {
            // Act
            var token = TokenizeSingle(" \t ",
                _brackets);

            // Assert
            Assert.AreEqual(" \t ", token.Text);
        }

        [TestMethod]
        public void TestTokenizeTheCharactersOfAnOperatorAreOneTokenEach()
        {
            // Act
            var tokens = SchemaExpressionTokenizer.Tokenize("::",
                _brackets);

            // Assert
            Assert.AreEqual(2, tokens.Count);
        }

        [TestMethod]
        public void TestTokenizeAColonAtTheEndIsAloneToken()
        {
            // Act
            var token = TokenizeSingle(":",
                _brackets);

            // Assert
            Assert.AreEqual(":", token.Text);
        }

        [TestMethod]
        public void TestTokenizeAColonBeforeAWordIsAloneToken()
        {
            // Act
            var tokens = SchemaExpressionTokenizer.Tokenize(":a",
                _brackets);

            // Assert
            Assert.AreEqual(":", tokens.First().Text);
        }

        [TestMethod]
        public void TestTokenizeEachOperatorCharacterIsAToken()
        {
            // Act
            var tokens = SchemaExpressionTokenizer.Tokenize("<=",
                _brackets);

            // Assert
            Assert.AreEqual(2, tokens.Count);
        }

        [TestMethod]
        public void TestTokenizeGivesTheSameExpressionWhenTheTextsAreJoined()
        {
            // Setup
            var expression = "(([BirthDate]<getdate()) AND [Name] <> 'a''[b]' AND 10.5 >= (1)::int)";

            // Act
            var tokens = SchemaExpressionTokenizer.Tokenize(expression,
                _brackets);

            // Assert
            Assert.AreEqual(expression, string.Concat(tokens.Select(token => token.Text)));
        }

        #endregion
    }
}
