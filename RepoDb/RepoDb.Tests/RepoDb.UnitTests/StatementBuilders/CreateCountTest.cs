using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.UnitTests.CustomObjects;
using System;

namespace RepoDb.UnitTests.StatementBuilders
{
    [TestClass]
    public class BaseStatementBuilderCreateCountTest
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
        public void TestBaseStatementBuilderCreateCount()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get(typeof(BaseStatementBuilderDbConnection));
            var queryBuilder = new QueryBuilder();
            var tableName = "Table";

            // Act
            var actual = statementBuilder.CreateCount(queryBuilder: queryBuilder,
                tableName: tableName,
                hints: null);
            var expected = "SELECT COUNT_BIG (*) AS [CountValue] FROM [Table] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestBaseStatementBuilderCreateCountWithWhereExpression()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get(typeof(BaseStatementBuilderDbConnection));
            var queryBuilder = new QueryBuilder();
            var tableName = "Table";
            var where = new QueryGroup(new QueryField("Id", 1));

            // Act
            var actual = statementBuilder.CreateCount(queryBuilder: queryBuilder,
                tableName: tableName,
                where: where);
            var expected = $"" +
                $"SELECT COUNT_BIG (*) AS [CountValue] " +
                $"FROM [Table] " +
                $"WHERE ([Id] = @Id) ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestBaseStatementBuilderCreateCountWithHints()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get(typeof(BaseStatementBuilderDbConnection));
            var queryBuilder = new QueryBuilder();
            var tableName = "Table";
            var hints = "WITH (NOLOCK)";

            // Act
            var actual = statementBuilder.CreateCount(queryBuilder: queryBuilder,
                tableName: tableName,
                hints: hints);
            var expected = "SELECT COUNT_BIG (*) AS [CountValue] FROM [Table] WITH (NOLOCK) ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestBaseStatementBuilderCreateCountWithWhereExpressionAndWithHints()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get(typeof(BaseStatementBuilderDbConnection));
            var queryBuilder = new QueryBuilder();
            var tableName = "Table";
            var where = new QueryGroup(new QueryField("Id", 1));
            var hints = "WITH (NOLOCK)";

            // Act
            var actual = statementBuilder.CreateCount(queryBuilder: queryBuilder,
                tableName: tableName,
                where: where,
                hints: hints);
            var expected = $"" +
                $"SELECT COUNT_BIG (*) AS [CountValue] " +
                $"FROM [Table] WITH (NOLOCK) " +
                $"WHERE ([Id] = @Id) ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestBaseStatementBuilderCreateCountWithQuotedTableSchema()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get(typeof(BaseStatementBuilderDbConnection));
            var queryBuilder = new QueryBuilder();
            var tableName = "[dbo].[Table]";

            // Act
            var actual = statementBuilder.CreateCount(queryBuilder: queryBuilder,
                tableName: tableName,
                hints: null);
            var expected = "SELECT COUNT_BIG (*) AS [CountValue] FROM [dbo].[Table] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestBaseStatementBuilderCreateCountWithUnquotedTableSchema()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get(typeof(BaseStatementBuilderDbConnection));
            var queryBuilder = new QueryBuilder();
            var tableName = "dbo.Table";

            // Act
            var actual = statementBuilder.CreateCount(queryBuilder: queryBuilder,
                tableName: tableName,
                hints: null);
            var expected = "SELECT COUNT_BIG (*) AS [CountValue] FROM [dbo].[Table] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnBaseStatementBuilderCreateCountIfTheTableIsNull()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get(typeof(BaseStatementBuilderDbConnection));
            var queryBuilder = new QueryBuilder();
            var tableName = (string)null;

            // Act
            statementBuilder.CreateCount(queryBuilder: queryBuilder,
                tableName: tableName,
                hints: null);
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnBaseStatementBuilderCreateCountIfTheTableIsEmpty()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get(typeof(BaseStatementBuilderDbConnection));
            var queryBuilder = new QueryBuilder();
            var tableName = "";

            // Act
            statementBuilder.CreateCount(queryBuilder: queryBuilder,
                tableName: tableName,
                hints: null);
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnBaseStatementBuilderCreateCountIfTheTableIsWhitespace()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get(typeof(BaseStatementBuilderDbConnection));
            var queryBuilder = new QueryBuilder();
            var tableName = " ";

            // Act
            statementBuilder.CreateCount(queryBuilder: queryBuilder,
                tableName: tableName,
                hints: null);
        }

        [TestMethod, ExpectedException(typeof(NotSupportedException))]
        public void ThrowExceptionOnBaseStatementBuilderCreateCountIIfTheHintsAreNotSupported()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get(typeof(NonHintsSupportingBaseStatementBuilderDbConnection));
            var queryBuilder = new QueryBuilder();
            var tableName = "Table";

            // Act
            statementBuilder.CreateCount(queryBuilder: queryBuilder,
                tableName: tableName,
                hints: "Hints");
        }
    }
}
