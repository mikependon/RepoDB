using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vertica.Data.VerticaClient;

namespace RepoDb.Vertica.UnitTests
{
    [TestClass]
    public class DbSettingTest
    {
        [TestInitialize]
        public void Initialize()
        {
            GlobalConfiguration
                .Setup()
                .UseVertica();
        }

        [TestMethod]
        public void TestVerticaDbSettingAreTableHintsSupportedProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<VerticaConnection>();

            // Assert
            Assert.IsFalse(setting.AreTableHintsSupported);
        }

        [TestMethod]
        public void TestVerticaDbSettingClosingQuoteProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<VerticaConnection>();

            // Assert
            Assert.AreEqual("\"", setting.ClosingQuote);
        }

        [TestMethod]
        public void TestVerticaDbSettingDefaultSchemaProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<VerticaConnection>();

            // Assert
            Assert.IsNull(setting.DefaultSchema);
        }

        [TestMethod]
        public void TestVerticaDbSettingIsDirectionSupportedProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<VerticaConnection>();

            // Assert
            Assert.IsFalse(setting.IsDirectionSupported);
        }

        [TestMethod]
        public void TestVerticaDbSettingIsExecuteReaderDisposableProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<VerticaConnection>();

            // Assert
            Assert.IsFalse(setting.IsExecuteReaderDisposable);
        }

        [TestMethod]
        public void TestVerticaDbSettingIsMultiStatementExecutableProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<VerticaConnection>();

            // Assert
            Assert.IsFalse(setting.IsMultiStatementExecutable);
        }

        [TestMethod]
        public void TestVerticaDbSettingIsInsertAllBatchableProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<VerticaConnection>();

            // Assert
            Assert.IsTrue(setting.IsInsertAllBatchable);
        }

        [TestMethod]
        public void TestVerticaDbSettingIsUseUpsertProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<VerticaConnection>();

            // Assert
            Assert.IsTrue(setting.IsUseUpsert);
        }

        [TestMethod]
        public void TestVerticaDbSettingMaxParameterCountProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<VerticaConnection>();

            // Assert
            Assert.AreEqual(1500, setting.MaxParameterCount);
        }

        [TestMethod]
        public void TestVerticaDbSettingOpeningQuoteProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<VerticaConnection>();

            // Assert
            Assert.AreEqual("\"", setting.OpeningQuote);
        }

        [TestMethod]
        public void TestVerticaDbSettingParameterPrefixProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<VerticaConnection>();

            // Assert
            Assert.AreEqual("@", setting.ParameterPrefix);
        }

        [TestMethod]
        public void TestVerticaDbSettingSqlTextParameterPrefixProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<VerticaConnection>();

            // Assert
            Assert.AreEqual("@", setting.SqlTextParameterPrefix);
        }
    }
}
