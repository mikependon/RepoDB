using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data;
using System;
using Vertica.Data.VerticaClient;

namespace RepoDb.Vertica.UnitTests.Attributes.Parameter
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
        [DataRow(null)]
        [DataRow("")]
        [DataRow("Column")]
        [TestMethod]
        public void SourceColumnAttributePreservesConfiguredValue(string value)
        {
            // Setup
            var attribute = new global::RepoDb.Attributes.Parameter.Vertica.SourceColumnAttribute(value);


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
            var attribute = new global::RepoDb.Attributes.Parameter.Vertica.SourceColumnNullMappingAttribute(value);


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
            var attribute = new global::RepoDb.Attributes.Parameter.Vertica.SourceVersionAttribute(value);


            // Act
            var actual = attribute.SourceVersion;


            // Assert
            Assert.AreEqual(value,
                actual);
        }

        /// <summary>
        /// Retains the value supplied to the parameter attribute.
        /// </summary>
        /// <param name="value">The configured parameter value.</param>
        [DataRow((VerticaType)0)]
        [DataRow((VerticaType)1)]
        [TestMethod]
        public void VerticaTypeAttributePreservesConfiguredValue(VerticaType value)
        {
            // Setup
            var attribute = new global::RepoDb.Attributes.Parameter.Vertica.VerticaTypeAttribute(value);


            // Act
            var actual = attribute.Type;


            // Assert
            Assert.AreEqual(value,
                actual);
        }

        #endregion
    }
}