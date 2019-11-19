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
            Bootstrap.Initialize();
        }

        [TestMethod]
        public void TestStatementBuilderMapper()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SQLiteConnection>();

            // Assert
            Assert.IsNotNull(builder);
        }

        [TestMethod]
        public void TestDbHelperMapper()
        {
            // Setup
            var helper = DbHelperMapper.Get<SQLiteConnection>();

            // Assert
            Assert.IsNotNull(helper);
        }

        [TestMethod]
        public void TestDbSettingMapper()
        {
            // Setup
            var setting = DbSettingMapper.Get<SQLiteConnection>();

            // Assert
            Assert.IsNotNull(setting);
        }
    }
}
