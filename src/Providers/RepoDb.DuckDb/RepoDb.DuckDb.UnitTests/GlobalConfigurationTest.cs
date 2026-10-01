#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using DuckDB.NET.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.DbSettings;
using RepoDb.StatementBuilders;
using System;

namespace RepoDb.DuckDb.UnitTests
{
    [TestClass]
    [DoNotParallelize]
    public class GlobalConfigurationTest
    {
        #region SubClasses

        /// <summary>
        /// A setting that inherits the <see cref="DuckDbDbSetting"/>, as the extension libraries do.
        /// </summary>
        private sealed class CustomDuckDbDbSetting : DuckDbDbSetting
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
                .UseDuckDb(new DuckDbDbSetting());
        }

        [TestMethod]
        public void TestUseDuckDbRegistersTheDefaultSetting()
        {
            // Act
            GlobalConfiguration
                .Setup()
                .UseDuckDb();

            // Assert
            Assert.IsTrue(DuckDbBootstrap.IsInitialized);
            Assert.IsInstanceOfType<DuckDbDbSetting>(DbSettingMapper.Get<DuckDBConnection>());
        }

        [TestMethod]
        public void TestUseDuckDbWithSettingRegistersTheGivenSetting()
        {
            // Setup
            var setting = new CustomDuckDbDbSetting { CustomValue = "Custom" };

            // Act
            var result = GlobalConfiguration
                .Setup()
                .UseDuckDb(setting);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(DuckDbBootstrap.IsInitialized);
            Assert.AreSame(setting, DbSettingMapper.Get<DuckDBConnection>());
            Assert.IsInstanceOfType<DuckDbStatementBuilder>(StatementBuilderMapper.Get<DuckDBConnection>());
        }

        [TestMethod]
        public void TestUseDuckDbWithSettingReplacesAnEarlierInitialization()
        {
            // Setup
            GlobalConfiguration
                .Setup()
                .UseDuckDb();
            var setting = new CustomDuckDbDbSetting();

            // Act
            GlobalConfiguration
                .Setup()
                .UseDuckDb(setting);

            // Assert
            Assert.AreSame(setting, DbSettingMapper.Get<DuckDBConnection>());
        }

        [TestMethod]
        public void TestUseDuckDbAfterUseDuckDbWithSettingKeepsTheGivenSetting()
        {
            // Setup
            var setting = new CustomDuckDbDbSetting();
            GlobalConfiguration
                .Setup()
                .UseDuckDb(setting);

            // Act (the parameterless call is skipped, as DuckDB is already initialized)
            GlobalConfiguration
                .Setup()
                .UseDuckDb();

            // Assert
            Assert.AreSame(setting, DbSettingMapper.Get<DuckDBConnection>());
        }

        [TestMethod]
        public void TestUseDuckDbWithSettingKeepsTheInheritedValues()
        {
            // Setup
            var expected = new DuckDbDbSetting();

            // Act
            GlobalConfiguration
                .Setup()
                .UseDuckDb(new CustomDuckDbDbSetting());
            var setting = DbSettingMapper.Get<DuckDBConnection>();

            // Assert
            Assert.AreEqual(expected.OpeningQuote, setting.OpeningQuote);
            Assert.AreEqual(expected.ClosingQuote, setting.ClosingQuote);
            Assert.AreEqual(expected.DefaultSchema, setting.DefaultSchema);
            Assert.AreEqual(expected.ParameterPrefix, setting.ParameterPrefix);
        }

        [TestMethod]
        public void ThrowExceptionOnUseDuckDbIfTheSettingIsNull()
        {
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() =>
                GlobalConfiguration
                    .Setup()
                    .UseDuckDb((DuckDbDbSetting)null));
        }
    }
}
