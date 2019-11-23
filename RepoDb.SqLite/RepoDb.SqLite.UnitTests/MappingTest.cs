using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.SQLite;

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
            var builder = StatementBuilderMapper.Get<SQLiteConnection>();

            // Assert
            Assert.IsNotNull(builder);
        }

        [TestMethod]
        public void TestSqLiteDbHelperMapper()
        {
            // Setup
            var helper = DbHelperMapper.Get<SQLiteConnection>();

            // Assert
            Assert.IsNotNull(helper);
        }

        [TestMethod]
        public void TestSqLiteDbSettingMapper()
        {
            // Setup
            var setting = DbSettingMapper.Get<SQLiteConnection>();

            // Assert
            Assert.IsNotNull(setting);
        }
    }
}
