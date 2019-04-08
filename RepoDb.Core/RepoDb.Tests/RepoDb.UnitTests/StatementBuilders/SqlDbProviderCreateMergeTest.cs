using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Attributes;
using System;
using System.Collections.Generic;

namespace RepoDb.UnitTests.StatementBuilders
{
    [TestClass]
    public class SqlDbProviderCreateMergeTest
    {
        private class TestSqlDbProviderCreateMergeWithoutMappingsClass
        {
            public int Field1 { get; set; }
            public string Field2 { get; set; }
            public DateTime Field3 { get; set; }
        }

        [TestMethod]
        public void TestSqlDbProviderCreateMergeWithoutMappings()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder();
            var qualifiers = Field.From("Field1");

            // Act
            var actual = statementBuilder.CreateMerge<TestSqlDbProviderCreateMergeWithoutMappingsClass>(queryBuilder, qualifiers);
            var expected = $"" +
                $"MERGE [TestSqlDbProviderCreateMergeWithoutMappingsClass] AS T " +
                $"USING ( SELECT @Field1 AS [Field1], @Field2 AS [Field2], @Field3 AS [Field3] ) " +
                $"AS S ON ( S.[Field1] = T.[Field1] ) " +
                $"WHEN NOT MATCHED THEN " +
                $"INSERT ( [Field1], [Field2], [Field3] ) " +
                $"VALUES ( S.[Field1], S.[Field2], S.[Field3] ) " +
                $"WHEN MATCHED THEN " +
                $"UPDATE SET [Field1] = S.[Field1], [Field2] = S.[Field2], [Field3] = S.[Field3] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [Map("ClassName")]
        private class TestSqlDbProviderCreateMergeWithClassMappingsClass
        {
            public int Field1 { get; set; }
            public string Field2 { get; set; }
            public DateTime Field3 { get; set; }
        }

        [TestMethod]
        public void TestSqlDbProviderCreateMergeWithClassMappings()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder();
            var qualifiers = Field.From("Field1");

            // Act
            var actual = statementBuilder.CreateMerge<TestSqlDbProviderCreateMergeWithClassMappingsClass>(queryBuilder, qualifiers);
            var expected = $"" +
                $"MERGE [ClassName] AS T " +
                $"USING ( SELECT @Field1 AS [Field1], @Field2 AS [Field2], @Field3 AS [Field3] ) " +
                $"AS S ON ( S.[Field1] = T.[Field1] ) " +
                $"WHEN NOT MATCHED THEN " +
                $"INSERT ( [Field1], [Field2], [Field3] ) " +
                $"VALUES ( S.[Field1], S.[Field2], S.[Field3] ) " +
                $"WHEN MATCHED THEN " +
                $"UPDATE SET [Field1] = S.[Field1], [Field2] = S.[Field2], [Field3] = S.[Field3] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        private class TestSqlDbProviderCreateMergeWithIdFieldClass
        {
            public int Id { get; set; }
            public string Field2 { get; set; }
            public DateTime Field3 { get; set; }
        }

        [TestMethod]
        public void TestSqlDbProviderCreateMergeWithIdField()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder();
            var qualifiers = (IEnumerable<Field>)null;

            // Act
            var actual = statementBuilder.CreateMerge<TestSqlDbProviderCreateMergeWithIdFieldClass>(queryBuilder, qualifiers);
            var expected = $"" +
                $"MERGE [TestSqlDbProviderCreateMergeWithIdFieldClass] AS T " +
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

        private class TestSqlDbProviderCreateMergeWithPrimaryKeyClass
        {
            [Primary]
            public int Field1 { get; set; }
            public string Field2 { get; set; }
            public DateTime Field3 { get; set; }
        }

        [TestMethod]
        public void TestSqlDbProviderCreateMergeWithPrimaryKey()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder();
            var qualifiers = (IEnumerable<Field>)null;

            // Act
            var actual = statementBuilder.CreateMerge<TestSqlDbProviderCreateMergeWithPrimaryKeyClass>(queryBuilder, qualifiers);
            var expected = $"" +
                $"MERGE [TestSqlDbProviderCreateMergeWithPrimaryKeyClass] AS T " +
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

