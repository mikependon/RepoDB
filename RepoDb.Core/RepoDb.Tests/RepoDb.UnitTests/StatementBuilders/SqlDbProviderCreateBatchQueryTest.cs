using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Attributes;
using RepoDb.Enumerations;
using System;

namespace RepoDb.UnitTests.StatementBuilders
{
    [TestClass]
    public class SqlDbProviderCreateBatchQueryTest
    {

        private class TestSqlDbProviderCreateBatchQueryWithoutMappingsClass
        {
            public int Field1 { get; set; }
            public int Field2 { get; set; }
        }

        [TestMethod]
        public void TestSqlDbProviderCreateBatchQueryWithoutMappings()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder<TestSqlDbProviderCreateBatchQueryWithoutMappingsClass>();
            var orderBy = OrderField.Parse(new
            {
                Field1 = Order.Ascending
            });

            // Act
            var actual = statementBuilder.CreateBatchQuery(queryBuilder, null, 0, 10, orderBy);
            var expected = $"" +
                $"WITH CTE AS " +
                $"( " +
                $"SELECT ROW_NUMBER() OVER ( ORDER BY [Field1] ASC ) AS [RowNumber], [Field1], [Field2] " +
                $"FROM [TestSqlDbProviderCreateBatchQueryWithoutMappingsClass] " +
                $") " +
                $"SELECT [Field1], [Field2] " +
                $"FROM CTE " +
                $"WHERE ([RowNumber] BETWEEN 1 AND 10) " +
                $"ORDER BY [Field1] ASC ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        private class TestSqlDbProviderCreateBatchQueryWithExpressionsClass
        {
            public int Field1 { get; set; }
            public int Field2 { get; set; }
        }

        [TestMethod]
        public void TestSqlDbProviderCreateBatchQueryWithExpressions()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder<TestSqlDbProviderCreateBatchQueryWithExpressionsClass>();
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
            var expected = $"" +
                $"WITH CTE AS " +
                $"( " +
                $"SELECT ROW_NUMBER() OVER ( ORDER BY [Field1] ASC ) AS [RowNumber], [Field1], [Field2] " +
                $"FROM [TestSqlDbProviderCreateBatchQueryWithExpressionsClass] " +
                $"WHERE ([Field1] = @Field1) " +
                $") " +
                $"SELECT [Field1], [Field2] " +
                $"FROM CTE " +
                $"WHERE ([RowNumber] BETWEEN 1 AND 10) " +
                $"ORDER BY [Field1] ASC ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        private class TestSqlDbProviderCreateBatchQueryWithMultipleOrderedColumnsAndWithoutAttributesClass
        {
            public int Field1 { get; set; }
            public int Field2 { get; set; }
        }

        [TestMethod]
        public void TestSqlDbProviderCreateBatchQueryWithMultipleOrderedColumnsAndWithoutAttributes()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder<TestSqlDbProviderCreateBatchQueryWithMultipleOrderedColumnsAndWithoutAttributesClass>();
            var orderBy = OrderField.Parse(new
            {
                Field1 = Order.Descending,
                Field2 = Order.Ascending
            });

            // Act
            var actual = statementBuilder.CreateBatchQuery(queryBuilder, null, 0, 10, orderBy);
            var expected = $"" +
                $"WITH CTE AS " +
                $"( " +
                $"SELECT ROW_NUMBER() OVER ( ORDER BY [Field1] DESC, [Field2] ASC ) AS [RowNumber], [Field1], [Field2] " +
                $"FROM [TestSqlDbProviderCreateBatchQueryWithMultipleOrderedColumnsAndWithoutAttributesClass] " +
                $") " +
                $"SELECT [Field1], [Field2] " +
                $"FROM CTE " +
                $"WHERE ([RowNumber] BETWEEN 1 AND 10) " +
                $"ORDER BY [Field1] DESC, [Field2] ASC ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [Map("ClassName")]
        private class TestSqlDbProviderCreateBatchQueryWithClassMappingClass
        {
            public int Field1 { get; set; }
            public int Field2 { get; set; }
        }

        [TestMethod]
        public void TestSqlDbProviderCreateBatchQueryWithClassMapping()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder<TestSqlDbProviderCreateBatchQueryWithClassMappingClass>();
            var orderBy = OrderField.Parse(new
            {
                Field1 = Order.Ascending
            });

            // Act
            var actual = statementBuilder.CreateBatchQuery(queryBuilder, null, 0, 10, orderBy);
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

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [Map("ClassName")]
        private class TestSqlDbProviderCreateBatchQueryWithFieldMappingsClass
        {
            public int Field1 { get; set; }
            [Map("Field3")]
            public int Field2 { get; set; }
        }

        [TestMethod]
        public void TestSqlDbProviderCreateBatchQueryWithFieldMappings()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder<TestSqlDbProviderCreateBatchQueryWithFieldMappingsClass>();
            var orderBy = OrderField.Parse(new
            {
                Field1 = Order.Ascending
            });

            // Act
            var actual = statementBuilder.CreateBatchQuery(queryBuilder, null, 0, 10, orderBy);
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

            // Assert
            Assert.AreEqual(expected, actual);
        }

        private class ThrowExceptionSqlDbProviderCreateBatchQueryIfThereAreNoQueryableFieldsClass
        {
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void ThrowExceptionOnSqlDbProviderCreateBatchQueryIfThereAreNoQueryableFields()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder<ThrowExceptionSqlDbProviderCreateBatchQueryIfThereAreNoQueryableFieldsClass>();
            var queryGroup = (QueryGroup)null;

            // Act/Assert
            statementBuilder.CreateBatchQuery(queryBuilder, queryGroup, 0, 10, null);
        }
    }
}
