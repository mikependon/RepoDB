#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Dialects;
using System;

namespace RepoDb.CockroachDb.UnitTests.Dialects
{
    [TestClass]
    public class CockroachDbDbJsonDialectFormatTest
    {
        #region FormatPath

        [TestMethod]
        public void TestCockroachDbDbJsonDialectFormatPath()
        {
            // Act
            var result = new CockroachDbDbJsonDialect().FormatPath(new[] { "Address", "City" });

            // Assert
            Assert.AreEqual("'{Address,City}'", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestCockroachDbDbJsonDialectFormatPathWithArrayIndex()
        {
            // Act
            var result = new CockroachDbDbJsonDialect().FormatPath(new[] { "Tags", "[0]" });

            // Assert
            Assert.AreEqual("'{Tags,0}'", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestCockroachDbDbJsonDialectFormatPathWithSpecialName()
        {
            // Act
            var result = new CockroachDbDbJsonDialect().FormatPath(new[] { "display-name" });

            // Assert
            Assert.AreEqual("'{\"display-name\"}'", result, StringComparer.Ordinal);
        }

        #endregion

        #region GetExtractFormat

        [TestMethod]
        public void TestCockroachDbDbJsonDialectGetExtractFormatForInteger()
        {
            // Act
            var result = new CockroachDbDbJsonDialect().GetExtractFormat(typeof(int));

            // Assert
            Assert.AreEqual("CAST({0} #>> @@path@@ AS integer)", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestCockroachDbDbJsonDialectGetExtractFormatForNullableInteger()
        {
            // Act
            var result = new CockroachDbDbJsonDialect().GetExtractFormat(typeof(int?));

            // Assert
            Assert.AreEqual("CAST({0} #>> @@path@@ AS integer)", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestCockroachDbDbJsonDialectGetExtractFormatForBoolean()
        {
            // Act
            var result = new CockroachDbDbJsonDialect().GetExtractFormat(typeof(bool));

            // Assert
            Assert.AreEqual("CAST({0} #>> @@path@@ AS boolean)", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestCockroachDbDbJsonDialectGetExtractFormatForString()
        {
            // Act
            var result = new CockroachDbDbJsonDialect().GetExtractFormat(typeof(string));

            // Assert
            Assert.AreEqual("({0} #>> @@path@@)", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestCockroachDbDbJsonDialectGetExtractFormatWithoutType()
        {
            // Act
            var result = new CockroachDbDbJsonDialect().GetExtractFormat(null);

            // Assert
            Assert.AreEqual("({0} #>> @@path@@)", result, StringComparer.Ordinal);
        }

        #endregion
    }
}
