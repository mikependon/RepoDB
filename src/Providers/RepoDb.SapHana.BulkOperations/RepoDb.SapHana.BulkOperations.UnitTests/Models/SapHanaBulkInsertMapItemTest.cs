using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.SapHana.BulkOperations;
using Sap.Data.Hana;
using System.Data;
using System;

namespace RepoDb.SapHana.BulkOperations.UnitTests.Models
{
    /// <summary>
    /// Covers bulk column mapping values.
    /// </summary>
    [TestClass]
    public class SapHanaBulkInsertMapItemTest
    {
        #region Public Methods

        /// <summary>
        /// Retains the supplied column name.
        /// </summary>
        [TestMethod]
        public void ConstructorPreservesSourceColumn()
        {
            // Act
            var mapping = new SapHanaBulkInsertMapItem("Source",
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
            var mapping = new SapHanaBulkInsertMapItem("Source",
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
            var mapping = new SapHanaBulkInsertMapItem("Source",
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
        public void ConstructorPreservesHanaDbType()
        {
            // Setup
            var expected = (HanaDbType)1;

            // Act
            var mapping = new SapHanaBulkInsertMapItem("Source",
                "Destination",
                expected);

            // Assert
            Assert.AreEqual(expected,
                mapping.HanaDbType);
        }

        /// <summary>
        /// Allows the provider to infer the database type.
        /// </summary>
        [TestMethod]
        public void DefaultConstructorLeavesHanaDbTypeUnspecified()
        {
            // Act
            var mapping = new SapHanaBulkInsertMapItem("Source",
                "Destination");

            // Assert
            Assert.IsNull(mapping.HanaDbType);
        }

        /// <summary>
        /// Includes the explicit type in diagnostics.
        /// </summary>
        [TestMethod]
        public void ToStringIncludesHanaDbType()
        {
            // Setup
            var mapping = new SapHanaBulkInsertMapItem("Source",
                "Destination",
                (HanaDbType)1);

            // Act
            var actual = mapping.ToString();

            // Assert
            StringAssert.Contains(actual,
                "(" + mapping.HanaDbType + ")",
                StringComparison.Ordinal);
        }

        #endregion
    }
}