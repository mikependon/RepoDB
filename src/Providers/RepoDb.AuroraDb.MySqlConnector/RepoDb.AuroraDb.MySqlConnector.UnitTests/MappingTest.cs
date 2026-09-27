#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Connector.AuroraDb.MySqlConnector;

namespace RepoDb.AuroraDb.MySqlConnector.UnitTests
{
    [TestClass]
    public class MappingTest
    {
        [TestInitialize]
        public void Initialize()
        {
            GlobalConfiguration
                .Setup()
                .UseAuroraDb();
        }

        [TestMethod]
        public void TestAuroraDbStatementBuilderMapper()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<AuroraDbConnection>();

            // Assert
            Assert.IsNotNull(builder);
        }

        [TestMethod]
        public void TestAuroraDbDbHelperMapper()
        {
            // Setup
            var helper = DbHelperMapper.Get<AuroraDbConnection>();

            // Assert
            Assert.IsNotNull(helper);
        }

        [TestMethod]
        public void TestAuroraDbDbSettingMapper()
        {
            // Setup
            var setting = DbSettingMapper.Get<AuroraDbConnection>();

            // Assert
            Assert.IsNotNull(setting);
        }
    }
}
