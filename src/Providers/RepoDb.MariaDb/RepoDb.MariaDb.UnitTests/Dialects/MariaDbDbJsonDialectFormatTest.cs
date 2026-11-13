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
    public class MariaDbDbJsonDialectFormatTest
    {
        #region GetExtractFormat

        [TestMethod]
        public void TestMariaDbDbJsonDialectGetExtractFormatForString()
        {
            // Act
            var result = new MariaDbDbJsonDialect().GetExtractFormat(typeof(string));

            // Assert
            Assert.AreEqual("JSON_UNQUOTE(JSON_EXTRACT({0}, @@path@@))", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMariaDbDbJsonDialectGetExtractFormatForInteger()
        {
            // Act
            var result = new MariaDbDbJsonDialect().GetExtractFormat(typeof(int));

            // Assert
            Assert.AreEqual("JSON_UNQUOTE(JSON_EXTRACT({0}, @@path@@))", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMariaDbDbJsonDialectGetExtractFormatWithoutType()
        {
            // Act
            var result = new MariaDbDbJsonDialect().GetExtractFormat(null);

            // Assert
            Assert.AreEqual("JSON_UNQUOTE(JSON_EXTRACT({0}, @@path@@))", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMariaDbDbJsonDialectGetExtractFormatForBoolean()
        {
            // Act
            var result = new MariaDbDbJsonDialect().GetExtractFormat(typeof(bool));

            // Assert
            Assert.AreEqual("(JSON_UNQUOTE(JSON_EXTRACT({0}, @@path@@)) = 'true')", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMariaDbDbJsonDialectGetExtractFormatForNullableBoolean()
        {
            // Act
            var result = new MariaDbDbJsonDialect().GetExtractFormat(typeof(bool?));

            // Assert
            Assert.AreEqual("(JSON_UNQUOTE(JSON_EXTRACT({0}, @@path@@)) = 'true')", result, StringComparer.Ordinal);
        }

        #endregion

        #region FormatPath

        [TestMethod]
        public void TestMariaDbDbJsonDialectFormatPath()
        {
            // Act
            var result = new MariaDbDbJsonDialect().FormatPath(new[] { "Address", "City" });

            // Assert
            Assert.AreEqual("'$.Address.City'", result, StringComparer.Ordinal);
        }

        #endregion
    }
}
