using NUnit.Framework;
using RepoDb.Attributes;

namespace RepoDb.UnitTests.SqlDbStatementBuilderTest
{
    [TestFixture]
    public class CreateTruncateTest
    {
        private class TestWithhoutMappingsClass : DataEntity
        {
        }

        [Test]
        public void TestWithhoutMappings()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<TestWithhoutMappingsClass>();

            // Act
            var actual = statementBuilder.CreateTruncate(queryBuilder);
            var expected = $"" +
                $"TRUNCATE TABLE [TestWithhoutMappingsClass] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [Map("ClassName")]
        private class TestWithClassMappingsClass : DataEntity
        {
        }

        [Test]
        public void TestWithClassMappings()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<TestWithClassMappingsClass>();

            // Act
            var actual = statementBuilder.CreateTruncate(queryBuilder);
            var expected = $"" +
                $"TRUNCATE TABLE [ClassName] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }
    }
}
