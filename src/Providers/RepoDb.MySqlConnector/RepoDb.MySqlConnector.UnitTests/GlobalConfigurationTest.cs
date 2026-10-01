#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using MySqlConnector;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.DbSettings;
using RepoDb.StatementBuilders;
using System;

namespace RepoDb.MySqlConnector.UnitTests
{
    [TestClass]
    [DoNotParallelize]
    public class GlobalConfigurationTest
    {
        #region SubClasses

        /// <summary>
        /// A setting that inherits the <see cref="MySqlConnectorDbSetting"/>, as the extension libraries do.
        /// </summary>
        private sealed class CustomMySqlConnectorDbSetting : MySqlConnectorDbSetting
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
                .UseMySqlConnector(new MySqlConnectorDbSetting());
        }

        [TestMethod]
        public void TestUseMySqlConnectorRegistersTheDefaultSetting()
        {
            // Act
            GlobalConfiguration
                .Setup()
                .UseMySqlConnector();

            // Assert
            Assert.IsTrue(MySqlConnectorBootstrap.IsInitialized);
            Assert.IsInstanceOfType<MySqlConnectorDbSetting>(DbSettingMapper.Get<MySqlConnection>());
        }

        [TestMethod]
        public void TestUseMySqlConnectorWithSettingRegistersTheGivenSetting()
        {
            // Setup
            var setting = new CustomMySqlConnectorDbSetting { CustomValue = "Custom" };

            // Act
            var result = GlobalConfiguration
                .Setup()
                .UseMySqlConnector(setting);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(MySqlConnectorBootstrap.IsInitialized);
            Assert.AreSame(setting, DbSettingMapper.Get<MySqlConnection>());
            Assert.IsInstanceOfType<MySqlConnectorStatementBuilder>(StatementBuilderMapper.Get<MySqlConnection>());
        }

        [TestMethod]
        public void TestUseMySqlConnectorWithSettingReplacesAnEarlierInitialization()
        {
            // Setup
            GlobalConfiguration
                .Setup()
                .UseMySqlConnector();
            var setting = new CustomMySqlConnectorDbSetting();

            // Act
            GlobalConfiguration
                .Setup()
                .UseMySqlConnector(setting);

            // Assert
            Assert.AreSame(setting, DbSettingMapper.Get<MySqlConnection>());
        }

        [TestMethod]
        public void TestUseMySqlConnectorAfterUseMySqlConnectorWithSettingKeepsTheGivenSetting()
        {
            // Setup
            var setting = new CustomMySqlConnectorDbSetting();
            GlobalConfiguration
                .Setup()
                .UseMySqlConnector(setting);

            // Act (the parameterless call is skipped, as MySQL is already initialized)
            GlobalConfiguration
                .Setup()
                .UseMySqlConnector();

            // Assert
            Assert.AreSame(setting, DbSettingMapper.Get<MySqlConnection>());
        }

        [TestMethod]
        public void TestUseMySqlConnectorWithSettingKeepsTheInheritedValues()
        {
            // Setup
            var expected = new MySqlConnectorDbSetting();

            // Act
            GlobalConfiguration
                .Setup()
                .UseMySqlConnector(new CustomMySqlConnectorDbSetting());
            var setting = DbSettingMapper.Get<MySqlConnection>();

            // Assert
            Assert.AreEqual(expected.OpeningQuote, setting.OpeningQuote);
            Assert.AreEqual(expected.ClosingQuote, setting.ClosingQuote);
            Assert.AreEqual(expected.DefaultSchema, setting.DefaultSchema);
            Assert.AreEqual(expected.ParameterPrefix, setting.ParameterPrefix);
        }

        [TestMethod]
        public void ThrowExceptionOnUseMySqlConnectorIfTheSettingIsNull()
        {
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() =>
                GlobalConfiguration
                    .Setup()
                    .UseMySqlConnector((MySqlConnectorDbSetting)null));
        }
    }
}
