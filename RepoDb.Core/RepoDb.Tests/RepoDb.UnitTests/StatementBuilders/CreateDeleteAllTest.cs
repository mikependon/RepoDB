using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.UnitTests.CustomObjects;
using System;

namespace RepoDb.UnitTests.StatementBuilders
{
    [TestClass]
    public class BaseStatementBuilderCreateDeleteAllTest
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
        public void TestBaseStatementBuilderCreateDeleteAll()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get(typeof(BaseStatementBuilderDbConnection));
            var queryBuilder = new QueryBuilder();
            var tableName = "Table";

            // Act
            var actual = statementBuilder.CreateDeleteAll(queryBuilder: queryBuilder,
                tableName: tableName);
            var expected = "DELETE FROM [Table] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestBaseStatementBuilderCreateDeleteAllWithQuotedTableSchema()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get(typeof(BaseStatementBuilderDbConnection));
            var queryBuilder = new QueryBuilder();
            var tableName = "[dbo].[Table]";

            // Act
            var actual = statementBuilder.CreateDeleteAll(queryBuilder: queryBuilder,
                tableName: tableName);
            var expected = "DELETE FROM [dbo].[Table] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestBaseStatementBuilderCreateDeleteAllWithUnquotedTableSchema()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get(typeof(BaseStatementBuilderDbConnection));
            var queryBuilder = new QueryBuilder();
            var tableName = "dbo.Table";

            // Act
            var actual = statementBuilder.CreateDeleteAll(queryBuilder: queryBuilder,
                tableName: tableName);
            var expected = "DELETE FROM [dbo].[Table] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnBaseStatementBuilderCreateDeleteAllIfTheTableIsNull()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get(typeof(BaseStatementBuilderDbConnection));
            var queryBuilder = new QueryBuilder();
            var tableName = (string)null;

            // Act
            statementBuilder.CreateDeleteAll(queryBuilder: queryBuilder,
                tableName: tableName);
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnBaseStatementBuilderCreateDeleteAllIfTheTableIsEmpty()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get(typeof(BaseStatementBuilderDbConnection));
            var queryBuilder = new QueryBuilder();
            var tableName = "";

            // Act
            statementBuilder.CreateDeleteAll(queryBuilder: queryBuilder,
                tableName: tableName);
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnBaseStatementBuilderCreateDeleteAllIfTheTableIsWhitespace()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get(typeof(BaseStatementBuilderDbConnection));
            var queryBuilder = new QueryBuilder();
            var tableName = " ";

            // Act
            statementBuilder.CreateDeleteAll(queryBuilder: queryBuilder,
                tableName: tableName);
        }
    }
}
