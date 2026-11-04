using Microsoft.VisualStudio.TestTools.UnitTesting;
using IBM.Data.Db2;
using System.Data;
using System;

namespace RepoDb.Db2.UnitTests.Attributes.Parameter
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
        [DataRow(-1)]
        [DataRow(0)]
        [DataRow(int.MaxValue)]
        [TestMethod]
        public void ArrayLengthAttributePreservesConfiguredValue(int value)
        {
            // Setup
            var attribute = new global::RepoDb.Attributes.Parameter.Db2.ArrayLengthAttribute(value);


            // Act
            var actual = attribute.ArrayLength;


            // Assert
            Assert.AreEqual(value,
                actual);
        }

        /// <summary>
        /// Retains the value supplied to the parameter attribute.
        /// </summary>
        /// <param name="value">The configured parameter value.</param>
        [DataRow((DB2Type)0)]
        [DataRow((DB2Type)1)]
        [TestMethod]
        public void Db2TypeAttributePreservesConfiguredValue(DB2Type value)
        {
            // Setup
            var attribute = new global::RepoDb.Attributes.Parameter.Db2.Db2TypeAttribute(value);


            // Act
            var actual = attribute.DB2Type;


            // Assert
            Assert.AreEqual(value,
                actual);
        }

        /// <summary>
        /// Retains the value supplied to the parameter attribute.
        /// </summary>
        /// <param name="value">The configured parameter value.</param>
        [DataRow(false)]
        [DataRow(true)]
        [TestMethod]
        public void Db2TypeOutputAttributePreservesConfiguredValue(bool value)
        {
            // Setup
            var attribute = new global::RepoDb.Attributes.Parameter.Db2.Db2TypeOutputAttribute(value);


            // Act
            var actual = attribute.Db2TypeOutput;


            // Assert
            Assert.AreEqual(value,
                actual);
        }

        /// <summary>
        /// Retains the value supplied to the parameter attribute.
        /// </summary>
        /// <param name="value">The configured parameter value.</param>
        [DataRow(false)]
        [DataRow(true)]
        [TestMethod]
        public void IsDefaultAttributePreservesConfiguredValue(bool value)
        {
            // Setup
            var attribute = new global::RepoDb.Attributes.Parameter.Db2.IsDefaultAttribute(value);


            // Act
            var actual = attribute.IsDefault;


            // Assert
            Assert.AreEqual(value,
                actual);
        }

        /// <summary>
        /// Retains the value supplied to the parameter attribute.
        /// </summary>
        /// <param name="value">The configured parameter value.</param>
        [DataRow(false)]
        [DataRow(true)]
        [TestMethod]
        public void IsUnassignedAttributePreservesConfiguredValue(bool value)
        {
            // Setup
            var attribute = new global::RepoDb.Attributes.Parameter.Db2.IsUnassignedAttribute(value);


            // Act
            var actual = attribute.IsUnassigned;


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
            var attribute = new global::RepoDb.Attributes.Parameter.Db2.SourceColumnAttribute(value);


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
            var attribute = new global::RepoDb.Attributes.Parameter.Db2.SourceColumnNullMappingAttribute(value);


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
            var attribute = new global::RepoDb.Attributes.Parameter.Db2.SourceVersionAttribute(value);


            // Act
            var actual = attribute.SourceVersion;


            // Assert
            Assert.AreEqual(value,
                actual);
        }

        #endregion
    }
}