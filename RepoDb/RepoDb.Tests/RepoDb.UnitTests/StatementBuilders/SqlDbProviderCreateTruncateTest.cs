using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Attributes;

namespace RepoDb.UnitTests.StatementBuilders
{
    [TestClass]
    public class SqlDbProviderCreateTruncateTest
    {
        private class TestSqlDbProviderCreateTruncateWithhoutMappingsClass
        {
        }

        [TestMethod]
        public void TestSqlDbProviderCreateTruncateWithhoutMappings()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder<TestSqlDbProviderCreateTruncateWithhoutMappingsClass>();

            // Act
            var actual = statementBuilder.CreateTruncate(queryBuilder);
            var expected = $"" +
                $"TRUNCATE TABLE [TestSqlDbProviderCreateTruncateWithhoutMappingsClass] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [Map("ClassName")]
        private class TestSqlDbProviderCreateTruncateWithClassMappingsClass
        {
        }

        [TestMethod]
        public void TestSqlDbProviderCreateTruncateWithClassMappings()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder<TestSqlDbProviderCreateTruncateWithClassMappingsClass>();

            // Act
            var actual = statementBuilder.CreateTruncate(queryBuilder);
            var expected = $"" +
                $"TRUNCATE TABLE [ClassName] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }
    }
}
