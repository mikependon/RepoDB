#region Copyright Attributions

// Copyright (c) 2019 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using Ahtola.Data.Sqlite;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RepoDb.Ahtola.UnitTests
{
    [TestClass]
    public class DbSettingTest
    {
        [TestInitialize]
        public void Initialize()
        {
            GlobalConfiguration
                .Setup()
                .UseAhtola();
        }

        #region MDS

        [TestMethod]
        public void TestMdsAhtolaDbSettingAreTableHintsSupportedProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<SqliteConnection>();

            // Assert
            Assert.IsFalse(setting.AreTableHintsSupported);
        }

        [TestMethod]
        public void TestMdsAhtolaDbSettingClosingQuoteProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<SqliteConnection>();

            // Assert
            Assert.AreEqual("]", setting.ClosingQuote, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMdsAhtolaDbSettingDefaultSchemaProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<SqliteConnection>();

            // Assert
            Assert.IsNull(setting.DefaultSchema);
        }

        [TestMethod]
        public void TestMdsAhtolaDbSettingIsDirectionSupportedSupportedProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<SqliteConnection>();

            // Assert
            Assert.IsFalse(setting.IsDirectionSupported);
        }

        [TestMethod]
        public void TestMdsAhtolaDbSettingIsExecuteReaderDisposableProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<SqliteConnection>();

            // Assert
            Assert.IsFalse(setting.IsExecuteReaderDisposable);
        }

        [TestMethod]
        public void TestMdsAhtolaDbSettingIsMultiStatementExecutableProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<SqliteConnection>();

            // Assert
            Assert.IsTrue(setting.IsMultiStatementExecutable);
        }

        [TestMethod]
        public void TestMdsAhtolaDbSettingIsUseUpsertProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<SqliteConnection>();

            // Assert
            Assert.IsTrue(setting.IsUseUpsert);
        }

        [TestMethod]
        public void TestMdsAhtolaDbSettingOpeningQuoteProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<SqliteConnection>();

            // Assert
            Assert.AreEqual("[", setting.OpeningQuote, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMdsAhtolaDbSettingParameterPrefixProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<SqliteConnection>();

            // Assert
            Assert.AreEqual("@", setting.ParameterPrefix, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMdsAhtolaDbSettingSqlTextParameterPrefixProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<SqliteConnection>();

            // Assert
            Assert.AreEqual("@", setting.SqlTextParameterPrefix, StringComparer.Ordinal);
        }

        #endregion
    }
}
