using NUnit.Framework;
using RepoDb.Attributes;

namespace RepoDb.UnitTests.SqlDbStatementBuilderTest
{
    [TestFixture]
    public class CreateTruncateTest
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
            var actual = statementBuilder.CreateTruncate(queryBuilder);
            var expected = $"" +
                $"TRUNCATE TABLE [TestCreateCountWithoutMappingsClass] ;";

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
            var actual = statementBuilder.CreateTruncate(queryBuilder);
            var expected = $"" +
                $"TRUNCATE TABLE [ClassName] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }
    }
}
