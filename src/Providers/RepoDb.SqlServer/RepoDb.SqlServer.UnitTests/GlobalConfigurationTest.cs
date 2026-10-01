#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.DbSettings;
using RepoDb.StatementBuilders;
using System;

namespace RepoDb.SqlServer.UnitTests
{
    [TestClass]
    [DoNotParallelize]
    public class GlobalConfigurationTest
    {
        #region SubClasses

        /// <summary>
        /// A setting that inherits the <see cref="SqlServerDbSetting"/>, as the extension libraries do.
        /// </summary>
        private sealed class CustomSqlServerDbSetting : SqlServerDbSetting
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
                .UseSqlServer(new SqlServerDbSetting());
        }

        [TestMethod]
        public void TestUseSqlServerRegistersTheDefaultSetting()
        {
            // Act
            GlobalConfiguration
                .Setup()
                .UseSqlServer();

            // Assert
            Assert.IsTrue(SqlServerBootstrap.IsInitialized);
            Assert.IsInstanceOfType<SqlServerDbSetting>(DbSettingMapper.Get<SqlConnection>());
        }

        [TestMethod]
        public void TestUseSqlServerWithSettingRegistersTheGivenSetting()
        {
            // Setup
            var setting = new CustomSqlServerDbSetting { CustomValue = "Custom" };

            // Act
            var result = GlobalConfiguration
                .Setup()
                .UseSqlServer(setting);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(SqlServerBootstrap.IsInitialized);
            Assert.AreSame(setting, DbSettingMapper.Get<SqlConnection>());
            Assert.IsInstanceOfType<SqlServerStatementBuilder>(StatementBuilderMapper.Get<SqlConnection>());
        }

        [TestMethod]
        public void TestUseSqlServerWithSettingReplacesAnEarlierInitialization()
        {
            // Setup
            GlobalConfiguration
                .Setup()
                .UseSqlServer();
            var setting = new CustomSqlServerDbSetting();

            // Act
            GlobalConfiguration
                .Setup()
                .UseSqlServer(setting);

            // Assert
            Assert.AreSame(setting, DbSettingMapper.Get<SqlConnection>());
        }

        [TestMethod]
        public void TestUseSqlServerAfterUseSqlServerWithSettingKeepsTheGivenSetting()
        {
            // Setup
            var setting = new CustomSqlServerDbSetting();
            GlobalConfiguration
                .Setup()
                .UseSqlServer(setting);

            // Act (the parameterless call is skipped, as SQL Server is already initialized)
            GlobalConfiguration
                .Setup()
                .UseSqlServer();

            // Assert
            Assert.AreSame(setting, DbSettingMapper.Get<SqlConnection>());
        }

        [TestMethod]
        public void TestUseSqlServerWithSettingKeepsTheInheritedValues()
        {
            // Setup
            var expected = new SqlServerDbSetting();

            // Act
            GlobalConfiguration
                .Setup()
                .UseSqlServer(new CustomSqlServerDbSetting());
            var setting = DbSettingMapper.Get<SqlConnection>();

            // Assert
            Assert.AreEqual(expected.OpeningQuote, setting.OpeningQuote);
            Assert.AreEqual(expected.ClosingQuote, setting.ClosingQuote);
            Assert.AreEqual(expected.DefaultSchema, setting.DefaultSchema);
            Assert.AreEqual(expected.ParameterPrefix, setting.ParameterPrefix);
        }

        [TestMethod]
        public void ThrowExceptionOnUseSqlServerIfTheSettingIsNull()
        {
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() =>
                GlobalConfiguration
                    .Setup()
                    .UseSqlServer((SqlServerDbSetting)null));
        }
    }
}
