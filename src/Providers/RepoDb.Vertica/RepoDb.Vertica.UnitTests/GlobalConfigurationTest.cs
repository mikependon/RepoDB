#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Vertica.Data.VerticaClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.DbSettings;
using RepoDb.StatementBuilders;
using System;

namespace RepoDb.Vertica.UnitTests
{
    [TestClass]
    [DoNotParallelize]
    public class GlobalConfigurationTest
    {
        #region SubClasses

        /// <summary>
        /// A setting that inherits the <see cref="VerticaDbSetting"/>, as the extension libraries do.
        /// </summary>
        private sealed class CustomVerticaDbSetting : VerticaDbSetting
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
                .UseVertica(new VerticaDbSetting());
        }

        [TestMethod]
        public void TestUseVerticaRegistersTheDefaultSetting()
        {
            // Act
            GlobalConfiguration
                .Setup()
                .UseVertica();

            // Assert
            Assert.IsTrue(VerticaBootstrap.IsInitialized);
            Assert.IsInstanceOfType<VerticaDbSetting>(DbSettingMapper.Get<VerticaConnection>());
        }

        [TestMethod]
        public void TestUseVerticaWithSettingRegistersTheGivenSetting()
        {
            // Setup
            var setting = new CustomVerticaDbSetting { CustomValue = "Custom" };

            // Act
            var result = GlobalConfiguration
                .Setup()
                .UseVertica(setting);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(VerticaBootstrap.IsInitialized);
            Assert.AreSame(setting, DbSettingMapper.Get<VerticaConnection>());
            Assert.IsInstanceOfType<VerticaStatementBuilder>(StatementBuilderMapper.Get<VerticaConnection>());
        }

        [TestMethod]
        public void TestUseVerticaWithSettingReplacesAnEarlierInitialization()
        {
            // Setup
            GlobalConfiguration
                .Setup()
                .UseVertica();
            var setting = new CustomVerticaDbSetting();

            // Act
            GlobalConfiguration
                .Setup()
                .UseVertica(setting);

            // Assert
            Assert.AreSame(setting, DbSettingMapper.Get<VerticaConnection>());
        }

        [TestMethod]
        public void TestUseVerticaAfterUseVerticaWithSettingKeepsTheGivenSetting()
        {
            // Setup
            var setting = new CustomVerticaDbSetting();
            GlobalConfiguration
                .Setup()
                .UseVertica(setting);

            // Act (the parameterless call is skipped, as Vertica is already initialized)
            GlobalConfiguration
                .Setup()
                .UseVertica();

            // Assert
            Assert.AreSame(setting, DbSettingMapper.Get<VerticaConnection>());
        }

        [TestMethod]
        public void TestUseVerticaWithSettingKeepsTheInheritedValues()
        {
            // Setup
            var expected = new VerticaDbSetting();

            // Act
            GlobalConfiguration
                .Setup()
                .UseVertica(new CustomVerticaDbSetting());
            var setting = DbSettingMapper.Get<VerticaConnection>();

            // Assert
            Assert.AreEqual(expected.OpeningQuote, setting.OpeningQuote);
            Assert.AreEqual(expected.ClosingQuote, setting.ClosingQuote);
            Assert.AreEqual(expected.DefaultSchema, setting.DefaultSchema);
            Assert.AreEqual(expected.ParameterPrefix, setting.ParameterPrefix);
        }

        [TestMethod]
        public void ThrowExceptionOnUseVerticaIfTheSettingIsNull()
        {
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() =>
                GlobalConfiguration
                    .Setup()
                    .UseVertica((VerticaDbSetting)null));
        }
    }
}
