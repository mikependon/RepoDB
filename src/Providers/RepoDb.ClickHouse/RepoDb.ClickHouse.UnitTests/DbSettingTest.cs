#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClickHouse.Driver.ADO;
using RepoDb.DbSettings;

namespace RepoDb.ClickHouse.UnitTests
{
    [TestClass]
    public class DbSettingTest
    {
        [TestInitialize]
        public void Initialize()
        {
            GlobalConfiguration
                .Setup()
                .UseClickHouse();
        }

        [TestMethod]
        public void TestClickHouseDbSettingAreTableHintsSupportedProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<ClickHouseConnection>();

            // Assert
            Assert.IsFalse(setting.AreTableHintsSupported);
        }

        [TestMethod]
        public void TestClickHouseDbSettingClosingQuoteProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<ClickHouseConnection>();

            // Assert
            Assert.AreEqual("`", setting.ClosingQuote, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClickHouseDbSettingDefaultSchemaProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<ClickHouseConnection>();

            // Assert
            Assert.IsNull(setting.DefaultSchema);
        }

        [TestMethod]
        public void TestClickHouseDbSettingIsDirectionSupportedProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<ClickHouseConnection>();

            // Assert
            Assert.IsFalse(setting.IsDirectionSupported);
        }

        [TestMethod]
        public void TestClickHouseDbSettingIsExecuteReaderDisposableProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<ClickHouseConnection>();

            // Assert
            Assert.IsFalse(setting.IsExecuteReaderDisposable);
        }

        [TestMethod]
        public void TestClickHouseDbSettingIsMultiStatementExecutableProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<ClickHouseConnection>();

            // Assert
            Assert.IsFalse(setting.IsMultiStatementExecutable);
        }

        [TestMethod]
        public void TestClickHouseDbSettingIsUseUpsertProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<ClickHouseConnection>();

            // Assert
            Assert.IsFalse(setting.IsUseUpsert);
        }

        [TestMethod]
        public void TestClickHouseDbSettingOpeningQuoteProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<ClickHouseConnection>();

            // Assert
            Assert.AreEqual("`", setting.OpeningQuote, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClickHouseDbSettingParameterPrefixProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<ClickHouseConnection>();

            // Assert
            // Unlike most providers, ClickHouse.Driver binds a real DbParameter.ParameterName without any
            // prefix, so ParameterPrefix is string.Empty here - the "@" prefix is still used for the SQL text
            // placeholder token, via SqlTextParameterPrefix (see the test below).
            Assert.AreEqual(string.Empty, setting.ParameterPrefix, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClickHouseDbSettingSqlTextParameterPrefixProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<ClickHouseConnection>();

            // Assert
            Assert.AreEqual("@", setting.SqlTextParameterPrefix, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestClickHouseDbSettingIsAffectedRowsSupportedProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<ClickHouseConnection>();

            // Assert
            Assert.IsFalse(setting.IsAffectedRowsSupported);
        }

        [TestMethod]
        public void TestClickHouseDbSettingIsPreparableProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<ClickHouseConnection>();

            // Assert
            Assert.IsFalse(setting.IsPreparable);
        }

        [TestMethod]
        public void TestClickHouseDbSettingIsTransactionSupportedProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<ClickHouseConnection>();

            // Assert
            Assert.IsFalse(setting.IsTransactionSupported);
        }

        [TestMethod]
        public void TestClickHouseDbSettingMultiStatementSeparatorProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<ClickHouseConnection>();

            // Assert
            Assert.AreEqual(";", setting.MultiStatementSeparator, StringComparer.Ordinal);
        }
    }
}
