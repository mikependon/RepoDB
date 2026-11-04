using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Data.SqlClient;
using System.Data.SqlTypes;
using System.Data;
using System;

namespace RepoDb.SqlServer.UnitTests.Attributes.Parameter
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
        [DataRow((SqlCompareOptions)0)]
        [DataRow((SqlCompareOptions)1)]
        [TestMethod]
        public void CompareInfoAttributePreservesConfiguredValue(SqlCompareOptions value)
        {
            // Setup
            var attribute = new global::RepoDb.Attributes.Parameter.SqlServer.CompareInfoAttribute(value);


            // Act
            var actual = attribute.CompareInfo;


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
        public void ForceColumnEncryptionAttributePreservesConfiguredValue(bool value)
        {
            // Setup
            var attribute = new global::RepoDb.Attributes.Parameter.SqlServer.ForceColumnEncryptionAttribute(value);


            // Act
            var actual = attribute.ForceColumnEncryption;


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
        public void LocaleIdAttributePreservesConfiguredValue(int value)
        {
            // Setup
            var attribute = new global::RepoDb.Attributes.Parameter.SqlServer.LocaleIdAttribute(value);


            // Act
            var actual = attribute.LocaleId;


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
            var attribute = new global::RepoDb.Attributes.Parameter.SqlServer.OffsetAttribute(value);


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
        [DataRow((SqlDbType)0)]
        [DataRow((SqlDbType)1)]
        [TestMethod]
        public void SqlDbTypeAttributePreservesConfiguredValue(SqlDbType value)
        {
            // Setup
            var attribute = new global::RepoDb.Attributes.Parameter.SqlServer.SqlDbTypeAttribute(value);


            // Act
            var actual = attribute.SqlDbType;


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
        public void TypeNameAttributePreservesConfiguredValue(string value)
        {
            // Setup
            var attribute = new global::RepoDb.Attributes.Parameter.SqlServer.TypeNameAttribute(value);


            // Act
            var actual = attribute.TypeName;


            // Assert
            Assert.AreEqual(value,
                actual,
                StringComparer.Ordinal);
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
            var attribute = new global::RepoDb.Attributes.Parameter.SqlServer.UdtTypeNameAttribute(value);


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
        /// <param name="value">The configured parameter value.</param>
        [DataRow(null)]
        [DataRow("")]
        [DataRow("Column")]
        [TestMethod]
        public void XmlSchemaCollectionDatabaseAttributePreservesConfiguredValue(string value)
        {
            // Setup
            var attribute = new global::RepoDb.Attributes.Parameter.SqlServer.XmlSchemaCollectionDatabaseAttribute(value);


            // Act
            var actual = attribute.XmlSchemaCollectionDatabase;


            // Assert
            Assert.AreEqual(value,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Retains the value supplied to the parameter attribute.
        /// </summary>
        /// <param name="value">The configured parameter value.</param>
        [DataRow(null)]
        [DataRow("")]
        [DataRow("Column")]
        [TestMethod]
        public void XmlSchemaCollectionNameAttributePreservesConfiguredValue(string value)
        {
            // Setup
            var attribute = new global::RepoDb.Attributes.Parameter.SqlServer.XmlSchemaCollectionNameAttribute(value);


            // Act
            var actual = attribute.XmlSchemaCollectionName;


            // Assert
            Assert.AreEqual(value,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Retains the value supplied to the parameter attribute.
        /// </summary>
        /// <param name="value">The configured parameter value.</param>
        [DataRow(null)]
        [DataRow("")]
        [DataRow("Column")]
        [TestMethod]
        public void XmlSchemaCollectionOwningSchemaAttributePreservesConfiguredValue(string value)
        {
            // Setup
            var attribute = new global::RepoDb.Attributes.Parameter.SqlServer.XmlSchemaCollectionOwningSchemaAttribute(value);


            // Act
            var actual = attribute.XmlSchemaCollectionOwningSchema;


            // Assert
            Assert.AreEqual(value,
                actual,
                StringComparer.Ordinal);
        }

        #endregion
    }
}