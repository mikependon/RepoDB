#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Connector.CockroachDb;

namespace RepoDb.CockroachDb.UnitTests
{
    [TestClass]
    public class MappingTest
    {
        [TestInitialize]
        public void Initialize()
        {
            GlobalConfiguration
                .Setup()
                .UseCockroachDb();
        }

        [TestMethod]
        public void TestCockroachDbStatementBuilderMapper()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<CockroachDbConnection>();

            // Assert
            Assert.IsNotNull(builder);
        }

        [TestMethod]
        public void TestCockroachDbDbHelperMapper()
        {
            // Setup
            var helper = DbHelperMapper.Get<CockroachDbConnection>();

            // Assert
            Assert.IsNotNull(helper);
        }

        [TestMethod]
        public void TestCockroachDbDbSettingMapper()
        {
            // Setup
            var setting = DbSettingMapper.Get<CockroachDbConnection>();

            // Assert
            Assert.IsNotNull(setting);
        }
    }
}
