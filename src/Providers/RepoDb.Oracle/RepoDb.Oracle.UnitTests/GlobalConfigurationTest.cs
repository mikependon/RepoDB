#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Oracle.ManagedDataAccess.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.DbSettings;
using RepoDb.StatementBuilders;
using System;

namespace RepoDb.Oracle.UnitTests
{
    [TestClass]
    [DoNotParallelize]
    public class GlobalConfigurationTest
    {
        #region SubClasses

        /// <summary>
        /// A setting that inherits the <see cref="OracleDbSetting"/>, as the extension libraries do.
        /// </summary>
        private sealed class CustomOracleDbSetting : OracleDbSetting
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
                .UseOracle(new OracleDbSetting());
        }

        [TestMethod]
        public void TestUseOracleRegistersTheDefaultSetting()
        {
            // Act
            GlobalConfiguration
                .Setup()
                .UseOracle();

            // Assert
            Assert.IsTrue(OracleBootstrap.IsInitialized);
            Assert.IsInstanceOfType<OracleDbSetting>(DbSettingMapper.Get<OracleConnection>());
        }

        [TestMethod]
        public void TestUseOracleWithSettingRegistersTheGivenSetting()
        {
            // Setup
            var setting = new CustomOracleDbSetting { CustomValue = "Custom" };

            // Act
            var result = GlobalConfiguration
                .Setup()
                .UseOracle(setting);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(OracleBootstrap.IsInitialized);
            Assert.AreSame(setting, DbSettingMapper.Get<OracleConnection>());
            Assert.IsInstanceOfType<OracleStatementBuilder>(StatementBuilderMapper.Get<OracleConnection>());
        }

        [TestMethod]
        public void TestUseOracleWithSettingReplacesAnEarlierInitialization()
        {
            // Setup
            GlobalConfiguration
                .Setup()
                .UseOracle();
            var setting = new CustomOracleDbSetting();

            // Act
            GlobalConfiguration
                .Setup()
                .UseOracle(setting);

            // Assert
            Assert.AreSame(setting, DbSettingMapper.Get<OracleConnection>());
        }

        [TestMethod]
        public void TestUseOracleAfterUseOracleWithSettingKeepsTheGivenSetting()
        {
            // Setup
            var setting = new CustomOracleDbSetting();
            GlobalConfiguration
                .Setup()
                .UseOracle(setting);

            // Act (the parameterless call is skipped, as Oracle is already initialized)
            GlobalConfiguration
                .Setup()
                .UseOracle();

            // Assert
            Assert.AreSame(setting, DbSettingMapper.Get<OracleConnection>());
        }

        [TestMethod]
        public void TestUseOracleWithSettingKeepsTheInheritedValues()
        {
            // Setup
            var expected = new OracleDbSetting();

            // Act
            GlobalConfiguration
                .Setup()
                .UseOracle(new CustomOracleDbSetting());
            var setting = DbSettingMapper.Get<OracleConnection>();

            // Assert
            Assert.AreEqual(expected.OpeningQuote, setting.OpeningQuote);
            Assert.AreEqual(expected.ClosingQuote, setting.ClosingQuote);
            Assert.AreEqual(expected.DefaultSchema, setting.DefaultSchema);
            Assert.AreEqual(expected.ParameterPrefix, setting.ParameterPrefix);
        }

        [TestMethod]
        public void ThrowExceptionOnUseOracleIfTheSettingIsNull()
        {
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() =>
                GlobalConfiguration
                    .Setup()
                    .UseOracle((OracleDbSetting)null));
        }
    }
}
