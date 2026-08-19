using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClickHouse.Driver.ADO;

namespace RepoDb.ClickHouse.UnitTests
{
    [TestClass]
    public class DbSettingTest
    {
        [TestInitialize]
        public void Initialize()
        {
            GlobalConfiguration
                .Setup()
                .UseClickHouse();
        }

        [TestMethod]
        public void TestClickHouseDbSettingAreTableHintsSupportedProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<RepoDbClickHouseConnection>();

            // Assert
            Assert.IsFalse(setting.AreTableHintsSupported);
        }

        [TestMethod]
        public void TestClickHouseDbSettingClosingQuoteProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<RepoDbClickHouseConnection>();

            // Assert
            Assert.AreEqual("`", setting.ClosingQuote);
        }

        [TestMethod]
        public void TestClickHouseDbSettingAverageableTypeProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<RepoDbClickHouseConnection>();

            // Assert
            Assert.AreEqual(typeof(double), setting.AverageableType);
        }

        [TestMethod]
        public void TestClickHouseDbSettingDefaultSchemaProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<RepoDbClickHouseConnection>();

            // Assert
            Assert.IsNull(setting.DefaultSchema);
        }

        [TestMethod]
        public void TestClickHouseDbSettingIsDirectionSupportedProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<RepoDbClickHouseConnection>();

            // Assert
            Assert.IsFalse(setting.IsDirectionSupported);
        }

        [TestMethod]
        public void TestClickHouseDbSettingIsExecuteReaderDisposableProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<RepoDbClickHouseConnection>();

            // Assert
            Assert.IsFalse(setting.IsExecuteReaderDisposable);
        }

        [TestMethod]
        public void TestClickHouseDbSettingIsMultiStatementExecutableProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<RepoDbClickHouseConnection>();

            // Assert
            Assert.IsFalse(setting.IsMultiStatementExecutable);
        }

        [TestMethod]
        public void TestClickHouseDbSettingIsUseUpsertProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<RepoDbClickHouseConnection>();

            // Assert
            Assert.IsFalse(setting.IsUseUpsert);
        }

        [TestMethod]
        public void TestClickHouseDbSettingOpeningQuoteProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<RepoDbClickHouseConnection>();

            // Assert
            Assert.AreEqual("`", setting.OpeningQuote);
        }

        [TestMethod]
        public void TestClickHouseDbSettingParameterPrefixProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<RepoDbClickHouseConnection>();

            // Assert
            Assert.AreEqual("@", setting.ParameterPrefix);
        }
    }
}
