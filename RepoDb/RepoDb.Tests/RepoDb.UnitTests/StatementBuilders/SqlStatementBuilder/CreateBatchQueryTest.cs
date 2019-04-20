using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Enumerations;
using System;

namespace RepoDb.UnitTests.StatementBuilders
{
    [TestClass]
    public class SqlStatementBuilderCreateBatchQueryTest
    {
        [TestMethod]
        public void TestSqlStatementBuilderCreateBatchQueryFirstBatch()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder();
            var tableName = "Table";
            var fields = Field.From("Field1", "Field2");
            var orderBy = OrderField.Parse(new
            {
                Field1 = Order.Ascending
            });

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
        public void TestSqlStatementBuilderCreateBatchQuerySecondBatch()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder();
            var tableName = "Table";
            var fields = Field.From("Field1", "Field2");
            var orderBy = OrderField.Parse(new
            {
                Field1 = Order.Ascending
            });

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
        public void TestSqlStatementBuilderCreateBatchQueryWithQuotedTableSchema()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder();
            var tableName = "[dbo].[Table]";
            var fields = Field.From("Field1", "Field2");
            var orderBy = OrderField.Parse(new
            {
                Field1 = Order.Ascending
            });

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
        public void TestSqlStatementBuilderCreateBatchQueryWithUnquotedTableSchema()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder();
            var tableName = "dbo.Table";
            var fields = Field.From("Field1", "Field2");
            var orderBy = OrderField.Parse(new
            {
                Field1 = Order.Ascending
            });

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
        public void TestSqlStatementBuilderCreateBatchQueryWithWhereExpression()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder();
            var tableName = "Table";
            var fields = Field.From("Field1", "Field2");
            var where = new QueryGroup(new QueryField("Field1", Operation.NotEqual, 1));
            var orderBy = OrderField.Parse(new
            {
                Field1 = Order.Ascending
            });

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
        public void TestSqlStatementBuilderCreateBatchQueryWithWhereExpressionUniqueField()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder();
            var tableName = "Table";
            var fields = Field.From("Field1", "Field2");
            var where = new QueryGroup(new QueryField("Id", Operation.NotEqual, 1));
            var orderBy = OrderField.Parse(new
            {
                Field1 = Order.Ascending
            });

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
        public void ThrowExceptionOnSqlStatementBuilderCreateBatchQueryIfTheTableIsNull()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder();
            var tableName = (string)null;
            var fields = Field.From("Field1", "Field2");

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
        public void ThrowExceptionOnSqlStatementBuilderCreateBatchQueryIfTheTableIsEmpty()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder();
            var tableName = "";
            var fields = Field.From("Field1", "Field2");

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
        public void ThrowExceptionOnSqlStatementBuilderCreateBatchQueryIfTheTableIsWhitespace()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder();
            var tableName = " ";
            var fields = Field.From("Field1", "Field2");

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
        public void ThrowExceptionOnSqlStatementBuilderCreateBatchQueryIfTheFieldsAreNull()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder();
            var tableName = "Table";
            var orderBy = OrderField.Parse(new
            {
                Field1 = Order.Ascending
            });

            // Act/Assert
            statementBuilder.CreateBatchQuery(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: null,
                page: 0,
                rowsPerBatch: 10,
                orderBy: orderBy,
                where: null);
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void ThrowExceptionOnSqlStatementBuilderCreateBatchQueryIfThereAreNoOrderFields()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder();
            var tableName = "Table";
            var fields = Field.From("Field1", "Field2");

            // Act/Assert
            statementBuilder.CreateBatchQuery(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields,
                page: 0,
                rowsPerBatch: 10,
                orderBy: null,
                where: null);
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void ThrowExceptionOnSqlStatementBuilderCreateBatchQueryIfThePageIsLessThanZero()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder();
            var tableName = "Table";
            var fields = Field.From("Field1", "Field2");
            var orderBy = OrderField.Parse(new
            {
                Field1 = Order.Ascending
            });

            // Act/Assert
            statementBuilder.CreateBatchQuery(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields,
                page: -1,
                rowsPerBatch: 10,
                orderBy: orderBy,
                where: null);
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void ThrowExceptionOnSqlStatementBuilderCreateBatchQueryIfTheRowsPerBatchIsLessThanZero()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder();
            var tableName = "Table";
            var fields = Field.From("Field1", "Field2");
            var orderBy = OrderField.Parse(new
            {
                Field1 = Order.Ascending
            });

            // Act/Assert
            statementBuilder.CreateBatchQuery(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields,
                page: 0,
                rowsPerBatch: -1,
                orderBy: orderBy,
                where: null);
        }
    }
}
