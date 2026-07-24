using Microsoft.VisualStudio.TestTools.UnitTesting;
using Oracle.ManagedDataAccess.Client;

namespace RepoDb.Oracle.UnitTests
{
    [TestClass]
    public class DbSettingTest
    {
        [TestInitialize]
        public void Initialize()
        {
            GlobalConfiguration
                .Setup()
                .UseOracle();
        }

        [TestMethod]
        public void TestOracleDbSettingAreTableHintsSupportedProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<OracleConnection>();

            // Assert
            Assert.IsFalse(setting.AreTableHintsSupported);
        }

        [TestMethod]
        public void TestOracleDbSettingAverageableTypeProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<OracleConnection>();

            // Assert
            Assert.AreEqual(typeof(double), setting.AverageableType);
        }

        [TestMethod]
        public void TestOracleDbSettingClosingQuoteProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<OracleConnection>();

            // Assert
            Assert.AreEqual("\"", setting.ClosingQuote);
        }

        [TestMethod]
        public void TestOracleDbSettingDefaultSchemaProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<OracleConnection>();

            // Assert
            Assert.IsNull(setting.DefaultSchema);
        }

        [TestMethod]
        public void TestOracleDbSettingIsDirectionSupportedSupportedProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<OracleConnection>();

            // Assert
            Assert.IsTrue(setting.IsDirectionSupported);
        }

        [TestMethod]
        public void TestOracleDbSettingIsExecuteReaderDisposableProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<OracleConnection>();

            // Assert
            Assert.IsTrue(setting.IsExecuteReaderDisposable);
        }

        [TestMethod]
        public void TestOracleDbSettingIsMultiStatementExecutableProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<OracleConnection>();

            // Assert
            Assert.IsFalse(setting.IsMultiStatementExecutable);
        }

        [TestMethod]
        public void TestOracleDbSettingIsUseUpsertProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<OracleConnection>();

            // Assert
            Assert.IsFalse(setting.IsUseUpsert);
        }

        [TestMethod]
        public void TestOracleDbSettingOpeningQuoteProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<OracleConnection>();

            // Assert
            Assert.AreEqual("\"", setting.OpeningQuote);
        }

        [TestMethod]
        public void TestOracleDbSettingParameterPrefixProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<OracleConnection>();

            // Assert
            Assert.AreEqual(":", setting.ParameterPrefix);
        }
    }
}
