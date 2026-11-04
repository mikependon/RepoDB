using Microsoft.VisualStudio.TestTools.UnitTesting;
using MySqlConnector;
using RepoDb.MySqlConnector.BulkOperations;
using System;

namespace RepoDb.MySqlConnector.BulkOperations.UnitTests.Models
{
    /// <summary>
    /// Covers bulk column mapping values.
    /// </summary>
    [TestClass]
    public class MySqlConnectorBulkInsertMapItemTest
    {
        #region Public Methods

        /// <summary>
        /// Retains the supplied column name.
        /// </summary>
        [TestMethod]
        public void ConstructorPreservesSourceColumn()
        {
            // Act
            var mapping = new MySqlConnectorBulkInsertMapItem("Source",
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
            var mapping = new MySqlConnectorBulkInsertMapItem("Source",
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
            var mapping = new MySqlConnectorBulkInsertMapItem("Source",
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
        public void ConstructorPreservesMySqlDbType()
        {
            // Setup
            var expected = (MySqlDbType)1;


            // Act
            var mapping = new MySqlConnectorBulkInsertMapItem("Source",
                "Destination",
                expected);


            // Assert
            Assert.AreEqual(expected,
                mapping.MySqlDbType);
        }

        /// <summary>
        /// Allows the provider to infer the database type.
        /// </summary>
        [TestMethod]
        public void DefaultConstructorLeavesMySqlDbTypeUnspecified()
        {
            // Act
            var mapping = new MySqlConnectorBulkInsertMapItem("Source",
                "Destination");


            // Assert
            Assert.IsNull(mapping.MySqlDbType);
        }

        /// <summary>
        /// Includes the explicit type in diagnostics.
        /// </summary>
        [TestMethod]
        public void ToStringIncludesMySqlDbType()
        {
            // Setup
            var mapping = new MySqlConnectorBulkInsertMapItem("Source",
                "Destination",
                (MySqlDbType)1);


            // Act
            var actual = mapping.ToString();


            // Assert
            StringAssert.Contains(actual,
                "(" + mapping.MySqlDbType + ")",
                StringComparison.Ordinal);
        }

        #endregion
    }
}