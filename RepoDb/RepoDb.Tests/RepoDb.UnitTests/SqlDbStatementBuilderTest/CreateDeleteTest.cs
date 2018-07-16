using NUnit.Framework;
using RepoDb.Attributes;

namespace RepoDb.UnitTests.SqlDbStatementBuilderTest
{
    [TestFixture]
    public class CreateDeleteTest
    {
        private class TestCreateDeleteWithoutMappingsClass : DataEntity
        {
        }

        [Test]
        public void TestCreateDeleteWithoutMappings()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<TestCreateDeleteWithoutMappingsClass>();

            // Act
            var actual = statementBuilder.CreateDelete(queryBuilder, null);
            var expected = $"" +
                $"DELETE " +
                $"FROM [TestCreateDeleteWithoutMappingsClass] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        private class TestCreateDeleteWithoutMappingsAndWithExpressionsClass : DataEntity
        {
        }

        [Test]
        public void TestCreateDeleteWithExpressions()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<TestCreateDeleteWithoutMappingsAndWithExpressionsClass>();
            var expression = new { Field1 = 1 };

            // Act
            var queryGroup = QueryGroup.Parse(expression);
            var actual = statementBuilder.CreateDelete(queryBuilder, queryGroup);
            var expected = $"" +
                $"DELETE " +
                $"FROM [TestCreateDeleteWithoutMappingsAndWithExpressionsClass] " +
                $"WHERE ([Field1] = @Field1) ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [Map("ClassName")]
        private class TestCreateDeleteWithMappingsClass : DataEntity
        {
        }

        [Test]
        public void TestCreateDeleteWithMappings()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<TestCreateDeleteWithMappingsClass>();
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
