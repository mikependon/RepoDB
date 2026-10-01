#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Npgsql;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.DbSettings;
using RepoDb.StatementBuilders;
using System;

namespace RepoDb.PostgreSql.UnitTests
{
    [TestClass]
    [DoNotParallelize]
    public class GlobalConfigurationTest
    {
        #region SubClasses

        /// <summary>
        /// A setting that inherits the <see cref="PostgreSqlDbSetting"/>, as the extension libraries do.
        /// </summary>
        private sealed class CustomPostgreSqlDbSetting : PostgreSqlDbSetting
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
                .UsePostgreSql(new PostgreSqlDbSetting());
        }

        [TestMethod]
        public void TestUsePostgreSqlRegistersTheDefaultSetting()
        {
            // Act
            GlobalConfiguration
                .Setup()
                .UsePostgreSql();

            // Assert
            Assert.IsTrue(PostgreSqlBootstrap.IsInitialized);
            Assert.IsInstanceOfType<PostgreSqlDbSetting>(DbSettingMapper.Get<NpgsqlConnection>());
        }

        [TestMethod]
        public void TestUsePostgreSqlWithSettingRegistersTheGivenSetting()
        {
            // Setup
            var setting = new CustomPostgreSqlDbSetting { CustomValue = "Custom" };

            // Act
            var result = GlobalConfiguration
                .Setup()
                .UsePostgreSql(setting);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(PostgreSqlBootstrap.IsInitialized);
            Assert.AreSame(setting, DbSettingMapper.Get<NpgsqlConnection>());
            Assert.IsInstanceOfType<PostgreSqlStatementBuilder>(StatementBuilderMapper.Get<NpgsqlConnection>());
        }

        [TestMethod]
        public void TestUsePostgreSqlWithSettingReplacesAnEarlierInitialization()
        {
            // Setup
            GlobalConfiguration
                .Setup()
                .UsePostgreSql();
            var setting = new CustomPostgreSqlDbSetting();

            // Act
            GlobalConfiguration
                .Setup()
                .UsePostgreSql(setting);

            // Assert
            Assert.AreSame(setting, DbSettingMapper.Get<NpgsqlConnection>());
        }

        [TestMethod]
        public void TestUsePostgreSqlAfterUsePostgreSqlWithSettingKeepsTheGivenSetting()
        {
            // Setup
            var setting = new CustomPostgreSqlDbSetting();
            GlobalConfiguration
                .Setup()
                .UsePostgreSql(setting);

            // Act (the parameterless call is skipped, as PostgreSQL is already initialized)
            GlobalConfiguration
                .Setup()
                .UsePostgreSql();

            // Assert
            Assert.AreSame(setting, DbSettingMapper.Get<NpgsqlConnection>());
        }

        [TestMethod]
        public void TestUsePostgreSqlWithSettingKeepsTheInheritedValues()
        {
            // Setup
            var expected = new PostgreSqlDbSetting();

            // Act
            GlobalConfiguration
                .Setup()
                .UsePostgreSql(new CustomPostgreSqlDbSetting());
            var setting = DbSettingMapper.Get<NpgsqlConnection>();

            // Assert
            Assert.AreEqual(expected.OpeningQuote, setting.OpeningQuote);
            Assert.AreEqual(expected.ClosingQuote, setting.ClosingQuote);
            Assert.AreEqual(expected.DefaultSchema, setting.DefaultSchema);
            Assert.AreEqual(expected.ParameterPrefix, setting.ParameterPrefix);
        }

        [TestMethod]
        public void ThrowExceptionOnUsePostgreSqlIfTheSettingIsNull()
        {
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() =>
                GlobalConfiguration
                    .Setup()
                    .UsePostgreSql((PostgreSqlDbSetting)null));
        }
    }
}
