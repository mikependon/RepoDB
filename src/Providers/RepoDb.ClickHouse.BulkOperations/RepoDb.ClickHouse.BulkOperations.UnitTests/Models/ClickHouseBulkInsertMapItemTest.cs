using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.ClickHouse.BulkOperations;
using System.Data;
using System;

namespace RepoDb.ClickHouse.BulkOperations.UnitTests.Models
{
    /// <summary>
    /// Covers bulk column mapping values.
    /// </summary>
    [TestClass]
    public class ClickHouseBulkInsertMapItemTest
    {
        #region Public Methods

        /// <summary>
        /// Retains the supplied column name.
        /// </summary>
        [TestMethod]
        public void ConstructorPreservesSourceColumn()
        {
            // Act
            var mapping = new ClickHouseBulkInsertMapItem("Source",
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
            var mapping = new ClickHouseBulkInsertMapItem("Source",
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
            var mapping = new ClickHouseBulkInsertMapItem("Source",
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
        public void ConstructorPreservesClickHouseType()
        {
            // Setup
            var expected = "CustomType";


            // Act
            var mapping = new ClickHouseBulkInsertMapItem("Source",
                "Destination",
                expected);


            // Assert
            Assert.AreEqual(expected,
                mapping.ClickHouseType,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Allows the provider to infer the database type.
        /// </summary>
        [TestMethod]
        public void DefaultConstructorLeavesClickHouseTypeUnspecified()
        {
            // Act
            var mapping = new ClickHouseBulkInsertMapItem("Source",
                "Destination");


            // Assert
            Assert.IsNull(mapping.ClickHouseType);
        }

        /// <summary>
        /// Includes the explicit type in diagnostics.
        /// </summary>
        [TestMethod]
        public void ToStringIncludesClickHouseType()
        {
            // Setup
            var mapping = new ClickHouseBulkInsertMapItem("Source",
                "Destination",
                "CustomType");


            // Act
            var actual = mapping.ToString();


            // Assert
            StringAssert.Contains(actual,
                "(" + mapping.ClickHouseType + ")",
                StringComparison.Ordinal);
        }

        #endregion
    }
}