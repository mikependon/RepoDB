using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Attributes;

namespace RepoDb.UnitTests.SqlDbStatementBuilderTest
{
    [TestClass]
    public class CreateCountTest
    {
        private class TestWithhoutMappingsClass : DataEntity
        {
        }

        [TestMethod]
        public void TestWithhoutMappings()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
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
        private class TestWithClassMappingsClass : DataEntity
        {
        }

        [TestMethod]
        public void TestWithClassMappings()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<TestWithClassMappingsClass>();

            // Act
            var actual = statementBuilder.CreateCount(queryBuilder, null);
            var expected = $"" +
                $"SELECT COUNT_BIG (1) AS [Counted] " +
                $"FROM [ClassName] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        private class TestWithhExpressionsClass : DataEntity
        {
        }

        [TestMethod]
        public void TestWithhExpressions()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<TestWithhExpressionsClass>();
            var expression = new { Field1 = 1 };

            // Act
            var queryGroup = QueryGroup.Parse(expression);
            var actual = statementBuilder.CreateCount(queryBuilder, queryGroup);
            var expected = $"" +
                $"SELECT COUNT_BIG (1) AS [Counted] " +
                $"FROM [TestWithhExpressionsClass] " +
                $"WHERE ([Field1] = @Field1) ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }
    }
}
