#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Dialects;
using System;

namespace RepoDb.DuckDb.UnitTests.Dialects
{
    [TestClass]
    public class DuckDbDbJsonDialectTest
    {
        #region GetExtractFormat

        [TestMethod]
        public void TestDuckDbDbJsonDialectGetExtractFormatForInteger()
        {
            // Act
            var result = new DuckDbDbJsonDialect().GetExtractFormat(typeof(int));

            // Assert
            Assert.AreEqual("CAST(json_extract_string({0}, @@path@@) AS INTEGER)", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDuckDbDbJsonDialectGetExtractFormatForNullableBoolean()
        {
            // Act
            var result = new DuckDbDbJsonDialect().GetExtractFormat(typeof(bool?));

            // Assert
            Assert.AreEqual("CAST(json_extract_string({0}, @@path@@) AS BOOLEAN)", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDuckDbDbJsonDialectGetExtractFormatForString()
        {
            // Act
            var result = new DuckDbDbJsonDialect().GetExtractFormat(typeof(string));

            // Assert
            Assert.AreEqual("json_extract_string({0}, @@path@@)", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDuckDbDbJsonDialectGetExtractFormatWithoutType()
        {
            // Act
            var result = new DuckDbDbJsonDialect().GetExtractFormat(null);

            // Assert
            Assert.AreEqual("json_extract_string({0}, @@path@@)", result, StringComparer.Ordinal);
        }

        #endregion

        #region FormatPath

        [TestMethod]
        public void TestDuckDbDbJsonDialectFormatPath()
        {
            // Act
            var result = new DuckDbDbJsonDialect().FormatPath(new[] { "Address", "City" });

            // Assert
            Assert.AreEqual("'$.Address.City'", result, StringComparer.Ordinal);
        }

        #endregion
    }
}
