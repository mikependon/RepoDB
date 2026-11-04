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
        [TestMethod]
        public void CollectionTypeAttributePreservesConfiguredValueForOracleCollectionTypeZero()
        {
            // Setup
            OracleCollectionType value = (OracleCollectionType)0;
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
        [TestMethod]
        public void CollectionTypeAttributePreservesConfiguredValueForOracleCollectionTypeOne()
        {
            // Setup
            OracleCollectionType value = (OracleCollectionType)1;
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
        [TestMethod]
        public void OffsetAttributePreservesConfiguredValueForNegativeOne()
        {
            // Setup
            int value = -1;
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
        [TestMethod]
        public void OffsetAttributePreservesConfiguredValueForZero()
        {
            // Setup
            int value = 0;
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
        [TestMethod]
        public void OffsetAttributePreservesConfiguredValueForInt32MaxValue()
        {
            // Setup
            int value = int.MaxValue;
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
        [TestMethod]
        public void OracleDbTypeAttributePreservesConfiguredValueForOracleDbTypeZero()
        {
            // Setup
            OracleDbType value = (OracleDbType)0;
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
        [TestMethod]
        public void OracleDbTypeAttributePreservesConfiguredValueForOracleDbTypeOne()
        {
            // Setup
            OracleDbType value = (OracleDbType)1;
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
        [TestMethod]
        public void OracleDbTypeExAttributePreservesConfiguredValueForOracleDbTypeZero()
        {
            // Setup
            OracleDbType value = (OracleDbType)0;
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
        [TestMethod]
        public void OracleDbTypeExAttributePreservesConfiguredValueForOracleDbTypeOne()
        {
            // Setup
            OracleDbType value = (OracleDbType)1;
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
        [TestMethod]
        public void SkipConversionToLocalTimeAttributePreservesConfiguredValueForFalse()
        {
            // Setup
            bool value = false;
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
        [TestMethod]
        public void SkipConversionToLocalTimeAttributePreservesConfiguredValueForTrue()
        {
            // Setup
            bool value = true;
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
        [TestMethod]
        public void SourceColumnAttributePreservesConfiguredValueForNull()
        {
            // Setup
            string value = null;
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
        [TestMethod]
        public void SourceColumnAttributePreservesConfiguredValueForEmptyString()
        {
            // Setup
            string value = "";
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
        [TestMethod]
        public void SourceColumnAttributePreservesConfiguredValueForColumnInMixedCase()
        {
            // Setup
            string value = "Column";
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
        [TestMethod]
        public void SourceColumnNullMappingAttributePreservesConfiguredValueForFalse()
        {
            // Setup
            bool value = false;
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
        [TestMethod]
        public void SourceColumnNullMappingAttributePreservesConfiguredValueForTrue()
        {
            // Setup
            bool value = true;
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
        [TestMethod]
        public void SourceVersionAttributePreservesConfiguredValueForDataRowVersionZero()
        {
            // Setup
            DataRowVersion value = (DataRowVersion)0;
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
        [TestMethod]
        public void SourceVersionAttributePreservesConfiguredValueForDataRowVersionOne()
        {
            // Setup
            DataRowVersion value = (DataRowVersion)1;
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
        [TestMethod]
        public void StatusAttributePreservesConfiguredValueForOracleParameterStatusZero()
        {
            // Setup
            OracleParameterStatus value = (OracleParameterStatus)0;
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
        [TestMethod]
        public void StatusAttributePreservesConfiguredValueForOracleParameterStatusOne()
        {
            // Setup
            OracleParameterStatus value = (OracleParameterStatus)1;
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
        [TestMethod]
        public void UdtTypeNameAttributePreservesConfiguredValueForNull()
        {
            // Setup
            string value = null;
            var attribute = new global::RepoDb.Attributes.Parameter.Oracle.UdtTypeNameAttribute(value);

            // Act
            var actual = attribute.UdtTypeName;

            // Assert
            Assert.AreEqual(value,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Retains the value supplied to the parameter attribute.
        /// </summary>
        [TestMethod]
        public void UdtTypeNameAttributePreservesConfiguredValueForEmptyString()
        {
            // Setup
            string value = "";
            var attribute = new global::RepoDb.Attributes.Parameter.Oracle.UdtTypeNameAttribute(value);

            // Act
            var actual = attribute.UdtTypeName;

            // Assert
            Assert.AreEqual(value,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Retains the value supplied to the parameter attribute.
        /// </summary>
        [TestMethod]
        public void UdtTypeNameAttributePreservesConfiguredValueForColumnInMixedCase()
        {
            // Setup
            string value = "Column";
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