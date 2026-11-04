using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Connector.MariaDbConnector;
using RepoDb.MariaDbConnector.BulkOperations;
using System.Data;
using System;

namespace RepoDb.MariaDbConnector.BulkOperations.UnitTests.Models
{
    /// <summary>
    /// Covers bulk column mapping values.
    /// </summary>
    [TestClass]
    public class MariaDbBulkInsertMapItemTest
    {
        #region Public Methods

        /// <summary>
        /// Retains the supplied column name.
        /// </summary>
        [TestMethod]
        public void ConstructorPreservesSourceColumn()
        {
            // Act
            var mapping = new MariaDbBulkInsertMapItem("Source",
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
            var mapping = new MariaDbBulkInsertMapItem("Source",
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
            var mapping = new MariaDbBulkInsertMapItem("Source",
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
        public void ConstructorPreservesMariaDbType()
        {
            // Setup
            var expected = (MariaDbType)1;

            // Act
            var mapping = new MariaDbBulkInsertMapItem("Source",
                "Destination",
                expected);

            // Assert
            Assert.AreEqual(expected,
                mapping.MariaDbType);
        }

        /// <summary>
        /// Allows the provider to infer the database type.
        /// </summary>
        [TestMethod]
        public void DefaultConstructorLeavesMariaDbTypeUnspecified()
        {
            // Act
            var mapping = new MariaDbBulkInsertMapItem("Source",
                "Destination");

            // Assert
            Assert.IsNull(mapping.MariaDbType);
        }

        /// <summary>
        /// Includes the explicit type in diagnostics.
        /// </summary>
        [TestMethod]
        public void ToStringIncludesMariaDbType()
        {
            // Setup
            var mapping = new MariaDbBulkInsertMapItem("Source",
                "Destination",
                (MariaDbType)1);

            // Act
            var actual = mapping.ToString();

            // Assert
            StringAssert.Contains(actual,
                "(" + mapping.MariaDbType + ")",
                StringComparison.Ordinal);
        }

        #endregion
    }
}