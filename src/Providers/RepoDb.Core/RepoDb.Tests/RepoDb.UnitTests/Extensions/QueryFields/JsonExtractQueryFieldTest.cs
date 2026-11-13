#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Enumerations;
using RepoDb.Extensions.QueryFields;
using RepoDb.UnitTests.CustomObjects;
using System;

namespace RepoDb.UnitTests.Extensions.QueryFields
{
    [TestClass]
    public class JsonExtractQueryFieldTest
    {
        #region Constructors

        [TestMethod]
        public void TestJsonExtractQueryFieldConstructor()
        {
            // Act
            var field = new JsonExtractQueryField("Document", "Address.City", Operation.NotEqual, "Value", null);

            // Assert
            Assert.AreEqual("Document", field.Field.Name, StringComparer.Ordinal);
            Assert.AreEqual(Operation.NotEqual, field.Operation);
            Assert.AreEqual("Value", field.Parameter.Value);
        }

        [TestMethod]
        public void TestJsonExtractQueryFieldConstructorWithDefaultOperation()
        {
            // Act
            var field = new JsonExtractQueryField("Document", "Id", 5);

            // Assert
            Assert.AreEqual(Operation.Equal, field.Operation);
        }

        [TestMethod]
        public void TestJsonExtractQueryFieldConstructorWithEmptyPath()
        {
            // Act and Assert
            Assert.Throws<ArgumentException>(() => new JsonExtractQueryField("Document", string.Empty, 5));
        }

        #endregion

        #region Properties

        [TestMethod]
        public void TestJsonExtractQueryFieldPath()
        {
            // Act
            var field = new JsonExtractQueryField("Document", "Address.City", "Value");

            // Assert
            Assert.AreEqual("Address.City", field.Path, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestJsonExtractQueryFieldSegments()
        {
            // Act
            var field = new JsonExtractQueryField("Document", "Tags[0]", "Value");

            // Assert
            Assert.AreEqual("Tags|[0]", string.Join("|", field.Segments), StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestJsonExtractQueryFieldFormat()
        {
            // Act
            var field = new JsonExtractQueryField("Document", "Id", 5);

            // Assert
            Assert.AreEqual("JSON_EXTRACT({0}, @@path@@)", field.Format, StringComparer.Ordinal);
        }

        #endregion

        #region Parameter

        [TestMethod]
        public void TestJsonExtractQueryFieldParameterName()
        {
            // Act
            var field = new JsonExtractQueryField("Document", "Address.City", "Value");

            // Assert
            Assert.AreEqual("Document_Address_City", field.Parameter.Name, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestJsonExtractQueryFieldParameterNameWithArrayIndex()
        {
            // Act
            var field = new JsonExtractQueryField("Tags", "Items[2]", "Value");

            // Assert
            Assert.AreEqual("Tags_Items_2", field.Parameter.Name, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestJsonExtractQueryFieldParameterNameWithSpecialName()
        {
            // Act
            var field = new JsonExtractQueryField("Document", "display-name", "Value");

            // Assert
            Assert.AreEqual("Document_display_name", field.Parameter.Name, StringComparer.Ordinal);
        }

        #endregion

        #region GetString

        [TestMethod]
        public void TestJsonExtractQueryFieldGetString()
        {
            // Setup
            var field = new JsonExtractQueryField("Document", "Address.City", "Value");

            // Act
            var text = field.GetString(0, new CustomDbSetting());

            // Assert
            Assert.AreEqual("JSON_EXTRACT([Document], '$.Address.City') = @Document_Address_City", text, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestJsonExtractQueryFieldGetStringWithIndex()
        {
            // Setup
            var field = new JsonExtractQueryField("Document", "Id", 5);

            // Act
            var text = field.GetString(1, new CustomDbSetting());

            // Assert
            Assert.AreEqual("JSON_EXTRACT([Document], '$.Id') = @Document_Id_1", text, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestJsonExtractQueryFieldGetStringWithArrayIndex()
        {
            // Setup
            var field = new JsonExtractQueryField("Tags", "[0]", "Value");

            // Act
            var text = field.GetString(0, new CustomDbSetting());

            // Assert
            Assert.AreEqual("JSON_EXTRACT([Tags], '$[0]') = @Tags_0", text, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestJsonExtractQueryFieldGetStringWithOperation()
        {
            // Setup
            var field = new JsonExtractQueryField("Document", "Id", Operation.GreaterThan, 5, null);

            // Act
            var text = field.GetString(0, new CustomDbSetting());

            // Assert
            Assert.AreEqual("JSON_EXTRACT([Document], '$.Id') > @Document_Id", text, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestJsonExtractQueryFieldGetStringWithNullValue()
        {
            // Setup
            var field = new JsonExtractQueryField("Document", "Id", null);

            // Act
            var text = field.GetString(0, new CustomDbSetting());

            // Assert
            Assert.AreEqual("JSON_EXTRACT([Document], '$.Id') IS NULL", text, StringComparer.Ordinal);
        }

        #endregion

        #region Equality

        [TestMethod]
        public void TestJsonExtractQueryFieldGetHashCodeWithSamePath()
        {
            // Setup
            var left = new JsonExtractQueryField("Document", "Id", 5);
            var right = new JsonExtractQueryField("Document", "Id", 5);

            // Act
            var result = left.GetHashCode() == right.GetHashCode();

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void TestJsonExtractQueryFieldGetHashCodeWithDifferentPath()
        {
            // Setup
            var left = new JsonExtractQueryField("Document", "Id", 5);
            var right = new JsonExtractQueryField("Document", "Address.City", 5);

            // Act
            var result = left.GetHashCode() == right.GetHashCode();

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void TestJsonExtractQueryFieldEqualsWithSamePath()
        {
            // Setup
            var left = new JsonExtractQueryField("Document", "Id", 5);
            var right = new JsonExtractQueryField("Document", "Id", 5);

            // Act
            var result = left.Equals(right);

            // Assert
            Assert.IsTrue(result);
        }

        #endregion
    }
}
