using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.DbSettings;
using RepoDb.Exceptions;
using RepoDb.StatementBuilders;
using System;

namespace RepoDb.MariaDb.UnitTests.StatementBuilders
{
    /// <summary>
    /// Covers boundary cases and supported mappings.
    /// </summary>
    [TestClass]
    public class MariaDbStatementBuilderBoundaryTest
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
            var builder = new MariaDbStatementBuilder(new MariaDbDbSetting());
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
            var builder = new MariaDbStatementBuilder(new MariaDbDbSetting());
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
        public void CreateMergeRejectsMissingFields(bool empty)
        {
            // Setup
            var builder = new MariaDbStatementBuilder(new MariaDbDbSetting());
            var fields = empty ? Array.Empty<Field>() : null;


            // Act
            Action action = () => builder.CreateMerge("Table",
                fields);


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
        public void CreateMergeAllRejectsMissingFields(bool empty)
        {
            // Setup
            var builder = new MariaDbStatementBuilder(new MariaDbDbSetting());
            var fields = empty ? Array.Empty<Field>() : null;


            // Act
            Action action = () => builder.CreateMergeAll("Table",
                fields,
                batchSize: 1);


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
        public void CreateSkipQueryRejectsMissingFields(bool empty)
        {
            // Setup
            var builder = new MariaDbStatementBuilder(new MariaDbDbSetting());
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
        /// Preserves unsigned primary key representations.
        /// </summary>
        /// <param name="type">The unsigned key type.</param>
        [DataRow(typeof(byte))]
        [DataRow(typeof(ushort))]
        [DataRow(typeof(uint))]
        [DataRow(typeof(ulong))]
        [DataRow(typeof(byte?))]
        [DataRow(typeof(ushort?))]
        [DataRow(typeof(uint?))]
        [DataRow(typeof(ulong?))]
        [TestMethod]
        public void CreateMergeCastsUnsignedPrimaryKeys(Type type)
        {
            // Setup
            var builder = new MariaDbStatementBuilder(new MariaDbDbSetting());
            var primary = new DbField("Id",
                true,
                false,
                false,
                type,
                null,
                null,
                null,
                null);


            // Act
            var actual = builder.CreateMerge("Table",
                new[] { new Field("Id") },
                primaryField: primary);


            // Assert
            StringAssert.Contains(actual,
                " AS UNSIGNED)",
                StringComparison.Ordinal);
        }

        /// <summary>
        /// Keeps text primary keys in their original representation.
        /// </summary>
        [TestMethod]
        public void CreateMergeDoesNotCastTextPrimaryKeyToInteger()
        {
            // Setup
            var builder = new MariaDbStatementBuilder(new MariaDbDbSetting());
            var primary = new DbField("Id",
                true,
                false,
                false,
                typeof(string),
                null,
                null,
                null,
                null);


            // Act
            var actual = builder.CreateMerge("Table",
                new[] { new Field("Id") },
                primaryField: primary);


            // Assert
            Assert.IsFalse(actual.Contains("CAST(",
                StringComparison.Ordinal));
        }

        #endregion
    }
}