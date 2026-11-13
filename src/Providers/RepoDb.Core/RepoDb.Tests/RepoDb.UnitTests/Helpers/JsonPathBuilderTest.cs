#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Helpers;
using System;

namespace RepoDb.UnitTests.Helpers
{
    [TestClass]
    public class JsonPathBuilderTest
    {
        #region IsIndex

        [TestMethod]
        public void TestJsonPathBuilderIsIndexWithIndex()
        {
            // Act
            var result = JsonPathBuilder.IsIndex("[0]");

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void TestJsonPathBuilderIsIndexWithName()
        {
            // Act
            var result = JsonPathBuilder.IsIndex("Tags");

            // Assert
            Assert.IsFalse(result);
        }

        #endregion

        #region Split

        [TestMethod]
        public void TestJsonPathBuilderSplitWithProperty()
        {
            // Act
            var segments = JsonPathBuilder.Split("Id");

            // Assert
            Assert.AreEqual("Id", string.Join("|", segments), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestJsonPathBuilderSplitWithNestedProperties()
        {
            // Act
            var segments = JsonPathBuilder.Split("Address.City");

            // Assert
            Assert.AreEqual("Address|City", string.Join("|", segments), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestJsonPathBuilderSplitWithArrayIndex()
        {
            // Act
            var segments = JsonPathBuilder.Split("Tags[0]");

            // Assert
            Assert.AreEqual("Tags|[0]", string.Join("|", segments), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestJsonPathBuilderSplitWithLeadingArrayIndex()
        {
            // Act
            var segments = JsonPathBuilder.Split("[1].Name");

            // Assert
            Assert.AreEqual("[1]|Name", string.Join("|", segments), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestJsonPathBuilderSplitWithNull()
        {
            // Act
            var segments = JsonPathBuilder.Split(null);

            // Assert
            Assert.AreEqual(0, segments.Count);
        }

        #endregion

        #region Join

        [TestMethod]
        public void TestJsonPathBuilderJoinWithNestedProperties()
        {
            // Act
            var path = JsonPathBuilder.Join(new[] { "Address", "City" });

            // Assert
            Assert.AreEqual("Address.City", path, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestJsonPathBuilderJoinWithArrayIndex()
        {
            // Act
            var path = JsonPathBuilder.Join(new[] { "Tags", "[0]" });

            // Assert
            Assert.AreEqual("Tags[0]", path, StringComparer.Ordinal);
        }

        #endregion

        #region ToJsonPath

        [TestMethod]
        public void TestJsonPathBuilderToJsonPathWithNestedProperties()
        {
            // Act
            var path = JsonPathBuilder.ToJsonPath(new[] { "Address", "City" });

            // Assert
            Assert.AreEqual("$.Address.City", path, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestJsonPathBuilderToJsonPathWithArrayIndex()
        {
            // Act
            var path = JsonPathBuilder.ToJsonPath(new[] { "Tags", "[0]" });

            // Assert
            Assert.AreEqual("$.Tags[0]", path, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestJsonPathBuilderToJsonPathWithSpecialName()
        {
            // Act
            var path = JsonPathBuilder.ToJsonPath(new[] { "display-name" });

            // Assert
            Assert.AreEqual("$.\"display-name\"", path, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestJsonPathBuilderToJsonPathWithoutSegments()
        {
            // Act
            var path = JsonPathBuilder.ToJsonPath(Array.Empty<string>());

            // Assert
            Assert.AreEqual("$", path, StringComparer.Ordinal);
        }

        #endregion

        #region ToSqlLiteral

        [TestMethod]
        public void TestJsonPathBuilderToSqlLiteral()
        {
            // Act
            var literal = JsonPathBuilder.ToSqlLiteral("$.Id");

            // Assert
            Assert.AreEqual("'$.Id'", literal, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestJsonPathBuilderToSqlLiteralWithSingleQuote()
        {
            // Act
            var literal = JsonPathBuilder.ToSqlLiteral("it's");

            // Assert
            Assert.AreEqual("'it''s'", literal, StringComparer.Ordinal);
        }

        #endregion
    }
}
