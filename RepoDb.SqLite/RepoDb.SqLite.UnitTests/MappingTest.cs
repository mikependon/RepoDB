using Microsoft.Data.Sqlite;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RepoDb.SqLite.UnitTests
{
    [TestClass]
    public class MappingTest
    {
        [TestInitialize]
        public void Initialize()
        {
            SqLiteBootstrap.Initialize();
        }

        [TestMethod]
        public void TestSqLiteStatementBuilderMapper()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SqliteConnection>();

            // Assert
            Assert.IsNotNull(builder);
        }

        [TestMethod]
        public void TestSqLiteDbHelperMapper()
        {
            // Setup
            var helper = DbHelperMapper.Get<SqliteConnection>();

            // Assert
            Assert.IsNotNull(helper);
        }

        [TestMethod]
        public void TestSqLiteDbSettingMapper()
        {
            // Setup
            var setting = DbSettingMapper.Get<SqliteConnection>();

            // Assert
            Assert.IsNotNull(setting);
        }
    }
}
