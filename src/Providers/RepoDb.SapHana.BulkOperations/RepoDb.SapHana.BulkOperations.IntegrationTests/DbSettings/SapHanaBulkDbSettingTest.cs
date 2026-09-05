#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sap.Data.Hana;
using RepoDb.DbSettings;
using RepoDb.Enumerations.SapHana;

namespace RepoDb.SapHana.BulkOperations.IntegrationTests.DbSettings
{
    [TestClass]
    public class SapHanaBulkDbSettingTest
    {
        [TestInitialize]
        public void Initialize()
        {
            GlobalConfiguration
                .Setup()
                .UseSapHana(new SapHanaBulkDbSetting());
        }

        [TestMethod]
        public void TestSapHanaBulkDbSettingWriteToServerExecutionDefaultValue()
        {
            // Setup
            var setting = new SapHanaBulkDbSetting();

            // Assert
            Assert.AreEqual(SapHanaWriteToServerExecution.SapHanaCommandBatcher, setting.WriteToServerExecution);
        }

        [TestMethod]
        public void TestSapHanaBulkDbSettingWriteToServerExecutionPropertyCanBeSetToAsyncOverSync()
        {
            // Setup
            var setting = new SapHanaBulkDbSetting
            {
                WriteToServerExecution = SapHanaWriteToServerExecution.AsyncOverSync
            };

            // Assert
            Assert.AreEqual(SapHanaWriteToServerExecution.AsyncOverSync, setting.WriteToServerExecution);
        }

        [TestMethod]
        public void TestSapHanaBulkDbSettingAreTableHintsSupportedProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<HanaConnection>();

            // Assert
            Assert.IsFalse(setting.AreTableHintsSupported);
        }

        [TestMethod]
        public void TestSapHanaBulkDbSettingClosingQuoteProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<HanaConnection>();

            // Assert
            Assert.AreEqual("\"", setting.ClosingQuote);
        }

        [TestMethod]
        public void TestSapHanaBulkDbSettingDefaultSchemaProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<HanaConnection>();

            // Assert
            Assert.IsNull(setting.DefaultSchema);
        }

        [TestMethod]
        public void TestSapHanaBulkDbSettingIsAffectedRowsSupportedProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<HanaConnection>();

            // Assert
            Assert.IsTrue(setting.IsAffectedRowsSupported);
        }

        [TestMethod]
        public void TestSapHanaBulkDbSettingIsDirectionSupportedProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<HanaConnection>();

            // Assert
            Assert.IsFalse(setting.IsDirectionSupported);
        }

        [TestMethod]
        public void TestSapHanaBulkDbSettingIsExecuteReaderDisposableProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<HanaConnection>();

            // Assert
            Assert.IsTrue(setting.IsExecuteReaderDisposable);
        }

        [TestMethod]
        public void TestSapHanaBulkDbSettingIsMultiStatementExecutableProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<HanaConnection>();

            // Assert
            Assert.IsFalse(setting.IsMultiStatementExecutable);
        }

        [TestMethod]
        public void TestSapHanaBulkDbSettingIsPreparableProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<HanaConnection>();

            // Assert
            Assert.IsTrue(setting.IsPreparable);
        }

        [TestMethod]
        public void TestSapHanaBulkDbSettingIsTransactionSupportedProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<HanaConnection>();

            // Assert
            Assert.IsTrue(setting.IsTransactionSupported);
        }

        [TestMethod]
        public void TestSapHanaBulkDbSettingIsUseUpsertProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<HanaConnection>();

            // Assert
            Assert.IsFalse(setting.IsUseUpsert);
        }

        [TestMethod]
        public void TestSapHanaBulkDbSettingMultiStatementSeparatorProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<HanaConnection>();

            // Assert
            Assert.AreEqual(";", setting.MultiStatementSeparator);
        }

        [TestMethod]
        public void TestSapHanaBulkDbSettingOpeningQuoteProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<HanaConnection>();

            // Assert
            Assert.AreEqual("\"", setting.OpeningQuote);
        }

        [TestMethod]
        public void TestSapHanaBulkDbSettingParameterPrefixProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<HanaConnection>();

            // Assert
            Assert.AreEqual(":", setting.ParameterPrefix);
        }

        [TestMethod]
        public void TestSapHanaBulkDbSettingSqlTextParameterPrefixProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<HanaConnection>();

            // Assert
            Assert.AreEqual(":", setting.SqlTextParameterPrefix);
        }

        [TestMethod]
        public void TestSapHanaBulkDbSettingIsRegisteredAsSapHanaBulkDbSetting()
        {
            // Setup
            var setting = DbSettingMapper.Get<HanaConnection>();

            // Assert
            Assert.IsInstanceOfType(setting, typeof(SapHanaBulkDbSetting));
        }

        [TestMethod]
        public void TestSapHanaBulkDbSettingIsRegisteredAsISapHanaBulkDbSetting()
        {
            // Setup
            var setting = DbSettingMapper.Get<HanaConnection>();

            // Assert
            Assert.IsInstanceOfType(setting, typeof(ISapHanaBulkDbSetting));
        }
    }
}
