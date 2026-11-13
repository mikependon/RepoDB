#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Dialects;
using System;

namespace RepoDb.MySqlConnector.UnitTests.Dialects
{
    [TestClass]
    public class MySqlConnectorDbJsonDialectFormatTest
    {
        #region GetExtractFormat

        [TestMethod]
        public void TestMySqlConnectorDbJsonDialectGetExtractFormatForString()
        {
            // Act
            var result = new MySqlConnectorDbJsonDialect().GetExtractFormat(typeof(string));

            // Assert
            Assert.AreEqual("JSON_UNQUOTE(JSON_EXTRACT({0}, @@path@@))", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMySqlConnectorDbJsonDialectGetExtractFormatForInteger()
        {
            // Act
            var result = new MySqlConnectorDbJsonDialect().GetExtractFormat(typeof(int));

            // Assert
            Assert.AreEqual("JSON_UNQUOTE(JSON_EXTRACT({0}, @@path@@))", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMySqlConnectorDbJsonDialectGetExtractFormatWithoutType()
        {
            // Act
            var result = new MySqlConnectorDbJsonDialect().GetExtractFormat(null);

            // Assert
            Assert.AreEqual("JSON_UNQUOTE(JSON_EXTRACT({0}, @@path@@))", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMySqlConnectorDbJsonDialectGetExtractFormatForBoolean()
        {
            // Act
            var result = new MySqlConnectorDbJsonDialect().GetExtractFormat(typeof(bool));

            // Assert
            Assert.AreEqual("(JSON_UNQUOTE(JSON_EXTRACT({0}, @@path@@)) = 'true')", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMySqlConnectorDbJsonDialectGetExtractFormatForNullableBoolean()
        {
            // Act
            var result = new MySqlConnectorDbJsonDialect().GetExtractFormat(typeof(bool?));

            // Assert
            Assert.AreEqual("(JSON_UNQUOTE(JSON_EXTRACT({0}, @@path@@)) = 'true')", result, StringComparer.Ordinal);
        }

        #endregion

        #region FormatPath

        [TestMethod]
        public void TestMySqlConnectorDbJsonDialectFormatPath()
        {
            // Act
            var result = new MySqlConnectorDbJsonDialect().FormatPath(new[] { "Address", "City" });

            // Assert
            Assert.AreEqual("'$.Address.City'", result, StringComparer.Ordinal);
        }

        #endregion
    }
}
