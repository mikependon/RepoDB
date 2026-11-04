using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data;
using System;
using Turso.Data.Sqlite;

namespace RepoDb.Sqlite.Turso.UnitTests.Attributes.Parameter
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
        [DataRow((SqliteType)0)]
        [DataRow((SqliteType)1)]
        [TestMethod]
        public void SqliteTypeAttributePreservesConfiguredValue(SqliteType value)
        {
            // Setup
            var attribute = new global::RepoDb.Attributes.Parameter.Turso.SqliteTypeAttribute(value);


            // Act
            var actual = attribute.SqliteType;


            // Assert
            Assert.AreEqual(value,
                actual);
        }

        #endregion
    }
}