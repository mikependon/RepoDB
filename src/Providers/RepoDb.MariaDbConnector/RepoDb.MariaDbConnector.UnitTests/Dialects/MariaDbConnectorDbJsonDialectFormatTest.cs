#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Dialects;
using System;

namespace RepoDb.MariaDb.UnitTests.Dialects
{
    [TestClass]
    public class MariaDbConnectorDbJsonDialectFormatTest
    {
        #region GetExtractFormat

        [TestMethod]
        public void TestMariaDbConnectorDbJsonDialectGetExtractFormatForString()
        {
            // Act
            var result = new MariaDbConnectorDbJsonDialect().GetExtractFormat(typeof(string));

            // Assert
            Assert.AreEqual("JSON_UNQUOTE(JSON_EXTRACT({0}, @@path@@))", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMariaDbConnectorDbJsonDialectGetExtractFormatForInteger()
        {
            // Act
            var result = new MariaDbConnectorDbJsonDialect().GetExtractFormat(typeof(int));

            // Assert
            Assert.AreEqual("JSON_UNQUOTE(JSON_EXTRACT({0}, @@path@@))", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMariaDbConnectorDbJsonDialectGetExtractFormatWithoutType()
        {
            // Act
            var result = new MariaDbConnectorDbJsonDialect().GetExtractFormat(null);

            // Assert
            Assert.AreEqual("JSON_UNQUOTE(JSON_EXTRACT({0}, @@path@@))", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMariaDbConnectorDbJsonDialectGetExtractFormatForBoolean()
        {
            // Act
            var result = new MariaDbConnectorDbJsonDialect().GetExtractFormat(typeof(bool));

            // Assert
            Assert.AreEqual("(JSON_UNQUOTE(JSON_EXTRACT({0}, @@path@@)) = 'true')", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMariaDbConnectorDbJsonDialectGetExtractFormatForNullableBoolean()
        {
            // Act
            var result = new MariaDbConnectorDbJsonDialect().GetExtractFormat(typeof(bool?));

            // Assert
            Assert.AreEqual("(JSON_UNQUOTE(JSON_EXTRACT({0}, @@path@@)) = 'true')", result, StringComparer.Ordinal);
        }

        #endregion

        #region FormatPath

        [TestMethod]
        public void TestMariaDbConnectorDbJsonDialectFormatPath()
        {
            // Act
            var result = new MariaDbConnectorDbJsonDialect().FormatPath(new[] { "Address", "City" });

            // Assert
            Assert.AreEqual("'$.Address.City'", result, StringComparer.Ordinal);
        }

        #endregion
    }
}
