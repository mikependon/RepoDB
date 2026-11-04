using Microsoft.VisualStudio.TestTools.UnitTesting;
using NpgsqlTypes;
using RepoDb.PostgreSql.BulkOperations;
using System;

namespace RepoDb.PostgreSql.BulkOperations.UnitTests.Models
{
    /// <summary>
    /// Covers bulk column mapping values.
    /// </summary>
    [TestClass]
    public class PostgreSqlBulkInsertMapItemTest
    {
        #region Public Methods

        /// <summary>
        /// Retains the supplied column name.
        /// </summary>
        [TestMethod]
        public void ConstructorPreservesSourceColumn()
        {
            // Act
            var mapping = new PostgreSqlBulkInsertMapItem("Source",
                "Destination");

            // Assert
            Assert.AreEqual("Source",
                mapping.SourceColumn,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Retains the supplied column name.
        /// </summary>
        [TestMethod]
        public void ConstructorPreservesDestinationColumn()
        {
            // Act
            var mapping = new PostgreSqlBulkInsertMapItem("Source",
                "Destination");

            // Assert
            Assert.AreEqual("Destination",
                mapping.DestinationColumn,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Reuses the hash code for an unchanged mapping.
        /// </summary>
        [TestMethod]
        public void HashCodeRemainsStableAcrossCalls()
        {
            // Setup
            var mapping = new PostgreSqlBulkInsertMapItem("Source",
                "Destination");
            var expected = mapping.GetHashCode();

            // Act
            var actual = mapping.GetHashCode();

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Retains the explicitly selected database type.
        /// </summary>
        [TestMethod]
        public void ConstructorPreservesNpgsqlDbType()
        {
            // Setup
            var expected = (NpgsqlDbType)1;

            // Act
            var mapping = new PostgreSqlBulkInsertMapItem("Source",
                "Destination",
                expected);

            // Assert
            Assert.AreEqual(expected,
                mapping.NpgsqlDbType);
        }

        /// <summary>
        /// Allows the provider to infer the database type.
        /// </summary>
        [TestMethod]
        public void DefaultConstructorLeavesNpgsqlDbTypeUnspecified()
        {
            // Act
            var mapping = new PostgreSqlBulkInsertMapItem("Source",
                "Destination");

            // Assert
            Assert.IsNull(mapping.NpgsqlDbType);
        }

        /// <summary>
        /// Includes the explicit type in diagnostics.
        /// </summary>
        [TestMethod]
        public void ToStringIncludesNpgsqlDbType()
        {
            // Setup
            var mapping = new PostgreSqlBulkInsertMapItem("Source",
                "Destination",
                (NpgsqlDbType)1);

            // Act
            var actual = mapping.ToString();

            // Assert
            StringAssert.Contains(actual,
                "(" + mapping.NpgsqlDbType + ")",
                StringComparison.Ordinal);
        }

        /// <summary>
        /// Retains the explicitly selected database type.
        /// </summary>
        [TestMethod]
        public void ConstructorPreservesDataTypeName()
        {
            // Setup
            var expected = "CustomType";

            // Act
            var mapping = new PostgreSqlBulkInsertMapItem("Source",
                "Destination",
                expected);

            // Assert
            Assert.AreEqual(expected,
                mapping.DataTypeName,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Allows the provider to infer the database type.
        /// </summary>
        [TestMethod]
        public void DefaultConstructorLeavesDataTypeNameUnspecified()
        {
            // Act
            var mapping = new PostgreSqlBulkInsertMapItem("Source",
                "Destination");

            // Assert
            Assert.IsNull(mapping.DataTypeName);
        }

        /// <summary>
        /// Includes the explicit type in diagnostics.
        /// </summary>
        [TestMethod]
        public void ToStringIncludesDataTypeName()
        {
            // Setup
            var mapping = new PostgreSqlBulkInsertMapItem("Source",
                "Destination",
                "CustomType");

            // Act
            var actual = mapping.ToString();

            // Assert
            StringAssert.Contains(actual,
                "(" + mapping.DataTypeName + ")",
                StringComparison.Ordinal);
        }

        #endregion
    }
}