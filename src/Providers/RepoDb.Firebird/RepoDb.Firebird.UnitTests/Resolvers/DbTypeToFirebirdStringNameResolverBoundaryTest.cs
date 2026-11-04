using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Resolvers;
using System.Data;
using System;

namespace RepoDb.Firebird.UnitTests.Resolvers
{
    /// <summary>
    /// Covers supported mappings and resolver boundaries.
    /// </summary>
    [TestClass]
    public class DbTypeToFirebirdStringNameResolverBoundaryTest
    {
        #region Public Methods

        /// <summary>
        /// Resolves supported values and undefined enum values.
        /// </summary>
        /// <param name="dbType">The provider type.</param>
        /// <param name="expected">The expected SQL type name.</param>
        [DataRow(DbType.Int64,
            "BIGINT")]
        [DataRow(DbType.Binary,
            "BLOB SUB_TYPE 0")]
        [DataRow(DbType.Boolean,
            "BOOLEAN")]
        [DataRow(DbType.String,
            "VARCHAR(8191)")]
        [DataRow(DbType.Date,
            "DATE")]
        [DataRow(DbType.DateTime,
            "TIMESTAMP")]
        [DataRow(DbType.DateTime2,
            "TIMESTAMP")]
        [DataRow(DbType.DateTimeOffset,
            "TIMESTAMP WITH TIME ZONE")]
        [DataRow(DbType.Decimal,
            "DECIMAL(18,2)")]
        [DataRow(DbType.Single,
            "FLOAT")]
        [DataRow(DbType.Double,
            "DOUBLE PRECISION")]
        [DataRow(DbType.Int32,
            "INTEGER")]
        [DataRow(DbType.Int16,
            "SMALLINT")]
        [DataRow(DbType.Time,
            "TIME")]
        [DataRow(DbType.Byte,
            "SMALLINT")]
        [DataRow(DbType.Guid,
            "CHAR(16) CHARACTER SET OCTETS")]
        [DataRow(DbType.AnsiString,
            "VARCHAR(8191)")]
        [DataRow(DbType.AnsiStringFixedLength,
            "CHAR(8191)")]
        [DataRow(DbType.StringFixedLength,
            "NCHAR(8191)")]
        [DataRow(DbType.Object,
            "BLOB SUB_TYPE 0")]
        [DataRow(DbType.Xml,
            "BLOB SUB_TYPE TEXT")]
        [DataRow((DbType)(-1),
            "VARCHAR(8191)")]
        [TestMethod]
        public void ResolveReturnsExpectedSqlTypeName(DbType dbType,
            string expected)
        {
            // Setup
            var resolver = new DbTypeToFirebirdStringNameResolver();


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