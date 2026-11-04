using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Resolvers;
using System.Data;
using System;

namespace RepoDb.Sqlite.Ahtola.UnitTests.Resolvers
{
    /// <summary>
    /// Covers supported mappings and resolver boundaries.
    /// </summary>
    [TestClass]
    public class DbTypeToAhtolaStringNameResolverBoundaryTest
    {
        #region Public Methods

        /// <summary>
        /// Resolves supported values and undefined enum values.
        /// </summary>
        /// <param name="dbType">The provider type.</param>
        /// <param name="expected">The expected SQL type name.</param>
        [DataRow(DbType.Int64,
            "BIGINT")]
        [DataRow(DbType.Byte,
            "BLOB")]
        [DataRow(DbType.Binary,
            "BLOB")]
        [DataRow(DbType.Boolean,
            "BOOLEAN")]
        [DataRow(DbType.String,
            "TEXT")]
        [DataRow(DbType.AnsiString,
            "TEXT")]
        [DataRow(DbType.AnsiStringFixedLength,
            "TEXT")]
        [DataRow(DbType.StringFixedLength,
            "TEXT")]
        [DataRow(DbType.Date,
            "DATE")]
        [DataRow(DbType.DateTime,
            "DATETIME")]
        [DataRow(DbType.DateTime2,
            "DATETIME")]
        [DataRow(DbType.DateTimeOffset,
            "DATETIME")]
        [DataRow(DbType.Decimal,
            "DECIMAL")]
        [DataRow(DbType.Single,
            "REAL")]
        [DataRow(DbType.Double,
            "DOUBLE")]
        [DataRow(DbType.Int32,
            "INT")]
        [DataRow(DbType.Int16,
            "INT")]
        [DataRow(DbType.Time,
            "TIME")]
        [DataRow((DbType)(-1),
            "TEXT")]
        [TestMethod]
        public void ResolveReturnsExpectedSqlTypeName(DbType dbType,
            string expected)
        {
            // Setup
            var resolver = new DbTypeToAhtolaStringNameResolver();


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