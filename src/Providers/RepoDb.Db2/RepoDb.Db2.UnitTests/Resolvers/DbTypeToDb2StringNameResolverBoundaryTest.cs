using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Resolvers;
using System.Data;
using System;

namespace RepoDb.Db2.UnitTests.Resolvers
{
    /// <summary>
    /// Covers supported mappings and resolver boundaries.
    /// </summary>
    [TestClass]
    public class DbTypeToDb2StringNameResolverBoundaryTest
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
            "BLOB(1M)")]
        [DataRow(DbType.Boolean,
            "SMALLINT")]
        [DataRow(DbType.String,
            "VARCHAR(2000)")]
        [DataRow(DbType.Date,
            "DATE")]
        [DataRow(DbType.DateTime,
            "TIMESTAMP")]
        [DataRow(DbType.DateTime2,
            "TIMESTAMP")]
        [DataRow(DbType.DateTimeOffset,
            "TIMESTAMP")]
        [DataRow(DbType.Decimal,
            "DECIMAL(31,15)")]
        [DataRow(DbType.Single,
            "REAL")]
        [DataRow(DbType.Double,
            "DOUBLE")]
        [DataRow(DbType.Int32,
            "INTEGER")]
        [DataRow(DbType.Int16,
            "SMALLINT")]
        [DataRow(DbType.Time,
            "TIME")]
        [DataRow(DbType.Byte,
            "SMALLINT")]
        [DataRow(DbType.Guid,
            "CHAR(16) FOR BIT DATA")]
        [DataRow(DbType.AnsiString,
            "VARCHAR(2000)")]
        [DataRow(DbType.AnsiStringFixedLength,
            "CHAR(254)")]
        [DataRow(DbType.StringFixedLength,
            "GRAPHIC(127)")]
        [DataRow(DbType.Object,
            "BLOB(1M)")]
        [DataRow(DbType.Xml,
            "XML")]
        [DataRow((DbType)(-1),
            "VARCHAR(2000)")]
        [TestMethod]
        public void ResolveReturnsExpectedSqlTypeName(DbType dbType,
            string expected)
        {
            // Setup
            var resolver = new DbTypeToDb2StringNameResolver();


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