#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.UnitTests.CustomObjects;
using System;
using System.Text.Json.Nodes;

namespace RepoDb.UnitTests.Types
{
    [TestClass]
    public class DbJsonValueTest
    {
        #region Constructors

        [TestMethod]
        public void TestDbJsonValueConstructorWithObject()
        {
            // Setup
            var document = new JsonSampleDocument { Id = 5 };

            // Act
            var value = new DbJsonValue<JsonSampleDocument>(document);

            // Assert
            Assert.AreSame(document, value.Value);
        }

        [TestMethod]
        public void TestDbJsonValueConstructorWithNode()
        {
            // Setup
            var node = JsonNode.Parse("{\"Id\":7}");

            // Act
            var value = new DbJsonValue<JsonSampleDocument>(node);

            // Assert
            Assert.AreEqual(7, value.Value.Id);
        }

        [TestMethod]
        public void TestDbJsonValueConstructorWithNullObject()
        {
            // Act and Assert
            Assert.Throws<ArgumentNullException>(() => new DbJsonValue<JsonSampleDocument>((JsonSampleDocument)null));
        }

        [TestMethod]
        public void TestDbJsonValueConstructorWithNullNode()
        {
            // Act and Assert
            Assert.Throws<ArgumentNullException>(() => new DbJsonValue<JsonSampleDocument>((JsonNode)null));
        }

        #endregion

        #region Properties

        [TestMethod]
        public void TestDbJsonValueIsEmptyWithDefault()
        {
            // Act
            var value = default(DbJsonValue<JsonSampleDocument>);

            // Assert
            Assert.IsTrue(value.IsEmpty);
        }

        [TestMethod]
        public void TestDbJsonValueIsEmptyWithObject()
        {
            // Act
            var value = new DbJsonValue<JsonSampleDocument>(new JsonSampleDocument());

            // Assert
            Assert.IsFalse(value.IsEmpty);
        }

        [TestMethod]
        public void TestDbJsonValueJsonFromObject()
        {
            // Act
            var value = new DbJsonValue<JsonSampleDocument>(new JsonSampleDocument { Id = 5 });

            // Assert
            Assert.AreEqual(5, value.Json["Id"].GetValue<int>());
        }

        [TestMethod]
        public void TestDbJsonValueJsonFromNode()
        {
            // Setup
            var node = JsonNode.Parse("{\"Id\":7}");

            // Act
            var value = new DbJsonValue<JsonSampleDocument>(node);

            // Assert
            Assert.AreSame(node, value.Json);
        }

        #endregion

        #region Parse

        [TestMethod]
        public void TestDbJsonValueParse()
        {
            // Act
            var value = DbJsonValue<JsonSampleDocument>.Parse("{\"Id\":5}");

            // Assert
            Assert.AreEqual(5, value.Value.Id);
        }

        [TestMethod]
        public void TestDbJsonValueParseWithNull()
        {
            // Act
            var value = DbJsonValue<JsonSampleDocument>.Parse(null);

            // Assert
            Assert.IsTrue(value.IsEmpty);
        }

        [TestMethod]
        public void TestDbJsonValueTryParseWithValidJson()
        {
            // Act
            var parsed = DbJsonValue<JsonSampleDocument>.TryParse("{\"Id\":5}", out var value);

            // Assert
            Assert.IsTrue(parsed);
            Assert.AreEqual(5, value.Value.Id);
        }

        [TestMethod]
        public void TestDbJsonValueTryParseWithInvalidJson()
        {
            // Act
            var parsed = DbJsonValue<JsonSampleDocument>.TryParse("{", out var value);

            // Assert
            Assert.IsFalse(parsed);
            Assert.IsTrue(value.IsEmpty);
        }

        [TestMethod]
        public void TestDbJsonValueTryParseWithNull()
        {
            // Act
            var parsed = DbJsonValue<JsonSampleDocument>.TryParse(null, out _);

            // Assert
            Assert.IsFalse(parsed);
        }

        #endregion

        #region ToJsonString and ToString

