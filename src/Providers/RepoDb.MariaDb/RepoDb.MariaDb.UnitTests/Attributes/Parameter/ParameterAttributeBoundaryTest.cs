using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Connector.MariaDb;
using System.Data;
using System;

namespace RepoDb.MariaDb.UnitTests.Attributes.Parameter
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
        /// <param name="value">The configured parameter value.</param>
        [DataRow((MariaDbType)0)]
        [DataRow((MariaDbType)1)]
        [TestMethod]
        public void MariaDbTypeAttributePreservesConfiguredValue(MariaDbType value)
        {
            // Setup
            var attribute = new global::RepoDb.Attributes.Parameter.MariaDb.MariaDbTypeAttribute(value);


            // Act
            var actual = attribute.MariaDbType;


            // Assert
            Assert.AreEqual(value,
                actual);
        }

        /// <summary>
        /// Retains the value supplied to the parameter attribute.
        /// </summary>
        /// <param name="value">The configured parameter value.</param>
        [DataRow(null)]
        [DataRow("")]
        [DataRow("Column")]
        [TestMethod]
        public void SourceColumnAttributePreservesConfiguredValue(string value)
        {
            // Setup
            var attribute = new global::RepoDb.Attributes.Parameter.MariaDb.SourceColumnAttribute(value);


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
        /// <param name="value">The configured parameter value.</param>
        [DataRow(false)]
        [DataRow(true)]
        [TestMethod]
        public void SourceColumnNullMappingAttributePreservesConfiguredValue(bool value)
        {
            // Setup
            var attribute = new global::RepoDb.Attributes.Parameter.MariaDb.SourceColumnNullMappingAttribute(value);


            // Act
            var actual = attribute.SourceColumnNullMapping;


            // Assert
            Assert.AreEqual(value,
                actual);
        }

        /// <summary>
        /// Retains the value supplied to the parameter attribute.
        /// </summary>
        /// <param name="value">The configured parameter value.</param>
        [DataRow((DataRowVersion)0)]
        [DataRow((DataRowVersion)1)]
        [TestMethod]
        public void SourceVersionAttributePreservesConfiguredValue(DataRowVersion value)
        {
            // Setup
            var attribute = new global::RepoDb.Attributes.Parameter.MariaDb.SourceVersionAttribute(value);


            // Act
            var actual = attribute.SourceVersion;


            // Assert
            Assert.AreEqual(value,
                actual);
        }

        #endregion
    }
}