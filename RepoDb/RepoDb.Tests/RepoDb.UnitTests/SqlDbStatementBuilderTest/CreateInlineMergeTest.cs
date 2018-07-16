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
        private class TestCreateInlineMergeWithoutMappingsClass : DataEntity
        {
            public int Field1 { get; set; }
            public string Field2 { get; set; }
            public DateTime Field3 { get; set; }
        }

        [Test]
        public void TestCreateInlineMergeWithoutMappings()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<TestCreateInlineMergeWithoutMappingsClass>();
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });
            var qualifiers = Field.From(new[] { "Field1", "Field2" });

            // Act
            var actual = statementBuilder.CreateInlineMerge(queryBuilder, fields, qualifiers);
            var expected = $"" +
                $"MERGE [TestCreateInlineMergeWithoutMappingsClass] AS T " +
                $"USING ( SELECT @Field1 AS [Field1], @Field2 AS [Field2], @Field3 AS [Field3] ) " +
                $"AS S ON ( S.[Field1] = T.[Field1] AND S.[Field2] = T.[Field2] ) " +
                $"WHEN NOT MATCHED THEN " +
                $"INSERT ( [Field1], [Field2], [Field3] ) " +
                $"VALUES ( @Field1, @Field2, @Field3 ) " +
                $"WHEN MATCHED THEN " +
                $"UPDATE SET [Field1] = S.[Field1], [Field2] = S.[Field2], [Field3] = S.[Field3] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [Map("ClassName")]
        private class TestCreateInlineMergeWithClassMappingClass : DataEntity
        {
            public int Field1 { get; set; }
            public string Field2 { get; set; }
            public DateTime Field3 { get; set; }
        }

        [Test]
        public void TestCreateInlineMergeWithClassMapping()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<TestCreateInlineMergeWithClassMappingClass>();
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });
            var qualifiers = Field.From(new[] { "Field1", "Field2" });

            // Act
            var actual = statementBuilder.CreateInlineMerge(queryBuilder, fields, qualifiers);
            var expected = $"" +
                $"MERGE [ClassName] AS T " +
                $"USING ( SELECT @Field1 AS [Field1], @Field2 AS [Field2], @Field3 AS [Field3] ) " +
                $"AS S ON ( S.[Field1] = T.[Field1] AND S.[Field2] = T.[Field2] ) " +
                $"WHEN NOT MATCHED THEN " +
                $"INSERT ( [Field1], [Field2], [Field3] ) " +
                $"VALUES ( @Field1, @Field2, @Field3 ) " +
                $"WHEN MATCHED THEN " +
                $"UPDATE SET [Field1] = S.[Field1], [Field2] = S.[Field2], [Field3] = S.[Field3] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        private class TestCreateInlineMergeWithAttributeMappingsClass : DataEntity
        {
            public int Field1 { get; set; }
            public string Field2 { get; set; }
            [Map("Field4")]
            public DateTime Field3 { get; set; }
        }

        [Test]
        public void TestCreateInlineMergeWithAttributeMappings()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<TestCreateInlineMergeWithAttributeMappingsClass>();
            var fields = Field.From(new[] { "Field1", "Field2", "Field4" });
            var qualifiers = Field.From(new[] { "Field1", "Field2" });

            // Act
            var actual = statementBuilder.CreateInlineMerge(queryBuilder, fields, qualifiers);
            var expected = $"" +
                $"MERGE [TestCreateInlineMergeWithAttributeMappingsClass] AS T " +
                $"USING ( SELECT @Field1 AS [Field1], @Field2 AS [Field2], @Field4 AS [Field4] ) " +
                $"AS S ON ( S.[Field1] = T.[Field1] AND S.[Field2] = T.[Field2] ) " +
                $"WHEN NOT MATCHED THEN " +
                $"INSERT ( [Field1], [Field2], [Field4] ) " +
                $"VALUES ( @Field1, @Field2, @Field4 ) " +
                $"WHEN MATCHED THEN " +
                $"UPDATE SET [Field1] = S.[Field1], [Field2] = S.[Field2], [Field4] = S.[Field4] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        private class TestCreateInlineMergeWithIgnoreInsertFieldClass : DataEntity
        {
            public int Field1 { get; set; }
            public string Field2 { get; set; }
            [Attributes.Ignore(Command.Insert)]
            public DateTime Field3 { get; set; }
        }

        [Test]
        public void TestCreateInlineMergeWithIgnoreInsertField()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<TestCreateInlineMergeWithIgnoreInsertFieldClass>();
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });
            var qualifiers = Field.From(new[] { "Field1", "Field2" });

            // Act
            var actual = statementBuilder.CreateInlineMerge(queryBuilder, fields, qualifiers);
            var expected = $"" +
                $"MERGE [TestCreateInlineMergeWithIgnoreInsertFieldClass] AS T " +
                $"USING ( SELECT @Field1 AS [Field1], @Field2 AS [Field2], @Field3 AS [Field3] ) " +
                $"AS S ON ( S.[Field1] = T.[Field1] AND S.[Field2] = T.[Field2] ) " +
                $"WHEN NOT MATCHED THEN " +
                $"INSERT ( [Field1], [Field2] ) " +
                $"VALUES ( @Field1, @Field2 ) " +
                $"WHEN MATCHED THEN " +
                $"UPDATE SET [Field1] = S.[Field1], [Field2] = S.[Field2], [Field3] = S.[Field3] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        private class TestCreateInlineMergeWithIgnoreUpdateFieldClass : DataEntity
        {
            public int Field1 { get; set; }
            public string Field2 { get; set; }
            [Attributes.Ignore(Command.Update)]
            public DateTime Field3 { get; set; }
        }

        [Test]
        public void TestCreateInlineMergeWithIgnoreUpdateField()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<TestCreateInlineMergeWithIgnoreUpdateFieldClass>();
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });
            var qualifiers = Field.From(new[] { "Field1", "Field2" });

            // Act
            var actual = statementBuilder.CreateInlineMerge(queryBuilder, fields, qualifiers);
            var expected = $"" +
                $"MERGE [TestCreateInlineMergeWithIgnoreUpdateFieldClass] AS T " +
                $"USING ( SELECT @Field1 AS [Field1], @Field2 AS [Field2], @Field3 AS [Field3] ) " +
                $"AS S ON ( S.[Field1] = T.[Field1] AND S.[Field2] = T.[Field2] ) " +
                $"WHEN NOT MATCHED THEN " +
                $"INSERT ( [Field1], [Field2], [Field3] ) " +
                $"VALUES ( @Field1, @Field2, @Field3 ) " +
                $"WHEN MATCHED THEN " +
                $"UPDATE SET [Field1] = S.[Field1], [Field2] = S.[Field2] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        private class TestCreateInlineMergeWithPrimaryKeyClass : DataEntity
        {
            [Primary]
            public int Field1 { get; set; }
            public string Field2 { get; set; }
            public DateTime Field3 { get; set; }
        }

        [Test]
        public void TestCreateInlineMergeWithPrimaryKey()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<TestCreateInlineMergeWithPrimaryKeyClass>();
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });
            var qualifiers = (IEnumerable<Field>)null;

            // Act
            var actual = statementBuilder.CreateInlineMerge(queryBuilder, fields, qualifiers);
            var expected = $"" +
                $"MERGE [TestCreateInlineMergeWithPrimaryKeyClass] AS T " +
                $"USING ( SELECT @Field1 AS [Field1], @Field2 AS [Field2], @Field3 AS [Field3] ) " +
                $"AS S ON ( S.[Field1] = T.[Field1] ) " +
                $"WHEN NOT MATCHED THEN " +
                $"INSERT ( [Field1], [Field2], [Field3] ) " +
                $"VALUES ( @Field1, @Field2, @Field3 ) " +
                $"WHEN MATCHED THEN " +
                $"UPDATE SET [Field2] = S.[Field2], [Field3] = S.[Field3] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        private class TestCreateInlineMergeWithOverrideIgnoreFromInlineMergeClass : DataEntity
        {
            public int Field1 { get; set; }
            public string Field2 { get; set; }
            [Attributes.Ignore(Command.InlineMerge)]
            public DateTime Field3 { get; set; }
        }

        [Test]
        public void TestCreateInlineMergeWithOverrideIgnoreFromInlineMerge()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<TestCreateInlineMergeWithOverrideIgnoreFromInlineMergeClass>();
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });
            var qualifiers = Field.From(new[] { "Field1" });

            // Act
            var actual = statementBuilder.CreateInlineMerge(queryBuilder, fields, qualifiers, true);
            var expected = $"" +
                $"MERGE [TestCreateInlineMergeWithOverrideIgnoreFromInlineMergeClass] AS T " +
                $"USING ( SELECT @Field1 AS [Field1], @Field2 AS [Field2], @Field3 AS [Field3] ) " +
                $"AS S ON ( S.[Field1] = T.[Field1] ) " +
                $"WHEN NOT MATCHED THEN " +
                $"INSERT ( [Field1], [Field2], [Field3] ) " +
                $"VALUES ( @Field1, @Field2, @Field3 ) " +
                $"WHEN MATCHED THEN " +
                $"UPDATE SET [Field1] = S.[Field1], [Field2] = S.[Field2], [Field3] = S.[Field3] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        private class TestCreateInlineMergeWithOverrideIgnoreFromMergeClass : DataEntity
        {
            public int Field1 { get; set; }
            public string Field2 { get; set; }
            [Attributes.Ignore(Command.Merge)]
            public DateTime Field3 { get; set; }
        }

        [Test]
        public void TestCreateInlineMergeWithOverrideIgnoreFromMerge()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<TestCreateInlineMergeWithOverrideIgnoreFromMergeClass>();
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });
            var qualifiers = Field.From(new[] { "Field1" });

            // Act
            var actual = statementBuilder.CreateInlineMerge(queryBuilder, fields, qualifiers, true);
            var expected = $"" +
                $"MERGE [TestCreateInlineMergeWithOverrideIgnoreFromMergeClass] AS T " +
                $"USING ( SELECT @Field1 AS [Field1], @Field2 AS [Field2], @Field3 AS [Field3] ) " +
                $"AS S ON ( S.[Field1] = T.[Field1] ) " +
                $"WHEN NOT MATCHED THEN " +
                $"INSERT ( [Field1], [Field2], [Field3] ) " +
                $"VALUES ( @Field1, @Field2, @Field3 ) " +
                $"WHEN MATCHED THEN " +
                $"UPDATE SET [Field1] = S.[Field1], [Field2] = S.[Field2], [Field3] = S.[Field3] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        private class TestCreateInlineMergeWithOverrideIgnoreFromInsertClass : DataEntity
        {
            public int Field1 { get; set; }
            public string Field2 { get; set; }
            [Attributes.Ignore(Command.Insert)]
            public DateTime Field3 { get; set; }
        }

        [Test]
        public void TestCreateInlineMergeWithOverrideIgnoreFromInsert()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<TestCreateInlineMergeWithOverrideIgnoreFromInsertClass>();
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });
            var qualifiers = Field.From(new[] { "Field1" });

            // Act
            var actual = statementBuilder.CreateInlineMerge(queryBuilder, fields, qualifiers, true);
            var expected = $"" +
                $"MERGE [TestCreateInlineMergeWithOverrideIgnoreFromInsertClass] AS T " +
                $"USING ( SELECT @Field1 AS [Field1], @Field2 AS [Field2], @Field3 AS [Field3] ) " +
                $"AS S ON ( S.[Field1] = T.[Field1] ) " +
                $"WHEN NOT MATCHED THEN " +
                $"INSERT ( [Field1], [Field2], [Field3] ) " +
                $"VALUES ( @Field1, @Field2, @Field3 ) " +
                $"WHEN MATCHED THEN " +
                $"UPDATE SET [Field1] = S.[Field1], [Field2] = S.[Field2], [Field3] = S.[Field3] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        private class TestCreateInlineMergeWithOverrideIgnoreFromUpdateClass : DataEntity
        {
            public int Field1 { get; set; }
            public string Field2 { get; set; }
            [Attributes.Ignore(Command.Update)]
            public DateTime Field3 { get; set; }
        }

        [Test]
        public void TestCreateInlineMergeWithOverrideIgnoreFromUpdate()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<TestCreateInlineMergeWithOverrideIgnoreFromUpdateClass>();
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });
            var qualifiers = Field.From(new[] { "Field1" });

            // Act
            var actual = statementBuilder.CreateInlineMerge(queryBuilder, fields, qualifiers, true);
            var expected = $"" +
                $"MERGE [TestCreateInlineMergeWithOverrideIgnoreFromUpdateClass] AS T " +
                $"USING ( SELECT @Field1 AS [Field1], @Field2 AS [Field2], @Field3 AS [Field3] ) " +
                $"AS S ON ( S.[Field1] = T.[Field1] ) " +
                $"WHEN NOT MATCHED THEN " +
                $"INSERT ( [Field1], [Field2], [Field3] ) " +
                $"VALUES ( @Field1, @Field2, @Field3 ) " +
                $"WHEN MATCHED THEN " +
                $"UPDATE SET [Field1] = S.[Field1], [Field2] = S.[Field2], [Field3] = S.[Field3] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        private class ThrowExceptionAtCreateInlineMergeIfTheFieldsAreNullClass : DataEntity
        {
            public int Field1 { get; set; }
        }

        [Test]
        public void ThrowExceptionAtCreateInlineMergeIfTheFieldsAreNull()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<ThrowExceptionAtCreateInlineMergeIfTheFieldsAreNullClass>();
            var fields = (IEnumerable<Field>)null;
            var qualifiers = Field.From(new[] { "Field1" });

            // Act/Assert
            Assert.Throws<NullReferenceException>(() => statementBuilder.CreateInlineMerge(queryBuilder, fields, qualifiers));
        }

        private class ThrowExceptionAtCreateInlineMergeIfTheQualifiersAreNullWithoutPrimaryKeyClass : DataEntity
        {
            public int Field1 { get; set; }
        }

        [Test]
        public void ThrowExceptionAtCreateInlineMergeIfTheQualifiersAreNullWithoutPrimaryKey()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<ThrowExceptionAtCreateInlineMergeIfTheQualifiersAreNullWithoutPrimaryKeyClass>();
            var fields = Field.From(new[] { "Field1" });
            var qualifiers = (IEnumerable<Field>)null;

            // Act/Assert
            Assert.Throws<NullReferenceException>(() => statementBuilder.CreateInlineMerge(queryBuilder, fields, qualifiers));
        }

        private class ThrowExceptionAtCreateInlineMergeIfAQualifierFieldIsNotPresentAtDataEntityClass : DataEntity
        {
            public int Field1 { get; set; }
            public string Field2 { get; set; }
            public DateTime Field3 { get; set; }
        }

        [Test]
        public void ThrowExceptionAtCreateInlineMergeIfAQualifierFieldIsNotPresentAtDataEntity()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<ThrowExceptionAtCreateInlineMergeIfAQualifierFieldIsNotPresentAtDataEntityClass>();
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });
            var qualifiers = Field.From(new[] { "Field1", "Field4" });

            // Act/Assert
            Assert.Throws<InvalidOperationException>(() => statementBuilder.CreateInlineMerge(queryBuilder, fields, qualifiers));
        }

        private class ThrowExceptionAtCreateInlineMergeIfAQualifierFieldIsNotPresentAtDataEntityFieldMappingClass : DataEntity
        {
            public int Field1 { get; set; }
            public string Field2 { get; set; }
            [Map("Field4")]
            public DateTime Field3 { get; set; }
        }

        [Test]
        public void ThrowExceptionAtCreateInlineMergeIfAQualifierFieldIsNotPresentAtDataEntityFieldMapping()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<ThrowExceptionAtCreateInlineMergeIfAQualifierFieldIsNotPresentAtDataEntityFieldMappingClass>();
            var fields = Field.From(new[] { "Field1", "Field2", "Field4" });
            var qualifiers = Field.From(new[] { "Field1", "Field3" });

            // Act/Assert
            Assert.Throws<InvalidOperationException>(() => statementBuilder.CreateInlineMerge(queryBuilder, fields, qualifiers));
        }

        private class ThrowExceptionAtCreateInlineMergeIfTheFieldsContainsAnIgnoreInlineMergeFieldsClass : DataEntity
        {
            public int Field1 { get; set; }
            public string Field2 { get; set; }
            [Attributes.Ignore(Command.InlineMerge)]
            public DateTime Field3 { get; set; }
        }

        [Test]
        public void ThrowExceptionAtCreateInlineMergeIfTheFieldsContainsAnIgnoreInlineMergeFields()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<ThrowExceptionAtCreateInlineMergeIfTheFieldsContainsAnIgnoreInlineMergeFieldsClass>();
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });
            var qualifiers = Field.From(new[] { "Field1", "Field2" });

            // Act/Assert
            Assert.Throws<InvalidOperationException>(() => statementBuilder.CreateInlineMerge(queryBuilder, fields, qualifiers));
        }

        private class ThrowExceptionAtCreateInlineMergeIfTheQualifiersContainsAnIgnoreInlineMergeFieldsClass : DataEntity
        {
            public int Field1 { get; set; }
            public string Field2 { get; set; }
            [Attributes.Ignore(Command.InlineMerge)]
            public DateTime Field3 { get; set; }
            public double Field4 { get; set; }
        }

        [Test]
        public void ThrowExceptionAtCreateInlineMergeIfTheQualifiersContainsAnIgnoreInlineMergeFields()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<ThrowExceptionAtCreateInlineMergeIfTheQualifiersContainsAnIgnoreInlineMergeFieldsClass>();
            var fields = Field.From(new[] { "Field1", "Field2", "Field4" });
            var qualifiers = Field.From(new[] { "Field1", "Field3" });

            // Act/Assert
            Assert.Throws<InvalidOperationException>(() => statementBuilder.CreateInlineMerge(queryBuilder, fields, qualifiers));
        }

        private class ThrowExceptionAtCreateInlineMergeIfFieldIsIgnoreInlineMergeClass : DataEntity
        {
            public int Field1 { get; set; }
            public string Field2 { get; set; }
            [Attributes.Ignore(Command.InlineMerge)]
            public DateTime Field3 { get; set; }
        }

        [Test]
        public void ThrowExceptionAtCreateInlineMergeIfFieldIsIgnoreInlineMerge()
        {
            // Setup
            var statementBuilder = new SqlDbStatementBuilder();
            var queryBuilder = new QueryBuilder<ThrowExceptionAtCreateInlineMergeIfFieldIsIgnoreInlineMergeClass>();
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });
            var qualifiers = Field.From(new[] { "Field1", "Field2" });

            // Act/Assert
            Assert.Throws<InvalidOperationException>(() => statementBuilder.CreateInlineMerge(queryBuilder, fields, qualifiers));
        }
    }
}
