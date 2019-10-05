using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Exceptions;
using RepoDb.StatementBuilders;
using System;

namespace RepoDb.UnitTests.StatementBuilders
{
    [TestClass]
    public class SqlStatementBuilderCreateMergeTest
    {
        [TestMethod]
        public void TestSqlStatementBuilderCreateMerge()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder();
            var tableName = "Table";
            var fields = Field.From("Field1", "Field2", "Field3");
            var qualifiers = Field.From("Field1");

            // Act
            var actual = statementBuilder.CreateMerge(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields,
                qualifiers: qualifiers,
                primaryField: null,
                identityField: null);
            var expected = $"" +
                $"MERGE [Table] AS T " +
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

        [TestMethod]
        public void TestSqlStatementBuilderCreateMergeWithQuotedTableSchema()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder();
            var tableName = "[dbo].[Table]";
            var fields = Field.From("Field1", "Field2", "Field3");
            var qualifiers = Field.From("Field1");

            // Act
            var actual = statementBuilder.CreateMerge(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields,
                qualifiers: qualifiers,
                primaryField: null,
                identityField: null);
            var expected = $"" +
                $"MERGE [dbo].[Table] AS T " +
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

        [TestMethod]
        public void TestSqlStatementBuilderCreateMergeWithUnquotedTableSchema()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder();
            var tableName = "dbo.Table";
            var fields = Field.From("Field1", "Field2", "Field3");
            var qualifiers = Field.From("Field1");

            // Act
            var actual = statementBuilder.CreateMerge(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields,
                qualifiers: qualifiers,
                primaryField: null,
                identityField: null);
            var expected = $"" +
                $"MERGE [dbo].[Table] AS T " +
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

        [TestMethod]
        public void TestSqlStatementBuilderCreateMergeWithCoveredPrimary()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder();
            var tableName = "Table";
            var fields = Field.From("Field1", "Field2", "Field3");
            var qualifiers = Field.From("Field1");
            var primaryField = new DbField("Field1", true, false, false, typeof(int), null, null, null);

            // Act
            var actual = statementBuilder.CreateMerge(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields,
                qualifiers: qualifiers,
                primaryField: primaryField,
                identityField: null);
            var expected = $"" +
                $"MERGE [Table] AS T " +
                $"USING ( SELECT @Field1 AS [Field1], @Field2 AS [Field2], @Field3 AS [Field3] ) " +
                $"AS S ON ( S.[Field1] = T.[Field1] ) " +
                $"WHEN NOT MATCHED THEN " +
                $"INSERT ( [Field1], [Field2], [Field3] ) " +
                $"VALUES ( S.[Field1], S.[Field2], S.[Field3] ) " +
                $"WHEN MATCHED THEN " +
                $"UPDATE SET [Field2] = S.[Field2], [Field3] = S.[Field3] " +
                $"OUTPUT INSERTED.[Field1] AS [Result] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestSqlStatementBuilderCreateMergeWithCoveredPrimaryAsIdentity()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder();
            var tableName = "Table";
            var fields = Field.From("Field1", "Field2", "Field3");
            var qualifiers = Field.From("Field1");
            var primaryField = new DbField("Field1", true, true, false, typeof(int), null, null, null);
            var identifyField = new DbField("Field1", true, true, false, typeof(int), null, null, null);

            // Act
            var actual = statementBuilder.CreateMerge(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields,
                qualifiers: qualifiers,
                primaryField: primaryField,
                identityField: primaryField);
            var expected = $"" +
                $"MERGE [Table] AS T " +
                $"USING ( SELECT @Field1 AS [Field1], @Field2 AS [Field2], @Field3 AS [Field3] ) " +
                $"AS S ON ( S.[Field1] = T.[Field1] ) " +
                $"WHEN NOT MATCHED THEN " +
                $"INSERT ( [Field2], [Field3] ) " +
                $"VALUES ( S.[Field2], S.[Field3] ) " +
                $"WHEN MATCHED THEN " +
                $"UPDATE SET [Field2] = S.[Field2], [Field3] = S.[Field3] " +
                $"OUTPUT INSERTED.[Field1] AS [Result] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestSqlStatementBuilderCreateMergeWithUncoveredPrimary()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder();
            var tableName = "Table";
            var fields = Field.From("Field1", "Field2", "Field3");
            var qualifiers = Field.From("Field1");
            var primaryField = new DbField("Id", true, true, false, typeof(int), null, null, null);

