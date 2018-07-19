using NUnit.Framework;
using RepoDb.Attributes;
using RepoDb.Enumerations;
using System;

namespace RepoDb.UnitTests.SqlDbStatementBuilderTest
{
    [TestFixture]
    public class CreateInsertTest
    {
        private class TestCreateInsertWithoutMappingsClass : DataEntity
        {
            public int Field1 { get; set; }
            public string Field2 { get; set; }
            public DateTime Field3 { get; set; }
        }

        [Test]
        public void TestCreateInsertWithoutMappings()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<TestCreateInsertWithoutMappingsClass>();

            // Act
            var actual = statementBuilder.CreateInsert(queryBuilder);
            var expected = $"" +
                $"INSERT INTO [TestCreateInsertWithoutMappingsClass] " +
                $"( [Field1], [Field2], [Field3] ) " +
                $"VALUES " +
                $"( @Field1, @Field2, @Field3 ) ; " +
                $"SELECT NULL AS [Result] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [Map("ClassName")]
        private class TestCreateInsertWithClassMappingClass : DataEntity
        {
            public int Field1 { get; set; }
            public string Field2 { get; set; }
            public DateTime Field3 { get; set; }
        }

        [Test]
        public void TestCreateInsertWithClassMapping()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<TestCreateInsertWithClassMappingClass>();

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

        private class TestCreateInsertWithFieldMappingsClass : DataEntity
        {
            public int Field1 { get; set; }
            public string Field2 { get; set; }
            [Map("Field4")]
            public DateTime Field3 { get; set; }
        }

        [Test]
        public void TestCreateInsertWithFieldMappings()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<TestCreateInsertWithFieldMappingsClass>();

            // Act
            var actual = statementBuilder.CreateInsert(queryBuilder);
            var expected = $"" +
                $"INSERT INTO [TestCreateInsertWithFieldMappingsClass] " +
                $"( [Field1], [Field2], [Field4] ) " +
                $"VALUES " +
                $"( @Field1, @Field2, @Field4 ) ; " +
                $"SELECT NULL AS [Result] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        private class TestCreateInsertWithIgnoreFieldClass : DataEntity
        {
            public int Field1 { get; set; }
            public string Field2 { get; set; }
            [Attributes.Ignore(Command.Insert)]
            public DateTime Field3 { get; set; }
        }

        [Test]
        public void TestCreateInsertWithIgnoreField()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<TestCreateInsertWithIgnoreFieldClass>();

            // Act
            var actual = statementBuilder.CreateInsert(queryBuilder);
            var expected = $"" +
                $"INSERT INTO [TestCreateInsertWithIgnoreFieldClass] " +
                $"( [Field1], [Field2] ) " +
                $"VALUES " +
                $"( @Field1, @Field2 ) ; " +
                $"SELECT NULL AS [Result] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        private class TestCreateInsertWithIdClass : DataEntity
        {
            public int Id { get; set; }
            public string Field2 { get; set; }
            public DateTime Field3 { get; set; }
        }

        [Test]
        public void TestCreateInsertWithId()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<TestCreateInsertWithIdClass>();

            // Act
            var actual = statementBuilder.CreateInsert(queryBuilder);
            var expected = $"" +
                $"INSERT INTO [TestCreateInsertWithIdClass] " +
                $"( [Id], [Field2], [Field3] ) " +
                $"VALUES " +
                $"( @Id, @Field2, @Field3 ) ; " +
                $"SELECT @Id AS [Result] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        private class TestCreateInsertWithClassIdFieldClass : DataEntity
        {
            public int TestCreateInsertWithClassIdFieldClassId { get; set; }
            public string Field2 { get; set; }
            public DateTime Field3 { get; set; }
        }

        [Test]
        public void TestCreateInsertWithClassIdField()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<TestCreateInsertWithClassIdFieldClass>();

