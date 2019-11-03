using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Enumerations;
using RepoDb.Exceptions;
using System;
using System.Data.SqlClient;

namespace RepoDb.UnitTests.StatementBuilders
{
    [TestClass]
    public class SqlServerStatementBuilderCreateBatchQueryTest
    {
        [TestMethod]
        public void TestSqlServerStatementBuilderCreateBatchQueryFirstBatch()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get(typeof(SqlConnection));
            var queryBuilder = new QueryBuilder();
            var tableName = "Table";
            var fields = Field.From(new[] { "Field1", "Field2" });
            var orderBy = OrderField.Parse(new { Field1 = Order.Ascending });

            // Act
            var actual = statementBuilder.CreateBatchQuery(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields,
                page: 0,
                rowsPerBatch: 10,
                orderBy: orderBy,
                where: null);
            var expected = $"" +
                $"WITH CTE AS " +
                $"( " +
                $"SELECT ROW_NUMBER() OVER ( ORDER BY [Field1] ASC ) AS [RowNumber], [Field1], [Field2] " +
                $"FROM [Table] " +
                $") " +
                $"SELECT [Field1], [Field2] " +
                $"FROM CTE " +
                $"WHERE ([RowNumber] BETWEEN 1 AND 10) " +
                $"ORDER BY [Field1] ASC ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestSqlServerStatementBuilderCreateBatchQuerySecondBatch()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get(typeof(SqlConnection));
            var queryBuilder = new QueryBuilder();
            var tableName = "Table";
            var fields = Field.From(new[] { "Field1", "Field2" });
            var orderBy = OrderField.Parse(new { Field1 = Order.Ascending });

            // Act
            var actual = statementBuilder.CreateBatchQuery(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields,
                page: 1,
                rowsPerBatch: 10,
                orderBy: orderBy,
                where: null);
            var expected = $"" +
                $"WITH CTE AS " +
                $"( " +
                $"SELECT ROW_NUMBER() OVER ( ORDER BY [Field1] ASC ) AS [RowNumber], [Field1], [Field2] " +
                $"FROM [Table] " +
                $") " +
                $"SELECT [Field1], [Field2] " +
                $"FROM CTE " +
                $"WHERE ([RowNumber] BETWEEN 11 AND 20) " +
                $"ORDER BY [Field1] ASC ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestSqlServerStatementBuilderCreateBatchQueryWithHints()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get(typeof(SqlConnection));
            var queryBuilder = new QueryBuilder();
            var tableName = "[dbo].[Table]";
            var fields = Field.From(new[] { "Field1", "Field2" });
            var orderBy = OrderField.Parse(new { Field1 = Order.Ascending });

            // Act
            var actual = statementBuilder.CreateBatchQuery(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields,
                page: 0,
                rowsPerBatch: 10,
                orderBy: orderBy,
                where: null,
                hints: SqlServerTableHints.NoLock);
            var expected = $"" +
                $"WITH CTE AS " +
                $"( " +
                $"SELECT ROW_NUMBER() OVER ( ORDER BY [Field1] ASC ) AS [RowNumber], [Field1], [Field2] " +
                $"FROM [dbo].[Table] WITH (NOLOCK) " +
                $") " +
                $"SELECT [Field1], [Field2] " +
                $"FROM CTE " +
                $"WHERE ([RowNumber] BETWEEN 1 AND 10) " +
                $"ORDER BY [Field1] ASC ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestSqlServerStatementBuilderCreateBatchQueryWithQuotedTableSchema()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get(typeof(SqlConnection));
            var queryBuilder = new QueryBuilder();
            var tableName = "[dbo].[Table]";
            var fields = Field.From(new[] { "Field1", "Field2" });
            var orderBy = OrderField.Parse(new { Field1 = Order.Ascending });

            // Act
            var actual = statementBuilder.CreateBatchQuery(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields,
                page: 0,
                rowsPerBatch: 10,
                orderBy: orderBy,
                where: null);
            var expected = $"" +
                $"WITH CTE AS " +
                $"( " +
                $"SELECT ROW_NUMBER() OVER ( ORDER BY [Field1] ASC ) AS [RowNumber], [Field1], [Field2] " +
                $"FROM [dbo].[Table] " +
                $") " +
                $"SELECT [Field1], [Field2] " +
                $"FROM CTE " +
                $"WHERE ([RowNumber] BETWEEN 1 AND 10) " +
                $"ORDER BY [Field1] ASC ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestSqlServerStatementBuilderCreateBatchQueryWithUnquotedTableSchema()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get(typeof(SqlConnection));
            var queryBuilder = new QueryBuilder();
            var tableName = "dbo.Table";
            var fields = Field.From(new[] { "Field1", "Field2" });
            var orderBy = OrderField.Parse(new { Field1 = Order.Ascending });

            // Act
            var actual = statementBuilder.CreateBatchQuery(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields,
                page: 0,
                rowsPerBatch: 10,
                orderBy: orderBy,
                where: null);
            var expected = $"" +
                $"WITH CTE AS " +
                $"( " +
                $"SELECT ROW_NUMBER() OVER ( ORDER BY [Field1] ASC ) AS [RowNumber], [Field1], [Field2] " +
                $"FROM [dbo].[Table] " +
                $") " +
                $"SELECT [Field1], [Field2] " +
                $"FROM CTE " +
                $"WHERE ([RowNumber] BETWEEN 1 AND 10) " +
                $"ORDER BY [Field1] ASC ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestSqlServerStatementBuilderCreateBatchQueryWithWhereExpression()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get(typeof(SqlConnection));
            var queryBuilder = new QueryBuilder();
            var tableName = "Table";
            var fields = Field.From(new[] { "Field1", "Field2" });
            var where = new QueryGroup(new QueryField("Field1", Operation.NotEqual, 1));
            var orderBy = OrderField.Parse(new { Field1 = Order.Ascending });

