#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DuckDB.NET.Data;

namespace RepoDb.DuckDb.UnitTests
{
    [TestClass]
    public class DbSettingTest
    {
        [TestInitialize]
        public void Initialize()
        {
            GlobalConfiguration
                .Setup()
                .UseDuckDb();
        }

        [TestMethod]
        public void TestDuckDbDbSettingAreTableHintsSupportedProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<DuckDBConnection>();

            // Assert
            Assert.IsFalse(setting.AreTableHintsSupported);
        }

        [TestMethod]
        public void TestDuckDbDbSettingClosingQuoteProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<DuckDBConnection>();

            // Assert
            Assert.AreEqual("\"", setting.ClosingQuote, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDuckDbDbSettingDefaultSchemaProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<DuckDBConnection>();

            // Assert
            Assert.AreEqual("main", setting.DefaultSchema, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDuckDbDbSettingIsDirectionSupportedProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<DuckDBConnection>();

            // Assert
            Assert.IsFalse(setting.IsDirectionSupported);
        }

        [TestMethod]
        public void TestDuckDbDbSettingIsExecuteReaderDisposableProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<DuckDBConnection>();

            // Assert
            Assert.IsTrue(setting.IsExecuteReaderDisposable);
        }

        [TestMethod]
        public void TestDuckDbDbSettingIsMultiStatementExecutableProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<DuckDBConnection>();

            // Assert
            Assert.IsTrue(setting.IsMultiStatementExecutable);
        }

        [TestMethod]
        public void TestDuckDbDbSettingIsUseUpsertProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<DuckDBConnection>();

            // Assert
            Assert.IsFalse(setting.IsUseUpsert);
        }

        [TestMethod]
        public void TestDuckDbDbSettingOpeningQuoteProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<DuckDBConnection>();

            // Assert
            Assert.AreEqual("\"", setting.OpeningQuote, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDuckDbDbSettingParameterPrefixProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<DuckDBConnection>();

            // Assert
            Assert.AreEqual("$", setting.ParameterPrefix, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDuckDbDbSettingSqlTextParameterPrefixProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<DuckDBConnection>();

            // Assert
            Assert.AreEqual("$", setting.SqlTextParameterPrefix, StringComparer.Ordinal);
        }
    }
}
