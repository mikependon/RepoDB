using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Resolvers;
using Sap.Data.Hana;
using System.Data;
using System;

namespace RepoDb.SapHana.UnitTests.Resolvers
{
    /// <summary>
    /// Covers supported mappings and resolver boundaries.
    /// </summary>
    [TestClass]
    public class SapHanaDbTypeToStringNameResolverBoundaryTest
    {
        #region Public Methods

        /// <summary>
        /// Resolves supported values and undefined enum values.
        /// </summary>
        /// <param name="dbType">The provider type.</param>
        /// <param name="expected">The expected SQL type name.</param>
        [DataRow(HanaDbType.TinyInt,
            "TINYINT")]
        [DataRow(HanaDbType.SmallInt,
            "SMALLINT")]
        [DataRow(HanaDbType.Integer,
            "INTEGER")]
        [DataRow(HanaDbType.BigInt,
            "BIGINT")]
        [DataRow(HanaDbType.Decimal,
            "DECIMAL")]
        [DataRow(HanaDbType.SmallDecimal,
            "SMALLDECIMAL")]
        [DataRow(HanaDbType.Real,
            "REAL")]
        [DataRow(HanaDbType.Double,
            "DOUBLE")]
        [DataRow(HanaDbType.Boolean,
            "BOOLEAN")]
        [DataRow(HanaDbType.VarChar,
            "VARCHAR")]
        [DataRow(HanaDbType.NVarChar,
            "NVARCHAR")]
        [DataRow(HanaDbType.Text,
            "TEXT")]
        [DataRow(HanaDbType.Clob,
            "CLOB")]
        [DataRow(HanaDbType.NClob,
            "NCLOB")]
        [DataRow(HanaDbType.Date,
            "DATE")]
        [DataRow(HanaDbType.Time,
            "TIME")]
        [DataRow(HanaDbType.TimeStamp,
            "TIMESTAMP")]
        [DataRow(HanaDbType.SecondDate,
            "SECONDDATE")]
        [DataRow(HanaDbType.Blob,
            "BLOB")]
        [DataRow(HanaDbType.VarBinary,
            "VARBINARY")]
        [DataRow((HanaDbType)(-1),
            "TEXT")]
        [TestMethod]
        public void ResolveReturnsExpectedSqlTypeName(HanaDbType dbType,
            string expected)
        {
            // Setup
            var resolver = new SapHanaDbTypeToStringNameResolver();


            // Act
            var actual = resolver.Resolve(dbType);


            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        #endregion
    }
}