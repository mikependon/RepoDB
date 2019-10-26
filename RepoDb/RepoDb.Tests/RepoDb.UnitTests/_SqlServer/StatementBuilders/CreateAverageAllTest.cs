using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Data.SqlClient;

namespace RepoDb.UnitTests.StatementBuilders
{
    [TestClass]
    public class SqlStatementBuilderCreateAverageAllTest
    {
        [TestMethod]
        public void TestSqlStatementBuilderCreateAverageAll()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get(typeof(SqlConnection));
            var queryBuilder = new QueryBuilder();
            var tableName = "Table";
            var field = new Field("Value");

            // Act
            var actual = statementBuilder.CreateAverageAll(queryBuilder: queryBuilder,
                field: field,
                tableName: tableName,
                hints: null);
            var expected = "SELECT AVG (CONVERT([FLOAT], [Value])) AS [AverageValue] FROM [Table] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestSqlStatementBuilderCreateAverageAllWithHints()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get(typeof(SqlConnection));
            var queryBuilder = new QueryBuilder();
            var tableName = "Table";
            var field = new Field("Value");
            var hints = "WITH (NOLOCK)";

            // Act
            var actual = statementBuilder.CreateAverageAll(queryBuilder: queryBuilder,
                tableName: tableName,
                field: field,
                hints: hints);
            var expected = "SELECT AVG (CONVERT([FLOAT], [Value])) AS [AverageValue] FROM [Table] WITH (NOLOCK) ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestSqlStatementBuilderCreateAverageAllWithQuotedTableSchema()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get(typeof(SqlConnection));
            var queryBuilder = new QueryBuilder();
            var tableName = "[dbo].[Table]";
            var field = new Field("Value");

            // Act
            var actual = statementBuilder.CreateAverageAll(queryBuilder: queryBuilder,
                tableName: tableName,
                field: field,
                hints: null);
            var expected = "SELECT AVG (CONVERT([FLOAT], [Value])) AS [AverageValue] FROM [dbo].[Table] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestSqlStatementBuilderCreateAverageAllWithUnquotedTableSchema()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get(typeof(SqlConnection));
            var queryBuilder = new QueryBuilder();
            var tableName = "dbo.Table";
            var field = new Field("Value");

            // Act
            var actual = statementBuilder.CreateAverageAll(queryBuilder: queryBuilder,
                tableName: tableName,
                field: field,
                hints: null);
            var expected = "SELECT AVG (CONVERT([FLOAT], [Value])) AS [AverageValue] FROM [dbo].[Table] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestSqlStatementBuilderCreateAverageAllForOtherAverageableFieldType()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get(typeof(SqlConnection));
            var queryBuilder = new QueryBuilder();
            var tableName = "Table";
            var field = new Field("Value", typeof(long));

            // Act
            var actual = statementBuilder.CreateAverageAll(queryBuilder: queryBuilder,
                field: field,
                tableName: tableName,
                hints: null);
            var expected = "SELECT AVG (CONVERT([FLOAT], [Value])) AS [AverageValue] FROM [Table] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestSqlStatementBuilderCreateAverageAllForNonAverageableFieldType()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get(typeof(SqlConnection));
            var queryBuilder = new QueryBuilder();
            var tableName = "Table";
            var field = new Field("Value", typeof(string));

            // Act
            var actual = statementBuilder.CreateAverageAll(queryBuilder: queryBuilder,
                field: field,
                tableName: tableName,
                hints: null);
            var expected = "SELECT AVG (CONVERT([NVARCHAR], [Value])) AS [AverageValue] FROM [Table] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnSqlStatementBuilderCreateAverageAllIfTheTableIsNull()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get(typeof(SqlConnection));
            var queryBuilder = new QueryBuilder();
            var tableName = (string)null;
            var field = new Field("Value");

            // Act
            statementBuilder.CreateAverageAll(queryBuilder: queryBuilder,
                tableName: tableName,
                field: field,
                hints: null);
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnSqlStatementBuilderCreateAverageAllIfTheTableIsEmpty()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get(typeof(SqlConnection));
            var queryBuilder = new QueryBuilder();
            var tableName = "";
            var field = new Field("Value");

            // Act
            statementBuilder.CreateAverageAll(queryBuilder: queryBuilder,
                tableName: tableName,
                field: field,
                hints: null);
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnSqlStatementBuilderCreateAverageAllIfTheTableIsWhitespace()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get(typeof(SqlConnection));
            var queryBuilder = new QueryBuilder();
            var tableName = " ";
            var field = new Field("Value");

            // Act
            statementBuilder.CreateAverageAll(queryBuilder: queryBuilder,
                tableName: tableName,
                field: field,
                hints: null);
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnSqlStatementBuilderCreateAverageAllIfTheFieldIsNull()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get(typeof(SqlConnection));
            var queryBuilder = new QueryBuilder();
            var tableName = " ";

            // Act
            statementBuilder.CreateAverageAll(queryBuilder: queryBuilder,
                tableName: tableName,
                field: null,
                hints: null);
        }
    }
}
