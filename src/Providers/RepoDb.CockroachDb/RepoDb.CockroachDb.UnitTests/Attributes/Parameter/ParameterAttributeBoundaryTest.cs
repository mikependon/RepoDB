using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Connector.CockroachDb;
using System.Data;
using System;

namespace RepoDb.CockroachDb.UnitTests.Attributes.Parameter
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
        public void CockroachDbTypeAttributePreservesConfiguredValueForCockroachDbTypeZero()
        {
            // Setup
            CockroachDbType value = (CockroachDbType)0;
            var attribute = new global::RepoDb.Attributes.Parameter.CockroachDb.CockroachDbTypeAttribute(value);

            // Act
            var actual = attribute.CockroachDbType;

            // Assert
            Assert.AreEqual(value,
                actual);
        }

        /// <summary>
        /// Retains the value supplied to the parameter attribute.
        /// </summary>
        [TestMethod]
        public void CockroachDbTypeAttributePreservesConfiguredValueForCockroachDbTypeOne()
        {
            // Setup
            CockroachDbType value = (CockroachDbType)1;
            var attribute = new global::RepoDb.Attributes.Parameter.CockroachDb.CockroachDbTypeAttribute(value);

            // Act
            var actual = attribute.CockroachDbType;

            // Assert
            Assert.AreEqual(value,
                actual);
        }

        /// <summary>
        /// Retains the value supplied to the parameter attribute.
        /// </summary>
        [TestMethod]
        public void DbTypeAttributePreservesConfiguredValueForDbTypeZero()
        {
            // Setup
            DbType value = (DbType)0;
            var attribute = new global::RepoDb.Attributes.Parameter.CockroachDb.DbTypeAttribute(value);

            // Act
            var actual = attribute.DbType;

            // Assert
            Assert.AreEqual(value,
                actual);
        }

        /// <summary>
        /// Retains the value supplied to the parameter attribute.
        /// </summary>
        [TestMethod]
        public void DbTypeAttributePreservesConfiguredValueForDbTypeOne()
        {
            // Setup
            DbType value = (DbType)1;
            var attribute = new global::RepoDb.Attributes.Parameter.CockroachDb.DbTypeAttribute(value);

            // Act
            var actual = attribute.DbType;

            // Assert
            Assert.AreEqual(value,
                actual);
        }

        /// <summary>
        /// Retains the value supplied to the parameter attribute.
        /// </summary>
        [TestMethod]
        public void DirectionAttributePreservesConfiguredValueForParameterDirectionZero()
        {
            // Setup
            ParameterDirection value = (ParameterDirection)0;
            var attribute = new global::RepoDb.Attributes.Parameter.CockroachDb.DirectionAttribute(value);

            // Act
            var actual = attribute.Direction;

            // Assert
            Assert.AreEqual(value,
                actual);
        }

        /// <summary>
        /// Retains the value supplied to the parameter attribute.
        /// </summary>
        [TestMethod]
        public void DirectionAttributePreservesConfiguredValueForParameterDirectionOne()
        {
            // Setup
            ParameterDirection value = (ParameterDirection)1;
            var attribute = new global::RepoDb.Attributes.Parameter.CockroachDb.DirectionAttribute(value);

            // Act
            var actual = attribute.Direction;

            // Assert
            Assert.AreEqual(value,
                actual);
        }

        /// <summary>
        /// Retains the value supplied to the parameter attribute.
        /// </summary>
        [TestMethod]
        public void IsNullableAttributePreservesConfiguredValueForFalse()
        {
            // Setup
            bool value = false;
            var attribute = new global::RepoDb.Attributes.Parameter.CockroachDb.IsNullableAttribute(value);

            // Act
            var actual = attribute.IsNullable;

            // Assert
            Assert.AreEqual(value,
                actual);
        }

        /// <summary>
        /// Retains the value supplied to the parameter attribute.
        /// </summary>
        [TestMethod]
        public void IsNullableAttributePreservesConfiguredValueForTrue()
        {
            // Setup
            bool value = true;
            var attribute = new global::RepoDb.Attributes.Parameter.CockroachDb.IsNullableAttribute(value);

            // Act
            var actual = attribute.IsNullable;

            // Assert
            Assert.AreEqual(value,
                actual);
        }

        /// <summary>
        /// Retains the value supplied to the parameter attribute.
        /// </summary>
        [TestMethod]
        public void ParameterNameAttributePreservesConfiguredValueForNull()
        {
            // Setup
            string value = null;
            var attribute = new global::RepoDb.Attributes.Parameter.CockroachDb.ParameterNameAttribute(value);

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
        [TestMethod]
        public void ParameterNameAttributePreservesConfiguredValueForEmptyString()
        {
            // Setup
            string value = "";
            var attribute = new global::RepoDb.Attributes.Parameter.CockroachDb.ParameterNameAttribute(value);

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
        [TestMethod]
        public void ParameterNameAttributePreservesConfiguredValueForColumnInMixedCase()
        {
            // Setup
            string value = "Column";
            var attribute = new global::RepoDb.Attributes.Parameter.CockroachDb.ParameterNameAttribute(value);

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
        [TestMethod]
        public void PrecisionAttributePreservesConfiguredValueForByteZero()
        {
            // Setup
            byte value = (byte)0;
            var attribute = new global::RepoDb.Attributes.Parameter.CockroachDb.PrecisionAttribute(value);

            // Act
            var actual = attribute.Precision;

            // Assert
            Assert.AreEqual(value,
                actual);
        }

        /// <summary>
        /// Retains the value supplied to the parameter attribute.
        /// </summary>
        [TestMethod]
        public void PrecisionAttributePreservesConfiguredValueForByteValue255()
        {
            // Setup
            byte value = (byte)255;
            var attribute = new global::RepoDb.Attributes.Parameter.CockroachDb.PrecisionAttribute(value);

            // Act
            var actual = attribute.Precision;

            // Assert
            Assert.AreEqual(value,
                actual);
        }

        /// <summary>
        /// Retains the value supplied to the parameter attribute.
        /// </summary>
        [TestMethod]
        public void ScaleAttributePreservesConfiguredValueForByteZero()
        {
            // Setup
            byte value = (byte)0;
            var attribute = new global::RepoDb.Attributes.Parameter.CockroachDb.ScaleAttribute(value);

            // Act
            var actual = attribute.Scale;

            // Assert
            Assert.AreEqual(value,
                actual);
        }

        /// <summary>
        /// Retains the value supplied to the parameter attribute.
        /// </summary>
        [TestMethod]
        public void ScaleAttributePreservesConfiguredValueForByteValue255()
        {
            // Setup
            byte value = (byte)255;
            var attribute = new global::RepoDb.Attributes.Parameter.CockroachDb.ScaleAttribute(value);

            // Act
            var actual = attribute.Scale;

            // Assert
            Assert.AreEqual(value,
                actual);
        }

        /// <summary>
        /// Retains the value supplied to the parameter attribute.
        /// </summary>
        [TestMethod]
        public void SizeAttributePreservesConfiguredValueForNegativeOne()
        {
            // Setup
            int value = -1;
            var attribute = new global::RepoDb.Attributes.Parameter.CockroachDb.SizeAttribute(value);

            // Act
            var actual = attribute.Size;

            // Assert
            Assert.AreEqual(value,
                actual);
        }

        /// <summary>
        /// Retains the value supplied to the parameter attribute.
        /// </summary>
        [TestMethod]
        public void SizeAttributePreservesConfiguredValueForZero()
        {
            // Setup
            int value = 0;
            var attribute = new global::RepoDb.Attributes.Parameter.CockroachDb.SizeAttribute(value);

            // Act
            var actual = attribute.Size;

            // Assert
            Assert.AreEqual(value,
                actual);
        }

        /// <summary>
        /// Retains the value supplied to the parameter attribute.
        /// </summary>
        [TestMethod]
        public void SizeAttributePreservesConfiguredValueForInt32MaxValue()
        {
            // Setup
            int value = int.MaxValue;
            var attribute = new global::RepoDb.Attributes.Parameter.CockroachDb.SizeAttribute(value);

            // Act
            var actual = attribute.Size;

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
            var attribute = new global::RepoDb.Attributes.Parameter.CockroachDb.SourceColumnAttribute(value);

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
            var attribute = new global::RepoDb.Attributes.Parameter.CockroachDb.SourceColumnAttribute(value);

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
            var attribute = new global::RepoDb.Attributes.Parameter.CockroachDb.SourceColumnAttribute(value);

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
            var attribute = new global::RepoDb.Attributes.Parameter.CockroachDb.SourceColumnNullMappingAttribute(value);

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
            var attribute = new global::RepoDb.Attributes.Parameter.CockroachDb.SourceColumnNullMappingAttribute(value);

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
            var attribute = new global::RepoDb.Attributes.Parameter.CockroachDb.SourceVersionAttribute(value);

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
            var attribute = new global::RepoDb.Attributes.Parameter.CockroachDb.SourceVersionAttribute(value);

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
        public void ValueAttributePreservesConfiguredValueForNull()
        {
            // Setup
            object value = null;
            var attribute = new global::RepoDb.Attributes.Parameter.CockroachDb.ValueAttribute(value);

            // Act
            var actual = attribute.Value;

            // Assert
            Assert.AreEqual(value,
                actual);
        }

        /// <summary>
        /// Retains the value supplied to the parameter attribute.
        /// </summary>
        [TestMethod]
        public void ValueAttributePreservesConfiguredValueForValue42()
        {
            // Setup
            object value = 42;
            var attribute = new global::RepoDb.Attributes.Parameter.CockroachDb.ValueAttribute(value);

            // Act
            var actual = attribute.Value;

            // Assert
            Assert.AreEqual(value,
                actual);
        }

        #endregion
    }
}