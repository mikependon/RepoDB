#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Dialects;
using System;

namespace RepoDb.EnterpriseDb.UnitTests.Dialects
{
    [TestClass]
    public class EnterpriseDbDbJsonDialectFormatTest
    {
        #region FormatPath

        [TestMethod]
        public void TestEnterpriseDbDbJsonDialectFormatPath()
        {
            // Act
            var result = new EnterpriseDbDbJsonDialect().FormatPath(new[] { "Address", "City" });

            // Assert
            Assert.AreEqual("'{Address,City}'", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestEnterpriseDbDbJsonDialectFormatPathWithArrayIndex()
        {
            // Act
            var result = new EnterpriseDbDbJsonDialect().FormatPath(new[] { "Tags", "[0]" });

            // Assert
            Assert.AreEqual("'{Tags,0}'", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestEnterpriseDbDbJsonDialectFormatPathWithSpecialName()
        {
            // Act
            var result = new EnterpriseDbDbJsonDialect().FormatPath(new[] { "display-name" });

            // Assert
            Assert.AreEqual("'{\"display-name\"}'", result, StringComparer.Ordinal);
        }

        #endregion

        #region GetExtractFormat

        [TestMethod]
        public void TestEnterpriseDbDbJsonDialectGetExtractFormatForInteger()
        {
            // Act
            var result = new EnterpriseDbDbJsonDialect().GetExtractFormat(typeof(int));

            // Assert
            Assert.AreEqual("CAST({0} #>> @@path@@ AS integer)", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestEnterpriseDbDbJsonDialectGetExtractFormatForNullableInteger()
        {
            // Act
            var result = new EnterpriseDbDbJsonDialect().GetExtractFormat(typeof(int?));

            // Assert
            Assert.AreEqual("CAST({0} #>> @@path@@ AS integer)", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestEnterpriseDbDbJsonDialectGetExtractFormatForBoolean()
        {
            // Act
            var result = new EnterpriseDbDbJsonDialect().GetExtractFormat(typeof(bool));

            // Assert
            Assert.AreEqual("CAST({0} #>> @@path@@ AS boolean)", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestEnterpriseDbDbJsonDialectGetExtractFormatForString()
        {
            // Act
            var result = new EnterpriseDbDbJsonDialect().GetExtractFormat(typeof(string));

            // Assert
            Assert.AreEqual("({0} #>> @@path@@)", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestEnterpriseDbDbJsonDialectGetExtractFormatWithoutType()
        {
            // Act
            var result = new EnterpriseDbDbJsonDialect().GetExtractFormat(null);

            // Assert
            Assert.AreEqual("({0} #>> @@path@@)", result, StringComparer.Ordinal);
        }

        #endregion
    }
}
