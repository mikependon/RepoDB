#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RepoDb.Schema.Core.UnitTests
{
    /// <summary>
    /// Contains the tests of the <c>SchemaDefinitionExpressionScanner</c> class.
    /// </summary>
    [TestClass]
    public class SchemaDefinitionExpressionScannerTest
    {
        #region Public Methods

        [TestMethod]
        [DataRow("([Age] > 0)", "Age")]
        [DataRow("\"Age\" > 0", "Age")]
        [DataRow("`Age` > 0", "Age")]
        [DataRow("[Age]>0", "Age")]
        public void TestSchemaDefinitionExpressionScannerFindsTheQuotedIdentifiers(string expression,
            string expected)
        {
            // Act
            var actual = SchemaDefinitionExpressionScanner.GetQuotedIdentifiers(expression);

            // Assert
            Assert.AreEqual(1, actual.Count);
            Assert.AreEqual(expected, actual[0]);
        }

        [TestMethod]
        public void TestSchemaDefinitionExpressionScannerFindsAllTheIdentifiersInOrderWithoutRepeats()
        {
            // Act
            var actual = SchemaDefinitionExpressionScanner.GetQuotedIdentifiers("[B] > [A] AND [B] < [C]");

            // Assert
            CollectionAssert.AreEqual(new[] { "B", "A", "C" }, actual.ToArray());
        }

        [TestMethod]
        public void TestSchemaDefinitionExpressionScannerSkipsTheStringLiterals()
        {
            // Act
            var actual = SchemaDefinitionExpressionScanner.GetQuotedIdentifiers("[Name] <> '[NotAColumn]' AND [Code] <> 'it''s [Hidden]'");

            // Assert
            CollectionAssert.AreEqual(new[] { "Name", "Code" }, actual.ToArray());
        }

        [TestMethod]
        public void TestSchemaDefinitionExpressionScannerSkipsTheQualifiedNames()
        {
            // Act
            var actual = SchemaDefinitionExpressionScanner.GetQuotedIdentifiers("[dbo].[Other] > 0 AND [Column] > T.[Qualified] AND [Own] > 0");

            // Assert
            CollectionAssert.AreEqual(new[] { "Column", "Own" }, actual.ToArray());
        }

        [TestMethod]
        public void TestSchemaDefinitionExpressionScannerSkipsTheFunctionCalls()
        {
            // Act
            var actual = SchemaDefinitionExpressionScanner.GetQuotedIdentifiers("[Total] > [dbo].[Limit]([Amount]) AND \"Round\" (1) > 0");

            // Assert
            CollectionAssert.AreEqual(new[] { "Total", "Amount" }, actual.ToArray());
        }

        [TestMethod]
        public void TestSchemaDefinitionExpressionScannerUnescapesTheDoubledClosingQuote()
        {
            // Act
            var actual = SchemaDefinitionExpressionScanner.GetQuotedIdentifiers("[Odd]]Name] > 0 AND \"Say \"\"Hi\"\"\" > 0");

            // Assert
            CollectionAssert.AreEqual(new[] { "Odd]Name", "Say \"Hi\"" }, actual.ToArray());
        }

        [TestMethod]
        public void TestSchemaDefinitionExpressionScannerIgnoresTheUnquotedNames()
        {
            // Act
            var actual = SchemaDefinitionExpressionScanner.GetQuotedIdentifiers("age > 0 AND length(name) > 3");

            // Assert
            Assert.AreEqual(0, actual.Count);
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        public void TestSchemaDefinitionExpressionScannerWithNoExpression(string expression)
        {
            // Act
            var actual = SchemaDefinitionExpressionScanner.GetQuotedIdentifiers(expression);

            // Assert
            Assert.AreEqual(0, actual.Count);
        }

        [TestMethod]
        [DataRow("[Open")]
        [DataRow("'open literal [Hidden]")]
        public void TestSchemaDefinitionExpressionScannerDoesNotFailOnAnUnclosedQuote(string expression)
        {
            // Act
            var actual = SchemaDefinitionExpressionScanner.GetQuotedIdentifiers(expression);

            // Assert
            Assert.IsTrue(actual.Count <= 1);
        }

        #endregion
    }
}
