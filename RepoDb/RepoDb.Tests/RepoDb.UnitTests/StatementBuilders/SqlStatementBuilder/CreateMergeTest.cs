using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Exceptions;
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
                primaryField: null,
                fields: fields,
                qualifiers: qualifiers);
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
                primaryField: null,
                fields: fields,
                qualifiers: qualifiers);
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
                primaryField: null,
                fields: fields,
                qualifiers: qualifiers);
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
        public void TestSqlStatementBuilderCreateMergeWithCoveredIdentityPrimary()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder();
            var tableName = "Table";
            var primaryField = new DbField("Field1", true, true, false);
            var fields = Field.From("Field1", "Field2", "Field3");
            var qualifiers = Field.From("Field1");

            // Act
            var actual = statementBuilder.CreateMerge(queryBuilder: queryBuilder,
                tableName: tableName,
                primaryField: primaryField,
                fields: fields,
                qualifiers: qualifiers);
            var expected = $"" +
                $"MERGE [Table] AS T " +
                $"USING ( SELECT @Field1 AS [Field1], @Field2 AS [Field2], @Field3 AS [Field3] ) " +
                $"AS S ON ( S.[Field1] = T.[Field1] ) " +
                $"WHEN NOT MATCHED THEN " +
                $"INSERT ( [Field2], [Field3] ) " +
                $"VALUES ( S.[Field2], S.[Field3] ) " +
                $"WHEN MATCHED THEN " +
                $"UPDATE SET [Field2] = S.[Field2], [Field3] = S.[Field3] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestSqlStatementBuilderCreateMergeWithCoveredNonIdentityPrimary()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder();
            var tableName = "Table";
            var primaryField = new DbField("Field1", true, false, false);
            var fields = Field.From("Field1", "Field2", "Field3");
            var qualifiers = Field.From("Field1");

            // Act
            var actual = statementBuilder.CreateMerge(queryBuilder: queryBuilder,
                tableName: tableName,
                primaryField: primaryField,
                fields: fields,
                qualifiers: qualifiers);
            var expected = $"" +
                $"MERGE [Table] AS T " +
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

        [TestMethod]
        public void TestSqlStatementBuilderCreateMergeWithUncoveredIdentityPrimary()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder();
            var tableName = "Table";
            var primaryField = new DbField("Id", true, true, false);
            var fields = Field.From("Field1", "Field2", "Field3");
            var qualifiers = Field.From("Field1");

            // Act
            var actual = statementBuilder.CreateMerge(queryBuilder: queryBuilder,
                tableName: tableName,
                primaryField: primaryField,
                fields: fields,
                qualifiers: qualifiers);
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
        public void TestSqlStatementBuilderCreateMergeWithUncoveredNonIdentityPrimary()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder();
            var tableName = "Table";
            var primaryField = new DbField("Id", true, false, false);
            var fields = Field.From("Field1", "Field2", "Field3");
            var qualifiers = Field.From("Field1");

            // Act
            var actual = statementBuilder.CreateMerge(queryBuilder: queryBuilder,
                tableName: tableName,
                primaryField: primaryField,
                fields: fields,
                qualifiers: qualifiers);
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
        public void TestSqlStatementBuilderCreateMergeWithConveredIdentityPrimaryButWithoutQualifiers()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder();
            var tableName = "Table";
            var primaryField = new DbField("Field1", true, true, false);
            var fields = Field.From("Field1", "Field2", "Field3");

            // Act
            var actual = statementBuilder.CreateMerge(queryBuilder: queryBuilder,
                tableName: tableName,
                primaryField: primaryField,
                fields: fields,
                qualifiers: null);
            var expected = $"" +
                $"MERGE [Table] AS T " +
                $"USING ( SELECT @Field1 AS [Field1], @Field2 AS [Field2], @Field3 AS [Field3] ) " +
                $"AS S ON ( S.[Field1] = T.[Field1] ) " +
                $"WHEN NOT MATCHED THEN " +
                $"INSERT ( [Field2], [Field3] ) " +
                $"VALUES ( S.[Field2], S.[Field3] ) " +
                $"WHEN MATCHED THEN " +
                $"UPDATE SET [Field2] = S.[Field2], [Field3] = S.[Field3] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestSqlStatementBuilderCreateMergeWithConveredNonIdentityPrimaryButWithoutQualifiers()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder();
            var tableName = "Table";
            var primaryField = new DbField("Field1", true, false, false);
            var fields = Field.From("Field1", "Field2", "Field3");

            // Act
            var actual = statementBuilder.CreateMerge(queryBuilder: queryBuilder,
                tableName: tableName,
                primaryField: primaryField,
                fields: fields,
                qualifiers: null);
            var expected = $"" +
                $"MERGE [Table] AS T " +
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
                primaryField: null,
                fields: null,
                qualifiers: qualifiers);
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
                primaryField: null,
                fields: fields,
                qualifiers: null);
        }

        [TestMethod, ExpectedException(typeof(InvalidQualiferFieldsException))]
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
                primaryField: null,
                fields: fields,
                qualifiers: qualifiers);
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
                primaryField: null,
                fields: fields,
                qualifiers: qualifiers);
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
                primaryField: null,
                fields: fields,
                qualifiers: qualifiers);
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
                primaryField: null,
                fields: fields,
                qualifiers: qualifiers);
        }
    }
}
