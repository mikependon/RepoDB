using Microsoft.VisualStudio.TestTools.UnitTesting;
using MySqlConnector;

namespace RepoDb.MySqlConnector.UnitTests
{
    [TestClass]
    public class DbSettingTest
    {
        [TestInitialize]
        public void Initialize()
        {
            GlobalConfiguration
                .Setup()
                .UseMySqlConnector();
        }

        [TestMethod]
        public void TestMySqlDbSettingAreTableHintsSupportedProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<MySqlConnection>();

            // Assert
            Assert.IsFalse(setting.AreTableHintsSupported);
        }

        [TestMethod]
        public void TestMySqlDbSettingClosingQuoteProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<MySqlConnection>();

            // Assert
            Assert.AreEqual("`", setting.ClosingQuote);
        }

        [TestMethod]
        public void TestMySqlDbSettingDefaultSchemaProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<MySqlConnection>();

            // Assert
            Assert.IsNull(setting.DefaultSchema);
        }

        [TestMethod]
        public void TestMySqlDbSettingIsDirectionSupportedProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<MySqlConnection>();

            // Assert
            Assert.IsFalse(setting.IsDirectionSupported);
        }

        [TestMethod]
        public void TestMySqlDbSettingIsExecuteReaderDisposableProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<MySqlConnection>();

            // Assert
            Assert.IsTrue(setting.IsExecuteReaderDisposable);
        }

        [TestMethod]
        public void TestMySqlDbSettingIsMultiStatementExecutableProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<MySqlConnection>();

            // Assert
            Assert.IsTrue(setting.IsMultiStatementExecutable);
        }

        [TestMethod]
        public void TestMySqlDbSettingIsUseUpsertProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<MySqlConnection>();

            // Assert
            Assert.IsFalse(setting.IsUseUpsert);
        }

        [TestMethod]
        public void TestMySqlDbSettingOpeningQuoteProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<MySqlConnection>();

            // Assert
            Assert.AreEqual("`", setting.OpeningQuote);
        }

        [TestMethod]
        public void TestMySqlDbSettingParameterPrefixProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<MySqlConnection>();

            // Assert
            Assert.AreEqual("@", setting.ParameterPrefix);
        }

        [TestMethod]
        public void TestMySqlDbSettingSqlTextParameterPrefixProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<MySqlConnection>();

            // Assert
            Assert.AreEqual("@", setting.SqlTextParameterPrefix);
        }
    }
}
