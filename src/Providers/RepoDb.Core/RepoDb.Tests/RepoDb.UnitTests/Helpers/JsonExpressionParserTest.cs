#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Helpers;
using RepoDb.UnitTests.CustomObjects;
using System;
using System.Linq.Expressions;

namespace RepoDb.UnitTests.Helpers
{
    [TestClass]
    public class JsonExpressionParserTest
    {
        #region ExtractValue

        [TestMethod]
        public void TestJsonExpressionParserTryParseWithExtractValueLambda()
        {
            // Setup
            Expression<Func<JsonSampleEntity, int>> expression = x => x.Document.ExtractValue(d => d.Id);

            // Act
            var found = JsonExpressionParser.TryParse(expression.Body, out var access);

            // Assert
            Assert.IsTrue(found);
            Assert.AreEqual("Document", access.Column.Member.Name, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestJsonExpressionParserTryParseWithExtractValueLambdaSegments()
        {
            // Setup
            Expression<Func<JsonSampleEntity, string>> expression = x => x.Document.ExtractValue(d => d.Address.City);

            // Act
            JsonExpressionParser.TryParse(expression.Body, out var access);

            // Assert
            Assert.AreEqual("Address|City", string.Join("|", access.Segments), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestJsonExpressionParserTryParseWithExtractValuePath()
        {
            // Setup
            Expression<Func<JsonSampleEntity, string>> expression = x => x.Document.ExtractValue<JsonSampleDocument, string>("Address.City");

            // Act
            JsonExpressionParser.TryParse(expression.Body, out var access);

            // Assert
            Assert.AreEqual("Address|City", string.Join("|", access.Segments), StringComparer.Ordinal);
        }

        #endregion

        #region Value chain

        [TestMethod]
        public void TestJsonExpressionParserTryParseWithValueChain()
        {
            // Setup
            Expression<Func<JsonSampleEntity, string>> expression = x => x.Document.Value.Address.City;

            // Act
            var found = JsonExpressionParser.TryParse(expression.Body, out var access);

            // Assert
            Assert.IsTrue(found);
            Assert.AreEqual("Address|City", string.Join("|", access.Segments), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestJsonExpressionParserTryParseWithValueChainColumn()
        {
            // Setup
            Expression<Func<JsonSampleEntity, int>> expression = x => x.Document.Value.Id;

            // Act
            JsonExpressionParser.TryParse(expression.Body, out var access);

            // Assert
            Assert.AreEqual("Document", access.Column.Member.Name, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestJsonExpressionParserTryParseWithValueChainArrayIndex()
        {
            // Setup
            Expression<Func<JsonSampleEntity, string>> expression = x => x.Tags.Value[0];

            // Act
            JsonExpressionParser.TryParse(expression.Body, out var access);

            // Assert
            Assert.AreEqual("[0]", string.Join("|", access.Segments), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestJsonExpressionParserTryParseWithConvertedValueChain()
        {
            // Setup
            Expression<Func<JsonSampleEntity, int?>> expression = x => x.Document.Value.Id;

            // Act
            var found = JsonExpressionParser.TryParse(expression.Body, out var access);

            // Assert
            Assert.IsTrue(found);
            Assert.AreEqual("Id", string.Join("|", access.Segments), StringComparer.Ordinal);
        }

        #endregion

        #region Not JSON

        [TestMethod]
        public void TestJsonExpressionParserTryParseWithPlainProperty()
        {
            // Setup
            Expression<Func<JsonSampleEntity, int>> expression = x => x.Id;

            // Act
            var found = JsonExpressionParser.TryParse(expression.Body, out var access);

            // Assert
            Assert.IsFalse(found);
            Assert.IsNull(access);
        }

        [TestMethod]
        public void TestJsonExpressionParserTryParseWithJsonColumnOnly()
        {
            // Setup
            Expression<Func<JsonSampleEntity, DbJsonValue<JsonSampleDocument>>> expression = x => x.Document;

            // Act
            var found = JsonExpressionParser.TryParse(expression.Body, out _);

            // Assert
            Assert.IsFalse(found);
        }

        [TestMethod]
        public void TestJsonExpressionParserTryParseWithCapturedEntity()
        {
            // Setup
            var captured = new JsonSampleEntity();
            Expression<Func<int>> expression = () => captured.Document.Value.Id;

            // Act
            var found = JsonExpressionParser.TryParse(expression.Body, out _);

            // Assert
            Assert.IsFalse(found);
        }

        #endregion
    }
}
