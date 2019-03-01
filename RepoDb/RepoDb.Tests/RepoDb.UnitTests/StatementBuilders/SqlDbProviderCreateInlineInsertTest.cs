using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Attributes;
using System;

namespace RepoDb.UnitTests.StatementBuilders
{
    [TestClass]
    public class SqlDbProviderCreateInlineInsertTest
    {
        private class TestSqlDbProviderCreateInlineInsertWithoutMappingsClass
        {
            public int Field1 { get; set; }
            public string Field2 { get; set; }
            public DateTime Field3 { get; set; }
        }

        [TestMethod]
        public void TestSqlDbProviderCreateInlineInsertWithoutMappings()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder<TestSqlDbProviderCreateInlineInsertWithoutMappingsClass>();
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });

            // Act
            var actual = statementBuilder.CreateInlineInsert(queryBuilder, fields);
            var expected = $"" +
                $"INSERT INTO [TestSqlDbProviderCreateInlineInsertWithoutMappingsClass] " +
                $"( [Field1], [Field2], [Field3] ) " +
                $"VALUES " +
                $"( @Field1, @Field2, @Field3 ) ; " +
                $"SELECT NULL AS [Result] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [Map("ClassName")]
        private class TestSqlDbProviderCreateInlineInsertWithClassMappingClass
        {
            public int Field1 { get; set; }
            public string Field2 { get; set; }
            public DateTime Field3 { get; set; }
        }

        [TestMethod]
        public void TestSqlDbProviderCreateInlineInsertWithClassMapping()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder<TestSqlDbProviderCreateInlineInsertWithClassMappingClass>();
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
        private class TestSqlDbProviderCreateInlineInsertWithFieldMappingsClass
        {
            public int Field1 { get; set; }
            public string Field2 { get; set; }
            [Map("Field4")]
            public DateTime Field3 { get; set; }
        }

        [TestMethod]
        public void TestSqlDbProviderCreateInlineInsertWithFieldMappings()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder<TestSqlDbProviderCreateInlineInsertWithFieldMappingsClass>();
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

        private class TestSqlDbProviderCreateInlineInsertWithIdClass
        {
            public int Id { get; set; }
            public string Field2 { get; set; }
            public DateTime Field3 { get; set; }
        }

        [TestMethod]
        public void TestSqlDbProviderCreateInlineInsertWithId()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder<TestSqlDbProviderCreateInlineInsertWithIdClass>();
            var fields = Field.From(new[] { "Field2", "Field3" });

            // Act
            var actual = statementBuilder.CreateInlineInsert(queryBuilder, fields);
            var expected = $"" +
                $"INSERT INTO [TestSqlDbProviderCreateInlineInsertWithIdClass] " +
                $"( [Field2], [Field3] ) " +
                $"VALUES " +
                $"( @Field2, @Field3 ) ; " +
                $"SELECT @Id AS [Result] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        private class TestSqlDbProviderCreateInlineInsertWithClassIdClass
        {
            public int TestSqlDbProviderCreateInlineInsertWithClassIdClassId { get; set; }
            public string Field2 { get; set; }
            public DateTime Field3 { get; set; }
        }

        [TestMethod]
        public void TestSqlDbProviderCreateInlineInsertWithClassId()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder<TestSqlDbProviderCreateInlineInsertWithClassIdClass>();
            var fields = Field.From(new[] { "Field2", "Field3" });

            // Act
            var actual = statementBuilder.CreateInlineInsert(queryBuilder, fields);
            var expected = $"" +
                $"INSERT INTO [TestSqlDbProviderCreateInlineInsertWithClassIdClass] " +
                $"( [Field2], [Field3] ) " +
                $"VALUES " +
                $"( @Field2, @Field3 ) ; " +
                $"SELECT @TestSqlDbProviderCreateInlineInsertWithClassIdClassId AS [Result] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [Map("ClassName")]
        private class TestSqlDbProviderCreateInlineInsertWithClassIdFromMappingClass
        {
            public int ClassNameId { get; set; }
            public string Field2 { get; set; }
            public DateTime Field3 { get; set; }
        }

        [TestMethod]
        public void TestSqlDbProviderCreateInlineInsertWithClassIdFromMapping()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder<TestSqlDbProviderCreateInlineInsertWithClassIdFromMappingClass>();
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
        private class TestSqlDbProviderCreateInlineInsertWithPrimaryKeyFromAttributeClass
        {
            [Primary]
            public int Field1 { get; set; }
            public string Field2 { get; set; }
            public DateTime Field3 { get; set; }
        }

        [TestMethod]
        public void TestSqlDbProviderCreateInlineInsertWithPrimaryKeyFromAttribute()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder<TestSqlDbProviderCreateInlineInsertWithPrimaryKeyFromAttributeClass>();
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
        
        private class TestSqlDbProviderCreateInlineInsertWithPrimaryIdentityClass
        {
            [Primary, Identity]
            public int Field1 { get; set; }
            public string Field2 { get; set; }
            public DateTime Field3 { get; set; }
        }

        [TestMethod]
        public void TestSqlDbProviderCreateInlineInsertWithPrimaryIdentity()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder<TestSqlDbProviderCreateInlineInsertWithPrimaryIdentityClass>();
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });

            // Act
            var actual = statementBuilder.CreateInlineInsert(queryBuilder, fields);
            var expected = $"" +
                $"INSERT INTO [TestSqlDbProviderCreateInlineInsertWithPrimaryIdentityClass] " +
                $"( [Field2], [Field3] ) " +
                $"VALUES " +
                $"( @Field2, @Field3 ) ; " +
                $"SELECT SCOPE_IDENTITY() AS [Result] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        private class ThrowExceptionOnSqlDbProviderCreateInlineInsertIfTheIdentityFieldIsNotThePrimaryKeyFieldClass
        {
            [Primary]
            public int Field1 { get; set; }
            [Identity]
            public string Field2 { get; set; }
            public DateTime Field3 { get; set; }
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void ThrowExceptionOnSqlDbProviderCreateInlineInsertIfTheIdentityFieldIsNotThePrimaryKeyField()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder<ThrowExceptionOnSqlDbProviderCreateInlineInsertIfTheIdentityFieldIsNotThePrimaryKeyFieldClass>();

            // Act/Assert
            statementBuilder.CreateInsert(queryBuilder);
        }

        private class ThrowExceptionOnSqlDbProviderCreateInlineInsertIfTheIdentityFieldIsNotTheClassIdFieldClass
        {
            public int ThrowExceptionOnSqlDbProviderCreateInlineInsertIfTheIdentityFieldIsNotTheClassIdFieldClassId { get; set; }
            [Identity]
            public string Field2 { get; set; }
            public DateTime Field3 { get; set; }
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void ThrowExceptionOnSqlDbProviderCreateInlineInsertIfTheIdentityFieldIsNotTheClassIdField()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder<ThrowExceptionOnSqlDbProviderCreateInlineInsertIfTheIdentityFieldIsNotTheClassIdFieldClass>();

            // Act/Assert
            statementBuilder.CreateInsert(queryBuilder);
        }

        private class ThrowExceptionOnSqlDbProviderCreateInlineInsertIfTheIdentityFieldIsNotTheIdFieldClass
        {
            public int Id { get; set; }
            [Identity]
            public string Field2 { get; set; }
            public DateTime Field3 { get; set; }
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void ThrowExceptionOnSqlDbProviderCreateInlineInsertIfTheIdentityFieldIsNotTheIdField()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder<ThrowExceptionOnSqlDbProviderCreateInlineInsertIfTheIdentityFieldIsNotTheIdFieldClass>();

            // Act/Assert
            statementBuilder.CreateInsert(queryBuilder);
        }

        [Map("ClassName")]
        private class ThrowExceptionOnSqlDbProviderCreateInlineInsertIfTheIdentityFieldIsNotTheClassMappingIdFieldClass
        {
            public int ClassNameId { get; set; }
            [Identity]
            public string Field2 { get; set; }
            public DateTime Field3 { get; set; }
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void ThrowExceptionOnSqlDbProviderCreateInlineInsertIfTheIdentityFieldIsNotTheClassMappingIdField()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder<ThrowExceptionOnSqlDbProviderCreateInlineInsertIfTheIdentityFieldIsNotTheClassMappingIdFieldClass>();

            // Act/Assert
            statementBuilder.CreateInsert(queryBuilder);
        }
        private class ThrowExceptionOnSqlDbProviderCreateInlineInsertIfFieldsAreNullClass
        {
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnSqlDbProviderCreateInlineInsertIfFieldsAreNull()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder<ThrowExceptionOnSqlDbProviderCreateInlineInsertIfFieldsAreNullClass>();

            // Act/Assert
            statementBuilder.CreateInlineInsert(queryBuilder, null);
        }

        private class ThrowOnSqlDbProviderCreateInlineInsertExceptionIfAtleastOneFieldIsMissingAtDataEntityClass
        {
            public int Field1 { get; set; }
            public string Field2 { get; set; }
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void ThrowExceptionOnSqlDbProviderCreateInlineInsertIfAtleastOneFieldIsMissingAtDataEntity()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder<ThrowOnSqlDbProviderCreateInlineInsertExceptionIfAtleastOneFieldIsMissingAtDataEntityClass>();
            var fields = Field.From(new[] { "Field1", "Field3" });

            // Act/Assert
            statementBuilder.CreateInlineInsert(queryBuilder, fields);
        }

        private class ThrowExceptionOnSqlDbProviderCreateInlineInsertIfAFieldIsMissingAtDataEntityMappingsClass
        {
            public int Field1 { get; set; }
            [Map("Field2")]
            public string Field3 { get; set; }
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void ThrowExceptionOnSqlDbProviderCreateInlineInsertIfAFieldIsMissingAtDataEntityMappings()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder<ThrowExceptionOnSqlDbProviderCreateInlineInsertIfAFieldIsMissingAtDataEntityMappingsClass>();
            var fields = Field.From(new[] { "Field1", "Field3" });

            // Act/Assert
            statementBuilder.CreateInlineInsert(queryBuilder, fields);
        }
    }
}
