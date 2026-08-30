#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Oracle.ManagedDataAccess.Client;

namespace RepoDb.Oracle.UnitTests
{
    [TestClass]
    public class MappingTest
    {
        [TestInitialize]
        public void Initialize()
        {
            GlobalConfiguration
                .Setup()
                .UseOracle();
        }

        [TestMethod]
        public void TestOracleStatementBuilderMapper()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<OracleConnection>();

            // Assert
            Assert.IsNotNull(builder);
        }

        [TestMethod]
        public void TestOracleDbHelperMapper()
        {
            // Setup
            var helper = DbHelperMapper.Get<OracleConnection>();

            // Assert
            Assert.IsNotNull(helper);
        }

        [TestMethod]
        public void TestOracleDbSettingMapper()
        {
            // Setup
            var setting = DbSettingMapper.Get<OracleConnection>();

            // Assert
            Assert.IsNotNull(setting);
        }
    }
}
