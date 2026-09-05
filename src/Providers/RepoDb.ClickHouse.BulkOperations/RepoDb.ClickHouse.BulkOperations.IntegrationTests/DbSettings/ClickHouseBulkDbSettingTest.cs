#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClickHouse.Driver.ADO;
using RepoDb.DbSettings;

namespace RepoDb.ClickHouse.BulkOperations.IntegrationTests.DbSettings
{
    [TestClass]
    public class ClickHouseBulkDbSettingTest
    {
        [TestInitialize]
        public void Initialize()
        {
            GlobalConfiguration
                .Setup()
                .UseClickHouse(new ClickHouseBulkDbSetting());
        }

        [TestMethod]
        public void TestClickHouseBulkDbSettingIsWaitForMutationsEnabledDefaultValue()
        {
            // Setup
            var setting = new ClickHouseBulkDbSetting();

            // Assert
            Assert.IsTrue(setting.IsWaitForMutationsEnabled);
        }

        [TestMethod]
        public void TestClickHouseBulkDbSettingIsWaitForMutationsEnabledPropertyCanBeDisabled()
        {
            // Setup
            var setting = new ClickHouseBulkDbSetting
            {
                IsWaitForMutationsEnabled = false
            };

            // Assert
            Assert.IsFalse(setting.IsWaitForMutationsEnabled);
        }

        [TestMethod]
        public void TestClickHouseBulkDbSettingAreTableHintsSupportedProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<ClickHouseConnection>();

            // Assert
            Assert.IsFalse(setting.AreTableHintsSupported);
        }

        [TestMethod]
        public void TestClickHouseBulkDbSettingClosingQuoteProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<ClickHouseConnection>();

            // Assert
            Assert.AreEqual("`", setting.ClosingQuote);
        }

        [TestMethod]
        public void TestClickHouseBulkDbSettingDefaultSchemaProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<ClickHouseConnection>();

            // Assert
            Assert.IsNull(setting.DefaultSchema);
        }

        [TestMethod]
        public void TestClickHouseBulkDbSettingIsAffectedRowsSupportedProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<ClickHouseConnection>();

            // Assert
            Assert.IsFalse(setting.IsAffectedRowsSupported);
        }

        [TestMethod]
        public void TestClickHouseBulkDbSettingIsDirectionSupportedProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<ClickHouseConnection>();

            // Assert
            Assert.IsFalse(setting.IsDirectionSupported);
        }

        [TestMethod]
        public void TestClickHouseBulkDbSettingIsExecuteReaderDisposableProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<ClickHouseConnection>();

            // Assert
            Assert.IsFalse(setting.IsExecuteReaderDisposable);
        }

        [TestMethod]
        public void TestClickHouseBulkDbSettingIsMultiStatementExecutableProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<ClickHouseConnection>();

            // Assert
            Assert.IsFalse(setting.IsMultiStatementExecutable);
        }

        [TestMethod]
        public void TestClickHouseBulkDbSettingIsPreparableProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<ClickHouseConnection>();

            // Assert
            Assert.IsFalse(setting.IsPreparable);
        }

        [TestMethod]
        public void TestClickHouseBulkDbSettingIsTransactionSupportedProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<ClickHouseConnection>();

            // Assert
            Assert.IsFalse(setting.IsTransactionSupported);
        }

        [TestMethod]
        public void TestClickHouseBulkDbSettingIsUseUpsertProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<ClickHouseConnection>();

            // Assert
            Assert.IsFalse(setting.IsUseUpsert);
        }

        [TestMethod]
        public void TestClickHouseBulkDbSettingMultiStatementSeparatorProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<ClickHouseConnection>();

            // Assert
            Assert.AreEqual(";", setting.MultiStatementSeparator);
        }

        [TestMethod]
        public void TestClickHouseBulkDbSettingOpeningQuoteProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<ClickHouseConnection>();

            // Assert
            Assert.AreEqual("`", setting.OpeningQuote);
        }

        [TestMethod]
        public void TestClickHouseBulkDbSettingParameterPrefixProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<ClickHouseConnection>();

            // Assert
            Assert.AreEqual(string.Empty, setting.ParameterPrefix);
        }

        [TestMethod]
        public void TestClickHouseBulkDbSettingSqlTextParameterPrefixProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<ClickHouseConnection>();

            // Assert
            Assert.AreEqual("@", setting.SqlTextParameterPrefix);
        }

        [TestMethod]
        public void TestClickHouseBulkDbSettingIsRegisteredAsClickHouseBulkDbSetting()
        {
            // Setup
            var setting = DbSettingMapper.Get<ClickHouseConnection>();

            // Assert
            Assert.IsInstanceOfType(setting, typeof(ClickHouseBulkDbSetting));
        }
    }
}
