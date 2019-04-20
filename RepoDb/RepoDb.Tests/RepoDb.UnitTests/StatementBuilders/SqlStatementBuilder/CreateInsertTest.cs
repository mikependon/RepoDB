using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace RepoDb.UnitTests.StatementBuilders
{
    [TestClass]
    public class SqlStatementBuilderCreateInsertTest
    {
        public void TestSqlStatementBuilderCreateInsert()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder();
            var tableName = "Table";
            var fields = Field.From("Field1", "Field2", "Field3");

            // Act
            var actual = statementBuilder.CreateInsert(queryBuilder: queryBuilder,
                tableName: tableName,
                primaryField: null,
                fields: fields);
            var expected = $"" +
                $"INSERT INTO [Table] " +
                $"( [Field1], [Field2], [Field3] ) " +
                $"VALUES " +
                $"( @Field1, @Field2, @Field3 ) ; " +
                $"SELECT NULL AS [Result] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        public void TestSqlStatementBuilderCreateInsertWithQuotedTableSchema()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder();
            var tableName = "[dbo].[Table]";
            var fields = Field.From("Field1", "Field2", "Field3");

            // Act
            var actual = statementBuilder.CreateInsert(queryBuilder: queryBuilder,
                tableName: tableName,
                primaryField: null,
                fields: fields);
            var expected = $"" +
                $"INSERT INTO [dbo].[Table] " +
                $"( [Field1], [Field2], [Field3] ) " +
                $"VALUES " +
                $"( @Field1, @Field2, @Field3 ) ; " +
                $"SELECT NULL AS [Result] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        public void TestSqlStatementBuilderCreateInsertWithUnquotedTableSchema()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder();
            var tableName = "dbo.Table";
            var fields = Field.From("Field1", "Field2", "Field3");

            // Act
            var actual = statementBuilder.CreateInsert(queryBuilder: queryBuilder,
                tableName: tableName,
                primaryField: null,
                fields: fields);
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
        public void TestSqlStatementBuilderCreateInsertWithIdentityPrimaryField()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder();
            var tableName = "Table";
            var primaryField = new DbField("Id", true, true, false);
            var fields = Field.From("Field1", "Field2", "Field3");

            // Act
            var actual = statementBuilder.CreateInsert(queryBuilder: queryBuilder,
                tableName: tableName,
                primaryField: primaryField,
                fields: fields);
            var expected = $"" +
                $"INSERT INTO [Table] " +
                $"( [Field1], [Field2], [Field3] ) " +
                $"VALUES " +
                $"( @Field1, @Field2, @Field3 ) ; " +
                $"SELECT SCOPE_IDENTITY() AS [Result] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestSqlStatementBuilderCreateInsertWithNonIdentityPrimaryField()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder();
            var tableName = "Table";
            var primaryField = new DbField("Id", true, false, false);
            var fields = Field.From("Field1", "Field2", "Field3");

            // Act
            var actual = statementBuilder.CreateInsert(queryBuilder: queryBuilder,
                tableName: tableName,
                primaryField: primaryField,
                fields: fields);
            var expected = $"" +
                $"INSERT INTO [Table] " +
                $"( [Field1], [Field2], [Field3] ) " +
                $"VALUES " +
                $"( @Field1, @Field2, @Field3 ) ; " +
                $"SELECT @Id AS [Result] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnSqlStatementBuilderCreateInsertIfThereAreNoFields()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder();
            var tableName = (string)null;

            // Act
            statementBuilder.CreateInsert(queryBuilder: queryBuilder,
                tableName: tableName,
                primaryField: null,
                fields: null);
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnSqlStatementBuilderCreateInsertIfTheTableIsNull()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder();
            var tableName = (string)null;

            // Act
            statementBuilder.CreateInsert(queryBuilder: queryBuilder,
                tableName: tableName,
                primaryField: null,
                fields: null);
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnSqlStatementBuilderCreateInsertIfTheTableIsEmpty()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder();
            var tableName = "";

            // Act
            statementBuilder.CreateInsert(queryBuilder: queryBuilder,
                tableName: tableName,
                primaryField: null,
                fields: null);
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnSqlStatementBuilderCreateInsertIfTheTableIsWhitespace()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder();
            var tableName = " ";

            // Act
            statementBuilder.CreateInsert(queryBuilder: queryBuilder,
                tableName: tableName,
                primaryField: null,
                fields: null);
        }
    }
}
