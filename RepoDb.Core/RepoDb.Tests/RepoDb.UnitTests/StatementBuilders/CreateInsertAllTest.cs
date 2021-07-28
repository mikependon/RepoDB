using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Exceptions;
using RepoDb.UnitTests.CustomObjects;
using System;

namespace RepoDb.UnitTests.StatementBuilders
{
    [TestClass]
    public class BaseStatementBuilderCreateInsertAllTest
    {
        [TestInitialize]
        public void Initialize()
        {
            StatementBuilderMapper.Add<BaseStatementBuilderDbConnection>(new CustomBaseStatementBuilder(), true);
            StatementBuilderMapper.Add<SingleStatementSupportBaseStatementBuilderDbConnection>(new CustomSingleStatementSupportBaseStatementBuilder(), true);
        }

        #region SubClasses

        private class BaseStatementBuilderDbConnection : CustomDbConnection { }

        private class SingleStatementSupportBaseStatementBuilderDbConnection : CustomDbConnection { }

        #endregion

        [TestMethod]
        public void TestBaseStatementBuilderCreateInsertAll()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<BaseStatementBuilderDbConnection>();
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
        public void TestBaseStatementBuilderCreateInsertAllWithQuotedTableSchema()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<BaseStatementBuilderDbConnection>();
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
        public void TestBaseStatementBuilderCreateInsertAllWithUnquotedTableSchema()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<BaseStatementBuilderDbConnection>();
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
        public void TestBaseStatementBuilderCreateInsertAllWithPrimary()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<BaseStatementBuilderDbConnection>();
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
        public void TestBaseStatementBuilderCreateInsertAllForThreeBatches()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<BaseStatementBuilderDbConnection>();
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
                $"INSERT INTO [Table] " +
                $"( [Field2], [Field3] ) " +
                $"VALUES " +
                $"( @Field2_1, @Field3_1 ) ; " +
                $"INSERT INTO [Table] " +
                $"( [Field2], [Field3] ) " +
                $"VALUES " +
                $"( @Field2_2, @Field3_2 ) ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestBaseStatementBuilderCreateInsertAllWithHints()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<BaseStatementBuilderDbConnection>();
            var queryBuilder = new QueryBuilder();
            var tableName = "Table";
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });

            // Act
            var actual = statementBuilder.CreateInsertAll(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields,
                batchSize: 1,
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

        [TestMethod]
        public void TestBaseStatementBuilderCreateInsertAllForThreeBatchesWithHints()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<BaseStatementBuilderDbConnection>();
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
                identityField: identityField,
                hints: "WITH (TABLOCK)");
            var expected = $"" +
                $"INSERT INTO [Table] WITH (TABLOCK) " +
                $"( [Field2], [Field3] ) " +
                $"VALUES " +
                $"( @Field2, @Field3 ) ; " +
                $"INSERT INTO [Table] WITH (TABLOCK) " +
                $"( [Field2], [Field3] ) " +
                $"VALUES " +
                $"( @Field2_1, @Field3_1 ) ; " +
                $"INSERT INTO [Table] WITH (TABLOCK) " +
                $"( [Field2], [Field3] ) " +
                $"VALUES " +
                $"( @Field2_2, @Field3_2 ) ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod, ExpectedException(typeof(PrimaryFieldNotFoundException))]
        public void ThrowExceptionOnBaseStatementBuilderCreateInsertAllIfTheNonIdentityPrimaryIsNotCovered()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<BaseStatementBuilderDbConnection>();
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
        public void ThrowExceptionOnBaseStatementBuilderCreateInsertAllIfThereAreNoFields()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<BaseStatementBuilderDbConnection>();
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
        public void ThrowExceptionOnBaseStatementBuilderCreateInsertAllIfTheTableIsNull()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<BaseStatementBuilderDbConnection>();
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
        public void ThrowExceptionOnBaseStatementBuilderCreateInsertAllIfTheTableIsEmpty()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<BaseStatementBuilderDbConnection>();
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
        public void ThrowExceptionOnBaseStatementBuilderCreateInsertAllIfTheTableIsWhitespace()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<BaseStatementBuilderDbConnection>();
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
        public void ThrowExceptionOnBaseStatementBuilderCreateInsertAllIfThePrimaryIsNotReallyAPrimary()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<BaseStatementBuilderDbConnection>();
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
        public void ThrowExceptionOnBaseStatementBuilderCreateInsertAllIfTheIdentityIsNotReallyAnIdentity()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<BaseStatementBuilderDbConnection>();
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

        [TestMethod, ExpectedException(typeof(NotSupportedException))]
        public void ThrowExceptionOnBaseStatementBuilderCreateInsertAllIfTheBatchSizeIsGreaterThan1AndTheMultipleStatementExecutionIsNotSupported()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<SingleStatementSupportBaseStatementBuilderDbConnection>();
            var queryBuilder = new QueryBuilder();
            var tableName = "Table";
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });
            var qualifiers = Field.From("Field1");
            var identifyField = new DbField("Field1", false, true, false, typeof(int), null, null, null, null);

            // Act
            statementBuilder.CreateInsertAll(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields,
                batchSize: 10,
                primaryField: null,
                identityField: identifyField);
        }
    }
}
