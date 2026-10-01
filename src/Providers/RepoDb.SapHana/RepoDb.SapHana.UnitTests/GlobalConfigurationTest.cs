#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Sap.Data.Hana;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.DbSettings;
using RepoDb.StatementBuilders;

namespace RepoDb.SapHana.UnitTests
{
    [TestClass]
    [DoNotParallelize]
    public class GlobalConfigurationTest
    {
        #region SubClasses

        /// <summary>
        /// A setting that inherits the <see cref="SapHanaDbSetting"/>, as the extension libraries do.
        /// </summary>
        private sealed class CustomSapHanaDbSetting : SapHanaDbSetting
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
                .UseSapHana(new SapHanaDbSetting());
        }

        [TestMethod]
        public void TestUseSapHanaRegistersTheDefaultSetting()
        {
            // Act
            GlobalConfiguration
                .Setup()
                .UseSapHana();

            // Assert
            Assert.IsTrue(SapHanaBootstrap.IsInitialized);
            Assert.IsInstanceOfType<SapHanaDbSetting>(DbSettingMapper.Get<HanaConnection>());
        }

        [TestMethod]
        public void TestUseSapHanaWithSettingRegistersTheGivenSetting()
        {
            // Setup
            var setting = new CustomSapHanaDbSetting { CustomValue = "Custom" };

            // Act
            var result = GlobalConfiguration
                .Setup()
                .UseSapHana(setting);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(SapHanaBootstrap.IsInitialized);
            Assert.AreSame(setting, DbSettingMapper.Get<HanaConnection>());
            Assert.IsInstanceOfType<SapHanaStatementBuilder>(StatementBuilderMapper.Get<HanaConnection>());
        }

        [TestMethod]
        public void TestUseSapHanaWithSettingReplacesAnEarlierInitialization()
        {
            // Setup
            GlobalConfiguration
                .Setup()
                .UseSapHana();
            var setting = new CustomSapHanaDbSetting();

            // Act
            GlobalConfiguration
                .Setup()
                .UseSapHana(setting);

            // Assert
            Assert.AreSame(setting, DbSettingMapper.Get<HanaConnection>());
        }

        [TestMethod]
        public void TestUseSapHanaAfterUseSapHanaWithSettingKeepsTheGivenSetting()
        {
            // Setup
            var setting = new CustomSapHanaDbSetting();
            GlobalConfiguration
                .Setup()
                .UseSapHana(setting);

            // Act (the parameterless call is skipped, as SAP HANA is already initialized)
            GlobalConfiguration
                .Setup()
                .UseSapHana();

            // Assert
            Assert.AreSame(setting, DbSettingMapper.Get<HanaConnection>());
        }

        [TestMethod]
        public void TestUseSapHanaWithSettingKeepsTheInheritedValues()
        {
            // Setup
            var expected = new SapHanaDbSetting();

            // Act
            GlobalConfiguration
                .Setup()
                .UseSapHana(new CustomSapHanaDbSetting());
            var setting = DbSettingMapper.Get<HanaConnection>();

            // Assert
            Assert.AreEqual(expected.OpeningQuote, setting.OpeningQuote);
            Assert.AreEqual(expected.ClosingQuote, setting.ClosingQuote);
            Assert.AreEqual(expected.DefaultSchema, setting.DefaultSchema);
            Assert.AreEqual(expected.ParameterPrefix, setting.ParameterPrefix);
        }
    }
}