        [TestMethod]
        public void TestDbJsonValueToJsonString()
        {
            // Setup
            var value = DbJsonValue<JsonSampleDocument>.Parse("{\"Id\":5}");

            // Act
            var json = value.ToJsonString();

            // Assert
            Assert.AreEqual("{\"Id\":5}", json, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbJsonValueToJsonStringWithEmpty()
        {
            // Act
            var json = default(DbJsonValue<JsonSampleDocument>).ToJsonString();

            // Assert
            Assert.IsNull(json);
        }

        [TestMethod]
        public void TestDbJsonValueToString()
        {
            // Setup
            var value = DbJsonValue<JsonSampleDocument>.Parse("{\"Id\":5}");

            // Act
            var text = value.ToString();

            // Assert
            Assert.AreEqual("{\"Id\":5}", text, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDbJsonValueToStringWithEmpty()
        {
            // Act
            var text = default(DbJsonValue<JsonSampleDocument>).ToString();

            // Assert
            Assert.AreEqual("null", text, StringComparer.Ordinal);
        }

        #endregion

        #region Equality

        [TestMethod]
        public void TestDbJsonValueEqualsWithSameJson()
        {
            // Setup
            var left = DbJsonValue<JsonSampleDocument>.Parse("{\"Id\":5}");
            var right = DbJsonValue<JsonSampleDocument>.Parse("{\"Id\":5}");

            // Act
            var result = left.Equals(right);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void TestDbJsonValueEqualsWithDifferentJson()
        {
            // Setup
            var left = DbJsonValue<JsonSampleDocument>.Parse("{\"Id\":5}");
            var right = DbJsonValue<JsonSampleDocument>.Parse("{\"Id\":6}");

            // Act
            var result = left.Equals(right);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void TestDbJsonValueEqualsWithObject()
        {
            // Setup
            var left = DbJsonValue<JsonSampleDocument>.Parse("{\"Id\":5}");
            object right = DbJsonValue<JsonSampleDocument>.Parse("{\"Id\":5}");

            // Act
            var result = left.Equals(right);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void TestDbJsonValueEqualsWithOtherType()
        {
            // Setup
            var left = DbJsonValue<JsonSampleDocument>.Parse("{\"Id\":5}");

            // Act
            var result = left.Equals("{\"Id\":5}");

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void TestDbJsonValueEqualityOperator()
        {
            // Setup
            var left = DbJsonValue<JsonSampleDocument>.Parse("{\"Id\":5}");
            var right = DbJsonValue<JsonSampleDocument>.Parse("{\"Id\":5}");

            // Act
            var result = left == right;

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void TestDbJsonValueInequalityOperator()
        {
            // Setup
            var left = DbJsonValue<JsonSampleDocument>.Parse("{\"Id\":5}");
            var right = DbJsonValue<JsonSampleDocument>.Parse("{\"Id\":6}");

            // Act
            var result = left != right;

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void TestDbJsonValueGetHashCodeWithSameJson()
        {
            // Setup
            var left = DbJsonValue<JsonSampleDocument>.Parse("{\"Id\":5}");
            var right = DbJsonValue<JsonSampleDocument>.Parse("{\"Id\":5}");

            // Act
            var result = left.GetHashCode() == right.GetHashCode();

            // Assert
            Assert.IsTrue(result);
        }

        #endregion

        #region Conversions

        [TestMethod]
        public void TestDbJsonValueImplicitConversionFromObject()
        {
            // Act
            DbJsonValue<JsonSampleDocument> value = new JsonSampleDocument { Id = 5 };

            // Assert
            Assert.AreEqual(5, value.Value.Id);
        }

        [TestMethod]
        public void TestDbJsonValueImplicitConversionFromNull()
        {
            // Act
            DbJsonValue<JsonSampleDocument> value = (JsonSampleDocument)null;

            // Assert
            Assert.IsTrue(value.IsEmpty);
        }

        [TestMethod]
        public void TestDbJsonValueExplicitConversionFromString()
        {
            // Act
            var value = (DbJsonValue<JsonSampleDocument>)"{\"Id\":5}";

            // Assert
            Assert.AreEqual(5, value.Value.Id);
        }

        #endregion
    }
}
