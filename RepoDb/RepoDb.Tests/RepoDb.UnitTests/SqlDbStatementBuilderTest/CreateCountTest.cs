using NUnit.Framework;
using RepoDb.Attributes;

namespace RepoDb.UnitTests.SqlDbStatementBuilderTest
{
    [TestFixture]
    public class CreateCountTest
    {
        private class TestCreateCountWithoutMappingsClass : DataEntity
        {
        }

        [Test]
        public void TestCreateCountWithoutMappings()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<TestCreateCountWithoutMappingsClass>();

            // Act
            var actual = statementBuilder.CreateCount(queryBuilder, null);
            var expected = $"" +
                $"SELECT COUNT_BIG (1) AS [Counted] " +
                $"FROM [TestCreateCountWithoutMappingsClass] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [Map("ClassName")]
        private class TestCreateCountWitClassMappingsClass : DataEntity
        {
        }

        [Test]
        public void TestCreateCountWitClassMappings()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<TestCreateCountWitClassMappingsClass>();

            // Act
            var actual = statementBuilder.CreateCount(queryBuilder, null);
            var expected = $"" +
                $"SELECT COUNT_BIG (1) AS [Counted] " +
                $"FROM [ClassName] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        private class TestCreateCountWithExpressionsClass : DataEntity
        {
        }

        [Test]
        public void TestCountWithExpressions()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<TestCreateCountWithExpressionsClass>();
            var expression = new { Field1 = 1 };

            // Act
            var queryGroup = QueryGroup.Parse(expression);
            var actual = statementBuilder.CreateCount(queryBuilder, queryGroup);
            var expected = $"" +
                $"SELECT COUNT_BIG (1) AS [Counted] " +
                $"FROM [TestCreateCountWithExpressionsClass] " +
                $"WHERE ([Field1] = @Field1) ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }
    }
}
