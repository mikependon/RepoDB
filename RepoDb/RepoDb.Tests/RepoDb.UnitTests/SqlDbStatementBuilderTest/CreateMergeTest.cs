using NUnit.Framework;
using RepoDb.Attributes;
using RepoDb.Enumerations;
using System;
using System.Collections.Generic;

namespace RepoDb.UnitTests.SqlDbStatementBuilderTest
{
    [TestFixture]
    public class CreateMergeTest
    {
        private class TestCreateMergeWithoutMappingsClass : DataEntity
        {
            public int Field1 { get; set; }
            public string Field2 { get; set; }
            public DateTime Field3 { get; set; }
        }

        [Test]
        public void TestCreateMergeWithoutMappings()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<TestCreateMergeWithoutMappingsClass>();
            var qualifiers = Field.From("Field1");

            // Act
            var actual = statementBuilder.CreateMerge(queryBuilder, qualifiers);
            var expected = $"" +
                $"MERGE [TestCreateMergeWithoutMappingsClass] AS T " +
                $"USING ( SELECT @Field1 AS [Field1], @Field2 AS [Field2], @Field3 AS [Field3] ) " +
                $"AS S ON ( S.[Field1] = T.[Field1] ) " +
                $"WHEN NOT MATCHED THEN " +
                $"INSERT ( [Field1], [Field2], [Field3] ) " +
                $"VALUES ( S.[Field1], S.[Field2], S.[Field3] ) " +
                $"WHEN MATCHED THEN " +
                $"UPDATE SET [Field2] = S.[Field2], [Field3] = S.[Field3] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [Map("ClassName")]
        private class TestCreateMergeWithClassMappingsClass : DataEntity
        {
            public int Field1 { get; set; }
            public string Field2 { get; set; }
            public DateTime Field3 { get; set; }
        }

        [Test]
        public void TestCreateMergeWithClassMappings()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<TestCreateMergeWithClassMappingsClass>();
            var qualifiers = Field.From("Field1");

            // Act
            var actual = statementBuilder.CreateMerge(queryBuilder, qualifiers);
            var expected = $"" +
                $"MERGE [ClassName] AS T " +
                $"USING ( SELECT @Field1 AS [Field1], @Field2 AS [Field2], @Field3 AS [Field3] ) " +
                $"AS S ON ( S.[Field1] = T.[Field1] ) " +
                $"WHEN NOT MATCHED THEN " +
                $"INSERT ( [Field1], [Field2], [Field3] ) " +
                $"VALUES ( S.[Field1], S.[Field2], S.[Field3] ) " +
                $"WHEN MATCHED THEN " +
                $"UPDATE SET [Field2] = S.[Field2], [Field3] = S.[Field3] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        private class TestCreateMergeWithIdFieldClass : DataEntity
        {
            public int Id { get; set; }
            public string Field2 { get; set; }
            public DateTime Field3 { get; set; }
        }

        [Test]
        public void TestCreateMergeWithIdField()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<TestCreateMergeWithIdFieldClass>();
            var qualifiers = (IEnumerable<Field>)null;

            // Act
            var actual = statementBuilder.CreateMerge(queryBuilder, qualifiers);
            var expected = $"" +
                $"MERGE [TestCreateMergeWithIdFieldClass] AS T " +
                $"USING ( SELECT @Id AS [Id], @Field2 AS [Field2], @Field3 AS [Field3] ) " +
                $"AS S ON ( S.[Id] = T.[Id] ) " +
                $"WHEN NOT MATCHED THEN " +
                $"INSERT ( [Id], [Field2], [Field3] ) " +
                $"VALUES ( S.[Id], S.[Field2], S.[Field3] ) " +
                $"WHEN MATCHED THEN " +
                $"UPDATE SET [Field2] = S.[Field2], [Field3] = S.[Field3] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        private class TestCreateMergeWithPrimaryKeyClass : DataEntity
        {
            [Primary]
            public int Field1 { get; set; }
            public string Field2 { get; set; }
            public DateTime Field3 { get; set; }
        }

        [Test]
        public void TestCreateMergeWithPrimaryKey()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<TestCreateMergeWithPrimaryKeyClass>();
            var qualifiers = (IEnumerable<Field>)null;

