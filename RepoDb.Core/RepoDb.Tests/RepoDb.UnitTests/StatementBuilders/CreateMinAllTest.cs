using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.UnitTests.CustomObjects;
using System;

namespace RepoDb.UnitTests.StatementBuilders
{
    [TestClass]
    public class BaseStatementBuilderCreateMinAllTest
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
        public void TestBaseStatementBuilderCreateMinAll()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get(typeof(BaseStatementBuilderDbConnection));
            var queryBuilder = new QueryBuilder();
            var tableName = "Table";
            var field = new Field("Value");

            // Act
            var actual = statementBuilder.CreateMinAll(queryBuilder: queryBuilder,
                field: field,
                tableName: tableName,
                hints: null);
            var expected = "SELECT MIN ([Value]) AS [MinValue] FROM [Table] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestBaseStatementBuilderCreateMinAllWithHints()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get(typeof(BaseStatementBuilderDbConnection));
            var queryBuilder = new QueryBuilder();
            var tableName = "Table";
            var field = new Field("Value");
            var hints = "WITH (NOLOCK)";

            // Act
            var actual = statementBuilder.CreateMinAll(queryBuilder: queryBuilder,
                tableName: tableName,
                field: field,
                hints: hints);
            var expected = "SELECT MIN ([Value]) AS [MinValue] FROM [Table] WITH (NOLOCK) ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestBaseStatementBuilderCreateMinAllWithQuotedTableSchema()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get(typeof(BaseStatementBuilderDbConnection));
            var queryBuilder = new QueryBuilder();
            var tableName = "[dbo].[Table]";
            var field = new Field("Value");

            // Act
            var actual = statementBuilder.CreateMinAll(queryBuilder: queryBuilder,
                tableName: tableName,
                field: field,
                hints: null);
            var expected = "SELECT MIN ([Value]) AS [MinValue] FROM [dbo].[Table] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestBaseStatementBuilderCreateMinAllWithUnquotedTableSchema()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get(typeof(BaseStatementBuilderDbConnection));
            var queryBuilder = new QueryBuilder();
            var tableName = "dbo.Table";
            var field = new Field("Value");

            // Act
            var actual = statementBuilder.CreateMinAll(queryBuilder: queryBuilder,
                tableName: tableName,
                field: field,
                hints: null);
            var expected = "SELECT MIN ([Value]) AS [MinValue] FROM [dbo].[Table] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnBaseStatementBuilderCreateMinAllIfTheTableIsNull()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get(typeof(BaseStatementBuilderDbConnection));
            var queryBuilder = new QueryBuilder();
            var tableName = (string)null;
            var field = new Field("Value");

            // Act
            statementBuilder.CreateMinAll(queryBuilder: queryBuilder,
                tableName: tableName,
                field: field,
                hints: null);
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnBaseStatementBuilderCreateMinAllIfTheTableIsEmpty()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get(typeof(BaseStatementBuilderDbConnection));
            var queryBuilder = new QueryBuilder();
            var tableName = "";
            var field = new Field("Value");

            // Act
            statementBuilder.CreateMinAll(queryBuilder: queryBuilder,
                tableName: tableName,
                field: field,
                hints: null);
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnBaseStatementBuilderCreateMinAllIfTheTableIsWhitespace()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get(typeof(BaseStatementBuilderDbConnection));
            var queryBuilder = new QueryBuilder();
            var tableName = " ";
            var field = new Field("Value");

            // Act
            statementBuilder.CreateMinAll(queryBuilder: queryBuilder,
                tableName: tableName,
                field: field,
                hints: null);
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnBaseStatementBuilderCreateMinAllIfTheFieldIsNull()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get(typeof(BaseStatementBuilderDbConnection));
            var queryBuilder = new QueryBuilder();
            var tableName = " ";

            // Act
            statementBuilder.CreateMinAll(queryBuilder: queryBuilder,
                tableName: tableName,
                field: null,
                hints: null);
        }

        [TestMethod, ExpectedException(typeof(NotSupportedException))]
        public void ThrowExceptionOnBaseStatementBuilderCreateMinAllIIfTheHintsAreNotSupported()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get(typeof(NonHintsSupportingBaseStatementBuilderDbConnection));
            var queryBuilder = new QueryBuilder();
            var tableName = "Table";
            var field = new Field("Value");

            // Act
            statementBuilder.CreateMinAll(queryBuilder: queryBuilder,
                tableName: tableName,
                field: field,
                hints: "Hints");
        }
    }
}
