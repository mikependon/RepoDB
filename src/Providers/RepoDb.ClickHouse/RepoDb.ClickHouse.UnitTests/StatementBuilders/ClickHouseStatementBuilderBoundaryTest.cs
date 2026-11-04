using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.DbSettings;
using RepoDb.Exceptions;
using RepoDb.StatementBuilders;
using System;

namespace RepoDb.ClickHouse.UnitTests.StatementBuilders
{
    /// <summary>
    /// Covers boundary cases and supported mappings.
    /// </summary>
    [TestClass]
    public class ClickHouseStatementBuilderBoundaryTest
    {
        #region Public Methods

        /// <summary>
        /// Rejects both absent and empty field lists.
        /// </summary>
        [TestMethod]
        public void CreateBatchQueryRejectsMissingFieldsForNullFields()
        {
            // Setup
            bool empty = false;
            var builder = new ClickHouseStatementBuilder(new ClickHouseDbSetting());
            var fields = empty ? Array.Empty<Field>() : null;

            // Act
            Action action = () => builder.CreateBatchQuery("Table",
                fields,
                0,
                10);

            // Assert
            Assert.ThrowsExactly<ArgumentNullException>(action);
        }

        /// <summary>
        /// Rejects both absent and empty field lists.
        /// </summary>
        [TestMethod]
        public void CreateBatchQueryRejectsMissingFieldsForEmptyFields()
        {
            // Setup
            bool empty = true;
            var builder = new ClickHouseStatementBuilder(new ClickHouseDbSetting());
            var fields = empty ? Array.Empty<Field>() : null;

            // Act
            Action action = () => builder.CreateBatchQuery("Table",
                fields,
                0,
                10);

            // Assert
            Assert.ThrowsExactly<ArgumentNullException>(action);
        }

        /// <summary>
        /// Rejects both absent and empty field lists.
        /// </summary>
        [TestMethod]
        public void CreateSkipQueryRejectsMissingFieldsForNullFields()
        {
            // Setup
            bool empty = false;
            var builder = new ClickHouseStatementBuilder(new ClickHouseDbSetting());
            var fields = empty ? Array.Empty<Field>() : null;

            // Act
            Action action = () => builder.CreateSkipQuery("Table",
                fields,
                0,
                10);

            // Assert
            Assert.ThrowsExactly<ArgumentNullException>(action);
        }

        /// <summary>
        /// Rejects both absent and empty field lists.
        /// </summary>
        [TestMethod]
        public void CreateSkipQueryRejectsMissingFieldsForEmptyFields()
        {
            // Setup
            bool empty = true;
            var builder = new ClickHouseStatementBuilder(new ClickHouseDbSetting());
            var fields = empty ? Array.Empty<Field>() : null;

            // Act
            Action action = () => builder.CreateSkipQuery("Table",
                fields,
                0,
                10);

            // Assert
            Assert.ThrowsExactly<ArgumentNullException>(action);
        }

        #endregion
    }
}