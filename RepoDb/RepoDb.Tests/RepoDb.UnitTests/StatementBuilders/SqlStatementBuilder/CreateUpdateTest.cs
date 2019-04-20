using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace RepoDb.UnitTests.StatementBuilders
{
    [TestClass]
    public class SqlStatementBuilderCreateUpdateTest
    {
        [TestMethod]
        public void TestSqlStatementBuilderCreateUpdate()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder();
            var tableName = "Table";
            var fields = Field.From("Field1", "Field2", "Field3");
            var where = (QueryGroup)null;

            // Act
            var actual = statementBuilder.CreateUpdate(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields,
                where: where);
            var expected = $"" +
                $"UPDATE [Table] " +
                $"SET [Field1] = @Field1, [Field2] = @Field2, [Field3] = @Field3 ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestSqlStatementBuilderCreateUpdateWithQuotedTableSchema()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder();
            var tableName = "[dbo].[Table]";
            var fields = Field.From("Field1", "Field2", "Field3");
            var where = (QueryGroup)null;

            // Act
            var actual = statementBuilder.CreateUpdate(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields,
                where: where);
            var expected = $"" +
                $"UPDATE [dbo].[Table] " +
                $"SET [Field1] = @Field1, [Field2] = @Field2, [Field3] = @Field3 ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestSqlStatementBuilderCreateUpdateWithUnquotedTableSchema()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder();
            var tableName = "dbo.Table";
            var fields = Field.From("Field1", "Field2", "Field3");
            var where = (QueryGroup)null;

            // Act
            var actual = statementBuilder.CreateUpdate(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields,
                where: where);
            var expected = $"" +
                $"UPDATE [dbo].[Table] " +
                $"SET [Field1] = @Field1, [Field2] = @Field2, [Field3] = @Field3 ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestSqlStatementBuilderCreateUpdateWithWhereExpression()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder();
            var tableName = "Table";
            var fields = Field.From("Field1", "Field2", "Field3");
            var where = new QueryGroup(new QueryField("Field1", 1));

            // Act
            var actual = statementBuilder.CreateUpdate(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields,
                where: where);
            var expected = $"" +
                $"UPDATE [Table] " +
                $"SET [Field1] = @Field1, [Field2] = @Field2, [Field3] = @Field3 " +
                $"WHERE ([Field1] = @_Field1) ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestSqlStatementBuilderCreateUpdateWithCoveredWhereExpressionAsIdentityPrimary()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder();
            var tableName = "Table";
            var fields = Field.From("Field1", "Field2", "Field3");
            var where = new QueryGroup(new QueryField("Field1", 1));
            var primaryField = new DbField("Field1", true, true, false);

            // Act
            var actual = statementBuilder.CreateUpdate(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields,
                where: where,
                primaryField: primaryField);
            var expected = $"" +
                $"UPDATE [Table] " +
                $"SET [Field2] = @Field2, [Field3] = @Field3 " +
                $"WHERE ([Field1] = @_Field1) ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestSqlStatementBuilderCreateUpdateWithUncoveredWhereExpressionAsIdentityPrimary()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder();
            var tableName = "Table";
            var fields = Field.From("Field1", "Field2", "Field3");
            var where = new QueryGroup(new QueryField("Id", 1));
            var primaryField = new DbField("Id", true, true, false);

            // Act
            var actual = statementBuilder.CreateUpdate(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields,
                where: where,
                primaryField: primaryField);
            var expected = $"" +
                $"UPDATE [Table] " +
                $"SET [Field1] = @Field1, [Field2] = @Field2, [Field3] = @Field3 " +
                $"WHERE ([Id] = @_Id) ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestSqlStatementBuilderCreateUpdateWithUncoveredWhereExpressionButCoveredIdentityPrimary()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder();
            var tableName = "Table";
            var fields = Field.From("Field1", "Field2", "Field3");
            var where = new QueryGroup(new QueryField("Id", 1));
            var primaryField = new DbField("Field1", true, true, false);

            // Act
            var actual = statementBuilder.CreateUpdate(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields,
                where: where,
                primaryField: primaryField);
            var expected = $"" +
                $"UPDATE [Table] " +
                $"SET [Field2] = @Field2, [Field3] = @Field3 " +
                $"WHERE ([Id] = @_Id) ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestSqlStatementBuilderCreateUpdateWithUncoveredWhereExpressionButCoveredNonIdentityPrimary()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder();
            var tableName = "Table";
            var fields = Field.From("Field1", "Field2", "Field3");
            var where = new QueryGroup(new QueryField("Id", 1));
            var primaryField = new DbField("Field1", true, false, false);

            // Act
            var actual = statementBuilder.CreateUpdate(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields,
                where: where,
                primaryField: primaryField);
            var expected = $"" +
                $"UPDATE [Table] " +
                $"SET [Field2] = @Field2, [Field3] = @Field3 " +
                $"WHERE ([Id] = @_Id) ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnSqlStatementBuilderCreateUpdateIfTheTableIsNull()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder();
            var tableName = (string)null;
            var fields = Field.From("Field1", "Field2", "Field3");

            // Act
            statementBuilder.CreateUpdate(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields,
                where: null);
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnSqlStatementBuilderCreateUpdateIfTheTableIsEmpty()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder();
            var tableName = "";
            var fields = Field.From("Field1", "Field2", "Field3");

            // Act
            statementBuilder.CreateUpdate(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields,
                where: null);
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnSqlStatementBuilderCreateUpdateIfTheTableIsWhitespace()
        {
            // Setup
            var statementBuilder = new SqlStatementBuilder();
            var queryBuilder = new QueryBuilder();
            var tableName = " ";
            var fields = Field.From("Field1", "Field2", "Field3");

            // Act
            statementBuilder.CreateUpdate(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields,
                where: null);
        }
    }
}
