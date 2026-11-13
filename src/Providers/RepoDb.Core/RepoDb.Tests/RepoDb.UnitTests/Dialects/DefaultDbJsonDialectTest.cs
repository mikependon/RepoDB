#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Dialects;
using RepoDb.UnitTests.CustomObjects;
using System;
using System.Data;

namespace RepoDb.UnitTests.Dialects
{
    [TestClass]
    public class DefaultDbJsonDialectTest
    {
        #region Instance

        [TestMethod]
        public void TestDefaultDbJsonDialectInstance()
        {
            // Act
            var instance = DefaultDbJsonDialect.Instance;

            // Assert
            Assert.IsNotNull(instance);
        }

        [TestMethod]
        public void TestDefaultDbJsonDialectInstanceIsShared()
        {
            // Act
            var same = ReferenceEquals(DefaultDbJsonDialect.Instance, DefaultDbJsonDialect.Instance);

            // Assert
            Assert.IsTrue(same);
        }

        #endregion

        #region ConfigureParameter

        [TestMethod]
        public void TestDefaultDbJsonDialectConfigureParameter()
        {
            // Setup
            var parameter = new CustomDbParameter { DbType = DbType.Int32 };

            // Act
            new DefaultDbJsonDialect().ConfigureParameter(parameter);

            // Assert
            Assert.AreEqual(DbType.String, parameter.DbType);
        }

        #endregion

        #region FormatPath

        [TestMethod]
        public void TestDefaultDbJsonDialectFormatPath()
        {
            // Act
            var path = new DefaultDbJsonDialect().FormatPath(new[] { "Address", "City" });

            // Assert
            Assert.AreEqual("'$.Address.City'", path, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDefaultDbJsonDialectFormatPathWithArrayIndex()
        {
            // Act
            var path = new DefaultDbJsonDialect().FormatPath(new[] { "Tags", "[0]" });

            // Assert
            Assert.AreEqual("'$.Tags[0]'", path, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDefaultDbJsonDialectFormatPathWithSingleQuote()
        {
            // Act
            var path = new DefaultDbJsonDialect().FormatPath(new[] { "o'k" });

            // Assert
            Assert.AreEqual("'$.\"o''k\"'", path, StringComparer.Ordinal);
        }

        #endregion

        #region GetExtractFormat

        [TestMethod]
        public void TestDefaultDbJsonDialectGetExtractFormat()
        {
            // Act
            var format = new DefaultDbJsonDialect().GetExtractFormat(typeof(int));

            // Assert
            Assert.AreEqual("JSON_EXTRACT({0}, @@path@@)", format, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDefaultDbJsonDialectGetExtractFormatWithoutType()
        {
            // Act
            var format = new DefaultDbJsonDialect().GetExtractFormat(null);

            // Assert
            Assert.AreEqual("JSON_EXTRACT({0}, @@path@@)", format, StringComparer.Ordinal);
        }

        #endregion
    }
}
