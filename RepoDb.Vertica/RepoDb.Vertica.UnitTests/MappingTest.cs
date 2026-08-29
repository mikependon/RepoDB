#region Copyright Attributions

// Copyright (c) 2019 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vertica.Data.VerticaClient;

namespace RepoDb.Vertica.UnitTests
{
    [TestClass]
    public class MappingTest
    {
        [TestInitialize]
        public void Initialize()
        {
            GlobalConfiguration
                .Setup()
                .UseVertica();
        }

        [TestMethod]
        public void TestVerticaStatementBuilderMapper()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<VerticaConnection>();

            // Assert
            Assert.IsNotNull(builder);
        }

        [TestMethod]
        public void TestVerticaDbHelperMapper()
        {
            // Setup
            var helper = DbHelperMapper.Get<VerticaConnection>();

            // Assert
            Assert.IsNotNull(helper);
        }

        [TestMethod]
        public void TestVerticaDbSettingMapper()
        {
            // Setup
            var setting = DbSettingMapper.Get<VerticaConnection>();

            // Assert
            Assert.IsNotNull(setting);
        }
    }
}