            // Act
            var actual = statementBuilder.CreateMerge(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields,
                qualifiers: qualifiers,
                primaryField: primaryField,
                identityField: null);
            var expected = $"" +
                $"MERGE [Table] AS T " +
                $"USING ( SELECT @Field1 AS [Field1], @Field2 AS [Field2], @Field3 AS [Field3] ) " +
                $"AS S ON ( S.[Field1] = T.[Field1] ) " +
                $"WHEN NOT MATCHED THEN " +
                $"INSERT ( [Field1], [Field2], [Field3] ) " +
                $"VALUES ( S.[Field1], S.[Field2], S.[Field3] ) " +
                $"WHEN MATCHED THEN " +
                $"UPDATE SET [Field1] = S.[Field1], [Field2] = S.[Field2], [Field3] = S.[Field3] " +
                $"OUTPUT INSERTED.[Id] AS [Result] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestSqlStatementBuilderCreateMergeWithCoveredIdentity()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder();
            var tableName = "Table";
            var fields = Field.From("Field1", "Field2", "Field3");
            var qualifiers = Field.From("Field1");
            var identityField = new DbField("Field1", false, true, false, typeof(int), null, null, null);

            // Act
            var actual = statementBuilder.CreateMerge(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields,
                qualifiers: qualifiers,
                primaryField: null,
                identityField: identityField);
            var expected = $"" +
                $"MERGE [Table] AS T " +
                $"USING ( SELECT @Field1 AS [Field1], @Field2 AS [Field2], @Field3 AS [Field3] ) " +
                $"AS S ON ( S.[Field1] = T.[Field1] ) " +
                $"WHEN NOT MATCHED THEN " +
                $"INSERT ( [Field2], [Field3] ) " +
                $"VALUES ( S.[Field2], S.[Field3] ) " +
                $"WHEN MATCHED THEN " +
                $"UPDATE SET [Field2] = S.[Field2], [Field3] = S.[Field3] " +
                $"OUTPUT INSERTED.[Field1] AS [Result] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestSqlStatementBuilderCreateMergeWithUncoveredIdentity()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder();
            var tableName = "Table";
            var fields = Field.From("Field1", "Field2", "Field3");
            var qualifiers = Field.From("Field1");
            var identityField = new DbField("Id", false, true, false, typeof(int), null, null, null);

            // Act
            var actual = statementBuilder.CreateMerge(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields,
                qualifiers: qualifiers,
                primaryField: null,
                identityField: identityField);
            var expected = $"" +
                $"MERGE [Table] AS T " +
                $"USING ( SELECT @Field1 AS [Field1], @Field2 AS [Field2], @Field3 AS [Field3] ) " +
                $"AS S ON ( S.[Field1] = T.[Field1] ) " +
                $"WHEN NOT MATCHED THEN " +
                $"INSERT ( [Field1], [Field2], [Field3] ) " +
                $"VALUES ( S.[Field1], S.[Field2], S.[Field3] ) " +
                $"WHEN MATCHED THEN " +
                $"UPDATE SET [Field1] = S.[Field1], [Field2] = S.[Field2], [Field3] = S.[Field3] " +
                $"OUTPUT INSERTED.[Id] AS [Result] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestSqlStatementBuilderCreateMergeWithCoveredPrimaryButWithoutQualifiers()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder();
            var tableName = "Table";
            var fields = Field.From("Field1", "Field2", "Field3");
            var primaryField = new DbField("Field1", true, true, false, typeof(int), null, null, null);

            // Act
            var actual = statementBuilder.CreateMerge(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields,
                qualifiers: null,
                primaryField: primaryField,
                identityField: null);
            var expected = $"" +
                $"MERGE [Table] AS T " +
                $"USING ( SELECT @Field1 AS [Field1], @Field2 AS [Field2], @Field3 AS [Field3] ) " +
                $"AS S ON ( S.[Field1] = T.[Field1] ) " +
                $"WHEN NOT MATCHED THEN " +
                $"INSERT ( [Field1], [Field2], [Field3] ) " +
                $"VALUES ( S.[Field1], S.[Field2], S.[Field3] ) " +
                $"WHEN MATCHED THEN " +
                $"UPDATE SET [Field2] = S.[Field2], [Field3] = S.[Field3] " +
                $"OUTPUT INSERTED.[Field1] AS [Result] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestSqlStatementBuilderCreateMergeWithCoveredPrimaryAndWithCoveredIdentityButWithoutQualifiers()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder();
            var tableName = "Table";
            var fields = Field.From("Field1", "Field2", "Field3");
            var primaryField = new DbField("Field1", true, false, false, typeof(int), null, null, null);
            var identityField = new DbField("Field2", false, true, false, typeof(int), null, null, null);

