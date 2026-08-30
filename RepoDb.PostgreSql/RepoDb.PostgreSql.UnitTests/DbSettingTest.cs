#region Copyright Attributions

// Copyright (c) 2020 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Npgsql;

namespace RepoDb.PostgreSql.UnitTests
{
    [TestClass]
    public class DbSettingTest
    {
        [TestInitialize]
        public void Initialize()
        {
            GlobalConfiguration
                .Setup()
                .UsePostgreSql();
        }

        [TestMethod]
        public void TestPostgreSqlDbSettingAreTableHintsSupportedProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<NpgsqlConnection>();

            // Assert
            Assert.IsFalse(setting.AreTableHintsSupported);
        }

        [TestMethod]
        public void TestPostgreSqlDbSettingClosingQuoteProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<NpgsqlConnection>();

            // Assert
            Assert.AreEqual("\"", setting.ClosingQuote);
        }

        [TestMethod]
        public void TestPostgreSqlDbSettingDefaultSchemaProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<NpgsqlConnection>();

            // Assert
            Assert.AreEqual("public", setting.DefaultSchema);
        }

        [TestMethod]
        public void TestPostgreSqlDbSettingIsDirectionSupportedProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<NpgsqlConnection>();

            // Assert
            Assert.IsTrue(setting.IsDirectionSupported);
        }

        [TestMethod]
        public void TestPostgreSqlDbSettingIsExecuteReaderDisposableProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<NpgsqlConnection>();

            // Assert
            Assert.IsTrue(setting.IsExecuteReaderDisposable);
        }

        [TestMethod]
        public void TestPostgreSqlDbSettingIsMultiStatementExecutableProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<NpgsqlConnection>();

            // Assert
            Assert.IsTrue(setting.IsMultiStatementExecutable);
        }

        [TestMethod]
        public void TestPostgreSqlDbSettingIsUseUpsertProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<NpgsqlConnection>();

            // Assert
            Assert.IsFalse(setting.IsUseUpsert);
        }

        [TestMethod]
        public void TestPostgreSqlDbSettingOpeningQuoteProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<NpgsqlConnection>();

            // Assert
            Assert.AreEqual("\"", setting.OpeningQuote);
        }

        [TestMethod]
        public void TestPostgreSqlDbSettingParameterPrefixProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<NpgsqlConnection>();

            // Assert
            Assert.AreEqual("@", setting.ParameterPrefix);
        }

        [TestMethod]
        public void TestPostgreSqlDbSettingSqlTextParameterPrefixProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<NpgsqlConnection>();

            // Assert
            Assert.AreEqual("@", setting.SqlTextParameterPrefix);
        }
    }
}
