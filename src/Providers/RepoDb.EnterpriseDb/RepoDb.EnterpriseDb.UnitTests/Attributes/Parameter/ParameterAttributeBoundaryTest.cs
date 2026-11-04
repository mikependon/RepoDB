using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Connector.EnterpriseDb;
using System.Data;
using System;

namespace RepoDb.EnterpriseDb.UnitTests.Attributes.Parameter
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
        [DataRow((DbType)0)]
        [DataRow((DbType)1)]
        [TestMethod]
        public void DbTypeAttributePreservesConfiguredValue(DbType value)
        {
            // Setup
            var attribute = new global::RepoDb.Attributes.Parameter.EnterpriseDb.DbTypeAttribute(value);


            // Act
            var actual = attribute.DbType;


            // Assert
            Assert.AreEqual(value,
                actual);
        }

        /// <summary>
        /// Retains the value supplied to the parameter attribute.
        /// </summary>
        /// <param name="value">The configured parameter value.</param>
        [DataRow((ParameterDirection)0)]
        [DataRow((ParameterDirection)1)]
        [TestMethod]
        public void DirectionAttributePreservesConfiguredValue(ParameterDirection value)
        {
            // Setup
            var attribute = new global::RepoDb.Attributes.Parameter.EnterpriseDb.DirectionAttribute(value);


            // Act
            var actual = attribute.Direction;


            // Assert
            Assert.AreEqual(value,
                actual);
        }

        /// <summary>
        /// Retains the value supplied to the parameter attribute.
        /// </summary>
        /// <param name="value">The configured parameter value.</param>
        [DataRow((EDBType)0)]
        [DataRow((EDBType)1)]
        [TestMethod]
        public void EnterpriseDbTypeAttributePreservesConfiguredValue(EDBType value)
        {
            // Setup
            var attribute = new global::RepoDb.Attributes.Parameter.EnterpriseDb.EnterpriseDbTypeAttribute(value);


            // Act
            var actual = attribute.EDBType;


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
        public void IsNullableAttributePreservesConfiguredValue(bool value)
        {
            // Setup
            var attribute = new global::RepoDb.Attributes.Parameter.EnterpriseDb.IsNullableAttribute(value);


            // Act
            var actual = attribute.IsNullable;


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
        public void ParameterNameAttributePreservesConfiguredValue(string value)
        {
            // Setup
            var attribute = new global::RepoDb.Attributes.Parameter.EnterpriseDb.ParameterNameAttribute(value);


            // Act
            var actual = attribute.ParameterName;


            // Assert
            Assert.AreEqual(value,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Retains the value supplied to the parameter attribute.
        /// </summary>
        /// <param name="value">The configured parameter value.</param>
        [DataRow((byte)0)]
        [DataRow((byte)255)]
        [TestMethod]
        public void PrecisionAttributePreservesConfiguredValue(byte value)
        {
            // Setup
            var attribute = new global::RepoDb.Attributes.Parameter.EnterpriseDb.PrecisionAttribute(value);


            // Act
            var actual = attribute.Precision;


            // Assert
            Assert.AreEqual(value,
                actual);
        }

        /// <summary>
        /// Retains the value supplied to the parameter attribute.
        /// </summary>
        /// <param name="value">The configured parameter value.</param>
        [DataRow((byte)0)]
        [DataRow((byte)255)]
        [TestMethod]
        public void ScaleAttributePreservesConfiguredValue(byte value)
        {
            // Setup
            var attribute = new global::RepoDb.Attributes.Parameter.EnterpriseDb.ScaleAttribute(value);


            // Act
            var actual = attribute.Scale;


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
        public void SizeAttributePreservesConfiguredValue(int value)
        {
            // Setup
            var attribute = new global::RepoDb.Attributes.Parameter.EnterpriseDb.SizeAttribute(value);


            // Act
            var actual = attribute.Size;


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
            var attribute = new global::RepoDb.Attributes.Parameter.EnterpriseDb.SourceColumnAttribute(value);


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
            var attribute = new global::RepoDb.Attributes.Parameter.EnterpriseDb.SourceColumnNullMappingAttribute(value);


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
            var attribute = new global::RepoDb.Attributes.Parameter.EnterpriseDb.SourceVersionAttribute(value);


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
        [DataRow(null)]
        [DataRow(42)]
        [TestMethod]
        public void ValueAttributePreservesConfiguredValue(object value)
        {
            // Setup
            var attribute = new global::RepoDb.Attributes.Parameter.EnterpriseDb.ValueAttribute(value);


            // Act
            var actual = attribute.Value;


            // Assert
            Assert.AreEqual(value,
                actual);
        }

        #endregion
    }
}