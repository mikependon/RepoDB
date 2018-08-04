using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Attributes;

namespace RepoDb.UnitTests.SqlDbStatementBuilderTest
{
    [TestClass]
    public class CreateTruncateTest
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

        [TestMethod]
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
