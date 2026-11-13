#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Dialects;
using System;

namespace RepoDb.AuroraDb.MySqlConnector.UnitTests.Dialects
{
    [TestClass]
    public class AuroraDbDbJsonDialectFormatTest
    {
        #region GetExtractFormat

        [TestMethod]
        public void TestAuroraDbDbJsonDialectGetExtractFormatForString()
        {
            // Act
            var result = new AuroraDbDbJsonDialect().GetExtractFormat(typeof(string));

            // Assert
            Assert.AreEqual("JSON_UNQUOTE(JSON_EXTRACT({0}, @@path@@))", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestAuroraDbDbJsonDialectGetExtractFormatForInteger()
        {
            // Act
            var result = new AuroraDbDbJsonDialect().GetExtractFormat(typeof(int));

            // Assert
            Assert.AreEqual("JSON_UNQUOTE(JSON_EXTRACT({0}, @@path@@))", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestAuroraDbDbJsonDialectGetExtractFormatWithoutType()
        {
            // Act
            var result = new AuroraDbDbJsonDialect().GetExtractFormat(null);

            // Assert
            Assert.AreEqual("JSON_UNQUOTE(JSON_EXTRACT({0}, @@path@@))", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestAuroraDbDbJsonDialectGetExtractFormatForBoolean()
        {
            // Act
            var result = new AuroraDbDbJsonDialect().GetExtractFormat(typeof(bool));

            // Assert
            Assert.AreEqual("(JSON_UNQUOTE(JSON_EXTRACT({0}, @@path@@)) = 'true')", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestAuroraDbDbJsonDialectGetExtractFormatForNullableBoolean()
        {
            // Act
            var result = new AuroraDbDbJsonDialect().GetExtractFormat(typeof(bool?));

            // Assert
            Assert.AreEqual("(JSON_UNQUOTE(JSON_EXTRACT({0}, @@path@@)) = 'true')", result, StringComparer.Ordinal);
        }

        #endregion

        #region FormatPath

        [TestMethod]
        public void TestAuroraDbDbJsonDialectFormatPath()
        {
            // Act
            var result = new AuroraDbDbJsonDialect().FormatPath(new[] { "Address", "City" });

            // Assert
            Assert.AreEqual("'$.Address.City'", result, StringComparer.Ordinal);
        }

        #endregion
    }
}
