using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.AuroraDb.MySqlConnector.BulkOperations;
using RepoDb.Connector.AuroraDb.MySqlConnector;
using System.Data;
using System;

namespace RepoDb.AuroraDb.MySqlConnector.BulkOperations.UnitTests.Models
{
    /// <summary>
    /// Covers bulk column mapping values.
    /// </summary>
    [TestClass]
    public class AuroraDbBulkInsertMapItemTest
    {
        #region Public Methods

        /// <summary>
        /// Retains the supplied column name.
        /// </summary>
        [TestMethod]
        public void ConstructorPreservesSourceColumn()
        {
            // Act
            var mapping = new AuroraDbBulkInsertMapItem("Source",
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
            var mapping = new AuroraDbBulkInsertMapItem("Source",
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
            var mapping = new AuroraDbBulkInsertMapItem("Source",
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
        public void ConstructorPreservesAuroraDbType()
        {
            // Setup
            var expected = (AuroraDbType)1;


            // Act
            var mapping = new AuroraDbBulkInsertMapItem("Source",
                "Destination",
                expected);


            // Assert
            Assert.AreEqual(expected,
                mapping.AuroraDbType);
        }

        /// <summary>
        /// Allows the provider to infer the database type.
        /// </summary>
        [TestMethod]
        public void DefaultConstructorLeavesAuroraDbTypeUnspecified()
        {
            // Act
            var mapping = new AuroraDbBulkInsertMapItem("Source",
                "Destination");


            // Assert
            Assert.IsNull(mapping.AuroraDbType);
        }

        /// <summary>
        /// Includes the explicit type in diagnostics.
        /// </summary>
        [TestMethod]
        public void ToStringIncludesAuroraDbType()
        {
            // Setup
            var mapping = new AuroraDbBulkInsertMapItem("Source",
                "Destination",
                (AuroraDbType)1);


            // Act
            var actual = mapping.ToString();


            // Assert
            StringAssert.Contains(actual,
                "(" + mapping.AuroraDbType + ")",
                StringComparison.Ordinal);
        }

        #endregion
    }
}