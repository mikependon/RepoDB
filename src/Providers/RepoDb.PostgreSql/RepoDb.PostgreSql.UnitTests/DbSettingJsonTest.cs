#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Npgsql;
using NpgsqlTypes;
using RepoDb.DbSettings;
using System;

namespace RepoDb.PostgreSql.UnitTests
{
    [TestClass]
    public class DbSettingJsonTest
    {
        #region ConfigureJsonParameter

        [TestMethod]
        public void TestPostgreSqlDbSettingConfigureJsonParameter()
        {
            // Setup
            var parameter = new NpgsqlParameter();

            // Act
            new PostgreSqlDbSetting().ConfigureJsonParameter(parameter);

            // Assert
            Assert.AreEqual(NpgsqlDbType.Jsonb, parameter.NpgsqlDbType);
        }

        #endregion

        #region FormatJsonPath

        [TestMethod]
        public void TestPostgreSqlDbSettingFormatJsonPath()
        {
            // Act
            var path = new PostgreSqlDbSetting().FormatJsonPath(new[] { "Address", "City" });

            // Assert
            Assert.AreEqual("'{Address,City}'", path, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestPostgreSqlDbSettingFormatJsonPathWithArrayIndex()
        {
            // Act
            var path = new PostgreSqlDbSetting().FormatJsonPath(new[] { "Tags", "[0]" });

            // Assert
            Assert.AreEqual("'{Tags,0}'", path, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestPostgreSqlDbSettingFormatJsonPathWithSpecialName()
        {
            // Act
            var path = new PostgreSqlDbSetting().FormatJsonPath(new[] { "display-name" });

            // Assert
            Assert.AreEqual("'{\"display-name\"}'", path, StringComparer.Ordinal);
        }

        #endregion

        #region GetJsonExtractFormat

        [TestMethod]
        public void TestPostgreSqlDbSettingGetJsonExtractFormatForInteger()
        {
            // Act
            var format = new PostgreSqlDbSetting().GetJsonExtractFormat(typeof(int));

            // Assert
            Assert.AreEqual("CAST({0} #>> @@path@@ AS integer)", format, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestPostgreSqlDbSettingGetJsonExtractFormatForNullableInteger()
        {
            // Act
            var format = new PostgreSqlDbSetting().GetJsonExtractFormat(typeof(int?));

            // Assert
            Assert.AreEqual("CAST({0} #>> @@path@@ AS integer)", format, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestPostgreSqlDbSettingGetJsonExtractFormatForBoolean()
        {
            // Act
            var format = new PostgreSqlDbSetting().GetJsonExtractFormat(typeof(bool));

            // Assert
            Assert.AreEqual("CAST({0} #>> @@path@@ AS boolean)", format, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestPostgreSqlDbSettingGetJsonExtractFormatForString()
        {
            // Act
            var format = new PostgreSqlDbSetting().GetJsonExtractFormat(typeof(string));

            // Assert
            Assert.AreEqual("({0} #>> @@path@@)", format, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestPostgreSqlDbSettingGetJsonExtractFormatWithoutType()
        {
            // Act
            var format = new PostgreSqlDbSetting().GetJsonExtractFormat(null);

            // Assert
            Assert.AreEqual("({0} #>> @@path@@)", format, StringComparer.Ordinal);
        }

        #endregion
    }
}