            // Act
            var actual = statementBuilder.CreateInsert(queryBuilder);
            var expected = $"" +
                $"INSERT INTO [TestCreateInsertWithClassIdFieldClass] " +
                $"( [TestCreateInsertWithClassIdFieldClassId], [Field2], [Field3] ) " +
                $"VALUES " +
                $"( @TestCreateInsertWithClassIdFieldClassId, @Field2, @Field3 ) ; " +
                $"SELECT @TestCreateInsertWithClassIdFieldClassId AS [Result] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        private class TestCreateInsertWithPrimaryKeyFieldClass : DataEntity
        {
            [Primary]
            public int Field1 { get; set; }
            public string Field2 { get; set; }
            public DateTime Field3 { get; set; }
        }

        [Test]
        public void TestCreateInsertWithPrimaryKeyField()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<TestCreateInsertWithPrimaryKeyFieldClass>();

            // Act
            var actual = statementBuilder.CreateInsert(queryBuilder);
            var expected = $"" +
                $"INSERT INTO [TestCreateInsertWithPrimaryKeyFieldClass] " +
                $"( [Field1], [Field2], [Field3] ) " +
                $"VALUES " +
                $"( @Field1, @Field2, @Field3 ) ; " +
                $"SELECT @Field1 AS [Result] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [Map("ClassName")]
        private class TestCreateInsertWithClassMappingIdClass : DataEntity
        {
            public int ClassNameId { get; set; }
            public string Field2 { get; set; }
            public DateTime Field3 { get; set; }
        }

        [Test]
        public void TestCreateInsertWithClassMappingId()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<TestCreateInsertWithClassMappingIdClass>();

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

        private class ThrowExceptionAtInsertIfTheIdentityFieldIsNotThePrimaryKeyFieldClass : DataEntity
        {
            [Primary]
            public int Field1 { get; set; }
            [Identity]
            public string Field2 { get; set; }
            public DateTime Field3 { get; set; }
        }

        [Test]
        public void ThrowExceptionAtInsertIfTheIdentityFieldIsNotThePrimaryKeyField()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<ThrowExceptionAtInsertIfTheIdentityFieldIsNotThePrimaryKeyFieldClass>();

            // Act/Assert
            Assert.Throws<InvalidOperationException>(() => statementBuilder.CreateInsert(queryBuilder));
        }

        private class ThrowExceptionAtInsertIfTheIdentityFieldIsNotTheClassIdFieldClass : DataEntity
        {
            public int ThrowExceptionAtInsertIfTheIdentityFieldIsNotTheClassIdFieldClassId { get; set; }
            [Identity]
            public string Field2 { get; set; }
            public DateTime Field3 { get; set; }
        }

        [Test]
        public void ThrowExceptionAtInsertIfTheIdentityFieldIsNotTheClassIdField()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<ThrowExceptionAtInsertIfTheIdentityFieldIsNotTheClassIdFieldClass>();

            // Act/Assert
            Assert.Throws<InvalidOperationException>(() => statementBuilder.CreateInsert(queryBuilder));
        }

        private class ThrowExceptionAtInsertIfTheIdentityFieldIsNotTheIdFieldClass : DataEntity
        {
            public int Id { get; set; }
            [Identity]
            public string Field2 { get; set; }
            public DateTime Field3 { get; set; }
        }

        [Test]
        public void ThrowExceptionAtInsertIfTheIdentityFieldIsNotTheIdField()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<ThrowExceptionAtInsertIfTheIdentityFieldIsNotTheIdFieldClass>();

            // Act/Assert
            Assert.Throws<InvalidOperationException>(() => statementBuilder.CreateInsert(queryBuilder));
        }

        [Map("ClassName")]
        private class ThrowExceptionAtInsertIfTheIdentityFieldIsNotTheClassMappingIdFieldClass : DataEntity
        {
            public int ClassNameId { get; set; }
            [Identity]
            public string Field2 { get; set; }
            public DateTime Field3 { get; set; }
        }

        [Test]
        public void ThrowExceptionAtInsertIfTheIdentityFieldIsNotTheClassMappingIdField()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<ThrowExceptionAtInsertIfTheIdentityFieldIsNotTheClassMappingIdFieldClass>();

            // Act/Assert
            Assert.Throws<InvalidOperationException>(() => statementBuilder.CreateInsert(queryBuilder));
        }
    }
}
