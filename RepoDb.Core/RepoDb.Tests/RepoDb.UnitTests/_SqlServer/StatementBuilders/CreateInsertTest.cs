using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.SqlClient;

namespace RepoDb.UnitTests.StatementBuilders
{
    [TestClass]
    public class SqlServerStatementBuilderCreateInsertTest
    {
        [TestMethod]
        public void TestSqlServerStatementBuilderCreateInsert()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get(typeof(SqlConnection));
            var queryBuilder = new QueryBuilder();
            var tableName = "Table";
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });

            // Act
            var actual = statementBuilder.CreateInsert(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields,
                primaryField: null,
                identityField: null);
            var expected = $"" +
                $"INSERT INTO [Table] " +
                $"( [Field1], [Field2], [Field3] ) " +
                $"VALUES " +
                $"( @Field1, @Field2, @Field3 ) ; " +
                $"SELECT NULL AS [Result] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestSqlServerStatementBuilderCreateInsertWithQuotedTableSchema()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get(typeof(SqlConnection));
            var queryBuilder = new QueryBuilder();
            var tableName = "[dbo].[Table]";
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });

            // Act
            var actual = statementBuilder.CreateInsert(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields,
                primaryField: null,
                identityField: null);
            var expected = $"" +
                $"INSERT INTO [dbo].[Table] " +
                $"( [Field1], [Field2], [Field3] ) " +
                $"VALUES " +
                $"( @Field1, @Field2, @Field3 ) ; " +
                $"SELECT NULL AS [Result] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestSqlServerStatementBuilderCreateInsertWithUnquotedTableSchema()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get(typeof(SqlConnection));
            var queryBuilder = new QueryBuilder();
            var tableName = "dbo.Table";
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });

            // Act
            var actual = statementBuilder.CreateInsert(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields,
                primaryField: null,
                identityField: null);
            var expected = $"" +
                $"INSERT INTO [dbo].[Table] " +
                $"( [Field1], [Field2], [Field3] ) " +
                $"VALUES " +
                $"( @Field1, @Field2, @Field3 ) ; " +
                $"SELECT NULL AS [Result] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestSqlServerStatementBuilderCreateInsertWithPrimary()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get(typeof(SqlConnection));
            var queryBuilder = new QueryBuilder();
            var tableName = "Table";
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });
            var primaryField = new DbField("Field1", true, false, false, typeof(int), null, null, null, null);

            // Act
            var actual = statementBuilder.CreateInsert(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields,
                primaryField: primaryField,
                identityField: null);
            var expected = $"" +
                $"INSERT INTO [Table] " +
                $"( [Field1], [Field2], [Field3] ) " +
                $"VALUES " +
                $"( @Field1, @Field2, @Field3 ) ; " +
                $"SELECT @Field1 AS [Result] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestSqlServerStatementBuilderCreateInsertWithIdentity()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get(typeof(SqlConnection));
            var queryBuilder = new QueryBuilder();
            var tableName = "Table";
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });
            var identityField = new DbField("Field1", false, true, false, typeof(int), null, null, null, null);

            // Act
            var actual = statementBuilder.CreateInsert(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields,
                primaryField: null,
                identityField: identityField);
            var expected = $"" +
                $"INSERT INTO [Table] " +
                $"( [Field2], [Field3] ) " +
                $"VALUES " +
                $"( @Field2, @Field3 ) ; " +
                $"SELECT CONVERT(INT, SCOPE_IDENTITY()) AS [Result] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestSqlServerStatementBuilderCreateInsertWithIdentityAsBigInt()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get(typeof(SqlConnection));
            var queryBuilder = new QueryBuilder();
            var tableName = "Table";
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });
            var identityField = new DbField("Field1", false, true, false, typeof(long), null, null, null, null);

            // Act
            var actual = statementBuilder.CreateInsert(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields,
                primaryField: null,
                identityField: identityField);
            var expected = $"" +
                $"INSERT INTO [Table] " +
                $"( [Field2], [Field3] ) " +
                $"VALUES " +
                $"( @Field2, @Field3 ) ; " +
                $"SELECT CONVERT(BIGINT, SCOPE_IDENTITY()) AS [Result] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestSqlServerStatementBuilderCreateInsertWithPrimaryAndIdentity()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get(typeof(SqlConnection));
            var queryBuilder = new QueryBuilder();
            var tableName = "Table";
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });
            var primaryField = new DbField("Field1", true, false, false, typeof(int), null, null, null, null);
            var identityField = new DbField("Field2", false, true, false, typeof(int), null, null, null, null);

            // Act
            var actual = statementBuilder.CreateInsert(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields,
                primaryField: null,
                identityField: identityField);
            var expected = $"" +
                $"INSERT INTO [Table] " +
                $"( [Field1], [Field3] ) " +
                $"VALUES " +
                $"( @Field1, @Field3 ) ; " +
                $"SELECT CONVERT(INT, SCOPE_IDENTITY()) AS [Result] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestSqlServerStatementBuilderCreateInsertWithPrimaryAndIdentityAsBigInt()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get(typeof(SqlConnection));
            var queryBuilder = new QueryBuilder();
            var tableName = "Table";
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });
            var primaryField = new DbField("Field1", true, false, false, typeof(int), null, null, null, null);
            var identityField = new DbField("Field2", false, true, false, typeof(long), null, null, null, null);

            // Act
            var actual = statementBuilder.CreateInsert(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields,
                primaryField: null,
                identityField: identityField);
            var expected = $"" +
                $"INSERT INTO [Table] " +
                $"( [Field1], [Field3] ) " +
                $"VALUES " +
                $"( @Field1, @Field3 ) ; " +
                $"SELECT CONVERT(BIGINT, SCOPE_IDENTITY()) AS [Result] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestSqlServerStatementBuilderCreateInsertWithHints()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get(typeof(SqlConnection));
            var queryBuilder = new QueryBuilder();
            var tableName = "Table";
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });

            // Act
            var actual = statementBuilder.CreateInsert(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields,
                primaryField: null,
                identityField: null,
                hints: SqlServerTableHints.TabLock);
            var expected = $"" +
                $"INSERT INTO [Table] WITH (TABLOCK) " +
                $"( [Field1], [Field2], [Field3] ) " +
                $"VALUES " +
                $"( @Field1, @Field2, @Field3 ) ; " +
                $"SELECT NULL AS [Result] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

    }
}
