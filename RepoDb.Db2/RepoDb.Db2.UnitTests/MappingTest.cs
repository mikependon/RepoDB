#region Copyright Attributions

// Copyright (c) 2019 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using IBM.Data.Db2;

namespace RepoDb.Db2.UnitTests
{
    [TestClass]
    public class MappingTest
    {
        [TestInitialize]
        public void Initialize()
        {
            GlobalConfiguration
                .Setup()
                .UseDb2();
        }

        [TestMethod]
        public void TestDb2StatementBuilderMapper()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<DB2Connection>();

            // Assert
            Assert.IsNotNull(builder);
        }

        [TestMethod]
        public void TestDb2DbHelperMapper()
        {
            // Setup
            var helper = DbHelperMapper.Get<DB2Connection>();

            // Assert
            Assert.IsNotNull(helper);
        }

        [TestMethod]
        public void TestDb2DbSettingMapper()
        {
            // Setup
            var setting = DbSettingMapper.Get<DB2Connection>();

            // Assert
            Assert.IsNotNull(setting);
        }
    }
}
