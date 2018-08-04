using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Attributes;
using RepoDb.Enumerations;
using System;

namespace RepoDb.UnitTests.SqlDbStatementBuilderTest
{
    [TestClass]
    public class CreateQueryTest
    {
        private class TestWithoutMappingsClass : DataEntity
        {
            public int Field1 { get; set; }
            public string Field2 { get; set; }
            public DateTime Field3 { get; set; }
        }

        [TestMethod]
        public void TestWithoutMappings()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<TestWithoutMappingsClass>();
            var queryGroup = (QueryGroup)null;

            // Act
            var actual = statementBuilder.CreateQuery(queryBuilder, queryGroup);
            var expected = $"" +
                $"SELECT [Field1], [Field2], [Field3] " +
                $"FROM [TestWithoutMappingsClass] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [Map("ClassName")]
        private class TestWithClassMappingClass : DataEntity
        {
            public int Field1 { get; set; }
            public string Field2 { get; set; }
            public DateTime Field3 { get; set; }
        }

        [TestMethod]
        public void TestWithClassMapping()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<TestWithClassMappingClass>();
            var queryGroup = (QueryGroup)null;

            // Act
            var actual = statementBuilder.CreateQuery(queryBuilder, queryGroup);
            var expected = $"" +
                $"SELECT [Field1], [Field2], [Field3] " +
                $"FROM [ClassName] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        private class TestWithFieldMappingClass : DataEntity
        {
            public int Field1 { get; set; }
            public string Field2 { get; set; }
            [Map("Field4")]
            public DateTime Field3 { get; set; }
        }

        [TestMethod]
        public void TestWithFieldMapping()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<TestWithFieldMappingClass>();
            var queryGroup = (QueryGroup)null;

            // Act
            var actual = statementBuilder.CreateQuery(queryBuilder, queryGroup);
            var expected = $"" +
                $"SELECT [Field1], [Field2], [Field4] " +
                $"FROM [TestWithFieldMappingClass] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        private class TestWithIgnoredFieldClass : DataEntity
        {
            public int Field1 { get; set; }
            public string Field2 { get; set; }
            [Attributes.Ignore(Enumerations.Command.Query)]
            public DateTime Field3 { get; set; }
        }

        [TestMethod]
        public void TestWithIgnoredField()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<TestWithIgnoredFieldClass>();
            var queryGroup = (QueryGroup)null;

            // Act
            var actual = statementBuilder.CreateQuery(queryBuilder, queryGroup);
            var expected = $"" +
                $"SELECT [Field1], [Field2] " +
                $"FROM [TestWithIgnoredFieldClass] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        private class TestWithTopClass : DataEntity
        {
            public int Field1 { get; set; }
            public string Field2 { get; set; }
            public DateTime Field3 { get; set; }
        }

        [TestMethod]
        public void TestWithTop()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<TestWithTopClass>();
            var queryGroup = (QueryGroup)null;

            // Act
            var actual = statementBuilder.CreateQuery(queryBuilder, queryGroup, top: 10);
            var expected = $"" +
                $"SELECT TOP (10) [Field1], [Field2], [Field3] " +
                $"FROM [TestWithTopClass] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        private class TestWithExpressionClass : DataEntity
        {
            public int Field1 { get; set; }
            public string Field2 { get; set; }
            public DateTime Field3 { get; set; }
        }

        [TestMethod]
        public void TestWithExpression()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<TestWithExpressionClass>();
            var expression = new { Field1 = 1 };
            var queryGroup = QueryGroup.Parse(expression);

            // Act
            var actual = statementBuilder.CreateQuery(queryBuilder, queryGroup);
            var expected = $"" +
                $"SELECT [Field1], [Field2], [Field3] " +
                $"FROM [TestWithExpressionClass] " +
                $"WHERE ([Field1] = @Field1) ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        /*******************************/

        /*
         * We should allow the developers to write whatever fields they would like to be
         * written at the query expression and ordering. We will only map back the object
         * fields back to the Data Entity properties. If any exception occurs because of
         * missing fields, then, simply show the exception
         */

        private class TestWithAscendingOrderFieldsClass : DataEntity
        {
            public int Field1 { get; set; }
            public string Field2 { get; set; }
            public DateTime Field3 { get; set; }
        }

        [TestMethod]
        public void TestWithAscendingOrderFields()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<TestWithAscendingOrderFieldsClass>();
            var queryGroup = (QueryGroup)null;
            var orderBy = OrderField.Parse(new { OrderField = Order.Ascending });

