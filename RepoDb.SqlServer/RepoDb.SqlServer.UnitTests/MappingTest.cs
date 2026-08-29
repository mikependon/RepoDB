#region Copyright Attributions

// Copyright (c) 2019 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RepoDb.SqlServer.UnitTests
{
    [TestClass]
    public class MappingTest
    {
        [TestInitialize]
        public void Initialize()
        {
            GlobalConfiguration
                .Setup()
                .UseSqlServer();
        }

        [TestMethod]
        public void TestSqlServerStatementBuilderMapper()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SqlConnection>();

            // Assert
            Assert.IsNotNull(builder);
        }

        [TestMethod]
        public void TestSqlServerDbHelperMapper()
        {
            // Setup
            var helper = DbHelperMapper.Get<SqlConnection>();

            // Assert
            Assert.IsNotNull(helper);
        }

        [TestMethod]
        public void TestSqlServerDbSettingMapper()
        {
            // Setup
            var setting = DbSettingMapper.Get<SqlConnection>();

            // Assert
            Assert.IsNotNull(setting);
        }
    }
}
