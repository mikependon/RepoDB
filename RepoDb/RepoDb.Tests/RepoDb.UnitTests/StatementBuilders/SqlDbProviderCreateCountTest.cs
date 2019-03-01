using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Attributes;

namespace RepoDb.UnitTests.StatementBuilders
{
    [TestClass]
    public class SqlDbProviderCreateCountTest
    {
        private class TestWithhoutMappingsClass
        {
        }

        [TestMethod]
        public void SqlDbProviderCreateCountWithhoutMappings()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder<TestWithhoutMappingsClass>();

            // Act
            var actual = statementBuilder.CreateCount(queryBuilder, null);
            var expected = $"" +
                $"SELECT COUNT_BIG (1) AS [Counted] " +
                $"FROM [TestWithhoutMappingsClass] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [Map("ClassName")]
        private class TestSqlDbProviderCreateCountWithClassMappingsClass
        {
        }

        [TestMethod]
        public void SqlDbProviderCreateCountWithClassMappings()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder<TestSqlDbProviderCreateCountWithClassMappingsClass>();

            // Act
            var actual = statementBuilder.CreateCount(queryBuilder, null);
            var expected = $"" +
                $"SELECT COUNT_BIG (1) AS [Counted] " +
                $"FROM [ClassName] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        private class TestSqlDbProviderCreateCountWithExpressionsClass
        {
        }

        [TestMethod]
        public void SqlDbProviderCreateCountWithhExpressions()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder<TestSqlDbProviderCreateCountWithExpressionsClass>();
            var expression = new { Field1 = 1 };

            // Act
            var queryGroup = QueryGroup.Parse(expression);
            var actual = statementBuilder.CreateCount(queryBuilder, queryGroup);
            var expected = $"" +
                $"SELECT COUNT_BIG (1) AS [Counted] " +
                $"FROM [TestSqlDbProviderCreateCountWithExpressionsClass] " +
                $"WHERE ([Field1] = @Field1) ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }
    }
}