        private class TestSqlDbProviderCreateMergeWithClassIdClass
        {
            public int TestSqlDbProviderCreateMergeWithClassIdClassId { get; set; }
            public string Field2 { get; set; }
            public DateTime Field3 { get; set; }
        }

        [TestMethod]
        public void TestSqlDbProviderCreateMergeWithClassId()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder();
            var qualifiers = (IEnumerable<Field>)null;

            // Act
            var actual = statementBuilder.CreateMerge<TestSqlDbProviderCreateMergeWithClassIdClass>(queryBuilder, qualifiers);
            var expected = $"" +
                $"MERGE [TestSqlDbProviderCreateMergeWithClassIdClass] AS T " +
                $"USING ( SELECT @TestSqlDbProviderCreateMergeWithClassIdClassId AS [TestSqlDbProviderCreateMergeWithClassIdClassId], @Field2 AS [Field2], @Field3 AS [Field3] ) " +
                $"AS S ON ( S.[TestSqlDbProviderCreateMergeWithClassIdClassId] = T.[TestSqlDbProviderCreateMergeWithClassIdClassId] ) " +
                $"WHEN NOT MATCHED THEN " +
                $"INSERT ( [TestSqlDbProviderCreateMergeWithClassIdClassId], [Field2], [Field3] ) " +
                $"VALUES ( S.[TestSqlDbProviderCreateMergeWithClassIdClassId], S.[Field2], S.[Field3] ) " +
                $"WHEN MATCHED THEN " +
                $"UPDATE SET [Field2] = S.[Field2], [Field3] = S.[Field3] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [Map("ClassName")]
        private class TestSqlDbProviderCreateMergeWithClassMappingIdClass
        {
            public int ClassNameId { get; set; }
            public string Field2 { get; set; }
            public DateTime Field3 { get; set; }
        }

        [TestMethod]
        public void TestSqlDbProviderCreateMergeWithClassMappingId()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder();
            var qualifiers = (IEnumerable<Field>)null;

            // Act
            var actual = statementBuilder.CreateMerge<TestSqlDbProviderCreateMergeWithClassMappingIdClass>(queryBuilder, qualifiers);
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

        private class TestSqlDbProviderCreateMergeWithFieldMappingClass
        {
            public int Field1 { get; set; }
            public string Field2 { get; set; }
            [Map("Field4")]
            public DateTime Field3 { get; set; }
        }

        [TestMethod]
        public void TestSqlDbProviderCreateMergeWithFieldMapping()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder();
            var qualifiers = Field.From("Field1");

            // Act
            var actual = statementBuilder.CreateMerge<TestSqlDbProviderCreateMergeWithFieldMappingClass>(queryBuilder, qualifiers);
            var expected = $"" +
                $"MERGE [TestSqlDbProviderCreateMergeWithFieldMappingClass] AS T " +
                $"USING ( SELECT @Field1 AS [Field1], @Field2 AS [Field2], @Field4 AS [Field4] ) " +
                $"AS S ON ( S.[Field1] = T.[Field1] ) " +
                $"WHEN NOT MATCHED THEN " +
                $"INSERT ( [Field1], [Field2], [Field4] ) " +
                $"VALUES ( S.[Field1], S.[Field2], S.[Field4] ) " +
                $"WHEN MATCHED THEN " +
                $"UPDATE SET [Field1] = S.[Field1], [Field2] = S.[Field2], [Field4] = S.[Field4] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        /* Michael: March 6, 2019 10:17 PM (UTC + 1)
         * Changed made when writing an extensive RegressionTest, we handled the scenario of this problem
         * inside the actual 'CreateMerge' method by assigning the primary key as the default qualifier */

        /*
        private class ThrowExceptionOnSqlDbProviderCreateMergeIfQualifierFieldsAreNullClass
        {
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void ThrowExceptionOnSqlDbProviderCreateMergeIfQualifierFieldsAreNull()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder<ThrowExceptionOnSqlDbProviderCreateMergeIfQualifierFieldsAreNullClass>();
            var qualifiers = (IEnumerable<Field>)null;

            // Act/Assert
            statementBuilder.CreateMerge(queryBuilder, qualifiers);
        }
        */

