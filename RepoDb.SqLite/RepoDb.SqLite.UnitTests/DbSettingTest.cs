using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.SQLite;

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
        public void TestDbSettingAreTableHintsSupportedProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<SQLiteConnection>();

            // Assert
            Assert.IsFalse(setting.AreTableHintsSupported);
        }

        [TestMethod]
        public void TestDbSettingClosingQuoteProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<SQLiteConnection>();

            // Assert
            Assert.AreEqual("]", setting.ClosingQuote);
        }

        [TestMethod]
        public void TestDbSettingDefaultAverageableTypeProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<SQLiteConnection>();

            // Assert
            Assert.AreEqual(typeof(double), setting.DefaultAverageableType);
        }

        [TestMethod]
        public void TestDbSettingDefaultSchemaProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<SQLiteConnection>();

            // Assert
            Assert.IsNull(setting.DefaultSchema);
        }

        [TestMethod]
        public void TestDbSettingIsDbParameterDirectionSettingSupportedProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<SQLiteConnection>();

            // Assert
            Assert.IsFalse(setting.IsDbParameterDirectionSettingSupported);
        }

        [TestMethod]
        public void TestDbSettingIsDisposeDbCommandAfterExecuteReaderProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<SQLiteConnection>();

            // Assert
            Assert.IsTrue(setting.IsDisposeDbCommandAfterExecuteReader);
        }

        [TestMethod]
        public void TestDbSettingIsMultipleStatementExecutionSupportedProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<SQLiteConnection>();

            // Assert
            Assert.IsTrue(setting.IsMultipleStatementExecutionSupported);
        }

        [TestMethod]
        public void TestDbSettingIsUseUpsertForMergeOperationProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<SQLiteConnection>();

            // Assert
            Assert.IsFalse(setting.IsUseUpsertForMergeOperation);
        }

        [TestMethod]
        public void TestDbSettingOpeningQuoteProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<SQLiteConnection>();

            // Assert
            Assert.AreEqual("[", setting.OpeningQuote);
        }

        [TestMethod]
        public void TestDbSettingParameterPrefixProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<SQLiteConnection>();

            // Assert
            Assert.AreEqual("@", setting.ParameterPrefix);
        }

        [TestMethod]
        public void TestDbSettingSchemaSeparatorProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<SQLiteConnection>();

            // Assert
            Assert.AreEqual(".", setting.SchemaSeparator);
        }
    }
}
