#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.UnitTests.CustomObjects;
using System;
using System.Text.Json.Nodes;

namespace RepoDb.UnitTests.Extensions
{
    [TestClass]
    public class JsonQueryExtensionsTest
    {
        #region JsonNode

        [TestMethod]
        public void TestJsonQueryExtensionsExtractValueFromNodeWithNestedPath()
        {
            // Setup
            var node = JsonNode.Parse("{\"Address\":{\"City\":\"Copenhagen\"}}");

            // Act
            var city = node.ExtractValue<string>("Address.City");

            // Assert
            Assert.AreEqual("Copenhagen", city, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestJsonQueryExtensionsExtractValueFromNodeWithArrayIndex()
        {
            // Setup
            var node = JsonNode.Parse("{\"Tags\":[\"a\",\"b\"]}");

            // Act
            var tag = node.ExtractValue<string>("Tags[1]");

            // Assert
            Assert.AreEqual("b", tag, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestJsonQueryExtensionsExtractValueFromNodeWithMissingPath()
        {
            // Setup
            var node = JsonNode.Parse("{\"Id\":5}");

            // Act
            var city = node.ExtractValue<string>("Address.City");

            // Assert
            Assert.IsNull(city);
        }

        [TestMethod]
        public void TestJsonQueryExtensionsExtractValueFromNodeWithIndexOutOfRange()
        {
            // Setup
            var node = JsonNode.Parse("{\"Tags\":[\"a\"]}");

            // Act
            var tag = node.ExtractValue<string>("Tags[3]");

            // Assert
            Assert.IsNull(tag);
        }

        [TestMethod]
        public void TestJsonQueryExtensionsExtractValueFromNullNode()
        {
            // Act
            var id = ((JsonNode)null).ExtractValue<int>("Id");

            // Assert
            Assert.AreEqual(0, id);
        }

        #endregion

        #region DbJsonValue

        [TestMethod]
        public void TestJsonQueryExtensionsExtractValueFromValueWithPath()
        {
            // Setup
            var value = DbJsonValue<JsonSampleDocument>.Parse("{\"Id\":5}");

            // Act
            var id = value.ExtractValue<JsonSampleDocument, int>("Id");

            // Assert
            Assert.AreEqual(5, id);
        }

        [TestMethod]
        public void TestJsonQueryExtensionsExtractValueFromValueWithExpression()
        {
            // Setup
            var value = DbJsonValue<JsonSampleDocument>.Parse("{\"Id\":5}");

            // Act
            var id = value.ExtractValue(d => d.Id);

            // Assert
            Assert.AreEqual(5, id);
        }

        [TestMethod]
        public void TestJsonQueryExtensionsExtractValueFromValueWithNestedExpression()
        {
            // Setup
            var value = DbJsonValue<JsonSampleDocument>.Parse("{\"Address\":{\"City\":\"Gentofte\"}}");

            // Act
            var city = value.ExtractValue(d => d.Address.City);

            // Assert
            Assert.AreEqual("Gentofte", city, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestJsonQueryExtensionsExtractValueFromEmptyValue()
        {
            // Setup
            var value = default(DbJsonValue<JsonSampleDocument>);

            // Act
            var id = value.ExtractValue(d => d.Id);

            // Assert
            Assert.AreEqual(0, id);
        }

        #endregion
    }
}
