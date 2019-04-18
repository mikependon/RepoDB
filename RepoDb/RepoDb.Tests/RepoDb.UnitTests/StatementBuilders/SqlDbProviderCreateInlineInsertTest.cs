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
            var queryBuilder = new QueryBuilder();
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });

            // Act
            var actual = statementBuilder.CreateInlineInsert<TestSqlDbProviderCreateInlineInsertWithoutMappingsClass>(queryBuilder, null, fields);
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
            var queryBuilder = new QueryBuilder();
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });

            // Act
            var actual = statementBuilder.CreateInlineInsert<TestSqlDbProviderCreateInlineInsertWithClassMappingClass>(queryBuilder, null, fields);
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
            var queryBuilder = new QueryBuilder();
            var fields = Field.From(new[] { "Field1", "Field2", "Field4" });

            // Act
            var actual = statementBuilder.CreateInlineInsert<TestSqlDbProviderCreateInlineInsertWithFieldMappingsClass>(queryBuilder, null, fields);
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
            var queryBuilder = new QueryBuilder();
            var fields = Field.From(new[] { "Field2", "Field3" });

            // Act
            var actual = statementBuilder.CreateInlineInsert<TestSqlDbProviderCreateInlineInsertWithIdClass>(queryBuilder, null, fields);
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
            var queryBuilder = new QueryBuilder();
            var fields = Field.From(new[] { "Field2", "Field3" });

            // Act
            var actual = statementBuilder.CreateInlineInsert<TestSqlDbProviderCreateInlineInsertWithClassIdClass>(queryBuilder, null, fields);
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
            var queryBuilder = new QueryBuilder();
            var fields = Field.From(new[] { "Field2", "Field3" });

            // Act
            var actual = statementBuilder.CreateInlineInsert<TestSqlDbProviderCreateInlineInsertWithClassIdFromMappingClass>(queryBuilder, null, fields);
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
            var queryBuilder = new QueryBuilder();
            var fields = Field.From(new[] { "Field2", "Field3" });

            // Act
            var actual = statementBuilder.CreateInlineInsert<TestSqlDbProviderCreateInlineInsertWithPrimaryKeyFromAttributeClass>(queryBuilder, null, fields);
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
            var queryBuilder = new QueryBuilder();
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });

            // Act
            var actual = statementBuilder.CreateInlineInsert<TestSqlDbProviderCreateInlineInsertWithPrimaryIdentityClass>(queryBuilder, null, fields);
            var expected = $"" +
                $"INSERT INTO [TestSqlDbProviderCreateInlineInsertWithPrimaryIdentityClass] " +
                $"( [Field2], [Field3] ) " +
                $"VALUES " +
                $"( @Field2, @Field3 ) ; " +
                $"SELECT SCOPE_IDENTITY() AS [Result] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestSqlDbProviderCreateInlineInsertViaTableName()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder();
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });

            // Act
            var actual = statementBuilder.CreateInlineInsert(queryBuilder, "TargetTable", null, fields);
            var expected = $"" +
                $"INSERT INTO [TargetTable] " +
                $"( [Field1], [Field2], [Field3] ) " +
                $"VALUES " +
                $"( @Field1, @Field2, @Field3 ) ; " +
                $"SELECT NULL AS [Result] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestSqlDbProviderCreateInlineInsertViaTableNameWithSchema()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder();
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });

            // Act
            var actual = statementBuilder.CreateInlineInsert(queryBuilder, "[dbo].[TargetTable]", null, fields);
            var expected = $"" +
                $"INSERT INTO [dbo].[TargetTable] " +
                $"( [Field1], [Field2], [Field3] ) " +
                $"VALUES " +
                $"( @Field1, @Field2, @Field3 ) ; " +
                $"SELECT NULL AS [Result] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestSqlDbProviderCreateInlineInsertViaTableNameWithUnquotedSchema()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder();
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });

            // Act
            var actual = statementBuilder.CreateInlineInsert(queryBuilder, "dbo.TargetTable", null, fields);
            var expected = $"" +
                $"INSERT INTO [dbo].[TargetTable] " +
                $"( [Field1], [Field2], [Field3] ) " +
                $"VALUES " +
                $"( @Field1, @Field2, @Field3 ) ; " +
                $"SELECT NULL AS [Result] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestSqlDbProviderCreateInlineInsertViaTableNameWithIdentityPrimaryDbField()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder();
            var dbField = new DbField("Field1", true, true, false);
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });

            // Act
            var actual = statementBuilder.CreateInlineInsert(queryBuilder, "TargetTable", dbField, fields);
            var expected = $"" +
                $"INSERT INTO [TargetTable] " +
                $"( [Field2], [Field3] ) " +
                $"VALUES " +
                $"( @Field2, @Field3 ) ; " +
                $"SELECT SCOPE_IDENTITY() AS [Result] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestSqlDbProviderCreateInlineInsertViaTableNameWithNonIdentityPrimaryDbField()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder();
            var dbField = new DbField("Field1", true, false, false);
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });

            // Act
            var actual = statementBuilder.CreateInlineInsert(queryBuilder, "TargetTable", dbField, fields);
            var expected = $"" +
                $"INSERT INTO [TargetTable] " +
                $"( [Field1], [Field2], [Field3] ) " +
                $"VALUES " +
                $"( @Field1, @Field2, @Field3 ) ; " +
                $"SELECT @Field1 AS [Result] ;";

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
            var queryBuilder = new QueryBuilder();

            // Act/Assert
            statementBuilder.CreateInsert<ThrowExceptionOnSqlDbProviderCreateInlineInsertIfTheIdentityFieldIsNotThePrimaryKeyFieldClass>(queryBuilder);
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
            var queryBuilder = new QueryBuilder();

            // Act/Assert
            statementBuilder.CreateInsert<ThrowExceptionOnSqlDbProviderCreateInlineInsertIfTheIdentityFieldIsNotTheClassIdFieldClass>(queryBuilder);
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
            var queryBuilder = new QueryBuilder();

            // Act/Assert
            statementBuilder.CreateInsert<ThrowExceptionOnSqlDbProviderCreateInlineInsertIfTheIdentityFieldIsNotTheIdFieldClass>(queryBuilder);
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
            var queryBuilder = new QueryBuilder();

            // Act/Assert
            statementBuilder.CreateInsert<ThrowExceptionOnSqlDbProviderCreateInlineInsertIfTheIdentityFieldIsNotTheClassMappingIdFieldClass>(queryBuilder);
        }
        private class ThrowExceptionOnSqlDbProviderCreateInlineInsertIfFieldsAreNullClass
        {
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnSqlDbProviderCreateInlineInsertIfFieldsAreNull()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder();

            // Act/Assert
            statementBuilder.CreateInlineInsert<ThrowExceptionOnSqlDbProviderCreateInlineInsertIfFieldsAreNullClass>(queryBuilder, null);
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
            var queryBuilder = new QueryBuilder();
            var fields = Field.From(new[] { "Field1", "Field3" });

            // Act/Assert
            statementBuilder.CreateInlineInsert<ThrowOnSqlDbProviderCreateInlineInsertExceptionIfAtleastOneFieldIsMissingAtDataEntityClass>(queryBuilder, null, fields);
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
            var queryBuilder = new QueryBuilder();
            var fields = Field.From(new[] { "Field1", "Field3" });

            // Act/Assert
            statementBuilder.CreateInlineInsert<ThrowExceptionOnSqlDbProviderCreateInlineInsertIfAFieldIsMissingAtDataEntityMappingsClass>(queryBuilder, null, fields);
        }
    }
}
