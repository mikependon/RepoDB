using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Attributes;
using System;
using System.Collections.Generic;

namespace RepoDb.UnitTests.StatementBuilders
{
    [TestClass]
    public class SqlDbProviderCreateInlineMergeTest
    {
        private class TestSqlDbProviderCreateInlineMergeWithoutMappingsClass
        {
            public int Field1 { get; set; }
            public string Field2 { get; set; }
            public DateTime Field3 { get; set; }
        }

        [TestMethod]
        public void TestSqlDbProviderCreateInlineMergeWithoutMappings()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder();
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });
            var qualifiers = Field.From(new[] { "Field1" });

            // Act
            var actual = statementBuilder.CreateInlineMerge<TestSqlDbProviderCreateInlineMergeWithoutMappingsClass>(queryBuilder, null, fields, qualifiers);
            var expected = $"" +
                $"MERGE [TestSqlDbProviderCreateInlineMergeWithoutMappingsClass] AS T " +
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
        private class TestSqlDbProviderCreateInlineMergeWithMappingClass
        {
            public int Field1 { get; set; }
            public string Field2 { get; set; }
            public DateTime Field3 { get; set; }
        }

        [TestMethod]
        public void TestSqlDbProviderCreateInlineMergeWithClassMapping()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder();
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });
            var qualifiers = Field.From(new[] { "Field1" });

            // Act
            var actual = statementBuilder.CreateInlineMerge<TestSqlDbProviderCreateInlineMergeWithMappingClass>(queryBuilder, null, fields, qualifiers);
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

        private class TestSqlDbProviderCreateInlineMergeWithAttributeMappingsClass
        {
            public int Field1 { get; set; }
            public string Field2 { get; set; }
            [Map("Field4")]
            public DateTime Field3 { get; set; }
        }

        [TestMethod]
        public void TestSqlDbProviderCreateInlineMergeWithAttributeMappings()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder();
            var fields = Field.From(new[] { "Field1", "Field2", "Field4" });
            var qualifiers = Field.From(new[] { "Field1" });

            // Act
            var actual = statementBuilder.CreateInlineMerge<TestSqlDbProviderCreateInlineMergeWithAttributeMappingsClass>(queryBuilder, null, fields, qualifiers);
            var expected = $"" +
                $"MERGE [TestSqlDbProviderCreateInlineMergeWithAttributeMappingsClass] AS T " +
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

        private class TestSqlDbProviderCreateInlineMergeWithIdFieldClass
        {
            public int Id { get; set; }
            public string Field2 { get; set; }
            public DateTime Field3 { get; set; }
        }

        [TestMethod]
        public void TestSqlDbProviderCreateInlineMergeWithIdField()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder();
            var fields = Field.From(new[] { "Id", "Field2", "Field3" });
            var qualifiers = (IEnumerable<Field>)null;

            // Act
            var actual = statementBuilder.CreateInlineMerge<TestSqlDbProviderCreateInlineMergeWithIdFieldClass>(queryBuilder, null, fields, qualifiers);
            var expected = $"" +
                $"MERGE [TestSqlDbProviderCreateInlineMergeWithIdFieldClass] AS T " +
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

        private class TestSqlDbProviderCreateInlineMergeWithPrimaryKeyFieldClass
        {
            [Primary]
            public int Field1 { get; set; }
            public string Field2 { get; set; }
            public DateTime Field3 { get; set; }
        }

        [TestMethod]
        public void TestSqlDbProviderCreateInlineMergeWithPrimaryKeyField()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder();
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });
            var qualifiers = (IEnumerable<Field>)null;

            // Act
            var actual = statementBuilder.CreateInlineMerge<TestSqlDbProviderCreateInlineMergeWithPrimaryKeyFieldClass>(queryBuilder, null, fields, qualifiers);
            var expected = $"" +
                $"MERGE [TestSqlDbProviderCreateInlineMergeWithPrimaryKeyFieldClass] AS T " +
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

        private class TestSqlDbProviderCreateInlineMergeWithClassIdFieldClass
        {
            public int TestSqlDbProviderCreateInlineMergeWithClassIdFieldClassId { get; set; }
            public string Field2 { get; set; }
            public DateTime Field3 { get; set; }
        }

        [TestMethod]
        public void TestSqlDbProviderCreateInlineMergeWithClassIdField()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder();
            var fields = Field.From(new[] { "TestSqlDbProviderCreateInlineMergeWithClassIdFieldClassId", "Field2", "Field3" });
            var qualifiers = (IEnumerable<Field>)null;

            // Act
            var actual = statementBuilder.CreateInlineMerge<TestSqlDbProviderCreateInlineMergeWithClassIdFieldClass>(queryBuilder, null, fields, qualifiers);
            var expected = $"" +
                $"MERGE [TestSqlDbProviderCreateInlineMergeWithClassIdFieldClass] AS T " +
                $"USING ( SELECT @TestSqlDbProviderCreateInlineMergeWithClassIdFieldClassId AS [TestSqlDbProviderCreateInlineMergeWithClassIdFieldClassId], @Field2 AS [Field2], @Field3 AS [Field3] ) " +
                $"AS S ON ( S.[TestSqlDbProviderCreateInlineMergeWithClassIdFieldClassId] = T.[TestSqlDbProviderCreateInlineMergeWithClassIdFieldClassId] ) " +
                $"WHEN NOT MATCHED THEN " +
                $"INSERT ( [TestSqlDbProviderCreateInlineMergeWithClassIdFieldClassId], [Field2], [Field3] ) " +
                $"VALUES ( S.[TestSqlDbProviderCreateInlineMergeWithClassIdFieldClassId], S.[Field2], S.[Field3] ) " +
                $"WHEN MATCHED THEN " +
                $"UPDATE SET [Field2] = S.[Field2], [Field3] = S.[Field3] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        private class TestSqlDbProviderCreateInlineMergeWithIdClass
        {
            public int Id { get; set; }
            public string Field2 { get; set; }
            public DateTime Field3 { get; set; }
        }

        [TestMethod]
        public void TestSqlDbProviderCreateInlineMergeWithId()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder();
            var fields = Field.From(new[] { "Id", "Field2", "Field3" });
            var qualifiers = (IEnumerable<Field>)null;

            // Act
            var actual = statementBuilder.CreateInlineMerge<TestSqlDbProviderCreateInlineMergeWithIdClass>(queryBuilder, null, fields, qualifiers);
            var expected = $"" +
                $"MERGE [TestSqlDbProviderCreateInlineMergeWithIdClass] AS T " +
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

        private class TestSqlDbProviderCreateInlineMergeWithClassIdClass
        {
            public int TestSqlDbProviderCreateInlineMergeWithClassIdClassId { get; set; }
            public string Field2 { get; set; }
            public DateTime Field3 { get; set; }
        }

        [TestMethod]
        public void TestSqlDbProviderCreateInlineMergeWithClassId()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder();
            var fields = Field.From(new[] { "TestSqlDbProviderCreateInlineMergeWithClassIdClassId", "Field2", "Field3" });
            var qualifiers = (IEnumerable<Field>)null;

            // Act
            var actual = statementBuilder.CreateInlineMerge<TestSqlDbProviderCreateInlineMergeWithClassIdClass>(queryBuilder, null, fields, qualifiers);
            var expected = $"" +
                $"MERGE [TestSqlDbProviderCreateInlineMergeWithClassIdClass] AS T " +
                $"USING ( SELECT @TestSqlDbProviderCreateInlineMergeWithClassIdClassId AS [TestSqlDbProviderCreateInlineMergeWithClassIdClassId], @Field2 AS [Field2], @Field3 AS [Field3] ) " +
                $"AS S ON ( S.[TestSqlDbProviderCreateInlineMergeWithClassIdClassId] = T.[TestSqlDbProviderCreateInlineMergeWithClassIdClassId] ) " +
                $"WHEN NOT MATCHED THEN " +
                $"INSERT ( [TestSqlDbProviderCreateInlineMergeWithClassIdClassId], [Field2], [Field3] ) " +
                $"VALUES ( S.[TestSqlDbProviderCreateInlineMergeWithClassIdClassId], S.[Field2], S.[Field3] ) " +
                $"WHEN MATCHED THEN " +
                $"UPDATE SET [Field2] = S.[Field2], [Field3] = S.[Field3] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [Map("ClassName")]
        private class TestSqlDbProviderCreateInlineMergeWithClassMappingIdClass
        {
            public int ClassNameId { get; set; }
            public string Field2 { get; set; }
            public DateTime Field3 { get; set; }
        }

        [TestMethod]
        public void TestSqlDbProviderCreateInlineMergeWithClassMappingId()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder();
            var fields = Field.From(new[] { "ClassNameId", "Field2", "Field3" });
            var qualifiers = (IEnumerable<Field>)null;

            // Act
            var actual = statementBuilder.CreateInlineMerge<TestSqlDbProviderCreateInlineMergeWithClassMappingIdClass>(queryBuilder, null, fields, qualifiers);
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

        [Map("ClassName")]
        private class TestSqlDbProviderCreateInlineMergeWithClassMappingIdFieldClass
        {
            public int ClassNameId { get; set; }
            public string Field2 { get; set; }
            public DateTime Field3 { get; set; }
        }

        [TestMethod]
        public void TestSqlDbProviderCreateInlineMergeWithClassMappingIdField()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder();
            var fields = Field.From(new[] { "ClassNameId", "Field2", "Field3" });
            var qualifiers = (IEnumerable<Field>)null;

            // Act
            var actual = statementBuilder.CreateInlineMerge<TestSqlDbProviderCreateInlineMergeWithClassMappingIdFieldClass>(queryBuilder, null, fields, qualifiers);
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

        private class ThrowExceptionOnSqlDbProviderCreateInlineMergeIfNoQualifiersAndNoPrimaryFieldDefinedClass
        {
            public int Field1 { get; set; }
            public string Field2 { get; set; }
            public DateTime Field3 { get; set; }
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnSqlDbProviderCreateInlineMergeIfNoQualifiersAndNoPrimaryFieldDefined()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder();
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });
            var qualifiers = (IEnumerable<Field>)null;

            // Act/Assert
            statementBuilder.CreateInlineMerge<ThrowExceptionOnSqlDbProviderCreateInlineMergeIfNoQualifiersAndNoPrimaryFieldDefinedClass>(queryBuilder, null, fields, qualifiers);
        }

        private class ThrowExceptionOnSqlDbProviderCreateInlineMergeIfAnIdentityFieldIsNotAPrimaryFieldClass
        {
            [Identity]
            public int Field1 { get; set; }
            public string Field2 { get; set; }
            public DateTime Field3 { get; set; }
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void ThrowExceptionOnSqlDbProviderCreateInlineMergeIfAnIdentityFieldIsNotAPrimaryField()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder();
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });
            var qualifiers = Field.From(new[] { "Field1" });

            // Act/Assert
            statementBuilder.CreateInlineMerge<ThrowExceptionOnSqlDbProviderCreateInlineMergeIfAnIdentityFieldIsNotAPrimaryFieldClass>(queryBuilder, null, fields, qualifiers);
        }

        private class ThrowExceptionOnSqlDbProviderCreateInlineMergeIfTheFieldsAreNullClass
        {
            public int Field1 { get; set; }
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnSqlDbProviderCreateInlineMergeIfTheFieldsAreNull()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder();
            var fields = (IEnumerable<Field>)null;
            var qualifiers = Field.From(new[] { "Field1" });

            // Act/Assert
            statementBuilder.CreateInlineMerge<ThrowExceptionOnSqlDbProviderCreateInlineMergeIfTheFieldsAreNullClass>(queryBuilder, null, fields, qualifiers);
        }

        private class ThrowExceptionOnSqlDbProviderCreateInlineMergeIfTheQualifiersAreNullWithoutPrimaryKeyClass
        {
            public int Field1 { get; set; }
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnSqlDbProviderCreateInlineMergeIfTheQualifiersAreNullWithoutPrimaryKey()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder();
            var fields = Field.From(new[] { "Field1" });
            var qualifiers = (IEnumerable<Field>)null;

            // Act/Assert
            statementBuilder.CreateInlineMerge<ThrowExceptionOnSqlDbProviderCreateInlineMergeIfTheQualifiersAreNullWithoutPrimaryKeyClass>(queryBuilder, null, fields, qualifiers);
        }

        private class ThrowExceptionOnSqlDbProviderCreateInlineMergeIfAQualifierFieldIsNotPresentAtDataEntityClass
        {
            public int Field1 { get; set; }
            public string Field2 { get; set; }
            public DateTime Field3 { get; set; }
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void ThrowExceptionOnSqlDbProviderCreateInlineMergeIfAQualifierFieldIsNotPresentAtDataEntity()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder();
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });
            var qualifiers = Field.From(new[] { "Field1", "Field4" });

            // Act/Assert
            statementBuilder.CreateInlineMerge<ThrowExceptionOnSqlDbProviderCreateInlineMergeIfAQualifierFieldIsNotPresentAtDataEntityClass>(queryBuilder, null, fields, qualifiers);
        }

        private class ThrowExceptionOnSqlDbProviderCreateInlineMergeIfAQualifierFieldIsNotPresentAtDataEntityFieldMappingClass
        {
            public int Field1 { get; set; }
            public string Field2 { get; set; }
            [Map("Field4")]
            public DateTime Field3 { get; set; }
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void ThrowExceptionOnSqlDbProviderCreateInlineMergeIfAQualifierFieldIsNotPresentAtDataEntityFieldMapping()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder();
            var fields = Field.From(new[] { "Field1", "Field2", "Field4" });
            var qualifiers = Field.From(new[] { "Field1", "Field3" });

            // Act/Assert
            statementBuilder.CreateInlineMerge<ThrowExceptionOnSqlDbProviderCreateInlineMergeIfAQualifierFieldIsNotPresentAtDataEntityFieldMappingClass>(queryBuilder, null, fields, qualifiers);
        }

        private class ThrowExceptionOnSqlDbProviderCreateInlineMergeIfTheIdentityFieldIsNotThePrimaryKeyFieldClass
        {
            [Primary]
            public int Field1 { get; set; }
            [Identity]
            public string Field2 { get; set; }
            public DateTime Field3 { get; set; }
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void ThrowExceptionOnSqlDbProviderCreateInlineMergeIfTheIdentityFieldIsNotThePrimaryKeyField()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder();

            // Act/Assert
            statementBuilder.CreateInsert<ThrowExceptionOnSqlDbProviderCreateInlineMergeIfTheIdentityFieldIsNotThePrimaryKeyFieldClass>(queryBuilder);
        }

        private class ThrowExceptionOnSqlDbProviderCreateInlineMergeIfTheIdentityFieldIsNotTheClassIdFieldClass
        {
            public int ThrowExceptionOnSqlDbProviderCreateInlineMergeIfTheIdentityFieldIsNotTheClassIdFieldClassId { get; set; }
            [Identity]
            public string Field2 { get; set; }
            public DateTime Field3 { get; set; }
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void ThrowExceptionOnSqlDbProviderCreateInlineMergeIfTheIdentityFieldIsNotTheClassIdField()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder();

            // Act/Assert
            statementBuilder.CreateInsert<ThrowExceptionOnSqlDbProviderCreateInlineMergeIfTheIdentityFieldIsNotTheClassIdFieldClass>(queryBuilder);
        }

        private class ThrowExceptionOnSqlDbProviderCreateInlineMergeIfTheIdentityFieldIsNotTheIdFieldClass
        {
            public int Id { get; set; }
            [Identity]
            public string Field2 { get; set; }
            public DateTime Field3 { get; set; }
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void ThrowExceptionOnSqlDbProviderCreateInlineMergeIfTheIdentityFieldIsNotTheIdField()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder();

            // Act/Assert
            statementBuilder.CreateInsert<ThrowExceptionOnSqlDbProviderCreateInlineMergeIfTheIdentityFieldIsNotTheIdFieldClass>(queryBuilder);
        }

        [Map("ClassName")]
        private class ThrowExceptionOnSqlDbProviderCreateInlineMergeIfTheIdentityFieldIsNotTheClassMappingIdFieldClass
        {
            public int ClassNameId { get; set; }
            [Identity]
            public string Field2 { get; set; }
            public DateTime Field3 { get; set; }
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void ThrowExceptionOnSqlDbProviderCreateInlineMergeIfTheIdentityFieldIsNotTheClassMappingIdField()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder();

            // Act/Assert
            statementBuilder.CreateInsert<ThrowExceptionOnSqlDbProviderCreateInlineMergeIfTheIdentityFieldIsNotTheClassMappingIdFieldClass>(queryBuilder);
        }
    }
}
