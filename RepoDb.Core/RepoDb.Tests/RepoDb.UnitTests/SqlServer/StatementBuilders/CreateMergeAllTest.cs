using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Exceptions;
using RepoDb.StatementBuilders;
using RepoDb.UnitTests.Setup;
using System;

namespace RepoDb.UnitTests.StatementBuilders
{
    [TestClass]
    public class SqlStatementBuilderCreateMergeAllTest
    {
        [TestMethod]
        public void TestSqlStatementBuilderCreateMergeAll()
        {
            // Setup
            var statementBuilder = Helper.StatementBuilder;
            var queryBuilder = new QueryBuilder();
            var tableName = "Table";
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });
            var qualifiers = Field.From("Field1");

            // Act
            var actual = statementBuilder.CreateMergeAll(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields,
                qualifiers: qualifiers,
                batchSize: 1,
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
        public void TestSqlStatementBuilderCreateMergeAllWithQuotedTableSchema()
        {
            // Setup
            var statementBuilder = Helper.StatementBuilder;
            var queryBuilder = new QueryBuilder();
            var tableName = "[dbo].[Table]";
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });
            var qualifiers = Field.From("Field1");

            // Act
            var actual = statementBuilder.CreateMergeAll(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields,
                qualifiers: qualifiers,
                batchSize: 1,
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
        public void TestSqlStatementBuilderCreateMergeAllWithUnquotedTableSchema()
        {
            // Setup
            var statementBuilder = Helper.StatementBuilder;
            var queryBuilder = new QueryBuilder();
            var tableName = "dbo.Table";
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });
            var qualifiers = Field.From("Field1");

            // Act
            var actual = statementBuilder.CreateMergeAll(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields,
                qualifiers: qualifiers,
                batchSize: 1,
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
        public void TestSqlStatementBuilderCreateMergeAllWithCoveredPrimary()
        {
            // Setup
            var statementBuilder = Helper.StatementBuilder;
            var queryBuilder = new QueryBuilder();
            var tableName = "Table";
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });
            var qualifiers = Field.From("Field1");
            var primaryField = new DbField("Field1", true, false, false, typeof(int), null, null, null, null);

            // Act
            var actual = statementBuilder.CreateMergeAll(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields,
                qualifiers: qualifiers,
                batchSize: 1,
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
        public void TestSqlStatementBuilderCreateMergeAllWithCoveredPrimaryAsIdentity()
        {
            // Setup
            var statementBuilder = Helper.StatementBuilder;
            var queryBuilder = new QueryBuilder();
            var tableName = "Table";
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });
            var qualifiers = Field.From("Field1");
            var primaryField = new DbField("Field1", true, true, false, typeof(int), null, null, null, null);
            var identifyField = new DbField("Field1", true, true, false, typeof(int), null, null, null, null);

            // Act
            var actual = statementBuilder.CreateMergeAll(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields,
                qualifiers: qualifiers,
                batchSize: 1,
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
        public void TestSqlStatementBuilderCreateMergeAllWithUncoveredPrimary()
        {
            // Setup
            var statementBuilder = Helper.StatementBuilder;
            var queryBuilder = new QueryBuilder();
            var tableName = "Table";
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });
            var qualifiers = Field.From("Field1");
            var primaryField = new DbField("Id", true, true, false, typeof(int), null, null, null, null);

            // Act
            var actual = statementBuilder.CreateMergeAll(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields,
                qualifiers: qualifiers,
                batchSize: 1,
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
        public void TestSqlStatementBuilderCreateMergeAllWithCoveredIdentity()
        {
            // Setup
            var statementBuilder = Helper.StatementBuilder;
            var queryBuilder = new QueryBuilder();
            var tableName = "Table";
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });
            var qualifiers = Field.From("Field1");
            var identityField = new DbField("Field1", false, true, false, typeof(int), null, null, null, null);

