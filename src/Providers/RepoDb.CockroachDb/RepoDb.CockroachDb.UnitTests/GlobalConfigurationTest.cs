#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using RepoDb.Connector.CockroachDb;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.DbSettings;
using RepoDb.StatementBuilders;
using System;

namespace RepoDb.CockroachDb.UnitTests
{
    [TestClass]
    [DoNotParallelize]
    public class GlobalConfigurationTest
    {
        #region SubClasses

        /// <summary>
        /// A setting that inherits the <see cref="CockroachDbDbSetting"/>, as the extension libraries do.
        /// </summary>
        private sealed class CustomCockroachDbDbSetting : CockroachDbDbSetting
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
                .UseCockroachDb(new CockroachDbDbSetting());
        }

        [TestMethod]
        public void TestUseCockroachDbRegistersTheDefaultSetting()
        {
            // Act
            GlobalConfiguration
                .Setup()
                .UseCockroachDb();

            // Assert
            Assert.IsTrue(CockroachDbBootstrap.IsInitialized);
            Assert.IsInstanceOfType<CockroachDbDbSetting>(DbSettingMapper.Get<CockroachDbConnection>());
        }

        [TestMethod]
        public void TestUseCockroachDbWithSettingRegistersTheGivenSetting()
        {
            // Setup
            var setting = new CustomCockroachDbDbSetting { CustomValue = "Custom" };

            // Act
            var result = GlobalConfiguration
                .Setup()
                .UseCockroachDb(setting);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(CockroachDbBootstrap.IsInitialized);
            Assert.AreSame(setting, DbSettingMapper.Get<CockroachDbConnection>());
            Assert.IsInstanceOfType<CockroachDbStatementBuilder>(StatementBuilderMapper.Get<CockroachDbConnection>());
        }

        [TestMethod]
        public void TestUseCockroachDbWithSettingReplacesAnEarlierInitialization()
        {
            // Setup
            GlobalConfiguration
                .Setup()
                .UseCockroachDb();
            var setting = new CustomCockroachDbDbSetting();

            // Act
            GlobalConfiguration
                .Setup()
                .UseCockroachDb(setting);

            // Assert
            Assert.AreSame(setting, DbSettingMapper.Get<CockroachDbConnection>());
        }

        [TestMethod]
        public void TestUseCockroachDbAfterUseCockroachDbWithSettingKeepsTheGivenSetting()
        {
            // Setup
            var setting = new CustomCockroachDbDbSetting();
            GlobalConfiguration
                .Setup()
                .UseCockroachDb(setting);

            // Act (the parameterless call is skipped, as CockroachDB is already initialized)
            GlobalConfiguration
                .Setup()
                .UseCockroachDb();

            // Assert
            Assert.AreSame(setting, DbSettingMapper.Get<CockroachDbConnection>());
        }

        [TestMethod]
        public void TestUseCockroachDbWithSettingKeepsTheInheritedValues()
        {
            // Setup
            var expected = new CockroachDbDbSetting();

            // Act
            GlobalConfiguration
                .Setup()
                .UseCockroachDb(new CustomCockroachDbDbSetting());
            var setting = DbSettingMapper.Get<CockroachDbConnection>();

            // Assert
            Assert.AreEqual(expected.OpeningQuote, setting.OpeningQuote);
            Assert.AreEqual(expected.ClosingQuote, setting.ClosingQuote);
            Assert.AreEqual(expected.DefaultSchema, setting.DefaultSchema);
            Assert.AreEqual(expected.ParameterPrefix, setting.ParameterPrefix);
        }

        [TestMethod]
        public void ThrowExceptionOnUseCockroachDbIfTheSettingIsNull()
        {
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() =>
                GlobalConfiguration
                    .Setup()
                    .UseCockroachDb((CockroachDbDbSetting)null));
        }
    }
}