            // Act
            var actual = statementBuilder.CreateBatchQuery(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields,
                page: 1,
                rowsPerBatch: 10,
                orderBy: orderBy,
                where: where);
            var expected = $"" +
                $"WITH CTE AS " +
                $"( " +
                $"SELECT ROW_NUMBER() OVER ( ORDER BY [Field1] ASC ) AS [RowNumber], [Field1], [Field2] " +
                $"FROM [Table] " +
                $"WHERE ([Field1] <> @Field1) " +
                $") " +
                $"SELECT [Field1], [Field2] " +
                $"FROM CTE " +
                $"WHERE ([RowNumber] BETWEEN 11 AND 20) " +
                $"ORDER BY [Field1] ASC ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestSqlServerStatementBuilderCreateBatchQueryWithWhereExpressionUniqueField()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get(typeof(SqlConnection));
            var queryBuilder = new QueryBuilder();
            var tableName = "Table";
            var fields = Field.From(new[] { "Field1", "Field2" });
            var where = new QueryGroup(new QueryField("Id", Operation.NotEqual, 1));
            var orderBy = OrderField.Parse(new { Field1 = Order.Ascending });

            // Act
            var actual = statementBuilder.CreateBatchQuery(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields,
                page: 1,
                rowsPerBatch: 10,
                orderBy: orderBy,
                where: where);
            var expected = $"" +
                $"WITH CTE AS " +
                $"( " +
                $"SELECT ROW_NUMBER() OVER ( ORDER BY [Field1] ASC ) AS [RowNumber], [Field1], [Field2] " +
                $"FROM [Table] " +
                $"WHERE ([Id] <> @Id) " +
                $") " +
                $"SELECT [Field1], [Field2] " +
                $"FROM CTE " +
                $"WHERE ([RowNumber] BETWEEN 11 AND 20) " +
                $"ORDER BY [Field1] ASC ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnSqlServerStatementBuilderCreateBatchQueryIfTheTableIsNull()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get(typeof(SqlConnection));
            var queryBuilder = new QueryBuilder();
            var tableName = (string)null;
            var fields = Field.From(new[] { "Field1", "Field2" });

            // Act/Assert
            statementBuilder.CreateBatchQuery(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields,
                page: 0,
                rowsPerBatch: 10,
                orderBy: null,
                where: null);
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnSqlServerStatementBuilderCreateBatchQueryIfTheTableIsEmpty()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get(typeof(SqlConnection));
            var queryBuilder = new QueryBuilder();
            var tableName = "";
            var fields = Field.From(new[] { "Field1", "Field2" });

            // Act/Assert
            statementBuilder.CreateBatchQuery(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields,
                page: 0,
                rowsPerBatch: 10,
                orderBy: null,
                where: null);
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnSqlServerStatementBuilderCreateBatchQueryIfTheTableIsWhitespace()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get(typeof(SqlConnection));
            var queryBuilder = new QueryBuilder();
            var tableName = " ";
            var fields = Field.From(new[] { "Field1", "Field2" });

            // Act/Assert
            statementBuilder.CreateBatchQuery(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields,
                page: 0,
                rowsPerBatch: 10,
                orderBy: null,
                where: null);
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnSqlServerStatementBuilderCreateBatchQueryIfTheFieldsAreNull()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get(typeof(SqlConnection));
            var queryBuilder = new QueryBuilder();
            var tableName = "Table";
            var orderBy = OrderField.Parse(new { Field1 = Order.Ascending });

            // Act/Assert
            statementBuilder.CreateBatchQuery(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: null,
                page: 0,
                rowsPerBatch: 10,
                orderBy: orderBy,
                where: null);
        }

        [TestMethod, ExpectedException(typeof(EmptyException))]
        public void ThrowExceptionOnSqlServerStatementBuilderCreateBatchQueryIfThereAreNoOrderFields()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get(typeof(SqlConnection));
            var queryBuilder = new QueryBuilder();
            var tableName = "Table";
            var fields = Field.From(new[] { "Field1", "Field2" });

            // Act/Assert
            statementBuilder.CreateBatchQuery(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields,
                page: 0,
                rowsPerBatch: 10,
                orderBy: null,
                where: null);
        }

        [TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ThrowExceptionOnSqlServerStatementBuilderCreateBatchQueryIfThePageIsLessThanZero()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get(typeof(SqlConnection));
            var queryBuilder = new QueryBuilder();
            var tableName = "Table";
            var fields = Field.From(new[] { "Field1", "Field2" });
            var orderBy = OrderField.Parse(new { Field1 = Order.Ascending });

            // Act/Assert
            statementBuilder.CreateBatchQuery(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields,
                page: -1,
                rowsPerBatch: 10,
                orderBy: orderBy,
                where: null);
        }

        [TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ThrowExceptionOnSqlServerStatementBuilderCreateBatchQueryIfTheRowsPerBatchIsLessThanOne()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get(typeof(SqlConnection));
            var queryBuilder = new QueryBuilder();
            var tableName = "Table";
            var fields = Field.From(new[] { "Field1", "Field2" });
            var orderBy = OrderField.Parse(new { Field1 = Order.Ascending });

            // Act/Assert
            statementBuilder.CreateBatchQuery(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields,
                page: 0,
                rowsPerBatch: 0,
                orderBy: orderBy,
                where: null);
        }
    }
}
