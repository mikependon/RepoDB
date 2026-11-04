using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Connector.EnterpriseDb;
using RepoDb.EnterpriseDb.BulkOperations;
using System;

namespace RepoDb.EnterpriseDb.BulkOperations.UnitTests.Models
{
    /// <summary>
    /// Covers bulk column mapping values.
    /// </summary>
    [TestClass]
    public class EDBBulkInsertMapItemTest
    {
        #region Public Methods

        /// <summary>
        /// Retains the supplied column name.
        /// </summary>
        [TestMethod]
        public void ConstructorPreservesSourceColumn()
        {
            // Act
            var mapping = new EDBBulkInsertMapItem("Source",
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
            var mapping = new EDBBulkInsertMapItem("Source",
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
            var mapping = new EDBBulkInsertMapItem("Source",
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
        public void ConstructorPreservesEDBType()
        {
            // Setup
            var expected = (EDBType)1;

            // Act
            var mapping = new EDBBulkInsertMapItem("Source",
                "Destination",
                expected);

            // Assert
            Assert.AreEqual(expected,
                mapping.EDBType);
        }

        /// <summary>
        /// Allows the provider to infer the database type.
        /// </summary>
        [TestMethod]
        public void DefaultConstructorLeavesEDBTypeUnspecified()
        {
            // Act
            var mapping = new EDBBulkInsertMapItem("Source",
                "Destination");

            // Assert
            Assert.IsNull(mapping.EDBType);
        }

        /// <summary>
        /// Includes the explicit type in diagnostics.
        /// </summary>
        [TestMethod]
        public void ToStringIncludesEDBType()
        {
            // Setup
            var mapping = new EDBBulkInsertMapItem("Source",
                "Destination",
                (EDBType)1);

            // Act
            var actual = mapping.ToString();

            // Assert
            StringAssert.Contains(actual,
                "(" + mapping.EDBType + ")",
                StringComparison.Ordinal);
        }

        #endregion
    }
}