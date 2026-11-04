using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Connector.AuroraDb.MySqlConnector;
using RepoDb.Resolvers;
using System;

namespace RepoDb.AuroraDb.MySqlConnector.UnitTests.Resolvers
{
    /// <summary>
    /// Covers supported mappings and resolver boundaries.
    /// </summary>
    [TestClass]
    public class AuroraDbDbTypeToStringNameResolverBoundaryTest
    {
        #region Public Methods

        /// <summary>
        /// Resolves supported values and undefined enum values.
        /// </summary>
        /// <param name="dbType">The provider type.</param>
        /// <param name="expected">The expected SQL type name.</param>
        [DataRow(AuroraDbType.TinyInt,
            "TINYINT")]
        [DataRow(AuroraDbType.SmallInt,
            "SMALLINT")]
        [DataRow(AuroraDbType.MediumInt,
            "MEDIUMINT")]
        [DataRow(AuroraDbType.Int,
            "INT")]
        [DataRow(AuroraDbType.BigInt,
            "BIGINT")]
        [DataRow(AuroraDbType.Decimal,
            "DECIMAL")]
        [DataRow(AuroraDbType.Float,
            "FLOAT")]
        [DataRow(AuroraDbType.Double,
            "DOUBLE")]
        [DataRow(AuroraDbType.Bit,
            "BIT")]
        [DataRow(AuroraDbType.Char,
            "CHAR")]
        [DataRow(AuroraDbType.VarChar,
            "VARCHAR")]
        [DataRow(AuroraDbType.TinyText,
            "TINYTEXT")]
        [DataRow(AuroraDbType.Text,
            "TEXT")]
        [DataRow(AuroraDbType.Enum,
            "TEXT")]
        [DataRow(AuroraDbType.Set,
            "TEXT")]
        [DataRow(AuroraDbType.MediumText,
            "MEDIUMTEXT")]
        [DataRow(AuroraDbType.LongText,
            "LONGTEXT")]
        [DataRow(AuroraDbType.Binary,
            "BINARY")]
        [DataRow(AuroraDbType.VarBinary,
            "VARBINARY")]
        [DataRow(AuroraDbType.TinyBlob,
            "TINYBLOB")]
        [DataRow(AuroraDbType.Blob,
            "BLOB")]
        [DataRow(AuroraDbType.MediumBlob,
            "MEDIUMBLOB")]
        [DataRow(AuroraDbType.LongBlob,
            "LONGBLOB")]
        [DataRow(AuroraDbType.Date,
            "DATE")]
        [DataRow(AuroraDbType.Time,
            "TIME")]
        [DataRow(AuroraDbType.DateTime,
            "DATETIME")]
        [DataRow(AuroraDbType.Timestamp,
            "TIMESTAMP")]
        [DataRow(AuroraDbType.Year,
            "YEAR")]
        [DataRow(AuroraDbType.Json,
            "JSON")]
        [DataRow(AuroraDbType.Geometry,
            "GEOMETRY")]
        [DataRow(AuroraDbType.Point,
            "GEOMETRY")]
        [DataRow(AuroraDbType.LineString,
            "GEOMETRY")]
        [DataRow(AuroraDbType.Polygon,
            "GEOMETRY")]
        [DataRow(AuroraDbType.MultiPoint,
            "GEOMETRY")]
        [DataRow(AuroraDbType.MultiLineString,
            "GEOMETRY")]
        [DataRow(AuroraDbType.MultiPolygon,
            "GEOMETRY")]
        [DataRow(AuroraDbType.GeometryCollection,
            "GEOMETRY")]
        [DataRow((AuroraDbType)(-1),
            "TEXT")]
        [TestMethod]
        public void ResolveReturnsExpectedSqlTypeName(AuroraDbType dbType,
            string expected)
        {
            // Setup
            var resolver = new AuroraDbDbTypeToStringNameResolver();


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