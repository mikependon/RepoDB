using NUnit.Framework;
using RepoDb.Attributes;
using RepoDb.Enumerations;
using System;

namespace RepoDb.UnitTests.SqlDbStatementBuilderTest
{
    [TestFixture]
    public class CreateInlineUpdate
    {
        private class TestCreateInlineUpdateWithoutMappingsClass : DataEntity
        {
            public int Field1 { get; set; }
            public string Field2 { get; set; }
            public DateTime Field3 { get; set; }
        }

        [Test]
        public void TestCreateInlineUpdateWithoutMappings()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<TestCreateInlineUpdateWithoutMappingsClass>();
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });

            // Act
            var actual = statementBuilder.CreateInlineUpdate(queryBuilder, fields, null);
            var expected = $"" +
                $"UPDATE [TestCreateInlineUpdateWithoutMappingsClass] " +
                $"SET [Field1] = @Field1, " +
                $"[Field2] = @Field2, " +
                $"[Field3] = @Field3 ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [Map("ClassName")]
        private class TestCreateInlineUpdateWithClassMappingClass : DataEntity
        {
            public int Field1 { get; set; }
            public string Field2 { get; set; }
            public DateTime Field3 { get; set; }
        }

        [Test]
        public void TestCreateInlineUpdateWithClassMapping()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<TestCreateInlineUpdateWithClassMappingClass>();
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });

            // Act
            var actual = statementBuilder.CreateInlineUpdate(queryBuilder, fields, null);
            var expected = $"" +
                $"UPDATE [ClassName] " +
                $"SET [Field1] = @Field1, " +
                $"[Field2] = @Field2, " +
                $"[Field3] = @Field3 ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [Map("ClassName")]
        private class TestCreateInlineUpdateWithFieldMappingsClass : DataEntity
        {
            public int Field1 { get; set; }
            public string Field2 { get; set; }
            [Map("Field4")]
            public DateTime Field3 { get; set; }
        }

        [Test]
        public void TestCreateInlineUpdateWithFieldMappings()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<TestCreateInlineUpdateWithFieldMappingsClass>();
            var fields = Field.From(new[] { "Field1", "Field2", "Field4" });

            // Act
            var actual = statementBuilder.CreateInlineUpdate(queryBuilder, fields, null);
            var expected = $"" +
                $"UPDATE [ClassName] " +
                $"SET [Field1] = @Field1, " +
                $"[Field2] = @Field2, " +
                $"[Field4] = @Field4 ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        private class TestCreateInlineUpdateWithExpressionsClass : DataEntity
        {
            public int Field1 { get; set; }
            public string Field2 { get; set; }
            public DateTime Field3 { get; set; }
        }

        [Test]
        public void TestCreateInlineUpdateWithExpressions()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<TestCreateInlineUpdateWithExpressionsClass>();
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });
            var expression = new { Field1 = 1 };

            // Act
            var queryGroup = QueryGroup.Parse(expression);
            var actual = statementBuilder.CreateInlineUpdate(queryBuilder, fields, queryGroup);
            var expected = $"" +
                $"UPDATE [TestCreateInlineUpdateWithExpressionsClass] " +
                $"SET [Field1] = @Field1, " +
                $"[Field2] = @Field2, " +
                $"[Field3] = @Field3 " +
                $"WHERE ([Field1] = @_Field1) ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        private class TestCreateInlineUpdateWithFieldMappingsAndExpressionsClass : DataEntity
        {
            public int Field1 { get; set; }
            public string Field2 { get; set; }
            [Map("Field4")]
            public DateTime Field3 { get; set; }
        }

        [Test]
        public void TestCreateInlineUpdateWithFieldMappingsAndExpressions()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<TestCreateInlineUpdateWithFieldMappingsAndExpressionsClass>();
            var fields = Field.From(new[] { "Field1", "Field2", "Field4" });
            var expression = new { Field1 = 1 };

            // Act
            var queryGroup = QueryGroup.Parse(expression);
            var actual = statementBuilder.CreateInlineUpdate(queryBuilder, fields, queryGroup);
            var expected = $"" +
                $"UPDATE [TestCreateInlineUpdateWithFieldMappingsAndExpressionsClass] " +
                $"SET [Field1] = @Field1, " +
                $"[Field2] = @Field2, " +
                $"[Field4] = @Field4 " +
                $"WHERE ([Field1] = @_Field1) ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        private class TestCreateInlineUpdateOverrideIgnoreForInlineUpdateClass : DataEntity
        {
            public int Field1 { get; set; }
            public string Field2 { get; set; }
            [Attributes.Ignore(Command.InlineUpdate)]
            public DateTime Field3 { get; set; }
        }

        [Test]
        public void TestCreateInlineUpdateOverrideIgnoreForInlineUpdate()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<TestCreateInlineUpdateOverrideIgnoreForInlineUpdateClass>();
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });
            var expression = new { Field1 = 1 };

            // Act
            var queryGroup = QueryGroup.Parse(expression);
            var actual = statementBuilder.CreateInlineUpdate(queryBuilder, fields, queryGroup, true);
            var expected = $"" +
                $"UPDATE [TestCreateInlineUpdateOverrideIgnoreForInlineUpdateClass] " +
                $"SET [Field1] = @Field1, " +
                $"[Field2] = @Field2, " +
                $"[Field3] = @Field3 " +
                $"WHERE ([Field1] = @_Field1) ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        private class TestCreateInlineUpdateOverrideIgnoreForUpdateClass : DataEntity
        {
            public int Field1 { get; set; }
            public string Field2 { get; set; }
            [Attributes.Ignore(Command.Update)]
            public DateTime Field3 { get; set; }
        }

        [Test]
        public void TestCreateInlineUpdateOverrideIgnoreForUpdate()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<TestCreateInlineUpdateOverrideIgnoreForUpdateClass>();
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });
            var expression = new { Field1 = 1 };

            // Act
            var queryGroup = QueryGroup.Parse(expression);
            var actual = statementBuilder.CreateInlineUpdate(queryBuilder, fields, queryGroup, true);
            var expected = $"" +
                $"UPDATE [TestCreateInlineUpdateOverrideIgnoreForUpdateClass] " +
                $"SET [Field1] = @Field1, " +
                $"[Field2] = @Field2, " +
                $"[Field3] = @Field3 " +
                $"WHERE ([Field1] = @_Field1) ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        private class ThrowExceptionAtInlineUpdateIfFieldsAreNullClass : DataEntity
        {
        }

        [Test]
        public void ThrowExceptionAtInlineUpdateIfFieldsAreNull()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<ThrowExceptionAtInlineUpdateIfFieldsAreNullClass>();

            // Act/Assert
            Assert.Throws<NullReferenceException>(() => statementBuilder.CreateInlineUpdate(queryBuilder, null, null));
        }

        private class ThrowExceptionAtInlineUpdateIfFieldIsMissingAtDataEntityClass : DataEntity
        {
            public int Field1 { get; set; }
            public string Field2 { get; set; }
            public DateTime Field3 { get; set; }
        }

        [Test]
        public void ThrowExceptionAtInlineUpdateIfFieldIsMissingAtDataEntity()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<ThrowExceptionAtInlineUpdateIfFieldIsMissingAtDataEntityClass>();
            var fields = Field.From(new[] { "Field1", "Field2", "Field4" });

            // Act/Assert
            Assert.Throws<InvalidOperationException>(() => statementBuilder.CreateInlineUpdate(queryBuilder, fields, null));
        }

        private class ThrowExceptionAtInlineUpdateIfFieldIsIgnoreInlineUpdateAtDataEntityClass : DataEntity
        {
            public int Field1 { get; set; }
            public string Field2 { get; set; }
            [Attributes.Ignore(Command.InlineUpdate)]
            public DateTime Field3 { get; set; }
        }

        [Test]
        public void ThrowExceptionAtInlineUpdateIfFieldIsIgnoreInlineUpdateAtDataEntity()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<ThrowExceptionAtInlineUpdateIfFieldIsIgnoreInlineUpdateAtDataEntityClass>();
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });

            // Act/Assert
            Assert.Throws<InvalidOperationException>(() => statementBuilder.CreateInlineUpdate(queryBuilder, fields, null));
        }

        private class ThrowExceptionAtInlineUpdateIfFieldIsIgnoreUpdateAtDataEntityClass : DataEntity
        {
            public int Field1 { get; set; }
            public string Field2 { get; set; }
            [Attributes.Ignore(Command.Update)]
            public DateTime Field3 { get; set; }
        }

        [Test]
        public void ThrowExceptionAtInlineUpdateIfFieldIsIgnoreUpdateAtDataEntity()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<ThrowExceptionAtInlineUpdateIfFieldIsIgnoreUpdateAtDataEntityClass>();
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });

            // Act/Assert
            Assert.Throws<InvalidOperationException>(() => statementBuilder.CreateInlineUpdate(queryBuilder, fields, null));
        }

        private class ThrowExceptionAtInlineUpdateIfFieldMappingIsDifferentAtDataEntityClass : DataEntity
        {
            public int Field1 { get; set; }
            public string Field2 { get; set; }
            [Map("Field4")]
            public DateTime Field3 { get; set; }
        }

        [Test]
        public void ThrowExceptionAtInlineUpdateIfFieldMappingIsDifferentAtDataEntity()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<ThrowExceptionAtInlineUpdateIfFieldMappingIsDifferentAtDataEntityClass>();
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });

            // Act/Assert
            Assert.Throws<InvalidOperationException>(() => statementBuilder.CreateInlineUpdate(queryBuilder, fields, null));
        }
    }
}
