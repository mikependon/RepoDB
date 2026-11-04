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
        [TestMethod]
        public void ResolveReturnsExpectedSqlTypeNameForDbTypeInt64()
        {
            // Setup
            DbType dbType = DbType.Int64;
            string expected = "BIGINT";
            var resolver = new DbTypeToSqlServerStringNameResolver();

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
            string expected = "BINARY";
            var resolver = new DbTypeToSqlServerStringNameResolver();

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
            string expected = "BIT";
            var resolver = new DbTypeToSqlServerStringNameResolver();

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
            string expected = "NVARCHAR";
            var resolver = new DbTypeToSqlServerStringNameResolver();

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
            var resolver = new DbTypeToSqlServerStringNameResolver();

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
            string expected = "DATETIME";
            var resolver = new DbTypeToSqlServerStringNameResolver();

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
            string expected = "DATETIME2";
            var resolver = new DbTypeToSqlServerStringNameResolver();

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
            string expected = "DATETIMEOFFSET";
            var resolver = new DbTypeToSqlServerStringNameResolver();

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
            string expected = "DECIMAL(18,2)";
            var resolver = new DbTypeToSqlServerStringNameResolver();

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
            var resolver = new DbTypeToSqlServerStringNameResolver();

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
            string expected = "FLOAT";
            var resolver = new DbTypeToSqlServerStringNameResolver();

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
            string expected = "INT";
            var resolver = new DbTypeToSqlServerStringNameResolver();

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
            var resolver = new DbTypeToSqlServerStringNameResolver();

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
            var resolver = new DbTypeToSqlServerStringNameResolver();

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
            string expected = "TINYINT";
            var resolver = new DbTypeToSqlServerStringNameResolver();

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
            string expected = "UNIQUEIDENTIFIER";
            var resolver = new DbTypeToSqlServerStringNameResolver();

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
            string expected = "VARCHAR";
            var resolver = new DbTypeToSqlServerStringNameResolver();

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
            string expected = "CHAR";
            var resolver = new DbTypeToSqlServerStringNameResolver();

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
            string expected = "NCHAR";
            var resolver = new DbTypeToSqlServerStringNameResolver();

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
            string expected = "OBJECT";
            var resolver = new DbTypeToSqlServerStringNameResolver();

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
            var resolver = new DbTypeToSqlServerStringNameResolver();

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
            string expected = "NVARCHAR";
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