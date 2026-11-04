using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Resolvers;
using System.Data;
using System;

namespace RepoDb.EnterpriseDb.UnitTests.Resolvers
{
    /// <summary>
    /// Covers supported mappings and resolver boundaries.
    /// </summary>
    [TestClass]
    public class DbTypeToEnterpriseDbStringNameResolverBoundaryTest
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
            var resolver = new DbTypeToEnterpriseDbStringNameResolver();

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
            string expected = "BYTEA";
            var resolver = new DbTypeToEnterpriseDbStringNameResolver();

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
            string expected = "BYTEA";
            var resolver = new DbTypeToEnterpriseDbStringNameResolver();

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
            string expected = "BOOLEAN";
            var resolver = new DbTypeToEnterpriseDbStringNameResolver();

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
            string expected = "TEXT";
            var resolver = new DbTypeToEnterpriseDbStringNameResolver();

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
            string expected = "TEXT";
            var resolver = new DbTypeToEnterpriseDbStringNameResolver();

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
            string expected = "TEXT";
            var resolver = new DbTypeToEnterpriseDbStringNameResolver();

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
            string expected = "TEXT";
            var resolver = new DbTypeToEnterpriseDbStringNameResolver();

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
            var resolver = new DbTypeToEnterpriseDbStringNameResolver();

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
            var resolver = new DbTypeToEnterpriseDbStringNameResolver();

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
            var resolver = new DbTypeToEnterpriseDbStringNameResolver();

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
            string expected = "TIMESTAMPTZ";
            var resolver = new DbTypeToEnterpriseDbStringNameResolver();

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
            string expected = "NUMERIC";
            var resolver = new DbTypeToEnterpriseDbStringNameResolver();

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
            var resolver = new DbTypeToEnterpriseDbStringNameResolver();

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
            string expected = "DOUBLE PRECISION";
            var resolver = new DbTypeToEnterpriseDbStringNameResolver();

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
            var resolver = new DbTypeToEnterpriseDbStringNameResolver();

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
            var resolver = new DbTypeToEnterpriseDbStringNameResolver();

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
            string expected = "INTERVAL";
            var resolver = new DbTypeToEnterpriseDbStringNameResolver();

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
            string expected = "TEXT";
            var resolver = new DbTypeToEnterpriseDbStringNameResolver();

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