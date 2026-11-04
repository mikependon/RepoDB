using Microsoft.VisualStudio.TestTools.UnitTesting;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System;

namespace RepoDb.Oracle.UnitTests.Attributes.Parameter
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
        [DataRow((OracleCollectionType)0)]
        [DataRow((OracleCollectionType)1)]
        [TestMethod]
        public void CollectionTypeAttributePreservesConfiguredValue(OracleCollectionType value)
        {
            // Setup
            var attribute = new global::RepoDb.Attributes.Parameter.Oracle.CollectionTypeAttribute(value);


            // Act
            var actual = attribute.CollectionType;


            // Assert
            Assert.AreEqual(value,
                actual);
        }

        /// <summary>
        /// Retains the value supplied to the parameter attribute.
        /// </summary>
        /// <param name="value">The configured parameter value.</param>
        [DataRow(-1)]
        [DataRow(0)]
        [DataRow(int.MaxValue)]
        [TestMethod]
        public void OffsetAttributePreservesConfiguredValue(int value)
        {
            // Setup
            var attribute = new global::RepoDb.Attributes.Parameter.Oracle.OffsetAttribute(value);


            // Act
            var actual = attribute.Offset;


            // Assert
            Assert.AreEqual(value,
                actual);
        }

        /// <summary>
        /// Retains the value supplied to the parameter attribute.
        /// </summary>
        /// <param name="value">The configured parameter value.</param>
        [DataRow((OracleDbType)0)]
        [DataRow((OracleDbType)1)]
        [TestMethod]
        public void OracleDbTypeAttributePreservesConfiguredValue(OracleDbType value)
        {
            // Setup
            var attribute = new global::RepoDb.Attributes.Parameter.Oracle.OracleDbTypeAttribute(value);


            // Act
            var actual = attribute.OracleDbType;


            // Assert
            Assert.AreEqual(value,
                actual);
        }

        /// <summary>
        /// Retains the value supplied to the parameter attribute.
        /// </summary>
        /// <param name="value">The configured parameter value.</param>
        [DataRow((OracleDbType)0)]
        [DataRow((OracleDbType)1)]
        [TestMethod]
        public void OracleDbTypeExAttributePreservesConfiguredValue(OracleDbType value)
        {
            // Setup
            var attribute = new global::RepoDb.Attributes.Parameter.Oracle.OracleDbTypeExAttribute(value);


            // Act
            var actual = attribute.OracleDbType;


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
        public void SkipConversionToLocalTimeAttributePreservesConfiguredValue(bool value)
        {
            // Setup
            var attribute = new global::RepoDb.Attributes.Parameter.Oracle.SkipConversionToLocalTimeAttribute(value);


            // Act
            var actual = attribute.SkipConversionToLocalTime;


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
            var attribute = new global::RepoDb.Attributes.Parameter.Oracle.SourceColumnAttribute(value);


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
            var attribute = new global::RepoDb.Attributes.Parameter.Oracle.SourceColumnNullMappingAttribute(value);


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
            var attribute = new global::RepoDb.Attributes.Parameter.Oracle.SourceVersionAttribute(value);


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
        [DataRow((OracleParameterStatus)0)]
        [DataRow((OracleParameterStatus)1)]
        [TestMethod]
        public void StatusAttributePreservesConfiguredValue(OracleParameterStatus value)
        {
            // Setup
            var attribute = new global::RepoDb.Attributes.Parameter.Oracle.StatusAttribute(value);


            // Act
            var actual = attribute.Status;


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
        public void UdtTypeNameAttributePreservesConfiguredValue(string value)
        {
            // Setup
            var attribute = new global::RepoDb.Attributes.Parameter.Oracle.UdtTypeNameAttribute(value);


            // Act
            var actual = attribute.UdtTypeName;


            // Assert
            Assert.AreEqual(value,
                actual,
                StringComparer.Ordinal);
        }

        #endregion
    }
}