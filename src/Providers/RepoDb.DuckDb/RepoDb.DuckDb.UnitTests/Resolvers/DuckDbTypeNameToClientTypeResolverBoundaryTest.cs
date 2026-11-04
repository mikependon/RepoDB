using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Resolvers;
using System.Numerics;
using System;

namespace RepoDb.DuckDb.UnitTests.Resolvers
{
    /// <summary>
    /// Covers supported mappings and resolver boundaries.
    /// </summary>
    [TestClass]
    public class DuckDbTypeNameToClientTypeResolverBoundaryTest
    {
        #region Public Methods

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForBoolean()
        {
            // Setup
            string name = "boolean";
            Type expected = typeof(bool);
            var resolver = new DuckDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForBooleanInUppercase()
        {
            // Setup
            string name = "BOOLEAN";
            Type expected = typeof(bool);
            var resolver = new DuckDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForBool()
        {
            // Setup
            string name = "bool";
            Type expected = typeof(bool);
            var resolver = new DuckDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForBoolInUppercase()
        {
            // Setup
            string name = "BOOL";
            Type expected = typeof(bool);
            var resolver = new DuckDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForLogical()
        {
            // Setup
            string name = "logical";
            Type expected = typeof(bool);
            var resolver = new DuckDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForLogicalInUppercase()
        {
            // Setup
            string name = "LOGICAL";
            Type expected = typeof(bool);
            var resolver = new DuckDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForTinyint()
        {
            // Setup
            string name = "tinyint";
            Type expected = typeof(sbyte);
            var resolver = new DuckDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForTinyintInUppercase()
        {
            // Setup
            string name = "TINYINT";
            Type expected = typeof(sbyte);
            var resolver = new DuckDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForInt1()
        {
            // Setup
            string name = "int1";
            Type expected = typeof(sbyte);
            var resolver = new DuckDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForInt1InUppercase()
        {
            // Setup
            string name = "INT1";
            Type expected = typeof(sbyte);
            var resolver = new DuckDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForSmallint()
        {
            // Setup
            string name = "smallint";
            Type expected = typeof(short);
            var resolver = new DuckDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForSmallintInUppercase()
        {
            // Setup
            string name = "SMALLINT";
            Type expected = typeof(short);
            var resolver = new DuckDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForInt2()
        {
            // Setup
            string name = "int2";
            Type expected = typeof(short);
            var resolver = new DuckDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForInt2InUppercase()
        {
            // Setup
            string name = "INT2";
            Type expected = typeof(short);
            var resolver = new DuckDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForShort()
        {
            // Setup
            string name = "short";
            Type expected = typeof(short);
            var resolver = new DuckDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForShortInUppercase()
        {
            // Setup
            string name = "SHORT";
            Type expected = typeof(short);
            var resolver = new DuckDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForInteger()
        {
            // Setup
            string name = "integer";
            Type expected = typeof(int);
            var resolver = new DuckDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForIntegerInUppercase()
        {
            // Setup
            string name = "INTEGER";
            Type expected = typeof(int);
            var resolver = new DuckDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForInt4()
        {
            // Setup
            string name = "int4";
            Type expected = typeof(int);
            var resolver = new DuckDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForInt4InUppercase()
        {
            // Setup
            string name = "INT4";
            Type expected = typeof(int);
            var resolver = new DuckDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForInt()
        {
            // Setup
            string name = "int";
            Type expected = typeof(int);
            var resolver = new DuckDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForIntInUppercase()
        {
            // Setup
            string name = "INT";
            Type expected = typeof(int);
            var resolver = new DuckDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForSigned()
        {
            // Setup
            string name = "signed";
            Type expected = typeof(int);
            var resolver = new DuckDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForSignedInUppercase()
        {
            // Setup
            string name = "SIGNED";
            Type expected = typeof(int);
            var resolver = new DuckDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForBigint()
        {
            // Setup
            string name = "bigint";
            Type expected = typeof(long);
            var resolver = new DuckDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForBigintInUppercase()
        {
            // Setup
            string name = "BIGINT";
            Type expected = typeof(long);
            var resolver = new DuckDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForInt8()
        {
            // Setup
            string name = "int8";
            Type expected = typeof(long);
            var resolver = new DuckDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForInt8InUppercase()
        {
            // Setup
            string name = "INT8";
            Type expected = typeof(long);
            var resolver = new DuckDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForLong()
        {
            // Setup
            string name = "long";
            Type expected = typeof(long);
            var resolver = new DuckDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForLongInUppercase()
        {
            // Setup
            string name = "LONG";
            Type expected = typeof(long);
            var resolver = new DuckDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForHugeint()
        {
            // Setup
            string name = "hugeint";
            Type expected = typeof(BigInteger);
            var resolver = new DuckDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForHugeintInUppercase()
        {
            // Setup
            string name = "HUGEINT";
            Type expected = typeof(BigInteger);
            var resolver = new DuckDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForUtinyint()
        {
            // Setup
            string name = "utinyint";
            Type expected = typeof(byte);
            var resolver = new DuckDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForUtinyintInUppercase()
        {
            // Setup
            string name = "UTINYINT";
            Type expected = typeof(byte);
            var resolver = new DuckDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForUsmallint()
        {
            // Setup
            string name = "usmallint";
            Type expected = typeof(ushort);
            var resolver = new DuckDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForUsmallintInUppercase()
        {
            // Setup
            string name = "USMALLINT";
            Type expected = typeof(ushort);
            var resolver = new DuckDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForUinteger()
        {
            // Setup
            string name = "uinteger";
            Type expected = typeof(uint);
            var resolver = new DuckDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForUintegerInUppercase()
        {
            // Setup
            string name = "UINTEGER";
            Type expected = typeof(uint);
            var resolver = new DuckDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForUbigint()
        {
            // Setup
            string name = "ubigint";
            Type expected = typeof(ulong);
            var resolver = new DuckDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForUbigintInUppercase()
        {
            // Setup
            string name = "UBIGINT";
            Type expected = typeof(ulong);
            var resolver = new DuckDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForUhugeint()
        {
            // Setup
            string name = "uhugeint";
            Type expected = typeof(BigInteger);
            var resolver = new DuckDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForUhugeintInUppercase()
        {
            // Setup
            string name = "UHUGEINT";
            Type expected = typeof(BigInteger);
            var resolver = new DuckDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForFloat()
        {
            // Setup
            string name = "float";
            Type expected = typeof(float);
            var resolver = new DuckDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForFloatInUppercase()
        {
            // Setup
            string name = "FLOAT";
            Type expected = typeof(float);
            var resolver = new DuckDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForFloat4()
        {
            // Setup
            string name = "float4";
            Type expected = typeof(float);
            var resolver = new DuckDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForFloat4InUppercase()
        {
            // Setup
            string name = "FLOAT4";
            Type expected = typeof(float);
            var resolver = new DuckDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForReal()
        {
            // Setup
            string name = "real";
            Type expected = typeof(float);
            var resolver = new DuckDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForRealInUppercase()
        {
            // Setup
            string name = "REAL";
            Type expected = typeof(float);
            var resolver = new DuckDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForDouble()
        {
            // Setup
            string name = "double";
            Type expected = typeof(double);
            var resolver = new DuckDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForDoubleInUppercase()
        {
            // Setup
            string name = "DOUBLE";
            Type expected = typeof(double);
            var resolver = new DuckDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForFloat8()
        {
            // Setup
            string name = "float8";
            Type expected = typeof(double);
            var resolver = new DuckDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForFloat8InUppercase()
        {
            // Setup
            string name = "FLOAT8";
            Type expected = typeof(double);
            var resolver = new DuckDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForDecimal()
        {
            // Setup
            string name = "decimal";
            Type expected = typeof(decimal);
            var resolver = new DuckDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForDecimalInUppercase()
        {
            // Setup
            string name = "DECIMAL";
            Type expected = typeof(decimal);
            var resolver = new DuckDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForNumeric()
        {
            // Setup
            string name = "numeric";
            Type expected = typeof(decimal);
            var resolver = new DuckDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForNumericInUppercase()
        {
            // Setup
            string name = "NUMERIC";
            Type expected = typeof(decimal);
            var resolver = new DuckDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForVarchar()
        {
            // Setup
            string name = "varchar";
            Type expected = typeof(string);
            var resolver = new DuckDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForVarcharInUppercase()
        {
            // Setup
            string name = "VARCHAR";
            Type expected = typeof(string);
            var resolver = new DuckDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForChar()
        {
            // Setup
            string name = "char";
            Type expected = typeof(string);
            var resolver = new DuckDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForCharInUppercase()
        {
            // Setup
            string name = "CHAR";
            Type expected = typeof(string);
            var resolver = new DuckDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForBpchar()
        {
            // Setup
            string name = "bpchar";
            Type expected = typeof(string);
            var resolver = new DuckDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForBpcharInUppercase()
        {
            // Setup
            string name = "BPCHAR";
            Type expected = typeof(string);
            var resolver = new DuckDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForText()
        {
            // Setup
            string name = "text";
            Type expected = typeof(string);
            var resolver = new DuckDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForTextInUppercase()
        {
            // Setup
            string name = "TEXT";
            Type expected = typeof(string);
            var resolver = new DuckDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForString()
        {
            // Setup
            string name = "string";
            Type expected = typeof(string);
            var resolver = new DuckDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForStringInUppercase()
        {
            // Setup
            string name = "STRING";
            Type expected = typeof(string);
            var resolver = new DuckDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForJson()
        {
            // Setup
            string name = "json";
            Type expected = typeof(string);
            var resolver = new DuckDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForJsonInUppercase()
        {
            // Setup
            string name = "JSON";
            Type expected = typeof(string);
            var resolver = new DuckDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForBlob()
        {
            // Setup
            string name = "blob";
            Type expected = typeof(byte[]);
            var resolver = new DuckDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForBlobInUppercase()
        {
            // Setup
            string name = "BLOB";
            Type expected = typeof(byte[]);
            var resolver = new DuckDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForBytea()
        {
            // Setup
            string name = "bytea";
            Type expected = typeof(byte[]);
            var resolver = new DuckDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForByteaInUppercase()
        {
            // Setup
            string name = "BYTEA";
            Type expected = typeof(byte[]);
            var resolver = new DuckDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForBinary()
        {
            // Setup
            string name = "binary";
            Type expected = typeof(byte[]);
            var resolver = new DuckDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForBinaryInUppercase()
        {
            // Setup
            string name = "BINARY";
            Type expected = typeof(byte[]);
            var resolver = new DuckDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForVarbinary()
        {
            // Setup
            string name = "varbinary";
            Type expected = typeof(byte[]);
            var resolver = new DuckDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForVarbinaryInUppercase()
        {
            // Setup
            string name = "VARBINARY";
            Type expected = typeof(byte[]);
            var resolver = new DuckDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForDate()
        {
            // Setup
            string name = "date";
            Type expected = typeof(DateTime);
            var resolver = new DuckDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForDateInUppercase()
        {
            // Setup
            string name = "DATE";
            Type expected = typeof(DateTime);
            var resolver = new DuckDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForTime()
        {
            // Setup
            string name = "time";
            Type expected = typeof(TimeSpan);
            var resolver = new DuckDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForTimeInUppercase()
        {
            // Setup
            string name = "TIME";
            Type expected = typeof(TimeSpan);
            var resolver = new DuckDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForTimestamp()
        {
            // Setup
            string name = "timestamp";
            Type expected = typeof(DateTime);
            var resolver = new DuckDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForTimestampInUppercase()
        {
            // Setup
            string name = "TIMESTAMP";
            Type expected = typeof(DateTime);
            var resolver = new DuckDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForDatetime()
        {
            // Setup
            string name = "datetime";
            Type expected = typeof(DateTime);
            var resolver = new DuckDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForDatetimeInUppercase()
        {
            // Setup
            string name = "DATETIME";
            Type expected = typeof(DateTime);
            var resolver = new DuckDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForTimestampS()
        {
            // Setup
            string name = "timestamp_s";
            Type expected = typeof(DateTime);
            var resolver = new DuckDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForTimestampSInUppercase()
        {
            // Setup
            string name = "TIMESTAMP_S";
            Type expected = typeof(DateTime);
            var resolver = new DuckDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForTimestampMs()
        {
            // Setup
            string name = "timestamp_ms";
            Type expected = typeof(DateTime);
            var resolver = new DuckDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForTimestampMsInUppercase()
        {
            // Setup
            string name = "TIMESTAMP_MS";
            Type expected = typeof(DateTime);
            var resolver = new DuckDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForTimestampNs()
        {
            // Setup
            string name = "timestamp_ns";
            Type expected = typeof(DateTime);
            var resolver = new DuckDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForTimestampNsInUppercase()
        {
            // Setup
            string name = "TIMESTAMP_NS";
            Type expected = typeof(DateTime);
            var resolver = new DuckDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForTimestampWithTimeZone()
        {
            // Setup
            string name = "timestamp with time zone";
            Type expected = typeof(DateTime);
            var resolver = new DuckDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForTimestampWithTimeZoneInUppercase()
        {
            // Setup
            string name = "TIMESTAMP WITH TIME ZONE";
            Type expected = typeof(DateTime);
            var resolver = new DuckDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForTimestamptz()
        {
            // Setup
            string name = "timestamptz";
            Type expected = typeof(DateTime);
            var resolver = new DuckDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForTimestamptzInUppercase()
        {
            // Setup
            string name = "TIMESTAMPTZ";
            Type expected = typeof(DateTime);
            var resolver = new DuckDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForUuid()
        {
            // Setup
            string name = "uuid";
            Type expected = typeof(Guid);
            var resolver = new DuckDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForUuidInUppercase()
        {
            // Setup
            string name = "UUID";
            Type expected = typeof(Guid);
            var resolver = new DuckDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForBit()
        {
            // Setup
            string name = "bit";
            Type expected = typeof(string);
            var resolver = new DuckDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForBitInUppercase()
        {
            // Setup
            string name = "BIT";
            Type expected = typeof(string);
            var resolver = new DuckDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForBitstring()
        {
            // Setup
            string name = "bitstring";
            Type expected = typeof(string);
            var resolver = new DuckDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForBitstringInUppercase()
        {
            // Setup
            string name = "BITSTRING";
            Type expected = typeof(string);
            var resolver = new DuckDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForInterval()
        {
            // Setup
            string name = "interval";
            Type expected = typeof(TimeSpan);
            var resolver = new DuckDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForIntervalInUppercase()
        {
            // Setup
            string name = "INTERVAL";
            Type expected = typeof(TimeSpan);
            var resolver = new DuckDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForNone()
        {
            // Setup
            string name = "none";
            Type expected = typeof(object);
            var resolver = new DuckDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Resolves aliases independently of letter casing.
        /// </summary>
        [TestMethod]
        public void ResolveSupportedNameReturnsExpectedTypeForNoneInUppercase()
        {
            // Setup
            string name = "NONE";
            Type expected = typeof(object);
            var resolver = new DuckDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(expected,
                actual);
        }

        /// <summary>
        /// Falls back for empty and unknown names.
        /// </summary>
        [TestMethod]
        public void ResolveUnknownNameReturnsObjectForEmptyString()
        {
            // Setup
            string name = "";
            var resolver = new DuckDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(typeof(object),
                actual);
        }

        /// <summary>
        /// Falls back for empty and unknown names.
        /// </summary>
        [TestMethod]
        public void ResolveUnknownNameReturnsObjectForUnsupportedType()
        {
            // Setup
            string name = "__unsupported_type__";
            var resolver = new DuckDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(typeof(object),
                actual);
        }

        /// <summary>
        /// Falls back for empty and unknown names.
        /// </summary>
        [TestMethod]
        public void ResolveUnknownNameReturnsObjectForWhitespace()
        {
            // Setup
            string name = "   ";
            var resolver = new DuckDbTypeNameToClientTypeResolver();

            // Act
            var actual = resolver.Resolve(name);

            // Assert
            Assert.AreEqual(typeof(object),
                actual);
        }

        /// <summary>
        /// Identifies the missing database type name.
        /// </summary>
        [TestMethod]
        public void ResolveNullReportsArgumentName()
        {
            // Setup
            var resolver = new DuckDbTypeNameToClientTypeResolver();

            // Act
            var exception = Assert.ThrowsExactly<ArgumentNullException>(() => resolver.Resolve(null));

            // Assert
            Assert.AreEqual("dbTypeName",
                exception.ParamName,
                StringComparer.Ordinal);
        }

        #endregion
    }
}