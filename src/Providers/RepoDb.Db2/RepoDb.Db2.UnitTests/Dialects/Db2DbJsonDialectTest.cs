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
    public class Db2DbJsonDialectTest
    {
        #region GetExtractFormat

        [TestMethod]
        public void TestDb2DbJsonDialectGetExtractFormatForInteger()
        {
            // Act
            var format = new Db2DbJsonDialect().GetExtractFormat(typeof(int));

            // Assert
            Assert.AreEqual("JSON_VALUE({0}, @@path@@ RETURNING INTEGER)", format, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDb2DbJsonDialectGetExtractFormatForNullableLong()
        {
            // Act
            var format = new Db2DbJsonDialect().GetExtractFormat(typeof(long?));

            // Assert
            Assert.AreEqual("JSON_VALUE({0}, @@path@@ RETURNING BIGINT)", format, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDb2DbJsonDialectGetExtractFormatForDecimal()
        {
            // Act
            var format = new Db2DbJsonDialect().GetExtractFormat(typeof(decimal));

            // Assert
            Assert.AreEqual("JSON_VALUE({0}, @@path@@ RETURNING DECIMAL(31,10))", format, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDb2DbJsonDialectGetExtractFormatForString()
        {
            // Act
            var format = new Db2DbJsonDialect().GetExtractFormat(typeof(string));

            // Assert
            Assert.AreEqual("JSON_VALUE({0}, @@path@@)", format, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDb2DbJsonDialectGetExtractFormatWithoutType()
        {
            // Act
            var format = new Db2DbJsonDialect().GetExtractFormat(null);

            // Assert
            Assert.AreEqual("JSON_VALUE({0}, @@path@@)", format, StringComparer.Ordinal);
        }

        #endregion
    }
}
