#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Npgsql;
using NpgsqlTypes;
using RepoDb.DbSettings;
using RepoDb.Dialects;
using System;

namespace RepoDb.PostgreSql.UnitTests.Dialects
{
    [TestClass]
    public class PostgreSqlDbJsonDialectTest
    {
        #region Mapping

        [TestMethod]
        public void TestPostgreSqlDbJsonDialectIsMappedByTheBootstrap()
        {
            // Setup
            GlobalConfiguration
                .Setup()
                .UsePostgreSql();

            // Act
            var dialect = DbJsonDialectMapper.Get(new PostgreSqlDbSetting());

            // Assert
            Assert.IsInstanceOfType<PostgreSqlDbJsonDialect>(dialect);
        }

        #endregion

        #region ConfigureParameter

        [TestMethod]
        public void TestPostgreSqlDbJsonDialectConfigureParameter()
        {
            // Setup
            var parameter = new NpgsqlParameter();

            // Act
            new PostgreSqlDbJsonDialect().ConfigureParameter(parameter);

            // Assert
            Assert.AreEqual(NpgsqlDbType.Jsonb, parameter.NpgsqlDbType);
        }

        #endregion

        #region FormatPath

        [TestMethod]
        public void TestPostgreSqlDbJsonDialectFormatPath()
        {
            // Act
            var path = new PostgreSqlDbJsonDialect().FormatPath(new[] { "Address", "City" });

            // Assert
            Assert.AreEqual("'{Address,City}'", path, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestPostgreSqlDbJsonDialectFormatPathWithArrayIndex()
        {
            // Act
            var path = new PostgreSqlDbJsonDialect().FormatPath(new[] { "Tags", "[0]" });

            // Assert
            Assert.AreEqual("'{Tags,0}'", path, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestPostgreSqlDbJsonDialectFormatPathWithSpecialName()
        {
            // Act
            var path = new PostgreSqlDbJsonDialect().FormatPath(new[] { "display-name" });

            // Assert
            Assert.AreEqual("'{\"display-name\"}'", path, StringComparer.Ordinal);
        }

        #endregion

        #region GetExtractFormat

        [TestMethod]
        public void TestPostgreSqlDbJsonDialectGetExtractFormatForInteger()
        {
            // Act
            var format = new PostgreSqlDbJsonDialect().GetExtractFormat(typeof(int));

            // Assert
            Assert.AreEqual("CAST({0} #>> @@path@@ AS integer)", format, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestPostgreSqlDbJsonDialectGetExtractFormatForNullableInteger()
        {
            // Act
            var format = new PostgreSqlDbJsonDialect().GetExtractFormat(typeof(int?));

            // Assert
            Assert.AreEqual("CAST({0} #>> @@path@@ AS integer)", format, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestPostgreSqlDbJsonDialectGetExtractFormatForBoolean()
        {
            // Act
            var format = new PostgreSqlDbJsonDialect().GetExtractFormat(typeof(bool));

            // Assert
            Assert.AreEqual("CAST({0} #>> @@path@@ AS boolean)", format, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestPostgreSqlDbJsonDialectGetExtractFormatForString()
        {
            // Act
            var format = new PostgreSqlDbJsonDialect().GetExtractFormat(typeof(string));

            // Assert
            Assert.AreEqual("({0} #>> @@path@@)", format, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestPostgreSqlDbJsonDialectGetExtractFormatWithoutType()
        {
            // Act
            var format = new PostgreSqlDbJsonDialect().GetExtractFormat(null);

            // Assert
            Assert.AreEqual("({0} #>> @@path@@)", format, StringComparer.Ordinal);
        }

        #endregion
    }
}