            // Act
            var actual = statementBuilder.CreateMergeAll(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields,
                qualifiers: qualifiers,
                batchSize: 1,
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
        public void TestSqlStatementBuilderCreateMergeAllWithUncoveredIdentity()
        {
            // Setup
            var statementBuilder = Helper.StatementBuilder;
            var queryBuilder = new QueryBuilder();
            var tableName = "Table";
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });
            var qualifiers = Field.From("Field1");
            var identityField = new DbField("Id", false, true, false, typeof(int), null, null, null, null);

            // Act
            var actual = statementBuilder.CreateMergeAll(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields,
                qualifiers: qualifiers,
                batchSize: 1,
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
        public void TestSqlStatementBuilderCreateMergeAllWithCoveredPrimaryButWithoutQualifiers()
        {
            // Setup
            var statementBuilder = Helper.StatementBuilder;
            var queryBuilder = new QueryBuilder();
            var tableName = "Table";
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });
            var primaryField = new DbField("Field1", true, true, false, typeof(int), null, null, null, null);

            // Act
            var actual = statementBuilder.CreateMergeAll(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields,
                qualifiers: null,
                batchSize: 1,
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
        public void TestSqlStatementBuilderCreateMergeAllWithCoveredPrimaryAndWithCoveredIdentityButWithoutQualifiers()
        {
            // Setup
            var statementBuilder = Helper.StatementBuilder;
            var queryBuilder = new QueryBuilder();
            var tableName = "Table";
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });
            var primaryField = new DbField("Field1", true, false, false, typeof(int), null, null, null, null);
            var identityField = new DbField("Field2", false, true, false, typeof(int), null, null, null, null);

            // Act
            var actual = statementBuilder.CreateMergeAll(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields,
                qualifiers: null,
                batchSize: 1,
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

        [TestMethod]
        public void TestSqlStatementBuilderCreateMergeAllWithIdentityForThreeBatches()
        {
            // Setup
            var statementBuilder = Helper.StatementBuilder;
            var queryBuilder = new QueryBuilder();
            var tableName = "Table";
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });
            var qualifiers = Field.From("Field1");
            var identityField = new DbField("Field1", false, true, false, typeof(int), null, null, null, null);

            // Act
            var actual = statementBuilder.CreateMergeAll(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields,
                qualifiers: qualifiers,
                batchSize: 3,
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
                $"OUTPUT INSERTED.[Field1] AS [Result] ; " +
                $"MERGE [Table] AS T " +
                $"USING ( SELECT @Field1_1 AS [Field1], @Field2_1 AS [Field2], @Field3_1 AS [Field3] ) " +
                $"AS S ON ( S.[Field1] = T.[Field1] ) " +
                $"WHEN NOT MATCHED THEN " +
                $"INSERT ( [Field2], [Field3] ) " +
                $"VALUES ( S.[Field2], S.[Field3] ) " +
                $"WHEN MATCHED THEN " +
                $"UPDATE SET [Field2] = S.[Field2], [Field3] = S.[Field3] " +
                $"OUTPUT INSERTED.[Field1] AS [Result] ; " +
                $"MERGE [Table] AS T " +
                $"USING ( SELECT @Field1_2 AS [Field1], @Field2_2 AS [Field2], @Field3_2 AS [Field3] ) " +
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

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnSqlStatementBuilderCreateMergeAllIfThereAreNoFields()
        {
            // Setup
            var statementBuilder = Helper.StatementBuilder;
            var queryBuilder = new QueryBuilder();
            var tableName = "Table";
            var qualifiers = Field.From("Id");

            // Act
            statementBuilder.CreateMergeAll(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: null,
                qualifiers: qualifiers,
                batchSize: 1,
                primaryField: null,
                identityField: null);
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnSqlStatementBuilderCreateMergeAllIfThereAreNoPrimaryAndNoQualifiers()
        {
            // Setup
            var statementBuilder = Helper.StatementBuilder;
            var queryBuilder = new QueryBuilder();
            var tableName = "Table";
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });

            // Act
            statementBuilder.CreateMergeAll(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields,
                qualifiers: null,
                batchSize: 1,
                primaryField: null,
                identityField: null);
        }

