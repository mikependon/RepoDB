using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Attributes;

namespace RepoDb.UnitTests.StatementBuilders
{
    [TestClass]
    public class SqlDbProviderCreateDeleteAllTest
    {
        private class TestSqlDbProviderCreateDeleteAllWithoutMappingsClass
        {
        }

        [TestMethod]
        public void TestSqlDbProviderCreateDeleteAllWithoutMappings()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder<TestSqlDbProviderCreateDeleteAllWithoutMappingsClass>();

            // Act
            var actual = statementBuilder.CreateDeleteAll(queryBuilder);
            var expected = $"" +
                $"DELETE " +
                $"FROM [TestSqlDbProviderCreateDeleteAllWithoutMappingsClass] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [Map("ClassName")]
        private class TestSqlDbProviderCreateDeleteAllWithMappingsClass
        {
        }

        [TestMethod]
        public void TestSqlDbProviderCreateDeleteAllWithMappings()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder<TestSqlDbProviderCreateDeleteAllWithMappingsClass>();

            // Act
            var actual = statementBuilder.CreateDeleteAll(queryBuilder);
            var expected = $"" +
                $"DELETE " +
                $"FROM [ClassName] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }
    }
}
