#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using RepoDb.Connector.EnterpriseDb;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.DbSettings;
using RepoDb.StatementBuilders;
using System;

namespace RepoDb.EnterpriseDb.UnitTests
{
    [TestClass]
    [DoNotParallelize]
    public class GlobalConfigurationTest
    {
        #region SubClasses

        /// <summary>
        /// A setting that inherits the <see cref="EnterpriseDbDbSetting"/>, as the extension libraries do.
        /// </summary>
        private sealed class CustomEnterpriseDbDbSetting : EnterpriseDbDbSetting
        {
            public string CustomValue { get; set; }
        }

        #endregion

        [TestCleanup]
        public void Cleanup()
        {
            // The setting is global, so always revert to the default for the other test classes
            GlobalConfiguration
                .Setup()
                .UseEnterpriseDb(new EnterpriseDbDbSetting());
        }

        [TestMethod]
        public void TestUseEnterpriseDbRegistersTheDefaultSetting()
        {
            // Act
            GlobalConfiguration
                .Setup()
                .UseEnterpriseDb();

            // Assert
            Assert.IsTrue(EnterpriseDbBootstrap.IsInitialized);
            Assert.IsInstanceOfType<EnterpriseDbDbSetting>(DbSettingMapper.Get<EDBConnection>());
        }

        [TestMethod]
        public void TestUseEnterpriseDbWithSettingRegistersTheGivenSetting()
        {
            // Setup
            var setting = new CustomEnterpriseDbDbSetting { CustomValue = "Custom" };

            // Act
            var result = GlobalConfiguration
                .Setup()
                .UseEnterpriseDb(setting);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(EnterpriseDbBootstrap.IsInitialized);
            Assert.AreSame(setting, DbSettingMapper.Get<EDBConnection>());
            Assert.IsInstanceOfType<EnterpriseDbStatementBuilder>(StatementBuilderMapper.Get<EDBConnection>());
        }

        [TestMethod]
        public void TestUseEnterpriseDbWithSettingReplacesAnEarlierInitialization()
        {
            // Setup
            GlobalConfiguration
                .Setup()
                .UseEnterpriseDb();
            var setting = new CustomEnterpriseDbDbSetting();

            // Act
            GlobalConfiguration
                .Setup()
                .UseEnterpriseDb(setting);

            // Assert
            Assert.AreSame(setting, DbSettingMapper.Get<EDBConnection>());
        }

        [TestMethod]
        public void TestUseEnterpriseDbAfterUseEnterpriseDbWithSettingKeepsTheGivenSetting()
        {
            // Setup
            var setting = new CustomEnterpriseDbDbSetting();
            GlobalConfiguration
                .Setup()
                .UseEnterpriseDb(setting);

            // Act (the parameterless call is skipped, as EnterpriseDB is already initialized)
            GlobalConfiguration
                .Setup()
                .UseEnterpriseDb();

            // Assert
            Assert.AreSame(setting, DbSettingMapper.Get<EDBConnection>());
        }

        [TestMethod]
        public void TestUseEnterpriseDbWithSettingKeepsTheInheritedValues()
        {
            // Setup
            var expected = new EnterpriseDbDbSetting();

            // Act
            GlobalConfiguration
                .Setup()
                .UseEnterpriseDb(new CustomEnterpriseDbDbSetting());
            var setting = DbSettingMapper.Get<EDBConnection>();

            // Assert
            Assert.AreEqual(expected.OpeningQuote, setting.OpeningQuote);
            Assert.AreEqual(expected.ClosingQuote, setting.ClosingQuote);
            Assert.AreEqual(expected.DefaultSchema, setting.DefaultSchema);
            Assert.AreEqual(expected.ParameterPrefix, setting.ParameterPrefix);
        }

        [TestMethod]
        public void ThrowExceptionOnUseEnterpriseDbIfTheSettingIsNull()
        {
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() =>
                GlobalConfiguration
                    .Setup()
                    .UseEnterpriseDb((EnterpriseDbDbSetting)null));
        }
    }
}
