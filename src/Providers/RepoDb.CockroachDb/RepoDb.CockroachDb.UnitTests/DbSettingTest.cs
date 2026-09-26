#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Connector.CockroachDb;

namespace RepoDb.CockroachDB.UnitTests
{
    [TestClass]
    public class DbSettingTest
    {
        [TestInitialize]
        public void Initialize()
        {
            GlobalConfiguration
                .Setup()
                .UseCockroachDb();
        }

        [TestMethod]
        public void TestCockroachDbDbSettingAreTableHintsSupportedProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<CockroachDbConnection>();

            // Assert
            Assert.IsFalse(setting.AreTableHintsSupported);
        }

        [TestMethod]
        public void TestCockroachDbDbSettingClosingQuoteProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<CockroachDbConnection>();

            // Assert
            Assert.AreEqual("\"", setting.ClosingQuote, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestCockroachDbDbSettingDefaultSchemaProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<CockroachDbConnection>();

            // Assert
            Assert.AreEqual("public", setting.DefaultSchema, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestCockroachDbDbSettingIsDirectionSupportedProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<CockroachDbConnection>();

            // Assert
            Assert.IsTrue(setting.IsDirectionSupported);
        }

        [TestMethod]
        public void TestCockroachDbDbSettingIsExecuteReaderDisposableProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<CockroachDbConnection>();

            // Assert
            Assert.IsTrue(setting.IsExecuteReaderDisposable);
        }

        [TestMethod]
        public void TestCockroachDbDbSettingIsMultiStatementExecutableProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<CockroachDbConnection>();

            // Assert
            Assert.IsTrue(setting.IsMultiStatementExecutable);
        }

        [TestMethod]
        public void TestCockroachDbDbSettingIsUseUpsertProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<CockroachDbConnection>();

            // Assert
            Assert.IsFalse(setting.IsUseUpsert);
        }

        [TestMethod]
        public void TestCockroachDbDbSettingOpeningQuoteProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<CockroachDbConnection>();

            // Assert
            Assert.AreEqual("\"", setting.OpeningQuote, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestCockroachDbDbSettingParameterPrefixProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<CockroachDbConnection>();

            // Assert
            Assert.AreEqual("@", setting.ParameterPrefix, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestCockroachDbDbSettingSqlTextParameterPrefixProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<CockroachDbConnection>();

            // Assert
            Assert.AreEqual("@", setting.SqlTextParameterPrefix, StringComparer.Ordinal);
        }
    }
}
