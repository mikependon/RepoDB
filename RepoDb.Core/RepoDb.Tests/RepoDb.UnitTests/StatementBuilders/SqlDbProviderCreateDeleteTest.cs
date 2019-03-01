using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Attributes;

namespace RepoDb.UnitTests.StatementBuilders
{
    [TestClass]
    public class SqlDbProviderCreateDeleteTest
    {
        private class TestSqlDbProviderCreateDeleteWithoutMappingsClass
        {
        }

        [TestMethod]
        public void TestSqlDbProviderCreateDeleteWithoutMappings()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder<TestSqlDbProviderCreateDeleteWithoutMappingsClass>();

            // Act
            var actual = statementBuilder.CreateDelete(queryBuilder, null);
            var expected = $"" +
                $"DELETE " +
                $"FROM [TestSqlDbProviderCreateDeleteWithoutMappingsClass] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        private class TestSqlDbProviderCreateDeleteWithoutMappingsAndWithExpressionsClass
        {
        }

        [TestMethod]
        public void TestSqlDbProviderCreateDeleteWithExpressions()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder<TestSqlDbProviderCreateDeleteWithoutMappingsAndWithExpressionsClass>();
            var expression = new { Field1 = 1 };

            // Act
            var queryGroup = QueryGroup.Parse(expression);
            var actual = statementBuilder.CreateDelete(queryBuilder, queryGroup);
            var expected = $"" +
                $"DELETE " +
                $"FROM [TestSqlDbProviderCreateDeleteWithoutMappingsAndWithExpressionsClass] " +
                $"WHERE ([Field1] = @Field1) ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [Map("ClassName")]
        private class TestSqlDbProviderCreateDeleteWithMappingsClass
        {
        }

        [TestMethod]
        public void TestSqlDbProviderCreateDeleteWithMappings()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder<TestSqlDbProviderCreateDeleteWithMappingsClass>();
            var expression = new { Field1 = 1 };

            // Act
            var queryGroup = QueryGroup.Parse(expression);
            var actual = statementBuilder.CreateDelete(queryBuilder, queryGroup);
            var expected = $"" +
                $"DELETE " +
                $"FROM [ClassName] " +
                $"WHERE ([Field1] = @Field1) ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }
    }
}
