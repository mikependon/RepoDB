using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Attributes;
using RepoDb.Enumerations;
using System;

namespace RepoDb.UnitTests.StatementBuilders
{
    [TestClass]
    public class CreateInlineUpdate
    {
        private class TestWithoutMappingsClass
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
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });

            // Act
            var actual = statementBuilder.CreateInlineUpdate(queryBuilder, fields, null);
            var expected = $"" +
                $"UPDATE [TestWithoutMappingsClass] " +
                $"SET [Field1] = @Field1, " +
                $"[Field2] = @Field2, " +
                $"[Field3] = @Field3 ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [Map("ClassName")]
        private class TestWithClassMappingClass
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
        private class TestWithFieldMappingsClass
        {
            public int Field1 { get; set; }
            public string Field2 { get; set; }
            [Map("Field4")]
            public DateTime Field3 { get; set; }
        }

        [TestMethod]
        public void TestWithFieldMappings()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<TestWithFieldMappingsClass>();
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

        private class TestWithExpressionsClass
        {
            public int Field1 { get; set; }
            public string Field2 { get; set; }
            public DateTime Field3 { get; set; }
        }

        [TestMethod]
        public void TestWithExpressions()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<TestWithExpressionsClass>();
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });
            var expression = new { Field1 = 1 };
            var queryGroup = QueryGroup.Parse(expression);

            // Act
            var actual = statementBuilder.CreateInlineUpdate(queryBuilder, fields, queryGroup);
            var expected = $"" +
                $"UPDATE [TestWithExpressionsClass] " +
                $"SET [Field1] = @Field1, " +
                $"[Field2] = @Field2, " +
                $"[Field3] = @Field3 " +
                $"WHERE ([Field1] = @_Field1) ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        private class TestWithFieldMappingsAndExpressionsClass
        {
            public int Field1 { get; set; }
            public string Field2 { get; set; }
            [Map("Field4")]
            public DateTime Field3 { get; set; }
        }

        [TestMethod]
        public void TestWithFieldMappingsAndExpressions()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<TestWithFieldMappingsAndExpressionsClass>();
            var fields = Field.From(new[] { "Field1", "Field2", "Field4" });
            var expression = new { Field1 = 1 };
            var queryGroup = QueryGroup.Parse(expression);

            // Act
            var actual = statementBuilder.CreateInlineUpdate(queryBuilder, fields, queryGroup);
            var expected = $"" +
                $"UPDATE [TestWithFieldMappingsAndExpressionsClass] " +
                $"SET [Field1] = @Field1, " +
                $"[Field2] = @Field2, " +
                $"[Field4] = @Field4 " +
                $"WHERE ([Field1] = @_Field1) ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        private class TestOverrideIgnoreForInlineUpdateClass
        {
            public int Field1 { get; set; }
            public string Field2 { get; set; }
            [Attributes.Ignore(Command.InlineUpdate)]
            public DateTime Field3 { get; set; }
        }

        [TestMethod]
        public void TestOverrideIgnoreForInlineUpdate()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<TestOverrideIgnoreForInlineUpdateClass>();
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });
            var expression = new { Field1 = 1 };
            var queryGroup = QueryGroup.Parse(expression);

            // Act
            var actual = statementBuilder.CreateInlineUpdate(queryBuilder, fields, queryGroup, true);
            var expected = $"" +
                $"UPDATE [TestOverrideIgnoreForInlineUpdateClass] " +
                $"SET [Field1] = @Field1, " +
                $"[Field2] = @Field2, " +
                $"[Field3] = @Field3 " +
                $"WHERE ([Field1] = @_Field1) ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        private class TestOverrideIgnoreForUpdateClass
        {
            public int Field1 { get; set; }
            public string Field2 { get; set; }
            [Attributes.Ignore(Command.Update)]
            public DateTime Field3 { get; set; }
        }

        [TestMethod]
        public void TestOverrideIgnoreForUpdate()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<TestOverrideIgnoreForUpdateClass>();
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });
            var expression = new { Field1 = 1 };
            var queryGroup = QueryGroup.Parse(expression);

            // Act
            var actual = statementBuilder.CreateInlineUpdate(queryBuilder, fields, queryGroup, true);
            var expected = $"" +
                $"UPDATE [TestOverrideIgnoreForUpdateClass] " +
                $"SET [Field1] = @Field1, " +
                $"[Field2] = @Field2, " +
                $"[Field3] = @Field3 " +
                $"WHERE ([Field1] = @_Field1) ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        private class ThrowExceptionIfTheTargetFieldsAreNullClass
        {
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionIfTheTargetFieldsAreNull()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<ThrowExceptionIfTheTargetFieldsAreNullClass>();

            // Act/Assert
            statementBuilder.CreateInlineUpdate(queryBuilder, null, null);
        }

        private class ThrowExceptionIfTheTargetFieldIsMissingAtDataEntityClass
        {
            public int Field1 { get; set; }
            public string Field2 { get; set; }
            public DateTime Field3 { get; set; }
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void ThrowExceptionIfTheTargetFieldIsMissingAtDataEntity()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<ThrowExceptionIfTheTargetFieldIsMissingAtDataEntityClass>();
            var fields = Field.From(new[] { "Field1", "Field2", "Field4" });

            // Act/Assert
            statementBuilder.CreateInlineUpdate(queryBuilder, fields, null);
        }

        private class ThrowExceptionIfTheTargetFieldIsIgnoreInlineUpdateAtDataEntityClass
        {
            public int Field1 { get; set; }
            public string Field2 { get; set; }
            [Attributes.Ignore(Command.InlineUpdate)]
            public DateTime Field3 { get; set; }
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void ThrowExceptionIfTheTargetFieldIsIgnoreInlineUpdateAtDataEntity()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<ThrowExceptionIfTheTargetFieldIsIgnoreInlineUpdateAtDataEntityClass>();
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });

            // Act/Assert
            statementBuilder.CreateInlineUpdate(queryBuilder, fields, null);
        }

        private class ThrowExceptionIfTheTargetFieldIsIgnoreUpdateAtDataEntityClass
        {
            public int Field1 { get; set; }
            public string Field2 { get; set; }
            [Attributes.Ignore(Command.Update)]
            public DateTime Field3 { get; set; }
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void ThrowExceptionIfTheTargetFieldIsIgnoreUpdateAtDataEntity()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<ThrowExceptionIfTheTargetFieldIsIgnoreUpdateAtDataEntityClass>();
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });

            // Act/Assert
            statementBuilder.CreateInlineUpdate(queryBuilder, fields, null);
        }

        private class ThrowExceptionIfTheTargetFieldMappingIsDifferentAtDataEntityClass
        {
            public int Field1 { get; set; }
            public string Field2 { get; set; }
            [Map("Field4")]
            public DateTime Field3 { get; set; }
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void ThrowExceptionIfTheTargetFieldMappingIsDifferentAtDataEntity()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<ThrowExceptionIfTheTargetFieldMappingIsDifferentAtDataEntityClass>();
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });

            // Act/Assert
            statementBuilder.CreateInlineUpdate(queryBuilder, fields, null);
        }

        private class ThrowExceptionIfTheTargetFieldIsThePrimaryKeyFieldClass
        {
            [Primary]
            public int Field1 { get; set; }
            public string Field2 { get; set; }
            public DateTime Field3 { get; set; }
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void ThrowExceptionIfTheTargetFieldIsThePrimaryKeyField()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<ThrowExceptionIfTheTargetFieldIsThePrimaryKeyFieldClass>();
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });

            // Act/Assert
            statementBuilder.CreateInlineUpdate(queryBuilder, fields, null);
        }

        private class ThrowExceptionIfTheTargetFieldIsTheClassIdClass
        {
            public int ThrowExceptionIfTheTargetFieldIsTheClassIdClassId { get; set; }
            public string Field2 { get; set; }
            public DateTime Field3 { get; set; }
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void ThrowExceptionIfTheTargetFieldIsTheClassId()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<ThrowExceptionIfTheTargetFieldIsTheClassIdClass>();
            var fields = Field.From(new[] { "ThrowExceptionIfTheTargetFieldIsTheClassIdClassId", "Field2", "Field3" });

            // Act/Assert
            statementBuilder.CreateInlineUpdate(queryBuilder, fields, null);
        }

        [Map("ClassName")]
        private class ThrowExceptionIfTheTargetFieldIsAClassMappingIdClass
        {
            public int ClassNameId { get; set; }
            public string Field2 { get; set; }
            public DateTime Field3 { get; set; }
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void ThrowExceptionIfTheTargetFieldIsAClassMappingId()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<ThrowExceptionIfTheTargetFieldIsAClassMappingIdClass>();
            var fields = Field.From(new[] { "ClassNameId", "Field2", "Field3" });

            // Act/Assert
            statementBuilder.CreateInlineUpdate(queryBuilder, fields, null);
        }

        private class ThrowExceptionIfTheTargetFieldIsTheIdClass
        {
            public int Id { get; set; }
            public string Field2 { get; set; }
            public DateTime Field3 { get; set; }
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void ThrowExceptionIfTheTargetFieldIsTheId()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<ThrowExceptionIfTheTargetFieldIsTheIdClass>();
            var fields = Field.From(new[] { "Id", "Field2", "Field3" });

            // Act/Assert
            statementBuilder.CreateInlineUpdate(queryBuilder, fields, null);
        }

        private class ThrowExceptionIfTheTargetFieldIsAnIdentityClass
        {
            [Primary, Identity]
            public int Field1 { get; set; }
            public string Field2 { get; set; }
            public DateTime Field3 { get; set; }
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void ThrowExceptionIfTheTargetFieldIsAnIdentity()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<ThrowExceptionIfTheTargetFieldIsAnIdentityClass>();
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });

            // Act/Assert
            statementBuilder.CreateInlineUpdate(queryBuilder, fields, null);
        }

        private class ThrowExceptionIfTheIdentityFieldIsNotThePrimaryFieldClass
        {
            [Identity]
            public int Field1 { get; set; }
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void ThrowExceptionIfTheIdentityFieldIsNotThePrimaryField()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<ThrowExceptionIfTheIdentityFieldIsNotThePrimaryFieldClass>();
            var fields = Field.From(new[] { "Field1" });

            // Act/Assert
            statementBuilder.CreateInlineUpdate(queryBuilder, fields, null);
        }
    }
}
