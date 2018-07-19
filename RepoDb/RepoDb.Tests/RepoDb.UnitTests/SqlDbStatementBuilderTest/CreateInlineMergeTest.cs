using NUnit.Framework;
using RepoDb.Attributes;
using RepoDb.Enumerations;
using System;
using System.Collections.Generic;

namespace RepoDb.UnitTests.SqlDbStatementBuilderTest
{
    [TestFixture]
    public class CreateInlineMergeTest
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
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });
            var qualifiers = Field.From(new[] { "Field1" });

            // Act
            var actual = statementBuilder.CreateInlineMerge(queryBuilder, fields, qualifiers);
            var expected = $"" +
                $"MERGE [TestWithoutMappingsClass] AS T " +
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
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });
            var qualifiers = Field.From(new[] { "Field1" });

            // Act
            var actual = statementBuilder.CreateInlineMerge(queryBuilder, fields, qualifiers);
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

        private class TestWithAttributeMappingsClass : DataEntity
        {
            public int Field1 { get; set; }
            public string Field2 { get; set; }
            [Map("Field4")]
            public DateTime Field3 { get; set; }
        }

        [Test]
        public void TestWithAttributeMappings()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<TestWithAttributeMappingsClass>();
            var fields = Field.From(new[] { "Field1", "Field2", "Field4" });
            var qualifiers = Field.From(new[] { "Field1" });

            // Act
            var actual = statementBuilder.CreateInlineMerge(queryBuilder, fields, qualifiers);
            var expected = $"" +
                $"MERGE [TestWithAttributeMappingsClass] AS T " +
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

        private class TestWithIgnoreInsertFieldClass : DataEntity
        {
            public int Field1 { get; set; }
            public string Field2 { get; set; }
            [Attributes.Ignore(Command.Insert)]
            public DateTime Field3 { get; set; }
        }

        [Test]
        public void TestWithIgnoreInsertField()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<TestWithIgnoreInsertFieldClass>();
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });
            var qualifiers = Field.From(new[] { "Field1" });

            // Act
            var actual = statementBuilder.CreateInlineMerge(queryBuilder, fields, qualifiers);
            var expected = $"" +
                $"MERGE [TestWithIgnoreInsertFieldClass] AS T " +
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

        private class TestWithIgnoreUpdateFieldClass : DataEntity
        {
            public int Field1 { get; set; }
            public string Field2 { get; set; }
            [Attributes.Ignore(Command.Update)]
            public DateTime Field3 { get; set; }
        }

        [Test]
        public void TestWithIgnoreUpdateField()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<TestWithIgnoreUpdateFieldClass>();
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });
            var qualifiers = Field.From(new[] { "Field1" });

            // Act
            var actual = statementBuilder.CreateInlineMerge(queryBuilder, fields, qualifiers);
            var expected = $"" +
                $"MERGE [TestWithIgnoreUpdateFieldClass] AS T " +
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

        private class TestWithIgnoreInlineMergeFieldClass : DataEntity
        {
            public int Field1 { get; set; }
            public string Field2 { get; set; }
            [Attributes.Ignore(Command.InlineMerge)]
            public DateTime Field3 { get; set; }
        }

        [Test]
        public void TestWithIgnoreInlineMergeField()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<TestWithIgnoreInlineMergeFieldClass>();
            var fields = Field.From(new[] { "Field1", "Field2" });
            var qualifiers = Field.From(new[] { "Field1" });

            // Act
            var actual = statementBuilder.CreateInlineMerge(queryBuilder, fields, qualifiers);
            var expected = $"" +
                $"MERGE [TestWithIgnoreInlineMergeFieldClass] AS T " +
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

        private class TestWithIdFieldClass : DataEntity
        {
            public int Id { get; set; }
            public string Field2 { get; set; }
            public DateTime Field3 { get; set; }
        }

        [Test]
        public void TestWithIdField()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<TestWithIdFieldClass>();
            var fields = Field.From(new[] { "Id", "Field2", "Field3" });
            var qualifiers = (IEnumerable<Field>)null;

            // Act
            var actual = statementBuilder.CreateInlineMerge(queryBuilder, fields, qualifiers);
            var expected = $"" +
                $"MERGE [TestWithIdFieldClass] AS T " +
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

        private class TestWithPrimaryKeyClass : DataEntity
        {
            [Primary]
            public int Field1 { get; set; }
            public string Field2 { get; set; }
            public DateTime Field3 { get; set; }
        }

        [Test]
        public void TestWithPrimaryKey()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<TestWithPrimaryKeyClass>();
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });
            var qualifiers = (IEnumerable<Field>)null;

            // Act
            var actual = statementBuilder.CreateInlineMerge(queryBuilder, fields, qualifiers);
            var expected = $"" +
                $"MERGE [TestWithPrimaryKeyClass] AS T " +
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
            var fields = Field.From(new[] { "TestWithClassIdFieldClassId", "Field2", "Field3" });
            var qualifiers = (IEnumerable<Field>)null;

            // Act
            var actual = statementBuilder.CreateInlineMerge(queryBuilder, fields, qualifiers);
            var expected = $"" +
                $"MERGE [TestWithClassIdFieldClass] AS T " +
                $"USING ( SELECT @TestWithClassIdFieldClassId AS [TestWithClassIdFieldClassId], @Field2 AS [Field2], @Field3 AS [Field3] ) " +
                $"AS S ON ( S.[TestWithClassIdFieldClassId] = T.[TestWithClassIdFieldClassId] ) " +
                $"WHEN NOT MATCHED THEN " +
                $"INSERT ( [TestWithClassIdFieldClassId], [Field2], [Field3] ) " +
                $"VALUES ( S.[TestWithClassIdFieldClassId], S.[Field2], S.[Field3] ) " +
                $"WHEN MATCHED THEN " +
                $"UPDATE SET [Field2] = S.[Field2], [Field3] = S.[Field3] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        private class TestWithPrimaryFieldClass : DataEntity
        {
            [Primary]
            public int Field1 { get; set; }
            public string Field2 { get; set; }
            public DateTime Field3 { get; set; }
        }

        [Test]
        public void TestWithPrimaryField()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<TestWithPrimaryFieldClass>();
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });
            var qualifiers = (IEnumerable<Field>)null;

            // Act
            var actual = statementBuilder.CreateInlineMerge(queryBuilder, fields, qualifiers);
            var expected = $"" +
                $"MERGE [TestWithPrimaryFieldClass] AS T " +
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
        private class TestWithClassMappingIdFieldClass : DataEntity
        {
            public int ClassNameId { get; set; }
            public string Field2 { get; set; }
            public DateTime Field3 { get; set; }
        }

        [Test]
        public void TestWithClassMappingIdField()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<TestWithClassMappingIdFieldClass>();
            var fields = Field.From(new[] { "ClassNameId", "Field2", "Field3" });
            var qualifiers = (IEnumerable<Field>)null;

            // Act
            var actual = statementBuilder.CreateInlineMerge(queryBuilder, fields, qualifiers);
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

        private class TestWithOverrideIgnoreFromInlineMergeClass : DataEntity
        {
            public int Field1 { get; set; }
            public string Field2 { get; set; }
            [Attributes.Ignore(Command.InlineMerge)]
            public DateTime Field3 { get; set; }
        }

        [Test]
        public void TestWithOverrideIgnoreFromInlineMerge()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<TestWithOverrideIgnoreFromInlineMergeClass>();
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });
            var qualifiers = Field.From(new[] { "Field1" });

            // Act
            var actual = statementBuilder.CreateInlineMerge(queryBuilder, fields, qualifiers, true);
            var expected = $"" +
                $"MERGE [TestWithOverrideIgnoreFromInlineMergeClass] AS T " +
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

        private class TestWithOverrideIgnoreFromMergeClass : DataEntity
        {
            public int Field1 { get; set; }
            public string Field2 { get; set; }
            [Attributes.Ignore(Command.Merge)]
            public DateTime Field3 { get; set; }
        }

        [Test]
        public void TestWithOverrideIgnoreFromMerge()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<TestWithOverrideIgnoreFromMergeClass>();
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });
            var qualifiers = Field.From(new[] { "Field1" });

            // Act
            var actual = statementBuilder.CreateInlineMerge(queryBuilder, fields, qualifiers, true);
            var expected = $"" +
                $"MERGE [TestWithOverrideIgnoreFromMergeClass] AS T " +
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

        private class TestWithOverrideIgnoreFromInsertClass : DataEntity
        {
            public int Field1 { get; set; }
            public string Field2 { get; set; }
            [Attributes.Ignore(Command.Insert)]
            public DateTime Field3 { get; set; }
        }

        [Test]
        public void TestWithOverrideIgnoreFromInsert()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<TestWithOverrideIgnoreFromInsertClass>();
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });
            var qualifiers = Field.From(new[] { "Field1" });

            // Act
            var actual = statementBuilder.CreateInlineMerge(queryBuilder, fields, qualifiers, true);
            var expected = $"" +
                $"MERGE [TestWithOverrideIgnoreFromInsertClass] AS T " +
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

        private class TestWithOverrideIgnoreFromUpdateClass : DataEntity
        {
            public int Field1 { get; set; }
            public string Field2 { get; set; }
            [Attributes.Ignore(Command.Update)]
            public DateTime Field3 { get; set; }
        }

        [Test]
        public void TestWithOverrideIgnoreFromUpdate()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<TestWithOverrideIgnoreFromUpdateClass>();
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });
            var qualifiers = Field.From(new[] { "Field1" });

            // Act
            var actual = statementBuilder.CreateInlineMerge(queryBuilder, fields, qualifiers, true);
            var expected = $"" +
                $"MERGE [TestWithOverrideIgnoreFromUpdateClass] AS T " +
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

        private class ThrowExceptionAtInlineMergeIfNoQualifiersAndNoPrimaryFieldDefinedClass : DataEntity
        {
            public int Field1 { get; set; }
            public string Field2 { get; set; }
            public DateTime Field3 { get; set; }
        }

        [Test]
        public void ThrowExceptionAtInlineMergeIfNoQualifiersAndNoPrimaryFieldDefined()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<ThrowExceptionAtInlineMergeIfNoQualifiersAndNoPrimaryFieldDefinedClass>();
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });
            var qualifiers = (IEnumerable<Field>)null;

            // Act/Assert
            Assert.Throws<NullReferenceException>(() => statementBuilder.CreateInlineMerge(queryBuilder, fields, qualifiers));
        }

        private class ThrowExceptionAtInlineMergeIfAnIdentityFieldIsNotAPrimaryFieldClass : DataEntity
        {
            [Identity]
            public int Field1 { get; set; }
            public string Field2 { get; set; }
            public DateTime Field3 { get; set; }
        }

        [Test]
        public void ThrowExceptionAtInlineMergeIfAnIdentityFieldIsNotAPrimaryField()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<ThrowExceptionAtInlineMergeIfAnIdentityFieldIsNotAPrimaryFieldClass>();
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });
            var qualifiers = Field.From(new[] { "Field1" });

            // Act/Assert
            Assert.Throws<InvalidOperationException>(() => statementBuilder.CreateInlineMerge(queryBuilder, fields, qualifiers));
        }

        private class ThrowExceptionIfTheFieldsAreNullClass : DataEntity
        {
            public int Field1 { get; set; }
        }

        [Test]
        public void ThrowExceptionIfTheFieldsAreNull()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<ThrowExceptionIfTheFieldsAreNullClass>();
            var fields = (IEnumerable<Field>)null;
            var qualifiers = Field.From(new[] { "Field1" });

            // Act/Assert
            Assert.Throws<NullReferenceException>(() => statementBuilder.CreateInlineMerge(queryBuilder, fields, qualifiers));
        }

        private class ThrowExceptionIfTheQualifiersAreNullWithoutPrimaryKeyClass : DataEntity
        {
            public int Field1 { get; set; }
        }

        [Test]
        public void ThrowExceptionIfTheQualifiersAreNullWithoutPrimaryKey()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<ThrowExceptionIfTheQualifiersAreNullWithoutPrimaryKeyClass>();
            var fields = Field.From(new[] { "Field1" });
            var qualifiers = (IEnumerable<Field>)null;

            // Act/Assert
            Assert.Throws<NullReferenceException>(() => statementBuilder.CreateInlineMerge(queryBuilder, fields, qualifiers));
        }

        private class ThrowExceptionIfAQualifierFieldIsNotPresentAtDataEntityClass : DataEntity
        {
            public int Field1 { get; set; }
            public string Field2 { get; set; }
            public DateTime Field3 { get; set; }
        }

        [Test]
        public void ThrowExceptionIfAQualifierFieldIsNotPresentAtDataEntity()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<ThrowExceptionIfAQualifierFieldIsNotPresentAtDataEntityClass>();
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });
            var qualifiers = Field.From(new[] { "Field1", "Field4" });

            // Act/Assert
            Assert.Throws<InvalidOperationException>(() => statementBuilder.CreateInlineMerge(queryBuilder, fields, qualifiers));
        }

        private class ThrowExceptionIfAQualifierFieldIsNotPresentAtDataEntityFieldMappingClass : DataEntity
        {
            public int Field1 { get; set; }
            public string Field2 { get; set; }
            [Map("Field4")]
            public DateTime Field3 { get; set; }
        }

        [Test]
        public void ThrowExceptionIfAQualifierFieldIsNotPresentAtDataEntityFieldMapping()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<ThrowExceptionIfAQualifierFieldIsNotPresentAtDataEntityFieldMappingClass>();
            var fields = Field.From(new[] { "Field1", "Field2", "Field4" });
            var qualifiers = Field.From(new[] { "Field1", "Field3" });

            // Act/Assert
            Assert.Throws<InvalidOperationException>(() => statementBuilder.CreateInlineMerge(queryBuilder, fields, qualifiers));
        }

        private class ThrowExceptionIfTheFieldsContainsAnIgnoreInlineMergeFieldsClass : DataEntity
        {
            public int Field1 { get; set; }
            public string Field2 { get; set; }
            [Attributes.Ignore(Command.InlineMerge)]
            public DateTime Field3 { get; set; }
        }

        [Test]
        public void ThrowExceptionIfTheFieldsContainsAnIgnoreInlineMergeFields()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<ThrowExceptionIfTheFieldsContainsAnIgnoreInlineMergeFieldsClass>();
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });
            var qualifiers = Field.From(new[] { "Field1" });

            // Act/Assert
            Assert.Throws<InvalidOperationException>(() => statementBuilder.CreateInlineMerge(queryBuilder, fields, qualifiers));
        }

        private class ThrowExceptionIfTheQualifiersContainsAnIgnoreInlineMergeFieldsClass : DataEntity
        {
            public int Field1 { get; set; }
            public string Field2 { get; set; }
            [Attributes.Ignore(Command.InlineMerge)]
            public DateTime Field3 { get; set; }
            public double Field4 { get; set; }
        }

        [Test]
        public void ThrowExceptionIfTheQualifiersContainsAnIgnoreInlineMergeFields()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<ThrowExceptionIfTheQualifiersContainsAnIgnoreInlineMergeFieldsClass>();
            var fields = Field.From(new[] { "Field1", "Field2", "Field4" });
            var qualifiers = Field.From(new[] { "Field1", "Field3" });

            // Act/Assert
            Assert.Throws<InvalidOperationException>(() => statementBuilder.CreateInlineMerge(queryBuilder, fields, qualifiers));
        }

        private class ThrowExceptionIfFieldIsIgnoreInlineMergeClass : DataEntity
        {
            public int Field1 { get; set; }
            public string Field2 { get; set; }
            [Attributes.Ignore(Command.InlineMerge)]
            public DateTime Field3 { get; set; }
        }

        [Test]
        public void ThrowExceptionIfFieldIsIgnoreInlineMerge()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<ThrowExceptionIfFieldIsIgnoreInlineMergeClass>();
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });
            var qualifiers = Field.From(new[] { "Field1" });

            // Act/Assert
            Assert.Throws<InvalidOperationException>(() => statementBuilder.CreateInlineMerge(queryBuilder, fields, qualifiers));
        }

        private class ThrowExceptionAtInlineMergeIfTheIdentityFieldIsNotThePrimaryKeyFieldClass : DataEntity
        {
            [Primary]
            public int Field1 { get; set; }
            [Identity]
            public string Field2 { get; set; }
            public DateTime Field3 { get; set; }
        }

        [Test]
        public void ThrowExceptionAtInlineMergeIfTheIdentityFieldIsNotThePrimaryKeyField()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<ThrowExceptionAtInlineMergeIfTheIdentityFieldIsNotThePrimaryKeyFieldClass>();

            // Act/Assert
            Assert.Throws<InvalidOperationException>(() => statementBuilder.CreateInsert(queryBuilder));
        }

        private class ThrowExceptionAtInlineMergeIfTheIdentityFieldIsNotTheClassIdFieldClass : DataEntity
        {
            public int ThrowExceptionAtInlineMergeIfTheIdentityFieldIsNotTheClassIdFieldClassId { get; set; }
            [Identity]
            public string Field2 { get; set; }
            public DateTime Field3 { get; set; }
        }

        [Test]
        public void ThrowExceptionAtInlineMergeIfTheIdentityFieldIsNotTheClassIdField()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<ThrowExceptionAtInlineMergeIfTheIdentityFieldIsNotTheClassIdFieldClass>();

            // Act/Assert
            Assert.Throws<InvalidOperationException>(() => statementBuilder.CreateInsert(queryBuilder));
        }

        private class ThrowExceptionAtInlineMergeIfTheIdentityFieldIsNotTheIdFieldClass : DataEntity
        {
            public int Id { get; set; }
            [Identity]
            public string Field2 { get; set; }
            public DateTime Field3 { get; set; }
        }

        [Test]
        public void ThrowExceptionAtInlineMergeIfTheIdentityFieldIsNotTheIdField()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<ThrowExceptionAtInlineMergeIfTheIdentityFieldIsNotTheIdFieldClass>();

            // Act/Assert
            Assert.Throws<InvalidOperationException>(() => statementBuilder.CreateInsert(queryBuilder));
        }

        [Map("ClassName")]
        private class ThrowExceptionAtInlineMergeIfTheIdentityFieldIsNotTheClassMappingIdFieldClass : DataEntity
        {
            public int ClassNameId { get; set; }
            [Identity]
            public string Field2 { get; set; }
            public DateTime Field3 { get; set; }
        }

        [Test]
        public void ThrowExceptionAtInlineMergeIfTheIdentityFieldIsNotTheClassMappingIdField()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<ThrowExceptionAtInlineMergeIfTheIdentityFieldIsNotTheClassMappingIdFieldClass>();

            // Act/Assert
            Assert.Throws<InvalidOperationException>(() => statementBuilder.CreateInsert(queryBuilder));
        }
    }
}
