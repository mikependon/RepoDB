#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Dialects;
using System;

namespace RepoDb.Oracle.UnitTests.Dialects
{
    [TestClass]
    public class OracleDbJsonDialectFormatTest
    {
        #region GetExtractFormat

        [TestMethod]
        public void TestOracleDbJsonDialectGetExtractFormatForString()
        {
            // Act
            var result = new OracleDbJsonDialect().GetExtractFormat(typeof(string));

            // Assert
            Assert.AreEqual("JSON_VALUE({0}, @@path@@)", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestOracleDbJsonDialectGetExtractFormatWithoutType()
        {
            // Act
            var result = new OracleDbJsonDialect().GetExtractFormat(null);

            // Assert
            Assert.AreEqual("JSON_VALUE({0}, @@path@@)", result, StringComparer.Ordinal);
        }

        #endregion

        #region FormatPath

        [TestMethod]
        public void TestOracleDbJsonDialectFormatPath()
        {
            // Act
            var result = new OracleDbJsonDialect().FormatPath(new[] { "Address", "City" });

            // Assert
            Assert.AreEqual("'$.Address.City'", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestOracleDbJsonDialectFormatPathWithArrayIndex()
        {
            // Act
            var result = new OracleDbJsonDialect().FormatPath(new[] { "Tags", "[0]" });

            // Assert
            Assert.AreEqual("'$.Tags[0]'", result, StringComparer.Ordinal);
        }

        #endregion
    }
}
