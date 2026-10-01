#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using RepoDb.Connector.AuroraDb.Npgsql;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.DbSettings;
using RepoDb.StatementBuilders;
using System;

namespace RepoDb.AuroraDb.PostgreSql.UnitTests
{
    [TestClass]
    [DoNotParallelize]
    public class GlobalConfigurationTest
    {
        #region SubClasses

        /// <summary>
        /// A setting that inherits the <see cref="AuroraDbDbSetting"/>, as the extension libraries do.
        /// </summary>
        private sealed class CustomAuroraDbDbSetting : AuroraDbDbSetting
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
                .UseAuroraDb(new AuroraDbDbSetting());
        }

        [TestMethod]
        public void TestUseAuroraDbRegistersTheDefaultSetting()
        {
            // Act
            GlobalConfiguration
                .Setup()
                .UseAuroraDb();

            // Assert
            Assert.IsTrue(AuroraDbBootstrap.IsInitialized);
            Assert.IsInstanceOfType<AuroraDbDbSetting>(DbSettingMapper.Get<AuroraDbConnection>());
        }

        [TestMethod]
        public void TestUseAuroraDbWithSettingRegistersTheGivenSetting()
        {
            // Setup
            var setting = new CustomAuroraDbDbSetting { CustomValue = "Custom" };

            // Act
            var result = GlobalConfiguration
                .Setup()
                .UseAuroraDb(setting);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(AuroraDbBootstrap.IsInitialized);
            Assert.AreSame(setting, DbSettingMapper.Get<AuroraDbConnection>());
            Assert.IsInstanceOfType<AuroraDbStatementBuilder>(StatementBuilderMapper.Get<AuroraDbConnection>());
        }

        [TestMethod]
        public void TestUseAuroraDbWithSettingReplacesAnEarlierInitialization()
        {
            // Setup
            GlobalConfiguration
                .Setup()
                .UseAuroraDb();
            var setting = new CustomAuroraDbDbSetting();

            // Act
            GlobalConfiguration
                .Setup()
                .UseAuroraDb(setting);

            // Assert
            Assert.AreSame(setting, DbSettingMapper.Get<AuroraDbConnection>());
        }

        [TestMethod]
        public void TestUseAuroraDbAfterUseAuroraDbWithSettingKeepsTheGivenSetting()
        {
            // Setup
            var setting = new CustomAuroraDbDbSetting();
            GlobalConfiguration
                .Setup()
                .UseAuroraDb(setting);

            // Act (the parameterless call is skipped, as AuroraDB is already initialized)
            GlobalConfiguration
                .Setup()
                .UseAuroraDb();

            // Assert
            Assert.AreSame(setting, DbSettingMapper.Get<AuroraDbConnection>());
        }

        [TestMethod]
        public void TestUseAuroraDbWithSettingKeepsTheInheritedValues()
        {
            // Setup
            var expected = new AuroraDbDbSetting();

            // Act
            GlobalConfiguration
                .Setup()
                .UseAuroraDb(new CustomAuroraDbDbSetting());
            var setting = DbSettingMapper.Get<AuroraDbConnection>();

            // Assert
            Assert.AreEqual(expected.OpeningQuote, setting.OpeningQuote);
            Assert.AreEqual(expected.ClosingQuote, setting.ClosingQuote);
            Assert.AreEqual(expected.DefaultSchema, setting.DefaultSchema);
            Assert.AreEqual(expected.ParameterPrefix, setting.ParameterPrefix);
        }

        [TestMethod]
        public void ThrowExceptionOnUseAuroraDbIfTheSettingIsNull()
        {
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() =>
                GlobalConfiguration
                    .Setup()
                    .UseAuroraDb((AuroraDbDbSetting)null));
        }
    }
}
