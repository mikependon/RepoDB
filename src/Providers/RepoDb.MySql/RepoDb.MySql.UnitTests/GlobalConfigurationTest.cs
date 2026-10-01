#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using MySql.Data.MySqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.DbSettings;
using RepoDb.StatementBuilders;
using System;

namespace RepoDb.MySql.UnitTests
{
    [TestClass]
    [DoNotParallelize]
    public class GlobalConfigurationTest
    {
        #region SubClasses

        /// <summary>
        /// A setting that inherits the <see cref="MySqlDbSetting"/>, as the extension libraries do.
        /// </summary>
        private sealed class CustomMySqlDbSetting : MySqlDbSetting
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
                .UseMySql(new MySqlDbSetting());
        }

        [TestMethod]
        public void TestUseMySqlRegistersTheDefaultSetting()
        {
            // Act
            GlobalConfiguration
                .Setup()
                .UseMySql();

            // Assert
            Assert.IsTrue(MySqlBootstrap.IsInitialized);
            Assert.IsInstanceOfType<MySqlDbSetting>(DbSettingMapper.Get<MySqlConnection>());
        }

        [TestMethod]
        public void TestUseMySqlWithSettingRegistersTheGivenSetting()
        {
            // Setup
            var setting = new CustomMySqlDbSetting { CustomValue = "Custom" };

            // Act
            var result = GlobalConfiguration
                .Setup()
                .UseMySql(setting);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(MySqlBootstrap.IsInitialized);
            Assert.AreSame(setting, DbSettingMapper.Get<MySqlConnection>());
            Assert.IsInstanceOfType<MySqlStatementBuilder>(StatementBuilderMapper.Get<MySqlConnection>());
        }

        [TestMethod]
        public void TestUseMySqlWithSettingReplacesAnEarlierInitialization()
        {
            // Setup
            GlobalConfiguration
                .Setup()
                .UseMySql();
            var setting = new CustomMySqlDbSetting();

            // Act
            GlobalConfiguration
                .Setup()
                .UseMySql(setting);

            // Assert
            Assert.AreSame(setting, DbSettingMapper.Get<MySqlConnection>());
        }

        [TestMethod]
        public void TestUseMySqlAfterUseMySqlWithSettingKeepsTheGivenSetting()
        {
            // Setup
            var setting = new CustomMySqlDbSetting();
            GlobalConfiguration
                .Setup()
                .UseMySql(setting);

            // Act (the parameterless call is skipped, as MySQL is already initialized)
            GlobalConfiguration
                .Setup()
                .UseMySql();

            // Assert
            Assert.AreSame(setting, DbSettingMapper.Get<MySqlConnection>());
        }

        [TestMethod]
        public void TestUseMySqlWithSettingKeepsTheInheritedValues()
        {
            // Setup
            var expected = new MySqlDbSetting();

            // Act
            GlobalConfiguration
                .Setup()
                .UseMySql(new CustomMySqlDbSetting());
            var setting = DbSettingMapper.Get<MySqlConnection>();

            // Assert
            Assert.AreEqual(expected.OpeningQuote, setting.OpeningQuote);
            Assert.AreEqual(expected.ClosingQuote, setting.ClosingQuote);
            Assert.AreEqual(expected.DefaultSchema, setting.DefaultSchema);
            Assert.AreEqual(expected.ParameterPrefix, setting.ParameterPrefix);
        }

        [TestMethod]
        public void ThrowExceptionOnUseMySqlIfTheSettingIsNull()
        {
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() =>
                GlobalConfiguration
                    .Setup()
                    .UseMySql((MySqlDbSetting)null));
        }
    }
}
