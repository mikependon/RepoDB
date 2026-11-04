using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Connector.MariaDb;
using RepoDb.Resolvers;
using System;

namespace RepoDb.MariaDb.UnitTests.Resolvers
{
    /// <summary>
    /// Covers supported mappings and resolver boundaries.
    /// </summary>
    [TestClass]
    public class MariaDbDbTypeToStringNameResolverBoundaryTest
    {
        #region Public Methods

        /// <summary>
        /// Resolves supported values and undefined enum values.
        /// </summary>
        /// <param name="dbType">The provider type.</param>
        /// <param name="expected">The expected SQL type name.</param>
        [DataRow(MariaDbType.TinyInt,
            "TINYINT")]
        [DataRow(MariaDbType.SmallInt,
            "SMALLINT")]
        [DataRow(MariaDbType.MediumInt,
            "MEDIUMINT")]
        [DataRow(MariaDbType.Int,
            "INT")]
        [DataRow(MariaDbType.BigInt,
            "BIGINT")]
        [DataRow(MariaDbType.Decimal,
            "DECIMAL")]
        [DataRow(MariaDbType.Float,
            "FLOAT")]
        [DataRow(MariaDbType.Double,
            "DOUBLE")]
        [DataRow(MariaDbType.Bit,
            "BIT")]
        [DataRow(MariaDbType.Char,
            "CHAR")]
        [DataRow(MariaDbType.VarChar,
            "VARCHAR")]
        [DataRow(MariaDbType.TinyText,
            "TINYTEXT")]
        [DataRow(MariaDbType.Text,
            "TEXT")]
        [DataRow(MariaDbType.Enum,
            "TEXT")]
        [DataRow(MariaDbType.Set,
            "TEXT")]
        [DataRow(MariaDbType.MediumText,
            "MEDIUMTEXT")]
        [DataRow(MariaDbType.LongText,
            "LONGTEXT")]
        [DataRow(MariaDbType.Binary,
            "BINARY")]
        [DataRow(MariaDbType.VarBinary,
            "VARBINARY")]
        [DataRow(MariaDbType.TinyBlob,
            "TINYBLOB")]
        [DataRow(MariaDbType.Blob,
            "BLOB")]
        [DataRow(MariaDbType.MediumBlob,
            "MEDIUMBLOB")]
        [DataRow(MariaDbType.LongBlob,
            "LONGBLOB")]
        [DataRow(MariaDbType.Date,
            "DATE")]
        [DataRow(MariaDbType.Time,
            "TIME")]
        [DataRow(MariaDbType.DateTime,
            "DATETIME")]
        [DataRow(MariaDbType.Timestamp,
            "TIMESTAMP")]
        [DataRow(MariaDbType.Year,
            "YEAR")]
        [DataRow(MariaDbType.Json,
            "JSON")]
        [DataRow(MariaDbType.Geometry,
            "GEOMETRY")]
        [DataRow(MariaDbType.Point,
            "GEOMETRY")]
        [DataRow(MariaDbType.LineString,
            "GEOMETRY")]
        [DataRow(MariaDbType.Polygon,
            "GEOMETRY")]
        [DataRow(MariaDbType.MultiPoint,
            "GEOMETRY")]
        [DataRow(MariaDbType.MultiLineString,
            "GEOMETRY")]
        [DataRow(MariaDbType.MultiPolygon,
            "GEOMETRY")]
        [DataRow(MariaDbType.GeometryCollection,
            "GEOMETRY")]
        [DataRow((MariaDbType)(-1),
            "TEXT")]
        [TestMethod]
        public void ResolveReturnsExpectedSqlTypeName(MariaDbType dbType,
            string expected)
        {
            // Setup
            var resolver = new MariaDbDbTypeToStringNameResolver();


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