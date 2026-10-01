#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using FirebirdSql.Data.FirebirdClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.DbSettings;
using RepoDb.StatementBuilders;
using System;

namespace RepoDb.Firebird.UnitTests
{
    [TestClass]
    [DoNotParallelize]
    public class GlobalConfigurationTest
    {
        #region SubClasses

        /// <summary>
        /// A setting that inherits the <see cref="FirebirdDbSetting"/>, as the extension libraries do.
        /// </summary>
        private sealed class CustomFirebirdDbSetting : FirebirdDbSetting
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
                .UseFirebird(new FirebirdDbSetting());
        }

        [TestMethod]
        public void TestUseFirebirdRegistersTheDefaultSetting()
        {
            // Act
            GlobalConfiguration
                .Setup()
                .UseFirebird();

            // Assert
            Assert.IsTrue(FirebirdBootstrap.IsInitialized);
            Assert.IsInstanceOfType<FirebirdDbSetting>(DbSettingMapper.Get<FbConnection>());
        }

        [TestMethod]
        public void TestUseFirebirdWithSettingRegistersTheGivenSetting()
        {
            // Setup
            var setting = new CustomFirebirdDbSetting { CustomValue = "Custom" };

            // Act
            var result = GlobalConfiguration
                .Setup()
                .UseFirebird(setting);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(FirebirdBootstrap.IsInitialized);
            Assert.AreSame(setting, DbSettingMapper.Get<FbConnection>());
            Assert.IsInstanceOfType<FirebirdStatementBuilder>(StatementBuilderMapper.Get<FbConnection>());
        }

        [TestMethod]
        public void TestUseFirebirdWithSettingReplacesAnEarlierInitialization()
        {
            // Setup
            GlobalConfiguration
                .Setup()
                .UseFirebird();
            var setting = new CustomFirebirdDbSetting();

            // Act
            GlobalConfiguration
                .Setup()
                .UseFirebird(setting);

            // Assert
            Assert.AreSame(setting, DbSettingMapper.Get<FbConnection>());
        }

        [TestMethod]
        public void TestUseFirebirdAfterUseFirebirdWithSettingKeepsTheGivenSetting()
        {
            // Setup
            var setting = new CustomFirebirdDbSetting();
            GlobalConfiguration
                .Setup()
                .UseFirebird(setting);

            // Act (the parameterless call is skipped, as Firebird is already initialized)
            GlobalConfiguration
                .Setup()
                .UseFirebird();

            // Assert
            Assert.AreSame(setting, DbSettingMapper.Get<FbConnection>());
        }

        [TestMethod]
        public void TestUseFirebirdWithSettingKeepsTheInheritedValues()
        {
            // Setup
            var expected = new FirebirdDbSetting();

            // Act
            GlobalConfiguration
                .Setup()
                .UseFirebird(new CustomFirebirdDbSetting());
            var setting = DbSettingMapper.Get<FbConnection>();

            // Assert
            Assert.AreEqual(expected.OpeningQuote, setting.OpeningQuote);
            Assert.AreEqual(expected.ClosingQuote, setting.ClosingQuote);
            Assert.AreEqual(expected.DefaultSchema, setting.DefaultSchema);
            Assert.AreEqual(expected.ParameterPrefix, setting.ParameterPrefix);
        }

        [TestMethod]
        public void ThrowExceptionOnUseFirebirdIfTheSettingIsNull()
        {
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() =>
                GlobalConfiguration
                    .Setup()
                    .UseFirebird((FirebirdDbSetting)null));
        }
    }
}
