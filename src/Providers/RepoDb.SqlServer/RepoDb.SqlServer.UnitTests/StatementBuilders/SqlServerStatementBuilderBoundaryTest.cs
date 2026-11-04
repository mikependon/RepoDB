using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.DbSettings;
using RepoDb.Exceptions;
using RepoDb.StatementBuilders;
using System;

namespace RepoDb.SqlServer.UnitTests.StatementBuilders
{
    /// <summary>
    /// Covers boundary cases and supported mappings.
    /// </summary>
    [TestClass]
    public class SqlServerStatementBuilderBoundaryTest
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
            var builder = new SqlServerStatementBuilder(new SqlServerDbSetting());
            var fields = empty ? Array.Empty<Field>() : null;

            // Act
            Action action = () => builder.CreateBatchQuery("Table",
                fields,
                0,
                10);

            // Assert
            Assert.ThrowsExactly<MissingFieldsException>(action);
        }

        /// <summary>
        /// Rejects both absent and empty field lists.
        /// </summary>
        [TestMethod]
        public void CreateBatchQueryRejectsMissingFieldsForEmptyFields()
        {
            // Setup
            bool empty = true;
            var builder = new SqlServerStatementBuilder(new SqlServerDbSetting());
            var fields = empty ? Array.Empty<Field>() : null;

            // Act
            Action action = () => builder.CreateBatchQuery("Table",
                fields,
                0,
                10);

            // Assert
            Assert.ThrowsExactly<MissingFieldsException>(action);
        }

        /// <summary>
        /// Rejects both absent and empty field lists.
        /// </summary>
        [TestMethod]
        public void CreateMergeRejectsMissingFieldsForNullFields()
        {
            // Setup
            bool empty = false;
            var builder = new SqlServerStatementBuilder(new SqlServerDbSetting());
            var fields = empty ? Array.Empty<Field>() : null;

            // Act
            Action action = () => builder.CreateMerge("Table",
                fields);

            // Assert
            Assert.ThrowsExactly<MissingFieldsException>(action);
        }

        /// <summary>
        /// Rejects both absent and empty field lists.
        /// </summary>
        [TestMethod]
        public void CreateMergeRejectsMissingFieldsForEmptyFields()
        {
            // Setup
            bool empty = true;
            var builder = new SqlServerStatementBuilder(new SqlServerDbSetting());
            var fields = empty ? Array.Empty<Field>() : null;

            // Act
            Action action = () => builder.CreateMerge("Table",
                fields);

            // Assert
            Assert.ThrowsExactly<MissingFieldsException>(action);
        }

        /// <summary>
        /// Rejects both absent and empty field lists.
        /// </summary>
        [TestMethod]
        public void CreateMergeAllRejectsMissingFieldsForNullFields()
        {
            // Setup
            bool empty = false;
            var builder = new SqlServerStatementBuilder(new SqlServerDbSetting());
            var fields = empty ? Array.Empty<Field>() : null;

            // Act
            Action action = () => builder.CreateMergeAll("Table",
                fields,
                batchSize: 1);

            // Assert
            Assert.ThrowsExactly<MissingFieldsException>(action);
        }

        /// <summary>
        /// Rejects both absent and empty field lists.
        /// </summary>
        [TestMethod]
        public void CreateMergeAllRejectsMissingFieldsForEmptyFields()
        {
            // Setup
            bool empty = true;
            var builder = new SqlServerStatementBuilder(new SqlServerDbSetting());
            var fields = empty ? Array.Empty<Field>() : null;

            // Act
            Action action = () => builder.CreateMergeAll("Table",
                fields,
                batchSize: 1);

            // Assert
            Assert.ThrowsExactly<MissingFieldsException>(action);
        }

        /// <summary>
        /// Rejects both absent and empty field lists.
        /// </summary>
        [TestMethod]
        public void CreateSkipQueryRejectsMissingFieldsForNullFields()
        {
            // Setup
            bool empty = false;
            var builder = new SqlServerStatementBuilder(new SqlServerDbSetting());
            var fields = empty ? Array.Empty<Field>() : null;

            // Act
            Action action = () => builder.CreateSkipQuery("Table",
                fields,
                0,
                10);

            // Assert
            Assert.ThrowsExactly<MissingFieldsException>(action);
        }

        /// <summary>
        /// Rejects both absent and empty field lists.
        /// </summary>
        [TestMethod]
        public void CreateSkipQueryRejectsMissingFieldsForEmptyFields()
        {
            // Setup
            bool empty = true;
            var builder = new SqlServerStatementBuilder(new SqlServerDbSetting());
            var fields = empty ? Array.Empty<Field>() : null;

            // Act
            Action action = () => builder.CreateSkipQuery("Table",
                fields,
                0,
                10);

            // Assert
            Assert.ThrowsExactly<MissingFieldsException>(action);
        }

        #endregion
    }
}