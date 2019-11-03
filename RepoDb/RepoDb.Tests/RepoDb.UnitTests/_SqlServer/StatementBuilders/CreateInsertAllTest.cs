using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.SqlClient;

namespace RepoDb.UnitTests.StatementBuilders
{
    [TestClass]
    public class SqlStatementBuilderCreateInsertAllTest
    {
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
    }
}
