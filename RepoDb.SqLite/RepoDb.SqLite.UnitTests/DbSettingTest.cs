using Microsoft.Data.Sqlite;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RepoDb.SqLite.UnitTests
{
    [TestClass]
    public class DbSettingTest
    {
        [TestInitialize]
        public void Initialize()
        {
            SqLiteBootstrap.Initialize();
        }

        [TestMethod]
        public void TestSqLiteDbSettingAreTableHintsSupportedProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<SqliteConnection>();

            // Assert
            Assert.IsFalse(setting.AreTableHintsSupported);
        }

        [TestMethod]
        public void TestSqLiteDbSettingAverageableTypeProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<SqliteConnection>();

            // Assert
            Assert.AreEqual(typeof(double), setting.AverageableType);
        }

        [TestMethod]
        public void TestSqLiteDbSettingClosingQuoteProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<SqliteConnection>();

            // Assert
            Assert.AreEqual("]", setting.ClosingQuote);
        }

        [TestMethod]
        public void TestSqLiteDbSettingDefaultSchemaProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<SqliteConnection>();

            // Assert
            Assert.IsNull(setting.DefaultSchema);
        }

        [TestMethod]
        public void TestSqLiteDbSettingIsDirectionSupportedSupportedProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<SqliteConnection>();

            // Assert
            Assert.IsFalse(setting.IsDirectionSupported);
        }

        [TestMethod]
        public void TestSqLiteDbSettingIsExecuteReaderDisposableProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<SqliteConnection>();

            // Assert
            Assert.IsFalse(setting.IsExecuteReaderDisposable);
        }

        [TestMethod]
        public void TestSqLiteDbSettingIsMultiStatementExecutableProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<SqliteConnection>();

            // Assert
            Assert.IsTrue(setting.IsMultiStatementExecutable);
        }

        [TestMethod]
        public void TestSqLiteDbSettingIsUseUpsertProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<SqliteConnection>();

            // Assert
            Assert.IsFalse(setting.IsUseUpsert);
        }

        [TestMethod]
        public void TestSqLiteDbSettingOpeningQuoteProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<SqliteConnection>();

            // Assert
            Assert.AreEqual("[", setting.OpeningQuote);
        }

        [TestMethod]
        public void TestSqLiteDbSettingParameterPrefixProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<SqliteConnection>();

            // Assert
            Assert.AreEqual("@", setting.ParameterPrefix);
        }
    }
}
