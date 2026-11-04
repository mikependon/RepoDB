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
        [TestMethod]
        public void CompareInfoAttributePreservesConfiguredValueForSqlCompareOptionsZero()
        {
            // Setup
            SqlCompareOptions value = (SqlCompareOptions)0;
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
        [TestMethod]
        public void CompareInfoAttributePreservesConfiguredValueForSqlCompareOptionsOne()
        {
            // Setup
            SqlCompareOptions value = (SqlCompareOptions)1;
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
        [TestMethod]
        public void ForceColumnEncryptionAttributePreservesConfiguredValueForFalse()
        {
            // Setup
            bool value = false;
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
        [TestMethod]
        public void ForceColumnEncryptionAttributePreservesConfiguredValueForTrue()
        {
            // Setup
            bool value = true;
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
        [TestMethod]
        public void LocaleIdAttributePreservesConfiguredValueForNegativeOne()
        {
            // Setup
            int value = -1;
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
        [TestMethod]
        public void LocaleIdAttributePreservesConfiguredValueForZero()
        {
            // Setup
            int value = 0;
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
        [TestMethod]
        public void LocaleIdAttributePreservesConfiguredValueForInt32MaxValue()
        {
            // Setup
            int value = int.MaxValue;
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
        [TestMethod]
        public void OffsetAttributePreservesConfiguredValueForNegativeOne()
        {
            // Setup
            int value = -1;
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
        [TestMethod]
        public void OffsetAttributePreservesConfiguredValueForZero()
        {
            // Setup
            int value = 0;
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
        [TestMethod]
        public void OffsetAttributePreservesConfiguredValueForInt32MaxValue()
        {
            // Setup
            int value = int.MaxValue;
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
        [TestMethod]
        public void SqlDbTypeAttributePreservesConfiguredValueForSqlDbTypeZero()
        {
            // Setup
            SqlDbType value = (SqlDbType)0;
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
        [TestMethod]
        public void SqlDbTypeAttributePreservesConfiguredValueForSqlDbTypeOne()
        {
            // Setup
            SqlDbType value = (SqlDbType)1;
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
        [TestMethod]
        public void TypeNameAttributePreservesConfiguredValueForNull()
        {
            // Setup
            string value = null;
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
        [TestMethod]
        public void TypeNameAttributePreservesConfiguredValueForEmptyString()
        {
            // Setup
            string value = "";
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
        [TestMethod]
        public void TypeNameAttributePreservesConfiguredValueForColumnInMixedCase()
        {
            // Setup
            string value = "Column";
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
        [TestMethod]
        public void UdtTypeNameAttributePreservesConfiguredValueForNull()
        {
            // Setup
            string value = null;
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
        [TestMethod]
        public void UdtTypeNameAttributePreservesConfiguredValueForEmptyString()
        {
            // Setup
            string value = "";
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
        [TestMethod]
        public void UdtTypeNameAttributePreservesConfiguredValueForColumnInMixedCase()
        {
            // Setup
            string value = "Column";
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
        [TestMethod]
        public void XmlSchemaCollectionDatabaseAttributePreservesConfiguredValueForNull()
        {
            // Setup
            string value = null;
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
        [TestMethod]
        public void XmlSchemaCollectionDatabaseAttributePreservesConfiguredValueForEmptyString()
        {
            // Setup
            string value = "";
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
        [TestMethod]
        public void XmlSchemaCollectionDatabaseAttributePreservesConfiguredValueForColumnInMixedCase()
        {
            // Setup
            string value = "Column";
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
        [TestMethod]
        public void XmlSchemaCollectionNameAttributePreservesConfiguredValueForNull()
        {
            // Setup
            string value = null;
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
        [TestMethod]
        public void XmlSchemaCollectionNameAttributePreservesConfiguredValueForEmptyString()
        {
            // Setup
            string value = "";
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
        [TestMethod]
        public void XmlSchemaCollectionNameAttributePreservesConfiguredValueForColumnInMixedCase()
        {
            // Setup
            string value = "Column";
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
        [TestMethod]
        public void XmlSchemaCollectionOwningSchemaAttributePreservesConfiguredValueForNull()
        {
            // Setup
            string value = null;
            var attribute = new global::RepoDb.Attributes.Parameter.SqlServer.XmlSchemaCollectionOwningSchemaAttribute(value);

            // Act
            var actual = attribute.XmlSchemaCollectionOwningSchema;

            // Assert
            Assert.AreEqual(value,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Retains the value supplied to the parameter attribute.
        /// </summary>
        [TestMethod]
        public void XmlSchemaCollectionOwningSchemaAttributePreservesConfiguredValueForEmptyString()
        {
            // Setup
            string value = "";
            var attribute = new global::RepoDb.Attributes.Parameter.SqlServer.XmlSchemaCollectionOwningSchemaAttribute(value);

            // Act
            var actual = attribute.XmlSchemaCollectionOwningSchema;

            // Assert
            Assert.AreEqual(value,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Retains the value supplied to the parameter attribute.
        /// </summary>
        [TestMethod]
        public void XmlSchemaCollectionOwningSchemaAttributePreservesConfiguredValueForColumnInMixedCase()
        {
            // Setup
            string value = "Column";
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