using Microsoft.VisualStudio.TestTools.UnitTesting;
using Npgsql;
using NpgsqlTypes;
using System.Data;
using System;

namespace RepoDb.PostgreSql.UnitTests.Attributes.Parameter
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
        public void DataTypeNameAttributePreservesConfiguredValueForNull()
        {
            // Setup
            string value = null;
            var attribute = new global::RepoDb.Attributes.Parameter.Npgsql.DataTypeNameAttribute(value);

            // Act
            var actual = attribute.DataTypeName;

            // Assert
            Assert.AreEqual(value,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Retains the value supplied to the parameter attribute.
        /// </summary>
        [TestMethod]
        public void DataTypeNameAttributePreservesConfiguredValueForEmptyString()
        {
            // Setup
            string value = "";
            var attribute = new global::RepoDb.Attributes.Parameter.Npgsql.DataTypeNameAttribute(value);

            // Act
            var actual = attribute.DataTypeName;

            // Assert
            Assert.AreEqual(value,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Retains the value supplied to the parameter attribute.
        /// </summary>
        [TestMethod]
        public void DataTypeNameAttributePreservesConfiguredValueForColumnInMixedCase()
        {
            // Setup
            string value = "Column";
            var attribute = new global::RepoDb.Attributes.Parameter.Npgsql.DataTypeNameAttribute(value);

            // Act
            var actual = attribute.DataTypeName;

            // Assert
            Assert.AreEqual(value,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Retains the value supplied to the parameter attribute.
        /// </summary>
        [TestMethod]
        public void NpgsqlDbTypeAttributePreservesConfiguredValueForNpgsqlDbTypeZero()
        {
            // Setup
            NpgsqlDbType value = (NpgsqlDbType)0;
            var attribute = new global::RepoDb.Attributes.Parameter.Npgsql.NpgsqlDbTypeAttribute(value);

            // Act
            var actual = attribute.NpgsqlDbType;

            // Assert
            Assert.AreEqual(value,
                actual);
        }

        /// <summary>
        /// Retains the value supplied to the parameter attribute.
        /// </summary>
        [TestMethod]
        public void NpgsqlDbTypeAttributePreservesConfiguredValueForNpgsqlDbTypeOne()
        {
            // Setup
            NpgsqlDbType value = (NpgsqlDbType)1;
            var attribute = new global::RepoDb.Attributes.Parameter.Npgsql.NpgsqlDbTypeAttribute(value);

            // Act
            var actual = attribute.NpgsqlDbType;

            // Assert
            Assert.AreEqual(value,
                actual);
        }

        #endregion
    }
}