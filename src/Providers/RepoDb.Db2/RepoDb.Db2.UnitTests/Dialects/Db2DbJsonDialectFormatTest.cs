#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Dialects;
using System;

namespace RepoDb.Db2.UnitTests.Dialects
{
    [TestClass]
    public class Db2DbJsonDialectFormatTest
    {
        #region GetExtractFormat

        [TestMethod]
        public void TestDb2DbJsonDialectGetExtractFormatForString()
        {
            // Act
            var result = new Db2DbJsonDialect().GetExtractFormat(typeof(string));

            // Assert
            Assert.AreEqual("JSON_VALUE({0}, @@path@@)", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDb2DbJsonDialectGetExtractFormatWithoutType()
        {
            // Act
            var result = new Db2DbJsonDialect().GetExtractFormat(null);

            // Assert
            Assert.AreEqual("JSON_VALUE({0}, @@path@@)", result, StringComparer.Ordinal);
        }

        #endregion

        #region FormatPath

        [TestMethod]
        public void TestDb2DbJsonDialectFormatPath()
        {
            // Act
            var result = new Db2DbJsonDialect().FormatPath(new[] { "Address", "City" });

            // Assert
            Assert.AreEqual("'$.Address.City'", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDb2DbJsonDialectFormatPathWithArrayIndex()
        {
            // Act
            var result = new Db2DbJsonDialect().FormatPath(new[] { "Tags", "[0]" });

            // Assert
            Assert.AreEqual("'$.Tags[0]'", result, StringComparer.Ordinal);
        }

        #endregion
    }
}
