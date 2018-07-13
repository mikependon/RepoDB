using NUnit.Framework;
using RepoDb.Attributes;
using RepoDb.Enumerations;

namespace RepoDb.UnitTests
{
    [TestFixture]
    public class SqlDbStatementBuilderTest
    {

        #region Sub Classes

        #region CreateBatchQuery

        private class CreateBatchQueryClass1 : DataEntity
        {
            public int Field1 { get; set; }
            public int Field2 { get; set; }
        }

        [Map("Customer")]
        private class CreateBatchQueryClass2 : DataEntity
        {
            public int Field1 { get; set; }
            public int Field2 { get; set; }
        }

        [Map("Customer")]
        private class CreateBatchQueryClass3 : DataEntity
        {
            public int Field1 { get; set; }

            [Map("Field3")]
            public int Field2 { get; set; }
        }

        #endregion

        #endregion

        #region CreateBatchQuery

        [Test]
        public void TestCreateBatchQueryWithoutAttributes()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<CreateBatchQueryClass1>();
            var orderBy = OrderField.Parse(new
            {
                Field1 = Order.Ascending
            });

            // Act
            var statement = statementBuilder.CreateBatchQuery(queryBuilder, null, 0, 10, orderBy);

            // Assert
            var expected = $"" +
                $"WITH CTE AS " +
                $"( " +
                $"SELECT ROW_NUMBER() OVER ( ORDER BY [Field1] ASC ) AS [RowNumber], [Field1], [Field2] " +
                $"FROM [CreateBatchQueryClass1] " +
                $") " +
                $"SELECT [Field1], [Field2] " +
                $"FROM CTE " +
                $"WHERE ([RowNumber] BETWEEN 1 AND 10) " +
                $"ORDER BY [Field1] ASC ;";
            Assert.IsNotEmpty(statement);
            Assert.AreEqual(expected, statement);
        }

        [Test]
        public void TestCreateBatchQueryWithExpression()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<CreateBatchQueryClass1>();
            var where = QueryGroup.Parse(new
            {
                Field1 = "Test"
            });
            var orderBy = OrderField.Parse(new
            {
                Field1 = Order.Ascending
            });

            // Act
            var statement = statementBuilder.CreateBatchQuery(queryBuilder, where, 0, 10, orderBy);

            // Assert
            var expected = $"" +
                $"WITH CTE AS " +
                $"( " +
                $"SELECT ROW_NUMBER() OVER ( ORDER BY [Field1] ASC ) AS [RowNumber], [Field1], [Field2] " +
                $"FROM [CreateBatchQueryClass1] " +
                $"WHERE ([Field1] = @Field1) " +
                $") " +
                $"SELECT [Field1], [Field2] " +
                $"FROM CTE " +
                $"WHERE ([RowNumber] BETWEEN 1 AND 10) " +
                $"ORDER BY [Field1] ASC ;";
            Assert.IsNotEmpty(statement);
            Assert.AreEqual(expected, statement);
        }

        [Test]
        public void TestCreateBatchQueryWithMultipleOrderedColumnsAndWithoutAttributes()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<CreateBatchQueryClass1>();
            var orderBy = OrderField.Parse(new
            {
                Field1 = Order.Descending,
                Field2 = Order.Ascending
            });

            // Act
            var statement = statementBuilder.CreateBatchQuery(queryBuilder, null, 0, 10, orderBy);

            // Assert
            var expected = $"" +
                $"WITH CTE AS " +
                $"( " +
                $"SELECT ROW_NUMBER() OVER ( ORDER BY [Field1] DESC, [Field2] ASC ) AS [RowNumber], [Field1], [Field2] " +
                $"FROM [CreateBatchQueryClass1] " +
                $") " +
                $"SELECT [Field1], [Field2] " +
                $"FROM CTE " +
                $"WHERE ([RowNumber] BETWEEN 1 AND 10) " +
                $"ORDER BY [Field1] DESC, [Field2] ASC ;";
            Assert.IsNotEmpty(statement);
            Assert.AreEqual(expected, statement);
        }

        [Test]
        public void TestCreateBatchQueryWithClassMapping()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<CreateBatchQueryClass2>();
            var orderBy = OrderField.Parse(new
            {
                Field1 = Order.Ascending
            });

            // Act
            var statement = statementBuilder.CreateBatchQuery(queryBuilder, null, 0, 10, orderBy);

            // Assert
            var expected = $"" +
                $"WITH CTE AS " +
                $"( " +
                $"SELECT ROW_NUMBER() OVER ( ORDER BY [Field1] ASC ) AS [RowNumber], [Field1], [Field2] " +
                $"FROM [Customer] " +
                $") " +
                $"SELECT [Field1], [Field2] " +
                $"FROM CTE " +
                $"WHERE ([RowNumber] BETWEEN 1 AND 10) " +
                $"ORDER BY [Field1] ASC ;";
            Assert.IsNotEmpty(statement);
            Assert.AreEqual(expected, statement);
        }

        [Test]
        public void TestCreateBatchQueryWithFieldMapping()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<CreateBatchQueryClass3>();
            var orderBy = OrderField.Parse(new
            {
                Field1 = Order.Ascending
            });

            // Act
            var statement = statementBuilder.CreateBatchQuery(queryBuilder, null, 0, 10, orderBy);

            // Assert
            var expected = $"" +
                $"WITH CTE AS " +
                $"( " +
                $"SELECT ROW_NUMBER() OVER ( ORDER BY [Field1] ASC ) AS [RowNumber], [Field1], [Field3] " +
                $"FROM [Customer] " +
                $") " +
                $"SELECT [Field1], [Field3] " +
                $"FROM CTE " +
                $"WHERE ([RowNumber] BETWEEN 1 AND 10) " +
                $"ORDER BY [Field1] ASC ;";
            Assert.IsNotEmpty(statement);
            Assert.AreEqual(expected, statement);
        }

        #endregion

    }
}
