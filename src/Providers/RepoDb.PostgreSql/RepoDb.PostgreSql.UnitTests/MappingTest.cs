#region Copyright Attributions

// Copyright (c) 2019 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Npgsql;

namespace RepoDb.PostgreSql.UnitTests
{
    [TestClass]
    public class MappingTest
    {
        [TestInitialize]
        public void Initialize()
        {
            GlobalConfiguration
                .Setup()
                .UsePostgreSql();
        }

        [TestMethod]
        public void TestPostgreSqlStatementBuilderMapper()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<NpgsqlConnection>();

            // Assert
            Assert.IsNotNull(builder);
        }

        [TestMethod]
        public void TestPostgreSqlDbHelperMapper()
        {
            // Setup
            var helper = DbHelperMapper.Get<NpgsqlConnection>();

            // Assert
            Assert.IsNotNull(helper);
        }

        [TestMethod]
        public void TestPostgreSqlDbSettingMapper()
        {
            // Setup
            var setting = DbSettingMapper.Get<NpgsqlConnection>();

            // Assert
            Assert.IsNotNull(setting);
        }
    }
}
