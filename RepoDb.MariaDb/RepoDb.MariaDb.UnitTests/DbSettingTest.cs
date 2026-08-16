using Microsoft.VisualStudio.TestTools.UnitTesting;
using MySql.Data.MySqlClient;

namespace RepoDb.MariaDb.UnitTests
{
    [TestClass]
    public class DbSettingTest
    {
        [TestInitialize]
        public void Initialize()
        {
            GlobalConfiguration
                .Setup()
                .UseMariaDb();
        }

        [TestMethod]
        public void TestMariaDbDbSettingAreTableHintsSupportedProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<MySqlConnection>();

            // Assert
            Assert.IsFalse(setting.AreTableHintsSupported);
        }

        [TestMethod]
        public void TestMariaDbDbSettingClosingQuoteProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<MySqlConnection>();

            // Assert
            Assert.AreEqual("`", setting.ClosingQuote);
        }

        [TestMethod]
        public void TestMariaDbDbSettingAverageableTypeProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<MySqlConnection>();

            // Assert
            Assert.AreEqual(typeof(double), setting.AverageableType);
        }

        [TestMethod]
        public void TestMariaDbDbSettingDefaultSchemaProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<MySqlConnection>();

            // Assert
            Assert.IsNull(setting.DefaultSchema);
        }

        [TestMethod]
        public void TestMariaDbDbSettingIsDirectionSupportedProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<MySqlConnection>();

            // Assert
            Assert.IsFalse(setting.IsDirectionSupported);
        }

        [TestMethod]
        public void TestMariaDbDbSettingIsExecuteReaderDisposableProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<MySqlConnection>();

            // Assert
            Assert.IsFalse(setting.IsExecuteReaderDisposable);
        }

        [TestMethod]
        public void TestMariaDbDbSettingIsMultiStatementExecutableProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<MySqlConnection>();

            // Assert
            Assert.IsTrue(setting.IsMultiStatementExecutable);
        }

        [TestMethod]
        public void TestMariaDbDbSettingIsUseUpsertProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<MySqlConnection>();

            // Assert
            Assert.IsFalse(setting.IsUseUpsert);
        }

        [TestMethod]
        public void TestMariaDbDbSettingOpeningQuoteProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<MySqlConnection>();

            // Assert
            Assert.AreEqual("`", setting.OpeningQuote);
        }

        [TestMethod]
        public void TestMariaDbDbSettingParameterPrefixProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<MySqlConnection>();

            // Assert
            Assert.AreEqual("@", setting.ParameterPrefix);
        }
    }
}
