using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Enumerations;
using RepoDb.StatementBuilders;
using System;

namespace RepoDb.UnitTests.StatementBuilders
{
    [TestClass]
    public class SqlStatementBuilderCreateQueryAllTest
    {
        [TestMethod]
        public void TestSqlStatementBuilderCreateQueryAll()
        {
            // Setup
            var statementBuilder = new SqlServerStatementBuilder();
            var queryBuilder = new QueryBuilder();
            var tableName = "Table";
            var fields = Field.From("Field1", "Field2", "Field3");

            // Act
            var actual = statementBuilder.CreateQueryAll(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields);
            var expected = "SELECT [Field1], [Field2], [Field3] FROM [Table] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestSqlStatementBuilderCreateQueryAllWithQuotedTableSchema()
        {
            // Setup
            var statementBuilder = new SqlServerStatementBuilder();
            var queryBuilder = new QueryBuilder();
            var tableName = "[dbo].[Table]";
            var fields = Field.From("Field1", "Field2", "Field3");

            // Act
            var actual = statementBuilder.CreateQueryAll(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields);
            var expected = "SELECT [Field1], [Field2], [Field3] FROM [dbo].[Table] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestSqlStatementBuilderCreateQueryAllWithUnquotedTableSchema()
        {
            // Setup
            var statementBuilder = new SqlServerStatementBuilder();
            var queryBuilder = new QueryBuilder();
            var tableName = "dbo.Table";
            var fields = Field.From("Field1", "Field2", "Field3");

            // Act
            var actual = statementBuilder.CreateQueryAll(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields);
            var expected = "SELECT [Field1], [Field2], [Field3] FROM [dbo].[Table] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestSqlStatementBuilderCreateQueryAllWithOrderBy()
        {
            // Setup
            var statementBuilder = new SqlServerStatementBuilder();
            var queryBuilder = new QueryBuilder();
            var tableName = "Table";
            var fields = Field.From("Field1", "Field2", "Field3");
            var orderBy = OrderField.Parse(new
            {
                Field1 = Order.Ascending,
                Field2 = Order.Descending
            });

            // Act
            var actual = statementBuilder.CreateQueryAll(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields,
                orderBy: orderBy);
            var expected = $"" +
                $"SELECT [Field1], [Field2], [Field3] " +
                $"FROM [Table] " +
                $"ORDER BY [Field1] ASC, [Field2] DESC ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestSqlStatementBuilderCreateQueryAllWithHints()
        {
            // Setup
            var statementBuilder = new SqlServerStatementBuilder();
            var queryBuilder = new QueryBuilder();
            var tableName = "Table";
            var fields = Field.From("Field1", "Field2", "Field3");
            var hints = SqlServerTableHints.NoLock;

            // Act
            var actual = statementBuilder.CreateQueryAll(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields,
                hints: hints);
            var expected = "SELECT [Field1], [Field2], [Field3] FROM [Table] WITH (NOLOCK) ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestSqlStatementBuilderCreateQueryAllWithOrderByAndWithHints()
        {
            // Setup
            var statementBuilder = new SqlServerStatementBuilder();
            var queryBuilder = new QueryBuilder();
            var tableName = "Table";
            var fields = Field.From("Field1", "Field2", "Field3");
            var orderBy = OrderField.Parse(new
            {
                Field1 = Order.Ascending,
                Field2 = Order.Descending
            });
            var hints = SqlServerTableHints.NoLock;

            // Act
            var actual = statementBuilder.CreateQueryAll(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields,
                orderBy: orderBy,
                hints: hints);
            var expected = $"" +
                $"SELECT [Field1], [Field2], [Field3] " +
                $"FROM [Table] WITH (NOLOCK) " +
                $"ORDER BY [Field1] ASC, [Field2] DESC ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void ThrowExceptionOnSqlStatementBuilderCreateQueryAllIfTheOrderFieldIsNotCovered()
        {
            // Setup
            var statementBuilder = new SqlServerStatementBuilder();
            var queryBuilder = new QueryBuilder();
            var tableName = "Table";
            var fields = Field.From("Field1", "Field2", "Field3");
            var orderBy = OrderField.Parse(new { Id = Order.Ascending, Field1 = Order.Ascending });

            // Act
            statementBuilder.CreateQueryAll(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields,
                orderBy: orderBy);
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnSqlStatementBuilderCreateQueryAllIfThereAreNoFields()
        {
            // Setup
            var statementBuilder = new SqlServerStatementBuilder();
            var queryBuilder = new QueryBuilder();
            var tableName = "Table";

            // Act
            statementBuilder.CreateQueryAll(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: null);
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnSqlStatementBuilderCreateQueryAllIfTheTableIsNull()
        {
            // Setup
            var statementBuilder = new SqlServerStatementBuilder();
            var queryBuilder = new QueryBuilder();
            var tableName = (string)null;
            var fields = Field.From("Field1", "Field2", "Field3");

            // Act
            statementBuilder.CreateQueryAll(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields);
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnSqlStatementBuilderCreateQueryAllIfTheTableIsEmpty()
        {
            // Setup
            var statementBuilder = new SqlServerStatementBuilder();
            var queryBuilder = new QueryBuilder();
            var tableName = "";
            var fields = Field.From("Field1", "Field2", "Field3");

            // Act
            statementBuilder.CreateQueryAll(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields);
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnSqlStatementBuilderCreateQueryAllIfTheTableIsWhitespace()
        {
            // Setup
            var statementBuilder = new SqlServerStatementBuilder();
            var queryBuilder = new QueryBuilder();
            var tableName = " ";
            var fields = Field.From("Field1", "Field2", "Field3");

            // Act
            statementBuilder.CreateQueryAll(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields);
        }
    }
}
