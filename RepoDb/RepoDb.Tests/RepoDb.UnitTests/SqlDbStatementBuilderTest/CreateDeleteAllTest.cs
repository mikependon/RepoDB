using NUnit.Framework;
using RepoDb.Attributes;

namespace RepoDb.UnitTests.SqlDbStatementBuilderTest
{
    [TestFixture]
    public class CreateDeleteAllTest
    {
        private class TestCreateDeleteAllWithoutMappingsClass : DataEntity
        {
        }

        [Test]
        public void TestCreateDeleteAllWithoutMappings()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<TestCreateDeleteAllWithoutMappingsClass>();

            // Act
            var actual = statementBuilder.CreateDeleteAll(queryBuilder);
            var expected = $"" +
                $"DELETE " +
                $"FROM [TestCreateDeleteAllWithoutMappingsClass] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [Map("ClassName")]
        private class TestCreateDeleteAllWithMappingsClass : DataEntity
        {
        }

        [Test]
        public void TestCreateDeleteAllWithMappings()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<TestCreateDeleteAllWithMappingsClass>();

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
