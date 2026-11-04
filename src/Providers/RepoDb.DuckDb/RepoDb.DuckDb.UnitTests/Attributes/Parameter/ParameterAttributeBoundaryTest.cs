using Microsoft.VisualStudio.TestTools.UnitTesting;
using DuckDB.NET.Data;
using System.Data;
using System;

namespace RepoDb.DuckDb.UnitTests.Attributes.Parameter
{
    /// <summary>
    /// Covers configured parameter attribute values.
    /// </summary>
    [TestClass]
    public class ParameterAttributeBoundaryTest
    {
        #region Public Methods

        /// <summary>
        /// Retains the value supplied to the parameter attribute.
        /// </summary>
        [TestMethod]
        public void SourceColumnAttributePreservesConfiguredValueForNull()
        {
            // Setup
            string value = null;
            var attribute = new global::RepoDb.Attributes.Parameter.DuckDb.SourceColumnAttribute(value);

            // Act
            var actual = attribute.SourceColumn;

            // Assert
            Assert.AreEqual(value,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Retains the value supplied to the parameter attribute.
        /// </summary>
        [TestMethod]
        public void SourceColumnAttributePreservesConfiguredValueForEmptyString()
        {
            // Setup
            string value = "";
            var attribute = new global::RepoDb.Attributes.Parameter.DuckDb.SourceColumnAttribute(value);

            // Act
            var actual = attribute.SourceColumn;

            // Assert
            Assert.AreEqual(value,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Retains the value supplied to the parameter attribute.
        /// </summary>
        [TestMethod]
        public void SourceColumnAttributePreservesConfiguredValueForColumnInMixedCase()
        {
            // Setup
            string value = "Column";
            var attribute = new global::RepoDb.Attributes.Parameter.DuckDb.SourceColumnAttribute(value);

            // Act
            var actual = attribute.SourceColumn;

            // Assert
            Assert.AreEqual(value,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Retains the value supplied to the parameter attribute.
        /// </summary>
        [TestMethod]
        public void SourceColumnNullMappingAttributePreservesConfiguredValueForFalse()
        {
            // Setup
            bool value = false;
            var attribute = new global::RepoDb.Attributes.Parameter.DuckDb.SourceColumnNullMappingAttribute(value);

            // Act
            var actual = attribute.SourceColumnNullMapping;

            // Assert
            Assert.AreEqual(value,
                actual);
        }

        /// <summary>
        /// Retains the value supplied to the parameter attribute.
        /// </summary>
        [TestMethod]
        public void SourceColumnNullMappingAttributePreservesConfiguredValueForTrue()
        {
            // Setup
            bool value = true;
            var attribute = new global::RepoDb.Attributes.Parameter.DuckDb.SourceColumnNullMappingAttribute(value);

            // Act
            var actual = attribute.SourceColumnNullMapping;

            // Assert
            Assert.AreEqual(value,
                actual);
        }

        /// <summary>
        /// Retains the value supplied to the parameter attribute.
        /// </summary>
        [TestMethod]
        public void SourceVersionAttributePreservesConfiguredValueForDataRowVersionZero()
        {
            // Setup
            DataRowVersion value = (DataRowVersion)0;
            var attribute = new global::RepoDb.Attributes.Parameter.DuckDb.SourceVersionAttribute(value);

            // Act
            var actual = attribute.SourceVersion;

            // Assert
            Assert.AreEqual(value,
                actual);
        }

        /// <summary>
        /// Retains the value supplied to the parameter attribute.
        /// </summary>
        [TestMethod]
        public void SourceVersionAttributePreservesConfiguredValueForDataRowVersionOne()
        {
            // Setup
            DataRowVersion value = (DataRowVersion)1;
            var attribute = new global::RepoDb.Attributes.Parameter.DuckDb.SourceVersionAttribute(value);

            // Act
            var actual = attribute.SourceVersion;

            // Assert
            Assert.AreEqual(value,
                actual);
        }

        #endregion
    }
}