using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Exceptions;
using System;
using System.Data.SqlClient;

namespace RepoDb.UnitTests.StatementBuilders
{
    [TestClass]
    public class SqlServerStatementBuilderCreateMergeAllTest
    {
        [TestMethod]
        public void TestSqlServerStatementBuilderCreateMergeAll()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get(typeof(SqlConnection));
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
        public void TestSqlServerStatementBuilderCreateMergeAllWithQuotedTableSchema()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get(typeof(SqlConnection));
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
        public void TestSqlServerStatementBuilderCreateMergeAllWithUnquotedTableSchema()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get(typeof(SqlConnection));
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
        public void TestSqlServerStatementBuilderCreateMergeAllWithCoveredPrimary()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get(typeof(SqlConnection));
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
        public void TestSqlServerStatementBuilderCreateMergeAllWithCoveredPrimaryAsIdentity()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get(typeof(SqlConnection));
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
        public void TestSqlServerStatementBuilderCreateMergeAllWithUncoveredPrimary()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get(typeof(SqlConnection));
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
        public void TestSqlServerStatementBuilderCreateMergeAllWithCoveredIdentity()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get(typeof(SqlConnection));
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
        public void TestSqlServerStatementBuilderCreateMergeAllWithUncoveredIdentity()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get(typeof(SqlConnection));
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
        public void TestSqlServerStatementBuilderCreateMergeAllWithCoveredPrimaryButWithoutQualifiers()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get(typeof(SqlConnection));
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
        public void TestSqlServerStatementBuilderCreateMergeAllWithCoveredPrimaryAndWithCoveredIdentityButWithoutQualifiers()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get(typeof(SqlConnection));
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
        public void TestSqlServerStatementBuilderCreateMergeAllWithIdentityForThreeBatches()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get(typeof(SqlConnection));
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

        [TestMethod]
        public void TestSqlServerStatementBuilderCreateMergeAllWithHints()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get(typeof(SqlConnection));
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
                identityField: null,
                hints: SqlServerTableHints.TabLock);
            var expected = $"" +
                $"MERGE [Table] AS T WITH (TABLOCK) " +
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
        public void TestSqlServerStatementBuilderCreateMergeAllWithIdentityForThreeBatchesWithHints()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get(typeof(SqlConnection));
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
                identityField: identityField,
                hints: SqlServerTableHints.TabLock);
            var expected = $"" +
                $"MERGE [Table] AS T WITH (TABLOCK) " +
                $"USING ( SELECT @Field1 AS [Field1], @Field2 AS [Field2], @Field3 AS [Field3] ) " +
                $"AS S ON ( S.[Field1] = T.[Field1] ) " +
                $"WHEN NOT MATCHED THEN " +
                $"INSERT ( [Field2], [Field3] ) " +
                $"VALUES ( S.[Field2], S.[Field3] ) " +
                $"WHEN MATCHED THEN " +
                $"UPDATE SET [Field2] = S.[Field2], [Field3] = S.[Field3] " +
                $"OUTPUT INSERTED.[Field1] AS [Result] ; " +
                $"MERGE [Table] AS T WITH (TABLOCK) " +
                $"USING ( SELECT @Field1_1 AS [Field1], @Field2_1 AS [Field2], @Field3_1 AS [Field3] ) " +
                $"AS S ON ( S.[Field1] = T.[Field1] ) " +
                $"WHEN NOT MATCHED THEN " +
                $"INSERT ( [Field2], [Field3] ) " +
                $"VALUES ( S.[Field2], S.[Field3] ) " +
                $"WHEN MATCHED THEN " +
                $"UPDATE SET [Field2] = S.[Field2], [Field3] = S.[Field3] " +
                $"OUTPUT INSERTED.[Field1] AS [Result] ; " +
                $"MERGE [Table] AS T WITH (TABLOCK) " +
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
        public void ThrowExceptionOnSqlServerStatementBuilderCreateMergeAllIfThereAreNoFields()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get(typeof(SqlConnection));
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
        public void ThrowExceptionOnSqlServerStatementBuilderCreateMergeAllIfThereAreNoPrimaryAndNoQualifiers()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get(typeof(SqlConnection));
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
        public void ThrowExceptionOnSqlServerStatementBuilderCreateMergeAllIfTheQualifiersAreNotPresentAtTheGivenFields()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get(typeof(SqlConnection));
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
        public void ThrowExceptionOnSqlServerStatementBuilderCreateMergeAllIfThePrimaryAsQualifierIsNotPresentAtTheGivenFields()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get(typeof(SqlConnection));
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
        public void ThrowExceptionOnSqlServerStatementBuilderCreateMergeAllIfTheTableIsNull()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get(typeof(SqlConnection));
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
        public void ThrowExceptionOnSqlServerStatementBuilderCreateMergeAllIfTheTableIsEmpty()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get(typeof(SqlConnection));
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
        public void ThrowExceptionOnSqlServerStatementBuilderCreateMergeAllIfTheTableIsWhitespace()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get(typeof(SqlConnection));
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
        public void ThrowExceptionOnSqlServerStatementBuilderCreateMergeAllIfThePrimaryIsNotReallyAPrimary()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get(typeof(SqlConnection));
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
        public void ThrowExceptionOnSqlServerStatementBuilderCreateMergeAllIfTheIdentityIsNotReallyAnIdentity()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get(typeof(SqlConnection));
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