            // Act
            var actual = statementBuilder.CreateMerge(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields,
                qualifiers: null,
                primaryField: primaryField,
                identityField: identityField);
            var expected = $"" +
                $"MERGE [Table] AS T " +
                $"USING ( SELECT @Field1 AS [Field1], @Field2 AS [Field2], @Field3 AS [Field3] ) " +
                $"AS S ON ( S.[Field1] = T.[Field1] ) " +
                $"WHEN NOT MATCHED THEN " +
                $"INSERT ( [Field1], [Field3] ) " +
                $"VALUES ( S.[Field1], S.[Field3] ) " +
                $"WHEN MATCHED THEN " +
                $"UPDATE SET [Field3] = S.[Field3] " +
                $"OUTPUT INSERTED.[Field2] AS [Result] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnSqlStatementBuilderCreateMergeIfThereAreNoFields()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder();
            var tableName = "Table";
            var qualifiers = Field.From("Id");

            // Act
            statementBuilder.CreateMerge(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: null,
                qualifiers: qualifiers,
                primaryField: null,
                identityField: null);
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnSqlStatementBuilderCreateMergeIfThereAreNoPrimaryAndNoQualifiers()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder();
            var tableName = "Table";
            var fields = Field.From("Field1", "Field2", "Field3");

            // Act
            statementBuilder.CreateMerge(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields,
                qualifiers: null,
                primaryField: null,
                identityField: null);
        }

        [TestMethod, ExpectedException(typeof(InvalidQualifierFieldsException))]
        public void ThrowExceptionOnSqlStatementBuilderCreateMergeIfTheQualifiersAreNotPresentAtTheGivenFields()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder();
            var tableName = "Table";
            var fields = Field.From("Field1", "Field2", "Field3");
            var qualifiers = Field.From("Id");

            // Act
            statementBuilder.CreateMerge(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields,
                qualifiers: qualifiers,
                primaryField: null,
                identityField: null);
        }

        [TestMethod, ExpectedException(typeof(InvalidQualifierFieldsException))]
        public void ThrowExceptionOnSqlStatementBuilderCreateMergeIfThePrimaryAsQualifierIsNotPresentAtTheGivenFields()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder();
            var tableName = "Table";
            var fields = Field.From("Field1", "Field2", "Field3");
            var primaryField = new DbField("Id", true, false, false, typeof(int), null, null, null);

            // Act
            statementBuilder.CreateMerge(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields,
                qualifiers: null,
                primaryField: primaryField,
                identityField: null);
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnSqlStatementBuilderCreateMergeIfTheTableIsNull()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder();
            var tableName = (string)null;
            var fields = Field.From("Field1", "Field2", "Field3");
            var qualifiers = Field.From("Field1");

            // Act
            statementBuilder.CreateMerge(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields,
                qualifiers: qualifiers,
                primaryField: null,
                identityField: null);
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnSqlStatementBuilderCreateMergeIfTheTableIsEmpty()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder();
            var tableName = "";
            var fields = Field.From("Field1", "Field2", "Field3");
            var qualifiers = Field.From("Field1");

            // Act
            statementBuilder.CreateMerge(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields,
                qualifiers: qualifiers,
                primaryField: null,
                identityField: null);
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnSqlStatementBuilderCreateMergeIfTheTableIsWhitespace()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder();
            var tableName = " ";
            var fields = Field.From("Field1", "Field2", "Field3");
            var qualifiers = Field.From("Field1");

            // Act
            statementBuilder.CreateMerge(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields,
                qualifiers: qualifiers,
                primaryField: null,
                identityField: null);
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void ThrowExceptionOnSqlStatementBuilderCreateMergeIfThePrimaryIsNotReallyAPrimary()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder();
            var tableName = "Table";
            var fields = Field.From("Field1", "Field2", "Field3");
            var primaryField = new DbField("Field1", false, false, false, typeof(int), null, null, null);

            // Act
            statementBuilder.CreateMerge(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields,
                qualifiers: null,
                primaryField: primaryField,
                identityField: null);
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void ThrowExceptionOnSqlStatementBuilderCreateMergeIfTheIdentityIsNotReallyAnIdentity()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder();
            var tableName = "Table";
            var fields = Field.From("Field1", "Field2", "Field3");
            var qualifiers = Field.From("Field1");
            var identifyField = new DbField("Field2", false, false, false, typeof(int), null, null, null);

            // Act
            statementBuilder.CreateMerge(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields,
                qualifiers: null,
                primaryField: null,
                identityField: identifyField);
        }
    }
}
