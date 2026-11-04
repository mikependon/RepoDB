using Microsoft.VisualStudio.TestTools.UnitTesting;
using MySql.Data.MySqlClient;
using RepoDb.Resolvers;
using System.Data;
using System;

namespace RepoDb.MySql.UnitTests.Resolvers
{
    /// <summary>
    /// Covers supported mappings and resolver boundaries.
    /// </summary>
    [TestClass]
    public class MySqlDbTypeToMySqlStringNameResolverBoundaryTest
    {
        #region Public Methods

        /// <summary>
        /// Resolves supported values and undefined enum values.
        /// </summary>
        [TestMethod]
        public void ResolveReturnsExpectedSqlTypeNameForMySqlDbTypeBinary()
        {
            // Setup
            MySqlDbType dbType = MySqlDbType.Binary;
            string expected = "BINARY";
            var resolver = new MySqlDbTypeToMySqlStringNameResolver();

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
        public void ResolveReturnsExpectedSqlTypeNameForMySqlDbTypeBit()
        {
            // Setup
            MySqlDbType dbType = MySqlDbType.Bit;
            string expected = "BIT";
            var resolver = new MySqlDbTypeToMySqlStringNameResolver();

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
        public void ResolveReturnsExpectedSqlTypeNameForMySqlDbTypeBlob()
        {
            // Setup
            MySqlDbType dbType = MySqlDbType.Blob;
            string expected = "BLOB";
            var resolver = new MySqlDbTypeToMySqlStringNameResolver();

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
        public void ResolveReturnsExpectedSqlTypeNameForMySqlDbTypeByte()
        {
            // Setup
            MySqlDbType dbType = MySqlDbType.Byte;
            string expected = "TINYINT";
            var resolver = new MySqlDbTypeToMySqlStringNameResolver();

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
        public void ResolveReturnsExpectedSqlTypeNameForMySqlDbTypeUByte()
        {
            // Setup
            MySqlDbType dbType = MySqlDbType.UByte;
            string expected = "TINYINT";
            var resolver = new MySqlDbTypeToMySqlStringNameResolver();

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
        public void ResolveReturnsExpectedSqlTypeNameForMySqlDbTypeDate()
        {
            // Setup
            MySqlDbType dbType = MySqlDbType.Date;
            string expected = "DATE";
            var resolver = new MySqlDbTypeToMySqlStringNameResolver();

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
        public void ResolveReturnsExpectedSqlTypeNameForMySqlDbTypeDateTime()
        {
            // Setup
            MySqlDbType dbType = MySqlDbType.DateTime;
            string expected = "DATETIME";
            var resolver = new MySqlDbTypeToMySqlStringNameResolver();

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
        public void ResolveReturnsExpectedSqlTypeNameForMySqlDbTypeDecimal()
        {
            // Setup
            MySqlDbType dbType = MySqlDbType.Decimal;
            string expected = "DECIMAL";
            var resolver = new MySqlDbTypeToMySqlStringNameResolver();

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
        public void ResolveReturnsExpectedSqlTypeNameForMySqlDbTypeDouble()
        {
            // Setup
            MySqlDbType dbType = MySqlDbType.Double;
            string expected = "DOUBLE";
            var resolver = new MySqlDbTypeToMySqlStringNameResolver();

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
        public void ResolveReturnsExpectedSqlTypeNameForMySqlDbTypeEnum()
        {
            // Setup
            MySqlDbType dbType = MySqlDbType.Enum;
            string expected = "TEXT";
            var resolver = new MySqlDbTypeToMySqlStringNameResolver();

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
        public void ResolveReturnsExpectedSqlTypeNameForMySqlDbTypeGuid()
        {
            // Setup
            MySqlDbType dbType = MySqlDbType.Guid;
            string expected = "TEXT";
            var resolver = new MySqlDbTypeToMySqlStringNameResolver();

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
        public void ResolveReturnsExpectedSqlTypeNameForMySqlDbTypeSet()
        {
            // Setup
            MySqlDbType dbType = MySqlDbType.Set;
            string expected = "TEXT";
            var resolver = new MySqlDbTypeToMySqlStringNameResolver();

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
        public void ResolveReturnsExpectedSqlTypeNameForMySqlDbTypeText()
        {
            // Setup
            MySqlDbType dbType = MySqlDbType.Text;
            string expected = "TEXT";
            var resolver = new MySqlDbTypeToMySqlStringNameResolver();

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
        public void ResolveReturnsExpectedSqlTypeNameForMySqlDbTypeFloat()
        {
            // Setup
            MySqlDbType dbType = MySqlDbType.Float;
            string expected = "FLOAT";
            var resolver = new MySqlDbTypeToMySqlStringNameResolver();

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
        public void ResolveReturnsExpectedSqlTypeNameForMySqlDbTypeGeometry()
        {
            // Setup
            MySqlDbType dbType = MySqlDbType.Geometry;
            string expected = "GEOMETRY";
            var resolver = new MySqlDbTypeToMySqlStringNameResolver();

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
        public void ResolveReturnsExpectedSqlTypeNameForMySqlDbTypeInt16()
        {
            // Setup
            MySqlDbType dbType = MySqlDbType.Int16;
            string expected = "SMALLINT";
            var resolver = new MySqlDbTypeToMySqlStringNameResolver();

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
        public void ResolveReturnsExpectedSqlTypeNameForMySqlDbTypeInt24()
        {
            // Setup
            MySqlDbType dbType = MySqlDbType.Int24;
            string expected = "SMALLINT";
            var resolver = new MySqlDbTypeToMySqlStringNameResolver();

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
        public void ResolveReturnsExpectedSqlTypeNameForMySqlDbTypeUInt24()
        {
            // Setup
            MySqlDbType dbType = MySqlDbType.UInt24;
            string expected = "SMALLINT";
            var resolver = new MySqlDbTypeToMySqlStringNameResolver();

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
        public void ResolveReturnsExpectedSqlTypeNameForMySqlDbTypeUInt16()
        {
            // Setup
            MySqlDbType dbType = MySqlDbType.UInt16;
            string expected = "SMALLINT";
            var resolver = new MySqlDbTypeToMySqlStringNameResolver();

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
        public void ResolveReturnsExpectedSqlTypeNameForMySqlDbTypeInt32()
        {
            // Setup
            MySqlDbType dbType = MySqlDbType.Int32;
            string expected = "INT";
            var resolver = new MySqlDbTypeToMySqlStringNameResolver();

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
        public void ResolveReturnsExpectedSqlTypeNameForMySqlDbTypeUInt32()
        {
            // Setup
            MySqlDbType dbType = MySqlDbType.UInt32;
            string expected = "INT";
            var resolver = new MySqlDbTypeToMySqlStringNameResolver();

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
        public void ResolveReturnsExpectedSqlTypeNameForMySqlDbTypeInt64()
        {
            // Setup
            MySqlDbType dbType = MySqlDbType.Int64;
            string expected = "BIGINT";
            var resolver = new MySqlDbTypeToMySqlStringNameResolver();

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
        public void ResolveReturnsExpectedSqlTypeNameForMySqlDbTypeUInt64()
        {
            // Setup
            MySqlDbType dbType = MySqlDbType.UInt64;
            string expected = "BIGINT";
            var resolver = new MySqlDbTypeToMySqlStringNameResolver();

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
        public void ResolveReturnsExpectedSqlTypeNameForMySqlDbTypeJSON()
        {
            // Setup
            MySqlDbType dbType = MySqlDbType.JSON;
            string expected = "JSON";
            var resolver = new MySqlDbTypeToMySqlStringNameResolver();

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
        public void ResolveReturnsExpectedSqlTypeNameForMySqlDbTypeLongBlob()
        {
            // Setup
            MySqlDbType dbType = MySqlDbType.LongBlob;
            string expected = "LONGBLOB";
            var resolver = new MySqlDbTypeToMySqlStringNameResolver();

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
        public void ResolveReturnsExpectedSqlTypeNameForMySqlDbTypeLongText()
        {
            // Setup
            MySqlDbType dbType = MySqlDbType.LongText;
            string expected = "LONGTEXT";
            var resolver = new MySqlDbTypeToMySqlStringNameResolver();

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
        public void ResolveReturnsExpectedSqlTypeNameForMySqlDbTypeMediumBlob()
        {
            // Setup
            MySqlDbType dbType = MySqlDbType.MediumBlob;
            string expected = "MEDIUMBLOB";
            var resolver = new MySqlDbTypeToMySqlStringNameResolver();

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
        public void ResolveReturnsExpectedSqlTypeNameForMySqlDbTypeMediumText()
        {
            // Setup
            MySqlDbType dbType = MySqlDbType.MediumText;
            string expected = "MEDIUMTEXT";
            var resolver = new MySqlDbTypeToMySqlStringNameResolver();

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
        public void ResolveReturnsExpectedSqlTypeNameForMySqlDbTypeNewdate()
        {
            // Setup
            MySqlDbType dbType = MySqlDbType.Newdate;
            string expected = "DATE";
            var resolver = new MySqlDbTypeToMySqlStringNameResolver();

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
        public void ResolveReturnsExpectedSqlTypeNameForMySqlDbTypeNewDecimal()
        {
            // Setup
            MySqlDbType dbType = MySqlDbType.NewDecimal;
            string expected = "DECIMAL";
            var resolver = new MySqlDbTypeToMySqlStringNameResolver();

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
        public void ResolveReturnsExpectedSqlTypeNameForMySqlDbTypeString()
        {
            // Setup
            MySqlDbType dbType = MySqlDbType.String;
            string expected = "STRING";
            var resolver = new MySqlDbTypeToMySqlStringNameResolver();

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
        public void ResolveReturnsExpectedSqlTypeNameForMySqlDbTypeTime()
        {
            // Setup
            MySqlDbType dbType = MySqlDbType.Time;
            string expected = "TIME";
            var resolver = new MySqlDbTypeToMySqlStringNameResolver();

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
        public void ResolveReturnsExpectedSqlTypeNameForMySqlDbTypeTimestamp()
        {
            // Setup
            MySqlDbType dbType = MySqlDbType.Timestamp;
            string expected = "TIMESTAMP";
            var resolver = new MySqlDbTypeToMySqlStringNameResolver();

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
        public void ResolveReturnsExpectedSqlTypeNameForMySqlDbTypeTinyBlob()
        {
            // Setup
            MySqlDbType dbType = MySqlDbType.TinyBlob;
            string expected = "TINYBLOB";
            var resolver = new MySqlDbTypeToMySqlStringNameResolver();

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
        public void ResolveReturnsExpectedSqlTypeNameForMySqlDbTypeTinyText()
        {
            // Setup
            MySqlDbType dbType = MySqlDbType.TinyText;
            string expected = "TINYTEXT";
            var resolver = new MySqlDbTypeToMySqlStringNameResolver();

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
        public void ResolveReturnsExpectedSqlTypeNameForMySqlDbTypeVarBinary()
        {
            // Setup
            MySqlDbType dbType = MySqlDbType.VarBinary;
            string expected = "VARBINARY";
            var resolver = new MySqlDbTypeToMySqlStringNameResolver();

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
        public void ResolveReturnsExpectedSqlTypeNameForMySqlDbTypeVarChar()
        {
            // Setup
            MySqlDbType dbType = MySqlDbType.VarChar;
            string expected = "VARCHAR";
            var resolver = new MySqlDbTypeToMySqlStringNameResolver();

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
        public void ResolveReturnsExpectedSqlTypeNameForMySqlDbTypeVarString()
        {
            // Setup
            MySqlDbType dbType = MySqlDbType.VarString;
            string expected = "VARCHAR";
            var resolver = new MySqlDbTypeToMySqlStringNameResolver();

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
        public void ResolveReturnsExpectedSqlTypeNameForMySqlDbTypeYear()
        {
            // Setup
            MySqlDbType dbType = MySqlDbType.Year;
            string expected = "YEAR";
            var resolver = new MySqlDbTypeToMySqlStringNameResolver();

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
        public void ResolveReturnsExpectedSqlTypeNameForMySqlDbTypeNegativeOne()
        {
            // Setup
            MySqlDbType dbType = (MySqlDbType)(-1);
            string expected = "TEXT";
            var resolver = new MySqlDbTypeToMySqlStringNameResolver();

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