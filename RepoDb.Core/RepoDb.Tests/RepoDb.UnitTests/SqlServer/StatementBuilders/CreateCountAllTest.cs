using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.StatementBuilders;
using RepoDb.UnitTests.Setup;
using System;

namespace RepoDb.UnitTests.StatementBuilders
{
    [TestClass]
    public class SqlStatementBuilderCreateCountAllTest
    {
        [TestMethod]
        public void TestSqlStatementBuilderCreateCountAll()
        {
            // Setup
            var statementBuilder = Helper.StatementBuilder;
            var queryBuilder = new QueryBuilder();
            var tableName = "Table";

            // Act
            var actual = statementBuilder.CreateCountAll(queryBuilder: queryBuilder,
                tableName: tableName,
                hints: null);
            var expected = "SELECT COUNT_BIG (1) AS [Counted] FROM [Table] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestSqlStatementBuilderCreateCountAllWithHints()
        {
            // Setup
            var statementBuilder = Helper.StatementBuilder;
            var queryBuilder = new QueryBuilder();
            var tableName = "Table";
            var hints = "WITH (NOLOCK)";

            // Act
            var actual = statementBuilder.CreateCountAll(queryBuilder: queryBuilder,
                tableName: tableName,
                hints: hints);
            var expected = "SELECT COUNT_BIG (1) AS [Counted] FROM [Table] WITH (NOLOCK) ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestSqlStatementBuilderCreateCountAllWithQuotedTableSchema()
        {
            // Setup
            var statementBuilder = Helper.StatementBuilder;
            var queryBuilder = new QueryBuilder();
            var tableName = "[dbo].[Table]";

            // Act
            var actual = statementBuilder.CreateCountAll(queryBuilder: queryBuilder,
                tableName: tableName,
                hints: null);
            var expected = "SELECT COUNT_BIG (1) AS [Counted] FROM [dbo].[Table] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestSqlStatementBuilderCreateCountAllWithUnquotedTableSchema()
        {
            // Setup
            var statementBuilder = Helper.StatementBuilder;
            var queryBuilder = new QueryBuilder();
            var tableName = "dbo.Table";

            // Act
            var actual = statementBuilder.CreateCountAll(queryBuilder: queryBuilder,
                tableName: tableName,
                hints: null);
            var expected = "SELECT COUNT_BIG (1) AS [Counted] FROM [dbo].[Table] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnSqlStatementBuilderCreateCountAllIfTheTableIsNull()
        {
            // Setup
            var statementBuilder = Helper.StatementBuilder;
            var queryBuilder = new QueryBuilder();
            var tableName = (string)null;

            // Act
            statementBuilder.CreateCountAll(queryBuilder: queryBuilder,
                tableName: tableName,
                hints: null);
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnSqlStatementBuilderCreateCountAllIfTheTableIsEmpty()
        {
            // Setup
            var statementBuilder = Helper.StatementBuilder;
            var queryBuilder = new QueryBuilder();
            var tableName = "";

            // Act
            statementBuilder.CreateCountAll(queryBuilder: queryBuilder,
                tableName: tableName,
                hints: null);
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnSqlStatementBuilderCreateCountAllIfTheTableIsWhitespace()
        {
            // Setup
            var statementBuilder = Helper.StatementBuilder;
            var queryBuilder = new QueryBuilder();
            var tableName = " ";

            // Act
            statementBuilder.CreateCountAll(queryBuilder: queryBuilder,
                tableName: tableName,
                hints: null);
        }
    }
}
