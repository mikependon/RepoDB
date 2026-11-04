using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Resolvers;
using System.Data;
using System;

namespace RepoDb.Oracle.UnitTests.Resolvers
{
    /// <summary>
    /// Covers supported mappings and resolver boundaries.
    /// </summary>
    [TestClass]
    public class DbTypeToOracleStringNameResolverBoundaryTest
    {
        #region Public Methods

        /// <summary>
        /// Resolves supported values and undefined enum values.
        /// </summary>
        /// <param name="dbType">The provider type.</param>
        /// <param name="expected">The expected SQL type name.</param>
        [DataRow(DbType.Int64,
            "NUMBER(19)")]
        [DataRow(DbType.Binary,
            "BLOB")]
        [DataRow(DbType.Boolean,
            "NUMBER(1)")]
        [DataRow(DbType.String,
            "NVARCHAR2(2000)")]
        [DataRow(DbType.Date,
            "DATE")]
        [DataRow(DbType.DateTime,
            "DATE")]
        [DataRow(DbType.DateTime2,
            "TIMESTAMP")]
        [DataRow(DbType.DateTimeOffset,
            "TIMESTAMP WITH TIME ZONE")]
        [DataRow(DbType.Decimal,
            "NUMBER(18,2)")]
        [DataRow(DbType.Single,
            "BINARY_FLOAT")]
        [DataRow(DbType.Double,
            "BINARY_DOUBLE")]
        [DataRow(DbType.Int32,
            "NUMBER(10)")]
        [DataRow(DbType.Int16,
            "NUMBER(5)")]
        [DataRow(DbType.Time,
            "INTERVAL DAY(0) TO SECOND(7)")]
        [DataRow(DbType.Byte,
            "NUMBER(3)")]
        [DataRow(DbType.Guid,
            "RAW(16)")]
        [DataRow(DbType.AnsiString,
            "VARCHAR2(2000)")]
        [DataRow(DbType.AnsiStringFixedLength,
            "CHAR(2000)")]
        [DataRow(DbType.StringFixedLength,
            "NCHAR(2000)")]
        [DataRow(DbType.Object,
            "BLOB")]
        [DataRow(DbType.Xml,
            "XMLTYPE")]
        [DataRow((DbType)(-1),
            "NVARCHAR2(2000)")]
        [TestMethod]
        public void ResolveReturnsExpectedSqlTypeName(DbType dbType,
            string expected)
        {
            // Setup
            var resolver = new DbTypeToOracleStringNameResolver();


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