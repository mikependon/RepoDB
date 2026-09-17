#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FirebirdSql.Data.FirebirdClient;

namespace RepoDb.Firebird.UnitTests
{
    [TestClass]
    public class DbSettingTest
    {
        [TestInitialize]
        public void Initialize()
        {
            GlobalConfiguration
                .Setup()
                .UseFirebird();
        }

        [TestMethod]
        public void TestFirebirdDbSettingAreTableHintsSupportedProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<FbConnection>();

            // Assert
            Assert.IsFalse(setting.AreTableHintsSupported);
        }

        [TestMethod]
        public void TestFirebirdDbSettingClosingQuoteProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<FbConnection>();

            // Assert
            Assert.AreEqual("\"", setting.ClosingQuote, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestFirebirdDbSettingDefaultSchemaProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<FbConnection>();

            // Assert
            Assert.IsNull(setting.DefaultSchema);
        }

        [TestMethod]
        public void TestFirebirdDbSettingIsDirectionSupportedProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<FbConnection>();

            // Assert
            Assert.IsFalse(setting.IsDirectionSupported);
        }

        [TestMethod]
        public void TestFirebirdDbSettingIsExecuteReaderDisposableProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<FbConnection>();

            // Assert
            Assert.IsFalse(setting.IsExecuteReaderDisposable);
        }

        [TestMethod]
        public void TestFirebirdDbSettingIsMultiStatementExecutableProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<FbConnection>();

            // Assert - Firebird's ADO.NET provider does not support executing multiple statements
            // in a single round-trip, unlike MySQL/PostgreSql/SQL Server.
            Assert.IsFalse(setting.IsMultiStatementExecutable);
        }

        [TestMethod]
        public void TestFirebirdDbSettingIsUseUpsertProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<FbConnection>();

            // Assert
            Assert.IsFalse(setting.IsUseUpsert);
        }

        [TestMethod]
        public void TestFirebirdDbSettingMaxParameterCountProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<FbConnection>();

            // Assert
            Assert.AreEqual(1500, setting.MaxParameterCount);
        }

        [TestMethod]
        public void TestFirebirdDbSettingOpeningQuoteProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<FbConnection>();

            // Assert
            Assert.AreEqual("\"", setting.OpeningQuote, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestFirebirdDbSettingParameterPrefixProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<FbConnection>();

            // Assert
            Assert.AreEqual("@", setting.ParameterPrefix, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestFirebirdDbSettingSqlTextParameterPrefixProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<FbConnection>();

            // Assert
            Assert.AreEqual("@", setting.SqlTextParameterPrefix, StringComparer.Ordinal);
        }
    }
}
