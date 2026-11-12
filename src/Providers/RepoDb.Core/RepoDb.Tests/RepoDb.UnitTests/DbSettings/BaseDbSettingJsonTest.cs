#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.UnitTests.CustomObjects;
using System;
using System.Data;

namespace RepoDb.UnitTests.DbSettings
{
    [TestClass]
    public class BaseDbSettingJsonTest
    {
        #region ConfigureJsonParameter

        [TestMethod]
        public void TestBaseDbSettingConfigureJsonParameter()
        {
            // Setup
            var parameter = new CustomDbParameter { DbType = DbType.Int32 };

            // Act
            new CustomDbSetting().ConfigureJsonParameter(parameter);

            // Assert
            Assert.AreEqual(DbType.String, parameter.DbType);
        }

        #endregion

        #region FormatJsonPath

        [TestMethod]
        public void TestBaseDbSettingFormatJsonPath()
        {
            // Act
            var path = new CustomDbSetting().FormatJsonPath(new[] { "Address", "City" });

            // Assert
            Assert.AreEqual("'$.Address.City'", path, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestBaseDbSettingFormatJsonPathWithArrayIndex()
        {
            // Act
            var path = new CustomDbSetting().FormatJsonPath(new[] { "Tags", "[0]" });

            // Assert
            Assert.AreEqual("'$.Tags[0]'", path, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestBaseDbSettingFormatJsonPathWithSingleQuote()
        {
            // Act
            var path = new CustomDbSetting().FormatJsonPath(new[] { "o'k" });

            // Assert
            Assert.AreEqual("'$.\"o''k\"'", path, StringComparer.Ordinal);
        }

        #endregion

        #region GetJsonExtractFormat

        [TestMethod]
        public void TestBaseDbSettingGetJsonExtractFormat()
        {
            // Act
            var format = new CustomDbSetting().GetJsonExtractFormat(typeof(int));

            // Assert
            Assert.AreEqual("JSON_EXTRACT({0}, @@path@@)", format, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestBaseDbSettingGetJsonExtractFormatWithoutType()
        {
            // Act
            var format = new CustomDbSetting().GetJsonExtractFormat(null);

            // Assert
            Assert.AreEqual("JSON_EXTRACT({0}, @@path@@)", format, StringComparer.Ordinal);
        }

        #endregion
    }
}
