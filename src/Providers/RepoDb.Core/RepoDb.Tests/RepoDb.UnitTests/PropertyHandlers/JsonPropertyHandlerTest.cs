#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System.Text.Json.Nodes;
using System.Text.Json.Serialization.Metadata;
using System.Text.Json;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.PropertyHandlers;

namespace RepoDb.UnitTests.PropertyHandlers
{
    [TestClass]
    public class JsonPropertyHandlerTest
    {
        [TestMethod]
        public void TestJsonNodeNull()
        {
            // Setup
            var handler = new JsonToJsonNodePropertyHandler();

            // Act
            var actual = handler.Get(null, null);

            // Assert
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void TestJsonNodeEmpty()
        {
            // Setup
            var handler = new JsonToJsonNodePropertyHandler();

            // Act
            var actual = handler.Get(string.Empty, null);

            // Assert
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void TestJsonNodeReadsValue()
        {
            // Setup
            var handler = new JsonToJsonNodePropertyHandler();

            // Act
            var actual = handler.Get("{\"Value\":42}", null);

            // Assert
            Assert.AreEqual(42, actual["Value"].GetValue<int>());
        }

        [TestMethod]
        public void TestJsonNodeWritesValue()
        {
            // Setup
            var handler = new JsonToJsonNodePropertyHandler();
            var input = JsonNode.Parse("{\"Value\":42}");

            // Act
            var actual = handler.Set(input, null);

            // Assert
            Assert.AreEqual("{\"Value\":42}", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestJsonNodeWritesAbsentValue()
        {
            // Setup
            var handler = new JsonToJsonNodePropertyHandler();

            // Act
            var actual = handler.Set(null, null);

            // Assert
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void TestJsonNodeRejectsMalformedJson()
        {
            // Setup
            var handler = new JsonToJsonNodePropertyHandler();

            // Act
            Action act = () => handler.Get("{", null);

            // Assert
            Assert.Throws<JsonException>(act);
        }

        [TestMethod]
        public void TestJsonDocumentNull()
        {
            // Setup
            var handler = new JsonToJsonDocumentPropertyHandler();

            // Act
            var actual = handler.Get(null, null);

            // Assert
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void TestJsonDocumentEmpty()
        {
            // Setup
            var handler = new JsonToJsonDocumentPropertyHandler();

            // Act
            var actual = handler.Get(string.Empty, null);

            // Assert
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void TestJsonDocumentReadsValue()
        {
            // Setup
            var handler = new JsonToJsonDocumentPropertyHandler();

            // Act
            using var actual = handler.Get("{\"Value\":42}", null);

            // Assert
            Assert.AreEqual(42, actual.RootElement.GetProperty("Value").GetInt32());
        }

        [TestMethod]
        public void TestJsonDocumentWritesValue()
        {
            // Setup
            var handler = new JsonToJsonDocumentPropertyHandler();
            using var input = JsonDocument.Parse("{\"Value\":42}");

            // Act
            var actual = handler.Set(input, null);

            // Assert
            Assert.AreEqual("{\"Value\":42}", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestJsonDocumentWritesAbsentValue()
        {
            // Setup
            var handler = new JsonToJsonDocumentPropertyHandler();

            // Act
            var actual = handler.Set(null, null);

            // Assert
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void TestJsonDocumentRejectsMalformedJson()
        {
            // Setup
            var handler = new JsonToJsonDocumentPropertyHandler();

            // Act
            Action act = () => handler.Get("{", null);

            // Assert
            Assert.Throws<JsonException>(act);
        }

        [TestMethod]
        public void TestJsonElementNull()
        {
            // Setup
            var handler = new JsonToJsonElementPropertyHandler();

            // Act
            var actual = handler.Get(null, null);

            // Assert
            Assert.AreEqual(JsonValueKind.Undefined, actual.ValueKind);
        }

        [TestMethod]
        public void TestJsonElementEmpty()
        {
            // Setup
            var handler = new JsonToJsonElementPropertyHandler();

            // Act
            var actual = handler.Get(string.Empty, null);

            // Assert
            Assert.AreEqual(JsonValueKind.Undefined, actual.ValueKind);
        }

        [TestMethod]
        public void TestJsonElementReadsValue()
        {
            // Setup
            var handler = new JsonToJsonElementPropertyHandler();

            // Act
            var actual = handler.Get("{\"Value\":42}", null);

            // Assert
            Assert.AreEqual(42, actual.GetProperty("Value").GetInt32());
        }

        [TestMethod]
        public void TestJsonElementWritesValue()
        {
            // Setup
            var handler = new JsonToJsonElementPropertyHandler();
            using var document = JsonDocument.Parse("{\"Value\":42}");
            var input = document.RootElement;

            // Act
            var actual = handler.Set(input, null);

            // Assert
            Assert.AreEqual("{\"Value\":42}", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestJsonElementWritesAbsentValue()
        {
            // Setup
            var handler = new JsonToJsonElementPropertyHandler();

            // Act
            var actual = handler.Set(default, null);

            // Assert
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void TestJsonElementRejectsMalformedJson()
        {
            // Setup
            var handler = new JsonToJsonElementPropertyHandler();

            // Act
            Action act = () => handler.Get("{", null);

            // Assert
            Assert.Throws<JsonException>(act);
        }

        [TestMethod]
        public void TestEntityWithReflectionNull()
        {
            // Setup
            var handler = new JsonToEntityPropertyHandler<JsonValue>();

            // Act
            var actual = handler.Get(null, null);

            // Assert
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void TestEntityWithReflectionEmpty()
        {
            // Setup
            var handler = new JsonToEntityPropertyHandler<JsonValue>();

            // Act
            var actual = handler.Get(string.Empty, null);

            // Assert
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void TestEntityWithReflectionJsonNull()
        {
            // Setup
            var handler = new JsonToEntityPropertyHandler<JsonValue>();

            // Act
            var actual = handler.Get("null", null);

            // Assert
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void TestEntityWithReflectionReadsValue()
        {
            // Setup
            var handler = new JsonToEntityPropertyHandler<JsonValue>();

            // Act
            var actual = handler.Get("{\"Value\":42}", null);

            // Assert
            Assert.AreEqual(42, actual.Value);
        }

        [TestMethod]
        public void TestEntityWithReflectionWritesValue()
        {
            // Setup
            var handler = new JsonToEntityPropertyHandler<JsonValue>();
            var input = new JsonValue { Value = 42 };

            // Act
            var actual = handler.Set(input, null);

            // Assert
            Assert.AreEqual("{\"Value\":42}", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestEntityWithReflectionWritesNull()
        {
            // Setup
            var handler = new JsonToEntityPropertyHandler<JsonValue>();

            // Act
            var actual = handler.Set(null, null);

            // Assert
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void TestEntityWithReflectionRejectsMalformedJson()
        {
            // Setup
            var handler = new JsonToEntityPropertyHandler<JsonValue>();

            // Act
            Action act = () => handler.Get("{", null);

            // Assert
            Assert.Throws<JsonException>(act);
        }

        [TestMethod]
        public void TestEntityWithTypeInfoNull()
        {
            // Setup
            var handler = new JsonToEntityPropertyHandler<JsonValue>(HandlerJsonContext.Default.JsonValue);

            // Act
            var actual = handler.Get(null, null);

            // Assert
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void TestEntityWithTypeInfoEmpty()
        {
            // Setup
            var handler = new JsonToEntityPropertyHandler<JsonValue>(HandlerJsonContext.Default.JsonValue);

            // Act
            var actual = handler.Get(string.Empty, null);

            // Assert
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void TestEntityWithTypeInfoJsonNull()
        {
            // Setup
            var handler = new JsonToEntityPropertyHandler<JsonValue>(HandlerJsonContext.Default.JsonValue);

            // Act
            var actual = handler.Get("null", null);

            // Assert
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void TestEntityWithTypeInfoReadsValue()
        {
            // Setup
            var handler = new JsonToEntityPropertyHandler<JsonValue>(HandlerJsonContext.Default.JsonValue);

            // Act
            var actual = handler.Get("{\"Value\":42}", null);

            // Assert
            Assert.AreEqual(42, actual.Value);
        }

        [TestMethod]
        public void TestEntityWithTypeInfoWritesValue()
        {
            // Setup
            var handler = new JsonToEntityPropertyHandler<JsonValue>(HandlerJsonContext.Default.JsonValue);
            var input = new JsonValue { Value = 42 };

            // Act
            var actual = handler.Set(input, null);

            // Assert
            Assert.AreEqual("{\"Value\":42}", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestEntityWithTypeInfoWritesNull()
        {
            // Setup
            var handler = new JsonToEntityPropertyHandler<JsonValue>(HandlerJsonContext.Default.JsonValue);

            // Act
            var actual = handler.Set(null, null);

            // Assert
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void TestEntityWithTypeInfoRejectsMalformedJson()
        {
            // Setup
            var handler = new JsonToEntityPropertyHandler<JsonValue>(HandlerJsonContext.Default.JsonValue);

            // Act
            Action act = () => handler.Get("{", null);

            // Assert
            Assert.Throws<JsonException>(act);
        }

        [TestMethod]
        public void TestEntityRejectsNullTypeInfo()
        {
            // Setup
            JsonTypeInfo<JsonValue> typeInfo = null;

            // Act
            Action act = () => new JsonToEntityPropertyHandler<JsonValue>(typeInfo);

            // Assert
            Assert.ThrowsExactly<ArgumentNullException>(act);
        }

        [TestMethod]
        public void TestObjectWithReflectionNull()
        {
            // Setup
            var handler = new JsonToObjectPropertyHandler<JsonValue>();

            // Act
            var actual = handler.Get(null, null);

            // Assert
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void TestObjectWithReflectionEmpty()
        {
            // Setup
            var handler = new JsonToObjectPropertyHandler<JsonValue>();

            // Act
            var actual = handler.Get(string.Empty, null);

            // Assert
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void TestObjectWithReflectionJsonNull()
        {
            // Setup
            var handler = new JsonToObjectPropertyHandler<JsonValue>();

            // Act
            var actual = handler.Get("null", null);

            // Assert
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void TestObjectWithReflectionReadsValue()
        {
            // Setup
            var handler = new JsonToObjectPropertyHandler<JsonValue>();

            // Act
            var actual = handler.Get("{\"Value\":42}", null);

            // Assert
            Assert.AreEqual(42, actual.Value);
        }

        [TestMethod]
        public void TestObjectWithReflectionWritesValue()
        {
            // Setup
            var handler = new JsonToObjectPropertyHandler<JsonValue>();
            var input = new JsonValue { Value = 42 };

            // Act
            var actual = handler.Set(input, null);

            // Assert
            Assert.AreEqual("{\"Value\":42}", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestObjectWithReflectionWritesNull()
        {
            // Setup
            var handler = new JsonToObjectPropertyHandler<JsonValue>();

            // Act
            var actual = handler.Set(null, null);

            // Assert
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void TestObjectWithReflectionRejectsMalformedJson()
        {
            // Setup
            var handler = new JsonToObjectPropertyHandler<JsonValue>();

            // Act
            Action act = () => handler.Get("{", null);

            // Assert
            Assert.Throws<JsonException>(act);
        }

        [TestMethod]
        public void TestObjectWithTypeInfoNull()
        {
            // Setup
            var handler = new JsonToObjectPropertyHandler<JsonValue>(HandlerJsonContext.Default.JsonValue);

            // Act
            var actual = handler.Get(null, null);

            // Assert
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void TestObjectWithTypeInfoEmpty()
        {
            // Setup
            var handler = new JsonToObjectPropertyHandler<JsonValue>(HandlerJsonContext.Default.JsonValue);

            // Act
            var actual = handler.Get(string.Empty, null);

            // Assert
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void TestObjectWithTypeInfoJsonNull()
        {
            // Setup
            var handler = new JsonToObjectPropertyHandler<JsonValue>(HandlerJsonContext.Default.JsonValue);

            // Act
            var actual = handler.Get("null", null);

            // Assert
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void TestObjectWithTypeInfoReadsValue()
        {
            // Setup
            var handler = new JsonToObjectPropertyHandler<JsonValue>(HandlerJsonContext.Default.JsonValue);

            // Act
            var actual = handler.Get("{\"Value\":42}", null);

            // Assert
            Assert.AreEqual(42, actual.Value);
        }

        [TestMethod]
        public void TestObjectWithTypeInfoWritesValue()
        {
            // Setup
            var handler = new JsonToObjectPropertyHandler<JsonValue>(HandlerJsonContext.Default.JsonValue);
            var input = new JsonValue { Value = 42 };

            // Act
            var actual = handler.Set(input, null);

            // Assert
            Assert.AreEqual("{\"Value\":42}", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestObjectWithTypeInfoWritesNull()
        {
            // Setup
            var handler = new JsonToObjectPropertyHandler<JsonValue>(HandlerJsonContext.Default.JsonValue);

            // Act
            var actual = handler.Set(null, null);

            // Assert
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void TestObjectWithTypeInfoRejectsMalformedJson()
        {
            // Setup
            var handler = new JsonToObjectPropertyHandler<JsonValue>(HandlerJsonContext.Default.JsonValue);

            // Act
            Action act = () => handler.Get("{", null);

            // Assert
            Assert.Throws<JsonException>(act);
        }

        [TestMethod]
        public void TestObjectRejectsNullTypeInfo()
        {
            // Setup
            JsonTypeInfo<JsonValue> typeInfo = null;

            // Act
            Action act = () => new JsonToObjectPropertyHandler<JsonValue>(typeInfo);

            // Assert
            Assert.ThrowsExactly<ArgumentNullException>(act);
        }

        public class JsonValue
        {
            public int Value { get; set; }
        }
    }
}
