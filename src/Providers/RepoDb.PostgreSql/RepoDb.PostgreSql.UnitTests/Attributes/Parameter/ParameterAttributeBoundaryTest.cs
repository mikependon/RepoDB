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
        /// <param name="value">The configured parameter value.</param>
        [DataRow(null)]
        [DataRow("")]
        [DataRow("Column")]
        [TestMethod]
        public void DataTypeNameAttributePreservesConfiguredValue(string value)
        {
            // Setup
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
        /// <param name="value">The configured parameter value.</param>
        [DataRow((NpgsqlDbType)0)]
        [DataRow((NpgsqlDbType)1)]
        [TestMethod]
        public void NpgsqlDbTypeAttributePreservesConfiguredValue(NpgsqlDbType value)
        {
            // Setup
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