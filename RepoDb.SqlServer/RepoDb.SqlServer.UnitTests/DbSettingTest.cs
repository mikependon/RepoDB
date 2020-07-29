using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RepoDb.SqlServer.UnitTests
{
    [TestClass]
    public class DbSettingTest
    {
        [TestInitialize]
        public void Initialize()
        {
            SqlServerBootstrap.Initialize();
        }

        [TestMethod]
        public void TestSqlServerDbSettingAreTableHintsSupportedProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<SqlConnection>();

            // Assert
            Assert.IsTrue(setting.AreTableHintsSupported);
        }

        [TestMethod]
        public void TestSqlServerDbSettingAverageableTypeProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<SqlConnection>();

            // Assert
            Assert.AreEqual(typeof(double), setting.AverageableType);
        }

        [TestMethod]
        public void TestSqlServerDbSettingClosingQuoteProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<SqlConnection>();

            // Assert
            Assert.AreEqual("]", setting.ClosingQuote);
        }

        [TestMethod]
        public void TestSqlServerDbSettingDefaultSchemaProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<SqlConnection>();

            // Assert
            Assert.AreEqual("dbo", setting.DefaultSchema);
        }

        [TestMethod]
        public void TestSqlServerDbSettingIsDirectionSupportedSupportedProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<SqlConnection>();

            // Assert
            Assert.IsTrue(setting.IsDirectionSupported);
        }

        [TestMethod]
        public void TestSqlServerDbSettingIsExecuteReaderDisposableProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<SqlConnection>();

            // Assert
            Assert.IsTrue(setting.IsExecuteReaderDisposable);
        }

        [TestMethod]
        public void TestSqlServerDbSettingIsMultiStatementExecutableProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<SqlConnection>();

            // Assert
            Assert.IsTrue(setting.IsMultiStatementExecutable);
        }

        [TestMethod]
        public void TestSqlServerDbSettingIsUseUpsertProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<SqlConnection>();

            // Assert
            Assert.IsFalse(setting.IsUseUpsert);
        }

        [TestMethod]
        public void TestSqlServerDbSettingOpeningQuoteProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<SqlConnection>();

            // Assert
            Assert.AreEqual("[", setting.OpeningQuote);
        }

        [TestMethod]
        public void TestSqlServerDbSettingParameterPrefixProperty()
        {
            // Setup
            var setting = DbSettingMapper.Get<SqlConnection>();

            // Assert
            Assert.AreEqual("@", setting.ParameterPrefix);
        }
    }
}