            // Act
            var actual = statementBuilder.CreateQuery(queryBuilder, queryGroup, orderBy);
            var expected = $"" +
                $"SELECT [Field1], [Field2], [Field3] " +
                $"FROM [TestWithAscendingOrderFieldsClass] " +
                $"ORDER BY [OrderField] ASC ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        private class TestWithDescendingOrderFieldsClass : DataEntity
        {
            public int Field1 { get; set; }
            public string Field2 { get; set; }
            public DateTime Field3 { get; set; }
        }

        [TestMethod]
        public void TestWithDescendingOrderFields()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<TestWithDescendingOrderFieldsClass>();
            var queryGroup = (QueryGroup)null;
            var orderBy = OrderField.Parse(new { OrderField = Order.Descending });

            // Act
            var actual = statementBuilder.CreateQuery(queryBuilder, queryGroup, orderBy);
            var expected = $"" +
                $"SELECT [Field1], [Field2], [Field3] " +
                $"FROM [TestWithDescendingOrderFieldsClass] " +
                $"ORDER BY [OrderField] DESC ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        private class TestWithAscendingAndDescendingOrderFieldsClass : DataEntity
        {
            public int Field1 { get; set; }
            public string Field2 { get; set; }
            public DateTime Field3 { get; set; }
        }

        [TestMethod]
        public void TestWithAscendingAndDescendingOrderFields()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<TestWithAscendingAndDescendingOrderFieldsClass>();
            var queryGroup = (QueryGroup)null;
            var orderBy = OrderField.Parse(new
            {
                AscendingField = Order.Ascending,
                DescendingField = Order.Descending
            });

            // Act
            var actual = statementBuilder.CreateQuery(queryBuilder, queryGroup, orderBy);
            var expected = $"" +
                $"SELECT [Field1], [Field2], [Field3] " +
                $"FROM [TestWithAscendingAndDescendingOrderFieldsClass] " +
                $"ORDER BY [AscendingField] ASC, [DescendingField] DESC ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        private class TestWithDescendingAndAscendingOrderFieldsClass : DataEntity
        {
            public int Field1 { get; set; }
            public string Field2 { get; set; }
            public DateTime Field3 { get; set; }
        }

        [TestMethod]
        public void TestWithDescendingAndAscendingOrderFields()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<TestWithDescendingAndAscendingOrderFieldsClass>();
            var queryGroup = (QueryGroup)null;
            var orderBy = OrderField.Parse(new
            {
                DescendingField = Order.Descending,
                AscendingField = Order.Ascending
            });

            // Act
            var actual = statementBuilder.CreateQuery(queryBuilder, queryGroup, orderBy);
            var expected = $"" +
                $"SELECT [Field1], [Field2], [Field3] " +
                $"FROM [TestWithDescendingAndAscendingOrderFieldsClass] " +
                $"ORDER BY [DescendingField] DESC, [AscendingField] ASC ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        private class TestWithAnyFieldsAtExpressionClass : DataEntity
        {
            public int Field1 { get; set; }
            public string Field2 { get; set; }
            public DateTime Field3 { get; set; }
        }

        [TestMethod]
        public void TestWithAnyFieldsAtExpression()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<TestWithAnyFieldsAtExpressionClass>();
            var expression = new { AnyField = 1 };
            var queryGroup = QueryGroup.Parse(expression);

            // Act
            var actual = statementBuilder.CreateQuery(queryBuilder, queryGroup);
            var expected = $"" +
                $"SELECT [Field1], [Field2], [Field3] " +
                $"FROM [TestWithAnyFieldsAtExpressionClass] " +
                $"WHERE ([AnyField] = @AnyField) ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        /*******************************/

        private class ThrowExceptionIfThereAreNoQueryableFieldsClass : DataEntity
        {
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void ThrowExceptionIfThereAreNoQueryableFields()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<ThrowExceptionIfThereAreNoQueryableFieldsClass>();
            var queryGroup = (QueryGroup)null;

            // Act/Assert
            statementBuilder.CreateQuery(queryBuilder, queryGroup);
        }

        private class ThrowExceptionIfAllFieldsWereIgnoredClass : DataEntity
        {
            [Attributes.Ignore(Command.Query)]
            public int Field1 { get; set; }
            [Attributes.Ignore(Command.Query)]
            public string Field2 { get; set; }
            [Attributes.Ignore(Command.Query)]
            public DateTime Field3 { get; set; }
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void ThrowExceptionIfAllFieldsWereIgnored()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<ThrowExceptionIfAllFieldsWereIgnoredClass>();
            var queryGroup = (QueryGroup)null;

            // Act/Assert
            statementBuilder.CreateQuery(queryBuilder, queryGroup);
        }
    }
}
