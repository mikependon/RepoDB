using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Exceptions;
using RepoDb.UnitTests.CustomObjects;
using System;

namespace RepoDb.UnitTests.StatementBuilders
{
    [TestClass]
    public class BaseStatementBuilderCreateInsertTest
    {
        [TestInitialize]
        public void Initialize()
        {
            StatementBuilderMapper.Add<BaseStatementBuilderDbConnection>(new CustomBaseStatementBuilder(), true);
        }

        #region SubClasses

        private class BaseStatementBuilderDbConnection : CustomDbConnection { }

        #endregion

        [TestMethod]
        public void TestBaseStatementBuilderCreateInsert()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<BaseStatementBuilderDbConnection>();
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
                $"( @Field1, @Field2, @Field3 ) ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestBaseStatementBuilderCreateInsertWithQuotedTableSchema()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<BaseStatementBuilderDbConnection>();
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
                $"( @Field1, @Field2, @Field3 ) ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestBaseStatementBuilderCreateInsertWithUnquotedTableSchema()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<BaseStatementBuilderDbConnection>();
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
                $"( @Field1, @Field2, @Field3 ) ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestBaseStatementBuilderCreateInsertWithPrimary()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<BaseStatementBuilderDbConnection>();
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
                $"( @Field1, @Field2, @Field3 ) ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestBaseStatementBuilderCreateInsertWithIdentity()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<BaseStatementBuilderDbConnection>();
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
                $"( @Field2, @Field3 ) ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestBaseStatementBuilderCreateInsertWithPrimaryAndIdentity()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<BaseStatementBuilderDbConnection>();
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
                $"( @Field1, @Field3 ) ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestBaseStatementBuilderCreateInsertWithHints()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<BaseStatementBuilderDbConnection>();
            var queryBuilder = new QueryBuilder();
            var tableName = "Table";
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });

            // Act
            var actual = statementBuilder.CreateInsert(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields,
                primaryField: null,
                identityField: null,
                hints: "WITH (TABLOCK)");
            var expected = $"" +
                $"INSERT INTO [Table] WITH (TABLOCK) " +
                $"( [Field1], [Field2], [Field3] ) " +
                $"VALUES " +
                $"( @Field1, @Field2, @Field3 ) ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod, ExpectedException(typeof(PrimaryFieldNotFoundException))]
        public void ThrowExceptionOnBaseStatementBuilderCreateInsertIfTheNonIdentityPrimaryIsNotCovered()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<BaseStatementBuilderDbConnection>();
            var queryBuilder = new QueryBuilder();
            var tableName = "Table";
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });
            var primaryField = new DbField("Id", true, false, false, typeof(int), null, null, null, null);

            // Act
            statementBuilder.CreateInsert(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields,
                primaryField: primaryField,
                identityField: null);
        }

        [TestMethod, ExpectedException(typeof(EmptyException))]
        public void ThrowExceptionOnBaseStatementBuilderCreateInsertIfThereAreNoFields()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<BaseStatementBuilderDbConnection>();
            var queryBuilder = new QueryBuilder();
            var tableName = "Table";

            // Act
            statementBuilder.CreateInsert(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: null,
                primaryField: null,
                identityField: null);
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnBaseStatementBuilderCreateInsertIfTheTableIsNull()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<BaseStatementBuilderDbConnection>();
            var queryBuilder = new QueryBuilder();
            var tableName = (string)null;

            // Act
            statementBuilder.CreateInsert(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: null,
                primaryField: null,
                identityField: null);
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnBaseStatementBuilderCreateInsertIfTheTableIsEmpty()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<BaseStatementBuilderDbConnection>();
            var queryBuilder = new QueryBuilder();
            var tableName = "";

            // Act
            statementBuilder.CreateInsert(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: null,
                primaryField: null,
                identityField: null);
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnBaseStatementBuilderCreateInsertIfTheTableIsWhitespace()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<BaseStatementBuilderDbConnection>();
            var queryBuilder = new QueryBuilder();
            var tableName = " ";

            // Act
            statementBuilder.CreateInsert(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: null,
                primaryField: null,
                identityField: null);
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void ThrowExceptionOnBaseStatementBuilderCreateInsertIfThePrimaryIsNotReallyAPrimary()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<BaseStatementBuilderDbConnection>();
            var queryBuilder = new QueryBuilder();
            var tableName = "Table";
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });
            var primaryField = new DbField("Field1", false, false, false, typeof(int), null, null, null, null);

            // Act
            statementBuilder.CreateInsert(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields,
                primaryField: primaryField,
                identityField: null);
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void ThrowExceptionOnBaseStatementBuilderCreateInsertIfTheIdentityIsNotReallyAnIdentity()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<BaseStatementBuilderDbConnection>();
            var queryBuilder = new QueryBuilder();
            var tableName = "Table";
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });
            var qualifiers = Field.From("Field1");
            var identifyField = new DbField("Field2", false, false, false, typeof(int), null, null, null, null);

            // Act
            statementBuilder.CreateInsert(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields,
                primaryField: null,
                identityField: identifyField);
        }
    }
}
