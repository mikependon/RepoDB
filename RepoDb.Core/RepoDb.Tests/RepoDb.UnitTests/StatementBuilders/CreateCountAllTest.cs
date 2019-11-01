using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.UnitTests.CustomObjects;
using System;

namespace RepoDb.UnitTests.StatementBuilders
{
    [TestClass]
    public class BaseStatementBuilderCreateCountAllTest
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
        public void TestBaseStatementBuilderCreateCountAll()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get(typeof(BaseStatementBuilderDbConnection));
            var queryBuilder = new QueryBuilder();
            var tableName = "Table";

            // Act
            var actual = statementBuilder.CreateCountAll(queryBuilder: queryBuilder,
                tableName: tableName,
                hints: null);
            var expected = "SELECT COUNT_BIG (*) AS [CountValue] FROM [Table] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestBaseStatementBuilderCreateCountAllWithHints()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get(typeof(BaseStatementBuilderDbConnection));
            var queryBuilder = new QueryBuilder();
            var tableName = "Table";
            var hints = "WITH (NOLOCK)";

            // Act
            var actual = statementBuilder.CreateCountAll(queryBuilder: queryBuilder,
                tableName: tableName,
                hints: hints);
            var expected = "SELECT COUNT_BIG (*) AS [CountValue] FROM [Table] WITH (NOLOCK) ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestBaseStatementBuilderCreateCountAllWithQuotedTableSchema()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get(typeof(BaseStatementBuilderDbConnection));
            var queryBuilder = new QueryBuilder();
            var tableName = "[dbo].[Table]";

            // Act
            var actual = statementBuilder.CreateCountAll(queryBuilder: queryBuilder,
                tableName: tableName,
                hints: null);
            var expected = "SELECT COUNT_BIG (*) AS [CountValue] FROM [dbo].[Table] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestBaseStatementBuilderCreateCountAllWithUnquotedTableSchema()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get(typeof(BaseStatementBuilderDbConnection));
            var queryBuilder = new QueryBuilder();
            var tableName = "dbo.Table";

            // Act
            var actual = statementBuilder.CreateCountAll(queryBuilder: queryBuilder,
                tableName: tableName,
                hints: null);
            var expected = "SELECT COUNT_BIG (*) AS [CountValue] FROM [dbo].[Table] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnBaseStatementBuilderCreateCountAllIfTheTableIsNull()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get(typeof(BaseStatementBuilderDbConnection));
            var queryBuilder = new QueryBuilder();
            var tableName = (string)null;

            // Act
            statementBuilder.CreateCountAll(queryBuilder: queryBuilder,
                tableName: tableName,
                hints: null);
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnBaseStatementBuilderCreateCountAllIfTheTableIsEmpty()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get(typeof(BaseStatementBuilderDbConnection));
            var queryBuilder = new QueryBuilder();
            var tableName = "";

            // Act
            statementBuilder.CreateCountAll(queryBuilder: queryBuilder,
                tableName: tableName,
                hints: null);
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnBaseStatementBuilderCreateCountAllIfTheTableIsWhitespace()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get(typeof(BaseStatementBuilderDbConnection));
            var queryBuilder = new QueryBuilder();
            var tableName = " ";

            // Act
            statementBuilder.CreateCountAll(queryBuilder: queryBuilder,
                tableName: tableName,
                hints: null);
        }

        [TestMethod, ExpectedException(typeof(NotSupportedException))]
        public void ThrowExceptionOnBaseStatementBuilderCreateCountAllIIfTheHintsAreNotSupported()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get(typeof(NonHintsSupportingBaseStatementBuilderDbConnection));
            var queryBuilder = new QueryBuilder();
            var tableName = "Table";

            // Act
            statementBuilder.CreateCountAll(queryBuilder: queryBuilder,
                tableName: tableName,
                hints: "Hints");
        }
    }
}
