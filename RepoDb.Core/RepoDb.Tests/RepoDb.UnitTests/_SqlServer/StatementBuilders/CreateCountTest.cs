using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.SqlClient;

namespace RepoDb.UnitTests.StatementBuilders
{
    [TestClass]
    public class SqlServerStatementBuilderCreateCountTest
    {
        [TestMethod]
        public void TestSqlServerStatementBuilderCreateCount()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get(typeof(SqlConnection));
            var queryBuilder = new QueryBuilder();
            var tableName = "Table";

            // Act
            var actual = statementBuilder.CreateCount(queryBuilder: queryBuilder,
                tableName: tableName,
                hints: null);
            var expected = "SELECT COUNT_BIG (*) AS [CountValue] FROM [Table] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestSqlServerStatementBuilderCreateCountWithWhereExpression()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get(typeof(SqlConnection));
            var queryBuilder = new QueryBuilder();
            var tableName = "Table";
            var where = new QueryGroup(new QueryField("Id", 1));

            // Act
            var actual = statementBuilder.CreateCount(queryBuilder: queryBuilder,
                tableName: tableName,
                where: where);
            var expected = $"" +
                $"SELECT COUNT_BIG (*) AS [CountValue] " +
                $"FROM [Table] " +
                $"WHERE ([Id] = @Id) ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestSqlServerStatementBuilderCreateCountWithHints()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get(typeof(SqlConnection));
            var queryBuilder = new QueryBuilder();
            var tableName = "Table";
            var hints = "WITH (NOLOCK)";

            // Act
            var actual = statementBuilder.CreateCount(queryBuilder: queryBuilder,
                tableName: tableName,
                hints: hints);
            var expected = "SELECT COUNT_BIG (*) AS [CountValue] FROM [Table] WITH (NOLOCK) ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestSqlServerStatementBuilderCreateCountWithWhereExpressionAndWithHints()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get(typeof(SqlConnection));
            var queryBuilder = new QueryBuilder();
            var tableName = "Table";
            var where = new QueryGroup(new QueryField("Id", 1));
            var hints = "WITH (NOLOCK)";

            // Act
            var actual = statementBuilder.CreateCount(queryBuilder: queryBuilder,
                tableName: tableName,
                where: where,
                hints: hints);
            var expected = $"" +
                $"SELECT COUNT_BIG (*) AS [CountValue] " +
                $"FROM [Table] WITH (NOLOCK) " +
                $"WHERE ([Id] = @Id) ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestSqlServerStatementBuilderCreateCountWithQuotedTableSchema()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get(typeof(SqlConnection));
            var queryBuilder = new QueryBuilder();
            var tableName = "[dbo].[Table]";

            // Act
            var actual = statementBuilder.CreateCount(queryBuilder: queryBuilder,
                tableName: tableName,
                hints: null);
            var expected = "SELECT COUNT_BIG (*) AS [CountValue] FROM [dbo].[Table] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestSqlServerStatementBuilderCreateCountWithUnquotedTableSchema()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get(typeof(SqlConnection));
            var queryBuilder = new QueryBuilder();
            var tableName = "dbo.Table";

            // Act
            var actual = statementBuilder.CreateCount(queryBuilder: queryBuilder,
                tableName: tableName,
                hints: null);
            var expected = "SELECT COUNT_BIG (*) AS [CountValue] FROM [dbo].[Table] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }
    }
}
