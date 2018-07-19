using NUnit.Framework;
using RepoDb.Attributes;

namespace RepoDb.UnitTests.SqlDbStatementBuilderTest
{
    [TestFixture]
    public class CreateCountTest
    {
        private class TestWithoutMappingsClass : DataEntity
        {
        }

        [Test]
        public void TestWithoutMappings()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<TestWithoutMappingsClass>();

            // Act
            var actual = statementBuilder.CreateCount(queryBuilder, null);
            var expected = $"" +
                $"SELECT COUNT_BIG (1) AS [Counted] " +
                $"FROM [TestWithoutMappingsClass] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [Map("ClassName")]
        private class TestWitClassMappingsClass : DataEntity
        {
        }

        [Test]
        public void TestWitClassMappings()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<TestWitClassMappingsClass>();

            // Act
            var actual = statementBuilder.CreateCount(queryBuilder, null);
            var expected = $"" +
                $"SELECT COUNT_BIG (1) AS [Counted] " +
                $"FROM [ClassName] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        private class TestWithExpressionsClass : DataEntity
        {
        }

        [Test]
        public void TestWithExpressions()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<TestWithExpressionsClass>();
            var expression = new { Field1 = 1 };

            // Act
            var queryGroup = QueryGroup.Parse(expression);
            var actual = statementBuilder.CreateCount(queryBuilder, queryGroup);
            var expected = $"" +
                $"SELECT COUNT_BIG (1) AS [Counted] " +
                $"FROM [TestWithExpressionsClass] " +
                $"WHERE ([Field1] = @Field1) ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }
    }
}
