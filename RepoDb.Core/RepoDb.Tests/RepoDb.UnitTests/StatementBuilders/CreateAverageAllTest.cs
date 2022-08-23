using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.UnitTests.CustomObjects;
using System;

namespace RepoDb.UnitTests.StatementBuilders
{
    [TestClass]
    public class BaseStatementBuilderCreateAverageAllTest
    {
        [TestInitialize]
        public void Initialize()
        {
            StatementBuilderMapper.Add<BaseStatementBuilderDbConnection>(new CustomBaseStatementBuilder(), true);
            StatementBuilderMapper.Add<DefinedBaseStatementBuilderDbConnection>(new CustomDefinedBaseStatementBuilder(), true);
            StatementBuilderMapper.Add<NonHintsSupportingBaseStatementBuilderDbConnection>(new CustomNonHintsSupportingBaseStatementBuilder(), true);
        }

        #region SubClasses

        private class BaseStatementBuilderDbConnection : CustomDbConnection { }

        private class DefinedBaseStatementBuilderDbConnection : CustomDbConnection { }

        private class NonHintsSupportingBaseStatementBuilderDbConnection : CustomDbConnection { }

        #endregion

        [TestMethod]
        public void TestBaseStatementBuilderCreateAverageAll()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<BaseStatementBuilderDbConnection>();
            var tableName = "Table";
            var field = new Field("Value");

            // Act
            var actual = statementBuilder.CreateAverageAll(field: field,
                tableName: tableName,
                hints: null);
            var expected = "SELECT AVG ([Value]) AS [AverageValue] FROM [Table] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestBaseStatementBuilderCreateAverageAllWithHints()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<BaseStatementBuilderDbConnection>();
            var tableName = "Table";
            var field = new Field("Value");
            var hints = "WITH (NOLOCK)";

            // Act
            var actual = statementBuilder.CreateAverageAll(tableName: tableName,
                field: field,
                hints: hints);
            var expected = "SELECT AVG ([Value]) AS [AverageValue] FROM [Table] WITH (NOLOCK) ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestBaseStatementBuilderCreateAverageAllWithQuotedTableSchema()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<BaseStatementBuilderDbConnection>();
            var tableName = "[dbo].[Table]";
            var field = new Field("Value");

            // Act
            var actual = statementBuilder.CreateAverageAll(tableName: tableName,
                field: field,
                hints: null);
            var expected = "SELECT AVG ([Value]) AS [AverageValue] FROM [dbo].[Table] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestBaseStatementBuilderCreateAverageAllWithUnquotedTableSchema()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<BaseStatementBuilderDbConnection>();
            var tableName = "dbo.Table";
            var field = new Field("Value");

            // Act
            var actual = statementBuilder.CreateAverageAll(tableName: tableName,
                field: field,
                hints: null);
            var expected = "SELECT AVG ([Value]) AS [AverageValue] FROM [dbo].[Table] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestBaseStatementBuilderCreateAverageAllForOtherAverageableFieldType()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<BaseStatementBuilderDbConnection>();
            var tableName = "Table";
            var field = new Field("Value", typeof(long));

            // Act
            var actual = statementBuilder.CreateAverageAll(field: field,
                tableName: tableName,
                hints: null);
            var expected = "SELECT AVG ([Value]) AS [AverageValue] FROM [Table] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestBaseStatementBuilderCreateAverageAllForFieldConverter()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DefinedBaseStatementBuilderDbConnection>();
            var tableName = "Table";
            var field = new Field("Value", typeof(int));

            // Act
            var actual = statementBuilder.CreateAverageAll(field: field,
                tableName: tableName,
                hints: null);
            var expected = "SELECT AVG (CONVERT([FLOAT], [Value])) AS [AverageValue] FROM [Table] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestBaseStatementBuilderCreateAverageAllForNonAverageableFieldType()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<DefinedBaseStatementBuilderDbConnection>();
            var tableName = "Table";
            var field = new Field("Value", typeof(string));

            // Act
            var actual = statementBuilder.CreateAverageAll(field: field,
                tableName: tableName,
                hints: null);
            var expected = "SELECT AVG (CONVERT([NVARCHAR], [Value])) AS [AverageValue] FROM [Table] ;";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnBaseStatementBuilderCreateAverageAllIfTheTableIsNull()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<BaseStatementBuilderDbConnection>();
            var tableName = (string)null;
            var field = new Field("Value");

            // Act
            statementBuilder.CreateAverageAll(tableName: tableName,
                field: field,
                hints: null);
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnBaseStatementBuilderCreateAverageAllIfTheTableIsEmpty()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<BaseStatementBuilderDbConnection>();
            var tableName = "";
            var field = new Field("Value");

            // Act
            statementBuilder.CreateAverageAll(tableName: tableName,
                field: field,
                hints: null);
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnBaseStatementBuilderCreateAverageAllIfTheTableIsWhitespace()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<BaseStatementBuilderDbConnection>();
            var tableName = " ";
            var field = new Field("Value");

            // Act
            statementBuilder.CreateAverageAll(tableName: tableName,
                field: field,
                hints: null);
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnBaseStatementBuilderCreateAverageAllIfTheFieldIsNull()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<BaseStatementBuilderDbConnection>();
            var tableName = " ";

            // Act
            statementBuilder.CreateAverageAll(tableName: tableName,
                field: null,
                hints: null);
        }

        [TestMethod, ExpectedException(typeof(NotSupportedException))]
        public void ThrowExceptionOnBaseStatementBuilderCreateAverageAllIfTheHintsAreNotSupported()
        {
            // Setup
            var statementBuilder = StatementBuilderMapper.Get<NonHintsSupportingBaseStatementBuilderDbConnection>();
            var tableName = "Table";
            var field = new Field("Value");

            // Act
            statementBuilder.CreateAverageAll(tableName: tableName,
                field: field,
                hints: "Hints");
        }
    }
}
