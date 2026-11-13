#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Dialects;
using System;

namespace RepoDb.AuroraDb.PostgreSql.UnitTests.Dialects
{
    [TestClass]
    public class AuroraDbDbJsonDialectFormatTest
    {
        #region FormatPath

        [TestMethod]
        public void TestAuroraDbDbJsonDialectFormatPath()
        {
            // Act
            var result = new AuroraDbDbJsonDialect().FormatPath(new[] { "Address", "City" });

            // Assert
            Assert.AreEqual("'{Address,City}'", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestAuroraDbDbJsonDialectFormatPathWithArrayIndex()
        {
            // Act
            var result = new AuroraDbDbJsonDialect().FormatPath(new[] { "Tags", "[0]" });

            // Assert
            Assert.AreEqual("'{Tags,0}'", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestAuroraDbDbJsonDialectFormatPathWithSpecialName()
        {
            // Act
            var result = new AuroraDbDbJsonDialect().FormatPath(new[] { "display-name" });

            // Assert
            Assert.AreEqual("'{\"display-name\"}'", result, StringComparer.Ordinal);
        }

        #endregion

        #region GetExtractFormat

        [TestMethod]
        public void TestAuroraDbDbJsonDialectGetExtractFormatForInteger()
        {
            // Act
            var result = new AuroraDbDbJsonDialect().GetExtractFormat(typeof(int));

            // Assert
            Assert.AreEqual("CAST({0} #>> @@path@@ AS integer)", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestAuroraDbDbJsonDialectGetExtractFormatForNullableInteger()
        {
            // Act
            var result = new AuroraDbDbJsonDialect().GetExtractFormat(typeof(int?));

            // Assert
            Assert.AreEqual("CAST({0} #>> @@path@@ AS integer)", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestAuroraDbDbJsonDialectGetExtractFormatForBoolean()
        {
            // Act
            var result = new AuroraDbDbJsonDialect().GetExtractFormat(typeof(bool));

            // Assert
            Assert.AreEqual("CAST({0} #>> @@path@@ AS boolean)", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestAuroraDbDbJsonDialectGetExtractFormatForString()
        {
            // Act
            var result = new AuroraDbDbJsonDialect().GetExtractFormat(typeof(string));

            // Assert
            Assert.AreEqual("({0} #>> @@path@@)", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestAuroraDbDbJsonDialectGetExtractFormatWithoutType()
        {
            // Act
            var result = new AuroraDbDbJsonDialect().GetExtractFormat(null);

            // Assert
            Assert.AreEqual("({0} #>> @@path@@)", result, StringComparer.Ordinal);
        }

        #endregion
    }
}
