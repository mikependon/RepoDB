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
        [TestMethod]
        public void ResolveReturnsExpectedSqlTypeNameForAuroraDbTypeTinyInt()
        {
            // Setup
            AuroraDbType dbType = AuroraDbType.TinyInt;
            string expected = "TINYINT";
            var resolver = new AuroraDbDbTypeToStringNameResolver();

            // Act
            var actual = resolver.Resolve(dbType);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Resolves supported values and undefined enum values.
        /// </summary>
        [TestMethod]
        public void ResolveReturnsExpectedSqlTypeNameForAuroraDbTypeSmallInt()
        {
            // Setup
            AuroraDbType dbType = AuroraDbType.SmallInt;
            string expected = "SMALLINT";
            var resolver = new AuroraDbDbTypeToStringNameResolver();

            // Act
            var actual = resolver.Resolve(dbType);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Resolves supported values and undefined enum values.
        /// </summary>
        [TestMethod]
        public void ResolveReturnsExpectedSqlTypeNameForAuroraDbTypeMediumInt()
        {
            // Setup
            AuroraDbType dbType = AuroraDbType.MediumInt;
            string expected = "MEDIUMINT";
            var resolver = new AuroraDbDbTypeToStringNameResolver();

            // Act
            var actual = resolver.Resolve(dbType);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Resolves supported values and undefined enum values.
        /// </summary>
        [TestMethod]
        public void ResolveReturnsExpectedSqlTypeNameForAuroraDbTypeInt()
        {
            // Setup
            AuroraDbType dbType = AuroraDbType.Int;
            string expected = "INT";
            var resolver = new AuroraDbDbTypeToStringNameResolver();

            // Act
            var actual = resolver.Resolve(dbType);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Resolves supported values and undefined enum values.
        /// </summary>
        [TestMethod]
        public void ResolveReturnsExpectedSqlTypeNameForAuroraDbTypeBigInt()
        {
            // Setup
            AuroraDbType dbType = AuroraDbType.BigInt;
            string expected = "BIGINT";
            var resolver = new AuroraDbDbTypeToStringNameResolver();

            // Act
            var actual = resolver.Resolve(dbType);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Resolves supported values and undefined enum values.
        /// </summary>
        [TestMethod]
        public void ResolveReturnsExpectedSqlTypeNameForAuroraDbTypeDecimal()
        {
            // Setup
            AuroraDbType dbType = AuroraDbType.Decimal;
            string expected = "DECIMAL";
            var resolver = new AuroraDbDbTypeToStringNameResolver();

            // Act
            var actual = resolver.Resolve(dbType);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Resolves supported values and undefined enum values.
        /// </summary>
        [TestMethod]
        public void ResolveReturnsExpectedSqlTypeNameForAuroraDbTypeFloat()
        {
            // Setup
            AuroraDbType dbType = AuroraDbType.Float;
            string expected = "FLOAT";
            var resolver = new AuroraDbDbTypeToStringNameResolver();

            // Act
            var actual = resolver.Resolve(dbType);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Resolves supported values and undefined enum values.
        /// </summary>
        [TestMethod]
        public void ResolveReturnsExpectedSqlTypeNameForAuroraDbTypeDouble()
        {
            // Setup
            AuroraDbType dbType = AuroraDbType.Double;
            string expected = "DOUBLE";
            var resolver = new AuroraDbDbTypeToStringNameResolver();

            // Act
            var actual = resolver.Resolve(dbType);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Resolves supported values and undefined enum values.
        /// </summary>
        [TestMethod]
        public void ResolveReturnsExpectedSqlTypeNameForAuroraDbTypeBit()
        {
            // Setup
            AuroraDbType dbType = AuroraDbType.Bit;
            string expected = "BIT";
            var resolver = new AuroraDbDbTypeToStringNameResolver();

            // Act
            var actual = resolver.Resolve(dbType);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Resolves supported values and undefined enum values.
        /// </summary>
        [TestMethod]
        public void ResolveReturnsExpectedSqlTypeNameForAuroraDbTypeChar()
        {
            // Setup
            AuroraDbType dbType = AuroraDbType.Char;
            string expected = "CHAR";
            var resolver = new AuroraDbDbTypeToStringNameResolver();

            // Act
            var actual = resolver.Resolve(dbType);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Resolves supported values and undefined enum values.
        /// </summary>
        [TestMethod]
        public void ResolveReturnsExpectedSqlTypeNameForAuroraDbTypeVarChar()
        {
            // Setup
            AuroraDbType dbType = AuroraDbType.VarChar;
            string expected = "VARCHAR";
            var resolver = new AuroraDbDbTypeToStringNameResolver();

            // Act
            var actual = resolver.Resolve(dbType);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Resolves supported values and undefined enum values.
        /// </summary>
        [TestMethod]
        public void ResolveReturnsExpectedSqlTypeNameForAuroraDbTypeTinyText()
        {
            // Setup
            AuroraDbType dbType = AuroraDbType.TinyText;
            string expected = "TINYTEXT";
            var resolver = new AuroraDbDbTypeToStringNameResolver();

            // Act
            var actual = resolver.Resolve(dbType);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Resolves supported values and undefined enum values.
        /// </summary>
        [TestMethod]
        public void ResolveReturnsExpectedSqlTypeNameForAuroraDbTypeText()
        {
            // Setup
            AuroraDbType dbType = AuroraDbType.Text;
            string expected = "TEXT";
            var resolver = new AuroraDbDbTypeToStringNameResolver();

            // Act
            var actual = resolver.Resolve(dbType);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Resolves supported values and undefined enum values.
        /// </summary>
        [TestMethod]
        public void ResolveReturnsExpectedSqlTypeNameForAuroraDbTypeEnum()
        {
            // Setup
            AuroraDbType dbType = AuroraDbType.Enum;
            string expected = "TEXT";
            var resolver = new AuroraDbDbTypeToStringNameResolver();

            // Act
            var actual = resolver.Resolve(dbType);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Resolves supported values and undefined enum values.
        /// </summary>
        [TestMethod]
        public void ResolveReturnsExpectedSqlTypeNameForAuroraDbTypeSet()
        {
            // Setup
            AuroraDbType dbType = AuroraDbType.Set;
            string expected = "TEXT";
            var resolver = new AuroraDbDbTypeToStringNameResolver();

            // Act
            var actual = resolver.Resolve(dbType);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Resolves supported values and undefined enum values.
        /// </summary>
        [TestMethod]
        public void ResolveReturnsExpectedSqlTypeNameForAuroraDbTypeMediumText()
        {
            // Setup
            AuroraDbType dbType = AuroraDbType.MediumText;
            string expected = "MEDIUMTEXT";
            var resolver = new AuroraDbDbTypeToStringNameResolver();

            // Act
            var actual = resolver.Resolve(dbType);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Resolves supported values and undefined enum values.
        /// </summary>
        [TestMethod]
        public void ResolveReturnsExpectedSqlTypeNameForAuroraDbTypeLongText()
        {
            // Setup
            AuroraDbType dbType = AuroraDbType.LongText;
            string expected = "LONGTEXT";
            var resolver = new AuroraDbDbTypeToStringNameResolver();

            // Act
            var actual = resolver.Resolve(dbType);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Resolves supported values and undefined enum values.
        /// </summary>
        [TestMethod]
        public void ResolveReturnsExpectedSqlTypeNameForAuroraDbTypeBinary()
        {
            // Setup
            AuroraDbType dbType = AuroraDbType.Binary;
            string expected = "BINARY";
            var resolver = new AuroraDbDbTypeToStringNameResolver();

            // Act
            var actual = resolver.Resolve(dbType);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Resolves supported values and undefined enum values.
        /// </summary>
        [TestMethod]
        public void ResolveReturnsExpectedSqlTypeNameForAuroraDbTypeVarBinary()
        {
            // Setup
            AuroraDbType dbType = AuroraDbType.VarBinary;
            string expected = "VARBINARY";
            var resolver = new AuroraDbDbTypeToStringNameResolver();

            // Act
            var actual = resolver.Resolve(dbType);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Resolves supported values and undefined enum values.
        /// </summary>
        [TestMethod]
        public void ResolveReturnsExpectedSqlTypeNameForAuroraDbTypeTinyBlob()
        {
            // Setup
            AuroraDbType dbType = AuroraDbType.TinyBlob;
            string expected = "TINYBLOB";
            var resolver = new AuroraDbDbTypeToStringNameResolver();

            // Act
            var actual = resolver.Resolve(dbType);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Resolves supported values and undefined enum values.
        /// </summary>
        [TestMethod]
        public void ResolveReturnsExpectedSqlTypeNameForAuroraDbTypeBlob()
        {
            // Setup
            AuroraDbType dbType = AuroraDbType.Blob;
            string expected = "BLOB";
            var resolver = new AuroraDbDbTypeToStringNameResolver();

            // Act
            var actual = resolver.Resolve(dbType);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Resolves supported values and undefined enum values.
        /// </summary>
        [TestMethod]
        public void ResolveReturnsExpectedSqlTypeNameForAuroraDbTypeMediumBlob()
        {
            // Setup
            AuroraDbType dbType = AuroraDbType.MediumBlob;
            string expected = "MEDIUMBLOB";
            var resolver = new AuroraDbDbTypeToStringNameResolver();

            // Act
            var actual = resolver.Resolve(dbType);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Resolves supported values and undefined enum values.
        /// </summary>
        [TestMethod]
        public void ResolveReturnsExpectedSqlTypeNameForAuroraDbTypeLongBlob()
        {
            // Setup
            AuroraDbType dbType = AuroraDbType.LongBlob;
            string expected = "LONGBLOB";
            var resolver = new AuroraDbDbTypeToStringNameResolver();

            // Act
            var actual = resolver.Resolve(dbType);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Resolves supported values and undefined enum values.
        /// </summary>
        [TestMethod]
        public void ResolveReturnsExpectedSqlTypeNameForAuroraDbTypeDate()
        {
            // Setup
            AuroraDbType dbType = AuroraDbType.Date;
            string expected = "DATE";
            var resolver = new AuroraDbDbTypeToStringNameResolver();

            // Act
            var actual = resolver.Resolve(dbType);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Resolves supported values and undefined enum values.
        /// </summary>
        [TestMethod]
        public void ResolveReturnsExpectedSqlTypeNameForAuroraDbTypeTime()
        {
            // Setup
            AuroraDbType dbType = AuroraDbType.Time;
            string expected = "TIME";
            var resolver = new AuroraDbDbTypeToStringNameResolver();

            // Act
            var actual = resolver.Resolve(dbType);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Resolves supported values and undefined enum values.
        /// </summary>
        [TestMethod]
        public void ResolveReturnsExpectedSqlTypeNameForAuroraDbTypeDateTime()
        {
            // Setup
            AuroraDbType dbType = AuroraDbType.DateTime;
            string expected = "DATETIME";
            var resolver = new AuroraDbDbTypeToStringNameResolver();

            // Act
            var actual = resolver.Resolve(dbType);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Resolves supported values and undefined enum values.
        /// </summary>
        [TestMethod]
        public void ResolveReturnsExpectedSqlTypeNameForAuroraDbTypeTimestamp()
        {
            // Setup
            AuroraDbType dbType = AuroraDbType.Timestamp;
            string expected = "TIMESTAMP";
            var resolver = new AuroraDbDbTypeToStringNameResolver();

            // Act
            var actual = resolver.Resolve(dbType);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Resolves supported values and undefined enum values.
        /// </summary>
        [TestMethod]
        public void ResolveReturnsExpectedSqlTypeNameForAuroraDbTypeYear()
        {
            // Setup
            AuroraDbType dbType = AuroraDbType.Year;
            string expected = "YEAR";
            var resolver = new AuroraDbDbTypeToStringNameResolver();

            // Act
            var actual = resolver.Resolve(dbType);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Resolves supported values and undefined enum values.
        /// </summary>
        [TestMethod]
        public void ResolveReturnsExpectedSqlTypeNameForAuroraDbTypeJson()
        {
            // Setup
            AuroraDbType dbType = AuroraDbType.Json;
            string expected = "JSON";
            var resolver = new AuroraDbDbTypeToStringNameResolver();

            // Act
            var actual = resolver.Resolve(dbType);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Resolves supported values and undefined enum values.
        /// </summary>
        [TestMethod]
        public void ResolveReturnsExpectedSqlTypeNameForAuroraDbTypeGeometry()
        {
            // Setup
            AuroraDbType dbType = AuroraDbType.Geometry;
            string expected = "GEOMETRY";
            var resolver = new AuroraDbDbTypeToStringNameResolver();

            // Act
            var actual = resolver.Resolve(dbType);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Resolves supported values and undefined enum values.
        /// </summary>
        [TestMethod]
        public void ResolveReturnsExpectedSqlTypeNameForAuroraDbTypePoint()
        {
            // Setup
            AuroraDbType dbType = AuroraDbType.Point;
            string expected = "GEOMETRY";
            var resolver = new AuroraDbDbTypeToStringNameResolver();

            // Act
            var actual = resolver.Resolve(dbType);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Resolves supported values and undefined enum values.
        /// </summary>
        [TestMethod]
        public void ResolveReturnsExpectedSqlTypeNameForAuroraDbTypeLineString()
        {
            // Setup
            AuroraDbType dbType = AuroraDbType.LineString;
            string expected = "GEOMETRY";
            var resolver = new AuroraDbDbTypeToStringNameResolver();

            // Act
            var actual = resolver.Resolve(dbType);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Resolves supported values and undefined enum values.
        /// </summary>
        [TestMethod]
        public void ResolveReturnsExpectedSqlTypeNameForAuroraDbTypePolygon()
        {
            // Setup
            AuroraDbType dbType = AuroraDbType.Polygon;
            string expected = "GEOMETRY";
            var resolver = new AuroraDbDbTypeToStringNameResolver();

            // Act
            var actual = resolver.Resolve(dbType);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Resolves supported values and undefined enum values.
        /// </summary>
        [TestMethod]
        public void ResolveReturnsExpectedSqlTypeNameForAuroraDbTypeMultiPoint()
        {
            // Setup
            AuroraDbType dbType = AuroraDbType.MultiPoint;
            string expected = "GEOMETRY";
            var resolver = new AuroraDbDbTypeToStringNameResolver();

            // Act
            var actual = resolver.Resolve(dbType);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Resolves supported values and undefined enum values.
        /// </summary>
        [TestMethod]
        public void ResolveReturnsExpectedSqlTypeNameForAuroraDbTypeMultiLineString()
        {
            // Setup
            AuroraDbType dbType = AuroraDbType.MultiLineString;
            string expected = "GEOMETRY";
            var resolver = new AuroraDbDbTypeToStringNameResolver();

            // Act
            var actual = resolver.Resolve(dbType);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Resolves supported values and undefined enum values.
        /// </summary>
        [TestMethod]
        public void ResolveReturnsExpectedSqlTypeNameForAuroraDbTypeMultiPolygon()
        {
            // Setup
            AuroraDbType dbType = AuroraDbType.MultiPolygon;
            string expected = "GEOMETRY";
            var resolver = new AuroraDbDbTypeToStringNameResolver();

            // Act
            var actual = resolver.Resolve(dbType);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Resolves supported values and undefined enum values.
        /// </summary>
        [TestMethod]
        public void ResolveReturnsExpectedSqlTypeNameForAuroraDbTypeGeometryCollection()
        {
            // Setup
            AuroraDbType dbType = AuroraDbType.GeometryCollection;
            string expected = "GEOMETRY";
            var resolver = new AuroraDbDbTypeToStringNameResolver();

            // Act
            var actual = resolver.Resolve(dbType);

            // Assert
            Assert.AreEqual(expected,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Resolves supported values and undefined enum values.
        /// </summary>
        [TestMethod]
        public void ResolveReturnsExpectedSqlTypeNameForAuroraDbTypeNegativeOne()
        {
            // Setup
            AuroraDbType dbType = (AuroraDbType)(-1);
            string expected = "TEXT";
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