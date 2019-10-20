using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Extensions;
using System.Data.SQLite;

namespace RepoDb.SqLite.UnitTests
{
    [TestClass]
    public class QueryGroupTest
    {
        [TestInitialize]
        public void Initialize()
        {
            Initializer.Initialize();
        }

        [TestMethod]
        public void TestQueryGroupQuoting()
        {
            // Setup
            var queryGroup = new QueryGroup(new QueryField("Name", "Michael"));
            var dbSetting = DbSettingMapper.Get<SQLiteConnection>();

            // Act
            var expected = "Name".AsQuoted(dbSetting);
            var actual = queryGroup.GetString(dbSetting);

            // Assert
            Assert.AreEqual(expected, actual);
        }
    }
}
