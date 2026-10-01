#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using IBM.Data.Db2;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.DbSettings;
using RepoDb.StatementBuilders;
using System;

namespace RepoDb.Db2.UnitTests
{
    [TestClass]
    [DoNotParallelize]
    public class GlobalConfigurationTest
    {
        #region SubClasses

        /// <summary>
        /// A setting that inherits the <see cref="Db2DbSetting"/>, as the extension libraries do.
        /// </summary>
        private sealed class CustomDb2DbSetting : Db2DbSetting
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
                .UseDb2(new Db2DbSetting());
        }

        [TestMethod]
        public void TestUseDb2RegistersTheDefaultSetting()
        {
            // Act
            GlobalConfiguration
                .Setup()
                .UseDb2();

            // Assert
            Assert.IsTrue(Db2Bootstrap.IsInitialized);
            Assert.IsInstanceOfType<Db2DbSetting>(DbSettingMapper.Get<DB2Connection>());
        }

        [TestMethod]
        public void TestUseDb2WithSettingRegistersTheGivenSetting()
        {
            // Setup
            var setting = new CustomDb2DbSetting { CustomValue = "Custom" };

            // Act
            var result = GlobalConfiguration
                .Setup()
                .UseDb2(setting);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(Db2Bootstrap.IsInitialized);
            Assert.AreSame(setting, DbSettingMapper.Get<DB2Connection>());
            Assert.IsInstanceOfType<Db2StatementBuilder>(StatementBuilderMapper.Get<DB2Connection>());
        }

        [TestMethod]
        public void TestUseDb2WithSettingReplacesAnEarlierInitialization()
        {
            // Setup
            GlobalConfiguration
                .Setup()
                .UseDb2();
            var setting = new CustomDb2DbSetting();

            // Act
            GlobalConfiguration
                .Setup()
                .UseDb2(setting);

            // Assert
            Assert.AreSame(setting, DbSettingMapper.Get<DB2Connection>());
        }

        [TestMethod]
        public void TestUseDb2AfterUseDb2WithSettingKeepsTheGivenSetting()
        {
            // Setup
            var setting = new CustomDb2DbSetting();
            GlobalConfiguration
                .Setup()
                .UseDb2(setting);

            // Act (the parameterless call is skipped, as DB2 is already initialized)
            GlobalConfiguration
                .Setup()
                .UseDb2();

            // Assert
            Assert.AreSame(setting, DbSettingMapper.Get<DB2Connection>());
        }

        [TestMethod]
        public void TestUseDb2WithSettingKeepsTheInheritedValues()
        {
            // Setup
            var expected = new Db2DbSetting();

            // Act
            GlobalConfiguration
                .Setup()
                .UseDb2(new CustomDb2DbSetting());
            var setting = DbSettingMapper.Get<DB2Connection>();

            // Assert
            Assert.AreEqual(expected.OpeningQuote, setting.OpeningQuote);
            Assert.AreEqual(expected.ClosingQuote, setting.ClosingQuote);
            Assert.AreEqual(expected.DefaultSchema, setting.DefaultSchema);
            Assert.AreEqual(expected.ParameterPrefix, setting.ParameterPrefix);
        }

        [TestMethod]
        public void ThrowExceptionOnUseDb2IfTheSettingIsNull()
        {
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() =>
                GlobalConfiguration
                    .Setup()
                    .UseDb2((Db2DbSetting)null));
        }
    }
}
