using NUnit.Framework;
using RepoDb.Attributes;
using RepoDb.Enumerations;

namespace RepoDb.UnitTests
{
    [TestFixture]
    public class SqlDbStatementBuilderTest
    {
        #region CreateBatchQuery

        private class TestCreateBatchQueryWithoutMappingsClass : DataEntity
        {
            public int Field1 { get; set; }
            public int Field2 { get; set; }
        }

        [Test]
        public void TestCreateBatchQueryWithoutMappings()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<TestCreateBatchQueryWithoutMappingsClass>();
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
                $"FROM [TestCreateBatchQueryWithoutAttributesClass] " +
                $") " +
                $"SELECT [Field1], [Field2] " +
                $"FROM CTE " +
                $"WHERE ([RowNumber] BETWEEN 1 AND 10) " +
                $"ORDER BY [Field1] ASC ;";
            Assert.AreEqual(expected, statement);
        }

        private class TestCreateBatchQueryWithExpressionsClass : DataEntity
        {
            public int Field1 { get; set; }
            public int Field2 { get; set; }
        }

        [Test]
        public void TestCreateBatchQueryWithExpressions()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<TestCreateBatchQueryWithExpressionsClass>();
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
                $"FROM [TestCreateBatchQueryWithExpressionsClass] " +
                $"WHERE ([Field1] = @Field1) " +
                $") " +
                $"SELECT [Field1], [Field2] " +
                $"FROM CTE " +
                $"WHERE ([RowNumber] BETWEEN 1 AND 10) " +
                $"ORDER BY [Field1] ASC ;";
            Assert.AreEqual(expected, statement);
        }

        private class TestCreateBatchQueryWithMultipleOrderedColumnsAndWithoutAttributesClass : DataEntity
        {
            public int Field1 { get; set; }
            public int Field2 { get; set; }
        }

        [Test]
        public void TestCreateBatchQueryWithMultipleOrderedColumnsAndWithoutAttributes()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<TestCreateBatchQueryWithMultipleOrderedColumnsAndWithoutAttributesClass>();
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
                $"FROM [TestCreateBatchQueryWithMultipleOrderedColumnsAndWithoutAttributesClass] " +
                $") " +
                $"SELECT [Field1], [Field2] " +
                $"FROM CTE " +
                $"WHERE ([RowNumber] BETWEEN 1 AND 10) " +
                $"ORDER BY [Field1] DESC, [Field2] ASC ;";
            Assert.AreEqual(expected, statement);
        }

        [Map("ClassName")]
        private class TestCreateBatchQueryWithClassMappingClass : DataEntity
        {
            public int Field1 { get; set; }
            public int Field2 { get; set; }
        }

        [Test]
        public void TestCreateBatchQueryWithClassMapping()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<TestCreateBatchQueryWithClassMappingClass>();
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
                $"FROM [ClassName] " +
                $") " +
                $"SELECT [Field1], [Field2] " +
                $"FROM CTE " +
                $"WHERE ([RowNumber] BETWEEN 1 AND 10) " +
                $"ORDER BY [Field1] ASC ;";
            Assert.AreEqual(expected, statement);
        }

        [Map("ClassName")]
        private class TestCreateBatchQueryWithFieldMappingsClass : DataEntity
        {
            public int Field1 { get; set; }
            [Map("NewField2")]
            public int Field2 { get; set; }
        }

        [Test]
        public void TestCreateBatchQueryWithFieldMappings()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<TestCreateBatchQueryWithFieldMappingsClass>();
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
                $"SELECT ROW_NUMBER() OVER ( ORDER BY [Field1] ASC ) AS [RowNumber], [Field1], [NewField2] " +
                $"FROM [ClassName] " +
                $") " +
                $"SELECT [Field1], [NewField2] " +
                $"FROM CTE " +
                $"WHERE ([RowNumber] BETWEEN 1 AND 10) " +
                $"ORDER BY [Field1] ASC ;";
            Assert.AreEqual(expected, statement);
        }

        [Map("ClassName")]
        private class TestCreateBatchQueryWithFieldMappingsAndWithIgnoredBatchQueryCommandClass : DataEntity
        {
            public int Field1 { get; set; }
            [RepoDb.Attributes.Ignore(Command.BatchQuery)]
            public int Field2 { get; set; }
            public int Field3 { get; set; }
        }

        [Test]
        public void TestCreateBatchQueryWithFieldMappingsAndWithIgnoredBatchQueryCommand()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<TestCreateBatchQueryWithFieldMappingsAndWithIgnoredBatchQueryCommandClass>();
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
                $"FROM [ClassName] " +
                $") " +
                $"SELECT [Field1], [Field3] " +
                $"FROM CTE " +
                $"WHERE ([RowNumber] BETWEEN 1 AND 10) " +
                $"ORDER BY [Field1] ASC ;";
            Assert.AreEqual(expected, statement);
        }

        [Map("ClassName")]
        private class TestCreateBatchQueryWithFieldMappingsAndWithIgnoredQueryCommandClass : DataEntity
        {
            public int Field1 { get; set; }
            [RepoDb.Attributes.Ignore(Command.Query)]
            public int Field2 { get; set; }
            public int Field3 { get; set; }
        }

        [Test]
        public void TestCreateBatchQueryWithFieldMappingsAndWithIgnoredQueryCommand()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<TestCreateBatchQueryWithFieldMappingsAndWithIgnoredQueryCommandClass>();
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
                $"FROM [ClassName] " +
                $") " +
                $"SELECT [Field1], [Field3] " +
                $"FROM CTE " +
                $"WHERE ([RowNumber] BETWEEN 1 AND 10) " +
                $"ORDER BY [Field1] ASC ;";
            Assert.AreEqual(expected, statement);
        }

        [Map("ClassName")]
        private class TestCreateBatchQueryWithFieldMappingsAndWithIgnoredBathQueryAndQueryCommandClass : DataEntity
        {
            public int Field1 { get; set; }
            [RepoDb.Attributes.Ignore(Command.Query)]
            public int Field2 { get; set; }
            public int Field3 { get; set; }
            [RepoDb.Attributes.Ignore(Command.BatchQuery)]
            public int Field4 { get; set; }
        }

        [Test]
        public void TestCreateBatchQueryWithFieldMappingsAndWithIgnoredBathQueryAndQueryCommand()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<TestCreateBatchQueryWithFieldMappingsAndWithIgnoredBathQueryAndQueryCommandClass>();
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
                $"FROM [ClassName] " +
                $") " +
                $"SELECT [Field1], [Field3] " +
                $"FROM CTE " +
                $"WHERE ([RowNumber] BETWEEN 1 AND 10) " +
                $"ORDER BY [Field1] ASC ;";
            Assert.AreEqual(expected, statement);
        }

        #endregion

        #region CreateCount

        private class TestCreateCountWithoutMappingsClass : DataEntity
        {
        }

        [Test]
        public void TestCreateCountWithoutMappings()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<TestCreateCountWithoutMappingsClass>();

            // Act
            var statement = statementBuilder.CreateCount(queryBuilder, null);

            // Assert
            var expected = $"SELECT COUNT_BIG (1) AS [Counted] FROM [TestCreateCountWithoutMappingsClass] ;";
            Assert.AreEqual(expected, statement);
        }

        [Map("ClassName")]
        private class TestCreateCountWitClassMappingsClass : DataEntity
        {
        }

        [Test]
        public void TestCreateCountWitClassMappings()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<TestCreateCountWitClassMappingsClass>();

            // Act
            var statement = statementBuilder.CreateCount(queryBuilder, null);

            // Assert
            var expected = $"SELECT COUNT_BIG (1) AS [Counted] FROM [ClassName] ;";
            Assert.AreEqual(expected, statement);
        }

        #endregion
    }
}
