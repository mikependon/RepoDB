#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using ClickHouse.Driver.ADO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.DbSettings;
using RepoDb.StatementBuilders;

namespace RepoDb.ClickHouse.UnitTests
{
    [TestClass]
    [DoNotParallelize]
    public class GlobalConfigurationTest
    {
        #region SubClasses

        /// <summary>
        /// A setting that inherits the <see cref="ClickHouseDbSetting"/>, as the extension libraries do.
        /// </summary>
        private sealed class CustomClickHouseDbSetting : ClickHouseDbSetting
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
                .UseClickHouse(new ClickHouseDbSetting());
        }

        [TestMethod]
        public void TestUseClickHouseRegistersTheDefaultSetting()
        {
            // Act
            GlobalConfiguration
                .Setup()
                .UseClickHouse();

            // Assert
            Assert.IsTrue(ClickHouseBootstrap.IsInitialized);
            Assert.IsInstanceOfType<ClickHouseDbSetting>(DbSettingMapper.Get<ClickHouseConnection>());
        }

        [TestMethod]
        public void TestUseClickHouseWithSettingRegistersTheGivenSetting()
        {
            // Setup
            var setting = new CustomClickHouseDbSetting { CustomValue = "Custom" };

            // Act
            var result = GlobalConfiguration
                .Setup()
                .UseClickHouse(setting);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(ClickHouseBootstrap.IsInitialized);
            Assert.AreSame(setting, DbSettingMapper.Get<ClickHouseConnection>());
            Assert.IsInstanceOfType<ClickHouseStatementBuilder>(StatementBuilderMapper.Get<ClickHouseConnection>());
        }

        [TestMethod]
        public void TestUseClickHouseWithSettingReplacesAnEarlierInitialization()
        {
            // Setup
            GlobalConfiguration
                .Setup()
                .UseClickHouse();
            var setting = new CustomClickHouseDbSetting();

            // Act
            GlobalConfiguration
                .Setup()
                .UseClickHouse(setting);

            // Assert
            Assert.AreSame(setting, DbSettingMapper.Get<ClickHouseConnection>());
        }

        [TestMethod]
        public void TestUseClickHouseAfterUseClickHouseWithSettingKeepsTheGivenSetting()
        {
            // Setup
            var setting = new CustomClickHouseDbSetting();
            GlobalConfiguration
                .Setup()
                .UseClickHouse(setting);

            // Act (the parameterless call is skipped, as ClickHouse is already initialized)
            GlobalConfiguration
                .Setup()
                .UseClickHouse();

            // Assert
            Assert.AreSame(setting, DbSettingMapper.Get<ClickHouseConnection>());
        }

        [TestMethod]
        public void TestUseClickHouseWithSettingKeepsTheInheritedValues()
        {
            // Setup
            var expected = new ClickHouseDbSetting();

            // Act
            GlobalConfiguration
                .Setup()
                .UseClickHouse(new CustomClickHouseDbSetting());
            var setting = DbSettingMapper.Get<ClickHouseConnection>();

            // Assert
            Assert.AreEqual(expected.OpeningQuote, setting.OpeningQuote);
            Assert.AreEqual(expected.ClosingQuote, setting.ClosingQuote);
            Assert.AreEqual(expected.DefaultSchema, setting.DefaultSchema);
            Assert.AreEqual(expected.ParameterPrefix, setting.ParameterPrefix);
        }
    }
}
