using NUnit.Framework;
using RepoDb.Attributes;
using RepoDb.Enumerations;
using System;

namespace RepoDb.UnitTests.SqlDbStatementBuilderTest
{
    [TestFixture]
    public class CreateInlineInsert
    {
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

        private class TestCreateInlineInsertWithIdClass : DataEntity
        {
            public int Id { get; set; }
            public string Field2 { get; set; }
            public DateTime Field3 { get; set; }
        }

        [Test]
        public void TestCreateInlineInsertWithId()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<TestCreateInlineInsertWithIdClass>();
            var fields = Field.From(new[] { "Field2", "Field3" });

            // Act
            var actual = statementBuilder.CreateInlineInsert(queryBuilder, fields);
            var expected = $"" +
                $"INSERT INTO [TestCreateInlineInsertWithIdClass] " +
                $"( [Field2], [Field3] ) " +
                $"VALUES " +
                $"( @Field2, @Field3 ) ; " +
                $"SELECT @Id AS [Result] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        private class TestCreateInlineInsertWithIdentifiedPrimaryKeyClass : DataEntity
        {
            public int TestCreateInlineInsertWithIdentifiedPrimaryKeyClassId { get; set; }
            public string Field2 { get; set; }
            public DateTime Field3 { get; set; }
        }

        [Test]
        public void TestCreateInlineInsertWithIdentifiedPrimaryKey()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<TestCreateInlineInsertWithIdentifiedPrimaryKeyClass>();
            var fields = Field.From(new[] { "Field2", "Field3" });

            // Act
            var actual = statementBuilder.CreateInlineInsert(queryBuilder, fields);
            var expected = $"" +
                $"INSERT INTO [TestCreateInlineInsertWithIdentifiedPrimaryKeyClass] " +
                $"( [Field2], [Field3] ) " +
                $"VALUES " +
                $"( @Field2, @Field3 ) ; " +
                $"SELECT @TestCreateInlineInsertWithIdentifiedPrimaryKeyClassId AS [Result] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [Map("ClassName")]
        private class TestCreateInlineInsertWithIdentifiedPrimaryKeyFromMappingClass : DataEntity
        {
            public int ClassNameId { get; set; }
            public string Field2 { get; set; }
            public DateTime Field3 { get; set; }
        }

        [Test]
        public void TestCreateInlineInsertWithIdentifiedPrimaryKeyFromMapping()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<TestCreateInlineInsertWithIdentifiedPrimaryKeyFromMappingClass>();
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
        private class TestCreateInlineInsertWithPrimaryKeyFromAttributeClass : DataEntity
        {
            [Primary]
            public int Field1 { get; set; }
            public string Field2 { get; set; }
            public DateTime Field3 { get; set; }
        }

        [Test]
        public void TestCreateInlineInsertWithPrimaryKeyFromAttribute()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<TestCreateInlineInsertWithPrimaryKeyFromAttributeClass>();
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
    }
}
