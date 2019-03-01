using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Attributes;
using System;

namespace RepoDb.UnitTests.StatementBuilders
{
    [TestClass]
    public class SqlDbProviderCreateInlineUpdateTest
    {
        private class TestSqlDbProviderCreateInlineUpdateWithoutMappingsClass
        {
            public int Field1 { get; set; }
            public string Field2 { get; set; }
            public DateTime Field3 { get; set; }
        }

        [TestMethod]
        public void TestSqlDbProviderCreateInlineUpdateWithoutMappings()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder<TestSqlDbProviderCreateInlineUpdateWithoutMappingsClass>();
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });

            // Act
            var actual = statementBuilder.CreateInlineUpdate(queryBuilder, fields, null);
            var expected = $"" +
                $"UPDATE [TestSqlDbProviderCreateInlineUpdateWithoutMappingsClass] " +
                $"SET [Field1] = @Field1, " +
                $"[Field2] = @Field2, " +
                $"[Field3] = @Field3 ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [Map("ClassName")]
        private class TestSqlDbProviderCreateInlineUpdateWithClassMappingClass
        {
            public int Field1 { get; set; }
            public string Field2 { get; set; }
            public DateTime Field3 { get; set; }
        }

        [TestMethod]
        public void TestSqlDbProviderCreateInlineUpdateWithClassMapping()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder<TestSqlDbProviderCreateInlineUpdateWithClassMappingClass>();
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
        private class TestSqlDbProviderCreateInlineUpdateWithFieldMappingsClass
        {
            public int Field1 { get; set; }
            public string Field2 { get; set; }
            [Map("Field4")]
            public DateTime Field3 { get; set; }
        }

        [TestMethod]
        public void TestSqlDbProviderCreateInlineUpdateWithFieldMappings()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder<TestSqlDbProviderCreateInlineUpdateWithFieldMappingsClass>();
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

        private class TestSqlDbProviderCreateInlineUpdateWithExpressionsClass
        {
            public int Field1 { get; set; }
            public string Field2 { get; set; }
            public DateTime Field3 { get; set; }
        }

        [TestMethod]
        public void TestSqlDbProviderCreateInlineUpdateWithExpressions()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder<TestSqlDbProviderCreateInlineUpdateWithExpressionsClass>();
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });
            var expression = new { Field1 = 1 };
            var queryGroup = QueryGroup.Parse(expression);

            // Act
            var actual = statementBuilder.CreateInlineUpdate(queryBuilder, fields, queryGroup);
            var expected = $"" +
                $"UPDATE [TestSqlDbProviderCreateInlineUpdateWithExpressionsClass] " +
                $"SET [Field1] = @Field1, " +
                $"[Field2] = @Field2, " +
                $"[Field3] = @Field3 " +
                $"WHERE ([Field1] = @_Field1) ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        private class TestSqlDbProviderCreateInlineUpdateWithFieldMappingsAndExpressionsClass
        {
            public int Field1 { get; set; }
            public string Field2 { get; set; }
            [Map("Field4")]
            public DateTime Field3 { get; set; }
        }

        [TestMethod]
        public void TestSqlDbProviderCreateInlineUpdateWithFieldMappingsAndExpressions()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder<TestSqlDbProviderCreateInlineUpdateWithFieldMappingsAndExpressionsClass>();
            var fields = Field.From(new[] { "Field1", "Field2", "Field4" });
            var expression = new { Field1 = 1 };
            var queryGroup = QueryGroup.Parse(expression);

            // Act
            var actual = statementBuilder.CreateInlineUpdate(queryBuilder, fields, queryGroup);
            var expected = $"" +
                $"UPDATE [TestSqlDbProviderCreateInlineUpdateWithFieldMappingsAndExpressionsClass] " +
                $"SET [Field1] = @Field1, " +
                $"[Field2] = @Field2, " +
                $"[Field4] = @Field4 " +
                $"WHERE ([Field1] = @_Field1) ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }
        
        private class ThrowExceptionOnSqlDbProviderCreateInlineUpdateIfTheTargetFieldsAreNullClass
        {
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnSqlDbProviderCreateInlineUpdateIfTheTargetFieldsAreNull()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder<ThrowExceptionOnSqlDbProviderCreateInlineUpdateIfTheTargetFieldsAreNullClass>();

            // Act/Assert
            statementBuilder.CreateInlineUpdate(queryBuilder, null, null);
        }

        private class ThrowExceptionOnSqlDbProviderCreateInlineUpdateIfTheTargetFieldIsMissingAtDataEntityClass
        {
            public int Field1 { get; set; }
            public string Field2 { get; set; }
            public DateTime Field3 { get; set; }
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void ThrowExceptionOnSqlDbProviderCreateInlineUpdateIfTheTargetFieldIsMissingAtDataEntity()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder<ThrowExceptionOnSqlDbProviderCreateInlineUpdateIfTheTargetFieldIsMissingAtDataEntityClass>();
            var fields = Field.From(new[] { "Field1", "Field2", "Field4" });

            // Act/Assert
            statementBuilder.CreateInlineUpdate(queryBuilder, fields, null);
        }

        private class ThrowExceptionOnSqlDbProviderCreateInlineUpdateIfTheTargetFieldMappingIsDifferentAtDataEntityClass
        {
            public int Field1 { get; set; }
            public string Field2 { get; set; }
            [Map("Field4")]
            public DateTime Field3 { get; set; }
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void ThrowExceptionOnSqlDbProviderCreateInlineUpdateIfTheTargetFieldMappingIsDifferentAtDataEntity()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder<ThrowExceptionOnSqlDbProviderCreateInlineUpdateIfTheTargetFieldMappingIsDifferentAtDataEntityClass>();
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });

            // Act/Assert
            statementBuilder.CreateInlineUpdate(queryBuilder, fields, null);
        }

        private class ThrowExceptionOnSqlDbProviderCreateInlineUpdateIfTheTargetFieldIsThePrimaryKeyFieldClass
        {
            [Primary]
            public int Field1 { get; set; }
            public string Field2 { get; set; }
            public DateTime Field3 { get; set; }
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void ThrowExceptionOnSqlDbProviderCreateInlineUpdateIfTheTargetFieldIsThePrimaryKeyField()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder<ThrowExceptionOnSqlDbProviderCreateInlineUpdateIfTheTargetFieldIsThePrimaryKeyFieldClass>();
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });

            // Act/Assert
            statementBuilder.CreateInlineUpdate(queryBuilder, fields, null);
        }

        private class ThrowExceptionOnSqlDbProviderCreateInlineUpdateIfTheTargetFieldIsTheClassIdClass
        {
            public int ThrowExceptionOnSqlDbProviderCreateInlineUpdateIfTheTargetFieldIsTheClassIdClassId { get; set; }
            public string Field2 { get; set; }
            public DateTime Field3 { get; set; }
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void ThrowExceptionOnSqlDbProviderCreateInlineUpdateIfTheTargetFieldIsTheClassId()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder<ThrowExceptionOnSqlDbProviderCreateInlineUpdateIfTheTargetFieldIsTheClassIdClass>();
            var fields = Field.From(new[] { "ThrowExceptionOnSqlDbProviderCreateInlineUpdateIfTheTargetFieldIsTheClassIdClassId", "Field2", "Field3" });

            // Act/Assert
            statementBuilder.CreateInlineUpdate(queryBuilder, fields, null);
        }

        [Map("ClassName")]
        private class ThrowExceptionOnSqlDbProviderCreateInlineUpdateIfTheTargetFieldIsAClassMappingIdClass
        {
            public int ClassNameId { get; set; }
            public string Field2 { get; set; }
            public DateTime Field3 { get; set; }
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void ThrowExceptionOnSqlDbProviderCreateInlineUpdateIfTheTargetFieldIsAClassMappingId()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder<ThrowExceptionOnSqlDbProviderCreateInlineUpdateIfTheTargetFieldIsAClassMappingIdClass>();
            var fields = Field.From(new[] { "ClassNameId", "Field2", "Field3" });

            // Act/Assert
            statementBuilder.CreateInlineUpdate(queryBuilder, fields, null);
        }

        private class ThrowExceptionOnSqlDbProviderCreateInlineUpdateIfTheTargetFieldIsTheIdClass
        {
            public int Id { get; set; }
            public string Field2 { get; set; }
            public DateTime Field3 { get; set; }
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void ThrowExceptionIfTheTargetFieldIsTheId()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder<ThrowExceptionOnSqlDbProviderCreateInlineUpdateIfTheTargetFieldIsTheIdClass>();
            var fields = Field.From(new[] { "Id", "Field2", "Field3" });

            // Act/Assert
            statementBuilder.CreateInlineUpdate(queryBuilder, fields, null);
        }

        private class ThrowExceptionOnSqlDbProviderCreateInlineUpdateIfTheTargetFieldIsAnIdentityClass
        {
            [Primary, Identity]
            public int Field1 { get; set; }
            public string Field2 { get; set; }
            public DateTime Field3 { get; set; }
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void ThrowExceptionOnSqlDbProviderCreateInlineUpdateIfTheTargetFieldIsAnIdentity()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder<ThrowExceptionOnSqlDbProviderCreateInlineUpdateIfTheTargetFieldIsAnIdentityClass>();
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });

            // Act/Assert
            statementBuilder.CreateInlineUpdate(queryBuilder, fields, null);
        }

        private class ThrowExceptionOnSqlDbProviderCreateInlineUpdateIfTheIdentityFieldIsNotThePrimaryFieldClass
        {
            [Identity]
            public int Field1 { get; set; }
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void ThrowExceptionOnSqlDbProviderCreateInlineUpdateIfTheIdentityFieldIsNotThePrimaryField()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder<ThrowExceptionOnSqlDbProviderCreateInlineUpdateIfTheIdentityFieldIsNotThePrimaryFieldClass>();
            var fields = Field.From(new[] { "Field1" });

            // Act/Assert
            statementBuilder.CreateInlineUpdate(queryBuilder, fields, null);
        }
    }
}
