#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sap.Data.Hana;

namespace RepoDb.SapHana.UnitTests
{
    [TestClass]
    public class DbSettingTest
    {
        [TestInitialize]
        public void Initialize()
        {
            GlobalConfiguration
                .Setup()
                .UseSapHana();
        }

        [TestMethod]
        public void TestSapHanaDbSettingAreTableHintsSupportedProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<HanaConnection>();

            // Assert
            Assert.IsFalse(setting.AreTableHintsSupported);
        }

        [TestMethod]
        public void TestSapHanaDbSettingClosingQuoteProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<HanaConnection>();

            // Assert
            Assert.AreEqual("\"", setting.ClosingQuote);
        }

        [TestMethod]
        public void TestSapHanaDbSettingDefaultSchemaProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<HanaConnection>();

            // Assert
            Assert.IsNull(setting.DefaultSchema);
        }

        [TestMethod]
        public void TestSapHanaDbSettingIsDirectionSupportedProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<HanaConnection>();

            // Assert
            Assert.IsFalse(setting.IsDirectionSupported);
        }

        [TestMethod]
        public void TestSapHanaDbSettingIsExecuteReaderDisposableProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<HanaConnection>();

            // Assert
            Assert.IsTrue(setting.IsExecuteReaderDisposable);
        }

        [TestMethod]
        public void TestSapHanaDbSettingIsMultiStatementExecutableProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<HanaConnection>();

            // Assert
            Assert.IsFalse(setting.IsMultiStatementExecutable);
        }

        [TestMethod]
        public void TestSapHanaDbSettingIsUseUpsertProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<HanaConnection>();

            // Assert
            Assert.IsFalse(setting.IsUseUpsert);
        }

        [TestMethod]
        public void TestSapHanaDbSettingOpeningQuoteProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<HanaConnection>();

            // Assert
            Assert.AreEqual("\"", setting.OpeningQuote);
        }

        [TestMethod]
        public void TestSapHanaDbSettingParameterPrefixProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<HanaConnection>();

            // Assert
            Assert.AreEqual(":", setting.ParameterPrefix);
        }

        [TestMethod]
        public void TestSapHanaDbSettingSqlTextParameterPrefixProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<HanaConnection>();

            // Assert
            Assert.AreEqual(":", setting.SqlTextParameterPrefix);
        }
    }
}
