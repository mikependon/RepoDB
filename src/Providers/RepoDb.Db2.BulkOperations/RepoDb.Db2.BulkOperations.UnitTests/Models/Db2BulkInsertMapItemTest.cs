using Microsoft.VisualStudio.TestTools.UnitTesting;
using IBM.Data.Db2;
using RepoDb.Db2.BulkOperations;
using System;

namespace RepoDb.Db2.BulkOperations.UnitTests.Models
{
    /// <summary>
    /// Covers bulk column mapping values.
    /// </summary>
    [TestClass]
    public class Db2BulkInsertMapItemTest
    {
        #region Public Methods

        /// <summary>
        /// Retains the supplied column name.
        /// </summary>
        [TestMethod]
        public void ConstructorPreservesSourceColumn()
        {
            // Act
            var mapping = new Db2BulkInsertMapItem("Source",
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
            var mapping = new Db2BulkInsertMapItem("Source",
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
            var mapping = new Db2BulkInsertMapItem("Source",
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
        public void ConstructorPreservesDB2Type()
        {
            // Setup
            var expected = (DB2Type)1;


            // Act
            var mapping = new Db2BulkInsertMapItem("Source",
                "Destination",
                expected);


            // Assert
            Assert.AreEqual(expected,
                mapping.DB2Type);
        }

        /// <summary>
        /// Allows the provider to infer the database type.
        /// </summary>
        [TestMethod]
        public void DefaultConstructorLeavesDB2TypeUnspecified()
        {
            // Act
            var mapping = new Db2BulkInsertMapItem("Source",
                "Destination");


            // Assert
            Assert.IsNull(mapping.DB2Type);
        }

        /// <summary>
        /// Includes the explicit type in diagnostics.
        /// </summary>
        [TestMethod]
        public void ToStringIncludesDB2Type()
        {
            // Setup
            var mapping = new Db2BulkInsertMapItem("Source",
                "Destination",
                (DB2Type)1);


            // Act
            var actual = mapping.ToString();


            // Assert
            StringAssert.Contains(actual,
                "(" + mapping.DB2Type + ")",
                StringComparison.Ordinal);
        }

        #endregion
    }
}