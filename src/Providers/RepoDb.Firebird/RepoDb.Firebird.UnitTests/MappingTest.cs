#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using FirebirdSql.Data.FirebirdClient;

namespace RepoDb.Firebird.UnitTests
{
    [TestClass]
    public class MappingTest
    {
        [TestInitialize]
        public void Initialize()
        {
            GlobalConfiguration
                .Setup()
                .UseFirebird();
        }

        [TestMethod]
        public void TestFirebirdStatementBuilderMapper()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<FbConnection>();

            // Assert
            Assert.IsNotNull(builder);
        }

        [TestMethod]
        public void TestFirebirdDbHelperMapper()
        {
            // Setup
            var helper = DbHelperMapper.Get<FbConnection>();

            // Assert
            Assert.IsNotNull(helper);
        }

        [TestMethod]
        public void TestFirebirdDbSettingMapper()
        {
            // Setup
            var setting = DbSettingMapper.Get<FbConnection>();

            // Assert
            Assert.IsNotNull(setting);
        }
    }
}
