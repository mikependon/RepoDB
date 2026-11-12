#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.DbSettings;
using System;

namespace RepoDb.SqlServer.UnitTests
{
    [TestClass]
    public class DbSettingJsonTest
    {
        #region GetJsonExtractFormat

        [TestMethod]
        public void TestSqlServerDbSettingGetJsonExtractFormat()
        {
            // Act
            var format = new SqlServerDbSetting().GetJsonExtractFormat(typeof(int));

            // Assert
            Assert.AreEqual("JSON_VALUE({0}, @@path@@)", format, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestSqlServerDbSettingGetJsonExtractFormatWithoutType()
        {
            // Act
            var format = new SqlServerDbSetting().GetJsonExtractFormat(null);

            // Assert
            Assert.AreEqual("JSON_VALUE({0}, @@path@@)", format, StringComparer.Ordinal);
        }

        #endregion

        #region FormatJsonPath

        [TestMethod]
        public void TestSqlServerDbSettingFormatJsonPath()
        {
            // Act
            var path = new SqlServerDbSetting().FormatJsonPath(new[] { "Address", "City" });

            // Assert
            Assert.AreEqual("'$.Address.City'", path, StringComparer.Ordinal);
        }

        #endregion
    }
}
