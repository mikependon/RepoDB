using NUnit.Framework;
using RepoDb.Attributes;
using RepoDb.Enumerations;
using System;

namespace RepoDb.UnitTests.SqlDbStatementBuilderTest
{
    [TestFixture]
    public class CreateInsertTest
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

            // Act
            var actual = statementBuilder.CreateInsert(queryBuilder);
            var expected = $"" +
                $"INSERT INTO [TestWithoutMappingsClass] " +
                $"( [Field1], [Field2], [Field3] ) " +
                $"VALUES " +
                $"( @Field1, @Field2, @Field3 ) ; " +
                $"SELECT NULL AS [Result] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [Map("ClassName")]
        private class TestWithClassMappingClass : DataEntity
        {
            public int Field1 { get; set; }
            public string Field2 { get; set; }
            public DateTime Field3 { get; set; }
        }

        [Test]
        public void TestWithClassMapping()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<TestWithClassMappingClass>();

            // Act
            var actual = statementBuilder.CreateInsert(queryBuilder);
            var expected = $"" +
                $"INSERT INTO [ClassName] " +
                $"( [Field1], [Field2], [Field3] ) " +
                $"VALUES " +
                $"( @Field1, @Field2, @Field3 ) ; " +
                $"SELECT NULL AS [Result] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        private class TestWithFieldMappingsClass : DataEntity
        {
            public int Field1 { get; set; }
            public string Field2 { get; set; }
            [Map("Field4")]
            public DateTime Field3 { get; set; }
        }

        [Test]
        public void TestWithFieldMappings()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<TestWithFieldMappingsClass>();

            // Act
            var actual = statementBuilder.CreateInsert(queryBuilder);
            var expected = $"" +
                $"INSERT INTO [TestWithFieldMappingsClass] " +
                $"( [Field1], [Field2], [Field4] ) " +
                $"VALUES " +
                $"( @Field1, @Field2, @Field4 ) ; " +
                $"SELECT NULL AS [Result] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        private class TestWithIgnoreFieldClass : DataEntity
        {
            public int Field1 { get; set; }
            public string Field2 { get; set; }
            [Attributes.Ignore(Command.Insert)]
            public DateTime Field3 { get; set; }
        }

        [Test]
        public void TestWithIgnoreField()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<TestWithIgnoreFieldClass>();

            // Act
            var actual = statementBuilder.CreateInsert(queryBuilder);
            var expected = $"" +
                $"INSERT INTO [TestWithIgnoreFieldClass] " +
                $"( [Field1], [Field2] ) " +
                $"VALUES " +
                $"( @Field1, @Field2 ) ; " +
                $"SELECT NULL AS [Result] ;";

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

            // Act
            var actual = statementBuilder.CreateInsert(queryBuilder);
            var expected = $"" +
                $"INSERT INTO [TestWithIdClass] " +
                $"( [Id], [Field2], [Field3] ) " +
                $"VALUES " +
                $"( @Id, @Field2, @Field3 ) ; " +
                $"SELECT @Id AS [Result] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        private class TestWithClassIdFieldClass : DataEntity
        {
            public int TestWithClassIdFieldClassId { get; set; }
            public string Field2 { get; set; }
            public DateTime Field3 { get; set; }
        }

        [Test]
        public void TestWithClassIdField()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<TestWithClassIdFieldClass>();

            // Act
            var actual = statementBuilder.CreateInsert(queryBuilder);
            var expected = $"" +
                $"INSERT INTO [TestWithClassIdFieldClass] " +
                $"( [TestWithClassIdFieldClassId], [Field2], [Field3] ) " +
                $"VALUES " +
                $"( @TestWithClassIdFieldClassId, @Field2, @Field3 ) ; " +
                $"SELECT @TestWithClassIdFieldClassId AS [Result] ;";

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

            // Act
            var actual = statementBuilder.CreateInsert(queryBuilder);
            var expected = $"" +
                $"INSERT INTO [TestWithPrimaryKeyFieldClass] " +
                $"( [Field1], [Field2], [Field3] ) " +
                $"VALUES " +
                $"( @Field1, @Field2, @Field3 ) ; " +
                $"SELECT @Field1 AS [Result] ;";

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

            // Act
            var actual = statementBuilder.CreateInsert(queryBuilder);
            var expected = $"" +
                $"INSERT INTO [ClassName] " +
                $"( [ClassNameId], [Field2], [Field3] ) " +
                $"VALUES " +
                $"( @ClassNameId, @Field2, @Field3 ) ; " +
                $"SELECT @ClassNameId AS [Result] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        private class ThrowExceptionIfTheIdentityFieldIsNotThePrimaryKeyFieldClass : DataEntity
        {
            [Primary]
            public int Field1 { get; set; }
            [Identity]
            public string Field2 { get; set; }
            public DateTime Field3 { get; set; }
        }

        [Test]
        public void ThrowExceptionIfTheIdentityFieldIsNotThePrimaryKeyField()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<ThrowExceptionIfTheIdentityFieldIsNotThePrimaryKeyFieldClass>();

            // Act/Assert
            Assert.Throws<InvalidOperationException>(() => statementBuilder.CreateInsert(queryBuilder));
        }

        private class ThrowExceptionIfTheIdentityFieldIsNotTheClassIdFieldClass : DataEntity
        {
            public int ThrowExceptionIfTheIdentityFieldIsNotTheClassIdFieldClassId { get; set; }
            [Identity]
            public string Field2 { get; set; }
            public DateTime Field3 { get; set; }
        }

        [Test]
        public void ThrowExceptionIfTheIdentityFieldIsNotTheClassIdField()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<ThrowExceptionIfTheIdentityFieldIsNotTheClassIdFieldClass>();

            // Act/Assert
            Assert.Throws<InvalidOperationException>(() => statementBuilder.CreateInsert(queryBuilder));
        }

        private class ThrowExceptionIfTheIdentityFieldIsNotTheIdFieldClass : DataEntity
        {
            public int Id { get; set; }
            [Identity]
            public string Field2 { get; set; }
            public DateTime Field3 { get; set; }
        }

        [Test]
        public void ThrowExceptionIfTheIdentityFieldIsNotTheIdField()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<ThrowExceptionIfTheIdentityFieldIsNotTheIdFieldClass>();

            // Act/Assert
            Assert.Throws<InvalidOperationException>(() => statementBuilder.CreateInsert(queryBuilder));
        }

        [Map("ClassName")]
        private class ThrowExceptionIfTheIdentityFieldIsNotTheClassMappingIdFieldClass : DataEntity
        {
            public int ClassNameId { get; set; }
            [Identity]
            public string Field2 { get; set; }
            public DateTime Field3 { get; set; }
        }

        [Test]
        public void ThrowExceptionIfTheIdentityFieldIsNotTheClassMappingIdField()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<ThrowExceptionIfTheIdentityFieldIsNotTheClassMappingIdFieldClass>();

            // Act/Assert
            Assert.Throws<InvalidOperationException>(() => statementBuilder.CreateInsert(queryBuilder));
        }
    }
}
