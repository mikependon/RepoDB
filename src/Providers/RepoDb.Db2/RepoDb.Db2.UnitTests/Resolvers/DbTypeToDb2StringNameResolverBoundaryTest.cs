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
        [TestMethod]
        public void ResolveReturnsExpectedSqlTypeNameForDbTypeInt64()
        {
            // Setup
            DbType dbType = DbType.Int64;
            string expected = "BIGINT";
            var resolver = new DbTypeToDb2StringNameResolver();

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
            string expected = "BLOB(1M)";
            var resolver = new DbTypeToDb2StringNameResolver();

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
            string expected = "SMALLINT";
            var resolver = new DbTypeToDb2StringNameResolver();

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
            string expected = "VARCHAR(2000)";
            var resolver = new DbTypeToDb2StringNameResolver();

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
            var resolver = new DbTypeToDb2StringNameResolver();

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
            string expected = "TIMESTAMP";
            var resolver = new DbTypeToDb2StringNameResolver();

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
            var resolver = new DbTypeToDb2StringNameResolver();

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
            string expected = "TIMESTAMP";
            var resolver = new DbTypeToDb2StringNameResolver();

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
            string expected = "DECIMAL(31,15)";
            var resolver = new DbTypeToDb2StringNameResolver();

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
            string expected = "REAL";
            var resolver = new DbTypeToDb2StringNameResolver();

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
            string expected = "DOUBLE";
            var resolver = new DbTypeToDb2StringNameResolver();

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
            string expected = "INTEGER";
            var resolver = new DbTypeToDb2StringNameResolver();

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
            string expected = "SMALLINT";
            var resolver = new DbTypeToDb2StringNameResolver();

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
            string expected = "TIME";
            var resolver = new DbTypeToDb2StringNameResolver();

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
            string expected = "SMALLINT";
            var resolver = new DbTypeToDb2StringNameResolver();

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
            string expected = "CHAR(16) FOR BIT DATA";
            var resolver = new DbTypeToDb2StringNameResolver();

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
            string expected = "VARCHAR(2000)";
            var resolver = new DbTypeToDb2StringNameResolver();

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
            string expected = "CHAR(254)";
            var resolver = new DbTypeToDb2StringNameResolver();

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
            string expected = "GRAPHIC(127)";
            var resolver = new DbTypeToDb2StringNameResolver();

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
            string expected = "BLOB(1M)";
            var resolver = new DbTypeToDb2StringNameResolver();

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
            string expected = "XML";
            var resolver = new DbTypeToDb2StringNameResolver();

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
            string expected = "VARCHAR(2000)";
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