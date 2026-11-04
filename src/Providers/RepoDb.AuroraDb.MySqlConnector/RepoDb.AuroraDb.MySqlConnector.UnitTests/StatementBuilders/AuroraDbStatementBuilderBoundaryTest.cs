using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.DbSettings;
using RepoDb.Exceptions;
using RepoDb.StatementBuilders;
using System;

namespace RepoDb.AuroraDb.MySqlConnector.UnitTests.StatementBuilders
{
    /// <summary>
    /// Covers boundary cases and supported mappings.
    /// </summary>
    [TestClass]
    public class AuroraDbStatementBuilderBoundaryTest
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
            var builder = new AuroraDbStatementBuilder(new AuroraDbDbSetting());
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
            var builder = new AuroraDbStatementBuilder(new AuroraDbDbSetting());
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
        public void CreateInsertAllRejectsMissingFieldsForNullFields()
        {
            // Setup
            bool empty = false;
            var builder = new AuroraDbStatementBuilder(new AuroraDbDbSetting());
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
        [TestMethod]
        public void CreateInsertAllRejectsMissingFieldsForEmptyFields()
        {
            // Setup
            bool empty = true;
            var builder = new AuroraDbStatementBuilder(new AuroraDbDbSetting());
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
        [TestMethod]
        public void CreateMergeRejectsMissingFieldsForNullFields()
        {
            // Setup
            bool empty = false;
            var builder = new AuroraDbStatementBuilder(new AuroraDbDbSetting());
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
        [TestMethod]
        public void CreateMergeRejectsMissingFieldsForEmptyFields()
        {
            // Setup
            bool empty = true;
            var builder = new AuroraDbStatementBuilder(new AuroraDbDbSetting());
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
        [TestMethod]
        public void CreateMergeAllRejectsMissingFieldsForNullFields()
        {
            // Setup
            bool empty = false;
            var builder = new AuroraDbStatementBuilder(new AuroraDbDbSetting());
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
        [TestMethod]
        public void CreateMergeAllRejectsMissingFieldsForEmptyFields()
        {
            // Setup
            bool empty = true;
            var builder = new AuroraDbStatementBuilder(new AuroraDbDbSetting());
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
        [TestMethod]
        public void CreateSkipQueryRejectsMissingFieldsForNullFields()
        {
            // Setup
            bool empty = false;
            var builder = new AuroraDbStatementBuilder(new AuroraDbDbSetting());
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
            var builder = new AuroraDbStatementBuilder(new AuroraDbDbSetting());
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
        [TestMethod]
        public void CreateMergeCastsUnsignedPrimaryKeysForByte()
        {
            // Setup
            Type type = typeof(byte);
            var builder = new AuroraDbStatementBuilder(new AuroraDbDbSetting());
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
        /// Preserves unsigned primary key representations.
        /// </summary>
        [TestMethod]
        public void CreateMergeCastsUnsignedPrimaryKeysForUInt16()
        {
            // Setup
            Type type = typeof(ushort);
            var builder = new AuroraDbStatementBuilder(new AuroraDbDbSetting());
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
        /// Preserves unsigned primary key representations.
        /// </summary>
        [TestMethod]
        public void CreateMergeCastsUnsignedPrimaryKeysForUInt32()
        {
            // Setup
            Type type = typeof(uint);
            var builder = new AuroraDbStatementBuilder(new AuroraDbDbSetting());
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
        /// Preserves unsigned primary key representations.
        /// </summary>
        [TestMethod]
        public void CreateMergeCastsUnsignedPrimaryKeysForUInt64()
        {
            // Setup
            Type type = typeof(ulong);
            var builder = new AuroraDbStatementBuilder(new AuroraDbDbSetting());
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
        /// Preserves unsigned primary key representations.
        /// </summary>
        [TestMethod]
        public void CreateMergeCastsUnsignedPrimaryKeysForNullableByte()
        {
            // Setup
            Type type = typeof(byte?);
            var builder = new AuroraDbStatementBuilder(new AuroraDbDbSetting());
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
        /// Preserves unsigned primary key representations.
        /// </summary>
        [TestMethod]
        public void CreateMergeCastsUnsignedPrimaryKeysForNullableUInt16()
        {
            // Setup
            Type type = typeof(ushort?);
            var builder = new AuroraDbStatementBuilder(new AuroraDbDbSetting());
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
        /// Preserves unsigned primary key representations.
        /// </summary>
        [TestMethod]
        public void CreateMergeCastsUnsignedPrimaryKeysForNullableUInt32()
        {
            // Setup
            Type type = typeof(uint?);
            var builder = new AuroraDbStatementBuilder(new AuroraDbDbSetting());
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
        /// Preserves unsigned primary key representations.
        /// </summary>
        [TestMethod]
        public void CreateMergeCastsUnsignedPrimaryKeysForNullableUInt64()
        {
            // Setup
            Type type = typeof(ulong?);
            var builder = new AuroraDbStatementBuilder(new AuroraDbDbSetting());
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
            var builder = new AuroraDbStatementBuilder(new AuroraDbDbSetting());
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