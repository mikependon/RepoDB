using Microsoft.VisualStudio.TestTools.UnitTesting;
using MySql.Data.MySqlClient;

namespace RepoDb.MySql.UnitTests
{
    [TestClass]
    public class DbSettingTest
    {
        [TestInitialize]
        public void Initialize()
        {
            MySqlBootstrap.Initialize();
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
        public void TestMySqlDbSettingDefaultAverageableTypeProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<MySqlConnection>();

            // Assert
            Assert.AreEqual(typeof(double), setting.DefaultAverageableType);
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
        public void TestMySqlDbSettingIsDbParameterDirectionSettingSupportedProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<MySqlConnection>();

            // Assert
            Assert.IsFalse(setting.IsDbParameterDirectionSettingSupported);
        }

        [TestMethod]
        public void TestMySqlDbSettingIsDisposeDbCommandAfterExecuteReaderProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<MySqlConnection>();

            // Assert
            Assert.IsTrue(setting.IsDisposeDbCommandAfterExecuteReader);
        }

        [TestMethod]
        public void TestMySqlDbSettingIsMultipleStatementExecutionSupportedProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<MySqlConnection>();

            // Assert
            Assert.IsTrue(setting.IsMultipleStatementExecutionSupported);
        }

        [TestMethod]
        public void TestMySqlDbSettingIsUseUpsertForMergeOperationProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<MySqlConnection>();

            // Assert
            Assert.IsFalse(setting.IsUseUpsertForMergeOperation);
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
        public void TestMySqlDbSettingSchemaSeparatorProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<MySqlConnection>();

            // Assert
            Assert.AreEqual(".", setting.SchemaSeparator);
        }
    }
}
