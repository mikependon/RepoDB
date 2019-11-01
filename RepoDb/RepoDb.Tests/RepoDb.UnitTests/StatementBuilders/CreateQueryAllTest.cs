using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Enumerations;
using RepoDb.Exceptions;
using RepoDb.UnitTests.CustomObjects;
using System;

namespace RepoDb.UnitTests.StatementBuilders
{
    [TestClass]
    public class BaseStatementBuilderCreateQueryAllTest
    {
        [TestInitialize]
        public void Initialize()
        {
            StatementBuilderMapper.Add(typeof(BaseStatementBuilderDbConnection), new CustomBaseStatementBuilder(), true);
            StatementBuilderMapper.Add(typeof(NonHintsSupportingBaseStatementBuilderDbConnection), new CustomNonHintsSupportingBaseStatementBuilder(), true);
        }

        #region SubClasses

        private class BaseStatementBuilderDbConnection : CustomDbConnection { }

        private class NonHintsSupportingBaseStatementBuilderDbConnection : CustomDbConnection { }

        #endregion

        [TestMethod]
        public void TestBaseStatementBuilderCreateQueryAll()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get(typeof(BaseStatementBuilderDbConnection));
            var queryBuilder = new QueryBuilder();
            var tableName = "Table";
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });

            // Act
            var actual = statementBuilder.CreateQueryAll(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields);
            var expected = "SELECT [Field1], [Field2], [Field3] FROM [Table] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestBaseStatementBuilderCreateQueryAllWithQuotedTableSchema()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get(typeof(BaseStatementBuilderDbConnection));
            var queryBuilder = new QueryBuilder();
            var tableName = "[dbo].[Table]";
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });

            // Act
            var actual = statementBuilder.CreateQueryAll(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields);
            var expected = "SELECT [Field1], [Field2], [Field3] FROM [dbo].[Table] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestBaseStatementBuilderCreateQueryAllWithUnquotedTableSchema()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get(typeof(BaseStatementBuilderDbConnection));
            var queryBuilder = new QueryBuilder();
            var tableName = "dbo.Table";
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });

            // Act
            var actual = statementBuilder.CreateQueryAll(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields);
            var expected = "SELECT [Field1], [Field2], [Field3] FROM [dbo].[Table] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestBaseStatementBuilderCreateQueryAllWithOrderBy()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get(typeof(BaseStatementBuilderDbConnection));
            var queryBuilder = new QueryBuilder();
            var tableName = "Table";
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });
            var orderBy = OrderField.Parse(new { Field1 = Order.Ascending, Field2 = Order.Descending });

            // Act
            var actual = statementBuilder.CreateQueryAll(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields,
                orderBy: orderBy);
            var expected = $"" +
                $"SELECT [Field1], [Field2], [Field3] " +
                $"FROM [Table] " +
                $"ORDER BY [Field1] ASC, [Field2] DESC ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestBaseStatementBuilderCreateQueryAllWithHints()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get(typeof(BaseStatementBuilderDbConnection));
            var queryBuilder = new QueryBuilder();
            var tableName = "Table";
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });
            var hints = SqlServerTableHints.NoLock;

            // Act
            var actual = statementBuilder.CreateQueryAll(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields,
                hints: hints);
            var expected = "SELECT [Field1], [Field2], [Field3] FROM [Table] WITH (NOLOCK) ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestBaseStatementBuilderCreateQueryAllWithOrderByAndWithHints()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get(typeof(BaseStatementBuilderDbConnection));
            var queryBuilder = new QueryBuilder();
            var tableName = "Table";
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });
            var orderBy = OrderField.Parse(new { Field1 = Order.Ascending, Field2 = Order.Descending });
            var hints = SqlServerTableHints.NoLock;

            // Act
            var actual = statementBuilder.CreateQueryAll(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields,
                orderBy: orderBy,
                hints: hints);
            var expected = $"" +
                $"SELECT [Field1], [Field2], [Field3] " +
                $"FROM [Table] WITH (NOLOCK) " +
                $"ORDER BY [Field1] ASC, [Field2] DESC ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod, ExpectedException(typeof(MissingFieldsException))]
        public void ThrowExceptionOnBaseStatementBuilderCreateQueryAllIfTheOrderFieldIsNotCovered()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get(typeof(BaseStatementBuilderDbConnection));
            var queryBuilder = new QueryBuilder();
            var tableName = "Table";
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });
            var orderBy = OrderField.Parse(new { Id = Order.Ascending, Field1 = Order.Ascending });

            // Act
            statementBuilder.CreateQueryAll(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields,
                orderBy: orderBy);
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnBaseStatementBuilderCreateQueryAllIfThereAreNoFields()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get(typeof(BaseStatementBuilderDbConnection));
            var queryBuilder = new QueryBuilder();
            var tableName = "Table";

            // Act
            statementBuilder.CreateQueryAll(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: null);
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnBaseStatementBuilderCreateQueryAllIfTheTableIsNull()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get(typeof(BaseStatementBuilderDbConnection));
            var queryBuilder = new QueryBuilder();
            var tableName = (string)null;
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });

            // Act
            statementBuilder.CreateQueryAll(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields);
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnBaseStatementBuilderCreateQueryAllIfTheTableIsEmpty()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get(typeof(BaseStatementBuilderDbConnection));
            var queryBuilder = new QueryBuilder();
            var tableName = "";
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });

            // Act
            statementBuilder.CreateQueryAll(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields);
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnBaseStatementBuilderCreateQueryAllIfTheTableIsWhitespace()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get(typeof(BaseStatementBuilderDbConnection));
            var queryBuilder = new QueryBuilder();
            var tableName = " ";
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });

            // Act
            statementBuilder.CreateQueryAll(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields);
        }

        [TestMethod, ExpectedException(typeof(NotSupportedException))]
        public void ThrowExceptionOnBaseStatementBuilderCreateQueryAllIfTheHintsAreNotSupported()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get(typeof(NonHintsSupportingBaseStatementBuilderDbConnection));
            var queryBuilder = new QueryBuilder();
            var tableName = "Table";
            var fields = Field.From(new[] { "Field1", "Field2", "Field3" });

            // Act
            statementBuilder.CreateQueryAll(queryBuilder: queryBuilder,
                tableName: tableName,
                fields: fields,
                hints: "Hints");
        }
    }
}
