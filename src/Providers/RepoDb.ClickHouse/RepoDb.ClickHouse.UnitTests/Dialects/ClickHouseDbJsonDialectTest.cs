#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Dialects;
using System;

namespace RepoDb.ClickHouse.UnitTests.Dialects
{
    [TestClass]
    public class ClickHouseDbJsonDialectTest
    {
        #region FormatPath

        [TestMethod]
        public void TestClickHouseDbJsonDialectFormatPath()
        {
            // Act
            var result = new ClickHouseDbJsonDialect().FormatPath(new[] { "Address", "City" });

            // Assert
            Assert.AreEqual("'Address','City'", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClickHouseDbJsonDialectFormatPathWithArrayIndex()
        {
            // Act
            var result = new ClickHouseDbJsonDialect().FormatPath(new[] { "Tags", "[0]" });

            // Assert
            Assert.AreEqual("'Tags',1", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClickHouseDbJsonDialectFormatPathWithSingleQuote()
        {
            // Act
            var result = new ClickHouseDbJsonDialect().FormatPath(new[] { "o'k" });

            // Assert
            Assert.AreEqual("'o''k'", result, StringComparer.Ordinal);
        }

        #endregion

        #region GetExtractFormat

        [TestMethod]
        public void TestClickHouseDbJsonDialectGetExtractFormatForInteger()
        {
            // Act
            var result = new ClickHouseDbJsonDialect().GetExtractFormat(typeof(int));

            // Assert
            Assert.AreEqual("JSONExtractInt({0}, @@path@@)", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClickHouseDbJsonDialectGetExtractFormatForNullableLong()
        {
            // Act
            var result = new ClickHouseDbJsonDialect().GetExtractFormat(typeof(long?));

            // Assert
            Assert.AreEqual("JSONExtractInt({0}, @@path@@)", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClickHouseDbJsonDialectGetExtractFormatForUnsigned()
        {
            // Act
            var result = new ClickHouseDbJsonDialect().GetExtractFormat(typeof(uint));

            // Assert
            Assert.AreEqual("JSONExtractUInt({0}, @@path@@)", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClickHouseDbJsonDialectGetExtractFormatForDouble()
        {
            // Act
            var result = new ClickHouseDbJsonDialect().GetExtractFormat(typeof(double));

            // Assert
            Assert.AreEqual("JSONExtractFloat({0}, @@path@@)", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClickHouseDbJsonDialectGetExtractFormatForBoolean()
        {
            // Act
            var result = new ClickHouseDbJsonDialect().GetExtractFormat(typeof(bool));

            // Assert
            Assert.AreEqual("JSONExtractBool({0}, @@path@@)", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClickHouseDbJsonDialectGetExtractFormatForString()
        {
            // Act
            var result = new ClickHouseDbJsonDialect().GetExtractFormat(typeof(string));

            // Assert
            Assert.AreEqual("JSONExtractString({0}, @@path@@)", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClickHouseDbJsonDialectGetExtractFormatWithoutType()
        {
            // Act
            var result = new ClickHouseDbJsonDialect().GetExtractFormat(null);

            // Assert
            Assert.AreEqual("JSONExtractString({0}, @@path@@)", result, StringComparer.Ordinal);
        }

        #endregion
    }
}
