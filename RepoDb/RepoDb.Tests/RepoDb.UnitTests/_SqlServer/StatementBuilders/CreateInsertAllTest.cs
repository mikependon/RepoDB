using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Exceptions;
using System;
using System.Data.SqlClient;

namespace RepoDb.UnitTests.StatementBuilders
{
    [TestClass]
    public class SqlStatementBuilderCreateInsertAllTest
    {
        [TestMethod]
        public void TestSqlStatementBuilderCreateInsertAll()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get(typeof(SqlConnection));
            var queryBuilder = new QueryBuilder();
            var tableName = "Table";
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });

            // Act
            var actual = statementBuilder.CreateInsertAll(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields,
                batchSize: 1,
                primaryField: null,
                identityField: null);
            var expected = $"" +
                $"INSERT INTO [Table] " +
                $"( [Field1], [Field2], [Field3] ) " +
                $"VALUES " +
                $"( @Field1, @Field2, @Field3 ) ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestSqlStatementBuilderCreateInsertAllWithQuotedTableSchema()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get(typeof(SqlConnection));
            var queryBuilder = new QueryBuilder();
            var tableName = "[dbo].[Table]";
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });

            // Act
            var actual = statementBuilder.CreateInsertAll(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields,
                batchSize: 1,
                primaryField: null,
                identityField: null);
            var expected = $"" +
                $"INSERT INTO [dbo].[Table] " +
                $"( [Field1], [Field2], [Field3] ) " +
                $"VALUES " +
                $"( @Field1, @Field2, @Field3 ) ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestSqlStatementBuilderCreateInsertAllWithUnquotedTableSchema()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get(typeof(SqlConnection));
            var queryBuilder = new QueryBuilder();
            var tableName = "dbo.Table";
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });

            // Act
            var actual = statementBuilder.CreateInsertAll(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields,
                batchSize: 1,
                primaryField: null,
                identityField: null);
            var expected = $"" +
                $"INSERT INTO [dbo].[Table] " +
                $"( [Field1], [Field2], [Field3] ) " +
                $"VALUES " +
                $"( @Field1, @Field2, @Field3 ) ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestSqlStatementBuilderCreateInsertAllWithPrimary()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get(typeof(SqlConnection));
            var queryBuilder = new QueryBuilder();
            var tableName = "Table";
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });
            var primaryField = new DbField("Field1", true, false, false, typeof(int), null, null, null, null);

            // Act
            var actual = statementBuilder.CreateInsertAll(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields,
                batchSize: 1,
                primaryField: primaryField,
                identityField: null);
            var expected = $"" +
                $"INSERT INTO [Table] " +
                $"( [Field1], [Field2], [Field3] ) " +
                $"VALUES " +
                $"( @Field1, @Field2, @Field3 ) ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestSqlStatementBuilderCreateInsertAllWithIdentity()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get(typeof(SqlConnection));
            var queryBuilder = new QueryBuilder();
            var tableName = "Table";
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });
            var identityField = new DbField("Field1", false, true, false, typeof(int), null, null, null, null);

            // Act
            var actual = statementBuilder.CreateInsertAll(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields,
                batchSize: 1,
                primaryField: null,
                identityField: identityField);
            var expected = $"" +
                $"INSERT INTO [Table] " +
                $"( [Field2], [Field3] ) " +
                $"VALUES " +
                $"( @Field2, @Field3 ) ; " +
                $"SET @Field1 = CONVERT(INT, SCOPE_IDENTITY()) ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestSqlStatementBuilderCreateInsertAllWithIdentityAsBigInt()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get(typeof(SqlConnection));
            var queryBuilder = new QueryBuilder();
            var tableName = "Table";
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });
            var identityField = new DbField("Field1", false, true, false, typeof(long), null, null, null, null);

            // Act
            var actual = statementBuilder.CreateInsertAll(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields,
                batchSize: 1,
                primaryField: null,
                identityField: identityField);
            var expected = $"" +
                $"INSERT INTO [Table] " +
                $"( [Field2], [Field3] ) " +
                $"VALUES " +
                $"( @Field2, @Field3 ) ; " +
                $"SET @Field1 = CONVERT(BIGINT, SCOPE_IDENTITY()) ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestSqlStatementBuilderCreateInsertAllWithPrimaryAndIdentity()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get(typeof(SqlConnection));
            var queryBuilder = new QueryBuilder();
            var tableName = "Table";
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });
            var primaryField = new DbField("Field1", true, false, false, typeof(int), null, null, null, null);
            var identityField = new DbField("Field2", false, true, false, typeof(int), null, null, null, null);

            // Act
            var actual = statementBuilder.CreateInsertAll(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields,
                batchSize: 1,
                primaryField: null,
                identityField: identityField);
            var expected = $"" +
                $"INSERT INTO [Table] " +
                $"( [Field1], [Field3] ) " +
                $"VALUES " +
                $"( @Field1, @Field3 ) ; " +
                $"SET @Field2 = CONVERT(INT, SCOPE_IDENTITY()) ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestSqlStatementBuilderCreateInsertAllWithPrimaryAndIdentityAsBigInt()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get(typeof(SqlConnection));
            var queryBuilder = new QueryBuilder();
            var tableName = "Table";
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });
            var primaryField = new DbField("Field1", true, false, false, typeof(int), null, null, null, null);
            var identityField = new DbField("Field2", false, true, false, typeof(long), null, null, null, null);

            // Act
            var actual = statementBuilder.CreateInsertAll(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields,
                batchSize: 1,
                primaryField: null,
                identityField: identityField);
            var expected = $"" +
                $"INSERT INTO [Table] " +
                $"( [Field1], [Field3] ) " +
                $"VALUES " +
                $"( @Field1, @Field3 ) ; " +
                $"SET @Field2 = CONVERT(BIGINT, SCOPE_IDENTITY()) ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestSqlStatementBuilderCreateInsertAllWithIdentityForThreeBatches()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get(typeof(SqlConnection));
            var queryBuilder = new QueryBuilder();
            var tableName = "Table";
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });
            var identityField = new DbField("Field1", false, true, false, typeof(int), null, null, null, null);

            // Act
            var actual = statementBuilder.CreateInsertAll(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields,
                batchSize: 3,
                primaryField: null,
                identityField: identityField);
            var expected = $"" +
                $"INSERT INTO [Table] " +
                $"( [Field2], [Field3] ) " +
                $"VALUES " +
                $"( @Field2, @Field3 ) ; " +
                $"SET @Field1 = CONVERT(INT, SCOPE_IDENTITY()) ; " +
                $"INSERT INTO [Table] " +
                $"( [Field2], [Field3] ) " +
                $"VALUES " +
                $"( @Field2_1, @Field3_1 ) ; " +
                $"SET @Field1_1 = CONVERT(INT, SCOPE_IDENTITY()) ; " +
                $"INSERT INTO [Table] " +
                $"( [Field2], [Field3] ) " +
                $"VALUES " +
                $"( @Field2_2, @Field3_2 ) ; " +
                $"SET @Field1_2 = CONVERT(INT, SCOPE_IDENTITY()) ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod, ExpectedException(typeof(PrimaryFieldNotFoundException))]
        public void ThrowExceptionOnSqlStatementBuilderCreateInsertAllIfTheNonIdentityPrimaryIsNotCovered()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get(typeof(SqlConnection));
            var queryBuilder = new QueryBuilder();
            var tableName = "Table";
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });
            var primaryField = new DbField("Id", true, false, false, typeof(int), null, null, null, null);

            // Act
            statementBuilder.CreateInsertAll(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields,
                batchSize: 1,
                primaryField: primaryField,
                identityField: null);
        }

        [TestMethod, ExpectedException(typeof(EmptyException))]
        public void ThrowExceptionOnSqlStatementBuilderCreateInsertAllIfThereAreNoFields()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get(typeof(SqlConnection));
            var queryBuilder = new QueryBuilder();
            var tableName = "Table";

            // Act
            statementBuilder.CreateInsertAll(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: null,
                batchSize: 1,
                primaryField: null,
                identityField: null);
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnSqlStatementBuilderCreateInsertAllIfTheTableIsNull()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get(typeof(SqlConnection));
            var queryBuilder = new QueryBuilder();
            var tableName = (string)null;

            // Act
            statementBuilder.CreateInsertAll(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: null,
                batchSize: 1,
                primaryField: null,
                identityField: null);
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnSqlStatementBuilderCreateInsertAllIfTheTableIsEmpty()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get(typeof(SqlConnection));
            var queryBuilder = new QueryBuilder();
            var tableName = "";

            // Act
            statementBuilder.CreateInsertAll(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: null,
                batchSize: 1,
                primaryField: null,
                identityField: null);
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnSqlStatementBuilderCreateInsertAllIfTheTableIsWhitespace()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get(typeof(SqlConnection));
            var queryBuilder = new QueryBuilder();
            var tableName = " ";

            // Act
            statementBuilder.CreateInsertAll(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: null,
                batchSize: 1,
                primaryField: null,
                identityField: null);
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void ThrowExceptionOnSqlStatementBuilderCreateInsertAllIfThePrimaryIsNotReallyAPrimary()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get(typeof(SqlConnection));
            var queryBuilder = new QueryBuilder();
            var tableName = "Table";
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });
            var primaryField = new DbField("Field1", false, false, false, typeof(int), null, null, null, null);

            // Act
            statementBuilder.CreateInsertAll(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields,
                batchSize: 1,
                primaryField: primaryField,
                identityField: null);
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void ThrowExceptionOnSqlStatementBuilderCreateInsertAllIfTheIdentityIsNotReallyAnIdentity()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get(typeof(SqlConnection));
            var queryBuilder = new QueryBuilder();
            var tableName = "Table";
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });
            var qualifiers = Field.From("Field1");
            var identifyField = new DbField("Field2", false, false, false, typeof(int), null, null, null, null);

            // Act
            statementBuilder.CreateInsertAll(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields,
                batchSize: 1,
                primaryField: null,
                identityField: identifyField);
        }
    }
}
