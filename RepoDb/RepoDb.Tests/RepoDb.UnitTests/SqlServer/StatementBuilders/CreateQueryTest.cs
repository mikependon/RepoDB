using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Enumerations;
using RepoDb.Exceptions;
using RepoDb.StatementBuilders;
using RepoDb.UnitTests.Setup;
using System;

namespace RepoDb.UnitTests.StatementBuilders
{
    [TestClass]
    public class SqlStatementBuilderCreateQueryTest
    {
        [TestMethod]
        public void TestSqlStatementBuilderCreateQuery()
        {
            // Setup
            var statementBuilder = new SqlServerStatementBuilder();
            var queryBuilder = new QueryBuilder();
            var tableName = "Table";
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });

            // Act
            var actual = statementBuilder.CreateQuery(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields);
            var expected = "SELECT [Field1], [Field2], [Field3] FROM [Table] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestSqlStatementBuilderCreateQueryWithQuotedTableSchema()
        {
            // Setup
            var statementBuilder = new SqlServerStatementBuilder();
            var queryBuilder = new QueryBuilder();
            var tableName = "[dbo].[Table]";
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });

            // Act
            var actual = statementBuilder.CreateQuery(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields);
            var expected = "SELECT [Field1], [Field2], [Field3] FROM [dbo].[Table] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestSqlStatementBuilderCreateQueryWithUnquotedTableSchema()
        {
            // Setup
            var statementBuilder = new SqlServerStatementBuilder();
            var queryBuilder = new QueryBuilder();
            var tableName = "dbo.Table";
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });

            // Act
            var actual = statementBuilder.CreateQuery(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields);
            var expected = "SELECT [Field1], [Field2], [Field3] FROM [dbo].[Table] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestSqlStatementBuilderCreateQueryWithWhereExpression()
        {
            // Setup
            var statementBuilder = new SqlServerStatementBuilder();
            var queryBuilder = new QueryBuilder();
            var tableName = "Table";
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });
            var where = new QueryGroup(new QueryField("Id", 1));

            // Act
            var actual = statementBuilder.CreateQuery(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields,
                where: where);
            var expected = $"" +
                $"SELECT [Field1], [Field2], [Field3] " +
                $"FROM [Table] " +
                $"WHERE ([Id] = @Id) ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestSqlStatementBuilderCreateQueryWithOrderBy()
        {
            // Setup
            var statementBuilder = new SqlServerStatementBuilder();
            var queryBuilder = new QueryBuilder();
            var tableName = "Table";
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });
            var orderBy = OrderField.Parse(new { Field1 = Order.Ascending, Field2 = Order.Descending });

            // Act
            var actual = statementBuilder.CreateQuery(queryBuilder: queryBuilder,
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
        public void TestSqlStatementBuilderCreateQueryWithTop()
        {
            // Setup
            var statementBuilder = new SqlServerStatementBuilder();
            var queryBuilder = new QueryBuilder();
            var tableName = "Table";
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });
            var top = 100;

            // Act
            var actual = statementBuilder.CreateQuery(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields,
                top: top);
            var expected = "SELECT TOP (100) [Field1], [Field2], [Field3] FROM [Table] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestSqlStatementBuilderCreateQueryWithHints()
        {
            // Setup
            var statementBuilder = new SqlServerStatementBuilder();
            var queryBuilder = new QueryBuilder();
            var tableName = "Table";
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });
            var hints = SqlServerTableHints.NoLock;

            // Act
            var actual = statementBuilder.CreateQuery(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields,
                hints: hints);
            var expected = "SELECT [Field1], [Field2], [Field3] FROM [Table] WITH (NOLOCK) ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestSqlStatementBuilderCreateQueryWithWhereAndWithOrderByAndWithTopAndWithHints()
        {
            // Setup
            var statementBuilder = new SqlServerStatementBuilder();
            var queryBuilder = new QueryBuilder();
            var tableName = "Table";
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });
            var where = new QueryGroup(new QueryField("Id", 1));
            var orderBy = OrderField.Parse(new { Field1 = Order.Ascending, Field2 = Order.Descending });
            var top = 100;
            var hints = SqlServerTableHints.NoLock;

            // Act
            var actual = statementBuilder.CreateQuery(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields,
                where: where,
                orderBy: orderBy,
                top: top,
                hints: hints);
            var expected = $"" +
                $"SELECT TOP (100) [Field1], [Field2], [Field3] " +
                $"FROM [Table] WITH (NOLOCK) " +
                $"WHERE ([Id] = @Id) " +
                $"ORDER BY [Field1] ASC, [Field2] DESC ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod, ExpectedException(typeof(MissingFieldsException))]
        public void ThrowExceptionOnSqlStatementBuilderCreateQueryIfTheOrderFieldIsNotCovered()
        {
            // Setup
            var statementBuilder = new SqlServerStatementBuilder();
            var queryBuilder = new QueryBuilder();
            var tableName = "Table";
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });
            var orderBy = OrderField.Parse(new { Id = Order.Ascending, Field1 = Order.Ascending });

            // Act
            statementBuilder.CreateQuery(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields,
                orderBy: orderBy);
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnSqlStatementBuilderCreateQueryIfThereAreNoFields()
        {
            // Setup
            var statementBuilder = new SqlServerStatementBuilder();
            var queryBuilder = new QueryBuilder();
            var tableName = "Table";

            // Act
            statementBuilder.CreateQuery(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: null);
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnSqlStatementBuilderCreateQueryIfTheTableIsNull()
        {
            // Setup
            var statementBuilder = new SqlServerStatementBuilder();
            var queryBuilder = new QueryBuilder();
            var tableName = (string)null;
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });

            // Act
            statementBuilder.CreateQuery(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields);
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnSqlStatementBuilderCreateQueryIfTheTableIsEmpty()
        {
            // Setup
            var statementBuilder = new SqlServerStatementBuilder();
            var queryBuilder = new QueryBuilder();
            var tableName = "";
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });

            // Act
            statementBuilder.CreateQuery(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields);
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnSqlStatementBuilderCreateQueryIfTheTableIsWhitespace()
        {
            // Setup
            var statementBuilder = new SqlServerStatementBuilder();
            var queryBuilder = new QueryBuilder();
            var tableName = " ";
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });

            // Act
            statementBuilder.CreateQuery(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields);
        }
    }
}
