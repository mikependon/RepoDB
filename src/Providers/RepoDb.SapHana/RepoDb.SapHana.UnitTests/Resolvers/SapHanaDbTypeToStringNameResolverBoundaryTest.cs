using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Resolvers;
using Sap.Data.Hana;
using System.Data;
using System;

namespace RepoDb.SapHana.UnitTests.Resolvers
{
    /// <summary>
    /// Covers supported mappings and resolver boundaries.
    /// </summary>
    [TestClass]
    public class SapHanaDbTypeToStringNameResolverBoundaryTest
    {
        #region Public Methods

        /// <summary>
        /// Resolves supported values and undefined enum values.
        /// </summary>
        [TestMethod]
        public void ResolveReturnsExpectedSqlTypeNameForHanaDbTypeTinyInt()
        {
            // Setup
            HanaDbType dbType = HanaDbType.TinyInt;
            string expected = "TINYINT";
            var resolver = new SapHanaDbTypeToStringNameResolver();

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
        public void ResolveReturnsExpectedSqlTypeNameForHanaDbTypeSmallInt()
        {
            // Setup
            HanaDbType dbType = HanaDbType.SmallInt;
            string expected = "SMALLINT";
            var resolver = new SapHanaDbTypeToStringNameResolver();

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
        public void ResolveReturnsExpectedSqlTypeNameForHanaDbTypeInteger()
        {
            // Setup
            HanaDbType dbType = HanaDbType.Integer;
            string expected = "INTEGER";
            var resolver = new SapHanaDbTypeToStringNameResolver();

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
        public void ResolveReturnsExpectedSqlTypeNameForHanaDbTypeBigInt()
        {
            // Setup
            HanaDbType dbType = HanaDbType.BigInt;
            string expected = "BIGINT";
            var resolver = new SapHanaDbTypeToStringNameResolver();

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
        public void ResolveReturnsExpectedSqlTypeNameForHanaDbTypeDecimal()
        {
            // Setup
            HanaDbType dbType = HanaDbType.Decimal;
            string expected = "DECIMAL";
            var resolver = new SapHanaDbTypeToStringNameResolver();

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
        public void ResolveReturnsExpectedSqlTypeNameForHanaDbTypeSmallDecimal()
        {
            // Setup
            HanaDbType dbType = HanaDbType.SmallDecimal;
            string expected = "SMALLDECIMAL";
            var resolver = new SapHanaDbTypeToStringNameResolver();

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
        public void ResolveReturnsExpectedSqlTypeNameForHanaDbTypeReal()
        {
            // Setup
            HanaDbType dbType = HanaDbType.Real;
            string expected = "REAL";
            var resolver = new SapHanaDbTypeToStringNameResolver();

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
        public void ResolveReturnsExpectedSqlTypeNameForHanaDbTypeDouble()
        {
            // Setup
            HanaDbType dbType = HanaDbType.Double;
            string expected = "DOUBLE";
            var resolver = new SapHanaDbTypeToStringNameResolver();

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
        public void ResolveReturnsExpectedSqlTypeNameForHanaDbTypeBoolean()
        {
            // Setup
            HanaDbType dbType = HanaDbType.Boolean;
            string expected = "BOOLEAN";
            var resolver = new SapHanaDbTypeToStringNameResolver();

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
        public void ResolveReturnsExpectedSqlTypeNameForHanaDbTypeVarChar()
        {
            // Setup
            HanaDbType dbType = HanaDbType.VarChar;
            string expected = "VARCHAR";
            var resolver = new SapHanaDbTypeToStringNameResolver();

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
        public void ResolveReturnsExpectedSqlTypeNameForHanaDbTypeNVarChar()
        {
            // Setup
            HanaDbType dbType = HanaDbType.NVarChar;
            string expected = "NVARCHAR";
            var resolver = new SapHanaDbTypeToStringNameResolver();

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
        public void ResolveReturnsExpectedSqlTypeNameForHanaDbTypeText()
        {
            // Setup
            HanaDbType dbType = HanaDbType.Text;
            string expected = "TEXT";
            var resolver = new SapHanaDbTypeToStringNameResolver();

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
        public void ResolveReturnsExpectedSqlTypeNameForHanaDbTypeClob()
        {
            // Setup
            HanaDbType dbType = HanaDbType.Clob;
            string expected = "CLOB";
            var resolver = new SapHanaDbTypeToStringNameResolver();

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
        public void ResolveReturnsExpectedSqlTypeNameForHanaDbTypeNClob()
        {
            // Setup
            HanaDbType dbType = HanaDbType.NClob;
            string expected = "NCLOB";
            var resolver = new SapHanaDbTypeToStringNameResolver();

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
        public void ResolveReturnsExpectedSqlTypeNameForHanaDbTypeDate()
        {
            // Setup
            HanaDbType dbType = HanaDbType.Date;
            string expected = "DATE";
            var resolver = new SapHanaDbTypeToStringNameResolver();

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
        public void ResolveReturnsExpectedSqlTypeNameForHanaDbTypeTime()
        {
            // Setup
            HanaDbType dbType = HanaDbType.Time;
            string expected = "TIME";
            var resolver = new SapHanaDbTypeToStringNameResolver();

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
        public void ResolveReturnsExpectedSqlTypeNameForHanaDbTypeTimeStamp()
        {
            // Setup
            HanaDbType dbType = HanaDbType.TimeStamp;
            string expected = "TIMESTAMP";
            var resolver = new SapHanaDbTypeToStringNameResolver();

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
        public void ResolveReturnsExpectedSqlTypeNameForHanaDbTypeSecondDate()
        {
            // Setup
            HanaDbType dbType = HanaDbType.SecondDate;
            string expected = "SECONDDATE";
            var resolver = new SapHanaDbTypeToStringNameResolver();

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
        public void ResolveReturnsExpectedSqlTypeNameForHanaDbTypeBlob()
        {
            // Setup
            HanaDbType dbType = HanaDbType.Blob;
            string expected = "BLOB";
            var resolver = new SapHanaDbTypeToStringNameResolver();

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
        public void ResolveReturnsExpectedSqlTypeNameForHanaDbTypeVarBinary()
        {
            // Setup
            HanaDbType dbType = HanaDbType.VarBinary;
            string expected = "VARBINARY";
            var resolver = new SapHanaDbTypeToStringNameResolver();

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
        public void ResolveReturnsExpectedSqlTypeNameForHanaDbTypeNegativeOne()
        {
            // Setup
            HanaDbType dbType = (HanaDbType)(-1);
            string expected = "TEXT";
            var resolver = new SapHanaDbTypeToStringNameResolver();

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