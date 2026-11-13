#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.DbSettings;
using RepoDb.Dialects;

namespace RepoDb.AuroraDb.PostgreSql.UnitTests.Dialects
{
    [TestClass]
    public class DbJsonDialectMappingTest
    {
        #region Mapping

        [TestMethod]
        public void TestAuroraDbPostgreSqlDbJsonDialectIsMappedByTheBootstrap()
        {
            // Setup
            GlobalConfiguration
                .Setup()
                .UseAuroraDb();

            // Act
            var dialect = DbJsonDialectMapper.Get(new AuroraDbDbSetting());

            // Assert
            Assert.IsInstanceOfType<AuroraDbDbJsonDialect>(dialect);
        }

        #endregion
    }
}
