using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Resolvers;
using System.Data;
using System;

namespace RepoDb.SqlServer.UnitTests.Resolvers
{
    /// <summary>
    /// Covers supported mappings and resolver boundaries.
    /// </summary>
    [TestClass]
    public class DbTypeToSqlServerStringNameResolverBoundaryTest
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
            "BINARY")]
        [DataRow(DbType.Boolean,
            "BIT")]
        [DataRow(DbType.String,
            "NVARCHAR")]
        [DataRow(DbType.Date,
            "DATE")]
        [DataRow(DbType.DateTime,
            "DATETIME")]
        [DataRow(DbType.DateTime2,
            "DATETIME2")]
        [DataRow(DbType.DateTimeOffset,
            "DATETIMEOFFSET")]
        [DataRow(DbType.Decimal,
            "DECIMAL(18,2)")]
        [DataRow(DbType.Single,
            "REAL")]
        [DataRow(DbType.Double,
            "FLOAT")]
        [DataRow(DbType.Int32,
            "INT")]
        [DataRow(DbType.Int16,
            "SMALLINT")]
        [DataRow(DbType.Time,
            "TIME")]
        [DataRow(DbType.Byte,
            "TINYINT")]
        [DataRow(DbType.Guid,
            "UNIQUEIDENTIFIER")]
        [DataRow(DbType.AnsiString,
            "VARCHAR")]
        [DataRow(DbType.AnsiStringFixedLength,
            "CHAR")]
        [DataRow(DbType.StringFixedLength,
            "NCHAR")]
        [DataRow(DbType.Object,
            "OBJECT")]
        [DataRow(DbType.Xml,
            "XML")]
        [DataRow((DbType)(-1),
            "NVARCHAR")]
        [TestMethod]
        public void ResolveReturnsExpectedSqlTypeName(DbType dbType,
            string expected)
        {
            // Setup
            var resolver = new DbTypeToSqlServerStringNameResolver();


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