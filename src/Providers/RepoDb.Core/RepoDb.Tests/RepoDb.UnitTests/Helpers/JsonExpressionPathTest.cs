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
    public class JsonExpressionPathTest
    {
        #region FromLambda

        [TestMethod]
        public void TestJsonExpressionPathFromLambdaWithProperty()
        {
            // Setup
            Expression<Func<JsonSampleDocument, int>> expression = d => d.Id;

            // Act
            var segments = JsonExpressionPath.FromLambda(expression);

            // Assert
            Assert.AreEqual("Id", string.Join("|", segments), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestJsonExpressionPathFromLambdaWithNestedProperties()
        {
            // Setup
            Expression<Func<JsonSampleDocument, string>> expression = d => d.Address.City;

            // Act
            var segments = JsonExpressionPath.FromLambda(expression);

            // Assert
            Assert.AreEqual("Address|City", string.Join("|", segments), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestJsonExpressionPathFromLambdaWithArrayIndex()
        {
            // Setup
            Expression<Func<JsonSampleDocument, string>> expression = d => d.Tags[0];

            // Act
            var segments = JsonExpressionPath.FromLambda(expression);

            // Assert
            Assert.AreEqual("Tags|[0]", string.Join("|", segments), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestJsonExpressionPathFromLambdaWithListIndexer()
        {
            // Setup
            Expression<Func<JsonSampleDocument, string>> expression = d => d.Names[1];

            // Act
            var segments = JsonExpressionPath.FromLambda(expression);

            // Assert
            Assert.AreEqual("Names|[1]", string.Join("|", segments), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestJsonExpressionPathFromLambdaWithCustomPropertyName()
        {
            // Setup
            Expression<Func<JsonSampleDocument, string>> expression = d => d.DisplayName;

            // Act
            var segments = JsonExpressionPath.FromLambda(expression);

            // Assert
            Assert.AreEqual("display-name", string.Join("|", segments), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestJsonExpressionPathFromLambdaWithUnsupportedExpression()
        {
            // Setup
            Expression<Func<JsonSampleDocument, int>> expression = d => d.Id + 1;

            // Act and Assert
            Assert.Throws<NotSupportedException>(() => JsonExpressionPath.FromLambda(expression));
        }

        #endregion

        #region GetIndex

        [TestMethod]
        public void TestJsonExpressionPathGetIndexWithConstant()
        {
            // Act
            var index = JsonExpressionPath.GetIndex(Expression.Constant(2));

            // Assert
            Assert.AreEqual("[2]", index, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestJsonExpressionPathGetIndexWithCapturedValue()
        {
            // Setup
            var captured = 3;
            Expression<Func<int>> expression = () => captured;

            // Act
            var index = JsonExpressionPath.GetIndex(expression.Body);

            // Assert
            Assert.AreEqual("[3]", index, StringComparer.Ordinal);
        }

        #endregion

        #region GetName

        [TestMethod]
        public void TestJsonExpressionPathGetNameWithProperty()
        {
            // Act
            var name = JsonExpressionPath.GetName(typeof(JsonSampleDocument).GetProperty(nameof(JsonSampleDocument.Id)));

            // Assert
            Assert.AreEqual("Id", name, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestJsonExpressionPathGetNameWithCustomPropertyName()
        {
            // Act
            var name = JsonExpressionPath.GetName(typeof(JsonSampleDocument).GetProperty(nameof(JsonSampleDocument.DisplayName)));

            // Assert
            Assert.AreEqual("display-name", name, StringComparer.Ordinal);
        }

        #endregion
    }
}
