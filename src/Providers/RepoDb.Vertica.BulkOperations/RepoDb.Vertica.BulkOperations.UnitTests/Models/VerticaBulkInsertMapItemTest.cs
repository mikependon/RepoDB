using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Vertica.BulkOperations;
using System;
using Vertica.Data.VerticaClient;

namespace RepoDb.Vertica.BulkOperations.UnitTests.Models
{
    /// <summary>
    /// Covers bulk column mapping values.
    /// </summary>
    [TestClass]
    public class VerticaBulkInsertMapItemTest
    {
        #region Public Methods

        /// <summary>
        /// Retains the supplied column name.
        /// </summary>
        [TestMethod]
        public void ConstructorPreservesSourceColumn()
        {
            // Act
            var mapping = new VerticaBulkInsertMapItem("Source",
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
            var mapping = new VerticaBulkInsertMapItem("Source",
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
            var mapping = new VerticaBulkInsertMapItem("Source",
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
        public void ConstructorPreservesType()
        {
            // Setup
            var expected = (VerticaType)1;


            // Act
            var mapping = new VerticaBulkInsertMapItem("Source",
                "Destination",
                expected);


            // Assert
            Assert.AreEqual(expected,
                mapping.Type);
        }

        /// <summary>
        /// Allows the provider to infer the database type.
        /// </summary>
        [TestMethod]
        public void DefaultConstructorLeavesTypeUnspecified()
        {
            // Act
            var mapping = new VerticaBulkInsertMapItem("Source",
                "Destination");


            // Assert
            Assert.IsNull(mapping.Type);
        }

        /// <summary>
        /// Includes the explicit type in diagnostics.
        /// </summary>
        [TestMethod]
        public void ToStringIncludesType()
        {
            // Setup
            var mapping = new VerticaBulkInsertMapItem("Source",
                "Destination",
                (VerticaType)1);


            // Act
            var actual = mapping.ToString();


            // Assert
            StringAssert.Contains(actual,
                "(" + mapping.Type + ")",
                StringComparison.Ordinal);
        }

        #endregion
    }
}