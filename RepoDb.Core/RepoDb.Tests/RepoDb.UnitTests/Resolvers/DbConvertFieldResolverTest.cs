using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.DbSettings;
using RepoDb.Resolvers;

namespace RepoDb.UnitTests.Resolvers
{
    [TestClass]
    public class DbConvertFieldResolverTest
    {
        private readonly DbConvertFieldResolver m_resolver = new DbConvertFieldResolver(new ClientTypeToDbTypeResolver(), new DbTypeToSqlServerStringNameResolver());

        [TestMethod]
        public void TestDbConvertFieldResolverForColumnName()
        {
            // Setup
            var setting = new SqlServerDbSetting();
            var field = new Field("Id");

            // Act
            var result = m_resolver.Resolve(field, setting);
            var expected = "[Id]";

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestDbConvertFieldResolverForColumnWithType()
        {
            // Setup
            var setting = new SqlServerDbSetting();
            var field = new Field("Id", typeof(string));

            // Act
            var result = m_resolver.Resolve(field, setting);
            var expected = "CAST([Id] AS [NVARCHAR])";

            // Assert
            Assert.AreEqual(expected, result);
        }
    }
}