            // Act
            var actual = statementBuilder.CreateMerge(queryBuilder, qualifiers);
            var expected = $"" +
                $"MERGE [TestCreateMergeWithPrimaryKeyClass] AS T " +
                $"USING ( SELECT @Field1 AS [Field1], @Field2 AS [Field2], @Field3 AS [Field3] ) " +
                $"AS S ON ( S.[Field1] = T.[Field1] ) " +
                $"WHEN NOT MATCHED THEN " +
                $"INSERT ( [Field1], [Field2], [Field3] ) " +
                $"VALUES ( S.[Field1], S.[Field2], S.[Field3] ) " +
                $"WHEN MATCHED THEN " +
                $"UPDATE SET [Field2] = S.[Field2], [Field3] = S.[Field3] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        private class TestCreateMergeWithClassIdClass : DataEntity
        {
            public int TestCreateMergeWithClassIdClassId { get; set; }
            public string Field2 { get; set; }
            public DateTime Field3 { get; set; }
        }

        [Test]
        public void TestCreateMergeWithClassId()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<TestCreateMergeWithClassIdClass>();
            var qualifiers = (IEnumerable<Field>)null;

            // Act
            var actual = statementBuilder.CreateMerge(queryBuilder, qualifiers);
            var expected = $"" +
                $"MERGE [TestCreateMergeWithClassIdClass] AS T " +
                $"USING ( SELECT @TestCreateMergeWithClassIdClassId AS [TestCreateMergeWithClassIdClassId], @Field2 AS [Field2], @Field3 AS [Field3] ) " +
                $"AS S ON ( S.[TestCreateMergeWithClassIdClassId] = T.[TestCreateMergeWithClassIdClassId] ) " +
                $"WHEN NOT MATCHED THEN " +
                $"INSERT ( [TestCreateMergeWithClassIdClassId], [Field2], [Field3] ) " +
                $"VALUES ( S.[TestCreateMergeWithClassIdClassId], S.[Field2], S.[Field3] ) " +
                $"WHEN MATCHED THEN " +
                $"UPDATE SET [Field2] = S.[Field2], [Field3] = S.[Field3] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [Map("ClassName")]
        private class TestCreateMergeWithClassMappingIdClass : DataEntity
        {
            public int ClassNameId { get; set; }
            public string Field2 { get; set; }
            public DateTime Field3 { get; set; }
        }

        [Test]
        public void TestCreateMergeWithClassMappingId()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<TestCreateMergeWithClassMappingIdClass>();
            var qualifiers = (IEnumerable<Field>)null;

            // Act
            var actual = statementBuilder.CreateMerge(queryBuilder, qualifiers);
            var expected = $"" +
                $"MERGE [ClassName] AS T " +
                $"USING ( SELECT @ClassNameId AS [ClassNameId], @Field2 AS [Field2], @Field3 AS [Field3] ) " +
                $"AS S ON ( S.[ClassNameId] = T.[ClassNameId] ) " +
                $"WHEN NOT MATCHED THEN " +
                $"INSERT ( [ClassNameId], [Field2], [Field3] ) " +
                $"VALUES ( S.[ClassNameId], S.[Field2], S.[Field3] ) " +
                $"WHEN MATCHED THEN " +
                $"UPDATE SET [Field2] = S.[Field2], [Field3] = S.[Field3] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        private class TestCreateMergeWithInsertIgnoreClass : DataEntity
        {
            public int Field1 { get; set; }
            public string Field2 { get; set; }
            [Attributes.Ignore(Command.Insert)]
            public DateTime Field3 { get; set; }
        }

        [Test]
        public void TestCreateMergeWithInsertIgnore()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<TestCreateMergeWithInsertIgnoreClass>();
            var qualifiers = Field.From("Field1");

