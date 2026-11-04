using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Resolvers;
using System.Data;
using System;

namespace RepoDb.Oracle.UnitTests.Resolvers
{
    /// <summary>
    /// Covers supported mappings and resolver boundaries.
    /// </summary>
    [TestClass]
    public class DbTypeToOracleStringNameResolverBoundaryTest
    {
        #region Public Methods

        /// <summary>
        /// Resolves supported values and undefined enum values.
        /// </summary>
        [TestMethod]
        public void ResolveReturnsExpectedSqlTypeNameForDbTypeInt64()
        {
            // Setup
            DbType dbType = DbType.Int64;
            string expected = "NUMBER(19)";
            var resolver = new DbTypeToOracleStringNameResolver();

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
        public void ResolveReturnsExpectedSqlTypeNameForDbTypeBinary()
        {
            // Setup
            DbType dbType = DbType.Binary;
            string expected = "BLOB";
            var resolver = new DbTypeToOracleStringNameResolver();

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
        public void ResolveReturnsExpectedSqlTypeNameForDbTypeBoolean()
        {
            // Setup
            DbType dbType = DbType.Boolean;
            string expected = "NUMBER(1)";
            var resolver = new DbTypeToOracleStringNameResolver();

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
        public void ResolveReturnsExpectedSqlTypeNameForDbTypeString()
        {
            // Setup
            DbType dbType = DbType.String;
            string expected = "NVARCHAR2(2000)";
            var resolver = new DbTypeToOracleStringNameResolver();

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
        public void ResolveReturnsExpectedSqlTypeNameForDbTypeDate()
        {
            // Setup
            DbType dbType = DbType.Date;
            string expected = "DATE";
            var resolver = new DbTypeToOracleStringNameResolver();

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
        public void ResolveReturnsExpectedSqlTypeNameForDbTypeDateTime()
        {
            // Setup
            DbType dbType = DbType.DateTime;
            string expected = "DATE";
            var resolver = new DbTypeToOracleStringNameResolver();

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
        public void ResolveReturnsExpectedSqlTypeNameForDbTypeDateTime2()
        {
            // Setup
            DbType dbType = DbType.DateTime2;
            string expected = "TIMESTAMP";
            var resolver = new DbTypeToOracleStringNameResolver();

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
        public void ResolveReturnsExpectedSqlTypeNameForDbTypeDateTimeOffset()
        {
            // Setup
            DbType dbType = DbType.DateTimeOffset;
            string expected = "TIMESTAMP WITH TIME ZONE";
            var resolver = new DbTypeToOracleStringNameResolver();

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
        public void ResolveReturnsExpectedSqlTypeNameForDbTypeDecimal()
        {
            // Setup
            DbType dbType = DbType.Decimal;
            string expected = "NUMBER(18,2)";
            var resolver = new DbTypeToOracleStringNameResolver();

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
        public void ResolveReturnsExpectedSqlTypeNameForDbTypeSingle()
        {
            // Setup
            DbType dbType = DbType.Single;
            string expected = "BINARY_FLOAT";
            var resolver = new DbTypeToOracleStringNameResolver();

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
        public void ResolveReturnsExpectedSqlTypeNameForDbTypeDouble()
        {
            // Setup
            DbType dbType = DbType.Double;
            string expected = "BINARY_DOUBLE";
            var resolver = new DbTypeToOracleStringNameResolver();

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
        public void ResolveReturnsExpectedSqlTypeNameForDbTypeInt32()
        {
            // Setup
            DbType dbType = DbType.Int32;
            string expected = "NUMBER(10)";
            var resolver = new DbTypeToOracleStringNameResolver();

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
        public void ResolveReturnsExpectedSqlTypeNameForDbTypeInt16()
        {
            // Setup
            DbType dbType = DbType.Int16;
            string expected = "NUMBER(5)";
            var resolver = new DbTypeToOracleStringNameResolver();

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
        public void ResolveReturnsExpectedSqlTypeNameForDbTypeTime()
        {
            // Setup
            DbType dbType = DbType.Time;
            string expected = "INTERVAL DAY(0) TO SECOND(7)";
            var resolver = new DbTypeToOracleStringNameResolver();

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
        public void ResolveReturnsExpectedSqlTypeNameForDbTypeByte()
        {
            // Setup
            DbType dbType = DbType.Byte;
            string expected = "NUMBER(3)";
            var resolver = new DbTypeToOracleStringNameResolver();

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
        public void ResolveReturnsExpectedSqlTypeNameForDbTypeGuid()
        {
            // Setup
            DbType dbType = DbType.Guid;
            string expected = "RAW(16)";
            var resolver = new DbTypeToOracleStringNameResolver();

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
        public void ResolveReturnsExpectedSqlTypeNameForDbTypeAnsiString()
        {
            // Setup
            DbType dbType = DbType.AnsiString;
            string expected = "VARCHAR2(2000)";
            var resolver = new DbTypeToOracleStringNameResolver();

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
        public void ResolveReturnsExpectedSqlTypeNameForDbTypeAnsiStringFixedLength()
        {
            // Setup
            DbType dbType = DbType.AnsiStringFixedLength;
            string expected = "CHAR(2000)";
            var resolver = new DbTypeToOracleStringNameResolver();

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
        public void ResolveReturnsExpectedSqlTypeNameForDbTypeStringFixedLength()
        {
            // Setup
            DbType dbType = DbType.StringFixedLength;
            string expected = "NCHAR(2000)";
            var resolver = new DbTypeToOracleStringNameResolver();

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
        public void ResolveReturnsExpectedSqlTypeNameForDbTypeObject()
        {
            // Setup
            DbType dbType = DbType.Object;
            string expected = "BLOB";
            var resolver = new DbTypeToOracleStringNameResolver();

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
        public void ResolveReturnsExpectedSqlTypeNameForDbTypeXml()
        {
            // Setup
            DbType dbType = DbType.Xml;
            string expected = "XMLTYPE";
            var resolver = new DbTypeToOracleStringNameResolver();

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
        public void ResolveReturnsExpectedSqlTypeNameForDbTypeNegativeOne()
        {
            // Setup
            DbType dbType = (DbType)(-1);
            string expected = "NVARCHAR2(2000)";
            var resolver = new DbTypeToOracleStringNameResolver();

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