        [TestMethod, ExpectedException(typeof(InvalidQualifiersException))]
        public void ThrowExceptionOnSqlStatementBuilderCreateMergeAllIfTheQualifiersAreNotPresentAtTheGivenFields()
        {
            // Setup
            var statementBuilder = Helper.StatementBuilder;
            var queryBuilder = new QueryBuilder();
            var tableName = "Table";
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });
            var qualifiers = Field.From("Id");

            // Act
            statementBuilder.CreateMergeAll(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields,
                qualifiers: qualifiers,
                batchSize: 1,
                primaryField: null,
                identityField: null);
        }

        [TestMethod, ExpectedException(typeof(InvalidQualifiersException))]
        public void ThrowExceptionOnSqlStatementBuilderCreateMergeAllIfThePrimaryAsQualifierIsNotPresentAtTheGivenFields()
        {
            // Setup
            var statementBuilder = Helper.StatementBuilder;
            var queryBuilder = new QueryBuilder();
            var tableName = "Table";
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });
            var primaryField = new DbField("Id", true, false, false, typeof(int), null, null, null, null);

            // Act
            statementBuilder.CreateMergeAll(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields,
                qualifiers: null,
                batchSize: 1,
                primaryField: primaryField,
                identityField: null);
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnSqlStatementBuilderCreateMergeAllIfTheTableIsNull()
        {
            // Setup
            var statementBuilder = Helper.StatementBuilder;
            var queryBuilder = new QueryBuilder();
            var tableName = (string)null;
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });
            var qualifiers = Field.From("Field1");

            // Act
            statementBuilder.CreateMergeAll(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields,
                qualifiers: qualifiers,
                batchSize: 1,
                primaryField: null,
                identityField: null);
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnSqlStatementBuilderCreateMergeAllIfTheTableIsEmpty()
        {
            // Setup
            var statementBuilder = Helper.StatementBuilder;
            var queryBuilder = new QueryBuilder();
            var tableName = "";
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });
            var qualifiers = Field.From("Field1");

            // Act
            statementBuilder.CreateMergeAll(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields,
                qualifiers: qualifiers,
                batchSize: 1,
                primaryField: null,
                identityField: null);
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnSqlStatementBuilderCreateMergeAllIfTheTableIsWhitespace()
        {
            // Setup
            var statementBuilder = Helper.StatementBuilder;
            var queryBuilder = new QueryBuilder();
            var tableName = " ";
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });
            var qualifiers = Field.From("Field1");

            // Act
            statementBuilder.CreateMergeAll(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields,
                qualifiers: qualifiers,
                batchSize: 1,
                primaryField: null,
                identityField: null);
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void ThrowExceptionOnSqlStatementBuilderCreateMergeAllIfThePrimaryIsNotReallyAPrimary()
        {
            // Setup
            var statementBuilder = Helper.StatementBuilder;
            var queryBuilder = new QueryBuilder();
            var tableName = "Table";
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });
            var primaryField = new DbField("Field1", false, false, false, typeof(int), null, null, null, null);

            // Act
            statementBuilder.CreateMergeAll(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields,
                qualifiers: null,
                batchSize: 1,
                primaryField: primaryField,
                identityField: null);
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void ThrowExceptionOnSqlStatementBuilderCreateMergeAllIfTheIdentityIsNotReallyAnIdentity()
        {
            // Setup
            var statementBuilder = Helper.StatementBuilder;
            var queryBuilder = new QueryBuilder();
            var tableName = "Table";
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });
            var qualifiers = Field.From("Field1");
            var identifyField = new DbField("Field2", false, false, false, typeof(int), null, null, null, null);

            // Act
            statementBuilder.CreateMergeAll(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields,
                qualifiers: null,
                batchSize: 1,
                primaryField: null,
                identityField: identifyField);
        }
    }
}
