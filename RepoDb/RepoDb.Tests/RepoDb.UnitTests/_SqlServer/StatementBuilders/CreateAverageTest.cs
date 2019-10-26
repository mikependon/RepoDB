using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Data.SqlClient;

namespace RepoDb.UnitTests.StatementBuilders
{
    [TestClass]
    public class SqlStatementBuilderCreateAverageTest
    {
        [TestMethod]
        public void TestSqlStatementBuilderCreateAverage()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get(typeof(SqlConnection));
            var queryBuilder = new QueryBuilder();
            var tableName = "Table";
            var field = new Field("Value");

            // Act
            var actual = statementBuilder.CreateAverage(queryBuilder: queryBuilder,
                field: field,
                tableName: tableName,
                hints: null);
            var expected = "SELECT AVG (CONVERT([FLOAT], [Value])) AS [AverageValue] FROM [Table] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestSqlStatementBuilderCreateAverageWithWhereExpression()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get(typeof(SqlConnection));
            var queryBuilder = new QueryBuilder();
            var tableName = "Table";
            var field = new Field("Value");
            var where = new QueryGroup(new QueryField("Id", 1));

            // Act
            var actual = statementBuilder.CreateAverage(queryBuilder: queryBuilder,
                tableName: tableName,
                field: field,
                where: where);
            var expected = $"" +
                $"SELECT AVG (CONVERT([FLOAT], [Value])) AS [AverageValue] " +
                $"FROM [Table] " +
                $"WHERE ([Id] = @Id) ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestSqlStatementBuilderCreateAverageWithHints()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get(typeof(SqlConnection));
            var queryBuilder = new QueryBuilder();
            var tableName = "Table";
            var field = new Field("Value");
            var hints = "WITH (NOLOCK)";

            // Act
            var actual = statementBuilder.CreateAverage(queryBuilder: queryBuilder,
                tableName: tableName,
                field: field,
                hints: hints);
            var expected = "SELECT AVG (CONVERT([FLOAT], [Value])) AS [AverageValue] FROM [Table] WITH (NOLOCK) ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestSqlStatementBuilderCreateAverageWithWhereExpressionAndWithHints()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get(typeof(SqlConnection));
            var queryBuilder = new QueryBuilder();
            var tableName = "Table";
            var field = new Field("Value");
            var where = new QueryGroup(new QueryField("Id", 1));
            var hints = "WITH (NOLOCK)";

            // Act
            var actual = statementBuilder.CreateAverage(queryBuilder: queryBuilder,
                tableName: tableName,
                field: field,
                where: where,
                hints: hints);
            var expected = $"" +
                $"SELECT AVG (CONVERT([FLOAT], [Value])) AS [AverageValue] " +
                $"FROM [Table] WITH (NOLOCK) " +
                $"WHERE ([Id] = @Id) ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestSqlStatementBuilderCreateAverageWithQuotedTableSchema()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get(typeof(SqlConnection));
            var queryBuilder = new QueryBuilder();
            var tableName = "[dbo].[Table]";
            var field = new Field("Value");

            // Act
            var actual = statementBuilder.CreateAverage(queryBuilder: queryBuilder,
                tableName: tableName,
                field: field,
                hints: null);
            var expected = "SELECT AVG (CONVERT([FLOAT], [Value])) AS [AverageValue] FROM [dbo].[Table] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestSqlStatementBuilderCreateAverageWithUnquotedTableSchema()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get(typeof(SqlConnection));
            var queryBuilder = new QueryBuilder();
            var tableName = "dbo.Table";
            var field = new Field("Value");

            // Act
            var actual = statementBuilder.CreateAverage(queryBuilder: queryBuilder,
                tableName: tableName,
                field: field,
                hints: null);
            var expected = "SELECT AVG (CONVERT([FLOAT], [Value])) AS [AverageValue] FROM [dbo].[Table] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestSqlStatementBuilderCreateAverageForOtherAverageableFieldType()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get(typeof(SqlConnection));
            var queryBuilder = new QueryBuilder();
            var tableName = "dbo.Table";
            var field = new Field("Value", typeof(long));

            // Act
            var actual = statementBuilder.CreateAverage(queryBuilder: queryBuilder,
                tableName: tableName,
                field: field,
                hints: null);
            var expected = "SELECT AVG (CONVERT([FLOAT], [Value])) AS [AverageValue] FROM [dbo].[Table] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestSqlStatementBuilderCreateAverageForNonAverageableFieldType()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get(typeof(SqlConnection));
            var queryBuilder = new QueryBuilder();
            var tableName = "dbo.Table";
            var field = new Field("Value", typeof(string));

            // Act
            var actual = statementBuilder.CreateAverage(queryBuilder: queryBuilder,
                tableName: tableName,
                field: field,
                hints: null);
            var expected = "SELECT AVG (CONVERT([NVARCHAR], [Value])) AS [AverageValue] FROM [dbo].[Table] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnSqlStatementBuilderCreateAverageIfTheTableIsNull()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get(typeof(SqlConnection));
            var queryBuilder = new QueryBuilder();
            var tableName = (string)null;
            var field = new Field("Value");

            // Act
            statementBuilder.CreateAverage(queryBuilder: queryBuilder,
                tableName: tableName,
                field: field,
                hints: null);
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnSqlStatementBuilderCreateAverageIfTheTableIsEmpty()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get(typeof(SqlConnection));
            var queryBuilder = new QueryBuilder();
            var tableName = "";
            var field = new Field("Value");

            // Act
            statementBuilder.CreateAverage(queryBuilder: queryBuilder,
                tableName: tableName,
                field: field,
                hints: null);
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnSqlStatementBuilderCreateAverageIfTheTableIsWhitespace()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get(typeof(SqlConnection));
            var queryBuilder = new QueryBuilder();
            var tableName = " ";
            var field = new Field("Value");

            // Act
            statementBuilder.CreateAverage(queryBuilder: queryBuilder,
                tableName: tableName,
                field: field,
                hints: null);
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnSqlStatementBuilderCreateAverageIfTheFieldIsNull()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get(typeof(SqlConnection));
            var queryBuilder = new QueryBuilder();
            var tableName = " ";

            // Act
            statementBuilder.CreateAverage(queryBuilder: queryBuilder,
                tableName: tableName,
                field: null,
                hints: null);
        }
    }
}
