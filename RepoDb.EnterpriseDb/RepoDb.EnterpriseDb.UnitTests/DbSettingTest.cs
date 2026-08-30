#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using EnterpriseDB.EDBClient;

namespace RepoDb.EnterpriseDb.UnitTests
{
    [TestClass]
    public class DbSettingTest
    {
        [TestInitialize]
        public void Initialize()
        {
            GlobalConfiguration
                .Setup()
                .UseEnterpriseDb();
        }

        [TestMethod]
        public void TestEnterpriseDbDbSettingAreTableHintsSupportedProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<EDBConnection>();

            // Assert
            Assert.IsFalse(setting.AreTableHintsSupported);
        }

        [TestMethod]
        public void TestEnterpriseDbDbSettingClosingQuoteProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<EDBConnection>();

            // Assert
            Assert.AreEqual("\"", setting.ClosingQuote);
        }

        [TestMethod]
        public void TestEnterpriseDbDbSettingDefaultSchemaProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<EDBConnection>();

            // Assert
            Assert.AreEqual("public", setting.DefaultSchema);
        }

        [TestMethod]
        public void TestEnterpriseDbDbSettingIsDirectionSupportedProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<EDBConnection>();

            // Assert
            Assert.IsTrue(setting.IsDirectionSupported);
        }

        [TestMethod]
        public void TestEnterpriseDbDbSettingIsExecuteReaderDisposableProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<EDBConnection>();

            // Assert
            Assert.IsTrue(setting.IsExecuteReaderDisposable);
        }

        [TestMethod]
        public void TestEnterpriseDbDbSettingIsMultiStatementExecutableProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<EDBConnection>();

            // Assert
            Assert.IsTrue(setting.IsMultiStatementExecutable);
        }

        [TestMethod]
        public void TestEnterpriseDbDbSettingIsUseUpsertProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<EDBConnection>();

            // Assert
            Assert.IsFalse(setting.IsUseUpsert);
        }

        [TestMethod]
        public void TestEnterpriseDbDbSettingOpeningQuoteProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<EDBConnection>();

            // Assert
            Assert.AreEqual("\"", setting.OpeningQuote);
        }

        [TestMethod]
        public void TestEnterpriseDbDbSettingParameterPrefixProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<EDBConnection>();

            // Assert
            Assert.AreEqual("@", setting.ParameterPrefix);
        }

        [TestMethod]
        public void TestEnterpriseDbDbSettingSqlTextParameterPrefixProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<EDBConnection>();

            // Assert
            Assert.AreEqual("@", setting.SqlTextParameterPrefix);
        }
    }
}
