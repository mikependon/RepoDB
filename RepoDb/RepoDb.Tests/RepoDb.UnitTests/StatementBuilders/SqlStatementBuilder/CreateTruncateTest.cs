using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace RepoDb.UnitTests.StatementBuilders
{
    [TestClass]
    public class SqlStatementBuilderCreateTruncateTest
    {
        [TestMethod]
        public void TestSqlStatementBuilderCreateTruncate()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder();
            var tableName = "Table";

            // Act
            var actual = statementBuilder.CreateTruncate(queryBuilder: queryBuilder,
                tableName: tableName);
            var expected = "TRUNCATE TABLE [Table] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestSqlStatementBuilderCreateTruncateWithQuotedTableSchema()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder();
            var tableName = "[dbo].[Table]";

            // Act
            var actual = statementBuilder.CreateTruncate(queryBuilder: queryBuilder,
                tableName: tableName);
            var expected = "TRUNCATE TABLE [dbo].[Table] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestSqlStatementBuilderCreateTruncateWithUnquotedTableSchema()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder();
            var tableName = "dbo.Table";

            // Act
            var actual = statementBuilder.CreateTruncate(queryBuilder: queryBuilder,
                tableName: tableName);
            var expected = "TRUNCATE TABLE [dbo].[Table] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnSqlStatementBuilderCreateTruncateIfTheTableIsNull()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder();
            var tableName = (string)null;

            // Act
            statementBuilder.CreateTruncate(queryBuilder: queryBuilder,
                tableName: tableName);
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnSqlStatementBuilderCreateTruncateIfTheTableIsEmpty()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder();
            var tableName = "";

            // Act
            statementBuilder.CreateTruncate(queryBuilder: queryBuilder,
                tableName: tableName);
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnSqlStatementBuilderCreateTruncateIfTheTableIsWhitespace()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder();
            var tableName = " ";

            // Act
            statementBuilder.CreateTruncate(queryBuilder: queryBuilder,
                tableName: tableName);
        }
    }
}
