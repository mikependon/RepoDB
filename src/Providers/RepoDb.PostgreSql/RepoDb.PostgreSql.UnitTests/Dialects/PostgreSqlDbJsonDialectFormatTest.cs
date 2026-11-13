#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Dialects;
using System;

namespace RepoDb.PostgreSql.UnitTests.Dialects
{
    [TestClass]
    public class PostgreSqlDbJsonDialectFormatTest
    {
        #region FormatPath

        [TestMethod]
        public void TestPostgreSqlDbJsonDialectFormatPath()
        {
            // Act
            var result = new PostgreSqlDbJsonDialect().FormatPath(new[] { "Address", "City" });

            // Assert
            Assert.AreEqual("'{Address,City}'", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestPostgreSqlDbJsonDialectFormatPathWithArrayIndex()
        {
            // Act
            var result = new PostgreSqlDbJsonDialect().FormatPath(new[] { "Tags", "[0]" });

            // Assert
            Assert.AreEqual("'{Tags,0}'", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestPostgreSqlDbJsonDialectFormatPathWithSpecialName()
        {
            // Act
            var result = new PostgreSqlDbJsonDialect().FormatPath(new[] { "display-name" });

            // Assert
            Assert.AreEqual("'{\"display-name\"}'", result, StringComparer.Ordinal);
        }

        #endregion

        #region GetExtractFormat

        [TestMethod]
        public void TestPostgreSqlDbJsonDialectGetExtractFormatForInteger()
        {
            // Act
            var result = new PostgreSqlDbJsonDialect().GetExtractFormat(typeof(int));

            // Assert
            Assert.AreEqual("CAST({0} #>> @@path@@ AS integer)", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestPostgreSqlDbJsonDialectGetExtractFormatForNullableInteger()
        {
            // Act
            var result = new PostgreSqlDbJsonDialect().GetExtractFormat(typeof(int?));

            // Assert
            Assert.AreEqual("CAST({0} #>> @@path@@ AS integer)", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestPostgreSqlDbJsonDialectGetExtractFormatForBoolean()
        {
            // Act
            var result = new PostgreSqlDbJsonDialect().GetExtractFormat(typeof(bool));

            // Assert
            Assert.AreEqual("CAST({0} #>> @@path@@ AS boolean)", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestPostgreSqlDbJsonDialectGetExtractFormatForString()
        {
            // Act
            var result = new PostgreSqlDbJsonDialect().GetExtractFormat(typeof(string));

            // Assert
            Assert.AreEqual("({0} #>> @@path@@)", result, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestPostgreSqlDbJsonDialectGetExtractFormatWithoutType()
        {
            // Act
            var result = new PostgreSqlDbJsonDialect().GetExtractFormat(null);

            // Assert
            Assert.AreEqual("({0} #>> @@path@@)", result, StringComparer.Ordinal);
        }

        #endregion
    }
}
