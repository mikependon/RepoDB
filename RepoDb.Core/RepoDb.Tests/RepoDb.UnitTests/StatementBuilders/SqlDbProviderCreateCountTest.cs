using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Attributes;

namespace RepoDb.UnitTests.StatementBuilders
{
    [TestClass]
    public class SqlDbProviderCreateCountTest
    {
        private class TestWithhoutMappingsClass
        {
        }

        [TestMethod]
        public void SqlDbProviderCreateCountWithhoutMappings()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder();

            // Act
            var actual = statementBuilder.CreateCount<TestWithhoutMappingsClass>(queryBuilder, null);
            var expected = $"" +
                $"SELECT COUNT_BIG (1) AS [Counted] " +
                $"FROM [TestWithhoutMappingsClass] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [Map("ClassName")]
        private class TestSqlDbProviderCreateCountWithClassMappingsClass
        {
        }

        [TestMethod]
        public void SqlDbProviderCreateCountWithClassMappings()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder();

            // Act
            var actual = statementBuilder.CreateCount<TestSqlDbProviderCreateCountWithClassMappingsClass>(queryBuilder, null);
            var expected = $"" +
                $"SELECT COUNT_BIG (1) AS [Counted] " +
                $"FROM [ClassName] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        private class TestSqlDbProviderCreateCountWithExpressionsClass
        {
        }

        [TestMethod]
        public void SqlDbProviderCreateCountWithhExpressions()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder();
            var expression = new { Field1 = 1 };

            // Act
            var queryGroup = QueryGroup.Parse(expression);
            var actual = statementBuilder.CreateCount<TestSqlDbProviderCreateCountWithExpressionsClass>(queryBuilder, queryGroup);
            var expected = $"" +
                $"SELECT COUNT_BIG (1) AS [Counted] " +
                $"FROM [TestSqlDbProviderCreateCountWithExpressionsClass] " +
                $"WHERE ([Field1] = @Field1) ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        private class TestSqlDbProviderCreateCountWithExpressionsAndWithTableHintsClass
        {
        }

        [TestMethod]
        public void TestSqlDbProviderCreateCountWithExpressionsAndWithTableHints()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder();
            var expression = new { Field1 = 1 };

            // Act
            var queryGroup = QueryGroup.Parse(expression);
            var actual = statementBuilder.CreateCount<TestSqlDbProviderCreateCountWithExpressionsAndWithTableHintsClass>(queryBuilder, queryGroup, SqlTableHints.NoLock);
            var expected = $"" +
                $"SELECT COUNT_BIG (1) AS [Counted] " +
                $"FROM [TestSqlDbProviderCreateCountWithExpressionsAndWithTableHintsClass] WITH (NOLOCK) " +
                $"WHERE ([Field1] = @Field1) ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }
    }
}
