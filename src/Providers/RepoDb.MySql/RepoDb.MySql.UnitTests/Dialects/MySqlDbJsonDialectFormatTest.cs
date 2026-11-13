#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Dialects;
using System;

namespace RepoDb.MySql.UnitTests.Dialects
{
    [TestClass]
    public class MySqlDbJsonDialectFormatTest
    {
        #region GetExtractFormat

        [TestMethod]
        public void TestMySqlDbJsonDialectGetExtractFormatForString()
        {
            // Act
            var result = new MySqlDbJsonDialect().GetExtractFormat(typeof(string));

            // Assert
            Assert.AreEqual("JSON_UNQUOTE(JSON_EXTRACT({0}, @@path@@))", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMySqlDbJsonDialectGetExtractFormatForInteger()
        {
            // Act
            var result = new MySqlDbJsonDialect().GetExtractFormat(typeof(int));

            // Assert
            Assert.AreEqual("JSON_UNQUOTE(JSON_EXTRACT({0}, @@path@@))", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMySqlDbJsonDialectGetExtractFormatWithoutType()
        {
            // Act
            var result = new MySqlDbJsonDialect().GetExtractFormat(null);

            // Assert
            Assert.AreEqual("JSON_UNQUOTE(JSON_EXTRACT({0}, @@path@@))", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMySqlDbJsonDialectGetExtractFormatForBoolean()
        {
            // Act
            var result = new MySqlDbJsonDialect().GetExtractFormat(typeof(bool));

            // Assert
            Assert.AreEqual("(JSON_UNQUOTE(JSON_EXTRACT({0}, @@path@@)) = 'true')", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMySqlDbJsonDialectGetExtractFormatForNullableBoolean()
        {
            // Act
            var result = new MySqlDbJsonDialect().GetExtractFormat(typeof(bool?));

            // Assert
            Assert.AreEqual("(JSON_UNQUOTE(JSON_EXTRACT({0}, @@path@@)) = 'true')", result, StringComparer.Ordinal);
        }

        #endregion

        #region FormatPath

        [TestMethod]
        public void TestMySqlDbJsonDialectFormatPath()
        {
            // Act
            var result = new MySqlDbJsonDialect().FormatPath(new[] { "Address", "City" });

            // Assert
            Assert.AreEqual("'$.Address.City'", result, StringComparer.Ordinal);
        }

        #endregion
    }
}
