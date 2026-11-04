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
        [TestMethod]
        public void ResolveReturnsExpectedSqlTypeNameForMariaDbTypeTinyInt()
        {
            // Setup
            MariaDbType dbType = MariaDbType.TinyInt;
            string expected = "TINYINT";
            var resolver = new MariaDbDbTypeToStringNameResolver();

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
        public void ResolveReturnsExpectedSqlTypeNameForMariaDbTypeSmallInt()
        {
            // Setup
            MariaDbType dbType = MariaDbType.SmallInt;
            string expected = "SMALLINT";
            var resolver = new MariaDbDbTypeToStringNameResolver();

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
        public void ResolveReturnsExpectedSqlTypeNameForMariaDbTypeMediumInt()
        {
            // Setup
            MariaDbType dbType = MariaDbType.MediumInt;
            string expected = "MEDIUMINT";
            var resolver = new MariaDbDbTypeToStringNameResolver();

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
        public void ResolveReturnsExpectedSqlTypeNameForMariaDbTypeInt()
        {
            // Setup
            MariaDbType dbType = MariaDbType.Int;
            string expected = "INT";
            var resolver = new MariaDbDbTypeToStringNameResolver();

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
        public void ResolveReturnsExpectedSqlTypeNameForMariaDbTypeBigInt()
        {
            // Setup
            MariaDbType dbType = MariaDbType.BigInt;
            string expected = "BIGINT";
            var resolver = new MariaDbDbTypeToStringNameResolver();

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
        public void ResolveReturnsExpectedSqlTypeNameForMariaDbTypeDecimal()
        {
            // Setup
            MariaDbType dbType = MariaDbType.Decimal;
            string expected = "DECIMAL";
            var resolver = new MariaDbDbTypeToStringNameResolver();

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
        public void ResolveReturnsExpectedSqlTypeNameForMariaDbTypeFloat()
        {
            // Setup
            MariaDbType dbType = MariaDbType.Float;
            string expected = "FLOAT";
            var resolver = new MariaDbDbTypeToStringNameResolver();

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
        public void ResolveReturnsExpectedSqlTypeNameForMariaDbTypeDouble()
        {
            // Setup
            MariaDbType dbType = MariaDbType.Double;
            string expected = "DOUBLE";
            var resolver = new MariaDbDbTypeToStringNameResolver();

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
        public void ResolveReturnsExpectedSqlTypeNameForMariaDbTypeBit()
        {
            // Setup
            MariaDbType dbType = MariaDbType.Bit;
            string expected = "BIT";
            var resolver = new MariaDbDbTypeToStringNameResolver();

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
        public void ResolveReturnsExpectedSqlTypeNameForMariaDbTypeChar()
        {
            // Setup
            MariaDbType dbType = MariaDbType.Char;
            string expected = "CHAR";
            var resolver = new MariaDbDbTypeToStringNameResolver();

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
        public void ResolveReturnsExpectedSqlTypeNameForMariaDbTypeVarChar()
        {
            // Setup
            MariaDbType dbType = MariaDbType.VarChar;
            string expected = "VARCHAR";
            var resolver = new MariaDbDbTypeToStringNameResolver();

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
        public void ResolveReturnsExpectedSqlTypeNameForMariaDbTypeTinyText()
        {
            // Setup
            MariaDbType dbType = MariaDbType.TinyText;
            string expected = "TINYTEXT";
            var resolver = new MariaDbDbTypeToStringNameResolver();

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
        public void ResolveReturnsExpectedSqlTypeNameForMariaDbTypeText()
        {
            // Setup
            MariaDbType dbType = MariaDbType.Text;
            string expected = "TEXT";
            var resolver = new MariaDbDbTypeToStringNameResolver();

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
        public void ResolveReturnsExpectedSqlTypeNameForMariaDbTypeEnum()
        {
            // Setup
            MariaDbType dbType = MariaDbType.Enum;
            string expected = "TEXT";
            var resolver = new MariaDbDbTypeToStringNameResolver();

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
        public void ResolveReturnsExpectedSqlTypeNameForMariaDbTypeSet()
        {
            // Setup
            MariaDbType dbType = MariaDbType.Set;
            string expected = "TEXT";
            var resolver = new MariaDbDbTypeToStringNameResolver();

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
        public void ResolveReturnsExpectedSqlTypeNameForMariaDbTypeMediumText()
        {
            // Setup
            MariaDbType dbType = MariaDbType.MediumText;
            string expected = "MEDIUMTEXT";
            var resolver = new MariaDbDbTypeToStringNameResolver();

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
        public void ResolveReturnsExpectedSqlTypeNameForMariaDbTypeLongText()
        {
            // Setup
            MariaDbType dbType = MariaDbType.LongText;
            string expected = "LONGTEXT";
            var resolver = new MariaDbDbTypeToStringNameResolver();

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
        public void ResolveReturnsExpectedSqlTypeNameForMariaDbTypeBinary()
        {
            // Setup
            MariaDbType dbType = MariaDbType.Binary;
            string expected = "BINARY";
            var resolver = new MariaDbDbTypeToStringNameResolver();

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
        public void ResolveReturnsExpectedSqlTypeNameForMariaDbTypeVarBinary()
        {
            // Setup
            MariaDbType dbType = MariaDbType.VarBinary;
            string expected = "VARBINARY";
            var resolver = new MariaDbDbTypeToStringNameResolver();

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
        public void ResolveReturnsExpectedSqlTypeNameForMariaDbTypeTinyBlob()
        {
            // Setup
            MariaDbType dbType = MariaDbType.TinyBlob;
            string expected = "TINYBLOB";
            var resolver = new MariaDbDbTypeToStringNameResolver();

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
        public void ResolveReturnsExpectedSqlTypeNameForMariaDbTypeBlob()
        {
            // Setup
            MariaDbType dbType = MariaDbType.Blob;
            string expected = "BLOB";
            var resolver = new MariaDbDbTypeToStringNameResolver();

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
        public void ResolveReturnsExpectedSqlTypeNameForMariaDbTypeMediumBlob()
        {
            // Setup
            MariaDbType dbType = MariaDbType.MediumBlob;
            string expected = "MEDIUMBLOB";
            var resolver = new MariaDbDbTypeToStringNameResolver();

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
        public void ResolveReturnsExpectedSqlTypeNameForMariaDbTypeLongBlob()
        {
            // Setup
            MariaDbType dbType = MariaDbType.LongBlob;
            string expected = "LONGBLOB";
            var resolver = new MariaDbDbTypeToStringNameResolver();

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
        public void ResolveReturnsExpectedSqlTypeNameForMariaDbTypeDate()
        {
            // Setup
            MariaDbType dbType = MariaDbType.Date;
            string expected = "DATE";
            var resolver = new MariaDbDbTypeToStringNameResolver();

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
        public void ResolveReturnsExpectedSqlTypeNameForMariaDbTypeTime()
        {
            // Setup
            MariaDbType dbType = MariaDbType.Time;
            string expected = "TIME";
            var resolver = new MariaDbDbTypeToStringNameResolver();

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
        public void ResolveReturnsExpectedSqlTypeNameForMariaDbTypeDateTime()
        {
            // Setup
            MariaDbType dbType = MariaDbType.DateTime;
            string expected = "DATETIME";
            var resolver = new MariaDbDbTypeToStringNameResolver();

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
        public void ResolveReturnsExpectedSqlTypeNameForMariaDbTypeTimestamp()
        {
            // Setup
            MariaDbType dbType = MariaDbType.Timestamp;
            string expected = "TIMESTAMP";
            var resolver = new MariaDbDbTypeToStringNameResolver();

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
        public void ResolveReturnsExpectedSqlTypeNameForMariaDbTypeYear()
        {
            // Setup
            MariaDbType dbType = MariaDbType.Year;
            string expected = "YEAR";
            var resolver = new MariaDbDbTypeToStringNameResolver();

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
        public void ResolveReturnsExpectedSqlTypeNameForMariaDbTypeJson()
        {
            // Setup
            MariaDbType dbType = MariaDbType.Json;
            string expected = "JSON";
            var resolver = new MariaDbDbTypeToStringNameResolver();

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
        public void ResolveReturnsExpectedSqlTypeNameForMariaDbTypeGeometry()
        {
            // Setup
            MariaDbType dbType = MariaDbType.Geometry;
            string expected = "GEOMETRY";
            var resolver = new MariaDbDbTypeToStringNameResolver();

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
        public void ResolveReturnsExpectedSqlTypeNameForMariaDbTypePoint()
        {
            // Setup
            MariaDbType dbType = MariaDbType.Point;
            string expected = "GEOMETRY";
            var resolver = new MariaDbDbTypeToStringNameResolver();

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
        public void ResolveReturnsExpectedSqlTypeNameForMariaDbTypeLineString()
        {
            // Setup
            MariaDbType dbType = MariaDbType.LineString;
            string expected = "GEOMETRY";
            var resolver = new MariaDbDbTypeToStringNameResolver();

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
        public void ResolveReturnsExpectedSqlTypeNameForMariaDbTypePolygon()
        {
            // Setup
            MariaDbType dbType = MariaDbType.Polygon;
            string expected = "GEOMETRY";
            var resolver = new MariaDbDbTypeToStringNameResolver();

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
        public void ResolveReturnsExpectedSqlTypeNameForMariaDbTypeMultiPoint()
        {
            // Setup
            MariaDbType dbType = MariaDbType.MultiPoint;
            string expected = "GEOMETRY";
            var resolver = new MariaDbDbTypeToStringNameResolver();

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
        public void ResolveReturnsExpectedSqlTypeNameForMariaDbTypeMultiLineString()
        {
            // Setup
            MariaDbType dbType = MariaDbType.MultiLineString;
            string expected = "GEOMETRY";
            var resolver = new MariaDbDbTypeToStringNameResolver();

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
        public void ResolveReturnsExpectedSqlTypeNameForMariaDbTypeMultiPolygon()
        {
            // Setup
            MariaDbType dbType = MariaDbType.MultiPolygon;
            string expected = "GEOMETRY";
            var resolver = new MariaDbDbTypeToStringNameResolver();

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
        public void ResolveReturnsExpectedSqlTypeNameForMariaDbTypeGeometryCollection()
        {
            // Setup
            MariaDbType dbType = MariaDbType.GeometryCollection;
            string expected = "GEOMETRY";
            var resolver = new MariaDbDbTypeToStringNameResolver();

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
        public void ResolveReturnsExpectedSqlTypeNameForMariaDbTypeNegativeOne()
        {
            // Setup
            MariaDbType dbType = (MariaDbType)(-1);
            string expected = "TEXT";
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