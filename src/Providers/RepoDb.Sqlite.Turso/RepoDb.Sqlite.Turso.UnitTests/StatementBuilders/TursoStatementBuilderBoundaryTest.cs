using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.DbSettings;
using RepoDb.Exceptions;
using RepoDb.StatementBuilders;
using System;

namespace RepoDb.Sqlite.Turso.UnitTests.StatementBuilders
{
    /// <summary>
    /// Covers boundary cases and supported mappings.
    /// </summary>
    [TestClass]
    public class TursoStatementBuilderBoundaryTest
    {
        #region Public Methods

        /// <summary>
        /// Rejects both absent and empty field lists.
        /// </summary>
        /// <param name="empty">Whether to supply an empty list.</param>
        [DataRow(false)]
        [DataRow(true)]
        [TestMethod]
        public void CreateBatchQueryRejectsMissingFields(bool empty)
        {
            // Setup
            var builder = new TursoStatementBuilder(new TursoDbSetting());
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
        /// <param name="empty">Whether to supply an empty list.</param>
        [DataRow(false)]
        [DataRow(true)]
        [TestMethod]
        public void CreateInsertAllRejectsMissingFields(bool empty)
        {
            // Setup
            var builder = new TursoStatementBuilder(new TursoDbSetting());
            var fields = empty ? Array.Empty<Field>() : null;


            // Act
            Action action = () => builder.CreateInsertAll("Table",
                fields,
                batchSize: 1);


            // Assert
            Assert.ThrowsExactly<EmptyException>(action);
        }

        /// <summary>
        /// Rejects both absent and empty field lists.
        /// </summary>
        /// <param name="empty">Whether to supply an empty list.</param>
        [DataRow(false)]
        [DataRow(true)]
        [TestMethod]
        public void CreateSkipQueryRejectsMissingFields(bool empty)
        {
            // Setup
            var builder = new TursoStatementBuilder(new TursoDbSetting());
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