using NUnit.Framework;
using RepoDb.Attributes;
using RepoDb.Enumerations;
using System;
using System.Collections.Generic;

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

            // Assert
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

            // Assert
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

            // Assert
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

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [Map("ClassName")]
        private class TestCreateBatchQueryWithFieldMappingsAndWithIgnoredBatchQueryCommandClass : DataEntity
        {
            public int Field1 { get; set; }
            [Attributes.Ignore(Command.BatchQuery)]
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

        [Map("ClassName")]
        private class TestCreateBatchQueryWithFieldMappingsAndWithIgnoredQueryCommandClass : DataEntity
        {
            public int Field1 { get; set; }
            [Attributes.Ignore(Command.Query)]
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

        [Map("ClassName")]
        private class TestCreateBatchQueryWithFieldMappingsAndWithIgnoredBathQueryAndQueryCommandClass : DataEntity
        {
            public int Field1 { get; set; }
            [Attributes.Ignore(Command.Query)]
            public int Field2 { get; set; }
            public int Field3 { get; set; }
            [Attributes.Ignore(Command.BatchQuery)]
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
            var expected = $"" +
                $"SELECT COUNT_BIG (1) AS [Counted] " +
                $"FROM [TestCreateCountWithoutMappingsClass] ;";

            // Assert
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
            var expected = $"" +
                $"SELECT COUNT_BIG (1) AS [Counted] " +
                $"FROM [ClassName] ;";

            // Assert
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
            var expression = new { Field1 = 1 };

            // Act
            var queryGroup = QueryGroup.Parse(expression);
            var actual = statementBuilder.CreateCount(queryBuilder, queryGroup);
            var expected = $"" +
                $"SELECT COUNT_BIG (1) AS [Counted] " +
                $"FROM [TestCreateCountWithExpressionsClass] " +
                $"WHERE ([Field1] = @Field1) ;";

            // Assert
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
            var expected = $"" +
                $"DELETE " +
                $"FROM [TestCreateDeleteWithoutMappingsClass] ;";

            // Assert
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
            var expression = new { Field1 = 1 };

            // Act
            var queryGroup = QueryGroup.Parse(expression);
            var actual = statementBuilder.CreateDelete(queryBuilder, queryGroup);
            var expected = $"" +
                $"DELETE " +
                $"FROM [TestCreateDeleteWithoutMappingsAndWithExpressionsClass] " +
                $"WHERE ([Field1] = @Field1) ;";

            // Assert
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
            var expression = new { Field1 = 1 };

            // Act
            var queryGroup = QueryGroup.Parse(expression);
            var actual = statementBuilder.CreateDelete(queryBuilder, queryGroup);
            var expected = $"" +
                $"DELETE " +
                $"FROM [ClassName] " +
                $"WHERE ([Field1] = @Field1) ;";

            // Assert
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
            var expected = $"" +
                $"DELETE " +
                $"FROM [TestCreateDeleteAllWithoutMappingsClass] ;";

            // Assert
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
            var expected = $"" +
                $"DELETE " +
                $"FROM [ClassName] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        #endregion

        #region CreateInlineInsert

        private class TestCreateInlineInsertWithoutMappingsClass : DataEntity
        {
            public int Field1 { get; set; }
            public string Field2 { get; set; }
            public DateTime Field3 { get; set; }
        }

        [Test]
        public void TestCreateInlineInsertWithoutMappings()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<TestCreateInlineInsertWithoutMappingsClass>();
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });

            // Act
            var actual = statementBuilder.CreateInlineInsert(queryBuilder, fields);
            var expected = $"" +
                $"INSERT INTO [TestCreateInlineInsertWithoutMappingsClass] " +
                $"( [Field1], [Field2], [Field3] ) " +
                $"VALUES " +
                $"( @Field1, @Field2, @Field3 ) ; " +
                $"SELECT NULL AS [Result] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [Map("ClassName")]
        private class TestCreateInlineInsertWithClassMappingClass : DataEntity
        {
            public int Field1 { get; set; }
            public string Field2 { get; set; }
            public DateTime Field3 { get; set; }
        }

        [Test]
        public void TestCreateInlineInsertWithClassMapping()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<TestCreateInlineInsertWithClassMappingClass>();
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });

            // Act
            var actual = statementBuilder.CreateInlineInsert(queryBuilder, fields);
            var expected = $"" +
                $"INSERT INTO [ClassName] " +
                $"( [Field1], [Field2], [Field3] ) " +
                $"VALUES " +
                $"( @Field1, @Field2, @Field3 ) ; " +
                $"SELECT NULL AS [Result] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [Map("ClassName")]
        private class TestCreateInlineInsertWithFieldMappingsClass : DataEntity
        {
            public int Field1 { get; set; }
            public string Field2 { get; set; }
            [Map("Field4")]
            public DateTime Field3 { get; set; }
        }

        [Test]
        public void TestCreateInlineInsertWithFieldMappings()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<TestCreateInlineInsertWithFieldMappingsClass>();
            var fields = Field.From(new[] { "Field1", "Field2", "Field4" });

            // Act
            var actual = statementBuilder.CreateInlineInsert(queryBuilder, fields);
            var expected = $"" +
                $"INSERT INTO [ClassName] " +
                $"( [Field1], [Field2], [Field4] ) " +
                $"VALUES " +
                $"( @Field1, @Field2, @Field4 ) ; " +
                $"SELECT NULL AS [Result] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        private class TestCreateInlineInserReturnIdClass : DataEntity
        {
            public int Id { get; set; }
            public string Field2 { get; set; }
            public DateTime Field3 { get; set; }
        }

        [Test]
        public void TestCreateInlineInserReturnId()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<TestCreateInlineInserReturnIdClass>();
            var fields = Field.From(new[] { "Field2", "Field3" });

            // Act
            var actual = statementBuilder.CreateInlineInsert(queryBuilder, fields);
            var expected = $"" +
                $"INSERT INTO [TestCreateInlineInserReturnIdClass] " +
                $"( [Field2], [Field3] ) " +
                $"VALUES " +
                $"( @Field2, @Field3 ) ; " +
                $"SELECT @Id AS [Result] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        private class TestCreateInlineInserReturnIdentifiedPrimaryKeyClass : DataEntity
        {
            public int TestCreateInlineInserReturnIdentifiedPrimaryKeyClassId { get; set; }
            public string Field2 { get; set; }
            public DateTime Field3 { get; set; }
        }

        [Test]
        public void TestCreateInlineInserReturnIdentifiedPrimaryKey()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<TestCreateInlineInserReturnIdentifiedPrimaryKeyClass>();
            var fields = Field.From(new[] { "Field2", "Field3" });

            // Act
            var actual = statementBuilder.CreateInlineInsert(queryBuilder, fields);
            var expected = $"" +
                $"INSERT INTO [TestCreateInlineInserReturnIdentifiedPrimaryKeyClass] " +
                $"( [Field2], [Field3] ) " +
                $"VALUES " +
                $"( @Field2, @Field3 ) ; " +
                $"SELECT @TestCreateInlineInserReturnIdentifiedPrimaryKeyClassId AS [Result] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [Map("ClassName")]
        private class TestCreateInlineInsertReturnIdentifiedPrimaryKeyFromMappingClass : DataEntity
        {
            public int ClassNameId { get; set; }
            public string Field2 { get; set; }
            public DateTime Field3 { get; set; }
        }

        [Test]
        public void TestCreateInlineInsertReturnIdentifiedPrimaryKeyFromMapping()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<TestCreateInlineInsertReturnIdentifiedPrimaryKeyFromMappingClass>();
            var fields = Field.From(new[] { "Field2", "Field3" });

            // Act
            var actual = statementBuilder.CreateInlineInsert(queryBuilder, fields);
            var expected = $"" +
                $"INSERT INTO [ClassName] " +
                $"( [Field2], [Field3] ) " +
                $"VALUES " +
                $"( @Field2, @Field3 ) ; " +
                $"SELECT @ClassNameId AS [Result] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [Map("ClassName")]
        private class TestCreateInlineInsertReturnPrimaryKeyFromAttributeClass : DataEntity
        {
            [Primary]
            public int Field1 { get; set; }
            public string Field2 { get; set; }
            public DateTime Field3 { get; set; }
        }

        [Test]
        public void TestCreateInlineInsertReturnPrimaryKeyFromAttribute()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<TestCreateInlineInsertReturnPrimaryKeyFromAttributeClass>();
            var fields = Field.From(new[] { "Field2", "Field3" });

            // Act
            var actual = statementBuilder.CreateInlineInsert(queryBuilder, fields);
            var expected = $"" +
                $"INSERT INTO [ClassName] " +
                $"( [Field2], [Field3] ) " +
                $"VALUES " +
                $"( @Field2, @Field3 ) ; " +
                $"SELECT @Field1 AS [Result] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [Map("ClassName")]
        private class TestCreateInlineInsertOverrideIgnoreFromInsertOperationClass : DataEntity
        {
            public int Field1 { get; set; }
            public string Field2 { get; set; }
            [Attributes.Ignore(Command.Insert)]
            public DateTime Field3 { get; set; }
        }

        [Test]
        public void TestCreateInlineInsertOverrideIgnoreFromInsertOperation()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<TestCreateInlineInsertOverrideIgnoreFromInsertOperationClass>();
            var fields = Field.From(new[] { "Field2", "Field3" });

            // Act
            var actual = statementBuilder.CreateInlineInsert(queryBuilder, fields, true);
            var expected = $"" +
                $"INSERT INTO [ClassName] " +
                $"( [Field2], [Field3] ) " +
                $"VALUES " +
                $"( @Field2, @Field3 ) ; " +
                $"SELECT NULL AS [Result] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [Map("ClassName")]
        private class TestCreateInlineInsertOverrideIgnoreFromInlineInsertOperationClass : DataEntity
        {
            public int Field1 { get; set; }
            public string Field2 { get; set; }
            [Attributes.Ignore(Command.InlineInsert)]
            public DateTime Field3 { get; set; }
        }

        [Test]
        public void TestCreateInlineInsertOverrideIgnoreFromInlineInsertOperation()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<TestCreateInlineInsertOverrideIgnoreFromInlineInsertOperationClass>();
            var fields = Field.From(new[] { "Field2", "Field3" });

            // Act
            var actual = statementBuilder.CreateInlineInsert(queryBuilder, fields, true);
            var expected = $"" +
                $"INSERT INTO [ClassName] " +
                $"( [Field2], [Field3] ) " +
                $"VALUES " +
                $"( @Field2, @Field3 ) ; " +
                $"SELECT NULL AS [Result] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        private class ThrowExceptionAtInlineInsertIfFieldsAreNullClass : DataEntity
        {
        }

        [Test]
        public void ThrowExceptionAtInlineInsertIfFieldsAreNull()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<ThrowExceptionAtInlineInsertIfFieldsAreNullClass>();

            // Act/Assert
            Assert.Throws<NullReferenceException>(() => statementBuilder.CreateInlineInsert(queryBuilder, null));
        }

        private class ThrowExceptionAtInlineInsertIfAtleastOneFieldIsMissingAtDataEntityClass : DataEntity
        {
            public int Field1 { get; set; }
            public string Field2 { get; set; }
        }

        [Test]
        public void ThrowExceptionAtInlineInsertIfAtleastOneFieldIsMissingAtDataEntity()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<ThrowExceptionAtInlineInsertIfAtleastOneFieldIsMissingAtDataEntityClass>();
            var fields = Field.From(new[] { "Field1", "Field3" });

            // Act/Assert
            Assert.Throws<InvalidOperationException>(() => statementBuilder.CreateInlineInsert(queryBuilder, fields));
        }

        private class ThrowExceptionAtInlineInsertIfAFieldIsMissingAtDataEntityMappingsClass : DataEntity
        {
            public int Field1 { get; set; }
            [Map("Field2")]
            public string Field3 { get; set; }
        }

        [Test]
        public void ThrowExceptionAtInlineInsertIfAFieldIsMissingAtDataEntityMappings()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<ThrowExceptionAtInlineInsertIfAFieldIsMissingAtDataEntityMappingsClass>();
            var fields = Field.From(new[] { "Field1", "Field3" });

            // Act/Assert
            Assert.Throws<InvalidOperationException>(() => statementBuilder.CreateInlineInsert(queryBuilder, fields));
        }

        private class ThrowExceptionAtInlineInsertIfAFieldIsIgnoredAtInsertOperationClass : DataEntity
        {
            public int Field1 { get; set; }
            public string Field2 { get; set; }
            [Attributes.Ignore(Command.Insert)]
            public DateTime Field3 { get; set; }
        }

        [Test]
        public void ThrowExceptionAtInlineInsertIfAFieldIsIgnoredAtInsertOperation()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<ThrowExceptionAtInlineInsertIfAFieldIsIgnoredAtInsertOperationClass>();
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });

            // Act/Assert
            Assert.Throws<InvalidOperationException>(() => statementBuilder.CreateInlineInsert(queryBuilder, fields));
        }

        private class ThrowExceptionAtInlineInsertIfAFieldIsIgnoredAtInlineInsertOperationClass : DataEntity
        {
            public int Field1 { get; set; }
            public string Field2 { get; set; }
            [Attributes.Ignore(Command.InlineInsert)]
            public DateTime Field3 { get; set; }
        }

        [Test]
        public void ThrowExceptionAtInlineInsertIfAFieldIsIgnoredAtInlineInsertOperation()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<ThrowExceptionAtInlineInsertIfAFieldIsIgnoredAtInlineInsertOperationClass>();
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });

            // Act/Assert
            Assert.Throws<InvalidOperationException>(() => statementBuilder.CreateInlineInsert(queryBuilder, fields));
        }

        #endregion

        #region CreateInlineMerge

        private class TestCreateInlineMergeWithoutMappingsClass : DataEntity
        {
            public int Field1 { get; set; }
            public string Field2 { get; set; }
            public DateTime Field3 { get; set; }
        }

        [Test]
        public void TestCreateInlineMergeWithoutMappings()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<TestCreateInlineMergeWithoutMappingsClass>();
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });
            var qualifiers = Field.From(new[] { "Field1", "Field2" });

            // Act
            var actual = statementBuilder.CreateInlineMerge(queryBuilder, fields, qualifiers);
            var expected = $"" +
                $"MERGE [TestCreateInlineMergeWithoutMappingsClass] AS T " +
                $"USING ( SELECT @Field1 AS [Field1], @Field2 AS [Field2], @Field3 AS [Field3] ) " +
                $"AS S ON ( S.[Field1] = T.[Field1] AND S.[Field2] = T.[Field2] ) " +
                $"WHEN NOT MATCHED THEN " +
                $"INSERT ( [Field1], [Field2], [Field3] ) " +
                $"VALUES ( @Field1, @Field2, @Field3 ) " +
                $"WHEN MATCHED THEN " +
                $"UPDATE SET [Field1] = S.[Field1], [Field2] = S.[Field2], [Field3] = S.[Field3] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [Map("ClassName")]
        private class TestCreateInlineMergeWithClassMappingClass : DataEntity
        {
            public int Field1 { get; set; }
            public string Field2 { get; set; }
            public DateTime Field3 { get; set; }
        }

        [Test]
        public void TestCreateInlineMergeWithClassMapping()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<TestCreateInlineMergeWithClassMappingClass>();
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });
            var qualifiers = Field.From(new[] { "Field1", "Field2" });

            // Act
            var actual = statementBuilder.CreateInlineMerge(queryBuilder, fields, qualifiers);
            var expected = $"" +
                $"MERGE [ClassName] AS T " +
                $"USING ( SELECT @Field1 AS [Field1], @Field2 AS [Field2], @Field3 AS [Field3] ) " +
                $"AS S ON ( S.[Field1] = T.[Field1] AND S.[Field2] = T.[Field2] ) " +
                $"WHEN NOT MATCHED THEN " +
                $"INSERT ( [Field1], [Field2], [Field3] ) " +
                $"VALUES ( @Field1, @Field2, @Field3 ) " +
                $"WHEN MATCHED THEN " +
                $"UPDATE SET [Field1] = S.[Field1], [Field2] = S.[Field2], [Field3] = S.[Field3] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        private class TestCreateInlineMergeWithAttributeMappingsClass : DataEntity
        {
            public int Field1 { get; set; }
            public string Field2 { get; set; }
            [Map("Field4")]
            public DateTime Field3 { get; set; }
        }

        [Test]
        public void TestCreateInlineMergeWithAttributeMappings()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<TestCreateInlineMergeWithAttributeMappingsClass>();
            var fields = Field.From(new[] { "Field1", "Field2", "Field4" });
            var qualifiers = Field.From(new[] { "Field1", "Field2" });

            // Act
            var actual = statementBuilder.CreateInlineMerge(queryBuilder, fields, qualifiers);
            var expected = $"" +
                $"MERGE [TestCreateInlineMergeWithAttributeMappingsClass] AS T " +
                $"USING ( SELECT @Field1 AS [Field1], @Field2 AS [Field2], @Field4 AS [Field4] ) " +
                $"AS S ON ( S.[Field1] = T.[Field1] AND S.[Field2] = T.[Field2] ) " +
                $"WHEN NOT MATCHED THEN " +
                $"INSERT ( [Field1], [Field2], [Field4] ) " +
                $"VALUES ( @Field1, @Field2, @Field4 ) " +
                $"WHEN MATCHED THEN " +
                $"UPDATE SET [Field1] = S.[Field1], [Field2] = S.[Field2], [Field4] = S.[Field4] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        private class TestCreateInlineMergeWithIgnoreInsertFieldClass : DataEntity
        {
            public int Field1 { get; set; }
            public string Field2 { get; set; }
            [Attributes.Ignore(Command.Insert)]
            public DateTime Field3 { get; set; }
        }

        [Test]
        public void TestCreateInlineMergeWithIgnoreInsertField()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<TestCreateInlineMergeWithIgnoreInsertFieldClass>();
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });
            var qualifiers = Field.From(new[] { "Field1", "Field2" });

            // Act
            var actual = statementBuilder.CreateInlineMerge(queryBuilder, fields, qualifiers);
            var expected = $"" +
                $"MERGE [TestCreateInlineMergeWithIgnoreInsertFieldClass] AS T " +
                $"USING ( SELECT @Field1 AS [Field1], @Field2 AS [Field2], @Field3 AS [Field3] ) " +
                $"AS S ON ( S.[Field1] = T.[Field1] AND S.[Field2] = T.[Field2] ) " +
                $"WHEN NOT MATCHED THEN " +
                $"INSERT ( [Field1], [Field2] ) " +
                $"VALUES ( @Field1, @Field2 ) " +
                $"WHEN MATCHED THEN " +
                $"UPDATE SET [Field1] = S.[Field1], [Field2] = S.[Field2], [Field3] = S.[Field3] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        private class TestCreateInlineMergeWithIgnoreUpdateFieldClass : DataEntity
        {
            public int Field1 { get; set; }
            public string Field2 { get; set; }
            [Attributes.Ignore(Command.Update)]
            public DateTime Field3 { get; set; }
        }

        [Test]
        public void TestCreateInlineMergeWithIgnoreUpdateField()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<TestCreateInlineMergeWithIgnoreUpdateFieldClass>();
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });
            var qualifiers = Field.From(new[] { "Field1", "Field2" });

            // Act
            var actual = statementBuilder.CreateInlineMerge(queryBuilder, fields, qualifiers);
            var expected = $"" +
                $"MERGE [TestCreateInlineMergeWithIgnoreUpdateFieldClass] AS T " +
                $"USING ( SELECT @Field1 AS [Field1], @Field2 AS [Field2], @Field3 AS [Field3] ) " +
                $"AS S ON ( S.[Field1] = T.[Field1] AND S.[Field2] = T.[Field2] ) " +
                $"WHEN NOT MATCHED THEN " +
                $"INSERT ( [Field1], [Field2], [Field3] ) " +
                $"VALUES ( @Field1, @Field2, @Field3 ) " +
                $"WHEN MATCHED THEN " +
                $"UPDATE SET [Field1] = S.[Field1], [Field2] = S.[Field2] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        private class TestCreateInlineMergeWithPrimaryKeyClass : DataEntity
        {
            [Primary]
            public int Field1 { get; set; }
            public string Field2 { get; set; }
            public DateTime Field3 { get; set; }
        }

        [Test]
        public void TestCreateInlineMergeWithPrimaryKey()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<TestCreateInlineMergeWithPrimaryKeyClass>();
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });
            var qualifiers = (IEnumerable<Field>)null;

            // Act
            var actual = statementBuilder.CreateInlineMerge(queryBuilder, fields, qualifiers);
            var expected = $"" +
                $"MERGE [TestCreateInlineMergeWithPrimaryKeyClass] AS T " +
                $"USING ( SELECT @Field1 AS [Field1], @Field2 AS [Field2], @Field3 AS [Field3] ) " +
                $"AS S ON ( S.[Field1] = T.[Field1] ) " +
                $"WHEN NOT MATCHED THEN " +
                $"INSERT ( [Field1], [Field2], [Field3] ) " +
                $"VALUES ( @Field1, @Field2, @Field3 ) " +
                $"WHEN MATCHED THEN " +
                $"UPDATE SET [Field2] = S.[Field2], [Field3] = S.[Field3] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        private class ThrowExceptionAtCreateInlineMergeIfTheFieldsAreNullClass : DataEntity
        {
            public int Field1 { get; set; }
        }

        [Test]
        public void ThrowExceptionAtCreateInlineMergeIfTheFieldsAreNull()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<ThrowExceptionAtCreateInlineMergeIfTheFieldsAreNullClass>();
            var fields = (IEnumerable<Field>)null;
            var qualifiers = Field.From(new[] { "Field1" });

            // Act/Assert
            Assert.Throws<NullReferenceException>(() => statementBuilder.CreateInlineMerge(queryBuilder, fields, qualifiers));
        }

        private class ThrowExceptionAtCreateInlineMergeIfTheQualifiersAreNullWithoutPrimaryKeyClass : DataEntity
        {
            public int Field1 { get; set; }
        }

        [Test]
        public void ThrowExceptionAtCreateInlineMergeIfTheQualifiersAreNullWithoutPrimaryKey()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<ThrowExceptionAtCreateInlineMergeIfTheQualifiersAreNullWithoutPrimaryKeyClass>();
            var fields = Field.From(new[] { "Field1" });
            var qualifiers = (IEnumerable<Field>)null;

            // Act/Assert
            Assert.Throws<NullReferenceException>(() => statementBuilder.CreateInlineMerge(queryBuilder, fields, qualifiers));
        }

        private class ThrowExceptionAtCreateInlineMergeIfAQualifierFieldIsNotPresentAtDataEntityClass : DataEntity
        {
            public int Field1 { get; set; }
            public string Field2 { get; set; }
            public DateTime Field3 { get; set; }
        }

        [Test]
        public void ThrowExceptionAtCreateInlineMergeIfAQualifierFieldIsNotPresentAtDataEntity()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<ThrowExceptionAtCreateInlineMergeIfAQualifierFieldIsNotPresentAtDataEntityClass>();
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });
            var qualifiers = Field.From(new[] { "Field1", "Field4" });

            // Act/Assert
            Assert.Throws<InvalidOperationException>(() => statementBuilder.CreateInlineMerge(queryBuilder, fields, qualifiers));
        }

        private class ThrowExceptionAtCreateInlineMergeIfAQualifierFieldIsNotPresentAtDataEntityFieldMappingClass : DataEntity
        {
            public int Field1 { get; set; }
            public string Field2 { get; set; }
            [Map("Field4")]
            public DateTime Field3 { get; set; }
        }

        [Test]
        public void ThrowExceptionAtCreateInlineMergeIfAQualifierFieldIsNotPresentAtDataEntityFieldMapping()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<ThrowExceptionAtCreateInlineMergeIfAQualifierFieldIsNotPresentAtDataEntityFieldMappingClass>();
            var fields = Field.From(new[] { "Field1", "Field2", "Field4" });
            var qualifiers = Field.From(new[] { "Field1", "Field3" });

            // Act/Assert
            Assert.Throws<InvalidOperationException>(() => statementBuilder.CreateInlineMerge(queryBuilder, fields, qualifiers));
        }

        private class ThrowExceptionAtCreateInlineMergeIfTheFieldsContainsAnIgnoreInlineMergeFieldsClass : DataEntity
        {
            public int Field1 { get; set; }
            public string Field2 { get; set; }
            [Attributes.Ignore(Command.InlineMerge)]
            public DateTime Field3 { get; set; }
        }

        [Test]
        public void ThrowExceptionAtCreateInlineMergeIfTheFieldsContainsAnIgnoreInlineMergeFields()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<ThrowExceptionAtCreateInlineMergeIfTheFieldsContainsAnIgnoreInlineMergeFieldsClass>();
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });
            var qualifiers = Field.From(new[] { "Field1", "Field2" });

            // Act/Assert
            Assert.Throws<InvalidOperationException>(() => statementBuilder.CreateInlineMerge(queryBuilder, fields, qualifiers));
        }

        private class ThrowExceptionAtCreateInlineMergeIfTheQualifiersContainsAnIgnoreInlineMergeFieldsClass : DataEntity
        {
            public int Field1 { get; set; }
            public string Field2 { get; set; }
            [Attributes.Ignore(Command.InlineMerge)]
            public DateTime Field3 { get; set; }
            public double Field4 { get; set; }
        }

        [Test]
        public void ThrowExceptionAtCreateInlineMergeIfTheQualifiersContainsAnIgnoreInlineMergeFields()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<ThrowExceptionAtCreateInlineMergeIfTheQualifiersContainsAnIgnoreInlineMergeFieldsClass>();
            var fields = Field.From(new[] { "Field1", "Field2", "Field4" });
            var qualifiers = Field.From(new[] { "Field1", "Field3" });

            // Act/Assert
            Assert.Throws<InvalidOperationException>(() => statementBuilder.CreateInlineMerge(queryBuilder, fields, qualifiers));
        }

        #endregion
    }
}
