#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using RepoDb.Connector.MariaDb;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.DbSettings;
using RepoDb.StatementBuilders;
using System;

namespace RepoDb.MariaDb.UnitTests
{
    [TestClass]
    [DoNotParallelize]
    public class GlobalConfigurationTest
    {
        #region SubClasses

        /// <summary>
        /// A setting that inherits the <see cref="MariaDbDbSetting"/>, as the extension libraries do.
        /// </summary>
        private sealed class CustomMariaDbDbSetting : MariaDbDbSetting
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
                .UseMariaDb(new MariaDbDbSetting());
        }

        [TestMethod]
        public void TestUseMariaDbRegistersTheDefaultSetting()
        {
            // Act
            GlobalConfiguration
                .Setup()
                .UseMariaDb();

            // Assert
            Assert.IsTrue(MariaDbBootstrap.IsInitialized);
            Assert.IsInstanceOfType<MariaDbDbSetting>(DbSettingMapper.Get<MariaDbConnection>());
        }

        [TestMethod]
        public void TestUseMariaDbWithSettingRegistersTheGivenSetting()
        {
            // Setup
            var setting = new CustomMariaDbDbSetting { CustomValue = "Custom" };

            // Act
            var result = GlobalConfiguration
                .Setup()
                .UseMariaDb(setting);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(MariaDbBootstrap.IsInitialized);
            Assert.AreSame(setting, DbSettingMapper.Get<MariaDbConnection>());
            Assert.IsInstanceOfType<MariaDbStatementBuilder>(StatementBuilderMapper.Get<MariaDbConnection>());
        }

        [TestMethod]
        public void TestUseMariaDbWithSettingReplacesAnEarlierInitialization()
        {
            // Setup
            GlobalConfiguration
                .Setup()
                .UseMariaDb();
            var setting = new CustomMariaDbDbSetting();

            // Act
            GlobalConfiguration
                .Setup()
                .UseMariaDb(setting);

            // Assert
            Assert.AreSame(setting, DbSettingMapper.Get<MariaDbConnection>());
        }

        [TestMethod]
        public void TestUseMariaDbAfterUseMariaDbWithSettingKeepsTheGivenSetting()
        {
            // Setup
            var setting = new CustomMariaDbDbSetting();
            GlobalConfiguration
                .Setup()
                .UseMariaDb(setting);

            // Act (the parameterless call is skipped, as MariaDB is already initialized)
            GlobalConfiguration
                .Setup()
                .UseMariaDb();

            // Assert
            Assert.AreSame(setting, DbSettingMapper.Get<MariaDbConnection>());
        }

        [TestMethod]
        public void TestUseMariaDbWithSettingKeepsTheInheritedValues()
        {
            // Setup
            var expected = new MariaDbDbSetting();

            // Act
            GlobalConfiguration
                .Setup()
                .UseMariaDb(new CustomMariaDbDbSetting());
            var setting = DbSettingMapper.Get<MariaDbConnection>();

            // Assert
            Assert.AreEqual(expected.OpeningQuote, setting.OpeningQuote);
            Assert.AreEqual(expected.ClosingQuote, setting.ClosingQuote);
            Assert.AreEqual(expected.DefaultSchema, setting.DefaultSchema);
            Assert.AreEqual(expected.ParameterPrefix, setting.ParameterPrefix);
        }

        [TestMethod]
        public void ThrowExceptionOnUseMariaDbIfTheSettingIsNull()
        {
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() =>
                GlobalConfiguration
                    .Setup()
                    .UseMariaDb((MariaDbDbSetting)null));
        }
    }
}
