using Microsoft.VisualStudio.TestTools.UnitTesting;
using IBM.Data.Db2;

namespace RepoDb.Db2.UnitTests
{
    [TestClass]
    public class DbSettingTest
    {
        [TestInitialize]
        public void Initialize()
        {
            GlobalConfiguration
                .Setup()
                .UseDb2();
        }

        [TestMethod]
        public void TestDb2DbSettingAreTableHintsSupportedProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<DB2Connection>();

            // Assert
            Assert.IsFalse(setting.AreTableHintsSupported);
        }

        [TestMethod]
        public void TestDb2DbSettingAverageableTypeProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<DB2Connection>();

            // Assert
            Assert.AreEqual(typeof(double), setting.AverageableType);
        }

        [TestMethod]
        public void TestDb2DbSettingClosingQuoteProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<DB2Connection>();

            // Assert
            Assert.AreEqual("\"", setting.ClosingQuote);
        }

        [TestMethod]
        public void TestDb2DbSettingDefaultSchemaProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<DB2Connection>();

            // Assert
            Assert.IsNull(setting.DefaultSchema);
        }

        [TestMethod]
        public void TestDb2DbSettingIsDirectionSupportedSupportedProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<DB2Connection>();

            // Assert
            Assert.IsTrue(setting.IsDirectionSupported);
        }

        [TestMethod]
        public void TestDb2DbSettingIsExecuteReaderDisposableProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<DB2Connection>();

            // Assert
            Assert.IsTrue(setting.IsExecuteReaderDisposable);
        }

        [TestMethod]
        public void TestDb2DbSettingIsMultiStatementExecutableProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<DB2Connection>();

            // Assert
            Assert.IsFalse(setting.IsMultiStatementExecutable);
        }

        [TestMethod]
        public void TestDb2DbSettingIsUseUpsertProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<DB2Connection>();

            // Assert
            Assert.IsFalse(setting.IsUseUpsert);
        }

        [TestMethod]
        public void TestDb2DbSettingIsPreparableProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<DB2Connection>();

            // Assert
            Assert.IsTrue(setting.IsPreparable);
        }

        [TestMethod]
        public void TestDb2DbSettingOpeningQuoteProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<DB2Connection>();

            // Assert
            Assert.AreEqual("\"", setting.OpeningQuote);
        }

        [TestMethod]
        public void TestDb2DbSettingParameterPrefixProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<DB2Connection>();

            // Assert
            Assert.AreEqual(":", setting.ParameterPrefix);
        }
    }
}
