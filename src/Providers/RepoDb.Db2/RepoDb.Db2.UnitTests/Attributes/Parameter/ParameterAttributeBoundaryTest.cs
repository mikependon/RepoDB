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
        [TestMethod]
        public void ArrayLengthAttributePreservesConfiguredValueForNegativeOne()
        {
            // Setup
            int value = -1;
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
        [TestMethod]
        public void ArrayLengthAttributePreservesConfiguredValueForZero()
        {
            // Setup
            int value = 0;
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
        [TestMethod]
        public void ArrayLengthAttributePreservesConfiguredValueForInt32MaxValue()
        {
            // Setup
            int value = int.MaxValue;
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
        [TestMethod]
        public void Db2TypeAttributePreservesConfiguredValueForDB2TypeZero()
        {
            // Setup
            DB2Type value = (DB2Type)0;
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
        [TestMethod]
        public void Db2TypeAttributePreservesConfiguredValueForDB2TypeOne()
        {
            // Setup
            DB2Type value = (DB2Type)1;
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
        [TestMethod]
        public void Db2TypeOutputAttributePreservesConfiguredValueForFalse()
        {
            // Setup
            bool value = false;
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
        [TestMethod]
        public void Db2TypeOutputAttributePreservesConfiguredValueForTrue()
        {
            // Setup
            bool value = true;
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
        [TestMethod]
        public void IsDefaultAttributePreservesConfiguredValueForFalse()
        {
            // Setup
            bool value = false;
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
        [TestMethod]
        public void IsDefaultAttributePreservesConfiguredValueForTrue()
        {
            // Setup
            bool value = true;
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
        [TestMethod]
        public void IsUnassignedAttributePreservesConfiguredValueForFalse()
        {
            // Setup
            bool value = false;
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
        [TestMethod]
        public void IsUnassignedAttributePreservesConfiguredValueForTrue()
        {
            // Setup
            bool value = true;
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
        [TestMethod]
        public void SourceColumnAttributePreservesConfiguredValueForNull()
        {
            // Setup
            string value = null;
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
        [TestMethod]
        public void SourceColumnAttributePreservesConfiguredValueForEmptyString()
        {
            // Setup
            string value = "";
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
        [TestMethod]
        public void SourceColumnAttributePreservesConfiguredValueForColumnInMixedCase()
        {
            // Setup
            string value = "Column";
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
        [TestMethod]
        public void SourceColumnNullMappingAttributePreservesConfiguredValueForFalse()
        {
            // Setup
            bool value = false;
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
        [TestMethod]
        public void SourceColumnNullMappingAttributePreservesConfiguredValueForTrue()
        {
            // Setup
            bool value = true;
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
        [TestMethod]
        public void SourceVersionAttributePreservesConfiguredValueForDataRowVersionZero()
        {
            // Setup
            DataRowVersion value = (DataRowVersion)0;
            var attribute = new global::RepoDb.Attributes.Parameter.Db2.SourceVersionAttribute(value);

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