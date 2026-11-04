using Microsoft.VisualStudio.TestTools.UnitTesting;
using Oracle.ManagedDataAccess.Client;
using RepoDb.Oracle.BulkOperations;
using System;

namespace RepoDb.Oracle.BulkOperations.UnitTests.Models
{
    /// <summary>
    /// Covers bulk column mapping values.
    /// </summary>
    [TestClass]
    public class OracleBulkInsertMapItemTest
    {
        #region Public Methods

        /// <summary>
        /// Retains the supplied column name.
        /// </summary>
        [TestMethod]
        public void ConstructorPreservesSourceColumn()
        {
            // Act
            var mapping = new OracleBulkInsertMapItem("Source",
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
            var mapping = new OracleBulkInsertMapItem("Source",
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
            var mapping = new OracleBulkInsertMapItem("Source",
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
        public void ConstructorPreservesOracleDbType()
        {
            // Setup
            var expected = (OracleDbType)1;

            // Act
            var mapping = new OracleBulkInsertMapItem("Source",
                "Destination",
                expected);

            // Assert
            Assert.AreEqual(expected,
                mapping.OracleDbType);
        }

        /// <summary>
        /// Allows the provider to infer the database type.
        /// </summary>
        [TestMethod]
        public void DefaultConstructorLeavesOracleDbTypeUnspecified()
        {
            // Act
            var mapping = new OracleBulkInsertMapItem("Source",
                "Destination");

            // Assert
            Assert.IsNull(mapping.OracleDbType);
        }

        /// <summary>
        /// Includes the explicit type in diagnostics.
        /// </summary>
        [TestMethod]
        public void ToStringIncludesOracleDbType()
        {
            // Setup
            var mapping = new OracleBulkInsertMapItem("Source",
                "Destination",
                (OracleDbType)1);

            // Act
            var actual = mapping.ToString();

            // Assert
            StringAssert.Contains(actual,
                "(" + mapping.OracleDbType + ")",
                StringComparison.Ordinal);
        }

        #endregion
    }
}