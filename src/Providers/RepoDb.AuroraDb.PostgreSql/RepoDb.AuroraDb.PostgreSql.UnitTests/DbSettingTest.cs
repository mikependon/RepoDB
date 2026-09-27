#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Connector.AuroraDb.Npgsql;

namespace RepoDb.AuroraDb.PostgreSql.UnitTests
{
    [TestClass]
    public class DbSettingTest
    {
        [TestInitialize]
        public void Initialize()
        {
            GlobalConfiguration
                .Setup()
                .UseAuroraDb();
        }

        [TestMethod]
        public void TestAuroraDbDbSettingAreTableHintsSupportedProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<AuroraDbConnection>();

            // Assert
            Assert.IsFalse(setting.AreTableHintsSupported);
        }

        [TestMethod]
        public void TestAuroraDbDbSettingClosingQuoteProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<AuroraDbConnection>();

            // Assert
            Assert.AreEqual("\"", setting.ClosingQuote, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestAuroraDbDbSettingDefaultSchemaProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<AuroraDbConnection>();

            // Assert
            Assert.AreEqual("public", setting.DefaultSchema, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestAuroraDbDbSettingIsDirectionSupportedProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<AuroraDbConnection>();

            // Assert
            Assert.IsTrue(setting.IsDirectionSupported);
        }

        [TestMethod]
        public void TestAuroraDbDbSettingIsExecuteReaderDisposableProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<AuroraDbConnection>();

            // Assert
            Assert.IsTrue(setting.IsExecuteReaderDisposable);
        }

        [TestMethod]
        public void TestAuroraDbDbSettingIsMultiStatementExecutableProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<AuroraDbConnection>();

            // Assert
            Assert.IsTrue(setting.IsMultiStatementExecutable);
        }

        [TestMethod]
        public void TestAuroraDbDbSettingIsUseUpsertProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<AuroraDbConnection>();

            // Assert
            Assert.IsFalse(setting.IsUseUpsert);
        }

        [TestMethod]
        public void TestAuroraDbDbSettingOpeningQuoteProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<AuroraDbConnection>();

            // Assert
            Assert.AreEqual("\"", setting.OpeningQuote, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestAuroraDbDbSettingParameterPrefixProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<AuroraDbConnection>();

            // Assert
            Assert.AreEqual("@", setting.ParameterPrefix, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestAuroraDbDbSettingSqlTextParameterPrefixProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<AuroraDbConnection>();

            // Assert
            Assert.AreEqual("@", setting.SqlTextParameterPrefix, StringComparer.Ordinal);
        }
    }
}
