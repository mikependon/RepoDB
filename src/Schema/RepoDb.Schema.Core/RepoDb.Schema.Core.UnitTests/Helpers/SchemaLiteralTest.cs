#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RepoDb.Schema.Core.UnitTests.Helpers
{
    [TestClass]
    public class SchemaLiteralTest
    {
        #region Public Methods

        [TestMethod]
        public void TestConvertHexNumberRewritesTheNumber()
        {
            // Act
            var actual = SchemaLiteral.ConvertHexNumber("0xDEAD");

            // Assert
            Assert.AreEqual("X'DEAD'", actual);
        }

        [TestMethod]
        public void TestConvertHexNumberRewritesAnEmptyNumber()
        {
            // Act
            var actual = SchemaLiteral.ConvertHexNumber("0x");

            // Assert
            Assert.AreEqual("X''", actual);
        }

        [TestMethod]
        public void TestConvertHexNumberKeepsAnythingElse()
        {
            // Act
            var actual = SchemaLiteral.ConvertHexNumber("'0xDEAD'");

            // Assert
            Assert.AreEqual("'0xDEAD'", actual);
        }

        [TestMethod]
        public void TestConvertQuotedHexRewritesTheText()
        {
            // Act
            var actual = SchemaLiteral.ConvertQuotedHex("'\\xDEAD'",
                "\\x");

            // Assert
            Assert.AreEqual("X'DEAD'", actual);
        }

        [TestMethod]
        public void TestConvertQuotedHexKeepsATextWithoutTheMarker()
        {
            // Act
            var actual = SchemaLiteral.ConvertQuotedHex("'DEAD'",
                "\\x");

            // Assert
            Assert.AreEqual("'DEAD'", actual);
        }

        [TestMethod]
        public void TestStripTrailingCastRemovesTheCast()
        {
            // Act
            var actual = SchemaLiteral.StripTrailingCast("'x'::text",
                "::");

            // Assert
            Assert.AreEqual("'x'", actual);
        }

        [TestMethod]
        public void TestStripTrailingCastRemovesTheCastWithASize()
        {
            // Act
            var actual = SchemaLiteral.StripTrailingCast("'10'::numeric(10,2)",
                "::");

            // Assert
            Assert.AreEqual("'10'", actual);
        }

        [TestMethod]
        public void TestStripTrailingCastRemovesManyCasts()
        {
            // Act
            var actual = SchemaLiteral.StripTrailingCast("'x'::text::character varying",
                "::");

            // Assert
            Assert.AreEqual("'x'", actual);
        }

        [TestMethod]
        public void TestStripTrailingCastKeepsTheCastInsideOfTheParentheses()
        {
            // Act
            var actual = SchemaLiteral.StripTrailingCast("lower('x'::text)",
                "::");

            // Assert
            Assert.AreEqual("lower('x'::text)", actual);
        }

        [TestMethod]
        public void TestStripTrailingCastKeepsTheCastInsideOfATextBetweenQuotes()
        {
            // Act
            var actual = SchemaLiteral.StripTrailingCast("'a::b'",
                "::");

            // Assert
            Assert.AreEqual("'a::b'", actual);
        }

        [TestMethod]
        public void TestStripTrailingCastKeepsAnExpressionWithoutACast()
        {
            // Act
            var actual = SchemaLiteral.StripTrailingCast("1 + 2",
                "::");

            // Assert
            Assert.AreEqual("1 + 2", actual);
        }

        #endregion
    }
}