            // Act
            var actual = statementBuilder.CreateMerge(queryBuilder, qualifiers);
            var expected = $"" +
                $"MERGE [TestCreateMergeWithInsertIgnoreClass] AS T " +
                $"USING ( SELECT @Field1 AS [Field1], @Field2 AS [Field2], @Field3 AS [Field3] ) " +
                $"AS S ON ( S.[Field1] = T.[Field1] ) " +
                $"WHEN NOT MATCHED THEN " +
                $"INSERT ( [Field1], [Field2] ) " +
                $"VALUES ( S.[Field1], S.[Field2] ) " +
                $"WHEN MATCHED THEN " +
                $"UPDATE SET [Field2] = S.[Field2], [Field3] = S.[Field3] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        private class TestCreateMergeWithUpdateIgnoreClass : DataEntity
        {
            public int Field1 { get; set; }
            public string Field2 { get; set; }
            [Attributes.Ignore(Command.Update)]
            public DateTime Field3 { get; set; }
        }

        [Test]
        public void TestCreateMergeWithUpdateIgnore()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<TestCreateMergeWithUpdateIgnoreClass>();
            var qualifiers = Field.From("Field1");

            // Act
            var actual = statementBuilder.CreateMerge(queryBuilder, qualifiers);
            var expected = $"" +
                $"MERGE [TestCreateMergeWithUpdateIgnoreClass] AS T " +
                $"USING ( SELECT @Field1 AS [Field1], @Field2 AS [Field2], @Field3 AS [Field3] ) " +
                $"AS S ON ( S.[Field1] = T.[Field1] ) " +
                $"WHEN NOT MATCHED THEN " +
                $"INSERT ( [Field1], [Field2], [Field3] ) " +
                $"VALUES ( S.[Field1], S.[Field2], S.[Field3] ) " +
                $"WHEN MATCHED THEN " +
                $"UPDATE SET [Field2] = S.[Field2] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        private class TestCreateMergeWithMergeIgnoreClass : DataEntity
        {
            public int Field1 { get; set; }
            public string Field2 { get; set; }
            [Attributes.Ignore(Command.Merge)]
            public DateTime Field3 { get; set; }
        }

        [Test]
        public void TestCreateMergeWithMergeIgnore()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<TestCreateMergeWithMergeIgnoreClass>();
            var qualifiers = Field.From("Field1");

            // Act
            var actual = statementBuilder.CreateMerge(queryBuilder, qualifiers);
            var expected = $"" +
                $"MERGE [TestCreateMergeWithMergeIgnoreClass] AS T " +
                $"USING ( SELECT @Field1 AS [Field1], @Field2 AS [Field2] ) " +
                $"AS S ON ( S.[Field1] = T.[Field1] ) " +
                $"WHEN NOT MATCHED THEN " +
                $"INSERT ( [Field1], [Field2] ) " +
                $"VALUES ( S.[Field1], S.[Field2] ) " +
                $"WHEN MATCHED THEN " +
                $"UPDATE SET [Field2] = S.[Field2] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        private class TestCreateMergeWithFieldMappingClass : DataEntity
        {
            public int Field1 { get; set; }
            public string Field2 { get; set; }
            [Map("Field4")]
            public DateTime Field3 { get; set; }
        }

        [Test]
        public void TestCreateMergeWithFieldMapping()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<TestCreateMergeWithFieldMappingClass>();
            var qualifiers = Field.From("Field1");

            // Act
            var actual = statementBuilder.CreateMerge(queryBuilder, qualifiers);
            var expected = $"" +
                $"MERGE [TestCreateMergeWithFieldMappingClass] AS T " +
                $"USING ( SELECT @Field1 AS [Field1], @Field2 AS [Field2], @Field4 AS [Field4] ) " +
                $"AS S ON ( S.[Field1] = T.[Field1] ) " +
                $"WHEN NOT MATCHED THEN " +
                $"INSERT ( [Field1], [Field2], [Field4] ) " +
                $"VALUES ( S.[Field1], S.[Field2], S.[Field4] ) " +
                $"WHEN MATCHED THEN " +
                $"UPDATE SET [Field2] = S.[Field2], [Field4] = S.[Field4] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        private class ThrowExceptionIfQualifierFieldsAreNullClass : DataEntity
        {
        }

        [Test]
        public void ThrowExceptionIfQualifierFieldsAreNull()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<ThrowExceptionIfQualifierFieldsAreNullClass>();
            var qualifiers = (IEnumerable<Field>)null;

