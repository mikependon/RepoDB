using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Data.Sqlite;
using System.Data;
using System;

namespace RepoDb.Sqlite.Microsoft.UnitTests.Attributes.Parameter
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
        public void SqliteTypeAttributePreservesConfiguredValueForSqliteTypeZero()
        {
            // Setup
            SqliteType value = (SqliteType)0;
            var attribute = new global::RepoDb.Attributes.Parameter.Sqlite.SqliteTypeAttribute(value);

            // Act
            var actual = attribute.SqliteType;

            // Assert
            Assert.AreEqual(value,
                actual);
        }

        /// <summary>
        /// Retains the value supplied to the parameter attribute.
        /// </summary>
        [TestMethod]
        public void SqliteTypeAttributePreservesConfiguredValueForSqliteTypeOne()
        {
            // Setup
            SqliteType value = (SqliteType)1;
            var attribute = new global::RepoDb.Attributes.Parameter.Sqlite.SqliteTypeAttribute(value);

            // Act
            var actual = attribute.SqliteType;

            // Assert
            Assert.AreEqual(value,
                actual);
        }

        #endregion
    }
}