#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Dialects;
using System;

namespace RepoDb.SapHana.UnitTests.Dialects
{
    [TestClass]
    public class SapHanaDbJsonDialectFormatTest
    {
        #region GetExtractFormat

        [TestMethod]
        public void TestSapHanaDbJsonDialectGetExtractFormatForString()
        {
            // Act
            var result = new SapHanaDbJsonDialect().GetExtractFormat(typeof(string));

            // Assert
            Assert.AreEqual("JSON_VALUE({0}, @@path@@)", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestSapHanaDbJsonDialectGetExtractFormatWithoutType()
        {
            // Act
            var result = new SapHanaDbJsonDialect().GetExtractFormat(null);

            // Assert
            Assert.AreEqual("JSON_VALUE({0}, @@path@@)", result, StringComparer.Ordinal);
        }

        #endregion

        #region FormatPath

        [TestMethod]
        public void TestSapHanaDbJsonDialectFormatPath()
        {
            // Act
            var result = new SapHanaDbJsonDialect().FormatPath(new[] { "Address", "City" });

            // Assert
            Assert.AreEqual("'$.Address.City'", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestSapHanaDbJsonDialectFormatPathWithArrayIndex()
        {
            // Act
            var result = new SapHanaDbJsonDialect().FormatPath(new[] { "Tags", "[0]" });

            // Assert
            Assert.AreEqual("'$.Tags[0]'", result, StringComparer.Ordinal);
        }

        #endregion
    }
}
