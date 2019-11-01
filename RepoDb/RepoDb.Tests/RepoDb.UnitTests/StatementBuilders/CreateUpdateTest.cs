using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.UnitTests.CustomObjects;
using System;

namespace RepoDb.UnitTests.StatementBuilders
{
    [TestClass]
    public class BaseStatementBuilderCreateUpdateTest
    {
        [TestInitialize]
        public void Initialize()
        {
            StatementBuilderMapper.Add(typeof(BaseStatementBuilderDbConnection), new CustomBaseStatementBuilder(), true);
        }

        #region SubClasses

        private class BaseStatementBuilderDbConnection : CustomDbConnection { }

        #endregion

        [TestMethod]
        public void TestBaseStatementBuilderCreateUpdate()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get(typeof(BaseStatementBuilderDbConnection));
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
        public void TestBaseStatementBuilderCreateUpdateWithQuotedTableSchema()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get(typeof(BaseStatementBuilderDbConnection));
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
        public void TestBaseStatementBuilderCreateUpdateWithUnquotedTableSchema()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get(typeof(BaseStatementBuilderDbConnection));
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
        public void TestBaseStatementBuilderCreateUpdateWithWhereExpression()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get(typeof(BaseStatementBuilderDbConnection));
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
        public void TestBaseStatementBuilderCreateUpdateWithCoveredPrimaryField()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get(typeof(BaseStatementBuilderDbConnection));
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
        public void TestBaseStatementBuilderCreateUpdateWithCoveredIdentityField()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get(typeof(BaseStatementBuilderDbConnection));
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
        public void TestBaseStatementBuilderCreateUpdateWithCoveredPrimaryAsIdentity()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get(typeof(BaseStatementBuilderDbConnection));
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
        public void TestBaseStatementBuilderCreateUpdateWithUncoveredWhereExpressionAndWithUncoveredPrimary()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get(typeof(BaseStatementBuilderDbConnection));
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
        public void TestBaseStatementBuilderCreateUpdateWithUncoveredWhereExpressionAndWithUncoveredIdentity()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get(typeof(BaseStatementBuilderDbConnection));
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
        public void TestBaseStatementBuilderCreateUpdateWithUncoveredWhereExpressionAndWithUncoveredPrimaryAsIdentity()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get(typeof(BaseStatementBuilderDbConnection));
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
        public void TestBaseStatementBuilderCreateUpdateWithUncoveredWhereExpressionButWithCoveredPrimary()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get(typeof(BaseStatementBuilderDbConnection));
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
        public void TestBaseStatementBuilderCreateUpdateWithUncoveredWhereExpressionButWithCoveredIdentity()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get(typeof(BaseStatementBuilderDbConnection));
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
        public void ThrowExceptionOnBaseStatementBuilderCreateUpdateIfTheTableIsNull()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get(typeof(BaseStatementBuilderDbConnection));
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
        public void ThrowExceptionOnBaseStatementBuilderCreateUpdateIfTheTableIsEmpty()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get(typeof(BaseStatementBuilderDbConnection));
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
        public void ThrowExceptionOnBaseStatementBuilderCreateUpdateIfTheTableIsWhitespace()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get(typeof(BaseStatementBuilderDbConnection));
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
        public void ThrowExceptionOnBaseStatementBuilderCreateUpdateIfThePrimaryIsNotReallyAPrimary()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get(typeof(BaseStatementBuilderDbConnection));
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
        public void ThrowExceptionOnBaseStatementBuilderCreateUpdateIfTheIdentityIsNotReallyAnIdentity()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get(typeof(BaseStatementBuilderDbConnection));
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
