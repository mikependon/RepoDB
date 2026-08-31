#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sap.Data.Hana;

namespace RepoDb.SapHana.UnitTests
{
    [TestClass]
    public class MappingTest
    {
        [TestInitialize]
        public void Initialize()
        {
            GlobalConfiguration
                .Setup()
                .UseSapHana();
        }

        [TestMethod]
        public void TestSapHanaStatementBuilderMapper()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<HanaConnection>();

            // Assert
            Assert.IsNotNull(builder);
        }

        [TestMethod]
        public void TestSapHanaDbHelperMapper()
        {
            // Setup
            var helper = DbHelperMapper.Get<HanaConnection>();

            // Assert
            Assert.IsNotNull(helper);
        }

        [TestMethod]
        public void TestSapHanaDbSettingMapper()
        {
            // Setup
            var setting = DbSettingMapper.Get<HanaConnection>();

            // Assert
            Assert.IsNotNull(setting);
        }
    }
}
