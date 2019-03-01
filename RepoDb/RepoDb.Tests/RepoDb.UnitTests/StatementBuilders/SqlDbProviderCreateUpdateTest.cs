using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Attributes;
using System;

namespace RepoDb.UnitTests.StatementBuilders
{
    [TestClass]
    public class SqlDbProviderCreateUpdateTest
    {
        private class TestSqlDbProviderCreateUpdateWithoutMappingsClass
        {
            public int Field1 { get; set; }
            public string Field2 { get; set; }
            public DateTime Field3 { get; set; }
        }

        [TestMethod]
        public void TestSqlDbProviderCreateUpdateWithoutMappings()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder<TestSqlDbProviderCreateUpdateWithoutMappingsClass>();
            var queryGroup = (QueryGroup)null;

            // Act
            var actual = statementBuilder.CreateUpdate(queryBuilder, queryGroup);
            var expected = $"" +
                $"UPDATE [TestSqlDbProviderCreateUpdateWithoutMappingsClass] " +
                $"SET [Field1] = @Field1, [Field2] = @Field2, [Field3] = @Field3 ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [Map("ClassName")]
        private class TestSqlDbProviderCreateUpdateWithClassMappingsClass
        {
            public int Field1 { get; set; }
            public string Field2 { get; set; }
            public DateTime Field3 { get; set; }
        }

        [TestMethod]
        public void TestSqlDbProviderCreateUpdateWithClassMappings()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder<TestSqlDbProviderCreateUpdateWithClassMappingsClass>();
            var queryGroup = (QueryGroup)null;

            // Act
            var actual = statementBuilder.CreateUpdate(queryBuilder, queryGroup);
            var expected = $"" +
                $"UPDATE [ClassName] " +
                $"SET [Field1] = @Field1, [Field2] = @Field2, [Field3] = @Field3 ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        private class TestSqlDbProviderCreateUpdateWithFieldMappingClass
        {
            public int Field1 { get; set; }
            public string Field2 { get; set; }
            [Map("Field4")]
            public DateTime Field3 { get; set; }
        }

        [TestMethod]
        public void TestSqlDbProviderCreateUpdateWithFieldMapping()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder<TestSqlDbProviderCreateUpdateWithFieldMappingClass>();
            var queryGroup = (QueryGroup)null;

            // Act
            var actual = statementBuilder.CreateUpdate(queryBuilder, queryGroup);
            var expected = $"" +
                $"UPDATE [TestSqlDbProviderCreateUpdateWithFieldMappingClass] " +
                $"SET [Field1] = @Field1, [Field2] = @Field2, [Field4] = @Field4 ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        private class TestSqlDbProviderCreateUpdateWithPrimaryKeyFieldClass
        {
            [Primary]
            public int Field1 { get; set; }
            public string Field2 { get; set; }
            public DateTime Field3 { get; set; }
        }

        [TestMethod]
        public void TestSqlDbProviderCreateUpdateWithPrimaryKeyField()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder<TestSqlDbProviderCreateUpdateWithPrimaryKeyFieldClass>();
            var queryGroup = (QueryGroup)null;

            // Act
            var actual = statementBuilder.CreateUpdate(queryBuilder, queryGroup);
            var expected = $"" +
                $"UPDATE [TestSqlDbProviderCreateUpdateWithPrimaryKeyFieldClass] " +
                $"SET [Field2] = @Field2, [Field3] = @Field3 ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        private class TestSqlDbProviderCreateUpdateWithIdClass
        {
            public int Id { get; set; }
            public string Field2 { get; set; }
            public DateTime Field3 { get; set; }
        }

        [TestMethod]
        public void TestSqlDbProviderCreateUpdateWithId()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder<TestSqlDbProviderCreateUpdateWithIdClass>();
            var queryGroup = (QueryGroup)null;

            // Act
            var actual = statementBuilder.CreateUpdate(queryBuilder, queryGroup);
            var expected = $"" +
                $"UPDATE [TestSqlDbProviderCreateUpdateWithIdClass] " +
                $"SET [Field2] = @Field2, [Field3] = @Field3 ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        private class TestSqlDbProviderCreateUpdateWithClassIdClass
        {
            public int TestSqlDbProviderCreateUpdateWithClassIdClassId { get; set; }
            public string Field2 { get; set; }
            public DateTime Field3 { get; set; }
        }

        [TestMethod]
        public void TestSqlDbProviderCreateUpdateWithClassId()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder<TestSqlDbProviderCreateUpdateWithClassIdClass>();
            var queryGroup = (QueryGroup)null;

            // Act
            var actual = statementBuilder.CreateUpdate(queryBuilder, queryGroup);
            var expected = $"" +
                $"UPDATE [TestSqlDbProviderCreateUpdateWithClassIdClass] " +
                $"SET [Field2] = @Field2, [Field3] = @Field3 ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [Map("ClassName")]
        private class TestSqlDbProviderCreateUpdateWithClassMappingIdClass
        {
            public int ClassNameId { get; set; }
            public string Field2 { get; set; }
            public DateTime Field3 { get; set; }
        }

        [TestMethod]
        public void TestSqlDbProviderCreateUpdateWithClassMappingId()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder<TestSqlDbProviderCreateUpdateWithClassMappingIdClass>();
            var queryGroup = (QueryGroup)null;

            // Act
            var actual = statementBuilder.CreateUpdate(queryBuilder, queryGroup);
            var expected = $"" +
                $"UPDATE [ClassName] " +
                $"SET [Field2] = @Field2, [Field3] = @Field3 ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        private class TestSqlDbProviderCreateUpdateWithIdentityFieldClass
        {
            [Primary, Identity]
            public int Field1 { get; set; }
            public string Field2 { get; set; }
            public DateTime Field3 { get; set; }
        }

        [TestMethod]
        public void TestSqlDbProviderCreateUpdateWithIdentityField()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder<TestSqlDbProviderCreateUpdateWithIdentityFieldClass>();
            var queryGroup = (QueryGroup)null;

            // Act
            var actual = statementBuilder.CreateUpdate(queryBuilder, queryGroup);
            var expected = $"" +
                $"UPDATE [TestSqlDbProviderCreateUpdateWithIdentityFieldClass] " +
                $"SET [Field2] = @Field2, [Field3] = @Field3 ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        private class ThrowExceptionOnSqlDbProviderCreateUpdateIfTheIdentityFieldIsNotThePrimaryFieldClass
        {
            [Identity]
            public int Field1 { get; set; }
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void ThrowExceptionOnSqlDbProviderCreateUpdateIfTheIdentityFieldIsNotThePrimaryField()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder<ThrowExceptionOnSqlDbProviderCreateUpdateIfTheIdentityFieldIsNotThePrimaryFieldClass>();
            var queryGroup = (QueryGroup)null;

            // Act/Assert
            statementBuilder.CreateUpdate(queryBuilder, queryGroup);
        }
    }
}
