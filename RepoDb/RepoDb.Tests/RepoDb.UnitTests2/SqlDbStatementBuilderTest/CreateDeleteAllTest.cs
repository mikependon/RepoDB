using NUnit.Framework;
using RepoDb.Attributes;

namespace RepoDb.UnitTests.SqlDbStatementBuilderTest
{
    [TestFixture]
    public class CreateDeleteAllTest
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
            var actual = statementBuilder.CreateDeleteAll(queryBuilder);
            var expected = $"" +
                $"DELETE " +
                $"FROM [TestWithoutMappingsClass] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [Map("ClassName")]
        private class TestWithMappingsClass : DataEntity
        {
        }

        [Test]
        public void TestWithMappings()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
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
