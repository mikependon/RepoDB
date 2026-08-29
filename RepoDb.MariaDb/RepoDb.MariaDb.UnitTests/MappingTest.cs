#region Copyright Attributions

// Copyright (c) 2019 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Connector.MariaDb;

namespace RepoDb.MariaDb.UnitTests
{
    [TestClass]
    public class MappingTest
    {
        [TestInitialize]
        public void Initialize()
        {
            GlobalConfiguration
                .Setup()
                .UseMariaDb();
        }

        [TestMethod]
        public void TestMariaDbStatementBuilderMapper()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<MariaDbConnection>();

            // Assert
            Assert.IsNotNull(builder);
        }

        [TestMethod]
        public void TestMariaDbDbHelperMapper()
        {
            // Setup
            var helper = DbHelperMapper.Get<MariaDbConnection>();

            // Assert
            Assert.IsNotNull(helper);
        }

        [TestMethod]
        public void TestMariaDbDbSettingMapper()
        {
            // Setup
            var setting = DbSettingMapper.Get<MariaDbConnection>();

            // Assert
            Assert.IsNotNull(setting);
        }
    }
}
