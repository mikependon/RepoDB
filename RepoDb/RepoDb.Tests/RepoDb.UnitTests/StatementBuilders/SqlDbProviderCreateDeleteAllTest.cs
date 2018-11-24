using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Attributes;

namespace RepoDb.UnitTests.StatementBuilders
{
    [TestClass]
    public class SqlDbProviderCreateDeleteAllTest
    {
        private class TestWithoutMappingsClass
        {
        }

        [TestMethod]
        public void TestWithoutMappings()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder<TestWithoutMappingsClass>();

            // Act
            var actual = statementBuilder.CreateDeleteAll(queryBuilder);
            var expected = $"" +
                $"DELETE " +
                $"FROM [TestWithoutMappingsClass] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [Map("ClassName")]
        private class TestWithMappingsClass
        {
        }

        [TestMethod]
        public void TestWithMappings()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder<TestWithMappingsClass>();

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
