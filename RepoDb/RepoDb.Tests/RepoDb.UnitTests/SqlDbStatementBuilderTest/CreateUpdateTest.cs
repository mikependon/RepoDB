using NUnit.Framework;
using RepoDb.Attributes;
using System;

namespace RepoDb.UnitTests.SqlDbStatementBuilderTest
{
    [TestFixture]
    public class CreateUpdateTest
    {
        private class TestWithoutMappingsClass : DataEntity
        {
            public int Field1 { get; set; }
            public string Field2 { get; set; }
            public DateTime Field3 { get; set; }
        }

        [Test]
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

        [Test]
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

        [Test]
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

        [Test]
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

        [Test]
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

        [Test]
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

        [Test]
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

        [Test]
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

        private class ThrowExceptionIfTheIdentityFieldIsNotThePrimaryFieldClass : DataEntity
        {
            [Identity]
            public int Field1 { get; set; }
        }

        [Test]
        public void ThrowExceptionIfTheIdentityFieldIsNotThePrimaryField()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<ThrowExceptionIfTheIdentityFieldIsNotThePrimaryFieldClass>();
            var queryGroup = (QueryGroup)null;

            // Act/Assert
            Assert.Throws<InvalidOperationException>(() => statementBuilder.CreateUpdate(queryBuilder, queryGroup));
        }
    }
}