        private class ThrowExceptionOnSqlDbProviderCreateMergeIfAtleastOneQualifierFieldIsMissingFromDataEntityClass
        {
            public int Field1 { get; set; }
            public string Field2 { get; set; }
            public DateTime Field3 { get; set; }
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void ThrowExceptionOnSqlDbProviderCreateMergeIfAtleastOneQualifierFieldIsMissingFromDataEntity()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder();
            var qualifiers = Field.From("Field1", "Field4");

            // Act/Assert
            statementBuilder.CreateMerge<ThrowExceptionOnSqlDbProviderCreateMergeIfAtleastOneQualifierFieldIsMissingFromDataEntityClass>(queryBuilder, qualifiers);
        }

        private class ThrowExceptionOnSqlDbProviderCreateMergeIfQualifierFieldIsNotMatchingFromMappingsClass
        {
            public int Field1 { get; set; }
            public string Field2 { get; set; }
            [Map("Field4")]
            public DateTime Field3 { get; set; }
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void ThrowExceptionOnSqlDbProviderCreateMergeIfQualifierFieldIsNotMatchingFromMappings()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder();
            var qualifiers = Field.From("Field1", "Field3");

            // Act/Assert
            statementBuilder.CreateMerge<ThrowExceptionOnSqlDbProviderCreateMergeIfQualifierFieldIsNotMatchingFromMappingsClass>(queryBuilder, qualifiers);
        }

        private class ThrowExceptionOnSqlDbProviderCreateMergeIfTheIdentityFieldIsNotThePrimaryKeyFieldClass
        {
            [Primary]
            public int Field1 { get; set; }
            [Identity]
            public string Field2 { get; set; }
            public DateTime Field3 { get; set; }
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void ThrowExceptionOnSqlDbProviderCreateMergeIfTheIdentityFieldIsNotThePrimaryKeyField()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder();

            // Act/Assert
            statementBuilder.CreateInsert<ThrowExceptionOnSqlDbProviderCreateMergeIfTheIdentityFieldIsNotThePrimaryKeyFieldClass>(queryBuilder);
        }

        private class ThrowExceptionSqlDbProviderCreateMergeIfTheIdentityFieldIsNotTheClassIdFieldClass
        {
            public int ThrowExceptionIfTheIdentityFieldIsNotTheClassIdFieldClassId { get; set; }
            [Identity]
            public string Field2 { get; set; }
            public DateTime Field3 { get; set; }
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void ThrowExceptionOnSqlDbProviderCreateMergeIfTheIdentityFieldIsNotTheClassIdField()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder();

            // Act/Assert
            statementBuilder.CreateInsert<ThrowExceptionSqlDbProviderCreateMergeIfTheIdentityFieldIsNotTheClassIdFieldClass>(queryBuilder);
        }

        private class ThrowExceptionOnSqlDbProviderCreateMergeIfTheIdentityFieldIsNotTheIdFieldClass
        {
            public int Id { get; set; }
            [Identity]
            public string Field2 { get; set; }
            public DateTime Field3 { get; set; }
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void ThrowExceptionOnSqlDbProviderCreateMergeIfTheIdentityFieldIsNotTheIdField()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder();

            // Act/Assert
            statementBuilder.CreateInsert<ThrowExceptionOnSqlDbProviderCreateMergeIfTheIdentityFieldIsNotTheIdFieldClass>(queryBuilder);
        }

        [Map("ClassName")]
        private class ThrowExceptionOnSqlDbProviderCreateMergeIfTheIdentityFieldIsNotTheClassMappingIdFieldClass
        {
            public int ClassNameId { get; set; }
            [Identity]
            public string Field2 { get; set; }
            public DateTime Field3 { get; set; }
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void ThrowExceptionOnSqlDbProviderCreateMergeIfTheIdentityFieldIsNotTheClassMappingIdField()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder();

            // Act/Assert
            statementBuilder.CreateInsert<ThrowExceptionOnSqlDbProviderCreateMergeIfTheIdentityFieldIsNotTheClassMappingIdFieldClass>(queryBuilder);
        }
    }
}
