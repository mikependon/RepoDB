using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.StatementBuilders;
using RepoDb.UnitTests.Setup;
using System;
using System.Data.SqlClient;

namespace RepoDb.UnitTests.StatementBuilders
{
    [TestClass]
    public class SqlStatementBuilderCreateUpdateTest
    {
        [TestMethod]
        public void TestSqlStatementBuilderCreateUpdate()
        {
            // Setup
            var statementBuilder = new SqlServerStatementBuilder();
            var queryBuilder = new QueryBuilder();
            var tableName = "Table";
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });
            var where = (QueryGroup)null;

            // Act
            var actual = statementBuilder.CreateUpdate(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields,
                where: where,
                primaryField: null,
                identityField: null);
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
            var statementBuilder = new SqlServerStatementBuilder();
            var queryBuilder = new QueryBuilder();
            var tableName = "[dbo].[Table]";
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });
            var where = (QueryGroup)null;

            // Act
            var actual = statementBuilder.CreateUpdate(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields,
                where: where,
                primaryField: null,
                identityField: null);
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
            var statementBuilder = new SqlServerStatementBuilder();
            var queryBuilder = new QueryBuilder();
            var tableName = "dbo.Table";
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });
            var where = (QueryGroup)null;

            // Act
            var actual = statementBuilder.CreateUpdate(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields,
                where: where,
                primaryField: null,
                identityField: null);
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
            var statementBuilder = new SqlServerStatementBuilder();
            var queryBuilder = new QueryBuilder();
            var tableName = "Table";
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });
            var where = new QueryGroup(new QueryField("Field1", 1));

            // Act
            var actual = statementBuilder.CreateUpdate(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields,
                where: where,
                primaryField: null,
                identityField: null);
            var expected = $"" +
                $"UPDATE [Table] " +
                $"SET [Field1] = @Field1, [Field2] = @Field2, [Field3] = @Field3 " +
                $"WHERE ([Field1] = @_Field1) ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestSqlStatementBuilderCreateUpdateWithCoveredPrimaryField()
        {
            // Setup
            var statementBuilder = new SqlServerStatementBuilder();
            var queryBuilder = new QueryBuilder();
            var tableName = "Table";
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });
            var where = new QueryGroup(new QueryField("Field1", 1));
            var field = new DbField("Field1", true, true, false, typeof(int), null, null, null, null);

            // Act
            var actual = statementBuilder.CreateUpdate(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields,
                where: where,
                primaryField: field,
                identityField: null);
            var expected = $"" +
                $"UPDATE [Table] " +
                $"SET [Field2] = @Field2, [Field3] = @Field3 " +
                $"WHERE ([Field1] = @_Field1) ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestSqlStatementBuilderCreateUpdateWithCoveredIdentityField()
        {
            // Setup
            var statementBuilder = new SqlServerStatementBuilder();
            var queryBuilder = new QueryBuilder();
            var tableName = "Table";
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });
            var where = new QueryGroup(new QueryField("Field1", 1));
            var field = new DbField("Field1", true, true, false, typeof(int), null, null, null, null);

            // Act
            var actual = statementBuilder.CreateUpdate(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields,
                where: where,
                primaryField: null,
                identityField: field);
            var expected = $"" +
                $"UPDATE [Table] " +
                $"SET [Field2] = @Field2, [Field3] = @Field3 " +
                $"WHERE ([Field1] = @_Field1) ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestSqlStatementBuilderCreateUpdateWithCoveredPrimaryAsIdentity()
        {
            // Setup
            var statementBuilder = new SqlServerStatementBuilder();
            var queryBuilder = new QueryBuilder();
            var tableName = "Table";
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });
            var where = new QueryGroup(new QueryField("Field1", 1));
            var field = new DbField("Field1", true, true, false, typeof(int), null, null, null, null);

            // Act
            var actual = statementBuilder.CreateUpdate(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields,
                where: where,
                primaryField: field,
                identityField: field);
            var expected = $"" +
                $"UPDATE [Table] " +
                $"SET [Field2] = @Field2, [Field3] = @Field3 " +
                $"WHERE ([Field1] = @_Field1) ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestSqlStatementBuilderCreateUpdateWithUncoveredWhereExpressionAndWithUncoveredPrimary()
        {
            // Setup
            var statementBuilder = new SqlServerStatementBuilder();
            var queryBuilder = new QueryBuilder();
            var tableName = "Table";
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });
            var where = new QueryGroup(new QueryField("Id", 1));
            var field = new DbField("Id", true, true, false, typeof(int), null, null, null, null);

            // Act
            var actual = statementBuilder.CreateUpdate(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields,
                where: where,
                primaryField: field,
                identityField: null);
            var expected = $"" +
                $"UPDATE [Table] " +
                $"SET [Field1] = @Field1, [Field2] = @Field2, [Field3] = @Field3 " +
                $"WHERE ([Id] = @_Id) ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestSqlStatementBuilderCreateUpdateWithUncoveredWhereExpressionAndWithUncoveredIdentity()
        {
            // Setup
            var statementBuilder = new SqlServerStatementBuilder();
            var queryBuilder = new QueryBuilder();
            var tableName = "Table";
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });
            var where = new QueryGroup(new QueryField("Id", 1));
            var field = new DbField("Id", true, true, false, typeof(int), null, null, null, null);

            // Act
            var actual = statementBuilder.CreateUpdate(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields,
                where: where,
                primaryField: null,
                identityField: field);
            var expected = $"" +
                $"UPDATE [Table] " +
                $"SET [Field1] = @Field1, [Field2] = @Field2, [Field3] = @Field3 " +
                $"WHERE ([Id] = @_Id) ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestSqlStatementBuilderCreateUpdateWithUncoveredWhereExpressionAndWithUncoveredPrimaryAsIdentity()
        {
            // Setup
            var statementBuilder = new SqlServerStatementBuilder();
            var queryBuilder = new QueryBuilder();
            var tableName = "Table";
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });
            var where = new QueryGroup(new QueryField("Id", 1));
            var field = new DbField("Id", true, true, false, typeof(int), null, null, null, null);

            // Act
            var actual = statementBuilder.CreateUpdate(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields,
                where: where,
                primaryField: field,
                identityField: field);
            var expected = $"" +
                $"UPDATE [Table] " +
                $"SET [Field1] = @Field1, [Field2] = @Field2, [Field3] = @Field3 " +
                $"WHERE ([Id] = @_Id) ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestSqlStatementBuilderCreateUpdateWithUncoveredWhereExpressionButWithCoveredPrimary()
        {
            // Setup
            var statementBuilder = new SqlServerStatementBuilder();
            var queryBuilder = new QueryBuilder();
            var tableName = "Table";
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });
            var where = new QueryGroup(new QueryField("Id", 1));
            var field = new DbField("Field1", true, true, false, typeof(int), null, null, null, null);

            // Act
            var actual = statementBuilder.CreateUpdate(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields,
                where: where,
                primaryField: field,
                identityField: null);
            var expected = $"" +
                $"UPDATE [Table] " +
                $"SET [Field2] = @Field2, [Field3] = @Field3 " +
                $"WHERE ([Id] = @_Id) ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestSqlStatementBuilderCreateUpdateWithUncoveredWhereExpressionButWithCoveredIdentity()
        {
            // Setup
            var statementBuilder = new SqlServerStatementBuilder();
            var queryBuilder = new QueryBuilder();
            var tableName = "Table";
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });
            var where = new QueryGroup(new QueryField("Id", 1));
            var field = new DbField("Field1", true, true, false, typeof(int), null, null, null, null);

            // Act
            var actual = statementBuilder.CreateUpdate(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields,
                where: where,
                primaryField: null,
                identityField: field);
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
            var statementBuilder = new SqlServerStatementBuilder();
            var queryBuilder = new QueryBuilder();
            var tableName = (string)null;
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });

            // Act
            statementBuilder.CreateUpdate(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields,
                where: null,
                primaryField: null,
                identityField: null);
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnSqlStatementBuilderCreateUpdateIfTheTableIsEmpty()
        {
            // Setup
            var statementBuilder = new SqlServerStatementBuilder();
            var queryBuilder = new QueryBuilder();
            var tableName = "";
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });

            // Act
            statementBuilder.CreateUpdate(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields,
                where: null,
                primaryField: null,
                identityField: null);
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnSqlStatementBuilderCreateUpdateIfTheTableIsWhitespace()
        {
            // Setup
            var statementBuilder = new SqlServerStatementBuilder();
            var queryBuilder = new QueryBuilder();
            var tableName = " ";
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });

            // Act
            statementBuilder.CreateUpdate(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields,
                where: null,
                primaryField: null,
                identityField: null);
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void ThrowExceptionOnSqlStatementBuilderCreateUpdateIfThePrimaryIsNotReallyAPrimary()
        {
            // Setup
            var statementBuilder = new SqlServerStatementBuilder();
            var queryBuilder = new QueryBuilder();
            var tableName = "Table";
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });
            var primaryField = new DbField("Field1", false, false, false, typeof(int), null, null, null, null);

            // Act
            statementBuilder.CreateUpdate(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields,
                where: null,
                primaryField: primaryField,
                identityField: null);
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void ThrowExceptionOnSqlStatementBuilderCreateUpdateIfTheIdentityIsNotReallyAnIdentity()
        {
            // Setup
            var statementBuilder = new SqlServerStatementBuilder();
            var queryBuilder = new QueryBuilder();
            var tableName = "Table";
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });
            var qualifiers = Field.From("Field1");
            var identifyField = new DbField("Field2", false, false, false, typeof(int), null, null, null, null);

            // Act
            statementBuilder.CreateUpdate(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields,
                where: null,
                primaryField: null,
                identityField: identifyField);
        }
    }
}
