#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.DbSettings;
using RepoDb.Dialects;

namespace RepoDb.ClickHouse.UnitTests.Dialects
{
    [TestClass]
    public class DbJsonDialectMappingTest
    {
        #region Mapping

        [TestMethod]
        public void TestClickHouseDbJsonDialectIsMappedByTheBootstrap()
        {
            // Setup
            GlobalConfiguration
                .Setup()
                .UseClickHouse();

            // Act
            var dialect = DbJsonDialectMapper.Get(new ClickHouseDbSetting());

            // Assert
            Assert.IsInstanceOfType<ClickHouseDbJsonDialect>(dialect);
        }

        #endregion
    }
}
