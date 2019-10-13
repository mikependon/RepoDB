using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Exceptions;
using RepoDb.StatementBuilders;
using RepoDb.UnitTests.Setup;
using System;

namespace RepoDb.UnitTests.StatementBuilders
{
    [TestClass]
    public class SqlStatementBuilderCreateUpdateAllTest
    {
        [TestMethod]
        public void TestSqlStatementBuilderCreateUpdateAll()
        {
            // Setup
            var statementBuilder = new SqlServerStatementBuilder();
            var queryBuilder = new QueryBuilder();
            var tableName = "Table";
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" }, Helper.DbSetting);
            var qualifiers = Field.From("Field1", Helper.DbSetting);

            // Act
            var actual = statementBuilder.CreateUpdateAll(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields,
                qualifiers: qualifiers,
                batchSize: 1,
                primaryField: null,
                identityField: null);
            var expected = $"" +
                $"UPDATE [Table] " +
                $"SET [Field2] = @Field2, [Field3] = @Field3 " +
                $"WHERE ([Field1] = @Field1) ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestSqlStatementBuilderCreateUpdateAllWithQuotedTableSchema()
        {
            // Setup
            var statementBuilder = new SqlServerStatementBuilder();
            var queryBuilder = new QueryBuilder();
            var tableName = "[dbo].[Table]";
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" }, Helper.DbSetting);
            var qualifiers = Field.From("Field1", Helper.DbSetting);

            // Act
            var actual = statementBuilder.CreateUpdateAll(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields,
                qualifiers: qualifiers,
                batchSize: 1,
                primaryField: null,
                identityField: null);
            var expected = $"" +
                $"UPDATE [dbo].[Table] " +
                $"SET [Field2] = @Field2, [Field3] = @Field3 " +
                $"WHERE ([Field1] = @Field1) ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestSqlStatementBuilderCreateUpdateAllWithUnquotedTableSchema()
        {
            // Setup
            var statementBuilder = new SqlServerStatementBuilder();
            var queryBuilder = new QueryBuilder();
            var tableName = "dbo.Table";
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" }, Helper.DbSetting);
            var qualifiers = Field.From("Field1", Helper.DbSetting);

            // Act
            var actual = statementBuilder.CreateUpdateAll(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields,
                qualifiers: qualifiers,
                batchSize: 1,
                primaryField: null,
                identityField: null);
            var expected = $"" +
                $"UPDATE [dbo].[Table] " +
                $"SET [Field2] = @Field2, [Field3] = @Field3 " +
                $"WHERE ([Field1] = @Field1) ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestSqlStatementBuilderCreateUpdateAllWithCoveredPrimaryField()
        {
            // Setup
            var statementBuilder = new SqlServerStatementBuilder();
            var queryBuilder = new QueryBuilder();
            var tableName = "Table";
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" }, Helper.DbSetting);
            var field = new DbField("Field1", true, true, false, typeof(int), null, null, null, null, Helper.DbSetting);

            // Act
            var actual = statementBuilder.CreateUpdateAll(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields,
                qualifiers: null,
                batchSize: 1,
                primaryField: field,
                identityField: null);
            var expected = $"" +
                $"UPDATE [Table] " +
                $"SET [Field2] = @Field2, [Field3] = @Field3 " +
                $"WHERE ([Field1] = @Field1) ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestSqlStatementBuilderCreateUpdateAllWithCoveredIdentityField()
        {
            // Setup
            var statementBuilder = new SqlServerStatementBuilder();
            var queryBuilder = new QueryBuilder();
            var tableName = "Table";
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" }, Helper.DbSetting);
            var qualifiers = Field.From("Field1", Helper.DbSetting);
            var field = new DbField("Field1", true, true, false, typeof(int), null, null, null, null, Helper.DbSetting);

            // Act
            var actual = statementBuilder.CreateUpdateAll(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields,
                qualifiers: qualifiers,
                batchSize: 1,
                primaryField: null,
                identityField: field);
            var expected = $"" +
                $"UPDATE [Table] " +
                $"SET [Field2] = @Field2, [Field3] = @Field3 " +
                $"WHERE ([Field1] = @Field1) ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestSqlStatementBuilderCreateUpdateAllWithCoveredPrimaryAsIdentity()
        {
            // Setup
            var statementBuilder = new SqlServerStatementBuilder();
            var queryBuilder = new QueryBuilder();
            var tableName = "Table";
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" }, Helper.DbSetting);
            var qualifiers = Field.From("Field1", Helper.DbSetting);
            var field = new DbField("Field1", true, true, false, typeof(int), null, null, null, null, Helper.DbSetting);

            // Act
            var actual = statementBuilder.CreateUpdateAll(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields,
                qualifiers: qualifiers,
                batchSize: 1,
                primaryField: field,
                identityField: field);
            var expected = $"" +
                $"UPDATE [Table] " +
                $"SET [Field2] = @Field2, [Field3] = @Field3 " +
                $"WHERE ([Field1] = @Field1) ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestSqlStatementBuilderCreateUpdateAllForThreeBatches()
        {
            // Setup
            var statementBuilder = new SqlServerStatementBuilder();
            var queryBuilder = new QueryBuilder();
            var tableName = "Table";
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" }, Helper.DbSetting);
            var qualifiers = Field.From("Field1", Helper.DbSetting);

            // Act
            var actual = statementBuilder.CreateUpdateAll(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields,
                qualifiers: qualifiers,
                batchSize: 3,
                primaryField: null,
                identityField: null);
            var expected = $"" +
                $"UPDATE [Table] " +
                $"SET [Field2] = @Field2, [Field3] = @Field3 " +
                $"WHERE ([Field1] = @Field1) ; " +
                $"UPDATE [Table] " +
                $"SET [Field2] = @Field2_1, [Field3] = @Field3_1 " +
                $"WHERE ([Field1] = @Field1_1) ; " +
                $"UPDATE [Table] " +
                $"SET [Field2] = @Field2_2, [Field3] = @Field3_2 " +
                $"WHERE ([Field1] = @Field1_2) ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnSqlStatementBuilderCreateUpdateAllIfTheTableIsNull()
        {
            // Setup
            var statementBuilder = new SqlServerStatementBuilder();
            var queryBuilder = new QueryBuilder();
            var tableName = (string)null;
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" }, Helper.DbSetting);
            var qualifiers = Field.From("Field1", Helper.DbSetting);

            // Act
            statementBuilder.CreateUpdateAll(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields,
                qualifiers: qualifiers,
                batchSize: 1,
                primaryField: null,
                identityField: null);
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnSqlStatementBuilderCreateUpdateAllIfTheTableIsEmpty()
        {
            // Setup
            var statementBuilder = new SqlServerStatementBuilder();
            var queryBuilder = new QueryBuilder();
            var tableName = "";
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" }, Helper.DbSetting);
            var qualifiers = Field.From("Field1", Helper.DbSetting);

            // Act
            statementBuilder.CreateUpdateAll(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields,
                qualifiers: qualifiers,
                batchSize: 1,
                primaryField: null,
                identityField: null);
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnSqlStatementBuilderCreateUpdateAllIfTheTableIsWhitespace()
        {
            // Setup
            var statementBuilder = new SqlServerStatementBuilder();
            var queryBuilder = new QueryBuilder();
            var tableName = " ";
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" }, Helper.DbSetting);
            var qualifiers = Field.From("Field1", Helper.DbSetting);

            // Act
            statementBuilder.CreateUpdateAll(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields,
                qualifiers: qualifiers,
                batchSize: 1,
                primaryField: null,
                identityField: null);
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void ThrowExceptionOnSqlStatementBuilderCreateUpdateAllIfThePrimaryIsNotReallyAPrimary()
        {
            // Setup
            var statementBuilder = new SqlServerStatementBuilder();
            var queryBuilder = new QueryBuilder();
            var tableName = "Table";
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" }, Helper.DbSetting);
            var qualifiers = Field.From("Field1", Helper.DbSetting);
            var primaryField = new DbField("Field1", false, false, false, typeof(int), null, null, null, null, Helper.DbSetting);

            // Act
            statementBuilder.CreateUpdateAll(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields,
                qualifiers: qualifiers,
                batchSize: 1,
                primaryField: primaryField,
                identityField: null);
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void ThrowExceptionOnSqlStatementBuilderCreateUpdateAllIfTheIdentityIsNotReallyAnIdentity()
        {
            // Setup
            var statementBuilder = new SqlServerStatementBuilder();
            var queryBuilder = new QueryBuilder();
            var tableName = "Table";
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" }, Helper.DbSetting);
            var qualifiers = Field.From("Field1", Helper.DbSetting);
            var identifyField = new DbField("Field2", false, false, false, typeof(int), null, null, null, null, Helper.DbSetting);

            // Act
            statementBuilder.CreateUpdateAll(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields,
                qualifiers: qualifiers,
                batchSize: 1,
                primaryField: null,
                identityField: identifyField);
        }

        [TestMethod, ExpectedException(typeof(InvalidQualifierFieldsException))]
        public void ThrowExceptionOnSqlStatementBuilderCreateUpdateAllIfAnyOfTheQualifierIsNotCovered()
        {
            // Setup
            var statementBuilder = new SqlServerStatementBuilder();
            var queryBuilder = new QueryBuilder();
            var tableName = "Table";
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" }, Helper.DbSetting);
            var qualifiers = Field.From("Id", Helper.DbSetting);

            // Act
            statementBuilder.CreateUpdateAll(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields,
                qualifiers: qualifiers,
                batchSize: 1,
                primaryField: null,
                identityField: null);
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnSqlStatementBuilderCreateUpdateAllIfThereAreNoQualifiers()
        {
            // Setup
            var statementBuilder = new SqlServerStatementBuilder();
            var queryBuilder = new QueryBuilder();
            var tableName = "Table";
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" }, Helper.DbSetting);

            // Act
            statementBuilder.CreateUpdateAll(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields,
                qualifiers: null,
                batchSize: 1,
                primaryField: null,
                identityField: null);
        }
    }
}
