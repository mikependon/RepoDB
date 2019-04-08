using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Attributes;
using System;

namespace RepoDb.UnitTests.StatementBuilders
{
    [TestClass]
    public class SqlDbProviderCreateInsertTest
    {
        private class TestSqlDbProviderCreateInsertWithoutMappingsClass
        {
            public int Field1 { get; set; }
            public string Field2 { get; set; }
            public DateTime Field3 { get; set; }
        }

        [TestMethod]
        public void TestSqlDbProviderCreateInsertWithoutMappings()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder();

            // Act
            var actual = statementBuilder.CreateInsert<TestSqlDbProviderCreateInsertWithoutMappingsClass>(queryBuilder);
            var expected = $"" +
                $"INSERT INTO [TestSqlDbProviderCreateInsertWithoutMappingsClass] " +
                $"( [Field1], [Field2], [Field3] ) " +
                $"VALUES " +
                $"( @Field1, @Field2, @Field3 ) ; " +
                $"SELECT NULL AS [Result] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [Map("ClassName")]
        private class TestSqlDbProviderCreateInsertWithClassMappingClass
        {
            public int Field1 { get; set; }
            public string Field2 { get; set; }
            public DateTime Field3 { get; set; }
        }

        [TestMethod]
        public void TestSqlDbProviderCreateInsertWithClassMapping()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder();

            // Act
            var actual = statementBuilder.CreateInsert<TestSqlDbProviderCreateInsertWithClassMappingClass>(queryBuilder);
            var expected = $"" +
                $"INSERT INTO [ClassName] " +
                $"( [Field1], [Field2], [Field3] ) " +
                $"VALUES " +
                $"( @Field1, @Field2, @Field3 ) ; " +
                $"SELECT NULL AS [Result] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        private class TestSqlDbProviderCreateInsertWithFieldMappingsClass
        {
            public int Field1 { get; set; }
            public string Field2 { get; set; }
            [Map("Field4")]
            public DateTime Field3 { get; set; }
        }

        [TestMethod]
        public void TestSqlDbProviderCreateInsertWithFieldMappings()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder();

            // Act
            var actual = statementBuilder.CreateInsert<TestSqlDbProviderCreateInsertWithFieldMappingsClass>(queryBuilder);
            var expected = $"" +
                $"INSERT INTO [TestSqlDbProviderCreateInsertWithFieldMappingsClass] " +
                $"( [Field1], [Field2], [Field4] ) " +
                $"VALUES " +
                $"( @Field1, @Field2, @Field4 ) ; " +
                $"SELECT NULL AS [Result] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        private class TestSqlDbProviderCreateInsertWithIdClass
        {
            public int Id { get; set; }
            public string Field2 { get; set; }
            public DateTime Field3 { get; set; }
        }

        [TestMethod]
        public void TestSqlDbProviderCreateInsertWithId()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder();

            // Act
            var actual = statementBuilder.CreateInsert<TestSqlDbProviderCreateInsertWithIdClass>(queryBuilder);
            var expected = $"" +
                $"INSERT INTO [TestSqlDbProviderCreateInsertWithIdClass] " +
                $"( [Id], [Field2], [Field3] ) " +
                $"VALUES " +
                $"( @Id, @Field2, @Field3 ) ; " +
                $"SELECT @Id AS [Result] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        private class TestSqlDbProviderCreateInsertWithClassIdFieldClass
        {
            public int TestSqlDbProviderCreateInsertWithClassIdFieldClassId { get; set; }
            public string Field2 { get; set; }
            public DateTime Field3 { get; set; }
        }

        [TestMethod]
        public void TestSqlDbProviderCreateInsertWithClassIdField()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder();

            // Act
            var actual = statementBuilder.CreateInsert<TestSqlDbProviderCreateInsertWithClassIdFieldClass>(queryBuilder);
            var expected = $"" +
                $"INSERT INTO [TestSqlDbProviderCreateInsertWithClassIdFieldClass] " +
                $"( [TestSqlDbProviderCreateInsertWithClassIdFieldClassId], [Field2], [Field3] ) " +
                $"VALUES " +
                $"( @TestSqlDbProviderCreateInsertWithClassIdFieldClassId, @Field2, @Field3 ) ; " +
                $"SELECT @TestSqlDbProviderCreateInsertWithClassIdFieldClassId AS [Result] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        private class TestSqlDbProviderCreateInsertWithPrimaryKeyFieldClass
        {
            [Primary]
            public int Field1 { get; set; }
            public string Field2 { get; set; }
            public DateTime Field3 { get; set; }
        }

