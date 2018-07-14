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
            var actual = statementBuilder.CreateBatchQuery(queryBuilder, null, 0, 10, orderBy);

            // Assert
            var expected = $"" +
                $"WITH CTE AS " +
                $"( " +
                $"SELECT ROW_NUMBER() OVER ( ORDER BY [Field1] ASC ) AS [RowNumber], [Field1], [Field2] " +
                $"FROM [TestCreateBatchQueryWithoutMappingsClass] " +
                $") " +
                $"SELECT [Field1], [Field2] " +
                $"FROM CTE " +
                $"WHERE ([RowNumber] BETWEEN 1 AND 10) " +
                $"ORDER BY [Field1] ASC ;";
            Assert.AreEqual(expected, actual);
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
            var actual = statementBuilder.CreateBatchQuery(queryBuilder, where, 0, 10, orderBy);

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
            Assert.AreEqual(expected, actual);
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
            var actual = statementBuilder.CreateBatchQuery(queryBuilder, null, 0, 10, orderBy);

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
            Assert.AreEqual(expected, actual);
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
            var actual = statementBuilder.CreateBatchQuery(queryBuilder, null, 0, 10, orderBy);

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
            Assert.AreEqual(expected, actual);
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
            var actual = statementBuilder.CreateBatchQuery(queryBuilder, null, 0, 10, orderBy);

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
            Assert.AreEqual(expected, actual);
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
            var actual = statementBuilder.CreateBatchQuery(queryBuilder, null, 0, 10, orderBy);

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
            Assert.AreEqual(expected, actual);
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
            var actual = statementBuilder.CreateBatchQuery(queryBuilder, null, 0, 10, orderBy);

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
            Assert.AreEqual(expected, actual);
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
            var actual = statementBuilder.CreateBatchQuery(queryBuilder, null, 0, 10, orderBy);

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
            Assert.AreEqual(expected, actual);
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
            var actual = statementBuilder.CreateCount(queryBuilder, null);

            // Assert
            var expected = $"" +
                $"SELECT COUNT_BIG (1) AS [Counted] " +
                $"FROM [TestCreateCountWithoutMappingsClass] ;";
            Assert.AreEqual(expected, actual);
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
            var actual = statementBuilder.CreateCount(queryBuilder, null);

            // Assert
            var expected = $"" +
                $"SELECT COUNT_BIG (1) AS [Counted] " +
                $"FROM [ClassName] ;";
            Assert.AreEqual(expected, actual);
        }

        private class TestCreateCountWithExpressionsClass : DataEntity
        {
        }

        [Test]
        public void TestCountWithExpressions()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<TestCreateCountWithExpressionsClass>();
            var where = QueryGroup.Parse(new
            {
                Field1 = "Test"
            });

            // Act
            var actual = statementBuilder.CreateCount(queryBuilder, where);

            // Assert
            var expected = $"" +
                $"SELECT COUNT_BIG (1) AS [Counted] " +
                $"FROM [TestCreateCountWithExpressionsClass] " +
                $"WHERE ([Field1] = @Field1) ;";
            Assert.AreEqual(expected, actual);
        }

        #endregion

        #region CreateDelete

        private class TestCreateDeleteWithoutMappingsClass : DataEntity
        {
        }

        [Test]
        public void TestCreateDeleteWithoutMappings()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<TestCreateDeleteWithoutMappingsClass>();

            // Act
            var actual = statementBuilder.CreateDelete(queryBuilder, null);

            // Assert
            var expected = $"" +
                $"DELETE " +
                $"FROM [TestCreateDeleteWithoutMappingsClass] ;";
            Assert.AreEqual(expected, actual);
        }

        private class TestCreateDeleteWithoutMappingsAndWithExpressionsClass : DataEntity
        {
        }

        [Test]
        public void TestCreateDeleteWithExpressions()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<TestCreateDeleteWithoutMappingsAndWithExpressionsClass>();
            var where = new { Field1 = "Test" };

            // Act
            var queryGroup = QueryGroup.Parse(where);
            var actual = statementBuilder.CreateDelete(queryBuilder, queryGroup);

            // Assert
            var expected = $"" +
                $"DELETE " +
                $"FROM [TestCreateDeleteWithoutMappingsAndWithExpressionsClass] " +
                $"WHERE ([Field1] = @Field1) ;";
            Assert.AreEqual(expected, actual);
        }

        [Map("ClassName")]
        private class TestCreateDeleteWithMappingsClass : DataEntity
        {
        }

        [Test]
        public void TestCreateDeleteWithMappings()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<TestCreateDeleteWithMappingsClass>();
            var where = new { Field1 = "Test" };

            // Act
            var queryGroup = QueryGroup.Parse(where);
            var actual = statementBuilder.CreateDelete(queryBuilder, queryGroup);

            // Assert
            var expected = $"" +
                $"DELETE " +
                $"FROM [ClassName] " +
                $"WHERE ([Field1] = @Field1) ;";
            Assert.AreEqual(expected, actual);
        }

        #endregion

        #region CreateDeleteAll

        private class TestCreateDeleteAllWithoutMappingsClass : DataEntity
        {
        }

        [Test]
        public void TestCreateDeleteAllWithoutMappings()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<TestCreateDeleteAllWithoutMappingsClass>();

            // Act
            var actual = statementBuilder.CreateDeleteAll(queryBuilder);

            // Assert
            var expected = $"" +
                $"DELETE " +
                $"FROM [TestCreateDeleteAllWithoutMappingsClass] ;";
            Assert.AreEqual(expected, actual);
        }

        [Map("ClassName")]
        private class TestCreateDeleteAllWithMappingsClass : DataEntity
        {
        }

        [Test]
        public void TestCreateDeleteAllWithMappings()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<TestCreateDeleteAllWithMappingsClass>();

            // Act
            var actual = statementBuilder.CreateDeleteAll(queryBuilder);

            // Assert
            var expected = $"" +
                $"DELETE " +
                $"FROM [ClassName] ;";
            Assert.AreEqual(expected, actual);
        }

        #endregion
    }
}
