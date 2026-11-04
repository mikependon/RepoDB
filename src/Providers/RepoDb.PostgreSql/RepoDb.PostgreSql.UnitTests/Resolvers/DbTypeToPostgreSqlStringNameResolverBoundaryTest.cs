using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Resolvers;
using System.Data;
using System;

namespace RepoDb.PostgreSql.UnitTests.Resolvers
{
    /// <summary>
    /// Covers supported mappings and resolver boundaries.
    /// </summary>
    [TestClass]
    public class DbTypeToPostgreSqlStringNameResolverBoundaryTest
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
            "BYTEA")]
        [DataRow(DbType.Byte,
            "BYTEA")]
        [DataRow(DbType.Boolean,
            "BOOLEAN")]
        [DataRow(DbType.AnsiString,
            "TEXT")]
        [DataRow(DbType.AnsiStringFixedLength,
            "TEXT")]
        [DataRow(DbType.String,
            "TEXT")]
        [DataRow(DbType.StringFixedLength,
            "TEXT")]
        [DataRow(DbType.Date,
            "DATE")]
        [DataRow(DbType.DateTime,
            "TIMESTAMP")]
        [DataRow(DbType.DateTime2,
            "TIMESTAMP")]
        [DataRow(DbType.DateTimeOffset,
            "TIMESTAMPTZ")]
        [DataRow(DbType.Decimal,
            "NUMERIC")]
        [DataRow(DbType.Single,
            "REAL")]
        [DataRow(DbType.Double,
            "DOUBLE PRECISION")]
        [DataRow(DbType.Int32,
            "INTEGER")]
        [DataRow(DbType.Int16,
            "SMALLINT")]
        [DataRow(DbType.Time,
            "INTERVAL")]
        [DataRow((DbType)(-1),
            "TEXT")]
        [TestMethod]
        public void ResolveReturnsExpectedSqlTypeName(DbType dbType,
            string expected)
        {
            // Setup
            var resolver = new DbTypeToPostgreSqlStringNameResolver();


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