            // Act/Assert
            Assert.Throws<InvalidOperationException>(() => statementBuilder.CreateMerge(queryBuilder, qualifiers));
        }

        private class ThrowExceptionIfAtleastOneQualifierFieldIsMissingFromDataEntityClass : DataEntity
        {
            public int Field1 { get; set; }
            public string Field2 { get; set; }
            public DateTime Field3 { get; set; }
        }

        [Test]
        public void ThrowExceptionIfAtleastOneQualifierFieldIsMissingFromDataEntity()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<ThrowExceptionIfAtleastOneQualifierFieldIsMissingFromDataEntityClass>();
            var qualifiers = Field.From("Field1", "Field4");

            // Act/Assert
            Assert.Throws<InvalidOperationException>(() => statementBuilder.CreateMerge(queryBuilder, qualifiers));
        }

        private class ThrowExceptionIfQualifierFieldIsNotMatchingFromMappingsClass : DataEntity
        {
            public int Field1 { get; set; }
            public string Field2 { get; set; }
            [Map("Field4")]
            public DateTime Field3 { get; set; }
        }

        [Test]
        public void ThrowExceptionIfQualifierFieldIsNotMatchingFromMappings()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<ThrowExceptionIfQualifierFieldIsNotMatchingFromMappingsClass>();
            var qualifiers = Field.From("Field1", "Field3");

            // Act/Assert
            Assert.Throws<InvalidOperationException>(() => statementBuilder.CreateMerge(queryBuilder, qualifiers));
        }


        private class ThrowExceptionAtMergeIfTheIdentityFieldIsNotThePrimaryKeyFieldClass : DataEntity
        {
            [Primary]
            public int Field1 { get; set; }
            [Identity]
            public string Field2 { get; set; }
            public DateTime Field3 { get; set; }
        }

        [Test]
        public void ThrowExceptionAtMergeIfTheIdentityFieldIsNotThePrimaryKeyField()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<ThrowExceptionAtMergeIfTheIdentityFieldIsNotThePrimaryKeyFieldClass>();

            // Act/Assert
            Assert.Throws<InvalidOperationException>(() => statementBuilder.CreateInsert(queryBuilder));
        }

        private class ThrowExceptionAtMergeIfTheIdentityFieldIsNotTheClassIdFieldClass : DataEntity
        {
            public int ThrowExceptionAtMergeIfTheIdentityFieldIsNotTheClassIdFieldClassId { get; set; }
            [Identity]
            public string Field2 { get; set; }
            public DateTime Field3 { get; set; }
        }

        [Test]
        public void ThrowExceptionAtMergeIfTheIdentityFieldIsNotTheClassIdField()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<ThrowExceptionAtMergeIfTheIdentityFieldIsNotTheClassIdFieldClass>();

            // Act/Assert
            Assert.Throws<InvalidOperationException>(() => statementBuilder.CreateInsert(queryBuilder));
        }

        private class ThrowExceptionAtMergeIfTheIdentityFieldIsNotTheIdFieldClass : DataEntity
        {
            public int Id { get; set; }
            [Identity]
            public string Field2 { get; set; }
            public DateTime Field3 { get; set; }
        }

        [Test]
        public void ThrowExceptionAtMergeIfTheIdentityFieldIsNotTheIdField()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<ThrowExceptionAtMergeIfTheIdentityFieldIsNotTheIdFieldClass>();

            // Act/Assert
            Assert.Throws<InvalidOperationException>(() => statementBuilder.CreateInsert(queryBuilder));
        }

        [Map("ClassName")]
        private class ThrowExceptionAtMergeIfTheIdentityFieldIsNotTheClassMappingIdFieldClass : DataEntity
        {
            public int ClassNameId { get; set; }
            [Identity]
            public string Field2 { get; set; }
            public DateTime Field3 { get; set; }
        }

        [Test]
        public void ThrowExceptionAtMergeIfTheIdentityFieldIsNotTheClassMappingIdField()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<ThrowExceptionAtMergeIfTheIdentityFieldIsNotTheClassMappingIdFieldClass>();

            // Act/Assert
            Assert.Throws<InvalidOperationException>(() => statementBuilder.CreateInsert(queryBuilder));
        }
    }
}
