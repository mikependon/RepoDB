using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Attributes;
using RepoDb.Enumerations;
using System;

namespace RepoDb.UnitTests.SqlDbStatementBuilderTest
{
    [TestClass]
    public class CreateUpdateTest
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
            var actual = statementBuilder.CreateUpdate(queryBuilder, queryGroup);
            var expected = $"" +
                $"UPDATE [TestWithoutMappingsClass] " +
                $"SET [Field1] = @Field1, [Field2] = @Field2, [Field3] = @Field3 ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [Map("ClassName")]
        private class TestWithClassMappingsClass : DataEntity
        {
            public int Field1 { get; set; }
            public string Field2 { get; set; }
            public DateTime Field3 { get; set; }
        }

        [TestMethod]
        public void TestWithClassMappings()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<TestWithClassMappingsClass>();
            var queryGroup = (QueryGroup)null;

            // Act
            var actual = statementBuilder.CreateUpdate(queryBuilder, queryGroup);
            var expected = $"" +
                $"UPDATE [ClassName] " +
                $"SET [Field1] = @Field1, [Field2] = @Field2, [Field3] = @Field3 ;";

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
            var actual = statementBuilder.CreateUpdate(queryBuilder, queryGroup);
            var expected = $"" +
                $"UPDATE [TestWithFieldMappingClass] " +
                $"SET [Field1] = @Field1, [Field2] = @Field2, [Field4] = @Field4 ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        private class TestWithPrimaryKeyFieldClass : DataEntity
        {
            [Primary]
            public int Field1 { get; set; }
            public string Field2 { get; set; }
            public DateTime Field3 { get; set; }
        }

        [TestMethod]
        public void TestWithPrimaryKeyField()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<TestWithPrimaryKeyFieldClass>();
            var queryGroup = (QueryGroup)null;

            // Act
            var actual = statementBuilder.CreateUpdate(queryBuilder, queryGroup);
            var expected = $"" +
                $"UPDATE [TestWithPrimaryKeyFieldClass] " +
                $"SET [Field2] = @Field2, [Field3] = @Field3 ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        private class TestWithIdClass : DataEntity
        {
            public int Id { get; set; }
            public string Field2 { get; set; }
            public DateTime Field3 { get; set; }
        }

        [TestMethod]
        public void TestWithId()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<TestWithIdClass>();
            var queryGroup = (QueryGroup)null;

            // Act
            var actual = statementBuilder.CreateUpdate(queryBuilder, queryGroup);
            var expected = $"" +
                $"UPDATE [TestWithIdClass] " +
                $"SET [Field2] = @Field2, [Field3] = @Field3 ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        private class TestWithClassIdClass : DataEntity
        {
            public int TestWithClassIdClassId { get; set; }
            public string Field2 { get; set; }
            public DateTime Field3 { get; set; }
        }

        [TestMethod]
        public void TestWithClassId()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<TestWithClassIdClass>();
            var queryGroup = (QueryGroup)null;

            // Act
            var actual = statementBuilder.CreateUpdate(queryBuilder, queryGroup);
            var expected = $"" +
                $"UPDATE [TestWithClassIdClass] " +
                $"SET [Field2] = @Field2, [Field3] = @Field3 ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [Map("ClassName")]
        private class TestWithClassMappingIdClass : DataEntity
        {
            public int ClassNameId { get; set; }
            public string Field2 { get; set; }
            public DateTime Field3 { get; set; }
        }

        [TestMethod]
        public void TestWithClassMappingId()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<TestWithClassMappingIdClass>();
            var queryGroup = (QueryGroup)null;

            // Act
            var actual = statementBuilder.CreateUpdate(queryBuilder, queryGroup);
            var expected = $"" +
                $"UPDATE [ClassName] " +
                $"SET [Field2] = @Field2, [Field3] = @Field3 ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        private class TestWithIdentityFieldClass : DataEntity
        {
            [Primary, Identity]
            public int Field1 { get; set; }
            public string Field2 { get; set; }
            public DateTime Field3 { get; set; }
        }

        [TestMethod]
        public void TestWithIdentityField()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<TestWithIdentityFieldClass>();
            var queryGroup = (QueryGroup)null;

            // Act
            var actual = statementBuilder.CreateUpdate(queryBuilder, queryGroup);
            var expected = $"" +
                $"UPDATE [TestWithIdentityFieldClass] " +
                $"SET [Field2] = @Field2, [Field3] = @Field3 ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        private class TestWithExpressionClass : DataEntity
        {
            [Primary, Identity]
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
            var queryGroup = QueryGroup.Parse(new { Field1 = 1, Field4 = new { Operation = Operation.GreaterThan, Value = 2 } });

            // Act
            var actual = statementBuilder.CreateUpdate(queryBuilder, queryGroup);
            var expected = $"" +
                $"UPDATE [TestWithExpressionClass] " +
                $"SET [Field2] = @Field2, [Field3] = @Field3 " +
                $"WHERE ([Field1] = @_Field1 AND [Field4] > @_Field4) ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        private class ThrowExceptionIfTheIdentityFieldIsNotThePrimaryFieldClass : DataEntity
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
            var queryGroup = (QueryGroup)null;

            // Act/Assert
            statementBuilder.CreateUpdate(queryBuilder, queryGroup);
        }
    }
}
