#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.DbSettings;
using RepoDb.Dialects;
using System;

namespace RepoDb.SqlServer.UnitTests.Dialects
{
    [TestClass]
    public class SqlServerDbJsonDialectTest
    {
        #region Mapping

        [TestMethod]
        public void TestSqlServerDbJsonDialectIsMappedByTheBootstrap()
        {
            // Setup
            GlobalConfiguration
                .Setup()
                .UseSqlServer();

            // Act
            var dialect = DbJsonDialectMapper.Get(new SqlServerDbSetting());

            // Assert
            Assert.IsInstanceOfType<SqlServerDbJsonDialect>(dialect);
        }

        #endregion

        #region GetExtractFormat

        [TestMethod]
        public void TestSqlServerDbJsonDialectGetExtractFormat()
        {
            // Act
            var format = new SqlServerDbJsonDialect().GetExtractFormat(typeof(int));

            // Assert
            Assert.AreEqual("JSON_VALUE({0}, @@path@@)", format, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestSqlServerDbJsonDialectGetExtractFormatWithoutType()
        {
            // Act
            var format = new SqlServerDbJsonDialect().GetExtractFormat(null);

            // Assert
            Assert.AreEqual("JSON_VALUE({0}, @@path@@)", format, StringComparer.Ordinal);
        }

        #endregion

        #region FormatPath

        [TestMethod]
        public void TestSqlServerDbJsonDialectFormatPath()
        {
            // Act
            var path = new SqlServerDbJsonDialect().FormatPath(new[] { "Address", "City" });

            // Assert
            Assert.AreEqual("'$.Address.City'", path, StringComparer.Ordinal);
        }

        #endregion
    }
}