        [TestMethod]
        public void TestSqlDbProviderCreateInsertWithPrimaryKeyField()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder();

            // Act
            var actual = statementBuilder.CreateInsert<TestSqlDbProviderCreateInsertWithPrimaryKeyFieldClass>(queryBuilder);
            var expected = $"" +
                $"INSERT INTO [TestSqlDbProviderCreateInsertWithPrimaryKeyFieldClass] " +
                $"( [Field1], [Field2], [Field3] ) " +
                $"VALUES " +
                $"( @Field1, @Field2, @Field3 ) ; " +
                $"SELECT @Field1 AS [Result] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [Map("ClassName")]
        private class TestWithSqlDbProviderCreateInsertClassMappingIdClass
        {
            public int ClassNameId { get; set; }
            public string Field2 { get; set; }
            public DateTime Field3 { get; set; }
        }

        [TestMethod]
        public void TestSqlDbProviderCreateInsertWithClassMappingId()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder();

            // Act
            var actual = statementBuilder.CreateInsert<TestWithSqlDbProviderCreateInsertClassMappingIdClass>(queryBuilder);
            var expected = $"" +
                $"INSERT INTO [ClassName] " +
                $"( [ClassNameId], [Field2], [Field3] ) " +
                $"VALUES " +
                $"( @ClassNameId, @Field2, @Field3 ) ; " +
                $"SELECT @ClassNameId AS [Result] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        private class ThrowExceptionOnSqlDbProviderCreateInsertIfTheIdentityFieldIsNotThePrimaryKeyFieldClass
        {
            [Primary]
            public int Field1 { get; set; }
            [Identity]
            public string Field2 { get; set; }
            public DateTime Field3 { get; set; }
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void ThrowExceptionOnSqlDbProviderCreateInsertIfTheIdentityFieldIsNotThePrimaryKeyField()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder();

            // Act/Assert
            statementBuilder.CreateInsert<ThrowExceptionOnSqlDbProviderCreateInsertIfTheIdentityFieldIsNotThePrimaryKeyFieldClass>(queryBuilder);
        }

        private class ThrowExceptionOnSqlDbProviderCreateInsertIfTheIdentityFieldIsNotTheClassIdFieldClass
        {
            public int ThrowExceptionOnSqlDbProviderCreateInsertIfTheIdentityFieldIsNotTheClassIdFieldClassId { get; set; }
            [Identity]
            public string Field2 { get; set; }
            public DateTime Field3 { get; set; }
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void ThrowExceptionOnSqlDbProviderCreateInsertIfTheIdentityFieldIsNotTheClassIdField()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder();

            // Act/Assert
            statementBuilder.CreateInsert<ThrowExceptionOnSqlDbProviderCreateInsertIfTheIdentityFieldIsNotTheClassIdFieldClass>(queryBuilder);
        }

        private class ThrowExceptionOnSqlDbProviderCreateInsertIfTheIdentityFieldIsNotTheIdFieldClass
        {
            public int Id { get; set; }
            [Identity]
            public string Field2 { get; set; }
            public DateTime Field3 { get; set; }
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void ThrowExceptionOnSqlDbProviderCreateInsertIfTheIdentityFieldIsNotTheIdField()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder();

            // Act/Assert
            statementBuilder.CreateInsert<ThrowExceptionOnSqlDbProviderCreateInsertIfTheIdentityFieldIsNotTheIdFieldClass>(queryBuilder);
        }

        [Map("ClassName")]
        private class ThrowExceptionOnSqlDbProviderCreateInsertIfTheIdentityFieldIsNotTheClassMappingIdFieldClass
        {
            public int ClassNameId { get; set; }
            [Identity]
            public string Field2 { get; set; }
            public DateTime Field3 { get; set; }
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void ThrowExceptionOnSqlDbProviderCreateInsertIfTheIdentityFieldIsNotTheClassMappingIdField()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder();

            // Act/Assert
            statementBuilder.CreateInsert<ThrowExceptionOnSqlDbProviderCreateInsertIfTheIdentityFieldIsNotTheClassMappingIdFieldClass>(queryBuilder);
        }
    }
}
